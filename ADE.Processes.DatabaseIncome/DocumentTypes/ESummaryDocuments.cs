using ADE.Configurations.DataAccess;
using ADE.Configurations.Entities;
using ADE.Configurations.Entities.Database;
using ADE.Configurations.Entities.Summaries;
using ADE.Configurations.Objects;
using ADE.Processes.DatabaseIncome;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace eProcessResumenDocumentos
{
    public class ESummaryDocuments
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
        public RBoletasCabecera RBC = new RBoletasCabecera();
        public List<RBoletasDetalle> RBD = new List<RBoletasDetalle>();
        DateTime now = DateTime.Now;
        public string NowDate = "";
        public string DocumentName, RUCNumber, DocumentType, VoidDate;
        DatabaseConnection DBConnection = null;
        public MainSettings MS = null;
        Validation VD = null;
        public DBDocument Document = new DBDocument();
        public Stream SXMLI = null;
        public string XMLI = "";

        public ESummaryDocuments(MainSettings MSI)
        {
            MS = MSI;
            DBConnection = new DatabaseConnection(MSI);
        }

        public void getDocumentName()
        {
            NowDate = now.Year + "-" + now.Month.ToString().PadLeft(2, '0') + "-" + now.Day.ToString().PadLeft(2, '0');
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

        public NoteType[] getNote()
        {
            NoteType[] IDT = new NoteType[1];
            IDT[0] = new NoteType();
            IDT[0].Value = $"<![CDATA[CONSOLIDADO DE BOLETAS DE VENTA - {MS.Emp.RazonSocial}]]>"; ;
            return IDT;
        }

        public IssueDateType getIssueDate()
        {
            IssueDateType IDK = new IssueDateType();
            IDK.Value = DateTime.Parse(NowDate);

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

        public SummaryDocumentsLineType[] getSummaryDocumentsLine()
        {
            SummaryDocumentsLineType HILT = new SummaryDocumentsLineType();
            SummaryDocumentsLineType[] MILT = new SummaryDocumentsLineType[RBD.Count];

            for (int i = 0; i < RBD.Count; ++i)
            {                
                MILT[i] = new SummaryDocumentsLineType();

                LineIDType LIT = new LineIDType();
                LIT.Value = (i + 1).ToString();
                MILT[i].LineID = LIT;

                DocumentTypeCodeType DTCT = new DocumentTypeCodeType();
                DTCT.Value = RBD[i].TPO_CPE;
                MILT[i].DocumentTypeCode = DTCT;

                IdentifierType IT = new IdentifierType();
                IT.Value = RBD[i].DOC_SER;
                MILT[i].DocumentSerialID = IT;

                IT = null; IT = new IdentifierType();
                IT.Value = RBD[i].NUM_INI.ToString();
                MILT[i].StartDocumentNumberID = IT;

                IT = null; IT = new IdentifierType();
                IT.Value = RBD[i].NUM_FIN.ToString();
                MILT[i].EndDocumentNumberID = IT;

                AmountType1 AT1 = new AmountType1();
                AT1.Value = RBD[i].MTO_TOT;
                AT1.currencyID = CurrencyCodeContentType.PEN;
                MILT[i].TotalAmount = AT1;


                // START - IMPORTES
                PaymentType[] PT = { null, null, null };
                PaidAmountType PAT; InstructionIDType IIT;



                // IMPORTES - GRAVADOS
                var ImpGravados = Convert.ToDecimal(RBD[i].MTO_GRA, CultureInfo.CreateSpecificCulture("es-PE"));
                if(ImpGravados > 0)
                {
                    PAT = new PaidAmountType();
                    IIT = new InstructionIDType();
                    PT[0] = new PaymentType();
                    PAT.Value = Convert.ToDecimal(RBD[i].MTO_GRA, CultureInfo.CreateSpecificCulture("es-PE"));
                    PAT.currencyID = CurrencyCodeContentType.PEN;
                    IIT.Value = "01";
                    PT[0].PaidAmount = PAT;
                    PT[0].InstructionID = IIT;
                    PAT = null; IIT = null;
                }

                // IMPORTES - EXONERADOS
                var ImpExonerado = Convert.ToDecimal(RBD[i].MTO_EXO, CultureInfo.CreateSpecificCulture("es-PE"));
                if(ImpExonerado > 0)
                {
                    PAT = new PaidAmountType();
                    IIT = new InstructionIDType();
                    PT[1] = new PaymentType();
                    PAT.Value = Convert.ToDecimal(RBD[i].MTO_EXO, CultureInfo.CreateSpecificCulture("es-PE"));
                    PAT.currencyID = CurrencyCodeContentType.PEN;
                    IIT.Value = "02";
                    PT[1].PaidAmount = PAT;
                    PT[1].InstructionID = IIT;
                    PAT = null; IIT = null;
                }


                // IMPORTES - INAFECTO
                var ImpInafecto = Convert.ToDecimal(RBD[i].MTO_INA, CultureInfo.CreateSpecificCulture("es-PE"));
                if(ImpInafecto > 0)
                {
                    PAT = new PaidAmountType();
                    IIT = new InstructionIDType();
                    PT[2] = new PaymentType();
                    PAT.Value = Convert.ToDecimal(RBD[i].MTO_INA, CultureInfo.CreateSpecificCulture("es-PE"));
                    PAT.currencyID = CurrencyCodeContentType.PEN;
                    IIT.Value = "03";
                    PT[2].PaidAmount = PAT;
                    PT[2].InstructionID = IIT;
                    PAT = null; IIT = null;
                    MILT[i].BillingPayment = PT;
                }

                // IMPORTES - OTROS CARGOS
                var ImpOtroCargos = Convert.ToDecimal(RBD[i].MTO_OCA, CultureInfo.CreateSpecificCulture("es-PE"));
                if(ImpOtroCargos > 0)
                {
                    AllowanceChargeType[] ACT = { null };
                    ACT[0] = new AllowanceChargeType();

                    ChargeIndicatorType CIT = new ChargeIndicatorType();
                    CIT.Value = true;
                    ACT[0].ChargeIndicator = CIT;

                    AT1 = null; AT1 = new AmountType1();
                    AT1.Value = Convert.ToDecimal(RBD[i].MTO_OCA, CultureInfo.CreateSpecificCulture("es-PE"));
                    AT1.currencyID = CurrencyCodeContentType.PEN;
                    ACT[0].Amount = AT1;
                    MILT[i].AllowanceCharge = ACT;
                }
                // END - IMPORTES

                // START - TAXTOTAL
                TaxTotalType[] TTP = { null, null, null };
                TaxAmountType TAT = null;
                TaxSubtotalType[] TST = { null };
                TaxSubtotalType[] TST2 = { null };
                TaxSubtotalType[] TST3 = { null };
                TaxCategoryType TCT = null;
                TaxSchemeType THT = null;
                IDType IDT = null;
                NameType1 TNT1 = null;
                TaxTypeCodeType TTCT = null;


                // TOTAL IGV
                IDT = new IDType();
                IDT.Value = "1000";
                TNT1 = new NameType1();
                TNT1.Value = "IGV";
                TTCT = new TaxTypeCodeType();
                TTCT.Value = "VAT";

                THT = new TaxSchemeType();
                THT.ID = IDT;
                THT.Name = TNT1;
                THT.TaxTypeCode = TTCT;

                TCT = new TaxCategoryType();
                TCT.TaxScheme = THT;

                TAT = new TaxAmountType();
                TAT.currencyID = CurrencyCodeContentType.PEN;
                TAT.Value = Convert.ToDecimal(RBD[i].IMP_IGV, CultureInfo.CreateSpecificCulture("es-PE"));

                TST[0] = new TaxSubtotalType();
                TST[0].TaxAmount = TAT;
                TST[0].TaxCategory = TCT;

                TTP[0] = new TaxTotalType();
                TTP[0].TaxAmount = TAT;
                TTP[0].TaxSubtotal = TST;

                TAT = null; TST = null; TCT = null; THT = null; IDT = null; TNT1 = null; TTCT = null;

                // TOTAL ISC
                IDT = new IDType();
                IDT.Value = "2000";
                TNT1 = new NameType1();
                TNT1.Value = "ISC";
                TTCT = new TaxTypeCodeType();
                TTCT.Value = "EXC";

                THT = new TaxSchemeType();
                THT.ID = IDT;
                THT.Name = TNT1;
                THT.TaxTypeCode = TTCT;

                TCT = new TaxCategoryType();
                TCT.TaxScheme = THT;

                TAT = new TaxAmountType();
                TAT.currencyID = CurrencyCodeContentType.PEN;
                TAT.Value = Convert.ToDecimal(RBD[i].IMP_ISC, CultureInfo.CreateSpecificCulture("es-PE"));

                TST2[0] = new TaxSubtotalType();
                TST2[0].TaxAmount = TAT;
                TST2[0].TaxCategory = TCT;

                TTP[1] = new TaxTotalType();
                TTP[1].TaxAmount = TAT;
                TTP[1].TaxSubtotal = TST2;

                TAT = null; TST = null; TCT = null; THT = null; IDT = null; TNT1 = null; TTCT = null;

                // TOTAL OTH
                IDT = new IDType();
                IDT.Value = "9999";
                TNT1 = new NameType1();
                TNT1.Value = "OTROS";
                TTCT = new TaxTypeCodeType();
                TTCT.Value = "OTH";

                THT = new TaxSchemeType();
                THT.ID = IDT;
                THT.Name = TNT1;
                THT.TaxTypeCode = TTCT;

                TCT = new TaxCategoryType();
                TCT.TaxScheme = THT;

                TAT = new TaxAmountType();
                TAT.currencyID = CurrencyCodeContentType.PEN;
                TAT.Value = Convert.ToDecimal(RBD[i].IMP_OTH, CultureInfo.CreateSpecificCulture("es-PE"));

                TST3[0] = new TaxSubtotalType();
                TST3[0].TaxAmount = TAT;
                TST3[0].TaxCategory = TCT;

                TTP[2] = new TaxTotalType();
                TTP[2].TaxAmount = TAT;
                TTP[2].TaxSubtotal = TST3;

                TAT = null; TST = null; TCT = null; THT = null; IDT = null; TNT1 = null; TTCT = null;

                MILT[i].TaxTotal = TTP;
                // END - TAXTOTAL
            }
            return MILT;
        }

        public List<RBoletasDetalle> ordenaResumen()
        {
            List<ResumenB> DBList = new List<ResumenB>();
            
            DBList = DBConnection.SP_ObtieneResumenBoletas();
            string sa = "", ta = "";
            int na = 0, sl = 0;

            RBoletasDetalle RBL = null;
            RBC = new RBoletasCabecera();

            RBC.TOT_DOC = DBList.Count;
            RBD = new List<RBoletasDetalle>();
            for (int i = 0; i < DBList.Count; ++i)
            {
                RBC.DOC_UPD_LIST.Add($"{MS.RucNumber}-{DBList[i].TPODOC}-{DBList[i].SERIE}-{DBList[i].NUMERO.ToString().PadLeft(8,'0')}");
                if (i == 0 || RBL == null)
                {
                    if (RBL == null)
                    {
                        RBL = new RBoletasDetalle();
                    }
                    sa = DBList[i].SERIE;
                    na = DBList[i].NUMERO - 1;
                    ta = DBList[i].TPODOC;
                    
                    // NUEVO MÉTODO
                    RBL.TPO_CPE = DBList[i].TPODOC;
                    RBL.NUM_INI = DBList[i].NUMERO;
                    RBL.NUM_FIN = DBList[i].NUMERO;

                    RBC.FEC_INI = DateTime.Parse(MS.SummaryDate);
                    RBC.FEC_FIN = DateTime.Parse(MS.SummaryDate);
                }
                if (sa == DBList[i].SERIE && (na + 1) == DBList[i].NUMERO && ta == DBList[i].TPODOC)
                {
                    /*
                     * NUEVO MÉTODO
                     */
                    RBL.NRO_LIN = RBD.Count + 1;
                    RBL.TPO_CPE = DBList[i].TPODOC;
                    RBL.DOC_SER = DBList[i].SERIE;
                    RBL.NUM_FIN = DBList[i].NUMERO;

                    // IMPORTES

                    RBL.MTO_GRA = RBL.MTO_GRA + DBList[i].GRAVADO;
                    RBL.MTO_EXO = RBL.MTO_EXO + DBList[i].EXONERADO;
                    RBL.MTO_INA = RBL.MTO_INA + DBList[i].INAFECTO;
                    RBL.MTO_OCA = RBL.MTO_OCA + DBList[i].OCARGOS;


                    // IMPUESTOS
                    RBL.IMP_IGV = RBL.IMP_IGV + DBList[i].IGV;
                    RBL.IMP_ISC = RBL.IMP_ISC + DBList[i].ISC;
                    RBL.IMP_OTH = RBL.IMP_OTH + DBList[i].OTH;

                    // TOTAL
                    RBL.MTO_TOT = RBL.MTO_TOT + DBList[i].TOTAL;

                    RBC.MTO_GRA = RBC.MTO_GRA + DBList[i].GRAVADO;
                    RBC.MTO_EXO = RBC.MTO_EXO + DBList[i].EXONERADO;
                    RBC.MTO_INA = RBC.MTO_INA + DBList[i].INAFECTO;
                    RBC.MTO_OCA = RBC.MTO_OCA + DBList[i].OCARGOS;

                    RBC.IMP_IGV = RBC.IMP_IGV + DBList[i].IGV;
                    RBC.IMP_ISC = RBC.IMP_ISC + DBList[i].ISC;
                    RBC.IMP_OTH = RBC.IMP_OTH + DBList[i].OTH;

                    RBC.MTO_TOT = RBC.MTO_TOT + DBList[i].TOTAL;

                    sl++; na = DBList[i].NUMERO;
                }
                else
                {
                    /*
                     * NUEVO MÉTODO
                     */
                    RBD.Add(RBL);
                    RBL = null;
                    RBL = new RBoletasDetalle();

                    RBL.NRO_LIN = RBD.Count + 1;
                    RBL.TPO_CPE = DBList[i].TPODOC;
                    RBL.DOC_SER = DBList[i].SERIE;
                    RBL.NUM_INI = DBList[i].NUMERO;
                    RBL.NUM_FIN = DBList[i].NUMERO;

                    // IMPORTES

                    RBL.MTO_GRA = RBL.MTO_GRA + DBList[i].GRAVADO;
                    RBL.MTO_EXO = RBL.MTO_EXO + DBList[i].EXONERADO;
                    RBL.MTO_INA = RBL.MTO_INA + DBList[i].INAFECTO;
                    RBL.MTO_OCA = RBL.MTO_OCA + DBList[i].OCARGOS;

                    // IMPUESTOS
                    RBL.IMP_IGV = RBL.IMP_IGV + DBList[i].IGV;
                    RBL.IMP_ISC = RBL.IMP_ISC + DBList[i].ISC;
                    RBL.IMP_OTH = RBL.IMP_OTH + DBList[i].OTH;

                    // TOTAL
                    RBL.MTO_TOT = RBL.MTO_TOT + DBList[i].TOTAL;

                    RBC.MTO_GRA = RBC.MTO_GRA + DBList[i].GRAVADO;
                    RBC.MTO_EXO = RBC.MTO_EXO + DBList[i].EXONERADO;
                    RBC.MTO_INA = RBC.MTO_INA + DBList[i].INAFECTO;
                    RBC.MTO_OCA = RBC.MTO_OCA + DBList[i].OCARGOS;

                    RBC.IMP_IGV = RBC.IMP_IGV + DBList[i].IGV;
                    RBC.IMP_ISC = RBC.IMP_ISC + DBList[i].ISC;
                    RBC.IMP_OTH = RBC.IMP_OTH + DBList[i].OTH;

                    RBC.MTO_TOT = RBC.MTO_TOT + DBList[i].TOTAL;

                    sl++;
                    sa = DBList[i].SERIE;
                    na = DBList[i].NUMERO;
                    ta = DBList[i].TPODOC;
                }
                if (i + 1 == DBList.Count)
                {
                    RBD.Add(RBL); RBL = null;
                }
            }
            return RBD;
        }

        public bool GenerateXML()
        {
            bool isGenerated = false;
            InsertaTC();
            RBD = ordenaResumen();
            if (RBD.Count == 0)
            {
                MS.GeneraLog.IngresoBD01($"No se obtuvieron boletas y/o documentos asociados de la fecha {MS.SummaryDate}");
                return false;
            }
            else
            {
                if (!DBConnection.ExchangueRateExists && DBConnection.DocumentDolarExists)
                {
                    MS.GeneraLog.IngresoBD01($"No se ha configurado el tipo de cambio para el día {MS.SummaryDate}");
                    return false;
                }
            }
            
            getDocumentName();
            MS.getFileNames();

            try
            {
                MS.GeneraLog.GeneracionXML("-----  Inicio de Generación de Archivo: " + MS.XMLIFileLocation);
                XmlSerializer mySerializer = new XmlSerializer(typeof(SummaryDocumentsType));
                XmlSerializerNamespaces ns = getNamespaces();
                XmlWriterSettings setting = getSettings();

                SummaryDocumentsType sum = new SummaryDocumentsType();

                sum.AccountingSupplierParty = getSupplierPartyType();
                sum.Signature = getSignatureType();
                sum.UBLVersionID = getUBLVersionID();
                sum.CustomizationID = getCustomizationID();
                sum.ID = getID();
                sum.IssueDate = getIssueDate();
                sum.ReferenceDate = getReferenceDate();
                sum.Note = getNote();
                sum.SummaryDocumentsLine = getSummaryDocumentsLine();
                sum.UBLExtensions = getUBLExtensions();

                StringWriterWithEncoding iso88591StringWriter = new StringWriterWithEncoding(ISO_8859_1);
                XmlSerializer serializer = new XmlSerializer(typeof(SummaryDocumentsType));
                XmlWriter writerx = XmlWriter.Create(iso88591StringWriter, setting);
                serializer.Serialize(writerx, sum, ns);

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

        public void InsertaTC()
        {
            try
            {
                string TC = GetTC(MS.SummaryDate);
                if (double.Parse(TC) > 0)
                {
                    MS.SP_InsertaTC(MS.RucNumber, MS.SummaryDate, "USD", TC);
                    MS.GeneraLog.GeneracionXML($"Tipo de Cambio insertado Correctamente");
                }
                else
                {
                    MS.GeneraLog.GeneracionXML($"No se pudo obtener el Tipo de Cambio");
                }
            }
            catch (Exception e)
            {
                MS.GeneraLog.GeneracionXML($"Error al consultar/Insertar el Tipo de Cambio : {e.Message}");
            }
            
        }

        public string GetTC(string SummaryDate)
        {
            string venta = "0.00000";
            string[] Fec = SummaryDate.Split('-');
            int pos = -1;
            HtmlAgilityPack.HtmlWeb web = new HtmlAgilityPack.HtmlWeb();
            HtmlAgilityPack.HtmlDocument doc = web.Load("http://www.sunat.gob.pe/cl-at-ittipcam/tcS01Alias?mes=" + Fec[1] + "&anho=" + Fec[0]);
            int j = 0;
            List<string> D = new List<string>();
            List<string> C = new List<string>();
            List<string> V = new List<string>();

            try
            {
                var Days = doc.DocumentNode.SelectNodes("//td[@class='H3']").ToList();
                var Rate = doc.DocumentNode.SelectNodes("//td[@class='tne10']").ToList();

                for (int i = 0; i < Days.Count; ++i)
                {
                    D.Add(Days[i].InnerText.Trim());
                    C.Add(Rate[j].InnerText.Trim());
                    V.Add(Rate[j + 1].InnerText.Trim());
                    j = j + 2;
                    if (int.Parse(Days[i].InnerText.Trim()) <= int.Parse(Fec[2]))
                    {
                        pos = i;
                    }
                    else
                    {
                        if (i == 0)
                        {
                            pos = -1;
                        }
                        i = Days.Count + 1;
                    }
                }
                venta = V[pos];
            }
            catch (Exception e)
            {
                try
                {
                    //var Warn = doc.DocumentNode.SelectNodes("//font[@class='warn']").ToList();
                    //if(Warn.Count > 0)
                    if (pos == -1)
                    {
                        Fec = GetPastMonth(SummaryDate);
                        doc = web.Load("http://www.sunat.gob.pe/cl-at-ittipcam/tcS01Alias?mes=" + Fec[1] + "&anho=" + Fec[0]);
                        var Days = doc.DocumentNode.SelectNodes("//td[@class='H3']").ToList();
                        var Rate = doc.DocumentNode.SelectNodes("//td[@class='tne10']").ToList();
                        j = 0;
                        D = null; D = new List<string>();
                        C = null; C = new List<string>();
                        V = null; V = new List<string>();

                        for (int i = 0; i < Days.Count; ++i)
                        {
                            D.Add(Days[i].InnerText.Trim());
                            C.Add(Rate[j].InnerText.Trim());
                            V.Add(Rate[j + 1].InnerText.Trim());
                            j = j + 2;
                        }
                        pos = V.Count - 1;
                        venta = V[pos];
                    }
                }
                catch (Exception d)
                {

                }
            }
            return venta;
        }

        public string[] GetPastMonth(string SummaryDate)
        {
            string[] res;
            string[] Fec = SummaryDate.Split('-');
            if (Fec[1] == "01")
            {
                Fec[0] = ("" + (int.Parse(Fec[0]) - 1));
                Fec[1] = "12";
            }
            else
            {
                Fec[1] = ("" + (int.Parse(Fec[1]) - 1)).PadLeft(2, '0'); ;
            }
            res = Fec;
            return res;
        }
    }

    [DataContract]
    public class TipoCambio
    {
        [DataMember]
        public string dias { get; set; }

        [DataMember]
        public string compra { get; set; }

        [DataMember]
        public string venta { get; set; }
    }
}
