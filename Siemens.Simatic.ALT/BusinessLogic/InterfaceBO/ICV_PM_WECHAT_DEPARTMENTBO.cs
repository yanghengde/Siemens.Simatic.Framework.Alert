
using System;
using System.Collections.Generic;
using System.Text;

using Siemens.Simatic.Platform.Common;
using Siemens.Simatic.Platform.Core;
using Siemens.Simatic.ALT.Common;

namespace Siemens.Simatic.ALT.BusinessLogic
{
    [DefaultImplementationAttreibute(typeof(DefaultImpl.CV_PM_WECHAT_DEPARTMENTBO))]
    public interface ICV_PM_WECHAT_DEPARTMENTBO
    {
        IList<CV_PM_WECHAT_DEPARTMENT> GetEntities();

        IList<CV_DEPARTMENT_ROOT_QuaryParam> GetTree(IList<CV_PM_WECHAT_DEPARTMENT> nodelist, Guid alertID);
        IList<CV_PM_WECHAT_DEPARTMENT> GetSameLevelDepartment(int parentID);
    }
}
