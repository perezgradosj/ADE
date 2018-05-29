using System;
using System.Collections.Generic;

namespace ADE.Configurations.Entities.Summaries
{
    public class RBajasCabecera
    {
        public int ID_RAC { get; set; }
        public string NUM_CPE { get; set; }
        public int TOT_DOC { get; set; }
        public DateTime FEC_REF { get; set; }
        public DateTime FEC_ANU { get; set; }
        public DateTime FEC_ENV { get; set; }
        public string DOC_EST { get; set; }
        public string DOC_MSG { get; set; }
        public string DOC_TCK { get; set; }
        public int NUM_SEC { get; set; }
        public string FEC_CAD { get; set; }
        public string TIPO { get; set; }
        public byte[] VAR_FIR { get; set; }
        public List<string> DOC_UPD_LIST { get; set; }

        public RBajasCabecera()
        {
            ID_RAC = 0;
            NUM_CPE = "";
            TOT_DOC = 0;
            FEC_REF = DateTime.Now;
            FEC_ANU = DateTime.Now;
            FEC_ENV = DateTime.Now;
            DOC_EST = "";
            DOC_MSG = "";
            DOC_TCK = "";
            NUM_SEC = 0;
            FEC_CAD = "";
            TIPO = "";
            DOC_UPD_LIST = new List<string>();
        }
    }
}
