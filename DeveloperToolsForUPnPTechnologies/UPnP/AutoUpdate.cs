using System;
using System.IO;
using System.Data;
using System.Text;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;
using Microsoft.Win32;

namespace OpenSource.UPnP
{
    public partial class AutoUpdate : Form
    {
        public static int existingversion = 0;
        public static Guid updatecode = Guid.Empty;
        private static string ShortProductName = null;
        private static string RegistryKey = "UpdateCheck";
        private static HttpRequestor requestor = null;
        private static AutoUpdate updateform = null;
        private static Form parentform = null;
        private static string GetVersionStr(int ver) { return string.Format("{0}.{1}.{2}", (ver / 10000) % 100, (ver / 100) % 100, ver % 100); }
        private static string updatelink = null;
        private int newversion = 0;
        private static string TempFolder { get { if (ShortProductName == null) return "OpenToolsTemp"; else return ShortProductName.Replace(" ", ""); } }
        private static string sitelink = null;

        public static void ShowMainSite()
        {
            if (sitelink == null) ReadUpdateSettings();
            if (sitelink != null)
            {
                try { System.Diagnostics.Process.Start(sitelink); }
                catch (System.ComponentModel.Win32Exception ex) 
                {
                    OpenSource.Utilities.EventLogger.Log(ex);
                }
            }
            else
            {
                try { System.Diagnostics.Process.Start("http://opentools.homeip.net"); }
                catch (System.ComponentModel.Win32Exception ex) 
                {
                    OpenSource.Utilities.EventLogger.Log(ex);
                }
            }
        }

        private static string VersionStringCache = null;
        public static string VersionString
        {
            get
            {
                if (VersionStringCache != null) return VersionStringCache;
                if (existingversion == 0) ReadUpdateSettings();
                VersionStringCache = GetVersionStr(existingversion);
                return VersionStringCache;
            }
        }

        private static bool languageSelection = true;
        public static bool LanguageSelection { get { if (!ReadUpdateSettingsCompleted) ReadUpdateSettings(); return languageSelection; } }
        private static bool allowAutoUpdate = true;
        public static bool AllowAutoUpdate { get { if (!ReadUpdateSettingsCompleted) ReadUpdateSettings(); return allowAutoUpdate; } }

        public AutoUpdate(int newversion)
        {
            this.newversion = newversion;
            InitializeComponent();
        }

        private void MeshToolsUpdate_Load(object sender, EventArgs e)
        {
            updateCheckBox.Checked = GetAutoUpdateCheck();
            currentVersionLabel.Text = "v" + GetVersionStr(existingversion);
            newVersionLabel.Text = "v" + GetVersionStr(newversion);
        }

        private static bool ReadUpdateSettingsCompleted = false;
        private static bool ReadUpdateSettings()
        {
            if (File.Exists(System.Windows.Forms.Application.StartupPath + "\\UpdateInfo.ini") == false) return false;

            // Fetch the service settings
            string[] settings = File.ReadAllLines(System.Windows.Forms.Application.StartupPath + "\\UpdateInfo.ini", Encoding.UTF8);
            foreach (string s in settings)
            {
                string[] nv = s.Split('=');
                if (nv.Length == 2)
                {
                    if (nv[0].ToLower().Equals("productname")) ShortProductName = nv[1];
                    if (nv[0].ToLower().Equals("updatecode")) updatecode = new Guid(nv[1]);
                    if (nv[0].ToLower().Equals("currentversion")) int.TryParse(nv[1], out existingversion);
                    if (nv[0].ToLower().Equals("updatelink")) updatelink = nv[1];
                    if (nv[0].ToLower().Equals("sitelink")) sitelink = nv[1];
                    if (nv[0].ToLower().Equals("autoupdate")) allowAutoUpdate = (nv[1] == "1");
                    if (nv[0].ToLower().Equals("languageselect")) languageSelection = (nv[1] == "1");
                }
            }

            ReadUpdateSettingsCompleted = true;
            if (updatecode == Guid.Empty) return false;
            if (existingversion <= 0) return false;
            if (updatelink == null) return false;
            return true;
        }

