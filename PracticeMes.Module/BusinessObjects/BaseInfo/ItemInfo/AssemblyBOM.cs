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
using DevExpress.Persistent.Base.General;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;

namespace PracticeMes.Module.BusinessObjects.BaseInfo.ItemInfo
{
    [RuleCombinationOfPropertiesIsUnique("AssemblyBOM_Unique", DefaultContexts.Save, "ItemObject, Level, ProductBOMObject")]
    [Appearance("Level2", AppearanceItemType.ViewItem, "Level == 2", TargetItems = "*", FontColor = "Blue")]
    [Appearance("Level3", AppearanceItemType.ViewItem, "Level == 3", TargetItems = "*", FontColor = "Green")]
    [Appearance("Level4", AppearanceItemType.ViewItem, "Level == 4", TargetItems = "*", FontColor = "Red")]
    [Appearance("Level5", AppearanceItemType.ViewItem, "Level == 5", TargetItems = "*", FontColor = "Purple")]
    [NavigationItem("품목 정보"), XafDisplayName("BOM 등록 상세")]
    [DefaultClassOptions]
    [ImageName("InsertTreeView")]
    [DefaultListViewOptions(MasterDetailMode.ListViewOnly, false, NewItemRowPosition.None)]
    [Persistent(nameof(AssemblyBOM))]
    public class AssemblyBOM : BaseObject, ITreeNode
    {
        #region Properties
        [XafDisplayName("레벨"), ToolTip("레벨")]
        public double Level
        {
            get { return Parent is null ? 1 : Parent.Level + 1; }
        }

        [ImmediatePostData(true)]
        [VisibleInLookupListView(true)]
        [ModelDefault("LookupProperty", nameof(Item.ItemCode))]
        [LookupEditorMode(LookupEditorMode.AllItemsWithSearch)]
        [XafDisplayName("품목 코드"), ToolTip("품목 코드")]
        public Item ItemObject
        {
            get { return GetPropertyValue<Item>(nameof(ItemObject)); }
            set { SetPropertyValue(nameof(ItemObject), value); }
        }

        [VisibleInLookupListView(true)]
        [XafDisplayName("품목 이름"), ToolTip("품목 이름")]
        public string ItemName
        {
            get { return this.ItemObject?.ItemName; }
        }

        [VisibleInLookupListView(true)]
        [ModelDefault("LookupProperty", nameof(ItemAccount.ItemAccountName))]
        [XafDisplayName("품목계정"), ToolTip("품목계정")]

        public ItemAccount ItemAccountObject
        {
            get { return ItemObject.ItemAccountObject; }
        }

        [VisibleInLookupListView(true)]
        [ModelDefault("EditMask", "###,###,###,###,###,###,###,###,###,##0.0")]
        [XafDisplayName("BOM 수량"), ToolTip("BOM 수량")]
        public double BOMQuantity
        {
            get { return Math.Round(GetPropertyValue<double>(nameof(BOMQuantity)), 1); }
            set { SetPropertyValue(nameof(BOMQuantity), Math.Round(value, 1)); }
        }

        [VisibleInLookupListView(true)]
        [ModelDefault("EditMask", "###,###,###,###,###,###,###,###,###,##0.###")]
        [XafDisplayName("단가"), ToolTip("단가")]
        public double UnitPrice
        {
            get { return ItemObject?.UnitPrice ?? 0; }
        }

        [VisibleInLookupListView(true)]
        [Browsable(false)]
        [XafDisplayName("노드 이름"), ToolTip("노드 이름")]
        public string Name
        {
            get { return ItemObject is null ? string.Empty : $"레벨: {Level} 품목 코드: {ItemObject.ItemCode} 품목 이름: {ItemObject.ItemName} 투입 수량: {BOMQuantity}"; }
        }

        // 보고서에서 필요함
        [VisibleInDetailView(false)]
        [VisibleInLookupListView(false)]
        [VisibleInListView(false)]
        [ModelDefault("AllowEdit", "false")]
        [XafDisplayName("부모 노드 이름"), ToolTip("부모 노드 이름")]
        [Association("AssemblyBOMParent-AssemblyBOMChild")]
        public AssemblyBOM Parent
        {
            get { return GetPropertyValue<AssemblyBOM>(nameof(Parent)); }
            set { SetPropertyValue(nameof(Parent), value); }
        }

        //[VisibleInDetailView(false)]
        //[VisibleInLookupListView(false)]
        //[VisibleInListView(false)]
        //[XafDisplayName("부모 노드 문자열"), ToolTip("부모 노드 문자열")]
        //public string ParentString
        //{
        //    get { return this.Parent == null ? string.Empty : Parent.Oid.ToString(); }
        //}

        //[VisibleInDetailView(false)]
        //[VisibleInLookupListView(false)]
        //[VisibleInListView(false)]
        //[XafDisplayName("자신 노드 문자열"), ToolTip("자신 노드 문자열")]
        //public string SelfString
        //{
        //    get { return this == null ? string.Empty : this.Oid.ToString(); }
        //}

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

        [VisibleInDetailView(false)]
        [VisibleInLookupListView(false)]
        [VisibleInListView(false)]
        [Association(@"AssemblyBOM-ProductBOM")]
        public ProductBOM ProductBOMObject
        {
            get { return GetPropertyValue<ProductBOM>(nameof(ProductBOMObject)); }
            set { SetPropertyValue(nameof(ProductBOMObject), value); }
        }

        [Association("AssemblyBOMParent-AssemblyBOMChild"), DevExpress.Xpo.Aggregated]
        public XPCollection<AssemblyBOM> Children { get { return GetCollection<AssemblyBOM>(nameof(Children)); } }
        #endregion

        #region Fields
        IBindingList ITreeNode.Children { get { return Children as IBindingList; } }
        ITreeNode ITreeNode.Parent { get { return Parent as ITreeNode; } }
        string ITreeNode.Name { get { return Name; } }
        #endregion

        #region Constructors
        public AssemblyBOM(Session session) : base(session) { }
        #endregion

        #region Fields
        private bool isDeleting;
        #endregion

        #region Methods
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            CreatedDateTime = DateTime.Now;
            IsEnabled = true;
        }
        protected override void OnChanged(string propertyName, object oldValue, object newValue)
        {
            base.OnChanged(propertyName, oldValue, newValue);
            if (this.Session.IsObjectsLoading)
            {
                return;
            }
            switch (propertyName)
            {
                case nameof(ItemObject):
                    foreach (var child in this.Children.ToList())
                        child.Delete();
                    var productBOMObject2 = new XPCollection<ProductBOM>(this.Session, CriteriaOperator.Parse("ItemObject.Oid == ?", this.ItemObject.Oid))
                                            .ToList()
                                            .FirstOrDefault();
                    if (productBOMObject2 != null)
                    {
                        foreach (var assemblyBOMObject in productBOMObject2.AssemblyBOMObjects.ToList())
                        {
                            CopyAssemblyBOMRecursive(assemblyBOMObject, this);
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        protected override void OnDeleting()
        {
            base.OnDeleting();
            isDeleting = true;
        }

        private void CopyAssemblyBOMRecursive(AssemblyBOM source, AssemblyBOM parentTarget)
        {
            var newChild = new AssemblyBOM(this.Session)
            {
                ItemObject = source.ItemObject,
                BOMQuantity = source.BOMQuantity,
                Parent = parentTarget
            };
            parentTarget.Children.Add(newChild);
        }
        #endregion
    }
}