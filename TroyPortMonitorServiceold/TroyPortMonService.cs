using System;
using System.Collections.Generic;
using System.Diagnostics;
//Added for Port Monitor
using System.IO;  //File Watcher
using System.ServiceProcess;
using System.Threading;  //Threading
using System.Windows.Forms; //MessageBox
using System.Xml.Serialization;
using Troy.PortMonitor.Core.XmlConfiguration;
//using Troy.PortMonitor.Core.License;
using Troy.Licensing;
using TroySecurePortMonitorUserInterface;


namespace TroyPortMonitorService
{
    public partial class TroyPortMonService : ServiceBase
    {
        static PortMonLogging portMonLogging; // = new PortMonLogging();

        //The path to the main PM config file
        static string serviceConfigFilePath;

        const string CMDLINEARG_SERVICE_TRACE_ON = "SERVICETRACEON";
        const string CMDLINEARG_ENABLE_MESSAGE_BOX = "MESSAGEBOXON";

        const string REGISTRY_PATH_LOCATION = "System\\CurrentControlSet\\Control\\Print\\Monitors\\TroySecurePortMonitor";
        const string REGISTRY_PORTS_LOCATION = REGISTRY_PATH_LOCATION + "\\Ports";

        static Dictionary<string, string> portsForFileWatcher = new Dictionary<string, string>();
        static Dictionary<string, TroyPortMonitorConfiguration> portConfigs = new Dictionary<string, TroyPortMonitorConfiguration>();
        static Dictionary<string, TroyFontConfiguration> portFonts = new Dictionary<string, TroyFontConfiguration>();
        static Dictionary<string, PortMonLogging> portLogs = new Dictionary<string, PortMonLogging>();
        static Dictionary<string, DataCaptureList> portDataCap = new Dictionary<string, DataCaptureList>();
        static Dictionary<string, TroyPrinterMap> portPrintMap = new Dictionary<string, TroyPrinterMap>();
        static Dictionary<string, DataCaptureFlags> dataCapFlags = new Dictionary<string, DataCaptureFlags>();
        static List<FileSystemWatcher> fileWatchers = new List<FileSystemWatcher>();
        static Dictionary<string, PantographConfiguration> portPantoConfig = new Dictionary<string, PantographConfiguration>();
        static bool UsingSoftwarePantograph = false;
        static bool UsingSoftwareTroymark = false;

        static TroyFontGlyphMapList tfgml;

        //static Troy.Core.Licensing.LicensingStatus ls;
        static LicenseQueryResult.ErrorCodes ls; // JLD new licensing 

        static bool ReprintFiles = false;
        static bool InsertPjl = false;

        string printToFilePath;

        public TroyPortMonService()
        {
            InitializeComponent();
        }
        protected override void OnStart(string[] args)
        {
            StartService(args);
        }

        public void StartService(string[] args)
        {
            try
            {
                //Call the routine to read the main configuration xml file for the service
                ReadMainConfiguration();
                XmlSerializer dsers = new XmlSerializer(typeof(TroyFontGlyphMapList));
                FileStream fss = new FileStream(serviceConfigFilePath + "TroyFontGlyphConfiguration.xml", FileMode.Open, FileAccess.Read, FileShare.Read);
                tfgml = (TroyFontGlyphMapList)dsers.Deserialize(fss);
                fss.Close();

                //Setup the file watchers for each defined port
                SetupFileWatchers();
            }
            catch (Exception ex)
            {
                //portMonLogging.LogError("Fatal Error in Service Start. " + ex.Message, EventLogEntryType.Error, true);

                EventLog.WriteEntry("Error in Troy Port Monitor Service: " + ex.Message, EventLogEntryType.Error);

                ServiceStop serviceStop = new ServiceStop(this.ServiceName);
                Thread workerThread = new Thread(new ThreadStart(serviceStop.Start));
                workerThread.Name = "Troy Service ServiceStop thread";
                workerThread.Start();
            }

        }

