using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TroyPortMonitorService
{
    class PortMonConfig
    {
        private string folderMonitorLocation;
        public string FolderMonitorLocation
        {
            get { return folderMonitorLocation; }
            set { folderMonitorLocation = value; }
        }

        private int fileReadDelay;
        public int FileReadDelay
        {
            get { return fileReadDelay; }
            set { fileReadDelay = value; }
        }

        private int queueDelay;
        public int QueueDelay
        {
            get { return queueDelay; }
            set { queueDelay = value; }
        }

        private int maxThreads;
        public int MaxThreads
        {
            get { return maxThreads; }
            set { maxThreads = value; }
        }

        private string fileExtension;
        public string FileExtension
        {
            get { return fileExtension; }
            set { fileExtension = value; }
        }

        private string filePrefix;
        public string FilePrefix
        {
            get { return filePrefix; }
            set { filePrefix = value; }
        }

        private string debugBackupFilesTo;
        public string DebugBackupFilesTo
        {
            get { return debugBackupFilesTo; }
            set { debugBackupFilesTo = value; }
        }

        private string defaultPrinter;
        public string DefaultPrinter
        {
            get { return defaultPrinter; }
            set { defaultPrinter = value; }
        }

        private string defaultTroyMarkPattern;
        public string DefaultTroyMarkPattern
        {
            get { return defaultTroyMarkPattern; }
            set { defaultTroyMarkPattern = value; }
        }

        private string defaultTroyMarkInclusionX;
        public string DefaultTroyMarkInclusionX
        {
            get { return defaultTroyMarkInclusionX; }
            set { defaultTroyMarkInclusionX = value; }
        }
        
        private string defaultTroyMarkInclusionY;
        public string DefaultTroyMarkInclusionY
        {
            get { return defaultTroyMarkInclusionY; }
            set { defaultTroyMarkInclusionY = value; }
        }

        private string defaultTroyMarkInclusionWidth;
        public string DefaultTroyMarkInclusionWidth
        {
            get { return defaultTroyMarkInclusionWidth; }
            set { defaultTroyMarkInclusionWidth = value; }
        }

        private string defaultTroyMarkInclusionHeight;
        public string DefaultTroyMarkInclusionHeight
        {
            get { return defaultTroyMarkInclusionHeight; }
            set { defaultTroyMarkInclusionHeight = value; }
        }


        private string defaultTroyMarkExclusionX;
        public string DefaultTroyMarkExclusionX
        {
            get { return defaultTroyMarkExclusionX; }
            set { defaultTroyMarkExclusionX = value; }
        }

        private string defaultTroyMarkExclusionY;
        public string DefaultTroyMarkExclusionY
        {
            get { return defaultTroyMarkExclusionY; }
            set { defaultTroyMarkExclusionY = value; }
        }

        private string defaultTroyMarkExclusionWidth;
        public string DefaultTroyMarkExclusionWidth
        {
            get { return defaultTroyMarkExclusionWidth; }
            set { defaultTroyMarkExclusionWidth = value; }
        }

        private string defaultTroyMarkExclusionHeight;
        public string DefaultTroyMarkExclusionHeight
        {
            get { return defaultTroyMarkExclusionHeight; }
            set { defaultTroyMarkExclusionHeight = value; }
        }


        private string encryptionType;
        public string EncryptionType
        {
            get { return encryptionType; }
            set { encryptionType = value; }
        }

        private string encryptionPassword;
        public string EncryptionPassword
        {
            get { return encryptionPassword; }
            set { encryptionPassword = value; }
        }

        private bool useTroyMark;
        public bool UseTroyMark
        {
            get { return useTroyMark; }
            set { useTroyMark = value; }
        }

        private string accountUserName;
        public string AccountUserName
        {
            get { return accountUserName; }
            set { accountUserName = value; }
        }

        private string accountPassword;
        public string AccountPassword
        {
            get { return accountPassword; }
            set { accountPassword = value; }
        }

        private bool defaultPaperTrayMapping;
        public bool DefaultPaperTrayMapping
        {
            get { return defaultPaperTrayMapping; }
            set { defaultPaperTrayMapping = value; }
        }

        private bool enableMicrMode;
        public bool EnableMicrMode
        {
            get { return enableMicrMode; }
            set { enableMicrMode = value; }
        }

        private bool autoPageRotate;
        public bool AutoPageRotate
        {
            get { return autoPageRotate; }
            set { autoPageRotate = value; }
        }

        private string printerPin;
        public string PrinterPin
        {
            get { return printerPin; }
            set { printerPin = value; }
        }

        private string micrPin;
        public string MicrPin
        {
            get { return micrPin; }
            set { micrPin = value; }
        }

        private string jobPin;
        public string JobPin
        {
            get { return jobPin; }
            set { jobPin = value; }
        }

        private string jobName;
        public string JobName
        {
            get { return jobName; }
            set { jobName = value; }
        }

        private string altEsc;
        public string AltEsc
        {
            get { return altEsc; }
            set { altEsc = value; }
        }

        private string hpJobPassword;
        public string HpJobPassword
        {
            get { return hpJobPassword; }
            set { hpJobPassword = value; }
        }

        private int enablePantograph;
        public int EnablePantograph
        {
            get { return enablePantograph; }
            set { enablePantograph = value; }
        }

        private int enableDuplex;
        public int EnableDuplex
        {
            get { return enableDuplex; }
            set { enableDuplex = value; }
        }

        private bool pjlTroyMarkDataPresent;
        public bool PjlTroyMarkDataPresent
        {
            get { return pjlTroyMarkDataPresent; }
            set { pjlTroyMarkDataPresent = value; }
        }

        private bool pjlTroyMarkInsertPage;
        public bool PjlTroyMarkInsertPage
        {
            get { return pjlTroyMarkInsertPage; }
            set { pjlTroyMarkInsertPage = value; }
        }

        private string defaultPantographInclusionX;
        public string DefaultPantographInclusionX
        {
            get { return defaultPantographInclusionX; }
            set { defaultPantographInclusionX = value; }
        }

        private string defaultPantographInclusionY;
        public string DefaultPantographInclusionY
        {
            get { return defaultPantographInclusionY; }
            set { defaultPantographInclusionY = value; }
        }

        private string defaultPantographInclusionWidth;
        public string DefaultPantographInclusionWidth
        {
            get { return defaultPantographInclusionWidth; }
            set { defaultPantographInclusionWidth = value; }
        }

        private string defaultPantographInclusionHeight;
        public string DefaultPantographInclusionHeight
        {
            get { return defaultPantographInclusionHeight; }
            set { defaultPantographInclusionHeight = value; }
        }

        private string defaultPantographExclusionX;
        public string DefaultPantographExclusionX
        {
            get { return defaultPantographExclusionX; }
            set { defaultPantographExclusionX = value; }
        }

        private string defaultPantographExclusionY;
        public string DefaultPantographExclusionY
        {
            get { return defaultPantographExclusionY; }
            set { defaultPantographExclusionY = value; }
        }

        private string defaultPantographExclusionWidth;
        public string DefaultPantographExclusionWidth
        {
            get { return defaultPantographExclusionWidth; }
            set { defaultPantographExclusionWidth = value; }
        }

        private string defaultPantographExclusionHeight;
        public string DefaultPantographExclusionHeight
        {
            get { return defaultPantographExclusionHeight; }
            set { defaultPantographExclusionHeight = value; }
        }

        private string defaultTroyMarkSpacing;
        public string DefaultTroyMarkSpacing
        {
            get { return defaultTroyMarkSpacing; }
            set { defaultTroyMarkSpacing = value; }
        }

        private string printPantographProfileList;
        public string PrintPantographProfileList
        {
            get { return printPantographProfileList; }
            set { printPantographProfileList = value; }
        }

        private string troyMarkCharsPerLine;
        public string TroyMarkCharsPerLine
        {
            get { return troyMarkCharsPerLine; }
            set { troyMarkCharsPerLine = value; }
        }

        private string endOfPagePclString;
        public string EndOfPagePclString
        {
            get { return endOfPagePclString; }
            set
            {
                endOfPagePclString = value;
                endOfPagePclString = endOfPagePclString.Replace("/e", "\u001B"); //Escape
                endOfPagePclString = endOfPagePclString.Replace("/f", "\u000C"); //Form Feed
                endOfPagePclString = endOfPagePclString.Replace("/n", "\u000D"); //Carriage Return
                endOfPagePclString = endOfPagePclString.Replace("/r", "\u000A"); //Line Feed
            }
        }

        private string insertPointPclString;
        public string InsertPointPclString
        {
            get { return insertPointPclString; }
            set
            {
                insertPointPclString = value;
                insertPointPclString = insertPointPclString.Replace("/e", "\u001B"); //Escape
                insertPointPclString = insertPointPclString.Replace("/f", "\u000C"); //Form Feed
                insertPointPclString = insertPointPclString.Replace("/n", "\u000D"); //Carriage Return
                insertPointPclString = insertPointPclString.Replace("/r", "\u000A"); //Line Feed
            }
        }
        private int maxPassThroughSize;
        public int MaxPassThroughSize
        {
            get { return maxPassThroughSize; }
            set { maxPassThroughSize = value; }
        }

        public List<string> PassThroughStringList = new List<string>();

        private bool passThroughEnabled;
        public bool PassThroughEnabled
        {
            get { return passThroughEnabled; }
            set { passThroughEnabled = value; }
        }

        private bool troyMarkOnBack;
        public bool TroyMarkOnBack
        {
            get { return troyMarkOnBack; }
            set { troyMarkOnBack = value; }
        }



    }
}
