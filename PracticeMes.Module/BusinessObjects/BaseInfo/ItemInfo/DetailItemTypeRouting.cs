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

namespace PracticeMes.Module.BusinessObjects.BaseInfo.ItemInfo
{
    [DefaultClassOptions]
    public class DetailItemTypeRouting : BaseObject
    {
        #region Properties
        [VisibleInLookupListView(true)]
        [XafDisplayName("Routing 순번"), ToolTip("Routing 순번")]
        public int RoutingIndex
        {
            get { return GetPropertyValue<int>(nameof(RoutingIndex)); }
            set { SetPropertyValue(nameof(RoutingIndex), value); }
        }

        [VisibleInLookupListView(true)]
        [DataSourceCriteria("IsEnabled == True")]
        [ModelDefault("LookupProperty", nameof(WorkCenter.WorkCenterName))]
        [LookupEditorMode(LookupEditorMode.AllItemsWithSearch)]
        [RuleRequiredField(CustomMessageTemplate = "작업장 이름을 입력하세요.")]
        [XafDisplayName("작업장 이름"), ToolTip("작업장 이름")]
        public WorkCenter WorkCenterObject
        {
            get { return GetPropertyValue<WorkCenter>(nameof(WorkCenterObject)); }
            set { SetPropertyValue(nameof(WorkCenterObject), value); }
        }

        [VisibleInLookupListView(true)]
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
        [XafDisplayName("최종 작업 공정 여부"), ToolTip("최종 작업 공정 여부")]
        public bool IsFinalWorkProcess
        {
            get { return GetPropertyValue<bool>(nameof(IsFinalWorkProcess)); }
            set { SetPropertyValue(nameof(IsFinalWorkProcess), value); }
        }

        [VisibleInLookupListView(true)]
        [XafDisplayName("활성화 여부"), ToolTip("활성화 여부")]
        public bool IsEnabled
        {
            get { return GetPropertyValue<bool>(nameof(IsEnabled)); }
            set { SetPropertyValue(nameof(IsEnabled), value); }
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

        [Browsable(false)]
        [Association(@"DetailItemTypeRoutingReferenceMasterItemTypeRouting")]
        public MasterItemTypeRouting MasterItemTypeRoutingObject
        {
            get { return GetPropertyValue<MasterItemTypeRouting>(nameof(MasterItemTypeRoutingObject)); }
            set { SetPropertyValue<MasterItemTypeRouting>(nameof(MasterItemTypeRoutingObject), value); }
        }
        #endregion

        #region Constructors
        public DetailItemTypeRouting(Session session) : base(session) { }
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
}