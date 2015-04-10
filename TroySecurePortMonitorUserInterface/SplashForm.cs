using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using Troy.Licensing.Client.UI.Form;
using Troy.TSPE3.Licensing;

namespace TroySecurePortMonitorUserInterface
{
    public partial class SplashForm : Form
    {
        public string LicenseString = "";
        public string VersionNumber = "";
        public bool ShowEnableButton = false;


        public SplashForm()
        {
            InitializeComponent();
        }


        private void SplashForm_Load(object sender, EventArgs e)
        {
            lblVersion.Text = "Version: " + VersionNumber;
            lblLicenseCount.Text = LicenseString;
            lblTitle.Text = MainScreen.ProductTitle;
        }

        private void btnEnableLicense_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            System.Threading.Thread.Sleep(500);
            var myService = new System.ServiceProcess.ServiceController("Troy Port Monitor Service");
            if (myService.Status == System.ServiceProcess.ServiceControllerStatus.Running)
            {
                myService.Stop();
            }
            System.Threading.Thread.Sleep(2000);
            myService.Start();
            this.Cursor = Cursors.Arrow;
        }

        private void btn_lrf_Click(object sender, EventArgs e)
        {
            var lrf = new LicenseRequestForm(Strings.ApplicationId);
            lrf.ShowDialog();
        }

        private void btn_alf_Click(object sender, EventArgs e)
        {
            var alf = new ActivateLicenseForm(Strings.ApplicationId);
            alf.ShowDialog();

        }
    }
}
