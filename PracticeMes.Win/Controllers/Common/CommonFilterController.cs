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
using PracticeMes.Module.BusinessObjects.ProductPlanning;
using PracticeMes.Module.BusinessObjects.WorkResult;

namespace PracticeMes.Win.Controllers.Common;

public partial class CommonFilterController : FilterController
{
    /// <summary>
    /// 각 비즈니스 오브젝트 리스트뷰 별로 필터를 추가할 수 있는 메소드
    /// 필터를 추가해야 하는 비즈니스오브젝트가 존재하면 InitializeDefaultBusinessObjectFilter 메소드 내 Case 문에 비즈니스오브젝트를 추가하여 처리
    /// 사용자 입력 범위 필터(날짜, 숫자 등)은 각 비즈니스오브젝트 뷰컨트롤러에서 구현하도록 함 모델이 필요한데 필드가 다 다르기 때문에 그렇게 구현할 수 밖에 없음.
    /// 그 이외 필요한 기본 필터는 여기서 구현
    /// </summary>
    public CommonFilterController()
    {
        InitializeComponent();
        TargetViewType = ViewType.ListView;
        SetFilterAction.SelectedItemChanged += SetFilterAction_SelectedItemChanged;
    }

    /// <summary>
    /// 이 이벤트 안쓰면 리스트 뷰 최초조회시에 필터 조건 적용이 안된다.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void SetFilterAction_SelectedItemChanged(object sender, EventArgs e)
    {
        if (View is ListView && View.IsRoot)
        {
            ApplySelectedFilter();
        }
    }

    protected override void OnActivated()
    {
        base.OnActivated();
        InitializeDefaultBusinessObjectFilter();
        UpdateSetFilterActionVisibility();
    }
    protected override void OnViewControlsCreated()
    {
        base.OnViewControlsCreated();
    }
    protected override void OnDeactivated()
    {
        base.OnDeactivated();
        SetFilterAction.SelectedItemChanged -= SetFilterAction_SelectedItemChanged;
    }

    // 필터 선택 후 필터 적용하는 메소드
    protected void ApplySelectedFilter()
    {
        if (View is ListView listView && SetFilterAction.SelectedItem != null)
        {
            string filterCriteria = SetFilterAction.SelectedItem.Data as string;
            if (!string.IsNullOrEmpty(filterCriteria))
            {
                CriteriaOperator criteria = CriteriaOperator.Parse(filterCriteria);
                listView.CollectionSource.Criteria["FilterCriteria"] = criteria;
            }
            else
            {
                listView.CollectionSource.Criteria.Remove("FilterCriteria");
            }
        }
    }

    protected void UpdateSetFilterActionVisibility()
    {
        SetFilterAction.Active["IsMainListView"] = View is ListView && View.IsRoot;
    }

    private void InitializeDefaultBusinessObjectFilter()
    {
        var targetType = View?.ObjectTypeInfo?.Type;
        if (targetType != null)
        {
            SetFilterAction.BeginUpdate();
            SetFilterAction.Items.Add(new ChoiceActionItem("전체 조회", "True"));
            switch (targetType)
            {
                case Type t when t == typeof(MasterProductionPlanning):
                    // 필터 정의
                    var masterProductionPlanningFilter1 = new ChoiceActionItem("미완료 작업 조회", "[IsComplete] = false");
                    SetFilterAction.Items.Add(masterProductionPlanningFilter1);
                    // 기본 필터 셋팅
                    SetFilterAction.SelectedItem = masterProductionPlanningFilter1;
                    break;
                case Type t when t == typeof(MasterWorkInstruction):
                    // 필터 정의
                    var masterWorkInstructionFilter1 = new ChoiceActionItem("미완료 작업 조회", "[IsComplete] = false");
                    SetFilterAction.Items.Add(masterWorkInstructionFilter1);
                    // 기본 필터 셋팅
                    SetFilterAction.SelectedItem = masterWorkInstructionFilter1;
                    break;
                case Type t when t == typeof(MiddleWorkResult):
                    // 필터 정의
                    var middleWorkResultFilter1 = new ChoiceActionItem("미완료 작업 조회", "[DetailWorkInstructionObject.Progress.CodeName] != '완료'");
                    SetFilterAction.Items.Add(middleWorkResultFilter1);
                    // 기본 필터 셋팅
                    SetFilterAction.SelectedItem = middleWorkResultFilter1;
                    break;
                case Type t when t == typeof(FinalWorkResult):
                    // 필터 정의
                    var finalWorkResultSplitFilter1 = new ChoiceActionItem("미완료 작업 조회", "[DetailWorkInstructionObject.Progress.CodeName] != '완료'");
                    SetFilterAction.Items.Add(finalWorkResultSplitFilter1);
                    // 기본 필터 셋팅
                    SetFilterAction.SelectedItem = finalWorkResultSplitFilter1;
                    break;
                case Type t when t == typeof(DetailWorkInstruction):
                    // 필터 정의
                    if (View.Id == "DetailWorkInstruction_ListView")
                    {
                        var detailWorkInstructionFilter1 = new ChoiceActionItem("미완료 작업 조회", "[Progress.CodeName] != '완료'");
                        SetFilterAction.Items.Add(detailWorkInstructionFilter1);
                        // 기본 필터 셋팅
                        SetFilterAction.SelectedItem = detailWorkInstructionFilter1;
                    }
                    break;
                default:
                    break;
            }
            SetFilterAction.EndUpdate();
            if (View is ListView && View.IsRoot)
            {
                ApplySelectedFilter();
            }

        }
    }
}
