
using System;
using System.Collections.Generic;
using System.Text;

using Siemens.Simatic.Platform.Core;
using Siemens.Simatic.Platform.Common;
using Siemens.Simatic.ALT.Common;

namespace Siemens.Simatic.ALT.BusinessLogic
{
    [DefaultImplementationAttreibute(typeof(DefaultImpl.CV_PM_ALT_NOTI_GROUP_DETAILBO))]
    public interface ICV_PM_ALT_NOTI_GROUP_DETAILBO
    {
        CV_PM_ALT_NOTI_GROUP_DETAIL GetEntity(Guid groupDtlID);
        IList<CV_PM_ALT_NOTI_GROUP_DETAIL> GetEntitiesByGroup(Guid groupID);
    }
}
