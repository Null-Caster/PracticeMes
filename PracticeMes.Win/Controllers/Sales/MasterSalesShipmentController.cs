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
                    }
                    // 수정/삭제인 경우
                    else
                    {
                        if ((detailSalesShipmentLot).IsDeleted) // 삭제인 경우
                        {
                            // 출하수량을 재고수량으로 변경해주기 단 출하수량이 존재하지 않는다면 재고반영 불가
                            var beforeDetailSalesShipmentLotObject = newObjectspace.GetObjects<DetailSalesShipmentLot>().Where(x => x.Oid == ((DetailSalesShipmentLot)detailSalesShipmentLot).Oid).FirstOrDefault();
                            var beforeShipmentQuantity = beforeDetailSalesShipmentLotObject.ShipmentQuantity;
                            var lotObject = View.ObjectSpace.GetObjects<Lot>().Where(x => x.Oid == beforeDetailSalesShipmentLotObject.LotObject.Oid).FirstOrDefault();
                            lotObject.ReleaseQuantity -= 1;

                        }
                        else // 수정인경우
                        {
                            // 이전 출하수량을 다시 재고로 덮고 그 새로입력받은 출하수량을 출하로 변경 단 재고가 존재해야함

                            var lotObject = detailSalesShipmentLot?.LotObject;
                            var beforeDetailSalesShipmentLotObject = newObjectspace.GetObjects<DetailSalesShipmentLot>().Where(x => x.Oid == detailSalesShipmentLot.Oid).FirstOrDefault();
                            var beforeLotObject = beforeDetailSalesShipmentLotObject.LotObject;
                            var beforeShipmentQuantity = beforeDetailSalesShipmentLotObject.ShipmentQuantity;

                            // 이전 로트 원복
                            var beforelotObject = View.ObjectSpace.GetObjects<Lot>().Where(x => x.Oid == beforeDetailSalesShipmentLotObject.LotObject.Oid).FirstOrDefault();
                            beforelotObject.ReleaseQuantity = beforelotObject.ReleaseQuantity - 1;

                            // 신규 로트 적용
                            if (lotObject.Oid == beforeLotObject.Oid) // 로트 동일한데 변경한경우
                            {
                                lotObject.ReleaseQuantity += 1;
                            }
                            else // 로트 바뀐경우
                            {
                                lotObject.ReleaseQuantity += 1;
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
