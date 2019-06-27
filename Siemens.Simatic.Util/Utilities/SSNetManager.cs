using System;
using System.Collections.Generic;
using System.Text;

using System.Net.NetworkInformation;

namespace Siemens.Simatic.Util.Utilities
{
    public class SSNetManager
    {
        public static string GetLocalhostMacAddress(bool isFullDisplay)
        {
            NetworkInterface[] nis = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface ni in nis)
            {
                if (ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                {
                    PhysicalAddress pa = ni.GetPhysicalAddress();
                    if (pa != null)
                    {
                        //parse address
                        string mac = pa.ToString();

                        if (isFullDisplay)
                        {
                            StringBuilder parsedMac;
                            if (mac.Length == 12)
                            {
                                parsedMac = new StringBuilder();
                                for (int i = 0; i < mac.Length; i = i + 2)
                                {
                                    parsedMac.Append(mac.Substring(i, 2)).Append("-");
                                }

                                return parsedMac.Remove(parsedMac.Length - 1, 1).ToString();
                            }
                        }

                        return mac;
                    }
                }
            }

            return String.Empty;
        }
    }
}
