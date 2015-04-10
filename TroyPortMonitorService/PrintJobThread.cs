using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using PantographPclBuilder;
using Troy.PortMonitor.Core.XmlConfiguration;
using Troy.TSPE3.Licensing;
using Troy.TROYmarkPclBuilder;
using Troy.Licensing.Client.Classes;

namespace TroyPortMonitorService
{
    class GlyphMapType
    {
        const int MaxNumberOfGlyphs = 255;
        public int[] GlyphMap = new int[MaxNumberOfGlyphs + 1];
        public void AddToGlyphMap(int fontCharId, int glyphId)
        {
            if (fontCharId <= MaxNumberOfGlyphs)
            {
                GlyphMap[fontCharId] = glyphId;
            }
        }
        public int GetGlyphId(int fontCharId)
        {
            if (fontCharId <= MaxNumberOfGlyphs)
            {
                return GlyphMap[fontCharId];
            }
            else
            {
                return -1;
            }
        }
    }

    class TaggedListPerPage
    {
        public List<string> TaggedList = new List<string>();
    }

    public class PrintJobThread
    {
        //Interface to service variables
        public string printFileName;
        public TroyFontConfiguration fontConfigs;
        public PortMonLogging pmLogging;
        public TroyPortMonitorConfiguration pmConfig;
        public DataCaptureList dCapConfig;
        public TroyPrinterMap printerMap;
        public DataCaptureFlags dataCapFlags;
        public PantographConfiguration pantoConfig;
        //public string FolderMonitorLocation = "";
        public bool insertPjl = false;
        public bool encryptionEnabled = false;
        public bool SoftwarePantograph = false;
        public bool SoftwareTroyMark = false;

        private const byte ESC = 0x1B;

        //The File
        private FileInfo printJobFileInfo;

        //The Input Buffer
        private byte[] inputBuffer;

        //Constants
        private const int MAX_FONT_ID_DIGITS = 5;
        private const int MAX_FONT_DESC_DIGITS = 6;
        private const int FONT_NAME_LOC = 48;
        private const int MAX_POS_DIGITS = 4;
        private const int MAX_PAGES_PER_JOB = 1000;

        private int LengthOfLeadingPcl = 64;
        private int LengthOfTrailingPcl = 64;

        private string printToPrinterName = "";

        private Dictionary<int, TroyFontInfo> fontConfigList = new Dictionary<int, TroyFontInfo>();
        private StringBuilder fontCharStr = new StringBuilder();
        private Dictionary<int, GlyphMapType> fontCharToGlyphMap = new Dictionary<int, GlyphMapType>();

        private int EndPjlLocation = -1;

        private int currentPageNum = 1;

        private string tempFileName = "";
        private string printerFileName = "";

        private enum EventPointType
        {
            epInsert = 0,
            epRemove = 1,
            epSubstitute = 2,
            epInsertPoint = 3,
            epPageEnd = 4,
            epUELLocation = 5,
            epPrinterReset = 6,
            epPaperSource = 7
        }

        private struct EventPoints
        {
            public int PageNumber;
            public int Location;
            public EventPointType EventType;
            public int EventLength;
            public EventPoints(int pageNumber, int location, EventPointType eventType, int eventLength)
            {
                PageNumber = pageNumber;
                Location = location;
                EventType = eventType;
                EventLength = eventLength;
            }
        }
        private List<EventPoints> fileEventPoints;
        EventPoints newEventPoint;

        private struct PageBoundaries
        {
            public int PageNumber;
            public int PageStart;
            public int PageEnd;
            public int InsertPoint;
            public bool ConfigPage;
            //public int AnchorPoint0Location;
            //public bool SkipPage;
        }
        private List<PageBoundaries> pageInfoList = new List<PageBoundaries>();

        PageBoundaries currentPage = new PageBoundaries();

        private struct InsertStrings
        {
            public int PageNumber;
            public int Location;
            public byte[] LeadingPcl;
            public int LeadingPclLength;
            public string ConvertedAscii;
            public byte[] TrailingPcl;
            public int TrailingPclLength;
            public bool MoveToInsertLocation;
            public bool RemoveString;
            public void SetLeadingPcl(byte[] pcl, int pclLength)
            {
                LeadingPclLength = pclLength;
                LeadingPcl = new byte[pclLength];
                for (int temp = 0; temp < pclLength; temp++)
                {
                    LeadingPcl[temp] = pcl[temp];
                }
            }
            public void SetTrailingPcl(byte[] pcl, int pclLength)
            {
                TrailingPclLength = pclLength;
                TrailingPcl = new byte[pclLength];
                for (int temp = 0; temp < pclLength; temp++)
                {
                    TrailingPcl[temp] = pcl[temp];
                }
            }
        }
        private List<InsertStrings> insertStringList;
        private InsertStrings insertString;

        private bool SentToPrinter = false;

        private string dataCapture = "";
        private string troyFontCapture = "";
        private Dictionary<int, string> PlainTextPerPage = new Dictionary<int, string>();
        //NOTE:  PAGE 0 is PJL HEADER
        private Dictionary<int, string> TroyMarkDataPerPage = new Dictionary<int, string>();
        private Dictionary<int, string> CapturedAllDataPerPage = new Dictionary<int, string>();
        private Dictionary<int, string> CapturedDataPerPage = new Dictionary<int, string>();
        private Dictionary<int, string> PassThroughPerPage = new Dictionary<int, string>();
        private Dictionary<int, string> PrinterMapPerPage = new Dictionary<int, string>();
        private Dictionary<int, string> TroyFontDataPerPage = new Dictionary<int, string>();
        private Dictionary<int, string> MpDataPerPage = new Dictionary<int, string>(); 
        private Dictionary<int, string> CapturedAllMpDataPerPage = new Dictionary<int, string>(); 

        private StringBuilder plainTextStr = new StringBuilder();

        private List<string> PrinterMapStringList = new List<string>();

        private string PassThroughString = "";
        private string TroymarkAllData = "";
        private string PrinterMapString = "";
        private string MpAllData = "";
        private string MpCapturedData = "";

        private string PantographDataFilesLocation = "";

        private byte[] EndOfPageString;
        private byte[] AltEndOfPageString;
        private byte[] InsertPointString;

        private byte[] RemoveEscStringBytes;

        //*p6400Y<FF>
        private byte[] DefaultEndOfPage = new byte[9] { 0x1B, 0x2A, 0x70, 0x36, 0x34, 0x30, 0x30, 0x59, 0x0C };
        //*p0x0Y
        private byte[] DefaultInsertPoint = new byte[7] { 0x1B, 0x2A, 0x70, 0x30, 0x78, 0x30, 0x59 };

        //number of copies
        private byte[] NumberOfCopiesOne = new byte[5] { 0x1B, 0x26, 0x6C, 0x31, 0x58 };

        private string PJL_DEFAULT_DENSITY = "@PJL DEFAULT DENSITY=3\u000D\u000A";
        private string PJL_SET_ECONOMODE_OFF = "@PJL SET ECONOMODE=OFF\u000D\u000A";
        private string PJL_SET_REPRINT_OFF = "@PJL SET REPRINT=OFF\u000D\u000A";
        private string PJL_SET_HOLD_OFF = "@PJL SET HOLD=OFF\u000D\u000A";

        private BinaryWriter outbuf = null;

        private List<string> taggedDataList = new List<string>();
        private List<TaggedListPerPage> TaggedListPageList = new List<TaggedListPerPage>();

        private bool InsertedDemoTroymark = false;

        private string PrintJobName = "";
        private string serviceConfigFilePath;

        private Dictionary<int, int> ExclusionRegion = new Dictionary<int, int>();

        //Forerun Specific 
        private string PrinterFromPJL = "";
        private string PatientNameFromPJL = "";
        private Dictionary<int, string> DrugPerPage = new Dictionary<int, string>();
        private Dictionary<int, string> RefillsPerPage = new Dictionary<int, string>();

        const string PJL_PRINTER_NAME = "@PJL COMMENT PRINTER";
        const string PJL_PATIENT_NAME = "@PJL COMMENT PATIENT";
        const string PJL_PAGE = "@PJL COMMENT COMMENT PAGE";
        const string PJL_DRUG = "@PJL COMMENT DRUGNAME";
        const string PJL_REFILLS = "@PJL COMMENT REFILLS";


