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

namespace PracticeMes.Module.BusinessObjects.BaseInfo.ItemInfo;

[DefaultClassOptions]
[NavigationItem("품목 정보"), XafDisplayName("품목 등록")]
[DefaultListViewOptions(MasterDetailMode.ListViewOnly, true, NewItemRowPosition.Top)]
public class Item : BaseObject
{
    #region Properties
    [VisibleInLookupListView(true)]
    [RuleUniqueValue(CustomMessageTemplate = "품목코드가 중복되었습니다.")]
    [RuleRequiredField(CustomMessageTemplate = "품목코드를 입력하세요.")]
    [XafDisplayName("품목코드"), ToolTip("품목코드")]
    public string ItemCode
    {
        get { return GetPropertyValue<string>(nameof(ItemCode)); }
        set { SetPropertyValue(nameof(ItemCode), value); }
    }

    [VisibleInLookupListView(true)]
    [RuleRequiredField(CustomMessageTemplate = "품목이름을 입력하세요.")]
    [XafDisplayName("품목이름"), ToolTip("품목이름")]
    public string ItemName
    {
        get { return GetPropertyValue<string>(nameof(ItemName)); }
        set { SetPropertyValue(nameof(ItemName), value); }
    }

    [VisibleInLookupListView(true)]
    [DataSourceCriteria("IsEnabled == True")]
    [ModelDefault("LookupProperty", nameof(ItemGroup.ItemGroupName))]
    [LookupEditorMode(LookupEditorMode.AllItemsWithSearch)]
    [XafDisplayName("품목그룹"), ToolTip("품목그룹")]
    public ItemGroup ItemGroupObject
    {
        get { return GetPropertyValue<ItemGroup>(nameof(ItemGroupObject)); }
        set { SetPropertyValue(nameof(ItemGroupObject), value); }
    }

    [VisibleInLookupListView(true)]
    [DataSourceCriteria("IsEnabled == True AND ItemAccountName != '반제품'")]
    [ModelDefault("LookupProperty", nameof(ItemAccount.ItemAccountName))]
    [LookupEditorMode(LookupEditorMode.AllItemsWithSearch)]
    [RuleRequiredField(CustomMessageTemplate = "품목계정을 입력하세요.")]
    [XafDisplayName("품목계정"), ToolTip("품목계정")]
    public ItemAccount ItemAccountObject
    {
        get { return GetPropertyValue<ItemAccount>(nameof(ItemAccountObject)); }
        set { SetPropertyValue(nameof(ItemAccountObject), value); }
    }

    [VisibleInLookupListView(true)]
    [DataSourceCriteria("UniversalMajorCodeObject.MajorCode == 'RegistType' AND IsEnabled == True")]
    [ModelDefault("LookupProperty", nameof(UniversalMinorCode.CodeName))]
    [LookupEditorMode(LookupEditorMode.AllItemsWithSearch)]
    [RuleRequiredField(CustomMessageTemplate = "품목계정을 입력하세요.")]
    [XafDisplayName("등록유형"), ToolTip("등록유형")]
    public UniversalMinorCode RegistType
    {
        get { return GetPropertyValue<UniversalMinorCode>(nameof(RegistType)); }
        set { SetPropertyValue(nameof(RegistType), value); }
    }

    [VisibleInLookupListView(true)]
    [ImmediatePostData]
    [ModelDefault("LookupProperty", nameof(Unit.UnitName))]
    [RuleRequiredField(CustomMessageTemplate = "단위를 입력하세요.")]
    [LookupEditorMode(LookupEditorMode.AllItemsWithSearch)]
    [XafDisplayName("단위"), ToolTip("단위")]
    public Unit UnitObject
    {
        get { return GetPropertyValue<Unit>(nameof(UnitObject)); }
        set { SetPropertyValue(nameof(UnitObject), value); }
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
    #endregion

    #region Constructors
    #endregion

    #region Methods
    #endregion

    public Item(Session session) : base(session) { }
    public override void AfterConstruction()
    {
        base.AfterConstruction();

        CreatedDateTime = DateTime.Now;
        IsEnabled = true;
    }
}