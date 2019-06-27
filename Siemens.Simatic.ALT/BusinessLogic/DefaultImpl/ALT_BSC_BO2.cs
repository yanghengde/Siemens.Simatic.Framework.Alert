
using System;
using System.Collections.Generic;

using Siemens.Simatic.Platform.Core;
using Siemens.Simatic.ALT.Common;

using System.Configuration;
using Siemens.Simatic.Wechat.Enums;
using Newtonsoft.Json;

namespace Siemens.Simatic.ALT.BusinessLogic.DefaultImpl
{
    public partial class ALT_BSC_BO : IALT_BSC_BO
    {
        private string SenderAccount = string.Empty;//from
        private string SenderUser = string.Empty; //add by wesley@20130508
        private string SenderPassword = string.Empty;
        private string SmtpServer = string.Empty;
        private string SmtpPort = string.Empty;

        private string corpid = string.Empty; //ConfigurationManager.AppSettings["corpid"];
        private string corpSecret = string.Empty;
        private string AuxApiUrl = string.Empty;

        //private IPM_ALT_EVENT_LOGBO _PM_ALT_EVENT_LOGBO = ObjectContainer.BuildUp<IPM_ALT_EVENT_LOGBO>();
        //private ICV_PM_ALT_EVENT_LOGBO _cv_PM_ALT_EVENT_LOGBO = ObjectContainer.BuildUp<ICV_PM_ALT_EVENT_LOGBO>();
        //private ICV_PM_ALT_EVENT_TYPE_GRPBO _cv_PM_ALT_EVENT_TYPE_GRPBO = ObjectContainer.BuildUp<ICV_PM_ALT_EVENT_TYPE_GRPBO>();
        private IPM_ALT_MESSAGEBO _co_BSC_EMAILBO = ObjectContainer.BuildUp<IPM_ALT_MESSAGEBO>(); 
        //private Siemens.Simatic.Wechat.BusinessLogic.ICV_PM_WECHAT_NOTIBO _cv_WECHAT_NOTIBO = ObjectContainer.BuildUp<Siemens.Simatic.Wechat.BusinessLogic.ICV_PM_WECHAT_NOTIBO>();
        //private IPM_ALT_BASEBO pm_ALT_BASEBO = ObjectContainer.BuildUp<IPM_ALT_BASEBO>();

        //add wuh 20180725
        private IAPI_WECHAT_BO api_wechat_bo = ObjectContainer.BuildUp<IAPI_WECHAT_BO>();
        //private IAPI_WECHAT_AUX api_WECHAT_AUX = ObjectContainer.BuildUp<IAPI_WECHAT_AUX>();
        private Http_Request http_Request = new Http_Request();
        private AuxApiEntity apiEntity = new AuxApiEntity();

        //public void InitServer()
        //{
        //    string strsql = "select sKey,sValue FROM SM_CONFIG_KEY WHERE sKey IN('SenderUser','SenderPwd','SenderAccount','SmtpServer','SmtpPort','corpid','corpsecret') ";
        //    DataTable dt = this.GetDataTableBySql(strsql);
        //    if (dt == null || dt.Rows.Count <= 0)
        //    {
        //        return;
        //    }
        //    else
        //    {
        //        foreach (DataRow dr in dt.Rows)
        //        {
        //            string sKey = dr[0].ToString();
        //            if (string.Compare(sKey, "SenderUser", true) == 0)
        //            {
        //                this.SenderUser = dr[1].ToString();
        //            }
        //            else if (string.Compare(sKey, "SenderPwd", true) == 0)
        //            {
        //                this.SenderPassword = dr[1].ToString();
        //            }
        //            else if (string.Compare(sKey, "SenderAccount", true) == 0)
        //            {
        //                this.SenderAccount = dr[1].ToString();
        //            }
        //            else if (string.Compare(sKey, "SmtpServer", true) == 0)
        //            {
        //                this.SmtpServer = dr[1].ToString();
        //            }
        //            else if (string.Compare(sKey, "SmtpPort", true) == 0)
        //            {
        //                this.SmtpPort = dr[1].ToString();
        //            }
        //            else if (string.Compare(sKey, "corpid", true) == 0)  //企业微信ID
        //            {
        //                this.corpid = dr[1].ToString();
        //            }
        //            else if (string.Compare(sKey, "corpsecret", true) == 0)  //企业微信ID
        //            {
        //                this.corpSecret = dr[1].ToString();
        //               // _pm_WECHAT_NOTIBO.Gettoken(corpid, corpSecret);                    
        //            }
        //        }
        //    }
        //}

