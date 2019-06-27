﻿
//=====================================================================
// This file was generated by Siemens.Simatic Platform
// 
// LiXiao Info Tech Ltd. Copyright (c) 2014 All rights reserved. 
//=====================================================================

using System;
using System.Collections.Generic;
using System.Text;
using Siemens.Simatic.Platform.Core;
using Siemens.Simatic.ALT.Common.Persistence;

namespace Siemens.Simatic.ALT.Common
{
    /// <summary>
    /// CV_PM_ALT_CRITERION_COND Class.
    /// </summary>
    /// Title: CV_PM_ALT_CRITERION_COND
    /// Copyright: LiXiao Info Tech Ltd.
    /// Version: 1.0
    [Serializable]
    public class CV_PM_ALT_CRITERION_COND : PersistentCV_PM_ALT_CRITERION_COND
    {
    }
}

namespace Siemens.Simatic.ALT.Common.Persistence
{
    /// <summary>
    /// CV_PM_ALT_CRITERION_COND PersistentClass.
    /// </summary>
    /// Title: PersistentCV_PM_ALT_CRITERION_COND
    /// Copyright: LiXiao Info Tech Ltd.
    /// Version: 1.0
    [Serializable]
    public class PersistentCV_PM_ALT_CRITERION_COND : BaseEntity
    {
        public PersistentCV_PM_ALT_CRITERION_COND()
            : base()
        {
            this.InitialDefaultValues();
        }


        #region Private Fields

        private Guid? _ConditionID;
        private Guid? _AlertID;
        private String _AlertName;
        private String _AlertAlias;
        private String _Relation;
        private String _Element;
        private String _ColumnAlias;
        private String _Operator;
        private String _Value;
        private Int32? _Sequence;
        private Guid? _ParentID;
        private Int32? _CondLevel;
        private Boolean? _IsActive;
        private String _CreatedBy;
        private DateTime? _CreatedOn;
        private String _ModifiedBy;
        private DateTime? _ModifiedOn;

        #endregion

        #region Public Properties

        public Guid? ConditionID
        {
            get { return _ConditionID; }
            set
            {
                _ConditionID = value;
                this.SetNotDefaultValue("ConditionID");
            }
        }
        public Guid? AlertID
        {
            get { return _AlertID; }
            set
            {
                _AlertID = value;
                this.SetNotDefaultValue("AlertID");
            }
        }
        public String AlertName
        {
            get { return _AlertName; }
            set
            {
                _AlertName = value;
                this.SetNotDefaultValue("AlertName");
            }
        }
        public String AlertAlias
        {
            get { return _AlertAlias; }
            set
            {
                _AlertAlias = value;
                this.SetNotDefaultValue("AlertAlias");
            }
        }
        public String Relation
        {
            get { return _Relation; }
            set
            {
                _Relation = value;
                this.SetNotDefaultValue("Relation");
            }
        }
        public String Element
        {
            get { return _Element; }
            set
            {
                _Element = value;
                this.SetNotDefaultValue("Element");
            }
        }
        public String ColumnAlias
        {
            get { return _ColumnAlias; }
            set
            {
                _ColumnAlias = value;
                this.SetNotDefaultValue("ColumnAlias");
            }
        }
        public String Operator
        {
            get { return _Operator; }
            set
            {
                _Operator = value;
                this.SetNotDefaultValue("Operator");
            }
        }
        public String Value
        {
            get { return _Value; }
            set
            {
                _Value = value;
                this.SetNotDefaultValue("Value");
            }
        }
        public Int32? Sequence
        {
            get { return _Sequence; }
            set
            {
                _Sequence = value;
                this.SetNotDefaultValue("Sequence");
            }
        }
        public Guid? ParentID
        {
            get { return _ParentID; }
            set
            {
                _ParentID = value;
                this.SetNotDefaultValue("ParentID");
            }
        }
        public Int32? CondLevel
        {
            get { return _CondLevel; }
            set
            {
                _CondLevel = value;
                this.SetNotDefaultValue("CondLevel");
            }
        }
        public Boolean? IsActive
        {
            get { return _IsActive; }
            set
            {
                _IsActive = value;
                this.SetNotDefaultValue("IsActive");
            }
        }
        public String CreatedBy
        {
            get { return _CreatedBy; }
            set
            {
                _CreatedBy = value;
                this.SetNotDefaultValue("CreatedBy");
            }
        }
        public DateTime? CreatedOn
        {
            get { return _CreatedOn; }
            set
            {
                _CreatedOn = value;
                this.SetNotDefaultValue("CreatedOn");
            }
        }
        public String ModifiedBy
        {
            get { return _ModifiedBy; }
            set
            {
                _ModifiedBy = value;
                this.SetNotDefaultValue("ModifiedBy");
            }
        }
        public DateTime? ModifiedOn
        {
            get { return _ModifiedOn; }
            set
            {
                _ModifiedOn = value;
                this.SetNotDefaultValue("ModifiedOn");
            }
        }

        #endregion

    }
}
