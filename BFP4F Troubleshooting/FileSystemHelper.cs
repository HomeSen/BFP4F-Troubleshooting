using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace BFP4F_Troubleshooting
{
    class FileSystemHelper
    {
        #region Hosts file

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

        #endregion


        #region Game files

        private static string GetUserFolderPath()
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (path.EndsWith(@"\") == false)
                path += @"\";
            path += @"Battlefield Play4Free";

            if (Directory.Exists(path) == false)
                Directory.CreateDirectory(path);
            
            return path;
        }

        public static void OpenScreenshotFolder()
        {
            string path = GetUserFolderPath();
            path += @"\Screenshots";

            if (Directory.Exists(path) == false)
                Directory.CreateDirectory(path);

            System.Diagnostics.Process.Start(path);
        }

        public static void ClearCacheFolder()
        {
            string path = GetUserFolderPath();
            path += @"\mods\main\cache";

            if (Directory.Exists(path) == false)
                return;
            try
            {
                Directory.Delete(path, true);
            }
            catch { }

            if (Directory.Exists(path) == false)
                Directory.CreateDirectory(path);
        }

        public static void DeleteControls()
        {
            string path = GetUserFolderPath();
            path += @"\Controls.con";

            if (File.Exists(path) == false)
                return;
            try
            {
                File.Move(path, path + "." + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
            }
            catch { }
        }

        #endregion
    }
}
