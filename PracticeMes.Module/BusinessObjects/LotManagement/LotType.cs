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

namespace PracticeMes.Module.BusinessObjects.LotManagement;

[DefaultClassOptions]
[NavigationItem("Lot 관리"), XafDisplayName("Lot 유형 등록")]
[DefaultListViewOptions(MasterDetailMode.ListViewOnly, true, NewItemRowPosition.None)]
public class LotType : BaseObject
{
    #region Properties
    [VisibleInLookupListView(true)]
    [RuleUniqueValue(CustomMessageTemplate = "Lot 유형 코드가 중복되었습니다.")]
    [RuleRequiredField(CustomMessageTemplate = "Lot 유형 코드를 입력하세요.")]
    [RuleRegularExpression("^[A-Z]{1,1}$", CustomMessageTemplate = "Lot 유형 코드는 알파벳 대문자 1글자만 입력할 수 있습니다.")]
    [XafDisplayName("Lot 유형 코드"), ToolTip("Lot 유형 코드")]
    public string LotTypeCode
    {
        get { return GetPropertyValue<string>(nameof(LotTypeCode)); }
        set { SetPropertyValue(nameof(LotTypeCode), value); }
    }

    [VisibleInLookupListView(true)]
    [RuleRequiredField(CustomMessageTemplate = "Lot 유형 이름을 입력하세요.")]
    [XafDisplayName("Lot 유형 이름"), ToolTip("Lot 유형 이름")]
    public string LotTypeName
    {
        get { return GetPropertyValue<string>(nameof(LotTypeName)); }
        set { SetPropertyValue(nameof(LotTypeName), value); }
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

    [VisibleInLookupListView(true)]
    [XafDisplayName("비고"), ToolTip("비고")]
    public string Note
    {
        get { return GetPropertyValue<string>(nameof(Note)); }
        set { SetPropertyValue(nameof(Note), value); }
    }
    #endregion

    #region Constructors
    public LotType(Session session) : base(session) { }
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