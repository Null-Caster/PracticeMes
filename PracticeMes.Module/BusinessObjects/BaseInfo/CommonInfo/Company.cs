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
[NavigationItem("공통 정보"), XafDisplayName("사업장 등록")]
[DefaultListViewOptions(MasterDetailMode.ListViewOnly, true, NewItemRowPosition.Top)]
public class Company : BaseObject
{
    #region Properties
    [VisibleInLookupListView(true)]
    [RuleUniqueValue(CustomMessageTemplate = "사업장 코드가 중복되었습니다.")]
    [RuleRequiredField(CustomMessageTemplate = "사업장 코드를 입력하세요.")]
    [XafDisplayName("사업장 코드"), ToolTip("사업장 코드")]
    public string CompanyCode
    {
        get { return GetPropertyValue<string>(nameof(CompanyCode)); }
        set { SetPropertyValue(nameof(CompanyCode), value); }
    }

    [VisibleInLookupListView(true)]
    [RuleRequiredField(CustomMessageTemplate = "사업장 이름을 입력하세요.")]
    [XafDisplayName("사업장 이름"), ToolTip("사업장 이름")]
    public string CompanyName
    {
        get { return GetPropertyValue<string>(nameof(CompanyName)); }
        set { SetPropertyValue(nameof(CompanyName), value); }
    }

    [VisibleInLookupListView(true)]
    [XafDisplayName("사업자 등록 번호"), ToolTip("사업자 등록 번호")]
    public string BusinessRegistrationNumber
    {
        get { return GetPropertyValue<string>(nameof(BusinessRegistrationNumber)); }
        set { SetPropertyValue(nameof(BusinessRegistrationNumber), value); }
    }

    [VisibleInLookupListView(true)]
    [XafDisplayName("대표명"), ToolTip("대표명")]
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
    [XafDisplayName("전화 번호"), ToolTip("전화 번호")]
    public string TelephoneNumber
    {
        get { return GetPropertyValue<string>(nameof(TelephoneNumber)); }
        set { SetPropertyValue(nameof(TelephoneNumber), value); }
    }

    [VisibleInLookupListView(true)]
    [XafDisplayName("우편 번호"), ToolTip("우편 번호")]
    public string PostalCode
    {
        get { return GetPropertyValue<string>(nameof(PostalCode)); }
        set { SetPropertyValue(nameof(PostalCode), value); }
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
    public Company(Session session) : base(session) { }
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