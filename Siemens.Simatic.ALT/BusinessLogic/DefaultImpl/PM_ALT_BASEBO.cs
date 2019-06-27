
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

using Siemens.Simatic.Platform.Core;
using Siemens.Simatic.Platform.Common;
using Siemens.Simatic.Platform.Common.ExceptionHandling;
using Siemens.Simatic.Platform.Data;

using Siemens.Simatic.Right.BusinessLogic;
using Siemens.Simatic.Util.Utilities.DAO;
using Siemens.Simatic.ALT.Common;
using Siemens.Simatic.ALT.DataAccess;
using Siemens.Simatic.Util.Utilities;

namespace Siemens.Simatic.ALT.BusinessLogic.DefaultImpl
{
    public class PM_ALT_BASEBO : IPM_ALT_BASEBO
    {
        private IPM_ALT_BASEDAO _PM_ALT_BASEDAO;
        private IPM_ALT_NOTIBO _PM_ALT_NOTIBO;
        private ICV_PM_EMAIL_NOTIBO cv_PM_EMAIL_NOTIBO = ObjectContainer.BuildUp<ICV_PM_EMAIL_NOTIBO>();
        private IPM_ALT_MESSAGEBO pm_ALT_MESSAGEBO = ObjectContainer.BuildUp<IPM_ALT_MESSAGEBO>();
        private IALT_BSC_BO alt_BSC_BO = ObjectContainer.BuildUp<IALT_BSC_BO>();

        public PM_ALT_BASEBO()
        {
            _PM_ALT_BASEDAO = ObjectContainer.BuildUp<IPM_ALT_BASEDAO>();
            _PM_ALT_NOTIBO = ObjectContainer.BuildUp<IPM_ALT_NOTIBO>();
        }


        #region base interface impl

        public PM_ALT_BASE Insert(PM_ALT_BASE entity)
        {
            PM_ALT_BASE newEntity = null;

            try
            {
                ArgumentValidator.CheckForNullArgument(entity, "PM_ALT_BASE Entity");

                newEntity = _PM_ALT_BASEDAO.Insert(entity);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }

            return newEntity;
        }

        public void Delete(PM_ALT_BASE entity)
        {
            try
            {
                ArgumentValidator.CheckForNullArgument(entity, "PM_ALT_BASE Entity");

                _PM_ALT_BASEDAO.Delete(entity.AlertID);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }
        }

        public void Delete(Guid entityGuid)
        {
            try
            {
                ArgumentValidator.CheckForNullArgument(entityGuid, "PM_ALT_BASE Guid");

                _PM_ALT_BASEDAO.Delete(entityGuid);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }
        }

        public void Update(PM_ALT_BASE entity)
        {
            try
            {
                ArgumentValidator.CheckForNullArgument(entity, "PM_ALT_BASE Entity");

                _PM_ALT_BASEDAO.Update(entity);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }
        }

        public void UpdateSome(PM_ALT_BASE entity)
        {
            try
            {
                ArgumentValidator.CheckForNullArgument(entity, "PM_ALT_BASE Entity");

                _PM_ALT_BASEDAO.Update(entity, false);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }
        }

        public PM_ALT_BASE GetEntity(Guid entityGuid)
        {
            PM_ALT_BASE entity = null;

            try
            {
                ArgumentValidator.CheckForNullArgument(entityGuid, "PM_ALT_BASE Guid");

                entity = _PM_ALT_BASEDAO.Get(entityGuid);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }

            return entity;
        }

        public IList<PM_ALT_BASE> GetAll()
        {
            long totalRecords = 0;
            IList<PM_ALT_BASE> entities = null;

            try
            {
                entities = _PM_ALT_BASEDAO.Find(0, -1, null, null, out totalRecords);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }

            return entities;
        }

        #endregion base interface impl



        public PM_ALT_BASE GetEntityByID(string alertID)
        {
            long totalRecords = 0;
            IList<PM_ALT_BASE> entities = null;

            try
            {
                AndFilter af = new AndFilter();
                MatchingFilter mf = new MatchingFilter();
                mf.AddMatching("AlertID", alertID);
                mf.AddMatching("RowDeleted", false);
                af.AddFilter(mf);

                entities = _PM_ALT_BASEDAO.Find(0, -1, af, null, out totalRecords);
                if (entities.Count > 0)
                    return entities[0];
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }

            return null;
        }

