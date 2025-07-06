using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using PracticeMes.Module.BusinessObjects.BaseInfo.ItemInfo;
using PracticeMes.Module.BusinessObjects.Inspect;
using PracticeMes.Module.BusinessObjects.Meterial;
using PracticeMes.Module.BusinessObjects.ProductPlanning;

namespace PracticeMes.Module.BusinessObjects.WorkResult
{
    [DefaultClassOptions]
    [NavigationItem("품질 관리"), XafDisplayName("공정 불량 등록")]
    [DefaultListViewOptions(MasterDetailMode.ListViewOnly, true, NewItemRowPosition.None)]
    public class MasterWorkProcessDefect : BaseObject
    {
        #region Properties
        [ImmediatePostData(true)]
        [VisibleInLookupListView(true)]
        [DataSourceProperty(nameof(AvailablePlannings))]
        [ModelDefault("LookupProperty", nameof(MasterProductionPlanning.ProductionPlanningNumber))]
        [LookupEditorMode(LookupEditorMode.AllItemsWithSearch)]
        [RuleRequiredField(CustomMessageTemplate = "생산 계획 번호를 선택하세요.")]
        [XafDisplayName("생산 계획 번호")]
        public MasterProductionPlanning MasterProductionPlanningObject
        {
            get => GetPropertyValue<MasterProductionPlanning>(nameof(MasterProductionPlanningObject));
            set => SetPropertyValue(nameof(MasterProductionPlanningObject), value);
        }

        [Browsable(false)]
        public List<MasterProductionPlanning> AvailablePlannings
        {
            get
            {
                // 최종 공정이 등록된 작업지시들의 Oid 리스트
                var usedInstructionOids = new XPCollection<FinalWorkResult>(Session)
                    .Where(x => x.DetailWorkInstructionObject != null)
                    .Select(x => x.DetailWorkInstructionObject.Oid)
                    .Distinct()
                    .ToList();

                // 제외할 생산계획 Oid 목록 (최종공정 등록된 작업지시가 포함된 계획)
                var excludedPlanningOids = new XPCollection<DetailWorkInstruction>(Session)
                    .Where(x => usedInstructionOids.Contains(x.Oid))
                    .Select(x => x.MasterWorkInstructionObject.MasterProductionPlanningObject.Oid)
                    .Distinct()
                    .ToList();

                // 전체 생산계획 중에서 제외 목록에 없는 것만 필터링
                return new XPCollection<MasterProductionPlanning>(Session)
                    .Where(p => !excludedPlanningOids.Contains(p.Oid))
                    .ToList();
            }
        }

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

        [Browsable(false)]
        public List<DetailWorkInstruction> AvailableInstructionObjects
        {
            get
            {
                if (MasterProductionPlanningObject == null)
                    return new List<DetailWorkInstruction>();

                // 모든 작업 지시 상세 조회
                var allInstructions = new XPCollection<DetailWorkInstruction>(this.Session)
                    .Where(x => x.MasterWorkInstructionObject.MasterProductionPlanningObject.Oid == MasterProductionPlanningObject.Oid)
                    .ToList();

                //var alreadyUsed = new XPCollection<MasterWorkProcessDefect>(this.Session)
                //    .Select(x => x.DetailWorkInstructionObject?.Oid)
                //    .Distinct()
                //    .ToList();

                //var currentInstructions = allInstructions
                //    .Where(x => x.Progress?.MinorCode == "Proceeding"
                //                && x.IsFinalWorkProcess == false
                //                && !alreadyUsed.Contains(x.Oid))
                //    .ToList();

                // "진행중", 최종 공정이 아닌거 필터
                var currentInstructions = allInstructions
                    .Where(x => x.Progress?.MinorCode == "Proceeding" && x.IsFinalWorkProcess == false)
                    .ToList();

                if (!currentInstructions.Any())
                    return new List<DetailWorkInstruction>();

                int maxRoutingIndex = currentInstructions.Max(x => x.RoutingIndex);

                var result = currentInstructions
                    .Where(x => x.RoutingIndex == maxRoutingIndex)
                    .ToList();

                return result;
            }
        }

        [VisibleInLookupListView(true)]
        [XafDisplayName("공정 명칭"), ToolTip("공정 명칭")]
        public string WorkProcessName
        {
            get { return DetailWorkInstructionObject?.WorkProcessObject?.WorkProcessName; }
        }

        [VisibleInLookupListView(true)]
        [ModelDefault("EditMask", "###,###,###,###,###,###,###,###,###,##0.##")]
        [XafDisplayName("입력 가능 수량"), ToolTip("입력 가능 수량")]
        public double ChipQuantity
        {
            get
            {
                if (DetailWorkInstructionObject == null || Session == null || IsDeleted)
                    return 0;

                //// 중간 공정 실적 찾기
                var middleResult = new XPCollection<MiddleWorkResult>(this.Session)
                    .Where(x => x.DetailWorkInstructionObject.Oid == DetailWorkInstructionObject.Oid)
                    .SingleOrDefault();

                //double goodQty = middleResult?.GoodQuantity ?? 0;

                //// 현재 작업 지시에 대해 등록된 모든 불량 수량 합산
                //double defectQty = new XPCollection<DetailWorkProcessDefect>(this.Session)
                //    .Where(x =>
                //        x.MasterWorkProcessDefectObject != null &&
                //        x.MasterWorkProcessDefectObject.DetailWorkInstructionObject != null &&
                //        x.MasterWorkProcessDefectObject.DetailWorkInstructionObject.Oid == DetailWorkInstructionObject.Oid
                //    )
                //    .Sum(x => x.DefectQuantity);

                //return goodQty - defectQty;

                return middleResult?.GoodQuantity ?? 0;
            }
        }

        [VisibleInLookupListView(true)]
        [ModelDefault("EditMask", "###,###,###,###,###,###,###,###,###,##0.##")]
        [XafDisplayName("총 불량수량"), ToolTip("해당 작업 지시에 등록된 전체 불량 수량")]
        public double TotalDefectQuantity
        {
            get
            {
                // 작업지시가 null이면 불량 합계도 0
                if (DetailWorkInstructionObject == null || Session == null || IsDeleted)
                    return 0;

                try
                {
                    return new XPCollection<DetailWorkProcessDefect>(Session)
                        .Where(x =>
                            x.MasterWorkProcessDefectObject != null &&
                            x.MasterWorkProcessDefectObject.DetailWorkInstructionObject != null &&
                            x.MasterWorkProcessDefectObject.DetailWorkInstructionObject.Oid == DetailWorkInstructionObject.Oid
                        )
                        .Sum(x => x.DefectQuantity);
                }
                catch
                {
                    return 0;
                }
            }
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

        [XafDisplayName("공정 불량 상세")]
        [Association(@"DetailWorkProcessDefectObjectRefernecesMasterWorkProcessDefect"), DevExpress.Xpo.Aggregated]
        public XPCollection<DetailWorkProcessDefect> DetailWorkProcessDefectObject { get { return GetCollection<DetailWorkProcessDefect>(nameof(DetailWorkProcessDefectObject)); } }
        #endregion

        #region Constructors
        public MasterWorkProcessDefect(Session session) : base(session) { }
        #endregion

        #region Methods
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            CreatedDateTime = DateTime.Now;
        }

        #endregion
    }
}