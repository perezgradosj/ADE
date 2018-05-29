using ADE.Configurations.DataAccess;
using ADE.Configurations.Objects;
using ADE.Entities.Database;
using ADE.Extras.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml;
using System.Xml.Serialization;

namespace ADE.Processes.SunatDelivery
{
    public class Receiver
    {
        #region Definicion de variables
        static Log GeneraLog = null;
        static string DocumentName, DocumentType, RucNumber, DocumentSerie, DocumentNumber;
        static byte[] ZIP, CDR, CDRZ;
        static string CDRcod, CDRmsg;
        static List<string> CDRnotes;
        static string[] DocumentSplit;
        static bool isSummary = false;
        static bool isQuery = false;
        static string SummaryType = "";
        #endregion

        static void Main(string[] args)
        {
            // Inicializa Configuración Inicial
            MainSettings MS = new MainSettings();
            if (!MS.ok) return;

            GeneraLog = MS.GeneraLog;
            GeneraLog.EnvioSunat01("= = = = INICIO DE ENVÍO A SUNAT = = = =");

            // Verificamos si es que el nombre del documento está en args[]
            if (args.Length < 1)
            {
                //args = new string[2];
                GeneraLog.EnvioSunat01(": : MODO MANUAL");
                GeneraLog.EnvioSunat01("Ingrese el nombre del documento a procesar: ");
                args = Console.ReadLine().Split(' ');
                GeneraLog.EnvioSunat01("Nombre del documento ingresado manualmente: " + args[0]);
            }

            GeneraLog.EnvioSunat01("Nombre del Documento a envíar a SUNAT: " + args[0]);

            //DocumentName = "20544014189-RR-20171024-1";
            DocumentName = args[0];
            if (args.Length > 1)
            {
                if (args[1].ToUpper() == "Q")
                {
                    isQuery = true;
                }
            }

            if (!Fill()) return;

            MS.DocumentName = DocumentName;
            MS.DocumentType = DocumentType;
            MS.RucNumber = RucNumber;
            MS.getFileNames();

            if (MS.DocumentType.StartsWith("R"))
            {
                isSummary = true;
                if (MS.DocumentType == "RC") SummaryType = "B";
                if (MS.DocumentType == "RA") SummaryType = "A";
                if (MS.DocumentType == "RR") SummaryType = "A";
            }

            string typedoc = string.Empty;
            if (MS.DocumentName.ToLower().Contains("r"))
            {
                if (MS.DocumentName.ToLower().Contains("rc-")) typedoc = "B";
                if (MS.DocumentName.ToLower().Contains("ra-")) typedoc = "A";
                if (MS.DocumentName.ToLower().Contains("rr-")) typedoc = "A";
            }

            #region Validacion de RUC
            if (!MS.Load(RucNumber))
            {
                if (MS.Envi == "")
                {
                    GeneraLog.LecturaArchivo($"El Tipo de Documento {DocumentType} no está habilitado para el envío a Sunat");
                    GeneraLog.EnvioSunat01("= = = = FIN    DE ENVÍO A SUNAT = = = =");
                    return;
                }
                else
                {
                    GeneraLog.LecturaArchivo($"La empresa con el RUC:{RucNumber} no está habilitada");
                    GeneraLog.EnvioSunat01("= = = = FIN    DE ENVÍO A SUNAT = = = =");
                    return;
                }
            }
            #endregion

            DatabaseConnection DB = new DatabaseConnection(MS);

            // Verificamos en la Base de Datos el numero de Documento ingresado
            int a = DB.SP_ObtenerDocumentoXML(DocumentName);
            MS = DB.MS;


            #region Error de Consulta
            if (a == -1)
            {
                GeneraLog.EnvioSunat01("Error al momento de consultar el documento");
            }
            #endregion
            #region Documento no Existe
            else if (a == 0)
            {
                GeneraLog.EnvioSunat01($"El Documento {DocumentName} no existe en base de datos");
            }
            #endregion
            #region Documento ya ha sido enviado
            else if (a == 1)
            {
                GeneraLog.EnvioSunat01($"El Documento {DocumentName} ya ha sido enviado a SUNAT");
            }
            #endregion

            #region Documento puede ser enviado
            else if (a == 2)
            {
                GeneraLog.EnvioSunat01($"El Documento {DocumentName} está listo para ser enviado a SUNAT");
                if (zipFile(DB, MS))
                {
                    GeneraLog.EnvioSunat01($"Documento comprimido en {MS.ZIPEFileLocation}");
                    SunatConnection SN = null;
                    if ("20|40|RR".Contains(DocumentType))
                    {
                        SN = new SunatConnection("Retenciones", MS);
                    }
                    else if ("01|07|08|RC|RA".Contains(DocumentType) && !DocumentSerie.StartsWith("B"))
                    {
                        SN = new SunatConnection("Documentos", MS);
                    }
                    else if ("03|".Contains(DocumentType) || DocumentSerie.StartsWith("B"))
                    {
                        if (MS.Envi == "HML")
                            SN = new SunatConnection("Documentos", MS);
                    }
                    else
                    {
                        GeneraLog.EnvioSunat01($"Documento {DocumentType} no configurado en ambiente {MS.Envi}");
                    }
                    //Thread.Sleep(5000);
                    if (SN != null)
                    {
                        #region Consulta de Ticket
                        if (isQuery)
                        {
                            Tuple<string, bool> XQ = null;
                            XQ = SN.ObtenerEstadoUnit(MS.im);

                            if (XQ.Item2)
                            {
                                #region my change
                                if (XQ.Item1 != null && XQ.Item1.Length > 200)//valida que sea un xml
                                {
                                    //aqui descomprime el cdr
                                    if (UnZipCDR(MS, XQ.Item1))
                                    {
                                        //obtiene el codigo de respuesta en el cdr
                                        #region insert cdr and update document header
                                        var rs = ADE.Extras.Common.Method.Methods.Instance.GetActionCodeResponse_CDR(MS);
                                        switch (rs)
                                        {
                                            case "SOK"://ok
                                            case "OBS"://obsv
                                                {
                                                    DB.SP_InsertaCDRResumen(CDR, MS.id, SummaryType);
                                                    GeneraLog.EnvioSunat01("CDR guardado correctamente");
                                                    GetNotes(DB, MS);

                                                    GeneraLog.EnvioSunat01("Documento ha sido aceptado correctamente por SUNAT");
                                                    //DB.addRSunatResponse(MS.id, "7", CDRmsg, MS.im, SummaryType);
                                                    DB.addRSunatResponse(MS.id, "SOK", CDRmsg, MS.im, SummaryType);

                                                    #region RA, RR NOTIFICACION
                                                    string path = Directory.GetCurrentDirectory();
                                                    string pathlog = MS.LOGS + "7_NotificacionBaja.log";
                                                    //if (typedoc.Contains("A"))
                                                    //{
                                                    var list = DB.Get_ListDocument_AN(MS.id);
                                                    if (list.Count > 0)
                                                    {
                                                        foreach (var d in list)
                                                        {
                                                            if (d.TO.Length > 5 && d.TypeRC == "ANS")
                                                            {
                                                                var res = ADE.Extras.Common.Method.Methods.Instance.SendNotify(path, d);
                                                                ADE.Extras.Common.Method.Methods.Instance.WriteLog(pathlog, "------------------------- INICIO -------------------------");
                                                                ADE.Extras.Common.Method.Methods.Instance.WriteLog(pathlog, "Enviando notificación de baja");
                                                                ADE.Extras.Common.Method.Methods.Instance.WriteLog(pathlog, "Documento    : " + d.Num_CE);
                                                                ADE.Extras.Common.Method.Methods.Instance.WriteLog(pathlog, "Email Destino: " + d.TO);
                                                                ADE.Extras.Common.Method.Methods.Instance.WriteLog(pathlog, "Resultado    : " + res);
                                                                ADE.Extras.Common.Method.Methods.Instance.WriteLog(pathlog, "-------------------------- FIN ---------------------------");

                                                                ADE.Extras.Common.Method.Methods.Instance.ConsoleLog("------------------------- INICIO -------------------------");
                                                                ADE.Extras.Common.Method.Methods.Instance.ConsoleLog("Enviando notificación de baja");
                                                                ADE.Extras.Common.Method.Methods.Instance.ConsoleLog("Documento    : " + d.Num_CE);
                                                                ADE.Extras.Common.Method.Methods.Instance.ConsoleLog("Email Destino: " + d.TO);
                                                                ADE.Extras.Common.Method.Methods.Instance.ConsoleLog("Resultado    : " + res);
                                                                ADE.Extras.Common.Method.Methods.Instance.ConsoleLog("-------------------------- FIN ---------------------------");
                                                            }
                                                        }
                                                    }
                                                    //typedoc = string.Empty;
                                                    //}
                                                    #endregion

                                                    break;
                                                }
                                            case "RCHZ"://rechazado
                                                {
                                                    string item1 = string.Empty;
                                                    if (XQ.Item1 !=  null && XQ.Item1.Length > 200)
                                                    {
                                                        item1 = "CDR";
                                                    }
                                                    else { item1 = XQ.Item1; }

                                                    DB.SP_InsertaCDRResumen(CDR, MS.id, SummaryType);
                                                    GeneraLog.EnvioSunat01("CDR guardado correctamente");
                                                    GetNotes(DB, MS);

                                                    GeneraLog.EnvioSunat01("Documento fue enviado pero tiene errores: " + item1);
                                                    //DB.addRSunatResponse(MS.id, "6", CDRmsg, MS.im, SummaryType);
                                                    DB.addRSunatResponse(MS.id, "SRE", CDRmsg, MS.im, SummaryType);
                                                    break;
                                                }
                                            case "EXC"://exception
                                                {
                                                    string item1 = string.Empty;
                                                    if (XQ.Item1 != null && XQ.Item1.Length > 200)
                                                    {
                                                        item1 = "CDR";
                                                    }
                                                    else { item1 = XQ.Item1; }

                                                    GeneraLog.EnvioSunat01("Se detectaron excepciones al enviar el archivo: " + item1);
                                                    break;
                                                }
                                            case ""://cuando no se obtuvo codigo del cdr
                                                {
                                                    string item1 = string.Empty;
                                                    if (XQ.Item1 != null && XQ.Item1.Length > 200)
                                                    {
                                                        item1 = "CDR";
                                                    }
                                                    else { item1 = XQ.Item1; }

                                                    GeneraLog.EnvioSunat01("El procedimiento no devolvio un cdr: " + item1);
                                                    break;
                                                }
                                        }
                                        #endregion
                                    }
                                    else
                                    {
                                        GeneraLog.EnvioSunat01("Error al descomprimir cdr");
                                    }
                                }
                                else
                                {
                                    GeneraLog.EnvioSunat01("El procedimiento no devolvio un cdr: " + XQ.Item1);
                                }
                                #endregion 
                                //aqui va el unzip de consulta ticket
                                //aqui ternima el unzip de consulta ticket
                            }
                            else
                            {
                                #region code 99                                

                                if (XQ.Item1 != null && XQ.Item1.Length > 200)//valida que sea un xml
                                {
                                    if (UnZipCDR(MS, XQ.Item1))
                                    {
                                        DB.SP_InsertaCDRResumen(CDR, MS.id, SummaryType);
                                        GeneraLog.EnvioSunat01("CDR guardado correctamente");
                                        GetNotes(DB, MS);

                                        string item1 = string.Empty;
                                        if (XQ.Item1 != null && XQ.Item1.Length > 200)
                                        {
                                            item1 = "CDR";
                                        }
                                        else { item1 = XQ.Item1; }

                                        GeneraLog.EnvioSunat01("Documento fue enviado pero tiene errores: " + item1);
                                        //DB.addRSunatResponse(MS.id, "6", CDRmsg, MS.im, SummaryType);
                                        DB.addRSunatResponse(MS.id, "SRE", CDRmsg, MS.im, SummaryType);
                                    }
                                }
                                else
                                {
                                    string item1 = string.Empty;
                                    if (XQ.Item1 != null && XQ.Item1.Length > 200)
                                    {
                                        item1 = "CDR";
                                    }
                                    else { item1 = XQ.Item1; }

                                    GeneraLog.EnvioSunat01("El proceso devolvio: " + item1 + ", " + XQ.Item2);
                                }
                                #endregion
                                //GeneraLog.EnvioSunat01("El procedimiento no devolvio un cdr: " + XQ.Item1);
                                //GeneraLog.EnvioSunat01("Resumen ha sido rechazado");  //descomentar
                                //DB.addRSunatResponse(MS.id, "6", "", MS.im, SummaryType); //descomentar
                            }
                        }
                        #endregion

                        #region Envío a Sunat
                        else
                        {
                            Tuple<string, bool> XA = null;
                            if (isSummary)
                                XA = SN.EnviarResumen(ZIP, MS.ZIPEFileName);
                            else
                                XA = SN.EnviarDocumento(ZIP, MS.ZIPEFileName);
                            
                            #region Envio Correcto
                            if (XA.Item2)
                            {
                                #region Resumenes-solo se enviara hasta el ticket,
                                if (isSummary)
                                {
                                    //aqui quite una parte
                                    DB.addRSunatResponse(MS.id, "ESX", "", XA.Item1, SummaryType);
                                    //aqui termina
                                }
                                #endregion

                                #region Documentos
                                else
                                {
                                    if (UnZipCDR(MS, XA.Item1))
                                    {
                                        DB.SP_InsertaCDR(CDR, DocumentName, "3");
                                        GeneraLog.EnvioSunat01("CDR guardado correctamente");
                                        GetNotes(DB, MS);

                                        int SNCode;
                                        if (int.TryParse(CDRcod, out SNCode))
                                        {
                                            if (SNCode == 0)
                                            {
                                                GeneraLog.EnvioSunat01("Documento ha sido aceptado correctamente por SUNAT");
                                                var res = DB.SP_ActualizaEstado(MS.id, "7", DocumentName);
                                                if(res < 0)
                                                {
                                                    DB.SP_ActualizaEstado(MS.id, "7", DocumentName);
                                                }
                                                GenerateMAILPDF(MS, DB, "7");
                                            }
                                            if (SNCode >= 2000 && SNCode <= 3999)
                                            {
                                                GeneraLog.EnvioSunat01("Documento ha sido rechazado por SUNAT");
                                                DB.SP_ActualizaEstado(MS.id, "6", DocumentName);
                                                GenerateMAILPDF(MS, DB, "6");
                                            }
                                            if (SNCode >= 4000 || CDRnotes.Count > 0)
                                            {
                                                GeneraLog.EnvioSunat01("Documento fue aceptado pero tiene observaciones");
                                                DB.SP_ActualizaEstado(MS.id, "5", DocumentName);
                                                GenerateMAILPDF(MS, DB, "5");
                                            }
                                        }
                                    }
                                    else
                                    {
                                        GeneraLog.EnvioSunat01($"Se produjo un error en la descompresión del documento");
                                    }
                                }
                                #endregion
                            }
                            #endregion

                            #region Error/Excepcion de Envio
                            else
                            {
                                int ErrCode;
                                string ErrSunat = (XA.Item1.Length > 3 ? XA.Item1.Substring(XA.Item1.Length - 4, 4) : XA.Item1);

                                #region Error Conocido
                                if (int.TryParse(ErrSunat, out ErrCode))
                                {
                                    if (isSummary)
                                    {
                                        if (ErrCode >= 2000 && ErrCode <= 3999)
                                        {
                                            MS.GeneraLog.EnvioSunat01($"El Documento ha sido rechazado por Sunat");
                                            //DB.addRSunatResponse(MS.id, "6", $"{ErrCode.ToString()} - El Documento ha sido rechazado por Sunat", "", SummaryType);
                                            DB.addRSunatResponse(MS.id, "SRE", $"{ErrCode.ToString()} - El Documento ha sido rechazado por Sunat", "", SummaryType);
                                        }
                                    }
                                    else
                                    {
                                        if ("20|40".Contains(DocumentType))
                                        {
                                            if (ErrCode == 2282)
                                            {
                                                MS.GeneraLog.EnvioSunat01($"El Documento ya ha sido enviado a Sunat anteriormente");
                                                MS.GeneraLog.EnvioSunat01($"Actualizando estado en Base de Datos");
                                                DB.SP_ActualizaEstado(MS.id, "7", DocumentName);
                                                DB.SP_InsertaMensajeRespuesta(MS.id, "0", $"El Comprobante  numero {DocumentSplit[2]}-{DocumentSplit[3]}, ha sido aceptado");
                                            }
                                            else
                                            {
                                                if (ErrCode >= 2000 && ErrCode <= 3999)
                                                {
                                                    MS.GeneraLog.EnvioSunat01($"El Documento ha sido rechazado por Sunat");
                                                    DB.SP_ActualizaEstado(MS.id, "6", DocumentName);

                                                    //Cambio del dia 10-05-2018
                                                    DB.SP_InsertaMensajeRespuesta_2(MS.id,XA.Item1);
                                                    GenerateMAILPDF(MS, DB, "6");
                                                }
                                            }
                                        }
                                        else
                                        {
                                            DB.SP_ActualizaEstado(MS.id, "4", DocumentName);
                                        }
                                    }
                                }
                                #endregion

                                #region Error Desconocido
                                else
                                {
                                    if (isSummary)
                                    {

                                    }
                                    else
                                    {
                                        DB.SP_ActualizaEstado(MS.id, "4", DocumentName);
                                    }
                                }
                                #endregion
                            }
                            #endregion
                        }
                        #endregion
                    }
                }
                else
                {
                    GeneraLog.EnvioSunat01($"Se produjo un error en la compresión del documento");
                    GeneraLog.EnvioSunat01("= = = = FIN    DE ENVÍO A SUNAT = = = =");
                    return;
                }
            }
            #endregion

            GeneraLog.EnvioSunat01("= = = = FIN    DE ENVÍO A SUNAT = = = =");
            GeneraLog.RegistraLog(DocumentName + ".txt");
        }

