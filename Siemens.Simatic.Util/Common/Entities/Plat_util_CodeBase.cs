﻿
//=====================================================================
// This file was generated by Siemens.Simatic Platform
// 
// Siemens Copyright (c) 2012 All rights reserved. 
//=====================================================================

using System;
using System.Collections.Generic;
using System.Text;
using Siemens.Simatic.Platform.Core;
using Siemens.Simatic.Util.Common.Persistence;

namespace Siemens.Simatic.Util.Common
{
    /// <summary>
    /// Plat_util_CodeBase Class.
    /// </summary>
    /// Title: Plat_util_CodeBase
    /// Copyright: Siemens
    /// Version: 1.0
    [Serializable]
    public class Plat_util_CodeBase : PersistentPlat_util_CodeBase
    {
    }
}

namespace Siemens.Simatic.Util.Common.Persistence
{
    /// <summary>
    /// Plat_util_CodeBase PersistentClass.
    /// </summary>
    /// Title: PersistentPlat_util_CodeBase
    /// Copyright: Siemens
    /// Version: 1.0
    [Serializable]
    public class PersistentPlat_util_CodeBase : BaseEntity
    {
        public PersistentPlat_util_CodeBase()
            : base()
        {
            this.InitialDefaultValues();
        }


        #region Private Fields

        private Guid? _CodeBaseGuid;
        private String _CodeBaseCode;
        private String _CodeBaseName;
        private String _CodeBaseType;
        private String _CodeBaseDesc;
        private Guid? _OnModuleGuid;
        private Guid? _ParentCodeBaseGuid;
        private Boolean? _IsActive;
        private Int32? _MyMark;

        #endregion

        #region Public Properties

        public Guid? CodeBaseGuid
        {
            get { return _CodeBaseGuid; }
            set
            {
                _CodeBaseGuid = value;
                this.SetNotDefaultValue("CodeBaseGuid");
            }
        }
        public String CodeBaseCode
        {
            get { return _CodeBaseCode; }
            set
            {
                _CodeBaseCode = value;
                this.SetNotDefaultValue("CodeBaseCode");
            }
        }
        public String CodeBaseName
        {
            get { return _CodeBaseName; }
            set
            {
                _CodeBaseName = value;
                this.SetNotDefaultValue("CodeBaseName");
            }
        }
        public String CodeBaseType
        {
            get { return _CodeBaseType; }
            set
            {
                _CodeBaseType = value;
                this.SetNotDefaultValue("CodeBaseType");
            }
        }
        public String CodeBaseDesc
        {
            get { return _CodeBaseDesc; }
            set
            {
                _CodeBaseDesc = value;
                this.SetNotDefaultValue("CodeBaseDesc");
            }
        }
        public Guid? OnModuleGuid
        {
            get { return _OnModuleGuid; }
            set
            {
                _OnModuleGuid = value;
                this.SetNotDefaultValue("OnModuleGuid");
            }
        }
        public Guid? ParentCodeBaseGuid
        {
            get { return _ParentCodeBaseGuid; }
            set
            {
                _ParentCodeBaseGuid = value;
                this.SetNotDefaultValue("ParentCodeBaseGuid");
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
        public Int32? MyMark
        {
            get { return _MyMark; }
            set
            {
                _MyMark = value;
                this.SetNotDefaultValue("MyMark");
            }
        }

        #endregion

    }
}