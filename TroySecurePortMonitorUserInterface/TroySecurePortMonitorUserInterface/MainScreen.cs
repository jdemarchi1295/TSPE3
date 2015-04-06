using Troy.Licensing.Client.Classes;

namespace TroySecurePortMonitorUserInterface
{
    #region References

    using System;
    using System.Diagnostics;
    using System.Collections.Generic;
    using System.Data;
    using System.IO;
    using System.Management;
    using System.Reflection;
    using System.ServiceProcess;
    using System.Threading;
    using System.Windows.Forms;
    using System.Xml.Serialization;
    using Microsoft.Win32;
    //using Troy.Core.Licensing;
    using PantographPclBuilder;
    using Troy.PortMonitor.Core.XmlConfiguration;
    using TroySecurePortMonitorUserInterface.Pantograph2;
    using Troy;
    #endregion

    public partial class MainScreen : Form
    {
        private const string CUSTOM_PATTERNS_FILENAME = "TroyCustomPatterns.xml";
        private const string CurrentVersion = "1.0.0";

        private const int bit0EnablePantograph = 1;
        private const int bit1EnablePantograph = 2;
        private const int bit2MicroPrintBorder = 4;
        private const int bit3MicroPrintBorder = 8;
        private const int bit4WarningBox = 16;
        private const int bit5SigLine = 32;
        private const int bit6SigLine = 64;
        private const int bit7BackOfPage = 128;
        private const int bit8Interference = 256;
        private static readonly Dictionary<string, string> portToBasePath = new Dictionary<string, string>();
        private static readonly Dictionary<string, string> portToConfigPath = new Dictionary<string, string>();
        private static readonly Dictionary<string, string> portToDataPath = new Dictionary<string, string>();
        private readonly List<string> PortTypes = new List<string>();
        private string CurrentLicenseLabelText = "";
        private bool ErrorLoggingVisible = true;

        private bool SaveButtonSelected;

        private bool UsingSoftwarePantograph;
        private bool UsingSoftwareTroymark;
        private string baseFilePath = "";

        private DataSet dsPrinterString = new DataSet("PrintStrings");
        private string filePath = "<Not loaded>";

        private bool possibleChange;
        private PantographConfiguration swPantograph;
        private static LicenseQueryResult lqr;

        public TroyPortMonitorConfiguration tpmc_local;
        public TroyPortMonitorServiceConfiguration tpmsc_local;
        public static string ProductTitle;

        public static string Version
        {
            get
            {
                Assembly asm = Assembly.GetExecutingAssembly();
                FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(asm.Location);
                return String.Format("{0}.{1}", fvi.ProductMajorPart, fvi.ProductMinorPart);
            }
        }

        public MainScreen()
        {
            InitializeComponent();
        }

        private void MainScreen_Load(object sender, EventArgs e)
        {
            tabMain.TabPages.Remove(MicroPrint);      
            //Set the Copyright characters
            chkEnableTroyMark.Text = "Enable TROYmark\u2122";
            lblCopyright.Text = "\u00A9 Copyright  TROY Group Inc.  2014 ";

            string message;

            //Get the base file path
            if (!GetBasePath())
            {
                Close();
            }
            else
            {
                if (!ValidLicense(filePath, false, out lqr, out message))
                {
                    MessageBox.Show("Invalid Licensing option: " + message);
                    CurrentLicenseLabelText = message;
                    Close();
                }
                else
                {
                    CurrentLicenseLabelText = String.Format("{0} {1} printer(s)",lqr.LicenseStatus, lqr.PrinterCount);
                }
            }


            //Version 1.0.16:  checking to see if there are multiple port types (folders inside the Configuration folder)
            var configurationFilePath = baseFilePath + "Configuration";
            var di = new DirectoryInfo(configurationFilePath);
            var subdircnt = di.GetDirectories().Length;
            if (subdircnt > 1)
            {
                //btnPortType.Visible = true;
                //btnPortType.Enabled = true;
                //foreach (DirectoryInfo subdi in di.GetDirectories())
                //{
                //    PortTypes.Add(subdi.Name);
                //}
            }
            else
            {
                btnPortType.Visible = false;
                btnPortType.Enabled = false;
            }

            //Read in the Port Monitor Service configuration.
            if (!ReadPortMonitorServiceConfig())
            {
                Close();
            }

            LoadBaselineDensitySettings();
            LoadCustomPatterns();
        }

