using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Troy.PortMonitor.Core.XmlConfiguration;

namespace TroySecurePortMonitorUserInterface
{
    public partial class AddDataTags : Form
    {
        public string LeadingTagString = "";
        public string TrailingTagString = "";
        public bool IncludeLeading = false;
        public string LeadingOuputString = "";
        public string TrailingOutputString = "";
        public bool OnePerPage = false;
        public DataCaptureType DcType = DataCaptureType.StandardFonts;
        public DataUseType DcUse = DataUseType.TroyMark;

        public AddDataTags()
        {
            InitializeComponent();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if ((DcType == DataCaptureType.PjlHeader) &&
                ((txtLeadingTag.Text == "") ||
                 ((!chkUseLF.Checked) && (txtTrailing.Text == ""))))
            {
                MessageBox.Show("For PJL data, Leading and Trailing tags must be defined.\n If using the standard PJL format, enter the PJL command for the Leading and select the Use Line Feed... check box");
            }
            else if ((DcType != DataCaptureType.PjlHeader) && 
                     ((txtLeadingTag.Text == "") || (txtTrailing.Text == "")))
            {
                MessageBox.Show("Must enter a valid string for the Leading and Trailing tags.");
            }
            else
            {
                LeadingTagString = txtLeadingTag.Text;
                TrailingTagString = txtTrailing.Text;
                IncludeLeading = chkIncludeLeading.Checked;
                LeadingOuputString = txtLeadingString.Text;
                TrailingOutputString = txtTrailingString.Text;
                OnePerPage = chkOnePerPage.Checked;
                if ((chkUseLF.Checked) && (DcType == DataCaptureType.PjlHeader))
                {
                    TrailingTagString = "/n";
                }
                this.Close();
            }
        }

        private void AddDataTags_Load(object sender, EventArgs e)
        {
            if (DcType == DataCaptureType.PjlHeader)
            {
                lblPjlNote.Visible = true;
                chkIncludeLeading.Enabled = false;
                txtLeadingString.Enabled = false;
                txtTrailingString.Enabled = false;
                chkUseLF.Visible = true;
                chkOnePerPage.Enabled = true;
            }
            else if (DcType == DataCaptureType.TroyFonts)
            {
                chkIncludeLeading.Enabled = false;
                txtLeadingTag.Text = "/e%m4615T";
                txtTrailing.Enabled = false;
                lblPjlNote.Text = "Note: Leading Tag must start if /e%m for Troy Fonts";
                lblPjlNote.Visible = true;
                chkOnePerPage.Enabled = false;
            }

            if (DcUse != DataUseType.TroyMark)
            {
                chkIncludeLeading.Enabled = false;
                txtLeadingString.Enabled = false;
                txtTrailingString.Enabled = false;
            }
        }

        private void chkUseLF_CheckedChanged(object sender, EventArgs e)
        {

        }


    }
}
