using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADE.Entities.Database
{
    public class Document
    {
        public int Id { get; set; }
        public string TypeDoc { get; set; }
        public string Serie { get; set; }
        public string Correlativo { get; set; }
        public string Num_CE { get; set; }
        public DateTime Fec_Emi { get; set; }
        public DateTime Fec_Res { get; set; }
        public string Rzn_Soc_Cli { get; set; }
        public string Rzn_Soc_Emi { get; set; }
        public string TO { get; set; }
        public string CC { get; set; }
        public string CCO { get; set; }
        public string Notify { get; set; }
        public int Cant_Docs { get; set; }
        public string Email { get; set; }
        public string Pwd { get; set; }
        public string Domain { get; set; }
        public string IP { get; set; }
        public int Port { get; set; }
        public int UseSSL { get; set; }
        public string Url_Logo { get; set; }
        public string TypeRC { get; set; }

        public Document()
        {
            Id = 0;
            TypeDoc = string.Empty;
            Serie = string.Empty;
            Correlativo = string.Empty;
            Num_CE = string.Empty;
            Fec_Emi = Convert.ToDateTime("01/01/1990");
            Fec_Res = Convert.ToDateTime("01/01/1990");
            Rzn_Soc_Cli = string.Empty;
            Rzn_Soc_Emi = string.Empty;
            TO = string.Empty;
            CC = string.Empty;
            CCO = string.Empty;
            Notify = string.Empty;
            Cant_Docs = 0;
            Email = string.Empty;
            Pwd = string.Empty;
            Domain = string.Empty;
            IP = string.Empty;
            Port = 0;
            UseSSL = 0;
            Url_Logo = string.Empty;
            TypeRC = string.Empty;
        }
    }

    public class ListDocument : List<Document>
    {

    }
}