        /// <summary>
        /// 获取配置
        /// 成功返回OK
        /// </summary>
        /// <returns></returns>
        public string InitServer()
        {
            //this.AuxApiUrl = ConfigurationSettings.AppSettings["AuxApiUrl"]; //Aux微信API的地址

            string strReturn = "OK";

            this.corpid = ConfigurationSettings.AppSettings["corpid"]; //企业微信号
            this.corpSecret = ConfigurationSettings.AppSettings["corpSecret"]; //应用Secret            

            this.SenderAccount = ConfigurationSettings.AppSettings["SenderAccount"]; //企业微信号
            this.SenderUser = ConfigurationSettings.AppSettings["SenderUser"]; //应用Secret
            this.SenderPassword = ConfigurationSettings.AppSettings["SenderPassword"];   //通讯录Secret
            this.SmtpServer = ConfigurationSettings.AppSettings["SmtpServer"]; //应用Secret
            this.SmtpPort = ConfigurationSettings.AppSettings["SmtpPort"];   //通讯录Secret
            if (string.IsNullOrEmpty(corpid))
            {
                strReturn = "corpid配置缺失";
            }
            if (string.IsNullOrEmpty(corpSecret))
            {
                strReturn = "corpSecret配置缺失";
            }
            if (string.IsNullOrEmpty(SenderAccount))
            {
                strReturn = "SenderAccount配置缺失";
            }
            if (string.IsNullOrEmpty(SenderUser))
            {
                strReturn = "SenderUser配置缺失";
            }
            if (string.IsNullOrEmpty(SenderPassword))
            {
                strReturn = "SenderPassword配置缺失";
            }
            if (string.IsNullOrEmpty(SmtpServer))
            {
                strReturn = "SmtpServer配置缺失";
            }
            if (string.IsNullOrEmpty(SmtpPort))
            {
                strReturn = "SmtpPort配置缺失";
            }
            return strReturn;
        }

