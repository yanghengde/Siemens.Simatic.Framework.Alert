using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

using Siemens.Simatic.Platform.Core;
using Siemens.Simatic.Platform.Common;
using Siemens.Simatic.ALT.Common;
using Siemens.Simatic.ALT.BusinessLogic;
using log4net;
using Newtonsoft.Json;

namespace Siemens.Simatic.Service.AlertService
{
    public class AlertService : MainService
    {
        private string _MessageToMembers;

        private IPM_ALT_MESSAGEBO _PM_ALT_MESSAGEBO;
        private ILog log = LogManager.GetLogger(typeof(AlertService));

        public AlertService()
            : this("AlertService")
        {
        }

        public AlertService(string name)
            : base(name)
        {
            _PM_ALT_MESSAGEBO = ObjectContainer.BuildUp<IPM_ALT_MESSAGEBO>();

        }

        protected override object GetReadyData()
        {
            this.GetMessageToList(_alert.AlertID.Value.ToString());

            //3.get triggered events
            DataTable dtAll = _PM_ALT_BASEBO.Run2(_alert.AlertID.Value.ToString());
            return dtAll;
        }

        protected override bool Execute(object readyData)
        {
            try
            {
                if (readyData == null) 
                    return false;
                //
                DataTable dtAll = readyData as DataTable;

                IList<PM_ALT_LOG> logs = new List<PM_ALT_LOG>();
                IList<PM_ALT_MESSAGE> messages = new List<PM_ALT_MESSAGE>();
                if (_alert.Format == "TABLE" || _alert.Format == "IMAGE")
                {
                    //fill Log Subject & Content
                    string logSubject = _alert.AlertContent;
                    if (!string.IsNullOrEmpty(logSubject))
                    {
                        logSubject = logSubject.Replace("@日期$", DateTime.Now.ToString("yyyy-MM-dd"));
                    }

                    string logContent = _alert.AlertDesc;
                    if (!string.IsNullOrEmpty(logContent))
                    {
                        logContent = _PM_ALT_BASEBO.BuildTable(_alert.AlertDesc, dtAll);
                    }

                    PM_ALT_MESSAGE msg = new PM_ALT_MESSAGE();
                    msg.MsgSubject = logSubject;
                    msg.MsgContent = logContent;
                    msg.MsgFrom = "MESAdmin";
                    msg.MsgTo = _MessageToMembers;
                    msg.Category = _alert.AlertName;
                    msg.MsgType = _alert.AlertType;
                    msg.Format = _alert.Format;
                    msg.ObjectID = _alert.AlertID;
                    msg.SentCnt = 0;
                    msg.RowDeleted = false;

                    messages.Add(msg);

                    PM_ALT_LOG log = new PM_ALT_LOG();
                    log.AlertID = _alert.AlertID;
                    log.LogTitle = logSubject; //_alert.AlertAlias;
                    log.LogContent = logContent;
                    log.NotifiedCnt = 1;
                    log.IsClosed = false;
                    log.RowDeleted = false;

                    logs.Add(log);
                }
                else for (int z = 0; z < dtAll.Rows.Count; z++)
                    {
                        //fill Log Subject & Content
                        string logSubject = _alert.AlertContent;

                        if (!string.IsNullOrEmpty(logSubject))
                        {
                            logSubject = logSubject.Replace("@日期$", DateTime.Now.ToString("yyyy-MM-dd"));
                        }


                        if (!string.IsNullOrEmpty(logSubject))
                        {
                            for (int i = 0; i < dtAll.Columns.Count; i++)
                            {
                                logSubject = logSubject.Replace("@" + dtAll.Columns[i].ColumnName + "$", dtAll.Rows[z][i].ToString());
                            }
                        }

                        string logURL = _alert.URL;
                        if (!string.IsNullOrEmpty(logURL))
                        {
                            for (int i = 0; i < dtAll.Columns.Count; i++)
                            {
                                logURL = logURL.Replace("@" + dtAll.Columns[i].ColumnName + "$", dtAll.Rows[z][i].ToString());
                            }
                        }

                        string logContent = _alert.AlertDesc;
                        if (!string.IsNullOrEmpty(logContent))
                        {
                          logContent = _PM_ALT_BASEBO.BuildContent(_alert.AlertDesc, dtAll, z);
                        }

                        PM_ALT_MESSAGE msg = new PM_ALT_MESSAGE();
                        msg.MsgSubject = logSubject; //_alert.AlertAlias;
                        msg.MsgContent = logContent;
                        msg.MsgFrom = "MESAdmin";
                        msg.MsgTo = _MessageToMembers;
                        msg.Category = _alert.AlertName;
                        msg.MsgType = _alert.AlertType;
                        msg.Format = _alert.Format;
                        msg.ObjectID = _alert.AlertID;
                        msg.SentCnt = 0;
                        msg.RowDeleted = false;
                        msg.URL = logURL;

                        messages.Add(msg);
                        //
                        //create log
                        PM_ALT_LOG log = new PM_ALT_LOG();
                        log.AlertID = _alert.AlertID;
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
                    log.Error("Exception: " + returnMessage + this.Name + ".SaveLogs()");
                    return false;
                }
               
                //
                _PM_ALT_MESSAGEBO.SaveBatch(messages, out returnMessage);
                if (!string.IsNullOrEmpty(returnMessage))
                {
                    log.Error("Exception: " + returnMessage + this.Name + ".SaveMessages()");
                    return false;
                }
                //
                if (!string.IsNullOrEmpty(_alert.PostProcedure))
                {
                    try
                    {
                        _PM_ALT_BASEBO.CallProcedure(_alert.PostProcedure);
                    }
                    catch (Exception ex)
                    {
                        log.Error(this.Name + ".CallPostProcedure()", ex);
                        return false;
                    }
                }
                //
                if (_alert != null)
                {
                    PM_ALT_BASE alert = new PM_ALT_BASE();
                    alert.AlertID = _alert.AlertID;
                    alert.LastAlertedTime = Siemens.Simatic.Util.Utilities.DAO.UtilDAO.GetDatabaseUtcDatetime().Value.AddHours(8);
                    _PM_ALT_BASEBO.UpdateSome(alert);
                }
            }
            catch (Exception ex)
            {
                log.Error(this.Name + ".Execute()", ex);
                return false;
            }
            //
            return true;
        }

        private string GetMessageToList(string alertID)
        {
            _MessageToMembers = _PM_ALT_BASEBO.GetEmailToList(alertID);
            return _MessageToMembers = _MessageToMembers.TrimEnd(',');
        }
    }
}
