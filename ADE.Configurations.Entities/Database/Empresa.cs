using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADE.Configurations.Entities.Database
{
    public class Empresa
    {
        public int IdEmpresa { get; set; }
        public string CodEmpresa { get; set; }
        public string Ubigeo { get; set; } 
        public string Ruc { get; set; }
        public string RazonSocial { get; set; }
        public string RazonComercial { get; set; }
        public string Telefono { get; set; }
        public string Fax { get; set; }
        public string Direccion { get; set; }
        public string DomicilioFiscal { get; set; }
        public string Urbanizacion { get; set; }
        public string PaginaWeb { get; set; }
        public string Email { get; set; }

        public Empresa()
        {
            IdEmpresa = 0;
            CodEmpresa = "";
            Ubigeo = "";
            Ruc = "";
            RazonSocial = "";
            RazonComercial = "";
            Telefono = "";
            Fax = "";
            Direccion = "";
            DomicilioFiscal = "";
            Urbanizacion = "";
            PaginaWeb = "";
            Email = "";
        }
    }
}
