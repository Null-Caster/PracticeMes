using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using DevExpress.CodeParser;
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
using PracticeMes.Module.BusinessObjects.WorkResult;

namespace PracticeMes.Win.Controllers.WorkResult
{
    public partial class MasterWorkProcessDefectController : ViewController
    {
        public MasterWorkProcessDefectController()
        {
            InitializeComponent();
            TargetObjectType = typeof(MasterWorkProcessDefect);
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            View.ObjectSpace.Committing += ObjectSpace_Committing;
        }

        private void ObjectSpace_Committing(object sender, CancelEventArgs e)
        {
            IObjectSpace newObjectSpace = Application.CreateObjectSpace(TargetObjectType);
            var modifiedObjects = View.ObjectSpace.ModifiedObjects;

            try
            {
                foreach (var modifiedObject in modifiedObjects)
                {
                    if (modifiedObject is MasterWorkProcessDefect masterWorkProcessDefect)
                    {
                        if (View.ObjectSpace.IsNewObject(masterWorkProcessDefect))
                        {
                            var nextRoutingIndex = masterWorkProcessDefect?.DetailWorkInstructionObject?.RoutingIndex + 1;

                            var nextMiddleWorkResults = View.ObjectSpace.GetObjects<MiddleWorkResult>()
                                .Any(x => (x.DetailWorkInstructionObject?.RoutingIndex ?? 0) == nextRoutingIndex &&
                                             x.DetailWorkInstructionObject?.MasterWorkInstructionObject?.Oid == masterWorkProcessDefect?.DetailWorkInstructionObject?.MasterWorkInstructionObject?.Oid);

                            var nextFinalWorkResults = View.ObjectSpace.GetObjects<FinalWorkResult>()
                                .Any(x => (x.DetailWorkInstructionObject?.RoutingIndex ?? 0) == nextRoutingIndex &&
                                             x.DetailWorkInstructionObject?.MasterWorkInstructionObject?.Oid == masterWorkProcessDefect?.DetailWorkInstructionObject?.MasterWorkInstructionObject?.Oid);

                            if (nextMiddleWorkResults || nextFinalWorkResults)
                            {
                                throw new UserFriendlyException("다음 공정 및 최종 공정이 등록되어 생성 할 수 없습니다.");
                            }
                        }
                        else if (View.ObjectSpace.IsDeletedObject(masterWorkProcessDefect))
                        {
                            var oldMaster = (MasterWorkProcessDefect)newObjectSpace.GetObjectByKey(typeof(MasterWorkProcessDefect), masterWorkProcessDefect.Oid);

                            // (마스터 삭제 시) 불량 수량 총합 가져오기
                            var oldTotal = newObjectSpace
                                .GetObjects<DetailWorkProcessDefect>()
                                .Where(x => x.MasterWorkProcessDefectObject.Oid == oldMaster.Oid)
                                .Sum(x => x.DefectQuantity);

                            var instructionOid = oldMaster.DetailWorkInstructionObject?.Oid;

                            if (instructionOid != null)
                            {
                                var targetMiddleResult = newObjectSpace
                                    .GetObjects<MiddleWorkResult>()
                                    .FirstOrDefault(x => x.DetailWorkInstructionObject != null &&
                                                         x.DetailWorkInstructionObject.Oid == instructionOid);

                                if (targetMiddleResult != null)
                                {
                                    targetMiddleResult.GoodQuantity += oldTotal;
                                    newObjectSpace.CommitChanges();
                                }
                            }
                        }
                        else
                        {
                        }
                    }
                    else if (modifiedObject is DetailWorkProcessDefect detailWorkProcessDefect)
                    {
                        if (View.ObjectSpace.IsNewObject(detailWorkProcessDefect))
                        {
                            var nextRoutingIndex = detailWorkProcessDefect?.MasterWorkProcessDefectObject?.DetailWorkInstructionObject?.RoutingIndex + 1;

                            var nextMiddleWorkResults = View.ObjectSpace.GetObjects<MiddleWorkResult>()
                                .Any(x => (x.DetailWorkInstructionObject?.RoutingIndex ?? 0) == nextRoutingIndex &&
                                             x.DetailWorkInstructionObject?.MasterWorkInstructionObject?.Oid == detailWorkProcessDefect?.MasterWorkProcessDefectObject?.DetailWorkInstructionObject?.MasterWorkInstructionObject?.Oid);

                            var nextFinalWorkResults = View.ObjectSpace.GetObjects<FinalWorkResult>()
                                .Any(x => (x.DetailWorkInstructionObject?.RoutingIndex ?? 0) == nextRoutingIndex &&
                                             x.DetailWorkInstructionObject?.MasterWorkInstructionObject?.Oid == detailWorkProcessDefect?.MasterWorkProcessDefectObject?.DetailWorkInstructionObject?.MasterWorkInstructionObject?.Oid);

                            if (nextMiddleWorkResults || nextFinalWorkResults)
                            {
                                throw new UserFriendlyException("다음 공정 및 최종 공정이 등록되어 생성 할 수 없습니다.");
                            }

                            // 여기서 부터 실제 로직
                            var instructionOid = detailWorkProcessDefect?.MasterWorkProcessDefectObject?.DetailWorkInstructionObject?.Oid;

                            if (instructionOid != null)
                            {
                                // 대응하는 MiddleWorkResult 찾기
                                var targetMiddleResult = View.ObjectSpace.GetObjects<MiddleWorkResult>()
                                    .FirstOrDefault(x => x.DetailWorkInstructionObject.Oid == instructionOid);

                                if (targetMiddleResult != null)
                                {
                                    targetMiddleResult.GoodQuantity -= detailWorkProcessDefect.DefectQuantity;

                                    // GoodQuantity가 음수가 되면 예외 처리
                                    if (targetMiddleResult.GoodQuantity < 0)
                                    {
                                        throw new UserFriendlyException("입력된 불량 수량이 입력 가능 수량보다 큽니다.");
                                    }
                                }
                            }
                        }
                        else if (View.ObjectSpace.IsDeletedObject(detailWorkProcessDefect))
                        {
                            var masterView = View.CurrentObject as MasterWorkProcessDefect;
                            if (masterView == null)
                                return;

                            var instruction = masterView.DetailWorkInstructionObject;
                            if (instruction == null)
                                return;

                            var targetMiddleResult = View.ObjectSpace.GetObjects<MiddleWorkResult>()
                                .FirstOrDefault(x => x.DetailWorkInstructionObject != null &&
                                                     x.DetailWorkInstructionObject.Oid == instruction.Oid);

                            if (targetMiddleResult != null)
                            {
                                targetMiddleResult.GoodQuantity += detailWorkProcessDefect.DefectQuantity;
                            }
                        }
                        else
                        {
                            var current = detailWorkProcessDefect;
                            var old = (DetailWorkProcessDefect)newObjectSpace.GetObjectByKey(
                                typeof(DetailWorkProcessDefect),
                                current.Oid
                            );

                            if (old != null && old.DefectQuantity != current.DefectQuantity)
                            {
                                var diff = old.DefectQuantity - current.DefectQuantity;

                                var instructionOid = current.MasterWorkProcessDefectObject?.DetailWorkInstructionObject?.Oid;

                                if (instructionOid != null)
                                {
                                    var middle = View.ObjectSpace.GetObjects<MiddleWorkResult>()
                                        .FirstOrDefault(x => x.DetailWorkInstructionObject.Oid == instructionOid);

                                    if (middle != null)
                                    {
                                        middle.GoodQuantity += diff;

                                        if (middle.GoodQuantity < 0)
                                            throw new UserFriendlyException("수정된 불량 수량이 남은 수량보다 많습니다.");
                                    }
                                }
                            }
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
                throw new UserFriendlyException(ex);
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
