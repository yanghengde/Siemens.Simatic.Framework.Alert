using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Threading;
using System.Data;

using Siemens.Simatic.Platform.Core;
using Siemens.Simatic.Platform.Common;

namespace Siemens.Simatic.Service.NotifyService
{
    public abstract class MainService : IService
    {
        protected TimeSpan _loopPeriod;
        protected bool _loopFlag = true;
        protected IList<DateTime> _fixedTimers; 

        protected string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        protected MesLog _mesLog;
        public MesLog MesLog
        {
            get { return _mesLog; }
            set { _mesLog = value; }
        }

        public MainService(MesLog mesLog)
            : this("MesService", mesLog)
        {
        }

        public MainService(string name, MesLog mesLog)
        {
            Name = name;
            _mesLog = mesLog;
            //
            string _loopSec = ConfigurationManager.AppSettings["loopPeriod"];
            try { 
                if (string.IsNullOrEmpty(_loopSec))
                {
                    _loopPeriod = new TimeSpan(0, 0, 10);
                }
                else
                {
                    _loopPeriod = new TimeSpan(0, 0, int.Parse(ConfigurationManager.AppSettings["loopPeriod"]));
                }
            }catch(Exception ex)
            {
                _loopPeriod = new TimeSpan(0, 0, 10);
            }
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
            _mesLog.LogInfo("***** " + this.Name + " thread started! *****");

            //2.loop to upload
            _loopFlag = true;
            while (_loopFlag)
            {
                try
                {
                    this.DoService();
                    _mesLog.LogInfo(this.Name + " processed one round completely.");
                    Thread.Sleep(_loopPeriod);

                }
                catch (Exception ex)
                {
                    _mesLog.LogException(ex.Message + Environment.NewLine + ex.StackTrace, this.Name + ".DoService");
                }
            }

            // time to end the thread
            if (Thread.CurrentThread.ThreadState != ThreadState.Aborted)
            {
                Thread.CurrentThread.Abort();
                _mesLog.LogInfo("***** " + this.Name + " thread to abort! *****");
            }
        }

        public virtual void Stop()
        {
            _loopFlag = false;

            //1.log stop process
            _mesLog.LogInfo("***** " + this.Name + " to stop! *****");
        }
    }
}
