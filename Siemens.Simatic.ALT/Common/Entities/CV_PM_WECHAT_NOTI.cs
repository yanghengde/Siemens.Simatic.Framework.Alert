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
    /// CV_PM_WECHAT_NOTI Class.
    /// </summary>
    /// Title: CV_PM_WECHAT_NOTI
    /// Copyright: Siemens
    /// Version: 1.0
    [Serializable]
    public class CV_PM_WECHAT_NOTI : PersistentCV_PM_WECHAT_NOTI
    {
    }
}

namespace Siemens.Simatic.ALT.Common.Persistence
{
    /// <summary>
    /// CV_PM_WECHAT_NOTI PersistentClass.
    /// </summary>
    /// Title: PersistentCV_PM_WECHAT_NOTI
    /// Copyright: Siemens
    /// Version: 1.0
    [Serializable]
    public class PersistentCV_PM_WECHAT_NOTI : BaseEntity
    {
        public PersistentCV_PM_WECHAT_NOTI()
            : base()
        {
            this.InitialDefaultValues();
        }


        #region Private Fields

        private Guid? _NotiGuid;
        private Guid? _AlertID;
        private Guid? _AgentGuid;
        private Int32? _AgentID;
        private String _SecretID;
        private String _UserIDs;

        #endregion

        #region Public Properties

        public Guid? NotiGuid
        {
            get { return _NotiGuid; }
            set
            {
                _NotiGuid = value;
                this.SetNotDefaultValue("NotiGuid");
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
        public Guid? AgentGuid
        {
            get { return _AgentGuid; }
            set
            {
                _AgentGuid = value;
                this.SetNotDefaultValue("AgentGuid");
            }
        }
        public Int32? AgentID
        {
            get { return _AgentID; }
            set
            {
                _AgentID = value;
                this.SetNotDefaultValue("AgentID");
            }
        }
        public String SecretID
        {
            get { return _SecretID; }
            set
            {
                _SecretID = value;
                this.SetNotDefaultValue("SecretID");
            }
        }
        public String UserIDs
        {
            get { return _UserIDs; }
            set
            {
                _UserIDs = value;
                this.SetNotDefaultValue("UserIDs");
            }
        }

        #endregion

    }
}
