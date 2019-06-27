using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Linq;

using Siemens.Simatic.Platform.Core;
using Siemens.Simatic.Platform.Common;
using Siemens.Simatic.ALT.Common;
using Siemens.Simatic.ALT.BusinessLogic;
using System.Collections;
using Siemens.Simatic.Wechat.Enums;
using Siemens.Simatic.Wechat.BusinessLogic;
using Siemens.Simatic.Wechat.BusinessLogic.DefaultImpl;
using Siemens.Simatic.Wechat.Common;
//using Siemens.Simatic.PM.BusinessLogic;
//using Siemens.Simatic.PM.Common;
using System.Drawing;
using log4net;
using System.Data;

namespace Siemens.Simatic.Service.NotifyService
{
    public class MsgService //: MainService
    {
        //private ISM_CONFIG_KEYBO SM_CONFIG_KEYBO;
        //private string SenderName;//from
        //private string SenderUser; //add by wesley@20130508
        //private string SenderPassword;
        //private string SmtpServer;
        //private string SmtpPort;

        private string corpid = ConfigurationManager.AppSettings["corpid"];
        //private IPM_ALT_MESSAGEBO _co_BSC_EMAILBO = ObjectContainer.BuildUp<IPM_ALT_MESSAGEBO>();
        //private Siemens.Simatic.Wechat.BusinessLogic.ICV_PM_WECHAT_NOTIBO _cv_WECHAT_NOTIBO = ObjectContainer.BuildUp<Siemens.Simatic.Wechat.BusinessLogic.ICV_PM_WECHAT_NOTIBO>();
        //private IPM_WECHAT_NOTIBO _co_WECHAT_NOTIBO = ObjectContainer.BuildUp<IPM_WECHAT_NOTIBO>();
        //public ICV_PM_ALT_EVENT_TYPE_GRPBO _cv_PM_ALT_EVENT_TYPE_GRPBO;
        //public IPM_ALT_EVENT_LOGBO _co_PM_ALT_EVENT_LOGBO;
        //public ICV_PM_ALT_EVENT_LOGBO _cv_PM_ALT_EVENT_LOGBO;
        //public IPM_ALT_EVENT_LOGBO _PM_ALT_EVENT_LOGBO;
        //public IPM_ALT_MESSAGEBO _pm_ALT_MESSAGEBO;
        //public Siemens.Simatic.Wechat.BusinessLogic.ICV_PM_WECHAT_NOTIBO _cv_PM_WECHAT_NOTIBO;
        //public IPM_WECHAT_NOTIBO _pm_WECHAT_NOTIBO;
        private ILog log = LogManager.GetLogger(typeof(MsgService));
        public IALT_BSC_BO alt_BSC_BO;
        

        public MsgService() : this("MessageTimer")
        {
        }

