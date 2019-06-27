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
    /// CV_PM_ALT_ELEMENT Class.
    /// </summary>
    /// Title: CV_PM_ALT_ELEMENT
    /// Copyright: LiXiao Info Tech Ltd.
    /// Version: 1.0
    [Serializable]
    public class CV_PM_ALT_ELEMENT : PersistentCV_PM_ALT_ELEMENT
    {
    }
}

namespace Siemens.Simatic.ALT.Common.Persistence
{
    /// <summary>
    /// CV_PM_ALT_ELEMENT PersistentClass.
    /// </summary>
    /// Title: PersistentCV_PM_ALT_ELEMENT
    /// Copyright: LiXiao Info Tech Ltd.
    /// Version: 1.0
    [Serializable]
    public class PersistentCV_PM_ALT_ELEMENT : BaseEntity
    {
        public PersistentCV_PM_ALT_ELEMENT()
            : base()
        {
            this.InitialDefaultValues();
        }


        #region Private Fields

        private Guid? _ElementID;
        private Guid? _AlertID;
        private String _AlertName;
        private String _AlertAlias;
        private String _ElementField;
        private String _ElementAlias;
        private String _ElementType;
        private Int32? _Sequence;
        private Boolean? _IsActive;
        private String _CreatedBy;
        private DateTime? _CreatedOn;
        private String _ModifiedBy;
        private DateTime? _ModifiedOn;

        #endregion

        #region Public Properties

        public Guid? ElementID
        {
            get { return _ElementID; }
            set
            {
                _ElementID = value;
                this.SetNotDefaultValue("ElementID");
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
        public String ElementField
        {
            get { return _ElementField; }
            set
            {
                _ElementField = value;
                this.SetNotDefaultValue("ElementField");
            }
        }
        public String ElementAlias
        {
            get { return _ElementAlias; }
            set
            {
                _ElementAlias = value;
                this.SetNotDefaultValue("ElementAlias");
            }
        }
        public String ElementType
        {
            get { return _ElementType; }
            set
            {
                _ElementType = value;
                this.SetNotDefaultValue("ElementType");
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