        public static void UpdateCheck(Form parent)
        {
            if (requestor != null) return;
            if (File.Exists(Application.StartupPath + "\\AutoUpdateTool.exe") == false) return;
            if (ReadUpdateSettings() == false) return;
            if (AllowAutoUpdate == false) return;

            parentform = parent;
            requestor = new HttpRequestor();
            requestor.OnRequestCompleted += new HttpRequestor.RequestCompletedHandler(requestor_OnRequestCompleted);
            requestor.LaunchProxyRequest(updatelink, null, 1);
        }

        public static RegistryKey GetGlobalRegKey()
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey("Software\\Open Source", true);
            if (key == null)
            {
                RegistryKey skey = Registry.CurrentUser.OpenSubKey("Software", true);
                if (skey == null) return null;
                key = skey.CreateSubKey("Open Source");
            }
            return key;
        }

        public static void SetAutoUpdateCheck(bool autoupdate)
        {
            RegistryKey key = GetGlobalRegKey();
            if (key == null) return;
            if (autoupdate)
            {
                key.SetValue(RegistryKey, DateTime.MinValue.ToString());
            }
            else
            {
                key.SetValue(RegistryKey, DateTime.MaxValue.ToString());
            }
            key.Close();
        }

        public static void SetAutoUpdateCheckNow(bool autoupdate)
        {
            RegistryKey key = GetGlobalRegKey();
            if (key == null) return;
            if (autoupdate)
            {
                key.SetValue(RegistryKey, DateTime.Now.ToString());
            }
            else
            {
                key.SetValue(RegistryKey, DateTime.MaxValue.ToString());
            }
            key.Close();
        }

        public static bool GetAutoUpdateCheck()
        {
            RegistryKey key = GetGlobalRegKey();
            if (key == null) return false;
            string updatecheck = (string)key.GetValue(RegistryKey, null);
            DateTime lastcheck = DateTime.MinValue;
            if (updatecheck != null) lastcheck = DateTime.Parse(updatecheck);
            key.Close();
            return (lastcheck.Year < 9000);
        }

        public static void AutoUpdateCheck(Form parent)
        {
            // Delete the temp folder if present
            if (AllowAutoUpdate == false) return;
            if (!File.Exists(Application.StartupPath + "\\AutoUpdateTool.exe")) return;
            string tpath = Path.GetTempPath();
            try
            {
                if (Directory.Exists(tpath + TempFolder)) Directory.Delete(tpath + TempFolder, true);
            }
            catch (Exception ex) 
            {
                OpenSource.Utilities.EventLogger.Log(ex);
            } // In some cases, we try to delete too quickly and this will fail. It's ok, not critical.

            // See if we need to perform a version check
            RegistryKey key = GetGlobalRegKey();
            if (key == null) return;
            string updatecheck = (string)key.GetValue(RegistryKey, null);
            DateTime lastcheck = DateTime.MinValue;
            if (updatecheck != null) { DateTime x; if (DateTime.TryParse(updatecheck, out x)) lastcheck = x; }
            if (lastcheck.Year < 9000)
            {
                if (lastcheck.AddDays(1).CompareTo(DateTime.Now) < 0)
                {
                    key.SetValue(RegistryKey, DateTime.Now.ToString());
                    UpdateCheck(parent);
                }
            }
            key.Close();
        }

        private static void ShowUpdateForm()
        {
            updateform.ShowDialog(parentform);
        }

