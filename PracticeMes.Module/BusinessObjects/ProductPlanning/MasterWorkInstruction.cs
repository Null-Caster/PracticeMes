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
using PracticeMes.Module.BusinessObjects.BaseInfo.ItemInfo;
using PracticeMes.Module.Common;

namespace PracticeMes.Module.BusinessObjects.ProductPlanning
{
    [DefaultClassOptions]
    [NavigationItem("생산 계획 관리"), XafDisplayName("작업 지시 등록")]
    [DefaultListViewOptions(MasterDetailMode.ListViewAndDetailView, true, NewItemRowPosition.Top)]
    public class MasterWorkInstruction : BaseObject
    {
        #region Properties
        [ImmediatePostData(true)]
        [VisibleInLookupListView(true)]
        [ModelDefault("LookupProperty", nameof(MasterProductionPlanning.ProductionPlanningNumber))]
        [LookupEditorMode(LookupEditorMode.AllItemsWithSearch)]
        [RuleRequiredField(CustomMessageTemplate = "생산 계획 번호를 입력하세요.")]
        [XafDisplayName("생산 계획 번호"), ToolTip("생산 계획 번호")]
        public MasterProductionPlanning MasterProductionPlanningObject
        {
            get { return GetPropertyValue<MasterProductionPlanning>(nameof(MasterProductionPlanningObject)); }
            set { SetPropertyValue(nameof(MasterProductionPlanningObject), value); }
        }

        [VisibleInLookupListView(true)]
        [ModelDefault("LookupProperty", nameof(BusinessPartner.BusinessPartnerName))]
        [XafDisplayName("거래처/수주처"), ToolTip("거래처/수주처")]
        public BusinessPartner BizPartnerObject
        {
            get => GetPropertyValue<BusinessPartner>(nameof(BizPartnerObject));
            set => SetPropertyValue(nameof(BizPartnerObject), value);
        }

        [VisibleInLookupListView(true)]
        [ModelDefault("LookupProperty", nameof(Item.ItemName))]
        [XafDisplayName("품목 이름"), ToolTip("품목 이름")]
        public Item ItemObject
        {
            get => GetPropertyValue<Item>(nameof(ItemObject));
            set => SetPropertyValue(nameof(ItemObject), value);
        }

        [VisibleInLookupListView(true)]
        [XafDisplayName("오더수량"), ToolTip("오더수량")]
        public double SalesOrderQuantity
        {
            get => GetPropertyValue<double>(nameof(SalesOrderQuantity));
            set => SetPropertyValue(nameof(SalesOrderQuantity), value);
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
        [ModelDefault("AllowEdit", "False")]
        [ModelDefault("LookupProperty", nameof(UniversalMinorCode.CodeName))]
        [XafDisplayName("지시상태"), ToolTip("지시상태")]
        public UniversalMinorCode Progress
        {
            get
            {
                ProgressCheck pg = new ProgressCheck();
                return pg.FindProgress(this.Session, this, "MasterWorkInstruction");
            }
        }

        [VisibleInLookupListView(true)]
        [XafDisplayName("시작 예정일"), ToolTip("시작 예정일")]
        public DateTime StartDateTime
        {
            get => GetPropertyValue<DateTime>(nameof(StartDateTime));
            set => SetPropertyValue(nameof(StartDateTime), value);
        }

        [VisibleInLookupListView(true)]
        [ModelDefault("AllowEdit", "False")]
        [XafDisplayName("마감"), ToolTip("마감")]
        public bool IsComplete
        {
            get { return GetPropertyValue<bool>(nameof(IsComplete)); }
            set { SetPropertyValue(nameof(IsComplete), value); }
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
        [ModelDefault("DisplayFormat", "yyyy/MM/dsd HH:mm:ss")]
        [ModelDefault("EditMask", "yyyy/MM/dd HH:mm:ss")]
        [XafDisplayName("생성 일시"), ToolTip("항목이 생성된 일시입니다.")]
        public DateTime CreatedDateTime
        {
            get { return GetPropertyValue<DateTime>(nameof(CreatedDateTime)); }
            set { SetPropertyValue(nameof(CreatedDateTime), value); }
        }

        [Association(@"DetailWorkInstructionReferencesMasterWrokInstruction"), DevExpress.Xpo.Aggregated]
        [XafDisplayName("작업 지시 상세")]
        public XPCollection<DetailWorkInstruction> DetailWorkInstructionObjects { get { return GetCollection<DetailWorkInstruction>(nameof(DetailWorkInstructionObjects)); } }
        #endregion

        #region Constructors
        public MasterWorkInstruction(Session session) : base(session) { }
        #endregion

        #region Methods
        public override void AfterConstruction()
        {
            base.AfterConstruction();

            CreatedDateTime = DateTime.Now;
        }

        protected override void OnChanged(string propertyName, object oldValue, object newValue)
        {
            base.OnChanged(propertyName, oldValue, newValue);

            if (Session.IsObjectsLoading) return;

            if (propertyName == nameof(MasterProductionPlanningObject))
            {
                BizPartnerObject = MasterProductionPlanningObject?.BusinessPartnerObject;
                ItemObject = MasterProductionPlanningObject?.ItemObject;
                SalesOrderQuantity = MasterProductionPlanningObject?.ProductPlanningQuantity ?? 0;
                StartDateTime = MasterProductionPlanningObject?.StartDateTime ?? DateTime.Now;
            }
        }
        #endregion

    }
}