        static bool Fill()
        {
            bool isValid = false;

            Regex r = new Regex(@"^\d{11}-\d{2}-[B|F|R|P][A-Z0-9]{3}-[0-9]{8}?$|^\d{11}-\w{2}-[0-9]{8}-[0-9]{1,3}?$", RegexOptions.IgnoreCase);
            Match m = r.Match(DocumentName);
            if (m.Success)
            {
                isValid = true;
            }
            else
            {
                GeneraLog.EnvioSunat01("El Documento ingresado no es válido");
                GeneraLog.EnvioSunat01("= = = = FIN    DE ENVÍO A SUNAT = = = =");
                return isValid;
            }

            DocumentSplit = DocumentName.Split('-');
            RucNumber = DocumentSplit[0];
            DocumentType = DocumentSplit[1];
            DocumentSerie = DocumentSplit[2];
            DocumentNumber = DocumentSplit[3];
            return isValid;
        }

        static bool zipFile(DatabaseConnection DB, MainSettings MS)
        {
            bool isZiped = false;
            if (File.Exists(MS.XMLSFileLocation))
            {
                if (!File.Exists(MS.ZIPEFileLocation))
                {
                    using (ZipArchive newFile = ZipFile.Open(MS.ZIPEFileLocation, ZipArchiveMode.Create))
                    {
                        newFile.CreateEntryFromFile(MS.XMLSFileLocation, MS.XMLFileName, CompressionLevel.Fastest);
                    }
                }

                ZIP = File.ReadAllBytes(MS.ZIPEFileLocation);
                isZiped = true;
            }
            else
            {
                using (var memoryStream = new MemoryStream())
                {
                    using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                    {
                        var demoFile = archive.CreateEntry(MS.XMLFileName);

                        using (var entryStream = demoFile.Open())
                        using (var streamWriter = new StreamWriter(entryStream))
                        {
                            streamWriter.Write(GetString(DB.VAR_FIR));
                        }
                    }
                    ZIP = memoryStream.ToArray();
                    File.WriteAllBytes(MS.ZIPEFileLocation, ZIP);
                    isZiped = true;
                }
            }

            return isZiped;
        }

