using ADE.Configurations.Entities;
using ADE.Configurations.Entities.Database;
using ADE.Configurations.Objects;
using eProcessFactura;
using eProcessNotaCredito;
using eProcessNotaDebito;
using eProcessRetencion;
using eProcessDocumentosAnulados;
using eProcessResumenDocumentos;
using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Xml;
using System.Globalization;

namespace ADE.Processes.DatabaseIncome
{
    public class XMLGeneration
    {
        public MainSettings MS = null;
        public Validation VD = null;
        public DBDocument Document = null;
        //public Extras.Common.Method.ListUtilClass ListUpdRC = null;

        //private Validation _vd;
        //public Validation VD
        //{
        //    get { return _vd; }
        //    set { _vd = value; }
        //}

        public XMLGeneration(MainSettings MSI, Validation VDI)
        {
            MS = MSI;
            VD = VDI;
            Document = new DBDocument();
            //ListUpdRC = new Extras.Common.Method.ListUtilClass();
        }

        //public bool Generate()
        //public object Generate(string TypeRC)
        public Extras.Common.Method.ListUtilClass Generate(string TypeRC)
        {
            Extras.Common.Method.ListUtilClass ListResult = new Extras.Common.Method.ListUtilClass();
            //object obj = new object();

            bool isGenerated = false;
            bool isSigned = false;
            DateTime Actual = DateTime.Now;

            #region Generacion y Firma del XML
            switch (MS.DocumentType)
            {
                case "01":
                case "03":
                    EInvoice inv = new EInvoice(MS, VD);
                    if (inv.GenerateXML())
                    {
                        if (Sign(inv.SXMLI))
                        {
                            isSigned = true;
                        }
                    }
                    break;
                case "07":
                    ECreditNote cre = new ECreditNote(MS, VD);
                    if (cre.GenerateXML())
                    {
                        if (Sign(cre.SXMLI))
                        {
                            isSigned = true;
                        }
                    }
                    break;
                case "08":
                    EDebitNote deb = new EDebitNote(MS, VD);
                    if (deb.GenerateXML())
                    {
                        if (Sign(deb.SXMLI))
                        {
                            isSigned = true;
                        }
                    }
                    break;
                case "20":
                    ERetention ret = new ERetention(MS, VD);
                    if (ret.GenerateXML())
                    {
                        if (Sign(ret.SXMLI))
                        { isSigned = true; }
                    }
                    break;
                case "40":
                    break;
                case "RC":
                    DocumentTypesNew.ESummaryDocumentsNew summ = new DocumentTypesNew.ESummaryDocumentsNew(MS);
                    ListResult = summ.GenerateXML(TypeRC);
                    ////if (summ.GenerateXML())
                    if (ListResult.Count > 0)
                    {
                        MS = summ.MS;
                        if (Sign(summ.SXMLI))
                        {
                            Document = new DBDocument();
                            Document.RBC = summ.RBC;
                            Document.RBD = summ.RBD;
                            Document.RBC.FEC_ENV = DateTime.Parse($"{Actual.Year}-{Actual.Month.ToString().PadLeft(2, '0')}-{Actual.Day.ToString().PadLeft(2, '0')}");
                            Document.RBC.FEC_CAD = Actual.ToString();
                            Document.RBC.VAR_FIR = GetBytes(MS.XMLS);
                            isSigned = true;
                        }
                    }
                    //ESummaryDocuments sum = new ESummaryDocuments(MS);
                    //if (sum.GenerateXML())
                    //{
                    //    MS = sum.MS;
                    //    if (Sign(sum.SXMLI))
                    //    {
                    //        Document = new DBDocument();
                    //        Document.RBC = sum.RBC;
                    //        Document.RBD = sum.RBD;
                    //        Document.RBC.FEC_ENV = DateTime.Parse($"{Actual.Year}-{Actual.Month.ToString().PadLeft(2, '0')}-{Actual.Day.ToString().PadLeft(2, '0')}");
                    //        Document.RBC.FEC_CAD = Actual.ToString();
                    //        Document.RBC.VAR_FIR = GetBytes(MS.XMLS);
                    //        isSigned = true;
                    //    }
                    //}
                    break;

                case "RA":
                case "RR":
                    EVoidedDocuments voi = new EVoidedDocuments(MS);
                    if (voi.GenerateXML())
                    {
                        MS = voi.MS;
                        if (Sign(voi.SXMLI))
                        {
                            Document = new DBDocument();
                            Document.RAC = voi.RAC;
                            Document.RAD = voi.RAD;
                            Document.RAC.FEC_ENV = DateTime.Parse($"{Actual.Year}-{Actual.Month.ToString().PadLeft(2, '0')}-{Actual.Day.ToString().PadLeft(2, '0')}");
                            Document.RAC.FEC_ANU = DateTime.Parse($"{Actual.Year}-{Actual.Month.ToString().PadLeft(2, '0')}-{Actual.Day.ToString().PadLeft(2, '0')}");
                            Document.RAC.FEC_REF = DateTime.Parse(MS.SummaryDate);
                            Document.RAC.FEC_CAD = Actual.ToString();
                            Document.RAC.VAR_FIR = GetBytes(MS.XMLS);
                            Document.RAC.TIPO = MS.DocumentType;

                            isSigned = true;
                        }
                    }
                    break;

                default:
                    MS.GeneraLog.GeneracionXML($"El Tipo de Documento {MS.DocumentType} no está permitido");
                    MS.GeneraLog.RegistraError(MS.DocumentName);
                    isSigned = false;
                    break;
            }
            #endregion

            if (isSigned && !MS.DocumentType.StartsWith("R"))
            {
                if (FillDocument())
                {
                    isGenerated = true;
                    //return isGenerated;
                    ListResult.Generate = true;
                    return ListResult;
                }
            }
            if (isSigned && MS.DocumentType.StartsWith("R"))
            {
                isGenerated = true;
                //obj = new Extras.Common.Method.ListUtilClass();
                
                ListResult.Generate = true;
                //obj = ListResult;
                
                
                return ListResult;
            }
            //return isGenerated;
            return ListResult;
        }

