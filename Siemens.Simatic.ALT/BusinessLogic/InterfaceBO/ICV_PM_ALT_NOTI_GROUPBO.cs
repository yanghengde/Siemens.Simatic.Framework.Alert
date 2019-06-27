
using System;
using System.Collections.Generic;
using System.Text;

using Siemens.Simatic.Platform.Core;
using Siemens.Simatic.Platform.Common;
using Siemens.Simatic.ALT.Common;

namespace Siemens.Simatic.ALT.BusinessLogic
{
    [DefaultImplementationAttreibute(typeof(DefaultImpl.CV_PM_ALT_NOTI_GROUPBO))]
    public interface ICV_PM_ALT_NOTI_GROUPBO : Siemens.Simatic.Platform.Data.IDataList<CV_PM_ALT_NOTI_GROUP>
    {
        CV_PM_ALT_NOTI_GROUP GetEntity(Guid groupID);
        CV_PM_ALT_NOTI_GROUP GetEntity(string groupName);
        IList<CV_PM_ALT_NOTI_GROUP> GetEntities();

    }
}
