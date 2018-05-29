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

using eProcessResumenDocumentos;

namespace ADE.Processes.DatabaseIncome.DocumentTypesNew
{
    public class ESummaryDocumentsNew
    {
        /*
        * CLASE Efactura [UBLPE-CreditNote-2.0.cs]
        * ----------------------------------------------------------------------------------
        * Author       :   Josué Puma, nyxjosue@gmail.com
        * Date         :   27-11-2017
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

        private Extras.Common.Method.ListUtilClass _listDocs;
        public Extras.Common.Method.ListUtilClass ListDocs
        {
            get { return _listDocs; }
            set { _listDocs = value; }
        }

        public ESummaryDocumentsNew(MainSettings MSI)
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
            CID.Value = "1.1";

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

            //HURIT.Value = $"#signature{MS.Emp.Ruc}";
            HURIT.Value = DocumentName.Substring(12);

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

            //HIT.Value = $"IDSign{MS.Emp.Ruc}";
            HIT.Value = DocumentName.Substring(12);

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

        public SummaryDocumentsLineType[] getSummaryDocumentsLine(int idStatusDC, string statusADE, string typerc)
        {
            SummaryDocumentsLineType HILT = new SummaryDocumentsLineType();
            SummaryDocumentsLineType[] MILT = new SummaryDocumentsLineType[RBD.Count];
            //ListDocs = new Extras.Common.Method.ListUtilClass();

            var list = new List<string>();
            for (int i = 0; i < RBD.Count; ++i)
            {
                HILT_s = new SummaryDocumentsLineType();
                HILT_s = GetLineRC(RBD[i], i, typerc);
                MILT[i] = HILT_s;
            }
            return MILT;
        }



        bool detOk = false;
        SummaryDocumentsLineType HILT_s = new SummaryDocumentsLineType();

        private SummaryDocumentsLineType GetLineRC(RBoletasDetalle det, int i, string typerc)
        {
            detOk = false;
            HILT_s = new SummaryDocumentsLineType();

            try
            {
                #region alter js

                LineIDType LIT = new LineIDType();
                LIT.Value = (i + 1).ToString();

                HILT_s.LineID = LIT;

                //tipo de documento (03, 07, 08)
                DocumentTypeCodeType DTCT = new DocumentTypeCodeType();
                DTCT.Value = RBD[i].TPO_CPE;
                HILT_s.DocumentTypeCode = DTCT;

                #region condition code

                StatusType StatusTP = new StatusType();
                ConditionCodeType Condition = new ConditionCodeType();
                RBD[i].ConditionCode = 0;

                if (RBD[i].ConditionCode == 0)
                {
                    if (typerc != "ANS")
                    {
                        Condition.Value = "1";
                    }
                    else
                    {
                        Condition.Value = "3";
                        #region other
                        //switch (RBD[i].SYSTEM_STATUS)
                        //{
                        //    case "XGN":
                        //        {
                        //            //Condition.Value = "1";
                        //            break;
                        //        }
                        //    case "ANS":
                        //        {
                        //            Condition.Value = "3";
                        //            //var res = ADE.Extras.Common.Method.Methods.Instance.DateCompare(DateTime.Now.ToString("yyyy-MM-dd"), RBD[i].FEC_EMIS.ToString("yyyy-MM-dd"));
                        //            //if (res == 1)
                        //            //{ Condition.Value = "3"; }
                        //            //else
                        //            ////{ Condition.Value = "4"; }
                        //            //{ Condition.Value = "3"; }
                        //            break;
                        //        }
                        //}
                        #endregion
                    }
                    ListDocs.Add(new Extras.Common.Method.UtilClass() { NUM_CE = RBD[i].NUM_CPE, STATUS_RC_DOC = int.Parse(Condition.Value) });
                }


                StatusTP.ConditionCode = Condition;
                HILT_s.Status = new StatusType();
                HILT_s.Status = StatusTP;
                #endregion

                //ID (xxxx-xxxxxxxx)
                IDType IdDocumentType = new IDType();
                IdDocumentType.Value = RBD[i].ID;
                HILT_s.ID = new IDType();
                HILT_s.ID = IdDocumentType;

                //added js
                AmountType1 TotAmount = new AmountType1();
                TotAmount.Value = RBD[i].MTO_TOT;
                TotAmount.currencyID = CurrencyCodeContentType.PEN;
                TotAmount.currencyID = RBD[i].MONEDA.ToUpper() == "USD" ? CurrencyCodeContentType.USD : CurrencyCodeContentType.PEN;
                HILT_s.TotalAmount = TotAmount;//total documento 

                CustomerPartyType CPT = new CustomerPartyType();
                CPT.CustomerAssignedAccountID = new CustomerAssignedAccountIDType();
                CPT.CustomerAssignedAccountID.Value = RBD[i].CustomerAssignedAccountID;

                AdditionalAccountIDType AAID = new AdditionalAccountIDType();
                AdditionalAccountIDType[] AAIDs = new AdditionalAccountIDType[1];

                AAID.Value = RBD[i].AdditionalAccountID;
                AAIDs[0] = AAID;

                CPT.AdditionalAccountID = AAIDs;

                HILT_s.AccountingCustomerParty = CPT;

                PaymentType[] billingPayment = new PaymentType[4];

                PaymentType bp_item = new PaymentType();
                InstructionIDType idInsType = new InstructionIDType();
                PaidAmountType paidAmountType = new PaidAmountType();

                #region START - IMPORTES

                // IMPORTES - GRAVADO
                var ImpGravado = Convert.ToDecimal(RBD[i].MTO_GRA, CultureInfo.CreateSpecificCulture("es-PE"));
                if (ImpGravado > 0)
                {
                    bp_item = new PaymentType();
                    idInsType = new InstructionIDType();
                    paidAmountType = new PaidAmountType();

                    idInsType.Value = "01";
                    paidAmountType.Value = Convert.ToDecimal(RBD[i].MTO_GRA, CultureInfo.CreateSpecificCulture("es-PE"));
                    //paidAmountType.currencyID = CurrencyCodeContentType.PEN;
                    paidAmountType.currencyID = RBD[i].MONEDA.ToUpper() == "USD" ? CurrencyCodeContentType.USD : CurrencyCodeContentType.PEN;

                    bp_item.PaidAmount = paidAmountType;
                    bp_item.InstructionID = idInsType;

                    billingPayment[0] = bp_item;
                }

                // IMPORTES - EXONERADOS
                var ImpExonerado = Convert.ToDecimal(RBD[i].MTO_EXO, CultureInfo.CreateSpecificCulture("es-PE"));
                if (ImpExonerado > 0)
                {
                    bp_item = new PaymentType();
                    idInsType = new InstructionIDType();
                    paidAmountType = new PaidAmountType();

                    idInsType.Value = "02";
                    paidAmountType.Value = Convert.ToDecimal(RBD[i].MTO_EXO, CultureInfo.CreateSpecificCulture("es-PE"));
                    //paidAmountType.currencyID = CurrencyCodeContentType.PEN;
                    paidAmountType.currencyID = RBD[i].MONEDA.ToUpper() == "USD" ? CurrencyCodeContentType.USD : CurrencyCodeContentType.PEN;

                    bp_item.PaidAmount = paidAmountType;
                    bp_item.InstructionID = idInsType;

                    billingPayment[1] = bp_item;
                }

                // IMPORTES - INAFECTO
                var ImpInafecto = Convert.ToDecimal(RBD[i].MTO_INA, CultureInfo.CreateSpecificCulture("es-PE"));
                if (ImpInafecto > 0)
                {
                    bp_item = new PaymentType();
                    idInsType = new InstructionIDType();
                    paidAmountType = new PaidAmountType();

                    idInsType.Value = "03";
                    paidAmountType.Value = Convert.ToDecimal(RBD[i].MTO_INA, CultureInfo.CreateSpecificCulture("es-PE"));
                    //paidAmountType.currencyID = CurrencyCodeContentType.PEN;
                    paidAmountType.currencyID = RBD[i].MONEDA.ToUpper() == "USD" ? CurrencyCodeContentType.USD : CurrencyCodeContentType.PEN;

                    bp_item.PaidAmount = paidAmountType;
                    bp_item.InstructionID = idInsType;

                    billingPayment[2] = bp_item;
                }

                // IMPORTES - GRATUITAS
                var ImpGratuitas = Convert.ToDecimal(RBD[i].MON_GRAT, CultureInfo.CreateSpecificCulture("es-PE"));
                if (ImpGratuitas > 0)
                {
                    bp_item = new PaymentType();
                    idInsType = new InstructionIDType();
                    paidAmountType = new PaidAmountType();

                    idInsType.Value = "05";
                    paidAmountType.Value = Convert.ToDecimal(RBD[i].MON_GRAT, CultureInfo.CreateSpecificCulture("es-PE"));
                    //paidAmountType.currencyID = CurrencyCodeContentType.PEN;
                    paidAmountType.currencyID = RBD[i].MONEDA.ToUpper() == "USD" ? CurrencyCodeContentType.USD : CurrencyCodeContentType.PEN;

                    bp_item.PaidAmount = paidAmountType;
                    bp_item.InstructionID = idInsType;

                    billingPayment[3] = bp_item;
                }

                HILT_s.BillingPayment = billingPayment;

                // IMPORTES - OTROS CARGOS
                var ImpOtroCargos = Convert.ToDecimal(RBD[i].MTO_OCA, CultureInfo.CreateSpecificCulture("es-PE"));
                if (ImpOtroCargos > 0)
                {
                    AllowanceChargeType[] allowCharge = new AllowanceChargeType[1];
                    AmountType1 AT1 = new AmountType1();

                    ChargeIndicatorType CIT = new ChargeIndicatorType();
                    CIT.Value = true;


                    allowCharge[0] = new AllowanceChargeType();
                    allowCharge[0].ChargeIndicator = new ChargeIndicatorType();

                    allowCharge[0].ChargeIndicator = CIT;

                    AT1 = null; AT1 = new AmountType1();
                    AT1.Value = Convert.ToDecimal(RBD[i].MTO_OCA, CultureInfo.CreateSpecificCulture("es-PE"));
                    //AT1.currencyID = CurrencyCodeContentType.PEN;
                    AT1.currencyID = RBD[i].MONEDA.ToUpper() == "USD" ? CurrencyCodeContentType.USD : CurrencyCodeContentType.PEN;

                    allowCharge[0].Amount = AT1;

                    HILT_s.AllowanceCharge = new AllowanceChargeType[1];
                    HILT_s.AllowanceCharge[0] = new AllowanceChargeType();

                    HILT_s.AllowanceCharge[0] = allowCharge[0];
                }

                #endregion END IMPORTES

                #region START IMPUESTOS
                TaxTotalType[] TOTs;// = new TaxTotalType[3];
                TOTs = new TaxTotalType[3];

                TaxTotalType TOT = new TaxTotalType();
                TaxAmountType TAT = new TaxAmountType();
                TaxSubtotalType TST = new TaxSubtotalType();
                TaxCategoryType TCT = new TaxCategoryType();
                TaxSchemeType TSchT = new TaxSchemeType();

                TaxSubtotalType[] TsTi;// = new TaxSubtotalType[3];


                IDType IDsch = new IDType();
                NameType1 nameType = new NameType1();
                TaxTypeCodeType tTCt = new TaxTypeCodeType();


                //IMP IGV
                //var imp_igv = Convert.ToDecimal(RBD[i].IMP_IGV, CultureInfo.CreateSpecificCulture("es-PE"));
                //if (imp_igv > 0)
                //{
                TOT = new TaxTotalType();
                TAT = new TaxAmountType();
                TST = new TaxSubtotalType();
                TCT = new TaxCategoryType();
                TSchT = new TaxSchemeType();
                TsTi = new TaxSubtotalType[1];

                //IDType IDsch = new IDType();
                //NameType1 nameType = new NameType1();
                //TaxTypeCodeType tTCt = new TaxTypeCodeType();

                IDsch = new IDType();
                nameType = new NameType1();
                tTCt = new TaxTypeCodeType();

                TAT.Value = Convert.ToDecimal(RBD[i].IMP_IGV, CultureInfo.CreateSpecificCulture("es-PE"));
                //TAT.currencyID = CurrencyCodeContentType.PEN;
                TAT.currencyID = RBD[i].MONEDA.ToUpper() == "USD" ? CurrencyCodeContentType.USD : CurrencyCodeContentType.PEN;

                IDsch.Value = "1000";
                nameType.Value = "IGV";
                tTCt.Value = "VAT";

                TSchT.ID = IDsch;
                TSchT.Name = nameType;
                TSchT.TaxTypeCode = tTCt;
                TCT.TaxScheme = TSchT;
                TST.TaxAmount = TAT;
                TST.TaxCategory = TCT;
                TOT.TaxAmount = TAT;
                TsTi[0] = TST;

                TOT.TaxSubtotal = TsTi;
                TOTs[0] = TOT;

                //}

                //IMP ISC
                //var imp_isc = Convert.ToDecimal(RBD[i].IMP_ISC, CultureInfo.CreateSpecificCulture("es-PE"));
                //if (imp_isc > 0)
                //{
                TOT = new TaxTotalType();
                TAT = new TaxAmountType();
                TST = new TaxSubtotalType();
                TCT = new TaxCategoryType();
                TSchT = new TaxSchemeType();
                TsTi = new TaxSubtotalType[1];

                //IDType IDsch = new IDType();
                //NameType1 nameType = new NameType1();
                //TaxTypeCodeType tTCt = new TaxTypeCodeType();

                IDsch = new IDType();
                nameType = new NameType1();
                tTCt = new TaxTypeCodeType();

                TAT.Value = Convert.ToDecimal(RBD[i].IMP_ISC, CultureInfo.CreateSpecificCulture("es-PE"));
                //TAT.currencyID = CurrencyCodeContentType.PEN;
                TAT.currencyID = RBD[i].MONEDA.ToUpper() == "USD" ? CurrencyCodeContentType.USD : CurrencyCodeContentType.PEN;

                IDsch.Value = "2000";
                nameType.Value = "ISC";
                tTCt.Value = "EXC";

                TSchT.ID = IDsch;
                TSchT.Name = nameType;
                TSchT.TaxTypeCode = tTCt;
                TCT.TaxScheme = TSchT;
                TST.TaxAmount = TAT;
                TST.TaxCategory = TCT;
                TOT.TaxAmount = TAT;
                TsTi[0] = TST;

                TOT.TaxSubtotal = TsTi;
                TOTs[1] = TOT;

                //}

                //IMP OTH
                var imp_oth = Convert.ToDecimal(RBD[i].IMP_OTH, CultureInfo.CreateSpecificCulture("es-PE"));
                if (imp_oth > 0)
                {
                    TOT = new TaxTotalType();
                    TAT = new TaxAmountType();
                    TST = new TaxSubtotalType();
                    TCT = new TaxCategoryType();
                    TSchT = new TaxSchemeType();
                    TsTi = new TaxSubtotalType[1];

                    //IDType IDsch = new IDType();
                    //NameType1 nameType = new NameType1();
                    //TaxTypeCodeType tTCt = new TaxTypeCodeType();

                    IDsch = new IDType();
                    nameType = new NameType1();
                    tTCt = new TaxTypeCodeType();

                    TAT.Value = Convert.ToDecimal(RBD[i].IMP_OTH, CultureInfo.CreateSpecificCulture("es-PE"));
                    //TAT.currencyID = CurrencyCodeContentType.PEN;
                    TAT.currencyID = RBD[i].MONEDA.ToUpper() == "USD" ? CurrencyCodeContentType.USD : CurrencyCodeContentType.PEN;

                    IDsch.Value = "9999";
                    nameType.Value = "OTROS";
                    tTCt.Value = "OTH";

                    TSchT.ID = IDsch;
                    TSchT.Name = nameType;
                    TSchT.TaxTypeCode = tTCt;
                    TCT.TaxScheme = TSchT;
                    TST.TaxAmount = TAT;
                    TST.TaxCategory = TCT;
                    TOT.TaxAmount = TAT;
                    TsTi[0] = TST;

                    TOT.TaxSubtotal = TsTi;
                    TOTs[2] = TOT;
                }

                HILT_s.TaxTotal = TOTs;

                #endregion END IMPUESTOS

                #region IF NOTE CREDI OR NOTE DEBIT
                if (RBD[i].TPO_CPE.Contains("07") || RBD[i].TPO_CPE.Contains("08"))
                {
                    string[] array_tpodoc_afec = RBD[i].TPO_DOC_AFEC.Split(',');
                    string[] array_nrodoc_afec = RBD[i].NRO_DOC_AFEC.Split(',');
                    if (array_tpodoc_afec.Length == array_nrodoc_afec.Length)
                    {
                        BillingReferenceType[] BRs = new BillingReferenceType[array_nrodoc_afec.Length];
                        BillingReferenceType BR = new BillingReferenceType();
                        DocumentReferenceType docRef = new DocumentReferenceType();
                        for (int af = 0; af < array_tpodoc_afec.Length; af++)
                        {
                            BR = new BillingReferenceType();
                            docRef = new DocumentReferenceType();

                            IDType idref = new IDType();
                            idref.Value = array_nrodoc_afec[af];

                            DocumentTypeCodeType doctyperef = new DocumentTypeCodeType();
                            doctyperef.Value = array_tpodoc_afec[af];

                            docRef.ID = idref;
                            docRef.DocumentTypeCode = doctyperef;

                            BR.InvoiceDocumentReference = docRef;
                            BRs[af] = BR;
                        }
                        HILT_s.BillingReference = BRs;
                    }
                }
                #endregion

                //MILT[i] = HILT;


                #endregion
                detOk = true;
            }
            catch (Exception ex)
            {
                detOk = false;
            }
            return HILT_s;
        }








        public List<RBoletasDetalle> ordenaResumen(string TypeRC)
        {
            List<ResumenB> DBList = new List<ResumenB>();

            DBList = DBConnection.SP_ObtieneResumenBoletas_2_1(TypeRC);

            string sa = "", ta = "";
            int na = 0, sl = 0;

            RBoletasDetalle RBL = null;
            RBC = new RBoletasCabecera();

            RBC.TOT_DOC = DBList.Count;
            RBD = new List<RBoletasDetalle>();

            for (int i = 0; i < DBList.Count; i++)
            {
                RBC.DOC_UPD_LIST.Add($"{MS.RucNumber}-{DBList[i].TPODOC}-{DBList[i].SERIE}-{DBList[i].NUMERO.ToString().PadLeft(8, '0')}");

                RBL = new RBoletasDetalle();

                sa = DBList[i].SERIE;
                na = DBList[i].NUMERO - 1;
                ta = DBList[i].TPODOC;

                RBC.FEC_INI = DateTime.Parse(MS.SummaryDate);
                RBC.FEC_FIN = DateTime.Parse(MS.SummaryDate);


                RBL.MONEDA = DBList[i].MONEDA;

                RBL.TPO_CPE = DBList[i].TPODOC;

                RBL.FEC_EMIS = DBList[i].DESDE;

                RBL.LineID = i + 1;
                RBL.DocumentTypeCode = DBList[i].TPODOC;
                RBL.ID = DBList[i].SERIE + "-" + DBList[i].NUMERO;

                RBL.CustomerAssignedAccountID = DBList[i].RE_NUMDOC;
                RBL.AdditionalAccountID = DBList[i].RE_TPODOC;
                RBL.ConditionCode = DBList[i].STATUS_RC_DOC;

                RBL.TPO_DOC_AFEC = DBList[i].TPO_DOC_AFEC;
                RBL.NRO_DOC_AFEC = DBList[i].NRO_DOC_AFEC;
                RBL.SYSTEM_STATUS = DBList[i].SYSTEM_STATUS;
                RBL.NUM_CPE = DBList[i].NUM_CPE;

                RBL.STATUS_RC_DOC = DBList[i].STATUS_RC_DOC;

                // IMPORTES
                RBL.MTO_GRA = DBList[i].GRAVADO;
                RBL.MTO_EXO = DBList[i].EXONERADO;
                RBL.MTO_INA = DBList[i].INAFECTO;
                RBL.MTO_OCA = DBList[i].OCARGOS;
                RBL.MTO_GRAT = DBList[i].GRATUITO;

                // IMPUESTOS
                RBL.IMP_IGV = DBList[i].IGV;
                RBL.IMP_ISC = DBList[i].ISC;
                RBL.IMP_OTH = DBList[i].OTH;

                // TOTAL
                RBL.MTO_TOT = DBList[i].TOTAL;

                //SUMMARY
                RBL.SUMMARY = DBList[i].SUMMARY;
                RBL.VOIDED = DBList[i].VOIDED;

                RBC.MTO_GRA = DBList[i].GRAVADO;
                RBC.MTO_EXO = DBList[i].EXONERADO;
                RBC.MTO_INA = DBList[i].INAFECTO;
                RBC.MTO_OCA = DBList[i].OCARGOS;
                RBC.MTO_GRAT = DBList[i].GRATUITO;

                RBC.IMP_IGV = DBList[i].IGV;
                RBC.IMP_ISC = DBList[i].ISC;
                RBC.IMP_OTH = DBList[i].OTH;

                sl++; na = DBList[i].NUMERO;
                RBD.Add(RBL);
            }
            return RBD;
        }

        //public bool GenerateXML()
        public Extras.Common.Method.ListUtilClass GenerateXML(string TypeRC)
        {
            //ListDocs.ListBoletas = new List<RBoletasDetalle>();
            ListDocs = new Extras.Common.Method.ListUtilClass();
            bool isGenerated = false;
            //InsertaTC(); exchange rate
            RBD = ordenaResumen(TypeRC);
            var temp = new List<RBoletasDetalle>();

            var temp_XGN = new List<RBoletasDetalle>();
            var temp_NCD = new List<RBoletasDetalle>();
            var temp_ANS = new List<RBoletasDetalle>();

            foreach (var doc in RBD)
            {
                if (doc.SYSTEM_STATUS == "XGN")
                {
                    if(doc.SUMMARY == 0)
                    {
                        temp_XGN.Add(doc);
                    }
                }
                else if (doc.SYSTEM_STATUS == "ANS")
                {
                    if(doc.VOIDED == 0)
                    {
                        temp_ANS.Add(doc);
                    }

                    if (doc.SUMMARY == 0)
                    {
                        temp_XGN.Add(doc);
                    }
                }

                if (doc.TPO_CPE.Contains("07") || doc.TPO_CPE.Contains("08"))
                {
                    temp_NCD.Add(doc);
                }
            }

            if(TypeRC == "ANS")
            {
                //temp = temp_ANS;

                foreach (var ans in temp_ANS)
                {
                    if(ans.STATUS_RC_DOC != 3)
                    {
                        temp.Add(ans);
                    }
                }

            }
            else if(TypeRC == "XGN")
            {
                foreach(var d in temp_XGN)
                {
                    if (!d.TPO_CPE.Contains("07") && !d.TPO_CPE.Contains("08") && d.STATUS_RC_DOC < 1)
                    {
                        temp.Add(d);
                    }
                }

                if (temp_NCD.Count > 0)
                {
                    ListDocs.ListNotesDoc = temp_NCD;
                }

                //if(temp_ANS.Count > 0)
                //{
                //    ListDocs.ListBoletasLow = temp_ANS;
                //}
                //ListDocs.ListBoletasLow = temp_ANS;
                ListDocs.ListBoletasLow.Add(new RBoletasDetalle() { NUM_CPE = "" });
            }
            else
            {
                foreach (var ncd in temp_NCD)
                {
                    if(ncd.STATUS_RC_DOC <= 1)
                    {
                        temp.Add(ncd);
                    }
                }

                //temp = temp_NCD;

                //if (temp_ANS.Count > 0)
                //{
                //    ListDocs.ListBoletasLow = temp_ANS;
                //}
                //ListDocs.ListBoletasLow = temp_ANS;
                ListDocs.ListBoletasLow.Add(new RBoletasDetalle() { NUM_CPE = "" });
            }

            #region old
            ////if (TypeRC == "ANS")
            ////{
            ////    foreach (var b in RBD)
            ////    {
            ////        if (b.SYSTEM_STATUS.Contains(TypeRC) && b.VOIDED == 0) { temp.Add(b); }
            ////    }
            ////}
            ////else if (TypeRC == "NCD")
            ////{
            ////    foreach (var b in RBD)
            ////    {
            ////        if (b.SYSTEM_STATUS.ToUpper().Contains("ANS"))
            ////        {
            ////            ListDocs.ListBoletasLow.Add(b);
            ////        }

            ////        if (!b.TPO_CPE.Contains("03") && (b.SUMMARY <= 0))
            ////        {
            ////            temp.Add(b);
            ////        }
            ////    }
            ////}
            ////else
            ////{
            ////    foreach (var b in RBD)
            ////    {
            ////        if (b.SYSTEM_STATUS.ToUpper().Contains("ANS"))
            ////        {
            ////            ListDocs.ListBoletasLow.Add(b);
            ////        }

            ////        if (b.TPO_CPE == "07" || b.TPO_CPE == "08")
            ////        {
            ////            ListDocs.ListNotesDoc.Add(b);
            ////        }

            ////        if (b.SUMMARY == 0 && !b.TPO_CPE.Contains("07") && !b.TPO_CPE.Contains("08"))
            ////        {
            ////            temp.Add(b);
            ////        }
            ////    }
            ////}
            //if (TypeRC == "ANS")
            //{
            //    foreach (var b in RBD)
            //    {
            //        if (b.SYSTEM_STATUS.Contains(TypeRC) && b.VOIDED == 0)
            //        {
            //            temp.Add(b);
            //        }
            //    }
            //}
            //else
            //{
            //    foreach (var b in RBD)
            //    {
            //        if (b.SYSTEM_STATUS.ToUpper().Contains("ANS"))
            //        {
            //            ListDocs.ListBoletasLow.Add(b);
            //        }

            //        if (b.SUMMARY == 0)
            //        {
            //            temp.Add(b);
            //        }

            //        if (b.TPO_CPE == "07" || b.TPO_CPE == "08")
            //        {
            //            ListDocs.ListNotesDoc.Add(b);
            //        }
            //    }
            //}

            //foreach (var b in RBD)
            //{
            //if (b.SYSTEM_STATUS.ToUpper().Contains("ANS"))
            //{
            //    ListDocs.ListBoletasLow.Add(b);
            //}
            //if (b.SYSTEM_STATUS.ToUpper().Contains(TypeRC))
            //{
            //    temp.Add(b);
            //}
            //}
            #endregion

            #region old cant
            //if (temp.Count > 1000)
            //{
            //    var tmp = new List<RBoletasDetalle>();
            //    for (int i = 0; i < 1000; i++)
            //    {
            //        tmp.Add(temp[i]);
            //    }
            //    RBD = tmp;
            //    ListDocs.CantDocs = temp.Count - 1000;
            //}
            //else
            //{
            //    RBD = temp;
            //}
            #endregion

            if (temp.Count > 500)
            {
                var tmp = new List<RBoletasDetalle>();
                for (int i = 0; i < 500; i++)
                {
                    tmp.Add(temp[i]);
                }
                RBD = tmp;
                ListDocs.CantDocs = temp.Count - 500;
            }
            else
            {
                RBD = temp;
            }


            if (RBD.Count == 0)
            {
                MS.GeneraLog.IngresoBD01($"No se obtuvieron boletas y/o documentos asociados de la fecha {MS.SummaryDate}");
                //return false;
                return ListDocs;
            }
            else
            {
                if (!DBConnection.ExchangueRateExists && DBConnection.DocumentDolarExists)
                {
                    MS.GeneraLog.IngresoBD01($"No se ha configurado el tipo de cambio para el día {MS.SummaryDate}");
                    //return false;
                    return ListDocs;
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
                sum.SummaryDocumentsLine = getSummaryDocumentsLine(1, "", TypeRC);
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
            //return isGenerated;
            return ListDocs;
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
}
