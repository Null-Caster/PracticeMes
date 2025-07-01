using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Pdf.Native.BouncyCastle.Utilities.IO.Pem;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using PracticeMes.Module.BusinessObjects.BaseInfo.CommonInfo;

namespace PracticeMes.Module.BusinessObjects.BaseInfo.ItemInfo;

[RuleCombinationOfPropertiesIsUnique("ProductBOM_Unique", DefaultContexts.Save, "BOMNumber, ItemObject, ProductType", "중복자료가 존재합니다.")]
[NavigationItem("품목 정보"), XafDisplayName("BOM 등록")]
[DefaultClassOptions]
[XafDefaultProperty(nameof(ItemObject))]
[ImageName("ListBullets")]
[DefaultListViewOptions(MasterDetailMode.ListViewOnly, true, NewItemRowPosition.Top)]
[Persistent(nameof(ProductBOM))]

// !!!!!!추후 관련 로직 물어보고 작업 진행 할 것!!!!!
public class ProductBOM : BaseObject
{
    #region Properties
    [VisibleInLookupListView(true)]
    [XafDisplayName("BOM_VER"), ToolTip("BOM_VER")]
    public double BOMNumber
    {
        get { return GetPropertyValue<double>(nameof(BOMNumber)); }
        set { SetPropertyValue(nameof(BOMNumber), value); }
    }
    [VisibleInLookupListView(true)]
    [ImmediatePostData(true)]
    [ModelDefault("LookupProperty", nameof(Item.ItemCode))]
    [LookupEditorMode(LookupEditorMode.AllItemsWithSearch)]
    // [RuleUniqueValue(CustomMessageTemplate = "품목 코드가 중복되었습니다.")]
    [RuleRequiredField(CustomMessageTemplate = "품목 코드를 입력하세요.")]
    [XafDisplayName("품목 코드"), ToolTip("품목 코드")]
    public Item ItemObject
    {
        get { return GetPropertyValue<Item>(nameof(ItemObject)); }
        set { SetPropertyValue(nameof(ItemObject), value); }
    }

    [VisibleInLookupListView(true)]
    [DataSourceCriteria("IsEnabled == True")]
    [ModelDefault("LookupProperty", nameof(ItemAccount.ItemAccountName))]
    [LookupEditorMode(LookupEditorMode.AllItemsWithSearch)]
    [RuleRequiredField(CustomMessageTemplate = "품목 유형를 입력하세요.")]
    [XafDisplayName("품목유형"), ToolTip("품목유형")]  // 제품, 반제품
    public ItemAccount ProductType
    {
        get { return GetPropertyValue<ItemAccount>(nameof(ProductType)); }
        set { SetPropertyValue(nameof(ProductType), value); }
    }
    [VisibleInLookupListView(true)]
    [XafDisplayName("품목 이름"), ToolTip("품목 이름")]
    public string ItemName
    {
        get { return this.ItemObject?.ItemName; }
    }
    [VisibleInLookupListView(true)]
    [DataSourceCriteria("IsEnabled == True")]
    [ModelDefault("LookupProperty", nameof(Factory.FactoryName))]
    [LookupEditorMode(LookupEditorMode.AllItemsWithSearch)]
    [RuleRequiredField(CustomMessageTemplate = "공장 이름을 입력하세요.")]
    [XafDisplayName("공장 이름"), ToolTip("공장 이름")]
    public Factory FactoryObject
    {
        get { return GetPropertyValue<Factory>(nameof(FactoryObject)); }
        set { SetPropertyValue(nameof(FactoryObject), value); }
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
    [ModelDefault("DisplayFormat", "yyyy/MM/dd HH:mm:ss")]
    [ModelDefault("EditMask", "yyyy/MM/dd HH:mm:ss")]
    [XafDisplayName("생성 일시"), ToolTip("항목이 생성된 일시입니다.")]
    public DateTime CreatedDateTime
    {
        get { return GetPropertyValue<DateTime>(nameof(CreatedDateTime)); }
        set { SetPropertyValue(nameof(CreatedDateTime), value); }
    }
    [VisibleInLookupListView(true)]
    [XafDisplayName("활성화 여부"), ToolTip("활성화 여부")]
    public bool IsEnabled
    {
        get { return GetPropertyValue<bool>(nameof(IsEnabled)); }
        set { SetPropertyValue(nameof(IsEnabled), value); }
    }
    [Browsable(false)]
    [ModelDefault("IsNonPersistent", "True")]
    public string fullNameOldValue
    {
        get { return GetPropertyValue<string>(nameof(fullNameOldValue)); }
        set { SetPropertyValue(nameof(fullNameOldValue), value); }
    }

    [XafDisplayName("BOM 등록 상세")]
    [Association(@"AssemblyBOM-ProductBOM")]
    public XPCollection<AssemblyBOM> AssemblyBOMObjects { get { return GetCollection<AssemblyBOM>(nameof(AssemblyBOMObjects)); } }
    #endregion

    #region Constructors
    public ProductBOM(Session session) : base(session) { }
    #endregion

    #region Fields
    private bool isDeleting;
    #endregion

    #region Methods
    public override void AfterConstruction()
    {
        base.AfterConstruction();
        FactoryObject = new XPCollection<Factory>()
        {
            Criteria = CriteriaOperator.Parse("IsEnabled == True"),
            Session = this.Session,
            Sorting = new SortingCollection(new SortProperty("CreatedDateTime", DevExpress.Xpo.DB.SortingDirection.Ascending)),
        }.FirstOrDefault();
        CreatedDateTime = DateTime.Now;
        IsEnabled = true;
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
            default:
                break;
        }
    }

    #endregion
}