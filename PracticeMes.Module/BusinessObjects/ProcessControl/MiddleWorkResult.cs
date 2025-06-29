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

namespace PracticeMes.Module.BusinessObjects.ProcessControl
{
    [DefaultClassOptions]
    [NavigationItem("공정 관리"), XafDisplayName("중간 공정 실적 등록")]
    [DefaultListViewOptions(MasterDetailMode.ListViewOnly, true, NewItemRowPosition.Top)]
    public class MiddleWorkResult : BaseObject
    {
        #region Properties
        #endregion

        #region Constructors
        public MiddleWorkResult(Session session) : base(session){}
        #endregion

        #region Methods
        public override void AfterConstruction()
        {
            base.AfterConstruction();
        }
        #endregion
    }
}