        public bool Sign(Stream SXMLI)
        {
            string l_xml = MS.XMLIFileLocation;
            string l_certificado = MS.CERF + MS.CERN;
            string l_pwd = MS.CERP;
            string l_xpath;

            X509Certificate2 l_cert = null;

            string IXML;
            using (StreamReader reader = new StreamReader(MS.XMLIFileLocation, Encoding.GetEncoding("ISO-8859-1")))
            {
                IXML = reader.ReadToEnd();
            }


            MS.GeneraLog.GeneracionXML("Inicio de Firmado del archivo");
            try
            {
                l_cert = new X509Certificate2(l_certificado, l_pwd, X509KeyStorageFlags.MachineKeySet);

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.PreserveWhitespace = true;
                //xmlDoc.Load(SXMLI);
                xmlDoc.LoadXml(IXML);

                SignedXml signedXml = new SignedXml(xmlDoc);

                signedXml.SigningKey = l_cert.PrivateKey;
                KeyInfo KeyInfo = new KeyInfo();
                
                Reference Reference = new Reference();
                Reference.Uri = "";

                Reference.AddTransform(new XmlDsigEnvelopedSignatureTransform());
                signedXml.AddReference(Reference);

                X509Chain X509Chain = new X509Chain();
                X509Chain.Build(l_cert);

                X509ChainElement local_element = X509Chain.ChainElements[0];
                KeyInfoX509Data x509Data = new KeyInfoX509Data(local_element.Certificate);
                string subjectName = local_element.Certificate.Subject;

                x509Data.AddSubjectName(subjectName);
                KeyInfo.AddClause(x509Data);

                signedXml.KeyInfo = KeyInfo;
                signedXml.ComputeSignature();

                XmlElement signature = signedXml.GetXml();
                signature.Prefix = "ds";
                signedXml.ComputeSignature();

                foreach (XmlNode loop_node in signature.SelectNodes("descendant-or-self::*[namespace-uri()='http://www.w3.org/2000/09/xmldsig#']"))
                {
                    if (loop_node.LocalName == "Signature")
                    {
                        XmlAttribute newAttribute = xmlDoc.CreateAttribute("Id");
                        newAttribute.Value = "SignatureSP";
                        loop_node.Attributes.Append(newAttribute);
                    }
                }

                XmlNamespaceManager nsMgr = new XmlNamespaceManager(xmlDoc.NameTable);
                nsMgr.AddNamespace("sac", "urn:sunat:names:specification:ubl:peru:schema:xsd:SunatAggregateComponents-1");
                nsMgr.AddNamespace("ccts", "urn:un:unece:uncefact:documentation:2");
                nsMgr.AddNamespace("xsi", "http://www.w3.org/2001/XMLSchema-instance");

                l_xpath = "";

                if (MS.DocumentType == "01" || MS.DocumentType == "03") //factura
                {
                    nsMgr.AddNamespace("tns", "urn:oasis:names:specification:ubl:schema:xsd:Invoice-2");
                    l_xpath = "/tns:Invoice/ext:UBLExtensions/ext:UBLExtension[3]/ext:ExtensionContent";
                }
                else if (MS.DocumentType == "07") //nota de crédito
                {
                    nsMgr.AddNamespace("tns", "urn:oasis:names:specification:ubl:schema:xsd:CreditNote-2");
                    l_xpath = "/tns:CreditNote/ext:UBLExtensions/ext:UBLExtension[3]/ext:ExtensionContent";
                }
                else if (MS.DocumentType == "08")//nota de débito
                {
                    nsMgr.AddNamespace("tns", "urn:oasis:names:specification:ubl:schema:xsd:DebitNote-2");
                    l_xpath = "/tns:DebitNote/ext:UBLExtensions/ext:UBLExtension[3]/ext:ExtensionContent";
                }
                else if (MS.DocumentType == "20")//comprobante de retención
                {
                    nsMgr.AddNamespace("tns", "urn:sunat:names:specification:ubl:peru:schema:xsd:Retention-1");
                    l_xpath = "/tns:Retention/ext:UBLExtensions/ext:UBLExtension[2]/ext:ExtensionContent";
                }
                else if (MS.DocumentType == "40")//comprobante de percepción
                {
                    nsMgr.AddNamespace("tns", "urn:sunat:names:specification:ubl:peru:schema:xsd:Perception-1");
                    l_xpath = "/tns:Perception/ext:UBLExtensions/ext:UBLExtension/ext:ExtensionContent";
                }
                else if (MS.DocumentType == "RC")//resumen de boletas
                {
                    nsMgr.AddNamespace("tns", "urn:sunat:names:specification:ubl:peru:schema:xsd:SummaryDocuments-1");
                    l_xpath = "/tns:SummaryDocuments/ext:UBLExtensions/ext:UBLExtension/ext:ExtensionContent";
                }
                else if (MS.DocumentType == "RA" || MS.DocumentType == "RR")//resumen de bajas
                {
                    nsMgr.AddNamespace("tns", "urn:sunat:names:specification:ubl:peru:schema:xsd:VoidedDocuments-1");
                    l_xpath = "/tns:VoidedDocuments/ext:UBLExtensions/ext:UBLExtension/ext:ExtensionContent";
                }


                nsMgr.AddNamespace("cac", "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2");
                nsMgr.AddNamespace("udt", "urn:un:unece:uncefact:data:specification:UnqualifiedDataTypesSchemaModule:2");
                nsMgr.AddNamespace("ext", "urn:oasis:names:specification:ubl:schema:xsd:CommonExtensionComponents-2");
                nsMgr.AddNamespace("qdt", "urn:oasis:names:specification:ubl:schema:xsd:QualifiedDatatypes-2");
                nsMgr.AddNamespace("cbc", "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2");
                nsMgr.AddNamespace("ds", "http://www.w3.org/2000/09/xmldsig#");
                try
                {
                    xmlDoc.SelectSingleNode(l_xpath, nsMgr).AppendChild(xmlDoc.ImportNode(signature, true));
                }
                catch (Exception F)
                {
                    MS.GeneraLog.GeneracionXML("No se encontró el namespace " + l_xpath + " en el archivo " + MS.XMLI);
                    MS.GeneraLog.GeneracionXML($"Error: {F.Message}");
                    return false;
                }






                try
                {
                    MemoryStream mstream = new MemoryStream();

                    xmlDoc.Save(mstream);
                    mstream.Position = 0;
                    var sr = new StreamReader(mstream, Encoding.GetEncoding("ISO-8859-1"));
                    MS.XMLS = sr.ReadToEnd();
                    using (StreamWriter sw = new StreamWriter(MS.XMLSFileLocation, false, Encoding.GetEncoding("ISO-8859-1")))
                    {
                        sw.Write(MS.XMLS);
                        MS.GeneraLog.GeneracionXML("Creación del archivo firmado correcta");
                    }

                    ///*xml*/Doc.Save(MS.XMLSFileLocation);
                    //MS.GeneraLog.GeneracionXML("Creación del archivo firmado correcta");
                }
                catch (Exception F)
                {
                    MS.GeneraLog.GeneracionXML("El archivo firmado no se pudo crear correctamente:  " + F.Message);
                    return false;
                }

                MS.GeneraLog.GeneracionXML("Se firmó el archivo en la ruta " + MS.XMLSFileLocation);
                //check signature
                MS.GeneraLog.GeneracionXML($"Validando firma digital");
                XmlNodeList nodeList = xmlDoc.GetElementsByTagName("ds:Signature");
                if (nodeList.Count <= 0)
                {
                    MS.GeneraLog.GeneracionXML($"Verification failed: No Signature was found in the document.");
                    MS.GeneraLog.GeneracionXML("-----  Fin    de Generación de Archivo: ");
                    return false;
                    //MessageBox.Show("");
                }
                else if (nodeList.Count >= 2)
                {
                    MS.GeneraLog.GeneracionXML($"Verification failed: More that one signature was found for the document.");
                    MS.GeneraLog.GeneracionXML("-----  Fin    de Generación de Archivo: ");
                    return false;
                    //MessageBox.Show("");
                }
                else
                {
                    signedXml.LoadXml((XmlElement)nodeList[0]);
                    if (signedXml.CheckSignature())
                    {
                        //signature ok
                        MS.GeneraLog.GeneracionXML($"Firmado Correcto");
                    }
                    else
                    {
                        //signature failed
                        MS.GeneraLog.GeneracionXML($"Firmado Fallido");
                        MS.GeneraLog.GeneracionXML("-----  Fin    de Generación de Archivo: ");
                        return false;
                    }
                }
                
            }
            catch (System.Security.Cryptography.CryptographicException e)
            {
                MS.GeneraLog.GeneracionXML($"Error al acceder al certificado: ");
                MS.GeneraLog.GeneracionXML($"{e.Message}");
                MS.GeneraLog.GeneracionXML("-----  Fin    de Generación de Archivo: ");
                return false;
            }
            catch (Exception e)
            {
                MS.GeneraLog.GeneracionXML($"Error al firmar el documento: ");
                MS.GeneraLog.GeneracionXML($"{e.Message}");
                MS.GeneraLog.GeneracionXML("-----  Fin    de Generación de Archivo: ");
                return false;
            }

            MS.GeneraLog.GeneracionXML("-----  Fin    de Generación de Archivo: ");            
            return true;
        }

