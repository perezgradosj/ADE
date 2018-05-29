using ADE.Configurations.DataAccess;
using ADE.Configurations.Entities;
using ADE.Configurations.Objects;
using ADE.Processes.DatabaseIncome;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace eProcessRetencion
{
    public class ERetention
    {
        /*
         * CLASE ERetention [UBLPE-Retention-2.0.cs]
         * ----------------------------------------------------------------------------------
         * Author       :   Johan Espinoza   
         * Date         :   22-07-2015
         * Proyect      :   Retención Electrónica
         * Description  :   Esta clase simplifica la creación de los objetos pertenecientes a la clase InvoiceType,
         *                  como pueden ser UBLExtensions, AccountingSupplierParty, AccountingCustomerParty, 
         *                  Signature, TaxTotal, UBLVersionID, CustomizationID, etc.
         */

        private Dictionary<string, string[]> fDict = new Dictionary<string, string[]>();
        private Encoding ISO_8859_1 = Encoding.GetEncoding("ISO-8859-1");
        public int IdHEader = 0;
        public string XMLI = "";
        public Dictionary<string, string> Interface = new Dictionary<string, string>();
        public string DocumentName, RUCNumber, DocumentType;
        public List<string> listaerror = new List<string>();
        DatabaseConnection DBConnection = null;
        MainSettings MS = null;
        Validation VD = null;
        public DBDocument Document = new DBDocument();
        public Stream SXMLI = null;

        public ERetention(MainSettings MSI, Validation VDI)
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

        public DocumentCurrencyCodeType getDocumentCurrencyCode()
        {
            return new DocumentCurrencyCodeType()
            {
                Value = Interface["Tmoneda"]
            };
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

        public PartyType getAgentParty()
        {
            var urb = Interface["EmiDirUrbani"];

            if (urb.Length > 0)
            {
                #region if

                return new PartyType()
                {
                    PartyIdentification = new PartyIdentificationType[]
                {
                    new PartyIdentificationType()
                    {
                        ID = new IDType()
                        {
                            schemeID = Interface["EmiTpoDoc"],
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
                };

                #endregion
            }
            else
            {
                #region else

                return new PartyType()
                {
                    PartyIdentification = new PartyIdentificationType[]
                {
                    new PartyIdentificationType()
                    {
                        ID = new IDType()
                        {
                            schemeID = Interface["EmiTpoDoc"],
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
                        //CitySubdivisionName = new CitySubdivisionNameType()
                        //{
                        //    Value = Interface["EmiDirUrbani"]
                        //},
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
                };

                #endregion
            }
        }

        public PartyType getReceiverParty()
        {
            var urb = Interface["RecDirUrbani"];
            if (urb.Length > 0)
            {
                #region if

                return new PartyType()
                {
                    PartyIdentification = new PartyIdentificationType[]
                {
                    new PartyIdentificationType()
                    {
                        ID = new IDType()
                        {
                            schemeID = Interface["RecTpoDoc"],
                            Value = Interface["RecNumDocu"]
                        }
                    }
                },
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
                };

                #endregion
            }
            else
            {
                #region else
                return new PartyType()
                {
                    PartyIdentification = new PartyIdentificationType[]
                {
                    new PartyIdentificationType()
                    {
                        ID = new IDType()
                        {
                            schemeID = Interface["RecTpoDoc"],
                            Value = Interface["RecNumDocu"]
                        }
                    }
                },
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
                        //CitySubdivisionName = new CitySubdivisionNameType()
                        //{
                        //    Value = Interface["RecDirUrbani"]
                        //},
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
                };

                #endregion
            }
        }

        public UBLExtensionType[] getUBLExtensions()
        {
            string EXT1 = $"<AdditionalInformationSLIN><Document><ID>{Interface["NUM_CE"]}</ID></Document>";

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

            return new UBLExtensionType[]
            {
                new UBLExtensionType()
                {
                    ExtensionContent = EXT1
                },
                new UBLExtensionType()
                {
                    ExtensionContent = EXT2
                }
            };
        }

        public IDType getSUNATRetentionSystemCode()
        {
            return new IDType()
            {
                Value = Interface["RegimenCE"]
            };
        }

        public PercentType getSUNATRetentionPercent()
        {
            return new PercentType()
            {
                Value = Interface["TasaCE"]
            };
        }

        public NoteType[] getNote()
        {
            return new NoteType[]{
                new NoteType()
                {
                    Value = "<![CDATA[" +  Interface["ObsCE"] + "]]>"
                }
            };
        }

        public TotalInvoiceAmountType getTotalInvoiceAmount()
        {
            return new TotalInvoiceAmountType()
            {
                currencyID = Interface["MonImpTotCE"],
                Value = Interface["ImpTotCE"]
            };
        }

        public AmountType1 getSUNATTotalPaid()
        {
            return new AmountType1()
            {
                currencyID = Interface["MonImpTot"],
                Value = Interface["ImpTot"]
            };
        }

        public SUNATRetentionDocumentReferenceType[] getSUNATRetentionDocumentReference()
        {
            SUNATRetentionDocumentReferenceType[] DocumentReference = new SUNATRetentionDocumentReferenceType[VD.it];

            for (int i = 0; i < VD.it; ++i)
            {
                int j = i + 1;
                DocumentReference[i] = new SUNATRetentionDocumentReferenceType()
                {
                    ID = new IDType()
                    {
                        schemeID = Interface["TpoDocRelac" + j],
                        Value = Interface["NumDocRelac" + j]
                    },
                    IssueDate = new IssueDateType()
                    {
                        Value = DateTime.Parse(Interface["FEmisDocRelac" + j])
                    },
                    TotalInvoiceAmount = new TotalInvoiceAmountType()
                    {
                        currencyID = Interface["MDocRelac" + j],
                        Value = Interface["ITotDocRelac" + j]
                    },
                    Payment = new PaymentType()
                    {
                        ID = new IDType()
                        {
                            Value = Interface["NumMovi" + j]
                        },
                        PaidAmount = new PaidAmountType()
                        {
                            currencyID = Interface["MonMovi" + j],
                            Value = Interface["ImpSOperMov" + j]
                        },
                        PaidDate = new PaidDateType()
                        {
                            Value = DateTime.Parse(Interface["FecMovi" + j])
                        }
                    },
                    SUNATRetentionInformation = new SUNATRetentionInformationType()
                    {
                        SUNATRetentionAmount = new AmountType1()
                        {
                            currencyID = Interface["MonImpOper" + j],
                            Value = Interface["ImpOper" + j]
                        },
                        SUNATRetentionDate = new SUNATRetentionDateType()
                        {
                            Value = DateTime.Parse(Interface["FecOper" + j])
                        },
                        SUNATNetTotalPaid = new AmountType1()
                        {
                            currencyID = Interface["MonOper" + j],
                            Value = Interface["ImpTotOper" + j]
                        }
                    }
                };
                if (Interface["MonRefeTC" + j] != "" || Interface["MonDestTC" + j] != "" || Interface["FactorTC" + j] != "" || Interface["FechaTC" + j] != "")
                {
                    ExchangeRateType EXRT = new ExchangeRateType();

                    if (Interface["MonRefeTC" + j] != "")
                    {
                        EXRT.SourceCurrencyCode = new SourceCurrencyCodeType()
                        {
                            Value = Interface["MonRefeTC" + j]
                        };
                    }
                    if (Interface["MonDestTC" + j] != "")
                    {
                        EXRT.TargetCurrencyCode = new TargetCurrencyCodeType()
                        {
                            Value = Interface["MonDestTC" + j]
                        };
                    }
                    if (Interface["FactorTC" + j] != "")
                    {
                        if (isDecimal(Interface["FactorTC" + j]))
                            EXRT.CalculationRate = new CalculationRateType()
                            {
                                Value = Interface["FactorTC" + j]
                            };

                    }
                    if (Interface["FechaTC" + j] != "")
                    {
                        if (isDate(Interface["FechaTC" + j]))
                            EXRT.Date = new DateType()
                            {
                                Value = DateTime.Parse(Interface["FechaTC" + j])
                            };
                    }
                    DocumentReference[i].SUNATRetentionInformation.ExchangeRate = EXRT;
                }
            }
            return DocumentReference;
        }

        static public string EncodeToISO(string toEncode)
        {
            byte[] toEncodeAsBytes
                  = System.Text.ASCIIEncoding.ASCII.GetBytes(toEncode);
            string returnValue
                  = System.Convert.ToBase64String(toEncodeAsBytes);
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
                XmlSerializer mySerializer = new XmlSerializer(typeof(RetentionType));
                XmlSerializerNamespaces ns = getNamespaces();
                XmlWriterSettings setting = getSettings();

                RetentionType ret = new RetentionType();

                ret.UBLExtensions = getUBLExtensions();
                ret.UBLVersionID = getUBLVersionID();
                ret.CustomizationID = getCustomizationID();
                ret.Signature = getSignatureType();
                ret.ID = getID();
                ret.IssueDate = getIssueDate();
                ret.AgentParty = getAgentParty();
                ret.ReceiverParty = getReceiverParty();
                ret.SUNATRetentionSystemCode = getSUNATRetentionSystemCode();
                ret.SUNATRetentionPercent = getSUNATRetentionPercent();
                ret.Note = getNote();
                ret.TotalInvoiceAmount = getTotalInvoiceAmount();
                ret.SUNATTotalPaid = getSUNATTotalPaid();
                ret.SUNATRetentionDocumentReference = getSUNATRetentionDocumentReference();

                StringWriterWithEncoding iso88591StringWriter = new StringWriterWithEncoding(ISO_8859_1);
                XmlSerializer serializer = new XmlSerializer(typeof(RetentionType));
                XmlWriter writerx = XmlWriter.Create(iso88591StringWriter, setting);
                serializer.Serialize(writerx, ret, ns);

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
                MS.GeneraLog.GeneracionXML("Ocurrió un error al momento de general el XML Inicial");
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
