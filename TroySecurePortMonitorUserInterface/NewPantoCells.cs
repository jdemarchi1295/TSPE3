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
    public partial class NewPantoCells : Form
    {
        public int PantographId = 0;

        public NewPantoCells()
        {
            InitializeComponent();
        }

        private bool ValidateSetting()
        {
            if ((chkCustomPattern.Checked) && (cboCustomPatterns.Text == ""))
            {
                MessageBox.Show("Please select a custom pattern name from the drop down list");
                return false;
            }

            return true;

        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (ValidateSetting())
            {
                SaveSettings();
                this.Close();
            }
        }

        private void SaveSettings()
        {

            if (!gbStandard.Enabled)
            {
                ClearStandard();
            }

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

            Globals.CellSetups[PantographId, 9].PatternEnabled = chkCustomPattern.Checked;
            Globals.CellSetups[PantographId, 9].CellText = txtCustomText.Text;
            Globals.CustomPatternName[PantographId] = cboCustomPatterns.Text;
        }

        private void ClearStandard()
        {
            chkUsePtrn1.Checked = false;
            txtCellText1.Text = "";
            chkUsePtrn2.Checked = false;
            txtCellText2.Text = "";
            chkUsePtrn3.Checked = false;
            txtCellText3.Text = "";
            chkUsePtrn4.Checked = false;
            txtCellText4.Text = "";
            chkUsePtrn5.Checked = false;
            txtCellText5.Text = "";
            chkUsePtrn6.Checked = false;
            txtCellText6.Text = "";
            chkUsePtrn7.Checked = false;
            txtCellText7.Text = "";
            chkUsePtrn8.Checked = false;
            txtCellText8.Text = "";
            chkUsePtrn9.Checked = false;
            txtCellText9.Text = "";

        
        }

        private void NewPantoCells_Load(object sender, EventArgs e)
        {
            if (Globals.CellSetups[PantographId, 9].PatternEnabled)
            {
                chkCustomPattern.Checked = true;
                gbStandard.Enabled = false;
                if (!String.IsNullOrEmpty(Globals.CellSetups[PantographId, 9].CellText))
                {
                    txtCustomText.Text = Globals.CellSetups[PantographId, 9].CellText;
                }
                else
                {
                    txtCustomText.Text = "";
                }
                if (!string.IsNullOrEmpty(Globals.CustomPatternName[PantographId]))
                {
                    cboCustomPatterns.Text = Globals.CustomPatternName[PantographId];
                }
            }
            else
            {
                chkCustomPattern.Checked = false;
                gbStandard.Enabled = true;
                txtCustomText.Text = "";
            }

                        
            if (Globals.CellSetups[PantographId, 0].PatternEnabled)
            {
                chkUsePtrn1.Checked = true;
                if (!String.IsNullOrEmpty(Globals.CellSetups[PantographId, 0].CellText))
                {
                    txtCellText1.Text = Globals.CellSetups[PantographId, 0].CellText;
                }
                else
                {
                    txtCellText1.Text = "";
                }
            }
            else
            {
                chkUsePtrn1.Checked = false;
                txtCellText1.Text = "";
            }
            if (Globals.CellSetups[PantographId, 1].PatternEnabled)
            {
                chkUsePtrn2.Checked = true;
                if (!String.IsNullOrEmpty(Globals.CellSetups[PantographId, 1].CellText))
                {
                    txtCellText2.Text = Globals.CellSetups[PantographId, 1].CellText;
                }
                else
                {
                    txtCellText2.Text = "";
                }
            }
            else
            {
                chkUsePtrn2.Checked = false;
                txtCellText2.Text = "";
            }

            if (Globals.CellSetups[PantographId, 2].PatternEnabled)
            {
                chkUsePtrn3.Checked = true;
                if (!String.IsNullOrEmpty(Globals.CellSetups[PantographId, 2].CellText))
                {
                    txtCellText3.Text = Globals.CellSetups[PantographId, 2].CellText;
                }
                else
                {
                    txtCellText3.Text = "";
                }
            }
            else
            {
                chkUsePtrn3.Checked = false;
                txtCellText3.Text = "";
            }

            if (Globals.CellSetups[PantographId, 3].PatternEnabled)
            {
                chkUsePtrn4.Checked = true;
                if (!String.IsNullOrEmpty(Globals.CellSetups[PantographId, 3].CellText))
                {
                    txtCellText4.Text = Globals.CellSetups[PantographId, 3].CellText;
                }
                else
                {
                    txtCellText4.Text = "";
                }
            }
            else
            {
                chkUsePtrn4.Checked = false;
                txtCellText4.Text = "";
            }


            if (Globals.CellSetups[PantographId, 4].PatternEnabled)
            {
                chkUsePtrn5.Checked = true;
                if (!String.IsNullOrEmpty(Globals.CellSetups[PantographId, 4].CellText))
                {
                    txtCellText5.Text = Globals.CellSetups[PantographId, 4].CellText;
                }
                else
                {
                    txtCellText5.Text = "";
                }
            }
            else
            {
                chkUsePtrn5.Checked = false;
                txtCellText5.Text = "";
            }

            if (Globals.CellSetups[PantographId, 5].PatternEnabled)
            {
                chkUsePtrn6.Checked = true;
                if (!String.IsNullOrEmpty(Globals.CellSetups[PantographId, 5].CellText))
                {
                    txtCellText6.Text = Globals.CellSetups[PantographId, 5].CellText;
                }
                else
                {
                    txtCellText6.Text = "";
                }
            }
            else
            {
                chkUsePtrn6.Checked = false;
                txtCellText6.Text = "";
            }

            if (Globals.CellSetups[PantographId, 6].PatternEnabled)
            {
                chkUsePtrn7.Checked = true;
                if (!String.IsNullOrEmpty(Globals.CellSetups[PantographId, 6].CellText))
                {
                    txtCellText7.Text = Globals.CellSetups[PantographId, 6].CellText;
                }
                else
                {
                    txtCellText7.Text = "";
                }
            }
            else
            {
                chkUsePtrn7.Checked = false;
                txtCellText7.Text = "";
            }

            if (Globals.CellSetups[PantographId, 7].PatternEnabled)
            {
                chkUsePtrn8.Checked = true;
                if (!String.IsNullOrEmpty(Globals.CellSetups[PantographId, 7].CellText))
                {
                    txtCellText8.Text = Globals.CellSetups[PantographId, 7].CellText;
                }
                else
                {
                    txtCellText8.Text = "";
                }
            }
            else
            {
                chkUsePtrn8.Checked = false;
                txtCellText8.Text = "";
            }

            if (Globals.CellSetups[PantographId, 8].PatternEnabled)
            {
                chkUsePtrn9.Checked = true;
                if (!String.IsNullOrEmpty(Globals.CellSetups[PantographId, 8].CellText))
                {
                    txtCellText9.Text = Globals.CellSetups[PantographId, 8].CellText;
                }
                else
                {
                    txtCellText9.Text = "";
                }
            }
            else
            {
                chkUsePtrn9.Checked = false;
                txtCellText9.Text = "";
            }



            cboCustomPatterns.Items.Clear();
            foreach (TroyPatterns tp in Globals.customPatterns.TroyStoredPattern)
            {
                if (!cboCustomPatterns.Items.Contains(tp.StoredPatternName))
                {
                    cboCustomPatterns.Items.Add(tp.StoredPatternName);
                }
            }

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
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

        private void chkCustomPattern_CheckedChanged(object sender, EventArgs e)
        {
            if (chkCustomPattern.Checked)
            {
                gbStandard.Enabled = false;
            }
            else
            {
                gbStandard.Enabled = true;
            }
        }



    }
}
