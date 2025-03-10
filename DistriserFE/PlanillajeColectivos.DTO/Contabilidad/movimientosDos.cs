using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanillajeColectivos.DTO.Contabilidad
{
    [Table("dbo.movimientosDos")]
    public class movimientosDos
    {
        public int id { get; set; }
        public int facturaId { get; set; }
        public string codigoCuenta { get; set; }
        public decimal debito { get; set; }
        public decimal credito { get; set; }
        public int terceroId { get; set; }
        public string tipo { get; set; }
        public DateTime fecha { get; set; }
    }
}
