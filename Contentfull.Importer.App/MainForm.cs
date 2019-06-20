using Contentful.Importer.App.Interfaces;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace Contentful.Importer.App
{
    public partial class MainForm : Form
    {
        List<ContentTypeGrid> grids = new List<ContentTypeGrid>();
        public MainForm()
        {

            InitializeComponent();
            //SetIEVersion();
            //if (!WBEmulator.IsBrowserEmulationSet())
            //{
            //    WBEmulator.SetBrowserEmulationVersion();
            //}
            Load += MainForm_Load;
        }
        private string AuthToken { get; set; }
        private void MainForm_Load(object sender, EventArgs e)
        {
            this.Hide();
            //var auth = new oAuthForm();
            //var result = auth.ShowDialog(this);
            var auth = new CapturePermaTokenForm();
            var result = auth.ShowDialog(this);
            if (result == DialogResult.OK && !string.IsNullOrEmpty(auth.ResultCode))
            {
                AuthToken = auth.ResultCode;
                lblLogin.Text = Library.ContentFul.Client.SelectedUsername;

                selectCountry.DataSource = Library.ContentFul.Client.AvailableCountries;
                selectCountry.SelectedIndex = 0;
                LoadTabs();

                this.Show();
                Program.SplashScreen.Hide();
            }
            else
            {
                this.Close();
            }

        }

        public void LoadTabs()
        {

            foreach (var ctype in Library.ContentFul.Client.ContentTypes.Where(p => !p.Label.StartsWith("_")).OrderBy(p=>p.Label))
            {                
                tabs.TabPages.Add(ctype.TypeID, ctype.Label);
                var grid = new Interfaces.ContentTypeGrid();
                grids.Add(grid);
                grid.SetContentfulType(ctype);
                grid.Dock = DockStyle.Fill;
                tabs.TabPages[ctype.TypeID].Controls.Add(grid);
            }
            grids[0].LoadDataRows();
        }

        #region Browsercontrolfix

        public void SetIEVersion()
        {
            var appName = System.Diagnostics.Process.GetCurrentProcess().ProcessName + ".exe";

            RegistryKey Regkey = null;
            try
            {
                // For 64 bit machine
                if (Environment.Is64BitOperatingSystem)
                {
                    Regkey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\\Wow6432Node\\Microsoft\\Internet Explorer\\MAIN\\FeatureControl\\FEATURE_BROWSER_EMULATION", true);
                }
                else  //For 32 bit machine
                {
                    Regkey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\\Microsoft\\Internet Explorer\\Main\\FeatureControl\\FEATURE_BROWSER_EMULATION", true);
                }

                // If the path is not correct or
                // if the user haven't priviledges to access the registry
                if (Regkey == null)
                {
                    MessageBox.Show("Application Settings Failed - Address Not found");
                    return;
                }

                string FindAppkey = Convert.ToString(Regkey.GetValue(appName));

                // Check if key is already present
                if (FindAppkey == "8000")
                {
                    //MessageBox.Show("Required Application Settings Present");
                    Regkey.Close();
                    return;
                }

                // If a key is not present add the key, Key value 8000 (decimal)
                if (string.IsNullOrEmpty(FindAppkey))
                {
                    Regkey.SetValue(appName, unchecked((int)0x1F40), RegistryValueKind.DWord);

                }

                // Check for the key after adding
                FindAppkey = Convert.ToString(Regkey.GetValue(appName));


            }
            catch (Exception ex)
            {
                MessageBox.Show("Application Settings Failed");
                MessageBox.Show(ex.Message);
            }
            finally
            {
                // Close the Registry
                if (Regkey != null)
                    Regkey.Close();
            }
        }

        #endregion

        private void selectCountry_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (selectCountry.SelectedIndex > -1)
            {
                Library.ContentFul.Client.SelectedCountryIndex = selectCountry.SelectedIndex;
                if (tabs.SelectedIndex > -1)
                {
                    grids[tabs.SelectedIndex].LoadDataRows();
                }
            }
        }

        private void tabs_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(tabs.SelectedIndex > -1)
            {
                grids[tabs.SelectedIndex].LoadDataRows();
            }
        }

    }
}
