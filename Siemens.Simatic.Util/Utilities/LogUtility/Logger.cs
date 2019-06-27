using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using log4net;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]
namespace Siemens.Simatic.Util.Utilities
{
    public static class Logger
    {
        static Logger()
        {
            log4net.Config.XmlConfigurator.Configure();
        }

        //debug
        public static void Debug(string logMessage)
        {
            ILog log = LogManager.GetLogger(typeof(Log));
            log.Debug(logMessage);
        }

        public static void Debug(string logMessage, Exception exception)
        {
            ILog log = LogManager.GetLogger(typeof(Log));
            log.Debug(logMessage, exception);
        }

        //info
        public static void Info(string logMessage)
        {
            ILog log = LogManager.GetLogger(typeof(Log));
            log.Info(logMessage);
        }

        public static void Info(string logMessage, Exception exception)
        {
            ILog log = LogManager.GetLogger(typeof(Log));
            log.Info(logMessage, exception);
        }

        //Warn
        public static void Warn(string logMessage)
        {
            ILog log = LogManager.GetLogger(typeof(Log));
            log.Warn(logMessage);
        }

        public static void Warn(string logMessage, Exception exception)
        {
            ILog log = LogManager.GetLogger(typeof(Log));
            log.Warn(logMessage, exception);
        }

        //Error
        public static void Error(string logMessage)
        {
            ILog log = LogManager.GetLogger(typeof(Log));
            log.Error(logMessage);
        }

        public static void Error(string logMessage, Exception exception)
        {
            ILog log = LogManager.GetLogger(typeof(Log));
            log.Error(logMessage, exception);
        }

        //Fatal
        public static void Fatal(string logMessage)
        {
            ILog log = LogManager.GetLogger(typeof(Log));
            log.Fatal(logMessage);
        }

        public static void Fatal(string logMessage, Exception exception)
        {
            ILog log = LogManager.GetLogger(typeof(Log));
            log.Fatal(logMessage, exception);
        }
    }
}
