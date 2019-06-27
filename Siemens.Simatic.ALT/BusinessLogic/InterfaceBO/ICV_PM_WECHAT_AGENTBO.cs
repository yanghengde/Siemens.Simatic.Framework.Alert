
using System;
using System.Collections.Generic;
using System.Text;

using Siemens.Simatic.Platform.Common;
using Siemens.Simatic.Platform.Core;
using Siemens.Simatic.ALT.Common;

namespace Siemens.Simatic.ALT.BusinessLogic
{
    [DefaultImplementationAttreibute(typeof(DefaultImpl.CV_PM_WECHAT_AGENTBO))]
    public interface ICV_PM_WECHAT_AGENTBO
    {
        IList<CV_PM_WECHAT_AGENT> GetEntities();
    }
}
