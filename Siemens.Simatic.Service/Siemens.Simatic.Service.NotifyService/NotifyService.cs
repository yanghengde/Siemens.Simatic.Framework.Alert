using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Linq;

using Siemens.Simatic.Platform.Core;
using Siemens.Simatic.Platform.Common;
using System.Collections;
using Siemens.Simatic.Wechat.Enums;
using Siemens.Simatic.Wechat.BusinessLogic;
using Siemens.Simatic.Wechat.BusinessLogic.DefaultImpl;
using Siemens.Simatic.Wechat.Common;
using System.Drawing;
using Siemens.Simatic.ALT.BusinessLogic;
using Siemens.Simatic.ALT.Common;

namespace Siemens.Simatic.Service.NotifyService
{
    public class NotifyService : MainService
    {
        private string SenderName;//from
        private string SenderUser; //add by wesley@20130508
        private string SenderPassword;
        private string SmtpServer;
        private string SmtpPort;

        private string corpid = ConfigurationManager.AppSettings["corpid"];
        private string corpsecret = ConfigurationManager.AppSettings["corpsecret"];

        private IPM_ALT_MESSAGEBO _PM_ALT_MESSAGEBO;

        private Siemens.Simatic.Wechat.BusinessLogic.ICV_PM_WECHAT_NOTIBO _CV_PM_WECHAT_NOTIBO;
        private IPM_WECHAT_NOTIBO _PM_WECHAT_NOTIBO;

        public NotifyService(MesLog mesLog)
            : this("NotifyTimer", mesLog)
        {
        }

        public NotifyService(string name, MesLog mesLog)
            : base(name, mesLog)
        {
            try
            {
                //
                _PM_ALT_MESSAGEBO = ObjectContainer.BuildUp<IPM_ALT_MESSAGEBO>();

                _CV_PM_WECHAT_NOTIBO = ObjectContainer.BuildUp<Siemens.Simatic.Wechat.BusinessLogic.ICV_PM_WECHAT_NOTIBO>();
                _PM_WECHAT_NOTIBO = ObjectContainer.BuildUp<IPM_WECHAT_NOTIBO>();
            }
            catch (Exception e)
            {
                // need EmailService instance to send email only.
            }

            this.SenderUser = ConfigurationManager.AppSettings["SenderUser"];
            this.SenderPassword = ConfigurationManager.AppSettings["SenderPwd"];
            this.SenderName = ConfigurationManager.AppSettings["SenderAccount"];
            this.SmtpServer = ConfigurationManager.AppSettings["SmtpServer"];
            this.SmtpPort = ConfigurationManager.AppSettings["SmtpPort"];
        }