        private static void requestor_OnRequestCompleted(HttpRequestor sender, bool success, object tag, string url, byte[] data)
        {
            if (success == true)
            {
                // Fetch version information
                int newversion = 0;
                string updatelink = null;
                try
                {
                    string page = UTF8Encoding.UTF8.GetString(data);
                    string x = "##" + updatecode.ToString() + "##";
                    x = x.ToUpper();
                    int i = page.IndexOf(x);
                    if (i == -1)
                    {
                        requestor.OnRequestCompleted += new HttpRequestor.RequestCompletedHandler(requestor_OnRequestCompleted);
                        requestor = null;
                        return;
                    }
                    page = page.Substring(i + x.Length);
                    i = page.IndexOf("##");
                    if (i == -1)
                    {
                        requestor.OnRequestCompleted += new HttpRequestor.RequestCompletedHandler(requestor_OnRequestCompleted);
                        requestor = null;
                        return;
                    }
                    string versionstr = page.Substring(0, i);
                    newversion = int.Parse(versionstr);
                    page = page.Substring(i + 2);
                    i = page.IndexOf("##");
                    if (i == -1)
                    {
                        requestor.OnRequestCompleted += new HttpRequestor.RequestCompletedHandler(requestor_OnRequestCompleted);
                        requestor = null;
                        return;
                    }
                    updatelink = page.Substring(0, i);
                }
                catch (Exception ex)
                {
                    OpenSource.Utilities.EventLogger.Log(ex);
                    requestor.OnRequestCompleted += new HttpRequestor.RequestCompletedHandler(requestor_OnRequestCompleted);
                    requestor = null;
                    return;
                }

                // Compare to existing version
                if (existingversion < newversion)
                {
                    updateform = new AutoUpdate(newversion);
                    parentform.Invoke(new System.Threading.ThreadStart(ShowUpdateForm));
                }
            }
            requestor.OnRequestCompleted += new HttpRequestor.RequestCompletedHandler(requestor_OnRequestCompleted);
            requestor = null;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            // Cancel
            Close();
        }

        public static void ForceUpdate()
        {
            String tpath = Path.GetTempPath();
            Directory.CreateDirectory(tpath + TempFolder);
            tpath += (TempFolder + "\\");
            File.Copy(Application.StartupPath + "\\AutoUpdateTool.exe", tpath + "AutoUpdateTool.exe", true);
            File.Copy(Application.StartupPath + "\\Interop.WindowsInstaller.dll", tpath + "Interop.WindowsInstaller.dll", true);
            File.Copy(Application.StartupPath + "\\UpdateInfo.ini", tpath + "UpdateInfo.ini", true);

            // Update
            try
            {
                string args = string.Format("-g:{0} -t:\"{1}\" -r:\"{2}\"", updatecode.ToString(), ShortProductName, Application.ExecutablePath);
                ProcessStartInfo startInfo = new ProcessStartInfo(tpath + "AutoUpdateTool.exe", args);
                Process process = Process.Start(startInfo);
            }
            catch (Exception ex)
            {
                OpenSource.Utilities.EventLogger.Log(ex);
                return;
            }
            Application.Exit();
        }

        private void updateButton_Click(object sender, EventArgs e)
        {
            ForceUpdate();
        }

        private void updateCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            SetAutoUpdateCheckNow(updateCheckBox.Checked);
        }

        public static void ReportCrash(string app, string message)
        {
            if (existingversion == 0) ReadUpdateSettings();
            string url = string.Format("http://meshcentral.homeip.net/crashpost.ashx?updatecode={0}&version={1}&app={2}", updatecode, existingversion, app);
            //string url = string.Format("http://localhost/MeshCentral/crashpost.ashx?updatecode={0}&version={1}&app={2}", updatecode, existingversion, app);
            new HttpRequestor().LaunchProxyRequest(url, UTF8Encoding.UTF8.GetBytes(message), null);
        }

        public static void ReportCrash(string app, Exception ex)
        {
            if (existingversion == 0) ReadUpdateSettings();
            string url = string.Format("http://meshcentral.homeip.net/crashpost.ashx?updatecode={0}&version={1}&app={2}", updatecode, existingversion, app);
            //string url = string.Format("http://localhost/MeshCentral/crashpost.ashx?updatecode={0}&version={1}&app={2}", updatecode, existingversion, app);

            StringBuilder sb = new StringBuilder();
            sb.Append(ex.ToString());
            if (ex.Data.Count > 0) sb.Append("\r\n\r\n--- Extra Data ---\r\n");
            {
                foreach (object key in ex.Data.Keys)
                {
                    string va = null;
                    if (ex.Data[key] == null) continue;
                    if (ex.Data[key].GetType() == typeof(byte[]))
                    {
                        va = UTF8Encoding.UTF8.GetString((byte[])ex.Data[key]);
                    }
                    else
                    {
                        va = ex.Data[key].ToString();
                    }

                    sb.AppendFormat("{0} = {1}\r\n", key.ToString(), va.ToString());
                }
            }

            new HttpRequestor().LaunchProxyRequest(url, UTF8Encoding.UTF8.GetBytes(sb.ToString()), null);
        }
    }
}
