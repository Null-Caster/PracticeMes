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
using PracticeMes.Module.BusinessObjects.WorkResult;

namespace PracticeMes.Module.BusinessObjects.Inspect
{
    [DefaultClassOptions]
    [NavigationItem("품질 관리"), XafDisplayName("공정검사")]
    [DefaultListViewOptions(MasterDetailMode.ListViewOnly, false, NewItemRowPosition.Top)]
    public class MasterIntermediateInspection : BaseObject
    {
        #region Properties
        [Browsable(false)]
        [RuleRequiredField(CustomMessageTemplate = "중간공정이 존재하지 않습니다.")]
        public MiddleWorkResult MiddleWorkResultObject
        {
            get { return GetPropertyValue<MiddleWorkResult>(nameof(MiddleWorkResultObject)); }
            set { SetPropertyValue(nameof(MiddleWorkResultObject), value); }
        }

        [VisibleInLookupListView(true)]
        [XafDisplayName("작업지시번호"), ToolTip("작업지시번호")]
        public string WorkInstructionNumber
        {
            get { return this.MiddleWorkResultObject?.DetailWorkInstructionObject?.WorkInstructionNumber; }
        }
        [VisibleInLookupListView(true)]
        [XafDisplayName("공정명"), ToolTip("공정명")]
        public string WorkProcessName
        {
            get { return this.MiddleWorkResultObject?.WorkProcessName; }
        }

        [VisibleInLookupListView(true)]
        [XafDisplayName("검사수량"), ToolTip("검사수량")]
        public int InspectionQuantity
        {
            get { return this.DetailIntermediateInspectionObject?.Sum(x => x.InspectionQuantity) ?? 0; }
        }

        [VisibleInLookupListView(true)]
        [XafDisplayName("불량수량"), ToolTip("불량수량")]
        public int DefectQuantity
        {
            get { return this.DetailIntermediateInspectionObject?.Sum(x => x.DefectQuantity) ?? 0; }
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

        [XafDisplayName("공정검사 상세")]
        [Association(@"DetailIntermediateInspectionObjectRefernecesMasterIntermediateInspection"), DevExpress.Xpo.Aggregated]
        public XPCollection<DetailIntermediateInspection> DetailIntermediateInspectionObject { get { return GetCollection<DetailIntermediateInspection>(nameof(DetailIntermediateInspectionObject)); } }

        #endregion

        #region Constructors
        public MasterIntermediateInspection(Session session) : base(session)  {}
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