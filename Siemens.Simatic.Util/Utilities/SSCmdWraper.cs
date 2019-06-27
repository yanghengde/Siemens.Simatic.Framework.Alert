using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace Siemens.Simatic.Util.Utilities
{
    public class SSCmdWraper
    {
        private Process _cmdProcess;

        public SSCmdWraper()
            : this(false)
        {

        }

        public SSCmdWraper(bool showCmdWindow)
        {
            _cmdProcess = new Process();
            _cmdProcess.StartInfo.FileName = "cmd.exe";
            _cmdProcess.StartInfo.UseShellExecute = false;
            _cmdProcess.StartInfo.RedirectStandardInput = true;
            _cmdProcess.StartInfo.RedirectStandardOutput = true;
            _cmdProcess.StartInfo.RedirectStandardError = true;
            _cmdProcess.StartInfo.CreateNoWindow = !showCmdWindow;    
        }

        public void Start()
        {
            _cmdProcess.Start();
        }

        public void WriteCmdLine(string command)
        {
            _cmdProcess.StandardInput.WriteLine(command);
        }

        public void Exit()
        {
            _cmdProcess.StandardInput.WriteLine("exit");
        }

        public void WaitForExit()
        {
            _cmdProcess.WaitForExit();
        }

        public string OutputToEnd()
        {
            return _cmdProcess.StandardOutput.ReadToEnd();
        }

        public void Close()
        {
            _cmdProcess.Close();
        }
    }
}
