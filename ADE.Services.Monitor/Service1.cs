using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;

namespace ADE.Services.Monitor
{
    public partial class InterfaceFileWatcher : ServiceBase
    {
        string PathToWatch = ConfigurationManager.AppSettings["PathToWatch"];
        string FileExtension = ConfigurationManager.AppSettings["FileExtension"];
        string ProcessToExecute = ConfigurationManager.AppSettings["ProcessToExecute"];
        int MaxNumberOfThreads;
        string ProcessToDatabaseRoute = "";
        string ProcessToSunatRoute = "";
        string ProcessWorkingDirectory = "";
        string LogPath = ConfigurationManager.AppSettings["LogPath"];
        bool isActive = false;
        public System.Timers.Timer GlobalTimer { get; set; }
        protected FileSystemWatcher Watcher;

        public InterfaceFileWatcher()
        {
            string name = DateTime.Now.Year + "-" + DateTime.Now.Month.ToString().PadLeft(2, '0');
            Log.Instance.LogPath = $@"{LogPath}\{name}";
            Log.Instance.LogFileName = "ADE.Services.Monitor";
            InitializeComponent();
            //CheckExistingFiles();
            //Watcher = new MyFileSystemWatcher();

            // NUEVO MÉTODO
            // 2017-07-21
            try {
                MaxNumberOfThreads = int.Parse(ConfigurationManager.AppSettings["MaxNumberOfThreads"]);
                Log.WriteLine($"Se ha configurado un máximo de {MaxNumberOfThreads} threads para el servicio");

                int w;
                int c;
                ThreadPool.GetMinThreads(out w, out c);
                ThreadPool.SetMinThreads(MaxNumberOfThreads, c);

                // Write the numbers of minimum threads
                //Log.WriteLine($"Minimun=> {w}, {c}");
            }
            catch (Exception ConvException)
            {
                MaxNumberOfThreads = 40;
                Log.WriteLine($"no se ha encontrado un valor de threads");
                Log.WriteLine($"Se ha configurado un máximo de {MaxNumberOfThreads} threads para el servicio, valor por defecto");
                int w;
                int c;
                ThreadPool.GetMinThreads(out w, out c);
                ThreadPool.SetMinThreads(MaxNumberOfThreads, c);

                //Log.WriteLine("Error al obtener el Numero Maximo de Threads: " + ConvException.Message);
            }
        }
        protected override void OnStart(string[] args)
        {
            Log.WriteLine("Iniciando Servicio (Timer)");
            GlobalTimer = new System.Timers.Timer();

            GlobalTimer.Enabled = true;
            GlobalTimer.Interval = 1000;//1 SEGUNDO
            GlobalTimer.Elapsed += new System.Timers.ElapsedEventHandler(CheckExistingFiles);
            Log.WriteLine("Iniciado correctamente");
        }

        protected override void OnStop()
        {
            Log.WriteLine("Se ha detenido el Servicio (Timer)");
        }

        public void InsertAndSendDocument(string InsertName)
        {
            ProcDatabaseIncome(InsertName);
            ProcSunatDelivery(InsertName.Replace(FileExtension, ""));
        }

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
            //Thread.Sleep(100);

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
            //startInfo.Verb = "runas";

            Process proc = new Process();
            proc.StartInfo = startInfo;
            proc.Start();
            //proc.WaitForExit();
            //proc.Close();
        }

        /// <summary>
        /// Este evento verificará si es que existen archivos
        /// con la extension configurada para que sean 
        /// procesados.
        /// </summary>
        public void CheckExistingFiles(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (!isActive)
            {
                isActive = true;

                DirectoryInfo di = new DirectoryInfo(PathToWatch);
                FileSystemInfo[] files = di.GetFileSystemInfos();
                var orderedFiles = files.OrderBy(f => f.CreationTime);

                if(orderedFiles.Count() > 0)
                {
                    ParallelOptions options = new ParallelOptions();
                    options.MaxDegreeOfParallelism = MaxNumberOfThreads * 2;

                    Parallel.ForEach(orderedFiles, options, file =>
                    {
                        if (file.Name.EndsWith(FileExtension))
                        {
                            Log.WriteLine($"Archivo sin procesar encontrado: {file.FullName}");
                            if (GetExclusiveFileLock(file.FullName))
                            {
                                string Nombre = file.Name;

                                if (VerifyRUC(Nombre))
                                {
                                    string RUC = Nombre.Substring(0, 11);
                                    string DFile = ConfigurationManager.AppSettings[RUC] + @"\EntradaCE\InterfTXT\" + Nombre;

                                    if (File.Exists(DFile))
                                        File.Delete(DFile);

                                    File.Move(file.FullName, DFile);

                                    ProcDatabaseIncome(Nombre);

                                    //envio sunat
                                    if (!file.FullName.Contains("RA") && !file.FullName.Contains("RR") && !file.FullName.Contains("RC"))
                                    {
                                        try
                                        {
                                            var cond = ConfigurationManager.AppSettings["ENV"].ToString();
                                            if (cond.ToLower() == "si")
                                            { ProcSunatDelivery(Nombre.Replace(FileExtension, "")); }
                                        }
                                        catch (Exception ex)
                                        { ProcSunatDelivery(Nombre.Replace(FileExtension, "")); }
                                    }
                                }
                            }
                        }
                    });
                }

                //foreach(FileSystemInfo fs in orderedFiles)
                //{
                //    isActive = true;
                //    //fs.
                //    if(NumThreads < MaxThreads)
                //    {
                //        NumThreads++;

                //        NumThreads--;
                //    }


                //}

                isActive = false;


                //if (ExistingFiles.Length > 0)
                //{
                //    foreach (string SFile in ExistingFiles)
                //    {
                //        if (SFile.EndsWith(FileExtension))
                //        {
                //            Log.WriteLine($"Archivo sin procesar encontrado: {SFile}");
                //            if (GetExclusiveFileLock(SFile))
                //            {
                //                string[] Nombres = SFile.Split(Path.DirectorySeparatorChar);
                //                string Nombre = Nombres[Nombres.Length - 1];

                //                if (VerifyRUC(Nombre))
                //                {
                //                    string RUC = Nombre.Substring(0, 11);
                //                    string DFile = ConfigurationManager.AppSettings[RUC] + @"\EntradaCE\InterfTXT\" + Nombre;

                //                    if (File.Exists(DFile))
                //                        File.Delete(DFile);

                //                    File.Move(SFile, DFile);

                //                    ProcDatabaseIncome(Nombre);
                //                    ProcSunatDelivery(Nombre.Replace(FileExtension, ""));
                //                }
                //            }
                //        }
                //    }
                //}
            }
            
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
            const int MaximumAttemptsAllowed = 15;
            var attemptsMade = 0;

            while (!fileReady && attemptsMade <= MaximumAttemptsAllowed)
            {
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
                    Thread.Sleep(1000);
                    Log.WriteLine($"Archivo {path} aún no termina de ser copiado.");
                }
            }
            return fileReady;
        }
    }
}