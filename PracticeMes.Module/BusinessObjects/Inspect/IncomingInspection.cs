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
using PracticeMes.Module.BusinessObjects.BaseInfo.CommonInfo;
using PracticeMes.Module.BusinessObjects.Purchase;

namespace PracticeMes.Module.BusinessObjects.Inspect
{
    [NavigationItem("품질 관리"), XafDisplayName("수입검사등록")]
    [DefaultClassOptions]
    [DefaultListViewOptions(MasterDetailMode.ListViewOnly, false, NewItemRowPosition.Top)]
    [Persistent(nameof(IncomingInspection))]
    [RuleCriteria(
        "SumMatchesDefectQuantity",
        DefaultContexts.Save,
        "(GoodQuantity + DefectQuantity + Apperance + Figure == 0) OR (DefectQuantity == Apperance + Figure)",
        CustomMessageTemplate = "모든 수량이 0이거나 [외관] [수치]의 합은 [불량수량]과 같아야 합니다.")]
    [RuleCriteria(
        "InspectionRequestMatchesTotal",
        DefaultContexts.Save,
        "(GoodQuantity + DefectQuantity + Apperance + Figure == 0) OR (InspectionRequestQuantity == GoodQuantity + DefectQuantity)",
        CustomMessageTemplate = "모든 수량은 0이거나 [양품수량과] [불량수량]의 합이 [검사요청수량]과 같아야 합니다.")]
    public class IncomingInspection : BaseObject
    {
        #region Properties
        [Browsable(false)]
        [RuleRequiredField(CustomMessageTemplate = "구매입고상세 자료를 입력하세요")]
        [XafDisplayName("구매입고상세 자료"), ToolTip("구매입고상세 자료")]
        public DetailPurchaseInput DetailPurchaseInputObject
        {
            get { return GetPropertyValue<DetailPurchaseInput>(nameof(DetailPurchaseInputObject)); }
            set { SetPropertyValue(nameof(DetailPurchaseInputObject), value); }
        }

        [VisibleInLookupListView(true)]
        [XafDisplayName("구매입고번호"), ToolTip("구매입고번호")]
        public string PurchaseInputNumber
        {
            get { return DetailPurchaseInputObject?.MasterPurchaseInputObject?.PurchaseInputNumber; }
        }

        [VisibleInLookupListView(true)]
        [XafDisplayName("품목 명칭"), ToolTip("품목 명칭")]
        public string ItemName
        {
            get { return DetailPurchaseInputObject?.ItemName; }
        }

        [VisibleInLookupListView(true)]
        [RuleRequiredField(CustomMessageTemplate = "검사일시를 입력하세요")]
        [ModelDefault("DisplayFormat", "yyyy/MM/dd")]
        [XafDisplayName("검사일시"), ToolTip("검사일시")]
        public DateTime InspectionDateTime
        {
            get { return GetPropertyValue<DateTime>(nameof(InspectionDateTime)); }
            set { SetPropertyValue(nameof(InspectionDateTime), value); }
        }
        [VisibleInLookupListView(true)]
        [ModelDefault("LookupProperty", nameof(Employee.EmployeeName))]
        [LookupEditorMode(LookupEditorMode.AllItemsWithSearch)]
        [RuleRequiredField(CustomMessageTemplate = "검사자를 입력하세요")]
        [XafDisplayName("검사자"), ToolTip("검사자")]
        public Employee Inspector
        {
            get { return GetPropertyValue<Employee>(nameof(Inspector)); }
            set { SetPropertyValue(nameof(Inspector), value); }
        }

        [VisibleInLookupListView(true)]
        [ModelDefault("EditMask", "###,###,###,###,###,###,###,###,###,##0.##")]
        [Appearance("InspectionrRequestQuantityNotNew", Enabled = false, Criteria = "!IsNewObject(this)", TargetItems = nameof(InspectionRequestQuantity), Context = "DetailView")]
        [XafDisplayName("검사요청수량"), ToolTip("검사요청수량")]
        public double InspectionRequestQuantity
        {
            get { return GetPropertyValue<double>(nameof(InspectionRequestQuantity)); }
            set { SetPropertyValue(nameof(InspectionRequestQuantity), value); }
        }

        [VisibleInLookupListView(true)]
        [ModelDefault("EditMask", "###,###,###,###,###,###,###,###,###,##0.##")]
        [XafDisplayName("검사수량"), ToolTip("검사수량")]
        public double InspectionQuantity
        {
            get { return GoodQuantity + DefectQuantity; }
        }

        [RuleValueComparison("", DefaultContexts.Save, ValueComparisonType.LessThanOrEqual, "InspectionRequestQuantity - DefectQuantity", ParametersMode.Expression, CustomMessageTemplate = "[양품수량]은 [검사요청수량]을 초과할 수 없습니다.")]
        [VisibleInLookupListView(true)]
        [RuleRange("", DefaultContexts.Save, 0, double.MaxValue, CustomMessageTemplate = "[양품수량]은 0 이상이어야 합니다.")]
        [ModelDefault("EditMask", "###,###,###,###,###,###,###,###,###,##0.##")]
        [XafDisplayName("양품수량"), ToolTip("양품수량")]
        public double GoodQuantity
        {
            get { return GetPropertyValue<double>(nameof(GoodQuantity)); }
            set { SetPropertyValue(nameof(GoodQuantity), value); }
        }

        [VisibleInLookupListView(true)]
        [RuleRange("", DefaultContexts.Save, 0, double.MaxValue, CustomMessageTemplate = "[불량수량]은 0 이상이어야 합니다.")]
        [RuleValueComparison("", DefaultContexts.Save, ValueComparisonType.LessThanOrEqual, "InspectionRequestQuantity - GoodQuantity", ParametersMode.Expression, CustomMessageTemplate = "[불량수량]은 [검사요청수량]을 초과할 수 없습니다.")]
        [ModelDefault("EditMask", "###,###,###,###,###,###,###,###,###,##0.##")]
        [XafDisplayName("불량수량"), ToolTip("불량수량")]
        public double DefectQuantity
        {
            get { return GetPropertyValue<double>(nameof(DefectQuantity)); }
            set { SetPropertyValue(nameof(DefectQuantity), value); }
        }

        [VisibleInLookupListView(true)]
        [RuleRange("", DefaultContexts.Save, 0, double.MaxValue, CustomMessageTemplate = "[외관]은 0 이상이어야 합니다.")]
        [ModelDefault("EditMask", "###,###,###,###,###,###,###,###,###,##0.##")]
        [XafDisplayName("외관"), ToolTip("외관")]
        public double Apperance
        {
            get { return GetPropertyValue<double>(nameof(Apperance)); }
            set { SetPropertyValue(nameof(Apperance), value); }
        }

        [VisibleInLookupListView(true)]
        [RuleRange("", DefaultContexts.Save, 0, double.MaxValue, CustomMessageTemplate = "[수치]은 0 이상이어야 합니다.")]
        [ModelDefault("EditMask", "###,###,###,###,###,###,###,###,###,##0.##")]
        [XafDisplayName("수치"), ToolTip("수치")]
        public double Figure
        {
            get { return GetPropertyValue<double>(nameof(Figure)); }
            set { SetPropertyValue(nameof(Figure), value); }
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
        #endregion

        #region Constructors
        public IncomingInspection(Session session) : base(session) { }
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