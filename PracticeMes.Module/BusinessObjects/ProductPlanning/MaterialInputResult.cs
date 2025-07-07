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
using PracticeMes.Module.BusinessObjects.BaseInfo.ItemInfo;
using PracticeMes.Module.BusinessObjects.LotManagement;

namespace PracticeMes.Module.BusinessObjects.ProductPlanning
{
    [DefaultClassOptions]
    [NavigationItem("생산 계획 관리"), XafDisplayName("원자재 투입 등록")]
    [DefaultListViewOptions(MasterDetailMode.ListViewOnly, true, NewItemRowPosition.None)]
    [RuleCriteria("MaterialInputQuantity_LessThanOrEqualTo_InputQuantity", DefaultContexts.Save, "MaterialInputQuantity + AlreadyInputQuantity <= InputQuantity", CustomMessageTemplate = "투입 수량은 총 원자재 총 필요 수량을 초과할 수 없습니다.")]
    [RuleCriteria("MaterialInputQuantity_LessThanOrEqualTo_StockQuantity", DefaultContexts.Save, "MaterialInputQuantity <= StockQuantity", CustomMessageTemplate = "투입 수량은 Lot 재고 수량을 초과할 수 없습니다.")]
    public class MaterialInputResult : BaseObject
    {
        #region Properties
        [Index(0)]
        [VisibleInLookupListView(true)]
        [ModelDefault("AllowEdit", "False")]
        [XafDisplayName("생산계획 번호"), ToolTip("생산계획 번호")]
        public string ProductionPlanningNumber
        {
            get { return DetailWorkInstructionObject?.WorkInstructionNumber; }
        }

        [Index(1)]
        [VisibleInLookupListView(true)]
        [ImmediatePostData(true)]
        [DataSourceCriteria("Progress.MinorCode == 'Proceeding' && IsFinalWorkProcess == False && RoutingIndex == 1")]
        [ModelDefault("LookupProperty", nameof(DetailWorkInstruction.WorkInstructionNumber))]
        [LookupEditorMode(LookupEditorMode.AllItemsWithSearch)]
        [RuleRequiredField(CustomMessageTemplate = "작업 지시 번호를 입력하세요.")]
        [XafDisplayName("작업 지시 번호"), ToolTip("작업 지시 번호")]
        public DetailWorkInstruction DetailWorkInstructionObject
        {
            get { return GetPropertyValue<DetailWorkInstruction>(nameof(DetailWorkInstructionObject)); }
            set { SetPropertyValue(nameof(DetailWorkInstructionObject), value); }
        }

        [VisibleInLookupListView(true)]
        [ImmediatePostData(true)]
        [ModelDefault("LookupProperty", nameof(Item.ItemName))]
        [DataSourceProperty(nameof(GetItemList))]
        [LookupEditorMode(LookupEditorMode.AllItemsWithSearch)]
        [RuleRequiredField(CustomMessageTemplate = "투입할 원자재를 입력하세요.")]
        [XafDisplayName("투입 원자재"), ToolTip("투입 원자재")]
        public Item ItemObject
        {
            get { return GetPropertyValue<Item>(nameof(ItemObject)); }
            set { SetPropertyValue(nameof(ItemObject), value); }
        }

        [VisibleInLookupListView(true)]
        [ImmediatePostData(true)]
        [DataSourceProperty(nameof(AvailableLots))]
        [ModelDefault("LookupProperty", nameof(Lot.LotNumber))]
        [LookupEditorMode(LookupEditorMode.AllItemsWithSearch)]
        [RuleRequiredField(CustomMessageTemplate = "Lot 번호를 입력하세요.")]
        [XafDisplayName("Lot 번호"), ToolTip("Lot 번호")]
        public Lot LotObject
        {
            get { return GetPropertyValue<Lot>(nameof(LotObject)); }
            set { SetPropertyValue(nameof(LotObject), value); }
        }

        [VisibleInLookupListView(true)]
        [XafDisplayName("작업 지시 수량"), ToolTip("작업 지시 수량")]
        public double WorkInstructionQuantity
        {
            get { return DetailWorkInstructionObject?.WorkInstructionQuantity ?? 0; }
        }

        [VisibleInLookupListView(true)]
        [ModelDefault("EditMask", "###,###,###,###,###,###,###,###,###,##0.0")]
        [ModelDefault("AllowEdit", "false")]
        [XafDisplayName("lot 재고 수량"), ToolTip("lot 재고 수량")]
        public double StockQuantity
        {
            get
            {
                return this.LotObject?.StockQuantity ?? 0;
            }
        }

