using System;
using System.ServiceModel;
using ADE.Configurations.Objects;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using ADE.Extras.Common.Method;

namespace ADE.Processes.SunatDelivery
{
    public class SunatConnection : IDisposable
    {
        public MainSettings MS = null;
        private SunatDEV.billServiceClient _proxyDEV;
        //private SunatPRD.billServiceClient _proxyHML;
        private SunatPRD.billServiceClient _proxyPRD;
        private SunatCDR.billServiceClient _proxyCDR;
        
        /// <summary>
        /// Nombre del Endpoint que referencia al Servicio de SUNAT
        /// </summary>
        public string EndPointServicio { get; set; }
        /// <summary>
        /// Constructor de la Clase ConexionSunat
        /// </summary>
        /// <param name="endPointServicio">Nombre del Endpoint</param>
        /// <param name="MS">Objeto MainSettings - Contiene ruc, user y pass de Sunat</param>
        public SunatConnection(string endPointServicio, MainSettings MS)
        {
            System.Net.ServicePointManager.UseNagleAlgorithm = true;
            System.Net.ServicePointManager.Expect100Continue = false;
            System.Net.ServicePointManager.CheckCertificateRevocationList = true;
            //System.Net.ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(IgnoreCertificateErrorHandler);
            
            this.MS = MS;
            MS.GeneraLog.EnvioSunat01("URL: " + MS.UEnv);
            if(MS.Envi == "DEV")
            {
                EndPointServicio = "DEV" + endPointServicio;
                //string urldev = @"https://e-beta.sunat.gob.pe:443/ol-ti-itcpfegem-beta/billService";
                //string urldev = @"https://www.sunat.gob.pe:443/ol-ti-itcpfegem/billService";
                _proxyDEV = new SunatDEV.billServiceClient(EndPointServicio,MS.UEnv);
                // Agregamos el behavior configurado para soportar WS-Security.
                var behavior = new PasswordDigestBehavior(string.Concat(MS.RucNumber, MS.SunatUsr), MS.SunatPwd);
                _proxyDEV.Endpoint.EndpointBehaviors.Add(behavior);
            }
            if (MS.Envi == "HML")
            {
                EndPointServicio = "HML" + endPointServicio;
                _proxyPRD = new SunatPRD.billServiceClient(EndPointServicio, MS.UEnv);
                // Agregamos el behavior configurado para soportar WS-Security.

                //var behavior = new PasswordDigestBehavior(string.Concat(MS.UserAltern.Length == 11 ? MS.UserAltern : MS.RucNumber, MS.SunatUsr), MS.SunatPwd);
                var behavior = new PasswordDigestBehavior(string.Concat(MS.RucNumber, MS.SunatUsr), MS.SunatPwd);
                _proxyPRD.Endpoint.EndpointBehaviors.Add(behavior);
            }
            if (MS.Envi == "PRD")
            {
                //if ("01|03|RC|RA".Contains(MS.DocumentType))
                //{
                    EndPointServicio = "PRD" + endPointServicio;
                    _proxyPRD = new SunatPRD.billServiceClient(EndPointServicio, MS.UEnv);
                    // Agregamos el behavior configurado para soportar WS-Security.
                    //var behavior = new PasswordDigestBehavior(string.Concat(MS.RucNumber, MS.SunatUsr), MS.SunatPwd);
                    var behavior = new PasswordDigestBehavior(string.Concat(MS.UserAltern.Length == 11 ? MS.UserAltern : MS.RucNumber, MS.SunatUsr), MS.SunatPwd);
                    _proxyPRD.Endpoint.EndpointBehaviors.Add(behavior);
                //}

                //if ("20|40".Contains(MS.DocumentType))
                //{
                //    EndPointServicio = "PRD" + endPointServicio;
                //    _proxyPRDr = new SunatPRDr.billServiceClient(EndPointServicio);
                //    //_proxyPRDr.ClientCredentials.CreateSecurityTokenManager();
                //    //_proxyPRDr.ClientCredentials.UserName.UserName = string.Concat(MS.RucNumber, MS.SunatUsr);
                //    //_proxyPRDr.ClientCredentials.UserName.Password = MS.SunatPwd;

                //    // Agregamos el behavior configurado para soportar WS-Security.
                //    var behavior = new PasswordDigestBehavior(string.Concat(MS.RucNumber, MS.SunatUsr), MS.SunatPwd);
                //    _proxyPRDr.Endpoint.EndpointBehaviors.Add(behavior);
                //}
            }
        }

