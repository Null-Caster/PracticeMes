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

namespace PracticeMes.Module.BusinessObjects.Purchase;

[DefaultClassOptions]
[NavigationItem("구매 관리"), XafDisplayName("구매입고 등록")]
[DefaultListViewOptions(MasterDetailMode.ListViewOnly, false, NewItemRowPosition.None)]
public class MasterPurchaseInput : BaseObject
{
    #region Properties
    [VisibleInLookupListView(true)]
    [ModelDefault("AllowEdit", "False")]
    [XafDisplayName("입고 번호"), ToolTip("입고 번호")]
    public string PurchaseInputNumber
    {
        get { return GetPropertyValue<string>(nameof(PurchaseInputNumber)); }
        set { SetPropertyValue(nameof(PurchaseInputNumber), value); }
    }

    [VisibleInLookupListView(true)]
    [ModelDefault("LookupProperty", nameof(MasterPurchaseOrder.PurchaseOrderNumber))]
    [LookupEditorMode(LookupEditorMode.AllItemsWithSearch)]
    [DataSourceProperty(nameof(AvailableObjects))]
    [XafDisplayName("구매발주번호"), ToolTip("구매발주번호")]
    [ImmediatePostData(true)]
    public MasterPurchaseOrder MasterPurchaseOrderObject
    {
        get { return GetPropertyValue<MasterPurchaseOrder>(nameof(MasterPurchaseOrderObject)); }
        set { SetPropertyValue(nameof(MasterPurchaseOrderObject), value); }
    }

    [Browsable(false)]
    public List<MasterPurchaseOrder> AvailableObjects
    {
        get
        {
            return new XPCollection<MasterPurchaseOrder>(this.Session)
                .Where(m => m.DetailPurchaseOrderobjects
                .Any(d => d.PurchaseOrderQuantity > d.PurchaseInputQuantity))
                .ToList();
        }
    }

    [VisibleInLookupListView(true)]
    [ModelDefault("DisplayFormat", "yyyy/MM/dd")]
    [XafDisplayName("발주일자"), ToolTip("발주일자")]
    public DateTime PurchaseOrderDateTime
    {
        get { return MasterPurchaseOrderObject?.PurchaseOrderDateTime ?? DateTime.MinValue; }
    }

    [VisibleInLookupListView(true)]
    [ModelDefault("DisplayFormat", "yyyy/MM/dd")]
    [XafDisplayName("납기예정일"), ToolTip("납기예정일")]
    public DateTime DeliveryDateTime
    {
        get { return MasterPurchaseOrderObject?.DeliveryDateTime ?? DateTime.MinValue; }
    }

    [VisibleInLookupListView(true)]
    [XafDisplayName("거래처"), ToolTip("거래처")]
    public string BizPartnerName
    {
        get { return MasterPurchaseOrderObject?.BusinessPartnerObject?.BusinessPartnerName; }
    }

    [VisibleInLookupListView(true)]
    [DataSourceCriteria("IsEnabled == True")]
    [ModelDefault("LookupProperty", nameof(Employee.EmployeeName))]
    [LookupEditorMode(LookupEditorMode.AllItemsWithSearch)]
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
        get { return MasterPurchaseOrderObject?.WareHouseObject; }
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

    [VisibleInLookupListView(true)]
    [XafDisplayName("비고"), ToolTip("비고")]
    public string Remark
    {
        get { return GetPropertyValue<string>(nameof(Remark)); }
        set { SetPropertyValue(nameof(Remark), value); }
    }

    [XafDisplayName("구매입고 등록 상세")]
    [Association(@"DetailPurchaseInputReferenceMasterPurchaseInput"), DevExpress.Xpo.Aggregated]
    public XPCollection<DetailPurchaseInput> DetailPurchaseInputObjects { get { return GetCollection<DetailPurchaseInput>(nameof(DetailPurchaseInputObjects)); } }
    #endregion

    #region Constructors
    public MasterPurchaseInput(Session session) : base(session) { }
    #endregion

    #region Methods
    public override void AfterConstruction()
    {
        base.AfterConstruction();
        CreatePurchaseOrederNum();
        CreatedDateTime = DateTime.Now;
    }

    protected override void OnChanged(string propertyName, object oldValue, object newValue)
    {
        base.OnChanged(propertyName, oldValue, newValue);

        if (this.Session.IsObjectsLoading) return;

        switch (propertyName)
        {
            case nameof(MasterPurchaseOrderObject):
                this.EmployeeObject = MasterPurchaseOrderObject.EmployeeObject;
                break;
            default:
                break;
        }
    }

    // 구매 입고 번호 생성
    private void CreatePurchaseOrederNum()
    {
        string todayPrefix = DateTime.Now.ToString("yyyyMMdd-");

        int maxNumber = new XPCollection<MasterPurchaseOrder>(this.Session)
         .Where(w => w.PurchaseOrderNumber.StartsWith(todayPrefix))
         .Select(s => int.TryParse(s.PurchaseOrderNumber[^4..], out var n) ? n : 0)
         .DefaultIfEmpty(0)
         .Max();

        PurchaseInputNumber = todayPrefix + (maxNumber + 1).ToString("0000");
    }
    #endregion
}