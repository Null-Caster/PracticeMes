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
[NavigationItem(false)]
[DefaultListViewOptions(MasterDetailMode.ListViewOnly, true, NewItemRowPosition.Top)]
public class UniversalMinorCode : BaseObject
{
    #region Properties
    [VisibleInLookupListView(true)]
    [RuleRequiredField(CustomMessageTemplate = "부 코드를 입력하세요.")]
    [XafDisplayName("부 코드"), ToolTip("부 코드")]
    public string MinorCode
    {
        get { return GetPropertyValue<string>(nameof(MinorCode)); }
        set { SetPropertyValue(nameof(MinorCode), value); }
    }

    [VisibleInLookupListView(true)]
    [RuleRequiredField(CustomMessageTemplate = "코드 이름을 입력하세요.")]
    [XafDisplayName("코드 이름"), ToolTip("코드 이름")]
    public string CodeName
    {
        get { return GetPropertyValue<string>(nameof(CodeName)); }
        set { SetPropertyValue(nameof(CodeName), value); }
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

    [RuleRequiredField]
    [Association(@"UniversalMinorCodeReferencesUniversalMajorCode")]
    [XafDisplayName("주 코드"), ToolTip("주 코드")]
    public UniversalMajorCode UniversalMajorCodeObject
    {
        get { return GetPropertyValue<UniversalMajorCode>(nameof(UniversalMajorCodeObject)); }
        set { SetPropertyValue<UniversalMajorCode>(nameof(UniversalMajorCodeObject), value); }
    }
    #endregion

    #region Constructors
    public UniversalMinorCode(Session session) : base(session) {}
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