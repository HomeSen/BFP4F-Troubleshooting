using System;
using System.Windows.Forms;

namespace BFP4F_Troubleshooting
{
    static class Program
    {
        internal static bool dxFailed = false;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            AppDomain.CurrentDomain.AssemblyResolve += CustomResolve;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }

        private static System.Reflection.Assembly CustomResolve(object sender, ResolveEventArgs args)
        {
            if (args.Name.Contains("DirectX"))
            {
                dxFailed = true;
                MessageBox.Show("Failed to load DirectX, please install the latest version and run this program again.",
                    "DirectX failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return null;
        }
    }
}
