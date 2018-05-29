using ADE.Entities.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Net;
using System.IO;
using ADE.Configurations.Entities.Summaries;
using ADE.Configurations.Objects;
using System.Xml;

namespace ADE.Extras.Common.Method
{
    public sealed class Methods
    {
        private static readonly Methods instance = new Methods();
        static Methods() { }
        private Methods() { }
        public static Methods Instance { get { return instance; } }

        #region send

        public void WriteLog(string pathlog, string msg)
        {
            using (StreamWriter sw = new StreamWriter(pathlog, true, Encoding.GetEncoding("iso-8859-1")))
            {
                sw.WriteLine("[" + DateTime.Now + "] " + msg);
            }
        }

        public void ConsoleLog(string msg)
        {
            Console.WriteLine(" [" + DateTime.Now + "] " + msg);
        }

        public string SendNotify(string path, Document obj)
        {
            //bool result = false;
            string result = string.Empty;

            using (System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage())
            {
                string[] AmailTO = obj.TO.Split(';');
                foreach (string em in AmailTO)
                { if (em.Trim().Contains("@")) { mail.To.Add(new MailAddress(em)); } }

                if(obj.CC.Length > 0)
                {
                    string[] AmailCC = obj.CC.Split(';');
                    foreach (string cc in AmailCC)
                    { if (cc.Trim().Contains("@")) { mail.CC.Add(new MailAddress(cc)); } }
                }

                if(obj.CCO.Length > 0)
                {
                    string[] AmailCCO = obj.CCO.Split(';');
                    foreach (string cco in AmailCCO)
                    { if (cco.Trim().Contains("@")) { mail.Bcc.Add(new MailAddress(cco)); } }
                }

                if(obj.Notify.Length > 0)
                {
                    string[] AmailCCO_Notify = obj.Notify.Split(';');
                    foreach (string cco_notify in AmailCCO_Notify)
                    { if (cco_notify.Trim().Contains("@")) { mail.Bcc.Add(new MailAddress(cco_notify)); } }
                }

                mail.From = new MailAddress(obj.Email, obj.Email, Encoding.GetEncoding("iso-8859-1"));
                mail.Subject = "Documento de baja: " + Get_Desc(obj.TypeDoc) + " - " + obj.Serie + "-" + obj.Correlativo + " - " + obj.Rzn_Soc_Emi;
                mail.SubjectEncoding = Encoding.GetEncoding("iso-8859-1");

                mail.Body = Get_Body(path, obj);
                mail.Priority = MailPriority.High;
                mail.BodyEncoding = Encoding.GetEncoding("iso-8859-1");
                mail.IsBodyHtml = true;

                SmtpClient smtp = new SmtpClient();

                ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErros)
                { return true; };

                if (obj.Domain != "-" && obj.Domain.Length > 1 && obj.Port > 0)
                {
                    smtp = new SmtpClient(obj.Domain, obj.Port);
                }
                else if (obj.IP != "" && obj.IP.Length > 0 && obj.Port > 0 && obj.Domain.Length < 1)
                {
                    smtp = new SmtpClient(obj.IP, obj.Port);
                }

                smtp.EnableSsl = obj.UseSSL == 1 ? true : false;
                smtp.Credentials = new NetworkCredential(obj.Email, new Util.Encrypt().DecryptKey(obj.Pwd));

                try
                {
                    smtp.Send(mail);
                    return "Notificación enviada correctamente.";
                }
                catch (Exception ex) { result = ex.Message; }
            }
            return result;
        }

        private string Get_Body(string path, Document obj)
        {
            string body = string.Empty;

            if (System.IO.File.Exists(path + "Low.htm"))
            { body = System.IO.File.ReadAllText(path + "Low.htm", Encoding.GetEncoding("iso-8859-1")); }
            else { body = BodyHtm; }
            
            try
            {
                body = body.Replace("{URL_COMPANY_LOGO}", obj.Url_Logo);
                body = body.Replace("{ClienteRazonSocial}", obj.Rzn_Soc_Cli);
                body = body.Replace("{TipoDocumento}", Get_Desc(obj.TypeDoc));
                body = body.Replace("{SerieCorrelativo}", obj.Serie + "-" + obj.Correlativo);
                body = body.Replace("{FechaEmision}", obj.Fec_Emi.ToString("dd/MM/yyyy"));
                body = body.Replace("{FechaAnulado}", obj.Fec_Res.ToString("dd/MM/yyyy"));
                body = body.Replace("{RazonSocial}", obj.Rzn_Soc_Emi);
            }
            catch (Exception ex)
            { return BodyHtm; }
            return body;
        }

