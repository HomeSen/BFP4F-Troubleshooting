using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace BFP4F_Troubleshooting
{
    public partial class MainForm : Form
    {
        #region Fields

        MainFormController _controller;
        int success = 0;
        int warnings = 0;
        int errors = 0;

        PictureBox[] _serverIcons;
        Label[] _serverLabels;

        #endregion


        #region Properties

        public int Success
        {
            get { return success; }
            set 
            { 
                success = value;
                statusSuccess.Text = success.ToString();
            }
        }

        public int Warnings
        {
            get { return warnings; }
            set 
            {
                warnings = value;
                statusWarning.Text = warnings.ToString(); 
            }
        }

        public int Errors
        {
            get { return errors; }
            set 
            {
                errors = value;
                statusError.Text = errors.ToString();
            }
        }

        public PictureBox[] ServerIcons
        {
            get { return this._serverIcons; }
        }

        public Label[] ServerLabels
        {
            get { return _serverLabels; }
        }

        #endregion


        public MainForm()
        {
            InitializeComponent();

            this._controller = new MainFormController(this);
            InitArrays();

            //this.btnStart_Click(this, new EventArgs());
        }

        private void InitArrays()
        {
            this._serverIcons = new PictureBox[7];
            this._serverIcons[0] = this.picServerMaster;
            this._serverIcons[1] = this.picServerRedirector;
            this._serverIcons[2] = this.picServerDataUSWest;
            this._serverIcons[3] = this.picServerDataUSEast;
            this._serverIcons[4] = this.picServerDataAustralia;
            this._serverIcons[5] = this.picServerDataEurope;
            this._serverIcons[6] = this.picServerCDN;

            this._serverLabels = new Label[7];
            this._serverLabels[0] = this.lblServerMaster;
            this._serverLabels[1] = this.lblServerRedirector;
            this._serverLabels[2] = this.lblServerDataUSWest;
            this._serverLabels[3] = this.lblServerDataUSEast;
            this._serverLabels[4] = this.lblServerDataAustralia;
            this._serverLabels[5] = this.lblServerDataEurope;
            this._serverLabels[6] = this.lblServerCDN;
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            Cursor currentCursor = this.Cursor;
            this.Cursor = Cursors.WaitCursor;

            this.btnStart.Enabled = false;
            this.Success = 0;
            this.Warnings = 0;
            this.Errors = 0;
            this._controller.RunTests();

            this.Cursor = currentCursor;
        }


        #region LinkLabels

        private void lblDriverUrl_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (lblDriverUrl.Text.Trim().StartsWith("http://"))
                System.Diagnostics.Process.Start(lblDriverUrl.Text.Trim());
        }
        
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.codeproject.com/KB/miscctrl/vb6_dotnet_check.aspx");
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://blogs.msdn.com/b/astebner/archive/2007/01/16/mailbag-how-to-detect-the-presence-of-the-vc-8-0-runtime-redistributable-package.aspx#comments");
        }

        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.microsoft.com/downloads/details.aspx?FamilyID=ab99342f-5d1a-413d-8319-81da479ab0d7&displaylang=en");
        }

        private void linkLabel4_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.microsoft.com/downloads/en/details.aspx?FamilyID=9cfb2d51-5ff4-4491-b0e5-b386f32c0992&displaylang=en");
        }

        private void linkLabel5_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.microsoft.com/downloads/en/details.aspx?FamilyID=200B2FD9-AE1A-4A14-984D-389C36F85647");
        }

        private void linkLabel6_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.microsoft.com/downloads/en/details.aspx?FamilyID=9b2da534-3e03-4391-8a4d-074b9f2bc1bf&displaylang=en");
        }

        private void linkLabel7_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.microsoft.com/downloads/en/details.aspx?FamilyID=2da43d38-db71-4c1b-bc6a-9b6652cd92a3&displaylang=en");
        }

        private void linkLabel8_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            FileSystemHelper.OpenHostsFile();
        }

        private void linkLabel9_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://dzaebel.net/VersionInfo.htm");
        }

        private void linkScreenshot_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            FileSystemHelper.OpenScreenshotFolder();
        }

        private void linkShaderCache_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            FileSystemHelper.ClearCacheFolder();
        }

        private void linkDeleteControls_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            FileSystemHelper.DeleteControls();
        }

        private void linkAutoProxy_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            RegistryHelper.DisableAutomaticProxy();
        }

        private void linkLabel8_LinkClicked_1(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("inetcpl.cpl");
        }

        private void linkLabel10_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this._controller.RunPunkbusterService();
        }

        private void linkLabel11_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.evenbalance.com/index.php?page=pbsetup.php");
        }

        #endregion
    }
}
