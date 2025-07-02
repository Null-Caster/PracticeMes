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
using PracticeMes.Module.BusinessObjects.LotManagement;
using PracticeMes.Module.BusinessObjects.Purchase;

namespace PracticeMes.Win.Controllers.Purchase
{
    public partial class PurchaseReturnController : ViewController
    {
        public PurchaseReturnController()
        {
            InitializeComponent();
            TargetObjectType = typeof(PurchaseReturn);
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
                IObjectSpace newObjectspace = Application.CreateObjectSpace(typeof(PurchaseReturn));
                var modifiedObjects = View.ObjectSpace.ModifiedObjects;

                foreach (var modifiedObject in modifiedObjects)
                {
                    if (modifiedObject is PurchaseReturn purchaseReturn)
                    {
                        // DB에 존재하는 PurchaseReturn 가져오기
                        PurchaseReturn dbPurchaseReturnobj = (PurchaseReturn)newObjectspace.GetObjectByKey(typeof(PurchaseReturn), purchaseReturn.Oid);

                        if (View.ObjectSpace.IsNewObject(purchaseReturn))
                        {
                            purchaseReturn.LotObject.ReturnQuantity = purchaseReturn.PurchaseReturnQuantity;
                        }
                        else if (View.ObjectSpace.IsDeletedObject(purchaseReturn))
                        {
                            if (dbPurchaseReturnobj != null)
                            {
                                purchaseReturn.LotObject.ReturnQuantity = 0;

                                purchaseReturn.LotObject.StockQuantity += dbPurchaseReturnobj.PurchaseReturnQuantity;
                            }
                        }
                        else
                        {
                            if (dbPurchaseReturnobj != null)
                            {
                                if (dbPurchaseReturnobj.LotObject.StockQuantity < Math.Abs(purchaseReturn.PurchaseReturnQuantity - dbPurchaseReturnobj.PurchaseReturnQuantity))
                                {
                                    throw new UserFriendlyException("반품 수량은 현재 재고 수량을 넘을 수 없습니다.");
                                }
                                else
                                {
                                    purchaseReturn.LotObject.ReturnQuantity += dbPurchaseReturnobj.PurchaseReturnQuantity - purchaseReturn.PurchaseReturnQuantity;
                                }
                            }
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
                throw new UserFriendlyException(ex.Message);
            }
            finally
            {

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