        /// <summary>
        /// Enviar documento ZIP al WS Sunat
        /// </summary>
        /// <param name="tramaArchivo">Cadena Base64 de la trama del archivo</param>
        /// <param name="nombreArchivo">Nombre del archivo (sin extension)</param>
        /// <returns>Devuelve una tupla con la cadena Base64 del ZIP de respuesta (CDR) y un booleano si SUNAT responde correctamente</returns>
        public Tuple<string, bool> EnviarDocumento(byte[] zipBytes, string nombreArchivo)
        {
            Tuple<string, bool> response = null;
            
            try
            {
                switch (MS.Envi)
                {
                    case "DEV":
                        MS.GeneraLog.EnvioSunat01($"Se enviará el documento en ambiemte {MS.Envi} - Pruebas");
                        _proxyDEV.Open();
                        var resDEV = _proxyDEV.sendBill(nombreArchivo, zipBytes);
                        _proxyDEV.Close();
                        MS.GeneraLog.EnvioSunat01($"Documento enviado correctamente");
                        response = new Tuple<string, bool>(Convert.ToBase64String(resDEV), true);
                        break;
                    case "HML":
                        MS.GeneraLog.EnvioSunat01($"Se enviará el documento en ambiemte {MS.Envi} - Homologación");
                        _proxyPRD.Open();
                        var resHML = _proxyPRD.sendBill(nombreArchivo, zipBytes);
                        _proxyPRD.Close();
                        MS.GeneraLog.EnvioSunat01($"Documento enviado correctamente");
                        response = new Tuple<string, bool>(Convert.ToBase64String(resHML), true);
                        break;
                    case "PRD":
                        //if("01|03|RC|RA".Contains(MS.DocumentType))
                        //{
                            MS.GeneraLog.EnvioSunat01($"Se enviará el documento en ambiemte {MS.Envi} - Produccion");
                            _proxyPRD.Open();
                            var resPRD = _proxyPRD.sendBill(nombreArchivo, zipBytes);
                            _proxyPRD.Close();
                            MS.GeneraLog.EnvioSunat01($"Documento enviado correctamente");
                            response = new Tuple<string, bool>(Convert.ToBase64String(resPRD), true);
                        //}
                        //if("20|40".Contains(MS.DocumentType))
                        //{
                        //    MS.GeneraLog.EnvioSunat01($"Se enviará el documento en ambiemte {MS.Envi} - Produccion");
                        //    _proxyPRDr.Open();
                        //    var resPRDr = _proxyPRDr.sendBill(nombreArchivo, zipBytes);
                        //    _proxyPRDr.Close();
                        //    MS.GeneraLog.EnvioSunat01($"Documento enviado correctamente");
                        //    response = new Tuple<string, bool>(Convert.ToBase64String(resPRDr), true);
                        //}
                        break;
                    default:
                        response = new Tuple<string, bool>("No se ha definido el ambiente de envío", false);
                        break;
                }
            }
            catch (FaultException ex)
            {
                response = new Tuple<string, bool>(EndPointServicio.Contains("Documento")
                    ? ex.Code.Name : ex.Message, false);
                
                if(response.Item1.Contains("1032") || response.Item1.Contains("1033"))
                {
                    MS.GeneraLog.EnvioSunat01($"El Documento ya ha sido enviado a Sunat anteriormente");
                    MS.GeneraLog.EnvioSunat01($"Obteniendo estado y CDR del documento");
                    _proxyCDR = new SunatCDR.billServiceClient("CDRConsulta", MS.UCdr);

                    var behavior = new PasswordDigestBehavior(string.Concat(MS.RucNumber, MS.SunatUsr), MS.SunatPwd);
                    _proxyCDR.Endpoint.EndpointBehaviors.Add(behavior);

                    _proxyCDR.Open();

                    string[] div = nombreArchivo.Replace(".zip", "").Split('-');

                    var resCDR = _proxyCDR.getStatusCdr(MS.RucNumber, div[1], div[2], int.Parse(div[3].Substring(0, 8)));
                    
                    _proxyCDR.Close();
                    response = new Tuple<string, bool>(Convert.ToBase64String(resCDR.content), true);
                    //response = new Tuple<string, bool>(resCDR.statusCode, true);
                }
                else
                {
                    MS.GeneraLog.EnvioSunat01($"Ocurrió un error en el envío: {response.Item1}");
                }
            }
            catch (Exception ex)
            {
                var msg = string.Concat(ex.InnerException.Message ?? string.Empty, ex.Message);
                
                var faultCode = "<faultcode>";
                if (msg.Contains(faultCode))
                {
                    var posicion = msg.IndexOf(faultCode, StringComparison.Ordinal);
                    var codigoError = msg.Substring(posicion + faultCode.Length, 4);
                    msg = string.Format("El Código de Error es {0}", codigoError);
                }
                MS.GeneraLog.EnvioSunat01(msg);
                response = new Tuple<string, bool>(msg, false);
            }
            return response;
        }

