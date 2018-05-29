namespace ADE.Configurations.Entities.Database
{
    public class Referencia
    {
        public string REF_NROORDEN { get; set; }
        public string REF_ID { get; set; }
        public string REF_TPO_DOC { get; set; }

        public Referencia()
        {
            REF_NROORDEN = "";
            REF_ID = "";
            REF_TPO_DOC = "";
        }
    }
}
