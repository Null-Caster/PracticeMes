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
using PracticeMes.Module.BusinessObjects.Meterial;
using PracticeMes.Module.BusinessObjects.ProductPlanning;
using PracticeMes.Module.BusinessObjects.WorkResult;

namespace PracticeMes.Win.Controllers.WorkResult
{
    public partial class MiddleWorkResultController : ViewController
    {
        public MiddleWorkResultController()
        {
            InitializeComponent();
            TargetObjectType = typeof(MiddleWorkResult);
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            View.ObjectSpace.Committing += ObjectSpace_Committing;
        }

        private void ObjectSpace_Committing(object sender, CancelEventArgs e)
        {

            var modifiedObjects = View.ObjectSpace.ModifiedObjects;
            IObjectSpace newObjectSpace = Application.CreateObjectSpace(typeof(MiddleWorkResult));

            foreach (var modifiedObject in modifiedObjects)
            {
                if (modifiedObject is MiddleWorkResult middleWorkResult)
                {
                    if (!View.ObjectSpace.IsNewObject(middleWorkResult) && !View.ObjectSpace.IsDeletedObject(middleWorkResult))
                    {
                        var hasDefect = newObjectSpace.GetObjects<MasterWorkProcessDefect>()
                            .Any(x => x.DetailWorkInstructionObject.Oid == middleWorkResult.DetailWorkInstructionObject.Oid);

                        if (hasDefect)
                        {
                            throw new UserFriendlyException("해당 공정에 불량 정보가 등록되어 있어 수정할 수 없습니다.");
                        }
                    }

                    if (View.ObjectSpace.IsNewObject(middleWorkResult))
                    {
                        MiddleWorkResult test = middleWorkResult;
                    }
                    else if (View.ObjectSpace.IsDeletedObject(middleWorkResult))
                    {

                    }
                    else { }
                }
            }
        }

        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();

            // 해당 중간 공정에 대한 불량 공정이 등록되어 있을 경우 읽기 전용으로 변경
            if (View is DetailView detailView && detailView.CurrentObject is MiddleWorkResult currentResult)
            {
                if (currentResult.DetailWorkInstructionObject == null)
                    return; // 아직 DetailWorkInstruction을 선택 안 한 상태 → 그냥 빠져나감

                var os = View.ObjectSpace;
                var hasDefect = os.GetObjects<MasterWorkProcessDefect>()
                    .Any(x => x.DetailWorkInstructionObject != null &&
                              x.DetailWorkInstructionObject.Oid == currentResult.DetailWorkInstructionObject.Oid);

                if (hasDefect)
                {
                    detailView.ViewEditMode = ViewEditMode.View;
                    Application.ShowViewStrategy.ShowMessage("해당 공정에 불량 정보가 등록되어 있어 수정할 수 없습니다.", InformationType.Warning);
                }
            }
        }
        protected override void OnDeactivated()
        {
            ObjectSpace.Committing -= ObjectSpace_Committing;
            base.OnDeactivated();
        }
    }
}
