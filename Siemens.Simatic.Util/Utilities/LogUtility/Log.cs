using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using log4net;

namespace Siemens.Simatic.Util.Utilities
{
    public static class Log
    {
        static Log()
        {
            //System.Reflection.Assembly dll = System.Reflection.Assembly.LoadFile(AppDomain.CurrentDomain.BaseDirectory+"//Siemens.Simatic.Util.Utilities.dll");
            //Stream stream = dll.GetManifestResourceStream("Siemens.Simatic.Util.Utilities.LogUtility.Log4Net.xml");
            //FileInfo fi = new FileInfo(stream.t);
            log4net.Config.XmlConfigurator.Configure();
        }

        //debug
        public static void Debug(LogMessage logMessage)
        {
            PushLogParam(logMessage);
            ILog log = LogManager.GetLogger(typeof(Log));
            log.Debug(logMessage.CustomMessage);
        }

        public static void Debug(LogMessage logMessage, Exception exception)
        {
            PushLogParam(logMessage);
            ILog log = LogManager.GetLogger(typeof(Log));
            log.Debug(logMessage.CustomMessage, exception);
        }

        //info
        public static void Info(LogMessage logMessage)
        {
            PushLogParam(logMessage);
            ILog log = LogManager.GetLogger(typeof(Log));
            log.Info(logMessage.CustomMessage);
        }

        public static void Info(LogMessage logMessage, Exception exception)
        {
            PushLogParam(logMessage);
            ILog log = LogManager.GetLogger(typeof(Log));
            log.Info(logMessage.CustomMessage, exception);
        }

        //Warn
        public static void Warn(LogMessage logMessage)
        {
            PushLogParam(logMessage);
            ILog log = LogManager.GetLogger(typeof(Log));
            log.Warn(logMessage.CustomMessage);
        }

        public static void Warn(LogMessage logMessage, Exception exception)
        {
            PushLogParam(logMessage);
            ILog log = LogManager.GetLogger(typeof(Log));
            log.Warn(logMessage.CustomMessage, exception);
        }

        //Error
        public static void Error(LogMessage logMessage)
        {
            PushLogParam(logMessage);
            ILog log = LogManager.GetLogger(typeof(Log));
            log.Error(logMessage.CustomMessage);
        }

        public static void Error(LogMessage logMessage, Exception exception)
        {
            PushLogParam(logMessage);
            ILog log = LogManager.GetLogger(typeof(Log));
            log.Error(logMessage.CustomMessage, exception);
        }

        //Fatal
        public static void Fatal(LogMessage logMessage)
        {
            PushLogParam(logMessage);
            ILog log = LogManager.GetLogger(typeof(Log));
            log.Fatal(logMessage.CustomMessage);
        }

        public static void Fatal(LogMessage logMessage, Exception exception)
        {
            PushLogParam(logMessage);
            ILog log = LogManager.GetLogger(typeof(Log));
            log.Fatal(logMessage.CustomMessage, exception);
        }


        private static void PushLogParam(LogMessage logMessage)
        {
            if (string.IsNullOrEmpty(logMessage.UserID))
            {
                logMessage.UserID = "";
            }

            if (string.IsNullOrEmpty(logMessage.MethodName))
            {
                logMessage.MethodName = "";
            }

            if (string.IsNullOrEmpty(logMessage.LogID))
            {
                logMessage.LogID = Guid.NewGuid().ToString();
            }

            ThreadContext.Properties["ModuleName"] = logMessage.ModuleName; //模块名
            ThreadContext.Properties["ClassName"] = logMessage.ClassName.FullName; //类  名
            ThreadContext.Properties["MethodName"] = logMessage.MethodName; //方法名
            ThreadContext.Properties["UserID"] = logMessage.UserID; //用户ID
            ThreadContext.Properties["LogID"] = logMessage.LogID; //日志ID
        }
    }

    public enum MESModules
    {
        SM = 1,//系统管理
        POM = 2,//工单管理
        PDM = 3,//工艺设计
        PM = 4,//生产管理
        WM = 5,//仓库管理
        QM = 6,//质量管理
        MM = 7,//物料管理
        ESOP=8,//电子说明书
        SapRfc = 9,//SapRfc

        ATSService = 10, //ATS Web Service
        HHRfcService = 11, // Hisense RFC Service

        SapOrderDownloadService=12,
        CreateOrderService = 13,
        DeviceIntegrationService = 14, //设备集成的WebService
        CreateSnService=15,
        SerialNumberManagementService=16,
        MaterialSupplierRelationshipDownloadService=17,
        OrderCompletedService = 18,
        SumbitOrderSnToWmsService=19,
        PDA = 20,
        PDA_SRM = 21,
        SyncInspectionInfoService = 22,
        LES = 23,
        CNHS = 24
    }
    /// <summary>
    /// 消息
    /// </summary>
    public class LogMessage
    {
        public LogMessage()
        {
            ClassName = null;
            MethodName = "";
            CustomMessage = "";
            LogID = Guid.NewGuid().ToString();
        }

        public LogMessage(string guid)
        {
            ClassName = null;
            MethodName = "";
            CustomMessage = "";
            LogID = guid;
        }

        public MESModules ModuleName
        {
            get;
            set;
        }

        public Type ClassName
        {
            get;
            set;
        }

        public string MethodName
        {
            get;
            set;
        }

        public string CustomMessage { get; set; }

        public string UserID
        {
            get;
            set;
        }

        public string LogID { get; set; }
    }
}