        public PM_ALT_BASE GetEntity(string alertName)
        {
            long totalRecords = 0;
            IList<PM_ALT_BASE> entities = null;

            try
            {
                AndFilter af = new AndFilter();
                MatchingFilter mf = new MatchingFilter();
                mf.AddMatching("AlertName", alertName);
                mf.AddMatching("RowDeleted", false);
                af.AddFilter(mf);

                entities = _PM_ALT_BASEDAO.Find(0, -1, af, null, out totalRecords);
                if (entities.Count > 0)
                    return entities[0];
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }

            return null;
        }

        public IList<PM_ALT_BASE> GetActiveEntities()
        {
            long totalRecords = 0;
            IList<PM_ALT_BASE> entities = null;

            try
            {
                AndFilter af = new AndFilter();
                MatchingFilter mf = new MatchingFilter();
                mf.AddMatching("IsActive", true);
                mf.AddMatching("RowDeleted", false);
                af.AddFilter(mf);

                entities = _PM_ALT_BASEDAO.Find(0, -1, af, null, out totalRecords);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }

            return entities;
        }

        /// <summary>
        /// 获取所有预警
        /// </summary>
        /// <returns></returns>
        public IList<PM_ALT_BASE> GetAllEntities()
        {
            long totalRecords = 0;
            IList<PM_ALT_BASE> entities = null;

            try
            {
                AndFilter af = new AndFilter();
                MatchingFilter mf = new MatchingFilter();
                mf.AddMatching("RowDeleted", false);
                af.AddFilter(mf);

                entities = _PM_ALT_BASEDAO.Find(0, -1, af, null, out totalRecords);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }

            return entities;
        }

        public void Save(PM_ALT_BASE entity, IList<PM_ALT_NOTI> notis, AlertSaveOptions saveOptions,
            out string returnMessage)
        {
            returnMessage = string.Empty;
            //
            try
            {
                DateTime? datetime = UtilDAO.GetDatabaseUtcDatetime().Value.AddHours(8);
                //
                #region check
                #region check basic
                if (saveOptions == null)
                {
                    returnMessage = "Parameter [SaveOptions saveOptions] can not be null.";
                    return;
                }
                if (entity == null)
                {
                    returnMessage = "Input parameter [ALERT entity] can not be null.";
                    return;
                }
                if (!entity.AlertID.HasValue)
                {
                    returnMessage = "Input parameter [ALERT.AlertID] can not be null.";
                    return;
                }
                if (saveOptions.IsChangeNoti)
                {
                    if (notis == null)
                    {
                        returnMessage = "Input parameter [IList<ALERT_NOTI> notis] can not be null.";
                        return;
                    }
                }
                //
                SSIdentity identity = SSAuthentication.CurrentIdentity as SSIdentity;
                if (identity == null)
                {
                    returnMessage = "[Authentication.Identity] is required.";
                    return;
                }
                #endregion
                //
                #region check alert
                if (saveOptions.IsChangeAlert)
                {
                    if (string.IsNullOrEmpty(entity.AlertName))
                    {
                        returnMessage = "[ALERT Name] is required.";
                        return;
                    }
                    if (string.IsNullOrEmpty(entity.AlertAlias))
                    {
                        returnMessage = "[ALERT Alias] is required.";
                        return;
                    }
                    if (string.IsNullOrEmpty(entity.AlertObject))
                    {
                        returnMessage = "[ALERT Object] is required.";
                        return;
                    }

                    if (!_PM_ALT_BASEDAO.CheckDatabaseObject(entity.AlertObject))
                    {
                        returnMessage = "[ALERT Object] is invalid.";
                        return;
                    }
                    //
                    if (!entity.AlertID.HasValue)
                    {
                        entity.AlertID = Guid.NewGuid();
                        entity.CreatedBy = identity.Name;
                        entity.CreatedOn = datetime;
                    }
                    else
                    {
                        entity.ModifiedBy = identity.Name;
                        entity.ModifiedOn = datetime;
                    }
                    //
                    if (!entity.RowDeleted.HasValue)
                    {
                        entity.RowDeleted = false;
                    }
                    if (string.IsNullOrEmpty(entity.CreatedBy))
                    {
                        entity.CreatedBy = identity.Name;
                    }
                    if (!entity.CreatedOn.HasValue)
                    {
                        entity.CreatedOn = datetime;
                    }
                }
                #endregion
                //

                #region check noti
                if (saveOptions.IsChangeNoti)
                {

                }
                #endregion

                #endregion


                //
                #region save
                //
                #region save alert
                if (saveOptions.IsChangeAlert)
                {
                    PM_ALT_BASE entityExisted = this.GetEntity(entity.AlertID.Value);
                    if (entityExisted == null)
                    {
                        if (null != this.GetEntity(entity.AlertName))
                        {
                            returnMessage = "The item with the same code has existed.";
                            return;
                        }
                        //
                        entity.ModifiedBy = null;
                        entity.ModifiedOn = null;
                        //
                        this.Insert(entity);
                    }
                    else
                    {
                        this.UpdateSome(entity);
                    }
                }
                #endregion
                //

                #region save noti
                if (saveOptions.IsChangeNoti)
                {
                    _PM_ALT_NOTIBO.SaveBatch(entity.AlertID.Value, notis, out returnMessage);
                    //
                    if (!string.IsNullOrEmpty(returnMessage))
                    {
                        return;
                    }
                }
                #endregion

                #endregion
            }
            catch (Exception ex)
            {
                returnMessage = ex.Message;
                return;
            }
        }