        private string GetPathFromRegistry(string portMonName)
        {
            string retStr;
            try
            {
                Microsoft.Win32.RegistryKey registryKey = Microsoft.Win32.Registry.LocalMachine;
                Microsoft.Win32.RegistryKey pmKey = registryKey.OpenSubKey(REGISTRY_PORTS_LOCATION, false);

                string filePath = pmKey.GetValue(portMonName).ToString();

                retStr = filePath;
                if ((retStr.Length > 0) && (!retStr.EndsWith("\\")))
                {
                    retStr += "\\";
                }
                pmKey.Close();
                registryKey.Close();
                return retStr;
            }
            catch
            {
                return "";
            }
        }

        
        private void ReadMainConfiguration()
        {
            try
            {
                Microsoft.Win32.RegistryKey registryKey = Microsoft.Win32.Registry.LocalMachine;
                Microsoft.Win32.RegistryKey pmKey = registryKey.OpenSubKey(REGISTRY_PATH_LOCATION, false);
                serviceConfigFilePath = pmKey.GetValue("MainConfigurationPath").ToString();
                if ((serviceConfigFilePath.Length > 0) && (!serviceConfigFilePath.EndsWith("\\")))
                {
                    serviceConfigFilePath += "\\";
                }
                
                string configFile = serviceConfigFilePath + "TroyPMServiceConfiguration.xml";
                
                if (File.Exists(configFile))
                {
                    //Deserialize the service configuration
                    XmlSerializer dser1 = new XmlSerializer(typeof(TroyPortMonitorServiceConfiguration));
                    FileStream fs = new FileStream(configFile, FileMode.Open, FileAccess.Read, FileShare.Read);
                    TroyPortMonitorServiceConfiguration tmsc = (TroyPortMonitorServiceConfiguration)dser1.Deserialize(fs);
                    fs.Close();

                    UsingSoftwarePantograph = tmsc.UserInterface.UseAsSecurePrint;
                    UsingSoftwareTroymark = tmsc.UserInterface.UseAsSecurePrint;

                    ReprintFiles = tmsc.PrintJobsOnRestart;
                    InsertPjl = tmsc.InsertPjlHeader;
                    
                    foreach (TroySecurePort tsp in tmsc.PortList)
                    {
                        if ((tsp.ConfigurationPath.ToUpper() != "DEFAULT") &&
                            (tsp.ConfigurationPath.Length > 0) &&
                            (!tsp.ConfigurationPath.EndsWith("\\")))
                        {
                            tsp.ConfigurationPath += "\\";
                        }
                        if (portsForFileWatcher.ContainsKey(tsp.PortMonitorName))
                        {
                            MessageBox.Show("Error: Multiple Secure Ports defined for the same TROYPORT Port Monitor.  Port Monitor = " + tsp.PortMonitorName, "TROY Port Monitor Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                            EventLog.WriteEntry("TROY SecurePort Monitor", "Error: Multiple Port Monitor Service defined for the same Port Monitor.  Port Monitor = " + tsp.PortMonitorName, EventLogEntryType.Error);
                        }
                        else
                        {
                            portsForFileWatcher.Add(tsp.PortMonitorName, tsp.ConfigurationPath);
                        }
                    }

                }
                else
                {
                    Exception newEx = new Exception("Invalid File Path for Main Configuration File.  File: " + configFile);
                    throw newEx;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Fatal Error!  Error In ReadMainConfiguation. " + ex.Message.ToString(), "TROY Port Monitor Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                throw ex;
            }
        }

        private void SetupFileWatchers()
        {
            try
            {
                printToFilePath = "";
                string configPath;
                FileSystemWatcher fileWatcher;
                TroyPortMonitorConfiguration tpmc = null;
                DataCaptureList dcl = null;

                foreach (KeyValuePair<string, string> kvp in portsForFileWatcher)
                {
                    printToFilePath = GetPathFromRegistry(kvp.Key);
                    if (kvp.Value == "")
                    {
                        configPath = "";
                    }
                    //Default is the print to file path + the config folder
                    else if (kvp.Value.ToUpper() == "DEFAULT")
                    {
                        configPath = printToFilePath + "Config\\";
                    }
                    else
                    {
                        configPath = kvp.Value.ToString();
                    }
                    string Extension = "";

                    if (configPath != "")
                    {
                        if (!configPath.EndsWith("\\"))
                        {
                            configPath += "\\";
                        }

                        DirectoryInfo dirInfo = new DirectoryInfo(configPath);
                        if (!dirInfo.Exists)
                        {
                        }
                        else
                        {
                            //Read port monitor configuraton file
                            tpmc = new TroyPortMonitorConfiguration();
                            ReadPortMonitorConfiguration(kvp.Key, configPath, printToFilePath, out Extension, out tpmc);

                            string fileName = "TroyDataCaptureConfiguration.xml";
                            if (!File.Exists(configPath + fileName))
                            {
                                EventLog.WriteEntry("TROY SecurePort Monitor", "Error: Unable to open the Data Capture XML file. File name = " + configPath + fileName, EventLogEntryType.Error);
                            }
                            else
                            {
                                XmlSerializer dser = new XmlSerializer(typeof(DataCaptureList));
                                FileStream fs = new FileStream(configPath + "TroyDataCaptureConfiguration.xml", FileMode.Open, FileAccess.Read, FileShare.Read);
                                dcl = (DataCaptureList)dser.Deserialize(fs);
                                fs.Close();
                                portDataCap.Add(printToFilePath, dcl);
                                SetupFontConfig(dcl, printToFilePath, tpmc);
                            }

                            fileName = "TroyPrinterMap.xml";
                            if (!File.Exists(configPath + fileName))
                            {
                                EventLog.WriteEntry("TROY SecurePort Monitor", "Error: Unable to open the Printer Map XML file. File name = " + configPath + fileName, EventLogEntryType.Error);
                            }
                            else
                            {
                                XmlSerializer dser = new XmlSerializer(typeof(TroyPrinterMap));
                                FileStream fs = new FileStream(configPath + fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                                TroyPrinterMap tpm = (TroyPrinterMap)dser.Deserialize(fs);
                                fs.Close();
                                portPrintMap.Add(printToFilePath, tpm);
                            }

                             fileName = "TroyPantographConfiguration.xml";
                             if (File.Exists(configPath + fileName))
                             {
                                 XmlSerializer dser = new XmlSerializer(typeof(PantographConfiguration));
                                 FileStream fs = new FileStream(configPath + fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                                 PantographConfiguration temp = (PantographConfiguration)dser.Deserialize(fs);
                                 fs.Close();
                                 portPantoConfig.Add(printToFilePath, temp);
                             }
                        }
                    }

                    DirectoryInfo dirCheck = new DirectoryInfo(printToFilePath);
                    if (!dirCheck.Exists)
                    {
                        dirCheck.Create();
                    }
                    else
                    {
                        if (!ReprintFiles)
                        {
                            foreach (FileInfo file in dirCheck.GetFiles("*." + Extension))
                            {
                                file.Delete();
                            }
                            foreach (FileInfo file2 in dirCheck.GetFiles("*.bak"))
                            {
                                file2.Delete();
                            }
                            foreach (FileInfo file2 in dirCheck.GetFiles("*.enc"))
                            {
                                file2.Delete();
                            }
                        }

                    }

                    fileWatcher = new FileSystemWatcher();
                    fileWatcher.InternalBufferSize = 12288;
                    fileWatcher.Path = printToFilePath;
                    fileWatcher.NotifyFilter = NotifyFilters.FileName;
                    fileWatcher.Filter = "*." + Extension;
                    fileWatcher.IncludeSubdirectories = false;

                    //EVENTS HANDLERS (Note: Only one event handler is needed for both events)
                    fileWatcher.Changed += new FileSystemEventHandler(fileWatcherService_Changed);
                    fileWatcher.Created += new FileSystemEventHandler(fileWatcherService_Changed);

                    //ENABLE
                    fileWatcher.EnableRaisingEvents = true;

                    fileWatchers.Add(fileWatcher);

                    if (ReprintFiles)
                    {
                        if (tpmc != null)
                        {
                            foreach (FileInfo file in dirCheck.GetFiles("*." + tpmc.FileExtension))
                            {
                                PrintJobThread printJob = new PrintJobThread();
                                printJob.printFileName = file.FullName;

                                printJob.pmConfig = tpmc;

                                printJob.pmLogging = portMonLogging;


                                TroyPrinterMap tpm = portPrintMap[printToFilePath];
                                if (tpm != null)
                                {
                                    printJob.printerMap = tpm;
                                }
                                TroyFontConfiguration tfc = portFonts[printToFilePath];
                                if (tfc != null)
                                {
                                    printJob.fontConfigs = tfc;
                                }
                                //TroyFontConfiguration tfc2 = portFonts2[printToFilePath];
                                //if (tfc2 != null)
                                //{
                                //    printJob.fontConfigs2 = tfc2;
                                //}
                                if (dcl != null)
                                {
                                    printJob.dCapConfig = dcl;
                                }
                                DataCaptureFlags dcf = dataCapFlags[printToFilePath];
                                if (dcf != null)
                                {
                                    printJob.dataCapFlags = dcf;
                                }
                                //printJob.LicenseStatus = ls;
                                printJob.SoftwarePantograph = UsingSoftwarePantograph;
                                printJob.SoftwareTroyMark = UsingSoftwareTroymark;

                                Thread printJobThread = new Thread(new ThreadStart(printJob.PrintJobReceived));

                                printJobThread.Name = file.Name + " thread";
                                printJobThread.IsBackground = true;
                                printJobThread.Priority = ThreadPriority.Normal;


                                printJobThread.Start();
                                Thread.Sleep(200);
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("TROY SecurePort Monitor", "Error in SetupFileWatchers.  Error: " + ex.Message, EventLogEntryType.Error);
                throw ex;
            }
            finally
            {
#if (DEBUG)
                Console.WriteLine("waiting...");
                Console.Read();
#endif
            }
        }

        private bool ReadPortMonitorConfiguration(string portMonName, string configPath, string fileWatchPath, out string Extension, out TroyPortMonitorConfiguration tpcm)
        {
            tpcm = null;
            try
            {

                string fileName = configPath + "TroyPortMonitorConfiguration.xml";
                XmlSerializer dser1 = new XmlSerializer(typeof(TroyPortMonitorConfiguration));
                FileStream fs1 = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                TroyPortMonitorConfiguration tmc = (TroyPortMonitorConfiguration)dser1.Deserialize(fs1);

                fs1.Close();
                tpcm = tmc;

                portMonLogging = new PortMonLogging();

                portMonLogging.LogErrorsToEventLog = tmc.LogErrorsToEventLog;

                if (tmc.DebugBackupFilesPath.ToUpper() == "DEFAULT")
                {
                    tmc.DebugBackupFilesPath = printToFilePath + "Backup\\";
                }
                if (tmc.DebugBackupFilesPath.Length > 0)
                {
                    if (!tmc.DebugBackupFilesPath.EndsWith("\\"))
                    {
                        tmc.DebugBackupFilesPath += "\\";
                    }
                    DirectoryInfo dirInfo2 = new DirectoryInfo(tmc.DebugBackupFilesPath);
                    if (!dirInfo2.Exists)
                    {
                        dirInfo2.Create();
                    }
                }

                if (tmc.ErrorLogPath.ToUpper() == "DEFAULT")
                {
                    tmc.ErrorLogPath = printToFilePath + "Error Log\\";
                }
                if (tmc.ErrorLogPath.Length > 0)
                {
                    portMonLogging.LogToErrorFile = true;
                    if (!tmc.ErrorLogPath.EndsWith("\\"))
                    {
                        tmc.ErrorLogPath += "\\";
                    }
                    DirectoryInfo dirInfo3 = new DirectoryInfo(tmc.ErrorLogPath);
                    if (!dirInfo3.Exists)
                    {
                        dirInfo3.Create();
                    }
                    portMonLogging.ErrorLogFilePath = tmc.ErrorLogPath;
                    portMonLogging.InitializeErrorLog();
                }

                portMonLogging.EnableMessageBoxes = tmc.EnableMessageBoxes;

                portLogs.Add(printToFilePath, portMonLogging);

                Extension = tmc.FileExtension;

                portConfigs.Add(fileWatchPath, tmc);

                return true;

            }
            catch (Exception ex)
            {
                MessageBox.Show("Fatal Error!  Error In ReadPortMonitorConfiguration. " + ex.Message.ToString(), "TROY Port Monitor Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                Extension = "";
                return false;
            }
        }
        
        private static void fileWatcherService_Changed(object source, FileSystemEventArgs e)
        {
            try
            {
                PrintJobThread printJob = new PrintJobThread();
                printJob.printFileName = e.FullPath;

                FileInfo getPath = new FileInfo(e.FullPath);
                string filePath = getPath.DirectoryName.ToString();
                if ((filePath.Length > 0) && (!filePath.EndsWith("\\")))
                {
                    filePath += "\\";
                }

                TroyPortMonitorConfiguration tpmc = portConfigs[filePath];
                if (tpmc != null)
                {
                    printJob.pmConfig = tpmc;
                }
                else
                {
                    Exception ex1 = new Exception("Error:  Could not find Configuration for file path " + filePath);
                    throw ex1;
                }

                portMonLogging = portLogs[filePath];
                if (portLogs != null)
                {
                    printJob.pmLogging = portMonLogging;
                }

                DataCaptureList customConfiguration = portDataCap[filePath];
                if (customConfiguration != null)
                {
                    printJob.dCapConfig = customConfiguration;
                }

                TroyPrinterMap tpm = portPrintMap[filePath];
                if (tpm != null)
                {
                    printJob.printerMap = tpm;
                }

                TroyFontConfiguration tfc = portFonts[filePath];
                if (tfc != null)
                {
                    printJob.fontConfigs = tfc;
                }

                DataCaptureFlags dcf = dataCapFlags[filePath];
                if (dcf != null)
                {
                    printJob.dataCapFlags = dcf;
                }

                PantographConfiguration pgc = portPantoConfig[filePath];
                if (pgc != null)
                {
                    printJob.pantoConfig = pgc;
                }

                printJob.insertPjl = InsertPjl;
                //printJob.LicenseStatus = ls;
                printJob.SoftwarePantograph = UsingSoftwarePantograph;
                printJob.SoftwareTroyMark = UsingSoftwareTroymark;

                Thread printJobThread = new Thread(new ThreadStart(printJob.PrintJobReceived));

                printJobThread.Name = e.Name + " thread";
                printJobThread.IsBackground = true;
                printJobThread.Priority = ThreadPriority.Normal;

                printJobThread.Start();

            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("TROY SecurePort Monitor", "Error in fileWatcherService_Changed.  Error: " + ex.Message, EventLogEntryType.Error);
            }

        }

        protected override void OnStop()
        {
            foreach (FileSystemWatcher fileWatcher in fileWatchers)
            {
                if (fileWatcher != null)
                {

                    fileWatcher.EnableRaisingEvents = false;
                    fileWatcher.Dispose();
                    ServiceStop serviceStop = new ServiceStop(this.ServiceName);
                }
            }
        }

        private void SetupFontConfig(DataCaptureList dcl, string path, TroyPortMonitorConfiguration tpmc)
        {
            try
            {
                Dictionary<string, string> fontgs = new Dictionary<string, string>();
                foreach (TroyFontGlyphMap tfgm in tfgml.FontGlyphMapList)
                {
                    fontgs.Add(tfgm.FontName, tfgm.FontGlyphFileName);
                }

                bool PlainText = false;
                bool TroyFont = false;
                bool PjlData = false;
                bool DynamicPrint = false;

                TroyFontConfiguration tfc = new TroyFontConfiguration();
                List<string> UsedFontNames = new List<string>();
                TroyFontInfo tfi;

                if (tpmc.UseConfigurableDataCapture)
                {
                    foreach (TroyDataCaptureConfiguration dcc in dcl.DataCaptureConfigurationList)
                    {
                        if (dcc.DataCapture == DataCaptureType.PjlHeader)
                        {
                            PjlData = true;
                        }
                        else if (dcc.DataCapture == DataCaptureType.PlainText)
                        {
                            PlainText = true;
                        }
                        else if (dcc.DataCapture == DataCaptureType.TroyFonts)
                        {
                            TroyFont = true;
                        }
                        if (dcc.DataUse == DataUseType.PrinterMap)
                        {
                            DynamicPrint = true;
                        }

                        if ((dcc.DataCapture == DataCaptureType.StandardFonts) &&
                            (dcc.FontNames.Count > 0))
                        {
                            foreach (string fn in dcc.FontNames)
                            {
                                if (UsedFontNames.Contains(fn))
                                {
                                    foreach (TroyFontInfo ti in tfc.TroyFontInfoList)
                                    {
                                        if (ti.FontName == fn)
                                        {
                                            if (dcc.DataUse == DataUseType.PassThrough)
                                            {
                                                ti.UseForPassThrough = true;
                                            }
                                            else if (dcc.DataUse == DataUseType.PrinterMap)
                                            {
                                                ti.UseForPrinterMap = true;
                                            }
                                            else if (dcc.DataUse == DataUseType.TroyMark)
                                            {
                                                ti.UseForTroyMark = true;
                                                ti.UseAllDataForTm = dcc.UseAllData;
                                            }
                                            else if (dcc.DataUse == DataUseType.MicroPrint)
                                            {
                                                ti.UseForMicroPrint = true;
                                                ti.UseAllDataForMp = dcc.UseAllData;
                                            }

                                            if (!ti.RemoveData)
                                            {
                                                ti.RemoveData = dcc.RemoveData;
                                            }

                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    if (fontgs.ContainsKey(fn))
                                    {
                                        UsedFontNames.Add(fn);
                                        tfi = new TroyFontInfo();
                                        tfi.FontName = fn;
                                        tfi.FontType = "Data Capture";
                                        tfi.GlyphMapFile = fontgs[fn];
                                        tfi.LoadGlyphToCharMap(serviceConfigFilePath + "Data\\" + tfi.GlyphMapFile);
                                        tfi.RemoveData = dcc.RemoveData;
                                        if (dcc.DataUse == DataUseType.PassThrough)
                                        {
                                            tfi.UseForPassThrough = true;
                                        }
                                        else if (dcc.DataUse == DataUseType.PrinterMap)
                                        {
                                            tfi.UseForPrinterMap = true;
                                        }
                                        else if (dcc.DataUse == DataUseType.TroyMark)
                                        {
                                            tfi.UseForTroyMark = true;
                                            tfi.UseAllDataForTm = dcc.UseAllData;
                                        }
                                        else if (dcc.DataUse == DataUseType.MicroPrint)
                                        {
                                            tfi.UseForMicroPrint = true;
                                            tfi.UseAllDataForMp = dcc.UseAllData;
                                        }

                                        tfc.TroyFontInfoList.Add(tfi);
                                    }
                                    else
                                    {
                                        //Error, can not find glyph
                                    }
                                }
                            }
                        }
                    }
                }
                else if (tpmc.UseSecureRxSetup)
                {
                    if (tpmc.SecureRxFonts != "")
                    {
                        foreach (string str in tpmc.SecureRxFonts.Split(','))
                        {
                            tfi = new TroyFontInfo();
                            tfi.FontName = str;
                            if (tpmc.SaveFontDataAsTokens)
                            {
                                tfi.FontType = "Data Tokens";
                            }
                            else
                            {
                                tfi.FontType = "Data Capture";
                            }
                            tfi.GlyphMapFile = "StandardGlyphMap.csv";
                            tfi.LoadGlyphToCharMap(serviceConfigFilePath + "Data\\" + tfi.GlyphMapFile);
                            tfi.RemoveData = false;
                            tfi.UseForTroyMark = true;
                            tfc.TroyFontInfoList.Add(tfi);
                        }
                    }
                    else if (tpmc.PlainTextData)
                    {
                        PlainText = true;
                    }
                }
                else
                {

                }
                DataCaptureFlags dcf = new DataCaptureFlags();
                dcf.EnableLookForTroyFontCalls = TroyFont;
                dcf.EnablePjlHeaderCapture = PjlData;
                dcf.EnablePlainTextCapture = PlainText;
                dcf.DynamicPrinterConfig = DynamicPrint;
                dataCapFlags.Add(path, dcf);
                portFonts.Add(path, tfc);


            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("TROY SecurePort Monitor", "Error in SetupFontConfig.  Error: " + ex.Message, EventLogEntryType.Error);
            }
        }


    }
}