        public int PrintJobReceived()
        {
            try
            {
                //Create an instance of the custom exception
                PortMonCustomException PortMonException;


                const string REGISTRY_PATH_LOCATION = "System\\CurrentControlSet\\Control\\Print\\Monitors\\TroySecurePortMonitor";
                Microsoft.Win32.RegistryKey registryKey = Microsoft.Win32.Registry.LocalMachine;
                Microsoft.Win32.RegistryKey pmKey = registryKey.OpenSubKey(REGISTRY_PATH_LOCATION, false);
                serviceConfigFilePath = pmKey.GetValue("MainConfigurationPath").ToString();

                /* JLD - crypto licensing */
                LicenseQueryResult lqr;
                if (Licensing.IsValidLicense(out lqr) && lqr.CurrentLicenseStatus == LicenseStatus.Trial)
                {
                    InsertedDemoTroymark = true;
                }

                //Verify that the file triggering the event exists and then free the file info pointer
                printJobFileInfo = new FileInfo(printFileName);
                if (printJobFileInfo.Exists == false)
                {
                    PortMonException = new PortMonCustomException("Error opening the file from Port Monitor. " + printFileName, true, EventLogEntryType.Error, true);
                    throw PortMonException;
                }
                //Create a file name for the new temp file
                tempFileName = printJobFileInfo.FullName.Replace(printJobFileInfo.Extension, ".bak");


                PantographDataFilesLocation = printJobFileInfo.DirectoryName;
                if (!PantographDataFilesLocation.EndsWith("\\")) PantographDataFilesLocation += "\\";
                PantographDataFilesLocation += "Data\\";

                if (!FileRead(printFileName, pmConfig.FileReadAttempts, true, out inputBuffer))
                {
                    pmLogging.LogError("Max attempts exceeded to read: " + printerFileName + " Printer Name: " + printToPrinterName, EventLogEntryType.Error, true);
                    return 999;
                }

                printJobFileInfo.Refresh();

                if (printJobFileInfo.Length == 0)
                {
                    SentToPrinter = true;
                    return 101;
                }

                if (pmConfig.MaxPassThroughSizeInKb > 0)
                {
                    if (printJobFileInfo.Length > (pmConfig.MaxPassThroughSizeInKb * 1000))
                    {
                        printerFileName = printFileName;
                        if (!SendJobToPrinter(printToPrinterName, printerFileName, "TROY Pass Through Max Size"))
                        {
                            pmLogging.LogError("Print failed.  filename: " + printerFileName + " Printer Name: " + printToPrinterName, EventLogEntryType.Error, false);
                        }
                        SentToPrinter = true;
                        return 102;
                    }
                }

                fileEventPoints = new List<EventPoints>();
                insertStringList = new List<InsertStrings>();

                if (pmConfig.PassThroughJobsWithNoPjl)
                {
                    if (!IsPclJob())
                    {
                        printerFileName = printFileName;
                        if (!SendJobToPrinter(pmConfig.DefaultPrinter, printerFileName, "TROY Pass Through"))
                        {
                            pmLogging.LogError("Print failed.  filename: " + printerFileName + " Printer Name: " + pmConfig.DefaultPrinter, EventLogEntryType.Error, false);
                        }
                        SentToPrinter = true;
                        return 103;
                    }
                }
                
                //Find the end of the PJL and beginning of the PCL
                EndPjlLocation = FindPjlEnterPcl();
                if (EndPjlLocation < 0)
                {
                    if (!insertPjl)
                    {
                        PortMonException = new PortMonCustomException("File Does Not Contain PJL String Enter PCL. File: " + printFileName, true, EventLogEntryType.Error, true);
                        throw PortMonException;
                    }
                }

                FindPjlJobName();

                //Main change for Forerun
                if (pmConfig.EnablePjlTmDataParse)
                {
                    if (!AnalyzePjlHeader())
                    {
                        printerFileName = printFileName;
                        if (!SendJobToPrinter(printToPrinterName, printerFileName, "TROY Pass Through"))
                        {
                            pmLogging.LogError("Print failed.  filename: " + printerFileName + " Printer Name: " + printToPrinterName, EventLogEntryType.Error, false);
                        }
                        SentToPrinter = true;
                        return 104;
                    }
                    printToPrinterName = PrinterFromPJL;
                }

                if (dataCapFlags.EnablePjlHeaderCapture)
                    EvaluatePjlDataCapture();

                if (pmConfig.EndOfPageString != "")
                    EndOfPageString = new UTF8Encoding(true).GetBytes(pmConfig.EndOfPageString);
                else
                    EndOfPageString = DefaultEndOfPage;

                if (pmConfig.AltEndOfPageString != "")
                    AltEndOfPageString = new UTF8Encoding(true).GetBytes(pmConfig.AltEndOfPageString);

                if (pmConfig.InsertPointPclString != "")
                    InsertPointString = new UTF8Encoding(true).GetBytes(pmConfig.InsertPointPclString);
                else
                    InsertPointString = DefaultInsertPoint;

                if (pmConfig.RemoveEscString != "")
                    RemoveEscStringBytes = new UTF8Encoding(true).GetBytes(pmConfig.RemoveEscString);

                //Read the file Input Buffer and find the beginning and end of pages and the fonts
                if (!ReadInputBuffer())
                {
                    //Do not log this but throw the error
                    PortMonException = new PortMonCustomException("Error in ReadInputBuffer()", false, EventLogEntryType.Error, true);
                    throw PortMonException;
                }

                //Check for a dynamic printer name
                if (dataCapFlags.DynamicPrinterConfig)
                {
                    string temp = FindDynamicPrintingString();
                    if (temp != "")
                        printToPrinterName = temp;
                }

                if (printToPrinterName.Length < 1)
                {
                    if (pmConfig.DefaultPrinter.Length < 1)
                    {
                        //Default printer not defined. End the job.
                        PortMonException = new PortMonCustomException("A printer is not defined for this job.  Default printer not found. File: " + printFileName, true, EventLogEntryType.Error, true);
                        throw PortMonException;
                    }
                    else
                    {
                        printToPrinterName = pmConfig.DefaultPrinter;
                    }
                }


                if (pmConfig.UseConfigurableDataCapture)
                {
                    //Check for pass through
                    if ((pmConfig.PassThroughEnabled) && (pmConfig.PassThroughList.Count > 0))
                    {
                        if (!CheckForPassThrough())
                        {
                            printerFileName = printFileName;
                            if (!SendJobToPrinter(printToPrinterName, printerFileName, "TROY Pass Through"))
                            {
                                pmLogging.LogError("Print failed.  filename: " + printerFileName + " Printer Name: " + printToPrinterName, EventLogEntryType.Error, false);
                            }
                            SentToPrinter = true;
                            return 105;
                        }
                    }

                    //Analyze the TroyMark data
                    TroyMarkDataCaptureAnalysis();
                    MpDataCaptureAnalysis();
                }
                else if (pmConfig.UseSecureRxSetup)
                {
                    if ((pmConfig.PassThroughEnabled) && (pmConfig.PassThroughList.Count > 0))
                    {
                        if (SecureRxPassThrough())
                        {
                            printerFileName = printFileName;
                            if (!SendJobToPrinter(printToPrinterName, printerFileName, "TROY Pass Through"))
                            {
                                pmLogging.LogError("Print failed.  filename: " + printerFileName + " Printer Name: " + printToPrinterName, EventLogEntryType.Error, false);
                            }
                            SentToPrinter = true;
                            return 106;
                        }
                    }
                    SecureRxDataCapture();
                }

                //For Version 1.0.13
                if ((pmConfig.TroyMarkUseFirstData) && (TroyMarkDataPerPage.Count > 0))
                {
                    if (TroyMarkDataPerPage.ContainsKey(1))
                    {
                        if (!TroyMarkDataPerPage.ContainsKey(0))
                        {
                            TroyMarkDataPerPage.Add(0, "");
                        }
                        TroyMarkDataPerPage[0] += TroyMarkDataPerPage[1];
                    }
                }


                //Change for 1.0.10 (Mount Nittany request)
                if ((pmConfig.ExclusionAreaString != null) && (pmConfig.ExclusionAreaString != ""))
                {
                    ExclusionAreaCountPerPage();
                }

                //If the print job does not include a printer name then get the default
                if (printToPrinterName.Length < 1)
                {
                    if (pmConfig.DefaultPrinter.Length < 1)
                    {
                        //Default printer not defined. End the job.
                        PortMonException = new PortMonCustomException("A printer is not defined for this job.  Default printer not found. File: " + printFileName, true, EventLogEntryType.Error, true);
                        throw PortMonException;
                    }
                    else
                    {
                        printToPrinterName = pmConfig.DefaultPrinter;
                    }
                }

                //Write out the data to a new file
                if (!WriteOutPcl())
                {
                    //Errors will be logged in the function
                    PortMonException = new PortMonCustomException("Error in WriteOutPcl()", false, EventLogEntryType.Error, true);
                    throw PortMonException;
                }

                //Print the new file
                if (!PrintTheJob(out lqr))
                {
                    //Errors will be logged in the function
                    PortMonException = new PortMonCustomException("Error in PrintTheJob()", false, EventLogEntryType.Error, true);
                    throw PortMonException;
                }
                SentToPrinter = true;
            }
            //Catch custom errors
            catch (PortMonCustomException pme)
            {
                pmLogging.LogError(pme.Message.ToString(), pme.EventType, pme.FatalError);
            }
            //Catch other errors
            catch (Exception ex)
            {
                pmLogging.LogError("Error in PrintJobReceived(). File: " + printFileName + " Error: " + ex.Message, EventLogEntryType.Error, false);
            }
            finally
            {
                // if all else fails, then print original print stream as pass through
                if (!SentToPrinter)
                {
                    if (FileRead(printFileName, pmConfig.FileReadAttempts, false, out inputBuffer))
                    {
                        if (printToPrinterName.Length > 1)
                        {
                            printerFileName = printFileName;
                            if (!SendJobToPrinter(printToPrinterName, printerFileName, "TROY Port Monitor Pass Through"))
                            {
                                pmLogging.LogError("Print failed.  filename: " + printerFileName + " Printer Name: " + printToPrinterName, EventLogEntryType.Error, false);
                            }
                            SentToPrinter = true;
                        }
                    }
                }
                FileCleanup();
            }
            return 0;
        }
        protected virtual bool FileRead(string filename, int maxAttempts, bool fillBuffer, out byte[] buffer)
        {
            FileStream fs = null;
            buffer = null;
            bool result = false;

            for (int attempts = 0; attempts < maxAttempts;  attempts++)
            {
                try
                {
                    fs = File.Open(filename, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
                    if (fillBuffer) buffer = new BinaryReader(fs).ReadBytes(Convert.ToInt32(fs.Length));
                    result = true;
                    break;
                }
                catch (IOException)
                {
                    // file is unavailable; try again after delay
                    if (fs != null) fs.Close();  // don't wait to do this in finally after sleep
                    pmLogging.LogError(String.Format("Input file read failed for {0} on attempt {1} retry in {2} ms", filename, attempts, pmConfig.FileReadDelay_ms), EventLogEntryType.Information, false);
                    Thread.Sleep(pmConfig.FileReadDelay_ms);
                    continue;
                }
                finally
                {
                    if (fs != null) fs.Close();
                }
            }

            return result;
        }
        private bool IsPclJob()
        {
            //Search the first few bytes for a PJL string
            if (inputBuffer.Length < 8)
            {
                return false;
            }

            if ((inputBuffer[0] == 0x1B) && (inputBuffer[1] == 0x25) &&
                (inputBuffer[2] == 0x2D) && (inputBuffer[3] == 0x31) &&
                (inputBuffer[4] == 0x32) && (inputBuffer[5] == 0x33) &&
                (inputBuffer[6] == 0x34) && (inputBuffer[7] == 0x35) &&
                (inputBuffer[8] == 0x58))
            {
                return true;
            }
            return false;
        }


        private bool AnalyzePjlHeader()
        {
            try
            {
                string PjlString = new UTF8Encoding(true).GetString(inputBuffer, 0, EndPjlLocation);
                if (!PjlString.Contains(PJL_PRINTER_NAME))
                {
                    return false;
                }
                int CurrentPage = 1;

                PjlString = PjlString.Replace("\u000D", "");

                foreach (string pjl in PjlString.Split('\u000A'))
                {
                    int indexeq = pjl.IndexOf("=");
                    if (indexeq > 0)
                    {
                        //Skip over the first "
                        string data = pjl.Substring(indexeq + 2).TrimEnd('"');
                        switch (pjl.Substring(0, indexeq))
                        {
                            case PJL_PRINTER_NAME:
                                PrinterFromPJL = data;
                                break;
                            case PJL_PATIENT_NAME:
                                PatientNameFromPJL = data.Trim();
                                break;
                            case PJL_PAGE:
                                CurrentPage = int.Parse(data);
                                break;
                            case PJL_DRUG:
                                DrugPerPage.Add(CurrentPage, data.Trim());
                                break;
                            case PJL_REFILLS:
                                RefillsPerPage.Add(CurrentPage, data.Trim());
                                break;
                        }
                    }
                }
                if (PrinterFromPJL == "")
                {
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                pmLogging.LogError("Error Parsing PJL Header. Error: " + ex.Message, EventLogEntryType.Error, false);
                return false;
            }
        }

        private void FindPjlJobName()
        {
            try
            {
                string PjlString = new UTF8Encoding(true).GetString(inputBuffer, 0, EndPjlLocation);

                int index = PjlString.IndexOf("@PJL JOB NAME=");
                if (index > -1)
                {
                    int LfIndex = PjlString.IndexOf("\u000A", index + 1);
                    if (LfIndex > -1)
                    {
                        PrintJobName = PjlString.Substring(index + 15, LfIndex - (index + 16));
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
        

        private bool SendJobToPrinter(string PrinterName, string Filename, string Source)
        {
            try
            {
                PrinterLib.PrintToSpooler.SendFileToPrinter(PrinterName, Filename, Source);
                return true;
            }
            catch (Exception ex)
            {
                pmLogging.LogError("Error in SendJobToPrinter.  Error: " + ex.Message, EventLogEntryType.Error, false);
                return false;
            }

        }

        private bool SecureRxPassThrough()
        {
            if (pmConfig.SaveFontDataAsTokens)
            {
                foreach (TaggedListPerPage tlpp in TaggedListPageList)
                {
                    if (!PassJobThroughTokens(tlpp.TaggedList))
                    {
                        return false;
                    }
                }
                return true;
            }
            else
            {
                if (!pmConfig.PlainTextData)
                {
                    if (!PassThroughStringList())
                    {
                        return false;
                    }
                }
                else
                {
                    if (!PassThroughPlainText())
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private void SecureRxDataCapture()
        {
            try
            {
                if (pmConfig.SaveFontDataAsTokens)
                {
                    int pgCntr = 1;
                    foreach (TaggedListPerPage tlpp in TaggedListPageList)
                    {
                        //Get Patient Name
                        string patientName = "";
                        int indexPN = tlpp.TaggedList.IndexOf(pmConfig.PatientNameTag);
                        if ((indexPN > -1) && ((indexPN + 1) < tlpp.TaggedList.Count))
                        {
                            patientName = tlpp.TaggedList[indexPN + 1];
                        }
                        else
                        {
                            foreach (string td in tlpp.TaggedList)
                            {
                                if (td.Contains(pmConfig.PatientNameTag))
                                {
                                    indexPN = td.IndexOf(pmConfig.PatientNameTag);
                                    indexPN += pmConfig.PatientNameTag.Length;
                                    patientName = td.Substring(indexPN);
                                    break;
                                }
                            }
                        }
                        if (patientName != "")
                        {
                            if (TroyMarkDataPerPage.ContainsKey(pgCntr))
                            {
                                TroyMarkDataPerPage[pgCntr] += patientName;
                            }
                            else
                            {
                                TroyMarkDataPerPage.Add(pgCntr, patientName);
                            }
                        }
                        //Get drug names/refills
                        int indexDN = 0, holdIndexDN = -1;
                        string drugName = "", refillAmount = "", customTag = "";
                        foreach (string td in tlpp.TaggedList)
                        {
                            holdIndexDN++;
                            if ((pmConfig.DrugNameTag != "") && (td.Contains(pmConfig.DrugNameTag)))
                            {
                                indexDN = td.IndexOf(pmConfig.DrugNameTag);
                                if (indexDN + pmConfig.DrugNameTag.Length < td.Length)
                                {
                                    drugName = td.Substring(indexDN + pmConfig.DrugNameTag.Length);
                                }
                                else
                                {
                                    indexDN = tlpp.TaggedList.IndexOf(pmConfig.DrugNameTag, holdIndexDN);
                                    drugName = tlpp.TaggedList[indexDN + 1];
                                }

                                if (drugName != "")
                                {
                                    if (TroyMarkDataPerPage.ContainsKey(pgCntr))
                                    {
                                        TroyMarkDataPerPage[pgCntr] += " " + drugName;
                                    }
                                    else
                                    {
                                        TroyMarkDataPerPage.Add(pgCntr, drugName);
                                    }
                                }
                            }
                            else if ((pmConfig.RefillsTag != "") && (td.Contains(pmConfig.RefillsTag)))
                            {
                                indexDN = td.IndexOf(pmConfig.RefillsTag);
                                if (indexDN + pmConfig.RefillsTag.Length < td.Length)
                                {
                                    refillAmount = " Refills: " + td.Substring(indexDN + pmConfig.RefillsTag.Length);
                                }
                                else
                                {
                                    indexDN = tlpp.TaggedList.IndexOf(pmConfig.RefillsTag, holdIndexDN);
                                    refillAmount = " Refills: " + tlpp.TaggedList[indexDN + 1];
                                }
                                if (refillAmount != "")
                                {
                                    if (TroyMarkDataPerPage.ContainsKey(pgCntr))
                                    {
                                        TroyMarkDataPerPage[pgCntr] += " " + refillAmount;
                                    }
                                    else
                                    {
                                        TroyMarkDataPerPage.Add(pgCntr, refillAmount);
                                    }
                                }
                            }
                            else if ((pmConfig.CustomTag != "") && (td.Contains(pmConfig.CustomTag)))
                            {
                                indexDN = td.IndexOf(pmConfig.CustomTag);
                                if (indexDN + pmConfig.CustomTag.Length < td.Length)
                                {
                                    customTag = " " + td.Substring(indexDN + pmConfig.CustomTag.Length);
                                }
                                else
                                {
                                    indexDN = tlpp.TaggedList.IndexOf(pmConfig.CustomTag, holdIndexDN);
                                    customTag = " " + tlpp.TaggedList[indexDN + 1];
                                }
                                if (customTag != "")
                                {
                                    if (TroyMarkDataPerPage.ContainsKey(pgCntr))
                                    {
                                        TroyMarkDataPerPage[pgCntr] += " " + customTag;
                                    }
                                    else
                                    {
                                        TroyMarkDataPerPage.Add(pgCntr, customTag);
                                    }
                                }
                            }
                        }
                        pgCntr++;

                    }
                }
                else
                {
                    string patientName = "";
                    int indexPN;
                    int endPN = -1, holdEndPN = int.MaxValue;

                    Dictionary<int, string> GenericPerPage;
                    if (!pmConfig.PlainTextData)
                    {
                        GenericPerPage = CapturedDataPerPage;
                    }
                    else
                    {
                        GenericPerPage = PlainTextPerPage;
                    }

                    foreach (KeyValuePair<int, string> kv in GenericPerPage)
                    {
                        indexPN = kv.Value.IndexOf(pmConfig.PatientNameTag);
                        if (indexPN > -1)
                        {
                            indexPN = indexPN + pmConfig.PatientNameTag.Length;
                            foreach (string msgstr in pmConfig.PatientNameEndList.Split(','))
                            {
                                endPN = kv.Value.IndexOf(msgstr, indexPN);
                                if ((endPN > -1) && (endPN < holdEndPN))
                                {
                                    holdEndPN = endPN;
                                }
                            }
                            if (holdEndPN != int.MaxValue)
                            {
                                patientName = kv.Value.Substring(indexPN, holdEndPN - indexPN);
                                patientName = patientName.TrimEnd(' ');
                            }
                        }
                        if (patientName != "")
                        {
                            if (TroyMarkDataPerPage.ContainsKey(kv.Key))
                            {
                                TroyMarkDataPerPage[kv.Key] += patientName;
                            }
                            else
                            {
                                TroyMarkDataPerPage.Add(kv.Key, patientName);
                            }
                        }

                        string drugName = "", refillAmount = "", customTag = "";
                        int endDN = -1, holdEndDN = int.MaxValue;
                        int endRF = -1, holdEndRF = int.MaxValue;
                        int endCT = -1, holdEndCT = int.MaxValue;

                        int holdIndexDN, nextIndexDN;
                        int indexDN = kv.Value.IndexOf(pmConfig.DrugNameTag);
                        if (indexDN > -1)
                        {
                            int indexRF, indexCT;
                            indexDN += pmConfig.DrugNameTag.Length;
                            foreach (string msgstr in pmConfig.DrugNameEndList.Split(','))
                            {
                                endDN = kv.Value.IndexOf(msgstr, indexDN);
                                if ((endDN > -1) && (endDN < holdEndDN))
                                {
                                    holdEndDN = endDN;
                                }
                            }
                            if (holdEndDN != int.MaxValue)
                            {
                                drugName = kv.Value.Substring(indexDN, holdEndDN - indexDN);
                                drugName = drugName.TrimEnd(' ');
                                if (drugName != "")
                                {
                                    if (TroyMarkDataPerPage.ContainsKey(kv.Key))
                                    {
                                        TroyMarkDataPerPage[kv.Key] += " " + drugName;
                                    }
                                    else
                                    {
                                        TroyMarkDataPerPage.Add(kv.Key, drugName);
                                    }
                                }
                            }

                            holdIndexDN = indexDN;
                            indexDN = kv.Value.IndexOf(pmConfig.DrugNameTag, holdIndexDN);
                            if (indexDN > -1)
                            {
                                nextIndexDN = indexDN;
                            }
                            else
                            {
                                nextIndexDN = int.MaxValue;
                            }

                            if (pmConfig.RefillsTag != "")
                            {
                                indexRF = kv.Value.IndexOf(pmConfig.RefillsTag, holdIndexDN);
                                if ((indexRF > -1) && (indexRF < nextIndexDN))
                                {
                                    indexRF += pmConfig.RefillsTag.Length;
                                    foreach (string msgstr in pmConfig.RefillsTagEndList.Split(','))
                                    {
                                        endRF = kv.Value.IndexOf(msgstr, indexRF);
                                        if ((endRF > -1) && (endRF < holdEndRF))
                                        {
                                            holdEndRF = endRF;
                                        }
                                    }
                                    if (holdEndRF != int.MaxValue)
                                    {
                                        refillAmount = pmConfig.RefillsTag + kv.Value.Substring(indexRF, holdEndRF - indexRF);
                                        refillAmount = refillAmount.TrimEnd(' ');
                                        if (refillAmount != "")
                                        {
                                            if (TroyMarkDataPerPage.ContainsKey(kv.Key))
                                            {
                                                TroyMarkDataPerPage[kv.Key] += " " + refillAmount;
                                            }
                                            else
                                            {
                                                TroyMarkDataPerPage.Add(kv.Key, refillAmount);
                                            }
                                        }
                                    }
                                }
                            }
                            if (pmConfig.CustomTag != "")
                            {
                                indexCT = kv.Value.IndexOf(pmConfig.CustomTag, holdIndexDN);
                                if ((indexCT > -1) && (indexCT < nextIndexDN))
                                {
                                    indexCT += pmConfig.CustomTag.Length;
                                    foreach (string msgstr in pmConfig.CustomTagEndList.Split(','))
                                    {
                                        endCT = kv.Value.IndexOf(msgstr, indexCT);
                                        if ((endCT > -1) && (endCT < holdEndCT))
                                        {
                                            holdEndCT = endCT;
                                        }
                                    }
                                    if (holdEndCT != int.MaxValue)
                                    {
                                        customTag = pmConfig.CustomTag + kv.Value.Substring(indexCT, holdEndCT - indexCT);
                                        customTag = customTag.TrimEnd(' ');
                                        if (customTag != "")
                                        {
                                            if (TroyMarkDataPerPage.ContainsKey(kv.Key))
                                            {
                                                TroyMarkDataPerPage[kv.Key] += " " + customTag;
                                            }
                                            else
                                            {
                                                TroyMarkDataPerPage.Add(kv.Key, customTag);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                pmLogging.LogError("Error in SecureRxDataCapture. Error: " + ex.Message, EventLogEntryType.Error, false);

            }
        }

        private bool PassJobThroughTokens(List<string> tokenList)
        {
            try
            {
                int foundCntr = 0;
                foreach (string pt in pmConfig.PassThroughList)
                {
                    foreach (string tagged in tokenList)
                    {
                        if (tagged.Contains(pt))
                        {
                            if (!pmConfig.PassThroughStringMatchAllFlag)
                            {
                                return false;
                            }
                            foundCntr++;
                            break;
                        }
                    }
                }
                if (foundCntr >= pmConfig.PassThroughList.Count)
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                pmLogging.LogError("Error in PassJobThroughTokens. Error: " + ex.Message, EventLogEntryType.Error, false);
                //Pass through errors
                return true;
            }
        }


        private bool PassThroughStringList()
        {
            try
            {
                int foundCntr = 0;
                foreach (string pt in pmConfig.PassThroughList)
                {
                    foreach (KeyValuePair<int, string> kv in CapturedDataPerPage)
                    {
                        if (kv.Value.Contains(pt))
                        {
                            if (!pmConfig.PassThroughStringMatchAllFlag)
                            {
                                return false;
                            }
                            foundCntr++;
                            break;
                        }
                    }
                }
                if (foundCntr >= pmConfig.PassThroughList.Count)
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private bool PassThroughPlainText()
        {
            try
            {
                int foundCntr = 0;
                foreach (string pt in pmConfig.PassThroughList)
                {
                    foreach (KeyValuePair<int, string> kv in PlainTextPerPage)
                    {
                        if (kv.Value.Contains(pt))
                        {
                            if (!pmConfig.PassThroughStringMatchAllFlag)
                            {
                                return false;
                            }
                            foundCntr++;
                            break;
                        }
                    }
                }
                if (foundCntr >= pmConfig.PassThroughList.Count)
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private bool CheckForPassThrough()
        {
            try
            {
                int found = 0;
                foreach (TroyDataCaptureConfiguration dcc in dCapConfig.DataCaptureConfigurationList)
                {
                    if (dcc.DataUse == DataUseType.PassThrough)
                    {
                        switch (dcc.DataCapture)
                        {
                            case DataCaptureType.PjlHeader:
                                if (PassThroughPerPage.ContainsKey(0))
                                {
                                    foreach (string ptstr in pmConfig.PassThroughList)
                                    {
                                        if (PassThroughPerPage[0].Contains(ptstr))
                                        {
                                            if (!pmConfig.PassThroughStringMatchAllFlag)
                                            {
                                                return true;
                                            }
                                            found++;
                                        }
                                        else
                                        {
                                            if (pmConfig.PassThroughStringMatchAllFlag)
                                            {
                                                break;
                                            }
                                        }
                                    }
                                    if (found == pmConfig.PassThroughList.Count)
                                    {
                                        return true;
                                    }
                                }
                                break;

                            case DataCaptureType.PlainText:
                                foreach (KeyValuePair<int, string> kv in PlainTextPerPage)
                                {
                                    foreach (string ptstr in pmConfig.PassThroughList)
                                    {
                                        if (kv.Value.Contains(ptstr))
                                        {
                                            if (!pmConfig.PassThroughStringMatchAllFlag)
                                            {
                                                return true;
                                            }
                                            found++;
                                        }
                                        else
                                        {
                                            if (pmConfig.PassThroughStringMatchAllFlag)
                                            {
                                                break;
                                            }

                                        }
                                    }
                                    if (found == pmConfig.PassThroughList.Count)
                                    {
                                        return true;
                                    }
                                }
                                break;

                            case DataCaptureType.StandardFonts:
                                foreach (KeyValuePair<int, string> kv in PassThroughPerPage)
                                {
                                    if (kv.Key != 0)
                                    {
                                        foreach (string ptstr in pmConfig.PassThroughList)
                                        {
                                            if (kv.Value.Contains(ptstr))
                                            {
                                                if (!pmConfig.PassThroughStringMatchAllFlag)
                                                {
                                                    return true;
                                                }
                                                found++;
                                            }
                                            else
                                            {
                                                if (pmConfig.PassThroughStringMatchAllFlag)
                                                {
                                                    break;
                                                }
                                            }
                                        }
                                        if (found == pmConfig.PassThroughList.Count)
                                        {
                                            return true;
                                        }
                                    }
                                }
                                break;

                            case DataCaptureType.TroyFonts:
                                foreach (KeyValuePair<int, string> kv in TroyFontDataPerPage)
                                {
                                    foreach (string ptstr in pmConfig.PassThroughList)
                                    {
                                        if (kv.Value.Contains(ptstr))
                                        {
                                            if (!pmConfig.PassThroughStringMatchAllFlag)
                                            {
                                                return true;
                                            }
                                            found++;
                                        }
                                        else
                                        {
                                            if (pmConfig.PassThroughStringMatchAllFlag)
                                            {
                                                break;
                                            }
                                        }
                                    }
                                    if (found == pmConfig.PassThroughList.Count)
                                    {
                                        return true;
                                    }
                                }
                                break;
                        }
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                pmLogging.LogError("Error in CheckForPassThrough.  Error: " + ex.Message, EventLogEntryType.Error, false);
                return false;
            }

        }



        private void EvaluatePjlDataCapture()
        {
            string PjlString = new UTF8Encoding(true).GetString(inputBuffer, 0, EndPjlLocation);

            foreach (TroyDataCaptureConfiguration dcc in dCapConfig.DataCaptureConfigurationList)
            {
                if (dcc.DataCapture == DataCaptureType.PjlHeader)
                {
                    foreach (DataTagsType dtt in dcc.DataTags)
                    {
                        if (dtt.LeadingTag != "")
                        {
                            dtt.LeadingTag = dtt.LeadingTag.Replace("/e", "\u001B");
                            dtt.LeadingTag = dtt.LeadingTag.Replace("/r", "\u000D");
                            dtt.LeadingTag = dtt.LeadingTag.Replace("/n", "\u000A");
                            dtt.TrailingTag = dtt.TrailingTag.Replace("/e", "\u001B");
                            dtt.TrailingTag = dtt.TrailingTag.Replace("/r", "\u000D");
                            dtt.TrailingTag = dtt.TrailingTag.Replace("/n", "\u000A");

                            int index = PjlString.IndexOf(dtt.LeadingTag);
                            while (index > -1)
                            {
                                index += +dtt.LeadingTag.Length;
                                int index2 = PjlString.IndexOf(dtt.TrailingTag, index);
                                if ((index2 > -1) && (index2 > index))
                                {
                                    string capturedData = PjlString.Substring(index, index2 - index);
                                    capturedData = capturedData.TrimStart('"');
                                    capturedData = capturedData.TrimEnd('"');
                                    switch (dcc.DataUse)
                                    {
                                        case DataUseType.PassThrough:
                                            if (PassThroughPerPage.ContainsKey(0))
                                            {
                                                PassThroughPerPage[0] += " " + capturedData;
                                            }
                                            else
                                            {
                                                PassThroughPerPage.Add(0, capturedData);
                                            }
                                            break;
                                        case DataUseType.PrinterMap:
                                            if (PrinterMapPerPage.ContainsKey(0))
                                            {
                                                PrinterMapPerPage[0] = capturedData;
                                            }
                                            else
                                            {
                                                PrinterMapPerPage.Add(0, capturedData);
                                            }
                                            break;
                                        case DataUseType.TroyMark:
                                            if (TroyMarkDataPerPage.ContainsKey(0))
                                            {
                                                TroyMarkDataPerPage[0] += " " + capturedData;
                                            }
                                            else
                                            {
                                                TroyMarkDataPerPage.Add(0, capturedData);
                                            }
                                            break;
                                    }


                                }
                                if (!dtt.OnePerPage)
                                {
                                    index = PjlString.IndexOf(dtt.LeadingTag, index2);
                                }
                                else
                                {
                                    index = -1;
                                }
                            }

                        }
                    }
                }
            }

        }


        private void FindTaggedPrinterString(List<DataTagsType> dttList, Dictionary<int, string> datalist)
        {
            try
            {
                foreach (DataTagsType dtt in dttList)
                {
                    foreach (KeyValuePair<int, string> kv in datalist)
                    {
                        if (kv.Value != "")
                        {
                            string temp = kv.Value;
                            int index1 = temp.IndexOf(dtt.LeadingTag);
                            if (index1 > -1)
                            {
                                if (!dtt.IncludeLeadingTag)
                                {
                                    index1 += dtt.LeadingTag.Length;
                                }
                                int index2 = temp.IndexOf(dtt.TrailingTag, index1 + 1);
                                if (index2 > index1)
                                {
                                    string data = temp.Substring(index1, index2 - index1);
                                    data = data.TrimStart(' ');
                                    data = data.TrimEnd(' ');
                                    PrinterMapStringList.Add(data);
                                }
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                pmLogging.LogError("Error in FindTaggedPrinterString. Error: " + ex.Message, EventLogEntryType.Error, false);
            }
        }

        private string FindDynamicPrintingString()
        {
            try
            {
                bool SearchTroyFonts = false;
                //First look for the tagged data in font data and plain text.
                foreach (TroyDataCaptureConfiguration dcc in dCapConfig.DataCaptureConfigurationList)
                {
                    if (dcc.DataUse == DataUseType.PrinterMap)
                    {
                        if (dcc.DataCapture == DataCaptureType.StandardFonts)
                        {
                            if (!dcc.UseAllData)
                            {
                                FindTaggedPrinterString(dcc.DataTags, PrinterMapPerPage);
                            }
                            else
                            {
                                foreach (KeyValuePair<int, string> kv in PrinterMapPerPage)
                                {
                                    if (!PrinterMapStringList.Contains(kv.Value))
                                    {
                                        PrinterMapStringList.Add(kv.Value);
                                    }
                                }
                            }
                        }
                        else if (dcc.DataCapture == DataCaptureType.PlainText)
                        {
                            if (!dcc.UseAllData)
                            {
                                FindTaggedPrinterString(dcc.DataTags, PlainTextPerPage);
                            }
                        }
                        else if (dcc.DataCapture == DataCaptureType.TroyFonts)
                        {
                            SearchTroyFonts = true;
                        }
                    }
                }


                //Check PJL First
                if (PrinterMapPerPage.ContainsKey(0))
                {
                    if (pmConfig.UsePrinterMap)
                    {
                        string pname = GetPrinterNameFromMap(PrinterMapPerPage[0]);
                        if (pname != "")
                        {
                            return pname;
                        }
                    }
                    else
                    {
                        if (PrinterMapPerPage[0] != "")
                        {
                            return PrinterMapPerPage[0];
                        }
                    }
                }

                //Check font
                foreach (string str in PrinterMapStringList)
                {
                    if (pmConfig.UsePrinterMap)
                    {
                        string pname = GetPrinterNameFromMap(str);
                        if (pname != "")
                        {
                            return pname;
                        }
                    }
                    else
                    {
                        if (str != "")
                        {
                            return str;
                        }
                    }
                }

                //Troy font
                if (SearchTroyFonts)
                {
                    foreach (KeyValuePair<int, string> kv in TroyFontDataPerPage)
                    {
                        if (kv.Value != "")
                        {
                            if (pmConfig.UsePrinterMap)
                            {
                                string pname = GetPrinterNameFromMap(kv.Value);
                                if (pname != "")
                                {
                                    return pname;
                                }
                            }
                            else
                            {
                                return kv.Value;
                            }
                        }
                    }
                }

                return "";
            }
            catch (Exception ex)
            {
                pmLogging.LogError("Error in CheckPrinterMap. Error: " + ex.Message, EventLogEntryType.Error, false);
                return "";
            }
        }

        private void FindTaggedMpData(List<DataTagsType> dttList, Dictionary<int, string> datalist)
         {
             try
             {
                 foreach (DataTagsType dtt in dttList)
                 {
                     foreach (KeyValuePair<int, string> kv in datalist)
                     {
                         if (kv.Value != "")
                         {
                             string temp = kv.Value;
                             int index1 = temp.IndexOf(dtt.LeadingTag);
                             while (index1 > -1)
                             {
                                 int index2 = temp.IndexOf(dtt.TrailingTag, index1 + 1);
                                 if (index2 > index1)
                                 {
                                     if (!dtt.IncludeLeadingTag)
                                     {
                                         index1 += dtt.LeadingTag.Length;
                                     }
                                     string data = temp.Substring(index1, index2 - index1);
                                     data = data.TrimStart(' ');
                                     data = data.TrimEnd(' ');
                                     data = dtt.LeadingText + data + dtt.TrailingText;
                                     if (MpDataPerPage.ContainsKey(kv.Key))
                                     {
                                         MpDataPerPage[kv.Key] += data;
                                     }
                                     else
                                     {
                                         MpDataPerPage.Add(kv.Key, data);
                                     }
                                     index1 = temp.IndexOf(dtt.LeadingTag, index2);
                                 }
                                 else
                                 {
                                     index1 = temp.IndexOf(dtt.LeadingTag, index1 + 1);
                                 }
                                 if (dtt.OnePerPage)
                                 {
                                     index1 = -1;
                                 }
                             }
                         }
                     }
                 }
             }
             catch (Exception ex)
             {
                 pmLogging.LogError("Error in FindTaggedMpDataStandard.  Error: " + ex.Message, EventLogEntryType.Error, false);
             }
         }

        private string GetPrinterNameFromMap(string MapString)
        {
            try
            {
                foreach (PrinterMapType pmt in printerMap.PrinterMap)
                {
                    if (pmt.MapString == MapString)
                    {
                        return pmt.PrinterQueueName;
                    }
                }
                return "";
            }
            catch (Exception ex)
            {
                pmLogging.LogError("Error in GetPrinterNameFromMap. Error: " + ex.Message, EventLogEntryType.Error, false);
                return "";
            }


        }

        private void FindTaggedTroyMarkData(List<DataTagsType> dttList, Dictionary<int, string> datalist)
        {
            try
            {
                foreach (DataTagsType dtt in dttList)
                {
                    foreach (KeyValuePair<int, string> kv in datalist)
                    {
                        if (kv.Value != "")
                        {
                            string temp = kv.Value;
                            int index1 = temp.IndexOf(dtt.LeadingTag);
                            while (index1 > -1)
                            {
                                int index2 = temp.IndexOf(dtt.TrailingTag, index1 + 1);
                                if (index2 > index1)
                                {
                                    if (!dtt.IncludeLeadingTag)
                                    {
                                        index1 += dtt.LeadingTag.Length;
                                    }
                                    string data = temp.Substring(index1, index2 - index1);
                                    data = data.TrimStart(' ');
                                    data = data.TrimEnd(' ');
                                    data = dtt.LeadingText + data + dtt.TrailingText;
                                    if (TroyMarkDataPerPage.ContainsKey(kv.Key))
                                    {
                                        TroyMarkDataPerPage[kv.Key] += data;
                                    }
                                    else
                                    {
                                        TroyMarkDataPerPage.Add(kv.Key, data);
                                    }
                                    index1 = temp.IndexOf(dtt.LeadingTag, index2);
                                }
                                else
                                {
                                    index1 = temp.IndexOf(dtt.LeadingTag, index1 + 1);
                                }
                                if (dtt.OnePerPage)
                                {
                                    index1 = -1;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                pmLogging.LogError("Error in FindTaggedTroyMarkDataStandard.  Error: " + ex.Message, EventLogEntryType.Error, false);
            }
        }

        private void MpDataCaptureAnalysis()
         {
             try
             {
                  //First look for the tagged data in font data and plain text.
                 foreach (TroyDataCaptureConfiguration dcc in dCapConfig.DataCaptureConfigurationList)
                 {
                     if (dcc.DataUse == DataUseType.MicroPrint)
                     {
                         if (dcc.DataCapture == DataCaptureType.StandardFonts)
                         {
                             if (!dcc.UseAllData)
                             {
                                 FindTaggedMpData(dcc.DataTags, CapturedDataPerPage);
                             }
                             else
                             {
                                 foreach (KeyValuePair<int, string> kv in CapturedAllMpDataPerPage)
                                 {
                                     if (MpDataPerPage.ContainsKey(kv.Key))
                                     {
                                         MpDataPerPage[kv.Key] += kv.Value;
                                     }
                                     else
                                     {
                                         MpDataPerPage.Add(kv.Key, kv.Value);
                                     }
                                 }
 
                             }
                         }
                         else if (dcc.DataCapture == DataCaptureType.PlainText)
                         {
                             if (!dcc.UseAllData)
                             {
                                 FindTaggedMpData(dcc.DataTags, PlainTextPerPage);
                             }
                             else
                             {
                                 foreach (KeyValuePair<int, string> kv in PlainTextPerPage)
                                 {
                                     if (MpDataPerPage.ContainsKey(kv.Key))
                                     {
                                         MpDataPerPage[kv.Key] += kv.Value;
                                     }
                                     else
                                     {
                                         MpDataPerPage.Add(kv.Key, kv.Value);
                                     }
                                 }
                             }
                         }
                     }
                 }
 
             }
             catch (Exception ex)
             {
                 pmLogging.LogError("Error in DataCaptureAnalysis.  Error: " + ex.Message, EventLogEntryType.Error, false);
 
             }
         }

        private void ExclusionAreaCountPerPage()
        {
            ExclusionRegion.Clear();
            if (PlainTextPerPage.Count > 0)
            {
                foreach (KeyValuePair<int, string> kv in PlainTextPerPage)
                {
                    int index = kv.Value.IndexOf(pmConfig.ExclusionAreaString);
                    int cntr = 0;
                    while (index > -1)
                    {
                        cntr++;
                        index = kv.Value.IndexOf(pmConfig.ExclusionAreaString, index + 1);
                    }
                    if (!ExclusionRegion.ContainsKey(kv.Key))
                    {
                        ExclusionRegion.Add(kv.Key, cntr);
                    }
                }
            }
            else if (CapturedDataPerPage.Count > 0)
            {
                foreach (KeyValuePair<int, string> kv in CapturedDataPerPage)
                {
                    int index = kv.Value.IndexOf(pmConfig.ExclusionAreaString);
                    int cntr = 0;
                    while (index > -1)
                    {
                        cntr++;
                        index = kv.Value.IndexOf(pmConfig.ExclusionAreaString, index + 1);
                    }
                    if (!ExclusionRegion.ContainsKey(kv.Key))
                    {
                        ExclusionRegion.Add(kv.Key, cntr);
                    }
                }
            }
        }

        private void TroyMarkDataCaptureAnalysis()
        {
            try
            {

                //First look for the tagged data in font data and plain text.
                foreach (TroyDataCaptureConfiguration dcc in dCapConfig.DataCaptureConfigurationList)
                {
                    if (dcc.DataUse == DataUseType.TroyMark)
                    {
                        if (dcc.DataCapture == DataCaptureType.StandardFonts)
                        {
                            if (!dcc.UseAllData)
                            {
                                FindTaggedTroyMarkData(dcc.DataTags, CapturedDataPerPage);
                            }
                            else
                            {
                                foreach (KeyValuePair<int, string> kv in CapturedAllDataPerPage)
                                {
                                    if (TroyMarkDataPerPage.ContainsKey(kv.Key))
                                    {
                                        TroyMarkDataPerPage[kv.Key] += kv.Value;
                                    }
                                    else
                                    {
                                        TroyMarkDataPerPage.Add(kv.Key, kv.Value);
                                    }
                                }

                            }
                        }
                        else if (dcc.DataCapture == DataCaptureType.PlainText)
                        {
                            if (!dcc.UseAllData)
                            {
                                FindTaggedTroyMarkData(dcc.DataTags, PlainTextPerPage);
                            }
                            else
                            {
                                foreach (KeyValuePair<int, string> kv in PlainTextPerPage)
                                {
                                    if (TroyMarkDataPerPage.ContainsKey(kv.Key))
                                    {
                                        TroyMarkDataPerPage[kv.Key] += kv.Value;
                                    }
                                    else
                                    {
                                        TroyMarkDataPerPage.Add(kv.Key, kv.Value);
                                    }
                                }
                            }
                        }
                        else if (dcc.DataCapture == DataCaptureType.TroyFonts)
                        {
                            foreach (KeyValuePair<int, string> kv in TroyFontDataPerPage)
                            {
                                if (TroyMarkDataPerPage.ContainsKey(kv.Key))
                                {
                                    TroyMarkDataPerPage[kv.Key] += kv.Value;
                                }
                                else
                                {
                                    TroyMarkDataPerPage.Add(kv.Key, kv.Value);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                pmLogging.LogError("Error in DataCaptureAnalysis.  Error: " + ex.Message, EventLogEntryType.Error, false);

            }
        }

        /********************* JLD code no longer used *********************
        private bool TopOfQueue(FileInfo currentFile)
        {
            try
            {
                string timestamp, compareTimestamp;
                bool returnVal = true;

                timestamp = currentFile.Name.Substring(pmConfig.FilePrefix.Length, 12);

                DirectoryInfo dirInfo = new DirectoryInfo(currentFile.Directory.ToString());
                foreach (FileInfo fi in dirInfo.GetFiles())
                {
                    if (fi.Name != currentFile.Name)
                    {
                        if (CheckValidName(fi))
                        {
                            compareTimestamp = fi.Name.Substring(pmConfig.FilePrefix.Length, 12);
                            if (Convert.ToInt64(timestamp) > Convert.ToInt64(compareTimestamp))
                            {
                                returnVal = false;
                            }
                        }
                    }
                }


                return returnVal;
            }
            catch (Exception ex)
            {
                pmLogging.LogError("Error in checking the queue for file: " + printFileName + " Error: " + ex.Message, EventLogEntryType.Error, true);
                throw; // Goal is to rethrow the exception to get the thread to end.
                //return false;
            }

        }

        private bool CheckValidName(FileInfo checkFileInfo)
        {
            try
            {
                if (checkFileInfo.Extension.IndexOf(pmConfig.FileExtension) < 0)
                {
                    return false;
                }

                if (checkFileInfo.Name.IndexOf(pmConfig.FilePrefix) < 0)
                {
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                pmLogging.LogError("Error in checking the file name format: " + printFileName + " Error: " + ex.Message, EventLogEntryType.Error, true);
                throw; // Goal is to rethrow the exception to get the thread to end.
                //return false;
            }
        }
        ********************* JLD code no longer used *********************/

        private bool ConfigurePrinter()
        {
            try
            {
                bool printerFound = false;

                if (!printerFound)
                {
                    //printToPrinterName = localPrinterConfigSet.GetDefaultPrinterName();
                    printToPrinterName = pmConfig.DefaultPrinter;
                    if (printToPrinterName != "")
                    {
                        printerFound = true;
                    }
                    else
                    {
                        printerFound = false;
                    }
                }

                //If a printer is not found, log the error in the calling function
                return printerFound;
            }
            catch (Exception ex)
            {
                pmLogging.LogError("Error in ConfigurePrinter().  File: " + printFileName + " Error: " + ex.Message, EventLogEntryType.Error, true);
                return false;
            }
        }

        private int FindPjlEnterPcl()
        {
            try
            {
                const int pjlEnterLangLength = 23;
                //@PJL ENTER LANGUAGE=PCL
                byte[] pjlEnterLang = new byte[pjlEnterLangLength] { 0x40, 0x50, 0x4A, 0x4C, 0x20, 0x45, 0x4E, 0x54, 0x45, 0x52, 0x20, 0x4C, 0x41, 0x4E, 0x47, 0x55, 0x41, 0x47, 0x45, 0x3D, 0x50, 0x43, 0x4C };

                bool continueLoop = true;
                int matchCntr = 0, cntr = 0;
                int bufferSize = inputBuffer.Length;

                int returnValue = -1;

                while ((cntr < bufferSize) && (continueLoop))
                {
                    if (inputBuffer[cntr] == pjlEnterLang[matchCntr])
                    {
                        matchCntr++;
                        if (matchCntr == pjlEnterLangLength)
                        {
                            continueLoop = false;
                            cntr = cntr + 4;
                            returnValue = cntr;
                        }
                    }
                    else
                    {
                        matchCntr = 0;
                    }
                    cntr++;
                }
                return returnValue;
            }
            catch (Exception ex)
            {
                pmLogging.LogError("Error in FindPjlEnterPcl(). File: " + printFileName + " Error: " + ex.Message, EventLogEntryType.Error, true);
                return -1;
            }
        }

        private bool RemoveString(int cntr, out int removeLen)
        {
            removeLen = 0;
            if (cntr + RemoveEscStringBytes.Length > inputBuffer.Length)
            {
                return false;
            }
            int lpcntr = 0;
            while ((lpcntr + cntr < inputBuffer.Length) && (lpcntr < RemoveEscStringBytes.Length))
            {
                if (inputBuffer[cntr + lpcntr] != RemoveEscStringBytes[lpcntr])
                {
                    return false;
                }
                lpcntr++;
            }
            removeLen = lpcntr;
            return true;
        }

        private bool ReadInputBuffer()
        {
            try
            {
                bool startFound = false, endFound = false;

                bool fontSelectedById = false;
                int selectedFont = 0;
                bool inEscSeq;
                bool continueLoop = true;
                //int matchCntr = 0, 
                int cntr;
                int bufferSize = inputBuffer.Length;

                currentPage.PageNumber = 1;

                byte[] troyMarkDataBytes = new byte[255];

                int EndOfPageLen = 0;
                int removelen = 0;

                cntr = EndPjlLocation + 1;
                currentPage.PageStart = cntr;

                while ((cntr < bufferSize) && (continueLoop))
                {
                    //Escape character
                    if (inputBuffer[cntr] == ESC)
                    {
                        //*p0x0Y -  The insert point for adding PCL
                        //if ((cntr + 6 <= bufferSize) &&
                        //    ((inputBuffer[cntr + 1] == 0x2A) && (inputBuffer[cntr + 2] == 0x70) &&
                        //     (inputBuffer[cntr + 3] == 0x30) && (inputBuffer[cntr + 4] == 0x78) &&
                        //     (inputBuffer[cntr + 5] == 0x30) && (inputBuffer[cntr + 6] == 0x59)))
                        if (CheckForInsertPoint(cntr))
                        {
                            //Add an entry in the file event list for a page insert location
                            if (!startFound)
                            {
                                currentPage.InsertPoint = cntr + InsertPointString.Length - 1;
                                newEventPoint = new EventPoints(currentPageNum, cntr, EventPointType.epInsertPoint, InsertPointString.Length);
                                fileEventPoints.Add(newEventPoint);
                                startFound = true;
                                endFound = false;
                            }
                            else
                            {
                                pmLogging.LogError("Warning: Found the string *p0x0Y at position " + cntr.ToString() + " while looking for an end of job. File: " + printFileName, EventLogEntryType.Warning, false);
                            }
                            cntr += InsertPointString.Length;
                        }
                        else if ((pmConfig.RemoveEscString != "") && (RemoveString(cntr, out removelen)))
                        {
                            newEventPoint = new EventPoints(currentPageNum, cntr, EventPointType.epRemove, removelen);
                            fileEventPoints.Add(newEventPoint);
                            cntr += removelen-1;

                        }
                        //%m
                        else if ((dataCapFlags.EnableLookForTroyFontCalls) &&
                                 ((cntr + 2 <= bufferSize) &&
                                  (inputBuffer[cntr + 1] == 0x25) && (inputBuffer[cntr + 2] == 0x6D)))
                        {
                            //Check for %m0T
                            if ((inputBuffer[cntr + 3] == 0x30) && (inputBuffer[cntr + 4] == 0x54))
                            {
                                cntr += 4;
                            }
                            else
                            {
                                while (inputBuffer[cntr] == ESC)
                                {
                                    cntr += 3; //Jump over first two chars of the Esc string
                                    while (!((inputBuffer[cntr] > 0x3F) && (inputBuffer[cntr] < 0x5B)))
                                    {
                                        cntr++;
                                    }
                                    cntr++;
                                }
                                if (pmConfig.UseTroyFonts)
                                {
                                    StringBuilder sb = new StringBuilder();
                                    int beginningIndex = cntr, holdCntr;
                                    while (inputBuffer[cntr] != ESC)
                                    {
                                        if ((inputBuffer[cntr] > 0x1F) && (inputBuffer[cntr] < 0x7F))
                                        {
                                            sb.Append(Encoding.ASCII.GetChars(inputBuffer, cntr, 1));
                                        }
                                        cntr++;
                                        holdCntr = cntr;

                                        //Check for cursor movements in between the text
                                        //<ESC>*p
                                        while ((inputBuffer[cntr] == ESC) && (inputBuffer[cntr + 1] == 0x2A) &&
                                            (inputBuffer[cntr + 2] == 0x70))
                                        {
                                            cntr += 3;
                                            while ((inputBuffer[cntr] > 0x2F) && (inputBuffer[cntr] < 0x3A))
                                            {
                                                cntr++;
                                            }
                                            cntr++;
                                        }
                                        //put the index back to where it was the cursor movements if they weren't between text
                                        if ((inputBuffer[cntr] == ESC) || (inputBuffer[cntr] == 0x0C))
                                        {
                                            cntr = holdCntr;
                                        }
                                    }
                                    troyFontCapture += sb.ToString();
                                    int endingIndex = cntr;
                                    if (endingIndex > beginningIndex)
                                    {
                                        //Remove the text
                                        newEventPoint = new EventPoints(currentPageNum, beginningIndex, EventPointType.epSubstitute, endingIndex - beginningIndex);
                                        fileEventPoints.Add(newEventPoint);
                                    }
                                }
                            }
                        }
                        //*p6400Y<FF> -  The end of the page
                        //else if ((cntr + 8 <= bufferSize) &&
                        //         ((inputBuffer[cntr + 1] == 0x2A) && (inputBuffer[cntr + 2] == 0x70) &&
                        //          (inputBuffer[cntr + 3] == 0x36) && (inputBuffer[cntr + 4] == 0x34) &&
                        //          (inputBuffer[cntr + 5] == 0x30) && (inputBuffer[cntr + 6] == 0x30) &&
                        //          (inputBuffer[cntr + 7] == 0x59) && (inputBuffer[cntr + 8] == 0x0C)))
                        else if (CheckForEndOfPage(cntr, ref EndOfPageLen))
                        {
                            //Add an entry in the file event list for a page end location
                            if (!endFound)
                            {
                                currentPage.PageEnd = cntr + EndOfPageLen - 1;
                                pageInfoList.Add(currentPage);
                                newEventPoint = new EventPoints(currentPageNum, cntr, EventPointType.epPageEnd, EndOfPageLen);
                                fileEventPoints.Add(newEventPoint);
                                startFound = false;
                                endFound = true;
                                EndOfPageFunctions();
                                currentPageNum++;
                                currentPage.PageNumber = currentPageNum;
                                currentPage.PageStart = cntr + EndOfPageLen;
                            }
                            else
                            {
                                pmLogging.LogError("Warning: Found the string *p6400<FF> at position " + cntr.ToString() + " while looking for a start of job. File: " + printFileName, EventLogEntryType.Warning, false);
                            }
                            cntr += EndOfPageLen;
                        }
                        //<ESC>E - Printer reset
                        else if ((cntr + 1 <= bufferSize) &&
                                 (inputBuffer[cntr + 1] == 0x45))
                        {
                            newEventPoint = new EventPoints(currentPageNum, cntr, EventPointType.epPrinterReset, 9);
                            fileEventPoints.Add(newEventPoint);
                            cntr += 2;
                        }
                        //UEL &-12345X
                        else if ((cntr + 8 <= bufferSize) &&
                                 ((inputBuffer[cntr + 1] == 0x25) && (inputBuffer[cntr + 2] == 0x2D) &&
                                  (inputBuffer[cntr + 3] == 0x31) && (inputBuffer[cntr + 4] == 0x32) &&
                                  (inputBuffer[cntr + 5] == 0x33) && (inputBuffer[cntr + 6] == 0x34) &&
                                  (inputBuffer[cntr + 7] == 0x35) && (inputBuffer[cntr + 8] == 0x58)))
                        {
                            newEventPoint = new EventPoints(currentPageNum, cntr, EventPointType.epUELLocation, 9);
                            fileEventPoints.Add(newEventPoint);
                            cntr += 9;
                            //Look for @PJL EOL so that its not considered plain text
                            if ((cntr + 7 <= bufferSize) &&
                                ((inputBuffer[cntr] == 0x40) && (inputBuffer[cntr + 1] == 0x50) &&
                                 (inputBuffer[cntr + 2] == 0x4A) && (inputBuffer[cntr + 3] == 0x4C) &&
                                 (inputBuffer[cntr + 4] == 0x20) && (inputBuffer[cntr + 5] == 0x45) &&
                                 (inputBuffer[cntr + 6] == 0x4F) && (inputBuffer[cntr + 7] == 0x4A)))
                            {
                                cntr += 8;
                            }
                        }
                        //<ESC>&l
                        else if ((cntr + 2 <= bufferSize) &&
                                 ((inputBuffer[cntr + 1] == 0x26) && (inputBuffer[cntr + 2] == 0x6C)))
                        {
                            //Paper Source &l#H where # is a number 1-9
                            if (inputBuffer[cntr + 4] == 0x48)
                            {
                                newEventPoint = new EventPoints(currentPageNum, cntr, EventPointType.epPaperSource, 9);
                                fileEventPoints.Add(newEventPoint);
                                cntr += 5;
                            }
                            else
                            {
                                int tempCntr = cntr + 3;
                                int newStrPos = cntr;
                                while ((inputBuffer[tempCntr] < 0x40) || (inputBuffer[tempCntr] > 0x5E))
                                {
                                    if (inputBuffer[tempCntr] == 0x1B)
                                    {
                                        tempCntr--;
                                        break;
                                    }
                                    if (inputBuffer[tempCntr] > 0x5E)
                                    {
                                        //Looking for x
                                        if (inputBuffer[tempCntr] == 0x78)
                                        {
                                            newEventPoint = new EventPoints(currentPageNum, newStrPos, EventPointType.epRemove, tempCntr - newStrPos + 1);
                                            fileEventPoints.Add(newEventPoint);
                                            break;
                                        }
                                        else
                                        {
                                            //end of an esc string
                                            newStrPos = tempCntr + 1;
                                        }
                                    }
                                    tempCntr++;
                                }
                                if (inputBuffer[tempCntr] == 0x58)
                                {
                                    newEventPoint = new EventPoints(currentPageNum, newStrPos, EventPointType.epRemove, tempCntr - newStrPos + 1);
                                    fileEventPoints.Add(newEventPoint);
                                }
                                cntr = tempCntr + 1;
                            }

                        }
                        //*c - begining of a font definition possibly
                        else if ((cntr + 2 <= bufferSize) &&
                                 ((inputBuffer[cntr + 1] == 0x2A) && (inputBuffer[cntr + 2] == 0x63)))
                        {
                            if ((inputBuffer[cntr + 3] == 0x30) && (inputBuffer[cntr + 4] == 0x74))
                            {
                                cntr++;
                                inEscSeq = true;
                                while ((inEscSeq) && (cntr < bufferSize))
                                {
                                    //Capital letter and @ mark the end of an Escape sequence
                                    if ((inputBuffer[cntr] > 0x3F) && (inputBuffer[cntr] < 0x5B))
                                    {
                                        inEscSeq = false;
                                        cntr++;
                                    }
                                    else
                                    {
                                        cntr++;
                                    }
                                }
                            }
                            else
                            {
                                EvaluateFontDescription(ref cntr, ref selectedFont, ref fontSelectedById);
                            }
                        }
                        // ( - font call
                        else if ((cntr + 1 <= bufferSize) &&
                                 (inputBuffer[cntr + 1] == 0x28))
                        {
                            EvaluateSymbolSet(ref cntr, ref selectedFont, ref fontSelectedById);
                        }
                        //*p - co-ordinate move that could precede a glyph id characters
                        else if ((cntr + 2 <= bufferSize) &&
                                 ((inputBuffer[cntr + 1] == 0x2a) && (inputBuffer[cntr + 2] == 0x70)))
                        {
                            if (fontSelectedById)
                            {
                                EvaluateChars(ref cntr, ref selectedFont, ref fontSelectedById);
                            }
                            else
                            {
                                cntr++;
                                inEscSeq = true;
                                while ((inEscSeq) && (cntr < bufferSize))
                                {
                                    //Capital letter and @ mark the end of an Escape sequence
                                    if ((inputBuffer[cntr] > 0x3F) && (inputBuffer[cntr] < 0x5B))
                                    {
                                        inEscSeq = false;
                                        cntr++;
                                    }
                                    else
                                    {
                                        cntr++;
                                    }
                                }
                            }
                        }
                        //1.0.25
                        //%#B  - enter HPGL
                        else if ((cntr + 3 < bufferSize) &&
                             ((inputBuffer[cntr + 1] == 0x25) &&
                              (inputBuffer[cntr + 3] == 0x42)))
                        {
                            cntr += 3;
                            while (cntr < bufferSize)
                            {
                                //%#A - exit HPGL
                                if ((cntr + 3 < bufferSize) &&
                                    ((inputBuffer[cntr] == 0x1B) && (inputBuffer[cntr + 1] == 0x25) &&
                                     (inputBuffer[cntr + 3] == 0x41)))
                                {
                                    cntr += 4;
                                    break;
                                }
                                else
                                {
                                    cntr++;
                                }
                            }
                        }
                        //Start raster data *r1A
                        else if ((cntr < cntr + 5) &&
                                 ((inputBuffer[cntr + 1] == 0x2A) && (inputBuffer[cntr + 2] == 0x72) &&
                                  (inputBuffer[cntr + 3] == 0x31) && (inputBuffer[cntr + 4] == 0x41)))
                        {
                            cntr += 5;
                            bool foundEndRaster = false;
                            //loop until end of raster is found
                            while ((cntr < bufferSize) && (!foundEndRaster))
                            {
                                if (inputBuffer[cntr] == 0x1B)
                                {

                                    if ((cntr < cntr + 4) &&
                                        ((inputBuffer[cntr + 1] == 0x2A) && (inputBuffer[cntr + 2] == 0x72) &&
                                         (inputBuffer[cntr + 3] == 0x42)))
                                    {
                                        foundEndRaster = true;
                                        //IMPORTANT: NEEDS TO EXIT WITH CNTR POINTING TO ESCAPE SO DO NOT INCREMENT CNTR IN CASE THIS IS ALSO THE END OF PAGE
                                    }
                                    else if ((cntr < cntr + 4) &&
                                        ((inputBuffer[cntr + 1] == 0x2A) && (inputBuffer[cntr + 2] == 0x72) &&
                                         (inputBuffer[cntr + 3] == 0x43)))
                                    {
                                        foundEndRaster = true;
                                        //IMPORTANT: NEEDS TO EXIT WITH CNTR POINTING TO ESCAPE SO DO NOT INCREMENT CNTR IN CASE THIS IS ALSO THE END OF PAGE
                                    }
                                    else
                                    {
                                        cntr++;
                                    }
                                }
                                else
                                {
                                    cntr++;
                                }
                            }
                        }
                        //1.0.25
                        //Look for ESC strings that have data (that end with W)
                        //)s,(s,(f,&n,*o,*b,*v,*m,*l,*i                    
                        else if (((cntr + 2) < bufferSize) &&
                             (((inputBuffer[cntr + 1] == 0x29) && (inputBuffer[cntr + 2] == 0x73)) ||
                             ((inputBuffer[cntr + 1] == 0x28) && (inputBuffer[cntr + 2] == 0x73)) ||
                             ((inputBuffer[cntr + 1] == 0x28) && (inputBuffer[cntr + 2] == 0x66)) ||
                             ((inputBuffer[cntr + 1] == 0x26) && (inputBuffer[cntr + 2] == 0x6E)) ||
                             ((inputBuffer[cntr + 1] == 0x2A) && (inputBuffer[cntr + 2] == 0x6F)) ||
                             ((inputBuffer[cntr + 1] == 0x2A) && (inputBuffer[cntr + 2] == 0x62)) ||
                             ((inputBuffer[cntr + 1] == 0x2A) && (inputBuffer[cntr + 2] == 0x76)) ||
                             ((inputBuffer[cntr + 1] == 0x2A) && (inputBuffer[cntr + 2] == 0x6D)) ||
                             ((inputBuffer[cntr + 1] == 0x2A) && (inputBuffer[cntr + 2] == 0x6C)) ||
                             ((inputBuffer[cntr + 1] == 0x2A) && (inputBuffer[cntr + 2] == 0x69))))
                        {
                            byte retChar = 0x00;
                            int LengthStartPos = 0;
                            int HoldStartPos = cntr + 2;
                            int newIndex = FindEndOfEscString(cntr, bufferSize, ref retChar, ref LengthStartPos, inputBuffer);
                            cntr = newIndex;
                            //W
                            if (retChar == 0x57)
                            {
                                int JumpCntr = Convert.ToInt32(Encoding.ASCII.GetString(inputBuffer, LengthStartPos + 1, cntr - LengthStartPos - 2));
                                cntr += JumpCntr;
                            }
                            //look for the *b ending with a V
                            else if ((inputBuffer[HoldStartPos - 1] == 0x2A) && (inputBuffer[HoldStartPos] == 0x62) && (retChar == 0x56))
                            {
                                int JumpCntr = Convert.ToInt32(Encoding.ASCII.GetString(inputBuffer, LengthStartPos, cntr - LengthStartPos - 1));
                                cntr += JumpCntr;
                            }
                        }

                        //else jump to the next character
                        else
                        {

                            inEscSeq = true;
                            cntr++;
                            while ((inEscSeq) && (cntr < bufferSize))
                            {
                                //Capital letter and @ mark the end of an Escape sequence
                                if ((inputBuffer[cntr] > 0x3F) && (inputBuffer[cntr] < 0x5B))
                                {
                                    inEscSeq = false;
                                    cntr++;
                                }
                                else
                                {
                                    cntr++;
                                }
                            }
                        }
                    }
                    //Not Escape character
                    else
                    {
                        //Version 1.0.15
                        if ((inputBuffer[cntr] == 0x0C) && (pmConfig.UseFormFeedAsPageBreak))
                        {
                            //Add an entry in the file event list for a page end location
                            if (!endFound)
                            {
                                currentPage.PageEnd = cntr;
                                pageInfoList.Add(currentPage);
                                newEventPoint = new EventPoints(currentPageNum, cntr, EventPointType.epPageEnd, 1);
                                fileEventPoints.Add(newEventPoint);
                                startFound = false;
                                endFound = true;
                                EndOfPageFunctions();
                                currentPageNum++;
                                currentPage.PageNumber = currentPageNum;
                                currentPage.PageStart = cntr + 1;


                                //Now do insert point
                                //1.0.25
                                if (string.IsNullOrEmpty(pmConfig.InsertPointPclString))
                                {
                                    currentPage.InsertPoint = cntr + 1;
                                    newEventPoint = new EventPoints(currentPageNum, cntr, EventPointType.epInsertPoint, 0);
                                    fileEventPoints.Add(newEventPoint);
                                    startFound = true;
                                    endFound = false;
                                }
                            }
                            else
                            {
                                pmLogging.LogError("Warning: Found the string *p6400<FF> at position " + cntr.ToString() + " while looking for a start of job. File: " + printFileName, EventLogEntryType.Warning, false);
                            }
                        }
                        else if (inputBuffer[cntr] == EndOfPageString[0])
                        {
                            if (CheckForEndOfPage(cntr, ref EndOfPageLen))
                            {
                                //Add an entry in the file event list for a page end location
                                if (!endFound)
                                {
                                    currentPage.PageEnd = cntr + EndOfPageLen - 1;
                                    pageInfoList.Add(currentPage);
                                    newEventPoint = new EventPoints(currentPageNum, cntr, EventPointType.epPageEnd, EndOfPageLen);
                                    fileEventPoints.Add(newEventPoint);
                                    startFound = false;
                                    endFound = true;
                                    EndOfPageFunctions();
                                    currentPageNum++;
                                    currentPage.PageNumber = currentPageNum;
                                    currentPage.PageStart = cntr + EndOfPageLen;
                                }
                                else
                                {
                                    pmLogging.LogError("Warning: Found the string *p6400<FF> at position " + cntr.ToString() + " while looking for a start of job. File: " + printFileName, EventLogEntryType.Warning, false);
                                }
                            }
                        }
                        else
                        {
                            if (dataCapFlags.EnablePlainTextCapture)
                            {
                                if ((inputBuffer[cntr] > 0x1F) && (inputBuffer[cntr] < 0x7F))
                                {
                                    plainTextStr.Append(Convert.ToChar(inputBuffer[cntr]));
                                }
                            }
                        }
                        cntr++;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                pmLogging.LogError("Error in ReadInputBuffer(). File: " + printFileName + " Error: " + ex.Message, EventLogEntryType.Error, true);
                return false;
            }
        }
        public int FindEndOfEscString(int index, int bufferSize, ref byte EndChar, ref int LengthStartPos, byte[] InputBuffer)
        {
            //Move past ESC
            index += 1;
            EndChar = 0x00;
            LengthStartPos = index;

            bool inEscSeq = true;
            while ((inEscSeq) && (index < bufferSize))
            {
                //Capital letter and @ mark the end of an Escape sequence
                if ((InputBuffer[index] > 0x3F) && (InputBuffer[index] < 0x5B))
                {
                    inEscSeq = false;
                    EndChar = InputBuffer[index];
                    index++;
                }
                //check for a none numeric character (lower case letter)
                else if (!((InputBuffer[index] > 0x29) && (InputBuffer[index] < 0x40)))
                {
                    LengthStartPos = index;
                    index++;
                }
                else
                {
                    index++;
                }
            }
            return index;
        }


        private void EndOfPageFunctions()
        {
            currentPage = new PageBoundaries();

            if (pmConfig.EnablePjlTmDataParse)
            {
                string data = PatientNameFromPJL + " ";
                if (DrugPerPage.ContainsKey(currentPageNum))
                {
                    data += DrugPerPage[currentPageNum] + " ";
                }
                if (RefillsPerPage.ContainsKey(currentPageNum))
                {
                    data += "Refills: " + RefillsPerPage[currentPageNum] + " ";
                }
                TroyMarkDataPerPage.Add(currentPageNum, data);

                //Clear these just to keep them from getting too large
                dataCapture = "";
                TroymarkAllData = "";
                PassThroughString = "";
                PrinterMapString = "";
                plainTextStr.Remove(0, plainTextStr.Length);
                troyFontCapture = "";

            }
            else
            {
                if (dataCapture != "")
                {
                    if (!CapturedAllDataPerPage.ContainsKey(currentPageNum))
                    {
                        CapturedDataPerPage.Add(currentPageNum, dataCapture);
                    }
                    dataCapture = "";
                }
                if (TroymarkAllData != "")
                {
                    if (!CapturedAllDataPerPage.ContainsKey(currentPageNum))
                    {
                        CapturedAllDataPerPage.Add(currentPageNum, TroymarkAllData);
                    }
                    TroymarkAllData = "";
                }
                if (MpAllData != "")
                 {
                     if (!CapturedAllMpDataPerPage.ContainsKey(currentPageNum))
                     {
                         CapturedAllMpDataPerPage.Add(currentPageNum, MpAllData);
                     }
                     MpAllData = "";
                 }
                 if (MpCapturedData != "")
                 {
                     if (!MpDataPerPage.ContainsKey(currentPageNum))
                     {
                         MpDataPerPage.Add(currentPageNum, MpCapturedData);
                     }
                     MpCapturedData = "";
                 }
                 if ((pmConfig.PassThroughEnabled) && (PassThroughString != ""))
                 {
                    if (!PassThroughPerPage.ContainsKey(currentPageNum))
                    {
                        PassThroughPerPage.Add(currentPageNum, PassThroughString);
                    }
                    PassThroughString = "";
                }
                if (PrinterMapString != "")
                {
                    if (!PrinterMapPerPage.ContainsKey(currentPageNum))
                    {
                        PrinterMapPerPage.Add(currentPageNum, PrinterMapString);
                    }
                    PrinterMapString = "";
                }

                if (plainTextStr.Length > 0)
                {
                    if (!PlainTextPerPage.ContainsKey(currentPageNum))
                    {
                        //1.0.25
                        string temp = plainTextStr.ToString().Replace("   ", "");
                        PlainTextPerPage.Add(currentPageNum, temp);
                    }
                    plainTextStr.Remove(0, plainTextStr.Length);
                }

                if (troyFontCapture != "")
                {
                    if (!TroyFontDataPerPage.ContainsKey(currentPageNum))
                    {
                        TroyFontDataPerPage.Add(currentPageNum, troyFontCapture);
                    }
                    troyFontCapture = "";
                }

                if (taggedDataList.Count > 0)
                {
                    TaggedListPerPage tlpp = new TaggedListPerPage();
                    tlpp.TaggedList = taggedDataList;
                    TaggedListPageList.Add(tlpp);
                    taggedDataList.Clear();
                }
            }

        }

        private bool CheckForEndOfPage(int cntr, ref int EndOfPageLength)
        {
            bool retval = true;

            EndOfPageLength = EndOfPageString.Length;
            if (cntr + EndOfPageString.Length > inputBuffer.Length)
            {
                //return false;
                retval = false;
            }
            else
            {
                int lpcntr = 0;
                while ((lpcntr + cntr < inputBuffer.Length) && (lpcntr < EndOfPageString.Length))
                {
                    if (inputBuffer[cntr + lpcntr] != EndOfPageString[lpcntr])
                    {
                        //return false;
                        retval = false;
                        break;
                    }
                    lpcntr++;
                }
            }

            //Look for an alternate end of page
            if ((AltEndOfPageString != null) && (!retval))
            {
                EndOfPageLength = AltEndOfPageString.Length;
                retval = true;
                if (cntr + AltEndOfPageString.Length > inputBuffer.Length)
                {
                    //return false;
                    retval = false;
                }
                else
                {
                    int lpcntr = 0;
                    while ((lpcntr + cntr < inputBuffer.Length) && (lpcntr < AltEndOfPageString.Length))
                    {
                        if (inputBuffer[cntr + lpcntr] != AltEndOfPageString[lpcntr])
                        {
                            //return false;
                            retval = false;
                            break;
                        }
                        lpcntr++;
                    }
                }
            }

            return retval;
        }

        private bool CheckForInsertPoint(int cntr)
        {
            if (cntr + InsertPointString.Length > inputBuffer.Length)
            {
                return false;
            }
            int lpcntr = 0;
            while ((lpcntr + cntr < inputBuffer.Length) && (lpcntr < InsertPointString.Length))
            {
                if (inputBuffer[cntr + lpcntr] != InsertPointString[lpcntr])
                {
                    return false;
                }
                lpcntr++;
            }

            return true;
        }
        private bool WriteOutPcl()
        {
            try
            {
                StringBuilder tempString = new StringBuilder();


                outbuf = new BinaryWriter(File.Open(tempFileName, FileMode.Create));
                
                WriteOutPjl();

                int currPage = 1;
                int currCntr = EndPjlLocation + 1;
                foreach (EventPoints evpt in fileEventPoints)
                {
                    if (evpt.PageNumber == currPage)
                    {
                        if (evpt.Location > currCntr)
                        {
                            outbuf.Write(inputBuffer, currCntr, evpt.Location - currCntr);
                            currCntr = evpt.Location;
                        }

                        switch (evpt.EventType)
                        {
                            case EventPointType.epInsertPoint:
                                //FOrce number of copies to 1
                                if (currPage == 1)
                                {
                                    outbuf.Write(NumberOfCopiesOne, 0, NumberOfCopiesOne.Length);
                                    if (pmConfig.PclCommandInsert != "")
                                    {
                                        string PclCommand = pmConfig.PclCommandInsert.Replace("/e", "\u001B");
                                        PclCommand = PclCommand.Replace("/a", "&");
                                        byte[] PclBytes = new UTF8Encoding(true).GetBytes(PclCommand);
                                        outbuf.Write(PclBytes, 0, PclBytes.Length);
                                    }
                                }
                                outbuf.Write(inputBuffer, evpt.Location, evpt.EventLength);
                                currCntr += evpt.EventLength;
                                if ((pmConfig.EnableDuplex > -1) && (currPage == 1))
                                {
                                    byte[] duplexInfo = new byte[15];
                                    int duplexInfoSize = 15;
                                    GetEnableDuplex(pmConfig.EnableDuplex, ref duplexInfo, ref duplexInfoSize);
                                    outbuf.Write(duplexInfo, 0, duplexInfoSize);
                                }
                                if (SoftwarePantograph)
                                {
                                    if (pmConfig.PrintPantographProfileList != "")
                                    {
                                        AddPantograph(currPage);
                                    }
                                    if ((!InsertedDemoTroymark) && (!pmConfig.TroyMarkEnabled))
                                    {
                                        if (!InsertDemoTroyMark())
                                        {
                                            pmLogging.LogError("Disabling Security Features.  Demo Troymark failed to print.", EventLogEntryType.Error, false);

                                        }
                                    }
                                }
                                if (pmConfig.MicroPrintConfig.Count > 0)
                                {
                                    foreach (MpConfiguration mc in pmConfig.MicroPrintConfig)
                                    {
                                        if (MpDataPerPage.ContainsKey(currPage))
                                        {
                                            string mpstr = MpDataPerPage[currPage];
                                            string fontpath = Path.Combine(printJobFileInfo.DirectoryName, "Data");
                                            if (Directory.Exists(fontpath))
                                            {
                                                var bp = new BuildPantographWrap.Wrapper();
                                                string outstring = bp.ManagedMakeMpLine((mc.UsePoint6 ? 6 : 8), mpstr, mc.XAnchor, mc.YAnchor, mc.Width, mc.Height, fontpath);
                                                byte[] outbytes = new UTF8Encoding(true).GetBytes(outstring);
                                                if ((outbytes != null) && (outbytes.Length > 0))
                                                {
                                                    outbuf.Write(outbytes, 0, outbytes.Length);
                                                }
                                            }
                                        }
                                    }
                                }
                                foreach (InsertStrings instr in insertStringList)
                                {
                                    if ((instr.PageNumber == currPage) && (instr.MoveToInsertLocation))
                                    {
                                        if (instr.LeadingPclLength > 0)
                                        {
                                            outbuf.Write(instr.LeadingPcl, 0, instr.LeadingPclLength);
                                        }
                                        foreach (char tempChar in instr.ConvertedAscii)
                                        {
                                            outbuf.Write(tempChar);
                                        }
                                        if (instr.TrailingPclLength > 0)
                                        {
                                            outbuf.Write(instr.TrailingPcl, 0, instr.TrailingPclLength);
                                        }
                                    }
                                }
                                if (!InsertedDemoTroymark)
                                {
                                    InsertDemoTroyMark();
                                }

                                if ((SoftwareTroyMark) && (pmConfig.TroyMarkEnabled))
                                {
                                    if (!pmConfig.TroyMarkOnBack)
                                    {
                                        InsertSoftwareTroymark(currPage, false, false);
                                    }
                                    else if (pmConfig.TroymarkDetachStaticText)
                                    {
                                        InsertSoftwareTroymark(currPage, false, true);
                                    }
                                }
                                InsertUniversalExclusion(currPage);
                                break;
                            case EventPointType.epPageEnd:
                                outbuf.Write(inputBuffer, evpt.Location, evpt.EventLength);
                                currCntr += evpt.EventLength;
                                currPage++;
                                // JLD begin v2.1.4
                                string pclaeop = pmConfig.PclCommandInsertAfterEndOfPage;
                                if (pclaeop != "")
                                {
                                    pclaeop = pclaeop.Replace("/e", "\u001B"); //Escape
                                    pclaeop = pclaeop.Replace("/f", "\u000C"); //Form Feed
                                    pclaeop = pclaeop.Replace("/n", "\u000D"); //Carriage Return
                                    pclaeop = pclaeop.Replace("/r", "\u000A"); //Line Feed
                                    pclaeop = pclaeop.Replace("/a", "&"); //Ampersand
                                    outbuf.Write(new UTF8Encoding(true).GetBytes(pclaeop), 0, pclaeop.Length);
                                }
                                // JLD end v2.1.4

                                foreach (PageBoundaries page in pageInfoList)
                                {
                                    if ((page.PageNumber == currPage) && (page.ConfigPage)) // || (page.SkipPage)))
                                    {
                                        currCntr = page.PageEnd + 1;
                                        currPage++;
                                    }
                                }

                                if ((SoftwareTroyMark) && (pmConfig.TroyMarkEnabled))
                                {
                                    if (pmConfig.TroyMarkOnBack)
                                    {
                                        if (!InsertedDemoTroymark)
                                        {
                                            InsertDemoTroyMark();
                                        }
                                        InsertSoftwareTroymark(currPage - 1, true, false);
                                    }
                                    else if (pmConfig.TroymarkDetachStaticText)
                                    {
                                        InsertSoftwareTroymark(currPage - 1, true, true);
                                    }
                                }

                                break;
                            case EventPointType.epRemove:
                                currCntr += evpt.EventLength;
                                break;
                            case EventPointType.epSubstitute:
                                currCntr += evpt.EventLength;
                                foreach (InsertStrings instr in insertStringList)
                                {
                                    if ((instr.PageNumber == currPage) && (instr.Location == evpt.Location))
                                    {
                                        if (instr.LeadingPclLength > 0)
                                        {
                                            outbuf.Write(instr.LeadingPcl, 0, instr.LeadingPclLength);
                                        }
                                        foreach (char tempChar in instr.ConvertedAscii)
                                        {
                                            outbuf.Write(tempChar);
                                        }
                                        if (instr.TrailingPclLength > 0)
                                        {
                                            outbuf.Write(instr.TrailingPcl, 0, instr.TrailingPclLength);
                                        }
                                    }
                                }
                                break;
                            case EventPointType.epUELLocation:
                                outbuf.Write(inputBuffer, evpt.Location, evpt.EventLength);
                                currCntr += evpt.EventLength;
                                break;
                        }
                    }
                }

                outbuf.Flush();
                if (outbuf != null)
                {
                    outbuf.Close();
                }

                printerFileName = tempFileName;

                return true;
            }
            catch (PortMonCustomException pme)
            {
                throw pme;
            }
            catch (Exception ex)
            {
                pmLogging.LogError("Error in WriteOutPcl(). File: " + printFileName + " Error: " + ex.Message, EventLogEntryType.Error, true);
                return false;
            }
            finally
            {
            }
        }

        private void InsertUniversalExclusion(int pageCntr)
        {
            if (pmConfig.UniversalExclusionRegions.Count > 0)
            {
                if (ExclusionRegion.ContainsKey(pageCntr))
                {
                    for (int cntr = 0; cntr < ExclusionRegion[pageCntr]; cntr++)
                    {
                        if (pmConfig.UniversalExclusionRegions[cntr] != null)
                        {
                            string excX = "", excY = "", excH = "", excV = "";
                            int cntr1 = 1;
                            bool AllFound = false;
                            foreach (string subStr1 in pmConfig.UniversalExclusionRegions[cntr].Split(','))
                            {
                                if (cntr1 == 1)
                                {
                                    excX = subStr1;
                                }
                                else if (cntr1 == 2)
                                {
                                    excY = subStr1;
                                }
                                else if (cntr1 == 3)
                                {
                                    excH = subStr1;
                                }
                                else if (cntr1 == 4)
                                {
                                    excV = subStr1;
                                    AllFound = true;
                                }
                                cntr1++;
                            }
                            if (AllFound)
                            {
                                string PclString = "\u001B&f0S"; //Push cursor position
                                PclString += "\u001B*p" + excX.ToString() + "x" + excY.ToString() + "Y";
                                PclString += "\u001B*c" + excH.ToString() + "a" + excV.ToString() + "B";
                                PclString += "\u001B*c1P";
                                PclString += "\u001B&f1S"; //Pop cursor position
                                byte[] PclBytes = new UTF8Encoding(true).GetBytes(PclString);
                                outbuf.Write(PclBytes, 0, PclBytes.Length);
                            }
                        }
                    }
                }
            }

        }

        private bool InsertSoftwareTroymark(int currPage, bool insertFF, bool StaticOnly)
        {
            try
            {
                if ((pmConfig.TroyMarkStaticText.Contains("/t")) || pmConfig.TroyMarkStaticText.Contains("/d"))
                {
                    string DateStr = DateTime.Now.ToString("MMM dd, yyyy");
                    string TimeStr = DateTime.Now.ToString("hh:mm:ss");

                    pmConfig.TroyMarkStaticText = pmConfig.TroyMarkStaticText.Replace("/d", DateStr);
                    pmConfig.TroyMarkStaticText = pmConfig.TroyMarkStaticText.Replace("/t", TimeStr);

                }

                if ((pmConfig.DefaultTroyMarkPattern.ToUpper() != "NONE") && (pmConfig.DefaultTroyMarkPattern != ""))
                {
                    Troy.TROYmarkPclBuilder.Interface.TROYmarkConfiguration tmConfig = new Troy.TROYmarkPclBuilder.Interface.TROYmarkConfiguration();
                    if (pmConfig.DefaultTroyMarkPattern.ToUpper() == "DARK")
                    {
                        tmConfig.Pattern = Troy.TROYmarkPclBuilder.Interface.TROYmarkPatternType.tmDark;
                    }
                    else if (pmConfig.DefaultTroyMarkPattern.ToUpper() == "MEDIUM")
                    {
                        tmConfig.Pattern = Troy.TROYmarkPclBuilder.Interface.TROYmarkPatternType.tmMedium;
                    }
                    else if (pmConfig.DefaultTroyMarkPattern.ToUpper() == "LIGHT")
                    {
                        tmConfig.Pattern = Troy.TROYmarkPclBuilder.Interface.TROYmarkPatternType.tmLight;
                    }
                    if (pmConfig.DefaultTroyMarkSpacing != "")
                    {
                        tmConfig.LineSpacing = Convert.ToInt32(pmConfig.DefaultTroyMarkSpacing);
                    }

                    tmConfig.TextColor =
                        (Interface.TextColor) Enum.Parse(typeof (Interface.TextColor), "tm" + pmConfig.TextColor.ToString());

                    if (pmConfig.TroyMarkCharsPerLine > 0)
                    {
                        // did KEN forget something here?
                    }

                    if (pmConfig.DefaultTroyMarkInclusion != "")
                    {
                        if (tmConfig.InclusionRegion == null)
                        {
                            tmConfig.InclusionRegion = new Troy.TROYmarkPclBuilder.Interface.TroyMarkRegionType();
                        }
                        int cntr1 = 1;
                        foreach (string subStr1 in pmConfig.DefaultTroyMarkInclusion.Split(','))
                        {
                            if (cntr1 == 1) tmConfig.InclusionRegion.XAnchor = Convert.ToInt32(subStr1);
                            else if (cntr1 == 2) tmConfig.InclusionRegion.YAnchor = Convert.ToInt32(subStr1);
                            else if (cntr1 == 3) tmConfig.InclusionRegion.Width = Convert.ToInt32(subStr1);
                            else if (cntr1 == 4) tmConfig.InclusionRegion.Height = Convert.ToInt32(subStr1);
                            cntr1++;
                        }

                    }

                    if (pmConfig.DefaultTroyMarkExclusion != "")
                    {
                        Troy.TROYmarkPclBuilder.Interface.TroyMarkRegionType tmRegion = new Troy.TROYmarkPclBuilder.Interface.TroyMarkRegionType();
                        int cntr1 = 1;
                        foreach (string subStr1 in pmConfig.DefaultTroyMarkExclusion.Split(','))
                        {
                            if (cntr1 == 1) tmRegion.XAnchor = Convert.ToInt32(subStr1);
                            else if (cntr1 == 2) tmRegion.YAnchor = Convert.ToInt32(subStr1);
                            else if (cntr1 == 3) tmRegion.Width = Convert.ToInt32(subStr1);
                            else if (cntr1 == 4) tmRegion.Height = Convert.ToInt32(subStr1);
                            cntr1++;
                        }
                        tmConfig.ExclusionRegion.Add(tmRegion);
                    }


                    string TMString = "";

                    if (StaticOnly)
                    {
                        if (pmConfig.TroyMarkStaticText != "")
                        {
                            TMString += pmConfig.TroyMarkStaticText;
                        }

                        if (pmConfig.TroymarkStaticPattern != "")
                        {
                            if (pmConfig.TroymarkStaticPattern.ToUpper() == "DARK")
                                tmConfig.Pattern = Troy.TROYmarkPclBuilder.Interface.TROYmarkPatternType.tmDark;
                            else if (pmConfig.TroymarkStaticPattern.ToUpper() == "MEDIUM")
                                tmConfig.Pattern = Troy.TROYmarkPclBuilder.Interface.TROYmarkPatternType.tmMedium;
                            else if (pmConfig.TroymarkStaticPattern.ToUpper() == "LIGHT")
                                tmConfig.Pattern = Troy.TROYmarkPclBuilder.Interface.TROYmarkPatternType.tmLight;
                        }

                        if (pmConfig.TroymarkStaticInclusion != "")
                        {
                            if (tmConfig.InclusionRegion == null)
                                tmConfig.InclusionRegion = new Troy.TROYmarkPclBuilder.Interface.TroyMarkRegionType();
                            int cntrst = 1;
                            foreach (string subStr1 in pmConfig.TroymarkStaticInclusion.Split(','))
                            {
                                if (cntrst == 1) tmConfig.InclusionRegion.XAnchor = Convert.ToInt32(subStr1);
                                else if (cntrst == 2) tmConfig.InclusionRegion.YAnchor = Convert.ToInt32(subStr1);
                                else if (cntrst == 3) tmConfig.InclusionRegion.Width = Convert.ToInt32(subStr1);
                                else if (cntrst == 4) tmConfig.InclusionRegion.Height = Convert.ToInt32(subStr1);
                                cntrst++;
                            }

                        }

                    }
                    else
                    {
                        if ((pmConfig.TroyMarkStaticText != "") && (!pmConfig.TroymarkDetachStaticText))
                        {
                            TMString += pmConfig.TroyMarkStaticText;
                        }

                        //PJL Data.  Use for all pages.
                        //  Version 1.0.13, using [0] for First Page Troymark too
                        if (TroyMarkDataPerPage.ContainsKey(0))
                        {
                            TMString += TroyMarkDataPerPage[0];
                        }

                        //Version 1.0.13
                        if (!pmConfig.TroyMarkUseFirstData)
                        {
                            if (TroyMarkDataPerPage.ContainsKey(currPage))
                                TMString += TroyMarkDataPerPage[currPage];
                        }

                        if ((PrintJobName != "") && (pmConfig.PjlDocNameToTmString.Count > 0))
                        {
                            //if (pmConfig.PjlDocNameToTmString.ContainsKey(PrintJobName))
                            //{
                            //    TMString += " " + pmConfig.PjlDocNameToTmString[PrintJobName].ToString();
                            //}
                            //KLK problem with this solution if there is a string that occurs as part of the key in multiple entries.
                            foreach (KeyValuePair<string, string> kvp in pmConfig.PjlDocNameToTmString)
                            {
                                if (PrintJobName.ToUpper().Contains(kvp.Key))
                                {
                                    TMString += " " + kvp.Value;
                                    break;
                                }
                            }
                        }
                    }

                    if (TMString != "")
                    {
                        if (pmConfig.TroyMarkCharsPerLine > 0)
                        {
                            int strCntr = 1, currIndex = 0;
                            int CharsPerLine = Convert.ToInt32(pmConfig.TroyMarkCharsPerLine);
                            while (TMString.Length > (strCntr * CharsPerLine))
                            {
                                tmConfig.TROYmarkText.Add(TMString.Substring(CharsPerLine * (strCntr - 1), CharsPerLine));
                                currIndex += CharsPerLine;
                                strCntr++;
                            }
                            string nextString = TMString.Substring(CharsPerLine * (strCntr - 1), TMString.Length - currIndex);
                            if (currIndex > 0)
                            {
                                nextString += TMString.Substring(0, CharsPerLine - (TMString.Length - currIndex - 1));  //Add some 'fill in' characters at the end.
                                nextString += " ";
                            }
                            tmConfig.TROYmarkText.Add(nextString);
                        }
                        else
                        {
                            tmConfig.TROYmarkText.Add(TMString);
                        }
                    }

                    //1.0.8
                    tmConfig.StandardFontDefinition.SymbolSet = pmConfig.TroyMarkSymbolSet;
                    tmConfig.StandardFontDefinition.FontSpacing = pmConfig.TroyMarkFontSpacing;
                    tmConfig.StandardFontDefinition.Pitch = pmConfig.TroyMarkPitch;
                    tmConfig.StandardFontDefinition.Posture = pmConfig.TroyMarkPosture;
                    tmConfig.StandardFontDefinition.Height = (float)Convert.ToDecimal(pmConfig.TroyMarkHeight);
                    tmConfig.StandardFontDefinition.StrokeWeight = pmConfig.TroyMarkStrokeWeight;
                    tmConfig.StandardFontDefinition.Typeface = pmConfig.TroyMarkTypeface;

                    byte[] tmData;
                    if (Troy.TROYmarkPclBuilder.Interface.GetTroyMarkPcl(tmConfig, out tmData, false))
                    {
                        if (tmData != null)
                        {
                            outbuf.Write(tmData, 0, tmData.Length);
                            if (insertFF)
                            {
                                outbuf.Write("\u000c");
                            }
                        }
                    }
                    else
                    {
                        pmLogging.LogError("Error.  TROYmark PCL Builder did not produce TROYmark PCL", EventLogEntryType.Error, false);
                        return false;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                pmLogging.LogError("Error in InsertSoftwareTroymark.  Error: " + ex.Message, EventLogEntryType.Error, false);
                return false;
            }
        }

        private bool PrintTheJob(out LicenseQueryResult lqr)
        {
            try
            {
                if (printerFileName.Length > 0)
                {
                    if (!Licensing.IsPrinterLicensed(printToPrinterName, out lqr))
                    {
                        pmLogging.LogError("Max Printer Licenses reached.  Can not print to printer: " + printToPrinterName, EventLogEntryType.Warning, false);
                        return false;
                    }
                    if (!SendJobToPrinter(printToPrinterName, printerFileName, "TROY Port Monitor Print"))
                    {
                        pmLogging.LogError("Print failed.  Filename: " + printerFileName + " Printer Name: " + printToPrinterName, EventLogEntryType.Error, false);
                        return false;
                    }

                }
                else
                {
                    PortMonCustomException pme = new PortMonCustomException("Job was not printer.  Job filename: " + printFileName, true);
                    throw pme;
                }
                return true;
            }
            catch (PortMonCustomException pe)
            {
                lqr = null;
                pmLogging.LogError("Job was not printed.  File: " + printFileName + " Error: " + pe.Message, EventLogEntryType.Error, true);
                return false;

            }
            catch (Exception ex)
            {
                lqr = null;
                pmLogging.LogError("Job was not printed.  File: " + printFileName + " Error: " + ex.Message, EventLogEntryType.Error, true);
                return false;
            }
        }

        private bool EvaluateFontDescription(ref int cntr, ref int selectedFont, ref bool fontSelectedById)
        {
            try
            {
                int subcntr = 1;
                bool continueSubLoop = true, fontDescFound = false, fontCharMapFound = false;
                bool skipGlyphDef = false;

                cntr += 2;
                //Look for a D or E (*c#D is font description, *c#E is a char definition)
                while ((subcntr <= MAX_FONT_ID_DIGITS + 1) && (continueSubLoop))
                {
                    //Look for the D
                    if (inputBuffer[cntr + subcntr] == 0x44)
                    {
                        continueSubLoop = false;
                        fontDescFound = true;
                    }
                    //Look for the E
                    else if (inputBuffer[cntr + subcntr] == 0x45)
                    {
                        continueSubLoop = false;
                        fontCharMapFound = true;
                    }
                    //Look for non-numeric
                    else if ((inputBuffer[cntr + subcntr] < 0x30) ||
                             (inputBuffer[cntr + subcntr] > 0x39))
                    {
                        continueSubLoop = false;
                    }
                    subcntr++;
                }

                //if D was found
                if (fontDescFound)
                {
                    int fontIdFromFile = Convert.ToInt32(Encoding.ASCII.GetString(inputBuffer, cntr + 1, subcntr - 2));
                    if (fontIdFromFile < 1)
                    {
                        pmLogging.LogError("Unexpected font id value " + fontIdFromFile + ".  File: " + printFileName, EventLogEntryType.Warning, false);
                    }
                    cntr += subcntr;

                    // not <ESC>)s,  this marks the beginning of the Font Descriptor
                    //  if not a font descriptor then its most likely a character definition coming up
                    if (!((inputBuffer[cntr] == 0x1B) && (inputBuffer[cntr + 1] == 0x29) &&
                          (inputBuffer[cntr + 2] == 0x73)))
                    {
                        if (fontConfigList.ContainsKey(fontIdFromFile))
                        {
                            fontSelectedById = true;
                            selectedFont = fontIdFromFile;
                        }
                        else
                        {
                            fontSelectedById = false;
                            selectedFont = 0;
                        }
                    }
                    //beginning of font descriptor found
                    else
                    {
                        cntr += 2;
                        subcntr = 1;
                        continueSubLoop = true;

                        //loop through until the W is found
                        while ((subcntr <= MAX_FONT_DESC_DIGITS + 1) && (continueSubLoop))
                        {
                            //Look for the W
                            if (inputBuffer[cntr + subcntr] == 0x57)
                            {
                                continueSubLoop = false;
                            }
                            subcntr++;
                        }

                        //if W was found
                        if ((!continueSubLoop) && (subcntr > 1))
                        {
                            int fontDescLength = Convert.ToInt32(Encoding.ASCII.GetString(inputBuffer, cntr + 1, subcntr - 2));
                            if (fontIdFromFile < 1)
                            {
                                pmLogging.LogError("Unexpected font id value " + fontIdFromFile + ".  File: " + printFileName, EventLogEntryType.Warning, false);
                            }
                            cntr += subcntr;

                            if (FONT_NAME_LOC > fontDescLength)
                            {
                                pmLogging.LogError("Unexpected font font description length " + fontDescLength + ".  File: " + printFileName, EventLogEntryType.Warning, false);
                            }

                            // Extract the font name
                            string fontName = Encoding.ASCII.GetString(inputBuffer, cntr + FONT_NAME_LOC, 15);
                            fontName = fontName.TrimEnd('\0');

                            cntr += fontDescLength;
                            //If the font name is in the configuration font list then add an entry to the array of fonts 
                            if (fontConfigs.FontInList(fontName))
                            {
                                if (fontConfigList.ContainsKey(fontIdFromFile))
                                {
                                    fontConfigList.Remove(fontIdFromFile);
                                }
                                fontConfigList.Add(fontIdFromFile, fontConfigs.GetFontConfig(fontName));
                            }
                        }
                    }
                }
                else if (fontCharMapFound)
                {
                    //If we are in a font description
                    if (fontSelectedById)
                    {
                        //Get the font char id (32 thru 254)
                        int fontCharId = Convert.ToInt32(Encoding.ASCII.GetString(inputBuffer, cntr + 1, subcntr - 2));
                        skipGlyphDef = false;
                        if ((fontCharId < 32) || (fontCharId > 254))
                        {
                            pmLogging.LogError("Unexpected font char id value " + fontCharId + ".  File: " + printFileName, EventLogEntryType.Warning, false);
                            skipGlyphDef = true;
                        }

                        cntr += subcntr;

                        //If we don't see an <ESC>(s then we have an unexpected sequence
                        if (!((inputBuffer[cntr] == 0x1b) && (inputBuffer[cntr + 1] == 0x28) &&
                              (inputBuffer[cntr + 2] == 0x73)))
                        {
                            pmLogging.LogError("Unexpected sequence in font char definition at location " + cntr + ".  File: " + printFileName, EventLogEntryType.Warning, false);
                        }

                        cntr += 3;
                        subcntr = 1;
                        continueSubLoop = true;
                        int HoldStartPos = cntr;

                        while ((subcntr <= MAX_FONT_DESC_DIGITS + 1) && (continueSubLoop))
                        {
                            //Look for the W
                            if (inputBuffer[cntr + subcntr] == 0x57)
                            {
                                continueSubLoop = false;
                            }
                            subcntr++;
                        }

                        //Added for 2.0.  Skip over the font glyph definition stuff
                        int JumpCntr = Convert.ToInt32(Encoding.ASCII.GetString(inputBuffer, HoldStartPos, subcntr - 1));

                        cntr += subcntr;

                        if ((!continueSubLoop) && (!skipGlyphDef))
                        {
                            int glyphId = Convert.ToInt32(inputBuffer[cntr + 7]);
                            if (!(fontCharToGlyphMap.ContainsKey(selectedFont)))
                            {
                                GlyphMapType gmt = new GlyphMapType();
                                gmt.AddToGlyphMap(fontCharId, glyphId);
                                fontCharToGlyphMap.Add(selectedFont, gmt);
                            }
                            else
                            {
                                GlyphMapType gmt = fontCharToGlyphMap[selectedFont];
                                gmt.AddToGlyphMap(fontCharId, glyphId);
                            }
                        }

                        //cntr += 7;
                        //Added for 2.0.  Skip over the font glyph definition stuff
                        cntr += JumpCntr;

                        //Added to 2.0.  Discovered that glyph chars can show up immediately after a glyph definition without a cursor movement.
                        if (inputBuffer[cntr] != 0x1B)
                        {
                            EvaluateChars(ref cntr, ref selectedFont, ref fontSelectedById);
                        }
                    }
                    else
                    {
                        cntr += subcntr;
                    }
                }
                else
                {
                    cntr += subcntr;
                }
                return true;
            }
            catch (Exception ex)
            {
                pmLogging.LogError("Error in EvaluateFontDescription.  File: " + printFileName + " Error: " + ex.Message, EventLogEntryType.Error, true);
                return false;
            }
        }


        private bool InsertDemoTroyMark()
        {
            try
            {
                Troy.TROYmarkPclBuilder.Interface.TROYmarkConfiguration tmConfig = new Troy.TROYmarkPclBuilder.Interface.TROYmarkConfiguration();
                //tmConfig.Pattern = Troy.TROYmarkPclBuilder.Interface.TROYmarkPatternType.tmMedium;
                tmConfig.Pattern = Troy.TROYmarkPclBuilder.Interface.TROYmarkPatternType.tmDark;
                tmConfig.LineSpacing = 800;
                tmConfig.InclusionRegion = new Troy.TROYmarkPclBuilder.Interface.TroyMarkRegionType();
                tmConfig.InclusionRegion.XAnchor = 0;
                tmConfig.InclusionRegion.YAnchor = 0;
                tmConfig.InclusionRegion.Width = 4800;
                tmConfig.InclusionRegion.Height = 6400;

                string TMString = "";

                if (!InsertedDemoTroymark) TMString = "*** TROY - ";

                tmConfig.TROYmarkText.Add(TMString);

                byte[] tmData;
                if (Troy.TROYmarkPclBuilder.Interface.GetTroyMarkPcl(tmConfig, out tmData, true))
                {
                    if (tmData != null)
                    {
                        outbuf.Write(tmData, 0, tmData.Length);
                    }
                    else
                    {
                        pmLogging.LogError("Error.  TROYmark PCL Builder did not produce TROYmark PCL.  Can not insert Demo TROYmark.", EventLogEntryType.Error, false);
                        return false;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                pmLogging.LogError("Error in InsertDemoTroymark.  Error: " + ex.Message, EventLogEntryType.Error, false);
                return false;
            }

        }


        private bool EvaluateSymbolSet(ref int cntr, ref int selectedFont, ref bool fontSelectedById)
        {
            try
            {
                int subcntr = 1;
                bool continueSubLoop = true;

                cntr++;
                while ((subcntr <= MAX_FONT_ID_DIGITS + 1) && (continueSubLoop))
                {
                    //Look for the X
                    if (inputBuffer[cntr + subcntr] == 0x58)
                    {
                        continueSubLoop = false;
                    }
                    //Could be a V, if so exit with cntr at next ESC
                    else if (inputBuffer[cntr + subcntr] == 0x56)
                    {
                        cntr = cntr + subcntr + 1;
                        return false;
                    }
                    subcntr++;
                }

                //if X was found
                if ((!continueSubLoop) && (subcntr > 1))
                {
                    int fontIdFromFile = Convert.ToInt32(Encoding.ASCII.GetString(inputBuffer, cntr + 1, subcntr - 2));
                    if (fontIdFromFile < 1)
                    {
                        pmLogging.LogError("Unexpected font id value " + fontIdFromFile + ".  File: " + printFileName, EventLogEntryType.Warning, false);
                    }
                    cntr += subcntr;

                    if (fontConfigList.ContainsKey(fontIdFromFile))
                    {
                        fontSelectedById = true;
                        selectedFont = fontIdFromFile;
                    }
                    else
                    {
                        fontSelectedById = false;
                        selectedFont = 0;
                    }

                }
                else if (inputBuffer[cntr + 1] == 0x73)
                {
                    int HoldStartPos = cntr + 2;
                    cntr++;
                    bool inEscSeq = true;
                    while ((inEscSeq) && (cntr < inputBuffer.Length))
                    {
                        //Capital letter and @ mark the end of an Escape sequence
                        if ((inputBuffer[cntr] > 0x3F) && (inputBuffer[cntr] < 0x5B))
                        {
                            //If W then
                            if (inputBuffer[cntr] == 0x57)
                            {
                                int JumpCntr = Convert.ToInt32(Encoding.ASCII.GetString(inputBuffer, HoldStartPos, cntr - HoldStartPos));
                                cntr += JumpCntr;
                                inEscSeq = false;
                            }
                            else
                            {
                                inEscSeq = false;
                                cntr++;
                            }
                        }
                        else cntr++;
                    }

                }
                else
                {
                    //Find the end of the PCL string
                    cntr++;
                    bool inEscSeq = true;
                    while ((inEscSeq) && (cntr < inputBuffer.Length))
                    {
                        //Capital letter and @ mark the end of an Escape sequence
                        if ((inputBuffer[cntr] > 0x3F) && (inputBuffer[cntr] < 0x5B))
                        {
                            inEscSeq = false;
                            cntr++;
                        }
                        else cntr++;
                    }

                }

                return true;
            }
            catch (Exception ex)
            {
                pmLogging.LogError("Error in EvaluateSymbolSet.  File: " + printFileName + " Error: " + ex.Message, EventLogEntryType.Error, true);
                return false;
            }
        }

        private bool EvaluateChars(ref int cntr, ref int selectedFont, ref bool fontSelectedById)
        {
            try
            {
                bool deleteFont;
                int finalLength;
                bool lookForString = false;
                string capturedStr;
                byte[] leadingPclInsert = new byte[LengthOfLeadingPcl];
                byte[] trailingPclInsert = new byte[LengthOfTrailingPcl];
                char transAsciiChar;

                //Create an instance of the custom exception
                PortMonCustomException PortMonException;

                int posCntr;
                int translatedChar;

                //Initialize variables
                int saveStartLoc = cntr;
                int saveWriteLength = 4;
                int subcntr = 1;
                bool continueSubLoop = true;


                fontCharStr.Remove(0, fontCharStr.Length);
                cntr += 3;

                while ((subcntr <= MAX_POS_DIGITS + 1) && (continueSubLoop))
                {
                    //Look for trailing X or Y
                    if ((inputBuffer[cntr + subcntr] == 0x58) || (inputBuffer[cntr + subcntr] == 0x59))
                    {
                        saveWriteLength += subcntr;
                        cntr += subcntr + 1;
                        //If the next character is not an Escape then it a glyph map char
                        if (!(inputBuffer[cntr] == 0x1B))
                        {
                            continueSubLoop = false;
                            lookForString = true;
                        }
                        //else if the next chars are <ESC>*p then another positioning string is next
                        else if ((inputBuffer[cntr + 1] == 0x2A) &&
                                 (inputBuffer[cntr + 2] == 0x70))
                        {
                            subcntr = 1;
                            saveWriteLength += 4;
                            cntr += 3;
                        }
                        //else another escape sequence follows, not a glyph char map
                        else
                        {
                            lookForString = false;
                            continueSubLoop = false;
                        }
                    }
                    else subcntr++;
                }

                if (lookForString)
                {
                    int startOfGlyphChars = cntr;
                    int endOfGlyphChars = cntr;
                    continueSubLoop = true;
                    subcntr = 0;
                    int EndOfPageLen = 0;
                    while (continueSubLoop)
                    {
                        if (inputBuffer[cntr + subcntr] == 0x1B)
                        {
                            //if not *p then its the end of this string
                            //if not *p or *p6400Y<FF> then its the end of this string
                            //if ((!((inputBuffer[cntr + subcntr + 1] == 0x2A) && (inputBuffer[cntr + subcntr + 2] == 0x70))) ||
                            //    ((inputBuffer[cntr + subcntr + 1] == 0x2A) && (inputBuffer[cntr + subcntr + 2] == 0x70) &&
                            //      (inputBuffer[cntr + subcntr + 3] == 0x36) && (inputBuffer[cntr + subcntr + 4] == 0x34) &&
                            //      (inputBuffer[cntr + subcntr + 5] == 0x30) && (inputBuffer[cntr + subcntr + 6] == 0x30) &&
                            //      (inputBuffer[cntr + subcntr + 7] == 0x59) && (inputBuffer[cntr + subcntr + 8] == 0x0C)))
                            if ((!((inputBuffer[cntr + subcntr + 1] == 0x2A) && (inputBuffer[cntr + subcntr + 2] == 0x70))) ||
                                (CheckForEndOfPage(cntr + subcntr,ref EndOfPageLen)))
                            {
                                cntr += subcntr;
                                continueSubLoop = false;
                                capturedStr = fontCharStr.ToString();
                                if (capturedStr.Length < 1)
                                {
                                    //Nothing to do. Exit and do nothing
                                }
                                else
                                {
                                    if (fontConfigList.ContainsKey(selectedFont))
                                    {
                                        if (fontConfigList[selectedFont].FontType.ToUpper() == "DATA CAPTURE")
                                        {
                                            if (fontConfigList[selectedFont].UseForTroyMark)
                                            {
                                                if (capturedStr.EndsWith("  "))
                                                {
                                                    capturedStr = capturedStr.TrimEnd(' ');
                                                    capturedStr += " ";
                                                }
                                                if (fontConfigList[selectedFont].UseAllDataForTm)
                                                {
                                                    TroymarkAllData += capturedStr;
                                                }
                                                else
                                                {
                                                    dataCapture += capturedStr;
                                                }
                                            }

                                            if (fontConfigList[selectedFont].UseForMicroPrint)
                                            {
                                                if (capturedStr.EndsWith("  "))
                                                {
                                                    capturedStr = capturedStr.TrimEnd(' ');
                                                    capturedStr += " ";
                                                }
                                                if (fontConfigList[selectedFont].UseAllDataForMp)
                                                {
                                                    MpAllData += capturedStr;
                                                }
                                                else
                                                {
                                                    MpCapturedData += capturedStr;
                                                }
                                            }

                                            if ((fontConfigList[selectedFont].UseForPassThrough) && (pmConfig.PassThroughEnabled))
                                            {
                                                PassThroughString += capturedStr;
                                            }

                                            if ((fontConfigList[selectedFont].UseForPrinterMap))
                                            {
                                                PrinterMapString += capturedStr;
                                            }
                                            if (fontConfigList[selectedFont].RemoveData)
                                            {
                                                //Remove the text
                                                if (endOfGlyphChars > startOfGlyphChars)
                                                {
                                                    newEventPoint = new EventPoints(currentPageNum, startOfGlyphChars, EventPointType.epSubstitute, endOfGlyphChars - startOfGlyphChars + 1);
                                                    fileEventPoints.Add(newEventPoint);
                                                }
                                            }

                                        }
                                        else if (fontConfigList[selectedFont].FontType.ToUpper() == "DATA TOKENS")
                                        {
                                            taggedDataList.Add(capturedStr);
                                        }
                                        else if (fontConfigList[selectedFont].FontType.ToUpper() == "PRINTER NAME")
                                        {
                                            printToPrinterName = capturedStr;
                                        }
                                        else if (fontConfigList[selectedFont].FontType.ToUpper() == "PASS THROUGH")
                                        {
                                            if (pmConfig.PassThroughEnabled)
                                            {
                                                PassThroughString += capturedStr;
                                            }
                                        }
                                        else
                                        {
                                            newEventPoint = new EventPoints(currentPageNum, startOfGlyphChars, EventPointType.epSubstitute, cntr - startOfGlyphChars);
                                            fileEventPoints.Add(newEventPoint);
                                            deleteFont = false;
                                            finalLength = LengthOfLeadingPcl;
                                            if (fontConfigList[selectedFont].GetLeadingPcl(ref leadingPclInsert, ref finalLength, ref deleteFont))
                                            {
                                                if (!deleteFont)
                                                {
                                                    insertString = new InsertStrings();
                                                    insertString.RemoveString = false;
                                                    insertString.PageNumber = currentPageNum;
                                                    insertString.Location = startOfGlyphChars;
                                                    insertString.MoveToInsertLocation = false;
                                                    insertString.SetLeadingPcl(leadingPclInsert, finalLength);
                                                    insertString.ConvertedAscii = capturedStr;
                                                    if (fontConfigList[selectedFont].FontType == "Configuration Page")
                                                    {
                                                        currentPage.ConfigPage = true;
                                                    }
                                                    finalLength = LengthOfTrailingPcl;
                                                    if (fontConfigList[selectedFont].GetTrailingPcl(ref trailingPclInsert, ref finalLength))
                                                    {
                                                        insertString.TrailingPclLength = finalLength;
                                                        if (finalLength > 0)
                                                        {
                                                            insertString.SetTrailingPcl(trailingPclInsert, finalLength);
                                                        }
                                                    }
                                                    insertStringList.Add(insertString);
                                                }
                                                else
                                                {
                                                    //TBD don't write out anything but use the info for configuration
                                                }

                                            }
                                            else
                                            {
                                                pmLogging.LogError("Leading PCL not found for font id " + selectedFont.ToString() + ".  File: " + printFileName, EventLogEntryType.Warning, false);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        PortMonException = new PortMonCustomException("Font Configuration entry not found for the font id " + selectedFont.ToString() + ".  File: " + printFileName, true, EventLogEntryType.Error, false);
                                        throw PortMonException;
                                    }
                                }
                            }
                            //else find the trailing X or Y then push the cntr ahead
                            else
                            {
                                subcntr += 3;
                                posCntr = 1;
                                while ((inputBuffer[cntr + subcntr] != 0x58) &&
                                       (inputBuffer[cntr + subcntr] != 0x59) &&
                                       (posCntr <= MAX_POS_DIGITS))
                                {
                                    posCntr++;
                                    subcntr++;
                                }
                                if (posCntr > MAX_POS_DIGITS + 1)
                                {
                                    PortMonException = new PortMonCustomException("Unexpected length of XY coordinate PCL and location " + cntr.ToString() + ". File: " + printFileName, true, EventLogEntryType.Error, false);
                                    throw PortMonException;
                                }
                            }
                        }
                        else
                        {
                            //Translate the glyph character to the equivalent ASCII, add to the string
                            if (fontCharToGlyphMap.ContainsKey(selectedFont))
                            {
                                GlyphMapType gmt = fontCharToGlyphMap[selectedFont];
                                translatedChar = gmt.GetGlyphId(Convert.ToInt32(inputBuffer[cntr + subcntr]));
                                if (fontConfigList.ContainsKey(selectedFont))
                                {
                                    TroyFontInfo fc2 = fontConfigList[selectedFont];
                                    transAsciiChar = fc2.GetCharFromGlyphId(translatedChar);
                                    if (transAsciiChar == '\0')
                                    {

                                    }
                                    else
                                    {
                                        fontCharStr.Append(transAsciiChar);
                                        endOfGlyphChars = cntr + subcntr;
                                    }
                                }
                            }

                            /*
                            //Translate the glyph character to the equivalent ASCII, add to the string
                            translatedChar = fontCharToGlyphMap[selectedFont, Convert.ToInt32(inputBuffer[cntr + subcntr])];
                            if (fontConfigList[selectedFont] != null)
                            {
                                transAsciiChar = fontConfigList[selectedFont].GetCharFromGlyphId(translatedChar);
                                if (transAsciiChar == '\0')
                                {

                                }
                                else
                                {
                                    fontCharStr.Append(transAsciiChar);
                                }
                            }
                             */
                            else
                            {
                                PortMonException = new PortMonCustomException("Font Configuration entry not found in glyph if map for the font id " + selectedFont.ToString() + ".  File: " + printFileName, true, EventLogEntryType.Error, false);
                                throw PortMonException;
                            }
                        }
                        subcntr++;
                    }
                }
                fontSelectedById = false;
                selectedFont = 0;


                return true;
            }
            catch (PortMonCustomException pme)
            {
                throw pme;
            }
            catch (Exception ex)
            {
                pmLogging.LogError("Error in EvaluateChars.  File: " + printFileName + " Error: " + ex.Message, EventLogEntryType.Error, true);
                return false;
            }

        }

        private bool AddPantograph(int currPage)
         {
             try
             {
                 var bp = new BuildPantographWrap.Wrapper();
                 string basepath = printJobFileInfo.DirectoryName;
                 string configpath = basepath +"\\Config";
                 string datapath = basepath + "\\Data";
                 int rc = bp.ManagedCreatePantograph(basepath, configpath, datapath);
                 
                 foreach (string substring in pmConfig.PrintPantographProfileList.Split(','))
                 {
                     int num = Int32.Parse(substring);
                     if ((pantoConfig.PantographConfigurations.Count >= num) && (num > 0))
                     {
                         CustomConfiguration cc = pantoConfig.PantographConfigurations[num - 1];
                         if (cc == null) continue;
                         if (cc.UseDynamicMp)
                         {
                             if (MpDataPerPage.ContainsKey(currPage))
                             {
                                 cc.BorderString += " " + MpDataPerPage[currPage].ToUpper();
                             }
                         }
                         //string filename = "temppg" + DateTime.Now.ToString("hhmmssffffddMMyyyy") + ".pnt";
                         //string tmpfile = Path.Combine(printJobFileInfo.DirectoryName, filename);
                         //string fontpath = Path.Combine(printJobFileInfo.DirectoryName, "Data");
                         //BuildPantograph bp = new BuildPantograph();
                         //bp.CreatePantographPcl(tmpfile, cc, fontpath, pmLogging.ErrorLogFilePath, !MpFontDownloaded);

                         string tmpfile = Path.Combine(printJobFileInfo.DirectoryName, "Data\\PantographProfile" + substring + "Page1.pcl");
                         FileStream pgFile = new FileStream(tmpfile, FileMode.Open, FileAccess.Read, FileShare.Read);
                         BinaryReader binReader = new BinaryReader(pgFile);
                         byte[] inBuffer = new Byte[pgFile.Length];
                         inBuffer = binReader.ReadBytes(Convert.ToInt32(pgFile.Length));
                         outbuf.Write(inBuffer, 0, inBuffer.Length);
                         binReader.Close();
                     }
                 }
                 return true;
             }
             catch (Exception ex)
            {
                return false;
            }
        }

        private bool GetEnableDuplex(int DuplexValue, ref byte[] pcl, ref int pclLength)
        {
            try
            {
                byte[] Duplex;
                Duplex = new UTF8Encoding(true).GetBytes(DuplexValue.ToString());
                int DuplexLength = Duplex.Length;

                if (pclLength < (4 + DuplexLength))
                {

                }
                else
                {
                    pcl[0] = ESC;
                    pcl[1] = 0x26; //&
                    pcl[2] = 0x6C; //l

                    int cntr;
                    for (cntr = 0; cntr < DuplexLength; cntr++)
                    {
                        pcl[3 + cntr] = Duplex[cntr];
                    }

                    pcl[3 + cntr] = 0x53; //S
                    pclLength = cntr + 4;


                }
                return true;
            }
            catch (Exception ex)
            {
                pmLogging.LogError("Error in GetEnableDuplex.  File: " + printFileName + " Error: " + ex.Message, EventLogEntryType.Error, false);
                return false;
            }
        }

        private bool WriteOutPjl()
        {
            try
            {
                string PjlString = "";
                if (EndPjlLocation > -1)
                {
                    PjlString = new UTF8Encoding(true).GetString(inputBuffer, 0, EndPjlLocation - 26);
                }
                else
                {
                    PjlString = "\u001B%-12345X@PJL COMMENT **** INSERTED BY TSPE ****\u000D\u000A";
                }

                int pos = PjlString.IndexOf("@PJL SET HOLD");
                if (pos > -1)
                {
                    int pos2 = PjlString.IndexOf("\u000A", pos);
                    if (pos2 > pos)
                    {
                        PjlString = PjlString.Remove(pos, pos2 - pos + 1);
                    }
                }
                PjlString += "@PJL SET HOLD=OFF\u000A";

                pos = PjlString.IndexOf("@PJL SET HOLDTYPE");
                if (pos > -1)
                {
                    int pos2 = PjlString.IndexOf("\u000A", pos);
                    if (pos2 > pos)
                    {
                        PjlString = PjlString.Remove(pos, pos2 - pos + 1);
                    }
                }

                pos = PjlString.IndexOf("@PJL SET DUPLICATEJOB");
                if (pos > -1)
                {
                    int pos2 = PjlString.IndexOf("\u000A", pos);
                    if (pos2 > pos)
                    {
                        PjlString = PjlString.Remove(pos, pos2 - pos + 1);
                    }
                }

                pos = PjlString.IndexOf("@PJL SET HOLDKEY");
                if (pos > -1)
                {
                    int pos2 = PjlString.IndexOf("\u000A", pos);
                    if (pos2 > pos)
                    {
                        PjlString = PjlString.Remove(pos, pos2 - pos + 1);
                    }
                }


                PjlString += PJL_DEFAULT_DENSITY;
                PjlString += PJL_SET_ECONOMODE_OFF;
                PjlString += PJL_SET_REPRINT_OFF;

                int tmpi = PjlString.IndexOf("@PJL SET RESOLUTION=300");
                if (tmpi > -1)
                {
                    PjlString = PjlString.Remove(tmpi, 24);
                }

                byte[] outBuffer = new UTF8Encoding(true).GetBytes(PjlString);
                outbuf.Write(outBuffer, 0, outBuffer.Length);

                if (EndPjlLocation > -1)
                {
                    outbuf.Write(inputBuffer, EndPjlLocation - 26, 27);
                }
                else
                {
                    string tmp = "@PJL ENTER LANGUAGE = PCL\u000D\u000A\u001BE";
                    byte[] tmpBuffer = new UTF8Encoding(true).GetBytes(tmp);
                    outbuf.Write(tmpBuffer, 0, tmpBuffer.Length);
                }

                return true;
            }
            catch (Exception ex)
            {
                pmLogging.LogError("Error in WriteOutPjl.  File: " + printFileName + " Error: " + ex.Message, EventLogEntryType.Error, false);
                return false;
            }
        }



        private void FileCleanup()
        {
            bool cont = true;
            int loopCntr = 0;
            FileInfo fileFromPM;
            FileInfo tempFile;
            fileFromPM = new FileInfo(printFileName);
            tempFile = new FileInfo(tempFileName);

            while (cont)
            {
                try
                {
                    if ((!fileFromPM.Exists) && (!tempFile.Exists))
                    {
                        cont = false;
                    }
                    else if (pmConfig.DebugBackupFilesPath.Length > 0)
                    {
                        string mainFileBackup, tempFileBackup, testFileBackup;
                        mainFileBackup = pmConfig.DebugBackupFilesPath + fileFromPM.Name;
                        tempFileBackup = pmConfig.DebugBackupFilesPath + tempFile.Name;
                        testFileBackup = mainFileBackup;
                        FileInfo CheckBackupMainFile = new FileInfo(testFileBackup);
                        int cntr = 0;
                        while (CheckBackupMainFile.Exists)
                        {
                            cntr++;
                            testFileBackup = mainFileBackup + cntr.ToString();
                            CheckBackupMainFile = new FileInfo(testFileBackup);
                        }
                        if (fileFromPM.Exists) fileFromPM.MoveTo(testFileBackup);

                        testFileBackup = tempFileBackup;
                        FileInfo CheckBackupTempFile = new FileInfo(testFileBackup);
                        cntr = 0;
                        while (CheckBackupTempFile.Exists)
                        {
                            cntr++;
                            testFileBackup = tempFileBackup + cntr.ToString();
                            CheckBackupTempFile = new FileInfo(testFileBackup);
                        }
                        if (tempFile.Exists) tempFile.MoveTo(testFileBackup);
                    }
                    else
                    {
                        fileFromPM.Delete();
                        if (tempFileName != "") tempFile.Delete();
                    }

                    cont = false;
                }
                catch (Exception ex)
                {
                    //try this for 15 seconds
                    Thread.Sleep(100);
                    if (loopCntr++ > 150)
                    {
                        cont = false;
                        if (fileFromPM.Exists && fileFromPM.IsReadOnly)
                            pmLogging.LogError("Error in FileCleanup. File: " + printFileName + " File is read only.", EventLogEntryType.Error, true);
                        else if (tempFile.Exists && tempFile.IsReadOnly)
                            pmLogging.LogError("Error in FileCleanup. File: " + tempFile.Name + " File is read only.", EventLogEntryType.Error, true);
                        else
                            pmLogging.LogError("Error in FileCleanup.  File: " + printFileName + " Error: " + ex.Message, EventLogEntryType.Error, true);
                    }
                    else cont = true;
                }
            }
        }
    }
}
