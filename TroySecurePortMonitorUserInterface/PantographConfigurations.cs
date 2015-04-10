using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Troy.PortMonitor.Core.XmlConfiguration;
using PantographPclBuilder;

namespace TroySecurePortMonitorUserInterface
{
    public partial class PantographConfigurations : Form
    {
        public PantographConfiguration PConfigs = new PantographConfiguration();
        public PageOrientationType masterPO;
        public PageType masterPT;

        private string[] WbStrings = new string[10];

        public PantographConfigurations()
        {
            InitializeComponent();
        }


        private void PantographConfigurations_Load(object sender, EventArgs e)
        {
            if (Globals.Lite)
            {
                btnWarningBox1.Visible = false;
                btnWarningBox2.Visible = false;
                btnWarningBox3.Visible = false;
                btnWarningBox4.Visible = false;
                groupBox6.Visible = false;
                groupBox7.Visible = false;
                groupBox8.Visible = false;
                groupBox9.Visible = false;
                chkDynamicMp1.Visible = false;
                chkDynamicMp2.Visible = false;
                chkDynamicMp3.Visible = false;
                chkDynamicMp4.Visible = false;
            }

            
            
            
            if (PConfigs == null)
            {
                MessageBox.Show("Error.  Pantograph Configuration values are not loaded. ", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                this.Close();
            }
            else
            {
                int cntr = 0;
                foreach (CustomConfiguration cc in PConfigs.PantographConfigurations)
                {
                    int cellCntr;
                    switch (cntr)
                    {
                        case 0:
                            WbStrings[1] = cc.WarningBoxString;
                            txtPConfig1.Text = cc.PantographConfiguration;
                            if (cc.InclusionRegion != null)
                            {
                                txtInclX1.Text = cc.InclusionRegion.XAnchor.ToString();
                                txtInclY1.Text = cc.InclusionRegion.YAnchor.ToString();
                                txtInclW1.Text = cc.InclusionRegion.Width.ToString();
                                txtInclH1.Text = cc.InclusionRegion.Height.ToString();
                            }
                            foreach (PantographRegionObjectType prot in cc.ExclusionRegions)
                            {
                               AddExcToList1(prot.XAnchor.ToString(), prot.YAnchor.ToString(), prot.Width.ToString(), prot.Height.ToString());
                            }
                            numIPtrn1.Value = Convert.ToDecimal(cc.InterferencePatternId);
                            cboColor1.Text = cc.PantographColor.ToString().Substring(2);
                            cellCntr = 1;
                            txtBorderTxt1.Text = cc.BorderString;
                            chkDynamicMp1.Checked = cc.UseDynamicMp;
                            if (cc.TROYmarkOn)
                            {
                                if (cc.UseDynamicTmMsg)
                                {
                                    radDynamicData1.Checked = true;
                                }
                                else
                                {
                                    radStaticData1.Checked = true;
                                }
                            }
                            else
                            {
                                radNoTmMsg1.Checked = true;
                            }
                            chkUseDefaultInclusion1.Checked = cc.UseDefaultInclusionForPaperSize;
                            break;
                        case 1:
                            WbStrings[2] = cc.WarningBoxString;
                            txtPConfig2.Text = cc.PantographConfiguration;
                            if (cc.InclusionRegion != null)
                            {
                                txtInclX2.Text = cc.InclusionRegion.XAnchor.ToString();
                                txtInclY2.Text = cc.InclusionRegion.YAnchor.ToString();
                                txtInclW2.Text = cc.InclusionRegion.Width.ToString();
                                txtInclH2.Text = cc.InclusionRegion.Height.ToString();
                            }
                            foreach (PantographRegionObjectType prot in cc.ExclusionRegions)
                            {
                                AddExcToList2(prot.XAnchor.ToString(), prot.YAnchor.ToString(), prot.Width.ToString(), prot.Height.ToString());
                            }
                            numIPtrn2.Value = Convert.ToDecimal(cc.InterferencePatternId);
                            cboColor2.Text = cc.PantographColor.ToString().Substring(2);
                            cellCntr = 1;
                            txtBorderTxt2.Text = cc.BorderString;
                            chkDynamicMp2.Checked = cc.UseDynamicMp;
                            if (cc.TROYmarkOn)
                            {
                                if (cc.UseDynamicTmMsg)
                                {
                                    radDynamicData2.Checked = true;
                                }
                                else
                                {
                                    radStaticData2.Checked = true;
                                }
                            }
                            else
                            {
                                radNoTmMsg2.Checked = true;
                            }
                            chkUseDefaultInclusion2.Checked = cc.UseDefaultInclusionForPaperSize;
                            break;

                        case 2:
                            WbStrings[3] = cc.WarningBoxString;
                            txtPConfig3.Text = cc.PantographConfiguration;
                            if (cc.InclusionRegion != null)
                            {
                                txtInclX3.Text = cc.InclusionRegion.XAnchor.ToString();
                                txtInclY3.Text = cc.InclusionRegion.YAnchor.ToString();
                                txtInclW3.Text = cc.InclusionRegion.Width.ToString();
                                txtInclH3.Text = cc.InclusionRegion.Height.ToString();
                            }
                            foreach (PantographRegionObjectType prot in cc.ExclusionRegions)
                            {
                                AddExcToList3(prot.XAnchor.ToString(), prot.YAnchor.ToString(), prot.Width.ToString(), prot.Height.ToString());
                            }
                            numIPtrn3.Value = Convert.ToDecimal(cc.InterferencePatternId);
                            cboColor3.Text = cc.PantographColor.ToString().Substring(2);
                            cellCntr = 1;
                            txtBorderTxt3.Text = cc.BorderString;
                            chkDynamicMp3.Checked = cc.UseDynamicMp;
                           if (cc.TROYmarkOn)
                            {
                                if (cc.UseDynamicTmMsg)
                                {
                                    radDynamicData3.Checked = true;
                                }
                                else
                                {
                                    radStaticData3.Checked = true;
                                }
                            }
                            else
                            {
                                radNoTmMsg3.Checked = true;
                            }
                            chkUseDefaultInclusion3.Checked = cc.UseDefaultInclusionForPaperSize;
                            break;
                        case 3:
                            WbStrings[4] = cc.WarningBoxString;
                            txtPConfig4.Text = cc.PantographConfiguration;
                            if (cc.InclusionRegion != null)
                            {
                                txtInclX4.Text = cc.InclusionRegion.XAnchor.ToString();
                                txtInclY4.Text = cc.InclusionRegion.YAnchor.ToString();
                                txtInclW4.Text = cc.InclusionRegion.Width.ToString();
                                txtInclH4.Text = cc.InclusionRegion.Height.ToString();
                            }
                            foreach (PantographRegionObjectType prot in cc.ExclusionRegions)
                            {
                                AddExcToList4(prot.XAnchor.ToString(), prot.YAnchor.ToString(), prot.Width.ToString(), prot.Height.ToString());
                            }
                            numIPtrn4.Value = Convert.ToDecimal(cc.InterferencePatternId);
                            cboColor4.Text = cc.PantographColor.ToString().Substring(2);
                            cellCntr = 1;
                            txtBorderTxt4.Text = cc.BorderString;
                            chkDynamicMp4.Checked = cc.UseDynamicMp;
                            if (cc.TROYmarkOn)
                            {
                                if (cc.UseDynamicTmMsg)
                                {
                                    radDynamicData4.Checked = true;
                                }
                                else
                                {
                                    radStaticData4.Checked = true;
                                }
                            }
                            else
                            {
                                radNoTmMsg4.Checked = true;
                            }
                            chkUseDefaultInclusion4.Checked = cc.UseDefaultInclusionForPaperSize;
                            break;
                    }
                    cntr++;
                }
            }

           
        }

        private void AddExcToList1(string XAnc, string YAnc, string Wdth, string Hgt)
        {
            string CompleteString;

            CompleteString = "XAnchor: " + XAnc + " YAnchor: " + YAnc + " Width: " + Wdth + " Height: " + Hgt;
            lstExclusions1.Items.Add(CompleteString);

        }
        private void AddExcToList2(string XAnc, string YAnc, string Wdth, string Hgt)
        {
            string CompleteString;

            CompleteString = "XAnchor: " + XAnc + " YAnchor: " + YAnc + " Width: " + Wdth + " Height: " + Hgt;
            lstExclusions2.Items.Add(CompleteString);

        }
        private void AddExcToList3(string XAnc, string YAnc, string Wdth, string Hgt)
        {
            string CompleteString;

            CompleteString = "XAnchor: " + XAnc + " YAnchor: " + YAnc + " Width: " + Wdth + " Height: " + Hgt;
            lstExclusions3.Items.Add(CompleteString);

        }
        private void AddExcToList4(string XAnc, string YAnc, string Wdth, string Hgt)
        {
            string CompleteString;

            CompleteString = "XAnchor: " + XAnc + " YAnchor: " + YAnc + " Width: " + Wdth + " Height: " + Hgt;
            lstExclusions4.Items.Add(CompleteString);

        }

        private void btnRemoveExc1_Click(object sender, EventArgs e)
        {
            if (lstExclusions1.SelectedIndex > -1)
            {
                lstExclusions1.Items.RemoveAt(lstExclusions1.SelectedIndex);
                //StandardControl_Changed(sender, e);
            }
        }

        private void btnRemoveExc2_Click(object sender, EventArgs e)
        {
            if (lstExclusions2.SelectedIndex > -1)
            {
                lstExclusions2.Items.RemoveAt(lstExclusions2.SelectedIndex);
                //StandardControl_Changed(sender, e);
            }

        }

        private void btnRemoveExc3_Click(object sender, EventArgs e)
        {
            if (lstExclusions3.SelectedIndex > -1)
            {
                lstExclusions3.Items.RemoveAt(lstExclusions3.SelectedIndex);
                //StandardControl_Changed(sender, e);
            }

        }

        private void btnRemoveExc4_Click(object sender, EventArgs e)
        {
            if (lstExclusions4.SelectedIndex > -1)
            {
                lstExclusions4.Items.RemoveAt(lstExclusions4.SelectedIndex);
                //StandardControl_Changed(sender, e);
            }

        }

        private void btnAddExc_Click(object sender, EventArgs e)
        {


        }

        private void btnAddExc4_Click(object sender, EventArgs e)
        {
            AddExclusion ae = new AddExclusion();
            ae.ShowDialog();
            if (ae.XAnchor != "")
            {
                //StandardControl_Changed(sender, e);
                AddExcToList4(ae.XAnchor, ae.YAnchor, ae.ExcWidth, ae.ExcHeight);
            }

        }

        private void btnAddExc3_Click(object sender, EventArgs e)
        {
            AddExclusion ae = new AddExclusion();
            ae.ShowDialog();
            if (ae.XAnchor != "")
            {
                //StandardControl_Changed(sender, e);
                AddExcToList3(ae.XAnchor, ae.YAnchor, ae.ExcWidth, ae.ExcHeight);
            }

        }

        private void btnAddExc2_Click(object sender, EventArgs e)
        {
            AddExclusion ae = new AddExclusion();
            ae.ShowDialog();
            if (ae.XAnchor != "")
            {
                //StandardControl_Changed(sender, e);
                AddExcToList2(ae.XAnchor, ae.YAnchor, ae.ExcWidth, ae.ExcHeight);
            }

        }

        private void btnAddExc1_Click(object sender, EventArgs e)
        {
            AddExclusion ae = new AddExclusion();
            ae.ShowDialog();
            if (ae.XAnchor != "")
            {
                //StandardControl_Changed(sender, e);
                AddExcToList1(ae.XAnchor, ae.YAnchor, ae.ExcWidth, ae.ExcHeight);
            }

        }

        private void btnConfig1_Click(object sender, EventArgs e)
        {
            PantoConfig pc = new PantoConfig();
            pc.ConfigValue = txtPConfig1.Text;
            pc.ShowDialog();
            txtPConfig1.Text = pc.ConfigValue;
        }

        private void btnConfig2_Click(object sender, EventArgs e)
        {
            PantoConfig pc = new PantoConfig();
            pc.ConfigValue = txtPConfig2.Text;
            pc.ShowDialog();
            txtPConfig2.Text = pc.ConfigValue;

        }

        private void btnConfig3_Click(object sender, EventArgs e)
        {
            PantoConfig pc = new PantoConfig();
            pc.ConfigValue = txtPConfig3.Text;
            pc.ShowDialog();
            txtPConfig3.Text = pc.ConfigValue;

        }

        private void btnConfig4_Click(object sender, EventArgs e)
        {
            PantoConfig pc = new PantoConfig();
            pc.ConfigValue = txtPConfig4.Text;
            pc.ShowDialog();
            txtPConfig4.Text = pc.ConfigValue;

        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (ValidateValues())
            {
                if (SaveSettings())
                {
                    this.Close();
                }

            }
        }

        private bool SaveSettings()
        {
            PantographConfiguration pc = new PantographConfiguration();
            for (int cntr = 0; cntr < 4; cntr++)
            {
                CustomConfiguration tempcc;
                if (PConfigs.PantographConfigurations.Count < cntr + 1)
                {
                    tempcc = new CustomConfiguration();
                }
                else
                {
                    tempcc = PConfigs.PantographConfigurations[cntr];
                }
                if (cntr == 0)
                {
                    tempcc.BorderString = txtBorderTxt1.Text;
                    tempcc.UseDynamicMp = chkDynamicMp1.Checked;
                    tempcc.CellList.Clear();
                    for (int cntr1 = 0; cntr1 < 10; cntr1++)
                    {
                        if (Globals.CellSetups[0, cntr1].PatternEnabled)
                        {
                            PantographCellDescriptorType pcdt = new PantographCellDescriptorType();
                            pcdt.pidx = cntr1+1;
                            pcdt.msg = Globals.CellSetups[0, cntr1].CellText;
                            tempcc.CellList.Add(pcdt);
                        }
                    }
                    if (!string.IsNullOrEmpty(Globals.CustomPatternName[0]))
                    {
                        tempcc.CustomPatternName = Globals.CustomPatternName[0];
                        foreach (TroyPatterns tp in Globals.customPatterns.TroyStoredPattern)
                        {
                            if (tp.StoredPatternName == Globals.CustomPatternName[0])
                            {
                                tempcc.CustomBackgroundPatternData = tp.BackgroundPattern;
                                tempcc.CustomForegroundPatternData = tp.ForegroundPattern;
                            }
                        }

                    }
                    tempcc.ExclusionRegions.Clear();
                    foreach (string str in lstExclusions1.Items)
                    {
                        int XInd = str.IndexOf("XAnchor:");
                        int YInd = str.IndexOf("YAnchor:");
                        int WInd = str.IndexOf("Width:");
                        int HInd = str.IndexOf("Height:");
                        if ((XInd > -1) && (YInd > -1) && (WInd > -1) && (HInd > -1))
                        {
                            PantographRegionObjectType exc = new PantographRegionObjectType();
                            string temp = str.Substring(XInd + 9, YInd - (XInd + 9));
                            exc.XAnchor = Convert.ToInt32(temp);
                            temp = str.Substring(YInd + 9, WInd - (YInd + 9));
                            exc.YAnchor = Convert.ToInt32(temp);
                            temp = str.Substring(WInd + 7, HInd - (WInd + 7));
                            exc.Width = Convert.ToInt32(temp);
                            temp = str.Substring(HInd + 8);
                            exc.Height = Convert.ToInt32(temp);
                            tempcc.ExclusionRegions.Add(exc);
                        }
                    }
                    tempcc.UseDefaultInclusionForPaperSize = chkUseDefaultInclusion1.Checked;
                    if (!chkUseDefaultInclusion1.Checked)
                    {
                        if (txtInclX1.Text != "")
                        {
                            if (tempcc.InclusionRegion == null)
                            {
                                tempcc.InclusionRegion = new PantographRegionObjectType();
                            }
                            tempcc.InclusionRegion.XAnchor = Convert.ToInt32(txtInclX1.Text);
                            tempcc.InclusionRegion.YAnchor = Convert.ToInt32(txtInclY1.Text);
                            tempcc.InclusionRegion.Width = Convert.ToInt32(txtInclW1.Text);
                            tempcc.InclusionRegion.Height = Convert.ToInt32(txtInclH1.Text);
                        }
                    }
                    tempcc.InterferencePatternId = Convert.ToInt32(numIPtrn1.Value);
                    tempcc.PantographColor =
                        (PantographColorType)
                            Enum.Parse(typeof(PantographColorType), "pg" + cboColor1.Text);
                    tempcc.PageOrientation = masterPO;
                    tempcc.PageType = masterPT;
                    tempcc.PantographConfiguration = txtPConfig1.Text;
                    if (radDynamicData1.Checked)
                    {
                        tempcc.UseDynamicTmMsg = true;
                        tempcc.TROYmarkOn = true;
                    }
                    else if (radStaticData1.Checked)
                    {
                        tempcc.UseDynamicTmMsg = false;
                        tempcc.TROYmarkOn = true;
                    }
                    else
                    {
                        tempcc.UseDynamicTmMsg = false;
                        tempcc.TROYmarkOn = false;
                    }
                    tempcc.WarningBoxString = WbStrings[1];
                }
                else if (cntr == 1)
                {
                    tempcc.BorderString = txtBorderTxt2.Text;
                    tempcc.UseDynamicMp = chkDynamicMp2.Checked;
                    tempcc.CellList.Clear();
                    for (int cntr1 = 0; cntr1 < 10; cntr1++)
                    {
                        if (Globals.CellSetups[1, cntr1].PatternEnabled)
                        {
                            PantographCellDescriptorType pcdt = new PantographCellDescriptorType();
                            pcdt.pidx = cntr1+1;
                            pcdt.msg = Globals.CellSetups[1, cntr1].CellText;
                            tempcc.CellList.Add(pcdt);
                        }
                    }
                    if (!string.IsNullOrEmpty(Globals.CustomPatternName[1]))
                    {
                        foreach (TroyPatterns tp in Globals.customPatterns.TroyStoredPattern)
                        {
                            if (tp.StoredPatternName == Globals.CustomPatternName[1])
                            {
                                tempcc.CustomBackgroundPatternData = tp.BackgroundPattern;
                                tempcc.CustomForegroundPatternData = tp.ForegroundPattern;
                            }
                        }
                    }

                    tempcc.ExclusionRegions.Clear();
                    foreach (string str in lstExclusions2.Items)
                    {
                        int XInd = str.IndexOf("XAnchor:");
                        int YInd = str.IndexOf("YAnchor:");
                        int WInd = str.IndexOf("Width:");
                        int HInd = str.IndexOf("Height:");
                        if ((XInd > -1) && (YInd > -1) && (WInd > -1) && (HInd > -1))
                        {
                            PantographRegionObjectType exc = new PantographRegionObjectType();
                            string temp = str.Substring(XInd + 9, YInd - (XInd + 9));
                            exc.XAnchor = Convert.ToInt32(temp);
                            temp = str.Substring(YInd + 9, WInd - (YInd + 9));
                            exc.YAnchor = Convert.ToInt32(temp);
                            temp = str.Substring(WInd + 7, HInd - (WInd + 7));
                            exc.Width = Convert.ToInt32(temp);
                            temp = str.Substring(HInd + 8);
                            exc.Height = Convert.ToInt32(temp);
                            tempcc.ExclusionRegions.Add(exc);
                        }
                    }
                    tempcc.UseDefaultInclusionForPaperSize = chkUseDefaultInclusion2.Checked;
                    if (!chkUseDefaultInclusion2.Checked)
                    {
                        if (txtInclX2.Text != "")
                        {
                            if (tempcc.InclusionRegion == null)
                            {
                                tempcc.InclusionRegion = new PantographRegionObjectType();
                            }
                            tempcc.InclusionRegion.XAnchor = Convert.ToInt32(txtInclX2.Text);
                            tempcc.InclusionRegion.YAnchor = Convert.ToInt32(txtInclY2.Text);
                            tempcc.InclusionRegion.Width = Convert.ToInt32(txtInclW2.Text);
                            tempcc.InclusionRegion.Height = Convert.ToInt32(txtInclH2.Text);
                        }
                    }
                    tempcc.InterferencePatternId = Convert.ToInt32(numIPtrn2.Value);
                    tempcc.PantographColor = (PantographColorType)
                                                Enum.Parse(typeof(PantographColorType), "pg" + cboColor2.Text); 
                    tempcc.PageOrientation = masterPO;
                    tempcc.PageType = masterPT;
                    tempcc.PantographConfiguration = txtPConfig2.Text;
                    if (radDynamicData2.Checked)
                    {
                        tempcc.UseDynamicTmMsg = true;
                        tempcc.TROYmarkOn = true;
                    }
                    else if (radStaticData2.Checked)
                    {
                        tempcc.UseDynamicTmMsg = false;
                        tempcc.TROYmarkOn = true;
                    }
                    else
                    {
                        tempcc.UseDynamicTmMsg = false;
                        tempcc.TROYmarkOn = false;
                    }
                    tempcc.WarningBoxString = WbStrings[2];

                }
                else if (cntr == 2)
                {
                    tempcc.BorderString = txtBorderTxt3.Text;
                    tempcc.UseDynamicMp = chkDynamicMp3.Checked;
                    tempcc.CellList.Clear();
                    for (int cntr1 = 0; cntr1 < 10; cntr1++)
                    {
                        if (Globals.CellSetups[2, cntr1].PatternEnabled)
                        {
                            PantographCellDescriptorType pcdt = new PantographCellDescriptorType();
                            pcdt.pidx = cntr1+1;
                            pcdt.msg = Globals.CellSetups[2, cntr1].CellText;
                            tempcc.CellList.Add(pcdt);
                        }
                    }
                    if (!string.IsNullOrEmpty(Globals.CustomPatternName[2]))
                    {
                        foreach (TroyPatterns tp in Globals.customPatterns.TroyStoredPattern)
                        {
                            if (tp.StoredPatternName == Globals.CustomPatternName[2])
                            {
                                tempcc.CustomBackgroundPatternData = tp.BackgroundPattern;
                                tempcc.CustomForegroundPatternData = tp.ForegroundPattern;
                            }
                        }
                    }
                    tempcc.ExclusionRegions.Clear();
                    foreach (string str in lstExclusions3.Items)
                    {
                        int XInd = str.IndexOf("XAnchor:");
                        int YInd = str.IndexOf("YAnchor:");
                        int WInd = str.IndexOf("Width:");
                        int HInd = str.IndexOf("Height:");
                        if ((XInd > -1) && (YInd > -1) && (WInd > -1) && (HInd > -1))
                        {
                            PantographRegionObjectType exc = new PantographRegionObjectType();
                            string temp = str.Substring(XInd + 9, YInd - (XInd + 9));
                            exc.XAnchor = Convert.ToInt32(temp);
                            temp = str.Substring(YInd + 9, WInd - (YInd + 9));
                            exc.YAnchor = Convert.ToInt32(temp);
                            temp = str.Substring(WInd + 7, HInd - (WInd + 7));
                            exc.Width = Convert.ToInt32(temp);
                            temp = str.Substring(HInd + 8);
                            exc.Height = Convert.ToInt32(temp);
                            tempcc.ExclusionRegions.Add(exc);
                        }
                    }
                    tempcc.UseDefaultInclusionForPaperSize = chkUseDefaultInclusion3.Checked;
                    if (!chkUseDefaultInclusion3.Checked)
                    {
                        if (txtInclX3.Text != "")
                        {
                            if (tempcc.InclusionRegion == null)
                            {
                                tempcc.InclusionRegion = new PantographRegionObjectType();
                            }
                            tempcc.InclusionRegion.XAnchor = Convert.ToInt32(txtInclX3.Text);
                            tempcc.InclusionRegion.YAnchor = Convert.ToInt32(txtInclY3.Text);
                            tempcc.InclusionRegion.Width = Convert.ToInt32(txtInclW3.Text);
                            tempcc.InclusionRegion.Height = Convert.ToInt32(txtInclH3.Text);
                        }
                    }
                    tempcc.InterferencePatternId = Convert.ToInt32(numIPtrn3.Value);
                    tempcc.PantographColor =
                        (PantographColorType)
                            Enum.Parse(typeof(PantographColorType), "pg" + cboColor3.Text);
                    tempcc.PageOrientation = masterPO;
                    tempcc.PageType = masterPT;
                    tempcc.PantographConfiguration = txtPConfig3.Text;
                    if (radDynamicData3.Checked)
                    {
                        tempcc.UseDynamicTmMsg = true;
                        tempcc.TROYmarkOn = true;
                    }
                    else if (radStaticData3.Checked)
                    {
                        tempcc.UseDynamicTmMsg = false;
                        tempcc.TROYmarkOn = true;
                    }
                    else
                    {
                        tempcc.UseDynamicTmMsg = false;
                        tempcc.TROYmarkOn = false;
                    }
                    tempcc.WarningBoxString = WbStrings[3];
                }
                else if (cntr == 3)
                {
                    tempcc.BorderString = txtBorderTxt4.Text;
                    tempcc.UseDynamicMp = chkDynamicMp4.Checked;
                    tempcc.CellList.Clear();
                    for (int cntr1 = 0; cntr1 < 10; cntr1++)
                    {
                        if (Globals.CellSetups[3, cntr1].PatternEnabled)
                        {
                            PantographCellDescriptorType pcdt = new PantographCellDescriptorType();
                            pcdt.pidx = cntr1+1;
                            pcdt.msg = Globals.CellSetups[3, cntr1].CellText;
                            tempcc.CellList.Add(pcdt);
                        }
                    }
                    if (!string.IsNullOrEmpty(Globals.CustomPatternName[3]))
                    {
                        foreach (TroyPatterns tp in Globals.customPatterns.TroyStoredPattern)
                        {
                            if (tp.StoredPatternName == Globals.CustomPatternName[3])
                            {
                                tempcc.CustomBackgroundPatternData = tp.BackgroundPattern;
                                tempcc.CustomForegroundPatternData = tp.ForegroundPattern;
                            }
                        }
                    }
                    tempcc.ExclusionRegions.Clear();
                    foreach (string str in lstExclusions4.Items)
                    {
                        int XInd = str.IndexOf("XAnchor:");
                        int YInd = str.IndexOf("YAnchor:");
                        int WInd = str.IndexOf("Width:");
                        int HInd = str.IndexOf("Height:");
                        if ((XInd > -1) && (YInd > -1) && (WInd > -1) && (HInd > -1))
                        {
                            PantographRegionObjectType exc = new PantographRegionObjectType();
                            string temp = str.Substring(XInd + 9, YInd - (XInd + 9));
                            exc.XAnchor = Convert.ToInt32(temp);
                            temp = str.Substring(YInd + 9, WInd - (YInd + 9));
                            exc.YAnchor = Convert.ToInt32(temp);
                            temp = str.Substring(WInd + 7, HInd - (WInd + 7));
                            exc.Width = Convert.ToInt32(temp);
                            temp = str.Substring(HInd + 8);
                            exc.Height = Convert.ToInt32(temp);
                            tempcc.ExclusionRegions.Add(exc);
                        }
                    }
                    tempcc.UseDefaultInclusionForPaperSize = chkUseDefaultInclusion4.Checked;
                    if (!chkUseDefaultInclusion4.Checked)
                    {
                        if (txtInclX4.Text != "")
                        {
                            if (tempcc.InclusionRegion == null)
                            {
                                tempcc.InclusionRegion = new PantographRegionObjectType();
                            }
                            tempcc.InclusionRegion.XAnchor = Convert.ToInt32(txtInclX4.Text);
                            tempcc.InclusionRegion.YAnchor = Convert.ToInt32(txtInclY4.Text);
                            tempcc.InclusionRegion.Width = Convert.ToInt32(txtInclW4.Text);
                            tempcc.InclusionRegion.Height = Convert.ToInt32(txtInclH4.Text);
                        }
                    }
                    tempcc.InterferencePatternId = Convert.ToInt32(numIPtrn4.Value);
                    tempcc.PantographColor =
                        (PantographColorType)
                            Enum.Parse(typeof(PantographColorType), "pg" + cboColor4.Text);
                    tempcc.PageOrientation = masterPO;
                    tempcc.PageType = masterPT;
                    tempcc.PantographConfiguration = txtPConfig4.Text;
                    if (radDynamicData4.Checked)
                    {
                        tempcc.UseDynamicTmMsg = true;
                        tempcc.TROYmarkOn = true;
                    }
                    else if (radStaticData4.Checked)
                    {
                        tempcc.UseDynamicTmMsg = false;
                        tempcc.TROYmarkOn = true;
                    }
                    else
                    {
                        tempcc.UseDynamicTmMsg = false;
                        tempcc.TROYmarkOn = false;
                    }
                    tempcc.WarningBoxString = WbStrings[4];

                }

                if (tempcc != null)
                {
                    pc.PantographConfigurations.Add(tempcc);
                }
                
            }

            if (pc.PantographConfigurations.Count > 3)
            {
                PConfigs = pc;
            }
            else
            {
                MessageBox.Show("Error saving pantograph configuration settings. ", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }

            return true;
        }

        private bool ValidateValues()
        {
            bool allPgIncFieldsSet1 = ((txtInclX1.Text != "") && (txtInclY1.Text != "") && (txtInclW1.Text != "") && (txtInclH1.Text != ""));
            bool allPgIncFieldsCleared1 = ((txtInclX1.Text == "") && (txtInclY1.Text == "") && (txtInclW1.Text == "") && (txtInclH1.Text == ""));
            if ((!allPgIncFieldsSet1) && (!allPgIncFieldsCleared1))
            {
                tabMainPConfig.SelectTab("Pantograph1");
                MessageBox.Show("Error! Not all Pantograph Inclusion fields are set for Pantograph 1.");
                return false;
            }

            bool allPgIncFieldsSet2 = ((txtInclX2.Text != "") && (txtInclY2.Text != "") && (txtInclW2.Text != "") && (txtInclH2.Text != ""));
            bool allPgIncFieldsCleared2 = ((txtInclX2.Text == "") && (txtInclY2.Text == "") && (txtInclW2.Text == "") && (txtInclH2.Text == ""));
            if ((!allPgIncFieldsSet2) && (!allPgIncFieldsCleared2))
            {
                tabMainPConfig.SelectTab("Pantograph2");
                MessageBox.Show("Error! Not all Pantograph Inclusion fields are set for Pantograph 2.");
                return false;
            }

            bool allPgIncFieldsSet3 = ((txtInclX3.Text != "") && (txtInclY3.Text != "") && (txtInclW3.Text != "") && (txtInclH3.Text != ""));
            bool allPgIncFieldsCleared3 = ((txtInclX3.Text == "") && (txtInclY3.Text == "") && (txtInclW3.Text == "") && (txtInclH3.Text == ""));
            if ((!allPgIncFieldsSet3) && (!allPgIncFieldsCleared3))
            {
                tabMainPConfig.SelectTab("Pantograph3");
                MessageBox.Show("Error! Not all Pantograph Inclusion fields are setfor Pantograph 3.");
                return false;
            }

            bool allPgIncFieldsSet4 = ((txtInclX4.Text != "") && (txtInclY4.Text != "") && (txtInclW4.Text != "") && (txtInclH4.Text != ""));
            bool allPgIncFieldsCleared4 = ((txtInclX4.Text == "") && (txtInclY4.Text == "") && (txtInclW4.Text == "") && (txtInclH4.Text == ""));
            if ((!allPgIncFieldsSet4) && (!allPgIncFieldsCleared4))
            {
                tabMainPConfig.SelectTab("Pantograph4");
                MessageBox.Show("Error! Not all Pantograph Inclusion fields are setfor Pantograph 4.");
                return false;
            }


            return true;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }


        private void chkUseDefaultInclusion_CheckedChanged(object sender, EventArgs e)
        {
            if (chkUseDefaultInclusion1.Checked)
            {
                lblXAnchor1.Enabled = false;
                lblYAnchor1.Enabled = false;
                lblWidth1.Enabled = false;
                lblHeight1.Enabled = false;
                txtInclH1.Enabled = false;
                txtInclW1.Enabled = false;
                txtInclX1.Enabled = false;
                txtInclY1.Enabled = false;
            }
            else
            {
                lblXAnchor1.Enabled = true;
                lblYAnchor1.Enabled = true;
                lblWidth1.Enabled = true;
                lblHeight1.Enabled = true;
                txtInclH1.Enabled = true;
                txtInclW1.Enabled = true;
                txtInclX1.Enabled = true;
                txtInclY1.Enabled = true;
            }
        }

        private void chkUseDefaultInclusion2_CheckedChanged(object sender, EventArgs e)
        {
            if (chkUseDefaultInclusion2.Checked)
            {
                lblXAnchor2.Enabled = false;
                lblYAnchor2.Enabled = false;
                lblWidth2.Enabled = false;
                lblHeight2.Enabled = false;
                txtInclH2.Enabled = false;
                txtInclW2.Enabled = false;
                txtInclX2.Enabled = false;
                txtInclY2.Enabled = false;
            }
            else
            {
                lblXAnchor2.Enabled = true;
                lblYAnchor2.Enabled = true;
                lblWidth2.Enabled = true;
                lblHeight2.Enabled = true;
                txtInclH2.Enabled = true;
                txtInclW2.Enabled = true;
                txtInclX2.Enabled = true;
                txtInclY2.Enabled = true;
            }

        }

        private void chkUseDefaultInclusion3_CheckedChanged(object sender, EventArgs e)
        {
            if (chkUseDefaultInclusion3.Checked)
            {
                lblXAnchor3.Enabled = false;
                lblYAnchor3.Enabled = false;
                lblWidth3.Enabled = false;
                lblHeight3.Enabled = false;
                txtInclH3.Enabled = false;
                txtInclW3.Enabled = false;
                txtInclX3.Enabled = false;
                txtInclY3.Enabled = false;
            }
            else
            {
                lblXAnchor3.Enabled = true;
                lblYAnchor3.Enabled = true;
                lblWidth3.Enabled = true;
                lblHeight3.Enabled = true;
                txtInclH3.Enabled = true;
                txtInclW3.Enabled = true;
                txtInclX3.Enabled = true;
                txtInclY3.Enabled = true;
            }

        }

        private void chkUseDefaultInclusion4_CheckedChanged(object sender, EventArgs e)
        {
            if (chkUseDefaultInclusion4.Checked)
            {
                lblXAnchor4.Enabled = false;
                lblYAnchor4.Enabled = false;
                lblWidth4.Enabled = false;
                lblHeight4.Enabled = false;
                txtInclH4.Enabled = false;
                txtInclW4.Enabled = false;
                txtInclX4.Enabled = false;
                txtInclY4.Enabled = false;
            }
            else
            {
                lblXAnchor4.Enabled = true;
                lblYAnchor4.Enabled = true;
                lblWidth4.Enabled = true;
                lblHeight4.Enabled = true;
                txtInclH4.Enabled = true;
                txtInclW4.Enabled = true;
                txtInclX4.Enabled = true;
                txtInclY4.Enabled = true;
            }

        }

        private void Pantograph1_Click(object sender, EventArgs e)
        {

        }

        private void btnSetPatterns1_Click(object sender, EventArgs e)
        {
            if (!Globals.Lite)
            {
                var pcs = new NewPantoCells();
                pcs.PantographId = 0;
                pcs.ShowDialog();
              
            }
            else
            {
                var pcs = new PantoCells();
                pcs.PantographId = 0;
                pcs.ShowDialog();
            }
        }

        private void btnPatterns1_Click(object sender, EventArgs e)
        {
            InterferencePatterns ip = new InterferencePatterns();
            ip.SelectedPattern = Convert.ToInt32(numIPtrn1.Value);
            ip.ShowDialog();
            if (!ip.Cancelled)
            {
                numIPtrn1.Value = Convert.ToDecimal(ip.SelectedPattern);
            }

        }

        private void btnSetPatterns2_Click(object sender, EventArgs e)
        {
            PantoCells pcs = new PantoCells();
            pcs.PantographId = 1;
            pcs.ShowDialog();
        }

        private void btnSetPatterns3_Click(object sender, EventArgs e)
        {
            PantoCells pcs = new PantoCells();
            pcs.PantographId = 2;
            pcs.ShowDialog();
        }

        private void btnSetPatterns4_Click(object sender, EventArgs e)
        {
            PantoCells pcs = new PantoCells();
            pcs.PantographId = 3;
            pcs.ShowDialog();
        }

        private void btnPatterns2_Click(object sender, EventArgs e)
        {
            InterferencePatterns ip = new InterferencePatterns();
            ip.SelectedPattern = Convert.ToInt32(numIPtrn2.Value);
            ip.ShowDialog();
            if (!ip.Cancelled)
            {
                numIPtrn2.Value = Convert.ToDecimal(ip.SelectedPattern);
            }
        }

        private void btnPatterns3_Click(object sender, EventArgs e)
        {
            InterferencePatterns ip = new InterferencePatterns();
            ip.SelectedPattern = Convert.ToInt32(numIPtrn3.Value);
            ip.ShowDialog();
            if (!ip.Cancelled)
            {
                numIPtrn3.Value = Convert.ToDecimal(ip.SelectedPattern);
            }
        }

        private void btnPatterns4_Click(object sender, EventArgs e)
        {
            InterferencePatterns ip = new InterferencePatterns();
            ip.SelectedPattern = Convert.ToInt32(numIPtrn4.Value);
            ip.ShowDialog();
            if (!ip.Cancelled)
            {
                numIPtrn4.Value = Convert.ToDecimal(ip.SelectedPattern);
            }
        }

        private void radStaticData_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void Pantograph3_Click(object sender, EventArgs e)
        {

        }

        private void btnWarningBox1_Click(object sender, EventArgs e)
        {
            WarningBoxConfig wbc = new WarningBoxConfig();
            wbc.WarningBoxString = WbStrings[1];
            wbc.ShowDialog();
            WbStrings[1] = wbc.WarningBoxString;
        }

        private void btnWarningBox2_Click(object sender, EventArgs e)
        {
            WarningBoxConfig wbc = new WarningBoxConfig();
            wbc.WarningBoxString = WbStrings[2];
            wbc.ShowDialog();
            WbStrings[2] = wbc.WarningBoxString;
        }

        private void btnWarningBox3_Click(object sender, EventArgs e)
        {
            WarningBoxConfig wbc = new WarningBoxConfig();
            wbc.WarningBoxString = WbStrings[3];
            wbc.ShowDialog();
            WbStrings[3] = wbc.WarningBoxString;
        }

        private void btnWarningBox4_Click(object sender, EventArgs e)
        {
            WarningBoxConfig wbc = new WarningBoxConfig();
            wbc.WarningBoxString = WbStrings[4];
            wbc.ShowDialog();
            WbStrings[4] = wbc.WarningBoxString;

        }

        
        
    }
}
