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
    public partial class AddExclusion : Form
    {
        public string XAnchor = "";
        public string YAnchor = "";
        public string ExcWidth = "";
        public string ExcHeight = "";

        public AddExclusion()
        {
            InitializeComponent();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if ((txtPgXAnchor.Text != "") && (txtPgYAnchor.Text != "") &&
                (txtPgWidth.Text != "") && (txtPgHeight.Text != ""))
            {
                XAnchor = txtPgXAnchor.Text;
                YAnchor = txtPgYAnchor.Text;
                ExcWidth = txtPgWidth.Text;
                ExcHeight = txtPgHeight.Text;
            }
            this.Close();
        }


    }
}
