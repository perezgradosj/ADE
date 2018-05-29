using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace ADE.Services.Monitor
{
    public class MyFileSystemWatcher : FileSystemWatcher
    {
        string PathToWatch = ConfigurationManager.AppSettings["PathToWatch"];
        string FileExtension = ConfigurationManager.AppSettings["FileExtension"];
        string ProcessToExecute = ConfigurationManager.AppSettings["ProcessToExecute"];
        string ProcessToDatabaseRoute = "";
        string ProcessToSunatRoute = "";
        string ProcessWorkingDirectory = "";

        public MyFileSystemWatcher()
        {
            Init();
        }
        
        private void Init()
        {
            Log.WriteLine("Inicializando el Watcher...");
            IncludeSubdirectories = false;
            Path = PathToWatch;
            Filter = $"*.{FileExtension.Replace(".", "")}";
            NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            Created += Watcher_Created;
            EnableRaisingEvents = true;
            Log.WriteLine($"Empieza a monitorear la ruta {PathToWatch} buscando archivos con el filtro {Filter}");

            ThreadPool.SetMaxThreads(5, 5);
        }

        public void Watcher_Created(object source, FileSystemEventArgs e)
        {
            try
            {
                if (GetExclusiveFileLock(e.FullPath))
                {
                    Log.WriteLine($"Archivo detectado {e.FullPath}");
                    if (VerifyRUC(e.Name))
                    {
                        string RUC = e.Name.Substring(0, 11);
                        string DFile = ConfigurationManager.AppSettings[RUC] + @"\EntradaCE\InterfTXT\" + e.Name;

                        if (File.Exists(DFile))
                            File.Delete(DFile);

                        File.Move(e.FullPath, DFile);

                        ProcDatabaseIncome(e.Name);
                        ProcSunatDelivery(e.Name.Replace(FileExtension, ""));

                        //Thread T = new Thread(() => InsertAndSendDocument(e.Name, e.Name.Replace(FileExtension, "")));

                        //T.Start();
                        //Log.WriteLine($"Thread iniciado..");

                        //ThreadPool.QueueUserWorkItem(new WaitCallback(InsertAndSendDocument), e.Name, e.Name.Replace(FileExtension, "")));
                        //ThreadPool.QueueUserWorkItem(new WaitCallback(InsertAndSendDocument), e.Name);
                    }
                }
            }
            catch(Exception ex)
            {
                Log.WriteLine($"Error: {ex.Message}");
                //throw ex;
            }
        }

        public void InsertAndSendDocument(string InsertName)
        {
            ProcDatabaseIncome(InsertName);
            ProcSunatDelivery(InsertName.Replace(FileExtension, ""));
        }


        //public void InsertAndSendDocument(string InsertName, string SendName)
        //{
        //    ProcDatabaseIncome(InsertName);
        //    ProcSunatDelivery(SendName);
        //}

        /// <summary>
        /// Este evento llamará al proceso enviandole la ruta 
        /// completa del archivo como parámetro
        /// </summary>
        /// <param name="FP"></param>
        public void ProcDatabaseIncome(string FP)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = false;
            startInfo.FileName = ProcessToDatabaseRoute;
            startInfo.Arguments = FP;
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = false;
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.WorkingDirectory = $@"{ProcessWorkingDirectory}\bin\";

            Thread.Sleep(100);

            Process proc = new Process();
            proc.StartInfo = startInfo;
            proc.Start();
            proc.WaitForExit();
            proc.Close();
        }

        /// <summary>
        /// Este evento llamará al proceso enviandole el mombre 
        /// completo del comprobante como parámetro
        /// </summary>
        /// <param name="FP"></param>
        public void ProcSunatDelivery(string FP)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = false;
            startInfo.FileName = ProcessToSunatRoute;
            startInfo.Arguments = FP;
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = false;
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.WorkingDirectory = $@"{ProcessWorkingDirectory}\env\";

            Process proc = new Process();
            proc.StartInfo = startInfo;
            proc.Start();
            //proc.WaitForExit();
            //proc.Close();
        }

        public bool VerifyRUC(string FileName)
        {
            bool isValid = false;
            string RUC = FileName.Substring(0, 11);

            try
            {
                string PathToCopy = ConfigurationManager.AppSettings[RUC];
                if (Directory.Exists(PathToCopy))
                {
                    isValid = true;
                    ProcessToDatabaseRoute = $@"{PathToCopy}\Procesos\bin\ADE.Processes.DatabaseIncome.exe";
                    ProcessToSunatRoute = $@"{PathToCopy}\Procesos\env\ADE.Processes.SunatDelivery.exe";
                    ProcessWorkingDirectory = $@"{PathToCopy}\Procesos";
                    Log.WriteLine($"RUC({RUC}) del Archivo Verificado, se copiará el archivo a {PathToCopy}");
                }
                else
                {
                    Log.WriteLine($"RUC({RUC}) del Archivo es Valido pero el directorio es inaccesible");
                    Log.WriteLine($"No se procesará el archivo {FileName}");
                }
            }
            catch (Exception e)
            {
                Log.WriteLine($"RUC({RUC}) del Archivo no Valido");
                Log.WriteLine($"No se procesará el archivo {FileName}");
            }

            return isValid;
        }

        /// <summary>
        /// Este evento verifica que el archivo no esté 
        /// en proceso de copia o siendo usado por otro proceso
        /// </summary>
        /// <param name="path"></param>
        private static bool GetExclusiveFileLock(string path)
        {
            var fileReady = false;
            const int MaximumAttemptsAllowed = 150;
            var attemptsMade = 0;

            while (!fileReady && attemptsMade <= MaximumAttemptsAllowed)
            {
                Thread.Sleep(100);
                try
                {
                    using (File.Open(path, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                    {
                        Log.WriteLine($"Archivo {path} está listo para ser procesado.");
                        return true;
                    }
                }
                catch (IOException)
                {
                    attemptsMade++;
                    Log.WriteLine($"Archivo {path} aún no termina de ser copiado.");
                }
            }

            return fileReady;
        }
    }
}
