namespace ADE.Configurations.Entities.Database
{
    public class DocumentoAfectado
    {
        public string DocNroOrden { get; set; }
        public string DocID { get; set; }
        public string DocTpoDoc { get; set; }

        public DocumentoAfectado()
        {
            DocNroOrden = "";
            DocID = "";
            DocTpoDoc = "";
        }
    }
}
