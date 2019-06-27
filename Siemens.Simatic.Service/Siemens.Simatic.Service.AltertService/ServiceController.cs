using log4net;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Siemens.Simatic.Service.AlertService
{
    public class ServiceController
    {
        private IList<Thread> _threads;
        private IList<IService> _services;
        private TimeSpan _joinTimeSpan = new TimeSpan(0, 0, 5);
        private ILog log = LogManager.GetLogger(typeof(ServiceController));

        public ServiceController(IList<IService> service)
        {
            _threads = new List<Thread>();
            _services = service;
        }

        public bool Run()
        {
            if (_services != null && _services.Count > 0)
            {
                foreach (IService service in _services)
                {
                    try
                    {
                        Thread thread = new Thread(new ThreadStart(service.Start));
                        thread.Name = service.Name;
                        thread.Start();

                        log.Info("***** " + service.Name + " started! *****");
                        //
                        _threads.Add(thread);
                    }
                    catch (Exception ex)
                    {
                        log.Error(service.Name + ".Run",ex);
                    }
                }
            }

            return true;
        }

        //public bool Stop()
        //{
        //    //stop uploader
        //    foreach (IService service in _services)
        //    {
        //        service.Stop();
        //    }

        //    //block main thread to confirm every service thread aborted completely.
        //    foreach (Thread thread in _threads)
        //    {
        //        thread.Join();
        //        _dataLog.LogInfo("***** " + thread.Name + " stoped! *****");
        //    }

        //    //
        //    return true;
        //}

        public bool Stop()
        {
            //block main thread to confirm every service thread aborted completely.
            foreach (Thread thread in _threads)
            {
                thread.Abort();
                log.Info("***** " + thread.Name + " stoped! *****");
            }

            //
            return true;
        }
    }
}