        /// <summary>
        /// 发送预警 -- 作废
        /// </summary>
        /// <param name="readyData"></param>
        /// <returns></returns>
        public bool ExecuteNotify(PM_ALT_MESSAGE msg)
        {
            //try
            //{
            //    string returnMessage = string.Empty;
            //    if (string.IsNullOrEmpty(corpid) || string.IsNullOrEmpty(corpSecret))
            //    {
            //        string strInitServer = this.InitServer();
            //        if (strInitServer != "OK")
            //        {
            //            msg.ErrorMsg = strInitServer;
            //            goto HanderProcess;
            //        }
            //    }

            //    bool isSent = false;

            //    if (string.IsNullOrEmpty(msg.MsgContent))
            //    {
            //        msg.ErrorMsg = "消息内容为空";
            //        goto HanderProcess;
            //    }

            //    if (msg.MsgType == 1) //邮件发送
            //    {
            //        isSent = false;
            //        string to = msg.MsgTo;
            //        string cc = msg.MsgCc;
            //        string bcc = msg.MsgBcc;
            //        string subject = msg.MsgSubject;
            //        string body = msg.MsgContent;
            //        body = System.Web.HttpUtility.HtmlDecode(body);

            //        try
            //        {
            //            isSent = MailHelper.SendNetMail(to, cc, bcc, subject, body, msg.Attachments, this.SenderAccount, this.SenderUser, this.SenderPassword, this.SmtpServer, this.SmtpPort, false);
            //            msg.ErrorMsg = string.Empty;
            //        }
            //        catch (Exception ex)
            //        {
            //            msg.ErrorMsg = ex.Message;
            //            goto HanderProcess;
            //        }
            //    }
            //    else if (msg.MsgType == 2) //微信发送
            //    {
            //        api_wechat_bo.Gettoken(this, corpid, corpSecret);

            //        IList<Siemens.Simatic.Wechat.Common.CV_PM_WECHAT_NOTI> notiList = _cv_WECHAT_NOTIBO.GetEntities(new Siemens.Simatic.Wechat.Common.CV_PM_WECHAT_NOTI { AlertID = msg.ObjectID });
            //        if (notiList != null && notiList.Count > 0)
            //        {
            //            foreach (Siemens.Simatic.Wechat.Common.CV_PM_WECHAT_NOTI noti in notiList)
            //            {
            //                if (msg.Format == WechatFormat.IMAGE)
            //                {
            //                    Siemens.Simatic.Wechat.Common.ReturnValue rv = api_wechat_bo.SendImage(noti.AgentID.Value, msg.MsgSubject, msg.MsgContent, noti.UserIDs, false);
            //                    msg.ErrorMsg = "SendImage: Success=[" + rv.Success + "]" + ",Message=[" + rv.Message + "]";
            //                }
            //                else if (msg.Format == WechatFormat.TEXT) //发送文本
            //                {
            //                    Siemens.Simatic.Wechat.Common.ReturnValue rv = api_wechat_bo.SendText(noti.AgentID.Value, msg.MsgContent, noti.UserIDs, false);
            //                    msg.ErrorMsg = "SendText: Success=[" + rv.Success + "]" + ",Message=[" + rv.Message + "]";
            //                }
            //                else if (msg.Format == WechatFormat.QRCODE)
            //                {
            //                    Content cnt = this.GetSplitEmailContent(msg.MsgContent);
            //                    Siemens.Simatic.Wechat.Common.ReturnValue rv = new Siemens.Simatic.Wechat.Common.ReturnValue();
            //                    if (!string.IsNullOrEmpty(cnt.BodyContent))
            //                    {
            //                        rv = api_wechat_bo.SendText(noti.AgentID.Value, cnt.BodyContent, noti.UserIDs, false);
            //                        msg.ErrorMsg = "SendText: Success=[" + rv.Success + "]" + ",Message=[" + rv.Message + "]";
            //                    }

            //                    if (rv.Success)
            //                    {
            //                        if (!string.IsNullOrEmpty(cnt.QrContent))
            //                        {
            //                            rv = api_wechat_bo.SendQRImage(noti.AgentID.Value, msg.MsgSubject, cnt.QrContent, noti.UserIDs, false);
            //                            msg.ErrorMsg = "SendQRCode: Success=[" + rv.Success + "]" + ",Message=[" + rv.Message + "]";
            //                        }
            //                    }
            //                }
            //            }
            //        }
            //    }

            //HanderProcess:
            //    msg.SentCnt += 1;
            //    msg.ModifiedOn = DateTime.Now;
            //    _co_BSC_EMAILBO.UpdateSome(msg); //更新message

            //    if (!string.IsNullOrEmpty(msg.ErrorMsg) && !msg.ErrorMsg.Contains("Success=[True],Message=[ok]"))
            //    {
            //        //log.Error("ExecuteNotify error: " + returnMessage);
            //        return false;
            //    }
            //    else //移到历史表
            //    {
            //        string sql = @"INSERT INTO PM_ALT_MESSAGE_HISTORY SELECT * FROM PM_ALT_MESSAGE WHERE MsgPK={0};
            //                           DELETE FROM PM_ALT_MESSAGE WHERE MsgPK={0};";
            //        sql = string.Format(sql, msg.MsgPK);
            //        this.ExecuteNonQueryBySql(sql);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    return false;
            //}
            //return true;
            return true;
        }

