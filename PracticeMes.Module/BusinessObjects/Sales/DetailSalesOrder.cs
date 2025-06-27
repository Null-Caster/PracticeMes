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
using PracticeMes.Module.BusinessObjects.BaseInfo.CommonInfol;
using PracticeMes.Module.BusinessObjects.BaseInfo.ItemInfo;

namespace PracticeMes.Module.BusinessObjects.Sales;

[DefaultClassOptions]
[NavigationItem(false)]
[XafDisplayName("수주 등록 상세")]
[DefaultListViewOptions(MasterDetailMode.ListViewOnly, false, NewItemRowPosition.Bottom)]
public class DetailSalesOrder : BaseObject
{
    #region Properties
    [ImmediatePostData(true)]
    [VisibleInLookupListView(true)]
    [DataSourceCriteria("IsEnabled == True AND ItemAccountObject.ItemAccountName != '원자재'")]
    [ModelDefault("LookupProperty", nameof(Item.ItemCode))]
    [LookupEditorMode(LookupEditorMode.AllItemsWithSearch)]
    [RuleRequiredField(CustomMessageTemplate = "품목 코드를 입력하세요.")]
    [XafDisplayName("품목 코드"), ToolTip("품목 코드")]
    public Item ItemObject
    {
        get { return GetPropertyValue<Item>(nameof(ItemObject)); }
        set { SetPropertyValue(nameof(ItemObject), value); }
    }

    [VisibleInLookupListView(true)]
    [VisibleInListView(false)]
    [VisibleInDetailView(false)]
    [XafDisplayName("수주 번호"), ToolTip("수주 번호")]
    public string SalesOrderNumber
    {
        get { return this.MasterSalesOrderObject?.SalesOrderNumber; }
    }

    [VisibleInLookupListView(true)]
    [XafDisplayName("품목 이름"), ToolTip("품목 이름")]
    public string ItemName
    {
        get { return ItemObject?.ItemName; }
    }

    [VisibleInLookupListView(true)]
    [RuleValueComparison(ValueComparisonType.GreaterThanOrEqual, 0, CustomMessageTemplate = "수주 수량은 0 이상이어야 합니다.")]
    [XafDisplayName("수주 수량"), ToolTip("수주 수량")]
    public double SalesOrderQuantity
    {
        get { return GetPropertyValue<double>(nameof(SalesOrderQuantity)); }
        set { SetPropertyValue(nameof(SalesOrderQuantity), value); }
    }

    [VisibleInLookupListView(true)]
    [RuleValueComparison(ValueComparisonType.GreaterThanOrEqual, 0, CustomMessageTemplate = "계획수량은 0 이상이어야 합니다.")]
    [XafDisplayName("계획수량"), ToolTip("계획수량")]
    public double ProductPlanningQuantity
    {
        get { return GetPropertyValue<double>(nameof(ProductPlanningQuantity)); }
        set { SetPropertyValue(nameof(ProductPlanningQuantity), value); }
    }

    [VisibleInLookupListView(true)]
    [ModelDefault("LookupProperty", nameof(Unit.UnitName))]
    [LookupEditorMode(LookupEditorMode.AllItemsWithSearch)]
    [XafDisplayName("수주단위"), ToolTip("수주단위")]
    public Unit SalesOrderUnit
    {
        get { return GetPropertyValue<Unit>(nameof(SalesOrderUnit)); }
        set { SetPropertyValue(nameof(SalesOrderUnit), value); }
    }

    [VisibleInLookupListView(true)]
    [DataSourceCriteria("UniversalMajorCodeObject.MajorCode == 'SalesOrderType' AND IsEnabled == True")]
    [ModelDefault("LookupProperty", nameof(UniversalMinorCode.CodeName))]
    [LookupEditorMode(LookupEditorMode.AllItemsWithSearch)]
    [RuleRequiredField(CustomMessageTemplate = "수주유형을 입력하세요.")]
    [XafDisplayName("수주유형"), ToolTip("수주유형")]
    public UniversalMinorCode SalesOrderType
    {
        get { return GetPropertyValue<UniversalMinorCode>(nameof(SalesOrderType)); }
        set { SetPropertyValue(nameof(SalesOrderType), value); }
    }

