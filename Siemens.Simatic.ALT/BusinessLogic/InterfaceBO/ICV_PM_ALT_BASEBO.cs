
using System;
using System.Collections.Generic;
using System.Text;

using Siemens.Simatic.Platform.Common;
using Siemens.Simatic.Platform.Core;
using Siemens.Simatic.ALT.Common;

namespace Siemens.Simatic.ALT.BusinessLogic
{
    [DefaultImplementationAttreibute(typeof(DefaultImpl.CV_PM_ALT_BASEBO))]
    public interface ICV_PM_ALT_BASEBO : Siemens.Simatic.Platform.Data.IDataList<CV_PM_ALT_BASE>
    {
        CV_PM_ALT_BASE GetEntity(Guid alertID);
        CV_PM_ALT_BASE GetEntity(string alertName);
        IList<CV_PM_ALT_BASE> GetEntities();
        IList<CV_PM_ALT_BASE> GetEntities(CV_PM_ALT_BASE entity);
    }
}
