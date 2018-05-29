using ADE.Configurations.DataAccess;
using ADE.Configurations.Entities.Summaries;
using ADE.Configurations.Objects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace ADE.Processes.DatabaseIncome
{
    class Receiver
    {
        static Log GeneraLog = null;
        static string FileName, FileLocation, DocumentName, DocumentType, RucNumber, SummaryDate;
        static Validation VD;
        static string TypeRC = string.Empty;

        static string PathWriteRC_In = string.Empty;
        static string ContentWriteRC_In = string.Empty;


        static List<RBoletasDetalle> LowDocs = new List<RBoletasDetalle>();
        static List<RBoletasDetalle> ListNoteDocs = new List<RBoletasDetalle>();
        static int ItsCantDocs = 0;


        static void Main(string[] args)
        {
            FileName = FileLocation = DocumentName = DocumentType = RucNumber = SummaryDate = "";

            //Variables que se usaran dentro del proceso de Validacion/GeneracionXML/IngresoBD
            List<string> DataTXT = new List<string>();
            DateTime hi = DateTime.Now;

            // Inicializa Configuración Inicial
            MainSettings MS = new MainSettings();
            GeneraLog = MS.GeneraLog;
            GeneraLog.LecturaArchivo("= = = = INICIO DE LECTURA DE ARCHIVO = = =");

            // Verificamos si es que la ruta está en args[]
            if (args.Length < 1)
            {
                args = new string[1];
                GeneraLog.LecturaArchivo(": : MODO MANUAL");
                GeneraLog.LecturaArchivo("Ingrese el nombre del archivo a procesar: ");
                args[0] = Console.ReadLine();
                GeneraLog.LecturaArchivo("Nombre del archivo ingresado manualmente: " + args[0]);
            }

            // Armamos la ruta y nombre del archivo
            MS.FileName = FileName = args[0];
            MS.FileLocation = FileLocation = MS.ITXT + FileName;

            // Si es que el FileName es vacío
            if (args[0].Length == 0)
            {
                GeneraLog.LecturaArchivo($"No se ha ingresado nombre del archivo a Procesar");
                GeneraLog.LecturaArchivo("= = = = FIN    DE LECTURA DE ARCHIVO = = =\n");
                GeneraLog.RegistraError(FileName);
                goto EndProgram;
            }

            GeneraLog.LecturaArchivo($"Ruta completa del Archivo a Verificar:{MS.ITXT + args[0]}");

           // bool archivoTxt = File.Exists(FileLocation+".txt");

            // Ahora verificamos que el archivo indicado exista.
            if (!File.Exists(FileLocation))
            {
                GeneraLog.LecturaArchivo($"Archivo en: {FileLocation} no existe");
                GeneraLog.LecturaArchivo("= = = = FIN    DE LECTURA DE ARCHIVO = = =\n");
                return;
            }

            if (!MS.ok)
            {
                if (File.Exists(MS.IERR + FileName)) File.Delete(MS.IERR + FileName);
                File.Move(FileLocation, MS.IERR + FileName);
                GeneraLog.LecturaArchivo("Se movió el archivo a la ruta " + MS.IERR + FileName);
                GeneraLog.RegistraError(FileName);
                goto EndProgram;
            }


            GeneraLog.LecturaArchivo($"Empezando a leer el archivo {FileName} ");

            using (StreamReader sr = new StreamReader(FileLocation, System.Text.Encoding.GetEncoding("ISO-8859-1")))
            {
                ContentWriteRC_In = string.Empty;
                string line = "";
                while ((line = sr.ReadLine()) != null)
                {
                    ContentWriteRC_In = line;
                    line = line.Replace("§", "°");
                    line = line.Replace("¥", "Ñ");
                    DataTXT.Add(line);
                    if (line.StartsWith("CABECERA-PRINCIPAL")) DocumentType = line;
                    if (line.StartsWith("CABECERA-EMISOR")) RucNumber = line;
                }
            }

            if (DocumentType.ToString() != "")
            {
                string[] HTemp = DocumentType.Split('|');
                string[] ETemp = RucNumber.Split('|');
                try
                {
                    int n;
                    DocumentType = HTemp[1].Trim();
                    SummaryDate = HTemp[2];
                    if (!DocumentType.StartsWith("R"))
                    {
                        RucNumber = ETemp[2];
                    }
                    else
                    {
                        RucNumber = HTemp[3];
                    }
                    if (int.TryParse(DocumentType, out n))
                    {
                        string docs = "01|03|07|08|20|40|RA|RC|RR";
                        if (docs.Contains(DocumentType))
                        {
                            GeneraLog.LecturaArchivo("Documento detectado: " + DocumentType);
                        }
                        else
                        {
                            GeneraLog.LecturaArchivo("El tipo de documento del archivo no es válido");
                            if (File.Exists(MS.IERR + FileName)) File.Delete(MS.IERR + FileName);
                            File.Move(FileLocation, MS.IERR + FileName);
                            GeneraLog.LecturaArchivo("Se movió el archivo a la ruta " + MS.IERR + FileName);
                            GeneraLog.RegistraError(FileName);
                            goto EndProgram;
                        }
                    }
                    else
                    {
                        if (DocumentType.StartsWith("R"))
                        {
                            SummaryDate = HTemp[2];

                            if (HTemp.Length > 4)
                            {
                                TypeRC = HTemp[4];
                            }
                            else
                            {
                                TypeRC = "XGN";
                            }

                            GeneraLog.LecturaArchivo("El archivo corresponde a un Resumen");
                        }
                    }
                }
                catch (Exception e)
                {
                    GeneraLog.LecturaArchivo("No se pudo obtener el tipo de documento del archivo");
                    if (File.Exists(MS.IERR + FileName)) File.Delete(MS.IERR + FileName);
                    File.Move(FileLocation, MS.IERR + FileName);
                    GeneraLog.LecturaArchivo($"Se movió el archivo a la ruta{MS.IERR + FileName}");
                    GeneraLog.RegistraError(FileName);
                    goto EndProgram;
                }
            }
            else
            {
                GeneraLog.LecturaArchivo("Tag CABECERA-PRINCIPAL es obligatorio");
                if (File.Exists(MS.IERR + FileName)) File.Delete(MS.IERR + FileName);
                File.Move(FileLocation, MS.IERR + FileName);
                GeneraLog.LecturaArchivo($"Se movió el archivo a la ruta{MS.IERR + FileName}");
                GeneraLog.RegistraError(FileName);
                goto EndProgram;
            }

            MS.DocumentType = DocumentType;
            MS.SummaryDate = SummaryDate;

            if (!MS.Load(RucNumber))
            {
                if (File.Exists(MS.IERR + FileName)) File.Delete(MS.IERR + FileName);
                File.Move(FileLocation, MS.IERR + FileName);
                GeneraLog.LecturaArchivo($"La empresa con el RUC:{RucNumber} no está habilitada");
                GeneraLog.RegistraError(FileName);
                goto EndProgram;
            }
            GeneraLog.LecturaArchivo("= = = = FIN    DE LECTURA DE ARCHIVO = = =\n");

            // Inicia Validación en ingreso a base de datos

            VD = new Validation(MS);
            if (VD.ValidateInterface(DataTXT) || DocumentType.StartsWith("R"))
            {
                MS = VD.MS;
                MS.getFileNames();
                XMLGeneration XML = new XMLGeneration(MS, VD);


                //bool res = (bool)XML.Generate();
                //var list = (Extras.Common.Method.ListUtilClass)XML.Generate(TypeRC);

                var list = XML.Generate(TypeRC);

                //if ((bool)XML.Generate())
                ItsCantDocs = 0;
                LowDocs = new List<RBoletasDetalle>();
                LowDocs = list.ListBoletasLow;
                ItsCantDocs = list.CantDocs;

                //aqui
                ListNoteDocs = list.ListNotesDoc;

                if (list.Generate == true)
                {
                    if (DocumentType.StartsWith("R")) MS = XML.MS;
                    //var xd = new DatabaseIncome(MS).InsertInDatabase(XML.Document, new Extras.Common.Method.ListUtilClass());
                    var xd = new DatabaseIncome(MS).InsertInDatabase(XML.Document, list, TypeRC);
                    if (xd)
                    {
                        DatabaseConnection DB = new DatabaseConnection(MS);
                        GenerateMAILPDF(MS, DB, "2");
                    }
                    if (File.Exists(MS.IPRC + FileName)) File.Delete(MS.IPRC + FileName);
                    File.Move(FileLocation, MS.IPRC + FileName);
                    goto EndProgram;
                }
                else
                {
                    if (File.Exists(MS.IERR + FileName)) File.Delete(MS.IERR + FileName);
                    File.Move(FileLocation, MS.IERR + FileName);
                    GeneraLog.RegistraError(FileName);
                    goto EndProgram;
                }
            }
            else
            {
                if (File.Exists(MS.IERR + FileName)) File.Delete(MS.IERR + FileName);
                File.Move(FileLocation, MS.IERR + FileName);
                GeneraLog.RegistraError(FileName);
                goto EndProgram;
            }

            EndProgram:

            DateTime hf = DateTime.Now;
            GeneraLog.LecturaArchivo("Duración : " + (hf - hi).TotalMinutes + " ms");
            GeneraLog.LecturaArchivo("Fin del Proceso..");
            GeneraLog.LecturaArchivo("-----------------------------------------------------");
            GeneraLog.RegistraLog(FileName);

            //si se encontro documentos en estado anulado then escribe un txt para resumen de bajas 
            //genera un nuevo txt
            //if(LowDocs.Count > 0 && !ContentWriteRC_In.Contains("ANS"))
            //{
            //    PathWriteRC_In = string.Empty;
            //    PathWriteRC_In = Path.GetFullPath(Path.Combine(MS.IERR, "../../../"));
            //    PathWriteRC_In += @"_ADE\in\" + FileName;
            //    using (StreamWriter sw = new StreamWriter(PathWriteRC_In))
            //    {
            //        sw.Write(ContentWriteRC_In + "|ANS");
            //    }
            //}

            if (ListNoteDocs.Count > 0)
            {
                PathWriteRC_In = string.Empty;
                PathWriteRC_In = Path.GetFullPath(Path.Combine(MS.IERR, "../../../"));
                PathWriteRC_In += @"_ADE\in\" + FileName;
                using (StreamWriter sw = new StreamWriter(PathWriteRC_In))
                {
                    sw.Write(ContentWriteRC_In + "|NCD");
                }
            }
            else
            {

                if(ItsCantDocs > LowDocs.Count)
                {
                    PathWriteRC_In = string.Empty;
                    PathWriteRC_In = Path.GetFullPath(Path.Combine(MS.IERR, "../../../"));
                    PathWriteRC_In += @"_ADE\in\" + FileName;
                    using (StreamWriter sw = new StreamWriter(PathWriteRC_In))
                    {
                        sw.Write(ContentWriteRC_In);
                    }
                }
                else
                {
                    if (LowDocs.Count > 0 && !ContentWriteRC_In.Contains("ANS"))
                    {
                        PathWriteRC_In = string.Empty;
                        PathWriteRC_In = Path.GetFullPath(Path.Combine(MS.IERR, "../../../"));
                        PathWriteRC_In += @"_ADE\in\" + FileName;
                        using (StreamWriter sw = new StreamWriter(PathWriteRC_In))
                        {
                            if (ContentWriteRC_In.Contains("NCD"))
                            { ContentWriteRC_In = ContentWriteRC_In.Replace("NCD", "ANS"); }
                            else
                            { ContentWriteRC_In = ContentWriteRC_In + "|ANS"; }
                            sw.Write(ContentWriteRC_In);
                        }
                    }
                }

                //if (LowDocs.Count > 0 && !ContentWriteRC_In.Contains("ANS"))
                //{
                //    PathWriteRC_In = string.Empty;
                //    PathWriteRC_In = Path.GetFullPath(Path.Combine(MS.IERR, "../../../"));
                //    PathWriteRC_In += @"_ADE\in\" + FileName;
                //    using (StreamWriter sw = new StreamWriter(PathWriteRC_In))
                //    {
                //        if (ContentWriteRC_In.Contains("NCD"))
                //        { ContentWriteRC_In = ContentWriteRC_In.Replace("NCD", "ANS"); }
                //        else
                //        { ContentWriteRC_In = ContentWriteRC_In + "|ANS"; }
                //        sw.Write(ContentWriteRC_In);
                //    }
                //}
            }

            if (ItsCantDocs > 0)
            {
                PathWriteRC_In = string.Empty;
                PathWriteRC_In = Path.GetFullPath(Path.Combine(MS.IERR, "../../../"));
                PathWriteRC_In += @"_ADE\in\" + FileName;
                using (StreamWriter sw = new StreamWriter(PathWriteRC_In))
                {
                    sw.Write(ContentWriteRC_In);
                }
            }
            //EnvioSunat(MS);
        }

        private static bool GetExclusiveFileLock(string path)
        {
            var fileReady = false;
            const int MaximumAttemptsAllowed = 50;
            var attemptsMade = 0;

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
                XML += $"<PrintName>{VD.Interface["Impresora"]}</PrintName>";
                XML += $"</DocumentState>";

                using (StreamWriter sw = new StreamWriter($@"{MS.ADE_ROOT_GOD}_ADE\smp\prt\{MS.DocumentName}.xml"))
                {
                    sw.Write(XML);
                }
                GeneraLog.EnvioSunat01("Generando XML para generación de PDF");
            }

            XML = "";

            if (DB.SP_MailPDF("S", state) || MS.DocumentType == "03" || MS.DocumentName.Substring(15, 1) == "B")
            {
                XML += $"<DocumentSend>";
                XML += $"<ID_CE>{MS.DocumentName}</ID_CE>";
                XML += $"<Tipo_CE>{MS.DocumentType}</Tipo_CE>";
                XML += $"<Estado_Doc>0</Estado_Doc>";
                XML += $"<Estado_Desc>Aceptado correcto</Estado_Desc>";
                XML += $"<RucEmisor>{MS.RucNumber}</RucEmisor>";
                XML += $"</DocumentSend>";

                using (StreamWriter sw = new StreamWriter($@"{MS.ADE_ROOT_GOD}_ADE\smc\cds\{MS.DocumentName}.xml"))
                {
                    sw.Write(XML);
                }
                GeneraLog.EnvioSunat01("Generando XML para envío de correo");
            }

            //if (DB.SP_MailPDF("S", state))
            //{
            //    if(MS.DocumentName.Contains("B"))
            //    {
            //        XML += $"<DocumentSend>";
            //        XML += $"<ID_CE>{MS.DocumentName}</ID_CE>";
            //        XML += $"<Tipo_CE>{MS.DocumentType}</Tipo_CE>";
            //        XML += $"<Estado_Doc>0</Estado_Doc>";
            //        XML += $"<Estado_Desc>Aceptado correcto</Estado_Desc>";
            //        XML += $"<RucEmisor>{MS.RucNumber}</RucEmisor>";
            //        XML += $"</DocumentSend>";

            //        using (StreamWriter sw = new StreamWriter($@"{MS.ADE_ROOT_GOD}_ADE\smc\cds\{MS.DocumentName}.xml"))
            //        {
            //            sw.Write(XML);
            //        }
            //        GeneraLog.EnvioSunat01("Generando XML para envío de correo");
            //    }
            //}
        }

        public static void EnvioSunat(MainSettings MS)
        {
            //string sPassword = "@ncr0c0n3ct1v4";
            //SecureString ss = sPassword.

            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.CreateNoWindow = false;
            startInfo.FileName = MS.PENV;
            startInfo.Arguments = MS.DocumentName;
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = false;
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;

            //startInfo.Domain = "ancro";
            //startInfo.UserName = "Administrator";

            System.Threading.Thread.Sleep(100);

            System.Diagnostics.Process proc = new System.Diagnostics.Process();
            proc.StartInfo = startInfo;
            proc.Start();

            //proc.WaitForExit();
            //proc.Close();
        }
    }

    //public class SummaryService
    //{

    //    public MainSettings MS = null;
    //    static Log GeneraLog = null;

    //    public string ProcesaResumen(string fecha, string ruc, string tipo, string TypeRC)
    //    {
    //        string msg = "";
    //        MS = new MainSettings(ruc);
    //        GeneraLog = MS.GeneraLog;
    //        MS.SummaryDate = fecha;
    //        MS.RucNumber = ruc;
    //        MS.DocumentType = tipo;

    //        MS.Load(ruc);

    //        Validation VD = new Validation(MS);

    //        XMLGeneration XML = new XMLGeneration(MS, VD);

    //        //var list = (Extras.Common.Method.ListUtilClass)XML.Generate(TypeRC);
    //        var list = XML.Generate(TypeRC);

    //        //XML.ListUpdRC = list;

    //        if (list.Count > 0)
    //        {

    //            if (MS.DocumentType.StartsWith("R")) MS = XML.MS;

    //            var xd = new DatabaseIncome(MS).InsertInDatabase(XML.Document, list, TypeRC);
    //            if (xd)
    //            {
    //                msg = "Resumen Generado Correctamente";
    //            }
    //            else
    //            {
    //                if (XML.Document.RAD.Count == 0 && XML.Document.RBD.Count == 0)
    //                {
    //                    msg = "No se encontraron documentos en la fecha " + fecha;
    //                }
    //                else
    //                {
    //                    msg = "Ocurrió un error al guardar Resumen";
    //                }
    //            }
    //        }
    //        else
    //        {
    //            if (XML.Document.RAD.Count == 0 && XML.Document.RBD.Count == 0)
    //            {
    //                msg = "No se encontraron documentos en la fecha " + fecha;
    //            }
    //            else
    //            {
    //                msg = "Ocurrió un error al guardar Resumen";
    //            }
    //        }
    //        GeneraLog.RegistraLog("");
    //        return msg;
    //    }
    //}
}
