using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ADE.Testing.Summary
{
    public partial class Capsula : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void envia_Click(object sender, EventArgs e)
        {
            Slin.Facturacion.Proxies.ServicioResumen.ServicioResumenClient Client = new Slin.Facturacion.Proxies.ServicioResumen.ServicioResumenClient();

            string xd = "";
            xd = Client.ProcesaResumen(fecha.Value, ruc.Value, td.Value);

            Client.Close();

        }
    }
}