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
using DevExpress.Pdf.Native.BouncyCastle.Utilities.IO.Pem;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Base.General;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using DevExpress.XtraRichEdit.Fields;
using PracticeMes.Module.BusinessObjects.BaseInfo.ItemInfo;
using PracticeMes.Module.BusinessObjects.LotManagement;

namespace PracticeMes.Module.BusinessObjects.ProductPlanning
{
    [Appearance("Level2", AppearanceItemType.ViewItem, "Level == 2", TargetItems = "*", FontColor = "Blue")]
    [Appearance("Level3", AppearanceItemType.ViewItem, "Level == 3", TargetItems = "*", FontColor = "Green")]
    [Appearance("Level4", AppearanceItemType.ViewItem, "Level == 4", TargetItems = "*", FontColor = "Red")]
    [Appearance("Level5", AppearanceItemType.ViewItem, "Level == 5", TargetItems = "*", FontColor = "Purple")]
    [DefaultClassOptions]
    [XafDisplayName("BOM정보")]
    [DefaultListViewOptions(MasterDetailMode.ListViewOnly, false, NewItemRowPosition.None)]
    public class DetailProductionPlanning : BaseObject, ITreeNode
    {
        #region Properties
        [Index(0)]
        [XafDisplayName("레벨"), ToolTip("레벨")]
        public int Level
        {
            get { return Parent is null ? 1 : Parent.Level + 1; }
        }

        [Index(1)]
        [ImmediatePostData(true)]
        [VisibleInLookupListView(true)]
        [ModelDefault("LookupProperty", nameof(Item.ItemName))]
        [LookupEditorMode(LookupEditorMode.AllItemsWithSearch)]
        [XafDisplayName("품목 이름"), ToolTip("품목 이름")]
        public Item ItemObject
        {
            get { return GetPropertyValue<Item>(nameof(ItemObject)); }
            set { SetPropertyValue(nameof(ItemObject), value); }
        }

        [Index(2)]
        [ImmediatePostData(true)]
        [LookupEditorMode(LookupEditorMode.AllItemsWithSearch)]
        [XafDisplayName("품목 코드"), ToolTip("품목 코드")]
        public string ItemCode
        {
            get { return ItemObject?.ItemCode; }
        }

        [ImmediatePostData(true)]
        [LookupEditorMode(LookupEditorMode.AllItemsWithSearch)]
        [XafDisplayName("품목 계정"), ToolTip("품목 코드")]
        public string ItemAccountName
        {
            get { return ItemObject?.ItemAccountObject?.ItemAccountName; }
        }

        [ImmediatePostData(true)]
        [LookupEditorMode(LookupEditorMode.AllItemsWithSearch)]
        [XafDisplayName("품목 단위"), ToolTip("품목 단위")]
        public string ItemUnit
        {
            get { return ItemObject?.UnitObject?.UnitName; }
        }

        [VisibleInLookupListView(true)]
        [ModelDefault("EditMask", "###,###,###,###,###,###,###,###,###,##0.0")]
        [ModelDefault("AllowEdit", "false")]
        [XafDisplayName("재고 수량"), ToolTip("재고 수량")]
        public double StockQuantity
        {
            get
            {
                if (ItemObject == null)
                {
                    return 0;
                }

                double stockQuantity = new XPCollection<Lot>(this.Session)
                    .Where(x => x.ItemObject?.Oid == this.ItemObject?.Oid)
                    .Sum(x => x.StockQuantity);

                return stockQuantity;
            }
        }

        [VisibleInLookupListView(true)]
        [ModelDefault("EditMask", "###,###,###,###,###,###,###,###,###,##0.0")]
        [ModelDefault("AllowEdit", "false")]
        [XafDisplayName("투입 수량"), ToolTip("투입 수량")]
        public double InputQuantity
        {
            get { return MasterProductionPlanningObject?.ProductPlanningQuantity * BOMQuantity ?? 0; }
        }

        [VisibleInLookupListView(true)]
        [ModelDefault("EditMask", "###,###,###,###,###,###,###,###,###,##0.0")]
        [ModelDefault("AllowEdit", "false")]
        [XafDisplayName("과부족수량"), ToolTip("과부족수량")]
        public double OverageQuantity
        {
            get
            {
                if (StockQuantity > InputQuantity)
                {
                    return 0;
                }
                else
                {
                    return InputQuantity - StockQuantity;
                }
            }
        }

        [ModelDefault("EditMask", "###,###,###,###,###,###,###,###,###,##0.0")]
        [XafDisplayName("BOM 수량"), ToolTip("BOM 수량")]
        public double BOMQuantity
        {
            get { return Math.Round(GetPropertyValue<double>(nameof(BOMQuantity)), 1); }
            set { SetPropertyValue(nameof(BOMQuantity), Math.Round(value, 1)); }
        }

        [Association(@"DetailProductionPlanningRefernencesMasterProductionPlanning")]
        public MasterProductionPlanning MasterProductionPlanningObject
        {
            get { return GetPropertyValue<MasterProductionPlanning>(nameof(MasterProductionPlanningObject)); }
            set { SetPropertyValue(nameof(MasterProductionPlanningObject), value); }
        }

        [VisibleInDetailView(false)]
        [VisibleInLookupListView(false)]
        [VisibleInListView(false)]
        [ModelDefault("AllowEdit", "false")]
        [XafDisplayName("부모 노드 이름"), ToolTip("부모 노드 이름")]
        [Association("AssemblyBOMParent-AssemblyBOMChild")]
        public DetailProductionPlanning Parent
        {
            get { return GetPropertyValue<DetailProductionPlanning>(nameof(Parent)); }
            set { SetPropertyValue(nameof(Parent), value); }
        }

        [Browsable(false)]
        [XafDisplayName("노드 이름"), ToolTip("노드 이름")]
        public string Name
        {
            get { return MasterProductionPlanningObject.ItemObject is null ? string.Empty : $"레벨: {Level} 품목 코드: {ItemCode} 품목 이름: {ItemObject?.ItemName}"; }
        }

        [Association("AssemblyBOMParent-AssemblyBOMChild"), DevExpress.Xpo.Aggregated]
        public XPCollection<DetailProductionPlanning> Children { get { return GetCollection<DetailProductionPlanning>(nameof(Children)); } }
        #endregion

        #region Fields
        IBindingList ITreeNode.Children { get { return Children as IBindingList; } }
        ITreeNode ITreeNode.Parent { get { return Parent as ITreeNode; } }
        string ITreeNode.Name { get { return Name; } }
        #endregion

        #region Constructors
        public DetailProductionPlanning(Session session) : base(session) { }
        #endregion

        #region Methods
        public override void AfterConstruction()
        {
            base.AfterConstruction();
        }

        #endregion
    }
}