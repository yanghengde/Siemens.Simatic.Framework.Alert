using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.IO;

namespace Siemens.Simatic.Service.NotifyService
{
    public class MesLog
    {
        private static string _logMode = ConfigurationManager.AppSettings["LogMode"];
        private static string _logFilePath = ConfigurationManager.AppSettings["LogFilePath"];
        private static double _maxLogFileAge = double.Parse(ConfigurationManager.AppSettings["MaxLogFileAge"].ToString());
        private static bool _isSendErrorMail = bool.Parse(ConfigurationManager.AppSettings["SendErrorMail"].ToString());

        //public static readonly string _mailTo = ConfigurationSettings.AppSettings["MailTo"].ToString();
        private string _logCategory;
        private string _emailTemplate = string.Empty;
        private string _applicationName = ConfigurationManager.AppSettings["ApplicationName"];

        public MesLog()
            : this("")
		{
           
		}

        public MesLog(string logCategory)
        {
            _logCategory = logCategory;
            //
            if (!Directory.Exists(_logFilePath))
            {
                Directory.CreateDirectory(_logFilePath);
            }
            //
            this.GetEmailTemplate();
        }

        private void GetEmailTemplate()
        {
            //string plant = MesToolStudio.BusinessLogicLibrary.Util.Common.getSystemConfig("Plants", "DEFAULT");
            ////
            //_emailTemplate = MesToolStudio.BusinessLogicLibrary.Util.Common.getSystemConfig("EmailTemplate-MESPerformance", plant);
        }

        public void LogException(string strData, string strSource)
        {
            this.LogException(strData, strSource, true);
        }

        public void LogException(string strData, string strSource, bool needMail)
        {
            switch (_logMode.ToUpper())
            {
                case "FILE":
                    WriteToFile(_logFilePath, "[ERROR] [" + DateTime.Now.ToString() + "] : " + strSource + " - " + strData);
                    break;
                case "CONSOLE":
                    Console.WriteLine("[ERROR] [" + DateTime.Now.ToString() + "] : " + strSource + " - " + strData);
                    break;
                case "APPLOGGER":
                    break;
            }
            //
            if (needMail && _isSendErrorMail)
            {
                this.SendMail(strData, strSource);
            }
        }

        public void LogInfo(string strData)
        {
            switch (_logMode.ToUpper())
            {
                case "FILE":
                    WriteToFile(_logFilePath, "[INFO] [" + DateTime.Now.ToString() + "] : " + strData);
                    break;
                case "CONSOLE":
                    Console.WriteLine("[INFO] [" + DateTime.Now.ToString() + "] : " + strData);
                    break;
                case "APPLOGGER":
                    break;
            }
        }

        private bool WriteToFile(string strFile, string strData)
        {
            DateTime dtNow = DateTime.Now;
            string fileName = string.Format("{0}_{1}.{2}", !string.IsNullOrEmpty(_logCategory) ? _logCategory : "_", dtNow.ToString("yyyyMMdd"), "txt");
            StreamWriter sw = new StreamWriter(Path.Combine(strFile, fileName), true);
            sw.WriteLine(strData);
            sw.Close();
            return true;
        }

        private void SendMail(string strData, string strSource)
        {
            if (string.IsNullOrEmpty(_emailTemplate)) return;
            //
            string emailContentXML = string.Format(_emailTemplate, "[" + DateTime.Now.ToString() + "] : " + strSource + " - " + strData);
            //
            //MesToolStudio.Utility.Log.MesEmailHelper.CreateEmail(_applicationName, emailContentXML);
        }

		public bool DeleteAgedLogs()
		{
			try
			{
				string[] arrLogFiles = Directory.GetFiles(_logFilePath, "*.txt");
				
				//loop through all the signal files found
				for(int i=0; i<arrLogFiles.Length; i++)
				{
					DateTime dtFileDate = File.GetLastWriteTime(arrLogFiles[i]);
					if (dtFileDate < DateTime.Now.Date.AddDays(0-_maxLogFileAge))
						File.Delete(arrLogFiles[i]);
				}
				return true;
			}
			catch
			{
				return false;
			}
		}
    }
}
