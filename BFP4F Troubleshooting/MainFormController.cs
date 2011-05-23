using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace BFP4F_Troubleshooting
{
    class MainFormController
    {
        MainForm mainForm = null;
        const int MIN_PIXELSHADER = 2;
        const int MIN_VIDEOMEMORY = 256;
        const int MIN_CPU = 1700;
        const int MIN_RAM = 512;

        public MainFormController(Form mainForm)
        {
            this.mainForm = (MainForm)mainForm;
        }

        private void GetVideoDevice()
        {
            this.mainForm.lblStatus.Text = "Checking Hardware (Video device) ...";
            Application.DoEvents();

            this.mainForm.lblDeviceName.Text = HardwareHelper.GetDeviceName();
            string[] vendor = HardwareHelper.GetDeviceVendor();
            if (vendor.Length == 2)
            {
                this.mainForm.lblDeviceVendor.Text = vendor[0];
                this.mainForm.lblDriverUrl.Text = vendor[1];
                this.mainForm.Success++;
            }
            else
            {
                this.mainForm.lblDeviceVendor.Text = Properties.Resources.TextUnknown;
                this.mainForm.lblDriverUrl.Text = Properties.Resources.TextUnknown;
                this.mainForm.Errors++;
            }
            this.mainForm.lblDriverDate.Text = HardwareHelper.GetDriverVersion();

            // Get max. supported PixelShader version
            Version version = HardwareHelper.GetPSVersion();
            if (version != null)
            {
                this.mainForm.lblPixelShader.Text = version.ToString();
                if (version.Major < MIN_PIXELSHADER)
                {
                    this.mainForm.picPixelShader.Image = Properties.Resources.IconError;
                    this.mainForm.Errors++;
                }
                else
                {
                    this.mainForm.picPixelShader.Image = Properties.Resources.IconSuccess;
                    this.mainForm.Success++;
                }
            }
            else
            {
                this.mainForm.lblPixelShader.Text = Properties.Resources.TextUnknown;
                this.mainForm.picPixelShader.Image = Properties.Resources.IconWarning;
                this.mainForm.Warnings++;
            }

            // Get (combined) video memory
            int videoMemory = HardwareHelper.GetVideoMemory();
            this.mainForm.lblVideoMem.Text = videoMemory + " MB";
            if (videoMemory > MIN_VIDEOMEMORY)
            {
                this.mainForm.picVideoMem.Image = Properties.Resources.IconSuccess;
                this.mainForm.Success++;
            }
            else if (videoMemory == MIN_VIDEOMEMORY)
            {
                this.mainForm.picVideoMem.Image = Properties.Resources.IconWarning;
                this.mainForm.Warnings++;
            }
            else
            {
                this.mainForm.picVideoMem.Image = Properties.Resources.IconError;
                this.mainForm.Errors++;
            }
        }

        public void RunTests()
        {
            NetFxTest();
            VCRTTest();
            DirectXTest();
            ConnectionTest();
            HardwareTest();

            this.mainForm.lblStatus.Text = "Finished testing.";
            Application.DoEvents();
        }

        private void HardwareTest()
        {
            this.mainForm.lblStatus.Text = "Checking Hardware (CPU + RAM) ...";
            Application.DoEvents();

            string windowsVersion = HardwareHelper.GetWindowsVersion();
            this.mainForm.lblOperatingSystem.Text = windowsVersion;

            uint cpuSpeed = HardwareHelper.GetCPUSpeed();
            if (cpuSpeed > 0)
            {
                this.mainForm.lblCPU.Text = cpuSpeed + " MHz";
                uint logicalCpus = HardwareHelper.GetLogicalCPUCount();
                if (logicalCpus > 0)
                {
                    this.mainForm.lblCPU.Text += " (" + logicalCpus + " cores)";
                }

                if ((cpuSpeed <= (MIN_CPU * 1.25)) && (logicalCpus <= 1))
                {
                    this.mainForm.picCPU.Image = Properties.Resources.IconWarning;
                    this.mainForm.Warnings++;
                }
                else
                {
                    this.mainForm.picCPU.Image = Properties.Resources.IconSuccess;
                    this.mainForm.Success++;
                }
            }
            else
            {
                this.mainForm.picCPU.Image = Properties.Resources.IconError;
                this.mainForm.lblCPU.Text = Properties.Resources.TextError;
                this.mainForm.Errors++;
            }
        
            double memory = HardwareHelper.GetMemory();
            if (memory > 0)
            {
                this.mainForm.lblRAM.Text = memory.ToString("#,##0.00") + " MB";

                if (memory < MIN_RAM)
                {
                    this.mainForm.picRAM.Image = Properties.Resources.IconError;
                    this.mainForm.Errors++;
                }
                else if ((memory >= MIN_RAM) && (memory <= (MIN_RAM * 2)))
                {
                    this.mainForm.picRAM.Image = Properties.Resources.IconWarning;
                    this.mainForm.Warnings++;
                }
                else
                {
                    this.mainForm.picRAM.Image = Properties.Resources.IconSuccess;
                    this.mainForm.Success++;
                }
            }
            else
            {
                this.mainForm.picRAM.Image = Properties.Resources.IconError;
                this.mainForm.lblRAM.Text = Properties.Resources.TextError;
                this.mainForm.Errors++;
            }
        }

        private void ConnectionTest()
        {
            CheckHostsFile();
            PingServers();
        }

        private void PingServers()
        {
            this.mainForm.lblStatus.Text = "Checking Connection (pinging servers) ...";
            Application.DoEvents();

            long replyTime = 0;

            for (int i = 0; i < NetworkHelper.servers.Length; i++)
            {
                replyTime = NetworkHelper.PingServer(NetworkHelper.servers[i]);
                if (replyTime < 0)
                {
                    this.mainForm.ServerIcons[i].Image = Properties.Resources.IconError;
                    this.mainForm.ServerLabels[i].Text = Properties.Resources.TextError;
                    this.mainForm.Errors++;
                }
                else if (replyTime == 0)
                {
                    this.mainForm.ServerIcons[i].Image = Properties.Resources.IconWarning;
                    this.mainForm.ServerLabels[i].Text = Properties.Resources.TextTimeOut;
                    this.mainForm.Warnings++;
                }
                else
                {
                    this.mainForm.ServerIcons[i].Image = Properties.Resources.IconSuccess;
                    this.mainForm.ServerLabels[i].Text = replyTime + " ms";
                    this.mainForm.Success++;
                }
            }
        }

        private void CheckHostsFile()
        {
            this.mainForm.lblStatus.Text = "Checking Connection (hosts file) ...";
            Application.DoEvents();

            int hosts = FileSystemHelper.CheckHostsFile();
            if (hosts == 0)
            {
                this.mainForm.picHosts.Image = Properties.Resources.IconSuccess;
                this.mainForm.lblHosts.Visible = false;
                this.mainForm.linkHosts.Enabled = true;
                this.mainForm.Success++;
            }
            else if (hosts == -1)
            {
                this.mainForm.picHosts.Image = Properties.Resources.IconWarning;
                this.mainForm.lblHosts.Text = "Could not find hosts file";
                this.mainForm.linkHosts.Enabled = false;
                this.mainForm.Warnings++;
            }
            else
            {
                this.mainForm.picHosts.Image = Properties.Resources.IconWarning;
                this.mainForm.lblHosts.Text = hosts + " suspicious entries found";
                this.mainForm.lblHosts.Visible = true;
                this.mainForm.linkHosts.Enabled = true;
                this.mainForm.Warnings++;
            }
        }

        void DirectXTest()
        {
            this.mainForm.lblStatus.Text = "Checking Prerequisites (DirectX) ...";
            Application.DoEvents();

            try
            {
                HardwareHelper.Initialize(this.mainForm);
            }
            catch { }
            
            if (Program.dxFailed)
            {
                this.mainForm.picDX.Image = Properties.Resources.IconError;
                this.mainForm.lblDriverUrl.Enabled = false;
                this.mainForm.Errors++;
                return;
            }
                
            this.mainForm.picDX.Image = Properties.Resources.IconSuccess;
            this.mainForm.lblDriverUrl.Enabled = true;
            this.mainForm.Success++;

            GetVideoDevice();
        }

        void NetFxTest()
        {
            this.mainForm.lblStatus.Text = "Checking Prerequisites (.NET Frameworks) ...";
            Application.DoEvents();

            bool netfx35 = false;
            bool netfx35sp1 = false;
            bool netfx40 = false;

            netfx35 = RegistryHelper.NetFrameworkInstalled(NETFX.v35);
            netfx35sp1 = RegistryHelper.NetFrameworkInstalled(NETFX.v35, 1);
            netfx40 = RegistryHelper.NetFrameworkInstalled(NETFX.v40);

            if (netfx35 && netfx35sp1)
            {
                this.mainForm.picNet35.Image = Properties.Resources.IconSuccess;
                this.mainForm.lblNetFx35sp1.Visible = false;
                this.mainForm.Success++;
            }
            else if (netfx35)
            {
                this.mainForm.picNet35.Image = Properties.Resources.IconWarning;
                this.mainForm.lblNetFx35sp1.Text = Properties.Resources.TextMissingSP1;
                this.mainForm.lblNetFx35sp1.Visible = true;
                this.mainForm.Warnings++;
            }
            else
            {
                this.mainForm.picNet35.Image = Properties.Resources.IconError;
                this.mainForm.lblNetFx35sp1.Text = Properties.Resources.TextMissing;
                this.mainForm.lblNetFx35sp1.Visible = true;
                this.mainForm.Errors++;
            }

            if (netfx40)
            {
                this.mainForm.picNet40.Image = Properties.Resources.IconSuccess;
                this.mainForm.lblNetFx40.Visible = false;
                this.mainForm.Success++;
            }
            else
            {
                this.mainForm.picNet40.Image = Properties.Resources.IconError;
                this.mainForm.lblNetFx40.Text = Properties.Resources.TextMissing;
                this.mainForm.lblNetFx40.Visible = true;
                this.mainForm.Errors++;
            }
        }

        void VCRTTest()
        {
            this.mainForm.lblStatus.Text = "Checking Prerequisites (Visual C++ Runtimes) ...";
            Application.DoEvents();

            bool vcrt2005 = false;
            bool vcrt2005sp1 = false;
            bool vcrt2008 = false;
            bool vcrt2008sp1 = false;

            vcrt2005 = RegistryHelper.VCRuntimeInstalled(MSVCRT.v2005);
            vcrt2005sp1 = RegistryHelper.VCRuntimeInstalled(MSVCRT.v2005, 1);
            vcrt2008 = RegistryHelper.VCRuntimeInstalled(MSVCRT.v2008);
            vcrt2008sp1 = RegistryHelper.VCRuntimeInstalled(MSVCRT.v2008, 1);

            if (vcrt2005 && vcrt2005sp1)
            {
                this.mainForm.picVC2005.Image = Properties.Resources.IconSuccess;
                this.mainForm.lblVC2005sp1.Visible = false;
                this.mainForm.Success++;
            }
            else if (vcrt2005)
            {
                this.mainForm.picVC2005.Image = Properties.Resources.IconWarning;
                this.mainForm.lblVC2005sp1.Text = Properties.Resources.TextMissingSP1;
                this.mainForm.lblVC2005sp1.Visible = true;
                this.mainForm.Warnings++;
            }
            else
            {
                this.mainForm.picVC2005.Image = Properties.Resources.IconError;
                this.mainForm.lblVC2005sp1.Text = Properties.Resources.TextMissing;
                this.mainForm.lblVC2005sp1.Visible = true;
                this.mainForm.Errors++;
            }

            if (vcrt2008 && vcrt2008sp1)
            {
                this.mainForm.picVC2008.Image = Properties.Resources.IconSuccess;
                this.mainForm.lblVC2008sp1.Visible = false;
                this.mainForm.Success++;
            }
            else if (vcrt2008)
            {
                this.mainForm.picVC2008.Image = Properties.Resources.IconWarning;
                this.mainForm.lblVC2008sp1.Text = Properties.Resources.TextMissingSP1;
                this.mainForm.lblVC2008sp1.Visible = true;
                this.mainForm.Warnings++;
            }
            else
            {
                this.mainForm.picVC2008.Image = Properties.Resources.IconError;
                this.mainForm.lblVC2008sp1.Text = Properties.Resources.TextMissing;
                this.mainForm.lblVC2008sp1.Visible = true;
                this.mainForm.Errors++;
            }
        }
    }
}
