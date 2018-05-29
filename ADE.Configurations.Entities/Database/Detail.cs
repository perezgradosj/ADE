using System;

namespace ADE.Configurations.Entities.Database
{
    public class Detail
    {
        /* Primary Keys */
        public int IdDetalle { get; set; }

        /* Fields */
        public string NUM_CPE { get; set; }
        public string IT_NRO_ORD { get; set; }
        public string IT_UND_MED { get; set; }
        public double IT_CANT_ITEM { get; set; }
        public string IT_COD_PROD { get; set; }
        public string IT_DESCRIP { get; set; }
        public decimal IT_VAL_UNIT { get; set; }
        public decimal IT_MNT_PVTA { get; set; }
        public decimal IT_VAL_VTA { get; set; }
        public decimal IT_MTO_IGV { get; set; }
        public decimal IT_COD_AFE_IGV { get; set; }
        public decimal IT_MTO_ISC { get; set; }
        public decimal IT_SIS_AFE_ISC { get; set; }
        public decimal IT_DESC_MNTO { get; set; }
        public string SERIE { get; set; }
        public string NUM_DOCUMENTO { get; set; }

        public decimal IT_IGV_PERCENTAGE { get; set; }


        public int IdDocumentoCabecera { get; set; }

        //CAMPOS PARA RETENTION AND PERCEPTION
        public string TPODOCRELAC { get; set; }
        public string NUMDOCRELAC { get; set; }
        public DateTime FEMISDOCRELAC { get; set; }
        public decimal ITOTDOCRELAC { get; set; }
        public string MDOCRELAC { get; set; }

        public DateTime FECMOVI { get; set; }
        public string NUMMOVI { get; set; }
        public decimal IMPSOPERMOV { get; set; }
        public string MONMOVI { get; set; }

        public decimal IMPOPER { get; set; }
        public string MONIMPOPER { get; set; }
        public DateTime FECOPER { get; set; }
        public decimal IMPTOTOPER { get; set; }
        public string MONOPER { get; set; }

        public string MONREFETC { get; set; }
        public string MONDESTTC { get; set; }
        public string FACTORTC { get; set; }
        public DateTime FECHATC { get; set; }

        public Detail()
        {
            IdDetalle = 0;

            /* Fields */
            NUM_CPE = string.Empty;
            IT_NRO_ORD = string.Empty;
            IT_UND_MED = string.Empty;
            IT_CANT_ITEM = 0.00;
            IT_COD_PROD = string.Empty;
            IT_DESCRIP = string.Empty;
            IT_VAL_UNIT = 0.00m;
            IT_MNT_PVTA = 0.00m;
            IT_VAL_VTA = 0.00m;
            IT_MTO_IGV = 0.00m;
            IT_COD_AFE_IGV = 0.00m;
            IT_MTO_ISC = 0.00m;
            IT_SIS_AFE_ISC = 0.00m;
            IT_DESC_MNTO = 0.00m;
            SERIE = string.Empty;
            NUM_DOCUMENTO = string.Empty;

            IdDocumentoCabecera = 0;



            //CAMPOS PARA RETENTION AND PERCEPTION
            TPODOCRELAC = string.Empty;
            NUMDOCRELAC = string.Empty;
            FEMISDOCRELAC = DateTime.Now;
            ITOTDOCRELAC = 0.00m;
            MDOCRELAC = string.Empty;

            FECMOVI = DateTime.Now;
            NUMMOVI = string.Empty;
            IMPSOPERMOV = 0.00m;
            MONMOVI = string.Empty;

            IMPOPER = 0.00m;
            MONIMPOPER = string.Empty;
            FECOPER = DateTime.Now;
            IMPTOTOPER = 0.00m;
            MONOPER = string.Empty;

            MONREFETC = string.Empty;
            MONDESTTC = string.Empty;
            FACTORTC = string.Empty;
            FECHATC = DateTime.Now;
        }
    }
}
