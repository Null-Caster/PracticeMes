using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace PracticeMes.Win.Controllers.BaseInfo.CommonInfo
{
    public partial class DepartmentController : ViewController
    {
        public DepartmentController()
        {
            InitializeComponent();
            TargetObjectType = typeof(Department);
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            if (View.Id != "Department_ListView" && View.Id != "Department_DetailView")
                return;

            ObjectSpace.Committing += ObjectSpace_Committing;
        }

        private void ObjectSpace_Committing(object sender, CancelEventArgs e)
        {
            try
            {
                var modifiedObjects = View.ObjectSpace.ModifiedObjects;
                IObjectSpace newObjectSpace = Application.CreateObjectSpace(typeof(Department));

                foreach (var modifiedObject in modifiedObjects)
                {
                    if (modifiedObject is Department department)
                    {
                        if (View.ObjectSpace.IsNewObject(department)) // 신규
                        {

                        }
                        else if (View.ObjectSpace.IsDeletedObject(department)) // 삭제
                        {
                            CheckIfObjectIsInUse(newObjectSpace, department, "삭제");
                        }
                        else // 수정
                        {
                            CheckIfObjectIsInUse(newObjectSpace, department, "수정");
                        }
                    }
                }
            }
            catch (UserFriendlyException ex)
            {
                throw new UserFriendlyException("부서 삭제 처리 중 오류가 발생했습니다: " + ex.Message);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("부서 삭제 처리 중 오류가 발생했습니다: " + ex.Message);
            }
        }

        private static void CheckIfObjectIsInUse(IObjectSpace newObjectSpace, Department currentDepartment, string action)
        {
            var referencingList = new List<string>();

            if (newObjectSpace.GetObjects<Employee>().Any(x => x.DepartmentObject.Oid == currentDepartment.Oid))
                referencingList.Add("직원 등록");

            if (newObjectSpace.GetObjects<WorkCenter>().Any(x => x.DepartmentObject.Oid == currentDepartment.Oid))
                referencingList.Add("작업장 등록");

            if (referencingList.Any())
            {
                string UsingList = string.Join(", ", referencingList);
                throw new UserFriendlyException(
                         $"이 부서는 다음 메뉴에서 사용 중이므로 {action}할 수 없습니다:\n[{UsingList}]");
            }
        }
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
        }
        protected override void OnDeactivated()
        {
            base.OnDeactivated();
        }
    }
}
