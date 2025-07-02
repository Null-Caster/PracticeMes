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
                        // 해당 객체의 Lot 가져오기
                        Lot oldLotObject = (Lot)newObjectspace.GetObjectByKey(typeof(Lot), oldDetailPurchaseInput.LotObject.Oid);

                        if (View.ObjectSpace.IsNewObject(detailPurchaseInput))
                        {
                            CreateLotObject(detailPurchaseInput);
                        }
                        else if (View.ObjectSpace.IsDeletedObject(detailPurchaseInput))
                        {
                            if (oldDetailPurchaseInput != null)
                            {
                                if (oldLotObject != null)
                                {
                                    if (oldLotObject.InputQuantity > 0 ||
                                        oldLotObject.DefectQuantity > 0 ||
                                        oldLotObject.ReleaseQuantity > 0 ||
                                        oldLotObject.ReturnQuantity > 0
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
                                    throw new UserFriendlyException("발주 수량보다 많은 수량을 입고할 수 없습니다.");
                                }

                                if (oldDetailPurchaseInput?.LotObject?.Oid != null)
                                {
                                    if (oldLotObject != null)
                                    {
                                        if (oldLotObject.InputQuantity > 0 ||
                                            oldLotObject.ReleaseQuantity > 0 ||
                                            oldLotObject.ReturnQuantity > 0
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

        // LOT 생성 함수
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

        // 시간되면 LotTracking도 이 MES에 맞춰 구현

        //private void CreateLotTrackingObject(DetailPurchaseInput currentObject, DetailPurchaseInput oldObject = null)
        //{
        //    var autoInspection = currentObject?.InspectionExecuteType?.CodeName.Trim();

        //    // 기존 랏 트래킹 가져옴
        //    var matchedLotTracking = View.ObjectSpace.GetObjects<LotTracking>()
        //        .FirstOrDefault(x => x.LotObject?.LotNumber == currentObject?.LotObject?.LotNumber);

        //    var lotTracking = View.ObjectSpace.CreateObject<LotTracking>();

        //    if (matchedLotTracking != null)
        //    {
        //        lotTracking.LotObject = matchedLotTracking.LotObject;
        //    }
        //    else
        //    {
        //        lotTracking.LotObject = currentObject.LotObject;
        //    }

        //    lotTracking.ItemObject = currentObject.ItemObject;
        //    lotTracking.CreationType = currentObject.LotObject?.LotTypeObject?.LotTypeName;
        //    lotTracking.EventDateTime = DateTime.Now;
        //    lotTracking.PreviousQuantity = 0;
        //    lotTracking.ChangeQuantity = currentObject.PurchaseInputQuantity;

        //    if (oldObject != null)
        //    {
        //        lotTracking.PreviousQuantity = oldObject.PurchaseInputQuantity;
        //    }

        //    if (autoInspection == "자동검사")
        //    {
        //        lotTracking.StockQuantity = 0;
        //        lotTracking.ProcessingType = "검사요청";
        //        lotTracking.ReferenceInfo = $"입고번호 : {currentObject.MasterPurchaseInputObject?.PurchaseInputNumber}, 검사수량 : {currentObject.PurchaseInputQuantity}";
        //        FindPreLotTracking(currentObject, "검사요청");
        //    }
        //    else
        //    {
        //        lotTracking.StockQuantity = currentObject.PurchaseInputQuantity;
        //        lotTracking.ProcessingType = "재고입고";
        //        lotTracking.ReferenceInfo = $"입고번호 : {currentObject.MasterPurchaseInputObject?.PurchaseInputNumber}, 입고수량 : {currentObject.PurchaseInputQuantity}";
        //        FindPreLotTracking(currentObject, "재고입고");
        //    }
        //}


        // 구매 입고 시 검사의 유무 변경해 수정/저장하면 이전에 생성된 다른 종류의 LotTracking 삭제
        //private void FindPreLotTracking(DetailPurchaseInput currentObject, string PreProcessingType)
        //{
        //    var findPreLotTrackingList = View.ObjectSpace.GetObjects<LotTracking>()
        //        .Where(x => x.LotObject?.Oid == currentObject?.LotObject?.Oid
        //        && x.ProcessingType != "검사완료"
        //        ).ToList();

        //    foreach (var preLotTrackingObject in findPreLotTrackingList)
        //    {
        //        if (preLotTrackingObject.ProcessingType != PreProcessingType)
        //        {
        //            preLotTrackingObject.Delete();
        //        }
        //    }
        //}

        //private static void DeleteLotTrackingObject(DetailPurchaseInput oldDetailPurchaseInput, IObjectSpace newObjectspace)
        //{
        //    var oldLotTrackingObjects = newObjectspace.GetObjects<LotTracking>()
        //                                              .Where(x => x.LotObject?.Oid == oldDetailPurchaseInput?.LotObject?.Oid)
        //                                              .ToList();

        //    if (oldLotTrackingObjects != null)
        //    {
        //        foreach (var oldLotTrackingObject in oldLotTrackingObjects)
        //        {
        //            double stockQuantity = oldLotTrackingObject.LotObject.StockQuantity;

        //            if (stockQuantity > 0)
        //            {
        //                throw new UserFriendlyException("재고 입고가 진행되어 삭제할 수 없습니다.");
        //            }
        //            else
        //            {
        //                oldLotTrackingObject.Delete();
        //            }
        //        }
        //    }
        //}

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
