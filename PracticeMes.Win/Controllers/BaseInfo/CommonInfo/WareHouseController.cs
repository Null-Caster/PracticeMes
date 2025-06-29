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
using PracticeMes.Module.BusinessObjects.BaseInfo.ProductionInfo;
using PracticeMes.Module.BusinessObjects.Lot;
using PracticeMes.Module.BusinessObjects.Purchase;
using PracticeMes.Module.BusinessObjects.Sales;

namespace PracticeMes.Win.Controllers.BaseInfo.CommonInfo;

public partial class WareHouseController : ViewController
{
    public WareHouseController()
    {
        InitializeComponent();
        TargetObjectType = typeof(WareHouse);
    }
    protected override void OnActivated()
    {
        base.OnActivated();
        if (View.Id != "WareHouse_DetailView" && View.Id != "WareHouse_ListView")
            return;

        ObjectSpace.Committing += ObjectSpace_Committing;
    }
    private void ObjectSpace_Committing(object sender, CancelEventArgs e)
    {
        try
        {
            var modifiedObjects = View.ObjectSpace.ModifiedObjects;
            IObjectSpace newObjectSpace = Application.CreateObjectSpace(typeof(WareHouse));

            foreach (var modifiedObject in modifiedObjects)
            {
                if (modifiedObject is WareHouse wareHouse)
                {
                    if (View.ObjectSpace.IsNewObject(wareHouse)) // 신규
                    {

                    }
                    else if (View.ObjectSpace.IsDeletedObject(wareHouse)) // 삭제
                    {
                        CheckIfObjectIsInUse(newObjectSpace, wareHouse, "삭제");
                    }
                    else // 수정
                    {
                        CheckIfObjectIsInUse(newObjectSpace, wareHouse, "수정");
                    }
                }
            }
        }
        catch (UserFriendlyException ex)
        {
            throw new UserFriendlyException(ex.Message);
        }
        catch (Exception ex)
        {
            throw new UserFriendlyException("창고 삭제 처리 중 오류가 발생했습니다: (원인 미상 오류) " + ex.Message);
        }
    }
    private static void CheckIfObjectIsInUse(IObjectSpace newObjectSpace, WareHouse currentWareHouse, string action)
    {
        var referencingList = new List<string>();

        if (newObjectSpace.GetObjects<MasterSalesOrder>().Any(x => x.WareHouseObject.Oid == currentWareHouse.Oid))
            referencingList.Add("구매 발주 등록");

        if (newObjectSpace.GetObjects<MasterSalesShipment>().Any(x => x.WareHouseObject.Oid == currentWareHouse.Oid))
            referencingList.Add("출하 등록");

        if (newObjectSpace.GetObjects<MasterPurchaseOrder>().Any(x => x.WareHouseObject.Oid == currentWareHouse.Oid))
            referencingList.Add("구매 발주 등록");

        if (newObjectSpace.GetObjects<Lot>().Any(x => x.WareHouseObject.Oid == currentWareHouse.Oid))
            referencingList.Add("Lot 관리");

        if (referencingList.Any())
        {
            string UsingList = string.Join(", ", referencingList);
            throw new UserFriendlyException(
                     $"이 창고는 다음 메뉴에서 사용 중이므로 {action}할 수 없습니다:\n[{UsingList}]");
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
