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

        #endregion


        public MainForm()
        {
            InitializeComponent();

            this._controller = new MainFormController(this);
            //this.btnStart_Click(this, new EventArgs());
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            this.Success = 0;
            this.Warnings = 0;
            this.Errors = 0;
            this._controller.RunTests();
        }

        private void lblPixelShader_TextChanged(object sender, EventArgs e)
        {
            picPixelShader.Left = (lblPixelShader.Location.X + lblPixelShader.Width + 6);
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

        #endregion
    }
}
