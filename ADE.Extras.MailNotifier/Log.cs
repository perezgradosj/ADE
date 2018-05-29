using System;
using System.IO;
using System.Text;
using System.Threading;

namespace ADE.Extras.MailNotifier
{
    public class Log
    {
        StringBuilder sb01 = new StringBuilder();
        public MainSettings MS = null;
        private string name = "";

        public Log(MainSettings MSX)
        {
            MS = MSX;
            name = DateTime.Now.Year + "-" + DateTime.Now.Month.ToString().PadLeft(2, '0');
            MS.LOGS += name + @"\";
            Directory.CreateDirectory(MS.LOGS);
        }

        public void Builder(string msg)
        {
            string path = MS.LOGS;
            if (path != null)
            {
                try
                {
                    Console.WriteLine(" [{0:G}] {1}.", System.DateTime.Now, msg);
                    string logLine = System.String.Format(" [{0:G}] {1}.", System.DateTime.Now, msg);
                    sb01.AppendLine(logLine);
                }
                finally
                {
                    //sw.Close();
                }
            }
            else
            {

            }
        }
        
        public void RegistraLog(string filename)
        {
            string path = MS.LOGS;

            if (path != null)
            {
                if (GetExclusiveFileLock(path + "ADE.Extras.MailNotifier.log") && sb01.Length > 0)
                {
                    using (StreamWriter sw01 = File.AppendText(path + "ADE.Extras.MailNotifier.log"))
                    {
                        sw01.WriteLine(sb01.ToString());
                    }
                }
            }
        }

        private static bool GetExclusiveFileLock(string path)
        {
            var fileReady = false;
            const int MaximumAttemptsAllowed = 50;
            var attemptsMade = 0;
            if (!File.Exists(path)) return true;
            while (!fileReady && attemptsMade <= MaximumAttemptsAllowed)
            {
                try
                {
                    using (File.Open(path, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                    {
                        fileReady = true;
                    }
                }
                catch (IOException)
                {
                    attemptsMade++;
                    Thread.Sleep(100);
                }
            }
            return fileReady;
        }
    }
}