        /// <summary>
        /// 发送预警 - Aux接口
        /// </summary>
        /// <param name="readyData"></param>
        /// <returns></returns>
        public bool ExecuteNotifyAux(PM_ALT_MESSAGE msg)
        {
            //try
            //{
            //    string returnMessage = string.Empty;
            //    if (string.IsNullOrEmpty(corpid) || string.IsNullOrEmpty(corpSecret))
            //    {
            //        string strInitServer = this.InitServer();
            //        if (strInitServer != "OK")
            //        {
            //            msg.ErrorMsg = strInitServer;
            //            goto HanderProcess;
            //        }
            //    }                               

            //    if (string.IsNullOrEmpty(msg.MsgContent))
            //    {
            //        msg.ErrorMsg = "消息内容为空";
            //        goto HanderProcess;
            //    }

            //    bool isSent = false;
            //    if (msg.MsgType == 1) //邮件发送
            //    {
            //        //isSent = false;
            //        //string to = msg.MsgTo;
            //        //string cc = msg.MsgCc;
            //        //string bcc = msg.MsgBcc;
            //        //string subject = msg.MsgSubject;
            //        //string body = msg.MsgContent;
            //        //body = System.Web.HttpUtility.HtmlDecode(body);

            //        //try
            //        //{
            //        //    isSent = MailHelper.SendNetMail(to, cc, bcc, subject, body, msg.Attachments, this.SenderAccount, this.SenderUser, this.SenderPassword, this.SmtpServer, this.SmtpPort, false);
            //        //    msg.ErrorMsg = string.Empty;
            //        //}
            //        //catch (Exception ex)
            //        //{
            //        //    msg.ErrorMsg = ex.Message;                        
            //        //}
            //        //goto HanderProcess;
            //    }
            //    else if (msg.MsgType == 2) //微信发送
            //    {
            //        //api_wechat_bo.Gettoken(this, corpid, corpSecret);
            //        Siemens.Simatic.Wechat.Common.CV_PM_WECHAT_NOTI qp = new Siemens.Simatic.Wechat.Common.CV_PM_WECHAT_NOTI { AlertID = msg.ObjectID };
            //        IList<Siemens.Simatic.Wechat.Common.CV_PM_WECHAT_NOTI> notiList = _cv_WECHAT_NOTIBO.GetEntities(qp);
            //        if (notiList == null || notiList.Count == 0)
            //        {
            //            msg.ErrorMsg = "找不到预警的配置";
            //            goto HanderProcess;
            //        }
            //        Siemens.Simatic.Wechat.Common.CV_PM_WECHAT_NOTI noti = notiList[0];

            //        //if (msg.Format == WechatFormat.IMAGE) 
            //        //{
            //        //    Siemens.Simatic.Wechat.Common.ReturnValue rv = api_wechat_bo.SendImage(noti.AgentID.Value, msg.MsgSubject, msg.MsgContent, noti.UserIDs, false);
            //        //    msg.ErrorMsg = "SendImage: Success=[" + rv.Success + "]" + ",Message=[" + rv.Message + "]";
            //        //}
            //        //else if (msg.Format == WechatFormat.TEXT)
            //        //{
            //        //    Siemens.Simatic.Wechat.Common.ReturnValue rv = api_wechat_bo.SendText(noti.AgentID.Value, msg.MsgContent, noti.UserIDs, false);
            //        //    msg.ErrorMsg = "SendText: Success=[" + rv.Success + "]" + ",Message=[" + rv.Message + "]";
            //        //}

            //        List<string> userlist = new List<string>();
            //        //userlist.Add("030301069"); //测试用户
            //        //userlist.Add("030301069"); //测试用户                    
            //        foreach (string user in noti.UserIDs.Split('|'))
            //        {
            //            userlist.Add(user);
            //        }
            //        apiEntity.agentId = "9";
            //        apiEntity.userIds = userlist; //noti.UserIDs;
            //        apiEntity.content = msg.MsgContent;
            //        string strjson = JsonConvert.SerializeObject(apiEntity);
            //        string strReturn = http_Request.HttpPost(this.AuxApiUrl, strjson);//aux的接口
            //        if (strReturn == "1") //成功
            //        {
            //            msg.ErrorMsg = string.Empty;
            //        }
            //        else
            //        {
            //            msg.ErrorMsg = strReturn;
            //        }
            //    }

            //HanderProcess:
            //    msg.SentCnt += 1;
            //    msg.ModifiedOn = DateTime.Now;
            //    _co_BSC_EMAILBO.UpdateSome(msg); //更新message

            //    if (!string.IsNullOrEmpty(msg.ErrorMsg)) //&& !msg.ErrorMsg.Contains("Success=[True],Message=[ok]")
            //    {
            //        return false;
            //    }
            //    else //成功，移到历史表
            //    {
            //        string sql = @"INSERT INTO PM_ALT_MESSAGE_HISTORY SELECT * FROM PM_ALT_MESSAGE WHERE MsgPK={0};
            //                       DELETE FROM PM_ALT_MESSAGE WHERE MsgPK={0};";
            //        sql = string.Format(sql, msg.MsgPK);
            //        this.ExecuteNonQueryBySql(sql);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    return false;
            //}
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
            return MailHelper.SendNetMail(to, subject, body, attachments, this.SenderAccount, this.SenderUser, this.SenderPassword, this.SmtpServer, this.SmtpPort, EnableSsl);
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
            return MailHelper.SendNetMail(to, cc, bcc, subject, body, attachments, this.SenderAccount, this.SenderUser, this.SenderPassword, this.SmtpServer, this.SmtpPort, EnableSsl);
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
            return MailHelper.SendNetMail(toList, ccList, bccList, subject, body, attachments, this.SenderAccount, this.SenderUser, this.SenderPassword, this.SmtpServer, this.SmtpPort, EnableSsl);
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

    #endregion


        

}

