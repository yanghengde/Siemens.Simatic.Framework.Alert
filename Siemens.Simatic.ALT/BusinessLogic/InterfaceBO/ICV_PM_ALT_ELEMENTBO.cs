
using System;
using System.Collections.Generic;
using System.Text;

using Siemens.Simatic.Platform.Common;
using Siemens.Simatic.Platform.Core;
using Siemens.Simatic.ALT.Common;

namespace Siemens.Simatic.ALT.BusinessLogic
{
    [DefaultImplementationAttreibute(typeof(DefaultImpl.CV_PM_ALT_ELEMENTBO))]
    public interface ICV_PM_ALT_ELEMENTBO : Siemens.Simatic.Platform.Data.IDataList<CV_PM_ALT_ELEMENT>
    {
        CV_PM_ALT_ELEMENT GetEntity(Guid elementID);
        CV_PM_ALT_ELEMENT GetEntity(Guid alertID, string column);
        IList<CV_PM_ALT_ELEMENT> GetEntitiesByAlert(Guid alertID);
        IList<CV_PM_ALT_ELEMENT> GetActiveEntitiesByAlert(Guid alertID);
    }
}
