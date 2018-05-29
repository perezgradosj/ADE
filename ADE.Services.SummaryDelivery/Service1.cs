using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.ServiceProcess;
//using System.Threading;
using System.Timers;

namespace ADE.Services.SummaryDelivery
{
    public partial class Service1 : ServiceBase
    {
        string LogPath = ConfigurationManager.AppSettings["LogPath"];
        List<ServicioResumen> LSR = new List<ServicioResumen>();
        List<SummaryTask> TL = new List<SummaryTask>();
        MainSettings MS = new MainSettings();
        public Service1()
        {
            //System.Threading.Thread.Sleep(30000);
            MS.validateConnectionString();
            string name = DateTime.Now.Year + "-" + DateTime.Now.Month.ToString().PadLeft(2, '0');
            Log.Instance.LogPath = $@"{LogPath}\{name}";
            Log.Instance.LogFileName = "ADE.Services.SummaryDelivery";
            Log.WriteLine("==============================================================");
            Log.WriteLine("Se inicia el Servicio de Envio de Resumnes a Sunat");

            Log.WriteLine("Obteniendo Configuración de Servicios en Base de Datos");
            LSR = MS.getIntervals();
            Log.WriteLine($"Configuraciones encontradas: {LSR.Count}");
            foreach (ServicioResumen SR in LSR)
            {
                if (SR.IdEstado == "1")
                {
                    SummaryTask T = new SummaryTask();
                    T.MS = MS;
                    T.SR = SR;
                    TL.Add(T);
                }
                else
                {
                    Log.WriteLine($"El Servicio de {SR.NameService} está deshabilitado para la empresa {SR.RucEntity}");
               }
            }
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            Log.WriteLine("Iniciando Timer para cada Servicio Activo");
            
            for (int i = 0; i < TL.Count; i++)
            {
                TL[i].Start();
            }
        }

        protected override void OnStop()
        {
            for (int i = 0; i < TL.Count; i++)
            {
                TL[i].Timer.Enabled = false;
            }
            Log.WriteLine("**");
            Log.WriteLine("Se detuvó el Servicio de Resumenes");
            Log.WriteLine("");
        }
    }

    public class SummaryTask{
        public Timer Timer { get; set; }
        public ServicioResumen SR { get; set; }
        public MainSettings MS { get; set; }

        public SummaryTask()
        {
            Timer = new Timer();
            SR = new ServicioResumen();
        }

        public void Start()
        {
            Log.WriteLine($"***********************************************");
            Log.WriteLine($"Empresa {SR.RucEntity}");
            Log.WriteLine($"Servicio de {SR.NameService} ha sido activado");
            
            Timer.Enabled = true;

            Timer.Interval = getDay(SR.ValueTime);
            Timer.Elapsed += new System.Timers.ElapsedEventHandler(GetDocuments);
        }

        public double getDay(string userInput)
        {
            Log.WriteLine($"Se buscará documentos todos los días, a las {userInput}");
            int day = 0;
            double res = 0;
            var time = TimeSpan.Parse(userInput);
            var dateTime = DateTime.Today.Add(time);
            DateTime schedule;
            var det = userInput.Split(':');
            int hour, minute;

            hour = int.Parse(det[0]);
            minute = int.Parse(det[1]);

            if (DateTime.Now >= dateTime)
                day = 1;
            else
                day = 0;

            schedule = DateTime.Today.AddDays(day).AddHours(hour).AddMinutes(minute);
            res = schedule.Subtract(DateTime.Now).TotalSeconds * 1000;
            Log.WriteLine($"Proxima busqueda de Resumen: {schedule}");
            return res;
        }

