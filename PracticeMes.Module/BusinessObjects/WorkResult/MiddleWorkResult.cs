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
using PracticeMes.Module.BusinessObjects.Meterial;
using PracticeMes.Module.BusinessObjects.ProductPlanning;

namespace PracticeMes.Module.BusinessObjects.WorkResult;

[DefaultClassOptions]
[NavigationItem("공정 관리"), XafDisplayName("중간 공정 실적 등록")]
[DefaultListViewOptions(MasterDetailMode.ListViewOnly, true, NewItemRowPosition.Top)]
public class MiddleWorkResult : BaseObject
{
    #region Properties
    [ImmediatePostData(true)]
    [VisibleInLookupListView(true)]
    [DataSourceProperty(nameof(AvailableInstructionObjects))]
    [ModelDefault("LookupProperty", nameof(DetailWorkInstruction.WorkInstructionNumber))]
    [LookupEditorMode(LookupEditorMode.AllItemsWithSearch)]
    [RuleRequiredField(CustomMessageTemplate = "작업 지시 번호를 입력하세요.")]
    [XafDisplayName("작업 지시 번호"), ToolTip("작업 지시 번호")]
    public DetailWorkInstruction DetailWorkInstructionObject
    {
        get { return GetPropertyValue<DetailWorkInstruction>(nameof(DetailWorkInstructionObject)); }
        set { SetPropertyValue(nameof(DetailWorkInstructionObject), value); }
    }

    // 작업 가능한 상세 작업지시 목록 반환
    // 조건: 상태가 Proceeding이고, 마지막 공정이 아닌 항목
    [Browsable(false)]
    public List<DetailWorkInstruction> AvailableInstructionObjects
    {
        get
        {
            var allInstructionsFull = new XPCollection<DetailWorkInstruction>(this.Session).ToList();

            // 진행중, 중간공정
            var currentInstructions = allInstructionsFull
                .Where(x => x.Progress?.MinorCode == "Proceeding" && x.IsFinalWorkProcess == false)
                .ToList();

            // 원자재 투입이 이루어진 작업 지시
            var usedInstructionNumbers = new XPCollection<MaterialInputResult>(this.Session)
                .Where(x => x.DetailWorkInstructionObject != null &&
                           x.DetailWorkInstructionObject.MasterWorkInstructionObject.MasterProductionPlanningObject != null)
                .Select(x => x.DetailWorkInstructionObject.WorkInstructionNumber)
                .Where(x => !string.IsNullOrEmpty(x))
                .Distinct()
                .ToList();

            var filtered = currentInstructions
                .Where(x => !string.IsNullOrEmpty(x.WorkInstructionNumber) &&
                            usedInstructionNumbers.Contains(x.WorkInstructionNumber))
                .ToList();

            return filtered;
        }
    }

    [VisibleInLookupListView(true)]
    [XafDisplayName("작업 공정 이름"), ToolTip("작업 공정 이름")]
    public string WorkProcessName
    {
        get { return DetailWorkInstructionObject?.WorkProcessObject.WorkProcessName; }
    }

    [VisibleInLookupListView(true)]
    [XafDisplayName("품목코드"), ToolTip("품목코드")]
    public string ItemCode
    {
        get { return DetailWorkInstructionObject?.MasterWorkInstructionObject?.MasterProductionPlanningObject?.ItemObject?.ItemCode; }
    }

    [VisibleInLookupListView(true)]
    [XafDisplayName("품목명칭"), ToolTip("품목명칭")]
    public string ItemName
    {
        get { return DetailWorkInstructionObject?.MasterWorkInstructionObject?.MasterProductionPlanningObject?.ItemObject?.ItemName; }
    }

    [VisibleInLookupListView(true)]
    [XafDisplayName("공장"), ToolTip("공장")]
    public string FactoryName
    {
        get { return DetailWorkInstructionObject?.MasterWorkInstructionObject?.MasterProductionPlanningObject?.FactoryObject?.FactoryName; }
    }

