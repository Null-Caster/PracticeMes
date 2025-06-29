using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using PracticeMes.Module.BusinessObjects.BaseInfo.CommonInfo;
using PracticeMes.Module.BusinessObjects.BaseInfo.ProductionInfo;
using PracticeMes.Module.BusinessObjects.Purchase;
using PracticeMes.Module.BusinessObjects.Sales;

namespace PracticeMes.Win.Controllers.BaseInfo.CommonInfo
{
    public partial class EmployeeController : ViewController
    {
        public EmployeeController()
        {
            InitializeComponent();
            TargetObjectType = (typeof(Employee));
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            if (View.Id != "Employee_ListView" && View.Id != "Employee_DetailView")
                return;

            ObjectSpace.Committing += ObjectSpace_Committing;
        }

        private void ObjectSpace_Committing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                var modifiedObjects = View.ObjectSpace.ModifiedObjects;
                IObjectSpace newObjectSpace = Application.CreateObjectSpace(typeof(Employee));

                foreach (var modifiedObject in modifiedObjects)
                {

                    if (modifiedObject is Employee employee)
                    {

                        if (View.ObjectSpace.IsNewObject(employee))
                        {

                        }
                        else if (View.ObjectSpace.IsDeletedObject(employee))
                        {
                            CheckIfObjectIsInUse(newObjectSpace, employee, "삭제");
                        }
                        else
                        {
                            CheckIfObjectIsInUse(newObjectSpace, employee, "수정");
                        }
                    }
                }
            }
            catch (UserFriendlyException ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("직원 삭제 처리 중 오류가 발생했습니다: " + ex.Message);
            }
        }

        private static void CheckIfObjectIsInUse(IObjectSpace newObjectSpace, Employee currentEmployee, string action)
        {
            var referencingList = new List<string>();

            if (newObjectSpace.GetObjects<MasterSalesOrder>().Any(x => x.EmployeeObject.Oid == currentEmployee.Oid))
                referencingList.Add("수주 등록");

            if (newObjectSpace.GetObjects<MasterPurchaseOrder>().Any(x => x.EmployeeObject.Oid == currentEmployee.Oid))
                referencingList.Add("구매 발주 등록");

            if (newObjectSpace.GetObjects<MasterPurchaseInput>().Any(x => x.EmployeeObject.Oid == currentEmployee.Oid))
                referencingList.Add("구매 입고 등록");

            if (newObjectSpace.GetObjects<MasterSalesShipment>().Any(x => x.EmployeeObject.Oid == currentEmployee.Oid))
                referencingList.Add("출하 등록");

            if (referencingList.Any())
            {
                string UsingList = string.Join(", ", referencingList);
                throw new UserFriendlyException(
                         $"이 직원은 다음 메뉴에서 사용 중이므로 {action}할 수 없습니다:\n[{UsingList}]");
            }
        }

        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
        }
        protected override void OnDeactivated()
        {
            ObjectSpace.Committing -= ObjectSpace_Committing;
            base.OnDeactivated();
        }
    }
}
