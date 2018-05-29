using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace ADE.Extras.MailNotifier
{
    class Program
    {
        static string LogPath = ConfigurationManager.AppSettings["LogPath"];
        static MainSettings MS = new MainSettings();
        static string ServiceName = "";
        static void Main(string[] args)
        {
            string html = "";
            MS.validateConnectionString();
            if (args.Length == 0)
                ServiceName = "";
            else
                ServiceName = args[0];

            Log LOG = new Log(MS);

            LOG.Builder("===============================================");
            LOG.Builder("PROCESO DE NOTIFICACIÓN DE ERROR EN SERVICIOS");
            LOG.Builder("Obteniendo correos de destino de Base de Datos");

            Encrypter EC = new Encrypter();
            try
            {
                MailObject MO = new MailObject();
                MO = MS.SP_ObtieneMailConfig();

                if (MO.EMAIL == "" || MO.PASSWORD == "" || MO.DOMAIN == "" || MO.PORT == "") { LOG.Builder("No se ha configurado correctamente el correo de envío."); goto Final; }
                if (MO.MAILNOTIF == "") { LOG.Builder("No se han configurado los correos de destino para la notificación."); goto Final; }

                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient(MO.DOMAIN);

                mail.From = new MailAddress(MO.EMAIL);
                //mail.To.Add(MO.MAILNOTIF);
                LOG.Builder("Se enviará a los siguientes correos: " + MO.MAILNOTIF);

                foreach (var address in MO.MAILNOTIF.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
                {
                    mail.To.Add(address);
                }

                mail.Subject = "[SLIN-ADE] Servicio Detenido el " + DateTime.Now.ToString("dd-MM-yyyy");
                
                html = "Se ha detenido el Servicio " + ServiceName + " luego de 2 intentos, en caso siga teniendo problemas por favor comunicar a dezapata@slin.com.pe";
                
                mail.Body = html;
                mail.IsBodyHtml = false;
                SmtpServer.Port = int.Parse(MO.PORT);
                
                SmtpServer.Credentials = new System.Net.NetworkCredential(MO.EMAIL, EC.DecryptKey(MO.PASSWORD));
                
                if (MO.USESSL == "1")
                {
                    SmtpServer.EnableSsl = true;
                }

                SmtpServer.Send(mail);
            }
            catch (SmtpException e)
            {
                LOG.Builder(e.Message + " - " + e.InnerException.Message);
            }
            catch (Exception e)
            {
                LOG.Builder("Error al enviar Correo: " + e.Message);
            }

            Final:

            LOG.Builder("===============================================");
            LOG.RegistraLog("");
        }
    }
}
