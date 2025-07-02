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
using PracticeMes.Module.BusinessObjects.LotManagement;

namespace PracticeMes.Module.BusinessObjects.Purchase
{
    [DefaultClassOptions]
    [NavigationItem("구매 관리"), XafDisplayName("구매반품 등록")]
    [DefaultListViewOptions(MasterDetailMode.ListViewOnly, false, NewItemRowPosition.None)]
    [RuleCriteria(
    "PurchaseReturn_StockLimit",
    DefaultContexts.Save,
    "PurchaseReturnQuantity > 0 AND " +
    "PurchaseReturnQuantity <= Iif(IsNull(DetailPurchaseInputObject.LotObject.StockQuantity), 0, DetailPurchaseInputObject.LotObject.StockQuantity)",
    CustomMessageTemplate = "반품 수량은 0 이상이며, 재고 수량을 초과할 수 없습니다.")]
    public class PurchaseReturn : BaseObject
    {

        #region Properties
        [ImmediatePostData(true)]
        [VisibleInLookupListView(true)]
        [ModelDefault("LookupProperty", nameof(MasterPurchaseInput.PurchaseInputNumber))]
        [LookupEditorMode(LookupEditorMode.AllItemsWithSearch)]
        [XafDisplayName("구매입고 번호"), ToolTip("구매입고 번호")]
        [Appearance("DisableReturnQtyAfterSave", AppearanceItemType.ViewItem, nameof(MasterPurchaseInputObject), Context = "DetailView", Enabled = false, Criteria = "Not IsNewObject(This)")]
        public MasterPurchaseInput MasterPurchaseInputObject
        {
            get { return GetPropertyValue<MasterPurchaseInput>(nameof(MasterPurchaseInputObject)); }
            set { SetPropertyValue(nameof(MasterPurchaseInputObject), value); }
        }

        [VisibleInLookupListView(true)]
        [XafDisplayName("거래처"), ToolTip("거래처")]
        public string BizPartnerName
        {
            get { return MasterPurchaseInputObject?.BizPartnerName; }
        }

        [VisibleInLookupListView(true)]
        [ImmediatePostData(true)]
        [DataSourceProperty("MasterPurchaseInputObject.DetailPurchaseInputObjects")]
        [ModelDefault("LookupProperty", nameof(DetailPurchaseInput.ItemCode))]
        [LookupEditorMode(LookupEditorMode.AllItemsWithSearch)]
        [RuleRequiredField(CustomMessageTemplate = "품목코드를 입력하세요.")]
        [XafDisplayName("품목코드"), ToolTip("품목코드")]

        public DetailPurchaseInput DetailPurchaseInputObject
        {
            get { return GetPropertyValue<DetailPurchaseInput>(nameof(DetailPurchaseInputObject)); }
            set { SetPropertyValue(nameof(DetailPurchaseInputObject), value); }
        }

        [VisibleInLookupListView(true)]
        [XafDisplayName("품목 명칭"), ToolTip("품목 명칭")]
        public string ItemName
        {
            get { return DetailPurchaseInputObject?.ItemObject?.ItemName; }
        }

        [Browsable(false)]
        public Lot LotObject
        {
            get { return DetailPurchaseInputObject?.LotObject; }
        }

        [VisibleInLookupListView(true)]
        [XafDisplayName("LOT번호"), ToolTip("LOT번호")]
        public string LotNumber
        {
            get { return DetailPurchaseInputObject?.LotObject?.LotNumber; }
        }

        [VisibleInLookupListView(true)]
        [ModelDefault("DisplayFormat", "yyyy/MM/dd HH:mm:ss")]
        [ModelDefault("DisplayFormat", "yyyy/MM/dd HH:mm:ss")]
        [RuleRequiredField(CustomMessageTemplate = "반품일시를 입력하세요.")]
        [XafDisplayName("반품일시"), ToolTip("반품일시")]
        public DateTime PurchaseReturnDateTime
        {
            get { return GetPropertyValue<DateTime>(nameof(PurchaseReturnDateTime)); }
            set { SetPropertyValue(nameof(PurchaseReturnDateTime), value); }
        }

        [VisibleInLookupListView(true)]
        [XafDisplayName("재고수량"), ToolTip("재고수량")]
        public double StockQuantity
        {
            get { return DetailPurchaseInputObject?.LotObject?.StockQuantity ?? 0; }
        }

        [VisibleInLookupListView(true)]
        [ImmediatePostData(true)]
        [XafDisplayName("구매반품 수량"), ToolTip("구매반품 수량")]
        public double PurchaseReturnQuantity
        {
            get { return GetPropertyValue<double>(nameof(PurchaseReturnQuantity)); }
            set { SetPropertyValue(nameof(PurchaseReturnQuantity), value); }
        }

        [VisibleInLookupListView(true)]
        [ModelDefault("EditMask", "###,###,###,###,###,###,###,###,###,##0.##")]
        [XafDisplayName("구매반품 금액"), ToolTip("구매반품 금액")]
        public double PurchaseReturnPrice
        {
            get { return GetPropertyValue<double>(nameof(PurchaseReturnPrice)); }
            set { SetPropertyValue(nameof(PurchaseReturnPrice), value); }
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
        public PurchaseReturn(Session session) : base(session) { }
        #endregion

        #region Methods
        public override void AfterConstruction()
        {
            base.AfterConstruction();

            PurchaseReturnDateTime = DateTime.Now;
            CreatedDateTime = DateTime.Now;
        }

        protected override void OnChanged(string propertyName, object oldValue, object newValue)
        {
            base.OnChanged(propertyName, oldValue, newValue);

            if (Session.IsObjectsLoading) return;

            switch (propertyName)
            {
                case nameof(PurchaseReturnQuantity):
                    this.PurchaseReturnPrice = this.PurchaseReturnQuantity * DetailPurchaseInputObject.UnitPrice;
                    break;
                default:
                    break;
            }
        }
        #endregion

    }
}