using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Troy.PortMonitor.Core.XmlConfiguration;

namespace AddSecurePortUtility
{
    public partial class frmMainForm : Form
    {
        static Dictionary<string, string> troyPorts = new Dictionary<string, string>();
        static Dictionary<string, string> configPaths = new Dictionary<string, string>();
        static Dictionary<string, string> portPath = new Dictionary<string, string>();
        private string filePath;
        private const string AddNewPortString =  "<Add new TROYPORT>";
        private int currentPortNumber = 1;
        private string baseFilePath = "";

        private const int MAX_NUMBER_OF_PORTS = 5000;

        private bool UsingSoftwarePantograph = false;
        private bool UsingSoftwareTroymark = false;

        private TroyPortMonitorServiceConfiguration tpmsc_local;

        private string PrintSpoolerName = "Print Spooler"; // default value

        public frmMainForm()
        {
            InitializeComponent();
        }

        private void InitForm()
        {
            troyPorts.Clear();
            configPaths.Clear();
            portPath.Clear();
            cboTroyPort.Items.Clear();
            cboTroyPort.Text = "";
            txtPrintPath.Text = "";
            txtConfigPath.Text = "Default";

            if (!ReadMainPortConfig())
            {
                this.Close();
            }
            cboTroyPort.Items.Add(AddNewPortString);

            if (!ReadTroyPortNames())
            {
                this.Close();
            }

        }