    [VisibleInLookupListView(true)]
    [DataSourceCriteria("IsEnabled == True")]
    [ModelDefault("LookupProperty", nameof(WareHouse.WareHouseName))]
    [LookupEditorMode(LookupEditorMode.AllItemsWithSearch)]
    [RuleRequiredField(CustomMessageTemplate = "입고 창고를 입력하세요.")]
    [XafDisplayName("입고 창고"), ToolTip("입고 창고")]
    public WareHouse WareHouseObject
    {
        get { return GetPropertyValue<WareHouse>(nameof(WareHouseObject)); }
        set { SetPropertyValue(nameof(WareHouseObject), value); }
    }

    [VisibleInLookupListView(true)]
    [ModelDefault("LookupProperty", nameof(Employee.EmployeeName))]
    [LookupEditorMode(LookupEditorMode.AllItemsWithSearch)]
    [RuleRequiredField(CustomMessageTemplate = "작업자를 입력하세요.")]
    [XafDisplayName("작업자"), ToolTip("작업자")]
    public Employee EmployeeObject
    {
        get { return GetPropertyValue<Employee>(nameof(EmployeeObject)); }
        set { SetPropertyValue(nameof(EmployeeObject), value); }
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

    [Browsable(false)]
    [VisibleInLookupListView(true)]
    [ModelDefault("EditMask", "###,###,###,###,###,###,###,###,###,##0.###")]
    [XafDisplayName("작업 지시 수량"), ToolTip("작업 지시 수량")]
    public double WorkInstructionQuantity
    {
        get { return DetailWorkInstructionObject?.MasterWorkInstructionObject?.WorkInstructionQuantity ?? 0; }
    }

    [VisibleInLookupListView(true)]
    [ModelDefault("EditMask", "###,###,###,###,###,###,###,###,###,##0.###")]
    [XafDisplayName("작업 상세 지시 수량"), ToolTip("작업 상세 지시 수량")]
    public double DetailWorkInstructionQuantity
    {
        get { return DetailWorkInstructionObject?.WorkInstructionQuantity ?? 0; }
    }

    [VisibleInLookupListView(true)]
    [ModelDefault("EditMask", "###,###,###,###,###,###,###,###,###,##0.###")]
    [XafDisplayName("생산 수량"), ToolTip("생산 수량")]
    public double GoodQuantity
    {
        get { return GetPropertyValue<double>(nameof(GoodQuantity)); }
        set { SetPropertyValue(nameof(GoodQuantity), value); }
    }

    //[VisibleInLookupListView(true)]
    //[ModelDefault("EditMask", "###,###,###,###,###,###,###,###,###,##0.###")]
    //[XafDisplayName("생산 가능 수량"), ToolTip("생산 가능 수량")]
    //public double AvailableGoodQuantity
    //{
    //    get
    //    {
    //    }
    //}

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

    #endregion

    #region Constructors
    public MiddleWorkResult(Session session) : base(session) { }
    #endregion

    #region Methods
    public override void AfterConstruction()
    {
        base.AfterConstruction();
        WorkResultDateTime = DateTime.Now;
        CreatedDateTime = DateTime.Now;
    }

    protected override void OnChanged(string propertyName, object oldValue, object newValue)
    {
        base.OnChanged(propertyName, oldValue, newValue);
        if (Session.IsObjectsLoading)
        {
            return;
        }
        switch (propertyName)
        {
            case nameof(DetailWorkInstructionObject):
                if (DetailWorkInstructionObject is null || DetailWorkInstructionObject.Oid == Guid.Empty)
                {
                    return;
                }
                EmployeeObject = DetailWorkInstructionObject.EmployeeObject;
                WareHouseObject = DetailWorkInstructionObject.WareHouseObject;
                break;
            default:
                break;
        }
    }
    #endregion
}