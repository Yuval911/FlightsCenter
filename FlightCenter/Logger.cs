using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Diagnostics;

namespace FlightCenter
{
    /// <summary>
    /// This class handles the log creation for actions and exceptions in the application.
    /// By design, this class won't log when the programs runs on a unit test.
    /// </summary>
    public static class Logger
    {
        static string workingDirectory = AppDomain.CurrentDomain.BaseDirectory;
        static string configFileName = "logger.config";
        static DirectoryInfo logsDirectory = new DirectoryInfo(@"C:\\FlightsLogs\\logs");
        static FileInfo currentLogFile;
        static int currentLogNumber;
        static int maxFileSize;
        static object writerKey = new object();

        static Logger()
        {
            if (AppDomain.CurrentDomain.BaseDirectory.Contains("UnitTests"))
                return;

            InitializeLogger();
        }

        /// <summary>
        /// Logs the given message.
        /// </summary>
        public static void Log(LogLevel level, string message)
        {
            if (AppDomain.CurrentDomain.BaseDirectory.Contains("UnitTests"))
                return;

            HandleFileSize();

            string date = DateTime.Now.ToString("dd-MM-yyy HH:mm:ss");
            string logLevel = level.ToString();

            string caller = GetCallerInfo();

            string log = $"{date} [{logLevel}] {caller} : {message}";

            WriteLogToFile(log, addBorders: false);
        }

        /// <summary>
        /// Logs the given application exception.
        /// </summary>
        public static void Log(LogLevel level, Exception exception)
        {
            if (AppDomain.CurrentDomain.BaseDirectory.Contains("UnitTests"))
                return;

            HandleFileSize();

            string date = DateTime.Now.ToString("dd-MM-yyy HH:mm:ss");
            string logLevel = level.ToString();

            string stackTrace = GetStackTrace();

            string log = $"{date} [{logLevel}] {exception.ToString()} Stack Trace: {stackTrace}";

            WriteLogToFile(log, addBorders: true);
        }

        private static void WriteLogToFile(string log, bool addBorders)
        {
            lock(writerKey)
            {
                using (StreamWriter file = new StreamWriter(currentLogFile.FullName, append: true))
                {
                    if (addBorders)
                    {
                        file.WriteLine("######################");
                        file.WriteLine(log);
                        file.WriteLine("######################");
                    }
                    else
                    {
                        file.WriteLine(log);
                    }
                }
            }
        }

        private static void InitializeLogger()
        {
            Configure();
            GetLatestLogFile();
        }

        private static string GetStackTrace()
        {
            string fullStackTrace = new StackTrace().ToString();

            List<string> stacks = fullStackTrace.Split("\n\r".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();

            stacks = stacks.Where(stack =>
            {
                if (!stack.Contains("Logger"))
                    return true;
                else
                    return false;
            }).ToList();

            StringBuilder stackTraceBuilder = new StringBuilder("");

            foreach (string stack in stacks)
            {
                stackTraceBuilder.Append(stack + "  <--");
            }

            return stackTraceBuilder.ToString();
        }

        private static string GetCallerInfo()
        {
            string stackTrace = GetStackTrace();

            List<string> elements = stackTrace.Split(new string[] { "<--" }, StringSplitOptions.RemoveEmptyEntries).ToList();

            string callerInfo = elements[0];

            callerInfo = callerInfo.Remove(0, 6);
            callerInfo = callerInfo.Remove(callerInfo.Length - 2, 2);

            return callerInfo;
        }

        [DllImport("kernel32.dll")]
        static extern uint GetCompressedFileSizeW([In, MarshalAs(UnmanagedType.LPWStr)] string lpFileName,
                                                          [Out, MarshalAs(UnmanagedType.U4)] out uint lpFileSizeHigh);

        [DllImport("kernel32.dll", SetLastError = true, PreserveSig = true)]
        static extern int GetDiskFreeSpaceW([In, MarshalAs(UnmanagedType.LPWStr)] string lpRootPathName,
                                                    out uint lpSectorsPerCluster, out uint lpBytesPerSector, out uint lpNumberOfFreeClusters,
                                                    out uint lpTotalNumberOfClusters);

        private static void HandleFileSize()
        {
            long logFileSize = GetLogFileSizeOnDisk();

            if (logFileSize >= maxFileSize - 10)
            {
                RollToNextLogFile();
            }
        }

        private static long GetLogFileSizeOnDisk()
        {
            string fileFullPath = currentLogFile.FullName;
            uint dummy, sectorsPerCluster, bytesPerSector;
            int result = GetDiskFreeSpaceW(currentLogFile.Directory.Root.FullName, out sectorsPerCluster, out bytesPerSector, out dummy, out dummy);
            if (result == 0)
                throw new Exception("An error occurred while calculating the log file size.");
            uint clusterSize = sectorsPerCluster * bytesPerSector;
            uint hosize;
            uint losize = GetCompressedFileSizeW(fileFullPath, out hosize);
            long size;
            size = (long)hosize << 32 | losize;

            return ((size + clusterSize - 1) / clusterSize) * clusterSize;
        }

        private static void RollToNextLogFile()
        {
            currentLogNumber++;

            string newLogFileName = $"log_{currentLogNumber}.txt";

            string newLogFileFullPath = $"{logsDirectory.FullName}\\{newLogFileName}";

            FileStream fs = File.Create(newLogFileFullPath);

            fs.Close();

            currentLogFile = new FileInfo(newLogFileFullPath);

        }

        private static void Configure()
        {
            XmlDocument config = new XmlDocument();

            config.Load(workingDirectory + configFileName);

            string maxFileSizeString = config.DocumentElement.SelectSingleNode("/logger/maxFileSize").Attributes["value"].Value;

            maxFileSize = ConvertSizeStringToBytesNumber(maxFileSizeString);
        }

        private static int ConvertSizeStringToBytesNumber(string sizeString)
        {
            if (sizeString.Contains("MB"))
            {
                int sizeInMB = Convert.ToInt32(sizeString.Split(new char[] { 'M', 'B' }, StringSplitOptions.RemoveEmptyEntries)[0]);
                return sizeInMB * 1000000;
            }
            else if (sizeString.Contains("KB"))
            {
                int sizeInKB = Convert.ToInt32(sizeString.Split(new char[] { 'K', 'B' }, StringSplitOptions.RemoveEmptyEntries)[0]);
                return sizeInKB * 1000;
            }
            else
            {
                return 2000000;
            }
        }

        private static void GetLatestLogFile()
        {
            Directory.CreateDirectory(logsDirectory.FullName);

            List<FileInfo> files = logsDirectory.GetFiles("*.txt").ToList();

            if (files.Count == 0)
            {
                CreateInitialLogFile();
                return;
            }

            currentLogFile = files[files.Count - 1];
            currentLogNumber = files.IndexOf(currentLogFile);
        }

        private static void CreateInitialLogFile()
        {
            string newFileDate = DateTime.Now.ToString("ddMMyyyy");

            string newLogFileName = $"log_0.txt";

            string newLogFileFullPath = $"{logsDirectory.FullName}\\{newLogFileName}";

            FileStream fs = File.Create(newLogFileFullPath);

            fs.Close();

            currentLogFile = new FileInfo(newLogFileFullPath);
            currentLogNumber = 0;
        }
    }
}

/// <summary>
/// This enum is being used by the Logger class to describe the log levels.
/// </summary>
public enum LogLevel
{
    Debug, Info, Warning, Error, Fatal
}
