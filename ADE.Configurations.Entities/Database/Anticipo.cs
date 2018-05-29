namespace ADE.Configurations.Entities.Database
{
    public class Anticipo
    {
        public string ANT_NROORDEN { get; set; }
        public string ANT_MONTO { get; set; }
        public string ANT_TPO_DOC_ANT { get; set; }
        public string ANT_ID_DOC_ANT { get; set; }
        public string ANT_NUM_DOC_EMI { get; set; }
        public string ANT_TPO_DOC_EMI { get; set; }

        public Anticipo()
        {
            ANT_NROORDEN = "";
            ANT_MONTO = "";
            ANT_TPO_DOC_ANT = "";
            ANT_ID_DOC_ANT = "";
            ANT_NUM_DOC_EMI = "";
            ANT_TPO_DOC_EMI = "";
        }
    }
}
