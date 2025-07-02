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
using PracticeMes.Module.BusinessObjects.LotManagement;

namespace PracticeMes.Module.BusinessObjects.Purchase;

[DefaultClassOptions]
[NavigationItem(false)]
[DefaultListViewOptions(MasterDetailMode.ListViewOnly, true, NewItemRowPosition.None)]
public class DetailPurchaseInput : BaseObject
{
    #region Properties
    [VisibleInLookupListView(true)]
    [ModelDefault("AllowEdit", "False")]
    [ModelDefault("LookupProperty", nameof(Lot.LotNumber))]
    [ModelDefault("LookupEditorMode", "AllItems")]
    [ModelDefault("LookupEditor.CreateNewItem", "False")]
    [RuleUniqueValue(CustomMessageTemplate = "Lot 번호가 중복되었습니다.")]
    [XafDisplayName("Lot 번호"), ToolTip("Lot 번호")]
    public Lot LotObject
    {
        get { return GetPropertyValue<Lot>(nameof(LotObject)); }
        set { SetPropertyValue(nameof(LotObject), value); }
    }

    [VisibleInLookupListView(true)]
    [ModelDefault("DisplayFormat", "yyyy/MM/dd")]
    [XafDisplayName("입고일자"), ToolTip("입고일자")]
    public DateTime InputDateTime
    {
        get { return GetPropertyValue<DateTime>(nameof(InputDateTime)); }
        set { SetPropertyValue(nameof(InputDateTime), value); }
    }

    [VisibleInLookupListView(true)]
    [ImmediatePostData(true)]
    [DataSourceProperty(nameof(ItemObjectDataSource))]
    [ModelDefault("LookupProperty", nameof(Item.ItemCode))]
    [LookupEditorMode(LookupEditorMode.AllItemsWithSearch)]
    [RuleRequiredField(CustomMessageTemplate = "품목코드을 입력하세요.")]
    [XafDisplayName("품목코드"), ToolTip("품목코드")]
    public Item ItemObject
    {
        get { return GetPropertyValue<Item>(nameof(ItemObject)); }
        set { SetPropertyValue(nameof(ItemObject), value); }
    }

    [Browsable(false)]
    public string ItemCode
    {
        get { return this.ItemObject?.ItemCode; }
    }

    [Browsable(false)]
    public XPCollection<Item> ItemObjectDataSource
    {
        get
        {
            var masterInputOid = MasterPurchaseInputObject?.MasterPurchaseOrderObject?.Oid;
            // DetailPurchaseOrder 에서 Oid 리스트 뽑고
            var itemOids = new XPCollection<DetailPurchaseOrder>(Session)
                .Where(d => d.MasterPurchaseOrderObject.Oid == masterInputOid)
                .Select(d => d.ItemObject.Oid)
                .Distinct()
                .ToList();

            if (masterInputOid != null)
            {
                return new XPCollection<Item>(
                    Session,
                    new InOperator("Oid", itemOids)
                );
            }
            return null;
        }
    }

    [VisibleInLookupListView(true)]
    [XafDisplayName("품목명칭"), ToolTip("품목명칭")]
    public string ItemName
    {
        get { return ItemObject?.ItemName; }
    }

    [VisibleInLookupListView(true)]
    [DataSourceCriteria("IsEnabled == True")]
    [ModelDefault("LookupProperty", nameof(Unit.UnitName))]
    [LookupEditorMode(LookupEditorMode.AllItemsWithSearch)]
    [RuleRequiredField(CustomMessageTemplate = "입고단위 입력하세요.")]
    [XafDisplayName("입고단위"), ToolTip("입고단위")]
    public Unit UnitObject
    {
        get { return GetPropertyValue<Unit>(nameof(UnitObject)); }
        set { SetPropertyValue(nameof(UnitObject), value); }
    }

    [DataSourceCriteria("UniversalMajorCodeObject.MajorCode == 'InspectionExecuteType' AND IsEnabled == True")]
    [ModelDefault("LookupProperty", nameof(UniversalMinorCode.CodeName))]
    [LookupEditorMode(LookupEditorMode.AllItemsWithSearch)]
    [XafDisplayName("검사요청"), ToolTip("검사요청")]
    public UniversalMinorCode InspectionExecuteType
    {
        get { return GetPropertyValue<UniversalMinorCode>(nameof(InspectionExecuteType)); }
        set { SetPropertyValue(nameof(InspectionExecuteType), value); }
    }

    [VisibleInLookupListView(true)]
    [ImmediatePostData(true)]
    [ModelDefault("EditMask", "###,###,###,###,###,###,###,###,###,##0.##")]
    [XafDisplayName("단가"), ToolTip("단가")]
    public double UnitPrice
    {
        get { return GetPropertyValue<double>(nameof(UnitPrice)); }
        set { SetPropertyValue(nameof(UnitPrice), value); }
    }

    [VisibleInLookupListView(true)]
    [XafDisplayName("총 입고수량"), ToolTip("총 입고수량")]
    public double TotalPurchaseInputQuantity
    {
        get
        {
            // 발주번호가 동일한 품목 입고자료 합계
            var detailPurchaseInputQuantity = new XPCollection<DetailPurchaseInput>(this.Session)
                .Where(x => x.MasterPurchaseInputObject?.MasterPurchaseOrderObject?.Oid == this.MasterPurchaseInputObject?.MasterPurchaseOrderObject?.Oid && 
                            x.ItemObject?.Oid == this.ItemObject?.Oid)
                .Sum(x => x.PurchaseInputQuantity);

            return detailPurchaseInputQuantity;
        }
    }