        private string Get_Desc(string type)
        {
            string desc = string.Empty;
            switch (type)
            {
                case "01": { desc = "FACTURA ELECTRÓNICA"; break; }
                case "03": { desc = "BOLETA DE VENTA ELECTRÓNICA"; break; }
                case "07": { desc = "NOTA DE CRÉDITO ELECTRÓNICA"; break; }
                case "08": { desc = "NOTA DE DÉBITO ELECTRÓNICA"; break; }
                case "20": { desc = "COMPROBANTE DE RETENCIÓN ELECTRÓNICO"; break; }
                case "40": { desc = "COMPROBANTE DE PERCEPCIÓN ELECTRÓNICO"; break; }
                case "09": { desc = "GUIA DE REMISIÓN ELECTRÓNICA"; break; }
                default: { desc = type; break; }
            }
            return desc;
        }

        public const string BodyHtm = "<html><head><title></title></head><body lang='es-pe'><div class='modal-content' style='border:solid'><div class='modal-header'><div class='col-lg-9' style='text-align:left; padding-left:5px'><img src='{URL_COMPANY_LOGO}' style='font-size:x-large' /></div></div><div class='modal-body'><div class='row' style='padding-left:5px'><div class='col-lg-3'><label style='font-family:Cambria;'>Señor(es),</label></div><div class='col-lg-9'><b>{ClienteRazonSocial}</b></div></div><br /><div class='row' style='padding-left:5px'><div class='col-lg-8'><b style='font-family:Cambria;'>Le Informamos que el siguiente documento electrónico a sido dado de baja:</b></div></div><br /><div class='row' style='padding-left:5px'><div class='col-lg-8'><table border='0'><tr><td><label style='font-family:Cambria;'>Tipo de Documento</label></td><td><b>: {TipoDocumento}</b></td></tr><tr><td><label style='font-family:Cambria;'>Número de Documento</label></td><td><b>: {SerieCorrelativo}</b></td></tr><tr><td><label style='font-family:Cambria;'>Fecha Emisión</label></td><td><b>: {FechaEmision}</b></td></tr><tr><td><label style='font-family:Cambria;'>Fecha Anulado</label></td><td><b>: {FechaAnulado}</b></td></tr></table></div><div class='col-lg-4'><label></label></div></div><br /><b style='padding-left:5px'>Atentamente, {RazonSocial}</b><br><br></div><div class='modal-footer'><div class='row' style='padding-left:5px'><div class='col-lg-6'><label style='font-family:Cambria; font-size:smaller;'>Este es un sistema automático, por favor no responda este mensaje.</label></div></div></div></div></body></html>";

        public int DateCompare(string date1, string date2)
        {
            int val = 0;

            try
            {
                DateTime fecha1; DateTime fecha2;
                if(date1.Length > 0)
                {
                    fecha1 = Convert.ToDateTime(date1);
                    fecha2 = Convert.ToDateTime(date2);

                    int result = DateTime.Compare(fecha1, fecha2);
                    val = result < 0 ? 2 : result;
                }
                else { val = 2; }
            }
            catch (Exception ex)
            { }
            return val;
        }

        public string GetActionCodeResponse_CDR(MainSettings ms)
        {
            string CodeAction = string.Empty;
            int value = 0;
            try
            {
                XmlDocument xml = new XmlDocument();
                xml.Load(ms.XMLRFileLocation);

                if (xml.InnerXml.Length > 300)
                {
                    XmlNodeList nodelist = xml.GetElementsByTagName("cbc:ResponseCode");
                    if (nodelist != null && nodelist.Count > 0)
                        value = int.Parse(nodelist[0].ChildNodes.Item(0).InnerText);
                    else
                        value = -1;
                }
                else
                { value = -1; }
                CodeAction = GetActionType(value);
            }
            catch (Exception ex)
            { CodeAction = ""; }
            return CodeAction;
        }

        public string GetActionType(int value)
        {
            if (value == Constantes.ValorCero)
                return "SOK";
            else if (value > 100 && value < 2000)
                return "EXC";
            else if (value > 1999 && value < 4000)
                return "RCHZ";
            else if (value > 3999)
                return "OBS";
            else return "";
        }




        #endregion
    }


    #region class public

    public class UtilClass
    {
        public string NUM_CE { get; set; }
        public int STATUS_RC_DOC { get; set; }    
        
        public int CodeResponse { get; set; }
        public string MessageResponse { get; set; }
    }

    public class ListUtilClass : List<UtilClass>
    {
        public bool Generate { get; set; }
        public List<RBoletasDetalle> ListBoletasLow { get; set; }
        public int CantDocs { get; set; }
        public List<RBoletasDetalle> ListNotesDoc { get; set; }



        public ListUtilClass()
        {
            Generate = false;
            ListBoletasLow = new List<RBoletasDetalle>();
            CantDocs = 0;
            ListNotesDoc = new List<RBoletasDetalle>();
        }
    }


    #endregion




}
