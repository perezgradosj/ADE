using System;
using System.Collections.Generic;

namespace ADE.Configurations.Entities.Summaries
{
    public class RBoletasCabecera
    {
        public int ID_RBC { get; set; }
        public string NUM_CPE { get; set; }
        public int TOT_DOC { get; set; }
        public DateTime FEC_INI { get; set; }
        public DateTime FEC_FIN { get; set; }
        public DateTime FEC_ENV { get; set; }
        public decimal MTO_GRA { get; set; }
        public decimal MTO_EXO { get; set; }
        public decimal MTO_INA { get; set; }
        public decimal MTO_OCA { get; set; }

        //added
        public decimal MTO_GRAT { get; set; }

        public decimal IMP_IGV { get; set; }
        public decimal IMP_ISC { get; set; }
        public decimal IMP_OTH { get; set; }
        public decimal MTO_TOT { get; set; }
        public string DOC_EST { get; set; }
        public string DOC_MSG { get; set; }
        public string DOC_TCK { get; set; }
        public int NUM_SEC { get; set; }
        public string FEC_CAD { get; set; }
        public byte[] VAR_FIR { get; set; }
        public List<string> DOC_UPD_LIST { get; set; }
        public string TYPERC { get; set; }

        public RBoletasCabecera()
        {
            ID_RBC = 0;
            NUM_CPE = "";
            TOT_DOC = 0;
            FEC_INI = DateTime.Now;
            FEC_FIN = DateTime.Now;
            FEC_ENV = DateTime.Now;
            MTO_GRA = 0.0m;
            MTO_EXO = 0.0m;
            MTO_INA = 0.0m;
            MTO_OCA = 0.0m;
            IMP_IGV = 0.0m;
            IMP_ISC = 0.0m;
            IMP_OTH = 0.0m;
            MTO_TOT = 0.0m;
            DOC_EST = "";
            DOC_MSG = "";
            DOC_TCK = "";
            NUM_SEC = 0;
            FEC_CAD = "";
            TYPERC = string.Empty;
            DOC_UPD_LIST = new List<string>();
        }
    }
}
