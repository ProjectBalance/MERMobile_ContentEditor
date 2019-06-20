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
          
                //Get motherboard's serial number 
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
