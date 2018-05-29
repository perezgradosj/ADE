using System;

namespace ADE.Configurations.Entities.Summaries
{
    public class RBoletasDetalle
    {
        public int ID_RBD { get; set; }
        public string NUM_CPE { get; set; }
        public string TPO_CPE { get; set; }
        public string DOC_SER { get; set; }
        public int NUM_INI { get; set; }
        public int NUM_FIN { get; set; }
        public decimal MTO_GRA { get; set; }
        //added js
        public decimal MTO_GRAT { get; set; }

        //added js
        public string SYSTEM_STATUS { get; set; }
        //added js
        public DateTime FEC_EMIS { get; set; }

        public decimal MTO_EXO { get; set; }
        public decimal MTO_INA { get; set; }
        public decimal MTO_OCA { get; set; }
        public decimal IMP_IGV { get; set; }
        public decimal IMP_ISC { get; set; }
        public decimal IMP_OTH { get; set; }
        public decimal MTO_TOT { get; set; }
        public int ID_RBC { get; set; }
        public int NRO_LIN { get; set; }

        public int MON_GRAT { get; set; }
        public string MONEDA { get; set; }
        public int SUMMARY { get; set; }
        public int VOIDED { get; set; }
        public int STATUS_RC_DOC { get; set; }




        //added 
        public int LineID { get; set; }
        public string DocumentTypeCode { get; set; }
        public string ID { get; set; }
        public string CustomerAssignedAccountID { get; set; }
        public string AdditionalAccountID { get; set; }
        public int ConditionCode { get; set; }

        public string TPO_DOC_AFEC { get; set; }
        public string NRO_DOC_AFEC { get; set; }



        public decimal TotalAmount { get; set; }
        public string Mon_TotalAmount { get; set; }

        public decimal PaidAmount { get; set; }
        public string InstructionID { get; set; }

        public bool ChargeIndicator { get; set; }
        public decimal Amount { get; set; }


        public decimal TaxAmount { get; set; }
        public decimal TaxSubTotal_TaxAmount { get; set; }
        public string TaxCategory_TaxScheme { get; set; }
        public string TaxCategory_Name { get; set; }
        public string TaxCategory_TaxTypeCode { get; set; }

        public RBoletasDetalle()
        {
            MONEDA = string.Empty;
            ID_RBD = 0;
            NUM_CPE = "";
            TPO_CPE = "";
            DOC_SER = "";
            NUM_INI = 0;
            NUM_FIN = 0;
            MTO_GRA = 0.0m;
            MTO_EXO = 0.0m;
            MTO_INA = 0.0m;
            MTO_OCA = 0.0m;
            IMP_IGV = 0.0m;
            IMP_ISC = 0.0m;
            IMP_OTH = 0.0m;
            MTO_TOT = 0.0m;
            ID_RBC = 0;
            NRO_LIN = 0;


            LineID = 0;
            DocumentTypeCode = "";
            ID = "";
            CustomerAssignedAccountID = "";
            AdditionalAccountID = "";
            ConditionCode = 0;
            TotalAmount = 0.0m;
            Mon_TotalAmount = "";
            PaidAmount = 0.0m;
            InstructionID = "";
            ChargeIndicator = false;
            Amount = 0.0m;
            TaxAmount = 0.0m;
            TaxSubTotal_TaxAmount = 0.0m;
            TaxCategory_TaxScheme = "";
            TaxCategory_Name = "";
            TaxCategory_TaxTypeCode = "";
        }
    }
}
