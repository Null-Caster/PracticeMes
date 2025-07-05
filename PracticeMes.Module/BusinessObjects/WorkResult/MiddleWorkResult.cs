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
using PracticeMes.Module.BusinessObjects.BaseInfo.CommonInfo;
using PracticeMes.Module.BusinessObjects.BaseInfo.ItemInfo;
using PracticeMes.Module.BusinessObjects.Meterial;
using PracticeMes.Module.BusinessObjects.ProductPlanning;

namespace PracticeMes.Module.BusinessObjects.WorkResult;

[DefaultClassOptions]
[NavigationItem("공정 관리"), XafDisplayName("중간 공정 실적 등록")]
[DefaultListViewOptions(MasterDetailMode.ListViewOnly, true, NewItemRowPosition.Top)]
public class MiddleWorkResult : BaseObject
{
    #region Properties
    [ImmediatePostData(true)]
    [VisibleInLookupListView(true)]
    [DataSourceProperty(nameof(AvailableInstructionObjects))]
    [ModelDefault("LookupProperty", nameof(DetailWorkInstruction.WorkInstructionNumber))]
    [LookupEditorMode(LookupEditorMode.AllItemsWithSearch)]
    [RuleRequiredField(CustomMessageTemplate = "작업 지시 번호를 입력하세요.")]
    [XafDisplayName("작업 지시 번호"), ToolTip("작업 지시 번호")]
    public DetailWorkInstruction DetailWorkInstructionObject
    {
        get { return GetPropertyValue<DetailWorkInstruction>(nameof(DetailWorkInstructionObject)); }
        set { SetPropertyValue(nameof(DetailWorkInstructionObject), value); }
    }

    // 작업 가능한 상세 작업지시 목록 반환
    // 조건: 상태가 Proceeding이고, 마지막 공정이 아닌 항목
    [Browsable(false)]
    public List<DetailWorkInstruction> AvailableInstructionObjects
    {
        get
        {
            var allInstructions = new XPCollection<DetailWorkInstruction>(this.Session).ToList();

            var currentInstructions = allInstructions
                .Where(x => x.Progress?.MinorCode == "Proceeding" && x.IsFinalWorkProcess == false)
                .ToList();

            var minRoutingIndex = allInstructions
                .GroupBy(x => x.MasterWorkInstructionObject)
                .ToDictionary(
                    g => g.Key,
                    g => g.Min(x => x.RoutingIndex)
                );

            var alreadyUsedInstructionOids = new XPCollection<MiddleWorkResult>(this.Session)
                .Select(x => x.DetailWorkInstructionObject.Oid)
                .Distinct()
                .ToList();

            var result = new List<DetailWorkInstruction>();

            foreach (var instruction in currentInstructions)
            {
                if (alreadyUsedInstructionOids.Contains(instruction.Oid))
                    continue;

                var master = instruction.MasterWorkInstructionObject;
                if (master == null) continue;

                bool isFirstRouting = minRoutingIndex.TryGetValue(master, out int minRouting) &&
                                      instruction.RoutingIndex == minRouting;

                if (isFirstRouting)
                {
                    var itemOid = instruction.MasterWorkInstructionObject?
                        .MasterProductionPlanningObject?.ItemObject?.Oid;

                    if (itemOid == null) continue;

                    var productBOM = new XPCollection<ProductBOM>(this.Session)
                        .Where(x => x.ItemObject.Oid == itemOid)
                        .OrderByDescending(x => x.BOMNumber)
                        .FirstOrDefault();

                    if (productBOM == null) continue;

                    var requiredItemOids = productBOM.AssemblyBOMObjects
                        .Where(x => x.IsEnabled && x.ItemObject != null)
                        .Select(x => x.ItemObject.Oid)
                        .Distinct()
                        .ToList();

                    var inputItemOids = new XPCollection<MaterialInputResult>(this.Session)
                        .Where(x => x.DetailWorkInstructionObject.Oid == instruction.Oid)
                        .Select(x => x.ItemObject.Oid)
                        .Distinct()
                        .ToList();

                    if (requiredItemOids.All(x => inputItemOids.Contains(x)))
                    {
                        result.Add(instruction);
                    }
                }
                else
                {
                    // 첫 공정이 아니면 조건만 맞으면 바로 추가
                    result.Add(instruction);
                }
            }

            return result;  // ✅ 루프 끝난 후에 result 반환
        }
    }

    [VisibleInLookupListView(true)]
    [XafDisplayName("작업 공정 이름"), ToolTip("작업 공정 이름")]
    public string WorkProcessName
    {
        get { return DetailWorkInstructionObject?.WorkProcessObject.WorkProcessName; }
    }

