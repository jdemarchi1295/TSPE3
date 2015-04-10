using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.IO;
using Troy.PortMonitor.Core.XmlConfiguration;

namespace TroySecurePortMonitorUserInterface
{
    public partial class PortType : Form
    {
        public List<string> Ports = new List<string>();
        public string CurrentPortType = "";
        public string PortName = "";
        public string filePath = "";

        public PortType()
        {
            InitializeComponent();
        }

        private void PortType_Load(object sender, EventArgs e)
        {
            cboPortType.Text = CurrentPortType;
            txtName.Text = PortName;

            string PortConfiguration = filePath + "Configuration\\";
            DirectoryInfo di = new DirectoryInfo(PortConfiguration);
            //int subdircnt = di.GetDirectories().Length;

            foreach (DirectoryInfo subdi in di.GetDirectories())
            {
                cboPortType.Items.Add(subdi.Name);
            }

        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if ((cboPortType.Text != CurrentPortType) ||
                (txtName.Text != PortName))
            {

                //Deserialize the service configuration
                string serviceXmlFile = filePath + "TroyPMServiceConfiguration.xml";
                XmlSerializer dser1 = new XmlSerializer(typeof(TroyPortMonitorServiceConfiguration));
                FileStream fs = new FileStream(serviceXmlFile, FileMode.Open, FileAccess.Read, FileShare.Read);
                TroyPortMonitorServiceConfiguration tpmsc = (TroyPortMonitorServiceConfiguration)dser1.Deserialize(fs);
                fs.Close();

                foreach (TroySecurePort tsp in tpmsc.PortList)
                {
                    if (tsp.PortName == PortName)
                    {
                        string portMonName = tsp.PortMonitorName;
                        string portPath = GetPortPath(portMonName);
                        if (portPath != "")
                        {
                            string portConfigPath = portPath + "Config\\";
                            if (cboPortType.Text != CurrentPortType)
                            {
                                string PortConfiguration = filePath + "Configuration\\" + cboPortType.Text;
                                if (Directory.Exists(PortConfiguration))
                                {
                                    DirectoryInfo filesCopy = new DirectoryInfo(PortConfiguration);
                                    foreach (FileInfo fInfo in filesCopy.GetFiles())
                                    {
                                        fInfo.CopyTo(portConfigPath + fInfo.Name, true);
                                    }
                                }
                            }
                        }
                        if (PortName != txtName.Text)
                        {
                            tsp.PortName = PortName;
                            XmlSerializer xser = new XmlSerializer(typeof(TroyPortMonitorServiceConfiguration));
                            TextWriter writer = new StreamWriter(serviceXmlFile);
                            xser.Serialize(writer, tsp);
                            writer.Close();

                        }


                        break;
                    }
                }

            }
            this.Close();
        }

        //Returns the path where the file will be written for the Default setting
        private string GetPortPath(string portMonName)
        {
            try
            {
                Microsoft.Win32.RegistryKey registryKey = Microsoft.Win32.Registry.LocalMachine;
                Microsoft.Win32.RegistryKey pmKey = registryKey.OpenSubKey
                    ("System\\CurrentControlSet\\Control\\Print\\Monitors\\TroySecurePortMonitor", false);
                Microsoft.Win32.RegistryKey pmPort = pmKey.OpenSubKey("Ports", false);
                string portPath = pmPort.GetValue(portMonName).ToString();

                return portPath;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in GetPortPath(). Can not get path for port: " + portMonName + " Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return "";
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}
