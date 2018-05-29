namespace ADE.Configurations.Entities.Database
{
    public class Detraccion
    {
        public int IDDC { get; set; }
        public string NUMCPE { get; set; }
        public string VALBBSS { get; set; }
        public string CTABN { get; set; }
        public string PORCENT { get; set; }
        public string MONTO { get; set; }

        public Detraccion()
        {
            IDDC = 0;
            NUMCPE = "";
            VALBBSS = "";
            CTABN = "";
            PORCENT = "";
            MONTO = "";
        }
    }
}
