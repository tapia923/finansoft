using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanillajeColectivos.DTO.FacturacionElectronica
{
    public class Customer
    {
        public string AdditionalAccountID { get; set; }
        public string CustomerName { get; set; }
        public string CustomerCityCode { get; set; }
        public string CustomerCity { get; set; }
        public string CustomerDepto { get; set; }
        public string CustomerDeptoCode { get; set; }
        public string CustomerAddress { get; set; }
        public string CustomerIdCode { get; set; }
        public string CustomerNit { get; set; }
    }
}
