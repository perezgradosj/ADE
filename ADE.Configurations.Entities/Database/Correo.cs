namespace ADE.Configurations.Entities.Database
{
    public class Correo
    {
        public int IDCE { get; set; }
        public int IDDC { get; set; }
        public string NUMCPE { get; set; }
        public string PARA { get; set; }
        public string CC { get; set; }
        public string CCO { get; set; }

        public Correo()
        {
            IDCE = 0;
            IDDC = 0;
            NUMCPE = "";
            PARA = "";
            CC = "";
            CCO = "";
        }
    }
}
