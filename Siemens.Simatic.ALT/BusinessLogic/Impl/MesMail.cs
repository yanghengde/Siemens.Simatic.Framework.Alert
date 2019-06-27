using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Siemens.Simatic.ALT.BusinessLogic;
using Siemens.Simatic.Platform.Core;

namespace Siemens.Simatic.ALT
{
    public class MesMail
    {
        static IPM_ALT_EVENT_LOGBO _PM_ALT_EVENT_LOGBO = ObjectContainer.BuildUp<IPM_ALT_EVENT_LOGBO>();
        /// <summary>
        /// MES 发送邮件
        /// </summary>
        /// <param name="type">事件模块MesMailEnum</param>
        /// <param name="brief">邮件标题</param>
        /// <param name="content">邮件内容</param>
        public static void Send(string type, string brief, string content, string attachments, string optor)
        {
            try
            {
                _PM_ALT_EVENT_LOGBO.Send(type, brief, content, attachments, optor);
            }
            catch
            {

            }
        }
    }
}