        public void Remove(PM_ALT_BASE entity, out string returnMessage)
        {
            returnMessage = string.Empty;
            //
            try
            {
                DateTime? datetime = UtilDAO.GetDatabaseUtcDatetime().Value.AddHours(8);
                //
                if (entity == null)
                {
                    returnMessage = "Input parameter [ALERT entity] can not be null.";
                    return;
                }
                if (!entity.AlertID.HasValue)
                {
                    returnMessage = "[ALERT ID] can not be null.";
                    return;
                }
                //
                entity.RowDeleted = true;
                entity.ModifiedOn = datetime;

                this.UpdateSome(entity);
            }
            catch (Exception ex)
            {
                returnMessage = ex.Message;
                return;
            }
        }

        public void Restore(PM_ALT_BASE entity, out string returnMessage)
        {
            returnMessage = string.Empty;
            //
            try
            {
                DateTime? datetime = UtilDAO.GetDatabaseUtcDatetime().Value.AddHours(8);
                //
                if (entity == null)
                {
                    returnMessage = "Input parameter [ALERT entity] can not be null.";
                    return;
                }
                if (!entity.AlertID.HasValue)
                {
                    returnMessage = "[ALERT ID] can not be null.";
                    return;
                }

                #region check security
                SSIdentity identity = SSAuthentication.CurrentIdentity as SSIdentity;
                if (identity == null)
                {
                    returnMessage = "[Authentication.Identity] is required.";
                    return;
                }
                #endregion
                //
                entity.ModifiedBy = identity.Name;
                entity.ModifiedOn = datetime;
                entity.RowDeleted = false;

                this.UpdateSome(entity);
            }
            catch (Exception ex)
            {
                returnMessage = ex.Message;
                return;
            }
        }
        
        /// <summary>
        /// 触发--查询预警数据
        /// </summary>
        /// <param name="alert"></param>
        /// <returns></returns>
        public DataTable Run2(string alertID)
        {
            //前置存储过程
            //if (!string.IsNullOrEmpty(alert.PreProcedure))
            //{
            //    this.CallProcedure(alert.PreProcedure);
            //}

            PM_ALT_BASE alert = GetEntityByID(alertID);

            DataTable dtResult = new DataTable();            
            if (string.IsNullOrEmpty(alert.SqlScript))
            {
                return null;
            }
            else
            {
                dtResult = _PM_ALT_BASEDAO.Run(alert.SqlScript);
                return dtResult;
            }
        }

