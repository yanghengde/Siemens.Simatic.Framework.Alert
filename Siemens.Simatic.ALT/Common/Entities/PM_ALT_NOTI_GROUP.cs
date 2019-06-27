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
    /// PM_ALT_NOTI_GROUP Class.
    /// </summary>
    /// Title: PM_ALT_NOTI_GROUP
    /// Copyright: LiXiao Info Tech Ltd.
    /// Version: 1.0
    [Serializable]
    public class PM_ALT_NOTI_GROUP : PersistentPM_ALT_NOTI_GROUP
    {
    }
}

namespace Siemens.Simatic.ALT.Common.Persistence
{
    /// <summary>
    /// PM_ALT_NOTI_GROUP PersistentClass.
    /// </summary>
    /// Title: PersistentPM_ALT_NOTI_GROUP
    /// Copyright: LiXiao Info Tech Ltd.
    /// Version: 1.0
    [Serializable]
    public class PersistentPM_ALT_NOTI_GROUP : BaseEntity
    {
        public PersistentPM_ALT_NOTI_GROUP()
            : base()
        {
            this.InitialDefaultValues();
        }


        #region Private Fields

        private Guid? _NotiGroupID;
        private String _GroupName;
        private String _GroupDesc;
        private Boolean? _NotiEmail;
        private Boolean? _NotiSMS;
        private Boolean? _RowDeleted;
        private String _CreatedBy;
        private DateTime? _CreatedOn;
        private String _ModifiedBy;
        private DateTime? _ModifiedOn;

        #endregion

        #region Public Properties

        public Guid? NotiGroupID
        {
            get { return _NotiGroupID; }
            set
            {
                _NotiGroupID = value;
                this.SetNotDefaultValue("NotiGroupID");
            }
        }
        public String GroupName
        {
            get { return _GroupName; }
            set
            {
                _GroupName = value;
                this.SetNotDefaultValue("GroupName");
            }
        }
        public String GroupDesc
        {
            get { return _GroupDesc; }
            set
            {
                _GroupDesc = value;
                this.SetNotDefaultValue("GroupDesc");
            }
        }
        public Boolean? NotiEmail
        {
            get { return _NotiEmail; }
            set
            {
                _NotiEmail = value;
                this.SetNotDefaultValue("NotiEmail");
            }
        }
        public Boolean? NotiSMS
        {
            get { return _NotiSMS; }
            set
            {
                _NotiSMS = value;
                this.SetNotDefaultValue("NotiSMS");
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
