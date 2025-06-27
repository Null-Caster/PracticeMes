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
[NavigationItem("공통 정보"), XafDisplayName("거래처")]
[DefaultListViewOptions(MasterDetailMode.ListViewOnly, true, NewItemRowPosition.Top)]
public class BusinessPartner : BaseObject
{
    #region Properties
    [VisibleInLookupListView(true)]
    [RuleUniqueValue(CustomMessageTemplate = "거래처 코드가 중복되었습니다.")]
    [RuleRequiredField(CustomMessageTemplate = "거래처 코드를 입력하세요.")]
    [XafDisplayName("거래처 코드"), ToolTip("거래처 코드")]
    public string BusinessPartnerCode
    {
        get { return GetPropertyValue<string>(nameof(BusinessPartnerCode)); }
        set { SetPropertyValue(nameof(BusinessPartnerCode), value); }
    }

    [VisibleInLookupListView(true)]
    [RuleRequiredField(CustomMessageTemplate = "거래처 이름을 입력하세요.")]
    [XafDisplayName("거래처 이름"), ToolTip("거래처 이름")]
    public string BusinessPartnerName
    {
        get { return GetPropertyValue<string>(nameof(BusinessPartnerName)); }
        set { SetPropertyValue(nameof(BusinessPartnerName), value); }
    }

    [VisibleInLookupListView(true)]
    [RuleRequiredField(CustomMessageTemplate = "사업자 등록 번호를 입력하세요.")]
    [XafDisplayName("사업자 등록 번호"), ToolTip("사업자 등록 번호")]
    public string BusinessRegistrationNumber
    {
        get { return GetPropertyValue<string>(nameof(BusinessRegistrationNumber)); }
        set { SetPropertyValue(nameof(BusinessRegistrationNumber), value); }
    }

    [VisibleInLookupListView(true)]
    [DataSourceCriteria("UniversalMajorCodeObject.MajorCode == 'BusinessPartnerType' AND IsEnabled == True")]
    [ModelDefault("LookupProperty", nameof(UniversalMinorCode.CodeName))]
    [LookupEditorMode(LookupEditorMode.AllItemsWithSearch)]
    [RuleRequiredField(CustomMessageTemplate = "거래처 유형을 입력하세요.")]
    [XafDisplayName("거래처 유형"), ToolTip("거래처 유형")]
    [ImmediatePostData]
    public UniversalMinorCode BizPartnerType
    {
        get { return GetPropertyValue<UniversalMinorCode>(nameof(BizPartnerType)); }
        set { SetPropertyValue(nameof(BizPartnerType), value); }
    }

    [VisibleInLookupListView(true)]
    [ModelDefault("AllowEdit", "False")]
    [XafDisplayName("거래처유형코드")]
    public string BizPartnerTypeCode
    {
        get { return BizPartnerType?.MinorCode; }
    }

    [VisibleInLookupListView(true)]
    [RuleRequiredField(CustomMessageTemplate = "대표자 이름을 입력하세요.")]
    [XafDisplayName("대표자 이름"), ToolTip("대표자 이름")]
    public string RepresentativeName
    {
        get { return GetPropertyValue<string>(nameof(RepresentativeName)); }
        set { SetPropertyValue(nameof(RepresentativeName), value); }
    }

    [VisibleInLookupListView(true)]
    [XafDisplayName("주소"), ToolTip("주소")]
    public string Address
    {
        get { return GetPropertyValue<string>(nameof(Address)); }
        set { SetPropertyValue(nameof(Address), value); }
    }

    [VisibleInLookupListView(true)]
    [XafDisplayName("전화번호"), ToolTip("전화번호")]
    public string TelephoneNumber
    {
        get { return GetPropertyValue<string>(nameof(TelephoneNumber)); }
        set { SetPropertyValue(nameof(TelephoneNumber), value); }
    }

    [VisibleInLookupListView(true)]
    [DataSourceCriteria("IsEnabled == True")]
    [ModelDefault("LookupProperty", nameof(Employee.EmployeeName))]
    [LookupEditorMode(LookupEditorMode.AllItemsWithSearch)]
    [XafDisplayName("계약담당자"), ToolTip("계약담당자")]
    public Employee EmployeeObject
    {
        get { return GetPropertyValue<Employee>(nameof(EmployeeObject)); }
        set { SetPropertyValue(nameof(EmployeeObject), value); }
    }

    [VisibleInLookupListView(true)]
    [XafDisplayName("계약담당자번호"), ToolTip("계약담당자번호")]
    public string MobilephoneNumber
    {
        get { return GetPropertyValue<string>(nameof(MobilephoneNumber)); }
        set { SetPropertyValue(nameof(MobilephoneNumber), value); }
    }

    [VisibleInLookupListView(true)]
    [XafDisplayName("계약담당자메일"), ToolTip("계약담당자메일")]
    public string Email
    {
        get { return GetPropertyValue<string>(nameof(Email)); }
        set { SetPropertyValue(nameof(Email), value); }
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

    [VisibleInLookupListView(true)]
    [XafDisplayName("우편 번호"), ToolTip("우편 번호")]
    public string PostalCode
    {
        get { return GetPropertyValue<string>(nameof(PostalCode)); }
        set { SetPropertyValue(nameof(PostalCode), value); }
    }

    [VisibleInLookupListView(true)]
    [XafDisplayName("종목"), ToolTip("종목")]
    public string BusinessItem
    {
        get { return GetPropertyValue<string>(nameof(BusinessItem)); }
        set { SetPropertyValue(nameof(BusinessItem), value); }
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
    public BusinessPartner(Session session) : base(session) { }
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