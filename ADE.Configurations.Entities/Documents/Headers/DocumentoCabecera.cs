using System;

namespace ADE.ConfigurationEntities.Documents.Headers
{
    class DocumentoCabecera
    {
        public int Id_DC { get; set; }
        public string NUM_CPE { get; set; }
        public string ID_TPO_CPE { get; set; }
        public string ID_CPE { get; set; }
        public string ID_TPO_OPERACION { get; set; }
        public DateTime FEC_EMIS { get; set; }
        public string TPO_MONEDA { get; set; }
        public string TPO_NOTA { get; set; }
        public string MOTIVO_NOTA { get; set; }
        public string EM_TPO_DOC { get; set; }
        public string EM_NUM_DOCU { get; set; }
        public string EM_NOMBRE { get; set; }
        public string EM_NCOMER { get; set; }
        public string EM_UBIGEO { get; set; }
        public string EM_DFISCAL { get; set; }
        public string EM_DURBANIZ { get; set; }
        public string EM_DIR_PROV { get; set; }
        public string EM_DIR_DPTO { get; set; }
        public string EM_DIR_DIST { get; set; }
        public string EM_COD_PAIS { get; set; }
        public string RE_TPODOC { get; set; }
        public string RE_NUMDOC { get; set; }
        public string RE_NOMBRE { get; set; }
        public string RE_DIRECCION { get; set; }
        public decimal TOT_GRAV_MTO { get; set; }
        public decimal TOT_INAF_MTO { get; set; }
        public decimal TOT_EXON_MTO { get; set; }
        public decimal TOT_GRAT_MTO { get; set; }
        public decimal TOT_DSCTO_MTO { get; set; }
        public decimal TOT_SUMA_IGV { get; set; }
        public decimal TOT_SUMA_ISC { get; set; }
        public decimal TOT_SUMA_OTRIB { get; set; }
        public decimal TOT_DCTO_GLOB { get; set; }
        public decimal TOT_SUM_OCARG { get; set; }
        public decimal ANT_TOT_ANTICIPO { get; set; }
        public decimal TOT_IMPOR_TOTAL { get; set; }
        public string MONTOLITERAL { get; set; }
        public decimal PER_BASE_IMPO { get; set; }
        public decimal PER_MNTO_PER { get; set; }
        public decimal PER_MNTO_TOT { get; set; }
        public string SERIE { get; set; }
        public string NUM_DOCUMENTO { get; set; }
        public int Id_ED { get; set; }
    }
}
