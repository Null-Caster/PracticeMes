using System;
using System.Collections.Generic;
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
using PracticeMes.Module.BusinessObjects.BaseInfo.CommonInfo;
using PracticeMes.Module.BusinessObjects.BaseInfo.ProductionInfo;
using PracticeMes.Module.BusinessObjects.Lot;
using PracticeMes.Module.BusinessObjects.ProductPlanning;
using PracticeMes.Module.BusinessObjects.Sales;

namespace PracticeMes.Win.Controllers.BaseInfo.CommonInfo;

public partial class FactoryController : ViewController
{
    public FactoryController()
    {
        InitializeComponent();
        TargetObjectType = typeof(Factory);
    }

    protected override void OnActivated()
    {
        base.OnActivated();
        if (View.Id != "CFactory_DetailView" && View.Id != "Factory_ListView")
            return;

        ObjectSpace.Committing += ObjectSpace_Committing;
        AssignDefaultCompanyObject();
        AssignLotCode();
    }

    private void ObjectSpace_Committing(object sender, System.ComponentModel.CancelEventArgs e)
    {
        try
        {
            var modifiedObjects = View.ObjectSpace.ModifiedObjects;
            IObjectSpace newObjectSpace = Application.CreateObjectSpace(typeof(Factory));

            if (modifiedObjects == null)
            {
                throw new UserFriendlyException("삭제 또는 수정된 회사 정보가 없습니다.");
            }

            foreach (var modifiedObject in modifiedObjects)
            {
                if (modifiedObject is Factory factory)
                {
                    if (View.ObjectSpace.IsNewObject(factory))
                    {

                    }
                    else if (View.ObjectSpace.IsDeletedObject(factory))
                    {
                        CheckIfObjectIsInUse(newObjectSpace, factory, "삭제");
                    }
                    else
                    {
                        CheckIfObjectIsInUse(newObjectSpace, factory, "수정");
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
            throw new UserFriendlyException("사업장 삭제 처리 중 오류가 발생했습니다: " + ex.Message);
        }
    }

    // 기본 사업장 자동 할당
    private void AssignDefaultCompanyObject()
    {
        var factory = View.CurrentObject as Factory;
        if (factory == null || !ObjectSpace.IsNewObject(factory))
            return;

        if (factory.CompanyObject == null)
        {
            factory.CompanyObject = GetDefaultCompany();
            ObjectSpace.SetModified(factory);
        }
    }

    // 사업장 조회
    private Company GetDefaultCompany()
    {
        return ObjectSpace.GetObjectsQuery<Company>()
            .Where(c => c.IsEnabled)
            .OrderBy(c => c.CreatedDateTime)
            .FirstOrDefault();
    }

    // 자동으로 LotCode 할당
    private void AssignLotCode()
    {
        var factory = View.CurrentObject as Factory;
        if (factory == null || !ObjectSpace.IsNewObject(factory))
            return;

        if (!string.IsNullOrEmpty(factory.LotCode))
            return;

        var availableCode = GetAvailableLotCode();

        if (availableCode == null)
        {
            throw new UserFriendlyException("현재 사용 가능한 LOT 코드가 없습니다.");
        }

        factory.LotCode = availableCode;
        ObjectSpace.SetModified(factory);
    }

    // 사용하지 않은 알파벳 중 가장 앞의 코드 반환
    private string GetAvailableLotCode()
    {
        List<string> alphabet = new() { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };

        var usedCodes = ObjectSpace.GetObjectsQuery<Factory>()
            .Select(f => f.LotCode)
            .Where(code => code != null)
            .ToList();

        foreach (var code in usedCodes)
        {
            alphabet.Remove(code);
        }

        return alphabet.FirstOrDefault(); // null일 수 있음 (예외는 상위 함수에서 처리)
    }

    private static void CheckIfObjectIsInUse(IObjectSpace newObjectSpace, Factory currentFactory, string action)
    {
        var referencingList = new List<string>();

        if (newObjectSpace.GetObjects<Employee>().Any(x => x.FactoryObject.Oid == currentFactory.Oid))
            referencingList.Add("직원 등록");

        if (newObjectSpace.GetObjects<Department>().Any(x => x.FactoryObject.Oid == currentFactory.Oid))
            referencingList.Add("부서 등록");

        if (newObjectSpace.GetObjects<Lot>().Any(x => x.FactoryObject.Oid == currentFactory.Oid))
            referencingList.Add("Lot 관리");

        if (newObjectSpace.GetObjects<WareHouse>().Any(x => x.FactoryObject.Oid == currentFactory.Oid))
            referencingList.Add("창고 등록");

        if (newObjectSpace.GetObjects<WorkCenter>().Any(x => x.FactoryObject.Oid == currentFactory.Oid))
            referencingList.Add("작업장 등록");

        if (newObjectSpace.GetObjects<WorkLine>().Any(x => x.FactoryObject.Oid == currentFactory.Oid))
            referencingList.Add("작업 라인 등록");

        if (newObjectSpace.GetObjects<MasterProductionPlanning>().Any(x => x.FactoryObject.Oid == currentFactory.Oid))
            referencingList.Add("생산 계획 등록");

        if (newObjectSpace.GetObjects<MasterSalesOrder>().Any(x => x.FactoryObject.Oid == currentFactory.Oid))
            referencingList.Add("수주 등록");

        if (referencingList.Any())
        {
            string UsingList = string.Join(", ", referencingList);
            throw new UserFriendlyException(
                     $"이 공장은 다음 메뉴에서 사용 중이므로 {action}할 수 없습니다:\n[{UsingList}]");
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
