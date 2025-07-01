using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using DevExpress.CodeParser;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using PracticeMes.Module.BusinessObjects.BaseInfo.CommonInfo;
using PracticeMes.Module.BusinessObjects.BaseInfo.ItemInfo;
using PracticeMes.Module.BusinessObjects.ProductPlanning;

namespace PracticeMes.Win.Controllers.ProductPlanning;

public partial class MasterWorkInstructionController : ViewController
{
    public MasterWorkInstructionController()
    {
        InitializeComponent();
        TargetObjectType = typeof(MasterWorkInstruction);
    }
    protected override void OnActivated()
    {
        base.OnActivated();

        ObjectSpace.Committing += ObjectSpace_Committing;
    }

    private readonly HashSet<Guid> processedMasterOids = new();

    private void ObjectSpace_Committing(object sender, CancelEventArgs e)
    {
        try
        {
            var modifiedObjects = View.ObjectSpace.ModifiedObjects;
            IObjectSpace newObjectSpace = Application.CreateObjectSpace(typeof(MasterWorkInstruction));

            foreach (var modifiedObject in modifiedObjects)
            {
                if (modifiedObject is MasterWorkInstruction masterInstruction)
                {
                    if (View.ObjectSpace.IsNewObject(masterInstruction)) // 신규
                    {
                        CreateDetailWorkInstructions(View.ObjectSpace, masterInstruction);
                    }
                    else if (View.ObjectSpace.IsDeletedObject(masterInstruction)) // 삭제
                    {
                        //CheckIfObjectIsInUse(newObjectSpace, productionPlanning, "삭제");
                    }
                    else // 수정
                    {
                        //CheckIfObjectIsInUse(newObjectSpace, productionPlanning, "수정");
                    }
                }
                else if (modifiedObject is DetailWorkInstruction detail)
                {
                    var master = detail.MasterWorkInstructionObject;
                    if (master != null && master.Progress?.CodeName == "진행중")
                    {
                        if (processedMasterOids.Contains(master.Oid)) continue;
                        processedMasterOids.Add(master.Oid);

                        CreateWorkInstructionNumber(View.ObjectSpace, master);
                        AddZeroInstruction(View.ObjectSpace, master);
                    }
                }
                else { }
            }
        }
        catch (UserFriendlyException ex)
        {
            throw new UserFriendlyException("생산 계획 삭제 처리 중 오류가 발생했습니다: " + ex.Message);
        }
        catch (Exception ex)
        {
            throw new UserFriendlyException("생산 계획 처리 중 오류가 발생했습니다: (원인 미상 오류) " + ex.Message);
        }
    }

    // 작업 지시 상세 (DetailWorkInstruction) 생성 메서드
    private static void CreateDetailWorkInstructions(IObjectSpace objectSpace, MasterWorkInstruction masterInstruction)
    {
        var MasterPlanning = masterInstruction.MasterProductionPlanningObject;

        if (MasterPlanning?.ItemObject == null || MasterPlanning.ItemObject.ItemGroupObject == null)
            return;

        // 라우팅 테이블 가져옴
        var routingList = objectSpace.GetObjects<DetailItemTypeRouting>()
            .Where(r => r.MasterItemTypeRoutingObject?.ItemType?.Oid == MasterPlanning?.ItemObject?.ItemAccountObject?.Oid
                     && r.MasterItemTypeRoutingObject?.ItemGroupObject?.Oid == MasterPlanning?.ItemObject?.ItemGroupObject?.Oid)
            .OrderBy(r => r.RoutingIndex)
            .ToList();

        var defaultProgress = objectSpace.GetObjects<UniversalMinorCode>()
            .FirstOrDefault(p => p.UniversalMajorCodeObject.MajorCode == "Progress" && p.CodeName == "예정");

        foreach (var route in routingList)
        {
            var detailPlanning = objectSpace.CreateObject<DetailWorkInstruction>();

            detailPlanning.MasterWorkInstructionObject = masterInstruction;
            detailPlanning.WorkProcessObject = route.WorkProcessObject;
            detailPlanning.WorkLineObject = route.WorkLineObject;
            detailPlanning.RoutingIndex = route.RoutingIndex;

            detailPlanning.WareHouseObject = route.WareHouseObject;
            detailPlanning.WorkInstructionQuantity = masterInstruction.WorkInstructionQuantity;
            detailPlanning.WokrStartDateTime = DateTime.Now;
            detailPlanning.IsFinalWorkProcess = route.IsFinalWorkProcess;
            detailPlanning.Progress = defaultProgress;
            detailPlanning.PutWorker = 1;
            detailPlanning.CreatedDateTime = DateTime.Now;
        }
    }

    // 작업 지시 번호 생성
    private static void CreateWorkInstructionNumber(IObjectSpace newObjectSpace, MasterWorkInstruction currentMasterInstruction)
    {
        var details = newObjectSpace.GetObjects<DetailWorkInstruction>()
            .Where(x => x.MasterWorkInstructionObject.Oid == currentMasterInstruction.Oid)
            .OrderBy(x => x.RoutingIndex)
            .ToList();

        var baseNumber = currentMasterInstruction.MasterProductionPlanningObject?.ProductionPlanningNumber?.Replace("-", "") ?? "";
        var maxSeq = details
            .Where(x => !string.IsNullOrEmpty(x.WorkInstructionNumber) && x.WorkInstructionNumber.StartsWith(baseNumber))
            .Select(x => int.Parse(x.WorkInstructionNumber[^4..]))
            .DefaultIfEmpty(0)
            .Max();

        int nextSeq = maxSeq + 1;
        foreach (var item in details)
        {
            item.WorkInstructionNumber = $"{baseNumber}-{nextSeq:0000}";
            nextSeq++;
        }
    }

    // 생산계획 수량을 기준으로 수량 합계가 수주수량 미만일 경우 생산계획 수량이 0인 객체 생성
    private static void AddZeroInstruction(IObjectSpace newObjectSpace, MasterWorkInstruction currentMasterInstruction)
    {
        var planningOid = currentMasterInstruction.MasterProductionPlanningObject?.Oid;
        if (planningOid == null) return;

        bool hasZeroQty = newObjectSpace.GetObjects<MasterWorkInstruction>()
            .Any(x => x.WorkInstructionQuantity == 0 && x.Oid != currentMasterInstruction.Oid);

        var samePlanningInstructions = newObjectSpace.GetObjects<MasterWorkInstruction>()
            .Where(x => x.MasterProductionPlanningObject.Oid == planningOid)
            .ToList();

        var totalQty = samePlanningInstructions.Sum(x => x.WorkInstructionQuantity);

        if (!hasZeroQty && currentMasterInstruction.SalesOrderQuantity > totalQty)
        {
            var zeroInst = newObjectSpace.CreateObject<MasterWorkInstruction>();
            zeroInst.MasterProductionPlanningObject = currentMasterInstruction.MasterProductionPlanningObject;
            zeroInst.WorkInstructionQuantity = 0;
            zeroInst.CreatedDateTime = DateTime.Now;
        }
    }

    protected override void OnViewControlsCreated()
    {
        base.OnViewControlsCreated();
    }
    protected override void OnDeactivated()
    {
        base.OnDeactivated();
    }
}
