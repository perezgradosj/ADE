using ADE.Configurations.Entities;
using ADE.Configurations.Entities.Database;
using ADE.Configurations.Entities.Summaries;
using ADE.Configurations.Objects;
using ADE.DataAccess.Helper;
using ADE.Entities.Database;
using ADE.Extras.Common.Method;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;

namespace ADE.Configurations.DataAccess
{
    public class DatabaseConnection
    {
        public MainSettings MS = null; 
        public byte[] VAR_FIR = null;
        public bool ExchangueRateExists = false;
        public bool DocumentDolarExists = false;

        public DatabaseConnection(MainSettings MSI)
        {
            MS = MSI;
        }

        public List<RegexDB> SP_ObtieneValidaciones()
        {
            List<RegexDB> Validaciones = new List<RegexDB>();
            try
            {
                using (SqlConnection con = new SqlConnection(MS.BDCon))
                {
                    using (SqlCommand cmd = new SqlCommand("[Fact].[Usp_ObtieneRegex]", con))
                    {
                        SqlDataReader dr;
                        cmd.CommandType = CommandType.StoredProcedure;
                        con.Open();
                        dr = cmd.ExecuteReader();
                        while (dr.Read())
                        {
                            RegexDB Val = new RegexDB();
                            Val.KEY = dr[0].ToString().Trim();
                            Val.NOM = dr[1].ToString().Trim();
                            Val.VAL = dr[2].ToString().Trim();
                            Val.MND = dr[3].ToString().Trim();
                            Val.DOC = dr[4].ToString().Trim();
                            Val.TAB = dr[5].ToString().Trim();
                            Val.MSG = dr[6].ToString().Trim();
                            Validaciones.Add(Val);
                        }
                        con.Close();
                    }
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine(" " + ex.Message);
            }
            return Validaciones;
        }

        public string SP_ObtieneTipoDocumento(string Codigo)
        {
            string td = "";
            try
            {
                using (SqlConnection con = new SqlConnection(MS.BDCon))
                {
                    using (SqlCommand cmd = new SqlCommand("[Fact].[Usp_ObtieneTipoDocumento]", con))
                    {
                        SqlDataReader dr;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Codigo", Codigo);
                        con.Open();
                        dr = cmd.ExecuteReader();
                        if (dr.Read())
                        {
                            td = dr[0].ToString().Trim();
                        }
                        con.Close();
                    }
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine(" " + ex.Message);
            }
            return td;
        }

        public bool SP_MailPDF(string table, string state)
        {
            bool existe = false;
            try
            {
                using (SqlConnection con = new SqlConnection(MS.BDCon))
                {
                    using (SqlCommand cmd = new SqlCommand("[Fact].[MailPDF]", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Table", table);
                        cmd.Parameters.AddWithValue("@DocumentType", MS.DocumentType);
                        cmd.Parameters.AddWithValue("@State", state);
                        cmd.Parameters.AddWithValue("@Ruc", MS.RucNumber);
                        con.Open();
                        SqlDataReader dr;
                        dr = cmd.ExecuteReader();
                        if (dr.Read())
                        {
                            con.Close();
                            return true;
                        }
                        else
                        {
                            con.Close();
                            return false;
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            return existe;
        }

        public bool SP_MailPDFAux(string table, string state)
        {
            bool existe = false;
            try
            {
                using (SqlConnection con = new SqlConnection(MS.BDCon))
                {
                    using (SqlCommand cmd = new SqlCommand("[Fact].[MailPDF]", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Table", table);
                        cmd.Parameters.AddWithValue("@DocumentType", MS.DocumentTypeAux);
                        cmd.Parameters.AddWithValue("@State", state);
                        cmd.Parameters.AddWithValue("@Ruc", MS.RucNumber);
                        con.Open();
                        SqlDataReader dr;
                        dr = cmd.ExecuteReader();
                        if (dr.Read())
                        {
                            con.Close();
                            return true;
                        }
                        else
                        {
                            con.Close();
                            return false;
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            return existe;
        }

        public bool SP_ValidarDocumentoExiste(string NUM_CE)
        {
            bool existe = false;
            try
            {
                using (SqlConnection con = new SqlConnection(MS.BDCon))
                {
                    using (SqlCommand cmd = new SqlCommand("[Fact].[Usp_ValidarExistsDocumento]", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@NUM_CPE", NUM_CE);
                        con.Open();
                        SqlDataReader dr;
                        dr = cmd.ExecuteReader();
                        if (dr.Read())
                        {
                            con.Close();
                            return true;
                        }
                        else
                        {
                            con.Close();
                            return false;
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            return existe;
        }

        /// <summary>
        /// Obtiene el XML en caso esté en el estado correcto
        /// Devuelve 0: El Documento no Existe
        /// Devuelve 1: El Documento ya ha sido enviado
        /// Devuelve 2: El Documento puede ser enviado a Sunat
        /// Devuleve -1 : Error en la consulta.
        /// </summary>
        /// <param name="NUM_CE">Numero del Documento a Consultar</param>
        /// <returns></returns>
        public int SP_ObtenerDocumentoXML(string NUM_CE)
        {
            int State = -1;
            try
            {
                using (SqlConnection con = new SqlConnection(MS.BDCon))
                {
                    using (SqlCommand cmd = new SqlCommand("[Fact].[O.Usp_ObtenerDocumentoXML]", con))
                    {
                        SqlDataReader dr;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@DocumentName", NUM_CE);
                        var est = ParameterDirection.Output;
                        //cmd.Parameters.Add("@Estado", SqlDbType.Int).Direction = ParameterDirection.Output;
                        cmd.Parameters.Add("@Estado", SqlDbType.Int).Value = est;
                        //cmd.Parameters.Add("@XML", SqlDbType.VarBinary, -1).Direction = ParameterDirection.Output;
                        con.Open();
                        dr = cmd.ExecuteReader();

                        if (dr.Read())
                        {
                            MS.id = int.Parse(dr[0].ToString());
                            VAR_FIR = (byte[])dr[1];
                            MS.im = dr[2].ToString();
                            dr.Close();

                            if (!File.Exists(MS.XMLSFileLocation))
                            {
                                FileStream fs = new FileStream(MS.XMLSFileLocation, FileMode.Create);
                                fs.Write(VAR_FIR, 0, VAR_FIR.Length);
                                fs.Close();
                            }
                        }
                        con.Close();
                        State = Convert.ToInt32(cmd.Parameters["@Estado"].Value);
                    }
                }
            }
            catch (SqlException ex)
            {
                MS.GeneraLog.EnvioSunat01("Error al obtener el XML Firmado");
                MS.GeneraLog.EnvioSunat01(ex.Message);
            }
            return State;
        }

        public List<ResumenB> SP_ObtieneResumenBoletas()
        {
            List<ResumenB> Summary = new List<ResumenB>();
            try
            {
                using (SqlConnection con = new SqlConnection(MS.BDCon))
                {
                    using (SqlCommand cmd = new SqlCommand("[Fact].[Usp_ObtieneResumenB_Old]", con))
                    {
                        SqlDataReader dr;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Fecha", MS.SummaryDate);
                        cmd.Parameters.AddWithValue("@Ruc", MS.RucNumber);
                        con.Open();
                        dr = cmd.ExecuteReader();
                        int i = 0;
                        while (dr.Read())
                        {
                            ResumenB RS = new ResumenB();

                            // TIPO DE CAMBIO
                            RS.MONEDA = dr[13].ToString().Trim();
                            RS.CAMBIO = Convert.ToDecimal(dr[14], CultureInfo.CreateSpecificCulture("es-PE"));

                            RS.TPODOC = dr[0].ToString().Trim();
                            RS.SERIE = dr[1].ToString().Trim();
                            RS.NUMERO = int.Parse(dr[2].ToString().Trim());
                            RS.DESDE = DateTime.Parse(dr[3].ToString().Trim());
                            RS.HASTA = DateTime.Parse(dr[4].ToString().Trim());

                            if (RS.CAMBIO > 0) ExchangueRateExists = true;

                            if (RS.MONEDA != "PEN")
                            {
                                DocumentDolarExists = true;
                                decimal t = 0M;
                                // IMPORTES
                                t = Convert.ToDecimal(dr[5], CultureInfo.CreateSpecificCulture("es-PE")) * RS.CAMBIO;
                                RS.GRAVADO = Math.Round(t, 2);

                                t = Convert.ToDecimal(dr[6], CultureInfo.CreateSpecificCulture("es-PE")) * RS.CAMBIO;
                                RS.EXONERADO = Math.Round(t, 2);

                                t = Convert.ToDecimal(dr[7], CultureInfo.CreateSpecificCulture("es-PE")) * RS.CAMBIO;
                                RS.INAFECTO = Math.Round(t, 2);

                                t = Convert.ToDecimal(dr[8], CultureInfo.CreateSpecificCulture("es-PE")) * RS.CAMBIO;
                                RS.OCARGOS = Math.Round(t, 2);


                                //IMPUESTOS
                                t = Convert.ToDecimal(dr[9], CultureInfo.CreateSpecificCulture("es-PE")) * RS.CAMBIO;
                                RS.IGV = Math.Round(t, 2);

                                t = Convert.ToDecimal(dr[10], CultureInfo.CreateSpecificCulture("es-PE")) * RS.CAMBIO;
                                RS.ISC = Math.Round(t, 2);

                                t = Convert.ToDecimal(dr[11], CultureInfo.CreateSpecificCulture("es-PE")) * RS.CAMBIO;
                                RS.OTH = Math.Round(t, 2);



                                // TOTAL
                                t = Convert.ToDecimal(dr[12], CultureInfo.CreateSpecificCulture("es-PE")) * RS.CAMBIO;
                                RS.TOTAL = Math.Round(t, 2, MidpointRounding.AwayFromZero);
                            }
                            else
                            {
                                // IMPORTES
                                RS.GRAVADO = Convert.ToDecimal(dr[5], CultureInfo.CreateSpecificCulture("es-PE"));
                                RS.EXONERADO = Convert.ToDecimal(dr[6], CultureInfo.CreateSpecificCulture("es-PE"));
                                RS.INAFECTO = Convert.ToDecimal(dr[7], CultureInfo.CreateSpecificCulture("es-PE"));
                                RS.OCARGOS = Convert.ToDecimal(dr[8], CultureInfo.CreateSpecificCulture("es-PE"));

                                //IMPUESTOS
                                RS.IGV = Convert.ToDecimal(dr[9], CultureInfo.CreateSpecificCulture("es-PE"));
                                RS.ISC = Convert.ToDecimal(dr[10], CultureInfo.CreateSpecificCulture("es-PE"));
                                RS.OTH = Convert.ToDecimal(dr[11], CultureInfo.CreateSpecificCulture("es-PE"));

                                // TOTAL
                                RS.TOTAL = Convert.ToDecimal(dr[12], CultureInfo.CreateSpecificCulture("es-PE"));
                            }

                            Summary.Add(RS);
                            i++;
                        }
                        con.Close();
                    }
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine(" " + ex.Message);
            }
            return Summary;
        }




        // METHOD FOR NEW RC
        public List<ResumenB> SP_ObtieneResumenBoletas_2_1(string TypeRC)
        {
            List<ResumenB> Summary = new List<ResumenB>();
            try
            {
                using (SqlConnection con = new SqlConnection(MS.BDCon))
                {
                    using (SqlCommand cmd = new SqlCommand("[Fact].[Usp_ObtieneResumenB]", con))
                    {
                        SqlDataReader dr;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Fecha", MS.SummaryDate);
                        cmd.Parameters.AddWithValue("@Ruc", MS.RucNumber);
                        cmd.Parameters.AddWithValue("@TypeRc", TypeRC);

                        con.Open();
                        dr = cmd.ExecuteReader();
                        int i = 0;
                        while (dr.Read())
                        {
                            ResumenB RS = new ResumenB();

                            // TIPO DE CAMBIO
                            RS.MONEDA = dr[15].ToString().Trim();
                            RS.CAMBIO = Convert.ToDecimal(dr[16], CultureInfo.CreateSpecificCulture("es-PE"));

                            RS.TPODOC = dr[1].ToString().Trim();
                            RS.SERIE = dr[2].ToString().Trim();
                            RS.NUMERO = int.Parse(dr[3].ToString().Trim());
                            RS.DESDE = DateTime.Parse(dr[4].ToString().Trim());
                            RS.HASTA = DateTime.Parse(dr[5].ToString().Trim());

                            if (RS.CAMBIO > 0) ExchangueRateExists = true;

                            #region now

                            decimal t = 0M;
                            // IMPORTES
                            t = Convert.ToDecimal(dr[6], CultureInfo.CreateSpecificCulture("es-PE"));
                            RS.GRAVADO = Math.Round(t, 2);

                            t = Convert.ToDecimal(dr[7], CultureInfo.CreateSpecificCulture("es-PE"));
                            RS.EXONERADO = Math.Round(t, 2);

                            t = Convert.ToDecimal(dr[8], CultureInfo.CreateSpecificCulture("es-PE"));
                            RS.INAFECTO = Math.Round(t, 2);

                            t = Convert.ToDecimal(dr[9], CultureInfo.CreateSpecificCulture("es-PE"));
                            RS.OCARGOS = Math.Round(t, 2);

                            t = Convert.ToDecimal(dr[10], CultureInfo.CreateSpecificCulture("es-PE"));
                            RS.GRATUITO = Math.Round(t, 2);

                            //IMPUESTOS
                            t = Convert.ToDecimal(dr[11], CultureInfo.CreateSpecificCulture("es-PE"));
                            RS.IGV = Math.Round(t, 2);

                            t = Convert.ToDecimal(dr[12], CultureInfo.CreateSpecificCulture("es-PE"));
                            RS.ISC = Math.Round(t, 2);
                            
                            t = Convert.ToDecimal(dr[13], CultureInfo.CreateSpecificCulture("es-PE"));
                            RS.OTH = Math.Round(t, 2);

                            // TOTAL
                            t = Convert.ToDecimal(dr[14], CultureInfo.CreateSpecificCulture("es-PE"));
                            RS.TOTAL = Math.Round(t, 2, MidpointRounding.AwayFromZero);
                            
                            #endregion

                            #region after
                            //if (RS.MONEDA != "PEN")
                            //{
                            //    DocumentDolarExists = true;
                            //    decimal t = 0M;
                            //    // IMPORTES
                            //    //t = Convert.ToDecimal(dr[6], CultureInfo.CreateSpecificCulture("es-PE")) * RS.CAMBIO;
                            //    t = Convert.ToDecimal(dr[6], CultureInfo.CreateSpecificCulture("es-PE"));
                            //    RS.GRAVADO = Math.Round(t, 2);

                            //    //t = Convert.ToDecimal(dr[7], CultureInfo.CreateSpecificCulture("es-PE")) * RS.CAMBIO;
                            //    t = Convert.ToDecimal(dr[7], CultureInfo.CreateSpecificCulture("es-PE"));
                            //    RS.EXONERADO = Math.Round(t, 2);

                            //    //t = Convert.ToDecimal(dr[8], CultureInfo.CreateSpecificCulture("es-PE")) * RS.CAMBIO;
                            //    t = Convert.ToDecimal(dr[8], CultureInfo.CreateSpecificCulture("es-PE"));
                            //    RS.INAFECTO = Math.Round(t, 2);

                            //    //t = Convert.ToDecimal(dr[9], CultureInfo.CreateSpecificCulture("es-PE")) * RS.CAMBIO;
                            //    t = Convert.ToDecimal(dr[9], CultureInfo.CreateSpecificCulture("es-PE"));
                            //    RS.OCARGOS = Math.Round(t, 2);

                            //    //t = Convert.ToDecimal(dr[10], CultureInfo.CreateSpecificCulture("es-PE")) * RS.CAMBIO;
                            //    t = Convert.ToDecimal(dr[10], CultureInfo.CreateSpecificCulture("es-PE"));
                            //    RS.GRATUITO = Math.Round(t, 2);


                            //    //IMPUESTOS
                            //    //t = Convert.ToDecimal(dr[11], CultureInfo.CreateSpecificCulture("es-PE")) * RS.CAMBIO;
                            //    t = Convert.ToDecimal(dr[11], CultureInfo.CreateSpecificCulture("es-PE"));
                            //    RS.IGV = Math.Round(t, 2);

                            //    //t = Convert.ToDecimal(dr[12], CultureInfo.CreateSpecificCulture("es-PE")) * RS.CAMBIO;
                            //    t = Convert.ToDecimal(dr[12], CultureInfo.CreateSpecificCulture("es-PE"));
                            //    RS.ISC = Math.Round(t, 2);

                            //    //t = Convert.ToDecimal(dr[13], CultureInfo.CreateSpecificCulture("es-PE")) * RS.CAMBIO;
                            //    t = Convert.ToDecimal(dr[13], CultureInfo.CreateSpecificCulture("es-PE"));
                            //    RS.OTH = Math.Round(t, 2);

                            //    // TOTAL
                            //    //t = Convert.ToDecimal(dr[14], CultureInfo.CreateSpecificCulture("es-PE")) * RS.CAMBIO;
                            //    t = Convert.ToDecimal(dr[14], CultureInfo.CreateSpecificCulture("es-PE"));
                            //    RS.TOTAL = Math.Round(t, 2, MidpointRounding.AwayFromZero);
                            //}
                            //else
                            //{
                            //    // IMPORTES
                            //    RS.GRAVADO = Convert.ToDecimal(dr[6], CultureInfo.CreateSpecificCulture("es-PE"));
                            //    RS.EXONERADO = Convert.ToDecimal(dr[7], CultureInfo.CreateSpecificCulture("es-PE"));
                            //    RS.INAFECTO = Convert.ToDecimal(dr[8], CultureInfo.CreateSpecificCulture("es-PE"));
                            //    RS.OCARGOS = Convert.ToDecimal(dr[9], CultureInfo.CreateSpecificCulture("es-PE"));
                            //    RS.GRATUITO = Convert.ToDecimal(dr[10], CultureInfo.CreateSpecificCulture("es-PE"));

                            //    //IMPUESTOS
                            //    RS.IGV = Convert.ToDecimal(dr[11], CultureInfo.CreateSpecificCulture("es-PE"));
                            //    RS.ISC = Convert.ToDecimal(dr[12], CultureInfo.CreateSpecificCulture("es-PE"));
                            //    RS.OTH = Convert.ToDecimal(dr[13], CultureInfo.CreateSpecificCulture("es-PE"));

                            //    // TOTAL
                            //    RS.TOTAL = Convert.ToDecimal(dr[14], CultureInfo.CreateSpecificCulture("es-PE"));
                            //}
                            #endregion

                            var nro = dr[18].ToString();

                            RS.RE_TPODOC = dr[17].ToString();

                            if (nro != null && nro.Length > 0)
                            {
                                RS.RE_NUMDOC = dr[18].ToString().Trim();
                            }
                            else { RS.RE_NUMDOC = dr[18].ToString(); }

                            var st = dr[19].ToString();
                            //RS.STATUS_RC_DOC = int.Parse(dr[19].ToString());
                            RS.STATUS_RC_DOC = st == null ? 0 : int.Parse(dr[19].ToString());


                            RS.TPO_DOC_AFEC = dr[20].ToString();
                            RS.NRO_DOC_AFEC = dr[21].ToString();
                            RS.SYSTEM_STATUS = dr[22].ToString();
                            RS.NUM_CPE = dr[23].ToString();

                            RS.SUMMARY = int.Parse(dr[24].ToString());
                            RS.VOIDED = int.Parse(dr[25].ToString());
                            Summary.Add(RS);
                            i++;
                        }
                        con.Close();
                    }
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine(" " + ex.Message);
            }
            return Summary;
        }


        public List<RBajasDetalle> ObtieneResumenAnulados()
        {
            List<RBajasDetalle> RBD = new List<RBajasDetalle>();
            try
            {
                using (SqlConnection con = new SqlConnection(MS.BDCon))
                {
                    using (SqlCommand cmd = new SqlCommand("[Fact].[Usp_ObtieneResumenA]", con))
                    {
                        SqlDataReader dr;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Fecha", MS.SummaryDate);
                        cmd.Parameters.AddWithValue("@Type", MS.DocumentType);
                        cmd.Parameters.AddWithValue("@Ruc", MS.RucNumber);
                        con.Open();
                        dr = cmd.ExecuteReader();

                        while (dr.Read())
                        {
                            RBajasDetalle RBL = new RBajasDetalle();
                            RBL.NUM_CPE = MS.DocumentName;
                            RBL.TPO_CPE = dr[0].ToString();
                            RBL.DOC_SER = dr[1].ToString();
                            RBL.DOC_NUM = int.Parse(dr[2].ToString());
                            RBL.DOC_DES = dr[3].ToString();
                            RBD.Add(RBL);
                        }
                        con.Close();
                    }
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine(" " + ex.Message);
            }
            return RBD;
        }

        public int SP_ObtieneSiguienteResumen(string date, string type, string ruc)
        {
            int next = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(MS.BDCon))
                {
                    using (SqlCommand cmd = new SqlCommand("[Fact].[Usp_ObtieneCorrelativo]", con))
                    {
                        SqlDataReader dr;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Fecha", date);
                        cmd.Parameters.AddWithValue("@Tipo", type);
                        cmd.Parameters.AddWithValue("@Ruc", ruc);
                        con.Open();
                        dr = cmd.ExecuteReader();
                        if (dr.Read())
                        {
                            next = int.Parse(dr[0].ToString());
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine(" " + ex.Message);
            }
            return next;
        }

        public List<RegexDB> getRegex()
        {
            List<RegexDB> Validaciones = new List<RegexDB>();
            try
            {
                using (SqlConnection con = new SqlConnection(MS.BDCon))
                {
                    using (SqlCommand cmd = new SqlCommand("[Fact].[Usp_ObtieneRegex]", con))
                    {
                        SqlDataReader dr;
                        cmd.CommandType = CommandType.StoredProcedure;
                        con.Open();
                        dr = cmd.ExecuteReader();
                        while (dr.Read())
                        {
                            RegexDB Val = new RegexDB();
                            Val.KEY = dr[0].ToString().Trim();
                            Val.NOM = dr[1].ToString().Trim();
                            Val.VAL = dr[2].ToString().Trim();
                            Val.MND = dr[3].ToString().Trim();
                            Val.DOC = dr[4].ToString().Trim();
                            Val.TAB = dr[5].ToString().Trim();
                            Val.MSG = dr[6].ToString().Trim();
                            Validaciones.Add(Val);
                        }
                        con.Close();
                    }
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine(" " + ex.Message);
            }
            return Validaciones;
        }

        public int SP_InsertaCabeceraDocumento(Header Cabecera)
        {
            int ret = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(MS.BDCon))
                {
                    using (SqlCommand cmd = new SqlCommand("[Fact].[O.Usp_InsertaCabecera]", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@NUM_CPE", Cabecera.NUM_CPE);
                        cmd.Parameters.AddWithValue("@ID_TPO_CPE", Cabecera.ID_TPO_CPE);
                        cmd.Parameters.AddWithValue("@ID_CPE", Cabecera.ID_CPE);
                        cmd.Parameters.AddWithValue("@ID_TPO_OPERACION", Cabecera.ID_TPO_OPERACION);
                        cmd.Parameters.AddWithValue("@FEC_EMIS", Cabecera.FEC_EMIS);
                        cmd.Parameters.AddWithValue("@TPO_MONEDA", Cabecera.TPO_MONEDA);
                        cmd.Parameters.AddWithValue("@TPO_NOTA", Cabecera.TPO_NOTA);
                        cmd.Parameters.AddWithValue("@MOTIVO_NOTA", Cabecera.MOTIVO_NOTA);
                        cmd.Parameters.AddWithValue("@EM_TPO_DOC", Cabecera.EM_TPO_DOC);
                        cmd.Parameters.AddWithValue("@EM_NUM_DOCU", Cabecera.EM_NUM_DOCU);
                        cmd.Parameters.AddWithValue("@EM_NOMBRE", Cabecera.EM_NOMBRE);
                        cmd.Parameters.AddWithValue("@EM_NCOMER", Cabecera.EM_NCOMER);
                        cmd.Parameters.AddWithValue("@EM_UBIGEO", Cabecera.EM_UBIGEO);
                        cmd.Parameters.AddWithValue("@EM_DFISCAL", Cabecera.EM_DFISCAL);
                        cmd.Parameters.AddWithValue("@EM_DURBANIZ", Cabecera.EM_DURBANIZ);
                        cmd.Parameters.AddWithValue("@EM_DIR_PROV", Cabecera.EM_DIR_PROV);
                        cmd.Parameters.AddWithValue("@EM_DIR_DPTO", Cabecera.EM_DIR_DPTO);
                        cmd.Parameters.AddWithValue("@EM_DIR_DIST", Cabecera.EM_DIR_DIST);
                        cmd.Parameters.AddWithValue("@EM_COD_PAIS", Cabecera.EM_COD_PAIS);
                        cmd.Parameters.AddWithValue("@RE_TPODOC", Cabecera.RE_TPODOC);
                        cmd.Parameters.AddWithValue("@RE_NUMDOC", Cabecera.RE_NUMDOC);
                        cmd.Parameters.AddWithValue("@RE_NOMBRE", Cabecera.RE_NOMBRE);
                        cmd.Parameters.AddWithValue("@RE_DIRECCION", Cabecera.RE_DIRECCION);
                        cmd.Parameters.AddWithValue("@TOT_GRAV_MTO", Cabecera.TOT_GRAV_MTO);
                        cmd.Parameters.AddWithValue("@TOT_INAF_MTO", Cabecera.TOT_INAF_MTO);
                        cmd.Parameters.AddWithValue("@TOT_EXON_MTO", Cabecera.TOT_EXON_MTO);
                        cmd.Parameters.AddWithValue("@TOT_GRAT_MTO", Cabecera.TOT_GRAT_MTO);
                        cmd.Parameters.AddWithValue("@TOT_DSCTO_MTO", Cabecera.TOT_DSCTO_MTO);
                        cmd.Parameters.AddWithValue("@TOT_SUMA_IGV", Cabecera.TOT_SUMA_IGV);
                        cmd.Parameters.AddWithValue("@TOT_SUMA_ISC", Cabecera.TOT_SUMA_ISC);
                        cmd.Parameters.AddWithValue("@TOT_SUMA_OTRIB", Cabecera.TOT_SUMA_OTRIB);
                        cmd.Parameters.AddWithValue("@TOT_DCTO_GLOB", Cabecera.TOT_DCTO_GLOB);
                        cmd.Parameters.AddWithValue("@TOT_SUM_OCARG", Cabecera.TOT_SUM_OCARG);
                        cmd.Parameters.AddWithValue("@ANT_TOT_ANTICIPO", Cabecera.ANT_TOT_ANTICIPO);
                        cmd.Parameters.AddWithValue("@TOT_IMPOR_TOTAL", Cabecera.TOT_IMPOR_TOTAL);
                        cmd.Parameters.AddWithValue("@MONTOLITERAL", Cabecera.MONTOLITERAL);
                        cmd.Parameters.AddWithValue("@PER_BASE_IMPO", Cabecera.PER_BASE_IMPO);
                        cmd.Parameters.AddWithValue("@PER_MNTO_PER", Cabecera.PER_MNTO_PER);
                        cmd.Parameters.AddWithValue("@PER_MNTO_TOT", Cabecera.PER_MNTO_TOT);
                        cmd.Parameters.AddWithValue("@SERIE", Cabecera.SERIE);
                        cmd.Parameters.AddWithValue("@NUM_DOCUMENTO", Cabecera.NUM_DOCUMENTO);
                        cmd.Parameters.AddWithValue("@IdEstadoDocumento", Cabecera.IdEstadoDocumento);

                        SqlParameter XML = new SqlParameter("@VAR_FIR", SqlDbType.VarBinary);
                        XML.Value = Cabecera.VAR_FIR;
                        cmd.Parameters.Add(XML);

                        cmd.Parameters.AddWithValue("@RE_NCOMER", Cabecera.RE_NCOMER);
                        cmd.Parameters.AddWithValue("@RE_DURBANIZ", Cabecera.RE_DURBANIZ);
                        cmd.Parameters.AddWithValue("@RE_DIR_PROV", Cabecera.RE_DIR_PROV);
                        cmd.Parameters.AddWithValue("@RE_DIR_DPTO", Cabecera.RE_DIR_DPTO);
                        cmd.Parameters.AddWithValue("@RE_DIR_DIST", Cabecera.RE_DIR_DIST);
                        cmd.Parameters.AddWithValue("@RE_COD_PAIS", Cabecera.RE_COD_PAIS);
                        cmd.Parameters.AddWithValue("@RE_UBIGEO", Cabecera.RE_UBIGEO);

                        cmd.Parameters.AddWithValue("@REGIMENCE", Cabecera.REGIMENCE);
                        cmd.Parameters.AddWithValue("@TASACE", Cabecera.TASACE);
                        cmd.Parameters.AddWithValue("@OBSCE", Cabecera.OBSCE);
                        cmd.Parameters.AddWithValue("@IMPTOTCE", Cabecera.IMPTOTCE);
                        cmd.Parameters.AddWithValue("@MONIMPTOTCE", Cabecera.MONIMPTOTCE);
                        cmd.Parameters.AddWithValue("@IMPTOT", Cabecera.IMPTOT);
                        cmd.Parameters.AddWithValue("@MONIMPTOT", Cabecera.MONIMPTOT);

                        cmd.Parameters.AddWithValue("@SEDE", Cabecera.SEDE);
                        cmd.Parameters.AddWithValue("@USUARIO", Cabecera.USUARIO);
                        cmd.Parameters.AddWithValue("@IMPRESORA", Cabecera.IMPRESORA);
                        cmd.Parameters.AddWithValue("@CAMPO1", Cabecera.CAMPO1);
                        cmd.Parameters.AddWithValue("@CAMPO2", Cabecera.CAMPO2);
                        cmd.Parameters.AddWithValue("@CAMPO3", Cabecera.CAMPO3);
                        cmd.Parameters.AddWithValue("@CAMPO4", Cabecera.CAMPO4);
                        cmd.Parameters.AddWithValue("@CAMPO5", Cabecera.CAMPO5);
                        cmd.Parameters.AddWithValue("@CAMPO6", Cabecera.CAMPO6);
                        cmd.Parameters.AddWithValue("@CAMPO7", Cabecera.CAMPO7);
                        cmd.Parameters.AddWithValue("@CAMPO8", Cabecera.CAMPO8);
                        cmd.Parameters.AddWithValue("@CAMPO9", Cabecera.CAMPO9);
                        cmd.Parameters.AddWithValue("@CAMPO10", Cabecera.CAMPO10);

                        con.Open();
                        //cmd.ExecuteNonQuery();
                        SqlDataReader dr;
                        dr = cmd.ExecuteReader();
                        if (dr.Read())
                        {
                            int Id = int.Parse(dr[0].ToString().Trim());
                            if (Id > 0)
                            {
                                con.Close();
                                MS.GeneraLog.IngresoBD01("Cabecera de Documento ingresada Correctamente");
                                return Id;
                            }
                            else { con.Close(); }
                        }
                        else { con.Close(); }
                    }
                }
            }
            catch (SqlException ex)
            {
                MS.GeneraLog.IngresoBD01("Error al ingresar la cabecera en la base de Datos");
                MS.GeneraLog.IngresoBD01(ex.Message);
            }
            return ret;
        }

        public string addDetail(List<Detail> Detalle, int Id_DC)
        {
            string ret = "0";
            try
            {
                using (SqlConnection con = new SqlConnection(MS.BDCon))
                {
                    foreach (Detail line in Detalle)
                    {
                        using (SqlCommand cmd = new SqlCommand("[Fact].[O.Usp_InsertaDetalle]", con))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@IdDocumentoCabecera", Id_DC);
                            cmd.Parameters.AddWithValue("@NUM_CPE", line.NUM_CPE);
                            cmd.Parameters.AddWithValue("@IT_NRO_ORD", line.IT_NRO_ORD);
                            cmd.Parameters.AddWithValue("@IT_UND_MED", line.IT_UND_MED);
                            cmd.Parameters.AddWithValue("@IT_CANT_ITEM", line.IT_CANT_ITEM);
                            cmd.Parameters.AddWithValue("@IT_COD_PROD", line.IT_COD_PROD);
                            cmd.Parameters.AddWithValue("@IT_DESCRIP", line.IT_DESCRIP);
                            cmd.Parameters.AddWithValue("@IT_VAL_UNIT", line.IT_VAL_UNIT);
                            cmd.Parameters.AddWithValue("@IT_MNT_PVTA", line.IT_MNT_PVTA);
                            cmd.Parameters.AddWithValue("@IT_VAL_VTA", line.IT_VAL_VTA);
                            cmd.Parameters.AddWithValue("@IT_MTO_IGV", line.IT_MTO_IGV);
                            cmd.Parameters.AddWithValue("@IT_COD_AFE_IGV", line.IT_COD_AFE_IGV);
                            cmd.Parameters.AddWithValue("@IT_MTO_ISC", line.IT_MTO_ISC);
                            cmd.Parameters.AddWithValue("@IT_SIS_AFE_ISC", line.IT_SIS_AFE_ISC);
                            cmd.Parameters.AddWithValue("@IT_DESC_MNTO", line.IT_DESC_MNTO);
                            cmd.Parameters.AddWithValue("@SERIE", line.SERIE);
                            cmd.Parameters.AddWithValue("@NUM_DOCUMENTO", line.NUM_DOCUMENTO);

                            cmd.Parameters.AddWithValue("@TPODOCRELAC", line.TPODOCRELAC);
                            cmd.Parameters.AddWithValue("@NUMDOCRELAC", line.NUMDOCRELAC);
                            cmd.Parameters.AddWithValue("@FEMISDOCRELAC", line.FEMISDOCRELAC);
                            cmd.Parameters.AddWithValue("@ITOTDOCRELAC", line.ITOTDOCRELAC);
                            cmd.Parameters.AddWithValue("@MDOCRELAC", line.MDOCRELAC);
                            cmd.Parameters.AddWithValue("@FECMOVI", line.FECMOVI);
                            cmd.Parameters.AddWithValue("@NUMMOVI", line.NUMMOVI);
                            cmd.Parameters.AddWithValue("@IMPSOPERMOV", line.IMPSOPERMOV);
                            cmd.Parameters.AddWithValue("@MONMOVI", line.MONMOVI);
                            cmd.Parameters.AddWithValue("@IMPOPER", line.IMPOPER);
                            cmd.Parameters.AddWithValue("@MONIMPOPER", line.MONIMPOPER);
                            cmd.Parameters.AddWithValue("@FECOPER", line.FECOPER);
                            cmd.Parameters.AddWithValue("@IMPTOTOPER", line.IMPTOTOPER);
                            cmd.Parameters.AddWithValue("@MONOPER", line.MONOPER);

                            cmd.Parameters.AddWithValue("@MONREFETC", line.MONREFETC);
                            cmd.Parameters.AddWithValue("@MONDESTTC", line.MONDESTTC);
                            cmd.Parameters.AddWithValue("@FACTORTC", line.FACTORTC);
                            cmd.Parameters.AddWithValue("@FECHATC", line.FECHATC);

                            con.Open();
                            cmd.ExecuteNonQuery();
                            con.Close();
                        }
                    }
                    MS.GeneraLog.IngresoBD01("Detalle del Documento ingresada Correctamente");
                    ret = "1";
                }
            }
            catch (SqlException ex)
            {
                ret = ex.Message;
                MS.GeneraLog.IngresoBD01("Error al ingresar el detalle en la base de Datos");
                MS.GeneraLog.IngresoBD01(ex.Message);
            }
            return ret;
        }

        public string SP_InsertaDetalleDocumento(DBDocument DBDocumento, int Id_DC)
        {
            string ret = "0";
            int i = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(MS.BDCon))
                {
                    foreach (Detail line in DBDocumento.Detalles)
                    {
                        using (SqlCommand cmd = new SqlCommand("[Fact].[O.Usp_InsertaDetalle]", con))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@IdDocumentoCabecera", Id_DC);
                            cmd.Parameters.AddWithValue("@NUM_CPE", line.NUM_CPE);
                            cmd.Parameters.AddWithValue("@IT_NRO_ORD", line.IT_NRO_ORD);
                            cmd.Parameters.AddWithValue("@IT_UND_MED", line.IT_UND_MED);
                            cmd.Parameters.AddWithValue("@IT_CANT_ITEM", line.IT_CANT_ITEM);
                            cmd.Parameters.AddWithValue("@IT_COD_PROD", line.IT_COD_PROD);
                            cmd.Parameters.AddWithValue("@IT_DESCRIP", line.IT_DESCRIP);
                            cmd.Parameters.AddWithValue("@IT_VAL_UNIT", line.IT_VAL_UNIT);
                            cmd.Parameters.AddWithValue("@IT_MNT_PVTA", line.IT_MNT_PVTA);
                            cmd.Parameters.AddWithValue("@IT_VAL_VTA", line.IT_VAL_VTA);
                            cmd.Parameters.AddWithValue("@IT_MTO_IGV", line.IT_MTO_IGV);
                            cmd.Parameters.AddWithValue("@IT_COD_AFE_IGV", line.IT_COD_AFE_IGV);
                            cmd.Parameters.AddWithValue("@IT_MTO_ISC", line.IT_MTO_ISC);
                            cmd.Parameters.AddWithValue("@IT_SIS_AFE_ISC", line.IT_SIS_AFE_ISC);
                            cmd.Parameters.AddWithValue("@IT_DESC_MNTO", line.IT_DESC_MNTO);
                            cmd.Parameters.AddWithValue("@SERIE", line.SERIE);
                            cmd.Parameters.AddWithValue("@NUM_DOCUMENTO", line.NUM_DOCUMENTO);

                            cmd.Parameters.AddWithValue("@TPODOCRELAC", line.TPODOCRELAC);
                            cmd.Parameters.AddWithValue("@NUMDOCRELAC", line.NUMDOCRELAC);
                            cmd.Parameters.AddWithValue("@FEMISDOCRELAC", line.FEMISDOCRELAC);
                            cmd.Parameters.AddWithValue("@ITOTDOCRELAC", line.ITOTDOCRELAC);
                            cmd.Parameters.AddWithValue("@MDOCRELAC", line.MDOCRELAC);
                            cmd.Parameters.AddWithValue("@FECMOVI", line.FECMOVI);
                            cmd.Parameters.AddWithValue("@NUMMOVI", line.NUMMOVI);
                            cmd.Parameters.AddWithValue("@IMPSOPERMOV", line.IMPSOPERMOV);
                            cmd.Parameters.AddWithValue("@MONMOVI", line.MONMOVI);
                            cmd.Parameters.AddWithValue("@IMPOPER", line.IMPOPER);
                            cmd.Parameters.AddWithValue("@MONIMPOPER", line.MONIMPOPER);
                            cmd.Parameters.AddWithValue("@FECOPER", line.FECOPER);
                            cmd.Parameters.AddWithValue("@IMPTOTOPER", line.IMPTOTOPER);
                            cmd.Parameters.AddWithValue("@MONOPER", line.MONOPER);

                            cmd.Parameters.AddWithValue("@MONREFETC", line.MONREFETC);
                            cmd.Parameters.AddWithValue("@MONDESTTC", line.MONDESTTC);
                            cmd.Parameters.AddWithValue("@FACTORTC", line.FACTORTC);
                            cmd.Parameters.AddWithValue("@FECHATC", line.FECHATC);

                            con.Open();
                            cmd.ExecuteNonQuery();
                            con.Close();
                            i = 1;
                        }
                    }
                    if (i == 1) MS.GeneraLog.IngresoBD01("Detalle del Documento ingresada Correctamente");
                    i = 0;
                    foreach (Extra lineextra in DBDocumento.Extras)
                    {
                        using (SqlCommand cmd = new SqlCommand("[Fact].[O.Usp_InsertaDocExtr]", con))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@ID_DC", Id_DC);
                            cmd.Parameters.AddWithValue("@EXLINEA", lineextra.EXLINEA);
                            cmd.Parameters.AddWithValue("@EXDATO", lineextra.EXDATO);
                            cmd.Parameters.AddWithValue("@EXTIPO", lineextra.EXTIPO);

                            con.Open();
                            cmd.ExecuteNonQuery();
                            con.Close();
                            i = 1;
                        }
                    }
                    if (i == 1) MS.GeneraLog.IngresoBD01("Extras del Documento ingresada Correctamente");
                    i = 0;
                    if (DBDocumento.Correos.PARA != "" || DBDocumento.Correos.CC != "" || DBDocumento.Correos.CCO != "")
                    {
                        using (SqlCommand cmd = new SqlCommand("[Fact].[O.Usp_InsertaFlexMailCab]", con))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@ID_DC", Id_DC);
                            cmd.Parameters.AddWithValue("@NUM_CPE", DBDocumento.Correos.NUMCPE);
                            cmd.Parameters.AddWithValue("@PARA", DBDocumento.Correos.PARA);
                            cmd.Parameters.AddWithValue("@CC", DBDocumento.Correos.CC);
                            cmd.Parameters.AddWithValue("@CCO", DBDocumento.Correos.CCO);

                            con.Open();
                            cmd.ExecuteNonQuery();
                            con.Close();
                            i = 1;
                        }
                        if (i == 1) MS.GeneraLog.IngresoBD01("Correo del Documento ingresada Correctamente");
                    }

                    i = 0;

                    foreach (Referencia linref in DBDocumento.Referencias)
                    {
                        using (SqlCommand cmd = new SqlCommand("[Fact].[O.Usp_InsertaDocRefer]", con))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@ID_DC", Id_DC);
                            cmd.Parameters.AddWithValue("@REF_NROORDEN", linref.REF_NROORDEN);
                            cmd.Parameters.AddWithValue("@REF_ID", linref.REF_ID);
                            cmd.Parameters.AddWithValue("@REF_TPO_DOC", linref.REF_TPO_DOC);

                            con.Open();
                            cmd.ExecuteNonQuery();
                            con.Close();
                            i = 1;
                        }
                     }
                    if (i == 1) MS.GeneraLog.IngresoBD01("Documentos de Referencia del Documento ingresada Correctamente");
                    i = 0;
                    if (DBDocumento.Detracciones.VALBBSS != "" || DBDocumento.Detracciones.CTABN != "" || DBDocumento.Detracciones.PORCENT != "" || DBDocumento.Detracciones.MONTO != "")
                    {
                        using (SqlCommand cmd = new SqlCommand("[Fact].[O.Usp_InsertaDocDetra]", con))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@ID_DC", Id_DC);
                            cmd.Parameters.AddWithValue("@NUMCPE", DBDocumento.Cabecera.NUM_CPE);
                            cmd.Parameters.AddWithValue("@VAL_BBSS", DBDocumento.Detracciones.VALBBSS);
                            cmd.Parameters.AddWithValue("@CTA_BN", DBDocumento.Detracciones.CTABN);
                            cmd.Parameters.AddWithValue("@PORCENT", DBDocumento.Detracciones.PORCENT);
                            cmd.Parameters.AddWithValue("@MONTO", DBDocumento.Detracciones.MONTO);

                            con.Open();
                            cmd.ExecuteNonQuery();
                            con.Close();
                            i = 1;
                        }
                    }
                    if (i == 1) MS.GeneraLog.IngresoBD01("Detracciones Documento ingresada Correctamente");

                    i = 0;
                    foreach (DocumentoAfectado docafec in DBDocumento.DocumentosAfectados)
                    {
                        using (SqlCommand cmd = new SqlCommand("[Fact].[O.Usp_InsertaDocAfectad]", con))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@ID_DC", Id_DC);
                            cmd.Parameters.AddWithValue("@NRO_ORD", docafec.DocNroOrden);
                            cmd.Parameters.AddWithValue("@DOC_AFEC_ID", docafec.DocID);
                            cmd.Parameters.AddWithValue("@ID_TPO_CPE", docafec.DocTpoDoc);

                            con.Open();
                            cmd.ExecuteNonQuery();
                            con.Close();
                            i = 1;
                        }
                    }
                    if (i == 1) MS.GeneraLog.IngresoBD01("Documentos Afectados ingresados Correctamente");

                    i = 0;
                    foreach (Anticipo anticipo in DBDocumento.Anticipos)
                    {
                        using (SqlCommand cmd = new SqlCommand("[Fact].[O.Usp_InsertaDocAnticip]", con))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@ID_DC", Id_DC);
                            cmd.Parameters.AddWithValue("@ANT_NROORDEN", anticipo.ANT_NROORDEN);
                            cmd.Parameters.AddWithValue("@ANT_MONTO", anticipo.ANT_MONTO);
                            cmd.Parameters.AddWithValue("@ANT_TPO_DOC_ANT", anticipo.ANT_TPO_DOC_ANT);
                            cmd.Parameters.AddWithValue("@ANT_ID_DOC_ANT", anticipo.ANT_ID_DOC_ANT);
                            cmd.Parameters.AddWithValue("@ANT_NUM_DOC_EMI", anticipo.ANT_NUM_DOC_EMI);
                            cmd.Parameters.AddWithValue("@ANT_TPO_DOC_EMI", anticipo.ANT_TPO_DOC_EMI);

                            con.Open();
                            cmd.ExecuteNonQuery();
                            con.Close();
                            i = 1;
                        }
                    }
                    if (i == 1) MS.GeneraLog.IngresoBD01("Anticipos ingresados Correctamente");

                    //actualiza documento cabecera indicando que todos sus extras, detalles y otros se hayan insertado, 1
                    using (SqlCommand cmd = new SqlCommand("[Fact].[Usp_UpdateStatusToSend]", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ID_DC", Id_DC);
                        //cmd.Connection = con;
                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }

                    ret = "1";
                }
            }
            catch (SqlException ex)
            {
                ret = ex.Message;
                MS.GeneraLog.IngresoBD01("Error al ingresar el detalle en la base de Datos");
                MS.GeneraLog.IngresoBD01(ex.Message);
            }
            return ret;
        }

        public int SP_InsertaCabeceraRC(RBoletasCabecera C, ListUtilClass List)
        {
            int ret = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(MS.BDCon))
                {
                    using (SqlCommand cmd = new SqlCommand("[Fact].[O.Usp_InsertaCabeceraRB]", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@NUM_CPE", C.NUM_CPE);
                        cmd.Parameters.AddWithValue("@TOT_DOC", C.TOT_DOC);
                        cmd.Parameters.AddWithValue("@FEC_INI", C.FEC_INI);
                        cmd.Parameters.AddWithValue("@FEC_FIN", C.FEC_FIN);
                        cmd.Parameters.AddWithValue("@FEC_ENV", C.FEC_ENV);
                        cmd.Parameters.AddWithValue("@MTO_GRA", C.MTO_GRA);
                        cmd.Parameters.AddWithValue("@MTO_EXO", C.MTO_EXO);
                        cmd.Parameters.AddWithValue("@MTO_INA", C.MTO_INA);
                        cmd.Parameters.AddWithValue("@MTO_OCA", C.MTO_OCA);
                        cmd.Parameters.AddWithValue("@IMP_IGV", C.IMP_IGV);
                        cmd.Parameters.AddWithValue("@IMP_ISC", C.IMP_ISC);
                        cmd.Parameters.AddWithValue("@IMP_OTH", C.IMP_OTH);
                        cmd.Parameters.AddWithValue("@MTO_TOT", C.MTO_TOT);
                        cmd.Parameters.AddWithValue("@DOC_EST", "INI");
                        cmd.Parameters.AddWithValue("@NUM_SEC", C.NUM_SEC);
                        cmd.Parameters.AddWithValue("@FEC_CAD", C.FEC_CAD);
                        cmd.Parameters.AddWithValue("@RUC", MS.RucNumber);
                        SqlParameter XML = new SqlParameter("@VAR_FIR", SqlDbType.VarBinary);
                        XML.Value = C.VAR_FIR;
                        cmd.Parameters.Add(XML);

                        con.Open();
                        SqlDataReader dr;
                        dr = cmd.ExecuteReader();
                        if (dr.Read())
                        {
                            int Id = int.Parse(dr[0].ToString().Trim());
                            if (Id > 0)
                            {
                                con.Close();
                                ret =  Id;
                            }
                            else { con.Close(); }
                        }
                        else { con.Close(); }
                    }
                }

                foreach(string DOC in C.DOC_UPD_LIST)
                {
                    using (SqlConnection con = new SqlConnection(MS.BDCon))
                    {
                        using (SqlCommand cmd = new SqlCommand("[Fact].[O.Usp_InsertaResumen]", con))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@NUM_CE", DOC);
                            cmd.Parameters.AddWithValue("@SUMMARY", ret);

                            con.Open();
                            cmd.ExecuteNonQuery();
                            con.Close();
                        }
                    }
                }

                foreach(var obj in List)
                {
                    using (SqlConnection con = new SqlConnection(MS.BDCon))
                    {
                        using (SqlCommand cmd = new SqlCommand("[Fact].[O.Usp_ActualizaEstado_RC_DOC]", con))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@NUM_CPE", obj.NUM_CE);
                            cmd.Parameters.AddWithValue("@STATUS_RC_DOC", obj.STATUS_RC_DOC);

                            con.Open();
                            cmd.ExecuteNonQuery();
                            con.Close();
                        }
                    }
                }

            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            return ret;
        }

        public int SP_InsertaCabeceraRC(RBoletasCabecera C, ListUtilClass List, string TypeRC)
        {
            int ret = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(MS.BDCon))
                {
                    using (SqlCommand cmd = new SqlCommand("[Fact].[O.Usp_InsertaCabeceraRB]", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@NUM_CPE", C.NUM_CPE);
                        cmd.Parameters.AddWithValue("@TOT_DOC", C.TOT_DOC);
                        cmd.Parameters.AddWithValue("@FEC_INI", C.FEC_INI);
                        cmd.Parameters.AddWithValue("@FEC_FIN", C.FEC_FIN);
                        cmd.Parameters.AddWithValue("@FEC_ENV", C.FEC_ENV);
                        cmd.Parameters.AddWithValue("@MTO_GRA", C.MTO_GRA);
                        cmd.Parameters.AddWithValue("@MTO_EXO", C.MTO_EXO);
                        cmd.Parameters.AddWithValue("@MTO_INA", C.MTO_INA);
                        cmd.Parameters.AddWithValue("@MTO_OCA", C.MTO_OCA);
                        cmd.Parameters.AddWithValue("@IMP_IGV", C.IMP_IGV);
                        cmd.Parameters.AddWithValue("@IMP_ISC", C.IMP_ISC);
                        cmd.Parameters.AddWithValue("@IMP_OTH", C.IMP_OTH);
                        cmd.Parameters.AddWithValue("@MTO_TOT", C.MTO_TOT);
                        cmd.Parameters.AddWithValue("@DOC_EST", "INI");
                        cmd.Parameters.AddWithValue("@NUM_SEC", C.NUM_SEC);
                        cmd.Parameters.AddWithValue("@FEC_CAD", C.FEC_CAD);
                        cmd.Parameters.AddWithValue("@RUC", MS.RucNumber);
                        cmd.Parameters.AddWithValue("@TYPERC", TypeRC);

                        SqlParameter XML = new SqlParameter("@VAR_FIR", SqlDbType.VarBinary);
                        XML.Value = C.VAR_FIR;
                        cmd.Parameters.Add(XML);

                        con.Open();
                        SqlDataReader dr;
                        dr = cmd.ExecuteReader();
                        if (dr.Read())
                        {
                            int Id = int.Parse(dr[0].ToString().Trim());
                            if (Id > 0)
                            {
                                con.Close();
                                ret = Id;
                            }
                            else { con.Close(); }
                        }
                        else { con.Close(); }
                    }
                }

                //foreach (string DOC in C.DOC_UPD_LIST)
                //{
                //    using (SqlConnection con = new SqlConnection(MS.BDCon))
                //    {
                //        using (SqlCommand cmd = new SqlCommand("[Fact].[O.Usp_InsertaResumen]", con))
                //        {
                //            cmd.CommandType = CommandType.StoredProcedure;
                //            cmd.Parameters.AddWithValue("@NUM_CE", DOC);
                //            cmd.Parameters.AddWithValue("@SUMMARY", ret);

                //            con.Open();
                //            cmd.ExecuteNonQuery();
                //            con.Close();
                //        }
                //    }
                //}

                foreach (var DOC in List)
                {
                    using (SqlConnection con = new SqlConnection(MS.BDCon))
                    {
                        using (SqlCommand cmd = new SqlCommand("[Fact].[O.Usp_InsertaResumen]", con))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@NUM_CE", DOC.NUM_CE);
                            cmd.Parameters.AddWithValue("@SUMMARY", ret);

                            con.Open();
                            cmd.ExecuteNonQuery();
                            con.Close();
                        }
                    }
                }

                foreach (var obj in List)
                {
                    using (SqlConnection con = new SqlConnection(MS.BDCon))
                    {
                        using (SqlCommand cmd = new SqlCommand("[Fact].[O.Usp_ActualizaEstado_RC_DOC]", con))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@NUM_CPE", obj.NUM_CE);
                            cmd.Parameters.AddWithValue("@STATUS_RC_DOC", obj.STATUS_RC_DOC);

                            con.Open();
                            cmd.ExecuteNonQuery();
                            con.Close();
                        }
                    }
                }

            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            return ret;
        }

        public string SP_InsertaDetalleRC(int ID_RBC, List<RBoletasDetalle> D)
        {
            string ret = "0";
            try
            {
                using (SqlConnection con = new SqlConnection(MS.BDCon))
                {
                    foreach (RBoletasDetalle line in D)
                    {
                        using (SqlCommand cmd = new SqlCommand("[Fact].[O.Usp_InsertaDetalleRB]", con))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@ID_RBC", ID_RBC);
                            cmd.Parameters.AddWithValue("@NUM_CPE", MS.DocumentName);
                            cmd.Parameters.AddWithValue("@TPO_CPE", line.TPO_CPE);
                            cmd.Parameters.AddWithValue("@DOC_SER", line.DOC_SER);
                            cmd.Parameters.AddWithValue("@NUM_INI", line.NUM_INI);
                            cmd.Parameters.AddWithValue("@NUM_FIN", line.NUM_FIN);
                            cmd.Parameters.AddWithValue("@MTO_GRA", line.MTO_GRA);
                            cmd.Parameters.AddWithValue("@MTO_EXO", line.MTO_EXO);
                            cmd.Parameters.AddWithValue("@MTO_INA", line.MTO_INA);
                            cmd.Parameters.AddWithValue("@MTO_OCA", line.MTO_OCA);
                            cmd.Parameters.AddWithValue("@IMP_IGV", line.IMP_IGV);
                            cmd.Parameters.AddWithValue("@IMP_ISC", line.IMP_ISC);
                            cmd.Parameters.AddWithValue("@IMP_OTH", line.IMP_OTH);
                            cmd.Parameters.AddWithValue("@MTO_TOT", line.MTO_TOT);
                            cmd.Parameters.AddWithValue("@NRO_LIN", line.NRO_LIN);
                            con.Open();
                            cmd.ExecuteNonQuery();
                            con.Close();
                        }
                    }
                    ret = "1";
                }
            }
            catch (SqlException ex)
            {
                ret = ex.Message;
            }
            return ret;
        }

        public int SP_InsertaCabeceraRA(RBajasCabecera C)
        {
            int ret = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(MS.BDCon))
                {
                    using (SqlCommand cmd = new SqlCommand("[Fact].[O.Usp_InsertaCabeceraRA]", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@NUM_CPE", C.NUM_CPE);
                        cmd.Parameters.AddWithValue("@TOT_DOC", C.TOT_DOC);
                        cmd.Parameters.AddWithValue("@FEC_REF", C.FEC_REF);
                        cmd.Parameters.AddWithValue("@FEC_ANU", C.FEC_ANU);
                        cmd.Parameters.AddWithValue("@FEC_ENV", C.FEC_ENV);
                        cmd.Parameters.AddWithValue("@DOC_EST", "INI");
                        cmd.Parameters.AddWithValue("@NUM_SEC", C.NUM_SEC);
                        cmd.Parameters.AddWithValue("@FEC_CAD", C.FEC_CAD);
                        cmd.Parameters.AddWithValue("@TIPO", C.TIPO);
                        cmd.Parameters.AddWithValue("@RUC", MS.RucNumber);
                        SqlParameter XML = new SqlParameter("@VAR_FIR", SqlDbType.VarBinary);
                        XML.Value = C.VAR_FIR;
                        cmd.Parameters.Add(XML);
                        con.Open();
                        SqlDataReader dr;
                        dr = cmd.ExecuteReader();
                        if (dr.Read())
                        {
                            int Id = int.Parse(dr[0].ToString().Trim());
                            if (Id > 0)
                            {
                                con.Close();
                                ret = Id;
                            }
                            else { con.Close(); }
                        }
                        else { con.Close(); }
                    }
                }
                foreach (string DOC in C.DOC_UPD_LIST)
                {
                    using (SqlConnection con = new SqlConnection(MS.BDCon))
                    {
                        using (SqlCommand cmd = new SqlCommand("[Fact].[O.Usp_InsertaAnulado]", con))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@NUM_CE", DOC);
                            cmd.Parameters.AddWithValue("@VOIDED", ret);

                            con.Open();
                            cmd.ExecuteNonQuery();
                            con.Close();
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            return ret;
        }

        public string SP_InsertaDetalleRA(List<RBajasDetalle> D, RBajasCabecera C)
        {
            string ret = "0";
            try
            {
                using (SqlConnection con = new SqlConnection(MS.BDCon))
                {
                    foreach (RBajasDetalle line in D)
                    {
                        using (SqlCommand cmd = new SqlCommand("[Fact].[O.Usp_InsertaDetalleRA]", con))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@ID_RAC", C.ID_RAC);
                            cmd.Parameters.AddWithValue("@NUM_CPE", MS.DocumentName);
                            cmd.Parameters.AddWithValue("@TPO_CPE", line.TPO_CPE);
                            cmd.Parameters.AddWithValue("@DOC_SER", line.DOC_SER);
                            cmd.Parameters.AddWithValue("@DOC_NUM", line.DOC_NUM);
                            cmd.Parameters.AddWithValue("@DOC_DES", line.DOC_DES);
                            string fechacadena = C.FEC_ANU.Day.ToString().PadLeft(2, '0') + "/" + C.FEC_ANU.Month.ToString().PadLeft(2, '0') + "/" + C.FEC_ANU.Year.ToString();
                            cmd.Parameters.AddWithValue("@DOC_FEC", fechacadena);
                            con.Open();
                            cmd.ExecuteNonQuery();
                            con.Close();
                        }
                    }
                    ret = "1";
                }
            }
            catch (SqlException ex)
            {
                ret = ex.Message;
            }
            return ret;
        }

        public int SP_Usp_InsertaXmlSign(byte[] VAR_FIR, int ID_DOC)
        {
            int ret = 1;
            try
            {
                using (SqlConnection con = new SqlConnection(MS.BDCon))
                {
                    using (SqlCommand cmd = new SqlCommand("[Fact].[O.Usp_InsertaXmlSign]", con))
                    {

                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Id_DC", ID_DOC);

                        SqlParameter XML = new SqlParameter("@VAR_FIR", SqlDbType.VarBinary);
                        XML.Value = VAR_FIR;
                        cmd.Parameters.Add(XML);

                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                        ret = 0;
                    }
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            return ret;
        }

        public int addXmlRes(byte[] VAR_RES, int ID_DOC)
        {
            int ret = 1;
            try
            {
                using (SqlConnection con = new SqlConnection(MS.BDCon))
                {
                    using (SqlCommand cmd = new SqlCommand("[Fact].[O.Usp_InsertaXmlResponse]", con))
                    {

                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Id_DC", ID_DOC);

                        SqlParameter XML = new SqlParameter("@VAR_RES", SqlDbType.VarBinary);
                        XML.Value = VAR_RES;
                        cmd.Parameters.Add(XML);

                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                        ret = 0;
                    }
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            return ret;
        }

        public int SP_InsertaCDR(byte[] VAR_RES, string NUM_CE, string ID_ED)
        {
            int ret = 1;
            SqlConnection con = null;
            try
            {
                using (con = new SqlConnection(MS.BDCon))
                {
                    using (SqlCommand cmd = new SqlCommand("[Fact].[O.Usp_InsertaCDR]", con))
                    {

                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@NUM_CE", NUM_CE);
                        cmd.Parameters.AddWithValue("@ID_ED", ID_ED);

                        SqlParameter XML = new SqlParameter("@VAR_RES", SqlDbType.VarBinary);
                        XML.Value = VAR_RES;
                        cmd.Parameters.Add(XML);

                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                        ret = 0;
                    }
                }
            }
            catch (SqlException ex)
            {
                con.Close();
                Console.WriteLine(ex.Message);
            }
            return ret;
        }

        public int SP_ActualizaEstado(int ID_DOC, string ID_ED, string NUM_CPE)
        {
            int ret = 1;
            try
            {
                using (SqlConnection con = new SqlConnection(MS.BDCon))
                {
                    using (SqlCommand cmd = new SqlCommand("[Fact].[O.Usp_ActualizaEstado]", con))
                    {

                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Id_DC", ID_DOC);
                        cmd.Parameters.AddWithValue("@Id_ED", ID_ED);
                        cmd.Parameters.AddWithValue("@NUM_CPE", NUM_CPE);

                        con.Open();
                        var res = cmd.ExecuteNonQuery();
                        con.Close();
                        ret = 0;

                        return res;
                    }
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            return ret;
        }

        public int addRXMLSign(byte[] VAR_FIR, int ID_DOC, char TIPO)
        {
            int ret = 1;
            try
            {
                using (SqlConnection con = new SqlConnection(MS.BDCon))
                {
                    using (SqlCommand cmd = new SqlCommand("[Fact].[O.Usp_InsertaRXMLSign]", con))
                    {

                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Id_DC", ID_DOC);
                        cmd.Parameters.AddWithValue("@TIPO", TIPO);

                        SqlParameter XML = new SqlParameter("@VAR_FIR", SqlDbType.VarBinary);
                        XML.Value = VAR_FIR;
                        cmd.Parameters.Add(XML);

                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                        ret = 0;
                    }
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            return ret;
        }

        public int SP_InsertaCDRResumen(byte[] VAR_RES, int ID_DOC, string TIPO)
        {
            if (TIPO == "R") TIPO = "A";
            int ret = 1;
            try
            {
                using (SqlConnection con = new SqlConnection(MS.BDCon))
                {
                    using (SqlCommand cmd = new SqlCommand("[Fact].[O.Usp_InsertaRXMLResponse]", con))
                    {

                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Id_DC", ID_DOC);
                        cmd.Parameters.AddWithValue("@TIPO", TIPO);

                        SqlParameter XML = new SqlParameter("@VAR_RES", SqlDbType.VarBinary);
                        XML.Value = VAR_RES;
                        cmd.Parameters.Add(XML);

                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                        ret = 0;
                    }
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            return ret;
        }

        public int addRSunatResponse(int ID_DOC, string DOC_EST, string DOC_MSG, string DOC_TCK, string TIPO)
        {
            if (TIPO == "R") TIPO = "A";
            int ret = 1;
            try
            {
                using (SqlConnection con = new SqlConnection(MS.BDCon))
                {
                    using (SqlCommand cmd = new SqlCommand("[Fact].[O.Usp_InsertaRespuesta]", con))
                    {

                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Id_DC", ID_DOC);
                        cmd.Parameters.AddWithValue("@DOC_EST", DOC_EST);
                        cmd.Parameters.AddWithValue("@DOC_MSG", DOC_MSG);
                        cmd.Parameters.AddWithValue("@DOC_TCK", DOC_TCK);
                        cmd.Parameters.AddWithValue("@TIPO", TIPO);

                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                        ret = 0;
                    }
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            return ret;
        }

        public int SP_InsertaMensajeRespuesta(int ID_DOC, string DOC_COD, string DOC_MSG)
        {
            int ret = 1;
            try
            {
                using (SqlConnection con = new SqlConnection(MS.BDCon))
                {
                    using (SqlCommand cmd = new SqlCommand("[Fact].[O.Usp_InsertaRespuestaSunat]", con))
                    {

                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Id_DC", ID_DOC);
                        cmd.Parameters.AddWithValue("@DOC_COD", DOC_COD);
                        cmd.Parameters.AddWithValue("@DOC_MSG", DOC_MSG);

                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                        ret = 0;
                    }
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            return ret;
        }

        public int SP_InsertaMensajeRespuesta_2(int ID_DOC, string DOC_COD)
        {
            int ret = 1;
            try
            {
                using (SqlConnection con = new SqlConnection(MS.BDCon))
                {
                    using (SqlCommand cmd = new SqlCommand("[Fact].[O.Usp_InsertaRespuestaSunat_2]", con))
                    {

                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Id_DC", ID_DOC);
                        cmd.Parameters.AddWithValue("@DOC_COD", DOC_COD);

                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                        ret = 0;
                    }
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            return ret;
        }

        public int SP_InsertaNotaSunat(int ID_CPE, string NUM_CPE, string ERR_COD, string ERR_TXT, char TIPO)
        {
            int ret = 1;
            try
            {
                using (SqlConnection con = new SqlConnection(MS.BDCon))
                {
                    using (SqlCommand cmd = new SqlCommand("[Fact].[O.Usp_InsertaNotasRespuesta]", con))
                    {

                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@NUM_CPE", NUM_CPE);
                        cmd.Parameters.AddWithValue("@ID_CPE", ID_CPE);
                        cmd.Parameters.AddWithValue("@ERR_COD", ERR_COD);
                        cmd.Parameters.AddWithValue("@ERR_TXT", ERR_TXT);
                        cmd.Parameters.AddWithValue("@TIPO", TIPO);
                        if (ERR_COD == "0")
                        {
                            cmd.Parameters.AddWithValue("@DOC_EST", "5");
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@DOC_EST", "6");
                        }
                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                        MS.GeneraLog.IngresoBD02("Ingreso de las notas fue correcto");
                        ret = 0;
                    }
                }
            }
            catch (SqlException ex)
            {
                MS.GeneraLog.IngresoBD02("Error en el ingreso de las notas: ");
                MS.GeneraLog.IngresoBD02(ex.Message);
            }
            return ret;
        }

        public List<string> SP_ObtieneDocumentosRes()
        {
            List<string> RES = new List<string>();
            try
            {
                using (SqlConnection con = new SqlConnection(MS.BDCon))
                {
                    using (SqlCommand cmd = new SqlCommand("[Fact].[O.Usp_ObtieneDocumentosRes]", con))
                    {
                        SqlDataReader dr;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Id_DC", MS.id);
                        con.Open();
                        dr = cmd.ExecuteReader();

                        while (dr.Read())
                        {
                            RES.Add(dr[0].ToString() + ";" + dr[1].ToString());
                        }
                        con.Close();
                    }
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine(" " + ex.Message);
            }
            return RES;
        }

        #region method

        public ListDocument Get_ListDocument_AN(int id)
        {
            Document obj = new Document();
            ListDocument oListObj = new ListDocument();
            SqlConnection cnn = new SqlConnection(MS.BDCon);
            try
            {
                SqlCommand cmd = new SqlCommand();

                cnn.Open();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "[Fact].[Usp_ObtenerDocument_AN]";
                cmd.Connection = cnn;

                cmd.Parameters.AddWithValue("@id_dc", id);

                using (IDataReader objReader = cmd.ExecuteReader())
                {
                    int indexId = objReader.GetOrdinal("Id");
                    int indexTypeDoc = objReader.GetOrdinal("TypeDoc");
                    int indexSerie = objReader.GetOrdinal("Serie");
                    int indexCorrelativo = objReader.GetOrdinal("Correlativo");
                    int indexNumCE = objReader.GetOrdinal("Num_CE");
                    int indexFec_Emi = objReader.GetOrdinal("Fec_Emi");
                    int indexFec_Res = objReader.GetOrdinal("Fec_Res");
                    int indexRznCli = objReader.GetOrdinal("Rzn_Soc_Cli");
                    int indexRznEmi = objReader.GetOrdinal("Rzn_Soc_Emi");
                    int indexTO = objReader.GetOrdinal("TO");
                    int indexCC = objReader.GetOrdinal("CC");
                    int indexCCO = objReader.GetOrdinal("CCO");
                    int indexNotify = objReader.GetOrdinal("Notify");
                    int indexCantDocs = objReader.GetOrdinal("Cant_Docs");
                    int indexEmail = objReader.GetOrdinal("Email");
                    int indexPwd = objReader.GetOrdinal("Pwd");
                    int indexDomain = objReader.GetOrdinal("Domain");
                    int indexIP = objReader.GetOrdinal("IP");
                    int indexPort = objReader.GetOrdinal("Port");
                    int indexUseSSL = objReader.GetOrdinal("UseSSL");
                    int indexUrlLogo = objReader.GetOrdinal("Url_Logo");
                    int indexTypeRC = objReader.GetOrdinal("TypeRC");

                    while (objReader.Read())
                    {
                        obj = new Document();

                        obj.Id = DataUtil.DbValueToDefault<int>(objReader[indexId]);
                        obj.TypeDoc = DataUtil.DbValueToDefault<string>(objReader[indexTypeDoc]);
                        obj.Serie = DataUtil.DbValueToDefault<string>(objReader[indexSerie]);
                        obj.Correlativo = DataUtil.DbValueToDefault<string>(objReader[indexCorrelativo]);
                        obj.Num_CE = DataUtil.DbValueToDefault<string>(objReader[indexNumCE]);
                        obj.Fec_Emi = DataUtil.DbValueToDefault<DateTime>(objReader[indexFec_Emi]);
                        obj.Fec_Res = DataUtil.DbValueToDefault<DateTime>(objReader[indexFec_Res]);
                        obj.Rzn_Soc_Cli = DataUtil.DbValueToDefault<string>(objReader[indexRznCli]);
                        obj.Rzn_Soc_Emi = DataUtil.DbValueToDefault<string>(objReader[indexRznEmi]);
                        obj.TO = DataUtil.DbValueToDefault<string>(objReader[indexTO]);
                        obj.CC = DataUtil.DbValueToDefault<string>(objReader[indexCC]);
                        obj.CCO = DataUtil.DbValueToDefault<string>(objReader[indexCCO]);
                        obj.Notify = DataUtil.DbValueToDefault<string>(objReader[indexNotify]);
                        obj.Cant_Docs = DataUtil.DbValueToDefault<int>(objReader[indexCantDocs]);
                        obj.Email = DataUtil.DbValueToDefault<string>(objReader[indexEmail]);
                        obj.Pwd = DataUtil.DbValueToDefault<string>(objReader[indexPwd]);
                        obj.Domain = DataUtil.DbValueToDefault<string>(objReader[indexDomain]);
                        obj.IP = DataUtil.DbValueToDefault<string>(objReader[indexIP]);
                        obj.Port = DataUtil.DbValueToDefault<int>(objReader[indexPort]);
                        obj.UseSSL = DataUtil.DbValueToDefault<int>(objReader[indexUseSSL]);
                        obj.Url_Logo = DataUtil.DbValueToDefault<string>(objReader[indexUrlLogo]);
                        obj.TypeRC = DataUtil.DbValueToDefault<string>(objReader[indexTypeRC]);
                        oListObj.Add(obj);
                    }
                }
                cnn.Close();
            }
            catch (Exception ex)
            {
                MS.GeneraLog.IngresoBD02("Error al obtener docs de la BD: ");
                MS.GeneraLog.IngresoBD02(ex.Message);
            }
            return oListObj;
        }

        #endregion
    }
}
