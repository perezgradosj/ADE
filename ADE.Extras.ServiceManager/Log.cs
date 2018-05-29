using System;
using System.IO;

namespace ADE.Extras.ServiceManager
{
    public class Log
    {
        public static Log Instance
        {
            get
            {
                if (_Instance == null)
                {
                    lock (_SyncRoot)
                    {
                        if (_Instance == null)
                            _Instance = new Log();
                    }
                }

                return _Instance;
            }
        }

        private static volatile Log _Instance;
        private static object _SyncRoot = new Object();

        private Log()
        {
            LogFileName = "ServiceManagerLog";
            LogFileExtension = ".log";
        }

        public StreamWriter Writer { get; set; }

        public string LogPath { get; set; }

        public string LogFileName { get; set; }

        public string LogFileExtension { get; set; }

        public string LogFile { get { return LogFileName + LogFileExtension; } }

        public string LogFullPath { get { return Path.Combine(LogPath, LogFile); } }

        public bool LogExists { get { return File.Exists(LogFullPath); } }

        public void WriteLineToLog(String inLogMessage)
        {
            WriteToLog(inLogMessage);
        }

        public void WriteToLog(String inLogMessage)
        {
            if (!Directory.Exists(LogPath))
            {
                Directory.CreateDirectory(LogPath);
            }
            if (Writer == null)
            {
                Writer = new StreamWriter(LogFullPath, true);
            }

            Writer.WriteLine(" [{0:G}] {1}.", System.DateTime.Now, inLogMessage);
            Writer.Flush();
        }

        public static void WriteLine(String inLogMessage)
        {
            Instance.WriteLineToLog(inLogMessage);
        }

        public static void Write(String inLogMessage)
        {
            Instance.WriteToLog(inLogMessage);
        }
    }
}