        public MsgService(string name) //: base(name)
        {
            try
            {
                //_PM_ALT_EVENT_LOGBO = ObjectContainer.BuildUp<IPM_ALT_EVENT_LOGBO>();
                //_cv_PM_ALT_EVENT_LOGBO = ObjectContainer.BuildUp<ICV_PM_ALT_EVENT_LOGBO>();
                //_cv_PM_ALT_EVENT_TYPE_GRPBO = ObjectContainer.BuildUp<ICV_PM_ALT_EVENT_TYPE_GRPBO>();
                //_pm_ALT_MESSAGEBO = ObjectContainer.BuildUp<IPM_ALT_MESSAGEBO>();
                //_cv_PM_WECHAT_NOTIBO = ObjectContainer.BuildUp<Siemens.Simatic.Wechat.BusinessLogic.ICV_PM_WECHAT_NOTIBO>();
                //_pm_WECHAT_NOTIBO = ObjectContainer.BuildUp<IPM_WECHAT_NOTIBO>();
                //SM_CONFIG_KEYBO = ObjectContainer.BuildUp<ISM_CONFIG_KEYBO>();
                alt_BSC_BO = ObjectContainer.BuildUp<IALT_BSC_BO>();
                
            }
            catch (Exception e)
            {
                log.Error(e.Message, e);
            }

            //string strsql = "select sKey,sValue FROM SM_CONFIG_KEY WHERE sKey IN('SenderUser','SenderPwd','SenderAccount','SmtpServer','SmtpPort') ";
            //DataTable dt = alt_BSC_BO.GetDataTableBySql(strsql);
            //if (dt == null || dt.Rows.Count <= 0)
            //{
            //    return;
            //}
            //else
            //{
            //    foreach (DataRow dr in dt.Rows)
            //    {
            //        if (dr[0] == "SenderUser")
            //        {
            //            this.SenderUser = dr[1].ToString();
            //        }
            //        else if (dr[0] == "SenderPwd")
            //        {
            //            this.SenderPassword = dr[1].ToString();
            //        }
            //        else if (dr[0] == "SenderAccount")
            //        {
            //            this.SenderName = dr[1].ToString();
            //        }
            //        else if (dr[0] == "SmtpServer")
            //        {
            //            this.SmtpServer = dr[1].ToString();
            //        }
            //        else if (dr[0] == "SmtpPort")
            //        {
            //            this.SmtpPort = dr[1].ToString();
            //        }
            //    }
            //}

            //this.SenderUser = SM_CONFIG_KEYBO.GetConfigKey("SenderUser").sValue;
            //this.SenderPassword = SM_CONFIG_KEYBO.GetConfigKey("SenderPwd").sValue;
            //this.SenderName = SM_CONFIG_KEYBO.GetConfigKey("SenderAccount").sValue;
            //this.SmtpServer = SM_CONFIG_KEYBO.GetConfigKey("SmtpServer").sValue;
            //this.SmtpPort = SM_CONFIG_KEYBO.GetConfigKey("SmtpPort").sValue;
        }

        //protected override object GetReadyData()
        //{
        //    try
        //    {
        //        return new object();
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        ////deleted by hans on 6.8
        //protected override bool Execute(object readyData)
        //{
        //    try
        //    {
        //        if (readyData == null) 
        //            return false;

        //        string returnMessage = string.Empty;
        //        bool isSent = false;

        //        #region PM_ALT_MESSAGE
        //        IList<PM_ALT_MESSAGE> pamsg = _co_BSC_EMAILBO.GetEntitiesToSend();
        //        if (pamsg == null)
        //        {
        //            return false;
        //        }

        //        foreach (PM_ALT_MESSAGE msg in pamsg)
        //        {
        //            if (msg.MsgType == 1)//邮件发送
        //            {
        //                isSent = false;
        //                //
        //                string to = msg.MsgTo;
        //                string cc = msg.MsgCc;
        //                string bcc = msg.MsgBcc;
        //                string subject = msg.MsgSubject;
        //                string body = msg.MsgContent;
        //                body = System.Web.HttpUtility.HtmlDecode(body);
        //                //
        //                try
        //                {
        //                    isSent = MailHelper.SendNetMail(to, cc, bcc, subject, body, msg.Attachments, this.SenderName, this.SenderUser, this.SenderPassword, this.SmtpServer, this.SmtpPort, false);
        //                    msg.ErrorMsg = string.Empty;
        //                }
        //                catch (Exception ex)
        //                {
        //                    msg.ErrorMsg = ex.Message;
        //                    //
        //                    log.Error(this.Name + ".Execute()", ex);
        //                }
        //                //
        //            }
        //            else if (msg.MsgType == 2)//微信发送
        //            {
        //                IList<Siemens.Simatic.Wechat.Common.CV_PM_WECHAT_NOTI> notiList = _cv_WECHAT_NOTIBO.GetEntities(new Siemens.Simatic.Wechat.Common.CV_PM_WECHAT_NOTI { AlertID = msg.ObjectID });
        //                if (notiList != null && notiList.Count > 0)
        //                {
        //                    foreach (Siemens.Simatic.Wechat.Common.CV_PM_WECHAT_NOTI noti in notiList)
        //                    {
        //                        _pm_WECHAT_NOTIBO.Gettoken(corpid, noti.SecretID);
        //                        if (msg.Format == WechatFormat.IMAGE)
        //                        {
        //                            Siemens.Simatic.Wechat.Common.ReturnValue rv = _pm_WECHAT_NOTIBO.SendImage(noti.AgentID.Value, msg.MsgSubject, msg.MsgContent, noti.UserIDs, false);
        //                            msg.ErrorMsg = "SendImage: Success=[" + rv.Success + "]" + ",Message=[" + rv.Message + "]";