        /// <summary>
        /// Envia el Resumen de Baja basado en la cadena Base64 del archivo ZIP
        /// </summary>
        /// <param name="tramaArchivo">Cadena Base64 del archivo ZIP</param>
        /// <param name="nombreArchivo">Nombre del archivo (sin extension)</param>
        /// <returns>Devuelve una tupla con el numero de Ticket del CDR y un booleano si SUNAT responde correctamente</returns>
        //public Tuple<string, bool> EnviarResumenBaja(byte[] zipBytes, string nombreArchivo)
        public Tuple<string, bool> EnviarResumen(byte[] zipBytes, string nombreArchivo)
        {
            Tuple<string, bool> response;

            try
            {
                switch (MS.Envi)
                {
                    case "DEV":
                        MS.GeneraLog.EnvioSunat01($"Se enviará el resumen en ambiemte {MS.Envi} - Pruebas");
                        _proxyDEV.Open();
                        var resDEV = _proxyDEV.sendSummary(nombreArchivo, zipBytes);
                        //_proxyDEV.Close();
                        MS.GeneraLog.EnvioSunat01($"Resumen enviado correctamente");
                        response = new Tuple<string, bool>(resDEV, true);
                        break;
                    case "HML":
                        MS.GeneraLog.EnvioSunat01($"Se enviará el resumen en ambiemte {MS.Envi} - Homologación");
                        _proxyPRD.Open();
                        var resHML = _proxyPRD.sendSummary(nombreArchivo, zipBytes);
                        //_proxyPRD.Close();
                        MS.GeneraLog.EnvioSunat01($"Resumen enviado correctamente");
                        response = new Tuple<string, bool>(resHML, true);
                        break;
                    case "PRD":
                        MS.GeneraLog.EnvioSunat01($"Se enviará el resumen en ambiemte {MS.Envi} - Produccion");
                        _proxyPRD.Open();
                        var resPRD = _proxyPRD.sendSummary(nombreArchivo, zipBytes);
                        //_proxyPRD.Close();
                        MS.GeneraLog.EnvioSunat01($"Resumen enviado correctamente");
                        response = new Tuple<string, bool>(resPRD, true);
                        break;
                    default:
                        response = new Tuple<string, bool>("No se ha definido el ambiente de envío", false);
                        break;
                }
            }
            catch (FaultException ex)
            {
                response = new Tuple<string, bool>(ex.Code.Name, false);
            }
            catch (Exception ex)
            {
                var msg = string.Concat(ex.InnerException?.Message ?? string.Empty, ex.Message);
                var faultCode = "<faultcode>";
                if (msg.Contains(faultCode))
                {
                    var posicion = msg.IndexOf(faultCode, StringComparison.Ordinal);
                    var codigoError = msg.Substring(posicion + faultCode.Length, 4);
                    msg = $"El Código de Error es {codigoError}";
                }
                response = new Tuple<string, bool>(msg, false);
            }

            return response;
        }

