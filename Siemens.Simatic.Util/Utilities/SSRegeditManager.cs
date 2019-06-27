using System;
using System.Collections.Generic;
using System.Text;

namespace Siemens.Simatic.Util.Utilities
{
    public class SSRegeditManager
    {
        public const string regPath_MESClient = @"SoftWare\SIEMENS\MESClient";

        //Get registry value.      
        public static string GetRegistryValue(SSRegistryMainNode RegType, string strPath, string strName, string strDefault)
        {
            string regValue;

            if (RegType == SSRegistryMainNode.Default)
                RegType = SSRegistryMainNode.CurrentUser;

            Microsoft.Win32.RegistryKey KeyRead;

            switch (RegType)
            {
                case SSRegistryMainNode.ClassesRoot:
                    KeyRead = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(strPath, true);
                    break;
                case SSRegistryMainNode.CurrentConfig:
                    KeyRead = Microsoft.Win32.Registry.CurrentConfig.OpenSubKey(strPath, true);
                    break;
                case SSRegistryMainNode.CurrentUser:
                    KeyRead = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(strPath, true);
                    break;
                case SSRegistryMainNode.LocalMachine:
                    KeyRead = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(strPath, true);
                    break;
                case SSRegistryMainNode.Users:
                    KeyRead = Microsoft.Win32.Registry.Users.OpenSubKey(strPath, true);
                    break;
                default:
                    KeyRead = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(strPath, true);
                    break;
            }
            if (KeyRead == null)
            {
                regValue = strDefault;
            }
            else
            {
                object obj = KeyRead.GetValue(strName);
                if (obj == null)
                {
                    regValue = strDefault;
                }
                else
                {
                    regValue = obj.ToString();
                }

                KeyRead.Close();
            }

            return regValue;
        }

        // Get registry value.                                                   
        public static string[] GetSubKeyNames(SSRegistryMainNode RegType, string strPath)
        {
            string[] aryValue = null;

            if (RegType == SSRegistryMainNode.Default)
                RegType = SSRegistryMainNode.CurrentUser;

            Microsoft.Win32.RegistryKey KeyRead;

            switch (RegType)
            {
                case SSRegistryMainNode.ClassesRoot:
                    KeyRead = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(strPath, true);
                    break;
                case SSRegistryMainNode.CurrentConfig:
                    KeyRead = Microsoft.Win32.Registry.CurrentConfig.OpenSubKey(strPath, true);
                    break;
                case SSRegistryMainNode.CurrentUser:
                    KeyRead = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(strPath, true);
                    break;
                case SSRegistryMainNode.LocalMachine:
                    KeyRead = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(strPath, true);
                    break;
                case SSRegistryMainNode.Users:
                    KeyRead = Microsoft.Win32.Registry.Users.OpenSubKey(strPath, true);
                    break;
                default:
                    KeyRead = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(strPath, true);
                    break;
            }

            if (KeyRead == null)
                return null;

            aryValue = KeyRead.GetSubKeyNames();
            KeyRead.Close();

            return (aryValue);
        }

        //Set registry value.                                                          
        public static bool SetRegistryValue(SSRegistryMainNode RegType, string strPath, string strName, string strValue)
        {
            if (RegType == SSRegistryMainNode.Default)
                RegType = SSRegistryMainNode.CurrentUser;

            try
            {
                Microsoft.Win32.RegistryKey KeyWrite;
                switch (RegType)
                {
                    case SSRegistryMainNode.ClassesRoot:
                        KeyWrite = Microsoft.Win32.Registry.ClassesRoot.CreateSubKey(strPath);
                        break;
                    case SSRegistryMainNode.CurrentConfig:
                        KeyWrite = Microsoft.Win32.Registry.CurrentConfig.CreateSubKey(strPath);
                        break;
                    case SSRegistryMainNode.CurrentUser:
                        KeyWrite = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(strPath);
                        break;
                    case SSRegistryMainNode.LocalMachine:
                        KeyWrite = Microsoft.Win32.Registry.LocalMachine.CreateSubKey(strPath);
                        break;
                    case SSRegistryMainNode.Users:
                        KeyWrite = Microsoft.Win32.Registry.Users.CreateSubKey(strPath);
                        break;
                    default:
                        KeyWrite = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(strPath);
                        break;
                }

                if (KeyWrite == null)
                    return false;

                KeyWrite.SetValue(strName, strValue);
                KeyWrite.Close();

                return true;
            }
            catch
            {
                //m_objError.WriteErrorLog("SetRegistryValue", "Write data to regedit error.", "");
                return false;
            }
        }

