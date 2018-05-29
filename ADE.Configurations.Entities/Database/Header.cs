using System;

namespace ADE.Configurations.Entities.Database
{
    public class Header
    {
        /* Primary Keys */
        public int IdDocumentoCabecera { get; set; }

        /* Fields */
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
        public string XML_ORIG { get; set; }
        public string XML_RES { get; set; }
        public string SERIE { get; set; }
        public string NUM_DOCUMENTO { get; set; }
        public string XML_SIGN { get; set; }
        public int IdEstadoDocumento { get; set; }

        public byte[] VAR_FIR { get; set; }

        public string RE_NCOMER { get; set; }
        public string RE_DURBANIZ { get; set; }
        public string RE_DIR_PROV { get; set; }
        public string RE_DIR_DPTO { get; set; }
        public string RE_DIR_DIST { get; set; }
        public string RE_COD_PAIS { get; set; }
        public string RE_UBIGEO { get; set; }

        //** RETENTION AND PERCEPTION **//
        public string REGIMENCE { get; set; }
        public decimal TASACE { get; set; }
        public string OBSCE { get; set; }
        public decimal IMPTOTCE { get; set; }
        public string MONIMPTOTCE { get; set; }
        public decimal IMPTOT { get; set; }
        public string MONIMPTOT { get; set; }


        //** DATOS ADICIONALES **//

        public string SEDE { get; set; }
        public string USUARIO { get; set; }
        public string IMPRESORA { get; set; }
        public string CAMPO1 { get; set; }
        public string CAMPO2 { get; set; }
        public string CAMPO3 { get; set; }
        public string CAMPO4 { get; set; }
        public string CAMPO5 { get; set; }
        public string CAMPO6 { get; set; }
        public string CAMPO7 { get; set; }
        public string CAMPO8 { get; set; }
        public string CAMPO9 { get; set; }
        public string CAMPO10 { get; set; }

        public string REF_FILES { get; set; }

        public Header()
        {
            IdDocumentoCabecera = 0;
            NUM_CPE = string.Empty;
            ID_TPO_CPE = string.Empty;
            ID_CPE = string.Empty;
            ID_TPO_OPERACION = string.Empty;
            FEC_EMIS = DateTime.MinValue;
            TPO_MONEDA = string.Empty;
            TPO_NOTA = string.Empty;
            MOTIVO_NOTA = string.Empty;
            EM_TPO_DOC = string.Empty;
            EM_NUM_DOCU = string.Empty;
            EM_NOMBRE = string.Empty;
            EM_NCOMER = string.Empty;
            EM_UBIGEO = string.Empty;
            EM_DFISCAL = string.Empty;
            EM_DURBANIZ = string.Empty;
            EM_DIR_PROV = string.Empty;
            EM_DIR_DPTO = string.Empty;
            EM_DIR_DIST = string.Empty;
            EM_COD_PAIS = string.Empty;
            RE_NCOMER = string.Empty;
            RE_TPODOC = string.Empty;
            RE_NUMDOC = string.Empty;
            RE_NOMBRE = string.Empty;
            RE_DIRECCION = string.Empty;
            TOT_GRAV_MTO = 0.00m;
            TOT_INAF_MTO = 0.00m;
            TOT_EXON_MTO = 0.00m;
            TOT_GRAT_MTO = 0.00m;
            TOT_DSCTO_MTO = 0.00m;
            TOT_SUMA_IGV = 0.00m;
            TOT_SUMA_ISC = 0.00m;
            TOT_SUMA_OTRIB = 0.00m;
            TOT_DCTO_GLOB = 0.00m;
            TOT_SUM_OCARG = 0.00m;
            ANT_TOT_ANTICIPO = 0.00m;
            TOT_IMPOR_TOTAL = 0.00m;
            MONTOLITERAL = string.Empty;
            PER_BASE_IMPO = 0.00m;
            PER_MNTO_PER = 0.00m;
            PER_MNTO_TOT = 0.00m;
            SERIE = string.Empty;
            NUM_DOCUMENTO = string.Empty;
            IdEstadoDocumento = 1;



            RE_DURBANIZ = string.Empty;
            RE_DIR_PROV = string.Empty;
            RE_DIR_DPTO = string.Empty;
            RE_DIR_DIST = string.Empty;
            RE_COD_PAIS = string.Empty;
            RE_UBIGEO = string.Empty;

            REGIMENCE = string.Empty;
            TASACE = 0.00m;
            OBSCE = string.Empty;
            IMPTOTCE = 0.00m;
            MONIMPTOTCE = string.Empty;
            IMPTOT = 0.00m;
            MONIMPTOT = string.Empty;

            SEDE = string.Empty;
            USUARIO = string.Empty;
            IMPRESORA = string.Empty;
            CAMPO1 = string.Empty;
            CAMPO2 = string.Empty;
            CAMPO3 = string.Empty;
            CAMPO4 = string.Empty;
            CAMPO5 = string.Empty;
            CAMPO6 = string.Empty;
            CAMPO7 = string.Empty;
            CAMPO8 = string.Empty;
            CAMPO9 = string.Empty;
            CAMPO10 = string.Empty;

            REF_FILES = string.Empty;
        }
    }
}
