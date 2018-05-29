namespace ADE.Configurations.Entities.Summaries
{
    public class RBajasDetalle
    {
        public int ID_RAD { get; set; }
        public string NUM_CPE { get; set; }
        public string TPO_CPE { get; set; }
        public string DOC_SER { get; set; }
        public int DOC_NUM { get; set; }
        public string DOC_DES { get; set; }
        public string DOC_FEC { get; set; }
        public int ID_RAC { get; set; }

        public RBajasDetalle()
        {
            ID_RAD = 0;
            NUM_CPE = "";
            TPO_CPE = "";
            DOC_SER = "";
            DOC_NUM = 0;
            DOC_DES = "";
            DOC_FEC = "";
            ID_RAC = 0;
        }
    }
}