        public string BuildContent(string contentTemplate, DataTable dtEvents,int rowIndex)
        {
            string contentBuilt = contentTemplate;

            bool hasIterBlock = false;

            bool hasIterBlockBegin = contentTemplate.ToUpper().Contains("#DETAILBEGIN");

            bool hasIterBlockEnd = contentTemplate.ToUpper().Contains("#DETAILEND");

            hasIterBlock = hasIterBlockBegin & hasIterBlockEnd;
            //
            int indexBlockBegin = -1;
            int indexBlockEnd = -1;
            string blockTemplate = string.Empty;

            if (hasIterBlock)
            {
                indexBlockBegin = contentTemplate.ToUpper().IndexOf("#DETAILBEGIN");
                indexBlockEnd = contentTemplate.ToUpper().IndexOf("#DETAILEND");
                blockTemplate = contentTemplate.Substring(indexBlockBegin + 12, indexBlockEnd - indexBlockBegin - 12).TrimEnd('\n').TrimEnd('\r');
            }
            else
            {
                return "格式不正确：[预警内容]里缺少#DetailBegin或者#DetailEnd";
            }
            //
            if (string.IsNullOrEmpty(contentTemplate)) return contentTemplate;

            //handle non-details
            for (int i = 0; i < dtEvents.Columns.Count; i++)
            {
                contentBuilt = contentBuilt.Replace("@" + dtEvents.Columns[i].ColumnName + "$", dtEvents.Rows[rowIndex][i].ToString());
            }

            if (hasIterBlock)
            {
                //delete template block
                contentBuilt = contentBuilt.Replace("#DetailBegin\n\r#DetailEnd", "").Replace("#DetailBegin", "").Replace("#DetailEnd", "");
            }
           
            //
            return contentBuilt;
        }

        /// <summary>
        /// 生成表格
        /// </summary>
        public string BuildContent2(string contentTitle, DataTable dtAggr, int rowIndex)
        {
            //string contentBuilt = contentTitle + "\r\n";
            //for (int i=0; i < dtAggr.Columns.Count; i++)
            //{ 
            //    string colName = dtAggr.Columns[i].ColumnName;
            //    string value = dtAggr.Rows[rowIndex][i].ToString();
            //    contentBuilt = contentBuilt + colName + "【" + value + "】 ";
            //}
            //return contentBuilt;

            string contentBuilt = "";
            contentBuilt = contentTitle + dtAggr.Rows[rowIndex][0].ToString();
            return contentBuilt;
        }

        public string BuildTable(string contentTemplate, DataTable dtEvents)
        {
            string contentBuilt = contentTemplate;
            
            bool hasIterBlock = false;

            bool hasIterBlockBegin = contentTemplate.ToUpper().Contains("#DETAILBEGIN");

            bool hasIterBlockEnd = contentTemplate.ToUpper().Contains("#DETAILEND");

            hasIterBlock = hasIterBlockBegin & hasIterBlockEnd;
            //
            int indexBlockBegin = -1;
            int indexBlockEnd = -1;
            string blockTemplate = string.Empty;

            if (hasIterBlock)
            {
                indexBlockBegin = contentTemplate.ToUpper().IndexOf("#DETAILBEGIN");
                indexBlockEnd = contentTemplate.ToUpper().IndexOf("#DETAILEND");
                blockTemplate = contentTemplate.Substring(indexBlockBegin + 12, indexBlockEnd - indexBlockBegin - 12).TrimEnd('\n').TrimEnd('\r');
            }
            else
            {
                return "格式不正确：[预警内容]里缺少#DetailBegin或者#DetailEnd";
            }
            //
            if (string.IsNullOrEmpty(contentTemplate)) return null;

            StringBuilder tableSb = new StringBuilder();
            tableSb.Append("<table  style=\"font-family: verdana,arial,sans-serif;font-size:12px;color:#333333;border-width:1px;border-color:#666666;border-collapse:collapse\">");
            //写入标题
            tableSb.Append("<tr>");
            for (int j = 0; j < dtEvents.Columns.Count; j++)
            {
                if (blockTemplate.IndexOf("@" + dtEvents.Columns[j].ColumnName+"$") > -1)
                {
                    tableSb.Append("<th style=\"border-width: 1px;padding: 2px;border-style: solid;border-color: #666666;background-color: #dedede;\">").Append(dtEvents.Columns[j].ColumnName).Append("</th>");
                }
            }
            tableSb.Append("</tr>");

            //写入表格
            for (int i = 0; i < dtEvents.Rows.Count; i++) //行
            {
                tableSb.Append("<tr>");
                for (int j = 0; j < dtEvents.Columns.Count; j++)
                {
                    if (blockTemplate.IndexOf("@" + dtEvents.Columns[j].ColumnName + "$") > -1)
                    {
                        tableSb.Append("<td style=\"border-width: 1px;padding: 2px;border-style: solid;border-color: #666666;background-color: #ffffff;\">").Append(dtEvents.Rows[i][j].ToString()).Append("</td>");
                    }
                }
                tableSb.Append("</tr>");
            }
            tableSb.Append("</table>");

            //replace template block to table
            contentBuilt = contentBuilt.Replace(blockTemplate, tableSb.ToString()).Replace("#DetailBegin\n\r#DetailEnd", "").Replace("#DetailBegin", "").Replace("#DetailEnd", "");
            //
            return contentBuilt;
        }

