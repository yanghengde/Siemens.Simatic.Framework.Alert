
using System;
using System.Collections.Generic;
using System.Text;

using Siemens.Simatic.Platform.Common;
using Siemens.Simatic.Platform.Core;
using Siemens.Simatic.ALT.Common;

namespace Siemens.Simatic.ALT.BusinessLogic
{
    [DefaultImplementationAttreibute(typeof(DefaultImpl.CV_PM_ALT_LOGBO))]
    public interface ICV_PM_ALT_LOGBO
    {
        CV_PM_ALT_LOG GetEntity(long logPK);
        IList<CV_PM_ALT_LOG> GetEntities(CV_PM_ALT_LOGQueryParam qp);
    }
}
