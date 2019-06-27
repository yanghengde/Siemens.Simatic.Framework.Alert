using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;

namespace Siemens.Simatic.Service.NotifyService
{
    [RunInstaller(true)]
    public partial class NotifyInstaller : System.Configuration.Install.Installer
    {
        private System.ServiceProcess.ServiceInstaller _serviceInstaller;
        private System.ServiceProcess.ServiceProcessInstaller _serviceProcessInstaller;

        public NotifyInstaller()
        {
            InitializeComponent();
            //
            _serviceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            _serviceInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // serviceProcessInstaller
            // 
            _serviceProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            _serviceProcessInstaller.Password = null;
            _serviceProcessInstaller.Username = null;
            // 
            // PDM_XMLParserInstaller
            // 
            _serviceInstaller.Description = "Alert Notify Service";
            _serviceInstaller.DisplayName = "SiemensAlertNotify@Opcenter - Alert Notify Service";
            _serviceInstaller.ServiceName = "AlertNotifyService";
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            _serviceProcessInstaller,
            _serviceInstaller});
        }
    }
}
