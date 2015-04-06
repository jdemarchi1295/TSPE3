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
    public partial class DataCapture : Form
    {
        public TroyDataCaptureConfiguration dcc;
        public bool Cancelled = true;
        public bool Adding = false;
        public bool PassThroughDefined = false;
        public bool StandardFontsTMTaggedExists = false;
        public bool StandardFontsTMAllExists = false;
        public bool StandardFontsPMTaggedExists = false;
        public bool StandardFontsPMAllExists = false;


        public DataCapture()
        {
            InitializeComponent();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            AddDataTags adt = new AddDataTags();
            if (radPjlHeader.Checked)
            {
                adt.DcType = DataCaptureType.PjlHeader;
            }
            else if (radPlainText.Checked)
            {
                adt.DcType = DataCaptureType.PlainText;
            }
            else if (radTroyFont.Checked)
            {
                adt.DcType = DataCaptureType.TroyFonts;
            }
            else
            {
                adt.DcType = DataCaptureType.StandardFonts;
            }

            if (radUseWithTroyMark.Checked)
            {
                adt.DcUse = DataUseType.TroyMark;
            }
            else if (radUseForPrinterName.Checked)
            {
                adt.DcUse = DataUseType.PrinterMap;
            }
            else
            {
                adt.DcUse = DataUseType.PassThrough;
            }
            adt.ShowDialog();
            if (adt.LeadingTagString != "")
            {
                string incl = "";
                string opp = "";
                if (adt.IncludeLeading)
                {
                    incl = "Yes";
                }
                else
                {
                    incl = "No";
                }
                if (adt.OnePerPage)
                {
                    opp = "Yes";
                }
                else
                {
                    opp = "No";
                }
                int index = dgvDataCap.Rows.Count;
                string[] rowstr = { index.ToString(), 
                                    adt.LeadingTagString, 
                                    adt.TrailingTagString, 
                                    incl, 
                                    adt.LeadingOuputString, 
                                    adt.TrailingOutputString,
                                    opp};
                dgvDataCap.Rows.Add(rowstr);
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (dgvDataCap.SelectedRows.Count > 0)
            {
                int index = Convert.ToInt32(dgvDataCap.SelectedRows[0].Cells[0].Value.ToString());
                dgvDataCap.Rows.Remove(dgvDataCap.SelectedRows[0]);
                //StandardControl_Changed(sender, e);
            }

        }


        private void DataCapture_Load(object sender, EventArgs e)
        {
            if (!Adding)
            {
                if (dcc.DataUse != DataUseType.PassThrough)
                {
                    //radPassThrough.Enabled = !PassThroughDefined;
                }

                switch (dcc.DataCapture)
                {
                    case DataCaptureType.PlainText:
                        radPlainText.Checked = true;
                        break;
                    case DataCaptureType.PjlHeader:
                        radPjlHeader.Checked = true;
                        break;
                    case DataCaptureType.StandardFonts:
                        radFont.Checked = true;
                        break;
                    case DataCaptureType.TroyFonts:
                        radTroyFont.Checked = true;
                        break;
                }

                switch (dcc.DataUse)
                {
                    case DataUseType.PassThrough:
                        radPassThrough.Checked = true;
                        break;
                    case DataUseType.PrinterMap:
                        radUseForPrinterName.Checked = true;
                        break;
                    case DataUseType.TroyMark:
                        radUseWithTroyMark.Checked = true;
                        break;
                }

                foreach (string name in dcc.FontNames)
                {
                    lstFonts.Items.Add(name);
                }

                int cntr = 0;
                foreach (string rstr in dcc.RemoveStrings)
                {
                    dgvRemoveList.Rows.Add();
                    dgvRemoveList.Rows[cntr].Cells[0].Value = rstr;
                    cntr++;
                }

                chkRemoveData.Checked = dcc.RemoveData;

                int index = 0;
                foreach (DataTagsType dtt in dcc.DataTags)
                {
                    index++;
                    string incl = "";
                    string opp = "";
                    if (dtt.IncludeLeadingTag)
                    {
                        incl = "Yes";
                    }
                    else
                    {
                        incl = "No";
                    }
                    if (dtt.OnePerPage)
                    {
                        opp = "Yes";
                    }
                    else
                    {
                        opp = "No";
                    }

                    string[] rowstr = { index.ToString(), 
                                    dtt.LeadingTag, 
                                    dtt.TrailingTag, 
                                    incl, 
                                    dtt.LeadingText, 
                                    dtt.TrailingText,
                                    opp};
                    dgvDataCap.Rows.Add(rowstr);

                }

                radUseAllData.Checked = dcc.UseAllData;
            }
            else
            {
                //radPassThrough.Enabled = !PassThroughDefined;
            }

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Cancelled = true;
            this.Close();
        }

        private bool ValidateSettings()
        {
            if ((radUseTags.Checked) && (dgvDataCap.Rows.Count < 1))
            {
                MessageBox.Show("Error. Use Tags is selected.  No tags defined.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if ((radFont.Checked) && (lstFonts.Items.Count < 1))
            {
                MessageBox.Show("Error. Standard Fonts is selected.  Font list is empty.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (!ValidateSettings())
            {
                return;
            }

            dcc.UseAllData = radUseAllData.Checked;
          

            if (!radPjlHeader.Checked)
            {
                dcc.RemoveData = chkRemoveData.Checked;
            }
            else
            {
                dcc.RemoveData = false;
            }

            if (radUseForPrinterName.Checked)
            {
                dcc.DataUse = DataUseType.PrinterMap;
            }
            else if (radUseWithTroyMark.Checked)
            {
                dcc.DataUse = DataUseType.TroyMark;
            }
            else if (radPassThrough.Checked)
            {
                dcc.DataUse = DataUseType.PassThrough;
            }
            else if (radMpText.Checked)
            {
                dcc.DataUse = DataUseType.MicroPrint;
            }

            if (!radFont.Checked)
            {
                lstFonts.Items.Clear();
            }

            if (radPjlHeader.Checked)
            {
                dcc.DataCapture = DataCaptureType.PjlHeader;
            }
            else if (radPlainText.Checked)
            {
                dcc.DataCapture = DataCaptureType.PlainText;
            }
            else if (radTroyFont.Checked)
            {
                dcc.DataCapture = DataCaptureType.TroyFonts;
            }
            else if (radFont.Checked)
            {
                dcc.DataCapture = DataCaptureType.StandardFonts;
            }

            dcc.FontNames.Clear();
            foreach (string fts in lstFonts.Items)
            {
                dcc.FontNames.Add(fts);
            }

            dcc.RemoveStrings.Clear();
            if (!radPjlHeader.Checked)
            {
                for (int cntr = 0; cntr < dgvRemoveList.Rows.Count; cntr++)
                {
                    if (dgvRemoveList.Rows[cntr].Cells[0].Value != null)
                    {
                        dcc.RemoveStrings.Add(dgvRemoveList.Rows[cntr].Cells[0].Value.ToString());
                    }
                }
            }

            dcc.DataTags.Clear();
            if (!radUseAllData.Checked)
            {
                if (dgvDataCap.Rows.Count > 0)
                {
                    for (int cntr = 0; cntr < dgvDataCap.Rows.Count; cntr++)
                    {
                        DataTagsType dtt = new DataTagsType();
                        dtt.LeadingTag = dgvDataCap.Rows[cntr].Cells[1].Value.ToString();
                        dtt.TrailingTag = dgvDataCap.Rows[cntr].Cells[2].Value.ToString();
                        if ((dgvDataCap.Rows[cntr].Cells[3].Value.ToString().ToUpper() == "YES") ||
                            (dgvDataCap.Rows[cntr].Cells[3].Value.ToString().ToUpper() == "TRUE"))
                        {
                            dtt.IncludeLeadingTag = true;
                        }
                        else
                        {
                            dtt.IncludeLeadingTag = false;
                        }
                        dtt.LeadingText = dgvDataCap.Rows[cntr].Cells[4].Value.ToString();
                        dtt.TrailingText = dgvDataCap.Rows[cntr].Cells[5].Value.ToString();
                        if ((dgvDataCap.Rows[cntr].Cells[6].Value.ToString().ToUpper() == "YES") ||
                            (dgvDataCap.Rows[cntr].Cells[6].Value.ToString().ToUpper() == "TRUE"))
                        {
                            dtt.OnePerPage = true;
                        }
                        else
                        {
                            dtt.OnePerPage = false;
                        }

                        dcc.DataTags.Add(dtt);
                    }
                }
            }
            Cancelled = false;

            this.Close();
        }

        private void btnAddFont_Click(object sender, EventArgs e)
        {
            //if ((txtAddFontName.Text != "") && (!lstFonts.Items.Contains(txtAddFontName.Text)))
            //{
            //    lstFonts.Items.Add(txtAddFontName.Text);
           // }


            FontList fl = new FontList();
            fl.ShowDialog();

            if (fl.SelectedFontName.Count > 0)
            {
                foreach (string fontStr in fl.SelectedFontName)
                {
                    if (!lstFonts.Items.Contains(fontStr))
                    {
                        lstFonts.Items.Add(fontStr);
                    }
                }
            }
        }

        private void btnDelFont_Click(object sender, EventArgs e)
        {
            if (lstFonts.SelectedIndex > -1)
            {
                lstFonts.Items.RemoveAt(lstFonts.SelectedIndex);
            }

        }

        private void EnableAll()
        {
            radUseAllData.Enabled = true;
            radUseTags.Enabled = true;
            radUseWithTroyMark.Enabled = true;
            radUseForPrinterName.Enabled = true;
            radPlainText.Enabled = true;
            radFont.Enabled = true;
            radPjlHeader.Enabled = true;
            gbTagList.Enabled = true;
        }

        private void EnableDisableControls()
        {
            if (radUseTags.Checked)
            {
                chkRemoveData.Checked = false;
                chkRemoveData.Enabled = false;
            }
            else if ((radPjlHeader.Checked) || (radPlainText.Checked))
            {
                chkRemoveData.Checked = false;
                chkRemoveData.Enabled = false;
            }
            else
            {
                chkRemoveData.Enabled = true;
            }


            if ((radUseWithTroyMark.Checked) || (radMpText.Checked))
            {
                if ((radPlainText.Checked) || (radPjlHeader.Checked))
                {
                    radUseAllData.Enabled = false;
                    radUseTags.Checked = true;
                    gbTagList.Enabled = true;
                    radUseTags.Enabled = true;
                }
                else
                {
                    EnableAll();
                }
            }
            else if (radUseForPrinterName.Checked)
            {
                if ((radPlainText.Checked) || (radPjlHeader.Checked))
                {
                    radUseAllData.Enabled = false;
                    gbTagList.Enabled = true;
                    radUseTags.Enabled = true;
                    radUseTags.Checked = true;
                }
                else
                {
                    EnableAll();
                }
            }
            else if (radPassThrough.Checked)
            {
                if ((radFont.Checked) || (radPlainText.Checked))
                {
                    gbTagList.Enabled = false;
                    radUseTags.Enabled = false;
                    radUseAllData.Checked = true;
                    radUseAllData.Enabled = true;
                }
                else
                {
                    radUseAllData.Enabled = false;
                    gbTagList.Enabled = true;
                    radUseTags.Checked = true;
                    radUseTags.Enabled = true;
                }
            }

        }

        private void radUseAllData_CheckedChanged(object sender, EventArgs e)
        {
            if (radUseAllData.Checked)
            {
                EnableDisableControls();
            }
/*            if (radUseAllData.Checked)
            {
                dgvDataCap.Enabled = false;
                btnAddTag.Enabled = false;
                btnRemoveTag.Enabled = false;
            }*/
        }

        private void radUseTags_CheckedChanged(object sender, EventArgs e)
        {
            if (radUseTags.Checked)
            {
                EnableDisableControls();
            }
//            if (radUseTags.Checked)
//            {
              //  EnableDisableControls();
                //dgvDataCap.Enabled = true;
                //btnAddTag.Enabled = true;
                //btnRemoveTag.Enabled = true;
            //}

        }

        private void radPlainText_CheckedChanged(object sender, EventArgs e)
        {
            if (radPlainText.Checked)
            {
                EnableDisableControls();
                gbFonts.Enabled = false;
                //radUseAllData.Enabled = true;
                //chkRemoveData.Checked = false;
                //chkRemoveData.Enabled = true;
            }
        }

        private void radFont_CheckedChanged(object sender, EventArgs e)
        {
            if (radFont.Checked)
            {
                EnableDisableControls();
                gbFonts.Enabled = true;
                //radUseAllData.Enabled = true;
                //chkRemoveData.Enabled = true;
                //chkRemoveData.Checked = false;
            }
            //CheckIfUseTagsEnabled();

        }

        private void radTroyFont_CheckedChanged(object sender, EventArgs e)
        {
            if (radTroyFont.Checked)
            {
                EnableDisableControls();
                gbFonts.Enabled = false;
                //radUseAllData.Enabled = true;
                //gbRemoveString.Enabled = true;
                //chkRemoveData.Checked = true;
                //chkRemoveData.Enabled = false;
                //gbTags.Enabled = false;
            }
        }

        private void radPjlHeader_CheckedChanged(object sender, EventArgs e)
        {
            if (radPjlHeader.Checked)
            {
                EnableDisableControls();
                gbFonts.Enabled = false;
                //radUseAllData.Enabled = false;
                //radUseAllData.Checked = false;
                //chkRemoveData.Enabled = false;
                //gbRemoveString.Enabled = false;
                //gbTags.Enabled = true;
            }
        }

        private void radPassThrough_CheckedChanged(object sender, EventArgs e)
        {
            EnableDisableControls();
            //CheckIfUseTagsEnabled();
        }


        private void radUseForPrinterName_CheckedChanged(object sender, EventArgs e)
        {
            EnableDisableControls();
/*            if (radUseForPrinterName.Checked)
            {
                radPlainText.Enabled = false;
                if (radPlainText.Checked)
                {
                    radFont.Checked = true;
                }
            }
            else
            {
                radPlainText.Enabled = true;
            }*/
        }

        private void radUseWithTroyMark_CheckedChanged(object sender, EventArgs e)
        {
            EnableDisableControls();
        }

        private void radMpText_CheckedChanged(object sender, EventArgs e)
        {
            EnableDisableControls();
        }
    }
}
