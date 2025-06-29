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
using PracticeMes.Module.BusinessObjects.BaseInfo.CommonInfo;

namespace PracticeMes.Module.BusinessObjects.Sales;

[DefaultClassOptions]
[NavigationItem("영업 관리"), XafDisplayName("수주 등록")]
[DefaultListViewOptions(MasterDetailMode.ListViewOnly, true, NewItemRowPosition.Top)]
public class MasterSalesOrder : BaseObject
{
    #region Properties
    [VisibleInLookupListView(true)]
    [Appearance("MasterSalesOrderAppear1", FontColor = "Blue")]
    [ModelDefault("AllowEdit", "False")]
    [RuleUniqueValue(CustomMessageTemplate = "수주 번호가 중복되었습니다.")]
    [RuleRequiredField(CustomMessageTemplate = "수주 번호를 입력하세요.")]
    [XafDisplayName("수주 번호"), ToolTip("수주 번호")]
    public string SalesOrderNumber
    {
        get { return GetPropertyValue<string>(nameof(SalesOrderNumber)); }
        set { SetPropertyValue(nameof(SalesOrderNumber), value); }
    }

    [ImmediatePostData(true)]
    [VisibleInLookupListView(true)]
    [RuleRequiredField(CustomMessageTemplate = "수주 일자를 입력하세요.")]
    [XafDisplayName("수주 일자"), ToolTip("수주 일자")]
    public DateTime SalesOrderDateTime
    {
        get { return GetPropertyValue<DateTime>(nameof(SalesOrderDateTime)); }
        set { SetPropertyValue(nameof(SalesOrderDateTime), value); }
    }

    [ImmediatePostData(true)]
    [VisibleInLookupListView(true)]
    [DataSourceCriteria("IsEnabled == True")]
    [ModelDefault("LookupProperty", nameof(Factory.FactoryName))]
    [LookupEditorMode(LookupEditorMode.AllItemsWithSearch)]
    [RuleRequiredField(CustomMessageTemplate = "공장명을 입력하세요.")]
    [XafDisplayName("공장명"), ToolTip("공장명")]
    public Factory FactoryObject
    {
        get { return GetPropertyValue<Factory>(nameof(FactoryObject)); }
        set { SetPropertyValue(nameof(FactoryObject), value); }
    }

    [VisibleInLookupListView(true)]
    [DataSourceCriteria("IsEnabled == True")]
    [ModelDefault("LookupProperty", nameof(BusinessPartner.BusinessPartnerName))]
    [LookupEditorMode(LookupEditorMode.AllItemsWithSearch)]
    [RuleRequiredField(CustomMessageTemplate = "거래처를 입력하세요.")]
    [XafDisplayName("거래처"), ToolTip("거래처")]
    public BusinessPartner BusinessPartnerObject
    {
        get { return GetPropertyValue<BusinessPartner>(nameof(BusinessPartnerObject)); }
        set { SetPropertyValue(nameof(BusinessPartnerObject), value); }
    }

    [VisibleInLookupListView(true)]
    [DataSourceCriteria("IsEnabled == True")]
    [ModelDefault("LookupProperty", nameof(Employee.EmployeeName))]
    [LookupEditorMode(LookupEditorMode.AllItemsWithSearch)]
    [RuleRequiredField(CustomMessageTemplate = "담당자를 입력하세요.")]
    [XafDisplayName("담당자"), ToolTip("담당자")]
    public Employee EmployeeObject
    {
        get { return GetPropertyValue<Employee>(nameof(EmployeeObject)); }
        set { SetPropertyValue(nameof(EmployeeObject), value); }
    }

    [VisibleInLookupListView(true)]
    [DataSourceCriteria("IsEnabled == True")]
    [ModelDefault("LookupProperty", nameof(WareHouse.WareHouseName))]
    [LookupEditorMode(LookupEditorMode.AllItemsWithSearch)]
    [RuleRequiredField(CustomMessageTemplate = "입고창고를 입력하세요.")]
    [XafDisplayName("입고창고"), ToolTip("입고창고")]
    public WareHouse WareHouseObject
    {
        get { return GetPropertyValue<WareHouse>(nameof(WareHouseObject)); }
        set { SetPropertyValue(nameof(WareHouseObject), value); }
    }

    //[VisibleInLookupListView(true)]
    //[DataSourceCriteria("UniversalMajorCodeObject.MajorCode == 'RegistType' AND IsEnabled == True")]
    //[Appearance("ProductionTypeEdit", Criteria = "!(IsNewObject(this))", Enabled = false)]
    //[ModelDefault("LookupProperty", nameof(UniversalMinorCode.CodeName))]
    //[LookupEditorMode(LookupEditorMode.AllItemsWithSearch)]
    //[RuleRequiredField(CustomMessageTemplate = "생산유형을 입력하세요.")]
    //[XafDisplayName("생산유형"), ToolTip("생산유형")]
    //public UniversalMinorCode ProductionType
    //{
    //    get { return GetPropertyValue<UniversalMinorCode>(nameof(ProductionType)); }
    //    set { SetPropertyValue(nameof(ProductionType), value); }
    //}

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

    [VisibleInLookupListView(true)]
    [XafDisplayName("비고"), ToolTip("비고")]
    public string Remark
    {
        get { return GetPropertyValue<string>(nameof(Remark)); }
        set { SetPropertyValue(nameof(Remark), value); }
    }

    [XafDisplayName("수주 등록 상세")]
    [Association(@"DetailSalesOrderReferencesMasterSalesOrder"), DevExpress.Xpo.Aggregated]
    public XPCollection<DetailSalesOrder> DetailSalesOrderObjects { get { return GetCollection<DetailSalesOrder>(nameof(DetailSalesOrderObjects)); } }
    #endregion

    #region Constructors
    public MasterSalesOrder(Session session) : base(session) { }
    #endregion

    #region Methods

    public override void AfterConstruction()
    {
        base.AfterConstruction();
        FactoryObject = this.Session.Query<Factory>().FirstOrDefault();
        //ProductionType = this.Session.Query<UniversalMinorCode>().FirstOrDefault(x => x.UniversalMajorCodeObject.MajorCode == "RegistType" && x.CodeName == "양산");
        SalesOrderDateTime = DateTime.Now;
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
            case nameof(SalesOrderDateTime):
                CreateSalesOrderNumber();
                break;
            default:
                break;
        }
    }

    // 수주 일자에 맞춰 수주 번호 생성
    private void CreateSalesOrderNumber()
    {
        var salesOrderDate = SalesOrderDateTime.ToString("yyyyMMdd");
        var salseOrderObjects = new XPCollection<MasterSalesOrder>(Session)
            .Where(x => x.SalesOrderNumber
            .StartsWith($"{salesOrderDate}"));
        string suffix = "001";

        if (salseOrderObjects.Any())
        {
            suffix = (salseOrderObjects.Select(s => int.Parse(s.SalesOrderNumber
                                                       .Split('-')
                                                       .Last()))
                                                       .Max() + 1)
                                                       .ToString("000");
        }
        SalesOrderNumber = $"{salesOrderDate}-{suffix}";
    }
    #endregion
}