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
    public partial class PantoConfig : Form
    {
        public string ConfigValue;

        public PantoConfig()
        {
            InitializeComponent();
        }

        private void PantoConfig_Load(object sender, EventArgs e)
        {
            int pgNum;
            if (ConfigValue == "")
            {
                pgNum = 0;
            }
            else
            {
                try
                {
                    pgNum = Convert.ToInt32(ConfigValue);
                }
                catch
                {
                    pgNum = 0;
                }
            }
            if (pgNum < 1)
            {
                radDisable.Checked = true;
                radMPBorderOff.Checked = true;
                radSigLineOff.Checked = true;
                chkInterferencePattern.Checked = false;
                chkPrintFrontAndBack.Checked = false;
                chkWarningBox.Checked = false;
            }
            else
            {
                radDisable.Checked = true;
                if ((pgNum & 3) == 3)
                {
                    radVertSwaths.Checked = true;
                }
                else if ((pgNum & 3) == 2)
                {
                    radHorSwaths.Checked = true;
                }
                else if ((pgNum & 3) == 1)
                {
                    radChessboard.Checked = true;
                }

                radMPBorderOff.Checked = true;
                if ((pgNum & 12) == 12)
                {
                    radPoint8Border.Checked = true;
                }
                else if ((pgNum & 12) == 8)
                {
                    radPoint6Border.Checked = true;
                }
                else if ((pgNum & 12) == 4)
                {
                    radPoint5Border.Checked = true;
                }

                radSigLineOff.Checked = true;
                if ((pgNum & 96) == 96)
                {
                    radPoint8SigLine.Checked = true;
                }
                else if ((pgNum & 96) == 64)
                {
                    radPoint6SigLine.Checked = true;
                }
                else if ((pgNum & 96) == 32)
                {
                    radPoint5SigLine.Checked = true;
                }

                if ((pgNum & 16) > 0)
                {
                    chkWarningBox.Checked = true;
                }

                if ((pgNum & 128) > 0)
                {
                    chkPrintFrontAndBack.Checked = true;
                }

                if ((pgNum & 256) > 0)
                {
                    chkInterferencePattern.Checked = true;
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            int cVal = CalculatePmValue();

            ConfigValue = cVal.ToString();

            this.Close();
        }

        private int CalculatePmValue()
        {
            int retVal = 0;

            if (radDisable.Checked)
            {
                retVal = 0;
            }
            else if (radChessboard.Checked)
            {
                retVal = 1;
            }
            else if (radHorSwaths.Checked)
            {
                retVal = 2;
            }
            else if (radVertSwaths.Checked)
            {
                retVal = 3;
            }

            if (radPoint5Border.Checked)
            {
                retVal += 4;
            }
            else if (radPoint6Border.Checked)
            {
                retVal += 8;
            }
            else if (radPoint8Border.Checked)
            {
                retVal += 12;
            }

            if (chkWarningBox.Checked)
            {
                retVal += 16;
            }

            if (radPoint5SigLine.Checked)
            {
                retVal += 32;
            }
            else if (radPoint6SigLine.Checked)
            {
                retVal += 64;
            }
            else if (radPoint8SigLine.Checked)
            {
                retVal += 96;
            }

            if (chkPrintFrontAndBack.Checked)
            {
                retVal += 128;
            }

            if (chkInterferencePattern.Checked)
            {
                retVal += 256;
            }
            return retVal;

        }

    }
}
