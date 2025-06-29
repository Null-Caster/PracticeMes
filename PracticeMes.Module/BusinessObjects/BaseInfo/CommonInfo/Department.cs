using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Base.General;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;

namespace PracticeMes.Module.BusinessObjects.BaseInfo.CommonInfo;

[DefaultClassOptions]
[NavigationItem("공통 정보"), XafDisplayName("부서 등록")]
[DefaultListViewOptions(MasterDetailMode.ListViewOnly, false, NewItemRowPosition.None)]
public class Department : BaseObject, ITreeNode
{
    #region Properties
    [RuleUniqueValue(CustomMessageTemplate = "부서 코드가 중복되었습니다.")]
    [RuleRequiredField(CustomMessageTemplate = "부서 코드를 입력하세요.")]
    [XafDisplayName("부서 코드"), ToolTip("부서 코드")]
    public string DepartmentCode
    {
        get { return GetPropertyValue<string>(nameof(DepartmentCode)); }
        set { SetPropertyValue(nameof(DepartmentCode), value); }
    }

    [RuleRequiredField(CustomMessageTemplate = "부서 이름을 입력하세요.")]
    [XafDisplayName("부서 이름"), ToolTip("부서 이름")]
    public string DepartmentName
    {
        get { return GetPropertyValue<string>(nameof(DepartmentName)); }
        set { SetPropertyValue(nameof(DepartmentName), value); }
    }

    [DataSourceCriteria("IsEnabled = True")]
    [ModelDefault("LookupProperty", nameof(Company.CompanyName))]
    [LookupEditorMode(LookupEditorMode.AllItemsWithSearch)]
    [RuleRequiredField(CustomMessageTemplate = "사업장 이름을 입력하세요.")]
    [XafDisplayName("사업장 이름"), ToolTip("사업장 이름")]
    public Company CompanyObject
    {
        get { return GetPropertyValue<Company>(nameof(CompanyObject)); }
        set { SetPropertyValue(nameof(CompanyObject), value); }
    }

    [DataSourceCriteria("IsEnabled = True")]
    [ModelDefault("LookupProperty", nameof(Factory.FactoryName))]
    [LookupEditorMode(LookupEditorMode.AllItemsWithSearch)]
    [RuleRequiredField(CustomMessageTemplate = "공장 이름을 입력하세요.")]
    [XafDisplayName("공장 이름"), ToolTip("공장 이름")]
    public Factory FactoryObject
    {
        get { return GetPropertyValue<Factory>(nameof(FactoryObject)); }
        set { SetPropertyValue(nameof(FactoryObject), value); }
    }

    [ModelDefault("AllowEdit", "False")]
    [ModelDefault("DisplayFormat", "yyyy/MM/dd HH:mm:ss.fff")]
    [RuleRequiredField(CustomMessageTemplate = "생성 일시를 입력하세요.")]
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

    [Browsable(false)]
    [ModelDefault("AllowEdit", "false")]
    [XafDisplayName("노드 이름"), ToolTip("노드 이름")]
    public string Name { get { return DepartmentName is null ? string.Empty : $"부서 코드: {DepartmentCode} 부서 이름: {DepartmentName}"; } }
    [Appearance("Parent", Enabled = false, Visibility = ViewItemVisibility.Hide)]
    [ModelDefault("AllowEdit", "false")]
    [Association("DepartmentParent-DepartmentChild")]
    [XafDisplayName("부모 노드 이름"), ToolTip("부모 노드 이름")]

    public Department Parent
    {
        get { return GetPropertyValue<Department>(nameof(Parent)); }
        set { SetPropertyValue(nameof(Parent), value); }
    }

    [Association("DepartmentParent-DepartmentChild"), DevExpress.Xpo.Aggregated]
    public XPCollection<Department> Children { get { return GetCollection<Department>(nameof(Children)); } }
    #endregion

    #region Fields
    IBindingList ITreeNode.Children { get { return Children as IBindingList; } }
    ITreeNode ITreeNode.Parent { get { return Parent as ITreeNode; } }
    #endregion

    #region Constructors
    public Department(Session session) : base(session) { }
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