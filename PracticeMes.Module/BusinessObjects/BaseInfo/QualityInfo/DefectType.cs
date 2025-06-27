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

namespace PracticeMes.Module.BusinessObjects.BaseInfo.QualityInfo;

[DefaultClassOptions]
[NavigationItem("품질 정보"), XafDisplayName("불량 유형 등록")]
[DefaultListViewOptions(MasterDetailMode.ListViewOnly, true, NewItemRowPosition.Top)]
public class DefectType : BaseObject
{
    #region Properties
    [VisibleInLookupListView(true)]
    [RuleUniqueValue(CustomMessageTemplate = "불량 유형 코드가 중복되었습니다.")]
    [RuleRequiredField(CustomMessageTemplate = "불량 유형 코드를 입력하세요.")]
    [XafDisplayName("불량 유형 코드"), ToolTip("불량 유형 코드")]
    public string DefectTypeCode
    {
        get { return GetPropertyValue<string>(nameof(DefectTypeCode)); }
        set { SetPropertyValue(nameof(DefectTypeCode), value); }
    }

    [VisibleInLookupListView(true)]
    [RuleRequiredField(CustomMessageTemplate = "불량 유형 이름을 입력하세요.")]
    [XafDisplayName("불량 유형 이름"), ToolTip("불량 유형 이름")]
    public string DefectTypeName
    {
        get { return GetPropertyValue<string>(nameof(DefectTypeName)); }
        set { SetPropertyValue(nameof(DefectTypeName), value); }
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

    [XafDisplayName("활성화 여부"), ToolTip("활성화 여부")]
    public bool IsEnabled
    {
        get { return GetPropertyValue<bool>(nameof(IsEnabled)); }
        set { SetPropertyValue(nameof(IsEnabled), value); }
    }

    #endregion

    #region Constructors
    public DefectType(Session session) : base(session) { }
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