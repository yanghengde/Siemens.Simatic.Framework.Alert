using Siemens.Simatic.Platform.Core;
using Siemens.Simatic.Platform.Common;
using Siemens.Simatic.Platform.Common.ExceptionHandling;
using Siemens.Simatic.Platform.Data;

using Newtonsoft.Json;
using Siemens.Simatic.Platform.Core;
using Siemens.Simatic.Wechat.Common;
using Siemens.Simatic.Wechat.Enterprise;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Siemens.Simatic.ALT.BusinessLogic
{
    [DefaultImplementationAttreibute(typeof(DefaultImpl.API_WECHAT_AUX))]
    public partial interface IAPI_WECHAT_AUX
    {
        /// <summary>
        /// 发送文本
        /// </summary>
        /// <param name="url"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        string SendText(string url, string message);
        

    }
}

namespace Siemens.Simatic.ALT.BusinessLogic.DefaultImpl
{
    public partial class API_WECHAT_AUX : IAPI_WECHAT_AUX
    {
        //private WeChatEnterprise enterprise;
        //private const string selectTokenSql = "select Access_token from PM_WECHAT_TOKEN where DATEDIFF(MINUTE,Date,GETDATE()) <= 100";
        //private const string updateSql = "update PM_WECHAT_TOKEN set Access_token = '{0}' , Date = GETDATE() where PK = 1 ";

        public API_WECHAT_AUX()
        {
            //enterprise = new WeChatEnterprise();
        }

        /// <summary>
        /// 发送文本
        /// </summary>
        /// <param name="url"></param>
        /// <param name="message"></param>
        /// <returns>成功返回1</returns>
        //public string SendText(string url, string message)
        //{
        //    Http_Request httpRequest = new Http_Request();
        //    ReturnValue rv = new ReturnValue();
            
        //    string srtRet = httpRequest.HttpPost(url, message);
        //    return srtRet;
        //}


     
    }
}
