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
    /// PM_ALT_EVENT_LOG Class.
    /// </summary>
    /// Title: PM_ALT_EVENT_LOG
    /// Copyright: Siemens
    /// Version: 1.0
    [Serializable]
    public class PM_ALT_EVENT_LOG : PersistentPM_ALT_EVENT_LOG
    {
    }
}

namespace Siemens.Simatic.ALT.Common.Persistence
{
    /// <summary>
    /// PM_ALT_EVENT_LOG PersistentClass.
    /// </summary>
    /// Title: PersistentPM_ALT_EVENT_LOG
    /// Copyright: Siemens
    /// Version: 1.0
    [Serializable]
    public class PersistentPM_ALT_EVENT_LOG : BaseEntity
    {
        public PersistentPM_ALT_EVENT_LOG()
            : base()
        {
            this.InitialDefaultValues();
        }


        #region Private Fields

        private Guid? _EventLogID;
        private Guid? _EventTypeID;
        private String _EventBrief;
        private String _EventContent;
        private String _Attachments;
        private Int32? _NotifiedCnt;
        private Boolean? _IsFinished;
        private String _CreatedBy;
        private DateTime? _CreatedOn;
        private String _NotifiedBy;
        private DateTime? _NotifiedOn;

        #endregion

        #region Public Properties

        public Guid? EventLogID
        {
            get { return _EventLogID; }
            set
            {
                _EventLogID = value;
                this.SetNotDefaultValue("EventLogID");
            }
        }
        public Guid? EventTypeID
        {
            get { return _EventTypeID; }
            set
            {
                _EventTypeID = value;
                this.SetNotDefaultValue("EventTypeID");
            }
        }
        public String EventBrief
        {
            get { return _EventBrief; }
            set
            {
                _EventBrief = value;
                this.SetNotDefaultValue("EventBrief");
            }
        }
        public String EventContent
        {
            get { return _EventContent; }
            set
            {
                _EventContent = value;
                this.SetNotDefaultValue("EventContent");
            }
        }
        public String Attachments
        {
            get { return _Attachments; }
            set
            {
                _Attachments = value;
                this.SetNotDefaultValue("Attachments");
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
        public Boolean? IsFinished
        {
            get { return _IsFinished; }
            set
            {
                _IsFinished = value;
                this.SetNotDefaultValue("IsFinished");
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