        private bool ReadTroyPortNames()
        {
            try
            {
                string portPathValue;
                Microsoft.Win32.RegistryKey registryKey = Microsoft.Win32.Registry.LocalMachine;
                Microsoft.Win32.RegistryKey pmKey = registryKey.OpenSubKey
                ("System\\CurrentControlSet\\Control\\Print\\Monitors\\TroySecurePortMonitor\\Ports", false);

                foreach (string portName in pmKey.GetValueNames())
                {
                    if (!troyPorts.ContainsValue(portName))
                    {
                        cboTroyPort.Items.Add(portName);
                        portPathValue = pmKey.GetValue(portName).ToString();
                        portPath.Add(portName, portPathValue);
                    }
                }

                return true;

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error reading TROYPORT names from registry.  Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

        }

        //Gets the base configuration path from the registry
        private bool GetBasePath()
        {
            try
            {
                Microsoft.Win32.RegistryKey registryKey = Microsoft.Win32.Registry.LocalMachine;
                Microsoft.Win32.RegistryKey pmKey = registryKey.OpenSubKey
                    ("System\\CurrentControlSet\\Control\\Print\\Monitors\\TroySecurePortMonitor", false);

                filePath = pmKey.GetValue("MainConfigurationPath").ToString();
                if ((filePath.Length > 0) && (!filePath.EndsWith("\\")))
                {
                    filePath += "\\";
                }
                baseFilePath = filePath;
                pmKey.Close();
                registryKey.Close();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error getting the base configuration path from registry. Can not continue.  Error: " + ex.Message, "Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }


        private void frmMainForm_Load(object sender, EventArgs e)
        {

            string copyrightString = "\u00A9 Copyright  TROY Group Inc.  2012  Version 1.0.16";
            lblCopyrightInfo.Text = copyrightString;
            
            InitForm();
        }
        private bool ReadMainPortConfig()
        {
            try
            {
                if (GetBasePath())
                {

                    //Check if the service configuration xml file exists.
                    string serviceXmlFile = baseFilePath + "TroyPMServiceConfiguration.xml";
                    if (!File.Exists(serviceXmlFile))
                    {
                        MessageBox.Show("Fatal Error.  File " + serviceXmlFile + " does not exist.  Can not continue.", "Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        this.Close();
                    }

                    //Deserialize the service configuration
                    XmlSerializer dser1 = new XmlSerializer(typeof(TroyPortMonitorServiceConfiguration));
                    FileStream fs = new FileStream(serviceXmlFile, FileMode.Open, FileAccess.Read, FileShare.Read);
                    tpmsc_local = (TroyPortMonitorServiceConfiguration)dser1.Deserialize(fs);
                    fs.Close();

                    PrintSpoolerName = tpmsc_local.PrintSpoolerName;

                    UsingSoftwarePantograph = tpmsc_local.UserInterface.UseAsSecurePrint;
                    UsingSoftwareTroymark = tpmsc_local.UserInterface.UseAsSecurePrint;

                    cboPortType.Items.Clear();
                    lblPortType.Enabled = false;
                    cboPortType.Enabled = false;
                    chkIncludeTypeInName.Enabled = false;
                    string configurationFilePath = baseFilePath + "Configuration";
                    DirectoryInfo di = new DirectoryInfo(configurationFilePath);
                    int subdircnt = di.GetDirectories().Length;
      
                    if (subdircnt < 1)
                    {
                        //keep port type controls disabled
                        cboPortType.Text = @"";
                    }
                    else if (subdircnt > 0)
                    {
                        foreach (DirectoryInfo subdi in di.GetDirectories())
                        {
                            cboPortType.Items.Add(subdi.Name);
                        }
                        if (subdircnt == 1)
                        {
                            cboPortType.Text = cboPortType.Items[0].ToString();
                        }
                        else
                        {
                            lblPortType.Enabled = true;
                            cboPortType.Enabled = true;
                            chkIncludeTypeInName.Enabled = true;
                        }
                    }

                    //Loop through each port
                    string portString, portConfigPath, portMonName, portPath, dataPath = "";
                    foreach (TroySecurePort tsp in tpmsc_local.PortList)
                    {    
                        portString = tsp.PortName;

                        portMonName = tsp.PortMonitorName;
                        portConfigPath = tsp.ConfigurationPath;

                        troyPorts.Add(portString, portMonName);
                       configPaths.Add(portString, portConfigPath);
                    }

                    string defaultPortName;
                    for (int cntr = 1; cntr < MAX_NUMBER_OF_PORTS; cntr++)
                    {
                        defaultPortName = "Troy Secure Port " + cntr.ToString();
                        bool nextonefound = false;
                        foreach (KeyValuePair<string,string> kvp in troyPorts)
                        {
                           
                            if (!kvp.Key.StartsWith(defaultPortName))
                            {
                                nextonefound = true;
                            }
                            else
                            {
                                nextonefound = false;
                                break;
                            }
                        }
                        if (nextonefound)
                        {
                            txtNewPortName.Text = defaultPortName;
                            if (chkIncludeTypeInName.Checked)
                            {
                                txtNewPortName.Text = txtNewPortName.Text + " (" + cboPortType.Text + ")";
                            }
                            currentPortNumber = cntr;
                            break;

                        }
                    }
                    return true;
                }
                else
                {
                    MessageBox.Show("Error reading base path from Registry.", "Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error Opening Port Monitor Configuration. Application will close. Path: " + filePath, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                MessageBox.Show(ex.Message, "Exception Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private void cboTroyPort_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnMainOK.Enabled = true;
            btnApply.Enabled = true;
            if (cboTroyPort.Text == AddNewPortString)
            {
                AddNewPortMonitor newDialog = new AddNewPortMonitor();
                Microsoft.Win32.RegistryKey registryKey = Microsoft.Win32.Registry.LocalMachine;
                Microsoft.Win32.RegistryKey pmKey = registryKey.OpenSubKey
                ("System\\CurrentControlSet\\Control\\Print\\Monitors\\TroySecurePortMonitor\\Ports", false);

                string TroyPortName;
                int cntr;
                for (cntr = 1; cntr < MAX_NUMBER_OF_PORTS; cntr++)
                {
                    TroyPortName = "TROYPORT" + cntr.ToString() + ":";
                    if (pmKey.GetValue(TroyPortName) == null)
                    {
                        break;
                    }
                }
                newDialog.UsingSoftwarePantograph = UsingSoftwarePantograph;
                newDialog.nextValue = cntr;
                newDialog.PrintSpoolerName = PrintSpoolerName;

                if (cboPortType.Enabled)
                {
                    if (cboPortType.Text != "")
                    {
                        newDialog.MainConfigurationPath = baseFilePath + @"Configuration\" + cboPortType.Text + @"\";
                        newDialog.MainDataPath = baseFilePath + @"Data\" + cboPortType.Text + @"\";
                    }
                    else
                    {
                        MessageBox.Show("Please select a Port Type before creating a new TROYPORT.", "Port Type", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        cboTroyPort.Text = "";
                        InitForm();
                        return;
                    }
                }
                else
                {
                    newDialog.MainConfigurationPath = baseFilePath + @"Configuration\";
                    newDialog.MainDataPath = baseFilePath + @"Data\";
                }


                newDialog.ShowDialog();

                if (newDialog.newPortName != "")
                {
                    InitForm();
                    cboTroyPort.Text = newDialog.newPortName;
                }
                else
                {
                    cboTroyPort.Text = "";
                }

            }
            else
            {
                txtPrintPath.Text = portPath[cboTroyPort.Text];
            }

        }

        private void btnMainCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnMainOK_Click(object sender, EventArgs e)
        {
            if (cboTroyPort.Text == "")
            {
                this.Close();
            }
            else
            {
                if (SaveSettings())
                {
                    this.Close();
                }

            }
        }

        private bool ValidateSettings()
        {
            if (txtNewPortName.Text.Length < 1)
            {
                MessageBox.Show("Invalid port name.  Can not Save changes.");
                return false;
            }
           
            if (!cboTroyPort.Text.StartsWith("TROYPORT"))
            {
                MessageBox.Show("Invalid TROYPORT value.  Can not Save changes.");
                return false;
            }

            if (txtConfigPath.Text.Length < 1)
            {
                MessageBox.Show("Invalid Configuration path value.");
                return false;
            }


            return true;


        }

        private bool SaveSettings()
        {
            string configPath;
            DialogResult retVal;
            retVal = MessageBox.Show("Save secure port settings?", "Are you sure?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            if (retVal == DialogResult.Yes)
            {
                if (!ValidateSettings())
                {
                    return false;
                }
                if (txtConfigPath.Text.ToUpper() == "DEFAULT")
                {
                 //   string progFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
                 //   string pathFromReg = portPath[cboTroyPort.Text.ToString()];
                 //   configPath = portPath + @"\TROY Group\Port Monitor\" + pathFromReg + @"\Config\";
                    txtConfigPath.Text = "default";
                }
                else
                {
                    if (!txtConfigPath.Text.EndsWith("\\"))
                    {
                        txtConfigPath.Text += "\\";
                    }
                    configPath = txtConfigPath.Text;
                }

                TroySecurePort tsp = new TroySecurePort();
                tsp.ConfigurationPath = txtConfigPath.Text.ToString();
                tsp.PortMonitorName = cboTroyPort.Text.ToString();
                tsp.PortName = txtNewPortName.Text.ToString();
                tpmsc_local.PortList.Add(tsp);

                //Save data capture
                XmlSerializer xser = new XmlSerializer(typeof(TroyPortMonitorServiceConfiguration));
                string filename = baseFilePath + "TroyPMServiceConfiguration.xml";
                TextWriter writer = new StreamWriter(filename);
                xser.Serialize(writer, tpmsc_local);
                writer.Close();

                MessageBox.Show("New secure port was added.");

                InitForm();
                btnApply.Enabled = false;

                return true;
            }
            else if (retVal == DialogResult.Cancel)
            {
                cboTroyPort.Text = "";
                txtPrintPath.Text = "";
                return false;
            }
            else
            {
                MessageBox.Show("New secure port was not added.");
                return true;
            }


        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            SaveSettings();
        }

  

        private void btnConfigPath_Click(object sender, EventArgs e)
        {
            string progFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            folderBrowserDialog1.SelectedPath = progFiles + @"\TROY Group\Port Monitor";
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                txtConfigPath.Text = folderBrowserDialog1.SelectedPath;
            }

        }

        private void btnAddMultiple_Click(object sender, EventArgs e)
        {
            if ((cboPortType.Enabled) && (cboPortType.Text == ""))
            {
                MessageBox.Show("Please select a Port Type.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                if (MessageBox.Show("Are you sure you want to add SecurePorts to the system?", "Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    XDocument xDoc = XDocument.Load(filePath + "TroyPMServiceConfiguration.xml");

                    Microsoft.Win32.RegistryKey registryKey = Microsoft.Win32.Registry.LocalMachine;
                    Microsoft.Win32.RegistryKey pmKey = registryKey.OpenSubKey
                        ("System\\CurrentControlSet\\Control\\Print\\Monitors\\TroySecurePortMonitor\\Ports", true);

                    //string progFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);

                    string newPortName;
                    for (int cntr = currentPortNumber; cntr < currentPortNumber + numericUpDown1.Value; cntr++)
                    {

                        newPortName = "TROYPORT" + cntr + ":";
                        string newFilePath;

                        //newFilePath = progFiles + @"\TROY Group\Port Monitor\PrintPort" + cntr + @"\";
                        newFilePath = baseFilePath + @"PrintPort" + cntr + @"\";
                        DirectoryInfo dirInfo = new DirectoryInfo(newFilePath);
                        if (!dirInfo.Exists)
                        {
                            dirInfo.Create();
                        }

                        DirectoryInfo configDir = new DirectoryInfo(newFilePath + "\\Config\\");
                        if (!configDir.Exists)
                        {
                            configDir.Create();
                            //DirectoryInfo filesCopy = new DirectoryInfo(progFiles + @"\TROY Group\Port Monitor\Configuration\");
                            DirectoryInfo filesCopy;
                            if (cboPortType.Text != "")
                            {
                                 filesCopy = new DirectoryInfo(baseFilePath + @"Configuration\" + cboPortType.Text + @"\");
                            }
                            else
                            {
                                filesCopy = new DirectoryInfo(baseFilePath + @"Configuration\");
                            }

                            foreach (FileInfo fInfo in filesCopy.GetFiles())
                            {
                                fInfo.CopyTo(configDir.FullName + fInfo.Name, true);
                            }
                        }

                        DirectoryInfo dataDir = new DirectoryInfo(newFilePath + "\\Data\\");
                        if (!dataDir.Exists)
                        {
                            dataDir.Create();
                            DirectoryInfo filesCopy;
                            if (cboPortType.Text != "")
                            {
                                filesCopy = new DirectoryInfo(baseFilePath + @"Data\" + cboPortType.Text + @"\");
                            }
                            else
                            {
                                filesCopy = new DirectoryInfo(baseFilePath + @"Data\");
                            }
                            if (UsingSoftwarePantograph)
                            {
                                //DirectoryInfo dataCopy = new DirectoryInfo(progFiles + @"\TROY Group\Port Monitor\Data\");
                                DirectoryInfo dataCopy = new DirectoryInfo(baseFilePath + @"Data\");
                                foreach (FileInfo fInfo in filesCopy.GetFiles())
                                {
                                    fInfo.CopyTo(dataDir.FullName + fInfo.Name, true);
                                }
                            }
                        }


                        pmKey.SetValue(newPortName, newFilePath);

                        TroySecurePort tsp = new TroySecurePort();
                        tsp.ConfigurationPath = "default";
                        tsp.PortMonitorName = "TROYPORT" + cntr + ":";
                        tsp.PortName = "Troy Secure Port " + cntr;
                        if (chkIncludeTypeInName.Checked)
                        {
                            if (cboPortType.Text != "")
                            {
                                tsp.PortName = tsp.PortName + " (" + cboPortType.Text + ")";
                            }
                        }
                        tpmsc_local.PortList.Add(tsp);


                    }
                    pmKey.Close();

                    //Save data capture
                    XmlSerializer xser = new XmlSerializer(typeof(TroyPortMonitorServiceConfiguration));
                    string filename = baseFilePath + "TroyPMServiceConfiguration.xml";
                    TextWriter writer = new StreamWriter(filename);
                    xser.Serialize(writer, tpmsc_local);
                    writer.Close();
                    if (MessageBox.Show("The changes will not take effect until the Print Spooler is restarted.  Would you like to restart the Print Spooler now?", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        if (PrintSpoolerName != "")
                        {
                            System.ServiceProcess.ServiceController myService = new System.ServiceProcess.ServiceController(PrintSpoolerName);
                            if (myService.Status == System.ServiceProcess.ServiceControllerStatus.Running)
                            {
                                myService.Stop();
                            }
                            System.Threading.Thread.Sleep(1000);
                            myService.Start();
                        }
                        MessageBox.Show("New SecurePorts were added to the system.", "SecurePorts Added", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("The Print Spooler was not restarted.  Please restart the services in order to enable the changes made.", "Restart Needed", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                    InitForm();
                }
                else
                {
                    MessageBox.Show("New SecurePorts were not added.", "SecurePorts Not Added", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void chkIncludeTypeInName_CheckedChanged(object sender, EventArgs e)
        {
            if (cboPortType.Items.Count > 0)
            {
                foreach (string str in cboPortType.Items)
                {
                    if (txtNewPortName.Text.Contains(" (" + str + ")"))
                    {
                        txtNewPortName.Text = txtNewPortName.Text.Remove(txtNewPortName.Text.IndexOf(" (" + str + ")"));
                        break;
                    }
                }
            }
            else if (cboPortType.Text != "")
            {
                if (txtNewPortName.Text.Contains(" (" + cboPortType.Text + ")"))
                {
                    txtNewPortName.Text.Remove(txtNewPortName.Text.IndexOf(" (" + cboPortType.Text + ")"));
                }
            }
            if (chkIncludeTypeInName.Checked)
            {
                if (cboPortType.Text != "")
                {
                    txtNewPortName.Text = txtNewPortName.Text + " (" + cboPortType.Text + ")";
                }
            }
            else
            {
                

            }
        }

        private void cboPortType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (chkIncludeTypeInName.Checked)
            {
                if (cboPortType.Items.Count > 0)
                {
                    foreach (string str in cboPortType.Items)
                    {
                        if (txtNewPortName.Text.Contains(" (" + str + ")"))
                        {
                            txtNewPortName.Text = txtNewPortName.Text.Remove(txtNewPortName.Text.IndexOf(" (" + str + ")"));
                            break;
                        }
                    }
                }
                else if (cboPortType.Text != "")
                {
                    if (txtNewPortName.Text.Contains(" (" + cboPortType.Text + ")"))
                    {
                        txtNewPortName.Text.Remove(txtNewPortName.Text.IndexOf(" (" + cboPortType.Text + ")"));
                    }
                }
                if (chkIncludeTypeInName.Checked)
                {
                    if (cboPortType.Text != "")
                    {
                        txtNewPortName.Text = txtNewPortName.Text + " (" + cboPortType.Text + ")";
                    }
                }
 

            }
        }



    }
}
