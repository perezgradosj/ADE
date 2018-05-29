using ADE.Configurations.DataAccess;
using ADE.Configurations.Entities;
using ADE.Configurations.Entities.Summaries;
using ADE.Configurations.Objects;
using ADE.Processes.DatabaseIncome;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace eProcessDocumentosAnulados
{
    public class EVoidedDocuments
    {
        /*
         * CLASE Efactura [UBLPE-CreditNote-2.0.cs]
         * ----------------------------------------------------------------------------------
         * Author       :   Johan Espinoza   
         * Date         :   22-07-2015
         * Proyect      :   Facturación Electrónica
         * Description  :   Esta clase simplifica la creación de los objetos pertenecientes a la clase InvoiceType,
         *                  como pueden ser UBLExtensions, AccountingSupplierParty, AccountingCustomerParty, 
         *                  Signature, TaxTotal, UBLVersionID, CustomizationID, etc.
         */

        private Dictionary<string, string[]> fDict = new Dictionary<string, string[]>();
        private Encoding ISO_8859_1 = Encoding.GetEncoding("ISO-8859-1");
        public int IdHEader = 0;
        public RBajasCabecera RAC = new RBajasCabecera();
        public List<RBajasDetalle> RAD = new List<RBajasDetalle>();
        DateTime now = DateTime.Now;
        public string NowDate = "";
        public string DocumentName, RUCNumber, DocumentType, VoidDate;
        DatabaseConnection DBConnection = null;
        public MainSettings MS = null;
        Validation VD = null;
        public DBDocument Document = new DBDocument();
        public Stream SXMLI = null;
        public string XMLI = "";

        public EVoidedDocuments(MainSettings MSI)
        {
            MS = MSI;
            DBConnection = new DatabaseConnection(MSI);
        }

        public void getDocumentName(string date)
        {
            //NowDate = now.Year + "-" + now.Month.ToString().PadLeft(2, '0') + "-" + now.Day.ToString().PadLeft(2, '0');

            NowDate = DateTime.Now.ToString("yyyy-MM-dd");

            NowDate = date;

            MS.SummaryNumber = DBConnection.SP_ObtieneSiguienteResumen(NowDate, MS.DocumentType, MS.RucNumber);
            DocumentName = $"{MS.RucNumber}-{MS.DocumentType}-{NowDate.Replace("-", "")}-{MS.SummaryNumber}";
            
            MS.DocumentName = DocumentName;
        }
        
        public XmlSerializerNamespaces getNamespaces()
        {
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("xsi", "http://www.w3.org/2001/XMLSchema-instance");
            ns.Add("udt", "urn:un:unece:uncefact:data:specification:UnqualifiedDataTypesSchemaModule:2");
            ns.Add("sac", "urn:sunat:names:specification:ubl:peru:schema:xsd:SunatAggregateComponents-1");
            ns.Add("qdt", "urn:oasis:names:specification:ubl:schema:xsd:QualifiedDatatypes-2");
            ns.Add("ds", "http://www.w3.org/2000/09/xmldsig#");
            ns.Add("ext", "urn:oasis:names:specification:ubl:schema:xsd:CommonExtensionComponents-2");
            ns.Add("ccts", "urn:un:unece:uncefact:documentation:2");
            ns.Add("cbc", "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2");
            ns.Add("cac", "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2");
            return ns;
        }

        public XmlWriterSettings getSettings()
        {
            XmlWriterSettings setting = new XmlWriterSettings();
            setting.ConformanceLevel = ConformanceLevel.Auto;
            setting.Indent = true;
            setting.IndentChars = "\t";
            setting.Encoding = ISO_8859_1;
            setting.NamespaceHandling = NamespaceHandling.OmitDuplicates;

            return setting;
        }

        public IDType getID()
        {
            IDType IDT = new IDType();
            IDT.Value = DocumentName.Substring(12);

            return IDT;
        }

        public IssueDateType getIssueDate()
        {
            IssueDateType IDK = new IssueDateType();
            IDK.Value = DateTime.Parse(NowDate);

            return IDK;
        }

        public IssueDateType getIssueDate(DateTime dateReference)
        {
            IssueDateType IDK = new IssueDateType();
            IDK.Value = DateTime.Parse(dateReference.ToString("yyyy-MM-dd"));

            return IDK;
        }

        public ReferenceDateType getReferenceDate()
        {
            ReferenceDateType RDK = new ReferenceDateType();
            RDK.Value = DateTime.Parse(MS.SummaryDate);
            
            return RDK;
        }
        
        public CustomizationIDType getCustomizationID()
        {
            CustomizationIDType CID = new CustomizationIDType();
            CID.Value = "1.0";

            return CID;
        }

        public UBLVersionIDType getUBLVersionID()
        {
            UBLVersionIDType UVer = new UBLVersionIDType();
            UVer.Value = "2.0";

            return UVer;
        }

        public SignatureType[] getSignatureType()
        {
            SignatureType HST = new SignatureType();
            SignatureType[] MST = { HST };

            IDType HIT = new IDType();
            HIT.Value = MS.Emp.Ruc;

            PartyIdentificationType HPIT = new PartyIdentificationType();
            PartyIdentificationType[] MPIT = { HPIT };
            MPIT[0].ID = HIT;

            NameType1 HNT1 = new NameType1();
            PartyNameType HPNT = new PartyNameType();
            PartyNameType[] MPNT = { HPNT };
            MPNT[0] = HPNT;
            HNT1.Value = $"<![CDATA[{MS.Emp.RazonSocial}]]>";

            MPNT[0].Name = HNT1;

            ExternalReferenceType HERT = new ExternalReferenceType();
            AttachmentType HAT_C = new AttachmentType();
            URIType HURIT = new URIType();
            HURIT.Value = "";

            HURIT.Value = $"#signature{MS.Emp.Ruc}";

            HERT.URI = HURIT;
            HAT_C.ExternalReference = HERT;
            PartyType HPT = new PartyType();
            HPT = null;
            HPT = new PartyType();
            HPT.PartyIdentification = MPIT;
            HPT.PartyName = MPNT;

            HIT = null;
            HIT = new IDType();
            HIT.Value = "";

            HIT.Value = $"IDSign{MS.Emp.Ruc}";

            MST[0].ID = HIT;
            MST[0].DigitalSignatureAttachment = HAT_C;
            MST[0].SignatoryParty = HPT;

            return MST;
        }

        public SupplierPartyType getSupplierPartyType()
        {
            PartyType HPT = new PartyType();

            NameType1 HNT1 = new NameType1();
            HNT1.Value = $"<![CDATA[{MS.Emp.RazonSocial}]]>";
            
            PartyNameType HPNT = new PartyNameType();
            PartyNameType[] MPNT = { HPNT };
            MPNT[0].Name = HNT1;
            
            IDType HIT = new IDType();
            AddressType HAT = new AddressType();
            AddressType[] MAT = { HAT };
            HIT.Value = MS.Emp.Ubigeo;

            StreetNameType HSNT = new StreetNameType();
            HSNT.Value = MS.Emp.DomicilioFiscal;

            CountryType HCT = new CountryType();
            IdentificationCodeType HICT = new IdentificationCodeType();
            HICT.listID = "PE";
            HCT.IdentificationCode = HICT;
            PartyLegalEntityType HPLET = new PartyLegalEntityType();
            PartyLegalEntityType[] MPLET = { HPLET };
            RegistrationNameType HRNT = new RegistrationNameType();
            HRNT.Value = $"<![CDATA[{MS.Emp.RazonSocial}]]>";

            MPLET[0].RegistrationName = HRNT;
            HAT.ID = HIT;
            HAT.StreetName = HSNT;

            HAT.Country = HCT;
            HPT.PartyLegalEntity = MPLET;

            AdditionalAccountIDType HAAIT = new AdditionalAccountIDType();
            AdditionalAccountIDType[] MAAIT = { HAAIT };

            MAAIT[0].Value = "6";

            CustomerAssignedAccountIDType CAAIDT = new CustomerAssignedAccountIDType();
            
            CAAIDT.Value = MS.Emp.Ruc;

            SupplierPartyType HSPT = new SupplierPartyType();
            HSPT.Party = HPT;
            HSPT.AdditionalAccountID = MAAIT;
            HSPT.CustomerAssignedAccountID = CAAIDT;

            return HSPT;
        }
        
        public UBLExtensionType[] getUBLExtensions()
        {
            UBLExtensionType HUBLET = new UBLExtensionType();
            UBLExtensionType[] MUBLET = { null, null, null };


            string EXT1 = "";

            HUBLET.ExtensionContent = EXT1;
            MUBLET[0] = HUBLET;

            return MUBLET;
        }

        public VoidedDocumentsLineType[] getVoidedDocumentsLine()
        {
            VoidedDocumentsLineType HILT = new VoidedDocumentsLineType();
            VoidedDocumentsLineType[] MILT = new VoidedDocumentsLineType[RAD.Count];
            MS.GeneraLog.IngresoBD01("Se añaden a la cola los siguientes documentos, para luego actualiza su estado");
            for (int i = 0; i < RAD.Count; ++i)
            {
                RAC.DOC_UPD_LIST.Add($"{MS.RucNumber}-{RAD[i].TPO_CPE}-{RAD[i].DOC_SER}-{RAD[i].DOC_NUM.ToString().PadLeft(8, '0')}");
                MS.GeneraLog.IngresoBD01($"{MS.RucNumber}-{RAD[i].TPO_CPE}-{RAD[i].DOC_SER}-{RAD[i].DOC_NUM.ToString().PadLeft(8, '0')}");
                MILT[i] = new VoidedDocumentsLineType();

                LineIDType LIT = new LineIDType();
                LIT.Value = (i + 1).ToString();
                MILT[i].LineID = LIT;

                DocumentTypeCodeType DTCT = new DocumentTypeCodeType();
                DTCT.Value = RAD[i].TPO_CPE;
                MILT[i].DocumentTypeCode = DTCT;

                IdentifierType IT1 = new IdentifierType();
                IT1.Value = RAD[i].DOC_SER;
                MILT[i].DocumentSerialID = IT1;

                IdentifierType IT2 = new IdentifierType();
                IT2.Value = RAD[i].DOC_NUM.ToString().PadLeft(8,'0');
                MILT[i].DocumentNumberID = IT2;

                TextType TT = new TextType();
                TT.Value = RAD[i].DOC_DES;
                MILT[i].VoidReasonDescription = TT;
            }
            return MILT;
        }

        public XmlElement getSerialize(object transformObject)
        {
            XmlElement serializedElement = null;
            try
            {
                MemoryStream memStream = new MemoryStream();
                XmlSerializer serializer = new XmlSerializer(transformObject.GetType());
                serializer.Serialize(memStream, transformObject);
                memStream.Position = 0;
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(memStream);
                //xmlDoc.NameTable = ns|
                serializedElement = xmlDoc.DocumentElement;
            }
            catch (Exception SerializeException)
            {
            }
            return serializedElement;
        }
        
        public bool InsertInDatabase()
        {
            bool insert = true;
            MS.GeneraLog.IngresoBD01("---------- INICIO DE INGRESO A BD ----------");
            if (RAD.Count > 0)
            {
                MS.GeneraLog.IngresoBD01("Nombre del Archivo a guardar: " + MS.FileLocation);

                NowDate = now.Year + "-" + now.Month.ToString().PadLeft(2, '0') + "-" + now.Day.ToString().PadLeft(2, '0');
                string fechacadena = now.Day.ToString().PadLeft(2, '0') + "/" + now.Month.ToString().PadLeft(2, '0') + "/" + now.Year.ToString();
                try
                {

                    string[] sec = DocumentName.Split('-');
                    RAC.NUM_CPE = DocumentName;
                    RAC.NUM_SEC = int.Parse(sec[sec.Length - 1]);
                    RAC.FEC_ENV = now;
                    RAC.FEC_REF = DateTime.Parse(NowDate);
                    RAC.FEC_ANU = DateTime.Parse(VoidDate);
                    RAC.FEC_CAD = fechacadena + " " + (((now.TimeOfDay.Hours + 11) % 12) + 1).ToString().PadLeft(2, '0') + ":" + now.TimeOfDay.Minutes.ToString().PadLeft(2, '0') + ":" + now.TimeOfDay.Seconds.ToString().PadLeft(2, '0') + " " + now.ToString("tt", CultureInfo.InvariantCulture);
                    RAC.TIPO = DocumentType;
                    IdHEader = DBConnection.SP_InsertaCabeceraRA(RAC);
                    RAC.ID_RAC = IdHEader;
                    if (IdHEader > 0)
                    {
                        string IdDetail = DBConnection.SP_InsertaDetalleRA(RAD, RAC);
                        if (IdDetail != "1")
                        {
                            insert = false;
                            MS.GeneraLog.RegistraError("");
                        }
                    }
                    else
                    {
                        insert = false;
                        MS.GeneraLog.RegistraError("");
                    }
                }
                catch (Exception e)
                {
                    insert = false;
                    MS.GeneraLog.IngresoBD01("Error al ingresar a Base de Datos: " + e.Message);
                    MS.GeneraLog.RegistraError("");
                }
            }
            MS.GeneraLog.IngresoBD01("---------- FIN   DE INGRESO A BD ----------");
            return insert;
        }

        public CurrencyCodeContentType getCurr(string curr)
        {
            CurrencyCodeContentType c = CurrencyCodeContentType.AED;

            if (curr == "PEN") c = CurrencyCodeContentType.PEN;
            if (curr == "EUR") c = CurrencyCodeContentType.EUR;
            if (curr == "USD") c = CurrencyCodeContentType.USD;

            return c;
        }

        public bool GenerateXML()
        {
            bool isGenerated = false;
            RAD = DBConnection.ObtieneResumenAnulados();

            if(RAD.Count == 0)
            {
                MS.GeneraLog.IngresoBD01($"No se obtuvieron documentos anulados de la fecha {MS.SummaryDate}");
                return false;
            }

            RAC.TOT_DOC = RAD.Count;

            //getDocumentName(RAC.FEC_REF.ToString("yyyy-MM-dd"));
            getDocumentName(Convert.ToDateTime(MS.SummaryDate).ToString("yyyy-MM-dd"));

            MS.getFileNames();
            
            try
            {
                MS.GeneraLog.GeneracionXML("-----  Inicio de Generación de Archivo: " + MS.XMLIFileLocation);
                XmlSerializer mySerializer = new XmlSerializer(typeof(VoidedDocumentsType));
                XmlSerializerNamespaces ns = getNamespaces();
                XmlWriterSettings setting = getSettings();

                VoidedDocumentsType voi = new VoidedDocumentsType();

                voi.AccountingSupplierParty = getSupplierPartyType();
                voi.Signature = getSignatureType();
                voi.UBLVersionID = getUBLVersionID();
                voi.CustomizationID = getCustomizationID();
                voi.ID = getID();
                //voi.IssueDate = getIssueDate();
                voi.ReferenceDate = getReferenceDate();
                voi.IssueDate = getIssueDate(voi.ReferenceDate.Value);

                voi.VoidedDocumentsLine = getVoidedDocumentsLine();
                voi.UBLExtensions = getUBLExtensions();

                StringWriterWithEncoding iso88591StringWriter = new StringWriterWithEncoding(ISO_8859_1);
                XmlSerializer serializer = new XmlSerializer(typeof(VoidedDocumentsType));
                XmlWriter writerx = XmlWriter.Create(iso88591StringWriter, setting);
                serializer.Serialize(writerx, voi, ns);

                XMLI = iso88591StringWriter.ToString();

                XMLI = XMLI.Replace("&lt;", "<");
                XMLI = XMLI.Replace("&gt;", ">");
                XMLI = XMLI.Replace("&amp;", "&");

                using (StreamWriter sw = new StreamWriter(MS.XMLIFileLocation, false, ISO_8859_1))
                {
                    sw.Write(XMLI);
                }

                SXMLI = GenerateStreamFromString(XMLI);

                isGenerated = true;
            }
            catch (Exception ex)
            {
                MS.GeneraLog.GeneracionXML("Ocurrió un error al momento de generar el XML Inicial");
                MS.GeneraLog.GeneracionXML("Error: " + ex.Message);
            }
            return isGenerated;
        }
        
        public sealed class StringWriterWithEncoding : StringWriter
        {
            private readonly Encoding encoding;

            public StringWriterWithEncoding(Encoding encoding)
            {
                this.encoding = encoding;
            }

            public override Encoding Encoding
            {
                get { return encoding; }
            }
        }

        public Stream GenerateStreamFromString(string s)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        public bool isDecimal(string value)
        {
            decimal number;
            if (Decimal.TryParse(value, out number))
                return true;
            else
                return false;
        }

        public bool isDate(string value)
        {
            DateTime fecha;
            if (DateTime.TryParse(value, out fecha))
                return true;
            else
                return false;
        }
    }
}
