using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.DirectX.Direct3D;
using System.Windows.Forms;
using System.Management;

namespace BFP4F_Troubleshooting
{
    class HardwareHelper
    {
        static Dictionary<int, string[]> vendors;

        static Device device = null;
        static bool initialized = false;
        
        public static bool Initialized
        {
            get
            {
                return initialized;
            }
        }

        public static void Initialize(Form form)
        {
            if (initialized)
                return;

            PresentParameters presentParams = new PresentParameters();
            presentParams.Windowed = true;
            presentParams.SwapEffect = SwapEffect.Discard;

            device = new Device(0, DeviceType.Hardware, form, CreateFlags.SoftwareVertexProcessing, presentParams);

            vendors = new Dictionary<int, string[]>(5);
            vendors.Add(0x8086, new string[]{ "Intel Corporation", "http://downloadcenter.intel.com/" });
            vendors.Add(0x10DE, new string[] { "nVidia Corporation", "http://www.nvidia.com/Download/index.aspx?lang=en-us" });
            vendors.Add(0x1002, new string[]{ "ATI Technologies", "http://support.amd.com/us/gpudownload/Pages/index.aspx" });
            vendors.Add(0x10EC, new string[] { "Realtek Semiconductor Co., Ltd.", "http://www.realtek-driver.com/index.php" });
            vendors.Add(0x1102, new string[] { "Creative Labs", "http://support.creative.com/" });
            vendors.Add(0x15AD, new string[] { "VMware", "Drivers are installed by \"VMware Tools\"" });

            initialized = true;
        }

        #region Video Device

        public static string GetDeviceName()
        {
            string result = "";

            try
            {
                result = Manager.Adapters[0].Information.Description;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString(), "GetDeviceName()");
            }

            return result;
        }

        public static string[] GetDeviceVendor()
        {
            string[] result;

            try
            {
                result = vendors[Manager.Adapters[0].Information.VendorId];
            }
            catch (KeyNotFoundException)
            {
                result = new string[] { "Unknown", "Unknown" };
            }
            catch (Exception ex)
            {
                result = new string[] { };
                System.Windows.Forms.MessageBox.Show(ex.ToString(), "GetDeviceVendor()");
            }

            return result;
        }

        public static string GetDriverVersion()
        {
            string result = "";

            try
            {
                result = Manager.Adapters[0].Information.DriverVersion.ToString();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString(), "GetDriverVersion()");
            }

            return result;
        }

        public static Version GetPSVersion()
        {
            Version result = null;

            try
            {
                result = device.DeviceCaps.PixelShaderVersion;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString(), "GetPSVersion()");
            }

            return result;
        }

        public static int GetVideoMemory()
        {
            int result = 0;

            try
            {
                result = device.AvailableTextureMemory; // Bytes, rounded to nearest MiBytes value
                result /= 1024; // KiBytes
                result /= 1024; // MiBytes
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString(), "GetDriverVersion()");
            }

            return result;
        } 

        #endregion


        #region General Hardware

        public static double GetMemory()
        {
            double result = 0;

            try
            {
                ObjectQuery wmiQuery = new ObjectQuery("SELECT * FROM Win32_ComputerSystem");
                ManagementObjectSearcher wmiSearcher = new ManagementObjectSearcher(wmiQuery);
                ManagementObjectCollection items = wmiSearcher.Get();

                foreach (ManagementObject item in items)
                {
                    result = Convert.ToDouble(item["TotalPhysicalMemory"]); // Bytes
                    result /= 1024; // KiBytes
                    result /= 1024; // MiBytes
                }
            }
            catch { }

            return result;
        }

        public static uint GetCPUSpeed()
        {
            uint result = 0;
            ManagementObject mo = null;

            try
            {
                mo = new ManagementObject("Win32_Processor.DeviceID='CPU0'");
                result = (uint)mo["MaxClockSpeed"];
            }
            catch { }
            finally
            {
                if (mo != null)
                    mo.Dispose();
            }

            return result;
        }

        public static uint GetLogicalCPUCount()
        {
            uint result = 0;

            try
            {
                ManagementObjectSearcher searcher =
                    new ManagementObjectSearcher("root\\CIMV2",
                    "SELECT * FROM Win32_Processor");

                foreach (ManagementObject queryObj in searcher.Get())
                {
                    result = (uint)queryObj["NumberOfLogicalProcessors"];
                }
            }
            catch { }

            return result;
        }

        #endregion
        
        public static string GetWindowsVersion()
        {
            string result = "";
            OperatingSystem osInfo = Environment.OSVersion;

            if (osInfo.Platform == PlatformID.Win32Windows)
            {
                switch (osInfo.Version.Minor)
                {
                    case 10:
                        result = "Windows 98";
                        break;
                    case 90:
                        result = "Windows ME";
                        break;
                }
            }
            else if (osInfo.Platform == PlatformID.Win32NT)
            {
                if (osInfo.Version.Major == 4)
                {
                    result = "Windows NT 4.0";
                }
                else if (osInfo.Version.Major == 5)
                {
                    switch (osInfo.Version.Minor)
                    {
                        case 0:
                            result = "Windows 2000";
                            break;
                        case 1:
                            result = "Windows XP";
                            break;
                        case 2:
                            result = "Windows Server 2003";
                            break;
                    }
                }
                else if (osInfo.Version.Major == 6)
                {
                    switch (osInfo.Version.Minor)
                    {
                        case 0:
                            result = "Windows Vista/Server 2008";
                            break;
                        case 1:
                            result = "Windows 7";
                            break;
                    }
                }

                if (result.Trim().Length > 0)
                {
                    if (osInfo.ServicePack.Trim().Length > 0)
                    {
                        result += " " + osInfo.ServicePack;
                    }
                }
            }

            return result;
        }
    }
}
