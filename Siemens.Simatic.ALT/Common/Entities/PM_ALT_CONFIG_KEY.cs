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
    /// PM_ALT_CONFIG_KEY Class.
    /// </summary>
    /// Title: PM_ALT_CONFIG_KEY
    /// Copyright: Siemens
    /// Version: 1.0
    [Serializable]
    public class PM_ALT_CONFIG_KEY : PersistentPM_ALT_CONFIG_KEY
    {
    }
}

namespace Siemens.Simatic.ALT.Common.Persistence
{
    /// <summary>
    /// PM_ALT_CONFIG_KEY PersistentClass.
    /// </summary>
    /// Title: PersistentPM_ALT_CONFIG_KEY
    /// Copyright: Siemens
    /// Version: 1.0
    [Serializable]
    public class PersistentPM_ALT_CONFIG_KEY : BaseEntity
    {
        public PersistentPM_ALT_CONFIG_KEY()
            : base()
        {
            this.InitialDefaultValues();
        }


        #region Private Fields

        private Int32? _ID;
        private String _sOwner;
        private String _sKey;
        private String _sValue;
        private String _sDescription;

        #endregion

        #region Public Properties

        public Int32? ID
        {
            get { return _ID; }
            set
            {
                _ID = value;
                this.SetNotDefaultValue("ID");
            }
        }
        public String sOwner
        {
            get { return _sOwner; }
            set
            {
                _sOwner = value;
                this.SetNotDefaultValue("sOwner");
            }
        }
        public String sKey
        {
            get { return _sKey; }
            set
            {
                _sKey = value;
                this.SetNotDefaultValue("sKey");
            }
        }
        public String sValue
        {
            get { return _sValue; }
            set
            {
                _sValue = value;
                this.SetNotDefaultValue("sValue");
            }
        }
        public String sDescription
        {
            get { return _sDescription; }
            set
            {
                _sDescription = value;
                this.SetNotDefaultValue("sDescription");
            }
        }

        #endregion

    }
}