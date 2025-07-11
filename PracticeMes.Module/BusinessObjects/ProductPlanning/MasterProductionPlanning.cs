﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using DevExpress.CodeParser;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using PracticeMes.Module.BusinessObjects.BaseInfo.CommonInfo;
using PracticeMes.Module.BusinessObjects.BaseInfo.CommonInfol;
using PracticeMes.Module.BusinessObjects.BaseInfo.ItemInfo;
using PracticeMes.Module.BusinessObjects.BaseInfo.ProductionInfo;
using PracticeMes.Module.BusinessObjects.Sales;

namespace PracticeMes.Module.BusinessObjects.ProductPlanning;

[DefaultClassOptions]
[NavigationItem("생산 계획 관리"), XafDisplayName("생산 계획 등록")]
[DefaultListViewOptions(MasterDetailMode.ListViewOnly, true, NewItemRowPosition.None)]
public class MasterProductionPlanning : BaseObject
{
    #region Properties
    [Index(0)]
    [VisibleInLookupListView(true)]
    [ModelDefault("AllowEdit", "False")]
    [XafDisplayName("생산계획 번호"), ToolTip("생산계획 번호")]
    public string ProductionPlanningNumber
    {
        get { return GetPropertyValue<string>(nameof(ProductionPlanningNumber)); }
        set { SetPropertyValue(nameof(ProductionPlanningNumber), value); }
    }

    [Index(1)]
    [VisibleInLookupListView(true)]
    [ImmediatePostData(true)]
    [ModelDefault("LookupProperty", nameof(DetailSalesOrder.SalesOrderNumber))]
    [LookupEditorMode(LookupEditorMode.AllItemsWithSearch)]
    [XafDisplayName("수주 번호"), ToolTip("수주 번호")]
    public DetailSalesOrder DetailSalesOrderObject
    {
        get { return GetPropertyValue<DetailSalesOrder>(nameof(DetailSalesOrderObject)); }
        set { SetPropertyValue(nameof(DetailSalesOrderObject), value); }
    }

    [VisibleInLookupListView(true)]
    [DataSourceCriteria("IsEnabled == True")]
    [ModelDefault("LookupProperty", nameof(BusinessPartner.BusinessPartnerName))]
    [LookupEditorMode(LookupEditorMode.AllItemsWithSearch)]
    [RuleRequiredField(CustomMessageTemplate = "거래처를 입력하세요.")]
    [XafDisplayName("거래처/수주처"), ToolTip("거래처/수주처")]
    public BusinessPartner BusinessPartnerObject
    {
        get { return GetPropertyValue<BusinessPartner>(nameof(BusinessPartnerObject)); }
        set { SetPropertyValue(nameof(BusinessPartnerObject), value); }
    }

    [ImmediatePostData(true)]
    [DataSourceCriteria("IsEnabled == True")]
    [ModelDefault("LookupProperty", nameof(Factory.FactoryName))]
    [LookupEditorMode(LookupEditorMode.AllItemsWithSearch)]
    [RuleRequiredField(CustomMessageTemplate = "공장을 입력하세요.")]
    [XafDisplayName("공장"), ToolTip("공장")]
    public Factory FactoryObject
    {
        get { return GetPropertyValue<Factory>(nameof(FactoryObject)); }
        set { SetPropertyValue(nameof(FactoryObject), value); }
    }

    [ImmediatePostData(true)]
    [DataSourceCriteria("IsEnabled == True")]
    [ModelDefault("LookupProperty", nameof(WorkCenter.WorkCenterName))]
    [LookupEditorMode(LookupEditorMode.AllItemsWithSearch)]
    [RuleRequiredField(CustomMessageTemplate = "작업장을 입력하세요.")]
    [XafDisplayName("작업장"), ToolTip("작업장")]
    public WorkCenter WorkCenterObject
    {
        get { return GetPropertyValue<WorkCenter>(nameof(WorkCenterObject)); }
        set { SetPropertyValue(nameof(WorkCenterObject), value); }
    }


    [VisibleInLookupListView(true)]
    [XafDisplayName("수주수량"), ToolTip("수주수량")]
    public double? SalesOrderQuantity
    {
        get { return DetailSalesOrderObject?.SalesOrderQuantity; }
    }

    [VisibleInLookupListView(true)]
    [RuleValueComparison(ValueComparisonType.GreaterThanOrEqual, 0, CustomMessageTemplate = "계획수량은 0 이상이어야 합니다.")]
    [XafDisplayName("계획수량"), ToolTip("계획수량")]
    public double ProductPlanningQuantity
    {
        get { return GetPropertyValue<double>(nameof(ProductPlanningQuantity)); }
        set { SetPropertyValue(nameof(ProductPlanningQuantity), value); }
    }

    [ImmediatePostData(true)]
    [ModelDefault("DisplayFormat", "yyyy/MM/dd")]
    [RuleRequiredField(CustomMessageTemplate = "시작 예정 일시를 입력하세요.")]
    [XafDisplayName("시작 예정 일시"), ToolTip("시작 예정 일시")]
    public DateTime StartDateTime
    {
        get { return GetPropertyValue<DateTime>(nameof(StartDateTime)); }
        set { SetPropertyValue(nameof(StartDateTime), value); }
    }

