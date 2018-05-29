using System;
using System.IO;
using System.Text;
using System.Threading;

namespace ADE.Configurations.Objects
{
    public class Log
    {
        StringBuilder sb01 = new StringBuilder();
        StringBuilder sb02 = new StringBuilder();
        StringBuilder sb03 = new StringBuilder();
        StringBuilder sb04 = new StringBuilder();
        StringBuilder sb05 = new StringBuilder();
        StringBuilder sb06 = new StringBuilder();
        public MainSettings MS = null;
        private string name = "";

        public Log(MainSettings MSX)
        {
            MS = MSX;
            name = DateTime.Now.Year + "-" + DateTime.Now.Month.ToString().PadLeft(2,'0');
            MS.LOGS += name + @"\";
            Directory.CreateDirectory(MS.LOGS);
        }

        public void LecturaArchivo(string msg)
        {
            string path = MS.LOGS;
            if (path != null)
            {
                //System.IO.StreamWriter sw = System.IO.File.AppendText(path + @"1_LecturaArchivo.log");
                try
                {
                    Console.WriteLine(" [{0:G}] {1}.", System.DateTime.Now, msg);
                    string logLine = System.String.Format(" [{0:G}] {1}.", System.DateTime.Now, msg);
                    sb01.AppendLine(logLine);
                    //sw.WriteLine(logLine);
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

        public void Validaciones(string msg)
        {
            string path = MS.LOGS;
            if (path != null)
            {
                //System.IO.StreamWriter sw = System.IO.File.AppendText(path + @"2_Validaciones.log");
                try
                {
                    Console.WriteLine(" [{0:G}] {1}.", System.DateTime.Now, msg);
                    string logLine = System.String.Format(" [{0:G}] {1}.", System.DateTime.Now, msg);
                    sb02.AppendLine(logLine);
                    //sw.WriteLine(logLine);
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

        public void GeneracionXML(string msg)
        {
            string path = MS.LOGS;
            if (path != null)
            {
                //System.IO.StreamWriter sw = System.IO.File.AppendText(path + @"3_GeneracionXML.log");
                try
                {
                    Console.WriteLine(" [{0:G}] {1}.", System.DateTime.Now, msg);
                    string logLine = System.String.Format(" [{0:G}] {1}.", System.DateTime.Now, msg);
                    sb03.AppendLine(logLine);
                    //sw.WriteLine(logLine);
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

        public void IngresoBD01(string msg)
        {
            string path = MS.LOGS;
            if (path != null)
            {
                //System.IO.StreamWriter sw = System.IO.File.AppendText(path + @"4_IngresoBD01.log");
                try
                {
                    Console.WriteLine(" [{0:G}] {1}.", System.DateTime.Now, msg);
                    string logLine = System.String.Format(" [{0:G}] {1}.", System.DateTime.Now, msg);
                    sb04.AppendLine(logLine);
                    //sw.WriteLine(logLine);
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

        public void EnvioSunat01(string msg)
        {
            string path = MS.LOGS;
            if (path != null)
            {
                //System.IO.StreamWriter sw = System.IO.File.AppendText(path + @"6_EnvioSunat01.log");
                try
                {
                    Console.WriteLine(" [{0:G}] {1}.", System.DateTime.Now, msg);
                    string logLine = System.String.Format(" [{0:G}] {1}.", System.DateTime.Now, msg);
                    sb05.AppendLine(logLine);
                    //sw.WriteLine(logLine);
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

        public void IngresoBD02(string msg)
        {
            string path = MS.LOGS;
            if (path != null)
            {
                //System.IO.StreamWriter sw = System.IO.File.AppendText(path + @"7_IngresoBD03.log");
                try
                {
                    Console.WriteLine(" [{0:G}] {1}.", System.DateTime.Now, msg);
                    string logLine = System.String.Format(" [{0:G}] {1}.", System.DateTime.Now, msg);
                    sb06.AppendLine(logLine);
                    //sw.WriteLine(logLine);
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

        public void RegistraError(string filename)
        {
            string path = MS.ITXT;

            if (path != null && GetExclusiveFileLock(path + filename.Replace("txt", "log")))
            {
                using (StreamWriter sw01 = File.AppendText(path + filename.Replace("txt", "log")))
                {
                    sw01.WriteLine(sb01.ToString());
                    sw01.WriteLine(sb02.ToString());
                    sw01.WriteLine(sb03.ToString());
                    sw01.WriteLine(sb04.ToString());
                    sw01.WriteLine(sb05.ToString());
                    sw01.WriteLine(sb06.ToString());
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
                if (GetExclusiveFileLock(path + "1_LecturaArchivo.log") && sb01.Length > 0) { using (StreamWriter sw01 = File.AppendText(path + "1_LecturaArchivo.log")) { sw01.WriteLine(sb01.ToString()); } }
                if (GetExclusiveFileLock(path + "2_Validaciones__.log") && sb02.Length > 0) { using (StreamWriter sw02 = File.AppendText(path + "2_Validaciones__.log")) { sw02.WriteLine(sb02.ToString()); } }
                if (GetExclusiveFileLock(path + "3_GeneracionXML_.log") && sb03.Length > 0) { using (StreamWriter sw03 = File.AppendText(path + "3_GeneracionXML_.log")) { sw03.WriteLine(sb03.ToString()); } }
                if (GetExclusiveFileLock(path + "4_IngresoBD01___.log") && sb04.Length > 0) { using (StreamWriter sw04 = File.AppendText(path + "4_IngresoBD01___.log")) { sw04.WriteLine(sb04.ToString()); } }
                if (GetExclusiveFileLock(path + "5_EnvioDocuSunat.log") && sb05.Length > 0) { using (StreamWriter sw05 = File.AppendText(path + "5_EnvioDocuSunat.log")) { sw05.WriteLine(sb05.ToString()); } }
                if (GetExclusiveFileLock(path + "6_IngresoBD02___.log") && sb06.Length > 0) { using (StreamWriter sw06 = File.AppendText(path + "6_IngresoBD02___.log")) { sw06.WriteLine(sb06.ToString()); } }
            }
            else
            {

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


        public void GetPathLog()
        {

        }
    }
}
