using ADE.Configurations.DataAccess;
using ADE.Configurations.Entities;
using ADE.Configurations.Objects;
using ADE.Processes.DatabaseIncome;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace eProcessNotaDebito
{
    public class EDebitNote
    {
        /*
         * CLASE Efactura [UBLPE-DebitNote-2.0.cs]
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
        public string XMLI = "";
        CultureInfo culture = new CultureInfo("es-PE");
        public Dictionary<string, string> Interface = new Dictionary<string, string>();
        public string DocumentName, RUCNumber, DocumentType;
        public Log log = null;
        public List<string> listaerror = new List<string>();
        DatabaseConnection DBConnection = null;
        MainSettings MS = null;
        Validation VD = null;
        public DBDocument Document = new DBDocument();
        public Stream SXMLI = null;

        public EDebitNote(MainSettings MSI, Validation VDI)
        {
            MS = MSI;
            VD = VDI;
            Interface = VD.Interface;
            DBConnection = new DatabaseConnection(MSI);
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
            Document.Cabecera.IdDocumentoCabecera = 1;

            return new IDType()
            {
                Value = Interface["Id_CE"]
            };
        }

        public IssueDateType getIssueDate()
        {
            return new IssueDateType()
            {
                Value = DateTime.Parse(Interface["FEmision"])
            };
        }

        public ResponseType[] getDiscrepancyResponseType()
        {
            ResponseType[] MRT = new ResponseType[1];

            string TpoNota = Interface["TpoNota"].ToString();

            if (TpoNota == "03")
            {
                for (int i = 0; i < 1; ++i)
                {
                    int j = i + 1;
                    MRT[i] = new ResponseType()
                    {
                        ReferenceID = new ReferenceIDType()
                        {
                            Value = Interface["DocID"]
                        },
                        ResponseCode = new ResponseCodeType()
                        {
                            Value = Interface["TpoNota"]
                        },
                        Description = new DescriptionType[]
                        {
                    new DescriptionType()
                    {
                        Value = Interface["MotivoNota"]
                    }
                        }
                    };
                }
            }
            else {
                for (int i = 0; i < 1; ++i)
                {
                    int j = i + 1;
                    MRT[i] = new ResponseType()
                    {
                        ReferenceID = new ReferenceIDType()
                        {
                            Value = Interface["DocID" + j]
                        },
                        ResponseCode = new ResponseCodeType()
                        {
                            Value = Interface["TpoNota"]
                        },
                        Description = new DescriptionType[]
                        {
                    new DescriptionType()
                    {
                        Value = Interface["MotivoNota"]
                    }
                        }
                    };
                }
            }
            
            
            return MRT;
        }

        public BillingReferenceType[] getBillingReferenceType()
        {
            BillingReferenceType[] BRT = new BillingReferenceType[VD.da];

            string TpoNota = Interface["TpoNota"].ToString();
            if (TpoNota != "03")
            {
                for (int i = 0; i < VD.da; ++i)
                {
                    int j = i + 1;
                    BRT[i] = new BillingReferenceType
                    {
                        InvoiceDocumentReference = new DocumentReferenceType()
                        {
                            ID = new IDType()
                            {
                                Value = Interface["DocID" + j]
                            },
                            DocumentTypeCode = new DocumentTypeCodeType()
                            {
                                Value = Interface["DocTpoDoc" + j]
                            }
                        }
                    };
                }
            }

            
            return BRT;
        }

        public DocumentCurrencyCodeType getDocumentCurrencyCode()
        {
            return new DocumentCurrencyCodeType()
            {
                Value = Interface["Tmoneda"]
            };
        }

        public MonetaryTotalType getRequestedMonetaryTotal()
        {
            MonetaryTotalType mtt = new MonetaryTotalType()
            {
                PayableAmount = new PayableAmountType()
                {
                    currencyID = Interface["Tmoneda"],
                    Value = Interface["TotImporTotal"]
                }
            };

            if (Interface["TotDctoGlobal"] != "" && Interface["TotDctoGlobal"] != "0.00")
            {
                mtt.AllowanceTotalAmount = new AllowanceTotalAmountType()
                {
                    currencyID = Interface["Tmoneda"],
                    Value = Interface["TotDctoGlobal"]
                };
            }

            if (Interface["TotSumOCargo"] != "" && Interface["TotSumOCargo"] != "0.00")
            {
                mtt.ChargeTotalAmount = new ChargeTotalAmountType()
                {
                    currencyID = Interface["Tmoneda"],
                    Value = Interface["TotSumOCargo"]
                };
            }

            return mtt;
        }

        public CustomizationIDType getCustomizationID()
        {
            return new CustomizationIDType()
            {
                Value = "1.0"
            };
        }

        public UBLVersionIDType getUBLVersionID()
        {
            return new UBLVersionIDType()
            {
                Value = "2.0"
            };
        }

        public TaxTotalType[] getTaxTotal()
        {
            TaxTotalType[] txp = new TaxTotalType[VD.Taxes.Count];
            string code, taxe, last;
            string valu = "0.00";
            for (int i = 0; i < VD.Taxes.Count; ++i)
            {
                code = taxe = last = "";
                if (VD.Taxes[i] == 4) { code = "1000"; taxe = "IGV"; last = "VAT"; valu = Interface["TotSumIGV"]; }
                if (VD.Taxes[i] == 5) { code = "2000"; taxe = "ISC"; last = "VAT"; valu = Interface["TotSumISC"]; }
                if (VD.Taxes[i] == 6) { code = "9999"; taxe = "OTROS"; last = "OTH"; valu = Interface["TotSumOTrib"]; }

                txp[i] = new TaxTotalType()
                {
                    TaxAmount = new TaxAmountType()
                    {
                        currencyID = Interface["Tmoneda"],
                        Value = valu
                    },
                    TaxSubtotal = new TaxSubtotalType[]
                    {
                        new TaxSubtotalType
                        {
                            TaxAmount = new TaxAmountType()
                            {
                                currencyID = Interface["Tmoneda"],
                                Value = valu
                            },
                            TaxCategory = new TaxCategoryType()
                            {
                                TaxScheme = new TaxSchemeType()
                                {
                                    ID = new IDType()
                                    {
                                        Value = code
                                    },
                                    Name = new NameType1()
                                    {
                                        Value = taxe
                                    },
                                    TaxTypeCode = new TaxTypeCodeType()
                                    {
                                        Value = last
                                    }
                                }
                            }
                        }
                    }
                };
            }

            return txp;
        }

        public SignatureType[] getSignatureType()
        {
            return new SignatureType[]
            {
                new SignatureType()
                {
                    ID = new IDType()
                    {
                        Value = "IDSign" + Interface["EmiNumDocu"]
                    },
                    SignatoryParty = new PartyType()
                    {
                        PartyIdentification = new PartyIdentificationType[]
                        {
                            new PartyIdentificationType
                            {
                                ID = new IDType()
                                {
                                    Value = Interface["EmiNumDocu"]
                                }
                            }
                        },
                        PartyName = new PartyNameType[]
                        {
                            new PartyNameType()
                            {
                                Name = new NameType1()
                                {
                                    Value = "<![CDATA[" + Interface["EmiNombre"] + "]]>"
                                }
                            }
                        }
                    },
                    DigitalSignatureAttachment = new AttachmentType()
                    {
                        ExternalReference = new ExternalReferenceType()
                        {
                            URI = new URIType()
                            {
                                Value = "#signature" + Interface["EmiNumDocu"]
                            }
                        }
                    }
                }
            };
        }

        public SupplierPartyType getSupplierPartyType()
        {
            return new SupplierPartyType()
            {
                CustomerAssignedAccountID = new CustomerAssignedAccountIDType()
                {
                    Value = Interface["EmiNumDocu"]
                },
                AdditionalAccountID = new AdditionalAccountIDType[]
                {
                    new AdditionalAccountIDType()
                    {
                        Value = Interface["EmiTpoDoc"]
                    }
                },
                Party = new PartyType()
                {
                    PartyName = new PartyNameType[]
                    {
                        new PartyNameType()
                        {
                            Name = new NameType1()
                            {
                                Value = "<![CDATA[" + Interface["EmiNComer"] + "]]>"
                            }
                        }
                    },
                    PostalAddress = new AddressType()
                    {
                        ID = new IDType()
                        {
                            Value = Interface["EmiUbigeo"]
                        },
                        StreetName = new StreetNameType()
                        {
                            Value = Interface["EmiDirFiscal"]
                        },
                        CitySubdivisionName = new CitySubdivisionNameType()
                        {
                            Value = Interface["EmiDirUrbani"]
                        },
                        CityName = new CityNameType()
                        {
                            Value = Interface["EmiDirProvin"]
                        },
                        CountrySubentity = new CountrySubentityType()
                        {
                            Value = Interface["EmiDirDepart"]
                        },
                        District = new DistrictType()
                        {
                            Value = Interface["EmiDirDistrito"]
                        },
                        Country = new CountryType()
                        {
                            IdentificationCode = new IdentificationCodeType()
                            {
                                Value = Interface["EmiCodPais"]
                            }
                        }
                    },
                    PartyLegalEntity = new PartyLegalEntityType[]
                    {
                        new PartyLegalEntityType()
                        {
                            RegistrationName = new RegistrationNameType()
                            {
                                Value = "<![CDATA[" + Interface["EmiNombre"] + "]]>"
                            }
                        }
                    }
                }
            };
        }

        public CustomerPartyType getAccountingCustomerParty()
        {
            return new CustomerPartyType()
            {
                CustomerAssignedAccountID = new CustomerAssignedAccountIDType()
                {
                    Value = Interface["RecNumDocu"]
                },
                AdditionalAccountID = new AdditionalAccountIDType[]
                {
                    new AdditionalAccountIDType()
                    {
                        Value = Interface["RecTpoDoc"]
                    }
                },
                Party = new PartyType()
                {
                    PartyName = new PartyNameType[]
                    {
                        new PartyNameType()
                        {
                            Name = new NameType1()
                            {
                                Value = "<![CDATA[" + Interface["RecNComer"] + "]]>"
                            }
                        }
                    },
                    PostalAddress = new AddressType()
                    {
                        ID = new IDType()
                        {
                            Value = Interface["RecUbigeo"]
                        },
                        StreetName = new StreetNameType()
                        {
                            Value = Interface["RecDirFiscal"]
                        },
                        CitySubdivisionName = new CitySubdivisionNameType()
                        {
                            Value = Interface["RecDirUrbani"]
                        },
                        CityName = new CityNameType()
                        {
                            Value = Interface["RecDirProvin"]
                        },
                        CountrySubentity = new CountrySubentityType()
                        {
                            Value = Interface["RecDirDepart"]
                        },
                        District = new DistrictType()
                        {
                            Value = Interface["RecDirDistrito"]
                        },
                        Country = new CountryType()
                        {
                            IdentificationCode = new IdentificationCodeType()
                            {
                                Value = Interface["RecCodPais"]
                            }
                        }
                    },
                    PartyLegalEntity = new PartyLegalEntityType[]
                    {
                        new PartyLegalEntityType()
                        {
                            RegistrationName = new RegistrationNameType()
                            {
                                Value = "<![CDATA[" + Interface["RecNombre"] + "]]>"
                            }
                        }
                    }
                }
            };
        }

        public UBLExtensionType[] getUBLExtensions()
        {
            var grav = 0.0m;
            var inafec = 0.0m;
            var exoner = 0.0m;

            string EXT1 = $"<AdditionalInformationSLIN><Document><ID>{Interface["NUM_CE"]}</ID></Document>";
            double p1 = 0, p2 = 0, p3 = 0;
            if (VD.ex > 0)
            {
                for (int i = 0; i < VD.ex; ++i)
                {
                    int j = i + 1;
                    string extra = "<Extra>" +
                                     $"<ExLinea>{Interface["ExLinea" + j]}</ExLinea>" +
                                     $"<ExDato><![CDATA[{Interface["ExDato" + j]}]]></ExDato>" +
                                     $"<ExTipo>{Interface["ExTipo" + j]}</ExTipo>" +
                                   "</Extra>";
                    EXT1 += extra;
                }
            }
            EXT1 += "</AdditionalInformationSLIN>";

            string EXT2 = "";

            var TpoOperation = Interface["IdTpoOperacion"];

            if (TpoOperation == "02")
            {
                EXT2 = "<sac:AdditionalInformation>" +
                 "<sac:AdditionalMonetaryTotal>" +
                     "<cbc:ID>1002</cbc:ID>" +
                     $"<cbc:PayableAmount currencyID=\"{Interface["Tmoneda"]}\">{Interface["TotVtaInaf"]}</cbc:PayableAmount>" +    //Inafecta
                 "</sac:AdditionalMonetaryTotal>";
            }
            else
            {
                EXT2 = "<sac:AdditionalInformation>" +
                 "<sac:AdditionalMonetaryTotal>" +
                     "<cbc:ID>1001</cbc:ID>" +
                     $"<cbc:PayableAmount currencyID=\"{Interface["Tmoneda"]}\">{Interface["TotVtaGrab"]}</cbc:PayableAmount>" +    //Gravada
                 "</sac:AdditionalMonetaryTotal>" +
                 "<sac:AdditionalMonetaryTotal>" +
                     "<cbc:ID>1002</cbc:ID>" +
                     $"<cbc:PayableAmount currencyID=\"{Interface["Tmoneda"]}\">{Interface["TotVtaInaf"]}</cbc:PayableAmount>" +    //Inafecta
                 "</sac:AdditionalMonetaryTotal>" +
                 "<sac:AdditionalMonetaryTotal>" +
                     "<cbc:ID>1003</cbc:ID>" +
                     $"<cbc:PayableAmount currencyID=\"{Interface["Tmoneda"]}\">{Interface["TotVtaExon"]}</cbc:PayableAmount>" +    //Exonerada
                 "</sac:AdditionalMonetaryTotal>";
            }

            

            if (Interface.ContainsKey("PerBaseImpo") && Interface.ContainsKey("PerMntoPer") && Interface.ContainsKey("PerMntoTot"))
            {
                if (double.TryParse(Interface["PerBaseImpo"], out p1) && double.TryParse(Interface["PerMntoPer"], out p2) && double.TryParse(Interface["PerMntoTot"], out p3))
                {
                    if (p1 > 0 && p2 > 0 && p3 > 0)
                    {
                        EXT2 += "<sac:AdditionalMonetaryTotal>" +
                             "<cbc:ID>2001</cbc:ID>" +
                            $"<sac:ReferenceAmount currencyID=\"{Interface["Tmoneda"]}\">{Interface["PerBaseImpo"]}</sac:ReferenceAmount>" +
                            $"<cbc:PayableAmount currencyID=\"{Interface["Tmoneda"]}\">{Interface["PerMntoPer"]}</cbc:PayableAmount>" +
                            $"<sac:TotalAmount currencyID=\"{Interface["Tmoneda"]}\">{Interface["PerMntoTot"]}</sac:TotalAmount>" +
                        "</sac:AdditionalMonetaryTotal>";
                    }
                }
            }

            if (Interface.ContainsKey("DetMonto") && Interface.ContainsKey("DetPorcent"))
            {
                if (Interface["DetMonto"] != "" && Interface["DetMonto"] != "0.00" && Interface["DetPorcent"] != "" && Interface["DetPorcent"] != "0.00")
                {
                    EXT2 += "<sac:AdditionalMonetaryTotal>" +
                            "<cbc:ID>2003</cbc:ID>" +
                            $"<cbc:PayableAmount currencyID=\"{Interface["Tmoneda"]}\">{Interface["DetMonto"]}</cbc:PayableAmount>" +
                            $"<cbc:Percent>{Interface["DetPorcent"]}</cbc:Percent>" +
                        "</sac:AdditionalMonetaryTotal>";
                }
            }
            if (Interface["TotTotDscto"] != "" && Interface["TotTotDscto"] != "0.00")
            {
                EXT2 += "<sac:AdditionalMonetaryTotal>" +
                            "<cbc:ID>2005</cbc:ID>" +
                           $"<cbc:PayableAmount currencyID=\"{Interface["Tmoneda"]}\">{Interface["TotTotDscto"]}</cbc:PayableAmount>" +  //Total Descuentos
                        "</sac:AdditionalMonetaryTotal>";
            }
            EXT2 += "<sac:AdditionalProperty>" +
                          "<cbc:ID>1000</cbc:ID>" +
                         $"<cbc:Value>{Interface["MontoLiteral"]}</cbc:Value>" +
                      "</sac:AdditionalProperty>";

            if (p1 > 0 && p2 > 0 && p3 > 0)
            {
                EXT2 += "<sac:AdditionalProperty>" +
                          "<cbc:ID>2000</cbc:ID>" +
                         $"<cbc:Value>COMPROBANTE DE PERCEPCION</cbc:Value>" +
                      "</sac:AdditionalProperty>";
            }

            if (Interface.ContainsKey("DetValBBSS") && Interface.ContainsKey("DetCtaBN"))
            {
                if (Interface["DetValBBSS"] != "" && Interface["DetCtaBN"] != "")
                {
                    EXT2 += "<sac:AdditionalProperty>" +
                          "<cbc:ID>3000</cbc:ID>" +
                         $"<cbc:Value>{Interface["DetValBBSS"]}</cbc:Value>" +
                      "</sac:AdditionalProperty>";

                    EXT2 += "<sac:AdditionalProperty>" +
                          "<cbc:ID>3001</cbc:ID>" +
                         $"<cbc:Value>{Interface["DetCtaBN"]}</cbc:Value>" +
                      "</sac:AdditionalProperty>";
                }
            }

            EXT2 += "</sac:AdditionalInformation>";

            string EXT3 = "";

            return new UBLExtensionType[]
            {
                new UBLExtensionType()
                {
                    ExtensionContent = EXT1
                },
                new UBLExtensionType()
                {
                    ExtensionContent = EXT2
                },
                new UBLExtensionType()
                {
                    ExtensionContent = EXT3
                }
            };
        }

        public DebitNoteLineType[] getDebitNoteLine()
        {
            DebitNoteLineType HILT = new DebitNoteLineType();
            DebitNoteLineType[] MILT = new DebitNoteLineType[VD.li];

            for (int i = 0; i < VD.li; ++i)
            {
                MILT[i] = new DebitNoteLineType();

                IDType HIT = new IDType();
                HIT.Value = (i + 1).ToString();
                MILT[i].ID = HIT;

                DebitedQuantityType HIQT = new DebitedQuantityType();
                HIQT.unitCode = Interface["LnUndMed" + (i + 1)];
                HIQT.unitCodeSpecified = true;
                HIQT.Value = Interface["LnCantidad" + (i + 1)];
                MILT[i].DebitedQuantity = HIQT;

                LineExtensionAmountType HLEAT = new LineExtensionAmountType();
                HLEAT.currencyID = Interface["Tmoneda"];
                HLEAT.Value = Interface["LnValVta" + (i + 1)];

                MILT[i].LineExtensionAmount = HLEAT;

                PricingReferenceType HPRT = new PricingReferenceType();
                PricingReferenceType[] MRPT = { HPRT, HPRT };

                PriceType HPCT = new PriceType();
                PriceType[] MPCT = null;
                PriceAmountType HXPT = new PriceAmountType();

                if (Interface["LnCodAfecIGV" + (i + 1)] == "10" || Interface["LnCodAfecIGV" + (i + 1)] == "20" || Interface["LnCodAfecIGV" + (i + 1)] == "30" || Interface["LnCodAfecIGV" + (i + 1)] == "40")
                {
                    MPCT = new PriceType[] {
                        new PriceType() {
                            PriceAmount = new PriceAmountType()
                            {
                                currencyID = Interface["Tmoneda"],
                                Value = Interface["LnMntPrcVta" + (i + 1)]
                            },
                            PriceTypeCode = new PriceTypeCodeType()
                            {
                                Value = "01"
                            }
                        }
                    };
                }
                else
                {
                    MPCT = new PriceType[] {
                        new PriceType() {
                            PriceAmount = new PriceAmountType()
                            {
                                currencyID = Interface["Tmoneda"],
                                Value = "0.00"
                            },
                            PriceTypeCode = new PriceTypeCodeType()
                            {
                                Value = "01"
                            }
                        },
                        new PriceType() {
                            PriceAmount = new PriceAmountType()
                            {
                                currencyID = Interface["Tmoneda"],
                                Value = Interface["LnMntPrcVta" + (i + 1)]
                            },
                            PriceTypeCode = new PriceTypeCodeType()
                            {
                                Value = "02"
                            }
                        },
                    };
                }

                MRPT[0].AlternativeConditionPrice = MPCT;

                MILT[i].PricingReference = MRPT[0];

                // TAX TOTAL
                double o = 0;
                double.TryParse(Interface["LnMntISC" + (i + 1)], out o);

                var igvPercent = Interface["LnIgvPercentage" + (i + 1)];
                if (igvPercent == null || igvPercent.Length == 0)
                {
                    igvPercent = "18.00";
                }

                if (o > 0)
                {
                    MILT[i].TaxTotal = new TaxTotalType[]{
                        new TaxTotalType()
                        {
                            TaxAmount = new TaxAmountType()
                            {
                                currencyID = Interface["Tmoneda"],
                                Value = Interface["LnMntIGV" + (i + 1)]
                            },
                            TaxSubtotal = new TaxSubtotalType[]
                            {
                                new TaxSubtotalType()
                                {
                                    TaxableAmount = new TaxableAmountType()
                                    {
                                        currencyID = Interface["Tmoneda"],
                                        Value = "0.00"
                                    },
                                    TaxAmount = new TaxAmountType()
                                    {
                                        currencyID = Interface["Tmoneda"],
                                        Value = Interface["LnMntIGV" + (i + 1)]
                                    },
                                    Percent = new PercentType()
                                    {
                                        //Value = "18.00"
                                        Value = igvPercent
                                    },
                                    TaxCategory = new TaxCategoryType()
                                    {
                                        TaxExemptionReasonCode = new TaxExemptionReasonCodeType()
                                        {
                                            Value = Interface["LnCodAfecIGV" + (i + 1)]
                                        },TaxScheme = new TaxSchemeType()
                                        {
                                            ID = new IDType()
                                            {
                                                Value = "1000"
                                            },
                                            Name = new NameType1()
                                            {
                                                Value = "IGV"
                                            },
                                            TaxTypeCode = new TaxTypeCodeType()
                                            {
                                                Value = "VAT"
                                            }
                                        }
                                    }
                                }
                            }
                        },
                        new TaxTotalType()
                        {
                            TaxAmount = new TaxAmountType()
                            {
                                currencyID = Interface["Tmoneda"],
                                Value = Interface["LnMntISC" + (i + 1)]
                            },
                            TaxSubtotal = new TaxSubtotalType[]
                            {
                                new TaxSubtotalType()
                                {
                                    TaxableAmount = new TaxableAmountType()
                                    {
                                        currencyID = Interface["Tmoneda"],
                                        Value = "0.00"
                                    },
                                    TaxAmount = new TaxAmountType()
                                    {
                                        currencyID = Interface["Tmoneda"],
                                        Value = Interface["LnMntISC" + (i + 1)]
                                    },
                                    TaxCategory = new TaxCategoryType()
                                    {
                                        TaxExemptionReasonCode = new TaxExemptionReasonCodeType()
                                        {
                                            Value = ""
                                        },
                                        TierRange = new TierRangeType()
                                        {
                                            Value = Interface["LnCodSisISC" + (i + 1)]
                                        },
                                        TaxScheme = new TaxSchemeType()
                                        {
                                            ID = new IDType()
                                            {
                                                Value = "2000"
                                            },
                                            Name = new NameType1()
                                            {
                                                Value = "ISC"
                                            },
                                            TaxTypeCode = new TaxTypeCodeType()
                                            {
                                                Value = "EXC"
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    };
                }
                else
                {
                    MILT[i].TaxTotal = new TaxTotalType[]{
                        new TaxTotalType()
                        {
                            TaxAmount = new TaxAmountType()
                            {
                                currencyID = Interface["Tmoneda"],
                                Value = Interface["LnMntIGV" + (i + 1)]
                            },
                            TaxSubtotal = new TaxSubtotalType[]
                            {
                                new TaxSubtotalType()
                                {
                                    TaxableAmount = new TaxableAmountType()
                                    {
                                        currencyID = Interface["Tmoneda"],
                                        Value = "0.00"
                                    },
                                    TaxAmount = new TaxAmountType()
                                    {
                                        currencyID = Interface["Tmoneda"],
                                        Value = Interface["LnMntIGV" + (i + 1)]
                                    },
                                    Percent = new PercentType()
                                    {
                                        //Credit note value
                                        //Value = "18.00"
                                        Value = igvPercent
                                    },
                                    TaxCategory = new TaxCategoryType()
                                    {
                                        TaxExemptionReasonCode = new TaxExemptionReasonCodeType()
                                        {
                                            Value = Interface["LnCodAfecIGV" + (i + 1)]
                                        },TaxScheme = new TaxSchemeType()
                                        {
                                            ID = new IDType()
                                            {
                                                Value = "1000"
                                            },
                                            Name = new NameType1()
                                            {
                                                Value = "IGV"
                                            },
                                            TaxTypeCode = new TaxTypeCodeType()
                                            {
                                                Value = "VAT"
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    };
                }

                // PRICE
                HPCT = null;
                HPCT = new PriceType();
                HXPT = null;
                HXPT = new PriceAmountType();
                HXPT.currencyID = Interface["Tmoneda"];

                HXPT.Value = Interface["LnValUnit" + (i + 1)];

                HPCT.PriceAmount = HXPT;
                MILT[i].Price = HPCT;

                // ITEM
                ItemType HIMT = new ItemType();
                DescriptionType HDT = new DescriptionType();
                DescriptionType[] MDT = { HDT };

                MDT[0].Value = "<![CDATA[" + Interface["LnDescrip" + (i + 1)] + "]]>";


                ItemIdentificationType HIIT = new ItemIdentificationType();
                HIT = null; HIT = new IDType();

                HIT.Value = "<![CDATA[" + Interface["LnCodProd" + (i + 1)] + "]]>";

                HIIT.ID = HIT;

                HIMT.Description = MDT;
                HIMT.SellersItemIdentification = HIIT;

                MILT[i].Item = HIMT;
            }
            return MILT;
        }

        public OrderReferenceType getOrderReferenceType()
        {
            OrderReferenceType ORT = null;

            for (int i = 1; i < (VD.re + 1); ++i)
            {
                if (Interface.ContainsKey("RefTpoDoc" + i))
                {
                    if (Interface["RefTpoDoc" + i] == "105")
                    {
                        ORT = new OrderReferenceType();
                        ORT.ID = new IDType()
                        {
                            Value = Interface["RefID" + i]
                        };
                        return ORT;
                    }
                }
            }
            return ORT;
        }

        public DocumentReferenceType[] getDespatchDocumentReferenceType()
        {
            DocumentReferenceType[] DRT = null;
            int n = 0;
            for (int i = 1; i < (VD.re + 1); ++i)
            {
                if (Interface.ContainsKey("RefTpoDoc" + i))
                {
                    if (Interface["RefTpoDoc" + i] == "09")
                    {
                        n++;
                    }
                }
            }

            if (n == 0)
            {
                return DRT;
            }
            else
            {
                DRT = new DocumentReferenceType[n];
                int j = 0;
                for (int i = 1; i < (VD.re + 1); ++i)
                {
                    if (Interface.ContainsKey("RefTpoDoc" + i))
                    {
                        if (Interface["RefTpoDoc" + i] == "09")
                        {
                            DRT[j] = new DocumentReferenceType()
                            {
                                ID = new IDType()
                                {
                                    Value = Interface["RefID" + i]
                                },
                                DocumentTypeCode = new DocumentTypeCodeType()
                                {
                                    Value = "09"
                                }
                            };
                            j++;
                        }
                    }
                }
            }
            return DRT;
        }

        public DocumentReferenceType[] getAdditionalDocumentReferenceType()
        {
            DocumentReferenceType[] DRT = null;
            int n = 0;
            for (int i = 1; i < (VD.re + 1); ++i)
            {
                if (Interface.ContainsKey("RefTpoDoc" + i))
                {
                    if (Interface["RefTpoDoc" + i] != "09" && Interface["RefTpoDoc" + i] != "105")
                    {
                        n++;
                    }
                }
            }

            if (n == 0)
            {
                return DRT;
            }
            else
            {
                DRT = new DocumentReferenceType[n];
                int j = 0;
                for (int i = 1; i < (VD.re + 1); ++i)
                {
                    if (Interface.ContainsKey("RefTpoDoc" + i))
                    {
                        if (Interface["RefTpoDoc" + i] != "09" && Interface["RefTpoDoc" + i] != "105")
                        {
                            DRT[j] = new DocumentReferenceType()
                            {
                                ID = new IDType()
                                {
                                    Value = Interface["RefID" + i]
                                },
                                DocumentTypeCode = new DocumentTypeCodeType()
                                {
                                    Value = Interface["RefTpoDoc" + i]
                                }
                            };
                            j++;
                        }
                    }
                }
            }
            return DRT;
        }

        static public string EncodeToISO(string toEncode)
        {
            byte[] toEncodeAsBytes = System.Text.ASCIIEncoding.ASCII.GetBytes(toEncode);
            string returnValue = System.Convert.ToBase64String(toEncodeAsBytes);
            return returnValue;
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

        public object getDeserialize(XmlElement xmlElement, System.Type tp)
        {
            Object transformedObject = null;
            try
            {
                Stream memStream = StringToStream(xmlElement.OuterXml);
                XmlSerializer serializer = new XmlSerializer(tp);
                transformedObject = serializer.Deserialize(memStream);
            }
            catch (Exception DeserializeException)
            {

            }
            return transformedObject;
        }

        public Stream StringToStream(String str)
        {
            MemoryStream memStream = null;
            try
            {
                byte[] buffer = Encoding.UTF8.GetBytes(str);//new byte[str.Length];
                memStream = new MemoryStream(buffer);
            }
            catch (Exception StringToStreamException)
            {
            }
            finally
            {
                memStream.Position = 0;
            }

            return memStream;
        }

        private XElement RemoveAllNamespaces(XElement xmlDocument)
        {
            if (!xmlDocument.HasElements)
            {
                XElement xElement = new XElement(xmlDocument.Name.LocalName);
                xElement.Value = xmlDocument.Value;
                xElement.ReplaceAttributes((from xattrib in xElement.Attributes().Where(xa => !xa.IsNamespaceDeclaration) select new XAttribute(xattrib.Name.LocalName, xattrib.Value)));
                foreach (XAttribute attribute in xmlDocument.Attributes())
                {
                    // xElement.Add(attribute);
                }
                return xElement;
            }
            return new XElement(xmlDocument.Name.LocalName, xmlDocument.Elements().Select(el => RemoveAllNamespaces(el)));
        }
        public XmlElement ToXmlElement(XElement xelement)
        {
            return new XmlDocument().ReadNode(xelement.CreateReader()) as XmlElement;
        }

        public XElement ToXElement(XmlElement xmlelement)
        {
            return XElement.Load(xmlelement.CreateNavigator().ReadSubtree());
        }

        public int InsertSignedXML()
        {
            return DBConnection.SP_Usp_InsertaXmlSign(GetBytes(File.ReadAllText(MS.XMLSFileLocation)), IdHEader);
        }

        public byte[] GetBytes(string str)
        {
            return Encoding.GetEncoding("iso-8859-1").GetBytes(str);
        }

        public string GetString(byte[] bytes)
        {
            return Encoding.GetEncoding("iso-8859-1").GetString(bytes);
        }

        public bool GenerateXML()
        {
            bool isGenerated = false;
            try
            {
                MS.GeneraLog.GeneracionXML("-----  Inicio de Generación de Archivo: " + MS.XMLIFileLocation);
                XmlSerializer mySerializer = new XmlSerializer(typeof(DebitNoteType));
                XmlSerializerNamespaces ns = getNamespaces();
                XmlWriterSettings setting = getSettings();

                DebitNoteType deb = new DebitNoteType();
                deb.AccountingSupplierParty = getSupplierPartyType();
                deb.AccountingCustomerParty = getAccountingCustomerParty();
                deb.Signature = getSignatureType();
                deb.UBLVersionID = getUBLVersionID();
                deb.CustomizationID = getCustomizationID();
                deb.ID = getID();
                deb.IssueDate = getIssueDate();
                deb.DiscrepancyResponse = getDiscrepancyResponseType();
                deb.BillingReference = getBillingReferenceType();
                deb.DocumentCurrencyCode = getDocumentCurrencyCode();
                deb.RequestedMonetaryTotal = getRequestedMonetaryTotal();
                deb.DebitNoteLine = getDebitNoteLine();
                deb.UBLExtensions = getUBLExtensions();

                if (VD.Taxes.Count > 0)
                    deb.TaxTotal = getTaxTotal();

                deb.AdditionalDocumentReference = getAdditionalDocumentReferenceType();
                deb.DespatchDocumentReference = getDespatchDocumentReferenceType();
                deb.OrderReference = getOrderReferenceType();

                XMLI = ObjectToXml(deb, ns);

                XMLI = XMLI.Replace("&lt;", "<");
                XMLI = XMLI.Replace("&gt;", ">");
                XMLI = XMLI.Replace("encoding=\"utf-16\"", "encoding=\"ISO-8859-1\"");
                XMLI = XMLI.Replace("q1:DebitNote", "DebitNote");
                XMLI = XMLI.Replace("q1", "DebitNote");
                XMLI = XMLI.Replace("xmlns=\"urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2\"", "");
                XMLI = XMLI.Replace("xmlns:DebitNote", "xmlns");

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
                MS.GeneraLog.GeneracionXML("Ocurrió un error al momento de general el XML Inicial");
                MS.GeneraLog.GeneracionXML("Error: " + ex.Message);
            }
            return isGenerated;
        }

        public static string ObjectToXml(object obj, XmlSerializerNamespaces ns)
        {
            var settings = new XmlWriterSettings
            {
                Indent = true,
                OmitXmlDeclaration = false,
                Encoding = Encoding.GetEncoding("ISO-8859-1")
            };

            var namespaces = ns;
            namespaces.Add(string.Empty, string.Empty);

            var serializer = new XmlSerializer(obj.GetType());

            using (var stringWriter = new StringWriter())
            {
                using (var xmlWriter = XmlWriter.Create(stringWriter, settings))
                {
                    serializer.Serialize(xmlWriter, obj, namespaces);
                }
                return stringWriter.ToString();
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

        public void PrintPDF()
        {
            string XML = "";

            XML += $"<DocumentState>";
            XML += $"<ID_CE>{DocumentName}</ID_CE>";
            XML += $"<Tipo_CE>{DocumentType}</Tipo_CE>";
            XML += $"<Estado_Doc>0</Estado_Doc>";
            XML += $"<Estado_Desc>Aceptado correcto</Estado_Desc>";
            XML += $"<RucEmisor>{RUCNumber}</RucEmisor>";
            XML += $"<PrintName>{Interface["Impresora"]}</PrintName>";
            XML += $"</DocumentState>";

            using (StreamWriter sw = new StreamWriter($@"D:\SLIN-ADE\ANCRO\Procesos\smp\prt\{DocumentName}.xml"))
            {
                sw.Write(XML);
            }
        }
    }
}
