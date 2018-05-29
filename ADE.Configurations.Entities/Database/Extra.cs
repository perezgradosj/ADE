namespace ADE.Configurations.Entities.Database
{
    public class Extra
    {
        public int ID { get; set; }
        public int IDDC { get; set; }
        public string EXLINEA { get; set; }
        public string EXDATO { get; set; }
        public string EXTIPO { get; set; }

        public Extra()
        {
            ID = 0;
            IDDC = 0;
            EXLINEA = "";
            EXDATO = "";
            EXTIPO = "";
        }
    }
}
