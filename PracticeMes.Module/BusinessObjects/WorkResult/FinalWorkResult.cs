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
using PracticeMes.Module.BusinessObjects.LotManagement;
using PracticeMes.Module.BusinessObjects.ProductPlanning;

namespace PracticeMes.Module.BusinessObjects.WorkResult;

[DefaultClassOptions]
[NavigationItem("공정 관리"), XafDisplayName("최종 공정 실적 등록")]
[DefaultListViewOptions(MasterDetailMode.ListViewOnly, true, NewItemRowPosition.Top)]
public class FinalWorkResult : BaseObject
{
    #region Properties
    [VisibleInLookupListView(true)]
    [ImmediatePostData(true)]
    [DataSourceProperty(nameof(AvailableFinalWorkInstructions))]
    [ModelDefault("LookupProperty", nameof(DetailWorkInstruction.WorkInstructionNumber))]
    [LookupEditorMode(LookupEditorMode.AllItemsWithSearch)]
    [RuleRequiredField(CustomMessageTemplate = "작업 지시 번호를 입력하세요.")]
    [XafDisplayName("작업 지시 번호"), ToolTip("작업 지시 번호")]
    public DetailWorkInstruction DetailWorkInstructionObject
    {
        get { return GetPropertyValue<DetailWorkInstruction>(nameof(DetailWorkInstructionObject)); }
        set { SetPropertyValue(nameof(DetailWorkInstructionObject), value); }
    }

    [Browsable(false)]
    public List<DetailWorkInstruction> AvailableFinalWorkInstructions
    {
        get
        {
            var allInstructions = new XPCollection<DetailWorkInstruction>(this.Session)
                .Where(x => x.IsFinalWorkProcess == true && x.Progress?.CodeName == "진행중")
                .ToList();

            return allInstructions;
        }
    }

    [ModelDefault("AllowEdit", "False")]
    [ModelDefault("LookupProperty", nameof(Lot.LotNumber))]
    [LookupEditorMode(LookupEditorMode.AllItemsWithSearch)]
    [RuleUniqueValue(CustomMessageTemplate = "Lot 번호가 중복되었습니다.")]
    [XafDisplayName("Lot 번호"), ToolTip("Lot 번호")]
    public Lot LotObject
    {
        get { return GetPropertyValue<Lot>(nameof(LotObject)); }
        set { SetPropertyValue(nameof(LotObject), value); }
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
    [XafDisplayName("생산담당자"), ToolTip("생산담당자")]
    public Employee EmployeeObject
    {
        get { return GetPropertyValue<Employee>(nameof(EmployeeObject)); }
        set { SetPropertyValue(nameof(EmployeeObject), value); }
    }

    [VisibleInLookupListView(true)]
    [ModelDefault("DisplayFormat", "yyyy/MM/dd")]
    [RuleRequiredField(CustomMessageTemplate = "작업 일시를 입력하세요.")]
    [XafDisplayName("작업 일시"), ToolTip("작업 일시")]
    public DateTime WorkDateTime
    {
        get { return GetPropertyValue<DateTime>(nameof(WorkDateTime)); }
        set { SetPropertyValue(nameof(WorkDateTime), value); }
    }

    [VisibleInLookupListView(true)]
    [ModelDefault("EditMask", "###,###,###,###,###,###,###,###,###,##0.###")]
    [XafDisplayName("작업 지시 수량"), ToolTip("작업 지시 수량")]
    public double WorkInstructionQuantity
    {
        get { return DetailWorkInstructionObject?.MasterWorkInstructionObject?.WorkInstructionQuantity ?? 0; }
    }

    [VisibleInLookupListView(true)]
    [XafDisplayName("생산 가능 수량"), ToolTip("생산 가능 수량")]
    public double AvailableGoodQuantity
    {
        get
        {
            if (DetailWorkInstructionObject?.MasterWorkInstructionObject?.MasterProductionPlanningObject == null)
                return 0;

            var planningOid = DetailWorkInstructionObject.MasterWorkInstructionObject.MasterProductionPlanningObject.Oid;

            var middleResult = new XPCollection<MiddleWorkResult>(this.Session)
                .Where(x => x.DetailWorkInstructionObject != null &&
                            x.DetailWorkInstructionObject.MasterWorkInstructionObject.MasterProductionPlanningObject.Oid == planningOid &&
                            x.DetailWorkInstructionObject.IsFinalWorkProcess == false)
                .OrderByDescending(x => x.DetailWorkInstructionObject.RoutingIndex)
                .FirstOrDefault();

            if (middleResult?.GoodQuantity == null)
                return 0;

            return middleResult.GoodQuantity;
        }
    }

    [VisibleInLookupListView(true)]
    [ModelDefault("EditMask", "###,###,###,###,###,###,###,###,###,##0.###")]
    [XafDisplayName("생산 수량"), ToolTip("생산 수량")]
    public double CreateQuantity
    {
        get { return GetPropertyValue<double>(nameof(CreateQuantity)); }
        set { SetPropertyValue(nameof(CreateQuantity), value); }
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
    #endregion

    #region Constructors
    public FinalWorkResult(Session session) : base(session) { }
    #endregion

    #region Methods
    public override void AfterConstruction()
    {
        base.AfterConstruction();

        CreatedDateTime = DateTime.Now;
        WorkDateTime = DateTime.Now;
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