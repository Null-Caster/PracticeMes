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
using DevExpress.Mvvm.POCO;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using PracticeMes.Module.BusinessObjects.LotManagement;
using PracticeMes.Module.BusinessObjects.ProductPlanning;

namespace PracticeMes.Win.Controllers.ProductPlanning
{
    public partial class MaterialInputResultController : ViewController
    {
        public MaterialInputResultController()
        {
            InitializeComponent();
            TargetObjectType = typeof(MaterialInputResult);
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            View.ObjectSpace.Committing += ObjectSpace_Committing;
        }

        private void ObjectSpace_Committing(object sender, CancelEventArgs e)
        {
            try
            {
                IObjectSpace newObjectspace = Application.CreateObjectSpace(typeof(MaterialInputResult));
                var modifiedObjects = View.ObjectSpace.ModifiedObjects;

                foreach (var modifiedObject in modifiedObjects)
                {
                    if (modifiedObject is MaterialInputResult materialInputResult)
                    {
                        // 수정, 삭제 시 필요한 이전 원재료 투입 Object 가져오기
                        MaterialInputResult preObject = null;

                        // 원재료 투입 할 품목의 입고 Lot 가져오기
                        Lot oldItemLotObject = null;

                        if (materialInputResult?.Oid != null)
                        {
                            preObject = (MaterialInputResult)newObjectspace.GetObjectByKey(typeof(MaterialInputResult), materialInputResult.Oid);
                        }

                        if (materialInputResult?.LotObject?.Oid != null)
                        {
                            oldItemLotObject = (Lot)newObjectspace.GetObjectByKey(typeof(Lot), materialInputResult.LotObject.Oid);
                        }

                        if (View.ObjectSpace.IsNewObject(materialInputResult))
                        {
                            if (oldItemLotObject != null)
                            {
                                oldItemLotObject.InputQuantity = materialInputResult.MaterialInputQuantity;
                                oldItemLotObject.StockQuantity -= materialInputResult.MaterialInputQuantity;
                            }
                        }
                        else if (View.ObjectSpace.IsDeletedObject(materialInputResult))
                        {
                            if (oldItemLotObject != null)
                            {
                                double previousInputQty = preObject?.MaterialInputQuantity ?? 0;

                                oldItemLotObject.InputQuantity = 0;
                                oldItemLotObject.StockQuantity += previousInputQty;
                            }
                        }
                        else
                        {
                            if (materialInputResult?.DetailWorkInstructionObject?.Progress?.CodeName == "완료") {
                                throw new UserFriendlyException("생산 계획이 마감되어 수정할 수 없습니다.");
                            }
                            if (oldItemLotObject != null && preObject != null)
                            {
                                double previousQty = preObject.MaterialInputQuantity;
                                double currentQty = materialInputResult.MaterialInputQuantity;
                                double diff = previousQty - currentQty;

                                // 기존 입력량 기준으로 차이만큼 조정
                                oldItemLotObject.InputQuantity = currentQty;
                                oldItemLotObject.StockQuantity += diff;
                            }
                        }
                    }
                }
                newObjectspace.CommitChanges();
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

        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
        }
        protected override void OnDeactivated()
        {
            View.ObjectSpace.Committing -= ObjectSpace_Committing;
            base.OnDeactivated();
        }
    }
}
