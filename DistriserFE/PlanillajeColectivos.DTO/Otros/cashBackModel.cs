using PlanillajeColectivos.DTO.Products;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanillajeColectivos.DTO.Otros
{
    [Table("dbo.cashBack")]
    public class cashBackModel
    {
        public int id { get; set; }

        [ForeignKey("persons")]
        public int terceroId { get; set; }
        public DateTime fechaInicio { get; set; }
        public DateTime fechaEntrega { get; set; }
        public decimal valorEntregado { get; set; }
        public DateTime fechaEntregado { get; set; }
        public int periodoEnMeses { get; set; }
        public decimal valorActual { get; set; }
        public decimal porcetaje { get; set; }
        public int destino { get; set; }//1 a devolucion de efectivo -- 2 a subsidio

        public virtual persons persons { get; set; }
    }
}
