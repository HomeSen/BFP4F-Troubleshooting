using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.DirectX.Direct3D;
using System.Windows.Forms;
using System.Management;

namespace BFP4F_Troubleshooting
{
    class SystemHelper
    {
        #region Fields

        private Dictionary<int, string[]> _vendors = null;
        private Device _device = null;
        private bool _dxInstalled = false;

        #endregion


        #region Properties

        public bool DxInstalled
        {
            get { return this._dxInstalled; }
        }

        #endregion


        #region Constructor(s)

        public SystemHelper(Form form)
        {
            try
            {
                PresentParameters presentParams = new PresentParameters();
                presentParams.Windowed = true;
                presentParams.SwapEffect = SwapEffect.Discard;

                this._device = new Device(0, DeviceType.Hardware, form, CreateFlags.SoftwareVertexProcessing, presentParams);
                this._dxInstalled = true;
            }
            catch
            {
                this._dxInstalled = false;
            }

            this._vendors = new Dictionary<int, string[]>(5);
            this._vendors.Add(0x8086, new string[] { "Intel Corporation", "http://downloadcenter.intel.com/" });
            this._vendors.Add(0x10DE, new string[] { "nVidia Corporation", "http://www.nvidia.com/Download/index.aspx?lang=en-us" });
            this._vendors.Add(0x1002, new string[] { "ATI Technologies", "http://support.amd.com/us/gpudownload/Pages/index.aspx" });
            this._vendors.Add(0x10EC, new string[] { "Realtek Semiconductor Co., Ltd.", "http://www.realtek-driver.com/index.php" });
            this._vendors.Add(0x1102, new string[] { "Creative Labs", "http://support.creative.com/" });
            this._vendors.Add(0x15AD, new string[] { "VMware", "Drivers are installed by \"VMware Tools\"" });
        }

        #endregion


        #region Video Device

        #region DeviceName
        public string GetDeviceName()
        {
            if (this._dxInstalled)
                return this.GetDeviceNameFromDirectX();
            else
                return this.GetDeviceNameFromWmi();
        }

        private string GetDeviceNameFromWmi()
        {
            string result = "";
            try
            {
                ManagementObjectSearcher searcher =
                    new ManagementObjectSearcher("root\\CIMV2",
                    "SELECT Name FROM Win32_VideoController WHERE PNPDeviceID LIKE 'PCI%'");

                foreach (ManagementObject item in searcher.Get())
                {
                    result = item["Name"].ToString();
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString(), "GetDeviceName()");
            }

            return result;
        }

        private string GetDeviceNameFromDirectX()
        {
            string result = "";

            try
            {
                result = Manager.Adapters[0].Information.Description;
            }
            catch (Exception ex)
            {
                //System.Windows.Forms.MessageBox.Show(ex.ToString(), "GetDeviceName()");
                return this.GetDeviceNameFromWmi();
            }

            return result;
        }
        #endregion

        #region DeviceVendor
        public string[] GetDeviceVendor()
        {
            if (this.DxInstalled)
                return this.GetDeviceVendorFromDirectX();
            else
                return this.GetDeviceVendorFromWmi();
        }

