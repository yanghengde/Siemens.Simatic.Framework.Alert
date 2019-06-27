
using System;
using System.Collections.Generic;
using System.Text;

using Siemens.Simatic.Platform.Common;
using Siemens.Simatic.Platform.Core;
using Siemens.Simatic.ALT.Common;

namespace Siemens.Simatic.ALT.BusinessLogic
{
    [DefaultImplementationAttreibute(typeof(DefaultImpl.CV_PM_EMAIL_NOTIBO))]
    public interface ICV_PM_EMAIL_NOTIBO
    {
        /// <summary>
        /// 根据预警ID获取需要接受邮件人员集合
        /// </summary>
        /// <param name="alertName"></param>
        /// <returns></returns>
        IList<CV_PM_EMAIL_NOTI> GetEntity(string alertName);
    }
}
