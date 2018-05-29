using ADE.Configurations.Entities;
using ADE.Configurations.Entities.Database;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace ADE.Configurations.Objects
{
    public class MainSettings
    {
        //Variables para el uso del progama
        public string UserAltern { get; set; }
        /// <summary>
        /// Directorio Raíz de la empresa
        /// </summary>
        public string ADE_ROOT { get; set; }

        /// <summary>
        /// Directorio Raíz de ADE
        /// </summary>
        public string ADE_ROOT_GOD { get; set; }

        /// <summary>
        /// Directorio InterfTXT - Donde se guardan TXT para ser procesados.
        /// </summary>
        public string ITXT { get; set; }

        /// <summary>
        /// Directorio InterfPROC - Donde se copian los TXT si pasaron Validacion
        /// </summary>
        public string IPRC { get; set; }

        /// <summary>
        /// Directorio InterfERRO - Donde se copian los TXT si la validación arroja errores
        /// </summary>
        public string IERR { get; set; }


        /// <summary>
        /// Directorio PDF - Donde se guardaran los PDF Generados
        /// </summary>
        public string OPDF { get; set; }

        /// <summary>
        /// Directorio XML - Donde se guardaran los XML con/sin firma
        /// </summary>
        public string OXML { get; set; }

        /// <summary>
        /// Directorio CDR - Donde se guardan las CDR (Constancias de Recepción) devueltas por SUNAT
        /// </summary>
        public string OCDR { get; set; }

        /// <summary>
        /// Directorio ENVIO - Donde se guarda el XML comprimido antes de ser enviado a SUNAT
        /// </summary>
        public string OENV { get; set; }

        /// <summary>
        /// Directorio PDF417 - Donde se guardan los Códigos de Barra PDF417
        /// </summary>
        public string O417 { get; set; }

        /// <summary>
        /// Directorio FILEDB - Donde se almacena el archivo XML, que actua como BD en casos excepcionales
        /// </summary>
        public string OXDB { get; set; }


        public string PENV { get; set; }

        /// <summary>
        /// Directorio Logs - Donde se guardan los archivos log por cada proceso
        /// </summary>
        public string LOGS { get; set; }

        /// <summary>
        /// Directorio crt - Donde se guarda el certificado, usado para firmar los XML
        /// </summary>
        public string CERF { get; set; }

        /// <summary>
        /// Nombre del Certificado
        /// </summary>
        public string CERN { get; set; }

        /// <summary>
        /// Contraseña del Certificado
        /// </summary>
        public string CERP { get; set; }

        /// <summary>
        /// Ambiente de Trabajo Actual (DEV, HML, PRD)
        /// </summary>
        public string Envi { get; set; }

        /// <summary>
        /// Usuario SOL de Sunat
        /// </summary>
        public string SunatUsr { get; set; }

        /// <summary>
        /// Passsword SOL de Sunat
        /// </summary>
        public string SunatPwd { get; set; }

        /// <summary>
        /// Link de Envio de F, B, NC, ND y Resumenes de Baja/Boletas
        /// </summary>
        public string UEnv { get; set; }

        /// <summary>
        /// Link para consulta de CDR/Status
        /// </summary>
        public string UCdr { get; set; }

        /// <summary>
        /// Link para envío de CP, CR y Resumenes de Reversiones
        /// </summary>
        public string UPRe { get; set; }

        /// <summary>
        /// Link para envío de Guías de Remisión
        /// </summary>
        public string UGRe { get; set; }

        /// <summary>
        /// Nombre de Base de Datos
        /// </summary>
        public string BDUsr { get; set; }

        /// <summary>
        /// Usuario de Base de Datos
        /// </summary>
        public string BDNam { get; set; }

        /// <summary>
        /// Password de Base de Datos
        /// </summary>
        public string BDPwd { get; set; }

        /// <summary>
        /// Dirección de Servidor Base de Datos
        /// </summary>
        public string BDSer { get; set; }

        /// <summary>
        /// Connection String para la Base de Datos
        /// </summary>
        public string BDCon { get; set; }

        public int id = 0;

        public string im { get; set; }
        public int Copies { get; set; }

        public string DocumentName { get; set; }
        public string DocumentNameAux { get; set; }
        public string DocumentType { get; set; }
        public string DocumentTypeAux { get; set; }
        public string RucNumber { get; set; }
        public string SummaryDate { get; set; }
        public int SummaryNumber { get; set; }
        public string XMLFileName { get; set; }
        public string ZIPEFileName { get; set; }
        public string ZIPRFileName { get; set; }
        public string ZIPEFileLocation { get; set; }
        public string ZIPRFileLocation { get; set; }
        public string XMLIFileLocation { get; set; }
        public string XMLRFileLocation { get; set; }
        public string XMLSFileLocation { get; set; }
        public string XMLZFileLocation { get; set; }
        public string CDRZFileLocation { get; set; }
        public string CDRXFileLocation { get; set; }

        public string FileName { get; set; }
        public string FileLocation { get; set; }

        public string XMLI { get; set; }
        public string XMLS { get; set; }

        /// <summary>
        /// Validaciones XML
        /// </summary>
        public List<RegexDB> ValidacionesXML { get; set; }

        /// <summary>
        /// Log 
        /// </summary>
        public Log GeneraLog = null;


        public Encrypter CR = new Encrypter();
        public Empresa Emp = new Empresa();
        public bool ok = false;

        public MainConfig MC;
        /// <summary>
        /// Constructor de la Clase.
        /// Arma las rutas necesarias
        /// </summary>
        public MainSettings()
        {
            Copies = 1;
            var FullPath = Directory.GetCurrentDirectory();
            var LevelsUp = @"..\..\";
            ADE_ROOT = Path.GetFullPath(Path.Combine(FullPath, LevelsUp));

            LevelsUp = @"..\..\..\";
            ADE_ROOT_GOD = Path.GetFullPath(Path.Combine(FullPath, LevelsUp));

            //En caso la ruta esté definida en el config
            if (ConfigurationManager.AppSettings["root"] != null)
            {
                //Ahora verificamos que la ruta exista
                if (Directory.Exists(ConfigurationManager.AppSettings["root"]))
                {
                    //En caso exista, la ruta definida en el config será la que
                    //el proceso usará en adelante.
                    ADE_ROOT = Path.GetFullPath(ConfigurationManager.AppSettings["root"]);
                    LevelsUp = @"..\";
                    ADE_ROOT_GOD = Path.GetFullPath(Path.Combine(ADE_ROOT, LevelsUp));
                }
            }

            ITXT = $@"{ADE_ROOT}EntradaCE\InterfTXT\";
            IPRC = $@"{ADE_ROOT}EntradaCE\InterfPROC\";
            IERR = $@"{ADE_ROOT}EntradaCE\InterfERRO\";

            OPDF = $@"{ADE_ROOT}ProcesoCE\PDF\";
            OXML = $@"{ADE_ROOT}ProcesoCE\XML\";
            OCDR = $@"{ADE_ROOT}ProcesoCE\CDR\";
            OENV = $@"{ADE_ROOT}ProcesoCE\ENVIO\";
            O417 = $@"{ADE_ROOT}ProcesoCE\PDF417\";
            OXDB = $@"{ADE_ROOT}ProcesoCE\FILEDB\";

            PENV = $@"{ADE_ROOT}Procesos\env\ADE.Processes.SunatDelivery.exe";

            LOGS = $@"{ADE_ROOT}Logs\";
            CERF = $@"{ADE_ROOT}Librerias\crt\";

            GeneraLog = new Log(this);

            if (!validateConnectionString())
            {
                GeneraLog.LecturaArchivo("Datos de Conexión a Base de Datos no configurados Correctamente");
                //GeneraLog.LecturaArchivo($"Server  : [{BDSer}]");
                //GeneraLog.LecturaArchivo($"Database: [{BDNam}]");
                //GeneraLog.LecturaArchivo($"UserId  : [{CR.EncryptKey(BDUsr)}]");
                //GeneraLog.LecturaArchivo($"Password: [{CR.EncryptKey(BDPwd)}]");
                //GeneraLog.LecturaArchivo("Todos los datos son obligatorios");
                //GeneraLog.RegistraError("errorxD");
            }
            else
            {
                GeneraLog.LecturaArchivo("Validando Accesos a Base de Datos");
                if (!SP_ValidateConnection())
                {
                    ok = false;
                }
                else
                {
                    ok = true;
                }
            }
        }

        /// <summary>
        /// Constructor de la Clase para Servicio de Resumenes.
        /// Arma las rutas necesarias
        /// </summary>
        public MainSettings(string ruc)
        {
            var FullPath = Directory.GetCurrentDirectory();
            var LevelsUp = @"..\..\";
            ADE_ROOT = Path.GetFullPath(Path.Combine(FullPath, LevelsUp));

            LevelsUp = @"..\..\..\";
            ADE_ROOT_GOD = Path.GetFullPath(Path.Combine(FullPath, LevelsUp));

            //En caso la ruta esté definida en el config
            if (ConfigurationManager.AppSettings[ruc] != null)
            {
                //Ahora verificamos que la ruta exista
                if (Directory.Exists(ConfigurationManager.AppSettings[ruc]))
                {
                    //En caso exista, la ruta definida en el config será la que
                    //el proceso usará en adelante.
                    ADE_ROOT = Path.GetFullPath(ConfigurationManager.AppSettings[ruc]);
                    LevelsUp = @"..\";
                    ADE_ROOT_GOD = Path.GetFullPath(Path.Combine(ADE_ROOT, LevelsUp));
                }
            }

            ITXT = $@"{ADE_ROOT}EntradaCE\InterfTXT\";
            IPRC = $@"{ADE_ROOT}EntradaCE\InterfPROC\";
            IERR = $@"{ADE_ROOT}EntradaCE\InterfERRO\";

            OPDF = $@"{ADE_ROOT}ProcesoCE\PDF\";
            OXML = $@"{ADE_ROOT}ProcesoCE\XML\";
            OCDR = $@"{ADE_ROOT}ProcesoCE\CDR\";
            OENV = $@"{ADE_ROOT}ProcesoCE\ENVIO\";
            O417 = $@"{ADE_ROOT}ProcesoCE\PDF417\";
            OXDB = $@"{ADE_ROOT}ProcesoCE\FILEDB\";

            PENV = $@"{ADE_ROOT}Procesos\env\ADE.Processes.SunatDelivery.exe";

            LOGS = $@"{ADE_ROOT}Logs\";
            CERF = $@"{ADE_ROOT}Librerias\crt\";

            GeneraLog = new Log(this);

            if (!validateConnectionString())
            {
                GeneraLog.LecturaArchivo("Datos de Conexión a Base de Datos no configurados Correctamente");
                //GeneraLog.LecturaArchivo($"Server  : [{BDSer}]");
                //GeneraLog.LecturaArchivo($"Database: [{BDNam}]");
                //GeneraLog.LecturaArchivo($"UserId  : [{CR.EncryptKey(BDUsr)}]");
                //GeneraLog.LecturaArchivo($"Password: [{CR.EncryptKey(BDPwd)}]");
                //GeneraLog.LecturaArchivo("Todos los datos son obligatorios");
                //GeneraLog.RegistraError("errorxD");
            }
            else
            {
                GeneraLog.LecturaArchivo("Validando Accesos a Base de Datos");
                if (!SP_ValidateConnection())
                {
                    ok = false;
                }
                else
                {
                    ok = true;
                }
            }
        }

        /// <summary>
        /// Valida si es que el RUC del archivo proveniente es válido
        /// </summary>
        /// <param name="ruc">RUC del Emisor del Comprobante</param>
        /// <returns></returns>
        public bool Load(string ruc)


        {
            bool ValidRuc = false;
            
            ValidRuc = SP_ValidaEmpresa(ruc);
            if (ValidRuc)
            {
                RucNumber = ruc;

                string DocumentEnvi = "";
                DocumentEnvi = DocumentType;
                if (DocumentType == "RC") DocumentEnvi = "03";
                if (DocumentType == "RA") DocumentEnvi = "01";
                if (DocumentType == "RR") DocumentEnvi = "20";

                SP_ObtieneAmbienteSunat(DocumentEnvi);

                if (SunatUsr == "" || SunatPwd == "")
                {
                    return false;
                }

                if (Envi == "")
                {
                    GeneraLog.LecturaArchivo($"No se ha configurado el ambiente para la empresa {Emp.RazonSocial}");
                    return false;
                }

                GeneraLog.LecturaArchivo($"Ambiente Actual: {Envi}");
            }
            return ValidRuc;
        }

        public void SP_ObtieneAmbienteSunat(string DocumentEnvi)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(BDCon))
                {
                    using (SqlCommand cmd = new SqlCommand("[Fact].[Usp_GetAmbDoc]", con))
                    {
                        SqlDataReader dr;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@DocumentType", DocumentEnvi);
                        cmd.Parameters.AddWithValue("@RUC", RucNumber);
                        con.Open();
                        dr = cmd.ExecuteReader();
                        if (dr.Read())
                        {
                            Envi = dr[0].ToString().Trim();

                            try
                            {
                                SunatUsr = CR.DecryptKey(dr[1].ToString().Trim());
                                SunatPwd = CR.DecryptKey(dr[2].ToString().Trim());
                                UserAltern = "";// dr[4].ToString().Trim();
                            }
                            catch
                            {
                                SunatUsr = "";
                                SunatPwd = "";
                                GeneraLog.LecturaArchivo($"Ocurrio un error al decodificar las credenciales de Sunat");
                            }

                            UEnv = dr[3].ToString().Trim();
                            UCdr = dr[4].ToString().Trim();
                        }
                        else
                        {
                            Envi = "";
                        }
                        con.Close();
                    }
                }
            }
            catch (SqlException ex)
            { }
        }

        public bool SP_ValidaEmpresa(string RucNumber)
        {
            bool BusinessExist = false;

            try
            {
                using (SqlConnection con = new SqlConnection(BDCon))
                {
                    using (SqlCommand cmd = new SqlCommand("[Mtro].[Usp_ValidarRucEmpresaData]", con))
                    {
                        SqlDataReader dr;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@RUC", RucNumber);
                        con.Open();

                        dr = cmd.ExecuteReader();
                        if (dr.Read())
                        {
                            Emp.IdEmpresa = int.Parse(dr[0].ToString().Trim());
                            Emp.CodEmpresa = dr[1].ToString().Trim();
                            Emp.Ubigeo = dr[2].ToString().Trim();
                            Emp.Ruc = dr[3].ToString().Trim();
                            Emp.RazonSocial = dr[4].ToString().Trim();
                            Emp.RazonComercial = dr[5].ToString().Trim();
                            Emp.Telefono = dr[6].ToString().Trim();
                            Emp.Fax = dr[7].ToString().Trim();
                            Emp.Direccion = dr[8].ToString().Trim();
                            Emp.DomicilioFiscal = dr[9].ToString().Trim();
                            Emp.Urbanizacion = dr[10].ToString().Trim();
                            Emp.PaginaWeb = dr[11].ToString().Trim();
                            Emp.Email = dr[12].ToString().Trim();

                            BusinessExist = true;
                        }
                        con.Close();
                    }
                }
            }
            catch (SqlException ex)
            {

            }
            return BusinessExist;
        }

        public bool SP_ValidateConnection()
        {
            using (SqlConnection con = new SqlConnection(BDCon))
            {
                try
                {
                    con.Open();
                    return true;
                }
                catch (SqlException se)
                {
                    GeneraLog.LecturaArchivo("Error de conexión a base de datos: ");
                    GeneraLog.LecturaArchivo($"{se.Message}");
                    return false;
                }
            }
        }

        public void SP_InsertaTC(string ruc, string fecha, string moneda, string cambio)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(BDCon))
                {
                    using (SqlCommand cmd = new SqlCommand("[Fact].[O.Usp_InsertaTC]", con))
                    {
                        SqlDataReader dr;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@RUC", ruc);
                        cmd.Parameters.AddWithValue("@FECHA", fecha);
                        cmd.Parameters.AddWithValue("@MONEDA", "USD");
                        cmd.Parameters.AddWithValue("@CAMBIO", decimal.Parse(cambio, CultureInfo.CreateSpecificCulture("es-PE")));
                        con.Open();
                        dr = cmd.ExecuteReader();
                        GeneraLog.GeneracionXML($"Se guarda el TC de Sunat: {fecha} : {cambio}");
                    }
                }
            }
            catch (SqlException ex)
            {

            }
        }

        public bool validateConnectionString()
        {
            bool res = true;

            //MC = new MainConfig();
            XmlSerializer MCSerializer = new XmlSerializer(typeof(MainConfig));
            try
            {
                using (XmlReader xr = XmlReader.Create($@"{ADE_ROOT}Configuracion\MainConfig.xml"))
                {
                    MC = (MainConfig)MCSerializer.Deserialize(xr);
                }
            }
            catch (Exception e)
            {
                GeneraLog.LecturaArchivo($@"Error al obtener la configuración del archivo {ADE_ROOT}Configuracion\MainConfig.xml");
                return false;
            }


            //BDSer = ConfigurationManager.AppSettings["Server"];
            //BDNam = ConfigurationManager.AppSettings["Database"];
            //BDUsr = ConfigurationManager.AppSettings["UserId"];
            //BDPwd = ConfigurationManager.AppSettings["Password"];
            //CERN = ConfigurationManager.AppSettings["CerName"];
            //CERP = CR.DecryptKey(ConfigurationManager.AppSettings["CerPass"]);

            BDSer = MC.Srv.Value;
            BDNam = MC.Dbn.Value;
            BDUsr = MC.Uid.Value;
            BDPwd = MC.Pwd.Value;
            CERN = MC.Cna.Value;
            CERP = MC.Cpa.Value;

            if (BDSer == "") return false;
            if (BDNam == "") return false;
            if (BDUsr == "") return false;
            if (BDPwd == "") return false;

            BDPwd = CR.DecryptKey(BDPwd);
            BDUsr = CR.DecryptKey(BDUsr);
            CERP = CR.DecryptKey(CERP);
            BDCon = $"Server={BDSer};Database={BDNam};User Id={BDUsr};Password={BDPwd};Connection Timeout=150;pooling=false;";

            return res;
        }

        public void getFileNames()
        {
            XMLFileName = DocumentName + ".xml";
            ZIPEFileName = DocumentName + ".zip";
            ZIPRFileName = "R-" + DocumentName + ".zip";

            ZIPEFileLocation = OENV + DocumentName + ".zip";
            ZIPRFileLocation = OCDR + "R-" + DocumentName + ".zip";
            XMLRFileLocation = OCDR + "R-" + DocumentName + ".xml";
            XMLIFileLocation = OXML + DocumentName + ".ini.xml";
            XMLSFileLocation = OXML + DocumentName + ".xml";
            XMLZFileLocation = OENV + DocumentName + ".zip";
            CDRZFileLocation = OCDR + "R-" + DocumentName + ".zip";
            CDRXFileLocation = OCDR + "R-" + DocumentName + ".xml";
        }
    }
}