        //                        }
        //                        else if (msg.Format == WechatFormat.TEXT)
        //                        {
        //                            Siemens.Simatic.Wechat.Common.ReturnValue rv = _pm_WECHAT_NOTIBO.SendText(noti.AgentID.Value, msg.MsgContent, noti.UserIDs, false);
        //                            msg.ErrorMsg = "SendText: Success=[" + rv.Success + "]" + ",Message=[" + rv.Message + "]";
        //                        }
        //                        else if (msg.Format == WechatFormat.QRCODE)
        //                        {
        //                            Content cnt = this.GetSplitEmailContent(msg.MsgContent);
        //                            Siemens.Simatic.Wechat.Common.ReturnValue rv = new Siemens.Simatic.Wechat.Common.ReturnValue();
        //                            if (!string.IsNullOrEmpty(cnt.BodyContent))
        //                            {
        //                                rv = _pm_WECHAT_NOTIBO.SendText(noti.AgentID.Value, cnt.BodyContent, noti.UserIDs, false);
        //                                msg.ErrorMsg = "SendText: Success=[" + rv.Success + "]" + ",Message=[" + rv.Message + "]";
        //                            }

        //                            if (rv.Success)
        //                            {
        //                                if (!string.IsNullOrEmpty(cnt.QrContent))
        //                                {
        //                                    rv = _pm_WECHAT_NOTIBO.SendQRImage(noti.AgentID.Value, msg.MsgSubject, cnt.QrContent, noti.UserIDs, false);
        //                                    msg.ErrorMsg = "SendQRCode: Success=[" + rv.Success + "]" + ",Message=[" + rv.Message + "]";
        //                                }
        //                            }
        //                        }
        //                    }
        //                }
        //            }

        //            msg.SentCnt += 1;

        //            //
        //            msg.ModifiedOn = DateTime.Now;
        //            _co_BSC_EMAILBO.UpdateSome(msg);
        //        }
        //        #endregion
        //        //
        //        if (!string.IsNullOrEmpty(returnMessage))
        //        {
        //            log.Error(this.Name + ".Execute()" + returnMessage);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        log.Error(this.Name + ".Execute()", ex);
        //    }
        //    //
        //    return true;
        //}


    //    /// <summary>
    //    /// 发送所有的报警
    //    /// </summary>
    //    /// <param name="readyData"></param>
    //    /// <returns></returns>
    //    public bool ExecuteNotifyAll()
    //    {
    //        try
    //        {
    //            string returnMessage = string.Empty;
    //            bool isSent = false;

    //            #region PM_ALT_MESSAGE
    //            IList<PM_ALT_MESSAGE> pamsg = _co_BSC_EMAILBO.GetEntitiesToSend();
    //            if (pamsg == null)
    //            {
    //                return false;
    //            }

    //            foreach (PM_ALT_MESSAGE msg in pamsg)
    //            {
    //                this.ExecuteNotify(msg);
    //            }
    //            #endregion
    //            //
    //            if (!string.IsNullOrEmpty(returnMessage))
    //            {
    //                log.Error("ExecuteNotifyAll error: " + returnMessage);
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            log.Error("ExecuteNotifyAll error: ", ex);
    //        }
    //        //
    //        return true;
    //    }