        private string[] GetDeviceVendorFromWmi()
        {
            string[] result = null;
            int vendorId = 0;
            string deviceId = "";

            try
            {
                ManagementObjectSearcher searcher =
                    new ManagementObjectSearcher("root\\CIMV2",
                    "SELECT PNPDeviceID FROM Win32_VideoController WHERE PNPDeviceID LIKE 'PCI%'");

                foreach (ManagementObject item in searcher.Get())
                {
                    deviceId = item["PNPDeviceID"].ToString(); // PCI\VEN_xxxx&DEV_yyyy&SUBSYS_*
                }

                deviceId = deviceId.Replace(@"PCI\VEN_", "");
                deviceId = deviceId.Remove(4);
                vendorId = int.Parse(deviceId, System.Globalization.NumberStyles.AllowHexSpecifier);

                result = _vendors[vendorId];
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

        private string[] GetDeviceVendorFromDirectX()
        {
            string[] result;

            try
            {
                result = _vendors[Manager.Adapters[0].Information.VendorId];
            }
            catch (KeyNotFoundException)
            {
                result = new string[] { "Unknown", "Unknown" };
            }
            catch (Exception ex)
            {
                result = new string[] { };
                //System.Windows.Forms.MessageBox.Show(ex.ToString(), "GetDeviceVendor()");
                return this.GetDeviceVendorFromWmi();
            }

            return result;
        }
        #endregion

        #region DriverVersion
        public string GetDriverVersion()
        {
            if (this.DxInstalled)
                return this.GetDriverVersionFromDirectX();
            else
                return this.GetDriverVersionFromWmi();
        }

        private string GetDriverVersionFromWmi()
        {
            string result = "";
            try
            {
                ManagementObjectSearcher searcher =
                    new ManagementObjectSearcher("root\\CIMV2",
                    "SELECT DriverVersion FROM Win32_VideoController WHERE PNPDeviceID LIKE 'PCI%'");

                foreach (ManagementObject item in searcher.Get())
                {
                    result = item["DriverVersion"].ToString();
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString(), "GetDeviceName()");
            }

            return result;
        }

        private string GetDriverVersionFromDirectX()
        {
            string result = "";

            try
            {
                result = Manager.Adapters[0].Information.DriverVersion.ToString();
            }
            catch (Exception ex)
            {
                //System.Windows.Forms.MessageBox.Show(ex.ToString(), "GetDriverVersion()");
                return this.GetDriverVersionFromWmi();
            }

            return result;
        }
        #endregion

        public DateTime GetDriverDate()
        {
            DateTime result = DateTime.MinValue;
            string dateString = "";
            int year, month, day;

            try
            {
                ManagementObjectSearcher searcher =
                    new ManagementObjectSearcher("root\\CIMV2",
                    "SELECT DriverDate FROM Win32_VideoController WHERE PNPDeviceID LIKE 'PCI%'");

                foreach (ManagementObject item in searcher.Get())
                {
                    dateString = item["DriverDate"].ToString().Trim();
                    if (dateString.Length < 8)
                        continue;

                    year = int.Parse(dateString.Substring(0, 4));
                    month = int.Parse(dateString.Substring(4, 2));
                    day = int.Parse(dateString.Substring(6, 2));

                    result = new DateTime(year, month, day);
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString(), "GetDeviceName()");
            }

            return result;
        }
        
        public Version GetPSVersion()
        {
            Version result = null;

            try
            {
                result = _device.DeviceCaps.PixelShaderVersion;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString(), "GetPSVersion()");
            }

            return result;
        }

        public uint GetVideoMemory()
        {
            uint result = 0;

            try
            {
                result = (uint)_device.AvailableTextureMemory; // Bytes, rounded to nearest MiBytes value
                result /= 1024; // KiBytes
                result /= 1024; // MiBytes
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString(), "GetVideoMemory()");
            }

            return result;
        }

        public uint GetDedicatedVideoMemory()
        {
            uint result = 0;

            try
            {
                ManagementObjectSearcher searcher =
                    new ManagementObjectSearcher("root\\CIMV2",
                    "SELECT * FROM Win32_VideoController WHERE PNPDeviceID LIKE 'PCI%'");
                
                foreach(ManagementObject item in searcher.Get())
                {
                    result = Convert.ToUInt32(item["AdapterRAM"]);// Bytes
                    result /= 1024; // KiBytes
                    result /= 1024; // MiBytes
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString(), "GetDedicatedVideoMemory()");
            }

            return result;
        }

        #endregion


        #region General Hardware

        public double GetMemory()
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

        public uint GetCPUSpeed()
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

        public uint GetLogicalCPUCount()
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


        #region Operating System

        public string GetWindowsVersion()
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
                    string architecture = GetOsArchitecture();
                    if (String.IsNullOrEmpty(architecture) == false)
                    {
                        result += " (" + architecture + ")";
                    }
                    if (osInfo.ServicePack.Trim().Length > 0)
                    {
                        result += " " + osInfo.ServicePack;
                    }
                }
            }

            return result;
        }

        public string GetOsArchitecture()
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

        #endregion
    }
}
