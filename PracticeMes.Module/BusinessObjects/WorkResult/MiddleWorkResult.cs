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

    [Browsable(false)]
    public List<DetailWorkInstruction> AvailableInstructionObjects
    {
        get
        {
            var allInstructionsFull = new XPCollection<DetailWorkInstruction>(this.Session).ToList();

            // 진행중, 중간공정
            var currentInstructions = allInstructionsFull
                .Where(x => x.Progress?.MinorCode == "Proceeding" && x.IsFinalWorkProcess == false)
                .ToList();

            return currentInstructions;
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
    [XafDisplayName("생산 가능 수량"), ToolTip("생산 가능 수량")]
    public double AvailableGoodQuantity
    {
        get
        {
            // 작업지시마스터 지시수량 == 작업지시디테일 지시수량
            if (this.DetailWorkInstructionQuantity == this.WorkInstructionQuantity)
            {
                var presentAvaliableStockQuantity = 0.0; // 현재공정 생산된 재고를 담을 변수
                var pastAvaliableStockQuantity = 0.0; // 이전공정 생산된 재고를 담을 변수

                // 현재 라우팅의 생산수량 합계
                var presentRoutings = new XPCollection<MiddleWorkResult>(this.Session)
                                       .Where(x => ((x.DetailWorkInstructionObject?.RoutingIndex ?? 0) == ((this.DetailWorkInstructionObject?.RoutingIndex ?? 0)))
                                       && x.DetailWorkInstructionObject?.MasterWorkInstructionObject?.Oid == this.DetailWorkInstructionObject?.MasterWorkInstructionObject?.Oid).ToList();
                if (presentRoutings.Count != 0)
                {

                    foreach (var presentRouting in presentRoutings)
                    {
                        presentAvaliableStockQuantity += presentRouting.AssySerialProductObjects?.Count ?? 0;
                    }
                }

                // 이전 공정 라우팅의 생산 수량 합계
                var pastRoutings = new XPCollection<MiddleWorkResult>(this.Session)
                                       .Where(x => ((x.DetailWorkInstructionObject?.RoutingIndex ?? 0) == ((this.DetailWorkInstructionObject?.RoutingIndex - 1 ?? 0)))
                                       && x.DetailWorkInstructionObject?.MasterWorkInstructionObject?.Oid == this.DetailWorkInstructionObject?.MasterWorkInstructionObject?.Oid).ToList();
                if (pastRoutings.Count != 0)
                {
                    if (pastRoutings.Any(x => x.DetailWorkInstructionQuantity != x.WorkInstructionQuantity))
                    {
                        // 이전 공정이 양품이 없으면 다음공정부터 생산 불가능 1개라도 있으면 다음공정부터 처음부터 시리얼 발행 가능
                        pastAvaliableStockQuantity = pastRoutings.SelectMany(x => x.AssySerialProductObjects ?? Enumerable.Empty<AssySerialProduct>()).Sum(x => x.GoodQuantity) == 0 ? 0 : DetailWorkInstructionQuantity;

                    }
                    else
                    {
                        foreach (var pastRouting in pastRoutings)
                        {
                            pastAvaliableStockQuantity += pastRouting.AssySerialProductObjects?.Count ?? 0;
                        }
                    }
                }

                if (this.DetailWorkInstructionObject?.RoutingIndex == 1) // 첫번째 공정일때
                {
                    if (presentRoutings.Count != 0)
                    {
                        return DetailWorkInstructionQuantity - presentAvaliableStockQuantity; // 첫번째 공정이나 생산을 진행한 이력이 있는 경우
                    }
                    else
                    {
                        return DetailWorkInstructionQuantity; // 첫번째 공정이면서 생산도 하나도 안한상태
                    }
                }
                else
                {
                    if (presentRoutings.Count != 0)
                    {
                        return pastAvaliableStockQuantity - presentAvaliableStockQuantity; // 첫번째 공정이 아니고 생산을 진행한 이력이 있는 경우
                    }
                    else
                    {
                        return pastAvaliableStockQuantity; // 첫번째 공정이 아니고 생산을 진행한 적이 없을때, 
                    }
                }
            }
            else // 작업지시 상세와 작업지시 마스터의 수량이 다른경우
            {
                var presentAvaliableStockQuantity = 0.0; // 현재공정 생산된 재고를 담을 변수

                // 현재 라우팅의 생산수량 합계
                var presentRoutings = new XPCollection<MiddleWorkResult>(this.Session)
                                       .Where(x => ((x.DetailWorkInstructionObject?.RoutingIndex ?? 0) == ((this.DetailWorkInstructionObject?.RoutingIndex ?? 0)))
                                       && x.DetailWorkInstructionObject?.MasterWorkInstructionObject?.Oid == this.DetailWorkInstructionObject?.MasterWorkInstructionObject?.Oid).ToList();
                if (presentRoutings.Count != 0)
                {
                    foreach (var presentRouting in presentRoutings)
                    {
                        presentAvaliableStockQuantity += presentRouting?.GoodQuantity ?? 0;
                    }
                }

                if (presentRoutings.Count != 0)
                {
                    return DetailWorkInstructionQuantity - presentAvaliableStockQuantity; // 첫번째 공정이나 생산을 진행한 이력이 있는 경우
                }
                else
                {
                    return DetailWorkInstructionQuantity; // 첫번째 공정이면서 생산도 하나도 안한상태
                }
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

    [Association(@"AssySerialProductReferencesMiddleWorkResult"), DevExpress.Xpo.Aggregated]
    public XPCollection<AssySerialProduct> AssySerialProductObjects { get { return GetCollection<AssySerialProduct>(nameof(AssySerialProductObjects)); } }

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