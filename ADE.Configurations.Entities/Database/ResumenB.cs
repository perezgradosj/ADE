using System;
namespace ADE.Configurations.Entities.Database
{
    public class ResumenB
    {
        public string TPODOC { get; set; }
        public string SERIE { get; set; }
        public int NUMERO { get; set; }
        public int INICIO { get; set; }
        public int FINAL { get; set; }
        public DateTime DESDE { get; set; }
        public DateTime HASTA { get; set; }

        // IMPORTES
        public decimal GRAVADO { get; set; }
        public decimal EXONERADO { get; set; }
        public decimal INAFECTO { get; set; }
        public decimal OCARGOS { get; set; }
        public decimal GRATUITO { get; set; }

        // IMPUESTOS
        public decimal IGV { get; set; }
        public decimal ISC { get; set; }
        public decimal OTH { get; set; }

        // TOTAL
        public decimal TOTAL { get; set; }

        // TIPO DE CAMBIO
        public string MONEDA { get; set; }
        public decimal CAMBIO { get; set; }

        //TIPO DOCUMENTO IDENTIDAD RECEPTOR
        public string RE_TPODOC { get; set; }
        public string RE_NUMDOC { get; set; }

        //NEW STATUS RC DOC
        public int STATUS_RC_DOC { get; set; }
        //added 
        public string TPO_DOC_AFEC { get; set; }
        //added
        public string NRO_DOC_AFEC { get; set; }
        //added
        public string SYSTEM_STATUS { get; set; }
        //added
        public string NUM_CPE { get; set; }

        //
        public int SUMMARY { get; set; }
        public int VOIDED { get; set; }

        //added 
        //public int LineID { get; set; }
        //public string DocumentTypeCode { get; set; }
        //public string ID { get; set; }
        //public string CustomerAssignedAccountID { get; set; }
        //public string AdditionalAccountID { get; set; }
        //public string ConditionCode { get; set; }
        //public decimal TotalAmount { get; set; }
        //public string Mon_TotalAmount { get; set; }

        //public decimal PaidAmount { get; set; }
        //public string InstructionID { get; set; }

        //public bool ChargeIndicator { get; set; }
        //public decimal Amount { get; set; }


        //public decimal TaxAmount { get; set; }
        //public decimal TaxSubTotal_TaxAmount { get; set; }
        //public string TaxCategory_TaxScheme { get; set; }
        //public string TaxCategory_Name { get; set; }
        //public string TaxCategory_TaxTypeCode { get; set; }



    }
}
