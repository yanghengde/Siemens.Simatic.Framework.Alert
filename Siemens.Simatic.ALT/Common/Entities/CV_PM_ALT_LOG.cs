﻿
//=====================================================================
// This file was generated by LxIT.Suite Platform
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
    /// CV_PM_ALT_LOG Class.
    /// </summary>
    /// Title: CV_PM_ALT_LOG
    /// Copyright: LiXiao Info Tech Ltd.
    /// Version: 1.0
    [Serializable]
    public class CV_PM_ALT_LOG : PersistentCV_PM_ALT_LOG
    {
    }
}

namespace Siemens.Simatic.ALT.Common.Persistence
{
    /// <summary>
    /// CV_PM_ALT_LOG PersistentClass.
    /// </summary>
    /// Title: PersistentCV_PM_ALT_LOG
    /// Copyright: LiXiao Info Tech Ltd.
    /// Version: 1.0
    [Serializable]
    public class PersistentCV_PM_ALT_LOG : BaseEntity
    {
        public PersistentCV_PM_ALT_LOG()
            : base()
        {
            this.InitialDefaultValues();
        }


        #region Private Fields

        private Int64? _LogPK;
        private Guid? _AlertID;
        private String _AlertName;
        private String _AlertAlias;
        private String _LogTitle;
        private String _LogContent;
        private Int32? _NotifiedCnt;
        private Boolean? _IsClosed;
        private String _RespondedBy;
        private DateTime? _RespondedOn;
        private String _ResponseAction;
        private String _ResponseActionName;
        private String _ResponseCause;
        private String _ResponseNotes;
        private String _AuditedBy;
        private DateTime? _AuditedOn;
        private String _CreatedBy;
        private DateTime? _CreatedOn;
        private String _NotifiedBy;
        private DateTime? _NotifiedOn;

        #endregion

        #region Public Properties

        public Int64? LogPK
        {
            get { return _LogPK; }
            set
            {
                _LogPK = value;
                this.SetNotDefaultValue("LogPK");
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
        public String LogTitle
        {
            get { return _LogTitle; }
            set
            {
                _LogTitle = value;
                this.SetNotDefaultValue("LogTitle");
            }
        }
        public String LogContent
        {
            get { return _LogContent; }
            set
            {
                _LogContent = value;
                this.SetNotDefaultValue("LogContent");
            }
        }
        public Int32? NotifiedCnt
        {
            get { return _NotifiedCnt; }
            set
            {
                _NotifiedCnt = value;
                this.SetNotDefaultValue("NotifiedCnt");
            }
        }
        public Boolean? IsClosed
        {
            get { return _IsClosed; }
            set
            {
                _IsClosed = value;
                this.SetNotDefaultValue("IsClosed");
            }
        }
        public String RespondedBy
        {
            get { return _RespondedBy; }
            set
            {
                _RespondedBy = value;
                this.SetNotDefaultValue("RespondedBy");
            }
        }
        public DateTime? RespondedOn
        {
            get { return _RespondedOn; }
            set
            {
                _RespondedOn = value;
                this.SetNotDefaultValue("RespondedOn");
            }
        }
        public String ResponseAction
        {
            get { return _ResponseAction; }
            set
            {
                _ResponseAction = value;
                this.SetNotDefaultValue("ResponseAction");
            }
        }
        public String ResponseActionName
        {
            get { return _ResponseActionName; }
            set
            {
                _ResponseActionName = value;
                this.SetNotDefaultValue("ResponseActionName");
            }
        }
        public String ResponseCause
        {
            get { return _ResponseCause; }
            set
            {
                _ResponseCause = value;
                this.SetNotDefaultValue("ResponseCause");
            }
        }
        public String ResponseNotes
        {
            get { return _ResponseNotes; }
            set
            {
                _ResponseNotes = value;
                this.SetNotDefaultValue("ResponseNotes");
            }
        }
        public String AuditedBy
        {
            get { return _AuditedBy; }
            set
            {
                _AuditedBy = value;
                this.SetNotDefaultValue("AuditedBy");
            }
        }
        public DateTime? AuditedOn
        {
            get { return _AuditedOn; }
            set
            {
                _AuditedOn = value;
                this.SetNotDefaultValue("AuditedOn");
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
        public String NotifiedBy
        {
            get { return _NotifiedBy; }
            set
            {
                _NotifiedBy = value;
                this.SetNotDefaultValue("NotifiedBy");
            }
        }
        public DateTime? NotifiedOn
        {
            get { return _NotifiedOn; }
            set
            {
                _NotifiedOn = value;
                this.SetNotDefaultValue("NotifiedOn");
            }
        }

        #endregion

    }
}
