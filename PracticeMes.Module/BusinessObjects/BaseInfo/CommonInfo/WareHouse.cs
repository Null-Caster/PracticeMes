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
public class WareHouse : BaseObject
{

    #region Properties
    [VisibleInLookupListView(true)]
    [RuleUniqueValue(CustomMessageTemplate = "창고 코드가 중복되었습니다.")]
    [RuleRequiredField(CustomMessageTemplate = "창고 코드를 입력하세요.")]
    [XafDisplayName("창고 코드"), ToolTip("창고 코드")]
    public string WareHouseCode
    {
        get { return GetPropertyValue<string>(nameof(WareHouseCode)); }
        set { SetPropertyValue(nameof(WareHouseCode), value); }
    }

    [VisibleInLookupListView(true)]
    [RuleRequiredField(CustomMessageTemplate = "창고 이름을 입력하세요.")]
    [XafDisplayName("창고 이름"), ToolTip("창고 이름")]
    public string WareHouseName
    {
        get { return GetPropertyValue<string>(nameof(WareHouseName)); }
        set { SetPropertyValue(nameof(WareHouseName), value); }
    }

    [VisibleInLookupListView(true)]
    [DataSourceCriteria("IsEnabled == True")]
    [ModelDefault("LookupProperty", nameof(Company.CompanyName))]
    [LookupEditorMode(LookupEditorMode.AllItemsWithSearch)]
    [RuleRequiredField(CustomMessageTemplate = "사업장 이름을 입력하세요.")]
    [XafDisplayName("사업장 이름"), ToolTip("사업장 이름")]
    public Company CompanyObject
    {
        get { return GetPropertyValue<Company>(nameof(CompanyObject)); }
        set { SetPropertyValue(nameof(CompanyObject), value); }
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
    [DataSourceCriteria("UniversalMajorCodeObject.MajorCode = 'WareHouseType' AND IsEnabled == True")]
    [ModelDefault("LookupProperty", nameof(UniversalMinorCode.CodeName))]
    [LookupEditorMode(LookupEditorMode.AllItemsWithSearch)]
    [RuleRequiredField(CustomMessageTemplate = "창고 유형을 입력하세요.")]
    [XafDisplayName("창고 유형"), ToolTip("창고 유형")]
    public UniversalMinorCode WareHouseType
    {
        get { return GetPropertyValue<UniversalMinorCode>(nameof(WareHouseType)); }
        set { SetPropertyValue(nameof(WareHouseType), value); }
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
    public WareHouse(Session session) : base(session) { }
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