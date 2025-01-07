using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanillajeColectivos.DTO.FacturacionElectronica
{
    public class TotalXML
    {
        public string PaymentMeansID { get; set; }
        public string PaymentMeansCode { get; set; }
        public string TaxAmount { get; set; }

        public List<TaxSubTotal> TaxSubTotal { get; set; }
        //public string TaxableAmount { get; set; }
        //public string Percent { get; set; }
        public string LineExtensionAmount { get; set; }
        public string AllowanceTotalAmount { get; set; }
        public string TaxExclusiveAmount { get; set; }
        public string TaxInclusiveAmount { get; set; }
        public string PayableAmount { get; set; }
    }
}