    [VisibleInLookupListView(true)]
    [XafDisplayName("입고 잔량"), ToolTip("입고 잔량")]
    public double AvailPurchaseInputQuantity
    {
        get
        {
            // 발주번호 디테일의 품목 발주수량 합계
            var detailPurchaseOrderQuantity = new XPCollection<DetailPurchaseOrder>(this.Session)
                .Where(x => x.MasterPurchaseOrderObject?.Oid == this.MasterPurchaseInputObject?.MasterPurchaseOrderObject?.Oid && 
                            x.ItemObject?.Oid == this.ItemObject?.Oid)
                .Sum(x => x.PurchaseOrderQuantity);

            // 발주번호가 동일한 품목 입고자료 합계
            var detailPurchaseInputQuantity = new XPCollection<DetailPurchaseInput>(this.Session)
                .Where(x => x.MasterPurchaseInputObject?.MasterPurchaseOrderObject?.Oid == this.MasterPurchaseInputObject?.MasterPurchaseOrderObject?.Oid && 
                            x.ItemObject?.Oid == this.ItemObject?.Oid)
                .Sum(x => x.PurchaseInputQuantity);

            return detailPurchaseOrderQuantity - detailPurchaseInputQuantity;
        }
    }

    [VisibleInLookupListView(true)]
    [ImmediatePostData(true)]
    [ModelDefault("EditMask", "###,###,###,###,###,###,###,###,###,##0.###")]
    [RuleValueComparison(ValueComparisonType.GreaterThanOrEqual, 0, CustomMessageTemplate = "구매입고 수량은 0 이상이어야 합니다.")]
    [XafDisplayName("구매입고 수량"), ToolTip("구매입고 수량")]
    public double PurchaseInputQuantity
    {
        get { return GetPropertyValue<double>(nameof(PurchaseInputQuantity)); }
        set { SetPropertyValue(nameof(PurchaseInputQuantity), value); }
    }

    [VisibleInLookupListView(true)]
    [ModelDefault("EditMask", "###,###,###,###,###,###,###,###,###,##0.##")]
    [XafDisplayName("구매입고 금액"), ToolTip("구매입고 금액")]
    public double PurchaseInputPrice
    {
        get { return GetPropertyValue<double>(nameof(PurchaseInputPrice)); }
        set { SetPropertyValue(nameof(PurchaseInputPrice), value); }
    }

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

    [Association(@"DetailPurchaseInputReferenceMasterPurchaseInput")]
    public MasterPurchaseInput MasterPurchaseInputObject
    {
        get { return GetPropertyValue<MasterPurchaseInput>(nameof(MasterPurchaseInputObject)); }
        set { SetPropertyValue(nameof(MasterPurchaseInputObject), value); }
    }
    #endregion

    #region Constructors
    public DetailPurchaseInput(Session session) : base(session) {}
    #endregion

    #region Methods
    public override void AfterConstruction()
    {
        base.AfterConstruction();

        InitialValueSetting();
    }

    protected override void OnChanged(string propertyName, object oldValue, object newValue)
    {
        base.OnChanged(propertyName, oldValue, newValue);
        if (this.Session.IsObjectsLoading)
        {
            return;
        }

        switch (propertyName)
        {
            case nameof(this.ItemObject):
                UpdateValuesFromPurchaseOrder();
                this.PurchaseInputQuantity = this.AvailPurchaseInputQuantity;
                break;

            case nameof(this.MasterPurchaseInputObject):
                UpdateValuesFromPurchaseOrder();
                break;

            case nameof(this.UnitPrice) or nameof(this.PurchaseInputQuantity):
                if (ItemObject is null || ItemObject.Oid == Guid.Empty) return;
                this.PurchaseInputPrice = this.UnitPrice * this.PurchaseInputQuantity;
                break;

            default:
                break;
        }
    }

    private void InitialValueSetting()
    {
        // 검사유형 기본 세팅
        if (InspectionExecuteType == null)
        {
            var firstInspectionExecuteType = Session.Query<UniversalMinorCode>()
                                      .Where(x => x.IsEnabled && 
                                                  x.UniversalMajorCodeObject.MajorCode == "InspectionExecuteType")
                                      .FirstOrDefault();

            InspectionExecuteType = firstInspectionExecuteType;
        }

        InputDateTime = DateTime.Now;
        CreatedDateTime = DateTime.Now;
    }

    // 현재 입고 상세 항목에 대해, 연결된 발주서의 상세 발주 정보들을 자동으로 설정
    // 단위, 단가
    private void UpdateValuesFromPurchaseOrder()
    {
        if (MasterPurchaseInputObject == null || ItemObject == null || ItemObject.Oid == Guid.Empty) return;

        var detailOrder = MasterPurchaseInputObject.MasterPurchaseOrderObject?.DetailPurchaseOrderobjects?
            .FirstOrDefault(x => x.ItemObject.Oid == this.ItemObject.Oid);

        this.UnitObject = detailOrder?.UnitObject;
        this.UnitPrice = detailOrder?.UnitPrice ?? 0;
    }
    #endregion
}