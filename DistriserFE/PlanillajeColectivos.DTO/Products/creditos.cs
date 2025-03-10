using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanillajeColectivos.DTO.Products
{
    [Table("dbo.creditos")]
    public class creditos
    {
        public string id { get; set; }
        public int facturaId { get; set; }
        public decimal valorPagado { get; set; }
        public decimal saldo { get; set; }
        public DateTime fechaPago { get; set; }
        public string estado { get; set; }
        public decimal efectivo { get; set; }
        public decimal cambio { get; set; }
    }
}