        public byte[] GetBytes(string str)
        {
            //return Encoding.GetEncoding("iso-8859-1").GetBytes();
            return File.ReadAllBytes(MS.XMLSFileLocation);
        }
        
        public bool FillDocument()
        {

            Document = new DBDocument();

            /*
             * En este método se llenan los objetos correspondientes a cada tabla para
             * que sean almacenados en la bd
             */
            //CE
            Document.Cabecera.NUM_CPE = VD.Interface["NUM_CE"];

            // CABECERA-PRINICIPAL
            Document.Cabecera.ID_TPO_CPE = VD.Interface["TipoCE"];
            Document.Cabecera.ID_CPE = VD.Interface["Id_CE"];
            Document.Cabecera.ID_TPO_OPERACION = VD.Interface["IdTpoOperacion"];
            Document.Cabecera.FEC_EMIS = DateTime.Parse(VD.Interface["FEmision"]);
            Document.Cabecera.TPO_MONEDA = VD.Interface["Tmoneda"];
            string[] sn = VD.Interface["Id_CE"].Split('-');
            Document.Cabecera.SERIE = sn[0];
            Document.Cabecera.NUM_DOCUMENTO = sn[1];

            // CABECERA-EMISOR
            Document.Cabecera.EM_TPO_DOC = VD.Interface["EmiTpoDoc"];
            Document.Cabecera.EM_NUM_DOCU = VD.Interface["EmiNumDocu"];
            Document.Cabecera.EM_NOMBRE = VD.Interface["EmiNombre"];
            Document.Cabecera.EM_NCOMER = VD.Interface["EmiNComer"];
            Document.Cabecera.EM_UBIGEO = VD.Interface["EmiUbigeo"];
            Document.Cabecera.EM_DFISCAL = VD.Interface["EmiDirFiscal"];
            Document.Cabecera.EM_DURBANIZ = VD.Interface["EmiDirUrbani"];
            Document.Cabecera.EM_DIR_PROV = VD.Interface["EmiDirProvin"];
            Document.Cabecera.EM_DIR_DPTO = VD.Interface["EmiDirDepart"];
            Document.Cabecera.EM_DIR_DIST = VD.Interface["EmiDirDistrito"];
            Document.Cabecera.EM_COD_PAIS = VD.Interface["EmiCodPais"];

            // CABECERA-RECEPTOR
            Document.Cabecera.RE_TPODOC = VD.Interface["RecTpoDoc"];
            Document.Cabecera.RE_NUMDOC = VD.Interface["RecNumDocu"];
            Document.Cabecera.RE_NOMBRE = VD.Interface["RecNombre"];
            Document.Cabecera.RE_NCOMER = VD.Interface["RecNComer"];
            Document.Cabecera.RE_UBIGEO = VD.Interface["RecUbigeo"];
            Document.Cabecera.RE_DIRECCION = VD.Interface["RecDirFiscal"];
            Document.Cabecera.RE_DURBANIZ = VD.Interface["RecDirUrbani"];
            Document.Cabecera.RE_DIR_PROV = VD.Interface["RecDirProvin"];
            Document.Cabecera.RE_DIR_DPTO = VD.Interface["RecDirDepart"];
            Document.Cabecera.RE_DIR_DIST = VD.Interface["RecDirDistrito"];
            Document.Cabecera.RE_COD_PAIS = VD.Interface["RecCodPais"];
            
            if(MS.DocumentType != "20" && MS.DocumentType != "40")
            {
                // TOTAL
                if (VD.Interface["TotVtaGrab"].Length > 0) Document.Cabecera.TOT_GRAV_MTO = decimal.Parse(VD.Interface["TotVtaGrab"], CultureInfo.CreateSpecificCulture("es-PE"));
                if (VD.Interface["TotVtaInaf"].Length > 0) Document.Cabecera.TOT_INAF_MTO = decimal.Parse(VD.Interface["TotVtaInaf"], CultureInfo.CreateSpecificCulture("es-PE"));
                if (VD.Interface["TotVtaExon"].Length > 0) Document.Cabecera.TOT_EXON_MTO = decimal.Parse(VD.Interface["TotVtaExon"], CultureInfo.CreateSpecificCulture("es-PE"));
                if (VD.Interface["TotVtaGrat"].Length > 0) Document.Cabecera.TOT_GRAT_MTO = decimal.Parse(VD.Interface["TotVtaGrat"], CultureInfo.CreateSpecificCulture("es-PE"));
                if (VD.Interface["TotTotDscto"].Length > 0) Document.Cabecera.TOT_DSCTO_MTO = decimal.Parse(VD.Interface["TotTotDscto"], CultureInfo.CreateSpecificCulture("es-PE"));
                if (VD.Interface["TotSumIGV"].Length > 0) Document.Cabecera.TOT_SUMA_IGV = decimal.Parse(VD.Interface["TotSumIGV"], CultureInfo.CreateSpecificCulture("es-PE"));
                if (VD.Interface["TotSumISC"].Length > 0) Document.Cabecera.TOT_SUMA_ISC = decimal.Parse(VD.Interface["TotSumISC"], CultureInfo.CreateSpecificCulture("es-PE"));
                if (VD.Interface["TotSumOTrib"].Length > 0) Document.Cabecera.TOT_SUMA_OTRIB = decimal.Parse(VD.Interface["TotSumOTrib"], CultureInfo.CreateSpecificCulture("es-PE"));
                if (VD.Interface["TotDctoGlobal"].Length > 0) Document.Cabecera.TOT_DCTO_GLOB = decimal.Parse(VD.Interface["TotDctoGlobal"], CultureInfo.CreateSpecificCulture("es-PE"));
                if (VD.Interface["TotSumOCargo"].Length > 0) Document.Cabecera.TOT_SUM_OCARG = decimal.Parse(VD.Interface["TotSumOCargo"], CultureInfo.CreateSpecificCulture("es-PE"));
                if (VD.Interface["TotAnticipo"].Length > 0) Document.Cabecera.ANT_TOT_ANTICIPO = decimal.Parse(VD.Interface["TotAnticipo"], CultureInfo.CreateSpecificCulture("es-PE"));
                if (VD.Interface["TotImporTotal"].Length > 0) Document.Cabecera.TOT_IMPOR_TOTAL = decimal.Parse(VD.Interface["TotImporTotal"], CultureInfo.CreateSpecificCulture("es-PE"));

                // Detracciones
                if (MS.DocumentType == "01")
                {
                    Document.Detracciones.NUMCPE = VD.Interface["NUM_CE"];
                    Document.Detracciones.VALBBSS = VD.Interface["DetValBBSS"];
                    Document.Detracciones.CTABN = VD.Interface["DetCtaBN"];
                    Document.Detracciones.PORCENT = VD.Interface["DetPorcent"];
                    Document.Detracciones.MONTO = VD.Interface["DetMonto"];
                }
            }
            else
            {
                Document.Cabecera.TOT_IMPOR_TOTAL = decimal.Parse(VD.Interface["ImpTotCE"], CultureInfo.CreateSpecificCulture("es-PE"));

                // DATOS-CE
                Document.Cabecera.REGIMENCE = VD.Interface["RegimenCE"];
                Document.Cabecera.TASACE = decimal.Parse(VD.Interface["TasaCE"], CultureInfo.CreateSpecificCulture("es-PE"));
                Document.Cabecera.OBSCE = VD.Interface["ObsCE"];
                Document.Cabecera.IMPTOTCE = decimal.Parse(VD.Interface["ImpTotCE"], CultureInfo.CreateSpecificCulture("es-PE"));
                Document.Cabecera.MONIMPTOTCE = VD.Interface["MonImpTotCE"];
                Document.Cabecera.IMPTOT = decimal.Parse(VD.Interface["ImpTot"], CultureInfo.CreateSpecificCulture("es-PE"));
                Document.Cabecera.MONIMPTOT = VD.Interface["MonImpTot"];
            }


            // Correos
            Document.Correos.NUMCPE = VD.Interface["NUM_CE"];
            Document.Correos.PARA = VD.Interface["Para"];
            Document.Correos.CC = VD.Interface["CC"];
            Document.Correos.CCO = VD.Interface["CO"];

            //ADICIONALES
            Document.Cabecera.SEDE = VD.Interface["Sede"];
            Document.Cabecera.USUARIO = VD.Interface["Usuario"];
            Document.Cabecera.IMPRESORA = VD.Interface["Impresora"];
            Document.Cabecera.CAMPO1 = VD.Interface["campo1"];
            Document.Cabecera.CAMPO2 = VD.Interface["campo2"];
            Document.Cabecera.CAMPO3 = VD.Interface["campo3"];
            Document.Cabecera.CAMPO4 = VD.Interface["campo4"];
            Document.Cabecera.CAMPO5 = VD.Interface["campo5"];
            Document.Cabecera.CAMPO6 = VD.Interface["campo6"];
            Document.Cabecera.CAMPO7 = VD.Interface["campo7"];
            Document.Cabecera.CAMPO8 = VD.Interface["campo8"];
            Document.Cabecera.CAMPO9 = VD.Interface["campo9"];
            Document.Cabecera.CAMPO10 = VD.Interface["campo10"];


            Document.Cabecera.MONTOLITERAL = VD.Interface["MontoLiteral"];


            /*
             * En este método se llenan los objetos correspondientes a cada tabla para
             * que sean almacenados en la bd
             */

            if (MS.DocumentType != "20" && MS.DocumentType != "40")
            {
                for (int i = 0; i < VD.li; i++)
                {
                    int j = i + 1;
                    Detail Linea = new Detail();

                    Linea.IdDetalle = j;
                    Linea.NUM_CPE = VD.Interface["NUM_CE"];
                    Linea.IT_NRO_ORD = VD.Interface["LnNroOrden" + j];
                    Linea.IT_UND_MED = VD.Interface["LnUndMed" + j];
                    if (VD.Interface["LnCantidad" + j].Length > 0) Linea.IT_CANT_ITEM = Convert.ToDouble(VD.Interface["LnCantidad" + j], CultureInfo.CreateSpecificCulture("es-PE")); //decimal.Parse(fDict["ITEM" + (i + 1)][3]); //
                    Linea.IT_COD_PROD = VD.Interface["LnCodProd" + j];
                    Linea.IT_DESCRIP = VD.Interface["LnDescrip" + j];
                    if (VD.Interface["LnValUnit" + j].Length > 0) Linea.IT_VAL_UNIT = decimal.Parse(VD.Interface["LnValUnit" + j], CultureInfo.CreateSpecificCulture("es-PE"));
                    if (VD.Interface["LnMntPrcVta" + j].Length > 0) Linea.IT_MNT_PVTA = decimal.Parse(VD.Interface["LnMntPrcVta" + j], CultureInfo.CreateSpecificCulture("es-PE"));
                    if (VD.Interface["LnValVta" + j].Length > 0) Linea.IT_VAL_VTA = decimal.Parse(VD.Interface["LnValVta" + j], CultureInfo.CreateSpecificCulture("es-PE"));
                    if (VD.Interface["LnMntIGV" + j].Length > 0) Linea.IT_MTO_IGV = decimal.Parse(VD.Interface["LnMntIGV" + j], CultureInfo.CreateSpecificCulture("es-PE"));
                    if (VD.Interface["LnCodAfecIGV" + j].Length > 0) Linea.IT_COD_AFE_IGV = decimal.Parse(VD.Interface["LnCodAfecIGV" + j], CultureInfo.CreateSpecificCulture("es-PE"));
                    if (VD.Interface["LnMntISC" + j].Length > 0) Linea.IT_MTO_ISC = decimal.Parse(VD.Interface["LnMntISC" + j], CultureInfo.CreateSpecificCulture("es-PE"));
                    if (VD.Interface["LnCodSisISC" + j].Length > 0) Linea.IT_SIS_AFE_ISC = decimal.Parse(VD.Interface["LnCodSisISC" + j], CultureInfo.CreateSpecificCulture("es-PE"));
                    if (VD.Interface["LnDescMnto" + j].Length > 0) Linea.IT_DESC_MNTO = decimal.Parse(VD.Interface["LnDescMnto" + j], CultureInfo.CreateSpecificCulture("es-PE"));
                    if (VD.Interface["LnIgvPercentage" + j].Length > 0) Linea.IT_IGV_PERCENTAGE = decimal.Parse(VD.Interface["LnIgvPercentage" + j], CultureInfo.CreateSpecificCulture("es-PE"));

                    Linea.SERIE = sn[0];
                    Linea.NUM_DOCUMENTO = sn[1];

                    Document.Detalles.Add(Linea);
                }

                for (int i = 0; i < VD.re; i++)
                {
                    int j = i + 1;
                    Referencia LReferencia = new Referencia();
                    LReferencia.REF_NROORDEN = VD.Interface["RefNroOrden" + j];
                    LReferencia.REF_ID = VD.Interface["RefID" + j];
                    LReferencia.REF_TPO_DOC = VD.Interface["RefTpoDoc" + j];
                    Document.Referencias.Add(LReferencia);
                }
            }
            else
            {
                for (int i = 0; i < VD.it; i++)
                {
                    int j = i + 1;
                    Detail Item = new Detail();

                    Item.IdDetalle = j;
                    Item.NUM_CPE = VD.Interface["NUM_CE"];
                    Item.IT_NRO_ORD = VD.Interface["ItNroOrden" + j];
                    Item.TPODOCRELAC = VD.Interface["TpoDocRelac" + j];
                    Item.NUMDOCRELAC = VD.Interface["NumDocRelac" + j];
                    Item.FEMISDOCRELAC = DateTime.Parse(VD.Interface["FEmisDocRelac" + j]);
                    Item.ITOTDOCRELAC = decimal.Parse(VD.Interface["ITotDocRelac" + j], CultureInfo.CreateSpecificCulture("es-PE"));
                    Item.MDOCRELAC = VD.Interface["MDocRelac" + j];
                    Item.FECMOVI = DateTime.Parse(VD.Interface["FecMovi" + j]);
                    Item.NUMMOVI = VD.Interface["NumMovi" + j];
                    Item.IMPSOPERMOV = decimal.Parse(VD.Interface["ImpSOperMov" + j], CultureInfo.CreateSpecificCulture("es-PE"));
                    Item.MONMOVI = VD.Interface["MonMovi" + j];
                    Item.IMPOPER = decimal.Parse(VD.Interface["ImpOper" + j], CultureInfo.CreateSpecificCulture("es-PE"));
                    Item.MONIMPOPER = VD.Interface["MonImpOper" + j];
                    try
                    {
                        Item.FECOPER = DateTime.Parse(VD.Interface["FecOper" + j]);
                    }
                    catch (Exception e) { }
                    try
                    {
                        Item.IMPTOTOPER = decimal.Parse(VD.Interface["ImpTotOper" + j], CultureInfo.CreateSpecificCulture("es-PE"));
                    }
                    catch (Exception e) { }
                    try
                    {
                        Item.MONOPER = VD.Interface["MonOper" + j];
                    }
                    catch (Exception e) { }
                    try
                    {
                        Item.MONREFETC = VD.Interface["MonRefeTC" + j];
                    }
                    catch (Exception e) { }
                    try
                    {
                        Item.MONDESTTC = VD.Interface["MonDestTC" + j];
                    }
                    catch (Exception e) { }
                    try
                    {
                        Item.FACTORTC = VD.Interface["FactorTC" + j];
                    }
                    catch (Exception e) { }
                    try
                    {
                        Item.FECHATC = DateTime.Parse(VD.Interface["FechaTC" + j]);
                    }
                    catch (Exception e) { }

                    Item.SERIE = sn[0];
                    Item.NUM_DOCUMENTO = sn[1];

                    Document.Detalles.Add(Item);
                }
            }

            if (MS.DocumentType == "07" || MS.DocumentType == "08")
            {
                for (int i = 0; i < VD.da; i++)
                {
                    int j = i + 1;
                    DocumentoAfectado Afectado = new DocumentoAfectado();
                    Afectado.DocNroOrden = VD.Interface["DocNroOrden" + j];
                    Afectado.DocID = VD.Interface["DocID" + j];
                    Afectado.DocTpoDoc = VD.Interface["DocTpoDoc" + j];

                    Document.DocumentosAfectados.Add(Afectado);
                }
            }
            

            for (int i = 0; i < VD.ex; i++)
            {
                int j = i + 1;
                Extra LExtra = new Extra();
                LExtra.EXLINEA = VD.Interface["ExLinea" + j];
                LExtra.EXDATO = VD.Interface["ExDato" + j];
                LExtra.EXTIPO = VD.Interface["ExTipo" + j];
                Document.Extras.Add(LExtra);
            }

            for (int i = 0; i < VD.an; i++)
            {
                int j = i + 1;
                Anticipo anticipo = new Anticipo();
                anticipo.ANT_NROORDEN = VD.Interface["AntNroOrden" + j];
                anticipo.ANT_MONTO = VD.Interface["AntMonto" + j];
                anticipo.ANT_TPO_DOC_ANT = VD.Interface["AntTpoDocAnt" + j];
                anticipo.ANT_ID_DOC_ANT = VD.Interface["AntIdDocAnt" + j];
                anticipo.ANT_NUM_DOC_EMI = VD.Interface["AntNumDocEmi" + j];
                anticipo.ANT_TPO_DOC_EMI = VD.Interface["AntTpoDocEmi" + j];
                Document.Anticipos.Add(anticipo);
            }

            // El XML Firmado
            Document.Cabecera.VAR_FIR = GetBytes(MS.XMLS);

            return true;
        }
    }
}
