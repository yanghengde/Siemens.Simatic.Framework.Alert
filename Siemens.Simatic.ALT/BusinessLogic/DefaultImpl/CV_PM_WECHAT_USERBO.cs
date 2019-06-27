
using System;
using System.Collections.Generic;
using System.Text;

using Siemens.Simatic.Platform.Core;
using Siemens.Simatic.Platform.Common;
using Siemens.Simatic.Platform.Common.ExceptionHandling;
using Siemens.Simatic.Platform.Data;

using Siemens.Simatic.ALT.Common;
using Siemens.Simatic.ALT.DataAccess;

namespace Siemens.Simatic.ALT.BusinessLogic.DefaultImpl
{
    public class CV_PM_WECHAT_USERBO : ICV_PM_WECHAT_USERBO
    {
        private ICV_PM_WECHAT_USERDAO cV_PM_WECHAT_USERDAO;

        public CV_PM_WECHAT_USERBO()
        {
            cV_PM_WECHAT_USERDAO = ObjectContainer.BuildUp<ICV_PM_WECHAT_USERDAO>();
        }

    }
}