        static byte[] GetBytes(string str)
        {
            return Encoding.GetEncoding("iso-8859-1").GetBytes(str);
        }

        static string GetString(byte[] bytes)
        {
            return Encoding.GetEncoding("iso-8859-1").GetString(bytes);
        }

        static void GenerateMAILPDF(MainSettings MS, DatabaseConnection DB, string state)
        {
            string XML = "";

            if (DB.SP_MailPDF("P", state))
            {
                XML += $"<DocumentState>";
                XML += $"<ID_CE>{MS.DocumentName}</ID_CE>";
                XML += $"<Tipo_CE>{MS.DocumentType}</Tipo_CE>";
                XML += $"<Estado_Doc>0</Estado_Doc>";
                XML += $"<Estado_Desc>Aceptado correcto</Estado_Desc>";
                XML += $"<RucEmisor>{MS.RucNumber}</RucEmisor>";
                XML += $"<TypeFormat>1</TypeFormat>";
                XML += $"<PrintName>{MS.im}</PrintName>";
                XML += $"<Copies>{MS.Copies}</Copies>";
                XML += $"</DocumentState>";

                //using (StreamWriter sw = new StreamWriter($@"{MS.ADE_ROOT}Procesos\smp\prt\{MS.DocumentName}.xml"))
                using (StreamWriter sw = new StreamWriter($@"{MS.ADE_ROOT_GOD}_ADE\smp\prt\{MS.DocumentName}.xml"))
                {
                    sw.Write(XML);
                }
                GeneraLog.EnvioSunat01("Generando XML para generación de PDF");
            }

            XML = "";

            if (DB.SP_MailPDF("S", state))
            {
                XML += $"<DocumentSend>";
                XML += $"<ID_CE>{MS.DocumentName}</ID_CE>";
                XML += $"<Tipo_CE>{MS.DocumentType}</Tipo_CE>";
                XML += $"<Estado_Doc>0</Estado_Doc>";
                XML += $"<Estado_Desc>Aceptado correcto</Estado_Desc>";
                XML += $"<RucEmisor>{MS.RucNumber}</RucEmisor>";
                XML += $"</DocumentSend>";

                //using (StreamWriter sw = new StreamWriter($@"{MS.ADE_ROOT}Procesos\smc\cds\{MS.DocumentName}.xml"))
                using (StreamWriter sw = new StreamWriter($@"{MS.ADE_ROOT_GOD}_ADE\smc\cds\{MS.DocumentName}.xml"))
                {
                    sw.Write(XML);
                }
                GeneraLog.EnvioSunat01("Generando XML para envío de correo");
            }
        }