        //Delete registry value.                                                     
        public static bool DeleteRegistryString(SSRegistryMainNode RegType, string strPath, string strName)
        {
            if (RegType == SSRegistryMainNode.Default)
                RegType = SSRegistryMainNode.CurrentUser;

            try
            {
                Microsoft.Win32.RegistryKey KeyNode;

                switch (RegType)
                {
                    case SSRegistryMainNode.ClassesRoot:
                        KeyNode = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(strPath, true);
                        break;
                    case SSRegistryMainNode.CurrentConfig:
                        KeyNode = Microsoft.Win32.Registry.CurrentConfig.OpenSubKey(strPath, true);
                        break;
                    case SSRegistryMainNode.CurrentUser:
                        KeyNode = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(strPath, true);
                        break;
                    case SSRegistryMainNode.LocalMachine:
                        KeyNode = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(strPath, true);
                        break;
                    case SSRegistryMainNode.Users:
                        KeyNode = Microsoft.Win32.Registry.Users.OpenSubKey(strPath, true);
                        break;
                    default:
                        KeyNode = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(strPath, true);
                        break;
                }

                if (KeyNode == null)
                    return false;

                KeyNode.DeleteValue(strName, false);
                KeyNode.Close();

                return (true);
            }
            catch
            {
                //m_objError.WriteErrorLog("DeleteRegistryString", "delete data from regedit error.", "");
                return false;
            }
        }

        //Delete registry node.                                                  
        public static bool DeleteRegistryNode(SSRegistryMainNode RegType, string strPath)
        {
            if (RegType == SSRegistryMainNode.Default)
                RegType = SSRegistryMainNode.CurrentUser;

            try
            {
                switch (RegType)
                {
                    case SSRegistryMainNode.ClassesRoot:
                        Microsoft.Win32.Registry.ClassesRoot.DeleteSubKeyTree(strPath);
                        break;
                    case SSRegistryMainNode.CurrentConfig:
                        Microsoft.Win32.Registry.CurrentConfig.DeleteSubKeyTree(strPath); ;
                        break;
                    case SSRegistryMainNode.CurrentUser:
                        Microsoft.Win32.Registry.CurrentUser.DeleteSubKeyTree(strPath);
                        break;
                    case SSRegistryMainNode.LocalMachine:
                        Microsoft.Win32.Registry.LocalMachine.DeleteSubKeyTree(strPath);
                        break;
                    case SSRegistryMainNode.Users:
                        Microsoft.Win32.Registry.Users.DeleteSubKeyTree(strPath);
                        break;
                    default:
                        Microsoft.Win32.Registry.CurrentUser.DeleteSubKeyTree(strPath);
                        break;
                }
                return (true);
            }
            catch
            {
                //m_objError.WriteErrorLog("DeleteRegistryString", "delete node from regedit error.", "");
                return (false);
            }
        }

    }

    public enum SSRegistryMainNode
    {
        Default,
        ClassesRoot,
        CurrentUser,
        LocalMachine,
        Users,
        CurrentConfig
    }

    public class SSRegistryNode
    {
        public const string MMClient_LoginUser = "LoginUser";
        public const string MMClient_DefaultPrintTemplate = "DefaultPrintTemplate";

        public const string MMClient_InWH_CurrentTerminal = "InWH_CurrentTerminal";
        public const string MMClient_InWH_CurrentWorkOperation = "InWH_CurrentWorkOperation";
        public const string MMClient_InWH_CurrentLocation = "InWH_CurrentLocation";
        public const string MMClient_OutWH_CurrentTerminal = "OutWH_CurrentTerminal";
        public const string MMClient_OutWH_CurrentWorkOperation = "OutWH_CurrentWorkOperation";
        public const string MMClient_OutPLT_CurrentTerminal = "OutPLT_CurrentTerminal";
        public const string MMClient_OutPLT_CurrentWorkOperation = "OutPLT_CurrentWorkOperation";
        public const string MMClient_BackWH_CurrentTerminal = "BackWH_CurrentTerminal";
        public const string MMClient_BackWH_CurrentWorkOperation = "BackWH_CurrentWorkOperation";
        public const string MMClient_BackWH_CurrentLocation = "BackWH_CurrentLocation";
        public const string MMClient_TranWH_CurrentTerminal = "TranWH_CurrentTerminal";
        public const string MMClient_TranWH_CurrentWorkOperation = "TranWH_CurrentWorkOperation";

        public const string MMClient_PrdClient_CurrentTerminal = "PrdClient_CurrentTerminal";
        public const string MMClient_PrdClient_CurrentWorkOperation = "PrdClient_CurrentWorkOperation";

        public const string MMClient_Assemble_CurrentTerminal = "Assemble_CurrentTerminal";
        public const string MMClient_Assemble_CurrentWorkOperation = "Assemble_CurrentWorkOperation";

        public const string MMClient_SNM_RAWSNTemplate = "RAW_SN";
        public const string MMClient_SNM_PRODUCTSNTemplate = "PRODUCT_SN";
        public const string MMClient_QM_TFNOTemplate = "TFNO_SN";
        public const string MMClient_SNM_NoOrderTemplate = "NoOrder_SN";
    }
}
