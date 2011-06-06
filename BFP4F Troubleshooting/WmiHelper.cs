using System;
using System.Collections.Generic;
using System.Text;
using System.Management;

namespace BFP4F_Troubleshooting
{
    class WmiHelper
    {
        public static string GetOsArchitecture()
        {
            string result = String.Empty;

            ManagementObjectSearcher searcher = null;
            ManagementObjectCollection items = null;

            try
            {
                searcher = new ManagementObjectSearcher("SELECT * FROM Win32_ComputerSystem");
                items = searcher.Get();

                foreach (ManagementObject item in items)
                {
                    if (item["SystemType"].ToString().Contains("x64") || item["SystemType"].ToString().Contains("64-bit"))
                        result = "64-bit";
                    else
                        result = "32-bit";
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString(), "GetOsArchitecture()");
            }
            finally
            {
                if (items != null)
                    items.Dispose();
                if (searcher != null)
                    searcher.Dispose();
            }

            return result;
        }
    }
}
