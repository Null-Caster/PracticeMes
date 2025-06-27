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
using PracticeMes.Module.BusinessObjects.BaseInfo.CommonInfo;

namespace PracticeMes.Module.BusinessObjects.BaseInfo.ProductionInfo;

[DefaultClassOptions]
[NavigationItem("생산 정보"), XafDisplayName("작업장 등록")]
[DefaultListViewOptions(MasterDetailMode.ListViewOnly, true, NewItemRowPosition.Top)]
public class WorkCenter : BaseObject
{
    #region Properties
    [VisibleInLookupListView(true)]
    [RuleUniqueValue(CustomMessageTemplate = "작업장코드가 중복되었습니다.")]
    [RuleRequiredField(CustomMessageTemplate = "작업장코드를 입력하세요.")]
    [XafDisplayName("작업장코드"), ToolTip("작업장코드")]
    public string WorkCenterCode
    {
        get { return GetPropertyValue<string>(nameof(WorkCenterCode)); }
        set { SetPropertyValue(nameof(WorkCenterCode), value); }
    }

    [VisibleInLookupListView(true)]
    [RuleRequiredField(CustomMessageTemplate = "작업장명을 입력하세요.")]
    [XafDisplayName("작업장명"), ToolTip("작업장명")]
    public string WorkCenterName
    {
        get { return GetPropertyValue<string>(nameof(WorkCenterName)); }
        set { SetPropertyValue(nameof(WorkCenterName), value); }
    }

    [VisibleInLookupListView(true)]
    [DataSourceCriteria("IsEnabled == True")]
    [ModelDefault("LookupProperty", nameof(Department.DepartmentName))]
    [LookupEditorMode(LookupEditorMode.AllItemsWithSearch)]
    [RuleRequiredField(CustomMessageTemplate = "소속부서명을 입력하세요.")]
    [XafDisplayName("소속부서명"), ToolTip("소속부서명")]
    public Department DepartmentObject
    {
        get { return GetPropertyValue<Department>(nameof(DepartmentObject)); }
        set { SetPropertyValue(nameof(DepartmentObject), value); }
    }

    [VisibleInLookupListView(true)]
    [DataSourceCriteria("UniversalMajorCodeObject.MajorCode == 'WorkCenterType' AND IsEnabled == True")]
    [ModelDefault("LookupProperty", nameof(UniversalMinorCode.CodeName))]
    [LookupEditorMode(LookupEditorMode.AllItemsWithSearch)]
    [RuleRequiredField(CustomMessageTemplate = "작업장유형을 입력하세요.")]
    [XafDisplayName("작업장유형"), ToolTip("작업장유형")]
    public UniversalMinorCode WorkCenterType
    {
        get { return GetPropertyValue<UniversalMinorCode>(nameof(WorkCenterType)); }
        set
        { SetPropertyValue(nameof(WorkCenterType), value); }
    }

    [VisibleInLookupListView(true)]
    [DataSourceCriteria("UniversalMajorCodeObject.MajorCode == 'WorkCenterStatus' AND IsEnabled == True")]
    [ModelDefault("LookupProperty", nameof(UniversalMinorCode.CodeName))]
    [LookupEditorMode(LookupEditorMode.AllItemsWithSearch)]
    [RuleRequiredField(CustomMessageTemplate = "작업장상태를 입력하세요.")]
    [XafDisplayName("작업장상태"), ToolTip("작업장상태")]
    public UniversalMinorCode WorkCenterStatus
    {
        get { return GetPropertyValue<UniversalMinorCode>(nameof(WorkCenterStatus)); }
        set
        { SetPropertyValue(nameof(WorkCenterStatus), value); }
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
    public WorkCenter(Session session) : base(session) { }
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