    [VisibleInLookupListView(true)]
    [XafDisplayName("품목코드"), ToolTip("품목코드")]
    public string ItemCode
    {
        get { return DetailWorkInstructionObject?.MasterWorkInstructionObject?.MasterProductionPlanningObject?.ItemObject?.ItemCode; }
    }

    [VisibleInLookupListView(true)]
    [XafDisplayName("품목명칭"), ToolTip("품목명칭")]
    public string ItemName
    {
        get { return DetailWorkInstructionObject?.MasterWorkInstructionObject?.MasterProductionPlanningObject?.ItemObject?.ItemName; }
    }

    [VisibleInLookupListView(true)]
    [XafDisplayName("공장"), ToolTip("공장")]
    public string FactoryName
    {
        get { return DetailWorkInstructionObject?.MasterWorkInstructionObject?.MasterProductionPlanningObject?.FactoryObject?.FactoryName; }
    }

    [VisibleInLookupListView(true)]
    [DataSourceCriteria("IsEnabled == True")]
    [ModelDefault("LookupProperty", nameof(WareHouse.WareHouseName))]
    [LookupEditorMode(LookupEditorMode.AllItemsWithSearch)]
    [RuleRequiredField(CustomMessageTemplate = "입고 창고를 입력하세요.")]
    [XafDisplayName("입고 창고"), ToolTip("입고 창고")]
    public WareHouse WareHouseObject
    {
        get { return GetPropertyValue<WareHouse>(nameof(WareHouseObject)); }
        set { SetPropertyValue(nameof(WareHouseObject), value); }
    }

    [VisibleInLookupListView(true)]
    [ModelDefault("LookupProperty", nameof(Employee.EmployeeName))]
    [LookupEditorMode(LookupEditorMode.AllItemsWithSearch)]
    [RuleRequiredField(CustomMessageTemplate = "작업자를 입력하세요.")]
    [XafDisplayName("작업자"), ToolTip("작업자")]
    public Employee EmployeeObject
    {
        get { return GetPropertyValue<Employee>(nameof(EmployeeObject)); }
        set { SetPropertyValue(nameof(EmployeeObject), value); }
    }

    [VisibleInLookupListView(true)]
    [ModelDefault("DisplayFormat", "yyyy/MM/dd")]
    [RuleRequiredField(CustomMessageTemplate = "작업 일시를 입력하세요.")]
    [XafDisplayName("작업 일시"), ToolTip("작업 일시")]
    public DateTime WorkResultDateTime
    {
        get { return GetPropertyValue<DateTime>(nameof(WorkResultDateTime)); }
        set { SetPropertyValue(nameof(WorkResultDateTime), value); }
    }

    [Browsable(false)]
    [VisibleInLookupListView(true)]
    [ModelDefault("EditMask", "###,###,###,###,###,###,###,###,###,##0.###")]
    [XafDisplayName("작업 지시 수량"), ToolTip("작업 지시 수량")]
    public double WorkInstructionQuantity
    {
        get { return DetailWorkInstructionObject?.MasterWorkInstructionObject?.WorkInstructionQuantity ?? 0; }
    }

    [VisibleInLookupListView(true)]
    [ModelDefault("EditMask", "###,###,###,###,###,###,###,###,###,##0.###")]
    [XafDisplayName("작업 상세 지시 수량"), ToolTip("작업 상세 지시 수량")]
    public double DetailWorkInstructionQuantity
    {
        get { return DetailWorkInstructionObject?.WorkInstructionQuantity ?? 0; }
    }

    [RuleFromBoolProperty("ValidateGoodQuantityLimit", DefaultContexts.Save,"생산 수량은 생산 가능 수량을 초과할 수 없습니다.")]
    public bool IsGoodQuantityValid => GoodQuantity <= AvailableGoodQuantity;

    [VisibleInLookupListView(true)]
    [ModelDefault("EditMask", "###,###,###,###,###,###,###,###,###,##0.###")]
    [XafDisplayName("생산 수량"), ToolTip("생산 수량")]
    public double GoodQuantity
    {
        get { return GetPropertyValue<double>(nameof(GoodQuantity)); }
        set { SetPropertyValue(nameof(GoodQuantity), value); }
    }

