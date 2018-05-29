using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace ADE.Extras.MailNotifier
{
    public class MainSettings
    {
        //Variables para el uso del servicio
        public string ADE_ROOT { get; set; }
        public string LOGS { get; set; }
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

        public Encrypter CR = new Encrypter();

        public MainSettings()
        {
            var FullPath = Directory.GetCurrentDirectory();
            var LevelsUp = @"..\";
            ADE_ROOT = Path.GetFullPath(Path.Combine(FullPath, LevelsUp));
            //ADE_ROOT = @"D:\SLIN-ADE\";
            LOGS = $@"{ADE_ROOT}Logs\";
        }

        public bool validateConnectionString()
        {
            bool res = true;
            BDSer = ConfigurationManager.AppSettings["Server"];
            BDNam = ConfigurationManager.AppSettings["Database"];
            BDUsr = ConfigurationManager.AppSettings["UserId"];
            BDPwd = ConfigurationManager.AppSettings["Password"];

            if (BDSer == "") return false;
            if (BDNam == "") return false;
            if (BDUsr == "") return false;
            if (BDPwd == "") return false;

            BDPwd = CR.DecryptKey(BDPwd);
            BDUsr = CR.DecryptKey(BDUsr);

            BDCon = $"Server={BDSer};Database={BDNam};User Id={BDUsr};Password={BDPwd};Connection Timeout=150;pooling=false;";

            return res;
        }

        public List<string> getDocs()
        {
            List<string> Documents = new List<string>();
            try
            {
                using (SqlConnection con = new SqlConnection(BDCon))
                {
                    using (SqlCommand cmd = new SqlCommand("[Fact].[O.Usp_ObtenerDocumentoParaEnviar]", con))
                    {
                        SqlDataReader dr;
                        con.Open();
                        dr = cmd.ExecuteReader();
                        while(dr.Read())
                        {
                            Documents.Add(dr[0].ToString().Trim());
                        }
                        con.Close();
                    }
                }
            }
            catch (SqlException ex)
            {

            }
            return Documents;
        }

        public List<ServicioResumen> getIntervals()
        {
            List<ServicioResumen> Services = new List<ServicioResumen>();
            try
            {
                using (SqlConnection con = new SqlConnection(BDCon))
                {
                    using (SqlCommand cmd = new SqlCommand("[Conf].[Usp_ObtieneIntervaloServicio]", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@CodeService", "ADE.Services.SummaryDelivery");
                        con.Open();
                        SqlDataReader dr;
                        dr = cmd.ExecuteReader();
                        while (dr.Read())
                        {
                            ServicioResumen line = new ServicioResumen();
                            line.Id = dr[0].ToString().Trim();
                            line.CodeService = dr[1].ToString().Trim();
                            line.NameService = dr[2].ToString().Trim();
                            line.ValueTime = dr[3].ToString().Trim();
                            line.IntervalValue = dr[4].ToString().Trim();
                            line.MaxNumAttempts = dr[5].ToString().Trim();
                            line.RucEntity = dr[6].ToString().Trim();
                            line.IdEstado = dr[7].ToString().Trim();
                            line.SubType = dr[8].ToString().Trim();
                            Services.Add(line);
                        }
                        con.Close();
                    }
                }
            }
            catch (SqlException ex)
            {
                
            }
            return Services;
        }
        
        public MailObject SP_ObtieneMailConfig()
        {
            MailObject MO = new MailObject();
            List<string> Documents = new List<string>();
            try
            {
                using (SqlConnection con = new SqlConnection(BDCon))
                {
                    using (SqlCommand cmd = new SqlCommand("[Fact].[c]", con))
                    {
                        SqlDataReader dr;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@RucEmpresa", "");
                        con.Open();
                        dr = cmd.ExecuteReader();
                        if (dr.Read())
                        {
                            MO.RUC = dr[0].ToString().Trim();
                            MO.EMAIL = dr[1].ToString().Trim();
                            MO.PASSWORD = dr[2].ToString().Trim();
                            MO.DOMAIN = dr[3].ToString().Trim();
                            MO.IP = dr[4].ToString().Trim();
                            MO.PORT = dr[5].ToString().Trim();
                            MO.USESSL = dr[6].ToString().Trim();
                            MO.MAILNOTIF = dr[7].ToString().Trim();
                            con.Close();
                            return MO;
                        }
                        con.Close();
                    }
                }
            }
            catch (SqlException ex)
            {
                
            }
            return MO;
        } 
    }

    public class ServicioResumen
    {
        public string Id { get; set; }
        public string CodeService { get; set; }
        public string NameService { get; set; }
        public string ValueTime { get; set; }
        public string IntervalValue { get; set; }
        public string MaxNumAttempts { get; set; }
        public string RucEntity { get; set; }
        public string IdEstado { get; set; }
        public string SubType { get; set; }
        public string Path { get; set; }

        public ServicioResumen()
        {
            Id = "";
            CodeService = "";
            NameService = "";
            ValueTime = "";
            IntervalValue = "";
            MaxNumAttempts = "";
            RucEntity = "";
            IdEstado = "";
            SubType = "";
            Path = "";
        }
    }

    public class MailObject
    {
        public string RUC { get; set; }
        public string EMAIL { get; set; }
        public string PASSWORD { get; set; }
        public string DOMAIN { get; set; }
        public string IP { get; set; }
        public string PORT { get; set; }
        public string USESSL { get; set; }
        public string MAILNOTIF { get; set; }

        public MailObject()
        {
            RUC = "";
            EMAIL = "";
            PASSWORD = "";
            DOMAIN = "";
            IP = "";
            PORT = "";
            USESSL = "";
            MAILNOTIF = "";
        }
    }

    public class DocumentsPending
    {
        public string CPE { get; set; }
        public DateTime FEC { get; set; }
        public decimal TOT { get; set; }
        public string RZN { get; set; }
        public string EST { get; set; }

        public DocumentsPending()
        {
            CPE = "";
            FEC = DateTime.MinValue;
            TOT = 0.00m;
        }
    }
}
