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

namespace PracticeMes.Module.BusinessObjects.Sales
{
    [DefaultClassOptions]
    [NavigationItem("영업 관리"), XafDisplayName("출하 등록")]
    [DefaultListViewOptions(MasterDetailMode.ListViewOnly, true, NewItemRowPosition.Top)]
    public class MasterSalesShipment : BaseObject
    {

        #region Properties
        [VisibleInLookupListView(true)]
        [ModelDefault("AllowEdit", "False")]
        [RuleUniqueValue(CustomMessageTemplate = "출하 번호가 중복되었습니다.")]
        [RuleRequiredField(CustomMessageTemplate = "출하 번호를 입력하세요.")]
        [XafDisplayName("출하 번호"), ToolTip("출하 번호")]
        public string SalesShipmentNumber
        {
            get { return GetPropertyValue<string>(nameof(SalesShipmentNumber)); }
            set { SetPropertyValue(nameof(SalesShipmentNumber), value); }
        }

        [ImmediatePostData(true)]
        [VisibleInLookupListView(true)]
        [ModelDefault("DisplayFormat", "yyyy/MM/dd")]
        [RuleRequiredField(CustomMessageTemplate = "출하 일자를 입력하세요.")]
        [XafDisplayName("출하 일자"), ToolTip("출하 일자")]
        public DateTime SalesShipmentDateTime
        {
            get { return GetPropertyValue<DateTime>(nameof(SalesShipmentDateTime)); }
            set { SetPropertyValue(nameof(SalesShipmentDateTime), value); }
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
        [XafDisplayName("출하창고"), ToolTip("출하창고")]
        public WareHouse WareHouseObject
        {
            get { return GetPropertyValue<WareHouse>(nameof(WareHouseObject)); }
            set { SetPropertyValue(nameof(WareHouseObject), value); }
        }

        [VisibleInLookupListView(true)]
        [XafDisplayName("거래처 담당자"), ToolTip("거래처 담당자")]
        public string ClientManager
        {
            get { return GetPropertyValue<string>(nameof(ClientManager)); }
            set { SetPropertyValue(nameof(ClientManager), value); }
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
        [ModelDefault("EditMask", "yyyy/MM/dd HH:mm:ss")]
        [ModelDefault("DisplayFormat", "yyyy/MM/dd HH:mm:ss")]
        [XafDisplayName("생성 일시"), ToolTip("항목이 생성된 일시입니다.")]
        public DateTime CreatedDateTime
        {
            get { return GetPropertyValue<DateTime>(nameof(CreatedDateTime)); }
            set { SetPropertyValue(nameof(CreatedDateTime), value); }
        }

        [XafDisplayName("출하 상세")]
        [Association(@"DetailSalesShipmentReferencesMasterSalesShipment"), DevExpress.Xpo.Aggregated]
        public XPCollection<DetailSalesShipment> DetailSalesShipmentObjects { get { return GetCollection<DetailSalesShipment>(nameof(DetailSalesShipmentObjects)); } }
        #endregion

        #region Constructors
        public MasterSalesShipment(Session session) : base(session) { }
        #endregion

        #region Methods
        public override void AfterConstruction()
        {
            base.AfterConstruction();

            SalesShipmentDateTime = DateTime.Now;
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
                case nameof(SalesShipmentDateTime):
                    CreateSalesOrderNumber();
                    break;
                default:
                    break;
            }
        }

        private void CreateSalesOrderNumber()
        {
            var salesShipmentDate = SalesShipmentDateTime.ToString("yyyyMMdd");
            var salesShipmentObjects = new XPCollection<MasterSalesShipment>(Session)
                .Where(x => x.SalesShipmentNumber.StartsWith($"{salesShipmentDate}"));
            string suffix = "001";
            if (salesShipmentObjects.Count() > 0)
            {
                suffix = (salesShipmentObjects.Select(s => int.Parse(s.SalesShipmentNumber.Split('-')[s.SalesShipmentNumber.Split('-').Length - 1])).Max() + 1).ToString("000");
            }
            SalesShipmentNumber = $"{salesShipmentDate}-{suffix}";
        }
        #endregion
    }
}