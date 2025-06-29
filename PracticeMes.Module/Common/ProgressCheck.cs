using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.Xpo;
using PracticeMes.Module.BusinessObjects.BaseInfo.CommonInfo;
using PracticeMes.Module.BusinessObjects.ProductPlanning;
using PracticeMes.Module.BusinessObjects.Purchase;

namespace PracticeMes.Module.Common;

public class ProgressCheck
{
    public UniversalMinorCode FindProgress(Session session, object businessObject, string businessObjectName)
    {
        IList<string> comparsionList = new List<string>();
        var complete = new XPCollection<UniversalMinorCode>(session).Where(x => x.UniversalMajorCodeObject.MajorCode == "Progress" && x.CodeName == "완료").FirstOrDefault();
        var proceeding = new XPCollection<UniversalMinorCode>(session).Where(x => x.UniversalMajorCodeObject.MajorCode == "Progress" && x.CodeName == "진행중").FirstOrDefault();
        var Ready = new XPCollection<UniversalMinorCode>(session).Where(x => x.UniversalMajorCodeObject.MajorCode == "Progress" && x.CodeName == "예정").FirstOrDefault();
        var pause = new XPCollection<UniversalMinorCode>(session).Where(x => x.UniversalMajorCodeObject.MajorCode == "Progress" && x.CodeName == "중단").FirstOrDefault();
        switch (businessObjectName)
        {
            case "MasterWorkInstruction":
                MasterWorkInstruction masterWorkInstructionObject = (MasterWorkInstruction)businessObject;
                var detailWorkInstructionObjects = new XPCollection<DetailWorkInstruction>(session)
                    .Where(x => x.MasterWorkInstructionObject?.Oid == masterWorkInstructionObject?.Oid).ToList();
                if (detailWorkInstructionObjects.Count() > 0)
                {
                    foreach (var detailWorkInstructionObject in detailWorkInstructionObjects)
                    {
                        comparsionList.Add(detailWorkInstructionObject?.Progress?.CodeName);
                    }
                }
                break;
            case "MasterProductionPlanning":
                MasterProductionPlanning masterProductionPlanning = (MasterProductionPlanning)businessObject;
                var masterWorkInstructionObjects = new XPCollection<MasterWorkInstruction>(session)
                    .Where(x => x.MasterProductionPlanningObject.Oid == masterProductionPlanning.Oid).ToList();
                if (masterWorkInstructionObjects.Count() > 0)
                {
                    foreach (var masterWorkInstructionObjectss in masterWorkInstructionObjects)
                    {
                        comparsionList.Add(masterWorkInstructionObjectss?.Progress?.CodeName);
                    }
                }
                break;
            case "MasterPurchaseOrder":
                MasterPurchaseOrder masterPurchaseOrder = (MasterPurchaseOrder)businessObject;
                var masterPurchaseInputObjects = new XPCollection<MasterPurchaseInput>(session)
                    .Where(x => x.MasterPurchaseOrderObject.Oid == masterPurchaseOrder.Oid);
                if (masterPurchaseInputObjects.Count() > 0)
                {
                    foreach (var masterPurchaseOrderObjectss in masterPurchaseInputObjects)
                    {
                        //comparsionList.Add(masterPurchaseOrderObjectss?.Progress?.CodeName);
                    }
                }
                break;
            case "MasterPurchaseInput":
                MasterPurchaseInput masterPurchaseInput = (MasterPurchaseInput)businessObject;
                var detailPurchaseInputObjects = new XPCollection<DetailPurchaseInput>(session)
                     .Where(x => x.MasterPurchaseInputObject.Oid == masterPurchaseInput.Oid);
                if (detailPurchaseInputObjects.Count() > 0)
                {
                    foreach (var masterPurchaseOrderObjectss in detailPurchaseInputObjects)
                    {
                        var detailPurchaseOrderObject = new XPCollection<DetailPurchaseOrder>(session)
                            .Where(x => x.ItemObject?.Oid == masterPurchaseOrderObjectss?.ItemObject?.Oid)
                            .Sum(x => x.PurchaseOrderQuantity);
                        if (detailPurchaseOrderObject == masterPurchaseOrderObjectss.PurchaseInputQuantity)
                        {
                            comparsionList.Add(complete?.CodeName);
                        }
                        else if (detailPurchaseOrderObject > 0)
                        {
                            comparsionList.Add(proceeding?.CodeName);
                        }
                        else
                        {
                            comparsionList.Add(Ready?.CodeName);
                        }
                    }
                }
                break;
        }
        if (comparsionList == null)
        {
            return Ready;
        }
        if (comparsionList.All(x => x.Equals("예정")))
        {
            return Ready;
        }
        else if (comparsionList.All(x => x.Equals("완료")))
        {
            return complete;
        }
        else if (comparsionList.All(x => x.Equals("예정") || x.Equals("완료")))
        {
            return proceeding;
        }
        else if (comparsionList.Any(x => x.Equals("진행중")))
        {
            return proceeding;
        }
        else if (comparsionList.All(x => x.Equals("중단")))
        {
            return pause;
        }
        else if (comparsionList.Any(x => x.Equals("중단")))
        {
            return proceeding;
        }

        return Ready;

    }
}
