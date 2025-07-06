using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
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
using DevExpress.Xpo;
using PracticeMes.Module.BusinessObjects.BaseInfo.CommonInfo;
using PracticeMes.Module.BusinessObjects.BaseInfo.ItemInfo;
using PracticeMes.Module.BusinessObjects.ProductPlanning;

namespace PracticeMes.Win.Controllers.ProductPlanning;
public partial class MasterProductionPlanningController : ViewController
{
    public MasterProductionPlanningController()
    {
        InitializeComponent();
        TargetObjectType = typeof(MasterProductionPlanning);

        SimpleAction masterProductionPlanningClose = new SimpleAction(this, "MasterProductionPlanningClose", PredefinedCategory.View)
        {
            Caption = "생산계획 마감",
            ImageName = "GroupByDate"
        };

        masterProductionPlanningClose.Execute += MasterProductionPlanningClose_Excuete;
    }
    protected override void OnActivated()
    {
        base.OnActivated();
        if (View.Id != "MasterProductionPlanning_DetailView" && View.Id != "MasterProductionPlanning_ListView")
            return;

        ObjectSpace.Committing += ObjectSpace_Committing;
    }

    private void MasterProductionPlanningClose_Excuete(object sender, SimpleActionExecuteEventArgs e)
    {
        try
        {
            MasterProductionPlanning masterProductionPlanningObject = View.CurrentObject as MasterProductionPlanning;

            var masterWorkInstructions = View.ObjectSpace.GetObjects<MasterWorkInstruction>()
                .Where(x => x.MasterProductionPlanningObject.Oid == masterProductionPlanningObject.Oid)
                .ToList();


            // 완료 상태 만들기(작업지시상세 완료 처리하기 위해서 변수생성)
            var universalMajorCode = View.ObjectSpace.GetObjects<UniversalMajorCode>()
                .Where(x => x.MajorCode == "Progress").FirstOrDefault();

            var universalMinorCode = View.ObjectSpace.GetObjects<UniversalMinorCode>()
                .Where(x => x.UniversalMajorCodeObject.Oid == universalMajorCode.Oid && x.MinorCode == "Complete")
                .FirstOrDefault();

            foreach (var masterWorkInstruction in masterWorkInstructions)
            {
                if (masterWorkInstruction.WorkInstructionQuantity == 0)
                {
                    masterWorkInstruction.Delete();
                }
                else
                {
                    masterWorkInstruction.IsComplete = true;
                    masterWorkInstruction.flag = "진행중";

                    var detailWorkInstructions = View.ObjectSpace.GetObjects<DetailWorkInstruction>()
                        .Where(x => x.MasterWorkInstructionObject?.Oid == masterWorkInstruction.Oid).ToList();
                    foreach (var detailWorkInstruction in detailWorkInstructions)
                    {
                        detailWorkInstruction.Progress = universalMinorCode;
                    }
                }
            }
            masterProductionPlanningObject.IsCompleting = true;
            masterProductionPlanningObject.IsComplete = true;

            View.ObjectSpace.CommitChanges();
        }
        catch (UserFriendlyException ex)
        {
            throw new UserFriendlyException(ex);
        }
        catch (Exception ex)
        {
            throw new UserFriendlyException(ex);
        }
        finally
        {
            View.ObjectSpace.ReloadObject(View.CurrentObject);
        }
    }

    private void ObjectSpace_Committing(object sender, CancelEventArgs e)
    {
        try
        {
            var modifiedObjects = View.ObjectSpace.ModifiedObjects;
            IObjectSpace newObjectSpace = Application.CreateObjectSpace(typeof(MasterProductionPlanning));

            var deletedObjects = View.ObjectSpace.ModifiedObjects
                .Cast<object>()
                .Where(obj => View.ObjectSpace.IsDeletedObject(obj))
                .ToList();

            foreach (var modifiedObject in modifiedObjects)
            {
                if (modifiedObject is MasterProductionPlanning productionPlanning)
                {
                    if (View.ObjectSpace.IsNewObject(productionPlanning)) // 신규
                    {
                        // 마감 중이면 검사 생략
                        if (productionPlanning.IsCompleting)
                            continue;

                        var recentBOMObject = View.ObjectSpace.GetObjects<ProductBOM>()
                          .Where(x => x.ItemObject.Oid == productionPlanning?.ItemObject?.Oid)
                          .ToList()
                          .OrderByDescending(x => x.BOMNumber)
                          .FirstOrDefault();

                        if (recentBOMObject == null) return;

                        else
                        {
                            foreach (var recentBom in recentBOMObject.AssemblyBOMObjects.Where(x => x.Parent == null)) {
                                CopyAssemblyBOMRecursive(recentBom, productionPlanning);
                            }
                        }
                    }
                    else if (View.ObjectSpace.IsDeletedObject(productionPlanning)) // 삭제
                    {
                        if (!productionPlanning.IsCompleting)
                            CheckIfObjectIsInUse(newObjectSpace, productionPlanning, "삭제");
                    }
                    else // 수정
                    {
                        if (!productionPlanning.IsCompleting)
                            CheckIfObjectIsInUse(newObjectSpace, productionPlanning, "수정");
                    }
                }
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

    // BOM 생성 재귀함수
    void CopyAssemblyBOMRecursive(AssemblyBOM source, MasterProductionPlanning newProductBOM, DetailProductionPlanning newParent = null)
    {
        var newAssemblyObject = View.ObjectSpace.CreateObject<DetailProductionPlanning>();

        newAssemblyObject.MasterProductionPlanningObject = newProductBOM;
        newAssemblyObject.ItemObject = source.ItemObject;
        newAssemblyObject.BOMQuantity = source.BOMQuantity;
        newAssemblyObject.Parent = newParent;

        foreach (var child in source.Children)
        {
            CopyAssemblyBOMRecursive(child, newProductBOM, newAssemblyObject);
        }
    }

    private static void CheckIfObjectIsInUse(IObjectSpace newObjectSpace, MasterProductionPlanning currentMasterProductionPlanning, string action)
    {
        var referencingList = new List<string>();

        if (newObjectSpace.GetObjects<MasterWorkInstruction>()
            .Any(x => x.MasterProductionPlanningObject.Oid == currentMasterProductionPlanning.Oid))

            referencingList.Add("작업 지시 등록");

        if (referencingList.Any())
        {
            string UsingList = string.Join(", ", referencingList);
            throw new UserFriendlyException(
                     $"이 생산 계획은 다음 메뉴에서 사용 중이므로 {action}할 수 없습니다:\n[{UsingList}]");
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