        public string BuildTable2(string contentTitle, DataTable dtEvents)
        {
            string contentBuilt = ""; //contentTemplate;

            if(dtEvents == null || dtEvents.Rows.Count == 0)
            {
                return contentBuilt;
            }

            StringBuilder tableSb = new StringBuilder();
            tableSb.Append("<P>" + contentTitle + "</P>");
            tableSb.Append("<table  style=\"font-family: verdana,arial,sans-serif;font-size:12px;color:#333333;border-width:1px;border-color:#666666;border-collapse:collapse\">");

            //写入标题行
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

        public bool CallProcedure(string procName)
        {
            return _PM_ALT_BASEDAO.CallProcedure(procName);
        }

        /// <summary>
        /// 复制预警配置
        /// </summary>
        /// <param name="newAlertName"></param>
        /// <param name="oldAlertName"></param>
        /// <param name="updatedBy"></param>
        /// <param name="sb"></param>
        /// <returns></returns>
        public bool DuplicateAlert(string newAlertName, string oldAlertName, string updatedBy, ref string sb)
        {
            string msg = _PM_ALT_BASEDAO.DuplicateAlert(newAlertName, oldAlertName, updatedBy);
            if (string.IsNullOrEmpty(msg) || msg == null)
            {
                sb = "执行Alert复制异常"; 
                return false;
            }
            else if (msg.Substring(0, 2) == "OK")
            {
                sb = msg; 
                return true;
            }
            else if (msg.Substring(0, 2) == "NG")
            {
                sb = msg; 
                return false;
            }
            return true;
        }

        /// <summary>
        /// 插入消息表
        /// </summary>
        /// <returns></returns>
        public string Trigger(DataTable dtTrigger, PM_ALT_BASE alertEntity)
        {
            try
            {
                if (dtTrigger == null || dtTrigger.Rows.Count == 0)
                {
                    return "没有执行内容";
                }

                IList<PM_ALT_LOG> logs = new List<PM_ALT_LOG>();
                IList<PM_ALT_MESSAGE> messages = new List<PM_ALT_MESSAGE>();
                if (alertEntity.Format == "TABLE" || alertEntity.Format == "IMAGE")
                {
                    //fill Log Subject & Content
                    string logSubject = alertEntity.AlertContent;
                    if (!string.IsNullOrEmpty(logSubject))
                    {
                        logSubject = logSubject.Replace("@日期$", DateTime.Now.ToString("yyyy-MM-dd"));
                    }

                    string logContent = alertEntity.AlertDesc;
                    if (!string.IsNullOrEmpty(logContent))
                    {
                        logContent = this.BuildTable(alertEntity.AlertDesc, dtTrigger);
                    }

                    PM_ALT_MESSAGE msg = new PM_ALT_MESSAGE();
                    msg.MsgSubject = logSubject;
                    msg.MsgContent = logContent;
                    msg.MsgFrom = "MESAdmin";
                    msg.MsgTo = this.GetEmailToList(alertEntity.AlertID.ToString());
                    msg.Category = alertEntity.AlertName;
                    msg.MsgType = alertEntity.AlertType;
                    msg.Format = alertEntity.Format;
                    msg.ObjectID = alertEntity.AlertID;
                    msg.SentCnt = 0;
                    msg.RowDeleted = false;

                    messages.Add(msg);

                    PM_ALT_LOG log = new PM_ALT_LOG();
                    log.AlertID = alertEntity.AlertID;
                    log.LogTitle = logSubject; //_alert.AlertAlias;
                    log.LogContent = logContent;
                    log.NotifiedCnt = 1;
                    log.IsClosed = false;
                    log.RowDeleted = false;

                    logs.Add(log);
                }
                else for (int z = 0; z < dtTrigger.Rows.Count; z++)
                    {
                        //fill Log Subject & Content
                        string logSubject = alertEntity.AlertContent;

                        if (!string.IsNullOrEmpty(logSubject))
                        {
                            logSubject = logSubject.Replace("@日期$", DateTime.Now.ToString("yyyy-MM-dd"));
                        }


                        if (!string.IsNullOrEmpty(logSubject))
                        {
                            for (int i = 0; i < dtTrigger.Columns.Count; i++)
                            {
                                logSubject = logSubject.Replace("@" + dtTrigger.Columns[i].ColumnName + "$", dtTrigger.Rows[z][i].ToString());
                            }
                        }

                        string logContent = alertEntity.AlertDesc;
                        if (!string.IsNullOrEmpty(logContent))
                        {
                            logContent = this.BuildContent(alertEntity.AlertDesc, dtTrigger, z);
                        }

                        string logURL = alertEntity.URL;
                        if (!string.IsNullOrEmpty(logURL))
                        {
                            for (int i = 0; i < dtTrigger.Columns.Count; i++)
                            {
                                logURL = logURL.Replace("@" + dtTrigger.Columns[i].ColumnName + "$", dtTrigger.Rows[z][i].ToString());
                            }
                        }

                        PM_ALT_MESSAGE msg = new PM_ALT_MESSAGE();
                        msg.MsgSubject = logSubject; //_alert.AlertAlias;
                        msg.MsgContent = logContent;
                        msg.MsgFrom = "MESAdmin";
                        msg.MsgTo = this.GetEmailToList(alertEntity.AlertID.ToString());
                        msg.Category = alertEntity.AlertName;
                        msg.MsgType = alertEntity.AlertType;
                        msg.Format = alertEntity.Format;
                        msg.ObjectID = alertEntity.AlertID;
                        msg.URL = logURL;
                        msg.SentCnt = 0;
                        msg.RowDeleted = false;

                        messages.Add(msg);
                        //
                        //create log
                        PM_ALT_LOG log = new PM_ALT_LOG();
                        log.AlertID = alertEntity.AlertID;
                        log.LogTitle = logSubject; //_alert.AlertAlias;
                        log.LogContent = logContent;
                        log.NotifiedCnt = 1;
                        log.IsClosed = false;
                        log.RowDeleted = false;

                        logs.Add(log);
                    }
                //
                string returnMessage = string.Empty;
                if (!string.IsNullOrEmpty(returnMessage))
                {
                    return "Exception: " + returnMessage + ".SaveLogs()";
                }

                //
                pm_ALT_MESSAGEBO.SaveBatch(messages, out returnMessage);
                if (!string.IsNullOrEmpty(returnMessage))
                {
                    return "Exception: " + returnMessage + ".SaveEmails()";
                }

                //
                if (alertEntity != null)
                {
                    PM_ALT_BASE alert = new PM_ALT_BASE();
                    alert.AlertID = alertEntity.AlertID;
                    alert.LastAlertedTime = Siemens.Simatic.Util.Utilities.DAO.UtilDAO.GetDatabaseUtcDatetime().Value.AddHours(8);
                    this.UpdateSome(alert);
                }
                return "生成预警成功";
            }
            catch(Exception ex) {
                return "生成预警失败";
            }
        }


        /// <summary>
        /// 插入消息表-并发送
        /// </summary>
        /// <param name="dtTrigger"></param>
        /// <param name="alertEntity"></param>
        /// <returns></returns>
        public string TriggerAndSend(DataTable dtTrigger, PM_ALT_BASE alertEntity)
        {
            if (dtTrigger == null || dtTrigger.Rows.Count == 0)
            {
                return "没有执行内容";
            }
            //IList<PM_ALT_SCALE> scales = new List<PM_ALT_SCALE>();
            //IList<PM_ALT_LOG> logs = new List<PM_ALT_LOG>();
            IList<PM_ALT_MESSAGE> messageList = new List<PM_ALT_MESSAGE>();

            if (alertEntity.Format == "TABLE" || alertEntity.Format == "IMAGE")
            {
                string logSubject = alertEntity.AlertName; //预警标题
                logSubject = logSubject.Replace("@日期", SSGlobalConfig.Now.ToString("yyyy-MM-dd"));

                string logContent = "";
                logContent = this.BuildTable2(logSubject + "\r\n", dtTrigger);

                PM_ALT_MESSAGE email = new PM_ALT_MESSAGE();
                email.MsgSubject = logSubject; //_alert.AlertAlias;
                email.MsgContent = logContent;
                email.MsgFrom = "MESAdmin";
                email.MsgTo = this.GetEmailToList(alertEntity.AlertID.Value.ToString());
                email.Category = alertEntity.Category;
                email.MsgType = alertEntity.AlertType;
                email.Format = alertEntity.Format;
                email.ObjectID = alertEntity.AlertID;
                email.SentCnt = 0;
                email.RowDeleted = false;
                messageList.Add(email);

                //PM_ALT_LOG log = new PM_ALT_LOG();
                //log.AlertID = alertEntity.AlertID;
                //log.LogTitle = logSubject; //_alert.AlertAlias;
                //log.LogContent = logContent;
                //log.NotifiedCnt = 1;
                //log.IsClosed = false;
                //log.RowDeleted = false;
                //logs.Add(log);
            }
            else if (alertEntity.Format == "TEXT")  //文本--作废
            {
                for (int z = 0; z < dtTrigger.Rows.Count; z++) //每行数据发一次
                {
                    string logSubject = alertEntity.AlertName;

                    string logContent = ""; //alertEntity.AlertDesc;   
                    logContent = this.BuildContent2(logSubject + "\r\n", dtTrigger, z);//logContent.Replace("@" + dtEvents.Columns[i].ColumnName, dtEvents.Rows[z][i].ToString());

                    PM_ALT_MESSAGE email = new PM_ALT_MESSAGE();
                    email.MsgSubject = logSubject; //_alert.AlertAlias;
                    email.MsgContent = logContent;
                    email.MsgFrom = "MESAdmin";
                    email.MsgTo = this.GetEmailToList(alertEntity.AlertID.ToString());
                    email.Category = alertEntity.Category;
                    email.MsgType = alertEntity.AlertType;
                    email.Format = alertEntity.Format;
                    email.ObjectID = alertEntity.AlertID;
                    email.SentCnt = 0;
                    email.RowDeleted = false;

                    messageList.Add(email);
                } //end for
            }

            string returnMessage = string.Empty;

            //if (scales != null)
            //{
            //    PM_ALT_SCALEBO.SaveBatch(scales, out returnMessage);
            //    if (!string.IsNullOrEmpty(returnMessage))
            //    {
            //        SSMessageBox.ShowError(returnMessage);
            //        return returnMessage;
            //    }
            //}

            //保存预警消息
            //pm_ALT_MESSAGEBO.SaveBatch(messageList, out returnMessage);
            //if (!string.IsNullOrEmpty(returnMessage))
            //{
            //    return returnMessage;
            //}
            foreach (PM_ALT_MESSAGE msg in messageList)
            {
                string insertSql = @"INSERT INTO PM_ALT_MESSAGE(MsgSubject,MsgContent,MsgType,Format,ObjectID,MsgFrom,MsgTo,Category,SentCnt,ModifiedOn,RowDeleted)
                SELECT '{0}','{1}',alt.AlertType,alt.Format,alt.AlertID,'MESAdmin',
                STUFF((SELECT ','+u.Email FROM PM_ALT_NOTI t JOIN PM_WECHAT_USER u ON t.UserGuid=u.UserGuid WHERE t.AlertID=alt.AlertID AND u.Email<>'' for xml path('') ),1,1,'' ) AS MsgTo,alt.Category,0,GETDATE(),0 
                FROM PM_ALT_BASE as alt WHERE alertID='{2}';select @@identity ";
                insertSql = string.Format(insertSql,msg.MsgSubject, msg.MsgContent, msg.ObjectID);
                DataTable dt = alt_BSC_BO.GetDataTableBySql(insertSql);
                if (dt == null || dt.Rows.Count == 0)
                {
                    return "插入数据失败";
                }
                else
                {
                    msg.MsgPK = int.Parse(dt.Rows[0][0].ToString());
                    alt_BSC_BO.ExecuteNotify(msg); //发送消息
                }                
            }
            return "生成预警成功";
        }

        /// <summary>
        /// 获取接收预警的人员email
        /// </summary>
        /// <returns></returns>
        public string GetEmailToList(string AlertID)
        {
            string toList = string.Empty;
            IList<CV_PM_EMAIL_NOTI> emailList = GetEmails(AlertID);

            foreach (CV_PM_EMAIL_NOTI member in emailList)
            {
                if (string.IsNullOrEmpty(member.Email))
                    continue;

                toList += member.Email + ",";
            }
            return toList.TrimEnd(',');
        }

        public IList<CV_PM_EMAIL_NOTI> GetEmails(string AlertID)
        {
            return cv_PM_EMAIL_NOTIBO.GetEntity(AlertID);
        }

    }
}