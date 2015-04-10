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
    public partial class PantoCells : Form
    {
        public int PantographId = 0;

        public PantoCells()
        {
            InitializeComponent();
        }

        private void chkUsePtrn1_CheckedChanged(object sender, EventArgs e)
        {
            if (chkUsePtrn1.Checked)
            {
                txtCellText1.Enabled = true;
                label1.Enabled = true;
            }
            else
            {
                txtCellText1.Enabled = false;
                label1.Enabled = false;
            }

        }

        private void chkUsePtrn2_CheckedChanged(object sender, EventArgs e)
        {
            if (chkUsePtrn2.Checked)
            {
                txtCellText2.Enabled = true;
                label2.Enabled = true;
            }
            else
            {
                txtCellText2.Enabled = false;
                label2.Enabled = false;
            }

        }

        private void chkUsePtrn3_CheckedChanged(object sender, EventArgs e)
        {
            if (chkUsePtrn3.Checked)
            {
                txtCellText3.Enabled = true;
                label3.Enabled = true;
            }
            else
            {
                txtCellText3.Enabled = false;
                label3.Enabled = false;
            }

        }

        private void chkUsePtrn4_CheckedChanged(object sender, EventArgs e)
        {
            if (chkUsePtrn4.Checked)
            {
                txtCellText4.Enabled = true;
                label4.Enabled = true;
            }
            else
            {
                txtCellText4.Enabled = false;
                label4.Enabled = false;
            }

        }

        private void chkUsePtrn5_CheckedChanged(object sender, EventArgs e)
        {
            if (chkUsePtrn5.Checked)
            {
                txtCellText5.Enabled = true;
                label5.Enabled = true;
            }
            else
            {
                txtCellText5.Enabled = false;
                label5.Enabled = false;
            }

        }

        private void chkUsePtrn6_CheckedChanged(object sender, EventArgs e)
        {
            if (chkUsePtrn6.Checked)
            {
                txtCellText6.Enabled = true;
                label6.Enabled = true;
            }
            else
            {
                txtCellText6.Enabled = false;
                label6.Enabled = false;
            }

        }

        private void chkUsePtrn7_CheckedChanged(object sender, EventArgs e)
        {
            if (chkUsePtrn7.Checked)
            {
                txtCellText7.Enabled = true;
                label7.Enabled = true;
            }
            else
            {
                txtCellText7.Enabled = false;
                label7.Enabled = false;
            }

        }

        private void chkUsePtrn8_CheckedChanged(object sender, EventArgs e)
        {
            if (chkUsePtrn8.Checked)
            {
                txtCellText8.Enabled = true;
                label8.Enabled = true;
            }
            else
            {
                txtCellText8.Enabled = false;
                label8.Enabled = false;
            }

        }

        private void chkUsePtrn9_CheckedChanged(object sender, EventArgs e)
        {
            if (chkUsePtrn9.Checked)
            {
                txtCellText9.Enabled = true;
                label9.Enabled = true;
            }
            else
            {
                txtCellText9.Enabled = false;
                label9.Enabled = false;
            }

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void PantoCells_Load(object sender, EventArgs e)
        {
            if (Globals.CellSetups[PantographId, 0].PatternEnabled)
            {
                chkUsePtrn1.Checked = true;
                txtCellText1.Text = Globals.CellSetups[PantographId, 0].CellText;
            }
            else
            {
                chkUsePtrn1.Checked = false;
                txtCellText1.Text = "";
            }
            if (Globals.CellSetups[PantographId, 1].PatternEnabled)
            {
                chkUsePtrn2.Checked = true;
                txtCellText2.Text = Globals.CellSetups[PantographId, 1].CellText;
            }
            else
            {
                chkUsePtrn2.Checked = false;
                txtCellText2.Text = "";
            }

            if (Globals.CellSetups[PantographId, 2].PatternEnabled)
            {
                chkUsePtrn3.Checked = true;
                txtCellText3.Text = Globals.CellSetups[PantographId, 2].CellText;
            }
            else
            {
                chkUsePtrn3.Checked = false;
                txtCellText3.Text = "";
            }

            if (Globals.CellSetups[PantographId, 3].PatternEnabled)
            {
                chkUsePtrn4.Checked = true;
                txtCellText4.Text = Globals.CellSetups[PantographId, 3].CellText;
            }
            else
            {
                chkUsePtrn4.Checked = false;
                txtCellText4.Text = "";
            }

            if (Globals.CellSetups[PantographId, 4].PatternEnabled)
            {
                chkUsePtrn5.Checked = true;
                txtCellText5.Text = Globals.CellSetups[PantographId, 4].CellText;
            }
            else
            {
                chkUsePtrn5.Checked = false;
                txtCellText5.Text = "";
            }

            if (Globals.CellSetups[PantographId, 5].PatternEnabled)
            {
                chkUsePtrn6.Checked = true;
                txtCellText6.Text = Globals.CellSetups[PantographId, 5].CellText;
            }
            else
            {
                chkUsePtrn6.Checked = false;
                txtCellText6.Text = "";
            }

            if (Globals.CellSetups[PantographId, 6].PatternEnabled)
            {
                chkUsePtrn7.Checked = true;
                txtCellText7.Text = Globals.CellSetups[PantographId, 6].CellText;
            }
            else
            {
                chkUsePtrn7.Checked = false;
                txtCellText7.Text = "";
            }

            if (Globals.CellSetups[PantographId, 7].PatternEnabled)
            {
                chkUsePtrn8.Checked = true;
                txtCellText8.Text = Globals.CellSetups[PantographId, 7].CellText;
            }
            else
            {
                chkUsePtrn8.Checked = false;
                txtCellText8.Text = "";
            }

            if (Globals.CellSetups[PantographId, 8].PatternEnabled)
            {
                chkUsePtrn9.Checked = true;
                txtCellText9.Text = Globals.CellSetups[PantographId, 8].CellText;
            }
            else
            {
                chkUsePtrn9.Checked = false;
                txtCellText9.Text = "";
            }


        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            SaveSettings();
            this.Close();
        }

        private void SaveSettings()
        {
            Globals.CellSetups[PantographId, 0].PatternEnabled = chkUsePtrn1.Checked;
            Globals.CellSetups[PantographId, 0].CellText = txtCellText1.Text;

            Globals.CellSetups[PantographId, 1].PatternEnabled = chkUsePtrn2.Checked;
            Globals.CellSetups[PantographId, 1].CellText = txtCellText2.Text;

            Globals.CellSetups[PantographId, 2].PatternEnabled = chkUsePtrn3.Checked;
            Globals.CellSetups[PantographId, 2].CellText = txtCellText3.Text;

            Globals.CellSetups[PantographId, 3].PatternEnabled = chkUsePtrn4.Checked;
            Globals.CellSetups[PantographId, 3].CellText = txtCellText4.Text;

            Globals.CellSetups[PantographId, 4].PatternEnabled = chkUsePtrn5.Checked;
            Globals.CellSetups[PantographId, 4].CellText = txtCellText5.Text;

            Globals.CellSetups[PantographId, 5].PatternEnabled = chkUsePtrn6.Checked;
            Globals.CellSetups[PantographId, 5].CellText = txtCellText6.Text;

            Globals.CellSetups[PantographId, 6].PatternEnabled = chkUsePtrn7.Checked;
            Globals.CellSetups[PantographId, 6].CellText = txtCellText7.Text;

            Globals.CellSetups[PantographId, 7].PatternEnabled = chkUsePtrn8.Checked;
            Globals.CellSetups[PantographId, 7].CellText = txtCellText8.Text;

            Globals.CellSetups[PantographId, 8].PatternEnabled = chkUsePtrn9.Checked;
            Globals.CellSetups[PantographId, 8].CellText = txtCellText9.Text;

        }


    }
}
