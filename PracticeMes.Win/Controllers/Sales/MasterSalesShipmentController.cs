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
using PracticeMes.Module.BusinessObjects.Sales;

namespace PracticeMes.Win.Controllers.Sales;

public partial class MasterSalesShipmentController : ViewController
{
    public MasterSalesShipmentController()
    {
        InitializeComponent();
        TargetObjectType = typeof(MasterSalesShipment);
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
            IObjectSpace newObjectspace = Application.CreateObjectSpace(typeof(DetailSalesShipmentLot));
            var modifiedObjects = View.ObjectSpace.ModifiedObjects;

            foreach (var modifiedObject in modifiedObjects)
            {
                if (modifiedObject is DetailSalesShipmentLot detailSalesShipmentLot)
                {
                    if (View.ObjectSpace.IsNewObject(detailSalesShipmentLot))
                    {
                        // 생산 Lot 가져와서 수량 재고, 출하 수량 적용시키기
                        Lot lotObject = (Lot)newObjectspace.GetObjectByKey(typeof(Lot), detailSalesShipmentLot?.LotObject?.Oid);

                        lotObject.ReleaseQuantity += detailSalesShipmentLot?.ShipmentQuantity ?? 0;
                    }
                    else if (View.ObjectSpace.IsDeletedObject(detailSalesShipmentLot))
                    {
                        // 출하 Lot
                        DetailSalesShipmentLot preShipmentLotObject = (DetailSalesShipmentLot)newObjectspace.GetObjectByKey(typeof(DetailSalesShipmentLot), detailSalesShipmentLot.Oid);

                        // 생산 Lot
                        Lot lotObject = (Lot)newObjectspace.GetObjectByKey(typeof(Lot), preShipmentLotObject?.LotObject?.Oid);

                        lotObject.ReleaseQuantity -= preShipmentLotObject?.ShipmentQuantity ?? 0;
                    }
                    else
                    {
                        // 이전 Lot
                        DetailSalesShipmentLot preShipmentLotObject = newObjectspace.GetObjectByKey<DetailSalesShipmentLot>(detailSalesShipmentLot.Oid);
                        Lot LotObject = newObjectspace.GetObjectByKey<Lot>(preShipmentLotObject?.LotObject?.Oid);

                        // 현재 사용자 출하 수량
                        var currentShipmentQty = detailSalesShipmentLot?.ShipmentQuantity ?? 0;

                        // 이전 출하 수량
                         var preQty = LotObject.ReleaseQuantity;

                        LotObject.ReleaseQuantity += currentShipmentQty - preQty;
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
