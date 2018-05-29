using ADE.Extras.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ADE.Processes.DatabaseIncome.Helper
{
    public sealed class XMLGenerationNew
    {



        public object ReturnXmlObject(object obj, string typedoc)
        {
            object res = new object();

            #region

            switch (typedoc)
            {
                case Constantes.Factura:
                case Constantes.Boleta:
                    {
                        var re = new DocumentTypes2_1.Invoice();
                        re.AccountingCustomerParty = new DocumentTypes2_1.AccountingCustomerParty();
                        re.AccountingCustomerParty.Party = new DocumentTypes2_1.Party();
                        re.AccountingCustomerParty.Party.PartyName = new DocumentTypes2_1.PartyName();
                        //re.AccountingCustomerParty.Party.PartyName.Name = "";

                        //re.AccountingSupplierParty = new DocumentTypes2_1.AccountingSupplierParty();
                        //re.AccountingSupplierParty.Party = new DocumentTypes2_1.Party();
                        //re.AccountingSupplierParty.Party.PartyName = new DocumentTypes2_1.PartyName();
                        //re.AccountingSupplierParty.Party.PartyName.Name = string.Empty;

                        re.DocumentCurrencyCode = new DocumentTypes2_1.DocumentCurrencyCode();
                        re.DocumentCurrencyCode.ListAgencyName = "Texto";

                        re.InvoiceLine = new List<DocumentTypes2_1.InvoiceLine>();
                        re.InvoiceLine.Add(
                            new DocumentTypes2_1.InvoiceLine()
                            {
                                ID = "asd",
                                Item = new DocumentTypes2_1.Item()
                                {
                                    Description = "" }
                            });



                        res = new DocumentTypes2_1.Invoice();
                        XmlSerializer xmlSerial = new XmlSerializer(typeof(DocumentTypes2_1.Invoice));
                        using (TextReader reader = new StringReader(""))
                        {
                            res = (DocumentTypes2_1.Invoice)xmlSerial.Deserialize(reader);
                        }
                        break;
                    }
                case Constantes.NotaCredito:
                case Constantes.NotaDebito:
                    {
                        break;
                    }
                case Constantes.Retencion:
                    {
                        break;
                    }
                case Constantes.Perpercion:
                    {
                        break;
                    }
                case Constantes.RC:
                    {
                        break;
                    }
                case Constantes.RA:
                    {
                        break;
                    }
                case Constantes.RR:
                    {
                        break;
                    }
                case Constantes.GuiaRemision:
                    {
                        break;
                    }
            }

            #endregion


            return res;
        }
    }
}
