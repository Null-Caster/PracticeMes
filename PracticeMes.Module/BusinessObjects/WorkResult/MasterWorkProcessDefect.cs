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
using PracticeMes.Module.BusinessObjects.Inspect;
using PracticeMes.Module.BusinessObjects.Meterial;

namespace PracticeMes.Module.BusinessObjects.WorkResult
{
    [DefaultClassOptions]
    [NavigationItem("공정 관리"), XafDisplayName("공정 불량 등록")]
    [DefaultListViewOptions(MasterDetailMode.ListViewOnly, true, NewItemRowPosition.Top)]
    public class MasterWorkProcessDefect : BaseObject
    {
        #region Properties
        [VisibleInLookupListView(true)]
        [ImmediatePostData(true)]
        [Appearance("AssySerialProductObjectEdit", Criteria = "!(IsNewObject(this))", Enabled = false)]
        [DataSourceProperty(nameof(AvailableAssySerialProductObjects))]
        [ModelDefault("LookupProperty", nameof(AssySerialProduct.SerialNumber))]
        [LookupEditorMode(LookupEditorMode.AllItemsWithSearch)]
        [RuleUniqueValue(CustomMessageTemplate = "공정의 시리얼번호가 중복되었습니다.")]
        [RuleRequiredField(CustomMessageTemplate = "시리얼 번호를 입력하세요.")]
        [XafDisplayName("시리얼 번호"), ToolTip("시리얼 번호")]
        public AssySerialProduct AssySerialProductObject
        {
            get { return GetPropertyValue<AssySerialProduct>(nameof(AssySerialProductObject)); }
            set { SetPropertyValue(nameof(AssySerialProductObject), value); }
        }

        [Browsable(false)]
        public List<AssySerialProduct> AvailableAssySerialProductObjects
        {
            get
            {
                return new XPCollection<AssySerialProduct>(this.Session).Where(x => x.MiddleWorkResultObject?.DetailWorkInstructionObject?.Progress.CodeName == "진행중" && 
                                                                                    x.Oid != this.AssySerialProductObject?.Oid)
                                                                        .ToList();
            }
        }

        [VisibleInLookupListView(true)]
        [XafDisplayName("작업지시 번호"), ToolTip("작업지시 번호")]
        public string WorkInstructionNumber
        {
            get { return AssySerialProductObject?.MiddleWorkResultObject?.DetailWorkInstructionObject?.WorkInstructionNumber; }
        }

        [VisibleInLookupListView(true)]
        [XafDisplayName("공정 명칭"), ToolTip("공정 명칭")]
        public string WorkProcessName
        {
            get { return AssySerialProductObject?.MiddleWorkResultObject?.WorkProcessName; }
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