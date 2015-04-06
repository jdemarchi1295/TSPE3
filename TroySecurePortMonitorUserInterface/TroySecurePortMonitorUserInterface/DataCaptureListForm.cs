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
    public partial class DataCaptureListForm : Form
    {
        
        public bool Cancelled = false;
        public bool PassThroughDefined = false;

        private DataCaptureList tempdclist = new DataCaptureList();

        public DataCaptureListForm()
        {
            InitializeComponent();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            DataCapture dc = new DataCapture();
            TroyDataCaptureConfiguration dcc = new TroyDataCaptureConfiguration();
            dc.dcc = dcc;
            dc.Adding = true;
            dc.PassThroughDefined = PassThroughDefined;
            dc.ShowDialog();
            if (!dc.Cancelled)
            {

                if (dcc.DataUse == DataUseType.PassThrough)
                {
                    PassThroughDefined = true;
                }
                tempdclist.DataCaptureConfigurationList.Add(dcc);
                AddToDataGrid(dcc, tempdclist.DataCaptureConfigurationList.Count - 1);
            }
        }

        private void AddToDataGrid(TroyDataCaptureConfiguration dcc, int index)
        {
            string Format = "";
            switch (dcc.DataCapture)
            {
                case DataCaptureType.StandardFonts:
                    Format = "Standard Font";
                    break;
                case DataCaptureType.TroyFonts:
                    Format = "Troy Fonts";
                    break;
                case DataCaptureType.PlainText:
                    Format = "Plain Text";
                    break;
                case DataCaptureType.PjlHeader:
                    Format = "PJL Header";
                    break;
            }

            switch (dcc.DataUse)
            {
                case DataUseType.TroyMark:
                    string[] rowstr1 = { index.ToString(),
                                        "TROYmark Data",
                                        Format,
                                        dcc.RemoveData.ToString(), 
                                        dcc.FontNames.Count.ToString(), 
                                        dcc.DataTags.Count.ToString(), 
                                        dcc.RemoveStrings.Count.ToString() };
                    dgvDataCap.Rows.Add(rowstr1);
                    break;
                case DataUseType.PrinterMap:
                    string[] rowstr2 = { index.ToString(),
                                        "Printer Map", 
                                        Format,
                                        dcc.RemoveData.ToString(), 
                                        dcc.FontNames.Count.ToString(), 
                                        dcc.DataTags.Count.ToString(), 
                                        "N/A"};
                    dgvDataCap.Rows.Add(rowstr2);
                    break;
                case DataUseType.PassThrough:
                    PassThroughDefined = true;
                    string[] rowstr3 = { index.ToString(),
                                        "Pass Through", 
                                        Format,
                                        dcc.RemoveData.ToString(), 
                                        dcc.FontNames.Count.ToString(), 
                                        dcc.DataTags.Count.ToString(), 
                                        "N/A"};
                    dgvDataCap.Rows.Add(rowstr3);
                    break;
                case DataUseType.MicroPrint:
                    string[] rowstr4 = { index.ToString(),
                                        "MicroPrint",
                                        Format,
                                        dcc.RemoveData.ToString(), 
                                        dcc.FontNames.Count.ToString(), 
                                        dcc.DataTags.Count.ToString(), 
                                        dcc.RemoveStrings.Count.ToString() };
                    dgvDataCap.Rows.Add(rowstr4);
                    break;
                default:
                    MessageBox.Show("Unknown data capture type. " + dcc.DataCapture.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
            }
        }

        private void DataCaptureListForm_Load(object sender, EventArgs e)
        {
            int cntr = 0;
            foreach (TroyDataCaptureConfiguration dcc in Globals.dcaplist.DataCaptureConfigurationList)
            {
                tempdclist.DataCaptureConfigurationList.Add(dcc);
                AddToDataGrid(dcc,cntr);
                cntr++;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Cancelled = true;
            this.Close();

        }


        private void btnEdit_Click(object sender, EventArgs e)
        {
            //if ((lstDataCaptureList.SelectedIndex > -1) && (lstDataCaptureList.SelectedIndex < dcaplist.DataCaptureConfigurationList.Count))
            if (dgvDataCap.SelectedRows.Count > 0)
            {
                DataCapture dc = new DataCapture();
                int index = Convert.ToInt32(dgvDataCap.SelectedRows[0].Cells[0].Value.ToString());
                if (index < tempdclist.DataCaptureConfigurationList.Count)
                {
                    dc.dcc = tempdclist.DataCaptureConfigurationList[index];
                }
                else
                {
                    return;
                }
                dc.Adding = false;
                dc.PassThroughDefined = PassThroughDefined;
                dc.ShowDialog();
                if (!dc.Cancelled)
                {
                    tempdclist.DataCaptureConfigurationList.RemoveAt(index);
                    tempdclist.DataCaptureConfigurationList.Insert(index, dc.dcc);
                    dgvDataCap.Rows.Clear();
                    PassThroughDefined = false;
                    int cntr = 0;
                    foreach (TroyDataCaptureConfiguration dcc in tempdclist.DataCaptureConfigurationList)
                    {
                        AddToDataGrid(dcc,cntr);
                        cntr++;
                    }
                }
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            //if ((lstDataCaptureList.SelectedIndex > -1) && (lstDataCaptureList.SelectedIndex < dcaplist.DataCaptureConfigurationList.Count))
            if (dgvDataCap.SelectedRows.Count > 0)
            {
                int index = Convert.ToInt32(dgvDataCap.SelectedRows[0].Cells[0].Value.ToString());
                tempdclist.DataCaptureConfigurationList.RemoveAt(index);
                dgvDataCap.Rows.Remove(dgvDataCap.SelectedRows[0]);

                int cntr = 0;
                dgvDataCap.Rows.Clear();
                foreach (TroyDataCaptureConfiguration dcc in tempdclist.DataCaptureConfigurationList)
                {
                    AddToDataGrid(dcc, cntr);
                    cntr++;
                }


                PassThroughDefined = false;
                for (int cntr2 = 0; cntr2 < dgvDataCap.Rows.Count; cntr2++)
                {
                    if (dgvDataCap.Rows[cntr2].Cells[1].Value.ToString() == "Pass Through")
                    {
                        PassThroughDefined = true;
                    }
                }
            }
        }

        private bool ValidateEntries()
        {
            bool FontTMTagged = false;
            bool FontTMAll = false;
            bool FontPMTagged = false;
            bool FontPMAll = false;

            foreach (TroyDataCaptureConfiguration dcc in tempdclist.DataCaptureConfigurationList)
            {
                if (dcc.DataCapture == DataCaptureType.StandardFonts)
                {
                    //KLK Version 1.0.1.  Handling this better in 1.0.1
                    //if ((dcc.DataUse == DataUseType.TroyMark) && (dcc.UseAllData))
                    //{
                    //    if (FontTMTagged)
                    //    {
                    //        MessageBox.Show("Error! Conflicting data capture entries.  The Data Capture List can not have entries using Tagged Data and All Data for Standard Fonts TROYmark.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //        return false;
                    //    }
                    //    FontTMAll = true;
                    //}
                    //else if ((dcc.DataUse == DataUseType.TroyMark) && (!dcc.UseAllData))
                    //{
                    //    if (FontTMAll)
                    //    {
                    //        MessageBox.Show("Error! Conflicting data capture entries.  The Data Capture List can not have entries using Tagged Data and All Data for Standard Fonts TROYmark.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //        return false;
                    //    }
                    //    FontTMTagged = true;
                    //}
                    //else if ((dcc.DataUse == DataUseType.PrinterMap) && (dcc.UseAllData))
                    if ((dcc.DataUse == DataUseType.PrinterMap) && (dcc.UseAllData))
                    {
                        if (FontPMTagged)
                        {
                            MessageBox.Show("Error! Conflicting data capture entries.  The Data Capture List can not have entries using Tagged Data and All Data for Standard Fonts Printer Map.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return false;
                        }
                        FontPMAll = true;
                    }
                    else if ((dcc.DataUse == DataUseType.PrinterMap) && (!dcc.UseAllData))
                    {
                        if (FontPMAll)
                        {
                           MessageBox.Show("Error! Conflicting data capture entries.  The Data Capture List can not have entries using Tagged Data and All Data for Standard Fonts Printer Map.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return false;
                        }
                        FontPMTagged = true;
                    }
                }

            }
            return true;

        }
        
        private void btnOk_Click(object sender, EventArgs e)
        {
            Cancelled = false;
            if (!ValidateEntries())
            {
                return;
            }
            Globals.dcaplist.DataCaptureConfigurationList.Clear();
            foreach (TroyDataCaptureConfiguration dcc in tempdclist.DataCaptureConfigurationList)
            {
                Globals.dcaplist.DataCaptureConfigurationList.Add(dcc);
            }
            this.Close();
        }
    }
}
