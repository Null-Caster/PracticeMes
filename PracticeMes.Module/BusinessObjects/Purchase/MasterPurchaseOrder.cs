using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using PracticeMes.Module.BusinessObjects.BaseInfo.CommonInfo;

namespace PracticeMes.Module.BusinessObjects.Purchase;

[DefaultClassOptions]
[NavigationItem("구매 관리"), XafDisplayName("구매발주 등록")]
[DefaultListViewOptions(MasterDetailMode.ListViewOnly, true, NewItemRowPosition.Top)]
public class MasterPurchaseOrder : BaseObject
{
    #region Properties
    [VisibleInLookupListView(true)]
    [ModelDefault("AllowEdit", "False")]
    [RuleUniqueValue(CustomMessageTemplate = "구매발주번호가 중복되었습니다.")]
    [XafDisplayName("구매발주번호"), ToolTip("구매발주번호")]
    public string PurchaseOrderNumber
    {
        get { return GetPropertyValue<string>(nameof(PurchaseOrderNumber)); }
        set { SetPropertyValue(nameof(PurchaseOrderNumber), value); }
    }

    [VisibleInLookupListView(true)]
    [ModelDefault("DisplayFormat", "yyyy/MM/dd")]
    [XafDisplayName("발주일자"), ToolTip("발주일자")]
    public DateTime PurchaseOrderDateTime
    {
        get { return GetPropertyValue<DateTime>(nameof(PurchaseOrderDateTime)); }
        set { SetPropertyValue(nameof(PurchaseOrderDateTime), value); }
    }

    [VisibleInLookupListView(true)]
    [DataSourceCriteria("IsEnabled == True")]
    [ModelDefault("LookupProperty", nameof(BusinessPartner.BusinessPartnerName))]
    [LookupEditorMode(LookupEditorMode.AllItemsWithSearch)]
    [RuleRequiredField(CustomMessageTemplate = "거래처를 선택하세요.")]
    [XafDisplayName("거래처정보"), ToolTip("거래처정보")]
    public BusinessPartner BusinessPartnerObject
    {
        get { return GetPropertyValue<BusinessPartner>(nameof(BusinessPartnerObject)); }
        set { SetPropertyValue(nameof(BusinessPartnerObject), value); }
    }

    [VisibleInLookupListView(true)]
    [DataSourceCriteria("IsEnabled == True")]
    [ModelDefault("LookupProperty", nameof(Employee.EmployeeName))]
    [LookupEditorMode(LookupEditorMode.AllItemsWithSearch)]
    [XafDisplayName("담당자"), ToolTip("담당자")]
    public Employee EmployeeObject
    {
        get { return GetPropertyValue<Employee>(nameof(EmployeeObject)); }
        set { SetPropertyValue(nameof(EmployeeObject), value); }
    }

    [VisibleInLookupListView(true)]
    [DataSourceCriteria("IsEnabled == True")]
    [ModelDefault("LookupProperty", nameof(WareHouse.WareHouseName))]
    [LookupEditorMode(LookupEditorMode.AllItemsWithSearch)]
    [RuleRequiredField(CustomMessageTemplate = "입고창고를 입력하세요.")]
    [XafDisplayName("입고창고"), ToolTip("입고창고")]
    public WareHouse WareHouseObject
    {
        get { return GetPropertyValue<WareHouse>(nameof(WareHouseObject)); }
        set { SetPropertyValue(nameof(WareHouseObject), value); }
    }

    [VisibleInLookupListView(true)]
    [ModelDefault("DisplayFormat", "yyyy/MM/dd")]
    [RuleRequiredField(CustomMessageTemplate = "납기예정일을 입력하세요.")]
    [XafDisplayName("납기예정일"), ToolTip("납기예정일")]
    public DateTime DeliveryDateTime
    {
        get { return GetPropertyValue<DateTime>(nameof(DeliveryDateTime)); }
        set { SetPropertyValue(nameof(DeliveryDateTime), value); }
    }

    [VisibleInLookupListView(true)]
    [ModelDefault("AllowEdit", "False")]
    [ModelDefault("EditMask", "yyyy/MM/dd HH:mm:ss")]
    [ModelDefault("DisplayFormat", "yyyy/MM/dd HH:mm:ss")]
    [XafDisplayName("생성일시"), ToolTip("항목이 생성된 일시입니다.")]
    public DateTime CreatedDateTime
    {
        get { return GetPropertyValue<DateTime>(nameof(CreatedDateTime)); }
        set { SetPropertyValue(nameof(CreatedDateTime), value); }
    }

    [VisibleInLookupListView(true)]
    [XafDisplayName("비고"), ToolTip("비고")]
    public string Remark
    {
        get { return GetPropertyValue<string>(nameof(Remark)); }
        set { SetPropertyValue(nameof(Remark), value); }
    }

    [XafDisplayName("구매발주 등록 상세")]
    [Association(@"DetailPurcjaseOrderRefernecesMasterPurchaseOrder"), DevExpress.Xpo.Aggregated]
    public XPCollection<DetailPurchaseOrder> DetailPurchaseOrderobjects { get { return GetCollection<DetailPurchaseOrder>(nameof(DetailPurchaseOrderobjects)); } }
    #endregion

    #region Constructors
    public MasterPurchaseOrder(Session session) : base(session) { }
    #endregion

    #region Methods
    public override void AfterConstruction()
    {
        base.AfterConstruction();
    }
    #endregion
}