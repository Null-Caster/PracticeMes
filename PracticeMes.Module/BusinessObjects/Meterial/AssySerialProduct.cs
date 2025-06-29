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
using PracticeMes.Module.BusinessObjects.Inspect;
using PracticeMes.Module.BusinessObjects.WorkResult;

namespace PracticeMes.Module.BusinessObjects.Meterial
{
    [DefaultClassOptions]
    [NavigationItem(false)]
    [XafDisplayName("반제품 시리얼")]
    [DefaultListViewOptions(MasterDetailMode.ListViewOnly, false, NewItemRowPosition.None)]
    public class AssySerialProduct : BaseObject
    {
        #region Properties
        [Browsable(false)]
        public DetailIntermediateInspection DetailIntermediateInspectionObject
        {
            get { return GetPropertyValue<DetailIntermediateInspection>(nameof(DetailIntermediateInspectionObject)); }
            set { SetPropertyValue(nameof(DetailIntermediateInspectionObject), value); }
        }

        [VisibleInLookupListView(true)]
        [RuleRequiredField(CustomMessageTemplate = "시리얼 번호를 입력하세요.")]
        [XafDisplayName("시리얼 번호"), ToolTip("시리얼 번호")]
        public string SerialNumber
        {
            get { return GetPropertyValue<string>(nameof(SerialNumber)); }
            set { SetPropertyValue(nameof(SerialNumber), value); }
        }

        [VisibleInLookupListView(true)]
        [XafDisplayName("품목코드"), ToolTip("품목코드")]
        public string ItemCode
        {
            get { return MiddleWorkResultObject?.ItemCode; }
        }

        [VisibleInLookupListView(true)]
        [XafDisplayName("품목명칭"), ToolTip("품목명칭")]
        public string ItemName
        {
            get { return MiddleWorkResultObject?.ItemName; }
        }

        [VisibleInLookupListView(true)]
        [XafDisplayName("공정이름"), ToolTip("공정이름")]
        public string WorkProcessName
        {
            get { return MiddleWorkResultObject?.WorkProcessName; }
        }

        [VisibleInLookupListView(true)]
        [ModelDefault("AllowEdit", "False")]
        [XafDisplayName("양품수량"), ToolTip("양품수량")]
        public double GoodQuantity
        {
            get
            {
                if (MiddleWorkResultObject?.DetailWorkInstructionQuantity != MiddleWorkResultObject?.WorkInstructionQuantity)
                {
                    return MiddleWorkResultObject.GoodQuantity - WorkProcessDefectQuantity;
                }
                else
                {
                    return 1 - WorkProcessDefectQuantity;
                }
            }
        }

        [VisibleInLookupListView(true)]
        [ModelDefault("AllowEdit", "False")]
        [XafDisplayName("불량수량(공정)"), ToolTip("불량수량(공정)")]
        public double WorkProcessDefectQuantity
        {
            get { return GetPropertyValue<double>(nameof(WorkProcessDefectQuantity)); }
            set { SetPropertyValue(nameof(WorkProcessDefectQuantity), value); }
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

        [Browsable(false)]
        [Association(@"AssySerialProductReferencesMiddleWorkResult")]
        public MiddleWorkResult MiddleWorkResultObject
        {
            get { return GetPropertyValue<MiddleWorkResult>(nameof(MiddleWorkResultObject)); }
            set { SetPropertyValue<MiddleWorkResult>(nameof(MiddleWorkResultObject), value); }
        }
        #endregion

        #region Constructors
        public AssySerialProduct(Session session) : base(session) { }
        #endregion

        #region Methods
        public override void AfterConstruction()
        {
            base.AfterConstruction();
        }
        #endregion
    }
}