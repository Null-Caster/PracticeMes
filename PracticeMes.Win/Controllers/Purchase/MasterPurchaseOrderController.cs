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

namespace PracticeMes.Win.Controllers.Purchase
{
    public partial class MasterPurchaseOrderController : ViewController
    {
        public MasterPurchaseOrderController()
        {
            InitializeComponent();
            TargetObjectType = typeof(MasterPurchaseOrder);
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            ObjectSpace.Committing += ObjectSpace_Committing;
        }

        private void ObjectSpace_Committing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                var modefiedObjects = View.ObjectSpace.ModifiedObjects;
                IObjectSpace newObjectSapce = Application.CreateObjectSpace(typeof(MasterPurchaseOrder));

                if (modefiedObjects != null)
                {
                    foreach (var modefiedObject in modefiedObjects)
                    {
                        // 마스터 객체
                        if (modefiedObject is MasterPurchaseOrder masterPurchaseOrder)
                        {
                            if (View.ObjectSpace.IsNewObject(masterPurchaseOrder))
                            {

                            }
                            else if (View.ObjectSpace.IsDeletedObject(masterPurchaseOrder))
                            {
                                CheckIfObjectIsInUse(newObjectSapce, masterPurchaseOrder, "삭제");
                            }
                            else
                            {
                                CheckIfObjectIsInUse(newObjectSapce, masterPurchaseOrder, "수정");
                            }
                        } // 디테일 객체
                        //else if (modefiedObject is DetailPurchaseOrder detailPurchaseOrder)
                        //{
                        //    if (View.ObjectSpace.IsNewObject(detailPurchaseOrder))
                        //    {

                        //    }
                        //    else if (View.ObjectSpace.IsDeletedObject(detailPurchaseOrder))
                        //    {

                        //    }
                        //    else
                        //    {

                        //    }
                        //}
                    }
                }
            }
            catch (UserFriendlyException ex)
            {
                throw new UserFriendlyException(ex.Message);

            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        private static void CheckIfObjectIsInUse(IObjectSpace newObjectSpace, MasterPurchaseOrder currentMasterPurchaseOrder, string action)
        {
            var masterPurchaseInputObjects = newObjectSpace.GetObjectsQuery<MasterPurchaseInput>()
                .Any(x => x.MasterPurchaseOrderObject.Oid == currentMasterPurchaseOrder.Oid);

            if (masterPurchaseInputObjects)
            {
                throw new UserFriendlyException(
                         $"이 구매 입고는 다음 메뉴에서 사용 중이므로 {action}할 수 없습니다.");
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