        /// <summary>
        /// Devuelve el estado de un CDR basado en el numero de Ticket
        /// </summary>
        /// <param name="numeroTicket">Numero de Ticket proporcionado por SUNAT</param>
        /// <returns>Devuelve una tupla con la cadena Base64 del ZIP de respuesta (CDR) y un booleano si SUNAT responde con el estado correcto</returns>
        public Tuple<string, bool> ObtenerEstadoAuto(string numeroTicket)
        {
            Tuple<string, bool> response;

            try
            {
                switch (MS.Envi)
                {
                    case "DEV":
                        MS.GeneraLog.EnvioSunat01($"Consultando estado del ticket {numeroTicket} en ambiemte {MS.Envi} - Pruebas");
                        //_proxyDEV.Open();
                        var resDEV = _proxyDEV.getStatus(numeroTicket);
                        _proxyDEV.Close();
                        MS.GeneraLog.EnvioSunat01($"Ticket consultado correctamente");
                        response = new Tuple<string, bool>(Convert.ToBase64String(resDEV.content), true);
                        var estDEV = (resDEV.statusCode != "98");

                        response = new Tuple<string, bool>(estDEV
                            ? Convert.ToBase64String(resDEV.content) : "Aun en proceso", estDEV);

                        break;
                    case "HML":
                        MS.GeneraLog.EnvioSunat01($"Consultando estado del ticket {numeroTicket} en ambiemte {MS.Envi} - Homologación");
                        //_proxyPRD.Open();
                        var resHML = _proxyPRD.getStatus(numeroTicket);
                        _proxyPRD.Close();
                        MS.GeneraLog.EnvioSunat01($"Ticket consultado correctamente");
                        response = new Tuple<string, bool>(Convert.ToBase64String(resHML.content), true);

                        var estHML = (resHML.statusCode != "98");

                        response = new Tuple<string, bool>(estHML
                            ? Convert.ToBase64String(resHML.content) : "Aun en proceso", estHML);

                        break;
                    case "PRD":
                        MS.GeneraLog.EnvioSunat01($"Consultando estado del ticket {numeroTicket} en ambiemte {MS.Envi} - Produccion");
                        //_proxyPRD.Open();
                        var resPRD = _proxyPRD.getStatus(numeroTicket);
                        _proxyPRD.Close();
                        MS.GeneraLog.EnvioSunat01($"Ticket consultado correctamente");
                        response = new Tuple<string, bool>(Convert.ToBase64String(resPRD.content), true);

                        var estPRD = (resPRD.statusCode != "98");
                        MS.GeneraLog.EnvioSunat01($"Status: {resPRD.statusCode} - {estPRD}");
                        response = new Tuple<string, bool>(estPRD
                            ? Convert.ToBase64String(resPRD.content) : "Aun en proceso", estPRD);

                        break;
                    default:
                        response = new Tuple<string, bool>("No se ha definido el ambiente de envío", false);
                        break;
                }

                //var estado = (resultado.statusCode != "98");

                //response = new Tuple<string, bool>(estado
                //    ? Convert.ToBase64String(resultado.content) : "Aun en proceso", estado);
            }
            catch (FaultException ex)
            {
                response = new Tuple<string, bool>(ex.Code.Name, false);
            }
            catch (Exception ex)
            {
                var msg = string.Concat(ex.InnerException?.Message ?? string.Empty, ex.Message);
                var faultCode = "<faultcode>";
                if (msg.Contains(faultCode))
                {
                    var posicion = msg.IndexOf(faultCode, StringComparison.Ordinal);
                    var codigoError = msg.Substring(posicion + faultCode.Length, 4);
                    msg = $"El Código de Error es {codigoError}";
                }
                response = new Tuple<string, bool>(msg, false);
            }

            return response;
        }

