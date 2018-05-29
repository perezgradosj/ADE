using ADE.Configurations.Entities.Database;
using ADE.Configurations.Entities.Summaries;
using System.Collections.Generic;

namespace ADE.Configurations.Entities
{
    public class DBDocument
    {
        public Header Cabecera { get; set; }
        public Correo Correos { get; set; }
        public Detraccion Detracciones { get; set; }

        public List<Detail> Detalles { get; set; }
        public List<DocumentoAfectado> DocumentosAfectados { get; set; }
        public List<Extra> Extras { get; set; }
        public List<Referencia> Referencias { get; set; }
        public List<Anticipo> Anticipos { get; set; }

        //Resumenes
        public RBajasCabecera RAC { get; set; }
        public RBoletasCabecera RBC { get; set; }

        public List<RBajasDetalle> RAD { get; set; }
        public List<RBoletasDetalle> RBD { get; set; }
        
        public DBDocument()
        {
            Cabecera = new Header();
            Correos = new Correo();
            Detracciones = new Detraccion();

            Detalles = new List<Detail>();
            DocumentosAfectados = new List<DocumentoAfectado>();
            Extras = new List<Extra>();
            Referencias = new List<Referencia>();
            Anticipos = new List<Anticipo>();

            RAC = new RBajasCabecera();
            RBC = new RBoletasCabecera();

            RAD = new List<RBajasDetalle>();
            RBD = new List<RBoletasDetalle>();
        }
    }
}