        [VisibleInLookupListView(true)]
        [ModelDefault("EditMask", "###,###,###,###,###,###,###,###,###,##0.0")]
        [ModelDefault("AllowEdit", "false")]
        [XafDisplayName("총 재고 수량"), ToolTip("같은 품목의 총 재고 수량 입니다.")]
        public double TotalStockQuantity
        {
            get
            {
                double stockQuantity = new XPCollection<Lot>(this.Session)
                    .Where(x => x.ItemObject.Oid == this.LotObject?.ItemObject?.Oid)
                    .Sum(x => x.StockQuantity);
                return stockQuantity;
            }
        }

        [VisibleInLookupListView(true)]
        [ModelDefault("EditMask", "###,###,###,###,###,###,###,###,###,##0.0")]
        [ModelDefault("AllowEdit", "false")]
        [XafDisplayName("BOM 수량"), ToolTip(" BOM 수량")]
        public double BOMQuantity
        {
            get
            {
                var productBOMItemOid = DetailWorkInstructionObject?.MasterWorkInstructionObject?.MasterProductionPlanningObject?.ItemObject?.Oid;
                var inputItemOid = ItemObject?.Oid;

                if (productBOMItemOid == null || inputItemOid == null) return 0;

                // 최신 BOM 1개 가져오기
                var productBOM = new XPCollection<ProductBOM>(this.Session)
                    .Where(x => x.ItemObject.Oid == productBOMItemOid)
                    .OrderByDescending(x => x.BOMNumber)
                    .FirstOrDefault();

                if (productBOM == null) return 0;

                // BOM 구성 중 선택된 투입 원자재를 찾아 BOM 수량 가져오기
                var bomQuantity = productBOM.AssemblyBOMObjects
                    .FirstOrDefault(x => x.ItemObject.Oid == inputItemOid)?.BOMQuantity ?? 0;

                return bomQuantity;
            }
        }

        [VisibleInLookupListView(true)]
        [ModelDefault("EditMask", "###,###,###,###,###,###,###,###,###,##0.0")]
        [ModelDefault("AllowEdit", "false")]
        [XafDisplayName("원자재 총 필요 수량"), ToolTip("작업지시 수량 × BOM 수량")]
        public double InputQuantity
        {
            get
            {
                var workInstructionQuantity = DetailWorkInstructionObject?.WorkInstructionQuantity ?? 0;
                var productBOMItemOid = DetailWorkInstructionObject?.MasterWorkInstructionObject?.MasterProductionPlanningObject?.ItemObject?.Oid;
                var inputItemOid = ItemObject?.Oid;

                if (productBOMItemOid == null || inputItemOid == null) return 0;

                // 최신 BOM 1개 가져오기
                var productBOM = new XPCollection<ProductBOM>(this.Session)
                    .Where(x => x.ItemObject.Oid == productBOMItemOid)
                    .OrderByDescending(x => x.BOMNumber)
                    .FirstOrDefault();

                if (productBOM == null) return 0;

                // BOM 구성 중 선택된 투입 원자재를 찾아 BOM 수량 가져오기
                var bomQuantity = productBOM.AssemblyBOMObjects
                    .FirstOrDefault(x => x.ItemObject.Oid == inputItemOid)?.BOMQuantity ?? 0;

                return workInstructionQuantity * bomQuantity;
            }
        }

        [VisibleInLookupListView(true)]
        [ModelDefault("EditMask", "###,###,###,###,###,###,###,###,###,##0.###")]
        [XafDisplayName("투입된 수량"), ToolTip("투입된 수량")]
        public double AlreadyInputQuantity
        {
            get
            {
                if (this.DetailWorkInstructionObject == null || this.DetailWorkInstructionObject.Oid == Guid.Empty)
                    return 0;

                // 투입된 품목별 투입 수량 조회
                var allInputs = new XPCollection<MaterialInputResult>(this.Session) 
                        .Where(x => x.DetailWorkInstructionObject != null && 
                                    x.DetailWorkInstructionObject.Oid == this.DetailWorkInstructionObject.Oid && 
                                    x.Oid != this.Oid)
                        .GroupBy(x => x.ItemObject)
                        .ToDictionary(g => g.Key, g => g.Sum(x => x.MaterialInputQuantity));

                if (allInputs.Count == 0) return 0;

                if (this.ItemObject != null)
                {
                    double alreadyInputQty = allInputs.ContainsKey(this.ItemObject) ? allInputs[this.ItemObject] : 0;
                    return alreadyInputQty;
                }
                return 0;
            }
        }

