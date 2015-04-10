using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using PantographPclBuilder;

namespace TroySecurePortMonitorUserInterface
{
    public partial class DensitySettings : Form
    {
        public string PrinterName = "";
        public CustomConfiguration ccDensity;
        public string BaseFilePath = "";

        public DensitySettings()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            chkPattern1.Checked = true;
            chkPattern2.Checked = true;
            chkPattern3.Checked = true;
            chkPattern4.Checked = true;
            chkPattern5.Checked = true;
            chkPattern6.Checked = true;
            chkPattern7.Checked = true;
            chkPattern8.Checked = true;
            chkPattern9.Checked = true;
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            chkPattern1.Checked = false;
            chkPattern2.Checked = false;
            chkPattern3.Checked = false;
            chkPattern4.Checked = false;
            chkPattern5.Checked = false;
            chkPattern6.Checked = false;
            chkPattern7.Checked = false;
            chkPattern8.Checked = false;
            chkPattern9.Checked = false;

        }

        private void DensitySettings_Load(object sender, EventArgs e)
        {
            if (ccDensity == null)
            {
                MessageBox.Show("Density settings not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                this.Close();
            }
            else
            {
                numPattern1.Value = Convert.ToDecimal(ccDensity.DensityPattern1);
                numPattern2.Value = Convert.ToDecimal(ccDensity.DensityPattern2);
                numPattern3.Value = Convert.ToDecimal(ccDensity.DensityPattern3);
                numPattern4.Value = Convert.ToDecimal(ccDensity.DensityPattern4);
                numPattern5.Value = Convert.ToDecimal(ccDensity.DensityPattern5);
                numPattern6.Value = Convert.ToDecimal(ccDensity.DensityPattern6);
                numPattern7.Value = Convert.ToDecimal(ccDensity.DensityPattern7);
                numPattern8.Value = Convert.ToDecimal(ccDensity.DensityPattern8);
                numPattern9.Value = Convert.ToDecimal(ccDensity.DensityPattern9);
                txtPrinterName.Text = PrinterName;
                numDarknessFactor.Value = Convert.ToDecimal(ccDensity.BgDarknessFactor);
            }

            cboBaseline.Items.Clear();
            foreach (KeyValuePair<string, Globals.Densities> kvp in Globals.BaselineDensity)
            {
                cboBaseline.Items.Add(kvp.Key);
            }

        }

        private void SendToPrinter(int patternId)
        {
            string pathName = BaseFilePath + "Data\\";
            int darkLevel = Convert.ToInt32(numDarknessFactor.Value);
            string fileName = pathName + "DensityPattern" + patternId.ToString();
                     
            if (darkLevel > 1)
            {
                fileName += "Dark" + darkLevel;
            }
            if (cboPgColor.Text != "Black")
            {
                fileName += cboPgColor.Text;
            }
            fileName += ".pcl";

            if (!File.Exists(fileName))
            {
                try
                {
                    MessageBox.Show("Creating file " + fileName);
                    PantographColorType pgcolor = (PantographColorType)Enum.Parse(typeof(PantographColorType), "pg" + cboPgColor.Text, true);
                    //bp.PrintTestPage(patternId, fileName, darkLevel, PageType.ptLetter, PageOrientation.poPortrait, pgcolor);

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error printing test page. " + ex.Message);
                    return;
                }
            }

            
            string prtMsg = "Troy Pantograph Density Pattern " + patternId.ToString();

            if (!File.Exists(fileName))
            {
                MessageBox.Show("Error.  Can not find PCL file for pattern " + patternId.ToString() + ".  File name: " + fileName, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                PrinterLib.PrintToSpooler.SendFileToPrinter(PrinterName, fileName, prtMsg);
            }

        }

        private void btnPrintPage_Click(object sender, EventArgs e)
        {
            if (chkPattern1.Checked)
            {
                SendToPrinter(1);
            }
            if (chkPattern2.Checked)
            {
                SendToPrinter(2);
            }
            if (chkPattern3.Checked)
            {
                SendToPrinter(3);
            }
            if (chkPattern4.Checked)
            {
                SendToPrinter(4);
            }
            if (chkPattern5.Checked)
            {
                SendToPrinter(5);
            }
            if (chkPattern6.Checked)
            {
                SendToPrinter(6);
            }
            if (chkPattern7.Checked)
            {
                SendToPrinter(7);
            }
            if (chkPattern8.Checked)
            {
                SendToPrinter(8);
            }
            if (chkPattern9.Checked)
            {
                SendToPrinter(9);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            ccDensity.DensityPattern1 = Convert.ToInt32(numPattern1.Value);
            ccDensity.DensityPattern2 = Convert.ToInt32(numPattern2.Value);
            ccDensity.DensityPattern3 = Convert.ToInt32(numPattern3.Value);
            ccDensity.DensityPattern4 = Convert.ToInt32(numPattern4.Value);
            ccDensity.DensityPattern5 = Convert.ToInt32(numPattern5.Value);
            ccDensity.DensityPattern6 = Convert.ToInt32(numPattern6.Value);
            ccDensity.DensityPattern7 = Convert.ToInt32(numPattern7.Value);
            ccDensity.DensityPattern8 = Convert.ToInt32(numPattern8.Value);
            ccDensity.DensityPattern9 = Convert.ToInt32(numPattern9.Value);
            ccDensity.BgDarknessFactor = Convert.ToInt32(numDarknessFactor.Value);
            this.Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            if (cboBaseline.Text != "")
            {
                if (Globals.BaselineDensity.ContainsKey(cboBaseline.Text))
                {
                    Globals.Densities dens = Globals.BaselineDensity[cboBaseline.Text];
                    numPattern1.Value = Convert.ToDecimal(dens.DensityPattern1);
                    numPattern2.Value = Convert.ToDecimal(dens.DensityPattern2);
                    numPattern3.Value = Convert.ToDecimal(dens.DensityPattern3);
                    numPattern4.Value = Convert.ToDecimal(dens.DensityPattern4);
                    numPattern5.Value = Convert.ToDecimal(dens.DensityPattern5);
                    numPattern6.Value = Convert.ToDecimal(dens.DensityPattern6);
                    numPattern7.Value = Convert.ToDecimal(dens.DensityPattern7);
                    numPattern8.Value = Convert.ToDecimal(dens.DensityPattern8);
                    numPattern9.Value = Convert.ToDecimal(dens.DensityPattern9);
                    numDarknessFactor.Value = Convert.ToDecimal(dens.DarknessFactor);
                }
            }
        }
    }
}
