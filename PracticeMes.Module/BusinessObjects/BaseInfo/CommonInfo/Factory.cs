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

namespace PracticeMes.Module.BusinessObjects.BaseInfo.CommonInfo;

[DefaultClassOptions]
[NavigationItem("공통 정보"), XafDisplayName("공장 등록")]
[DefaultListViewOptions(MasterDetailMode.ListViewOnly, true, NewItemRowPosition.Top)]
public class Factory : BaseObject
{
    #region Properties
    [VisibleInLookupListView(true)]
    [RuleUniqueValue(CustomMessageTemplate = "공장 코드가 중복되었습니다.")]
    [RuleRequiredField(CustomMessageTemplate = "공장 코드를 입력하세요.")]
    [XafDisplayName("공장 코드"), ToolTip("공장 코드")]
    public string FactoryCode
    {
        get { return GetPropertyValue<string>(nameof(FactoryCode)); }
        set { SetPropertyValue(nameof(FactoryCode), value); }
    }

    [VisibleInLookupListView(true)]
    [RuleRequiredField(CustomMessageTemplate = "공장 이름을 입력하세요.")]
    [XafDisplayName("공장 이름"), ToolTip("공장 이름")]
    public string FactoryName
    {
        get { return GetPropertyValue<string>(nameof(FactoryName)); }
        set { SetPropertyValue(nameof(FactoryName), value); }
    }

    [VisibleInLookupListView(true)]
    [XafDisplayName("공장 전화 번호"), ToolTip("공장 전화 번호")]
    public string TelephoneNumber
    {
        get { return GetPropertyValue<string>(nameof(TelephoneNumber)); }
        set { SetPropertyValue(nameof(TelephoneNumber), value); }
    }

    [VisibleInLookupListView(true)]
    [DataSourceCriteria("IsEnabled == True")]
    [ModelDefault("LookupProperty", nameof(Company.CompanyName))]
    [LookupEditorMode(LookupEditorMode.AllItemsWithSearch)]
    [RuleRequiredField(CustomMessageTemplate = "소속 사업장 이름을 입력하세요.")]
    [XafDisplayName("소속 사업장 이름"), ToolTip("소속 사업장 이름")]
    [ImmediatePostData]
    public Company CompanyObject
    {
        get { return GetPropertyValue<Company>(nameof(CompanyObject)); }
        set { SetPropertyValue(nameof(CompanyObject), value); }
    }

    [VisibleInLookupListView(true)]
    [ModelDefault("LookupProperty", nameof(Company.CompanyCode))]
    [LookupEditorMode(LookupEditorMode.AllItemsWithSearch)]
    [ModelDefault("AllowEdit", "False")]
    [XafDisplayName("소속 사업장 코드"), ToolTip("소속 사업장 코드")]
    public string CompanyObjectCode
    {
        get { return GetPropertyValue<string>(nameof(CompanyObjectCode)); }
        set { SetPropertyValue(nameof(CompanyObjectCode), value); }
    }

    [VisibleInLookupListView(true)]
    [RuleUniqueValue(CustomMessageTemplate = "Lot 코드가 중복되었습니다.")]
    [RuleRequiredField(CustomMessageTemplate = "Lot 코드를 입력하세요.")]
    [RuleRegularExpression("^[A-Z]{1,1}$", CustomMessageTemplate = "Lot 코드는 알파벳 대문자 1글자만 입력할 수 있습니다.")]
    [XafDisplayName("Lot 코드"), ToolTip("Lot 코드")]
    public string LotCode
    {
        get { return GetPropertyValue<string>(nameof(LotCode)); }
        set { SetPropertyValue(nameof(LotCode), value); }
    }

    [VisibleInLookupListView(true)]
    [DataSourceCriteria("UniversalMajorCodeObject.MajorCode == 'OrderType' AND IsEnabled == True")]
    [ModelDefault("LookupProperty", nameof(UniversalMinorCode.CodeName))]
    [LookupEditorMode(LookupEditorMode.AllItemsWithSearch)]
    [RuleRequiredField(CustomMessageTemplate = "생산 유형을 입력하세요.")]
    [XafDisplayName("생산 유형"), ToolTip("생산 유형")]
    public UniversalMinorCode OrderType
    {
        get { return GetPropertyValue<UniversalMinorCode>(nameof(OrderType)); }
        set { SetPropertyValue(nameof(OrderType), value); }
    }

    [VisibleInLookupListView(true)]
    [XafDisplayName("비고"), ToolTip("비고")]
    public string Note
    {
        get { return GetPropertyValue<string>(nameof(Note)); }
        set { SetPropertyValue(nameof(Note), value); }
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

    [VisibleInLookupListView(true)]
    [XafDisplayName("활성화 여부"), ToolTip("활성화 여부")]
    public bool IsEnabled
    {
        get { return GetPropertyValue<bool>(nameof(IsEnabled)); }
        set { SetPropertyValue(nameof(IsEnabled), value); }
    }
    #endregion

    #region Constructors
    public Factory(Session session) : base(session) { }
    #endregion

    #region Methods
    public override void AfterConstruction()
    {
        base.AfterConstruction();

        CreatedDateTime = DateTime.Now;
        IsEnabled = true;
    }
    #endregion

}