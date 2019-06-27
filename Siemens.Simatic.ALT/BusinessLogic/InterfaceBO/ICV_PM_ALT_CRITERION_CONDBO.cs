
using System;
using System.Collections.Generic;
using System.Text;

using Siemens.Simatic.Platform.Common;
using Siemens.Simatic.Platform.Core;
using Siemens.Simatic.ALT.Common;

namespace Siemens.Simatic.ALT.BusinessLogic
{
    [DefaultImplementationAttreibute(typeof(DefaultImpl.CV_PM_ALT_CRITERION_CONDBO))]
    public interface ICV_PM_ALT_CRITERION_CONDBO
    {
        CV_PM_ALT_CRITERION_COND GetEntity(Guid conditionID);
        IList<CV_PM_ALT_CRITERION_COND> GetEntitiesByAlert(Guid alertID);
        IList<CV_PM_ALT_CRITERION_COND> GetActiveEntitiesByAlert(Guid alertID);
    }
}