    [VisibleInLookupListView(true)]
    [ModelDefault("EditMask", "###,###,###,###,###,###,###,###,###,##0.###")]
    [XafDisplayName("생산 가능 수량"), ToolTip("투입 자재 또는 앞 공정 기준으로 생산 가능한 수량")]
    public double AvailableGoodQuantity
    {
        get
        {
            if (DetailWorkInstructionObject == null) return 0;

            // 작업 지시 가져오기
            var master = DetailWorkInstructionObject.MasterWorkInstructionObject;
            if (master == null)  return 0;

            // 작업지시 내 모든 상세 목록 조회
            var allInstructions = new XPCollection<DetailWorkInstruction>(Session)
                .Where(x => x.MasterWorkInstructionObject == master)
                .ToList();

            // 가장 빠른 공정(RoutingIndex) 구하기, 원재료 투입 공정
            int minRoutingIndex = allInstructions.Min(x => x.RoutingIndex);
            // 현재 사용자가 고른 공정
            int currentRoutingIndex = DetailWorkInstructionObject.RoutingIndex;

            // 3. 현재 공정이 첫 번째 공정일 경우 → 원자재 투입 확인 및 기준 계산
            if (currentRoutingIndex == minRoutingIndex)
            {
                var item = master?.MasterProductionPlanningObject?.ItemObject;
                if (item == null) return 0;

                var bom = new XPCollection<ProductBOM>(Session)
                    .Where(x => x.ItemObject.Oid == item.Oid)
                    .OrderByDescending(x => x.BOMNumber)
                    .FirstOrDefault();

                if (bom == null) return 0;

                var requiredDict = bom.AssemblyBOMObjects
                    .Where(x => x.IsEnabled && x.ItemObject != null)
                    .GroupBy(x => x.ItemObject.Oid)
                    .ToDictionary(g => g.Key, g => g.Sum(x => x.BOMQuantity));

                if (requiredDict.Count == 0)
                    return 0;

                var inputDict = new XPCollection<MaterialInputResult>(Session)
                    .Where(x => x.DetailWorkInstructionObject.Oid == DetailWorkInstructionObject.Oid)
                    .GroupBy(x => x.ItemObject.Oid)
                    .ToDictionary(g => g.Key, g => g.Sum(x => x.MaterialInputQuantity));

                double minPossibleQuantity = double.MaxValue;

                foreach (var required in requiredDict)
                {
                    if (!inputDict.TryGetValue(required.Key, out double actualQty))
                        return 0;

                    if (required.Value == 0)
                        return 0;

                    double possible = actualQty / required.Value;
                    minPossibleQuantity = Math.Min(minPossibleQuantity, Math.Floor(possible));
                }

                return minPossibleQuantity == double.MaxValue ? 0 : minPossibleQuantity;
            }
            else
            {
                // 이전 공정의 생산 수량을 기준으로 생산 가능 수량 파악
                int prevRoutingIndex = currentRoutingIndex - 1;

                var prevInstruction = allInstructions
                    .FirstOrDefault(x => x.RoutingIndex == prevRoutingIndex);

                if (prevInstruction == null) return 0;

                var prevWorkResult = new XPCollection<MiddleWorkResult>(Session)
                    .Where(x => x.DetailWorkInstructionObject.Oid == prevInstruction.Oid)
                    .OrderByDescending(x => x.WorkResultDateTime)
                    .FirstOrDefault();

                return prevWorkResult?.GoodQuantity ?? 0;
            }
        }
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
    public MiddleWorkResult(Session session) : base(session) { }
    #endregion

    #region Methods
    public override void AfterConstruction()
    {
        base.AfterConstruction();
        WorkResultDateTime = DateTime.Now;
        CreatedDateTime = DateTime.Now;
    }

    protected override void OnSaving()
    {
        base.OnSaving();

        if (GoodQuantity <= 0)
            throw new UserFriendlyException("[생산 수량]은 0보다 커야 합니다.");

        if (GoodQuantity > AvailableGoodQuantity)
            throw new UserFriendlyException($"[생산 수량]은 [생산 가능 수량]({AvailableGoodQuantity})을 초과할 수 없습니다.");
    }

    protected override void OnChanged(string propertyName, object oldValue, object newValue)
    {
        base.OnChanged(propertyName, oldValue, newValue);
        if (Session.IsObjectsLoading)
        {
            return;
        }
        switch (propertyName)
        {
            case nameof(DetailWorkInstructionObject):
                if (DetailWorkInstructionObject is null || DetailWorkInstructionObject.Oid == Guid.Empty)
                {
                    return;
                }
                EmployeeObject = DetailWorkInstructionObject.EmployeeObject;
                WareHouseObject = DetailWorkInstructionObject.WareHouseObject;
                break;
            default:
                break;
        }
    }
    #endregion
}