        public void GetDocuments(object sender, System.Timers.ElapsedEventArgs e)
        {
            Log.WriteLine("-----------------------------------------------");

            if (((Timer)sender).Interval != 24 * 60 * 60 * 1000)
            {
                ((Timer)sender).Interval = 24 * 60 * 60 * 1000;
            }

            #region anulados
            if (SR.SubType == "RA" || SR.SubType == "RR")
            {
                Log.WriteLine($"Buscando documentos anulados con 7 días de antiguedad.");
                for (int i = 7; i > -1; i--)
                {
                    int days = 0;

                    if (i == 0)
                        days = 0;
                    else
                        days = i * -1;

                    DateTime ResDate = e.SignalTime.AddDays(days);
                    string SummaryDate = $"{ResDate.Year.ToString()}-{ResDate.Month.ToString().PadLeft(2, '0')}-{ResDate.Day.ToString().PadLeft(2, '0')}";
                    bool DocumentReady = false;

                    DocumentReady = MS.SP_ObtieneResumenAnulados(SummaryDate, SR.SubType, SR.RucEntity);

                    if (DocumentReady)
                    {
                        Log.WriteLine($"Se encontraron documentos para {SR.NameService} el día {SummaryDate}");
                        string TRC = $"CABECERA-PRINCIPAL|{SR.SubType}|{SummaryDate}|{SR.RucEntity}";
                        string MPath = ConfigurationManager.AppSettings[SR.RucEntity];
                        var LevelsUp = @"..\";
                        string inPath = Path.GetFullPath(Path.Combine(MPath, LevelsUp)) + @"_ADE\in\";
                        string FileName = $"{SR.RucEntity}-{SR.SubType}-{SummaryDate.Replace("-", "")}-{e.SignalTime.TimeOfDay}".Replace(":", "") + ".txt";

                        Log.WriteLine($"Escribiendo archivo para generar Resumen:  {FileName}");
                        using (StreamWriter sw = new StreamWriter($@"{inPath + FileName}"))
                        {
                            sw.Write(TRC);
                        }
                    }
                    else
                    {
                        Log.WriteLine($"No se han encontrado Documentos para {SR.NameService} el día {SummaryDate}");
                    }
                }
            }
            #endregion

            #region boletas
            if (SR.SubType == "RC")
            {
                Log.WriteLine($"Buscando boletas sin enviar con 7 días de antiguedad.");
                for (int i = 7; i > -1; i--)
                {
                    int days = 0;

                    if (i == 0)
                        days = 0;
                    else
                        days = i * -1;

                    DateTime ResDate = e.SignalTime.AddDays(days);
                    string SummaryDate = $"{ResDate.Year.ToString()}-{ResDate.Month.ToString().PadLeft(2, '0')}-{ResDate.Day.ToString().PadLeft(2, '0')}";
                    bool DocumentReady = false;

                    if (days != 0)
                        DocumentReady = MS.SP_ObtieneResumenBoletas_Ant(SummaryDate, SR.RucEntity, string.Empty);
                    else
                        DocumentReady = MS.SP_ObtieneResumenBoletas(SummaryDate, SR.RucEntity, string.Empty);

                    if (DocumentReady)
                    {
                        Log.WriteLine($"Se encontraron documentos para {SR.NameService} el día {SummaryDate}");
                        string TRC = $"CABECERA-PRINCIPAL|{SR.SubType}|{SummaryDate}|{SR.RucEntity}";
                        string MPath = ConfigurationManager.AppSettings[SR.RucEntity];
                        var LevelsUp = @"..\";
                        string inPath = Path.GetFullPath(Path.Combine(MPath, LevelsUp)) + @"_ADE\in\";
                        string FileName = $"{SR.RucEntity}-{SR.SubType}-{SummaryDate.Replace("-", "")}-{e.SignalTime.TimeOfDay}".Replace(":", "") + ".txt";

                        Log.WriteLine($"Escribiendo archivo para generar Resumen:  {inPath + FileName}");
                        using (StreamWriter sw = new StreamWriter($@"{inPath + FileName}"))
                        {
                            sw.Write(TRC);
                        }
                    }
                    else
                    {
                        Log.WriteLine($"No se han encontrado Documentos para {SR.NameService} el día {SummaryDate}");
                    }

                    if (DocumentReady)
                    {
                        System.Threading.Thread.Sleep(120000);//
                    }
                }
            }
            #endregion

            if (SR.SubType == "SA")
            {
                try
                {
                    List<DocumentsPending> PendingDocuments = MS.SP_ObtieneDocumentosParaCorreo(SR.RucEntity);
                    if (PendingDocuments.Count > 0)
                    {
                        Log.WriteLine("Se encontraron Documentos pendientes de envío, se enviará correo.");
                        if (SendMail(PendingDocuments))
                        {
                            Log.WriteLine("El correo ha sido enviado correctamente.");
                        }
                        else
                        {
                            Log.WriteLine("No se ha enviado el correo.");
                        }
                    }
                    else
                    {
                        Log.WriteLine("No se han encontrado Documentos pendientes de envío.");
                    }
                }
                catch (Exception exe)
                {
                    Log.WriteLine(">> " + exe.Message);
                }
            }

            var time = TimeSpan.FromMilliseconds(((Timer)sender).Interval);
            Log.WriteLine($"La siguiente búsqueda será el {e.SignalTime.Add(time)}");
        }
        
        public void Procesa(string FP, string ProcessToExecuteRoute)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = false;
            startInfo.FileName = ProcessToExecuteRoute;
            startInfo.Arguments = FP;
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = false;
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;

            System.Threading.Thread.Sleep(100);

            Process proc = new Process();
            proc.StartInfo = startInfo;
            proc.Start();
            proc.WaitForExit();
            proc.Close();
        }

