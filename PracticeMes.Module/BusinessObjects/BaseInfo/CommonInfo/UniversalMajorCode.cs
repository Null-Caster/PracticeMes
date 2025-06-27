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
[NavigationItem("공통 정보"), XafDisplayName("코드 등록")]
[DefaultListViewOptions(MasterDetailMode.ListViewAndDetailView, true, NewItemRowPosition.Top)]
public class UniversalMajorCode : BaseObject
{
    #region Properties
    [VisibleInLookupListView(true)]
    [RuleUniqueValue(CustomMessageTemplate = "주 코드가 중복되었습니다.")]
    [RuleRequiredField(CustomMessageTemplate = "주 코드를 입력하세요.")]
    [XafDisplayName("주 코드"), ToolTip("주 코드")]
    public string MajorCode
    {
        get { return GetPropertyValue<string>(nameof(MajorCode)); }
        set { SetPropertyValue(nameof(MajorCode), value); }
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

    [Association(@"UniversalMinorCodeReferencesUniversalMajorCode"), DevExpress.Xpo.Aggregated]
    [XafDisplayName("부 코드"), ToolTip("부 코드")]
    public XPCollection<UniversalMinorCode> UniversalMinorCodeObjects { get { return GetCollection<UniversalMinorCode>(nameof(UniversalMinorCodeObjects)); } }
    #endregion

    #region Constructors
    public UniversalMajorCode(Session session) : base(session) { }
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