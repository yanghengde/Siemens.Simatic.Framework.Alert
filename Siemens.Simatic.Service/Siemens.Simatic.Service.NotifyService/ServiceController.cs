using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Siemens.Simatic.Service.NotifyService
{
    public class ServiceController
    {
        private IList<Thread> _threads;
        private IList<IService> _services;
        private MesLog _dataLog;
        private TimeSpan _joinTimeSpan = new TimeSpan(0, 0, 5);

        public ServiceController(IList<IService> service, MesLog dataLog)
        {
            _threads = new List<Thread>();
            _services = service;
            _dataLog = dataLog;
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

                        _dataLog.LogInfo("***** " + service.Name + " started! *****");
                        //
                        _threads.Add(thread);
                    }
                    catch (Exception ex)
                    {
                        _dataLog.LogException(ex.Message + Environment.NewLine + ex.StackTrace, service.Name + ".Run");
                    }
                }
            }

            return true;
        }   

        public bool Stop()
        {
            foreach (Thread thread in _threads)
            {
                thread.Abort();
                _dataLog.LogInfo("***** " + thread.Name + " stoped! *****");
            }

            //
            return true;
        }
    }
}
