using ADE.Configurations.DataAccess;
using ADE.Configurations.Entities;
using ADE.Configurations.Objects;
using eProcessFactura;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace ADE.Processes.DatabaseIncome.DocumentTypesNew
{
    public class EInvoice_2_1
    {
        /*
         * CLASE Efactura [UBLPE-Invoice-2.0.cs]
         * ----------------------------------------------------------------------------------
         * Author       :   Josué PA   
         * Date         :   05-12-2017
         * Proyect      :   Facturación Electrónica
         * Description  :   Esta clase simplifica la creación de los objetos pertenecientes a la clase InvoiceType,
         *                  como pueden ser UBLExtensions, AccountingSupplierParty, AccountingCustomerParty, 
         *                  Signature, TaxTotal, UBLVersionID, CustomizationID, etc. en la Versión 2.1
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

        public EInvoice_2_1(MainSettings MSI, Validation VDI)
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

        public InvoiceTypeCodeType getInvoiceTypeCode()
        {
            return new InvoiceTypeCodeType()
            {
                Value = Interface["TipoCE"]
            };
        }

        public DocumentCurrencyCodeType getDocumentCurrencyCode()
        {
            return new DocumentCurrencyCodeType()
            {
                Value = Interface["Tmoneda"]
            };
        }

        public MonetaryTotalType getLegalMonetaryTotal()
        {
            double d = 0, o = 0, a = 0;
            MonetaryTotalType mtt = new MonetaryTotalType()
            {
                PayableAmount = new PayableAmountType()
                {
                    currencyID = Interface["Tmoneda"],
                    Value = Interface["TotImporTotal"]
                }
            };

            if (Interface["TotDctoGlobal"] != "" && double.TryParse(Interface["TotDctoGlobal"], out d))
            {
                if (d > 0)
                {
                    mtt.AllowanceTotalAmount = new AllowanceTotalAmountType()
                    {
                        currencyID = Interface["Tmoneda"],
                        Value = Interface["TotDctoGlobal"]
                    };
                }
            }

            if (Interface["TotSumOCargo"] != "" && double.TryParse(Interface["TotSumOCargo"], out o))
            {
                if (o > 0)
                {
                    mtt.ChargeTotalAmount = new ChargeTotalAmountType()
                    {
                        currencyID = Interface["Tmoneda"],

                        Value = Interface["TotSumOCargo"]
                    };
                }
            }

            if (Interface["TotAnticipo"] != "" && double.TryParse(Interface["TotAnticipo"], out a))
            {
                if (a > 0)
                {
                    mtt.PrepaidAmount = new PrepaidAmountType()
                    {
                        currencyID = Interface["Tmoneda"],
                        Value = Interface["TotAnticipo"]
                    };
                }
            }

            return mtt;
        }

        public CustomizationIDType getCustomizationID()
        {
            return new CustomizationIDType()
            {
                Value = "1.1"
            };
        }

        public UBLVersionIDType getUBLVersionID()
        {
            return new UBLVersionIDType()
            {
                Value = "2.1"
            };
        }

        public TaxTotalType[] getTaxTotal()
        {
            if (VD.Taxes.Count == 0)
            {
                VD.Taxes.Add(6);
            }
            TaxTotalType[] txp = new TaxTotalType[VD.Taxes.Count];
            string code, taxe, last;
            string valu = "0.00";

            for (int i = 0; i < VD.Taxes.Count; ++i)
            {
                code = taxe = last = ""; valu = "0.00";
                if (VD.Taxes[i] == 6) { code = "1000"; taxe = "IGV"; last = "VAT"; valu = Interface["TotSumIGV"]; }
                if (VD.Taxes[i] == 7) { code = "2000"; taxe = "ISC"; last = "EXC"; valu = Interface["TotSumISC"]; }
                if (VD.Taxes[i] == 8) { code = "9999"; taxe = "OTROS"; last = "OTH"; valu = Interface["TotSumOTrib"]; }

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

        public SupplierPartyType getSellerSupplierPartyType()
        {
            if (Interface["IdTpoOperacion"] == "05")
            {
                return new SupplierPartyType()
                {
                    Party = new PartyType()
                    {
                        PostalAddress = new AddressType()
                        {
                            ID = new IDType()
                            {
                                Value = Interface["LugUbigeo"]
                            },
                            StreetName = new StreetNameType()
                            {
                                Value = Interface["LugDirFiscal"]
                            },
                            CitySubdivisionName = new CitySubdivisionNameType()
                            {
                                Value = Interface["LugDirUrbani"]
                            },
                            CityName = new CityNameType()
                            {
                                Value = Interface["LugDirProvin"]
                            },
                            CountrySubentity = new CountrySubentityType()
                            {
                                Value = Interface["LugDirDepart"]
                            },
                            District = new DistrictType()
                            {
                                Value = Interface["LugDirDistrito"]
                            },
                            Country = new CountryType()
                            {
                                IdentificationCode = new IdentificationCodeType()
                                {
                                    Value = Interface["LugCodPais"]
                                }
                            },
                            AddressTypeCode = new AddressTypeCodeType()
                            {
                                Value = Interface["LugAnex"]
                            }
                        }
                    }
                };
            }
            else
            {
                return null;
            }
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
                            Value = "<![CDATA[" + Interface["EmiDirFiscal"] + "]]>"
                        },
                        CitySubdivisionName = new CitySubdivisionNameType()
                        {
                            Value = "<![CDATA[" + Interface["EmiDirUrbani"] + "]]>"
                        },
                        CityName = new CityNameType()
                        {
                            Value = "<![CDATA[" + Interface["EmiDirProvin"] + "]]>"
                        },
                        CountrySubentity = new CountrySubentityType()
                        {
                            Value = "<![CDATA[" + Interface["EmiDirDepart"] + "]]>"
                        },
                        District = new DistrictType()
                        {
                            Value = "<![CDATA[" + Interface["EmiDirDistrito"] + "]]>"
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
                            Value = "<![CDATA[" + Interface["RecDirFiscal"] + "]]>"
                        },

                        CitySubdivisionName = new CitySubdivisionNameType()
                        {
                            Value = "<![CDATA[" + Interface["RecDirUrbani"] + "]]>"
                        },

                        CityName = new CityNameType()
                        {
                            Value = "<![CDATA[" + Interface["RecDirProvin"] + "]]>"
                        },
                        CountrySubentity = new CountrySubentityType()
                        {
                            Value = "<![CDATA[" + Interface["RecDirDepart"] + "]]>"
                        },
                        District = new DistrictType()
                        {
                            Value = "<![CDATA[" + Interface["RecDirDistrito"] + "]]>"
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

        public CustomerPartyType getAccountingCustomerParty(string val)
        {
            if (val.Length > 0)
            {
                #region if

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
                                Value = "<![CDATA[" + Interface["RecDirFiscal"] + "]]>"
                            },

                            CitySubdivisionName = new CitySubdivisionNameType()
                            {
                                Value = "<![CDATA[" + Interface["RecDirUrbani"] + "]]>"
                            },

                            CityName = new CityNameType()
                            {
                                Value = "<![CDATA[" + Interface["RecDirProvin"] + "]]>"
                            },
                            CountrySubentity = new CountrySubentityType()
                            {
                                Value = "<![CDATA[" + Interface["RecDirDepart"] + "]]>"
                            },
                            District = new DistrictType()
                            {
                                Value = "<![CDATA[" + Interface["RecDirDistrito"] + "]]>"
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

                #endregion
            }
            else
            {
                #region else

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
                                Value = "<![CDATA[" + Interface["RecDirFiscal"] + "]]>"
                            },

                            //CitySubdivisionName = new CitySubdivisionNameType()
                            //{
                            //    Value = "<![CDATA[" + Interface["RecDirUrbani"] + "]]>"
                            //},

                            CityName = new CityNameType()
                            {
                                Value = "<![CDATA[" + Interface["RecDirProvin"] + "]]>"
                            },
                            CountrySubentity = new CountrySubentityType()
                            {
                                Value = "<![CDATA[" + Interface["RecDirDepart"] + "]]>"
                            },
                            District = new DistrictType()
                            {
                                Value = "<![CDATA[" + Interface["RecDirDistrito"] + "]]>"
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
                #endregion
            }
        }

        public PaymentType[] getPrepaidPayment()
        {
            PaymentType[] PT = new PaymentType[VD.an];
            for (int i = 0; i < VD.an; ++i)
            {
                int j = i + 1;
                PT[i] = new PaymentType()
                {
                    ID = new IDType()
                    {
                        schemeID = Interface["AntTpoDocAnt" + j],
                        Value = Interface["AntIdDocAnt" + j]
                    },
                    PaidAmount = new PaidAmountType()
                    {
                        currencyID = Interface["Tmoneda"],
                        Value = Interface["AntMonto" + j]
                    },
                    InstructionID = new InstructionIDType()
                    {
                        schemeID = Interface["AntTpoDocEmi" + j],
                        Value = Interface["AntNumDocEmi" + j]
                    }
                };
            }

            return PT;
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
                         $"<cbc:PayableAmount currencyID=\"{Interface["Tmoneda"]}\">{Interface["TotVtaGrab"]}</cbc:PayableAmount>" +    //Graavada
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

            if (Interface["TotVtaGrat"] != "" && Interface["TotVtaGrat"] != "0.00")
            {
                if (double.Parse(Interface["TotVtaGrat"]) > 0)
                {
                    EXT2 += "<sac:AdditionalMonetaryTotal>" +
                            "<cbc:ID>1004</cbc:ID>" +
                            $"<cbc:PayableAmount currencyID=\"{Interface["Tmoneda"]}\">{Interface["TotVtaGrat"]}</cbc:PayableAmount>" +  //Gratuita
                        "</sac:AdditionalMonetaryTotal>";
                }
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
                if (double.Parse(Interface["TotTotDscto"]) > 0)
                {
                    EXT2 += "<sac:AdditionalMonetaryTotal>" +
                            "<cbc:ID>2005</cbc:ID>" +
                           $"<cbc:PayableAmount currencyID=\"{Interface["Tmoneda"]}\">{Interface["TotTotDscto"]}</cbc:PayableAmount>" +  //Total Descuentos
                        "</sac:AdditionalMonetaryTotal>";
                }
            }
            EXT2 += "<sac:AdditionalProperty>" +
                          "<cbc:ID>1000</cbc:ID>" +
                         $"<cbc:Value>{Interface["MontoLiteral"]}</cbc:Value>" +
                      "</sac:AdditionalProperty>";



            grav = Interface["TotVtaGrat"].ToString().Length > 0 ? Convert.ToDecimal(Interface["TotVtaGrat"].ToString()) : 0.0m;
            inafec = Interface["TotVtaInaf"].ToString().Length > 0 ? Convert.ToDecimal(Interface["TotVtaGrat"].ToString()) : 0.0m;
            exoner = Interface["TotVtaExon"].ToString().Length > 0 ? Convert.ToDecimal(Interface["TotVtaGrat"].ToString()) : 0.0m;

            if ((grav + exoner + inafec) <= 0)
            {
                if (Interface["TotVtaGrat"] != "" && Interface["TotVtaGrat"] != "0.00")
                {
                    if (double.Parse(Interface["TotVtaGrat"]) > 0)
                    {
                        EXT2 += "<sac:AdditionalProperty>" +
                              "<cbc:ID>1002</cbc:ID>" +
                              "<cbc:Value>TRANSFERENCIA GRATUITA DE UN BIEN Y/O SERVICIO PRESTADO GRATUITAMENTE</cbc:Value>" +
                          "</sac:AdditionalProperty>";
                    }
                }
            }

            if (p1 > 0 && p2 > 0 && p3 > 0)
            {
                EXT2 += "<sac:AdditionalProperty>" +
                          "<cbc:ID>2000</cbc:ID>" +
                         $"<cbc:Value>COMPROBANTE DE PERCEPCION</cbc:Value>" +
                      "</sac:AdditionalProperty>";
            }

            if (Interface["IdTpoOperacion"] == "05")
            {
                EXT2 += "<sac:AdditionalProperty>" +
                          "<cbc:ID>3000</cbc:ID>" +
                         $"<cbc:Value>Venta realizada por emisor itinerante</cbc:Value>" +
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

            if (Interface.ContainsKey("IdTpoOperacion"))
            {
                if (Interface["IdTpoOperacion"] != "")
                {
                    if (Interface["IdTpoOperacion"] == "06")
                    {
                        EXT2 += $"<sac:SUNATEmbededDespatchAdvice>" +
                                  $"<cac:DeliveryAddress>" +
                                    $"<cbc:ID>070101</cbc:ID>" +
                                    $"<cbc:StreetName>AV. REPUBLICA DE ARGENTINA N? 2976 URB.</cbc:StreetName>" +
                                    $"<cbc:CitySubdivisionName>CALLAO 1</cbc:CitySubdivisionName>" +
                                    $"<cbc:CityName>LIMA</cbc:CityName>" +
                                    $"<cbc:CountrySubentity>LIMA</cbc:CountrySubentity>" +
                                    $"<cbc:District>LIMA</cbc:District>" +
                                    $"<cac:Country>" +
                                      $"<cbc:IdentificationCode>PE</cbc:IdentificationCode>" +
                                    $"</cac:Country>" +
                                  $"</cac:DeliveryAddress>" +
                                  $"<cac:OriginAddress>" +
                                    $"<cbc:ID>070101</cbc:ID>" +
                                    $"<cbc:StreetName>AV. REPUBLICA DE ARGENTINA N? 2976 URB.</cbc:StreetName>" +
                                    $"<cbc:CitySubdivisionName>CALLAO 1</cbc:CitySubdivisionName>" +
                                    $"<cbc:CityName>LIMA</cbc:CityName>" +
                                    $"<cbc:CountrySubentity>LIMA</cbc:CountrySubentity>" +
                                    $"<cbc:District>LIMA</cbc:District>" +
                                    $"<cac:Country>" +
                                      $"<cbc:IdentificationCode>PE</cbc:IdentificationCode>" +
                                    $"</cac:Country>" +
                                  $"</cac:OriginAddress>" +
                                  $"<sac:SUNATCarrierParty>" +
                                    $"<cbc:CustomerAssignedAccountID>30100006376</cbc:CustomerAssignedAccountID>" +
                                    $"<cbc:AdditionalAccountID>06</cbc:AdditionalAccountID>" +
                                    $"<cac:Party>" +
                                      $"<cac:PartyLegalEntity>" +
                                        $"<cbc:RegistrationName>Nombre transportista</cbc:RegistrationName>" +
                                      $"</cac:PartyLegalEntity>" +
                                    $"</cac:Party>" +
                                  $"</sac:SUNATCarrierParty>" +
                                  $"<sac:DriverParty>" +
                                    $"<cac:Party>" +
                                      $"<cac:PartyIdentification>" +
                                        $"<cbc:ID>1111111111</cbc:ID>" +
                                      $"</cac:PartyIdentification>" +
                                    $"</cac:Party>" +
                                  $"</sac:DriverParty>" +
                                  $"<sac:SUNATRoadTransport>" +
                                    $"<cbc:LicensePlateID>B9Y-778</cbc:LicensePlateID>" +
                                    $"<cbc:TransportAuthorizationCode>1</cbc:TransportAuthorizationCode>" +
                                    $"<cbc:BrandName>Scania</cbc:BrandName>" +
                                  $"</sac:SUNATRoadTransport>" +
                                  $"<cbc:TransportModeCode>01</cbc:TransportModeCode>" +
                                  $"<cbc:GrossWeightMeasure unitCode=\"KG\">10.00</cbc:GrossWeightMeasure>" +
                                $"</sac:SUNATEmbededDespatchAdvice>";
                    }
                    else
                    {
                        EXT2 += "<sac:SUNATTransaction>" +
                                 $"<cbc:ID>{Interface["IdTpoOperacion"]}</cbc:ID>" +
                                "</sac:SUNATTransaction>";
                    }
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

        public InvoiceLineType[] getInvoiceLine()
        {
            InvoiceLineType HILT = new InvoiceLineType();
            InvoiceLineType[] MILT = new InvoiceLineType[VD.li];

            for (int i = 0; i < VD.li; ++i)
            {
                MILT[i] = new InvoiceLineType();

                IDType HIT = new IDType();
                HIT.Value = (i + 1).ToString();
                MILT[i].ID = HIT;

                InvoicedQuantityType HIQT = new InvoicedQuantityType();
                HIQT.unitCode = Interface["LnUndMed" + (i + 1)];
                HIQT.unitCodeSpecified = true;
                HIQT.Value = Interface["LnCantidad" + (i + 1)];
                MILT[i].InvoicedQuantity = HIQT;

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

                if (Interface["LnDescMnto" + (i + 1)] != "" && Interface["LnDescMnto" + (i + 1)] != "0.00")
                {
                    AllowanceChargeType[] HWCT = new AllowanceChargeType[1];
                    HWCT[0] = new AllowanceChargeType();

                    HWCT[0].ChargeIndicator = new ChargeIndicatorType
                    {
                        Value = false
                    };
                    HWCT[0].Amount = new AmountType1
                    {
                        currencyID = Interface["Tmoneda"],
                        Value = Interface["LnDescMnto" + (i + 1)]
                    };
                    MILT[i].AllowanceCharge = HWCT;
                }

                //TAXT TOTAL - Prueba con el nuevo formato
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
                                        Value = igvPercent
                                        //Value = "18.00"
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

                // PRICE 20503520350 C10589
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
                XmlSerializer mySerializer = new XmlSerializer(typeof(InvoiceType));
                XmlSerializerNamespaces ns = getNamespaces();
                XmlWriterSettings setting = getSettings();

                InvoiceType inv = new InvoiceType();
                inv.SellerSupplierParty = getSellerSupplierPartyType();
                inv.AccountingSupplierParty = getSupplierPartyType();
                //inv.AccountingCustomerParty = getAccountingCustomerParty();
                inv.AccountingCustomerParty = getAccountingCustomerParty();
                inv.PrepaidPayment = getPrepaidPayment();
                inv.Signature = getSignatureType();
                inv.UBLVersionID = getUBLVersionID();
                inv.CustomizationID = getCustomizationID();
                inv.ID = getID();
                inv.IssueDate = getIssueDate();
                inv.InvoiceTypeCode = getInvoiceTypeCode();
                inv.DocumentCurrencyCode = getDocumentCurrencyCode();
                inv.LegalMonetaryTotal = getLegalMonetaryTotal();
                inv.InvoiceLine = getInvoiceLine();
                inv.UBLExtensions = getUBLExtensions();
                inv.AdditionalDocumentReference = getAdditionalDocumentReferenceType();
                inv.DespatchDocumentReference = getDespatchDocumentReferenceType();
                inv.OrderReference = getOrderReferenceType();
                inv.TaxTotal = getTaxTotal();

                StringWriterWithEncoding iso88591StringWriter = new StringWriterWithEncoding(ISO_8859_1);
                XmlSerializer serializer = new XmlSerializer(typeof(InvoiceType));
                XmlWriter writerx = XmlWriter.Create(iso88591StringWriter, setting);
                serializer.Serialize(writerx, inv, ns);

                XMLI = iso88591StringWriter.ToString();

                XMLI = XMLI.Replace("&lt;", "<");
                XMLI = XMLI.Replace("&gt;", ">");
                XMLI = XMLI.Replace("&amp;", "&");

                using (StreamWriter sw = new StreamWriter(MS.XMLIFileLocation, false, ISO_8859_1))
                {
                    sw.Write(XMLI);
                }


                SXMLI = GenerateStreamFromString(XMLI);
                //Stream SX = null;
                //SXMLI.Seek(0, SeekOrigin.Begin);
                //SXMLI.CopyTo(SX);

                //using (var fileStream = File.Create(MS.XMLIFileLocation))
                //{
                //    SX.Seek(0, SeekOrigin.Begin);
                //    SX.CopyTo(fileStream);
                //}

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
            // 01-06-2017 SE AGREGÓ EL ENCODING
            //StreamWriter writer = new StreamWriter(stream, ISO_8859_1);
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

            using (StreamWriter sw = new StreamWriter($@"{MS.ADE_ROOT}Procesos\smp\prt\{DocumentName}.xml"))
            {
                sw.Write(XML);
            }
        }

    }
}