        /// <summary>
        /// Devuelve el estado de un CDR basado en el numero de Ticket
        /// </summary>
        /// <param name="numeroTicket">Numero de Ticket proporcionado por SUNAT</param>
        /// <returns>Devuelve una tupla con la cadena Base64 del ZIP de respuesta (CDR) y un booleano si SUNAT responde con el estado correcto</returns>
        public Tuple<string, bool> ObtenerEstadoUnit(string numeroTicket)
        {
            UtilClass responseC = new UtilClass();
            

            Tuple<string, bool> response;

            try
            {
                switch (MS.Envi)
                {
                    case "DEV":
                        MS.GeneraLog.EnvioSunat01($"Consultando estado del ticket {numeroTicket} en ambiemte {MS.Envi} - Pruebas");
                        _proxyDEV.Open();
                        var resDEV = _proxyDEV.getStatus(numeroTicket);
                        _proxyDEV.Close();
                        MS.GeneraLog.EnvioSunat01($"Ticket consultado correctamente");
                        response = new Tuple<string, bool>(Convert.ToBase64String(resDEV.content), true);
                        var estDEV = (!resDEV.statusCode.EndsWith("98"));

                        response = new Tuple<string, bool>(estDEV ? Convert.ToBase64String(resDEV.content) : "Aun en proceso", estDEV);

                        break;
                    case "HML":
                        MS.GeneraLog.EnvioSunat01($"Consultando estado del ticket {numeroTicket} en ambiemte {MS.Envi} - Produccion");
                        _proxyPRD.Open();
                        var resHML = _proxyPRD.getStatus(numeroTicket);
                        _proxyPRD.Close();
                        MS.GeneraLog.EnvioSunat01($"Ticket consultado correctamente");
                        response = new Tuple<string, bool>(Convert.ToBase64String(resHML.content), true);

                        var estHML = (!resHML.statusCode.EndsWith("98"));

                        response = new Tuple<string, bool>(estHML
                            ? Convert.ToBase64String(resHML.content) : "Aun en proceso", estHML);

                        break;
                    case "PRD":
                        MS.GeneraLog.EnvioSunat01($"Consultando estado del ticket {numeroTicket} en ambiemte {MS.Envi} - Produccion");
                        _proxyPRD.Open();
                        var resPRD = _proxyPRD.getStatus(numeroTicket);
                        _proxyPRD.Close();
                        MS.GeneraLog.EnvioSunat01($"Ticket consultado correctamente");
                        response = new Tuple<string, bool>(Convert.ToBase64String(resPRD.content), true);

                        var estPRD = (resPRD.statusCode.EndsWith("0"));
                        MS.GeneraLog.EnvioSunat01($"Status: {resPRD.statusCode} - {estPRD}");
                        response = new Tuple<string, bool>(estPRD
                            ? Convert.ToBase64String(resPRD.content) : "Aun en proceso", estPRD);

                        if(resPRD.statusCode == "99")
                        {
                            response = null;
                            //response = new Tuple<string, bool>(resPRD.statusCode, estPRD);
                            if(Convert.ToBase64String(resPRD.content).Length > 200)
                            {
                                response = new Tuple<string, bool>(Convert.ToBase64String(resPRD.content), estPRD);
                            }
                            else
                            {
                                response = new Tuple<string, bool>(resPRD.statusCode, estPRD);
                            }
                        }

                        break;
                    default:
                        response = new Tuple<string, bool>("No se ha definido el ambiente de envío", false);
                        break;
                }

                //var estado = (resultado.statusCode != "98");

                //response = new Tuple<string, bool>(estado
                //    ? Convert.ToBase64String(resultado.content) : "Aun en proceso", estado);
            }
            catch (FaultException ex)
            {
                MS.GeneraLog.EnvioSunat01($"Error(1) al consultar Ticket {numeroTicket} : {ex.Code.Name} | {ex.Message}");
                response = new Tuple<string, bool>(ex.Code.Name, false);
            }
            catch (Exception ex)
            {
                var msg = string.Concat(ex.InnerException?.Message ?? string.Empty, ex.Message);
                var faultCode = "<faultcode>";
                MS.GeneraLog.EnvioSunat01($"Error(2) al consultar Ticket {numeroTicket} :  {ex.Message}");
                if (msg.Contains(faultCode))
                {
                    var posicion = msg.IndexOf(faultCode, StringComparison.Ordinal);
                    var codigoError = msg.Substring(posicion + faultCode.Length, 4);
                    msg = $"El Código de Error es {codigoError}";
                    MS.GeneraLog.EnvioSunat01($"El Código de Error es {codigoError}");
                }
                response = new Tuple<string, bool>(msg, false);
            }

            return response;
        }

        #region IDisposable Support
        private bool _disposedValue; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _proxyDEV = null;
                }

                _disposedValue = true;
            }
        }

        // ~Connect() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // GC.SuppressFinalize(this);
        }
        #endregion

        public static bool IgnoreCertificateErrorHandler(
            object sender,
            X509Certificate certificate,
            X509Chain chain,
            SslPolicyErrors sslPolicyErrors)
        {
            System.Console.WriteLine("Warning, trust any certificate");

            return true;
        }
    }


}