    [VisibleInLookupListView(true)]
    [ModelDefault("EditMask", "###,###,###,###,###,###,###,###,###,##0.##")]
    [XafDisplayName("단가"), ToolTip("단가")]
    public double UnitPrice
    {
        get { return GetPropertyValue<double>(nameof(UnitPrice)); }
        set { SetPropertyValue(nameof(UnitPrice), value); }
    }

    [VisibleInLookupListView(true)]
    [ModelDefault("EditMask", "###,###,###,###,###,###,###,###,###,##0.##")]
    [XafDisplayName("수주 금액"), ToolTip("수주 금액")]
    public double SalesOrderPrice
    {
        get { return GetPropertyValue<double>(nameof(SalesOrderPrice)); }
        set { SetPropertyValue(nameof(SalesOrderPrice), value); }
    }

    [VisibleInLookupListView(true)]
    [ModelDefault("EditMask", "###,###,###,###,###,###,###,###,###,##0.###")]
    [XafDisplayName("부가세"), ToolTip("부가세")]
    public double VATPrice
    {
        get { return GetPropertyValue<double>(nameof(VATPrice)); }
        set { SetPropertyValue(nameof(VATPrice), value); }
    }

    [VisibleInLookupListView(true)]
    [RuleRequiredField(CustomMessageTemplate = "납기예정일을 입력하세요.")]
    [XafDisplayName("납기예정일"), ToolTip("납기예정일")]
    public DateTime DeliveryDateTime
    {
        get { return GetPropertyValue<DateTime>(nameof(DeliveryDateTime)); }
        set { SetPropertyValue(nameof(DeliveryDateTime), value); }
    }

    [VisibleInLookupListView(true)]
    [XafDisplayName("비고"), ToolTip("비고")]
    public string Remark
    {
        get { return GetPropertyValue<string>(nameof(Remark)); }
        set { SetPropertyValue(nameof(Remark), value); }
    }

    [VisibleInLookupListView(true)]
    [ModelDefault("AllowEdit", "False")]
    [ModelDefault("EditMask", "yyyy/MM/dd HH:mm:ss")]
    [ModelDefault("DisplayFormat", "yyyy/MM/dd HH:mm:ss")]
    [XafDisplayName("생성 일시"), ToolTip("항목이 생성된 일시입니다.")]
    public DateTime CreatedDateTime
    {
        get { return GetPropertyValue<DateTime>(nameof(CreatedDateTime)); }
        set { SetPropertyValue(nameof(CreatedDateTime), value); }
    }

    [Association(@"DetailSalesOrderReferencesMasterSalesOrder")]
    public MasterSalesOrder MasterSalesOrderObject
    {
        get { return GetPropertyValue<MasterSalesOrder>(nameof(MasterSalesOrderObject)); }
        set { SetPropertyValue(nameof(MasterSalesOrderObject), value); }
    }
    #endregion

    #region Constructors
    public DetailSalesOrder(Session session) : base(session) {  }
    #endregion

    #region Methods
    public override void AfterConstruction()
    {
        base.AfterConstruction();

        CreatedDateTime = DateTime.Now;
    }

    protected override void OnChanged(string propertyName, object oldValue, object newValue)
    {
        base.OnChanged(propertyName, oldValue, newValue);
        if (Session.IsObjectsLoading)
        {
            return;
        }

        // 수주 금액 계산
        var salesOrderPrice = UnitPrice * SalesOrderQuantity;
        // 부가세 계산
        var vatPrice = SalesOrderPrice * 0.1;

        switch (propertyName)
        {
            case nameof(SalesOrderQuantity):
                SalesOrderPrice = salesOrderPrice;
                VATPrice = vatPrice;
                break;
            case nameof(UnitPrice):
                SalesOrderPrice = salesOrderPrice;
                VATPrice = vatPrice;
                break;
            case nameof(ItemObject):
                if (ItemObject != null)
                {
                    UnitPrice = ItemObject.UnitPrice; // 품목 단가 가져오기
                }
                break;
            default:
                break;
        }
    }
    #endregion
}