        private void LoadCustomPatterns()
        {
            if (File.Exists(Application.StartupPath + @"\Baseline\" + CUSTOM_PATTERNS_FILENAME))
            {
                var dser = new XmlSerializer(typeof (TroyCustomPatterns));
                var fs = new FileStream(Application.StartupPath + @"\Baseline\" + CUSTOM_PATTERNS_FILENAME, FileMode.Open, FileAccess.Read, FileShare.Read);
                Globals.customPatterns = (TroyCustomPatterns) dser.Deserialize(fs);
                fs.Close();
            }
        }

        private void LoadBaselineDensitySettings()
        {
            try
            {
                var di = new DirectoryInfo(Application.StartupPath + @"\Baseline\");
                if (di.Exists)
                {
                    foreach (var dir in di.GetDirectories())
                    {
                        if (File.Exists(dir.FullName + @"\TroyPantographConfiguration.xml"))
                        {
                            var dser = new XmlSerializer(typeof(PantographConfiguration));
                            var fs = new FileStream(dir.FullName + @"\TroyPantographConfiguration.xml", FileMode.Open, FileAccess.Read, FileShare.Read);
                            var temp = (PantographConfiguration)dser.Deserialize(fs);
                            fs.Close();
                            var dirName = dir.Name;
                            var den = new Globals.Densities();
                            den.DensityPattern1 = temp.PantographConfigurations[0].DensityPattern1;
                            den.DensityPattern2 = temp.PantographConfigurations[0].DensityPattern2;
                            den.DensityPattern3 = temp.PantographConfigurations[0].DensityPattern3;
                            den.DensityPattern4 = temp.PantographConfigurations[0].DensityPattern4;
                            den.DensityPattern5 = temp.PantographConfigurations[0].DensityPattern5;
                            den.DensityPattern6 = temp.PantographConfigurations[0].DensityPattern6;
                            den.DensityPattern7 = temp.PantographConfigurations[0].DensityPattern7;
                            den.DensityPattern8 = temp.PantographConfigurations[0].DensityPattern8;
                            den.DensityPattern9 = temp.PantographConfigurations[0].DensityPattern9;
                            den.DarknessFactor = temp.PantographConfigurations[0].BgDarknessFactor;
                            Globals.BaselineDensity.Add(dirName, den);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in LoadBaselineDensitySettings.  Error: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //Gets the base configuration path from the registry
        private bool GetBasePath()
        {
            try
            {
                var registryKey = Registry.LocalMachine;
                var pmKey = registryKey.OpenSubKey
                    ("System\\CurrentControlSet\\Control\\Print\\Monitors\\TroySecurePortMonitor", false);

                filePath = pmKey.GetValue("MainConfigurationPath").ToString();
                if ((filePath.Length > 0) && (!filePath.EndsWith("\\")))
                {
                    filePath += "\\";
                }
                baseFilePath = filePath;
                pmKey.Close();
                registryKey.Close();
                Globals.appDataPath = baseFilePath + "Data\\";
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    string.Format(
                        "Error getting the base configuration path from registry. Can not continue.  Error: {0}",
                        ex.Message), "Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        //Read TroyPMServiceConfiguration.xml
        private bool ReadPortMonitorServiceConfig()
        {
            try
            {
                //Check if the service configuration xml file exists.
                var serviceXmlFile = filePath + "TroyPMServiceConfiguration.xml";
                if (!File.Exists(serviceXmlFile))
                {
                    MessageBox.Show(
                        string.Format("Fatal Error.  File {0} does not exist.  Can not continue.", serviceXmlFile),
                        "Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Close();
                }

                //Deserialize the service configuration
                var dser1 = new XmlSerializer(typeof (TroyPortMonitorServiceConfiguration));
                var fs = new FileStream(serviceXmlFile, FileMode.Open, FileAccess.Read, FileShare.Read);
                tpmsc_local = (TroyPortMonitorServiceConfiguration) dser1.Deserialize(fs);
                fs.Close();

                // set TSPE product, version
                ProductTitle = tpmsc_local.UserInterface.ProductTitle;
                Text = ProductTitle;
                Globals.Lite = tpmsc_local.UserInterface.Lite;
                if (Globals.Lite)
                {
                    tpmsc_local.UserInterface.EnableDataCapture = false;
                    gbDynamicPrint.Visible = false;
                    btnPanto2.Visible = false;
                    btnTextMap.Visible = false;
                    tabMain.TabPages.Remove(Printing);
                    label12.Text = "Printer";
                }
                Text += " v"+Version;
                
                //Set the TSPE pantograph
                UsingSoftwarePantograph = tpmsc_local.UserInterface.UseAsSecurePrint;
                if (UsingSoftwarePantograph)
                {
                    foreach (var str in Enum.GetNames(typeof(PageOrientationType)))
                    {
                        //Limiting this first release to Potrait and Landscape
                        if ((str == "poPortrait") || (str == "poLandscape"))
                        {
                            cboPageOrientation.Items.Add(str.Substring(2));
                        }
                    }

                    foreach (var str in Enum.GetNames(typeof (PageType)))
                    {
                        cboPageSize.Items.Add(str.Substring(2));
                    }
                }
                else
                {
                    if (tabMain.TabPages.ContainsKey("PantographNF"))
                    {
                        tabMain.TabPages.RemoveByKey("PantographNF");
                    }
                }

                // Enable / Disable PassThrough
                btnPassThrough.Visible = tpmsc_local.UserInterface.EnableDataCapture;

                //First release, all or none. All TSPE or none.
                UsingSoftwareTroymark = UsingSoftwarePantograph;
                if (UsingSoftwareTroymark)
                {
                    UsingSoftwareTroymark = true;
                    lblLineSpacing.Visible = true;
                    numLineSpacing.Visible = true;
                    lblMaxCharsPerLine.Visible = true;
                    numMaxCharPerLine.Visible = true;
                    chkTroyMarkOnBack.Visible = true;
                    txtStaticText.Visible = true;
                    lblStaticText.Visible = true;
                }

                //Error tab is not enabled, remove the tab
                if (!tpmsc_local.UserInterface.ShowErrorTab)
                {
                    ErrorLoggingVisible = false;
                    tabMain.TabPages.RemoveByKey("ErrorLogging");
                }

                //Loop through each port
                string portString, portConfigPath, portMonName, portPath, dataPath = "";
                foreach (var tsp in tpmsc_local.PortList)
                {
                    portString = tsp.PortName;
                    cboTroySecurePortMonitor.Items.Add(portString);

                    portConfigPath = tsp.ConfigurationPath;
                    portMonName = tsp.PortMonitorName;
                    portPath = GetPortPath(portMonName);
                    if (portConfigPath.ToUpper() == "DEFAULT")
                    {
                        if (portPath != "")
                        {
                            portConfigPath = portPath + "Config\\";
                            dataPath = portPath + "Data\\";
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        if ((portConfigPath.Length > 0) && (!portConfigPath.EndsWith("\\")))
                        {
                            portConfigPath += "\\";
                            portPath = portConfigPath;
                            dataPath = portConfigPath + "\\Data\\";
                        }
                    }
                    portToBasePath.Add(portString, portPath);
                    portToConfigPath.Add(portString, portConfigPath);
                    portToDataPath.Add(portString, dataPath);
                }
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in ReadPortMonitorServiceConfig. Application will close. Path: " + filePath,
                    "Error!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                MessageBox.Show(ex.Message, "Exception Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }


        //Returns the path where the file will be written for the Default setting
        private string GetPortPath(string portMonName)
        {
            try
            {
                var registryKey = Registry.LocalMachine;
                var pmKey = registryKey.OpenSubKey
                    ("System\\CurrentControlSet\\Control\\Print\\Monitors\\TroySecurePortMonitor", false);
                var pmPort = pmKey.OpenSubKey("Ports", false);
                var portPath = pmPort.GetValue(portMonName).ToString();

                return portPath;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error in GetPortPath(). Can not get path for port: " + portMonName + " Error: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return "";
            }
        }

        //Read TroyPortMonitorConfiguration.xml
        private bool ReadConfigurationFromXml()
        {
            try
            {
                //Deserialize the service configuration
                var fileName = filePath + "TroyPortMonitorConfiguration.xml";
                var dser1 = new XmlSerializer(typeof (TroyPortMonitorConfiguration));
                var fs1 = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                tpmc_local = (TroyPortMonitorConfiguration) dser1.Deserialize(fs1);
                fs1.Close();

                if (!tpmsc_local.UserInterface.EnableDataCapture)
                {
                    tpmc_local.UseConfigurableDataCapture = false;
                    chkStaticSeparate.Visible = false;
                }
                if (tpmsc_local.UserInterface.Lite)
                {
                    btnTextMap.Visible = false;
                    btnPanto2.Visible = false;

                }

                var mpcntr = 1;
                if (tpmc_local.MicroPrintConfig.Count > 0)
                {
                    txtStaticMpText.Text = tpmc_local.MicroPrintConfig[0].LeadingText;
                    numClearSpaceHeight.Value = tpmc_local.MicroPrintConfig[0].Height;
                    radPoint6.Checked = tpmc_local.MicroPrintConfig[0].UsePoint6;
                }
                else
                {
                    txtStaticMpText.Text = "";
                    numClearSpaceHeight.Value = 60;
                    radPoint6.Checked = false;
                }
                foreach (var mpc in tpmc_local.MicroPrintConfig)
                {
                    if (mpcntr == 1)
                    {
                        numFromLeft1.Value = mpc.XAnchor;
                        numFromTop1.Value = mpc.YAnchor;
                        numWidth1.Value = mpc.Width;
                    }
                    else if (mpcntr == 2)
                    {
                        numFromLeft2.Value = mpc.XAnchor;
                        numFromTop2.Value = mpc.YAnchor;
                        numWidth2.Value = mpc.Width;
                    }
                    else if (mpcntr == 3)
                    {
                        numFromLeft3.Value = mpc.XAnchor;
                        numFromTop3.Value = mpc.YAnchor;
                        numWidth3.Value = mpc.Width;
                    }
                    else if (mpcntr == 4)
                    {
                        numFromLeft4.Value = mpc.XAnchor;
                        numFromTop4.Value = mpc.YAnchor;
                        numWidth4.Value = mpc.Width;
                    }
                    mpcntr++;
                }

                btnDataCapture.Visible = tpmc_local.UseConfigurableDataCapture;
                btnPassThrough.Visible = tpmc_local.UseConfigurableDataCapture;
                radUseAsPrinterName.Enabled = tpmc_local.UseConfigurableDataCapture;
                radUsePrinterMap.Enabled = tpmc_local.UseConfigurableDataCapture;
                dgvPrinterMap.Enabled = tpmc_local.UseConfigurableDataCapture;

                //Error Logging
                if (ErrorLoggingVisible)
                {
                    txtSaveDataFiles.Text = tpmc_local.DebugBackupFilesPath;
                    txtErrorLogPath.Text = tpmc_local.ErrorLogPath;
                    if (tpmc_local.LogErrorsToEventLog)
                    {
                        chkLogErrorToEventLog.Checked = true;
                    }
                    else
                    {
                        chkLogErrorToEventLog.Checked = false;
                    }
                    if ((chkLogErrorToEventLog.Checked) ||
                        (txtSaveDataFiles.Text != "") ||
                        (txtErrorLogPath.Text != ""))
                    {
                        chkEnableErrorLogging.Checked = true;
                    }
                    else
                    {
                        chkEnableErrorLogging.Checked = false;
                    }
                    SetErrorLogPathControls();
                }


                //DEFAULT PRINTER
                cboDefaultPrinter.Text = tpmc_local.DefaultPrinter;

                radUseAsPrinterName.Checked = !tpmc_local.UsePrinterMap;
                radUsePrinterMap.Checked = tpmc_local.UsePrinterMap;

                //TROYMARK
                LoadTroyMarkControls();


                if (UsingSoftwarePantograph)
                {
                    LoadSoftwarePantographControls();
                }

                ReadDataCaptureConfiguration();
                ReadCustomPatterns();
                ReadFontGlyphMap();
                ReadPrinterMap();

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error Opening Port Monitor Configuration. Application will close. Path: " + filePath,
                    "Error!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                MessageBox.Show(ex.Message, "Exception Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private void LoadTroyMarkControls()
        {
            if (tpmc_local.TroyMarkEnabled)
            {
                chkEnableTroyMark.Checked = true;
            }
            else
            {
                chkEnableTroyMark.Checked = false;
            }
            SetTroyMarkControls();
            var tmPattern = tpmc_local.DefaultTroyMarkPattern.ToUpper();
            if ((tmPattern == "DARK") || (tmPattern == "MEDIUM") ||
                (tmPattern == "LIGHT") || (tmPattern == "CUSTOM 1") ||
                (tmPattern == "CUSTOM 2") || (tmPattern == "CUSTOM 3"))
            {
                cboTroyMarkPattern.Text = tmPattern;
            }
            else
            {
                cboTroyMarkPattern.Text = "None";
            }
            cboTmColor.Text = tpmc_local.TextColor.ToString();
            var tmInc = tpmc_local.DefaultTroyMarkInclusion;
            if (tmInc.Length > 6)
            {
                var cntr1 = 1;
                foreach (var subStr1 in tmInc.Split(','))
                {
                    if (cntr1 == 1)
                    {
                        txtTroyMarkIncX.Text = subStr1;
                    }
                    else if (cntr1 == 2)
                    {
                        txtTroyMarkIncY.Text = subStr1;
                    }
                    else if (cntr1 == 3)
                    {
                        txtTroyMarkIncW.Text = subStr1;
                    }
                    else if (cntr1 == 4)
                    {
                        txtTroyMarkIncH.Text = subStr1;
                    }
                    cntr1++;
                }
            }
            else
            {
                txtTroyMarkIncX.Text = "";
                txtTroyMarkIncY.Text = "";
                txtTroyMarkIncW.Text = "";
                txtTroyMarkIncH.Text = "";
            }
            var tmExc = tpmc_local.DefaultTroyMarkExclusion;
            if (tmExc.Length > 6)
            {
                var cntr2 = 1;
                foreach (var subStr2 in tmExc.Split(','))
                {
                    if (cntr2 == 1)
                    {
                        txtTroyMarkExcX.Text = subStr2;
                    }
                    else if (cntr2 == 2)
                    {
                        txtTroyMarkExcY.Text = subStr2;
                    }
                    else if (cntr2 == 3)
                    {
                        txtTroyMarkExcW.Text = subStr2;
                    }
                    else if (cntr2 == 4)
                    {
                        txtTroyMarkExcH.Text = subStr2;
                    }
                    cntr2++;
                }
            }
            else
            {
                txtTroyMarkExcX.Text = "";
                txtTroyMarkExcY.Text = "";
                txtTroyMarkExcW.Text = "";
                txtTroyMarkExcH.Text = "";
            }

            if (tpmc_local.DefaultTroyMarkSpacing != "")
            {
                numLineSpacing.Value = Convert.ToDecimal(tpmc_local.DefaultTroyMarkSpacing);
            }

            numMaxCharPerLine.Value = Convert.ToDecimal(tpmc_local.TroyMarkCharsPerLine);

            chkTroyMarkOnBack.Checked = tpmc_local.TroyMarkOnBack;
            txtStaticText.Text = tpmc_local.TroyMarkStaticText;
            chkStaticSeparate.Checked = tpmc_local.TroymarkDetachStaticText;
        }


        private void LoadSoftwarePantographControls()
        {
            var temp = tpmc_local.PrintPantographProfileList;
            chkPantograph1.Checked = false;
            chkPantograph2.Checked = false;
            chkPantograph3.Checked = false;
            chkPantograph4.Checked = false;
            if (temp.Contains("1"))
            {
                chkPantograph1.Checked = true;
            }
            if (temp.Contains("2"))
            {
                chkPantograph2.Checked = true;
            }
            if (temp.Contains("3"))
            {
                chkPantograph3.Checked = true;
            }
            if (temp.Contains("4"))
            {
                chkPantograph4.Checked = true;
            }

            //Load Pantograph Configuration information
            var dser = new XmlSerializer(typeof(PantographConfiguration));
            var pgFileName = filePath + "TroyPantographConfiguration.xml";
            if (!File.Exists(pgFileName))
            {
                MessageBox.Show(
                    "File not found.  File name: " + pgFileName + ". Can not read pantograph configuration.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                var fs = new FileStream(pgFileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                swPantograph = (PantographConfiguration)dser.Deserialize(fs);
                fs.Close();
                if (swPantograph != null)
                {
                    if (swPantograph.PantographConfigurations[0] != null)
                    {
                        var tp = swPantograph.PantographConfigurations[0].PageOrientation.ToString();
                        cboPageOrientation.Text = tp.Substring(2);
                        tp = swPantograph.PantographConfigurations[0].PageType.ToString();
                        cboPageSize.Text = tp.Substring(2);
                    }

                    var ccCntr = 0;
                    foreach (var cc in swPantograph.PantographConfigurations)
                    {
                        foreach (var pcdt in cc.CellList)
                        {
                            var index = pcdt.pidx;
                            if ((ccCntr < 4) && (index < 11) && (index > 0))
                            {
                                Globals.CellSetups[ccCntr, index - 1].CellText = pcdt.msg;
                                Globals.CellSetups[ccCntr, index - 1].PatternEnabled = true;
                            }
                        }
                        Globals.CustomPatternName[ccCntr] = cc.CustomPatternName;
                        ccCntr++;
                    }
                }
                else
                {
                    MessageBox.Show("Error extracting the configuration from the file " + pgFileName, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
        }

        private bool ReadFontGlyphMap()
        {
            try
            {
                if (File.Exists(baseFilePath + "TroyFontGlyphConfiguration.xml"))
                {
                    var dsers = new XmlSerializer(typeof (TroyFontGlyphMapList));
                    var fss = new FileStream(baseFilePath + "TroyFontGlyphConfiguration.xml", FileMode.Open, FileAccess.Read, FileShare.Read);
                    try
                    {
                        Globals.FontGlyphMapList = (TroyFontGlyphMapList) dsers.Deserialize(fss);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error in ReadFontGlyphMap. Error: " + ex.Message);
                        fss.Close();
                        return false;
                    }
                    fss.Close();
                }
                else
                {
                    MessageBox.Show("Error in ReadFontGlyphMap.  File not found.  File: " + baseFilePath +
                                    "TroyFontGlyphConfiguration.xml");
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in ReadFontGlyphMap. Error: " + ex.Message);
                return false;
            }
        }

        private bool ReadCustomPatterns()
        {
            try
            {
                if (File.Exists(baseFilePath + "TroyCustomPatterns.xml"))
                {
                    var dser = new XmlSerializer(typeof (TroyCustomPatterns));
                    var fs = new FileStream(baseFilePath + "TroyCustomPatterns.xml", FileMode.Open, FileAccess.Read, FileShare.Read);
                    Globals.customPatterns = (TroyCustomPatterns) dser.Deserialize(fs);
                    fs.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error extracting from Troy Custom Patterns. Path: " + filePath + " Error: " + ex.Message, "Error!",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }
        }

        private bool ReadDataCaptureConfiguration()
        {
            try
            {
                var dser = new XmlSerializer(typeof (DataCaptureList));
                var fs = new FileStream(filePath + "TroyDataCaptureConfiguration.xml", FileMode.Open, FileAccess.Read, FileShare.Read);
                Globals.dcaplist = (DataCaptureList) dser.Deserialize(fs);
                fs.Close();

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error extracting from Data Capture Configuration. Path: " + filePath + " Error: " + ex.Message,
                    "Error!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }
        }


        private void ReadPrinterMap()
        {
            FileStream fs = null;
            try
            {
                var dser = new XmlSerializer(typeof (TroyPrinterMap));
                fs = new FileStream(filePath + "TroyPrinterMap.xml", FileMode.Open, FileAccess.Read, FileShare.Read);
                var tpm = (TroyPrinterMap) dser.Deserialize(fs);
                dgvPrinterMap.Rows.Clear();
                foreach (var pmt in tpm.PrinterMap)
                {
                    string[] strRow = {pmt.MapString, pmt.PrinterQueueName};
                    dgvPrinterMap.Rows.Add(strRow);
                }

                fs.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error extracting from Troy Print Map. Path: " + filePath + "\n Error: " + ex.Message,
                    "Error!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                if (fs != null)
                {
                    fs.Close();
                }
            }
        }


        private void SetTroyMarkControls()
        {
            if (chkEnableTroyMark.Checked)
            {
                lblTroyMarkPattern.Enabled = true;
                cboTroyMarkPattern.Enabled = true;
                cboTmColor.Enabled = true;
                gbTroyMarkExclusion.Enabled = true;
                gbTroyMarkInclusion.Enabled = true;
                lblLineSpacing.Enabled = true;
                numLineSpacing.Enabled = true;
                lblMaxCharsPerLine.Enabled = true;
                numMaxCharPerLine.Enabled = true;
                chkTroyMarkOnBack.Enabled = true;
                txtStaticText.Enabled = true;
                lblStaticText.Enabled = true;
                chkStaticSeparate.Enabled = true;
            }
            else
            {
                lblTroyMarkPattern.Enabled = false;
                cboTroyMarkPattern.Enabled = false;
                cboTmColor.Enabled = false;
                gbTroyMarkExclusion.Enabled = false;
                gbTroyMarkInclusion.Enabled = false;
                lblLineSpacing.Enabled = false;
                numLineSpacing.Enabled = false;
                lblMaxCharsPerLine.Enabled = false;
                numMaxCharPerLine.Enabled = false;
                chkTroyMarkOnBack.Enabled = false;
                txtStaticText.Enabled = false;
                lblStaticText.Enabled = false;
                chkStaticSeparate.Enabled = false;
            }
        }

        private void SetErrorLogPathControls()
        {
            if (chkEnableErrorLogging.Checked)
            {
                txtErrorLogPath.Enabled = true;
                btnErrorLogPath.Enabled = true;
                lblErrorPath.Enabled = true;
                chkLogErrorToEventLog.Enabled = true;
                lblSaveDataFiles.Enabled = true;
                txtSaveDataFiles.Enabled = true;
                btnSaveDataFilesPath.Enabled = true;
            }
            else
            {
                txtErrorLogPath.Enabled = false;
                btnErrorLogPath.Enabled = false;
                lblErrorPath.Enabled = false;
                chkLogErrorToEventLog.Enabled = false;
                lblSaveDataFiles.Enabled = false;
                txtSaveDataFiles.Enabled = false;
                btnSaveDataFilesPath.Enabled = false;
            }
        }

        private void chkEnableTroyMark_CheckedChanged(object sender, EventArgs e)
        {
            SetTroyMarkControls();
        }


        private void btnSaveDataFilesPath_Click(object sender, EventArgs e)
        {
            if (folderErrorLogPath.ShowDialog() == DialogResult.OK)
            {
                txtSaveDataFiles.Text = folderErrorLogPath.SelectedPath;
            }
        }

        private bool ValidateValues()
        {
            var saveTab = tabMain.SelectedTab.Name;
            //TROYMark validation
            if (chkEnableTroyMark.Checked)
            {
                var allIncFieldsSet = ((txtTroyMarkIncX.Text != "") && (txtTroyMarkIncY.Text != "") &&
                                       (txtTroyMarkIncH.Text != "") && (txtTroyMarkIncW.Text != ""));
                var allIncFieldsCleared = ((txtTroyMarkIncX.Text == "") && (txtTroyMarkIncY.Text == "") &&
                                           (txtTroyMarkIncH.Text == "") && (txtTroyMarkIncW.Text == ""));
                var allExcFieldsSet = ((txtTroyMarkExcX.Text != "") && (txtTroyMarkExcY.Text != "") &&
                                       (txtTroyMarkExcH.Text != "") && (txtTroyMarkExcW.Text != ""));
                var allExcFieldsCleared = ((txtTroyMarkExcX.Text == "") && (txtTroyMarkExcY.Text == "") &&
                                           (txtTroyMarkExcH.Text == "") && (txtTroyMarkExcW.Text == ""));

                if (cboTmColor.Text == "")
                {
                    cboTmColor.Text = "Black";
                }
                //All or nothing, anything else is an error
                if (!(allIncFieldsSet || allIncFieldsCleared))
                {
                    tabMain.SelectTab("DefaultSettings");
                    MessageBox.Show("Error! All TROYMark Inclusion fields must be set if inclusion region is defined.");
                    return false;
                }
                if ((cboTroyMarkPattern.Text != "") && allIncFieldsCleared)
                {
                    tabMain.SelectTab("DefaultSettings");
                    MessageBox.Show("Error! TROYMark Inclusion fields must be set if TROYMark Pattern is set.");
                    return false;
                }
                if ((cboTroyMarkPattern.Text == "") && allIncFieldsSet)
                {
                    tabMain.SelectTab("DefaultSettings");
                    MessageBox.Show("Error! TROYMark Pattern must be set if TROYMark Inclusion fields are set.");
                    return false;
                }
                if (!(allExcFieldsSet || allExcFieldsCleared))
                {
                    tabMain.SelectTab("DefaultSettings");
                    MessageBox.Show("Error! All TROYMark Exclusion fields must be set if exclusion region is defined.");
                    return false;
                }
                if (allExcFieldsSet && allIncFieldsCleared)
                {
                    tabMain.SelectTab("DefaultSettings");
                    MessageBox.Show("Error! TROYMark Exclusion fields can be set only if an inclusion region is set.");
                    return false;
                }
            }


            if (chkEnableErrorLogging.Checked)
            {
                if ((chkLogErrorToEventLog.Checked == false) && (txtSaveDataFiles.Text == "") &&
                    (txtErrorLogPath.Text == ""))
                {
                    tabMain.SelectTab("ErrorLogging");
                    MessageBox.Show("Error! Error logging enabled but logging fields are not set.");
                    return false;
                }
            }


            tabMain.SelectTab(saveTab);
            return true;
        }

        private bool SerializeConfig()
        {
            try
            {
                //TROYmark tab
                tpmc_local.TroyMarkEnabled = chkEnableTroyMark.Checked;
                if ((cboTroyMarkPattern.Text.ToUpper() == "DARK") || (cboTroyMarkPattern.Text.ToUpper() == "MEDIUM") ||
                    (cboTroyMarkPattern.Text.ToUpper() == "LIGHT") || (cboTroyMarkPattern.Text.ToUpper() == "CUSTOM 1") ||
                    (cboTroyMarkPattern.Text.ToUpper() == "CUSTOM 2") ||
                    (cboTroyMarkPattern.Text.ToUpper() == "CUSTOM 3"))
                {
                    tpmc_local.DefaultTroyMarkPattern = cboTroyMarkPattern.Text;
                }
                else
                {
                    tpmc_local.DefaultTroyMarkPattern = "None";
                }

                tpmc_local.TextColor =
                    (TmTextColor)
                        Enum.Parse(typeof (TmTextColor), cboTmColor.Text);

                if (txtTroyMarkIncX.Text != "")
                {
                    tpmc_local.DefaultTroyMarkInclusion = txtTroyMarkIncX.Text + "," + txtTroyMarkIncY.Text + "," +
                                                          txtTroyMarkIncW.Text + "," + txtTroyMarkIncH.Text;
                }
                else
                {
                    tpmc_local.DefaultTroyMarkInclusion = "";
                }
                if (txtTroyMarkExcX.Text != "")
                {
                    tpmc_local.DefaultTroyMarkExclusion = txtTroyMarkExcX.Text + "," + txtTroyMarkExcY.Text + "," +
                                                          txtTroyMarkExcW.Text + "," + txtTroyMarkExcH.Text;
                }
                else
                {
                    tpmc_local.DefaultTroyMarkExclusion = "";
                }

                tpmc_local.DefaultTroyMarkSpacing = numLineSpacing.Value.ToString();
                tpmc_local.TroyMarkCharsPerLine = Convert.ToInt32(numMaxCharPerLine.Value);
                tpmc_local.TroyMarkOnBack = chkTroyMarkOnBack.Checked;
                tpmc_local.TroyMarkStaticText = txtStaticText.Text;
                tpmc_local.TroymarkDetachStaticText = chkStaticSeparate.Checked;
                if ((chkTroyMarkOnBack.Checked) || (chkStaticSeparate.Checked))
                {
                    tpmc_local.EnableDuplex = 1;
                }
                else
                {
                    tpmc_local.EnableDuplex = -1;
                }

                //Printer tab
                tpmc_local.DefaultPrinter = cboDefaultPrinter.Text;
                tpmc_local.UsePrinterMap = radUsePrinterMap.Checked;

                //Mp screen
                tpmc_local.MicroPrintConfig.Clear();
                if (numWidth1.Value > 0)
                {
                    var mpc = new MpConfiguration();
                    mpc.LeadingText = txtStaticMpText.Text;
                    mpc.Width = Convert.ToInt32(numWidth1.Value);
                    mpc.XAnchor = Convert.ToInt32(numFromLeft1.Value);
                    mpc.YAnchor = Convert.ToInt32(numFromTop1.Value);
                    mpc.Height = Convert.ToInt32(numClearSpaceHeight.Value);
                    tpmc_local.MicroPrintConfig.Add(mpc);
                    if (numWidth2.Value > 0)
                    {
                        mpc = new MpConfiguration();
                        mpc.LeadingText = txtStaticMpText.Text;
                        mpc.Width = Convert.ToInt32(numWidth2.Value);
                        mpc.XAnchor = Convert.ToInt32(numFromLeft2.Value);
                        mpc.YAnchor = Convert.ToInt32(numFromTop2.Value);
                        mpc.Height = Convert.ToInt32(numClearSpaceHeight.Value);
                        tpmc_local.MicroPrintConfig.Add(mpc);
                    }
                    if (numWidth3.Value > 0)
                    {
                        mpc = new MpConfiguration();
                        mpc.LeadingText = txtStaticMpText.Text;
                        mpc.Width = Convert.ToInt32(numWidth3.Value);
                        mpc.XAnchor = Convert.ToInt32(numFromLeft3.Value);
                        mpc.YAnchor = Convert.ToInt32(numFromTop3.Value);
                        mpc.Height = Convert.ToInt32(numClearSpaceHeight.Value);
                        tpmc_local.MicroPrintConfig.Add(mpc);
                    }
                    if (numWidth4.Value > 0)
                    {
                        mpc = new MpConfiguration();
                        mpc.LeadingText = txtStaticMpText.Text;
                        mpc.Width = Convert.ToInt32(numWidth4.Value);
                        mpc.XAnchor = Convert.ToInt32(numFromLeft4.Value);
                        mpc.YAnchor = Convert.ToInt32(numFromTop4.Value);
                        mpc.Height = Convert.ToInt32(numClearSpaceHeight.Value);
                        tpmc_local.MicroPrintConfig.Add(mpc);
                    }
                }

                //Pantograph Tab
                var temp = "";
                if (chkPantograph1.Checked)
                {
                    temp += "1,";
                }
                if (chkPantograph2.Checked)
                {
                    temp += "2,";
                }
                if (chkPantograph3.Checked)
                {
                    temp += "3,";
                }
                if (chkPantograph4.Checked)
                {
                    temp += "4";
                }
                temp = temp.TrimEnd(',');
                tpmc_local.PrintPantographProfileList = temp;

                //Error Logging tab
                if (chkEnableErrorLogging.Checked)
                {
                    if ((txtSaveDataFiles.Text.Length > 0) && (txtSaveDataFiles.Text.ToUpper() != "DEFAULT") &&
                        (!txtSaveDataFiles.Text.EndsWith("\\")))
                    {
                        txtSaveDataFiles.Text += "\\";
                    }
                    tpmc_local.DebugBackupFilesPath = txtSaveDataFiles.Text;
                    if ((txtErrorLogPath.Text.Length > 0) && (txtErrorLogPath.Text.ToUpper() != "DEFAULT") &&
                        (!txtErrorLogPath.Text.EndsWith("\\")))
                    {
                        txtErrorLogPath.Text += "\\";
                    }
                    tpmc_local.ErrorLogPath = txtErrorLogPath.Text;
                    tpmc_local.LogErrorsToEventLog = chkLogErrorToEventLog.Checked;
                }
                else
                {
                    tpmc_local.DebugBackupFilesPath = "";
                    tpmc_local.ErrorLogPath = "";
                    tpmc_local.LogErrorsToEventLog = false;
                }


                var xser = new XmlSerializer(typeof (TroyPortMonitorConfiguration));
                var filename = filePath + "TroyPortMonitorConfiguration.xml";
                TextWriter writer = new StreamWriter(filename);
                xser.Serialize(writer, tpmc_local);
                writer.Close();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in SerializeConfig.  Error: " + ex.Message);
                return false;
            }
        }

        private bool SaveSettingsToXml()
        {
            XmlSerializer xser;
            string filename;
            TextWriter writer;
            try
            {
                //Save port monitor configuration settings.
                if (!SerializeConfig()) return false;

                // save pantogrpah config
                if (Globals.PantographChanged)
                {
                    var cntr = 1;
                    foreach (var cc in swPantograph.PantographConfigurations)
                    {
                        var tp = cboPageOrientation.Text;
                        tp = tp.Insert(0, "po");
                        cc.PageOrientation = (PageOrientationType)Enum.Parse(typeof(PageOrientationType), tp, true);
                        tp = cboPageSize.Text;
                        tp = tp.Insert(0, "pt");
                        cc.PageType = (PageType)Enum.Parse(typeof(PageType), tp, true);

                        /****************************************
                        var tmpwb = cc.WarningBoxString;
                        cc.WarningBoxString = cc.WarningBoxString.Replace(@"/s", "\u0002");
                        cc.WarningBoxString = cc.WarningBoxString.Replace(@"/d", "\u007F");
                        cc.WarningBoxString = cc.WarningBoxString.Replace(@"/n", "\u000A");
                        cc.WarningBoxString = cc.WarningBoxString.Replace(@"/r", "\u000D");
                        cc.WarningBoxString = cc.WarningBoxString.Replace(@"/t", "\u0009");

                        var fileName = Globals.currentDataPath + "PantographProfile" + cntr + "Page1-OLD.pcl";
                        var pBuilder = new Troy.PantographPclBuilder.BuildPantograph();
                        if (!(pBuilder.CreatePantographPcl(fileName, CloneOf(cc), Globals.currentDataPath, true)))
                        {
                            MessageBox.Show("Error.  Failed to create the Pantograph PCL file.  File name: " + fileName, "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        }
                        cc.WarningBoxString = tmpwb; //Put this back to the /s,/d,etc. so that it can be stored in XML
                        ************************************************/
                        cntr++;
                    }

                    xser = new XmlSerializer(typeof(PantographConfiguration));
                    filename = filePath + "TroyPantographConfiguration.xml";
                    writer = new StreamWriter(filename);
                    xser.Serialize(writer, swPantograph);
                    writer.Close();
                   
                    var bp = new BuildPantographWrap.Wrapper();
                    int rc = bp.ManagedCreatePantograph(Globals.currentBasePath, Globals.currentConfigPath, Globals.currentDataPath);

                    Globals.PantographChanged = false;
                }

                //Save data capture
                xser = new XmlSerializer(typeof (DataCaptureList));
                filename = filePath + "TroyDataCaptureConfiguration.xml";
                writer = new StreamWriter(filename);
                xser.Serialize(writer, Globals.dcaplist);
                writer.Close();

                //Custom Patterns
                xser = new XmlSerializer(typeof (TroyCustomPatterns));
                filename = baseFilePath + "TroyCustomPatterns.xml";
                writer = new StreamWriter(filename);
                xser.Serialize(writer, Globals.customPatterns);
                writer.Close();

                //Save FontGlyphMap data
                xser = new XmlSerializer(typeof (TroyFontGlyphMapList));
                filename = baseFilePath + "TroyFontGlyphConfiguration.xml";
                writer = new StreamWriter(filename);
                xser.Serialize(writer, Globals.FontGlyphMapList);
                writer.Close();

                //Save printer map
                var tpm = new TroyPrinterMap();
                if (dgvPrinterMap.Rows.Count > 0)
                {
                    PrinterMapType pmt;
                    for (var cntr = 0; cntr < dgvPrinterMap.Rows.Count; cntr++)
                    {
                        pmt = new PrinterMapType();
                        pmt.MapString = dgvPrinterMap.Rows[cntr].Cells[0].Value.ToString();
                        pmt.PrinterQueueName = dgvPrinterMap.Rows[cntr].Cells[1].Value.ToString();
                        tpm.PrinterMap.Add(pmt);
                    }
                }
                else
                {
                    tpm.PrinterMap.Clear();
                }
                xser = new XmlSerializer(typeof (TroyPrinterMap));
                filename = filePath + "TroyPrinterMap.xml";
                writer = new StreamWriter(filename);
                xser.Serialize(writer, tpm);
                writer.Close();

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving configuration. " + ex.Message);
                return false;
            }
        }
/*******************************
        private Troy.PantographPclBuilder.CustomConfiguration CloneOf(CustomConfiguration ccin)
        {
            var xserin = new XmlSerializer(typeof(CustomConfiguration));
            var xserout = new XmlSerializer(typeof(Troy.PantographPclBuilder.CustomConfiguration));
            Stream stream = new MemoryStream();
            using (stream)
            {
                xserin.Serialize(stream, ccin);
                stream.Seek(0, SeekOrigin.Begin);
                return (Troy.PantographPclBuilder.CustomConfiguration)xserout.Deserialize(stream);
            }
        }
 * *********************/
        private void txtCurrentPassword_TextChanged(object sender, EventArgs e)
        {
        }

        private void btnApply_Click_1(object sender, EventArgs e)
        {
            SaveButtonSelected = true;
            SaveSettings();
        }

        private void btnMainCancel_Click_1(object sender, EventArgs e)
        {
            Close();
        }

        private void cboTroySecurePortMonitor_SelectedIndexChanged(object sender, EventArgs e)
        {
            Globals.Initialize();

            if (Globals.currentPortName == cboTroySecurePortMonitor.Text)
            {
                return;
            }
            if ((possibleChange) && (Globals.currentPortName != ""))
            {
                if (
                    MessageBox.Show(
                        "Warning.  Changes made to the current Secure Port will be lost if another port is selected.  Select Save before switching Ports.  Continute to switch Ports?",
                        "Warning", MessageBoxButtons.YesNo) == DialogResult.No)
                {
                    cboTroySecurePortMonitor.Text = Globals.currentPortName;
                    return;
                }
            }
            possibleChange = false;
            Globals.currentPortName = cboTroySecurePortMonitor.Text;

            tabMain.Enabled = true;
            btnMainOK.Enabled = true;
            btnApply.Enabled = true;
            //btnPortType.Enabled = true;

            filePath = portToConfigPath[cboTroySecurePortMonitor.Text];
            Globals.currentConfigPath = filePath;
            Globals.currentDataPath = portToDataPath[cboTroySecurePortMonitor.Text];
            Globals.currentBasePath = portToBasePath[cboTroySecurePortMonitor.Text];

            if ((filePath == null) || (filePath == ""))
            {
                MessageBox.Show("Can not find file path associated with the select secure port.");
            }
            else
            {
                if (!ReadConfigurationFromXml())
                {
                    Close();
                }
                else
                {
                    cboDefaultPrinter.Items.Clear();

                    var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Printer");
                    foreach (ManagementObject printer in searcher.Get())
                    {
                        var printerName = "";
                        var portName = "";
                        portName = printer["PortName"].ToString();
                        printerName = printer["Name"].ToString();
                        if (!portName.Contains("TROYPORT"))
                        {
                            cboDefaultPrinter.Items.Add(printerName);
                        }
                    }

                    SetErrorLogPathControls();
                    SetTroyMarkControls();
                }
            }
        }

        private void chkEnableErrorLogging_CheckedChanged(object sender, EventArgs e)
        {
            SetErrorLogPathControls();
        }

        private void btnErrorLogPath_Click(object sender, EventArgs e)
        {
            if (folderErrorLogPath.ShowDialog() == DialogResult.OK)
            {
                txtErrorLogPath.Text = folderErrorLogPath.SelectedPath;
            }
        }

        private void btnSaveDataFilesPath_Click_1(object sender, EventArgs e)
        {
            if (folderErrorLogPath.ShowDialog() == DialogResult.OK)
            {
                txtSaveDataFiles.Text = folderErrorLogPath.SelectedPath;
            }
        }

        private bool SaveSettings()
        {
            var retCancel = false;
            DialogResult retVal;
            if (!SaveButtonSelected)
            {
                retVal = MessageBox.Show("Save configuration settings before exiting?", "Are you sure?",
                    MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            }
            else
            {
                retVal = MessageBox.Show("Save configuration settings?", "Are you sure?", MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Question);
            }
            if (retVal == DialogResult.Yes)
            {
                if (ValidateValues())
                {
                    if (SaveSettingsToXml())
                    {
                        MessageBox.Show("Configuration settings saved.", "Save Complete", MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Error occurred while attempting to save changes. Original settings restored.",
                            "Save Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else
                {
                    MessageBox.Show("Values were not saved due to errors.", "Values Not Saved", MessageBoxButtons.OK);
                    retCancel = true;
                }
            }
            else if (retVal == DialogResult.Cancel)
            {
                retCancel = true;
            }
            else
            {
                MessageBox.Show("Settings were not saved.", "Save Cancelled", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                retCancel = true;
            }

            if (!retCancel)
            {
                possibleChange = false;
                Cursor = Cursors.WaitCursor;
                Thread.Sleep(500);
                var myService = new ServiceController("Troy Port Monitor Service");
                if (myService.Status == ServiceControllerStatus.Running)
                {
                    myService.Stop();
                }
                Thread.Sleep(2000);
                myService.Start();
                Cursor = Cursors.Arrow;
            }

            return retCancel;
        }

        private void btnMainOK_Click(object sender, EventArgs e)
        {
            SaveButtonSelected = false;
            if (!SaveSettings())
            {
                Close();
            }
        }


        private void cboTroySecurePortMonitor_Leave(object sender, EventArgs e)
        {
            if (cboTroySecurePortMonitor.Text != "")
            {
                Globals.currentPortName = cboTroySecurePortMonitor.Text;
                possibleChange = true;
            }
        }

        private void btnAbout_Click(object sender, EventArgs e)
        {
            var splash = new SplashForm();
            splash.LicenseString = CurrentLicenseLabelText;
            splash.VersionNumber = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            splash.VersionNumber = Application.ProductVersion;
            // if ((ls == LicensingStatus.FullyLicensed) || (ls == LicensingStatus.TrialMode))
            if ((lqr.CurrentLicenseStatus == LicenseStatus.Active) || (lqr.CurrentLicenseStatus == LicenseStatus.Trial))
            {
                splash.ShowEnableButton = true;
            }
            splash.ShowDialog();
        }


        private void btnDensity_Click(object sender, EventArgs e)
        {
            Globals.PantographChanged = true;
            var ds = new DensitySettings();
            ds.PrinterName = cboDefaultPrinter.Text;
            ds.ccDensity = swPantograph.PantographConfigurations[0];
            ds.BaseFilePath = baseFilePath;
            ds.ShowDialog();

            for (var cntr = 1; cntr < swPantograph.PantographConfigurations.Count; cntr++)
            {
                swPantograph.PantographConfigurations[cntr].DensityPattern1 =
                    swPantograph.PantographConfigurations[0].DensityPattern1;
                swPantograph.PantographConfigurations[cntr].DensityPattern2 =
                    swPantograph.PantographConfigurations[0].DensityPattern2;
                swPantograph.PantographConfigurations[cntr].DensityPattern3 =
                    swPantograph.PantographConfigurations[0].DensityPattern3;
                swPantograph.PantographConfigurations[cntr].DensityPattern4 =
                    swPantograph.PantographConfigurations[0].DensityPattern4;
                swPantograph.PantographConfigurations[cntr].DensityPattern5 =
                    swPantograph.PantographConfigurations[0].DensityPattern5;
                swPantograph.PantographConfigurations[cntr].DensityPattern6 =
                    swPantograph.PantographConfigurations[0].DensityPattern6;
                swPantograph.PantographConfigurations[cntr].DensityPattern7 =
                    swPantograph.PantographConfigurations[0].DensityPattern7;
                swPantograph.PantographConfigurations[cntr].DensityPattern8 =
                    swPantograph.PantographConfigurations[0].DensityPattern8;
                swPantograph.PantographConfigurations[cntr].DensityPattern9 =
                    swPantograph.PantographConfigurations[0].DensityPattern9;
            }
        }

        private void btnPantoConfig_Click(object sender, EventArgs e)
        {
            Globals.PantographChanged = true;
            var pcs = new PantographConfigurations();
            pcs.PConfigs = swPantograph;
            pcs.masterPO = (PageOrientationType)Enum.Parse(typeof(PageOrientationType), "po" + cboPageOrientation.Text, true);
            pcs.masterPT = (PageType) Enum.Parse(typeof (PageType), "pt" + cboPageSize.Text, true);
            pcs.ShowDialog();
        }

        private void btnDataCapture_Click(object sender, EventArgs e)
        {
            var dclf = new DataCaptureListForm();
            dclf.ShowDialog();
        }

        private void btnAddPrintMap_Click(object sender, EventArgs e)
        {
            var pm = new PrinterMap();
            pm.PrinterName = "";
            pm.MapString = "";
            pm.ShowDialog();
            if (pm.ReturnOk)
            {
                if (pm.PrinterName != "")
                {
                    AddToPrinterGrid(pm.MapString, pm.PrinterName);
                }
            }
        }

        private void AddToPrinterGrid(string Map, string Printer)
        {
            string[] rowStr = {Map, Printer};
            dgvPrinterMap.Rows.Add(rowStr);
        }

        private void btnDelPrinterMap_Click(object sender, EventArgs e)
        {
            if (dgvPrinterMap.SelectedRows.Count > 0)
            {
                dgvPrinterMap.Rows.Remove(dgvPrinterMap.SelectedRows[0]);
            }
        }

        private void btnEditPrinterMap_Click(object sender, EventArgs e)
        {
            if (dgvPrinterMap.SelectedRows.Count > 0)
            {
                string Map, Printer;
                Map = dgvPrinterMap.SelectedRows[0].Cells[0].Value.ToString();
                Printer = dgvPrinterMap.SelectedRows[0].Cells[1].Value.ToString();
                var pm = new PrinterMap();
                pm.PrinterName = Printer;
                pm.MapString = Map;
                pm.ShowDialog();
                if (pm.ReturnOk)
                {
                    if (pm.PrinterName != "")
                    {
                        dgvPrinterMap.SelectedRows[0].Cells[0].Value = pm.MapString;
                        dgvPrinterMap.SelectedRows[0].Cells[1].Value = pm.PrinterName;
                    }
                }
            }
        }

        private void gbDynamicPrint_Enter(object sender, EventArgs e)
        {
        }

        private void radUseAsPrinterName_CheckedChanged(object sender, EventArgs e)
        {
            if (radUseAsPrinterName.Checked)
            {
                gbPrinterMap.Enabled = false;
            }
        }

        private void radUsePrinterMap_CheckedChanged(object sender, EventArgs e)
        {
            if (radUsePrinterMap.Checked)
            {
                gbPrinterMap.Enabled = true;
            }
        }

        private void cboPageSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            Globals.PantographChanged = true;
        }

        private void cboPageOrientation_SelectedIndexChanged(object sender, EventArgs e)
        {
            Globals.PantographChanged = true;
        }

        private void btnPassThrough_Click(object sender, EventArgs e)
        {
            var pt = new PassThrough();
            pt.PassThroughEnabled = tpmc_local.PassThroughEnabled;
            pt.UseAll = tpmc_local.PassThroughStringMatchAllFlag;
            pt.PassThroughList = tpmc_local.PassThroughList;
            pt.ShowDialog();
            if (!pt.Cancelled)
            {
                tpmc_local.PassThroughEnabled = pt.PassThroughEnabled;
                tpmc_local.PassThroughStringMatchAllFlag = pt.UseAll;
                tpmc_local.PassThroughList = pt.PassThroughList;
            }
        }

        private void btnTextMap_Click(object sender, EventArgs e)
        {
            var pjttd = new PjlJobToTmData();
            pjttd.PjlToTmList = tpmc_local.PjlDocNameToTmStringList;
            pjttd.ShowDialog();
        }

        private void chkTroyMarkOnBack_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void groupBox3_Enter(object sender, EventArgs e)
        {
        }

        private void btnPortType_Click(object sender, EventArgs e)
        {
            var pt = new PortType();
            pt.PortName = cboTroySecurePortMonitor.Text;
            pt.Ports = PortTypes;
            pt.filePath = baseFilePath;
            pt.ShowDialog();
        }

        private void btnPanto2_Click(object sender, EventArgs e)
        {
            var P2W = new P2WizardForm
            {
                PrinterName = cboDefaultPrinter.Text,
                IntfPattern = 20,
                IntfFontPath = Globals.currentDataPath
            };
            P2W.ShowDialog();


            if (!string.IsNullOrEmpty(P2W.CustomName))
            {
                if (P2W.background.pattern1.data != null)
                {
                    for (var cntr = 0; cntr < swPantograph.PantographConfigurations.Count; cntr++)
                    {
                        swPantograph.PantographConfigurations[cntr].CustomBackgroundPatternData =
                            P2W.background.pattern1.data;
                        //Temporary
                        if (P2W.foreground.pattern1.data == null)
                        {
                            //Set the foreground equal to the background
                            swPantograph.PantographConfigurations[cntr].CustomForegroundPatternData =
                                P2W.background.pattern1.data;
                        }
                        else
                        {
                            swPantograph.PantographConfigurations[cntr].CustomForegroundPatternData =
                                P2W.foreground.pattern1.data;
                        }
                    }
                }

                var SavePattern = true;
                foreach (var tp in Globals.customPatterns.TroyStoredPattern)
                {
                    if (tp.StoredPatternName == P2W.CustomName)
                    {
                        if (MessageBox.Show("The name " + P2W.CustomName +
                                            " is already used. Do you want to overwrite?  \n  Note: Overwriting will affect all ports using this name.",
                            "Overwrite?",
                            MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                        {
                            SavePattern = false;
                            MessageBox.Show("Custom pattern was not saved!");
                            break;
                        }
                    }
                }

                if (SavePattern)
                {
                    var tptns = new TroyPatterns();
                    tptns.StoredPatternName = P2W.CustomName;
                    tptns.BackgroundPattern = P2W.background.pattern1.data;
                    tptns.ForegroundPattern = P2W.foreground.pattern1.data;
                    Globals.customPatterns.TroyStoredPattern.Add(tptns);
                    MessageBox.Show("Custom pattern was saved to the list of available patterns.");
                }
            }
        }
    }
}