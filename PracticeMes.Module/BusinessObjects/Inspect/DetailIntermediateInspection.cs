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

namespace PracticeMes.Module.BusinessObjects.Inspect;

[DefaultClassOptions]
public class DetailIntermediateInspection : BaseObject
{
    #region Properties
    [VisibleInLookupListView(true)]
    [PersistentAlias("PadLeft(ToStr(SequentialNumber), 8,'0')")]
    [XafDisplayName("검사번호"), ToolTip("검사번호")]
    public string InspectionNumber
    {
        get { return Convert.ToString(EvaluateAlias(nameof(InspectionNumber))); }
    }

    [VisibleInLookupListView(true)]
    [XafDisplayName("시리얼번호"), ToolTip("시리얼번호")]
    public string SerialNumber
    {
        get { return GetPropertyValue<string>(nameof(SerialNumber)); }
        set { SetPropertyValue(nameof(SerialNumber), value); }
    }

    [VisibleInLookupListView(true)]
    [XafDisplayName("검사일시"), ToolTip("검사일시")]
    public DateTime InspectionDateTime
    {
        get { return GetPropertyValue<DateTime>(nameof(InspectionDateTime)); }
        set { SetPropertyValue(nameof(InspectionDateTime), value); }
    }

    [VisibleInLookupListView(true)]
    [ModelDefault("AllowEdit", "false")]
    [ModelDefault("EditMask", "###,###,###,###,###,###,###,###,###,##0.##")]
    [XafDisplayName("검사요청수량"), ToolTip("검사요청수량")]
    public double InspectionRequestQuantity
    {
        get { return GetPropertyValue<double>(nameof(InspectionRequestQuantity)); }
        set { SetPropertyValue(nameof(InspectionRequestQuantity), value); }
    }

    [VisibleInLookupListView(true)]
    [ModelDefault("EditMask", "###,###,###,###,###,###,###,###,###,##0.##")]
    [XafDisplayName("검사수량"), ToolTip("검사수량")]
    public int InspectionQuantity
    {
        get { return DefectQuantity; }
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
    [DataSourceCriteria("IsEnabled == True")]
    [LookupEditorMode(LookupEditorMode.AllItemsWithSearch)]
    [ModelDefault("LookupProperty", nameof(Employee.EmployeeName))]
    [XafDisplayName("검사자"), ToolTip("검사자")]
    public Employee Inspector
    {
        get { return GetPropertyValue<Employee>(nameof(Inspector)); }
        set { SetPropertyValue(nameof(Inspector), value); }
    }

    [VisibleInLookupListView(true)]
    [XafDisplayName("비고"), ToolTip("비고")]
    public string Remark
    {
        get { return GetPropertyValue<string>(nameof(Remark)); }
        set { SetPropertyValue(nameof(Remark), value); }
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

    [Association(@"DetailIntermediateInspectionObjectRefernecesMasterIntermediateInspection")]
    public MasterIntermediateInspection MasterIntermediateInspectionObject
    {
        get { return GetPropertyValue<MasterIntermediateInspection>(nameof(MasterIntermediateInspectionObject)); }
        set { SetDelayedPropertyValue(nameof(MasterIntermediateInspectionObject), value); }
    }
    #endregion

    #region Constructors
    public DetailIntermediateInspection(Session session) : base(session) { }
    #endregion

    #region Methods
    public override void AfterConstruction()
    {
        base.AfterConstruction();
        CreatedDateTime = DateTime.Now;
    }
    #endregion
}