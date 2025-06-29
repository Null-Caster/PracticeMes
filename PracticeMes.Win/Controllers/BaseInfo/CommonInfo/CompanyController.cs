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
using PracticeMes.Module.BusinessObjects.BaseInfo.CommonInfo;
using PracticeMes.Module.BusinessObjects.BaseInfo.ProductionInfo;

namespace PracticeMes.Win.Controllers.BaseInfo.CommonInfo;

public partial class CompanyController : ViewController
{
    public CompanyController()
    {
        InitializeComponent();
        TargetObjectType = typeof(Company);
    }
    protected override void OnActivated()
    {
        base.OnActivated();
        if (View.Id != "Company_DetailView" && View.Id != "Company_ListView")
            return;

        ObjectSpace.Committing += ObjectSpace_Committing;
    }

    private void ObjectSpace_Committing(object sender, CancelEventArgs e)
    {
        try
        {
            var modifiedObjects = View.ObjectSpace.ModifiedObjects;
            IObjectSpace newObjectSpace = Application.CreateObjectSpace(typeof(Company));

            foreach (var modifiedObject in modifiedObjects)
            {
                if (modifiedObject is Company company)
                {
                    if (View.ObjectSpace.IsNewObject(company)) // 신규
                    {

                    }
                    else if (View.ObjectSpace.IsDeletedObject(company)) // 삭제
                    {
                        CheckIfObjectIsInUse(newObjectSpace, company, "삭제");
                    }
                    else // 수정
                    {
                        CheckIfObjectIsInUse(newObjectSpace, company, "수정");
                    }
                }
            }
        }
        catch (UserFriendlyException ex)
        {
            throw new UserFriendlyException("사업장 삭제 처리 중 오류가 발생했습니다: " + ex.Message);
        }
        catch (Exception ex)
        {
            throw new UserFriendlyException("사업장 삭제 처리 중 오류가 발생했습니다: (원인 미상 오류) " + ex.Message);
        }
    }

    // 기존 메모리 기반 방식(Session.CollectReferencingObjects) 사용 시 모든 객체 순회 참조
    // 많은 객체가 로딩된 상황, 불필요한 객체 메모리 호출 등으로 성능 하락 가능성 높음
    // Company를 참조하는 객체가 존재하는지 직접 SQL 수준에서 필터링
    private static void CheckIfObjectIsInUse(IObjectSpace newObjectSpace, Company currentCompany, string action)
    {
        var referencingList = new List<string>();

        if (newObjectSpace.GetObjects<Employee>().Any(x => x.CompanyObject.Oid == currentCompany.Oid))
            referencingList.Add("직원 등록");

        if (newObjectSpace.GetObjects<Department>().Any(x => x.CompanyObject.Oid == currentCompany.Oid))
            referencingList.Add("부서 등록");

        if (newObjectSpace.GetObjects<Factory>().Any(x => x.CompanyObject.Oid == currentCompany.Oid))
            referencingList.Add("공장 등록");

        if (newObjectSpace.GetObjects<WareHouse>().Any(x => x.CompanyObject.Oid == currentCompany.Oid))
            referencingList.Add("창고 등록");

        if (newObjectSpace.GetObjects<WorkLine>().Any(x => x.CompanyObject.Oid == currentCompany.Oid))
            referencingList.Add("작업라인 등록");

        if (referencingList.Any())
        {
            string UsingList = string.Join(", ", referencingList);
            throw new UserFriendlyException(
                     $"이 회사는 다음 메뉴에서 사용 중이므로 {action}할 수 없습니다:\n[{UsingList}]");
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
