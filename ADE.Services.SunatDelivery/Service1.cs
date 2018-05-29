using System;
using System.Collections.Generic;
using System.Configuration;
using System.ServiceProcess;
using System.Threading.Tasks;

namespace ADE.Services.SunatDelivery
{
    public partial class Service1 : ServiceBase
    {
        private System.Timers.Timer timer;
        string LogPath = ConfigurationManager.AppSettings["LogPath"];
        int TimeToCheck = 0;
        string ProcessWorkingDirectory = "";

        MainSettings MS = new MainSettings();
        public Service1()
        {
            MS.validateConnectionString();
            string name = DateTime.Now.Year + "-" + DateTime.Now.Month.ToString().PadLeft(2, '0');
            Log.Instance.LogPath = $@"{LogPath}\{name}";
            Log.Instance.LogFileName = "ADE.Services.SunatDelivery";
            Log.WriteLine("Se inicia el Monitor de Documento a enviar a Sunat");

            TimeToCheck = MS.getInterval();

            if (TimeToCheck == -2)
            {
                Log.WriteLine("Error al conectarse a Base de Datos.");
                OnStop();
            }
            if (TimeToCheck == -1)
            {
                Log.WriteLine("No se ha configurado el intervalo de envío. Verifique por favor.");
                OnStop();
            }
            
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            Log.WriteLine("Iniciando Servicio...");
            try
            {
                Log.WriteLine($"Se monitorea cada {TimeToCheck} minutos");
                GetFirstDocuments();
                TimeToCheck = TimeToCheck * 1000 * 60;
                timer = new System.Timers.Timer(TimeToCheck);  // 30000 milliseconds = 30 seconds
                timer.AutoReset = true;
                timer.Elapsed += new System.Timers.ElapsedEventHandler(this.GetDocuments);
                timer.Start();
            }
            catch(Exception x)
            {
                Log.WriteLine($"Error al inicial el servicio:  {x.Message}");
            }
        }

        protected override void OnStop()
        {
            Log.WriteLine("Se detuvo el Servicio");
        }

        public void GetDocuments(object sender, System.Timers.ElapsedEventArgs e)
        {
            List<string> DBDocuments = MS.getDocs();
            if(DBDocuments.Count > 0)
            {
                Log.WriteLine($"Se encontraron {DBDocuments.Count} documentos para envío a Sunat.");
                foreach (string Document in DBDocuments)
                {
                    string Path = ConfigurationManager.AppSettings[Document.Substring(0, 11)];
                    if (Path != null)
                    {
                        Log.WriteLine($"Enviando Documento: {Document}");
                        ProcessWorkingDirectory = $@"{Path}\Procesos\env\";
                        Path += @"\Procesos\env\ADE.Processes.SunatDelivery.exe";
                        
                        EnvioSunat(Document, Path);
                    }
                }
            }
            else
            {
                //Log.WriteLine("No se han encontrado Documentos para envío.");
            }
        }

        public void GetFirstDocuments()
        {
            List<string> DBDocuments = MS.getDocs();
            if (DBDocuments.Count > 0)
            {
                Log.WriteLine($"Se encontraron {DBDocuments.Count} documentos para envío a Sunat.");
                foreach (string Document in DBDocuments)
                {
                    string Path = ConfigurationManager.AppSettings[Document.Substring(0, 11)];
                    if (Path != null)
                    {
                        Log.WriteLine($"Enviado Documento: {Document}");
                        ProcessWorkingDirectory = $@"{Path}\Procesos\env\";
                        Path += @"\Procesos\env\ADE.Processes.SunatDelivery.exe";
                        EnvioSunat(Document, Path);
                    }
                }
            }
            else
            {
                //Log.WriteLine("No se han encontrado Documentos para envío.");
            }
        }

        public void EnvioSunat(string Document, string Proc)
        {
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.CreateNoWindow = false;
            startInfo.FileName = Proc;
            startInfo.Arguments = Document;
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = false;
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.WorkingDirectory = ProcessWorkingDirectory;
            startInfo.Verb = "runas";
            //System.Threading.Thread.Sleep(100);

            System.Diagnostics.Process proc = new System.Diagnostics.Process();
            proc.StartInfo = startInfo;
            proc.Start();
            //proc.WaitForExit();
            //proc.Close();
        }
    }
}
