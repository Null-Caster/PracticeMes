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

namespace PracticeMes.Module.BusinessObjects.LotManagement;

[DefaultClassOptions]
[NavigationItem("Lot 관리"), XafDisplayName("Lot 조회")]
[DefaultListViewOptions(MasterDetailMode.ListViewOnly, false, NewItemRowPosition.Top)]
public class Lot : BaseObject
{

    #region Properties
    [VisibleInLookupListView(true)]
    [ModelDefault("AllowEdit", "false")]
    [RuleUniqueValue(CustomMessageTemplate = "Lot 번호가 중복되었습니다.")]
    [RuleRequiredField(CustomMessageTemplate = "Lot 번호를 입력하세요.")]
    [XafDisplayName("Lot 번호"), ToolTip("Lot 번호")]
    public string LotNumber
    {
        get { return GetPropertyValue<string>(nameof(LotNumber)); }
        set { SetPropertyValue(nameof(LotNumber), value); }
    }

    [VisibleInLookupListView(true)]
    [ModelDefault("AllowEdit", "false")]
    [DataSourceCriteria("IsEnabled == True")]
    [ModelDefault("LookupProperty", nameof(LotType.LotTypeName))]
    [LookupEditorMode(LookupEditorMode.AllItemsWithSearch)]
    [RuleRequiredField(CustomMessageTemplate = "Lot유형을 입력하세요.")]
    [XafDisplayName("Lot유형"), ToolTip("Lot유형")]
    public LotType LotTypeObject
    {
        get { return GetPropertyValue<LotType>(nameof(LotTypeObject)); }
        set { SetPropertyValue(nameof(LotTypeObject), value); }
    }

    [VisibleInLookupListView(true)]
    [ModelDefault("AllowEdit", "false")]
    [DataSourceCriteria("IsEnabled == True")]
    [ModelDefault("LookupProperty", nameof(Factory.FactoryName))]
    [LookupEditorMode(LookupEditorMode.AllItemsWithSearch)]
    [XafDisplayName("공장"), ToolTip("공장")]
    public Factory FactoryObject
    {
        get { return GetPropertyValue<Factory>(nameof(FactoryObject)); }
        set { SetPropertyValue(nameof(FactoryObject), value); }
    }

    [VisibleInLookupListView(true)]
    [ModelDefault("AllowEdit", "false")]
    [DataSourceCriteria("IsEnabled == True")]
    [ModelDefault("LookupProperty", nameof(WareHouse.WareHouseName))]
    [LookupEditorMode(LookupEditorMode.AllItemsWithSearch)]
    [XafDisplayName("창고"), ToolTip("창고")]
    public WareHouse WareHouseObject
    {
        get { return GetPropertyValue<WareHouse>(nameof(WareHouseObject)); }
        set { SetPropertyValue(nameof(WareHouseObject), value); }
    }

    [VisibleInLookupListView(true)]
    [ModelDefault("AllowEdit", "false")]
    [DataSourceCriteria("IsEnabled == True")]
    [ModelDefault("LookupProperty", nameof(ItemAccount.ItemAccountName))]
    [LookupEditorMode(LookupEditorMode.AllItemsWithSearch)]
    [RuleRequiredField(CustomMessageTemplate = "품목 계정을 입력하세요.")]
    [XafDisplayName("품목 계정 이름"), ToolTip("품목 계정 이름")]
    public ItemAccount ItemAccountObject
    {
        get { return GetPropertyValue<ItemAccount>(nameof(ItemAccountObject)); }
        set { SetPropertyValue(nameof(ItemAccountObject), value); }
    }

    [VisibleInLookupListView(true)]
    [ModelDefault("AllowEdit", "false")]
    [DataSourceCriteria("IsEnabled == True")]
    [ModelDefault("LookupProperty", nameof(Item.ItemName))]
    [LookupEditorMode(LookupEditorMode.AllItemsWithSearch)]
    [RuleRequiredField(CustomMessageTemplate = "품목을 입력하세요.")]
    [XafDisplayName("품목"), ToolTip("품목")]
    public Item ItemObject
    {
        get { return GetPropertyValue<Item>(nameof(ItemObject)); }
        set { SetPropertyValue(nameof(ItemObject), value); }
    }

    [VisibleInLookupListView(true)]
    [DataSourceCriteria("IsEnabled == True")]
    [ModelDefault("LookupProperty", nameof(Unit.UnitName))]
    [LookupEditorMode(LookupEditorMode.AllItemsWithSearch)]
    [RuleRequiredField(CustomMessageTemplate = "단위 입력하세요.")]
    [XafDisplayName("단위"), ToolTip("단위")]
    public Unit UnitObject
    {
        get { return GetPropertyValue<Unit>(nameof(UnitObject)); }
        set { SetPropertyValue(nameof(UnitObject), value); }
    }

    [VisibleInLookupListView(true)]
    [ModelDefault("EditMask", "###,###,###,###,###,###,###,###,###,##0.###")]
    [ModelDefault("AllowEdit", "false")]
    [XafDisplayName("재고수량"), ToolTip("재고수량")]
    public double StockQuantity
    {
        get { return GetPropertyValue<double>(nameof(StockQuantity)); }
        set { SetPropertyValue(nameof(StockQuantity), value); }
    }

