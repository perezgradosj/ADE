namespace ADE.ConfigurationEntities.Documents.Headers
{
    public class Impresora
    {
        public string RutaImpr { get; set; }
        public string TipoImpr { get; set; }

        public Impresora()
        {
            RutaImpr = "";
            TipoImpr = "";
        }
    }
}
