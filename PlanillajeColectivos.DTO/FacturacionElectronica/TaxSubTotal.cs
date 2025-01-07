using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanillajeColectivos.DTO.FacturacionElectronica
{
    public class TaxSubTotal
    {
        public string TaxableAmount { get; set; }
        public string TaxAmount { get; set; }
        public string Percent { get; set; }
    }
}
