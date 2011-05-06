using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace BFP4F_Troubleshooting
{
    class FileSystemHelper
    {
        public static int CheckHostsFile()
        {
            int result = 0;
            string path = Environment.GetFolderPath(Environment.SpecialFolder.System);

            if (path.EndsWith(@"\") == false)
                path += @"\";
            path += @"drivers\etc\hosts";

            if (File.Exists(path) == false)
                return -1;

            StreamReader tr = new StreamReader(path);
            while (!tr.EndOfStream)
            {
                string line = tr.ReadLine();
                if (line.Contains(".ea.com") && !line.Trim().StartsWith("#"))
                    result++;
            }

            return result;
        }

        public static void OpenHostsFile()
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.System);

            if (path.EndsWith(@"\") == false)
                path += @"\";
            path += @"drivers\etc\hosts";

            if (File.Exists(path) == false)
                return;

            System.Diagnostics.ProcessStartInfo startInfo 
                = new System.Diagnostics.ProcessStartInfo("notepad.exe", path);
            startInfo.Verb = "runas";
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
            startInfo.UseShellExecute = true;

            try
            {
                System.Diagnostics.Process.Start(startInfo);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString());
            }
        }
    }
}