        public bool SendMail(List<DocumentsPending> PendingDocuments)
        {
            bool env = false;
            string html = "";
            Encrypter EC = new Encrypter();
            try
            {
                MailObject MO = new MailObject();
                MO = MS.SP_ObtieneMailConfig(SR.RucEntity);
                 
                if(MO.EMAIL == "" || MO.PASSWORD == "" || MO.DOMAIN == "" || MO.PORT == "") { Log.WriteLine("No se ha configurado correctamente el correo de envío."); return false; }
                if(MO.MAILNOTIF == "") { Log.WriteLine("No se han configurado los correos de destino para la notificación."); return false; }

                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient(MO.DOMAIN);

                mail.From = new MailAddress(MO.EMAIL);
                //mail.To.Add(MO.MAILNOTIF);
                Log.WriteLine("Se enviará a los siguientes correos: " + MO.MAILNOTIF);

                foreach (var address in MO.MAILNOTIF.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
                {
                    mail.To.Add(address);
                }
                
                mail.Subject = "[SLIN-ADE] Documentos Electrónicos pendientes de envío a Sunat al " + DateTime.Now.ToString("dd-MM-yyyy");
                
                html = "<html>Estimado cliente: <h4>" + PendingDocuments[0].RZN + "</h4><br>A continuación se muestra un listado de documentos electrónicos sobre los cuales se debe tomar una acción, ya que no tienen un estado final y se encuentran aún en proceso.<body lang=\"es-pe\"><br><table border=\"1\" cellpadding=\"0\" cellspacing=\"0\" border=\"1\">" +
                                //"<tbody>" +
                                "<tr>" +
                                    "<th width =\"50\" align=\"center\" bgcolor=\"263B8E\">" +
                                        "<font size =\"3\" color=\"ffffff\"><b> Nro.</b></font>" +
                                    "</th>" +
                                    "<th width =\"80\" align=\"center\" bgcolor=\"263B8E\">" +
                                        "<font size =\"3\" color=\"ffffff\"><b> Fecha</b></font>" +
                                    "</th>" +
                                    "<th width =\"220\" align=\"center\" bgcolor=\"263B8E\">" +
                                        "<font size =\"3\" color=\"ffffff\"><b> Documento Electrónico</b></font>" +
                                    "</th>" +
                                    //"<th width =\"80\" align=\"center\" bgcolor=\"263B8E\">" +
                                    //    "<font size =\"3\" color=\"ffffff\"><b> Monto</b></font>" +
                                    //"</th>" +
                                    "<th width =\"190\" align=\"center\" bgcolor=\"263B8E\">" +
                                        "<font size =\"3\" color=\"ffffff\"><b> Estado</b></font>" +
                                    "</th>" +
                                "</tr>";
                //"</tbody>" +
                //"</table>" +
                //"<table border=\"1\">" +
                //"<tbody>";
                int index = 1;
                foreach(DocumentsPending DP in PendingDocuments)
                {
                    //html += "<tr>" +
                    html += "<tr>" +
                                " <td width =\"50\" align=\"center\" bgcolor=\"\">" +
                                    $"<font size =\"2\" color=\"\"><b>{index}</b></font>" +
                                "</td>" +
                                " <td width =\"80\" align=\"center\" bgcolor=\"\">" +
                                    $"<font size =\"2\" color=\"\"><b>{DP.FEC.ToString("dd-MM-yyyy")}</b></font>" +
                                "</td>" +
                                "<td width =\"220\" align=\"center\" bgcolor=\"\">" +
                                    $"<font size =\"2\" color=\"\"><b>{DP.CPE}</b></font>" +
                                "</td>" +
                                //"<td width =\"80\" align=\"right\" bgcolor=\"\">" +
                                //    $"<font size =\"2\" color=\"\"><b>{DP.TOT}</b></font>" +
                                //"</td>" +
                                "<td width =\"190\" align=\"center\" bgcolor=\"\">" +
                                    $"<font size =\"2\" color=\"\"><b>{DP.EST}</b></font>" +
                                "</td>" +
                            "</tr>";
                    index++;
                }

                //html += "</tbody>" +
                html += "</table></body><br>SLIN-ADE<br><hr>Nota: Tener en consideración la fecha de procesamiento de máximo (07) días, considerados desde el día siguiente de la fecha de emisión que figure en el documento electrónico. De tener algún inconveniente puede comunicarse con el área respectiva.</hr></html>";


                mail.Body = html;
                mail.IsBodyHtml = true;
                SmtpServer.Port = int.Parse(MO.PORT);
                //if(SR.RucEntity == "20101071562" || SR.RucEntity == "20106896276")
                //{
                //    SmtpServer.Credentials = CredentialCache.DefaultNetworkCredentials;
                //}
                //else
                //{
                    SmtpServer.Credentials = new System.Net.NetworkCredential(MO.EMAIL, EC.DecryptKey(MO.PASSWORD));
                //}
                
                if (MO.USESSL == "1")
                {
                    SmtpServer.EnableSsl = true;
                }

                SmtpServer.Send(mail);
                env = true;
            }
            catch (Exception e) {
                Log.WriteLine("Error al enviar Correo: " + e.Message);
            }
            return env;
        }
    }
}
