using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CustomInstallFunctions
{
    class Program
    {
        static List<string> logInfo = new List<string>();
        static string logFilename;

        static void Main(string[] args)
        {
            const string REGISTRY_PATH_LOCATION = "System\\CurrentControlSet\\Control\\Print\\Monitors\\TroySecurePortMonitor";
            const string REGISTRY_PORTS_LOCATION = REGISTRY_PATH_LOCATION + "\\Ports";
            string msg;
            string progFiles = System.Windows.Forms.Application.StartupPath;
            logFilename = Path.Combine(progFiles, "CustomInstallLog.txt");

            if (args.Length < 1)
            {
                WriteMsg("You must specify setup exec folder as first arg");
                WriteLogList(logFilename);
                return;
            }
            string sourceDir = args[0];
            if (!sourceDir.EndsWith("\\"))
            {
                sourceDir += "\\";
            }
            WriteMsg("SourceDir = " + sourceDir);

            if (!progFiles.EndsWith("\\"))
            {
                progFiles += "\\";
            }
            WriteMsg("Startup Path = " + progFiles);

/*********************************************
            Microsoft.Win32.RegistryKey registryKey = Microsoft.Win32.Registry.LocalMachine;
            Microsoft.Win32.RegistryKey troyRegPath = registryKey.OpenSubKey(REGISTRY_PATH_LOCATION, true);
            Microsoft.Win32.RegistryKey pmKey = registryKey.OpenSubKey(REGISTRY_PORTS_LOCATION, true);
            troyRegPath.SetValue("MainConfigurationPath", progFiles);
            WriteMsg("Set MainConfigurationPath = " + progFiles);
            WriteMsg("Finding Each TROYPORT in Registry");
            foreach (string valnames in pmKey.GetValueNames())
            {
                if (valnames.Contains("TROYPORT"))
                {
                    string temp = valnames.Replace("TROYPORT", "");
                    temp = temp.Replace(":", "");
                    string printpath = progFiles + @"PrintPort" + temp + @"\";
                    pmKey.SetValue(valnames, printpath);
                    WriteMsg("Setting " + valnames + " = " + printpath);
                }
                else
                {
                    WriteMsg("Non-TROYPORT value found. " + valnames);
                }
            }
            System.Threading.Thread.Sleep(1000);
            WriteMsg("--- Setting Registry Values COMPLETE ---");
*************************************************/

            WriteMsg("--- Copying Configuration and Data Files  ---");
            try
            {
                string tspeFolder = sourceDir + "TspeFiles";
                string configFolder = sourceDir + "Configuration";
                if (Directory.Exists(tspeFolder))
                {
                    WriteMsg("Found TSPE Files folder.");
                    DirectoryInfo dirInfo = new DirectoryInfo(tspeFolder);
                    foreach (FileInfo fi in dirInfo.GetFiles())
                    {
                        string fn = Path.Combine(progFiles, fi.Name);
                        fi.CopyTo(fn, true);
                        WriteMsg("Copied " + fi.Name + " to " + fn);
                    }
                    foreach (DirectoryInfo dir in dirInfo.GetDirectories())
                    {
                        if (dir.Name.ToLower() == "data")
                        {
                            foreach (DirectoryInfo dir2 in dir.GetDirectories())
                            {
                                if (!Directory.Exists(progFiles + @"Data\" + dir2.Name))
                                {
                                    Directory.CreateDirectory(progFiles + @"Data\" + dir2.Name);
                                    WriteMsg("Created directory " + progFiles + @"Data\" + dir2.Name);
                                }
                                foreach (FileInfo fi in dir2.GetFiles())
                                {
                                    string fn = Path.Combine(progFiles + @"Data\" + dir2.Name, fi.Name);
                                    fi.CopyTo(fn, true);
                                    WriteMsg("Copied " + fi.Name + " to " + fn);
                                }

                            }
                        }
                        else
                        {
                            if (!Directory.Exists(progFiles + @"Baseline\" + dir.Name))
                            {
                                Directory.CreateDirectory(progFiles + @"Baseline\" + dir.Name);
                                WriteMsg("Created directory " + progFiles + @"Baseline\" + dir.Name);
                            }
                            foreach (FileInfo fi in dir.GetFiles())
                            {
                                string fn = Path.Combine(progFiles + @"Baseline\" + dir.Name, fi.Name);
                                fi.CopyTo(fn, true);
                                WriteMsg("Copied " + fi.Name + " to " + fn);
                            }
                        }
                    }
                }
                else
                {
                    WriteMsg(String.Format("No Baseline Data Files to Copy.  {0} not found (Note: Case Sensitive)", tspeFolder));
                }
                    
                if (Directory.Exists(configFolder))
                {
                    WriteMsg("Found Configuration Files folder");
                    DirectoryInfo dirInfo = new DirectoryInfo(configFolder);
                    foreach (DirectoryInfo dir in dirInfo.GetDirectories())
                    {
                        if (dir.Name.ToLower() == "config")
                        {
                            foreach (FileInfo fi in dir.GetFiles())
                            {
                                string fn = Path.Combine(progFiles + "Configuration", fi.Name);
                                fi.CopyTo(fn, true);
                                WriteMsg("Copied " + fi.Name + " to " + fn);
                                fn = Path.Combine(progFiles + @"PrintPort1\Config", fi.Name);
                                fi.CopyTo(fn, true);
                                WriteMsg("Copied " + fi.Name + " to " + fn);
                            }
                        }
                        else if (dir.Name.ToLower() == "data")
                        {
                            foreach (FileInfo fi in dir.GetFiles())
                            {
                                string fn = Path.Combine(progFiles + "Data", fi.Name);
                                fi.CopyTo(fn, true);
                                WriteMsg("Copied " + fi.Name + " to " + fn);
                                fn = Path.Combine(progFiles + @"PrintPort1\Data", fi.Name);
                                fi.CopyTo(fn, true);
                                WriteMsg("      Copied " + fi.Name + " to " + fn);
                            }
                        }
                        else
                        {
                            if (!Directory.Exists(progFiles + "Configuration\\" + dir.Name))
                            {
                                Directory.CreateDirectory(progFiles + "Configuration\\" + dir.Name);
                            }
                            foreach (FileInfo fi in dir.GetFiles())
                            {
                                string fn = Path.Combine(progFiles + "Configuration\\" + dir.Name, fi.Name);
                                fi.CopyTo(fn, true);
                                WriteMsg("Copied Configuration Files to " + progFiles + "Configuration\\" + dir.Name);
                            }
                        }
                    }

                }
                else
                {
                    WriteMsg(String.Format("No Configuration Files to Copy.  {0} not found (Note: Case Sensitive)", configFolder));
                }

                string src = sourceDir + "Port Monitor";
                string dest = progFiles;
                if (Directory.Exists(src))
                {
                    Console.WriteLine("Copying Port Monitor File(s)....");
                    DirectoryInfo dirInfo = new DirectoryInfo(sourceDir + "Port Monitor");
                    string[] exts = { ".xml", ".lic" };
                    foreach (FileInfo fi in dirInfo.GetFiles())
                    {
                        if (exts.Contains(fi.Extension.ToLower()))
                        {
                            string fn = Path.Combine(progFiles, fi.Name);
                            fi.CopyTo(fn, true);
                            WriteMsg("      Copied " + fi.Name + " to " + fn);
                        }
                    }
                    copyAllSubfolders(src, dest);
                }
                else
                {
                    WriteMsg("   No Port Monitor Folder to migrate.  " + sourceDir + "Port Monitor folder not found (Note: Case Sensitive)");
                }



            }
            catch (Exception ex)
            {
                WriteMsg("ERROR! " + ex.Message + ex.StackTrace);
            }
            WriteMsg("--- Copying Configuration and Data Files COMPLETE  ---");

            System.Threading.Thread.Sleep(500);
            
            string regFile = progFiles + "registry.txt";
            if (!File.Exists(regFile))
            {
                WriteMsg("Restarting the Windows Print Spooler....");
                System.ServiceProcess.ServiceController psService = new System.ServiceProcess.ServiceController("Print Spooler");
                if (psService.Status == System.ServiceProcess.ServiceControllerStatus.Running)
                {
                    psService.Stop();
                }
                System.Threading.Thread.Sleep(1000);
                psService.Start();
                WriteMsg("Windows Print Spooler restarted");
            }
            else File.Delete(regFile);


            WriteMsg("Restarting the TROY SecurePort Monitor Service....");
            System.ServiceProcess.ServiceController pmService = new System.ServiceProcess.ServiceController("Troy Port Monitor Service");
            if (pmService.Status == System.ServiceProcess.ServiceControllerStatus.Running)
            {
                pmService.Stop();
            }
            System.Threading.Thread.Sleep(1000);
            pmService.Start();
            WriteMsg("Troy Port Monitor Service restarted");
            WriteLogList(logFilename);
        }

        private static void copyAllSubfolders(string SourcePath, string DestinationPath)
        {
            if (SourcePath.EndsWith("\\")) SourcePath = DestinationPath.Remove(SourcePath.Length - 1);
            if (DestinationPath.EndsWith("\\")) DestinationPath = DestinationPath.Remove(DestinationPath.Length - 1);

            //Now Create all of the directories
            foreach (string dirPath in Directory.GetDirectories(SourcePath, "*", SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(dirPath.Replace(SourcePath, DestinationPath));
                //Copy all the files
                foreach (string srcFile in Directory.GetFiles(dirPath))
                {
                    string destFile = srcFile.Replace(SourcePath, DestinationPath);
                    File.Copy(srcFile, destFile, true);
                    WriteMsg(srcFile + " copied to " + destFile);
                }
            }
        }

        static void WriteMsg(string msg)
        {
            Console.WriteLine(msg);
            logInfo.Add(msg);

        }

        static void WriteLogList(string filename)
        {
            try
            {
                if (File.Exists(filename))
                {
                    FileInfo saveFile = new FileInfo(filename);
                    StreamWriter saveWrite = new StreamWriter(saveFile.OpenWrite());
                    foreach (string listString in logInfo)
                    {
                        saveWrite.WriteLine(listString);
                    }
                    saveWrite.Close();

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error writing to Log.  " + ex.Message);
            }
        }
    }
}
