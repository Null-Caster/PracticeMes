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
[NavigationItem("생산 정보"), XafDisplayName("작업 공정 등록")]
[DefaultListViewOptions(MasterDetailMode.ListViewOnly, true, NewItemRowPosition.Top)]
public class WorkProcess : BaseObject
{
    #region Properties
    [VisibleInLookupListView(true)]
    [RuleUniqueValue(CustomMessageTemplate = "작업 공정 코드가 중복되었습니다.")]
    [RuleRequiredField(CustomMessageTemplate = "작업 공정 코드를 입력하세요.")]
    [XafDisplayName("작업 공정 코드"), ToolTip("작업 공정 코드")]
    public string WorkProcessCode
    {
        get { return GetPropertyValue<string>(nameof(WorkProcessCode)); }
        set { SetPropertyValue(nameof(WorkProcessCode), value); }
    }

    [VisibleInLookupListView(true)]
    [RuleRequiredField(CustomMessageTemplate = "작업공정이름을 입력하세요.")]
    [XafDisplayName("작업공정이름"), ToolTip("작업공정이름")]
    public string WorkProcessName
    {
        get { return GetPropertyValue<string>(nameof(WorkProcessName)); }
        set { SetPropertyValue(nameof(WorkProcessName), value); }
    }

    [VisibleInLookupListView(true)]
    [DataSourceCriteria("IsEnabled == True")]
    [ImmediatePostData]
    [ModelDefault("LookupProperty", nameof(WorkCenter.WorkCenterName))]
    [LookupEditorMode(LookupEditorMode.AllItemsWithSearch)]
    [RuleRequiredField(CustomMessageTemplate = "작업장을 입력하세요.")]
    [XafDisplayName("작업장"), ToolTip("작업장")]
    public WorkCenter WorkCenterObject
    {
        get { return GetPropertyValue<WorkCenter>(nameof(WorkCenterObject)); }
        set { SetPropertyValue(nameof(WorkCenterObject), value); }
    }

    [VisibleInLookupListView(true)]
    [ModelDefault("AllowEdit", "False")]
    [XafDisplayName("작업장코드")]
    public string WorkCenterObjectCode
    {
        get { return WorkCenterObject?.WorkCenterCode; }
    }

    [VisibleInLookupListView(true)]
    [RuleUniqueValue(CustomMessageTemplate = "정렬순서가 중복되었습니다.")]
    [XafDisplayName("정렬순서"), ToolTip("정렬순서")]
    public double WorkProcessSortOrder
    {
        get { return GetPropertyValue<double>(nameof(WorkProcessSortOrder)); }
        set { SetPropertyValue(nameof(WorkProcessSortOrder), value); }
    }

    [VisibleInLookupListView(true)]
    [DataSourceCriteria("UniversalMajorCodeObject.MajorCode == 'YESNO' AND IsEnabled == True")]
    [ModelDefault("LookupProperty", nameof(UniversalMinorCode.CodeName))]
    [LookupEditorMode(LookupEditorMode.AllItemsWithSearch)]
    [RuleRequiredField(CustomMessageTemplate = "설비필수여부를 입력하세요.")]
    [XafDisplayName("설비필수여부"), ToolTip("설비필수여부")]
    public UniversalMinorCode EquipmentUsageOption
    {
        get { return GetPropertyValue<UniversalMinorCode>(nameof(EquipmentUsageOption)); }
        set
        { SetPropertyValue(nameof(EquipmentUsageOption), value); }
    }

    [VisibleInLookupListView(true)]
    [DataSourceCriteria("UniversalMajorCodeObject.MajorCode == 'YESNO' AND IsEnabled == True")]
    [ModelDefault("LookupProperty", nameof(UniversalMinorCode.CodeName))]
    [LookupEditorMode(LookupEditorMode.AllItemsWithSearch)]
    [RuleRequiredField(CustomMessageTemplate = "생산작업자등록을 입력하세요.")]
    [XafDisplayName("생산작업자등록"), ToolTip("생산작업자등록")]
    public UniversalMinorCode ProductionWorkerAssignment
    {
        get { return GetPropertyValue<UniversalMinorCode>(nameof(ProductionWorkerAssignment)); }
        set
        { SetPropertyValue(nameof(ProductionWorkerAssignment), value); }
    }

    [VisibleInLookupListView(true)]
    [ModelDefault("LookupProperty", nameof(UniversalMinorCode.CodeName))]
    [LookupEditorMode(LookupEditorMode.AllItemsWithSearch)]
    [XafDisplayName("생산실적입력"), ToolTip("생산실적입력")]
    public bool IsProductionResultEnabled
    {
        get { return GetPropertyValue<bool>(nameof(IsProductionResultEnabled)); }
        set
        { SetPropertyValue(nameof(IsProductionResultEnabled), value); }
    }

    [VisibleInLookupListView(true)]
    [ModelDefault("LookupProperty", nameof(UniversalMinorCode.CodeName))]
    [LookupEditorMode(LookupEditorMode.AllItemsWithSearch)]
    [XafDisplayName("불량입력"), ToolTip("불량입력")]
    public bool IsDefectInputEnabled
    {
        get { return GetPropertyValue<bool>(nameof(IsDefectInputEnabled)); }
        set
        { SetPropertyValue(nameof(IsDefectInputEnabled), value); }
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
    public WorkProcess(Session session) : base(session) { }
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