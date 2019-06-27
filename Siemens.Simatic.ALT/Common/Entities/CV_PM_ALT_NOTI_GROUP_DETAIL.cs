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
    /// CV_PM_ALT_NOTI_GROUP_DETAIL Class.
    /// </summary>
    /// Title: CV_PM_ALT_NOTI_GROUP_DETAIL
    /// Copyright: LiXiao Info Tech Ltd.
    /// Version: 1.0
    [Serializable]
    public class CV_PM_ALT_NOTI_GROUP_DETAIL : PersistentCV_PM_ALT_NOTI_GROUP_DETAIL
    {
    }
}

namespace Siemens.Simatic.ALT.Common.Persistence
{
    /// <summary>
    /// CV_PM_ALT_NOTI_GROUP_DETAIL PersistentClass.
    /// </summary>
    /// Title: PersistentCV_PM_ALT_NOTI_GROUP_DETAIL
    /// Copyright: LiXiao Info Tech Ltd.
    /// Version: 1.0
    [Serializable]
    public class PersistentCV_PM_ALT_NOTI_GROUP_DETAIL : BaseEntity
    {
        public PersistentCV_PM_ALT_NOTI_GROUP_DETAIL()
            : base()
        {
            this.InitialDefaultValues();
        }


        #region Private Fields

        private Guid? _NotiGroupDetailID;
        private Guid? _NotiGroupID;
        private String _NotiGroupName;
        private Guid? _SubGroupID;
        private String _MemberID;
        private String _EmployeeName;
        private String _EmailAddress;
        private String _MobilePhone;
        private Boolean? _NotiEmail;
        private Boolean? _NotiSMS;

        #endregion

        #region Public Properties

        public Guid? NotiGroupDetailID
        {
            get { return _NotiGroupDetailID; }
            set
            {
                _NotiGroupDetailID = value;
                this.SetNotDefaultValue("NotiGroupDetailID");
            }
        }
        public Guid? NotiGroupID
        {
            get { return _NotiGroupID; }
            set
            {
                _NotiGroupID = value;
                this.SetNotDefaultValue("NotiGroupID");
            }
        }
        public String NotiGroupName
        {
            get { return _NotiGroupName; }
            set
            {
                _NotiGroupName = value;
                this.SetNotDefaultValue("NotiGroupName");
            }
        }
        public Guid? SubGroupID
        {
            get { return _SubGroupID; }
            set
            {
                _SubGroupID = value;
                this.SetNotDefaultValue("SubGroupID");
            }
        }
        public String MemberID
        {
            get { return _MemberID; }
            set
            {
                _MemberID = value;
                this.SetNotDefaultValue("MemberID");
            }
        }
        public String EmployeeName
        {
            get { return _EmployeeName; }
            set
            {
                _EmployeeName = value;
                this.SetNotDefaultValue("EmployeeName");
            }
        }
        public String EmailAddress
        {
            get { return _EmailAddress; }
            set
            {
                _EmailAddress = value;
                this.SetNotDefaultValue("EmailAddress");
            }
        }
        public String MobilePhone
        {
            get { return _MobilePhone; }
            set
            {
                _MobilePhone = value;
                this.SetNotDefaultValue("MobilePhone");
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

        #endregion

    }
}