        protected override object GetReadyData()
        {
            try
            {
                return new object();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        protected override bool Execute(object readyData)
        {
            try
            {
                if (readyData == null) return false;

                string returnMessage = string.Empty;
                bool isSent = false;

                #region PM_ALT_MESSAGE
                IList<PM_ALT_MESSAGE> messages = _PM_ALT_MESSAGEBO.GetEntitiesToSend();
                //
                foreach (PM_ALT_MESSAGE msg in messages)
                {
                    if (msg.MsgType == 1)//邮件发送
                    {
                        isSent = false;
                        //
                        string to = msg.MsgTo;
                        string cc = msg.MsgCc;
                        string bcc = msg.MsgBcc;
                        string subject = msg.MsgSubject;
                        string body = msg.MsgContent;
                        body = System.Web.HttpUtility.HtmlDecode(body);
                        //
                        try
                        {
                            isSent = NotifyHelper.SendNetMail(to, cc, bcc, subject, body, msg.Attachments, this.SenderName, this.SenderUser, this.SenderPassword, this.SmtpServer, this.SmtpPort, false);
                            msg.ErrorMsg = string.Empty;
                        }
                        catch (Exception ex)
                        {
                            msg.ErrorMsg = ex.Message;
                            //
                            _mesLog.LogException("Exception: " + returnMessage + ". StackTree: " + ex.Message + Environment.NewLine + ex.StackTrace, this.Name + ".Execute()", true);
                        }
                        //
                    }
                    else if (msg.MsgType == 2)//微信发送
                    {
                        IList<Siemens.Simatic.Wechat.Common.CV_PM_WECHAT_NOTI> notiList = _CV_PM_WECHAT_NOTIBO.GetEntities(new Siemens.Simatic.Wechat.Common.CV_PM_WECHAT_NOTI { AlertID = msg.ObjectID });
                        if (notiList != null && notiList.Count > 0)
                        {
                            foreach (Siemens.Simatic.Wechat.Common.CV_PM_WECHAT_NOTI noti in notiList)
                            {
                                corpsecret = noti.SecretID;
                                _PM_WECHAT_NOTIBO.Gettoken(corpid, corpsecret);
                                if (msg.Format == WechatFormat.IMAGE)
                                {
                                    ReturnValue rv = _PM_WECHAT_NOTIBO.SendImage(noti.AgentID.Value, msg.MsgSubject, msg.MsgContent, noti.UserIDs, false);
                                    msg.ErrorMsg = "SendImage: Success=[" + rv.Success + "]" + ",Message=[" + rv.Message + "]";

                                }
                                else if (msg.Format == WechatFormat.TEXT)
                                {
                                    ReturnValue rv = _PM_WECHAT_NOTIBO.SendText(noti.AgentID.Value, msg.MsgContent, noti.UserIDs, false);
                                    msg.ErrorMsg = "SendText: Success=[" + rv.Success + "]" + ",Message=[" + rv.Message + "]";
                                }else if(msg.Format == WechatFormat.CARD)
                                {
                                    ReturnValue rv = _PM_WECHAT_NOTIBO.SendCard(noti.AgentID.Value,msg.MsgSubject, msg.MsgContent,msg.URL,"更多", "","",noti.UserIDs);
                                    msg.ErrorMsg = "SendText: Success=[" + rv.Success + "]" + ",Message=[" + rv.Message + "]";
                                }
                            }
                        }
                    }

                    msg.SentCnt += 1;

                    //
                    msg.ModifiedOn = DateTime.Now;
                    _PM_ALT_MESSAGEBO.UpdateSome(msg);
                }
                #endregion
                //
                if (!string.IsNullOrEmpty(returnMessage))
                {
                    _mesLog.LogException("Exception: " + returnMessage, this.Name + ".Execute()", true);
                }
               
            }
            catch (Exception ex)
            {
                _mesLog.LogException(ex.Message + Environment.NewLine + ex.StackTrace, this.Name + ".Execute()", true);
            }
            //
            return true;
        }
        #region SendNetMail

        /// <summary>
        /// send net mail
        /// </summary>
        /// <param name="to"></param>
        /// <param name="subject"></param>
        /// <param name="body">message</param>
        /// <param name="EnableSsl">use ssl. true for default. please set false for byd.</param>
        /// <returns></returns>
        public bool SendNetMail(string to, string subject, string body, IList<string> attachments, bool EnableSsl)
        {
            return NotifyHelper.SendNetMail(to, subject, body, attachments, this.SenderName, this.SenderUser, this.SenderPassword, this.SmtpServer, this.SmtpPort, EnableSsl);
        }

        /// <summary>
        /// send net mail
        /// </summary>
        /// <param name="to"></param>
        /// <param name="cc"></param>
        /// <param name="bcc"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <param name="EnableSsl">use ssl. true for default. please set false for byd.</param>
        /// <returns></returns>
        public bool SendNetMail(string to, string cc, string bcc, string subject, string body, string attachments, bool EnableSsl)
        {
            return NotifyHelper.SendNetMail(to, cc, bcc, subject, body, attachments, this.SenderName, this.SenderUser, this.SenderPassword, this.SmtpServer, this.SmtpPort, EnableSsl);
        }

        /// <summary>
        /// send net mail
        /// </summary>
        /// <param name="toList"></param>
        /// <param name="ccList"></param>
        /// <param name="bccList"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <param name="EnableSsl">use ssl. true for default. please set false for byd.</param>
        /// <returns></returns>
        public bool SendNetMail(IList<string> toList, IList<string> ccList, IList<string> bccList, string subject, string body, IList<string> attachments, bool EnableSsl)
        {
            return NotifyHelper.SendNetMail(toList, ccList, bccList, subject, body, attachments, this.SenderName, this.SenderUser, this.SenderPassword, this.SmtpServer, this.SmtpPort, EnableSsl);
        }

        #endregion

        #region 分离Body和QR的部分,用$$$分隔

        private Content GetSplitEmailContent(string emailContent)
        {
            Content cnt = new Content();
            int index = emailContent.IndexOf("$$$");
            if (index < 0)
            {
                cnt.BodyContent = emailContent;
            }
            else
            {
                cnt.BodyContent = emailContent.Substring(0, index);
                cnt.QrContent = emailContent.Substring(index + 3, emailContent.Length - index - 3);
            }

            return cnt;
        }


        #endregion


    }

    public class Content
    {
        private string bodyContent;
        private string qrContent;

        public string BodyContent
        {
            get { return bodyContent; }
            set { bodyContent = value; }
        }

        public string QrContent
        {
            get { return qrContent; }
            set { qrContent = value; }
        }
    }
}
