using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Reflection;
using System.Globalization;

using Siemens.Simatic.Platform.Core;
using Siemens.Simatic.Platform.Core.ORMapping;
using Siemens.Simatic.Platform.Common;
using Siemens.Simatic.Platform.Common.ExceptionHandling;
using Siemens.Simatic.Platform.Data;
using Siemens.Simatic.Platform.Data.DataAccess;

using Siemens.Simatic.ALT.Common;
using Siemens.Simatic.ALT.Common.Persistence;

namespace Siemens.Simatic.ALT.DataAccess
{
    public partial interface IPM_ALT_EVENT_TYPEDAO : IDataAccessor<PM_ALT_EVENT_TYPE>
    {
    }
}

namespace Siemens.Simatic.ALT.DataAccess.DefaultImpl
{
    public partial class PM_ALT_EVENT_TYPEDAO : IPM_ALT_EVENT_TYPEDAO
    {
        
    }
}