using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Serialization;

namespace ADE.Extras.PDFGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            //UblSharp.ApplicationResponseType response = null;
            //using (FileStream fs = File.OpenRead(@"D:\SLIN-ADE\PWC\ProcesoCE\CDR\R-20106896276-01-F007-00003717.xml"))
            //{
            //    XmlSerializer xs = new XmlSerializer(typeof(UblSharp.ApplicationResponseType));
            //    fs.Position = 0;
            //    response = (UblSharp.ApplicationResponseType)xs.Deserialize(fs);
            //}
            //if (response != null)
            //{
            //    if (response.Note != null)
            //    {

            //    }
            //}
            //if (response.DocumentResponse != null)
            //{

            //}

            //XmlDocument xml = new XmlDocument();
            //using (StreamReader sr = new StreamReader(@"D:\SLIN-ADE\PWC\ProcesoCE\CDR\R-20106896276-01-F007-00003717.xml"))
            //{
            //    xml.LoadXml(sr.ReadToEnd());
            //}

            //XmlNodeList nList = xml.SelectNodes("/ar:ApplicationResponse/cbc:Note");


            //// suppose that myXmlString contains "<Names>...</Names>"


            //foreach (XmlNode xn in nList)
            //{
                
            //}

            XmlDocument doc = new XmlDocument();
            doc.Load(@"D:\SLIN-ADE\PWC\ProcesoCE\CDR\R-20106896276-01-F007-00003717.xml");

            XmlNamespaceManager ns = new XmlNamespaceManager(doc.NameTable);
            ns.AddNamespace("soap", "http://schemas.xmlsoap.org/soap/envelope/");
            ns.AddNamespace("ext", "urn:oasis:names:specification:ubl:schema:xsd:CommonExtensionComponents-2");
            ns.AddNamespace("ds", "http://www.w3.org/2000/09/xmldsig#");
            ns.AddNamespace("cbc", "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2");
            ns.AddNamespace("cac", "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2");
            ns.AddNamespace("ar", "urn:oasis:names:specification:ubl:schema:xsd:ApplicationResponse-2");

            XmlNodeList nList = doc.SelectNodes("ar:ApplicationResponse/cbc:Note", ns);

            foreach(XmlNode note in nList)
            {
                Console.WriteLine(note.InnerText);
            }

            XmlNode drRCList = doc.SelectSingleNode("ar:ApplicationResponse/cac:DocumentResponse/cac:Response/cbc:ResponseCode", ns);
            XmlNode drDEList = doc.SelectSingleNode("ar:ApplicationResponse/cac:DocumentResponse/cac:Response/cbc:Description", ns);

            if(drRCList != null && drDEList != null)
            {
                Console.WriteLine("ResponseCode: " + drRCList.InnerText);
                Console.WriteLine("Description : " + drDEList.InnerText);
            }


        }
    }

        //public static string GetTC(string SummaryDate)
        //{
        //    string venta = "0.00000";
        //    string[] Fec = SummaryDate.Split('-');
        //    int pos = -1;
        //    HtmlAgilityPack.HtmlWeb web = new HtmlAgilityPack.HtmlWeb();
        //    HtmlAgilityPack.HtmlDocument doc = web.Load("http://www.sunat.gob.pe/cl-at-ittipcam/tcS01Alias?mes=" + Fec[1] + "&anho=" + Fec[0]);
        //    int j = 0;
        //    List<string> D = new List<string>();
        //    List<string> C = new List<string>();
        //    List<string> V = new List<string>();
            
        //    try
        //    {
        //        var Days = doc.DocumentNode.SelectNodes("//td[@class='H3']").ToList();
        //        var Rate = doc.DocumentNode.SelectNodes("//td[@class='tne10']").ToList();

        //        for(int i = 0; i < Days.Count; ++i)
        //        {
        //            D.Add(Days[i].InnerText.Trim());
        //            C.Add(Rate[j].InnerText.Trim());
        //            V.Add(Rate[j + 1].InnerText.Trim());
        //            j = j + 2;
        //            if (int.Parse(Days[i].InnerText.Trim()) <= int.Parse(Fec[2]))
        //            {
        //                pos = i;
        //            }
        //            else
        //            {
        //                if(i == 0)
        //                {
        //                    pos = -1;
        //                }
        //                i = Days.Count + 1;
        //            }
        //        }
        //        venta = V[pos];
        //    }
        //    catch (Exception e)
        //    {
        //        try
        //        {
        //            //var Warn = doc.DocumentNode.SelectNodes("//font[@class='warn']").ToList();
        //            //if(Warn.Count > 0)
        //            if (pos == -1)
        //            {
        //                Fec = GetPastMonth(SummaryDate);
        //                doc = web.Load("http://www.sunat.gob.pe/cl-at-ittipcam/tcS01Alias?mes=" + Fec[1] + "&anho=" + Fec[0]);
        //                var Days = doc.DocumentNode.SelectNodes("//td[@class='H3']").ToList();
        //                var Rate = doc.DocumentNode.SelectNodes("//td[@class='tne10']").ToList();
        //                j = 0;
        //                D = null; D = new List<string>();
        //                C = null; C = new List<string>();
        //                V = null; V = new List<string>();

        //                for (int i = 0; i < Days.Count; ++i)
        //                {
        //                    D.Add(Days[i].InnerText.Trim());
        //                    C.Add(Rate[j].InnerText.Trim());
        //                    V.Add(Rate[j + 1].InnerText.Trim());
        //                    j = j + 2;
        //                }
        //                pos = V.Count - 1;
        //                venta = V[pos];
        //            }
        //        }
        //        catch (Exception d)
        //        {

        //        }
        //    }
        //    return venta;
        //}

        //public static string[] GetPastMonth(string SummaryDate)
        //{
        //    string[] res;
        //    string[] Fec = SummaryDate.Split('-');
        //    if (Fec[1] == "01")
        //    {
        //        Fec[0] = ("" + (int.Parse(Fec[0]) - 1));
        //        Fec[1] = "12";
        //    }
        //    else
        //    {
        //        Fec[1] = ("" + (int.Parse(Fec[1]) - 1)).PadLeft(2,'0'); ;
        //    }
        //    res = Fec;
        //    return res;
        //}


    [DataContract]
    public class TipoCambio{
        [DataMember]
        public string dias { get; set; }

        [DataMember]
        public string compra { get; set; }

        [DataMember]
        public string venta { get; set; }
    }

}
