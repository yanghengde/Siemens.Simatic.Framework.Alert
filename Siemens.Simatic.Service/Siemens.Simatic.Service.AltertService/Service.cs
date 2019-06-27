using System;
using System.Configuration;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;

using Siemens.Simatic.Platform.Core;
using Siemens.Simatic.Platform.Common;
using Siemens.Simatic.Right.BusinessLogic;
using Siemens.Simatic.Service.AlertService;
using Siemens.Simatic.ALT.Common;
using Siemens.Simatic.ALT.BusinessLogic;
using log4net;

namespace Siemens.Simatic.Service.AlertService
{
    /// <summary>
    /// 生成报警记录
    /// </summary>
    public partial class Service : ServiceBase
    {
        private IPM_ALT_BASEBO _PM_ALT_BASEBO;
        private ServiceController _serviceController;
        private ILog log = LogManager.GetLogger(typeof(MainService));

        public Service()
        {
            InitializeComponent();
            _PM_ALT_BASEBO = ObjectContainer.BuildUp<IPM_ALT_BASEBO>();
        }

        static void Main()
        {
            //#if DEBUG
            //Service ServicesToDebug = new Service();
            //ServicesToDebug.ProcessDataObjectItem();

            //#else
            System.ServiceProcess.ServiceBase[] ServicesToRun;
            ServicesToRun = new System.ServiceProcess.ServiceBase[] { new Service() };
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

            IList<IService> services = new List<IService>();
            IList<PM_ALT_BASE> alerts = _PM_ALT_BASEBO.GetActiveEntities();
            //
            foreach (PM_ALT_BASE alert in alerts)
            {
                services.Add(new AlertService(alert.AlertName));
            }
            //
            _serviceController = new ServiceController(services);
            result = _serviceController.Run();
            //
            return result;
        }
    }
}
