using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;  //File Watcher
using System.ServiceProcess;
using System.Threading;  // only used to stop troy service
using System.Windows.Forms; //MessageBox
using System.Xml.Serialization;
using Troy.PortMonitor.Core.XmlConfiguration;
using Troy;
//using TroySecurePortMonitorUserInterface;
using PantographPclBuilder;

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
        static Dictionary<string, DataCaptureFlags> portDataCapFlags = new Dictionary<string, DataCaptureFlags>();
        static List<FileSystemWatcher> fileWatchers = new List<FileSystemWatcher>();
        static Dictionary<string, PantographConfiguration> portPantoConfig = new Dictionary<string, PantographConfiguration>();
        static bool UsingSoftwarePantograph = false;
        static bool UsingSoftwareTroymark = false;

        static TroyFontGlyphMapList tfgml;

        static ELicenseStatus ls; // JLD new licensing 

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
                //Setup the file watchers for each defined port
                SetupFileWatchers();
            }
            catch (Exception ex)
            {
                //portMonLogging.LogError("Fatal Error in Service Start. " + ex.Message, EventLogEntryType.Error, true);

                EventLog.WriteEntry("Error in Troy Port Monitor Service: " + ex.Message, EventLogEntryType.Error);

                this.Stop();

                /********
                ServiceStop serviceStop = new ServiceStop(this.ServiceName);
                Thread workerThread = new Thread(new ThreadStart(serviceStop.Start));
                workerThread.Name = "Troy Service ServiceStop thread";
                workerThread.Start();
                 * **************/
            }

        }
        private static string extractPath(string fqname)
        {
            int idx = fqname.LastIndexOf("\\");
            string path = (idx < 0 ? null : fqname.Substring(0, idx + 1));
            return path;
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
                if ((retStr.Length > 0) && (!retStr.EndsWith("\\"))) retStr += "\\";
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
                        if (portsForFileWatcher.ContainsKey(tsp.PortMonitorName))
                        {
                            MessageBox.Show("Error: Multiple Secure Ports defined for the same TROYPORT Port Monitor.  Port Monitor = " + tsp.PortMonitorName, "TROY Port Monitor Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                            EventLog.WriteEntry("TROY SecurePort Monitor", "Error: Multiple Port Monitor Service defined for the same Port Monitor.  Port Monitor = " + tsp.PortMonitorName, EventLogEntryType.Error);
                        }
                        else
                        {
                            if (tsp.ConfigurationPath.Length == 0 || tsp.ConfigurationPath.ToLower() == "default")
                                tsp.ConfigurationPath = GetPathFromRegistry(tsp.PortMonitorName) + "Config\\";
                            else if (!tsp.ConfigurationPath.EndsWith("\\"))
                                tsp.ConfigurationPath += "\\";
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

        private TroyPortMonitorConfiguration ConfigPort(KeyValuePair<string, string> kvp)
        {
            string configPath = kvp.Value.ToString();
            printToFilePath = GetPathFromRegistry(kvp.Key);

            var tpmc = new TroyPortMonitorConfiguration();
            DirectoryInfo dirInfo = new DirectoryInfo(configPath);
            if (!dirInfo.Exists)
            {
                EventLog.WriteEntry("TROY SecurePort Monitor", configPath + " does not exist", EventLogEntryType.Error);
            }
            else
            {
                //Read port monitor configuraton file
                ReadPortMonitorConfiguration(kvp.Key, configPath, printToFilePath, out tpmc);
                var keyName = new DirectoryInfo(printToFilePath).Name;

                string fileName = "TroyDataCaptureConfiguration.xml";
                if (!File.Exists(configPath + fileName))
                {
                    EventLog.WriteEntry("TROY SecurePort Monitor", "Error: Unable to open the Data Capture XML file. File name = " + configPath + fileName, EventLogEntryType.Error);
                }
                else
                {
                    XmlSerializer dser = new XmlSerializer(typeof(DataCaptureList));
                    FileStream fs = new FileStream(configPath + "TroyDataCaptureConfiguration.xml", FileMode.Open, FileAccess.Read, FileShare.Read);
                    DataCaptureList dcl = (DataCaptureList)dser.Deserialize(fs);
                    fs.Close();
                    portDataCap.Add(keyName, dcl);
                    SetupFontConfig(dcl, keyName, tpmc);
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
                    portPrintMap.Add(keyName, tpm);
                }

                fileName = "TroyPantographConfiguration.xml";
                if (File.Exists(configPath + fileName))
                {
                    XmlSerializer dser = new XmlSerializer(typeof(PantographConfiguration));
                    FileStream fs = new FileStream(configPath + fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                    PantographConfiguration temp = (PantographConfiguration)dser.Deserialize(fs);
                    fs.Close();
                    portPantoConfig.Add(keyName, temp);
                }
            }
            return tpmc;
        }

        private FileSystemWatcher portWatcher(TroyPortMonitorConfiguration tpmc)
        {
            var fileWatcher = new FileSystemWatcher();
            fileWatcher.InternalBufferSize = 12288;
            fileWatcher.Path = printToFilePath;
            fileWatcher.NotifyFilter = NotifyFilters.FileName;
            fileWatcher.Filter = "*." + tpmc.FileExtension;
            fileWatcher.IncludeSubdirectories = false;


            //EVENTS HANDLERS (Note: Only one event handler is needed for both events)
            fileWatcher.Changed += new FileSystemEventHandler(w_FileCreated);
            fileWatcher.Created += new FileSystemEventHandler(w_FileCreated);

            //ENABLE
            fileWatcher.EnableRaisingEvents = true;
            return fileWatcher;
        }

        private void SetupFileWatchers()
        {
            try
            {
                foreach (KeyValuePair<string, string> kvp in portsForFileWatcher)
                {
                    TroyPortMonitorConfiguration tpmc = ConfigPort(kvp);
                    tpmc.InitializeValues();
                    HandleExistingFiles(tpmc, ReprintFiles);
                    fileWatchers.Add(portWatcher(tpmc));
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
        private static void HandleExistingFiles(TroyPortMonitorConfiguration tpmc, bool process)
        {
            DirectoryInfo dirCheck = new DirectoryInfo(tpmc.PrintFilePath);
            if (dirCheck.Exists)
            {
                if (!process)
                {
                    foreach (FileInfo file in dirCheck.GetFiles("*." + tpmc.FileExtension))
                    {
                        file.Delete();
                    }
                    foreach (FileInfo file2 in dirCheck.GetFiles("*.bak"))
                    {
                        file2.Delete();
                    }
                    foreach (FileInfo file3 in dirCheck.GetFiles("*.enc"))
                    {
                        file3.Delete();
                    }
                }
                else
                {
                    if (tpmc != null)
                    {
                        foreach (FileInfo file in dirCheck.GetFiles("*." + tpmc.FileExtension))
                        {
                            TaskMgr.ProcessFile(file.Name, file.FullName);
                        }
                    }
                }
            }
            else dirCheck.Create();
        }

        public static PrintJobThread setupPrintJob(string fullname)
        {
            string fullnamePath = extractPath(fullname);
            PrintJobThread printJob = new PrintJobThread();
            printJob.printFileName = fullname;

            FileInfo fi = new FileInfo(fullname);
            string dirName = fi.Directory.Name;

            TroyPortMonitorConfiguration tpmc = portConfigs[dirName];
            if (tpmc != null)
                printJob.pmConfig = tpmc;
            else
            {
                Exception ex1 = new Exception("Error:  Could not find Configuration for directory " + dirName);
                throw ex1;
            }

            var pml = portLogs[dirName];
            if (pml != null) printJob.pmLogging = pml;

            DataCaptureList dcl = portDataCap[dirName];
            if (dcl != null) printJob.dCapConfig = dcl;

            TroyPrinterMap pm = portPrintMap[dirName];
            if (pm != null) printJob.printerMap = pm;

            TroyFontConfiguration tfc = portFonts[dirName];
            if (tfc != null) printJob.fontConfigs = tfc;

            DataCaptureFlags dcf = portDataCapFlags[dirName];
            if (dcf != null) printJob.dataCapFlags = dcf;

            PantographConfiguration pgc = portPantoConfig[dirName];
            if (pgc != null) printJob.pantoConfig = pgc;

            printJob.insertPjl = InsertPjl;
            printJob.SoftwarePantograph = UsingSoftwarePantograph;
            printJob.SoftwareTroyMark = UsingSoftwareTroymark;
            return printJob;
        }

        private bool ReadPortMonitorConfiguration(string portMonName, string configPath, string printToFilePath, out TroyPortMonitorConfiguration tpmc)
        {
            tpmc = null;
            try
            {
                string fileName = configPath + "TroyPortMonitorConfiguration.xml";
                var dirName = new DirectoryInfo(printToFilePath).Name;

                var dser1 = new XmlSerializer(typeof(TroyPortMonitorConfiguration));
                var fs1 = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                tpmc = (TroyPortMonitorConfiguration)dser1.Deserialize(fs1);

                fs1.Close();
                tpmc.PrintFilePath = printToFilePath;

                portMonLogging = new PortMonLogging();

                portMonLogging.LogErrorsToEventLog = tpmc.LogErrorsToEventLog;
                portMonLogging.EnableTraceLog = tpmc.EnableTraceLog;

                if (tpmc.DebugBackupFilesPath.ToUpper() == "DEFAULT") tpmc.DebugBackupFilesPath = printToFilePath + "Backup\\";
                if (tpmc.DebugBackupFilesPath.Length > 0)
                {
                    if (!tpmc.DebugBackupFilesPath.EndsWith("\\")) tpmc.DebugBackupFilesPath += "\\";
                    Directory.CreateDirectory(tpmc.DebugBackupFilesPath);
                }

                if (tpmc.ErrorLogPath.ToUpper() == "DEFAULT") tpmc.ErrorLogPath = printToFilePath + "Error Log\\";
                if (tpmc.ErrorLogPath.Length > 0)
                {
                    portMonLogging.LogToErrorFile = true;
                    if (!tpmc.ErrorLogPath.EndsWith("\\")) tpmc.ErrorLogPath += "\\";
                    Directory.CreateDirectory(tpmc.ErrorLogPath);
                    portMonLogging.ErrorLogFilePath = tpmc.ErrorLogPath;
                    portMonLogging.InitializeErrorLog();
                    if (tpmc.EnableTraceLog)
                    {
                        portMonLogging.TraceLogPath = tpmc.ErrorLogPath;
                        portMonLogging.InitializeTraceLog();
                    }

                }

                portMonLogging.EnableMessageBoxes = tpmc.EnableMessageBoxes;

                portLogs.Add(dirName, portMonLogging);

                portConfigs.Add(dirName, tpmc);

                return true;

            }
            catch (Exception ex)
            {
                MessageBox.Show("Fatal Error!  Error In ReadPortMonitorConfiguration. " + ex.Message.ToString(), "TROY Port Monitor Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                return false;
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
                }
            }
            base.OnStop();
            //ServiceStop serviceStop = new ServiceStop(this.ServiceName);
        }

        private void SetupFontConfig(DataCaptureList dcl, string dirName, TroyPortMonitorConfiguration tpmc)
        {
            try
            {
                Dictionary<string, string> fontgs = new Dictionary<string, string>();
                XmlSerializer dsers = new XmlSerializer(typeof(TroyFontGlyphMapList));
                FileStream fss = new FileStream(serviceConfigFilePath + "TroyFontGlyphConfiguration.xml", FileMode.Open, FileAccess.Read, FileShare.Read);
                tfgml = (TroyFontGlyphMapList)dsers.Deserialize(fss);
                fss.Close();
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
                        switch (dcc.DataCapture)
                        {
                            case DataCaptureType.PjlHeader:
                                PjlData = true;
                                break;
                            case DataCaptureType.PlainText:
                                PlainText = true;
                                break;
                            case DataCaptureType.TroyFonts:
                                TroyFont = true;
                                break;
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
                                            switch (dcc.DataUse)
                                            {
                                                case DataUseType.PassThrough:
                                                    ti.UseForPassThrough = true;
                                                    break;
                                                case DataUseType.PrinterMap:
                                                    ti.UseForPrinterMap = true;
                                                    break;
                                                case DataUseType.TroyMark:
                                                    ti.UseForTroyMark = true;
                                                    ti.UseAllDataForTm = dcc.UseAllData;
                                                    break;
                                                case DataUseType.MicroPrint:
                                                    ti.UseForMicroPrint = true;
                                                    ti.UseAllDataForMp = dcc.UseAllData;
                                                    break;
                                            }
                                            if (!ti.RemoveData) ti.RemoveData = dcc.RemoveData;
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

                                        switch (dcc.DataUse)
                                        {
                                            case DataUseType.PassThrough:
                                                tfi.UseForPassThrough = true;
                                                break;
                                            case DataUseType.PrinterMap:
                                                tfi.UseForPrinterMap = true;
                                                break;
                                            case DataUseType.TroyMark:
                                                tfi.UseForTroyMark = true;
                                                tfi.UseAllDataForTm = dcc.UseAllData;
                                                break;
                                            case DataUseType.MicroPrint:
                                                tfi.UseForMicroPrint = true;
                                                tfi.UseAllDataForMp = dcc.UseAllData;
                                                break;
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
                portDataCapFlags.Add(dirName, dcf);
                portFonts.Add(dirName, tfc);


            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("TROY SecurePort Monitor", "Error in SetupFontConfig.  Error: " + ex.Message, EventLogEntryType.Error);
            }
        }

        static void w_FileCreated(object sender, FileSystemEventArgs e)
        {
            TaskMgr.ProcessFile(e.Name, e.FullPath);
        }

        static bool HandleLeftovers(string portFolder)
        {
            if (portConfigs.ContainsKey(portFolder))
            {
                TroyPortMonitorConfiguration tpmc = portConfigs[portFolder];
                HandleExistingFiles(tpmc, true);
                return true;
            }
            else return false;
        }
    }
    class TaskMgr
    {
        private static int result;

        public static int ProcessFile(string name, string fullPath)
        {
            if (added2List(name, fullPath))
            {
                var item = fileList[name];
                //Console.WriteLine(String.Format("{0} added to list of {1}", name, TaskMgr.fileList.Count));

                PrintJobThread printJob = TroyPortMonService.setupPrintJob(fullPath);
                item.status = statusType.Processing;
                item.start = DateTime.Now;
                lock (_lockObject)
                {
                    fileList[name] = item;
                }
                int result = doTask(printJob);
                lock (_lockObject)
                {
                    fileList.Remove(name);
                }

                //Console.WriteLine(String.Format("{0} removed from list of {1}", name, TaskMgr.fileList.Count));
                return result;
            }
            else
            { 
                return 1;
            }
        }
        
        static bool added2List(string fn, string fullPath)
        {
            if (fileList.ContainsKey(fn))
            {
                return false;
            }
            else
            {
                var item = new printFile();
                item.fullPath = fullPath;
                item.status = statusType.Waiting;
                lock (_lockObject)
                {
                    fileList.Add(fn, item);
                }
                return true;
            }
        }

        public static int doTask(PrintJobThread printJob)
        {
            // Create Task, defer starting it, continue with another task
            Task<int> t = Task<int>.Factory.StartNew(() =>
                {
                     return printJob.PrintJobReceived();
                });
            Task cwt = t.ContinueWith(task => { result = task.Result; Console.WriteLine("Task result " + task.Result); },
                TaskContinuationOptions.OnlyOnFaulted);

            return result;
        }
        public struct printFile
        {
            public string fullPath;
            public statusType status;
            public DateTime start;
            public DateTime finish;
        }
        public enum statusType
        { Waiting, Processing, Processed }
        public static Dictionary<string, printFile> fileList = new Dictionary<string, printFile>();
        static readonly object _lockObject = new object();
    }

}
