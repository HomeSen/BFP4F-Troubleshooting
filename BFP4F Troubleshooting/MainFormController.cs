using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace BFP4F_Troubleshooting
{
    class MainFormController
    {
        #region Fields

        MainForm _mainForm = null;
        SystemHelper _systemHelper = null;
        bool _gameInstalled = false;

        const int MIN_PIXELSHADER = 2;
        const int MIN_VIDEOMEMORY = 256;
        const int MIN_CPU = 1700;
        const int MIN_RAM = 512; 
        
        #endregion


        #region Constructor

        public MainFormController(Form mainForm)
        {
            this._mainForm = (MainForm)mainForm;

            string path = RegistryHelper.GetGamePath();
            if (String.IsNullOrEmpty(path))
            {
                this._mainForm.statusGamePath.Text = Properties.Resources.TextNotFound;
                this._mainForm.statusGamePath.IsLink = false;
                this._gameInstalled = false;
            }
            else
            {
                this._mainForm.statusGamePath.Text = path;
                this._mainForm.statusGamePath.IsLink = true;
                this._mainForm.statusGamePath.Click += new EventHandler(delegate(object o, EventArgs e)
                    { System.Diagnostics.Process.Start(path); });
                this._gameInstalled = true;
            }
        }

        #endregion
        

        public void RunTests()
        {
            // create a new SystemHelper object and "destroy" the old one
            if (this._systemHelper != null)
                this._systemHelper = null;
            this._systemHelper = new SystemHelper(this._mainForm);

            NetFxTest();
            VCRTTest();
            DirectXTest();
            ConnectionTest();
            HardwareTest();

            this._mainForm.lblStatus.Text = "Finished testing.";
            Application.DoEvents();
        }


        #region Hardware Tests

        private void GetVideoDevice()
        {
            this._mainForm.lblStatus.Text = "Checking Hardware (Video device) ...";
            Application.DoEvents();

            this._mainForm.lblDeviceName.Text = this._systemHelper.GetDeviceName();
            string[] vendor = this._systemHelper.GetDeviceVendor();
            if (vendor.Length == 2)
            {
                this._mainForm.lblDeviceVendor.Text = vendor[0];
                this._mainForm.lblDriverUrl.Text = vendor[1];
                this._mainForm.Success++;
            }
            else
            {
                this._mainForm.lblDeviceVendor.Text = Properties.Resources.TextUnknown;
                this._mainForm.lblDriverUrl.Text = Properties.Resources.TextUnknown;
                this._mainForm.Errors++;
            }

            // Get driver version and date
            this._mainForm.lblDriverDate.Text = this._systemHelper.GetDriverVersion();
            DateTime driverDate = this._systemHelper.GetDriverDate();
            if (driverDate != DateTime.MinValue)
            {
                this._mainForm.lblDriverDate.Text += " (" + driverDate.ToShortDateString() + ")";
            }

            // Get max. supported PixelShader version
            Version version = this._systemHelper.GetPSVersion();
            if (version != null)
            {
                this._mainForm.lblPixelShader.Text = version.ToString();
                if (version.Major < MIN_PIXELSHADER)
                {
                    this._mainForm.picPixelShader.Image = Properties.Resources.IconError;
                    this._mainForm.picPixelShader.BorderStyle = BorderStyle.None;
                    this._mainForm.Errors++;
                }
                else
                {
                    this._mainForm.picPixelShader.Image = Properties.Resources.IconSuccess;
                    this._mainForm.picPixelShader.BorderStyle = BorderStyle.None;
                    this._mainForm.Success++;
                }
            }
            else
            {
                this._mainForm.lblPixelShader.Text = Properties.Resources.TextUnknown;
                this._mainForm.picPixelShader.Image = Properties.Resources.IconWarning;
                this._mainForm.picPixelShader.BorderStyle = BorderStyle.None;
                this._mainForm.Warnings++;
            }

            // Get (combined) video memory
            uint videoMemory = this._systemHelper.GetVideoMemory();
            this._mainForm.lblVideoMem.Text = videoMemory.ToString("#,##0") + " MB";
            if (videoMemory > MIN_VIDEOMEMORY)
            {
                this._mainForm.picVideoMem.Image = Properties.Resources.IconSuccess;
                this._mainForm.picVideoMem.BorderStyle = BorderStyle.None;
                this._mainForm.Success++;
            }
            else if (videoMemory == MIN_VIDEOMEMORY)
            {
                this._mainForm.picVideoMem.Image = Properties.Resources.IconWarning;
                this._mainForm.picVideoMem.BorderStyle = BorderStyle.None;
                this._mainForm.Warnings++;
            }
            else
            {
                this._mainForm.picVideoMem.Image = Properties.Resources.IconError;
                this._mainForm.picVideoMem.BorderStyle = BorderStyle.None;
                this._mainForm.Errors++;
            }

            // Get (dedicated) video memory
            videoMemory = this._systemHelper.GetDedicatedVideoMemory();
            this._mainForm.lblVideoMemDedicated.Text = videoMemory.ToString("#,##0") + " MB";
        }

        private void HardwareTest()
        {
            this._mainForm.lblStatus.Text = "Checking Hardware (CPU + RAM) ...";
            Application.DoEvents();

            string windowsVersion = this._systemHelper.GetWindowsVersion();
            this._mainForm.lblOperatingSystem.Text = windowsVersion;

            uint cpuSpeed = this._systemHelper.GetCPUSpeed();
            if (cpuSpeed > 0)
            {
                this._mainForm.lblCPU.Text = cpuSpeed.ToString("#,##0") + " MHz";
                uint logicalCpus = this._systemHelper.GetLogicalCPUCount();
                if (logicalCpus > 0)
                {
                    this._mainForm.lblCPU.Text += " (" + logicalCpus + " cores)";
                }

                if ((cpuSpeed <= (MIN_CPU * 1.25)) && (logicalCpus <= 1))
                {
                    this._mainForm.picCPU.Image = Properties.Resources.IconWarning;
                    this._mainForm.picCPU.BorderStyle = BorderStyle.None;
                    this._mainForm.Warnings++;
                }
                else
                {
                    this._mainForm.picCPU.Image = Properties.Resources.IconSuccess;
                    this._mainForm.picCPU.BorderStyle = BorderStyle.None;
                    this._mainForm.Success++;
                }
            }
            else
            {
                this._mainForm.picCPU.Image = Properties.Resources.IconError;
                this._mainForm.picCPU.BorderStyle = BorderStyle.None;
                this._mainForm.lblCPU.Text = Properties.Resources.TextError;
                this._mainForm.Errors++;
            }

            double memory = this._systemHelper.GetMemory();
            if (memory > 0)
            {
                this._mainForm.lblRAM.Text = memory.ToString("#,##0.00") + " MB";

                if (memory < MIN_RAM)
                {
                    this._mainForm.picRAM.Image = Properties.Resources.IconError;
                    this._mainForm.picRAM.BorderStyle = BorderStyle.None;
                    this._mainForm.Errors++;
                }
                else if ((memory >= MIN_RAM) && (memory <= (MIN_RAM * 2)))
                {
                    this._mainForm.picRAM.Image = Properties.Resources.IconWarning;
                    this._mainForm.picRAM.BorderStyle = BorderStyle.None;
                    this._mainForm.Warnings++;
                }
                else
                {
                    this._mainForm.picRAM.Image = Properties.Resources.IconSuccess;
                    this._mainForm.picRAM.BorderStyle = BorderStyle.None;
                    this._mainForm.Success++;
                }
            }
            else
            {
                this._mainForm.picRAM.Image = Properties.Resources.IconError;
                this._mainForm.picRAM.BorderStyle = BorderStyle.None;
                this._mainForm.lblRAM.Text = Properties.Resources.TextError;
                this._mainForm.Errors++;
            }
        }

        #endregion


        #region Connection Tests

        private void ConnectionTest()
        {
            CheckHostsFile();
            PingServers();
        }

        private void PingServers()
        {
            this._mainForm.lblStatus.Text = "Checking Connection (pinging servers) ...";
            Application.DoEvents();

            long replyTime = 0;

            for (int i = 0; i < NetworkHelper.servers.Length; i++)
            {
                replyTime = NetworkHelper.PingServer(NetworkHelper.servers[i]);
                if (replyTime < 0)
                {
                    this._mainForm.ServerIcons[i].Image = Properties.Resources.IconError;
                    this._mainForm.ServerIcons[i].BorderStyle = BorderStyle.None;
                    this._mainForm.ServerLabels[i].Text = Properties.Resources.TextError;
                    this._mainForm.Errors++;
                }
                else if (replyTime == 0)
                {
                    this._mainForm.ServerIcons[i].Image = Properties.Resources.IconWarning;
                    this._mainForm.ServerIcons[i].BorderStyle = BorderStyle.None;
                    this._mainForm.ServerLabels[i].Text = Properties.Resources.TextTimeOut;
                    this._mainForm.Warnings++;
                }
                else
                {
                    this._mainForm.ServerIcons[i].Image = Properties.Resources.IconSuccess;
                    this._mainForm.ServerIcons[i].BorderStyle = BorderStyle.None;
                    this._mainForm.ServerLabels[i].Text = replyTime + " ms";
                    this._mainForm.Success++;
                }
            }
        }

        private void CheckHostsFile()
        {
            this._mainForm.lblStatus.Text = "Checking Connection (hosts file) ...";
            Application.DoEvents();

            int hosts = FileSystemHelper.CheckHostsFile();
            if (hosts == 0)
            {
                this._mainForm.picHosts.Image = Properties.Resources.IconSuccess;
                this._mainForm.picHosts.BorderStyle = BorderStyle.None;
                this._mainForm.lblHosts.Visible = false;
                this._mainForm.linkHosts.Enabled = true;
                this._mainForm.Success++;
            }
            else if (hosts == -1)
            {
                this._mainForm.picHosts.Image = Properties.Resources.IconWarning;
                this._mainForm.picHosts.BorderStyle = BorderStyle.None;
                this._mainForm.lblHosts.Text = "Could not find hosts file";
                this._mainForm.linkHosts.Enabled = false;
                this._mainForm.Warnings++;
            }
            else
            {
                this._mainForm.picHosts.Image = Properties.Resources.IconWarning;
                this._mainForm.picHosts.BorderStyle = BorderStyle.None;
                this._mainForm.lblHosts.Text = hosts + " suspicious entries found";
                this._mainForm.lblHosts.Visible = true;
                this._mainForm.linkHosts.Enabled = true;
                this._mainForm.Warnings++;
            }
        }

        #endregion


        #region Prerequisites Tests

        void DirectXTest()
        {
            this._mainForm.lblStatus.Text = "Checking Prerequisites (DirectX) ...";
            Application.DoEvents();

            if (this._systemHelper.DxInstalled == false)
            {
                this._mainForm.picDX.Image = Properties.Resources.IconError;
                this._mainForm.picDX.BorderStyle = BorderStyle.None;
                this._mainForm.lblDriverUrl.Enabled = false;
                this._mainForm.Errors++;
                //return;
            }

            this._mainForm.picDX.Image = Properties.Resources.IconSuccess;
            this._mainForm.picDX.BorderStyle = BorderStyle.None;
            this._mainForm.lblDriverUrl.Enabled = true;
            this._mainForm.Success++;

            GetVideoDevice();
        }

        void NetFxTest()
        {
            this._mainForm.lblStatus.Text = "Checking Prerequisites (.NET Frameworks) ...";
            Application.DoEvents();

            bool netfx35 = false;
            bool netfx35sp1 = false;
            bool netfx40 = false;

            netfx35 = RegistryHelper.NetFrameworkInstalled(NETFX.v35);
            netfx35sp1 = RegistryHelper.NetFrameworkInstalled(NETFX.v35, 1);
            netfx40 = RegistryHelper.NetFrameworkInstalled(NETFX.v40);

            if (netfx35 && netfx35sp1)
            {
                this._mainForm.picNet35.Image = Properties.Resources.IconSuccess;
                this._mainForm.picNet35.BorderStyle = BorderStyle.None;
                this._mainForm.lblNetFx35sp1.Visible = false;
                this._mainForm.Success++;
            }
            else if (netfx35)
            {
                this._mainForm.picNet35.Image = Properties.Resources.IconWarning;
                this._mainForm.picNet35.BorderStyle = BorderStyle.None;
                this._mainForm.lblNetFx35sp1.Text = Properties.Resources.TextMissingSP1;
                this._mainForm.lblNetFx35sp1.Visible = true;
                this._mainForm.Warnings++;
            }
            else
            {
                this._mainForm.picNet35.Image = Properties.Resources.IconError;
                this._mainForm.picNet35.BorderStyle = BorderStyle.None;
                this._mainForm.lblNetFx35sp1.Text = Properties.Resources.TextMissing;
                this._mainForm.lblNetFx35sp1.Visible = true;
                this._mainForm.Errors++;
            }

            if (netfx40)
            {
                this._mainForm.picNet40.Image = Properties.Resources.IconSuccess;
                this._mainForm.picNet40.BorderStyle = BorderStyle.None;
                this._mainForm.lblNetFx40.Visible = false;
                this._mainForm.Success++;
            }
            else
            {
                this._mainForm.picNet40.Image = Properties.Resources.IconError;
                this._mainForm.picNet40.BorderStyle = BorderStyle.None;
                this._mainForm.lblNetFx40.Text = Properties.Resources.TextMissing;
                this._mainForm.lblNetFx40.Visible = true;
                this._mainForm.Errors++;
            }
        }

        void VCRTTest()
        {
            this._mainForm.lblStatus.Text = "Checking Prerequisites (Visual C++ Runtimes) ...";
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
                this._mainForm.picVC2005.Image = Properties.Resources.IconSuccess;
                this._mainForm.picVC2005.BorderStyle = BorderStyle.None;
                this._mainForm.lblVC2005sp1.Visible = false;
                this._mainForm.Success++;
            }
            else if (vcrt2005)
            {
                this._mainForm.picVC2005.Image = Properties.Resources.IconWarning;
                this._mainForm.picVC2005.BorderStyle = BorderStyle.None;
                this._mainForm.lblVC2005sp1.Text = Properties.Resources.TextMissingSP1;
                this._mainForm.lblVC2005sp1.Visible = true;
                this._mainForm.Warnings++;
            }
            else
            {
                this._mainForm.picVC2005.Image = Properties.Resources.IconError;
                this._mainForm.picVC2005.BorderStyle = BorderStyle.None;
                this._mainForm.lblVC2005sp1.Text = Properties.Resources.TextMissing;
                this._mainForm.lblVC2005sp1.Visible = true;
                this._mainForm.Errors++;
            }

            if (vcrt2008 && vcrt2008sp1)
            {
                this._mainForm.picVC2008.Image = Properties.Resources.IconSuccess;
                this._mainForm.picVC2008.BorderStyle = BorderStyle.None;
                this._mainForm.lblVC2008sp1.Visible = false;
                this._mainForm.Success++;
            }
            else if (vcrt2008)
            {
                this._mainForm.picVC2008.Image = Properties.Resources.IconWarning;
                this._mainForm.picVC2008.BorderStyle = BorderStyle.None;
                this._mainForm.lblVC2008sp1.Text = Properties.Resources.TextMissingSP1;
                this._mainForm.lblVC2008sp1.Visible = true;
                this._mainForm.Warnings++;
            }
            else
            {
                this._mainForm.picVC2008.Image = Properties.Resources.IconError;
                this._mainForm.picVC2008.BorderStyle = BorderStyle.None;
                this._mainForm.lblVC2008sp1.Text = Properties.Resources.TextMissing;
                this._mainForm.lblVC2008sp1.Visible = true;
                this._mainForm.Errors++;
            }
        }

        #endregion


        #region Miscellaneous

        public void RunPunkbusterService()
        {
            string path = "";

            if (this._gameInstalled == true)
            {
                path = this._mainForm.statusGamePath.Text;
                if (path.EndsWith(@"\") == false)
                    path += @"\";
                path += "pbsvc_p4f.exe";
            }
            
            if ((System.IO.File.Exists(path) == false) || (this._gameInstalled == false))
            {
                this._mainForm.lblStatus.Text = "Downloading \"pbsvc.exe\" ...";

                path = Environment.GetEnvironmentVariable("TEMP");
                if (path.EndsWith(@"\") == false)
                    path += @"\";
                path += "pbsvc.exe";

                this._mainForm.lblStatus.Text = "Finished downloading.";

                if (NetworkHelper.DownloadPbSvc(path) == false)
                    return;
            }

            this._mainForm.lblStatus.Text = "Starting pbsvc.exe ...";
            System.Diagnostics.Process.Start(path);
            this._mainForm.lblStatus.Text = "Finished starting \"pbsvc.exe\" ...";
        }

        public void OpenPbcl()
        {
            string path = "";
            if (this._gameInstalled)
            {
                path = RegistryHelper.GetGamePath();
            }
            else
            {
                DialogResult result = MessageBox.Show("It seems like the game is not installed."
                    + Environment.NewLine + Environment.NewLine
                    + "Do you want to manually select the folder where you installed the game to?",
                    "Not installed", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.No)
                    return;

                result = this._mainForm.folderBrowserDialog1.ShowDialog(this._mainForm);
                if (result == DialogResult.OK)
                    path = this._mainForm.folderBrowserDialog1.SelectedPath;
                else
                    return;
            }

            path = FileSystemHelper.GetPbclPath(path);
            if (String.IsNullOrEmpty(path))
            {
                MessageBox.Show("Could not find PunkBuster logfile: pbcl.log", "Error");
                return;
            }

            System.Diagnostics.Process.Start("notepad.exe", path);
        }

        #endregion
    }
}