    //    /// <summary>
    //    /// 发送报警
    //    /// </summary>
    //    /// <param name="readyData"></param>
    //    /// <returns></returns>
    //    public bool ExecuteNotify(PM_ALT_MESSAGE msg)
    //    {
    //        try
    //        {
    //            string returnMessage = string.Empty;
    //            bool isSent = false;

    //            if (msg.MsgType == 1) //邮件发送
    //            {
    //                isSent = false;
    //                string to = msg.MsgTo;
    //                string cc = msg.MsgCc;
    //                string bcc = msg.MsgBcc;
    //                string subject = msg.MsgSubject;
    //                string body = msg.MsgContent;
    //                body = System.Web.HttpUtility.HtmlDecode(body);

    //                try
    //                {
    //                    isSent = MailHelper.SendNetMail(to, cc, bcc, subject, body, msg.Attachments, this.SenderName, this.SenderUser, this.SenderPassword, this.SmtpServer, this.SmtpPort, false);
    //                    msg.ErrorMsg = string.Empty;
    //                }
    //                catch (Exception ex)
    //                {
    //                    msg.ErrorMsg = ex.Message;
    //                    log.Error("ExecuteNotify error: ", ex);
    //                    return false;
    //                }
    //            }
    //            else if (msg.MsgType == 2) //微信发送
    //            {
    //                IList<Siemens.Simatic.Wechat.Common.CV_PM_WECHAT_NOTI> notiList = _cv_WECHAT_NOTIBO.GetEntities(new Siemens.Simatic.Wechat.Common.CV_PM_WECHAT_NOTI { AlertID = msg.ObjectID });
    //                if (notiList != null && notiList.Count > 0)
    //                {
    //                    foreach (Siemens.Simatic.Wechat.Common.CV_PM_WECHAT_NOTI noti in notiList)
    //                    {
    //                        _pm_WECHAT_NOTIBO.Gettoken(corpid, noti.SecretID);
    //                        if (msg.Format == WechatFormat.IMAGE)
    //                        {
    //                            Siemens.Simatic.Wechat.Common.ReturnValue rv = _pm_WECHAT_NOTIBO.SendImage(noti.AgentID.Value, msg.MsgSubject, msg.MsgContent, noti.UserIDs, false);
    //                            msg.ErrorMsg = "SendImage: Success=[" + rv.Success + "]" + ",Message=[" + rv.Message + "]";
    //                        }
    //                        else if (msg.Format == WechatFormat.TEXT)
    //                        {
    //                            Siemens.Simatic.Wechat.Common.ReturnValue rv = _pm_WECHAT_NOTIBO.SendText(noti.AgentID.Value, msg.MsgContent, noti.UserIDs, false);
    //                            msg.ErrorMsg = "SendText: Success=[" + rv.Success + "]" + ",Message=[" + rv.Message + "]";
    //                        }
    //                        else if (msg.Format == WechatFormat.QRCODE)
    //                        {
    //                            Content cnt = this.GetSplitEmailContent(msg.MsgContent);
    //                            Siemens.Simatic.Wechat.Common.ReturnValue rv = new Siemens.Simatic.Wechat.Common.ReturnValue();
    //                            if (!string.IsNullOrEmpty(cnt.BodyContent))
    //                            {
    //                                rv = _pm_WECHAT_NOTIBO.SendText(noti.AgentID.Value, cnt.BodyContent, noti.UserIDs, false);
    //                                msg.ErrorMsg = "SendText: Success=[" + rv.Success + "]" + ",Message=[" + rv.Message + "]";
    //                            }

    //                            if (rv.Success)
    //                            {
    //                                if (!string.IsNullOrEmpty(cnt.QrContent))
    //                                {
    //                                    rv = _pm_WECHAT_NOTIBO.SendQRImage(noti.AgentID.Value, msg.MsgSubject, cnt.QrContent, noti.UserIDs, false);
    //                                    msg.ErrorMsg = "SendQRCode: Success=[" + rv.Success + "]" + ",Message=[" + rv.Message + "]";
    //                                }
    //                            }
    //                        }
    //                    }
    //                }
    //            }

