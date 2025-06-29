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
    }
    protected override void OnActivated()
    {
        base.OnActivated();
        if (View.Id != "MasterProductionPlanning_DetailView" && View.Id != "MasterProductionPlanning_ListView")
            return;

        ObjectSpace.Committing += ObjectSpace_Committing;
    }

    private void ObjectSpace_Committing(object sender, CancelEventArgs e)
    {
        try
        {
            var modifiedObjects = View.ObjectSpace.ModifiedObjects;
            IObjectSpace newObjectSpace = Application.CreateObjectSpace(typeof(MasterProductionPlanning));

            foreach (var modifiedObject in modifiedObjects)
            {
                if (modifiedObject is MasterProductionPlanning productionPlanning)
                {
                    if (View.ObjectSpace.IsNewObject(productionPlanning)) // 신규
                    {

                    }
                    else if (View.ObjectSpace.IsDeletedObject(productionPlanning)) // 삭제
                    {
                        CheckIfObjectIsInUse(newObjectSpace, productionPlanning, "삭제");
                    }
                    else // 수정
                    {
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

    private static void CheckIfObjectIsInUse(IObjectSpace newObjectSpace, MasterProductionPlanning currentMasterProductionPlanning, string action)
    {
        var referencingList = new List<string>();

        if (newObjectSpace.GetObjects<MasterWorkInstruction>().Any(x => x.MasterProductionPlanningObject.Oid == currentMasterProductionPlanning.Oid))
            referencingList.Add("작업 지시 등록");

        if (referencingList.Any())
        {
            string UsingList = string.Join(", ", referencingList);
            throw new UserFriendlyException(
                     $"이 회사는 다음 메뉴에서 사용 중이므로 {action}할 수 없습니다:\n[{UsingList}]");
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
