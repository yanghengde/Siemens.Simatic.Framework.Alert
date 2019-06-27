using log4net;
using Siemens.Simatic.ALT.BusinessLogic;
using Siemens.Simatic.Platform.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;

namespace Siemens.Simatic.Service.NotifyService
{
    /// <summary>
    /// 发送报警消息
    /// </summary>
    public partial class NotifyService : ServiceBase
    {
        //private ServiceController _serviceController;
        private ILog log = LogManager.GetLogger(typeof(NotifyService));
        public IALT_BSC_BO alt_BSC_BO;

        bool doAlert = false;
        bool doNotify= false;
        protected int Alert_Interval;
        protected int Notify_Interval;

        public NotifyService()
        {
            InitializeComponent();
            alt_BSC_BO = ObjectContainer.BuildUp<IALT_BSC_BO>();

            //Alert_Interval = Convert.ToInt32(ConfigurationSettings.AppSettings["Alert_Interval"]);
            //this.tmrAlert.Interval = Alert_Interval;
            Notify_Interval = Convert.ToInt32(ConfigurationSettings.AppSettings["Notify_Interval"]);            
            this.tmrNotify.Interval = Notify_Interval;

            //alt_BSC_BO.ExecuteNotifyAll();
            //return;

            this.tmrNotify.Start();
            Thread.Sleep(1000);
        }

        static void Main()
        {
            //NotifyService ServicesToDebug = new NotifyService();
            //ServicesToDebug.ProcessDataObjectItem();

            System.ServiceProcess.ServiceBase[] ServicesToRun;
            ServicesToRun = new System.ServiceProcess.ServiceBase[] { new NotifyService() };
            System.ServiceProcess.ServiceBase.Run(ServicesToRun);
        }

        protected override void OnStart(string[] args)
        {
            //this.ProcessDataObjectItem();

            log.Info("OnStart");
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
            services.Add(new MsgService());
            //
            _serviceController = new ServiceController(services);
            result = _serviceController.Run();
            //
            return result;
        }

    }
}
