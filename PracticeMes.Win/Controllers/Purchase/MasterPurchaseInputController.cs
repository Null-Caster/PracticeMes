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
using PracticeMes.Module.BusinessObjects.LotManagement;
using PracticeMes.Module.BusinessObjects.Purchase;

namespace PracticeMes.Win.Controllers.Purchase
{
    public partial class MasterPurchaseInputController : ViewController
    {
        public MasterPurchaseInputController()
        {
            InitializeComponent();
            TargetObjectType = typeof(MasterPurchaseInput);
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            ObjectSpace.Committing += ObjectSpace_Committing;
        }

        private void ObjectSpace_Committing(object sender, CancelEventArgs e)
        {
            try
            {
                IObjectSpace newObjectspace = Application.CreateObjectSpace(typeof(DetailPurchaseInput));
                var modifiedObjects = View.ObjectSpace.ModifiedObjects;

                foreach (var modifiedObject in modifiedObjects)
                {
                    if (modifiedObject is DetailPurchaseInput detailPurchaseInput)
                    {
                        // GetObjectByKey: 단일 객체 조회 시 가장 효율이 좋음(XAF/XPO)
                        DetailPurchaseInput oldDetailPurchaseInput = (DetailPurchaseInput)newObjectspace.GetObjectByKey(typeof(DetailPurchaseInput), detailPurchaseInput.Oid);

                        if (View.ObjectSpace.IsNewObject(detailPurchaseInput))
                        {
                            CreateLotObject(detailPurchaseInput);
                        }
                        else if (View.ObjectSpace.IsDeletedObject(detailPurchaseInput))
                        {
                            if (oldDetailPurchaseInput != null)
                            {
                                var oldLotObject = newObjectspace.GetObjects<Lot>()
                                    .Where(x => x?.Oid == oldDetailPurchaseInput?.LotObject?.Oid)
                                    .FirstOrDefault();

                                if (oldLotObject != null)
                                {
                                    if (oldLotObject.InputQuantity > 0 ||
                                        oldLotObject.DefectQuantity > 0 ||
                                        oldLotObject.ReleaseQuantity > 0 ||
                                        oldLotObject.ReturnQuantity > 0
                                        //oldLotObject.RejectDefectQuantity > 0 ||
                                        //oldLotObject.RecycleDefectQuantity > 0 ||
                                        //oldLotObject.MeterialReleaseQuantity > 0 ||
                                        //oldLotObject.MeterialRestockingQuantity > 0 ||
                                        //oldLotObject.StockMoveQuantity > 0 ||
                                        //oldLotObject.StockChangeQuantity > 0
                                        )
                                    {
                                        throw new UserFriendlyException("로트가 사용되어 삭제 할 수 없습니다.");
                                    }
                                    else
                                    {
                                        oldLotObject.Delete();
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (oldDetailPurchaseInput != null)
                            {
                                // 발주번호 디테일의 품목 발주수량 합계
                                var OldDetailPurchaseOrderQuantity = newObjectspace.GetObjects<DetailPurchaseOrder>()
                                    .Where(x => x.MasterPurchaseOrderObject?.Oid == oldDetailPurchaseInput.MasterPurchaseInputObject?.MasterPurchaseOrderObject?.Oid &&
                                                x.ItemObject?.Oid == oldDetailPurchaseInput.ItemObject?.Oid)
                                    .Sum(x => x.PurchaseOrderQuantity);

                                // 발주번호가 동일한 품목 입고자료 합계
                                var OldDetailPurchaseInputQuantity = newObjectspace.GetObjects<DetailPurchaseInput>()
                                    .Where(x => x.MasterPurchaseInputObject?.MasterPurchaseOrderObject?.Oid == oldDetailPurchaseInput.MasterPurchaseInputObject?.MasterPurchaseOrderObject?.Oid &&
                                                x.ItemObject?.Oid == oldDetailPurchaseInput.ItemObject?.Oid)
                                    .Sum(x => x.PurchaseInputQuantity);

                                if (OldDetailPurchaseOrderQuantity < detailPurchaseInput.PurchaseInputQuantity - OldDetailPurchaseInputQuantity)
                                {
                                    throw new UserFriendlyException("발주수량을 초과하여 입고 불가능합니다.");
                                }

                                var oldLotObject = newObjectspace.GetObjects<Lot>()
                                    .Where(x => x?.Oid == oldDetailPurchaseInput?.LotObject?.Oid)
                                    .FirstOrDefault();

                                if (oldLotObject != null)
                                {
                                    if (oldLotObject.InputQuantity > 0 ||
                                        oldLotObject.ReleaseQuantity > 0 ||
                                        oldLotObject.ReturnQuantity > 0
                                        //oldLotObject.RejectDefectQuantity > 0 ||
                                        //oldLotObject.RecycleDefectQuantity > 0 ||
                                        //oldLotObject.MeterialReleaseQuantity > 0 ||
                                        //oldLotObject.MeterialRestockingQuantity > 0 ||
                                        //oldLotObject.StockMoveQuantity > 0 ||
                                        //oldLotObject.StockChangeQuantity > 0
                                        )
                                    {
                                        throw new UserFriendlyException("로트가 사용되어 수정할 수 없습니다.");
                                    }
                                    else
                                    {
                                        // 기존 Lot 수정
                                        var detailPurchaseInputInNewSpace = newObjectspace.GetObject(detailPurchaseInput);

                                        oldLotObject.UnitObject = newObjectspace.GetObject(detailPurchaseInput.UnitObject);
                                        oldLotObject.ItemAccountObject = newObjectspace.GetObject(detailPurchaseInput.ItemObject?.ItemAccountObject);
                                        oldLotObject.CreateQuantity = detailPurchaseInput?.PurchaseInputQuantity ?? 0.0;

                                        detailPurchaseInputInNewSpace.LotObject = oldLotObject;
                                    }
                                }
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

        private void CreateLotObject(DetailPurchaseInput currentObject)
        {
            LotType lotType = View.ObjectSpace.GetObjects<LotType>()
                .FirstOrDefault(x => x.LotTypeCode == "I") ?? throw new UserFriendlyException("LotTypeCode가 'I'인 LotType이 존재하지 않습니다.");
            Lot lotObject = View.ObjectSpace.CreateObject<Lot>();

            lotObject.LotTypeObject = lotType;
            lotObject.FactoryObject = currentObject?.MasterPurchaseInputObject?.WareHouseObject?.FactoryObject;
            lotObject.WareHouseObject = currentObject?.MasterPurchaseInputObject?.WareHouseObject;
            lotObject.ItemObject = currentObject?.ItemObject;
            lotObject.UnitObject = currentObject?.ItemObject?.UnitObject;
            lotObject.ItemAccountObject = currentObject?.ItemObject?.ItemAccountObject;
            lotObject.CreateQuantity = currentObject.PurchaseInputQuantity;
            lotObject.LotNumber = lotType.LotTypeCode + currentObject?.ItemObject.ItemAccountObject?.ItemAccountCode + currentObject?.MasterPurchaseInputObject?.WareHouseObject?.FactoryObject?.LotCode
                + DateTime.Now.Minute.ToString()
                + DateTime.Now.Millisecond.ToString();

            currentObject.LotObject = lotObject;
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
