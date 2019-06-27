
using System;
using System.Collections.Generic;
using System.Text;

using Siemens.Simatic.Platform.Common;
using Siemens.Simatic.Platform.Core;
using Siemens.Simatic.ALT.Common;

namespace Siemens.Simatic.ALT.BusinessLogic
{
    [DefaultImplementationAttreibute(typeof(DefaultImpl.CV_PM_ALT_CRITERION_AGGRBO))]
    public interface ICV_PM_ALT_CRITERION_AGGRBO
    {
        CV_PM_ALT_CRITERION_AGGR GetEntity(Guid aggregationID);
        IList<CV_PM_ALT_CRITERION_AGGR> GetEntitiesByAlert(Guid alertID);
        IList<CV_PM_ALT_CRITERION_AGGR> GetActiveEntitiesByAlert(Guid alertID);
    }
}
