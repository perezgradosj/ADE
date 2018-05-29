using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace ADE.WebServices.Resumen
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IServicioResumen" in both code and config file together.
    [ServiceContract]
    public interface IServicioResumen
    {
        //[OperationContract]
        //string ProcesaResumen(string fecha, string ruc, string tipo);
    }
}
