using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Threading;
using System.Data;

using Siemens.Simatic.Platform.Core;
using Siemens.Simatic.Platform.Common;
using Siemens.Simatic.ALT.Common;
using Siemens.Simatic.ALT.BusinessLogic;
using log4net;

namespace Siemens.Simatic.Service.AlertService
{
    public abstract class MainService : IService
    {
        protected PM_ALT_BASE _alert;
        //
        protected IPM_ALT_BASEBO _PM_ALT_BASEBO;
       // protected ICV_PM_ALT_NOTIBO _CV_PM_ALT_NOTIBO;
        protected ICV_PM_EMAIL_NOTIBO _ICV_PM_EMAIL_NOTIBO;
        protected ICV_PM_WECHAT_NOTIBO _ICV_PM_WECHAT_NOTIBO;
        protected TimeSpan _loopPeriod;
        protected bool _loopFlag = true;
        protected IList<DateTime> _fixedTimers;
        private ILog log = LogManager.GetLogger(typeof(MainService));

        protected string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public MainService()
            : this("MesService")
        {
        }

        /// <summary>
        /// 频率设置
        /// </summary>
        /// <param name="name"></param>
        public MainService(string name)
        {
            Name = name;

            _PM_ALT_BASEBO = ObjectContainer.BuildUp<IPM_ALT_BASEBO>();
            _ICV_PM_EMAIL_NOTIBO = ObjectContainer.BuildUp<ICV_PM_EMAIL_NOTIBO>();
            _ICV_PM_WECHAT_NOTIBO = ObjectContainer.BuildUp<ICV_PM_WECHAT_NOTIBO>();
            //
            //1.load alert 
            _alert = _PM_ALT_BASEBO.GetEntity(name);
            if (_alert == null)
            {
                _loopPeriod = new TimeSpan(0, 0, 300);
            }
            else
            {
                if (_alert.AlertInterval.HasValue)
                {
                    _loopPeriod = new TimeSpan(0, 0, _alert.AlertInterval.Value);
                }
                else if (!string.IsNullOrEmpty(_alert.AlertTimePoints))
                {
                    _fixedTimers = new List<DateTime>();
                    foreach (string fixedTimer in _alert.AlertTimePoints.Split(','))
                    {
                        if (string.IsNullOrEmpty(fixedTimer.Trim())) 
                            continue;
                        //
                        _fixedTimers.Add(Convert.ToDateTime(string.Format("2000-01-01 {0}:00", fixedTimer.Trim())));
                    }
                    //
                    _loopPeriod = new TimeSpan(0, 0, 60);
                }
                else //没有设置频率，默认5分钟
                {
                    _loopPeriod = new TimeSpan(0, 0, 300);
                }
            }
        }

        protected abstract object GetReadyData();
        protected abstract bool Execute(object readyData);

        public virtual bool DoService()
        {
            //后续可能被删除
            _alert = _PM_ALT_BASEBO.GetEntity(_alert.AlertID.Value);
            if (_alert == null || _alert.RowDeleted == true || _alert.IsActive == false)
            {
                return true;
            }

            object readyData = this.GetReadyData();
            if (readyData != null)
            {
                try
                {
                    this.Execute(readyData);
                }
                finally
                {
                    GC.Collect();
                }
            }
            //
            return true;
        }

        /// <summary>
        /// 循环调用
        /// </summary>
        public virtual void Start()
        {
            //1.log start process
            log.Info("***** " + this.Name + " thread started! *****");

            //2.loop to upload
            _loopFlag = true;
            while (_loopFlag)
            {
                try
                {
                    if (_alert != null && !_alert.AlertInterval.HasValue && !string.IsNullOrEmpty(_alert.AlertTimePoints))
                    {
                        if (_fixedTimers != null)
                        {
                            DateTime curTime = DateTime.Now;//Siemens.Simatic.Util.Utilities.DAO.UtilDAO.GetDatabaseUtcDatetime().Value.AddHours(8);
                            foreach (DateTime fixedTime in _fixedTimers)
                            {
                                //_mesLog.LogInfo(this.Name + ":fixedTime.Hour=[" + fixedTime.Hour + "],curTime.Hour=[" + curTime.Hour + "],fixedTime.Minute=[" + fixedTime.Minute + "],curTime.Minute=[" + curTime.Minute + "]");
                                if (fixedTime.Hour == curTime.Hour && fixedTime.Minute == curTime.Minute)
                                {
                                    if (this.DoService())
                                    {
                                        log.Info(this.Name + " processed one round completely.");
                                    }
                                    //
                                    break;
                                }
                            }
                            //
                            Thread.Sleep(_loopPeriod);
                        }
                        else
                        {
                            Thread.Sleep(_loopPeriod);
                        }
                    }
                    else if (this.DoService())
                    {
                        log.Info(this.Name + " processed one round completely.");
                        Thread.Sleep(_loopPeriod);
                    }
                    //else
                    //{
                    //    Thread.Sleep(_loopPeriod);
                    //}
                }
                catch (Exception ex)
                {
                    log.Error(this.Name + ".DoService",ex);
                }
            }

            // time to end the thread
            if (Thread.CurrentThread.ThreadState != ThreadState.Aborted)
            {
                Thread.CurrentThread.Abort();
                log.Info("***** " + this.Name + " thread to abort! *****");
            }
        }

        public virtual void Stop()
        {
            _loopFlag = false;

            //1.log stop process
            log.Info("***** " + this.Name + " to stop! *****");
        }
    }
}
