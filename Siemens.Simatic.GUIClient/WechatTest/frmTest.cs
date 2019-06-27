using Siemens.Simatic.Platform.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
//using Siemens.Simatic.Util.Utilities;
using System.Configuration;
using Siemens.Simatic.Wechat.Enums;
//using System.Web.UI.WebControls;
using Siemens.Simatic.Wechat.BusinessLogic;
using Siemens.Simatic.WechatTest.BusinessLogic;
using Siemens.Simatic.WechatTest.Entities;

namespace Siemens.Simatic.WechatTest
{
    public partial class frmTest : Form
    {
        private string SenderName;//from
        private string SenderUser; //邮件的发送者
        private string SenderPassword;
        private string SmtpServer;
        private string SmtpPort;
        private string corpid = string.Empty;
        private string corpSecret = string.Empty;
        private string orgSecret = string.Empty;

        private PM_WECHAT_DEPARTMENTBO pm_WECHAT_DEPARTMENTBO ;
        private API_WECHAT_BO api_wechat_bo;

        public frmTest()
        {
            InitializeComponent();
        }

        private void frmTest_Load(object sender, EventArgs e)
        {
            DbHelperSQL.connectionString = ConfigurationSettings.AppSettings["ConnectionString"]; 
            this.corpid = ConfigurationSettings.AppSettings["corpid"]; //企业微信号
            this.corpSecret = ConfigurationSettings.AppSettings["corpSecret"]; //应用Secret
            this.orgSecret = ConfigurationSettings.AppSettings["orgSecret"];   //通讯录Secret

            api_wechat_bo = new API_WECHAT_BO();
            pm_WECHAT_DEPARTMENTBO = new PM_WECHAT_DEPARTMENTBO(corpid, corpSecret, orgSecret);
        }

        bool IsDelDetail = true;
        //扫描箱号
        private void txtBoxID_KeyDown(object sender, KeyEventArgs e)
        { }

        //扫描序列号 装箱
        private void txtLotID_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void Beep(int a, int b)
        {
            throw new NotImplementedException();
        }

