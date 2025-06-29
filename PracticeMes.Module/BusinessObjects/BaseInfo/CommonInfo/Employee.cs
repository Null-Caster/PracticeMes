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
[NavigationItem("공통 정보"), XafDisplayName("직원 등록")]
[DefaultListViewOptions(MasterDetailMode.ListViewOnly, true, NewItemRowPosition.Top)]
public class Employee : BaseObject
{
    #region Properties
    [VisibleInLookupListView(true)]
    [RuleUniqueValue(CustomMessageTemplate = "사번이 중복되었습니다.")]
    [RuleRequiredField(CustomMessageTemplate = "사번을 입력하세요.")]
    [XafDisplayName("사번"), ToolTip("사번")]
    public string EmployeeID
    {
        get { return GetPropertyValue<string>(nameof(EmployeeID)); }
        set { SetPropertyValue(nameof(EmployeeID), value); }
    }

    [VisibleInLookupListView(true)]
    [RuleRequiredField(CustomMessageTemplate = "직원 이름을 입력하세요.")]
    [XafDisplayName("직원 이름"), ToolTip("직원 이름")]
    public string EmployeeName
    {
        get { return GetPropertyValue<string>(nameof(EmployeeName)); }
        set { SetPropertyValue(nameof(EmployeeName), value); }
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
    [DataSourceCriteria("IsEnabled == True")]
    [ModelDefault("LookupProperty", nameof(Department.DepartmentName))]
    [LookupEditorMode(LookupEditorMode.AllItemsWithSearch)]
    [RuleRequiredField(CustomMessageTemplate = "부서 이름을 입력하세요.")]
    [XafDisplayName("부서 이름"), ToolTip("부서 이름")]
    public Department DepartmentObject
    {
        get { return GetPropertyValue<Department>(nameof(DepartmentObject)); }
        set { SetPropertyValue(nameof(DepartmentObject), value); }
    }

    [VisibleInLookupListView(true)]
    [DataSourceCriteria("UniversalMajorCodeObject.MajorCode == 'Position' AND IsEnabled == True")]
    [ModelDefault("LookupProperty", nameof(UniversalMinorCode.CodeName))]
    [LookupEditorMode(LookupEditorMode.AllItemsWithSearch)]
    [XafDisplayName("직책"), ToolTip("직책")]
    public UniversalMinorCode Position
    {
        get { return GetPropertyValue<UniversalMinorCode>(nameof(Position)); }
        set { SetPropertyValue(nameof(Position), value); }
    }

    [VisibleInLookupListView(true)]
    [DataSourceCriteria("UniversalMajorCodeObject.MajorCode == 'TaskType' AND IsEnabled == True")]
    [ModelDefault("LookupProperty", nameof(UniversalMinorCode.CodeName))]
    [LookupEditorMode(LookupEditorMode.AllItemsWithSearch)]
    [XafDisplayName("직무"), ToolTip("직무")]
    public UniversalMinorCode TaskType
    {
        get { return GetPropertyValue<UniversalMinorCode>(nameof(TaskType)); }
        set { SetPropertyValue(nameof(TaskType), value); }
    }

    [VisibleInLookupListView(true)]
    [XafDisplayName("휴대 전화 번호"), ToolTip("휴대 전화 번호")]
    public string MobilephoneNumber
    {
        get { return GetPropertyValue<string>(nameof(MobilephoneNumber)); }
        set { SetPropertyValue(nameof(MobilephoneNumber), value); }
    }

    [VisibleInLookupListView(true)]
    [XafDisplayName("전화 번호"), ToolTip("전화 번호")]
    public string TelephoneNumber
    {
        get { return GetPropertyValue<string>(nameof(TelephoneNumber)); }
        set { SetPropertyValue(nameof(TelephoneNumber), value); }
    }

    [VisibleInLookupListView(true)]
    [XafDisplayName("Email"), ToolTip("Email")]
    public string Email
    {
        get { return GetPropertyValue<string>(nameof(Email)); }
        set { SetPropertyValue(nameof(Email), value); }
    }

    [VisibleInLookupListView(true)]
    [ModelDefault("AllowEdit", "False")]
    [ModelDefault("EditMask", "yyyy/MM/dd HH:mm:ss")]
    [ModelDefault("DisplayFormat", "yyyy/MM/dd HH:mm:ss")]
    [RuleRequiredField(CustomMessageTemplate = "생성 일시를 입력하세요.")]
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
    public Employee(Session session) : base(session) { }
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