    [VisibleInLookupListView(true)]
    [ModelDefault("EditMask", "###,###,###,###,###,###,###,###,###,##0.###")]
    [ModelDefault("AllowEdit", "false")]
    [XafDisplayName("생성수량"), ToolTip("생성수량")]
    public double CreateQuantity
    {
        get { return GetPropertyValue<double>(nameof(CreateQuantity)); }
        set { SetPropertyValue(nameof(CreateQuantity), value); }
    }

    [VisibleInLookupListView(true)]
    [ModelDefault("EditMask", "###,###,###,###,###,###,###,###,###,##0.###")]
    [ModelDefault("AllowEdit", "false")]
    [XafDisplayName("불량수량"), ToolTip("불량수량")]
    public double DefectQuantity
    {
        get { return GetPropertyValue<double>(nameof(DefectQuantity)); }
        set { SetPropertyValue(nameof(DefectQuantity), value); }
    }

    [VisibleInLookupListView(true)]
    [ModelDefault("EditMask", "###,###,###,###,###,###,###,###,###,##0.###")]
    [ModelDefault("AllowEdit", "false")]
    [XafDisplayName("투입수량"), ToolTip("투입수량")]
    public double InputQuantity
    {
        get { return GetPropertyValue<double>(nameof(InputQuantity)); }
        set { SetPropertyValue(nameof(InputQuantity), value); }
    }

    [VisibleInLookupListView(true)]
    [ModelDefault("EditMask", "###,###,###,###,###,###,###,###,###,##0.###")]
    [ModelDefault("AllowEdit", "false")]
    [XafDisplayName("출고수량"), ToolTip("출고수량")]
    public double ReleaseQuantity
    {
        get { return GetPropertyValue<double>(nameof(ReleaseQuantity)); }
        set { SetPropertyValue(nameof(ReleaseQuantity), value); }
    }

    [VisibleInLookupListView(true)]
    [ModelDefault("EditMask", "###,###,###,###,###,###,###,###,###,##0.###")]
    [ModelDefault("AllowEdit", "false")]
    [XafDisplayName("반품수량"), ToolTip("반품수량")]
    public double ReturnQuantity
    {
        get { return GetPropertyValue<double>(nameof(ReturnQuantity)); }
        set { SetPropertyValue(nameof(ReturnQuantity), value); }
    }

    [VisibleInLookupListView(true)]
    [ModelDefault("EditMask", "###,###,###,###,###,###,###,###,###,##0.###")]
    [ModelDefault("AllowEdit", "false")]
    [XafDisplayName("검사요청수량"), ToolTip("검사요청수량")]
    public double InspectionRequestQuantity
    {
        get { return GetPropertyValue<double>(nameof(InspectionRequestQuantity)); }
        set { SetPropertyValue(nameof(InspectionRequestQuantity), value); }
    }

    [VisibleInLookupListView(true)]
    [ModelDefault("EditMask", "###,###,###,###,###,###,###,###,###,##0.###")]
    [ModelDefault("AllowEdit", "false")]
    [XafDisplayName("검사수량"), ToolTip("검사수량")]
    public double InspectiontQuantity
    {
        get { return GetPropertyValue<double>(nameof(InspectiontQuantity)); }
        set { SetPropertyValue(nameof(InspectiontQuantity), value); }
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
    #endregion

    #region Constructors
    public Lot(Session session) : base(session) { }
    #endregion

    #region Fields
    private bool isDeleting;
    #endregion

    #region Methods
    public override void AfterConstruction()
    {
        base.AfterConstruction();

        CreatedDateTime = DateTime.Now;
    }

    protected override void OnSaving()
    {
        base.OnSaving();

        // 신규 등록
        if ((this.Session is not NestedUnitOfWork)
            && (this.Session.DataLayer is not null)
            && (this.Session.ObjectLayer is SimpleObjectLayer)
            && (this.Session.IsNewObject(this) == true))
        {
            this.StockQuantity = CreateQuantity - DefectQuantity - InputQuantity - ReleaseQuantity - InspectionRequestQuantity + InspectiontQuantity - ReturnQuantity;
            if (LotTypeObject?.LotTypeName == "불량")
            {
                this.StockQuantity = 0;
            }
        }
        // 수정
        if ((this.Session is not NestedUnitOfWork)
             && (this.Session.DataLayer is not null)
             && (this.Session.ObjectLayer is SimpleObjectLayer)
             && (this.Session.IsNewObject(this) == false)
             && isDeleting == false && IsDeleted == false)
        {
            this.StockQuantity = CreateQuantity - DefectQuantity - InputQuantity - ReleaseQuantity - InspectionRequestQuantity + InspectiontQuantity - ReturnQuantity;
            if (LotTypeObject?.LotTypeName == "불량")
            {
                this.StockQuantity = 0;
            }
        }
    }
    #endregion

}
