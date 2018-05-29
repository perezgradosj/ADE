using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using ADE.Processes.DatabaseIncome;

namespace ADE.WebServices.Summary
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "ServicioResumen" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select ServicioResumen.svc or ServicioResumen.svc.cs at the Solution Explorer and start debugging.
    public class ServicioResumen : IServicioResumen
    {
        public string ProcesaResumen(string fecha, string ruc, string tipo)
        {
            return new SummaryService().ProcesaResumen(fecha, ruc,tipo);
        }
    }
}
