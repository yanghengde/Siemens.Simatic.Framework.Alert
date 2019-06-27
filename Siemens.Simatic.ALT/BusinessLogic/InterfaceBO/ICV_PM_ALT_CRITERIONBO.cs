
using System;
using System.Collections.Generic;
using System.Text;

using Siemens.Simatic.Platform.Common;
using Siemens.Simatic.Platform.Core;
using Siemens.Simatic.ALT.Common;

namespace Siemens.Simatic.ALT.BusinessLogic
{
    [DefaultImplementationAttreibute(typeof(DefaultImpl.CV_PM_ALT_CRITERIONBO))]
    public interface ICV_PM_ALT_CRITERIONBO
    {
        CV_PM_ALT_CRITERION GetEntity(Guid criterionID);
        IList<CV_PM_ALT_CRITERION> GetEntitiesByAlert(Guid alertID);
        IList<CV_PM_ALT_CRITERION> GetActiveEntitiesByAlert(Guid alertID);
    }
}