    [ModelDefault("DisplayFormat", "yyyy/MM/dd")]
    [RuleRequiredField(CustomMessageTemplate = "종료 예정 일시를 입력하세요.")]
    [XafDisplayName("종료 예정 일시"), ToolTip("종료 예정 일시")]
    public DateTime EndDateTime
    {
        get { return GetPropertyValue<DateTime>(nameof(EndDateTime)); }
        set { SetPropertyValue(nameof(EndDateTime), value); }
    }

    [Index(2)]
    [ImmediatePostData(true)]
    [DataSourceCriteria("IsEnabled == True && (ItemAccountObject.ItemAccountName == '제품' || ItemAccountObject.ItemAccountName == '반제품')")]
    [ModelDefault("LookupProperty", nameof(Item.ItemName))]
    [LookupEditorMode(LookupEditorMode.AllItemsWithSearch)]
    [RuleRequiredField(CustomMessageTemplate = "품목 이름을 입력하세요.")]
    [XafDisplayName("품목 이름"), ToolTip("품목 이름")]
    public Item ItemObject
    {
        get { return GetPropertyValue<Item>(nameof(ItemObject)); }
        set { SetPropertyValue(nameof(ItemObject), value); }
    }

    [Index(3)]
    [VisibleInLookupListView(true)]
    [XafDisplayName("품목 코드"), ToolTip("품목 코드")]
    public string ItemCode
    {
        get { return ItemObject?.ItemCode; }
    }

    [Index(4)]
    [VisibleInLookupListView(true)]
    [XafDisplayName("품목 유형"), ToolTip("품목 유형")]
    public string ItemAccountObject
    {
        get { return ItemObject?.ItemAccountObject?.ItemAccountName; }
    }

    [ModelDefault("AllowEdit", "False")]
    [DataSourceCriteria("IsEnabled == True")]
    [ModelDefault("LookupProperty", nameof(Unit.UnitName))]
    [LookupEditorMode(LookupEditorMode.AllItemsWithSearch)]
    [RuleRequiredField(CustomMessageTemplate = "단위를 입력하세요.")]
    [XafDisplayName("단위"), ToolTip("단위")]
    public Unit UnitObject
    {
        get { return GetPropertyValue<Unit>(nameof(UnitObject)); }
        set { SetPropertyValue(nameof(UnitObject), value); }
    }

    [VisibleInLookupListView(true)]
    [ModelDefault("AllowEdit", "False")]
    [XafDisplayName("마감"), ToolTip("마감")]
    public bool IsComplete
    {
        get { return GetPropertyValue<bool>(nameof(IsComplete)); }
        set { SetPropertyValue(nameof(IsComplete), value); }
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

    // 마감칠때 플래그
    [Browsable(false)]
    [NonPersistent]
    public bool IsCompleting
    {
        get { return GetPropertyValue<bool>(nameof(IsCompleting)); }
        set { SetPropertyValue(nameof(IsCompleting), value); }
    }

    [XafDisplayName("BOM 목록")]
    [Association(@"DetailProductionPlanningRefernencesMasterProductionPlanning"), DevExpress.Xpo.Aggregated]
    public XPCollection<DetailProductionPlanning> DetailProductionPlannings { get { return GetCollection<DetailProductionPlanning>(nameof(DetailProductionPlannings)); } }
    #endregion

    #region Constructors
    public MasterProductionPlanning(Session session) : base(session) { }
    #endregion

    #region Fields
    private bool isDeleting;
    #endregion

    #region Methods
    public override void AfterConstruction()
    {
        base.AfterConstruction();

        InitialValueSetting();
        CreateProductionPlanningNumber();
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
            case nameof(DetailSalesOrderObject):
                ItemObject = DetailSalesOrderObject?.ItemObject;
                FactoryObject = DetailSalesOrderObject?.MasterSalesOrderObject?.FactoryObject;
                BusinessPartnerObject = DetailSalesOrderObject?.MasterSalesOrderObject?.BusinessPartnerObject;
                UnitObject = DetailSalesOrderObject?.SalesOrderUnit;
                break;
            case nameof(ItemObject):
                this.UnitObject = ItemObject?.UnitObject;
                break;
            default:
                break;
        }
    }

    // 초기값 세팅
    private void InitialValueSetting()
    {
        // 공장 기본 세팅
        if (FactoryObject == null)
        {
            var firstFactory = Session.Query<Factory>()
                                      .Where(x => x.IsEnabled)
                                      .OrderBy(x => x.FactoryName)
                                      .FirstOrDefault();

            FactoryObject = firstFactory;
        }

        StartDateTime = DateTime.Now;
        CreatedDateTime = DateTime.Now;
    }

    // 생산 계획 번호 생성 (수주 번호랑 방식 같음)
    private void CreateProductionPlanningNumber()
    {
        var productPlanningDateTime = DateTime.Now.ToString("yyyyMMdd"); ;
        var productPlanningObject = new XPCollection<MasterProductionPlanning>(Session)
            .Where(x => x.ProductionPlanningNumber.StartsWith($"{productPlanningDateTime}"));
        string suffix = "001";

        if (productPlanningObject.Any())
        {
            suffix = (productPlanningObject.Select(s => int.Parse(s.ProductionPlanningNumber
                .Split('-')
                .Last()))
                .Max() + 1)
                .ToString("000");
        }
        this.ProductionPlanningNumber = $"{productPlanningDateTime}-{suffix}";
    }
    #endregion
}