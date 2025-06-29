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
using PracticeMes.Module.BusinessObjects.Purchase;
using PracticeMes.Module.BusinessObjects.Sales;

namespace PracticeMes.Win.Controllers.BaseInfo.CommonInfo;

public partial class BusinessPartnerController : ViewController
{
    public BusinessPartnerController()
    {
        InitializeComponent();
        TargetObjectType = typeof(BusinessPartner);
    }
    protected override void OnActivated()
    {
        base.OnActivated();
        // Lookup으로 참조하는 화면에서도 자동으로 Controller가 활성화되어 ObjectSpace_Committing를 호출하게 됨.
        // 이때 modifiedObjects는 NULL이므로 오류 발생하게 됨.
        // 따라서 View.Id 기준으로 명시
        if (View.Id != "BusinessPartner_DetailView" && View.Id != "BusinessPartner_ListView")
            return;

        ObjectSpace.Committing += ObjectSpace_Committing;
    }

    private void ObjectSpace_Committing(object sender, CancelEventArgs e)
    {
        try
        {
            var modifiedObjects = View.ObjectSpace.ModifiedObjects;
            IObjectSpace newObjectSpace = Application.CreateObjectSpace(typeof(BusinessPartner));

            if (modifiedObjects == null)
            {
                throw new UserFriendlyException("삭제 또는 수정된 회사 정보가 없습니다.");
            }

            foreach (var modifiedObject in modifiedObjects)
            {
                if (modifiedObject is BusinessPartner businessPartner)
                {
                    if (View.ObjectSpace.IsNewObject(businessPartner)) // 신규
                    {

                    }
                    else if (View.ObjectSpace.IsDeletedObject(businessPartner)) // 삭제
                    {
                        CheckIfObjectIsInUse(newObjectSpace, businessPartner, "삭제");
                    }
                    else // 수정
                    {
                        CheckIfObjectIsInUse(newObjectSpace, businessPartner, "수정");
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
            throw new UserFriendlyException("거래처 삭제 처리 중 오류가 발생했습니다: (원인 미상 오류)" + ex.Message);
        }
    }
    private static void CheckIfObjectIsInUse(IObjectSpace newObjectSpace, BusinessPartner currentBusinessPartner, string action)
    {
        var referencingList = new List<string>();

        if (newObjectSpace.GetObjects<MasterSalesOrder>().Any(x => x.BusinessPartnerObject.Oid == currentBusinessPartner.Oid))
            referencingList.Add("수주 등록");

        if (newObjectSpace.GetObjects<MasterPurchaseOrder>().Any(x => x.BusinessPartnerObject.Oid == currentBusinessPartner.Oid))
            referencingList.Add("구매 발주 등록");

        if (newObjectSpace.GetObjects<MasterSalesShipment>().Any(x => x.BusinessPartnerObject.Oid == currentBusinessPartner.Oid))
            referencingList.Add("출하 등록");

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
