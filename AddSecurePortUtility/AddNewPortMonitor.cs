using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace AddSecurePortUtility
{
    public partial class AddNewPortMonitor : Form
    {
        public int nextValue;
        public bool UsingSoftwarePantograph;
        public string newPortName;
        public string PrintSpoolerName = "Print Spooler";

        public string MainConfigurationPath = "";
        public string MainDataPath = "";

        public AddNewPortMonitor()
        {
            InitializeComponent();
        }

        private void AddNewPortMonitor_Load(object sender, EventArgs e)
        {
            numericUpDown1.Minimum = nextValue;
            numericUpDown1.Value = nextValue;

        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            Microsoft.Win32.RegistryKey registryKey = Microsoft.Win32.Registry.LocalMachine;
            Microsoft.Win32.RegistryKey pmKey = registryKey.OpenSubKey
                ("System\\CurrentControlSet\\Control\\Print\\Monitors\\TroySecurePortMonitor\\Ports", true);


            Microsoft.Win32.RegistryKey mainKey = registryKey.OpenSubKey
                ("System\\CurrentControlSet\\Control\\Print\\Monitors\\TroySecurePortMonitor", true);

            string progFiles = mainKey.GetValue("MainConfigurationPath").ToString();

            //string progFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);

            newPortName = "TROYPORT" + numericUpDown1.Value.ToString() + ":";
            string filePath;
            if (txtPath.Text.ToUpper() == "DEFAULT")
            {
                filePath = progFiles + @"PrintPort" + numericUpDown1.Value.ToString() + @"\";
                DirectoryInfo dirInfo = new DirectoryInfo(filePath);
                if (!dirInfo.Exists)
                {
                    dirInfo.Create();
                }
                
                DirectoryInfo configDir = new DirectoryInfo(filePath + "\\Config\\");
                if (!configDir.Exists)
                {
                    configDir.Create();
                }
                if (configDir.GetFiles().Length < 1)
                {
                    DirectoryInfo filesCopy = new DirectoryInfo(MainConfigurationPath);
                    foreach (FileInfo fInfo in filesCopy.GetFiles())
                    {
                        fInfo.CopyTo(configDir.FullName + fInfo.Name, true);
                    }
                }

                DirectoryInfo dataDir = new DirectoryInfo(filePath + "\\Data\\");
                if (!dataDir.Exists)
                {
                    dataDir.Create();
                }
                if (dataDir.GetFiles().Length < 1)
                {
                    if (UsingSoftwarePantograph)
                    {
                        DirectoryInfo dataCopy = new DirectoryInfo(MainDataPath);
                        foreach (FileInfo fInfo in dataCopy.GetFiles("*.pcl"))
                        {
                            fInfo.CopyTo(dataDir.FullName + fInfo.Name, true);
                        }
                        foreach (FileInfo fInfo in dataCopy.GetFiles("*.sfp"))
                        {
                            fInfo.CopyTo(dataDir.FullName + fInfo.Name, true);
                        }
                    }
                }
            }
            else if (txtPath.Text.Length < 1)
            {
                MessageBox.Show("Path name must be set to valid path.");
                return;
            }
            else
            {
                if (!(txtPath.Text.EndsWith("\\")))
                {
                    filePath = txtPath.Text + "\\";
                }
                else
                {
                    filePath = txtPath.Text;
                }
            }

            if (numericUpDown1.Value < 1)
            {
                MessageBox.Show("Invalid Port Number.");
                return;
            }

            pmKey.SetValue(newPortName, filePath);

            pmKey.Close();

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
            }
            else
            {
                MessageBox.Show("The Print Spooler was not restarted.  Please restart the services in order to enable the changes made.", "Restart Needed", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string progFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            folderBrowserDialog1.SelectedPath = progFiles + @"\TROY Group\Port Monitor";
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                txtPath.Text = folderBrowserDialog1.SelectedPath;   
            }

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            newPortName = "";
            this.Close();
        }
    }
}
