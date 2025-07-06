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
using PracticeMes.Module.BusinessObjects.Inspect;

namespace PracticeMes.Win.Controllers.Inspect
{
    public partial class IncomingInspectionController : ViewController
    {
        public IncomingInspectionController()
        {
            InitializeComponent();
            TargetObjectType = typeof(IncomingInspection);
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            ObjectSpace.Committing += ObjectSpace_Committing;
        }

        private void ObjectSpace_Committing(object sender, CancelEventArgs e)
        {
            IObjectSpace newObjectspace = Application.CreateObjectSpace(TargetObjectType);
            var modifiedObjects = View.ObjectSpace.ModifiedObjects;

            foreach (var modifiedObject in modifiedObjects)
            {
                if (modifiedObject is IncomingInspection incomingInspection)
                {
                    if (View.ObjectSpace.IsNewObject(incomingInspection))
                    {
                        // 수입검사 신규일 경우 구매 입고 시 자동 생성
                    }
                    else if (View.ObjectSpace.IsDeletedObject(incomingInspection))
                    {

                    }
                    else
                    {
                        ModifyLotObject(incomingInspection);
                    }
                }
            }
            newObjectspace.CommitChanges();
        }

        private static void ModifyLotObject (IncomingInspection incomingInspection)
        {
            // Lot 가져오기
            var lotObject = incomingInspection?.DetailPurchaseInputObject?.LotObject;

            lotObject.DefectQuantity = incomingInspection.DefectQuantity;
            lotObject.StockQuantity = incomingInspection.GoodQuantity;
            lotObject.InspectiontQuantity = incomingInspection.InspectionQuantity;
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
