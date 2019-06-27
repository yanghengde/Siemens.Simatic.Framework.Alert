using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace Siemens.Simatic.Service.NotifyService
{
    public partial class Service1 : ServiceBase
    {
        private ServiceController _serviceController;
        private MesLog _mesLog;

        public Service1()
        {
            InitializeComponent();
            //
            _mesLog = new MesLog();
        }

        static void Main()
        {
            //#if DEBUG
            //Service1 ServicesToDebug = new Service1();

            //ServicesToDebug.ProcessDataObjectItem();
            //#else
            System.ServiceProcess.ServiceBase[] ServicesToRun;

            ServicesToRun = new System.ServiceProcess.ServiceBase[] { new Service1() };

            System.ServiceProcess.ServiceBase.Run(ServicesToRun);
            //#endif
        }

        protected override void OnStart(string[] args)
        {
            this.ProcessDataObjectItem();
        }

        protected override void OnStop()
        {
            if (_serviceController != null)
            {
                while (!_serviceController.Stop())
                {
                    //do nothing until _serviceController stoped all services.
                }
            }
        }

        public bool ProcessDataObjectItem()
        {
            bool result = false;
            //
            IList<IService> services = new List<IService>();
            services.Add(new NotifyService(new MesLog("EmailService")));
            //
            _serviceController = new ServiceController(services, _mesLog);
            result = _serviceController.Run();
            //
            return result;
        }
    }
}
