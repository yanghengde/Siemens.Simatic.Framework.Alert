
using System;
using System.Collections.Generic;
using System.Text;

using Siemens.Simatic.Platform.Common;
using Siemens.Simatic.Platform.Core;
using Siemens.Simatic.ALT.Common;

namespace Siemens.Simatic.ALT.BusinessLogic
{
    [DefaultImplementationAttreibute(typeof(DefaultImpl.CV_PM_ALT_EVENT_TYPEBO))]
    public interface ICV_PM_ALT_EVENT_TYPEBO
    {
        CV_PM_ALT_EVENT_TYPE GetEntity(Guid typeID);
        CV_PM_ALT_EVENT_TYPE GetEntity(string typeName);
        IList<CV_PM_ALT_EVENT_TYPE> GetEntities();
    }
}
