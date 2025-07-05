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
using PracticeMes.Module.BusinessObjects.LotManagement;

namespace PracticeMes.Module.BusinessObjects.Sales
{
    [XafDisplayName("Lot 정보")]
    [DefaultClassOptions]
    [DefaultListViewOptions(MasterDetailMode.ListViewOnly, false, NewItemRowPosition.Top)]
    public class DetailSalesShipmentLot : BaseObject
    {

        #region Properties
        [VisibleInLookupListView(true)]
        [ImmediatePostData(true)]
        [ModelDefault("LookupProperty", nameof(Lot.LotNumber))]
        [DataSourceProperty(nameof(AvailableObjects))]
        [LookupEditorMode(LookupEditorMode.AllItemsWithSearch)]
        [XafDisplayName("Lot 번호"), ToolTip("Lot 번호")]
        public Lot LotObject
        {
            get { return GetPropertyValue<Lot>(nameof(LotObject)); }
            set { SetPropertyValue(nameof(LotObject), value); }
        }

        [VisibleInLookupListView(true)]
        [XafDisplayName("품목 코드"), ToolTip("품목 코드")]
        public string ItemCode
        {
            get { return LotObject?.ItemObject?.ItemCode; }
        }

        [VisibleInLookupListView(true)]
        [XafDisplayName("품목 명칭"), ToolTip("품목 명칭")]
        public string ItemName
        {
            get { return LotObject?.ItemObject?.ItemName; }
        }

        [VisibleInLookupListView(true)]
        [XafDisplayName("로트 재고 수량"), ToolTip("로트 재고 수량")]
        public double StockQuantity
        {
            get { return (LotObject?.StockQuantity ?? 0); }
        }

        [VisibleInLookupListView(true)]
        [ModelDefault("EditMask", "###,###,###,###,###,###,###,###,###,##0.###")]
        [XafDisplayName("출고 수량"), ToolTip("출고 수량")]
        public double ShipmentQuantity
        {
            get { return Math.Round(GetPropertyValue<double>(nameof(ShipmentQuantity)), 1); }
            set { SetPropertyValue(nameof(ShipmentQuantity), Math.Round(value, 1)); }
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

        [Association(@"DetailSalesShipmentLotReferencesMasterSalesShipment")]
        public MasterSalesShipment MasterSalesShipmentObject
        {
            get { return GetPropertyValue<MasterSalesShipment>(nameof(MasterSalesShipmentObject)); }
            set { SetPropertyValue(nameof(MasterSalesShipmentObject), value); }
        }

        #endregion

        #region Constructors
        public DetailSalesShipmentLot(Session session) : base(session) { }
        #endregion

        #region Methods
        public override void AfterConstruction()
        {
            base.AfterConstruction();

            CreatedDateTime = DateTime.Now;
        }

        private List<Lot> AvailableObjects
        {
            get
            {
                if (this.MasterSalesShipmentObject != null)
                {
                    var test = new XPCollection<Lot>(this.Session).Where(x => x.ItemObject.Oid == MasterSalesShipmentObject?.DetailSalesOrderObject?.ItemObject.Oid)
                                                                  .Where(x => x.StockQuantity > 0 && x.ItemAccountObject?.ItemAccountName == "제품").ToList();

                    foreach (var visiblelotobject in MasterSalesShipmentObject.DetailSalesShipmentLotObjects)
                    {
                        if (test.Contains(visiblelotobject.LotObject))
                        {
                            test.Remove(visiblelotobject.LotObject);
                        }
                    }
                    return test;
                }
                // 아니면
                else
                {
                    return null;
                }
            }
        }
        #endregion
    }
}