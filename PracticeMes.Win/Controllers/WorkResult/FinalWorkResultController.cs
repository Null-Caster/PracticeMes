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
using DevExpress.Pdf.Native.BouncyCastle.Utilities.IO.Pem;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using PracticeMes.Module.BusinessObjects.BaseInfo.CommonInfo;
using PracticeMes.Module.BusinessObjects.LotManagement;
using PracticeMes.Module.BusinessObjects.Purchase;
using PracticeMes.Module.BusinessObjects.WorkResult;

namespace PracticeMes.Win.Controllers.WorkResult
{
    public partial class FinalWorkResultController : ViewController
    {
        public FinalWorkResultController()
        {
            InitializeComponent();
            TargetObjectType = typeof(FinalWorkResult);
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            View.ObjectSpace.Committing += ObjectSpace_Committing;
        }

        private void ObjectSpace_Committing(object sender, CancelEventArgs e)
        {
            var modifiedObjects = View.ObjectSpace.ModifiedObjects;
            IObjectSpace newObjectSpace = Application.CreateObjectSpace(typeof(FinalWorkResult));
            try
            {
                foreach (var modifiedObject in modifiedObjects)
                {
                    if (modifiedObject is FinalWorkResult finalWorkResult)
                    {
                        if (View.ObjectSpace.IsNewObject(finalWorkResult)) // 신규
                        {
                            CreateLotObject(finalWorkResult);
                        }
                        else if (View.ObjectSpace.IsDeletedObject(finalWorkResult)) // 삭제
                        {
                            //CheckIfObjectIsInUse(newObjectSpace, company, "삭제");
                        }
                        else // 수정
                        {
                            //CheckIfObjectIsInUse(newObjectSpace, company, "수정");
                        }
                    }
                }
            }
            catch (UserFriendlyException ex)
            {
                throw new UserFriendlyException(ex);
            }
            catch (Exception ex)
            {
                string x = ex.StackTrace;
                throw new UserFriendlyException(ex);
            }

            finally
            {
                newObjectSpace?.Dispose(); // 자원 해제 (메모리 누수 방지하는거임)
            }
        }

        private void CreateLotObject(FinalWorkResult currentObject)
        {

            var lotType = View.ObjectSpace.GetObjects<LotType>().FirstOrDefault(x => x.LotTypeCode == "P") ?? throw new UserFriendlyException("LotTypeCode가 'I'인 LotType이 존재하지 않습니다.");
            var lotObject = View.ObjectSpace.CreateObject<Lot>();
            lotObject.LotTypeObject = lotType;
            lotObject.FactoryObject = currentObject?.DetailWorkInstructionObject?.MasterWorkInstructionObject?.MasterProductionPlanningObject?.FactoryObject;
            lotObject.WareHouseObject = currentObject?.DetailWorkInstructionObject?.WareHouseObject;
            lotObject.ItemObject = currentObject?.DetailWorkInstructionObject?.MasterWorkInstructionObject?.ItemObject;
            lotObject.ItemAccountObject = currentObject?.DetailWorkInstructionObject?.MasterWorkInstructionObject?.ItemObject.ItemAccountObject;
            lotObject.CreateQuantity = currentObject?.CreateQuantity ?? 0;
            lotObject.StockQuantity = currentObject?.CreateQuantity ?? 0;
            lotObject.LotNumber = lotType.LotTypeCode + currentObject?.DetailWorkInstructionObject?.MasterWorkInstructionObject?.ItemObject.ItemAccountObject?.ItemAccountCode + currentObject?.DetailWorkInstructionObject?.WareHouseObject?.FactoryObject?.LotCode
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
            View.ObjectSpace.Committing -= ObjectSpace_Committing;
            base.OnDeactivated();
        }
    }
}
