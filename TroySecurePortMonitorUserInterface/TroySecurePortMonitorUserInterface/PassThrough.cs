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
    public partial class PassThrough : Form
    {
        public bool PassThroughEnabled = false;
        public List<string> PassThroughList = new List<string>();
        public bool UseAll = true;
        public bool Cancelled = false;

        public PassThrough()
        {
            InitializeComponent();
        }

        private void gbAllOrOne_Enter(object sender, EventArgs e)
        {

        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (txtAddText.Text != "")
            {
                string[] RowStr = { txtAddText.Text };
                dgvPassThroughStrings.Rows.Add(RowStr);
                txtAddText.Text = "";
            }
        }

        private void PassThrough_Load(object sender, EventArgs e)
        {
            radAllString.Checked = UseAll;
            radOneString.Checked = !UseAll;
            if (PassThroughEnabled)
            {
                chkEnablePassThrough.Checked = true;
                gbAllOrOne.Enabled = true;
                gbStrings.Enabled = true;
            }
            else
            {
                chkEnablePassThrough.Checked = false;
                gbAllOrOne.Enabled = false;
                gbStrings.Enabled = false;
            }

            foreach (string str in PassThroughList)
            {
                string[] temp = { str };
                dgvPassThroughStrings.Rows.Add(temp);
            }

        }

        private void btnDel_Click(object sender, EventArgs e)
        {
            if (dgvPassThroughStrings.SelectedRows.Count > 0)
            {
                dgvPassThroughStrings.Rows.Remove(dgvPassThroughStrings.SelectedRows[0]);
            }
        }

        private void chkEnablePassThrough_CheckedChanged(object sender, EventArgs e)
        {
            if (chkEnablePassThrough.Checked)
            {
                gbAllOrOne.Enabled = true;
                gbStrings.Enabled = true;
            }
            else
            {
                gbAllOrOne.Enabled = false;
                gbStrings.Enabled = false;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Cancelled = true;
            this.Close();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            Cancelled = false;
            PassThroughEnabled = chkEnablePassThrough.Checked;
            UseAll = radAllString.Checked;
            PassThroughList.Clear();
            for (int cntr = 0; cntr < dgvPassThroughStrings.Rows.Count; cntr++)
            {
                PassThroughList.Add(dgvPassThroughStrings.Rows[cntr].Cells[0].Value.ToString());
            }
            this.Close();
        }
    }
}
