using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADE.Configurations.Entities.Database
{
    public class ServicioResumen
    {
        public string Id { get; set; }
        public string CodeService { get; set; }
        public string NameService { get; set; }
        public string ValueTime { get; set; }
        public string IntervalValue { get; set; }
        public string MaxNumAttempts { get; set; }
        public string RucEntity { get; set; }
        public string IdEstado { get; set; }
        public string SubType { get; set; }

        public ServicioResumen()
        {
            Id = "";
            CodeService = "";
            NameService = "";
            ValueTime = "";
            IntervalValue = "";
            MaxNumAttempts = "";
            RucEntity = "";
            IdEstado = "";
            SubType = "";
        }
    }
}
