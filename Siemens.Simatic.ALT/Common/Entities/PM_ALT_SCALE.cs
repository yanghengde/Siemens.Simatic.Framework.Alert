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
    /// PM_ALT_SCALE Class.
    /// </summary>
    /// Title: PM_ALT_SCALE
    /// Copyright: LiXiao Info Tech Ltd.
    /// Version: 1.0
    [Serializable]
    public class PM_ALT_SCALE : PersistentPM_ALT_SCALE
    {
    }
}

namespace Siemens.Simatic.ALT.Common.Persistence
{
    /// <summary>
    /// PM_ALT_SCALE PersistentClass.
    /// </summary>
    /// Title: PersistentPM_ALT_SCALE
    /// Copyright: LiXiao Info Tech Ltd.
    /// Version: 1.0
    [Serializable]
    public class PersistentPM_ALT_SCALE : BaseEntity
    {
        public PersistentPM_ALT_SCALE()
            : base()
        {
            this.InitialDefaultValues();
        }


        #region Private Fields

        private Int64? _ScalePK;
        private Guid? _CriterionID;
        private String _Scales;
        private String _ScaledValue;
        private Boolean? _RowDeleted;
        private String _CreatedBy;
        private DateTime? _CreatedOn;
        private String _ModifiedBy;
        private DateTime? _ModifiedOn;

        #endregion

        #region Public Properties

        public Int64? ScalePK
        {
            get { return _ScalePK; }
            set
            {
                _ScalePK = value;
                this.SetNotDefaultValue("ScalePK");
            }
        }
        public Guid? CriterionID
        {
            get { return _CriterionID; }
            set
            {
                _CriterionID = value;
                this.SetNotDefaultValue("CriterionID");
            }
        }
        public String Scales
        {
            get { return _Scales; }
            set
            {
                _Scales = value;
                this.SetNotDefaultValue("Scales");
            }
        }
        public String ScaledValue
        {
            get { return _ScaledValue; }
            set
            {
                _ScaledValue = value;
                this.SetNotDefaultValue("ScaledValue");
            }
        }
        public Boolean? RowDeleted
        {
            get { return _RowDeleted; }
            set
            {
                _RowDeleted = value;
                this.SetNotDefaultValue("RowDeleted");
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