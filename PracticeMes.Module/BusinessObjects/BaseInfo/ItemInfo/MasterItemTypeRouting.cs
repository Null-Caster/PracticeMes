using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using PracticeMes.Module.BusinessObjects.BaseInfo.CommonInfo;

namespace PracticeMes.Module.BusinessObjects.BaseInfo.ItemInfo
{
    [DefaultClassOptions]
    [NavigationItem("품목 정보"), XafDisplayName("라우팅 등록")]
    [DefaultListViewOptions(MasterDetailMode.ListViewOnly, true, NewItemRowPosition.Top)]
    public class MasterItemTypeRouting : BaseObject
    {
        #region Properties
        [VisibleInLookupListView(true)]
        [DataSourceCriteria("IsEnabled == True")]
        [ModelDefault("LookupProperty", nameof(Factory.FactoryName))]
        [LookupEditorMode(LookupEditorMode.AllItemsWithSearch)]
        [RuleRequiredField(CustomMessageTemplate = "공장 이름을 입력하세요.")]
        [XafDisplayName("공장 이름"), ToolTip("공장 이름")]
        public Factory FactoryObject
        {
            get { return GetPropertyValue<Factory>(nameof(FactoryObject)); }
            set { SetPropertyValue(nameof(FactoryObject), value); }
        }

        [VisibleInLookupListView(true)]
        [ImmediatePostData]
        [DataSourceCriteria("IsEnabled == True")]
        [ModelDefault("LookupProperty", nameof(ItemAccount.ItemAccountName))]
        [LookupEditorMode(LookupEditorMode.AllItemsWithSearch)]
        [RuleRequiredField(CustomMessageTemplate = "품목계정을 입력하세요.")]
        [XafDisplayName("품목계정"), ToolTip("품목계정")]
        public ItemAccount ItemType
        {
            get { return GetPropertyValue<ItemAccount>(nameof(ItemType)); }
            set { SetPropertyValue(nameof(ItemType), value); }
        }

        [VisibleInLookupListView(true)]
        [DataSourceCriteria("IsEnabled == True")]
        [ModelDefault("LookupProperty", nameof(ItemGroup.ItemGroupName))]
        [LookupEditorMode(LookupEditorMode.AllItemsWithSearch)]
        [XafDisplayName("품목그룹"), ToolTip("품목그룹")]
        public ItemGroup ItemGroupObject
        {
            get { return GetPropertyValue<ItemGroup>(nameof(ItemGroupObject)); }
            set { SetPropertyValue(nameof(ItemGroupObject), value); }
        }

        [VisibleInLookupListView(true)]
        [ModelDefault("EditMask", "##0.#")]
        [XafDisplayName("Rev"), ToolTip("Rev")]
        public double Revision
        {
            get { return GetPropertyValue<double>(nameof(Revision)); }
            set { SetPropertyValue(nameof(Revision), value); }
        }

        [VisibleInLookupListView(true)]
        [XafDisplayName("활성화 여부"), ToolTip("활성화 이름")]
        public bool IsEnabled
        {
            get { return GetPropertyValue<bool>(nameof(IsEnabled)); }
            set { SetPropertyValue(nameof(IsEnabled), value); }
        }

        [VisibleInLookupListView(true)]
        [XafDisplayName("비고"), ToolTip("비고")]
        public string Remark
        {
            get { return GetPropertyValue<string>(nameof(Remark)); }
            set { SetPropertyValue(nameof(Remark), value); }
        }

        [VisibleInLookupListView(true)]
        [ModelDefault("AllowEdit", "False")]
        [ModelDefault("DisplayFormat", "yyyy/MM/dd HH:mm:ss")]
        [ModelDefault("EditMask", "yyyy/MM/dd HH:mm:ss")]
        [XafDisplayName("생성 일시"), ToolTip("항목이 생성된 일시입니다.")]
        public DateTime CreatedDateTime
        {
            get { return GetPropertyValue<DateTime>(nameof(CreatedDateTime)); }
            set { SetPropertyValue(nameof(CreatedDateTime), value); }
        }


        [Association(@"DetailItemTypeRoutingReferenceMasterItemTypeRouting")]
        [XafDisplayName("라우팅 상세")]
        public XPCollection<DetailItemTypeRouting> DetailItemTypeRoutingObjects { get { return GetCollection<DetailItemTypeRouting>(nameof(DetailItemTypeRoutingObjects)); } }

        #endregion

        #region Constructors
        public MasterItemTypeRouting(Session session) : base(session) { }
        #endregion

        #region Methods
        public override void AfterConstruction()
        {
            base.AfterConstruction();

            CreatedDateTime = DateTime.Now;
            IsEnabled = true;
        }
        #endregion
    }
}