        private void txtLotID_TextChanged(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 文字测试
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTest_Click(object sender, EventArgs e)
        {
            try
            {
                PM_ALT_MESSAGE altMes = new PM_ALT_MESSAGE();
                altMes.MsgPK = 0;  //需要插入表产生新的MsgPK
                altMes.MsgType = 2;
                altMes.ObjectID = new Guid("6BCBEA79-2F97-47F0-A20D-6D3D9A36CA1A"); //用于查询发送的人
                altMes.Format = "TEXT";
                altMes.MsgSubject = "文本测试标题";
                altMes.MsgContent = "文本测试内容";

                bool boolReturn = this.ExecuteNotify(altMes);
                if (!boolReturn)
                {
                    MessageBox.Show("发送失败");
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("系统异常" +ex.Message);
            }
        }

        /// <summary>
        /// 图片测试
        /// 数据源为DataTable
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSendImage_Click(object sender, EventArgs e)
        {
            try
            {
                PM_ALT_MESSAGE altMes = new PM_ALT_MESSAGE();
                altMes.MsgPK = 0; //需要插入表产生新的MsgPK
                altMes.MsgType = 2;
                altMes.ObjectID = new Guid("6BCBEA79-2F97-47F0-A20D-6D3D9A36CA1A");//预警主键，用于查询发送的人
                altMes.Format = "IMAGE";
                altMes.MsgSubject = "图片测试标题";

                DataTable dtContent = new DataTable();
                dtContent.Columns.Add("姓名");
                dtContent.Columns.Add("年龄");
                dtContent.Columns.Add("岗位");
                dtContent.Rows.Add("张三", "21", "贴片");
                dtContent.Rows.Add("李四", "17", "AOI");
                dtContent.Rows.Add("王五", "19", "回流焊");
                string content = this.BuildTable(dtContent); //把DataTable转成html table

                altMes.MsgContent = content;
                this.ExecuteNotify(altMes);
            }
            catch (Exception ex)
            {
                MessageBox.Show("系统异常:" + ex.Message);
            }
        }

        private void btnSyncAgent_Click(object sender, EventArgs e)
        {
            try
            {
                pm_WECHAT_DEPARTMENTBO.SyncWeChatAgentTest();
            }
            catch (Exception ex)
            {
                MessageBox.Show("系统异常:" + ex.Message);
            }
        }

        private void btnSyncOrg_Click(object sender, EventArgs e)
        {
            try
            {
                pm_WECHAT_DEPARTMENTBO.SyncWeChatDepartmentTest();
            }
            catch (Exception ex)
            {
                MessageBox.Show("系统异常:" + ex.Message);
            }
        }



        /// <summary>
        /// 发送报警
        /// </summary>
        /// <param name="readyData"></param>
        /// <returns></returns>
        public bool ExecuteNotify(PM_ALT_MESSAGE msg)
        {
            try
            {
                string returnMessage = string.Empty;
                if (string.IsNullOrEmpty(corpid))
                {
                    msg.ErrorMsg = "企业ID为空";
                    return false;
                }
                bool isSent = false;

                if (msg.MsgType == 1) //邮件发送
                {
                    isSent = false;
                    string to = msg.MsgTo;
                    string cc = msg.MsgCc;
                    //string bcc = msg.MsgBcc;
                    string subject = msg.MsgSubject;
                    string body = msg.MsgContent;
                    //body = System.Web.HttpUtility.HtmlDecode(body);
                    //try
                    //{
                    //    isSent = MailHelper.SendNetMail(to, cc, bcc, subject, body, msg.Attachments, this.SenderName, this.SenderUser, this.SenderPassword, this.SmtpServer, this.SmtpPort, false);
                    //    msg.ErrorMsg = string.Empty;
                    //}
                    //catch (Exception ex)
                    //{
                    //    msg.ErrorMsg = ex.Message;
                    //    //log.Error("ExecuteNotify error: ", ex);
                    //    return false;
                    //}
                }
                else if (msg.MsgType == 2) //微信发送
                {
                    //获取微信token
                    api_wechat_bo.Gettoken(corpid, corpSecret);

                    //查询发给哪些人
                    string strSelectUsers = @"WITH sql1 AS
                    (
                      SELECT DISTINCT pwn.AlertID,pwa.AgentGuid,pwa.AgentID,pwa.SecretID,pwu.UserID 
                      FROM PM_ALT_NOTI pwn WITH(NOLOCK) 
                      INNER JOIN PM_WECHAT_USER pwu WITH(NOLOCK) ON pwn.UserGuid = pwu.UserGuid
                      --INNER JOIN PM_WECHAT_DEPARTMENT pwd WITH(NOLOCK) ON pwn.DepartmentGuid = pwd.DepartmentGuid
                      INNER JOIN PM_WECHAT_AGENT pwa WITH(NOLOCK) ON pwn.AgentGuid = pwa.AgentGuid
                    )
                    SELECT AlertID,AgentGuid,AgentID,SecretID,[UserIDs]=STUFF((SELECT '|' + UserID FROM sql1 t WHERE t.AlertID=sql1.AlertID AND t.AgentGuid = sql1.AgentGuid FOR XML PATH('')), 1, 1, '') 
                    FROM sql1 
                    WHERE AlertID ='{0}'
                    GROUP BY AlertID,AgentGuid,AgentID,SecretID ";
                    strSelectUsers = string.Format(strSelectUsers, msg.ObjectID);
                    DataTable dtNotiUsers = DbHelperSQL.QueryDt(strSelectUsers);
                    if (dtNotiUsers == null || dtNotiUsers.Rows.Count == 0)
                    {
                        msg.ErrorMsg = "缺少预警的人";
                        return false;
                    }
                    int agentID = int.Parse(dtNotiUsers.Rows[0]["AgentID"].ToString());
                    string userIDs = dtNotiUsers.Rows[0]["UserIDs"].ToString();

                    if (msg.Format == WechatFormat.IMAGE) //图片
                    {
                        Siemens.Simatic.Wechat.Common.ReturnValue rv = api_wechat_bo.SendImage(agentID, msg.MsgSubject, msg.MsgContent, userIDs, false);
                        msg.ErrorMsg = "SendImage: Success=[" + rv.Success + "]" + ",Message=[" + rv.Message + "]";
                    }
                    else if (msg.Format == WechatFormat.TEXT) //文字
                    {
                        Siemens.Simatic.Wechat.Common.ReturnValue rv = api_wechat_bo.SendText(agentID, msg.MsgContent, userIDs, false);
                        msg.ErrorMsg = "SendText: Success=[" + rv.Success + "]" + ",Message=[" + rv.Message + "]";
                    }
                }

                //发送失败，更新message
                if (!string.IsNullOrEmpty(msg.ErrorMsg) && !msg.ErrorMsg.Contains("Success=[True],Message=[ok]"))
                {
                    string sql = @"UPDATE PM_ALT_MESSAGE SET ErrorMsg='{1}',SentCnt=SentCnt+1 WHERE MsgPK={0}";
                    sql = string.Format(sql, msg.MsgPK, msg.ErrorMsg);
                    DbHelperSQL.ExecuteSql(sql);
                    return false;
                }
                else //发送成功，移到历史表
                {
                    string sql = @"INSERT INTO PM_ALT_MESSAGE_HISTORY SELECT * FROM PM_ALT_MESSAGE WHERE MsgPK={0};
                                   DELETE FROM PM_ALT_MESSAGE WHERE MsgPK={0};";
                    sql = string.Format(sql, msg.MsgPK);
                    DbHelperSQL.ExecuteSql(sql);
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
           
        }

        /// <summary>
        /// 把DataTable转成Html Table
        /// </summary>
        /// <param name="dtEvents"></param>
        /// <returns></returns>
        public string BuildTable(DataTable dtEvents)
        {
            string contentBuilt = string.Empty;
            StringBuilder tableSb = new StringBuilder();
            tableSb.Append("<table  style=\"font-family: verdana,arial,sans-serif;font-size:12px;color=:#333333;border-width:1px;border-color:#666666;border-collapse:collapse\"");
            //写入标题
            tableSb.Append("<tr>");
            for (int j = 0; j < dtEvents.Columns.Count; j++)
            {
                tableSb.Append("<th style=\"border-width: 1px;padding: 2px;border-style: solid;border-color: #666666;background-color: #dedede;\">").Append(dtEvents.Columns[j].ColumnName).Append("</th>");
            }
            tableSb.Append("</tr>");

            //写入表格
            for (int i = 0; i < dtEvents.Rows.Count; i++) //行
            {
                tableSb.Append("<tr>");
                for (int j = 0; j < dtEvents.Columns.Count; j++)
                {
                    tableSb.Append("<td style=\"border-width: 1px;padding: 2px;border-style: solid;border-color: #666666;background-color: #ffffff;\">").Append(dtEvents.Rows[i][j].ToString()).Append("</td>");
                }
                tableSb.Append("</tr>");
            }
            tableSb.Append("</table>");

            contentBuilt = tableSb.ToString();
            return contentBuilt;
        }





    }
}
