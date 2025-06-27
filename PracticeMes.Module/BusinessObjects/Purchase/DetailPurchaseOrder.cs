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
using PracticeMes.Module.BusinessObjects.BaseInfo.CommonInfol;
using PracticeMes.Module.BusinessObjects.BaseInfo.ItemInfo;

namespace PracticeMes.Module.BusinessObjects.Purchase;

[DefaultClassOptions]
[NavigationItem(false)]
[DefaultListViewOptions(MasterDetailMode.ListViewOnly, true, NewItemRowPosition.Bottom)]
public class DetailPurchaseOrder : BaseObject
{
    #region Properties
    [VisibleInLookupListView(true)]
    [ImmediatePostData(true)]
    [ModelDefault("LookupProperty", nameof(Item.ItemCode))]
    [DataSourceCriteria("ItemAccountObject.ItemAccountName == '원자재'")]
    [LookupEditorMode(LookupEditorMode.AllItemsWithSearch)]
    [RuleRequiredField(CustomMessageTemplate = "품목 코드를 입력하세요.")]
    [XafDisplayName("품목 코드"), ToolTip("품목 코드")]
    public Item ItemObject
    {
        get { return GetPropertyValue<Item>(nameof(ItemObject)); }
        set { SetPropertyValue(nameof(ItemObject), value); }
    }

    [VisibleInLookupListView(true)]
    [XafDisplayName("품목 명칭"), ToolTip("품목 명칭")]
    public string ItemName
    {
        get { return ItemObject?.ItemName; }
    }

    [VisibleInLookupListView(true)]
    [DataSourceCriteria("IsEnabled == True")]
    [ModelDefault("LookupProperty", nameof(Unit.UnitName))]
    [LookupEditorMode(LookupEditorMode.AllItemsWithSearch)]
    [RuleRequiredField(CustomMessageTemplate = "단위를 입력하세요.")]
    [XafDisplayName("단위"), ToolTip("단위")]
    public Unit UnitObject
    {
        get { return GetPropertyValue<Unit>(nameof(UnitObject)); }
        set { SetPropertyValue(nameof(UnitObject), value); }
    }

    [VisibleInLookupListView(true)]
    [RuleValueComparison(ValueComparisonType.GreaterThanOrEqual, 0, CustomMessageTemplate = "구매수량은 0 이상이어야 합니다.")]
    [XafDisplayName("발주수량"), ToolTip("발주수량")]
    public double PurchaseOrderQuantity
    {
        get { return GetPropertyValue<double>(nameof(PurchaseOrderQuantity)); }
        set { SetPropertyValue(nameof(PurchaseOrderQuantity), value); }
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
    [XafDisplayName("발주금액"), ToolTip("발주금액")]
    public double PurchaseOrderPrice
    {
        get { return GetPropertyValue<double>(nameof(PurchaseOrderPrice)); }
        set { SetPropertyValue(nameof(PurchaseOrderPrice), value); }
    }

    [VisibleInLookupListView(true)]
    [ModelDefault("EditMask", "###,###,###,###,###,###,###,###,###,##0.###")]
    [XafDisplayName("부가세"), ToolTip("부가세")]
    public double VAT
    {
        get { return GetPropertyValue<double>(nameof(VAT)); }
        set { SetPropertyValue(nameof(VAT), value); }
    }

    //[VisibleInLookupListView(true)]
    //[XafDisplayName("입고 수량"), ToolTip("입고 수량")]
    //public double PurchaseInputQuantity
    //{
    //    get
    //    {
    //        return new XPCollection<DetailPurchaseInput>(this.Session)
    //            .Where(x => x.MasterPurchaseInputObject?.MasterPurchaseOrderObject?.Oid == this.MasterPurchaseOrderObject?.Oid
    //                   && x.ItemObject?.Oid == this.ItemObject?.Oid)
    //            .Sum(s => s.PurchaseInputQuantity);
    //    }
    //}

    [VisibleInLookupListView(true)]
    [ModelDefault("AllowEdit", "False")]
    [ModelDefault("DisplayFormat", "yyyy/MM/dd HH:mm:ss")]
    [ModelDefault("EditMask", "yyyy/MM/dd HH:mm:ss")]
    [XafDisplayName("생성 일시"), ToolTip("항목이 생성된 일시입니다.")]
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

    [Association(@"DetailPurcjaseOrderRefernecesMasterPurchaseOrder")]
    public MasterPurchaseOrder MasterPurchaseOrderObject
    {
        get { return GetPropertyValue<MasterPurchaseOrder>(nameof(MasterPurchaseOrderObject)); }
        set { SetPropertyValue(nameof(MasterPurchaseOrderObject), value); }
    }
    #endregion

    #region Constructors
    public DetailPurchaseOrder(Session session) : base(session) { }
    #endregion

    #region Methods
    public override void AfterConstruction()
    {
        base.AfterConstruction();
    }
    #endregion

}