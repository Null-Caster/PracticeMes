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
using PracticeMes.Module.BusinessObjects.BaseInfo.ProductionInfo;

namespace PracticeMes.Module.BusinessObjects.ProductPlanning
{
    [DefaultClassOptions]
    [NavigationItem("공정 관리"), XafDisplayName("작업 지시 상태 변경")]
    [DefaultListViewOptions(MasterDetailMode.ListViewOnly, true, NewItemRowPosition.None)]
    public class DetailWorkInstruction : BaseObject
    {
        #region Properties
        [VisibleInLookupListView(true)]
        [ModelDefault("AllowEdit", "False")]
        [XafDisplayName("작업 지시 번호"), ToolTip("작업 지시 번호")]
        public string WorkInstructionNumber
        {
            get { return GetPropertyValue<string>(nameof(WorkInstructionNumber)); }
            set { SetPropertyValue(nameof(WorkInstructionNumber), value); }
        }

        [VisibleInLookupListView(true)]
        [ModelDefault("AllowEdit", "False")]
        [DataSourceCriteria("IsEnabled == True")]
        [ModelDefault("LookupProperty", nameof(WorkProcess.WorkProcessName))]
        [LookupEditorMode(LookupEditorMode.AllItemsWithSearch)]
        [RuleRequiredField(CustomMessageTemplate = "작업 공정 이름을 입력하세요.")]
        [XafDisplayName("작업 공정 이름"), ToolTip("작업 공정 이름")]
        public WorkProcess WorkProcessObject
        {
            get { return GetPropertyValue<WorkProcess>(nameof(WorkProcessObject)); }
            set { SetPropertyValue(nameof(WorkProcessObject), value); }
        }

        [VisibleInLookupListView(true)]
        [ModelDefault("AllowEdit", "False")]
        [XafDisplayName("공정 순번"), ToolTip("공정 순번")]
        public int RoutingIndex
        {
            get { return GetPropertyValue<int>(nameof(RoutingIndex)); }
            set { SetPropertyValue(nameof(RoutingIndex), value); }
        }

        [VisibleInLookupListView(true)]
        [ModelDefault("AllowEdit", "False")]
        [DataSourceCriteria("IsEnabled == True")]
        [ModelDefault("LookupProperty", nameof(WorkLine.WorkLineName))]
        [LookupEditorMode(LookupEditorMode.AllItemsWithSearch)]
        [XafDisplayName("작업 라인 이름"), ToolTip("작업 라인 이름")]
        public WorkLine WorkLineObject
        {
            get { return GetPropertyValue<WorkLine>(nameof(WorkLineObject)); }
            set { SetPropertyValue(nameof(WorkLineObject), value); }
        }

        [VisibleInLookupListView(true)]
        [ModelDefault("EditMask", "###,###,###,###,###,###,###,###,###,##0.###")]
        [XafDisplayName("작업 지시 수량"), ToolTip("작업 지시 수량")]
        public double WorkInstructionQuantity
        {
            get { return GetPropertyValue<double>(nameof(WorkInstructionQuantity)); }
            set { SetPropertyValue(nameof(WorkInstructionQuantity), value); }
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
        [ModelDefault("DisplayFormat", "yyyy/MM/dd")]
        [RuleRequiredField(CustomMessageTemplate = "작업예정일 입력하세요.")]
        [XafDisplayName("작업예정일"), ToolTip("작업예정일")]
        public DateTime WokrStartDateTime
        {
            get { return GetPropertyValue<DateTime>(nameof(WokrStartDateTime)); }
            set { SetPropertyValue(nameof(WokrStartDateTime), value); }
        }

        [VisibleInLookupListView(true)]
        [DataSourceCriteria("UniversalMajorCodeObject.MajorCode == 'Progress' AND IsEnabled == True")]
        [ModelDefault("LookupProperty", nameof(UniversalMinorCode.CodeName))]
        [LookupEditorMode(LookupEditorMode.AllItemsWithSearch)]
        [RuleRequiredField(CustomMessageTemplate = "작업 지시 상태를 입력하세요.")]
        [XafDisplayName("작업 지시 상태"), ToolTip("작업 지시 상태")]
        public UniversalMinorCode Progress
        {
            get { return GetPropertyValue<UniversalMinorCode>(nameof(Progress)); }
            set { SetPropertyValue(nameof(Progress), value); }
        }

        [VisibleInLookupListView(true)]
        [XafDisplayName("최종 작업 공정 여부"), ToolTip("최종 작업 공정 여부")]
        public bool IsFinalWorkProcess
        {
            get { return GetPropertyValue<bool>(nameof(IsFinalWorkProcess)); }
            set { SetPropertyValue(nameof(IsFinalWorkProcess), value); }
        }

        [VisibleInLookupListView(true)]
        [XafDisplayName("투입인원"), ToolTip("투입인원")]
        public int PutWorker
        {
            get { return GetPropertyValue<int>(nameof(PutWorker)); }
            set { SetPropertyValue(nameof(PutWorker), value); }
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
        [ModelDefault("DisplayFormat", "yyyy/MM/dd HH:mm:ss")]
        [ModelDefault("EditMask", "yyyy/MM/dd HH:mm:ss")]
        [XafDisplayName("생성 일시"), ToolTip("항목이 생성된 일시입니다.")]
        public DateTime CreatedDateTime
        {
            get { return GetPropertyValue<DateTime>(nameof(CreatedDateTime)); }
            set { SetPropertyValue(nameof(CreatedDateTime), value); }
        }

        // 아래 쪽에 존재하는 작업 지시 상세
        [Association(@"DetailWorkInstructionReferencesMasterWrokInstruction")]
        [Browsable(false)]
        public MasterWorkInstruction MasterWorkInstructionObject
        {
            get { return GetPropertyValue<MasterWorkInstruction>(nameof(MasterWorkInstructionObject)); }
            set { SetPropertyValue<MasterWorkInstruction>(nameof(MasterWorkInstructionObject), value); }
        }
        #endregion

        #region Constructors
        #endregion

        #region Methods
        #endregion
        public DetailWorkInstruction(Session session) : base(session) { }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
        }
    }
}