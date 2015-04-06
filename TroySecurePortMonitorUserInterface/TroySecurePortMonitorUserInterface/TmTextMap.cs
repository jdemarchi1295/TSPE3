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
    public partial class TmTextMap : Form
    {
        public string PjlJobName = "";
        public string TmString = "";
        public bool ReturnOk = false;

        public TmTextMap()
        {
            InitializeComponent();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            PjlJobName = txtPjlJobName.Text;
            TmString = txtTroymarkText.Text;
            ReturnOk = true;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            ReturnOk = false;
            this.Close();
        }

        private void TmTextMap_Load(object sender, EventArgs e)
        {
            txtPjlJobName.Text = PjlJobName;
            txtTroymarkText.Text = TmString;
        }

        
    }
}
