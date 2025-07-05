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

namespace PracticeMes.Module.BusinessObjects.Sales
{
    [DefaultClassOptions]
    [NavigationItem(false)]
    [XafDisplayName("출하 등록 상세")]
    [DefaultListViewOptions(MasterDetailMode.ListViewOnly, false, NewItemRowPosition.None)]
    public class DetailSalesShipment : BaseObject
    {
        #region Properties
        [ModelDefault("LookupProperty", nameof(DetailSalesOrder.SalesOrderNumber))]
        [LookupEditorMode(LookupEditorMode.AllItemsWithSearch)]
        [RuleRequiredField(CustomMessageTemplate = "수주 번호를 입력하세요.")]
        [XafDisplayName("수주 번호"), ToolTip("수주 번호")]
        [ImmediatePostData(true)]
        public DetailSalesOrder DetailSalesOrderObject
        {
            get { return GetPropertyValue<DetailSalesOrder>(nameof(DetailSalesOrderObject)); }
            set { SetPropertyValue(nameof(DetailSalesOrderObject), value); }
        }

        [VisibleInLookupListView(true)]
        [XafDisplayName("품목 코드"), ToolTip("품목 코드")]
        public string ItemCode
        {
            get { return DetailSalesOrderObject?.ItemObject?.ItemCode; }
        }

        [VisibleInLookupListView(true)]
        [XafDisplayName("품목 이름"), ToolTip("품목 이름")]
        public string ItemName
        {
            get { return DetailSalesOrderObject?.ItemName; }
        }

        [XafDisplayName("수주 수량"), ToolTip("수주 수량")]
        public double SalesOrderQuantity
        {
            get { return DetailSalesOrderObject?.SalesOrderQuantity ?? 0; }
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
        [ModelDefault("EditMask", "yyyy/MM/dd HH:mm:ss")]
        [ModelDefault("DisplayFormat", "yyyy/MM/dd HH:mm:ss")]
        [XafDisplayName("생성 일시"), ToolTip("항목이 생성된 일시입니다.")]
        public DateTime CreatedDateTime
        {
            get { return GetPropertyValue<DateTime>(nameof(CreatedDateTime)); }
            set { SetPropertyValue(nameof(CreatedDateTime), value); }
        }

        //[Association(@"DetailSalesShipmentReferencesMasterSalesShipment")]
        //public MasterSalesShipment MasterSalesShipmentObject
        //{
        //    get { return GetPropertyValue<MasterSalesShipment>(nameof(MasterSalesShipmentObject)); }
        //    set { SetPropertyValue(nameof(MasterSalesShipmentObject), value); }
        //}
        #endregion

        #region Constructors
        public DetailSalesShipment(Session session) : base(session) { }
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