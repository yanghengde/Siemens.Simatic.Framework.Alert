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

namespace Siemens.Simatic.Service.NotifyService
{
    public abstract class MainService : IService
    {
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

        public MainService(string name)
        {
            Name = name;
            _loopPeriod = new TimeSpan(0, 0, 5);

        }

        protected abstract object GetReadyData();
        protected abstract bool Execute(object readyData);

        public virtual bool DoService()
        {
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
                    if (false)
                    {
                        if (_fixedTimers != null)
                        {
                            DateTime curTime = DateTime.Now;
                            foreach (DateTime fixedTime in _fixedTimers)
                            {
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
