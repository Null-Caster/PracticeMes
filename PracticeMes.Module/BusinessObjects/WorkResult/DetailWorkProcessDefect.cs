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
using PracticeMes.Module.BusinessObjects.BaseInfo.QualityInfo;
using PracticeMes.Module.BusinessObjects.Inspect;

namespace PracticeMes.Module.BusinessObjects.WorkResult;

[DefaultClassOptions]
[NavigationItem(false)]
[XafDisplayName("공정 불량 상세 등록")]
[DefaultListViewOptions(MasterDetailMode.ListViewOnly, false, NewItemRowPosition.Top)]
public class DetailWorkProcessDefect : BaseObject
{
    #region Properties
    [VisibleInLookupListView(true)]
    [ModelDefault("LookupProperty", nameof(DefectCause.DefectCauseName))]
    [LookupEditorMode(LookupEditorMode.AllItemsWithSearch)]
    [RuleRequiredField(CustomMessageTemplate = "불량 원인을 입력하세요.")]
    [XafDisplayName("불량 원인"), ToolTip("불량 원인")]
    public DefectCause DefectCauseObject
    {
        get { return GetPropertyValue<DefectCause>(nameof(DefectCauseObject)); }
        set { SetPropertyValue(nameof(DefectCauseObject), value); }
    }

    [VisibleInLookupListView(true)]
    [ModelDefault("LookupProperty", nameof(DefectType.DefectTypeName))]
    [LookupEditorMode(LookupEditorMode.AllItemsWithSearch)]
    [RuleRequiredField(CustomMessageTemplate = "불량 유형을 입력하세요.")]
    [XafDisplayName("불량 유형"), ToolTip("불량 유형")]
    public DefectType DefectTypeObject
    {
        get { return GetPropertyValue<DefectType>(nameof(DefectTypeObject)); }
        set { SetPropertyValue(nameof(DefectTypeObject), value); }
    }

    [VisibleInLookupListView(true)]
    [ModelDefault("DisplayFormat", "yyyy/MM/dd")]
    [RuleRequiredField(CustomMessageTemplate = "작업 일시를 입력하세요.")]
    [XafDisplayName("작업 일시"), ToolTip("작업 일시")]
    public DateTime WorkResultDateTime
    {
        get { return GetPropertyValue<DateTime>(nameof(WorkResultDateTime)); }
        set { SetPropertyValue(nameof(WorkResultDateTime), value); }
    }

    [VisibleInLookupListView(true)]
    [ModelDefault("EditMask", "###,###,###,###,###,###,###,###,###,##0.##")]
    [XafDisplayName("불량수량"), ToolTip("불량수량")]
    public int DefectQuantity
    {
        get { return GetPropertyValue<int>(nameof(DefectQuantity)); }
        set { SetPropertyValue(nameof(DefectQuantity), value); }
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

    [Association(@"DetailWorkProcessDefectObjectRefernecesMasterWorkProcessDefect")]
    public MasterWorkProcessDefect MasterWorkProcessDefectObject
    {
        get { return GetPropertyValue<MasterWorkProcessDefect>(nameof(MasterWorkProcessDefectObject)); }
        set { SetDelayedPropertyValue(nameof(MasterWorkProcessDefectObject), value); }
    }
    #endregion

    #region Constructors
    public DetailWorkProcessDefect(Session session) : base(session) { }
    #endregion

    #region Methods
    public override void AfterConstruction()
    {
        base.AfterConstruction();

        WorkResultDateTime = DateTime.Now;
        CreatedDateTime = DateTime.Now;
    }
    #endregion
}