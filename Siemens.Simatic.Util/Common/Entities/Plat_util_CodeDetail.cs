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
    /// Plat_util_CodeDetail Class.
    /// </summary>
    /// Title: Plat_util_CodeDetail
    /// Copyright: Siemens
    /// Version: 1.0
    [Serializable]
    public class Plat_util_CodeDetail : PersistentPlat_util_CodeDetail
    {
    }
}

namespace Siemens.Simatic.Util.Common.Persistence
{
    /// <summary>
    /// Plat_util_CodeDetail PersistentClass.
    /// </summary>
    /// Title: PersistentPlat_util_CodeDetail
    /// Copyright: Siemens
    /// Version: 1.0
    [Serializable]
    public class PersistentPlat_util_CodeDetail : BaseEntity
    {
        public PersistentPlat_util_CodeDetail()
            : base()
        {
            this.InitialDefaultValues();
        }


        #region Private Fields

        private Guid? _CodeDetailGuid;
        private String _CodeBaseCode;
        private String _CodeDetailCode;
        private String _CodeDetailName;
        private String _CodeDetailDesc;
        private String _CodeDetailValue;
        private Int32? _MySequence;
        private Boolean? _IsDefault;
        private Boolean? _IsDisplay;
        private Boolean? _IsActive;

        #endregion

        #region Public Properties

        public Guid? CodeDetailGuid
        {
            get { return _CodeDetailGuid; }
            set
            {
                _CodeDetailGuid = value;
                this.SetNotDefaultValue("CodeDetailGuid");
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
        public String CodeDetailCode
        {
            get { return _CodeDetailCode; }
            set
            {
                _CodeDetailCode = value;
                this.SetNotDefaultValue("CodeDetailCode");
            }
        }
        public String CodeDetailName
        {
            get { return _CodeDetailName; }
            set
            {
                _CodeDetailName = value;
                this.SetNotDefaultValue("CodeDetailName");
            }
        }
        public String CodeDetailDesc
        {
            get { return _CodeDetailDesc; }
            set
            {
                _CodeDetailDesc = value;
                this.SetNotDefaultValue("CodeDetailDesc");
            }
        }
        public String CodeDetailValue
        {
            get { return _CodeDetailValue; }
            set
            {
                _CodeDetailValue = value;
                this.SetNotDefaultValue("CodeDetailValue");
            }
        }
        public Int32? MySequence
        {
            get { return _MySequence; }
            set
            {
                _MySequence = value;
                this.SetNotDefaultValue("MySequence");
            }
        }
        public Boolean? IsDefault
        {
            get { return _IsDefault; }
            set
            {
                _IsDefault = value;
                this.SetNotDefaultValue("IsDefault");
            }
        }
        public Boolean? IsDisplay
        {
            get { return _IsDisplay; }
            set
            {
                _IsDisplay = value;
                this.SetNotDefaultValue("IsDisplay");
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

        #endregion

    }
}