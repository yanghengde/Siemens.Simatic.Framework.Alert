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
    /// PM_WECHAT_DEPARTMENT_TEMP Class.
    /// </summary>
    /// Title: PM_WECHAT_DEPARTMENT_TEMP
    /// Copyright: Siemens
    /// Version: 1.0
    [Serializable]
    public class PM_WECHAT_DEPARTMENT_TEMP : PersistentPM_WECHAT_DEPARTMENT_TEMP
    {
    }
}

namespace Siemens.Simatic.ALT.Common.Persistence
{
    /// <summary>
    /// PM_WECHAT_DEPARTMENT_TEMP PersistentClass.
    /// </summary>
    /// Title: PersistentPM_WECHAT_DEPARTMENT_TEMP
    /// Copyright: Siemens
    /// Version: 1.0
    [Serializable]
    public class PersistentPM_WECHAT_DEPARTMENT_TEMP : BaseEntity
    {
        public PersistentPM_WECHAT_DEPARTMENT_TEMP() : base()
        {
            this.InitialDefaultValues();
        }


        #region Private Fields

        private String _ID;
        private String _Name;
        private String _ParentID;
        private String _Order;

        #endregion

        #region Public Properties

        public String ID
        {
            get { return _ID; }
            set
            {
                _ID = value;
                this.SetNotDefaultValue("ID");
            }
        }
        public String Name
        {
            get { return _Name; }
            set
            {
                _Name = value;
                this.SetNotDefaultValue("Name");
            }
        }
        public String ParentID
        {
            get { return _ParentID; }
            set
            {
                _ParentID = value;
                this.SetNotDefaultValue("ParentID");
            }
        }
        public String Order
        {
            get { return _Order; }
            set
            {
                _Order = value;
                this.SetNotDefaultValue("Order");
            }
        }

        #endregion

    }
}