        static void GenerateMAILPDFAux(MainSettings MS, DatabaseConnection DB, string state)
        {
            string XML = "";

            if (DB.SP_MailPDFAux("P", state))
            {
                XML += $"<DocumentState>";
                XML += $"<ID_CE>{MS.DocumentNameAux}</ID_CE>";
                XML += $"<Tipo_CE>{MS.DocumentTypeAux}</Tipo_CE>";
                XML += $"<Estado_Doc>0</Estado_Doc>";
                XML += $"<Estado_Desc>Aceptado correcto</Estado_Desc>";
                XML += $"<RucEmisor>{MS.RucNumber}</RucEmisor>";
                XML += $"<TypeFormat>1</TypeFormat>";
                XML += $"<PrintName>{MS.im}</PrintName>";
                XML += $"<Copies>{MS.Copies}</Copies>";
                XML += $"</DocumentState>";

                //using (StreamWriter sw = new StreamWriter($@"{MS.ADE_ROOT}Procesos\smp\prt\{MS.DocumentName}.xml"))
                using (StreamWriter sw = new StreamWriter($@"{MS.ADE_ROOT_GOD}_ADE\smp\prt\{MS.DocumentNameAux}.xml"))
                {
                    sw.Write(XML);
                }
                GeneraLog.EnvioSunat01("Generando XML para generación de PDF de " + MS.DocumentNameAux);
            }

            XML = "";

            if (DB.SP_MailPDFAux("S", state))
            {
                XML += $"<DocumentSend>";
                XML += $"<ID_CE>{MS.DocumentNameAux}</ID_CE>";
                XML += $"<Tipo_CE>{MS.DocumentTypeAux}</Tipo_CE>";
                XML += $"<Estado_Doc>0</Estado_Doc>";
                XML += $"<Estado_Desc>Aceptado correcto</Estado_Desc>";
                XML += $"<RucEmisor>{MS.RucNumber}</RucEmisor>";
                XML += $"</DocumentSend>";

                //using (StreamWriter sw = new StreamWriter($@"{MS.ADE_ROOT}Procesos\smc\cds\{MS.DocumentName}.xml"))
                using (StreamWriter sw = new StreamWriter($@"{MS.ADE_ROOT_GOD}_ADE\smc\cds\{MS.DocumentNameAux}.xml"))
                {
                    sw.Write(XML);
                }
                GeneraLog.EnvioSunat01("Generando XML para envío de correo de " + MS.DocumentNameAux);
            }
        }

