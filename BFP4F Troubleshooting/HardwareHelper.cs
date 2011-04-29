using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.DirectX.Direct3D;
using System.Windows.Forms;

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

            initialized = true;
        }

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
    }
}
