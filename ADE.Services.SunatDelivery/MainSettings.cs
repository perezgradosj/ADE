using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;

namespace ADE.Services.SunatDelivery
{
    class MainSettings
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

        /// <summary>
        /// Numero Máximo de Envíos
        /// </summary>
        public int Interval { get; set; }

        public Encrypter CR = new Encrypter();

        public MainSettings()
        {
            var FullPath = Directory.GetCurrentDirectory();
            var LevelsUp = @"..\..\";
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
            List<string> Documents = null;
            List<string> ListVerf = null;
            try
            {
                using (SqlConnection con = new SqlConnection(BDCon))
                {
                    using (SqlCommand cmd = new SqlCommand("[Fact].[O.Usp_ObtenerDocumentoParaEnviar]", con))
                    {
                        Documents = new List<string>();
                        ListVerf = new List<string>();

                        SqlDataReader dr;
                        con.Open();
                        dr = cmd.ExecuteReader();
                        Interval = -1;
                        while (dr.Read())
                        {
                            var TypeRc = dr[2].ToString();
                            var tp = TypeRc.Length > 0 ? TypeRc.ToUpper() : string.Empty;
                            Documents.Add(tp + "|" + dr[0].ToString().Trim());
                            #region last
                            ////if (TypeRc == "ANS")
                            //if (TypeRc == "ANS")
                            //{
                            //    if (!ListVerf.Contains("XGN"))
                            //    {
                            //        Documents.Add(dr[0].ToString().Trim());
                            //        ListVerf.Add(TypeRc);
                            //    }
                            //}
                            //else
                            //{
                            //    Documents.Add(dr[0].ToString().Trim());
                            //    ListVerf.Add(TypeRc);
                            //}
                            #endregion

                            #region dont
                            ////aqui estoy modificando

                            //    if(TypeRc == "ANS")
                            //    {
                            //        if (!ListVerf.Contains("NCD"))
                            //        {

                            //            if (!ListVerf.Contains("XGN"))
                            //            {
                            //                Documents.Add(dr[0].ToString().Trim());
                            //                ListVerf.Add(TypeRc);
                            //            }
                            //        }
                            //    }
                            //    else
                            //    {
                            //        if(TypeRc == "NCD")
                            //        {
                            //            if (!ListVerf.Contains("XGN"))
                            //            {
                            //                Documents.Add(dr[0].ToString().Trim());
                            //                ListVerf.Add(TypeRc);
                            //            }
                            //        }
                            //    }



                            //    if (TypeRc == "NCD")
                            //    {
                            //        if (!ListVerf.Contains("XGN"))
                            //        {
                            //            Documents.Add(dr[0].ToString().Trim());
                            //            ListVerf.Add(TypeRc);
                            //        }
                            //    }
                            //}

                            //Documents.Sort();

                            //foreach(var d in Documents)
                            //{

                            //}

                            //var TypeRc = dr[3].ToString();
                            //TypeRc = TypeRc.Length > 0 ? TypeRc.ToUpper() : string.Empty;
                            #endregion
                        }
                        con.Close();
                    }
                }


                #region new

                //Documents.Sort();
                //Documents.Reverse();

                var li = new List<string>();
                var li2 = new List<string>();
                foreach (var s in Documents)
                {
                    string[] array = s.Split('|');

                    var ss = array[0];
                    var val = array[1];

                    if (ss == "XGN")
                    {
                        if (!li2.Contains("ANS") && !li2.Contains("NCD"))
                        {
                            li.Add(val);
                            li2.Add(ss);
                        }         
                    }
                    else if (ss == "NCD")
                    {
                        if (!li2.Contains("ANS") && !li2.Contains("XGN"))
                        {
                            li.Add(val);
                            li2.Add(ss);
                        }
                    }
                    else if (ss == "ANS")
                    {
                        if (!li2.Contains("NCD") && !li2.Contains("XGN"))
                        {
                            li.Add(val);
                            li2.Add(ss);
                        }
                    }
                    else
                    {
                        li.Add(val);
                        li2.Add(ss);
                    }
                }

                Documents = new List<string>();
                Documents = li;
                #endregion
                //return li;
            }
            catch (SqlException ex)
            {
                Log.WriteLine("[error] " + ex.Message);
            }
            return Documents;
        }

        public int getInterval()
        {
            Interval = -1;
            try
            {
                using (SqlConnection con = new SqlConnection(BDCon))
                {
                    using (SqlCommand cmd = new SqlCommand("[Conf].[Usp_ObtieneIntervaloEnvio]", con))
                    {
                        SqlDataReader dr;
                        con.Open();
                        dr = cmd.ExecuteReader();
                        while (dr.Read())
                        {
                            if (Interval == -1) Interval = int.Parse(dr[0].ToString().Trim());
                        }
                        con.Close();
                    }
                }
            }
            catch (SqlException ex)
            {
                Log.WriteLine("[error] " + ex.Message);
                //Interval = -2;
            }
            return Interval;
        }
    }
}