        [VisibleInLookupListView(true)]
        [ModelDefault("EditMask", "###,###,###,###,###,###,###,###,###,##0.###")]
        [XafDisplayName("투입 수량"), ToolTip("투입 수량")]
        public double MaterialInputQuantity
        {
            get { return GetPropertyValue<double>(nameof(MaterialInputQuantity)); }
            set { SetPropertyValue(nameof(MaterialInputQuantity), value); }
        }

        [VisibleInLookupListView(true)]
        [ModelDefault("DisplayFormat", "yyyy/MM/dd")]
        [ModelDefault("DisplayFormat", "yyyy/MM/dd HH:mm:ss")]
        [XafDisplayName("투입 일시"), ToolTip("투입 일시")]
        public DateTime MaterialInputDateTime
        {
            get { return GetPropertyValue<DateTime>(nameof(MaterialInputDateTime)); }
            set { SetPropertyValue(nameof(MaterialInputDateTime), value); }
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
        public MaterialInputResult(Session session) : base(session) { }
        #endregion

        #region Methods
        public override void AfterConstruction()
        {
            base.AfterConstruction();

            MaterialInputDateTime = DateTime.Now;
            CreatedDateTime = DateTime.Now;
        }

        protected override void OnChanged(string propertyName, object oldValue, object newValue)
        {
            base.OnChanged(propertyName, oldValue, newValue);

            switch (propertyName)
            {
                case nameof(DetailWorkInstructionObject):
                case nameof(ItemObject):
                    LotObject = null;
                    OnChanged(nameof(LotObject));
                    OnChanged(nameof(AvailableLots));
                    break;
                default:
                    break;
            }
        }

        [Browsable(false)]
        private List<Item> GetItemList
        {
            get
            {
                if (this.DetailWorkInstructionObject != null)
                {
                    // 생산 계획에서 ItemObject 가져오기
                    var itemOid = DetailWorkInstructionObject?.MasterWorkInstructionObject?.MasterProductionPlanningObject?.ItemObject?.Oid;

                    if (itemOid == null) return null;

                    // 레벨? 최신? 버전 BOM 가져오기
                    var recentBOM = new XPCollection<ProductBOM>(this.Session)
                            .Where(x => x.ItemObject.Oid == itemOid)
                            .OrderByDescending(x => x.BOMNumber)
                            .FirstOrDefault();

                    if (recentBOM == null) return null;

                    // 하위 원자재들 가져오기
                    var BOMItems = recentBOM.AssemblyBOMObjects
                            .Where(x => x.ItemObject != null && x.IsEnabled)
                            .Select(x => x.ItemObject)
                            .Distinct()
                            .ToList();

                    // 원자재별 투입량 합산
                    var allInputs = new XPCollection<MaterialInputResult>(this.Session)
                            .Where(x => x.DetailWorkInstructionObject.Oid == this.DetailWorkInstructionObject.Oid && x.Oid != this.Oid)
                            .GroupBy(x => x.ItemObject)
                            .ToDictionary(g => g.Key, g => g.Sum(x => x.MaterialInputQuantity));


                    var finalList = new List<Item>();

                    foreach (var bom in recentBOM.AssemblyBOMObjects)
                    {
                        if (!bom.IsEnabled || bom.ItemObject == null)
                            continue;

                        var item = bom.ItemObject; // 품목
                        double bomQty = bom.BOMQuantity;// BOM 수량
                        // 계획 수량 * BOM 수량 => 총 필요 수량
                        double requiredQty = (DetailWorkInstructionObject?.WorkInstructionQuantity ?? 0) * bomQty;

                        // 이미 투입된 수량
                        double alreadyInputQty = allInputs.ContainsKey(item) ? allInputs[item] : 0;

                        // 이미 투입된 수량이 총 필요 수량보다 적다면 추가 투입 가능
                        if (alreadyInputQty < requiredQty)
                            finalList.Add(item);
                    }

                    return finalList;
                }
                return null;
            }
        }

        [Browsable(false)]
        public List<Lot> AvailableLots
        {
            get
            {
                if (ItemObject == null) return null;

                // 재고가 0이 아닌 원자재 Lot들 가져오기
                return new XPCollection<Lot>(this.Session)
                    .Where(x => x.ItemObject.Oid == ItemObject.Oid && x.StockQuantity > 0)
                    .ToList();
            }
        }
        #endregion
    }
}