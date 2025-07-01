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
                    var oldMiddleWorkResult = (MiddleWorkResult)newObjectSpace.GetObjectByKey(typeof(MiddleWorkResult), middleWorkResult.Oid);
                    // 이전 공정이 존재하지 않거나 다음공정이 있는 경우 등록 불가능

                    // 다음 공정 라우팅 인덱스
                    //var nextRoutingIndex = middleWorkResult?.DetailWorkInstructionObject?.RoutingIndex + 1;

                    // 중간공정 등록되었을 때,
                    //var nextMiddleWorkResults = View.ObjectSpace.GetObjects<MiddleWorkResult>()
                    //    .Where(x => (x.DetailWorkInstructionObject?.RoutingIndex ?? 0) == nextRoutingIndex &&
                    //                 x.DetailWorkInstructionObject?.MasterWorkInstructionObject?.Oid == middleWorkResult?.DetailWorkInstructionObject?.MasterWorkInstructionObject?.Oid);

                    // 삭제 시 시리얼 자료가 다 사라져서 삭제되기 전 자료로 비교해야함.
                    var currentSerials = oldMiddleWorkResult?.AssySerialProductObjects?
                        .Select(x => x.SerialNumber)
                        .ToList() ?? new List<string>();

                    //bool nextMiddleContainSerial = nextMiddleWorkResults?
                    //                               .Any(result => result.AssySerialProductObjects?
                    //                               .Any(x => currentSerials.Contains(x.SerialNumber)) ?? false) ?? false;


                    var preRoutingIndex = middleWorkResult?.DetailWorkInstructionObject?.RoutingIndex - 1;

                    // 중간공정 등록되어 있을 때,
                    var preMiddleWorkResults = View.ObjectSpace.GetObjects<MiddleWorkResult>()
                        .Where(x => (x.DetailWorkInstructionObject?.RoutingIndex ?? 0) == preRoutingIndex &&
                                     x.DetailWorkInstructionObject?.MasterWorkInstructionObject?.Oid == middleWorkResult.DetailWorkInstructionObject?.MasterWorkInstructionObject?.Oid);

                    if (middleWorkResult?.DetailWorkInstructionObject?.RoutingIndex != 1 &&
                        preMiddleWorkResults.Count() == 0 &&
                        !preMiddleWorkResults.Any(x => x.DetailWorkInstructionQuantity != x.WorkInstructionQuantity))
                    {
                        throw new UserFriendlyException("이전 공정이 존재하지않아 내용을 등록, 수정할 수 없습니다.");
                    }

                    if (middleWorkResult?.DetailWorkInstructionObject?.Progress.CodeName == "완료")
                    {
                        throw new UserFriendlyException("작업지시가 완료되어 수정이 불가능합니다.");
                    }

                    if (View.ObjectSpace.IsNewObject(middleWorkResult)) // 신규인 경우
                    {
                        // 생산수량 > 가능수량(작지수량) -> 에러
                        if (middleWorkResult.GoodQuantity > middleWorkResult.AvailableGoodQuantity)
                        {
                            throw new UserFriendlyException("생산수량은 생산가능 수량을 초과 할 수 없습니다.");
                        }
                        if (middleWorkResult.GoodQuantity > 0)
                        {
                            var lastIndex = 0;
                            lastIndex = Convert.ToInt32(View.ObjectSpace.GetObjects<MiddleWorkResult>()
                                            .Where(x => x.DetailWorkInstructionObject?.Oid == middleWorkResult?.DetailWorkInstructionObject.Oid
                                            && x.DetailWorkInstructionObject?.RoutingIndex == middleWorkResult?.DetailWorkInstructionObject.RoutingIndex)
                                            .SelectMany(x => x.AssySerialProductObjects ?? Enumerable.Empty<AssySerialProduct>())
                                            .Select(x => x.SerialNumber.Substring(x.SerialNumber.Length - 3, 3))
                                            .DefaultIfEmpty("000")  // 결과가 없을 경우 기본값
                                            .Max());

                            if (middleWorkResult?.WorkInstructionQuantity == middleWorkResult?.DetailWorkInstructionQuantity)
                            {
                                for (int i = Convert.ToInt32(lastIndex); i < (int)middleWorkResult.GoodQuantity + Convert.ToInt32(lastIndex); i++)
                                {
                                    var serialProduct = View.ObjectSpace.CreateObject<AssySerialProduct>();
                                    serialProduct.MiddleWorkResultObject = middleWorkResult;
                                    serialProduct.SerialNumber = CreatedSerialNumberProduct(middleWorkResult, i);
                                    serialProduct.CreatedDateTime = DateTime.Now;
                                    serialProduct.WorkProcessDefectQuantity = preMiddleWorkResults.SelectMany(x => x.AssySerialProductObjects ?? Enumerable.Empty<AssySerialProduct>()).FirstOrDefault(x => x.SerialNumber == serialProduct?.SerialNumber)?.WorkProcessDefectQuantity ?? 0;

                                }
                            }
                        }
                    }
                    else
                    {
                        if (middleWorkResult.IsDeleted) // 삭제
                        {
                            var assySerialProductObjects = newObjectSpace.GetObjects<MiddleWorkResult>().Where(x => x.Oid == middleWorkResult.Oid).FirstOrDefault().AssySerialProductObjects.ToList();
                            foreach (var assySerialProductObject in assySerialProductObjects)
                            {
                                var workProcessDefectObject = ObjectSpace.GetObjects<MasterWorkProcessDefect>().Where(x => x.AssySerialProductObject?.Oid == assySerialProductObject?.Oid).FirstOrDefault();
                                if (workProcessDefectObject != null)
                                {
                                    throw new UserFriendlyException("공정 불량 등록 자료가 존재하여 수정할 수 없습니다.");
                                }
                            }
                        }
                        else
                        {
                            // ASSY 삭제
                            var assySerialProductObjects = View.ObjectSpace.GetObjects<MiddleWorkResult>().Where(x => x.Oid == middleWorkResult.Oid).FirstOrDefault().AssySerialProductObjects.ToList();
                            foreach (var assySerialProductObject in assySerialProductObjects)
                            {
                                var workProcessDefectObject = ObjectSpace.GetObjects<MasterWorkProcessDefect>().Where(x => x.AssySerialProductObject?.Oid == assySerialProductObject?.Oid).FirstOrDefault();
                                if (workProcessDefectObject != null)
                                {
                                    throw new UserFriendlyException("공정 불량 등록 자료가 존재하여 수정할 수 없습니다.");
                                }
                                else
                                {
                                    assySerialProductObject.Delete();
                                }
                            }

                            // 저장된 거 다시 삭제 후 다시 만들기 스킬
                            var middleWorkResultOldValue = newObjectSpace.GetObjects<MiddleWorkResult>().Where(w => w.Oid == middleWorkResult.Oid).FirstOrDefault();
                            // 생산수량 > 가능수량(작지수량)->에러
                            if (middleWorkResult.GoodQuantity > middleWorkResultOldValue.AvailableGoodQuantity + middleWorkResultOldValue.GoodQuantity)
                            {
                                throw new UserFriendlyException("생산수량은 생산가능 수량을 초과 할 수 없습니다.");
                            }
                            if (middleWorkResult.GoodQuantity > 0)
                            {
                                // AssySerialProduct 데이터 생성-----------------------------------------------------
                                var lastIndex = 0;
                                lastIndex = Convert.ToInt32(View.ObjectSpace.GetObjects<MiddleWorkResult>()
                                                .Where(x => x.DetailWorkInstructionObject?.Oid == middleWorkResult?.DetailWorkInstructionObject.Oid
                                                && x.DetailWorkInstructionObject?.RoutingIndex == middleWorkResult?.DetailWorkInstructionObject.RoutingIndex)
                                                .SelectMany(x => x.AssySerialProductObjects ?? Enumerable.Empty<AssySerialProduct>()).Select(x => x.SerialNumber.Substring(x.SerialNumber.Length - 3, 3))
                                                .DefaultIfEmpty("000")  // 결과가 없을 경우 기본값
                                                .Max());


                                if (middleWorkResult?.WorkInstructionQuantity == middleWorkResult?.DetailWorkInstructionQuantity)
                                {
                                    for (int i = Convert.ToInt32(lastIndex); i < (int)middleWorkResult.GoodQuantity + Convert.ToInt32(lastIndex); i++)
                                    {
                                        var serialProduct = View.ObjectSpace.CreateObject<AssySerialProduct>();
                                        serialProduct.MiddleWorkResultObject = middleWorkResult;
                                        serialProduct.SerialNumber = CreatedSerialNumberProduct(middleWorkResult, i);
                                        serialProduct.CreatedDateTime = DateTime.Now;
                                        serialProduct.WorkProcessDefectQuantity = preMiddleWorkResults.SelectMany(x => x.AssySerialProductObjects ?? Enumerable.Empty<AssySerialProduct>()).FirstOrDefault(x => x.SerialNumber == serialProduct?.SerialNumber)?.WorkProcessDefectQuantity ?? 0;

                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        // 제품 (시트 시리얼)
        private static string CreatedSerialNumberProduct(MiddleWorkResult middleWorkResult, int index)
        {
            var workInstructionNumber = middleWorkResult.DetailWorkInstructionObject.WorkInstructionNumber;
            var dateCode = DateTime.Now.ToString("yyMMdd");

            string serialNumber = $"{dateCode}{workInstructionNumber}{index + 1:D3}";
            return serialNumber;
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
