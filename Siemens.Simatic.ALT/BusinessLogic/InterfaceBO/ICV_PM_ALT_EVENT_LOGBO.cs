
using System;
using System.Collections.Generic;
using System.Text;

using Siemens.Simatic.Platform.Common;
using Siemens.Simatic.Platform.Core;
using Siemens.Simatic.ALT.Common;

namespace Siemens.Simatic.ALT.BusinessLogic
{
    [DefaultImplementationAttreibute(typeof(DefaultImpl.CV_PM_ALT_EVENT_LOGBO))]
    public interface ICV_PM_ALT_EVENT_LOGBO
    {
        IList<CV_PM_ALT_EVENT_LOG> GetEntities(CV_PM_ALT_EVENT_LOGQueryParam qp);
    }
}
