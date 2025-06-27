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

namespace PracticeMes.Module.BusinessObjects.BaseInfo.CommonInfol;

[DefaultClassOptions]
[NavigationItem("공통 정보"), XafDisplayName("단위 등록")]
[DefaultListViewOptions(MasterDetailMode.ListViewOnly, true, NewItemRowPosition.Top)]
public class Unit : BaseObject
{
    #region Properties
    [VisibleInLookupListView(true)]
    [RuleUniqueValue(CustomMessageTemplate = "단위 코드가 중복되었습니다.")]
    [RuleRequiredField(CustomMessageTemplate = "단위 코드를 입력하세요.")]
    [XafDisplayName("단위 코드"), ToolTip("단위 코드")]
    public string UnitCode
    {
        get { return GetPropertyValue<string>(nameof(UnitCode)); }
        set { SetPropertyValue(nameof(UnitCode), value); }
    }

    [VisibleInLookupListView(true)]
    [RuleRequiredField(CustomMessageTemplate = "단위 이름을 입력하세요.")]
    [XafDisplayName("단위 이름"), ToolTip("단위 이름")]
    public string UnitName
    {
        get { return GetPropertyValue<string>(nameof(UnitName)); }
        set { SetPropertyValue(nameof(UnitName), value); }
    }

    [VisibleInLookupListView(true)]
    [XafDisplayName("기호"), ToolTip("기호")]
    public string Symbol
    {
        get { return GetPropertyValue<string>(nameof(Symbol)); }
        set { SetPropertyValue(nameof(Symbol), value); }
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
    public Unit(Session session) : base(session) { }
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