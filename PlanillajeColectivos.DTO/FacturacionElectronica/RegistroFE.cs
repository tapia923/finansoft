using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanillajeColectivos.DTO.FacturacionElectronica
{
    [Table("dbo.historialFacturasElectronicas")]
    public class RegistroFE
    {
        public int id { get; set; }
        public string registro { get; set; }
        public string dianResponse { get; set; }
    }
}
