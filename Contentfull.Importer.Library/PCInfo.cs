using System;
using System.Management;

namespace Contentful.Importer.Library
{
    public class PCInfo
    {
        public static string GetMotherBoardID()
        {
            String serial = "S1TmxV12Rxx";
            try
            {
          
                //Get mainboard serial number and combine with static secret to encrypt login details in settings file to make it unique to this PC so that it can't be reused.
                ManagementObjectSearcher mbs = new ManagementObjectSearcher("Select * From Win32_BaseBoard");
                foreach (ManagementObject mo in mbs.Get())
                {
                    serial += mo["SerialNumber"].ToString();
                }
                return serial;
            }
            catch (Exception)
            {
                return serial;
            }
        }
    }
}