        public static bool UnZipCDR(MainSettings MS, string sBase64)
        {
            bool isUnZiped = false;
            FileStream fs = new FileStream(MS.ZIPRFileLocation, FileMode.Create);
            BinaryWriter bw = new BinaryWriter(fs);

            try
            {
                CDRZ = Convert.FromBase64String(sBase64);
                bw.Write(CDRZ);
                isUnZiped = true;
            }
            catch
            {
                isUnZiped = false;
            }
            finally
            {
                fs.Close();
                CDRZ = null;
                bw = null;
                sBase64 = null;
            }
            try
            {
                if (isUnZiped)
                {
                    if (!File.Exists(MS.XMLRFileLocation))
                    {
                        using (ZipArchive archive = ZipFile.OpenRead(MS.ZIPRFileLocation))
                        {
                            foreach (ZipArchiveEntry entry in archive.Entries)
                            {
                                if (entry.FullName.EndsWith(".xml", StringComparison.OrdinalIgnoreCase))
                                {
                                    entry.ExtractToFile(MS.XMLRFileLocation);
                                }
                            }
                        }
                    }

                    CDR = File.ReadAllBytes(MS.XMLRFileLocation);
                    GeneraLog.EnvioSunat01("Descomprimiendo CDR..");
                }
            }
            catch (Exception ex)
            {
                isUnZiped = false;
            }

            return isUnZiped;
        }

