﻿
//=====================================================================
// This file was generated by Siemens.Simatic Platform
// 
// Siemens Copyright (c) 2014 All rights reserved. 
//=====================================================================

using System;
using System.Collections.Generic;
using System.Text;
using Siemens.Simatic.Platform.Core;
using Siemens.Simatic.ALT.Common.Persistence;

namespace Siemens.Simatic.ALT.Common
{
    /// <summary>
    /// CV_PM_EMAIL_NOTI Class.
    /// </summary>
    /// Title: CV_PM_EMAIL_NOTI
    /// Copyright: Siemens
    /// Version: 1.0
    [Serializable]
    public class CV_PM_EMAIL_NOTI : PersistentCV_PM_EMAIL_NOTI
    {
    }
}

namespace Siemens.Simatic.ALT.Common.Persistence
{
    /// <summary>
    /// CV_PM_EMAIL_NOTI PersistentClass.
    /// </summary>
    /// Title: PersistentCV_PM_EMAIL_NOTI
    /// Copyright: Siemens
    /// Version: 1.0
    [Serializable]
    public class PersistentCV_PM_EMAIL_NOTI : BaseEntity
    {
        public PersistentCV_PM_EMAIL_NOTI()
            : base()
        {
            this.InitialDefaultValues();
        }


        #region Private Fields

        private Guid? _ObjectGuid;
        private Guid? _AlertID;
        private String _UserID;
        private String _UserName;
        private String _Email;

        #endregion

        #region Public Properties

        public Guid? ObjectGuid
        {
            get { return _ObjectGuid; }
            set
            {
                _ObjectGuid = value;
                this.SetNotDefaultValue("ObjectGuid");
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
        public String UserID
        {
            get { return _UserID; }
            set
            {
                _UserID = value;
                this.SetNotDefaultValue("UserID");
            }
        }
        public String UserName
        {
            get { return _UserName; }
            set
            {
                _UserName = value;
                this.SetNotDefaultValue("UserName");
            }
        }
        public String Email
        {
            get { return _Email; }
            set
            {
                _Email = value;
                this.SetNotDefaultValue("Email");
            }
        }

        #endregion

    }
}