    //            //更新message
    //            msg.SentCnt += 1;
    //            msg.ModifiedOn = DateTime.Now;
    //            _co_BSC_EMAILBO.UpdateSome(msg);

    //            if (!string.IsNullOrEmpty(returnMessage))
    //            {
    //                log.Error("ExecuteNotify error: " + returnMessage);
    //                return false;
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            log.Error("ExecuteNotify error: ", ex);
    //            return false;
    //        }
    //        return true;
    //    }


    //    #region SendNetMail

    //    /// <summary>
    //    /// send net mail
    //    /// </summary>
    //    /// <param name="to"></param>
    //    /// <param name="subject"></param>
    //    /// <param name="body">message</param>
    //    /// <param name="EnableSsl">use ssl. true for default. please set false for byd.</param>
    //    /// <returns></returns>
    //    public bool SendNetMail(string to, string subject, string body, IList<string> attachments, bool EnableSsl)
    //    {
    //        return MailHelper.SendNetMail(to, subject, body, attachments, this.SenderName, this.SenderUser, this.SenderPassword, this.SmtpServer, this.SmtpPort, EnableSsl);
    //    }

    //    /// <summary>
    //    /// send net mail
    //    /// </summary>
    //    /// <param name="to"></param>
    //    /// <param name="cc"></param>
    //    /// <param name="bcc"></param>
    //    /// <param name="subject"></param>
    //    /// <param name="body"></param>
    //    /// <param name="EnableSsl">use ssl. true for default. please set false for byd.</param>
    //    /// <returns></returns>
    //    public bool SendNetMail(string to, string cc, string bcc, string subject, string body, string attachments, bool EnableSsl)
    //    {
    //        return MailHelper.SendNetMail(to, cc, bcc, subject, body, attachments, this.SenderName, this.SenderUser, this.SenderPassword, this.SmtpServer, this.SmtpPort, EnableSsl);
    //    }

    //    /// <summary>
    //    /// send net mail
    //    /// </summary>
    //    /// <param name="toList"></param>
    //    /// <param name="ccList"></param>
    //    /// <param name="bccList"></param>
    //    /// <param name="subject"></param>
    //    /// <param name="body"></param>
    //    /// <param name="EnableSsl">use ssl. true for default. please set false for byd.</param>
    //    /// <returns></returns>
    //    public bool SendNetMail(IList<string> toList, IList<string> ccList, IList<string> bccList, string subject, string body, IList<string> attachments, bool EnableSsl)
    //    {
    //        return MailHelper.SendNetMail(toList, ccList, bccList, subject, body, attachments, this.SenderName, this.SenderUser, this.SenderPassword, this.SmtpServer, this.SmtpPort, EnableSsl);
    //    }

    //    #endregion

    //    #region 分离Body和QR的部分,用$$$分隔

    //    private Content GetSplitEmailContent(string emailContent)
    //    {
    //        Content cnt = new Content();
    //        int index = emailContent.IndexOf("$$$");
    //        if (index < 0)
    //        {
    //            cnt.BodyContent = emailContent;
    //        }
    //        else
    //        {
    //            cnt.BodyContent = emailContent.Substring(0, index);
    //            cnt.QrContent = emailContent.Substring(index + 3, emailContent.Length - index - 3);
    //        }
    //        return cnt;
    //    }
    //    #endregion

    }

    //public class Content
    //{
    //    private string bodyContent;
    //    private string qrContent;

    //    public string BodyContent
    //    {
    //        get { return bodyContent; }
    //        set { bodyContent = value; }
    //    }

    //    public string QrContent
    //    {
    //        get { return qrContent; }
    //        set { qrContent = value; }
    //    }
    //}


}