        //added method
        //private static int GetCodeResponse_CDR(MainSettings ms)
        //{

        //    XmlDocument xml = new XmlDocument();
        //    xml.Load(ms.XMLRFileLocation);



        //    XmlNodeList nodelist = xml.GetElementsByTagName("cbc:ResponseCode");
        //    if (nodelist != null && nodelist.Count > 0)
        //        return int.Parse(nodelist[0].ChildNodes.Item(0).InnerText);
        //    else
        //        return 0;
        //}

        public static void GetNotes(DatabaseConnection DB, MainSettings MS)
        {
            CDRcod = CDRmsg = "";
            CDRnotes = new List<string>();

            XmlDocument doc = new XmlDocument();
            doc.Load(MS.XMLRFileLocation);

            XmlNamespaceManager ns = new XmlNamespaceManager(doc.NameTable);
            ns.AddNamespace("soap", "http://schemas.xmlsoap.org/soap/envelope/");
            ns.AddNamespace("ext", "urn:oasis:names:specification:ubl:schema:xsd:CommonExtensionComponents-2");
            ns.AddNamespace("ds", "http://www.w3.org/2000/09/xmldsig#");
            ns.AddNamespace("cbc", "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2");
            ns.AddNamespace("cac", "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2");
            ns.AddNamespace("ar", "urn:oasis:names:specification:ubl:schema:xsd:ApplicationResponse-2");

            XmlNodeList nList = doc.SelectNodes("ar:ApplicationResponse/cbc:Note", ns);
            if (nList.Count > 0)
            {
                GeneraLog.IngresoBD02($"Se detectaron notas en el documento de respuesta {MS.XMLRFileLocation}");
                foreach (XmlNode note in nList)
                {
                    Console.WriteLine(note.InnerText);
                    string[] not = note.InnerText.Split('-');
                    if (not.Length > 1)
                    {
                        DB.SP_InsertaNotaSunat(MS.id, DocumentName, not[0].Trim(), not[1].Trim(), 'A');
                    }
                    else
                    {
                        DB.SP_InsertaNotaSunat(MS.id, DocumentName, "", not[0].Trim(), 'A');
                    }
                    CDRnotes.Add(note.InnerText);
                }
            }


            XmlNode drRCList = doc.SelectSingleNode("ar:ApplicationResponse/cac:DocumentResponse/cac:Response/cbc:ResponseCode", ns);
            XmlNode drDEList = doc.SelectSingleNode("ar:ApplicationResponse/cac:DocumentResponse/cac:Response/cbc:Description", ns);

            if (drRCList != null && drDEList != null)
            {
                //Console.WriteLine("ResponseCode: " + drRCList.InnerText);
                //Console.WriteLine("Description : " + drDEList.InnerText);
                CDRcod = drRCList.InnerText;
                CDRmsg = drDEList.InnerText;
                DB.SP_InsertaMensajeRespuesta(MS.id, drRCList.InnerText, drDEList.InnerText);
            }

            ///////////////


            //UblSharp.ApplicationResponseType response = null;
            //using (FileStream fs = File.OpenRead(MS.XMLRFileLocation))
            //{
            //    XmlSerializer xs = new XmlSerializer(typeof(UblSharp.ApplicationResponseType));
            //    fs.Position = 0;
            //    response = (UblSharp.ApplicationResponseType)xs.Deserialize(fs);
            //}
            //if (response != null)
            //{
            //    if (response.Note != null)
            //    {
            //        GeneraLog.IngresoBD02($"Se detectaron notas en el documento de respuesta {MS.XMLRFileLocation}");
            //        foreach (UblSharp.UnqualifiedDataTypes.TextType nota in response.Note)
            //        {
            //            string[] not = nota.Value.Split('-');
            //            if (not.Length > 1)
            //            {
            //                DB.SP_InsertaNotaSunat(MS.id, DocumentName, not[0].Trim(), not[1].Trim(), 'A');
            //            }
            //            else
            //            {
            //                DB.SP_InsertaNotaSunat(MS.id, DocumentName, "", not[0].Trim(), 'A');
            //            }
            //            CDRnotes.Add(nota.Value);
            //        }
            //    }
            //    if (response.DocumentResponse != null)
            //    {
            //        CDRcod = response.DocumentResponse[0].Response.ResponseCode.Value;
            //        CDRmsg = response.DocumentResponse[0].Response.Description[0].Value;
            //        if (CDRmsg != "" || CDRcod != "")
            //        {
            //            DB.SP_InsertaMensajeRespuesta(MS.id, CDRcod, CDRmsg);
            //        }
            //    }
            //}
        }
    }
}
