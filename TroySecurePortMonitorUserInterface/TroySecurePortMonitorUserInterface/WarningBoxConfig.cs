using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TroySecurePortMonitorUserInterface
{
    public partial class WarningBoxConfig : Form
    {
        public string WarningBoxString = "";

        public WarningBoxConfig()
        {
            InitializeComponent();
        }

        private void WarningBoxConfig_Load(object sender, EventArgs e)
        {
            txtWbConfig.Text = WarningBoxString;
        }

        private void btnOKWB_Click(object sender, EventArgs e)
        {
            WarningBoxString = txtWbConfig.Text;
            this.Close();
        }

        private void btnCancelWB_Click(object sender, EventArgs e)
        {
            this.Close();
        }



    }
}
