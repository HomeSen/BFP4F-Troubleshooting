﻿
using System;
using Microsoft.Win32;
namespace BFP4F_Troubleshooting
{
    public enum NETFX
    {
        v20,
        v30,
        v35,
        v40
    }

    public enum MSVCRT
    {
        v2005,
        v2008
    }

    internal class RegistryHelper
    {
        #region Consts

        const string REG_NETFX20 = @"SOFTWARE\Microsoft\NET Framework Setup\NDP\v2.0.50727";
        const string REG_NETFX30 = @"SOFTWARE\Microsoft\NET Framework Setup\NDP\v3.0";
        const string REG_NETFX35 = @"SOFTWARE\Microsoft\NET Framework Setup\NDP\v3.5";
        const string REG_NETFX40 = @"SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Client";

        const string REG_VC2005 = @"SOFTWARE\Microsoft\DevDiv\VC\Servicing\8.0\RED"; // 1st subkey should contain
        const string REG_VC2008 = @"SOFTWARE\Microsoft\DevDiv\VC\Servicing\9.0\RED"; // "Install" = DWORD: 1

        const string REG_VAL_INSTALLED = @"Install"; // should be DWORD: 1
        const string REG_VAL_PATH = @"InstallPath";
        const string REG_VAL_SP = @"SP";

        const string REG_AUTOMATIC_PROXY = @"Software\Microsoft\Windows\CurrentVersion\Internet Settings\Connections";
        const string REG_AUTOMATIC_PROXY_VAL = @"DefaultConnectionSettings"; // subtract 8 from 9th byte to disable

        const string REG_BFP4F = @"SOFTWARE\Electronic Arts\EA Games\Battlefield Play4Free";
        const string REG_BFP4F_VAL = @"Installation Dir";

        #endregion


        #region Frameworks

        public static bool NetFrameworkInstalled(NETFX version)
        {
            return NetFrameworkInstalled(version, 0);
        }

        public static bool NetFrameworkInstalled(NETFX version, int sp_level)
        {
            bool result = false;
            string regPath = "";

            switch (version)
            {
                case NETFX.v20:
                    regPath = REG_NETFX20;
                    break;
                case NETFX.v30:
                    regPath = REG_NETFX30;
                    break;
                case NETFX.v35:
                    regPath = REG_NETFX35;
                    break;
                case NETFX.v40:
                    regPath = REG_NETFX40;
                    break;
                default:
                    return false;
            }

            try
            {
                RegistryKey key = Registry.LocalMachine;
                key = key.OpenSubKey(regPath);
                if (key == null)
                    return false;

                // check if installed
                object value = key.GetValue(REG_VAL_INSTALLED);
                if (value != null)
                    if (int.Parse(value.ToString()) == 1)
                        result = true;

                // check if SP level is equal to/higher than requested
                if (result && (sp_level > 0))
                {
                    value = key.GetValue(REG_VAL_SP);
                    if (value != null)
                        result = (int.Parse(value.ToString()) >= sp_level);
                }
            }
            catch (Exception ex)
            {
                result = false;
                System.Windows.Forms.MessageBox.Show(
                    "An error occured while checking if the .NET Framework " + version.ToString() + " is installed"
                        + Environment.NewLine + Environment.NewLine + ex.ToString(),
                    "Error", 
                    System.Windows.Forms.MessageBoxButtons.OK, 
                    System.Windows.Forms.MessageBoxIcon.Error);
            }

            return result;
        }

        public static bool VCRuntimeInstalled(MSVCRT version)
        {
            return VCRuntimeInstalled(version, 0);
        }

        public static bool VCRuntimeInstalled(MSVCRT version, int sp_level)
        {
            bool result = false;
            string regPath = "";

            switch (version)
            {
                case MSVCRT.v2005:
                    regPath = REG_VC2005;
                    break;
                case MSVCRT.v2008:
                    regPath = REG_VC2008;
                    break;
                default:
                    return false;
            }

            try
            {
                RegistryKey key = Registry.LocalMachine;
                key = key.OpenSubKey(regPath);
                if (key == null)
                    return false;
                
                // Move one subkey down, since the redistributable is localized
                key = key.OpenSubKey(key.GetSubKeyNames()[0]);

                // check if installed
                object value = key.GetValue(REG_VAL_INSTALLED);
                if (value != null)
                    if (int.Parse(value.ToString()) == 1)
                        result = true;

                // check if SP level is equal to/higher than requested
                if (result && (sp_level > 0))
                {
                    value = key.GetValue(REG_VAL_SP);
                    if (value != null)
                        result = (int.Parse(value.ToString()) >= sp_level);
                }
            }
            catch (Exception ex)
            {
                result = false;
                System.Windows.Forms.MessageBox.Show(
                    "An error occured while checking if the Visual C++ Runtime " + version.ToString() + " is installed"
                        + Environment.NewLine + Environment.NewLine + ex.ToString(),
                    "Error",
                    System.Windows.Forms.MessageBoxButtons.OK,
                    System.Windows.Forms.MessageBoxIcon.Error);
            }

            return result;
        }

        #endregion


        #region InternetSettings

        public static void DisableAutomaticProxy()
        {
            try
            {
                RegistryKey key = Registry.CurrentUser;
                key = key.OpenSubKey(REG_AUTOMATIC_PROXY, true);
                byte[] value = (byte[])key.GetValue(REG_AUTOMATIC_PROXY_VAL);
                if (value[8] > 8)
                {
                    value[8] -= 8;
                    key.SetValue(REG_AUTOMATIC_PROXY_VAL, value);
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(
                    "An error occured while trying to disable \"Automatic Proxy Discovery\""
                        + Environment.NewLine + Environment.NewLine + ex.ToString(),
                    "Error",
                    System.Windows.Forms.MessageBoxButtons.OK,
                    System.Windows.Forms.MessageBoxIcon.Error);
            }
        }

        #endregion


        #region GamePath

        public static string GetGamePath()
        {
            string result = "";

            try
            {
                RegistryKey key = Registry.LocalMachine;
                key = key.OpenSubKey(REG_BFP4F);
                if (key == null)
                    return String.Empty;

                object value = key.GetValue(REG_BFP4F_VAL);
                if (value == null)
                    result = String.Empty;
                else
                    result = value.ToString();

                key.Close();
            }
            catch { }

            return result;
        }

        #endregion
    }
}
