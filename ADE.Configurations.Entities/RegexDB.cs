namespace ADE.Configurations.Entities
{
    public class RegexDB
    {
        /* Fields */
        public string KEY { get; set; }
        public string NOM { get; set; }
        public string VAL { get; set; }
        public string MND { get; set; }
        public string DOC { get; set; }
        public string TAB { get; set; }
        public string MSG { get; set; }
        public string ECV { get; set; }
        public string ECN { get; set; }

        /* Campos necesarios para valida obligatoriedad */

        public int _POS { get; set; }
        public int _USE { get; set; }
    }
}
