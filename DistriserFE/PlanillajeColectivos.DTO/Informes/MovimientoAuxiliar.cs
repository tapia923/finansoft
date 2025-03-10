using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanillajeColectivos.DTO.Informes
{
    public partial class MovimientoAuxiliar
    {
        [Key]
        public string CUENTA { get; set; }
        public string TERCERO { get; set; }
        public decimal DEBITO { get; set; }
        public decimal CREDITO { get; set; }
        public string CCOSTO { get; set; }
        public DateTime FECHAMOVIMIENTO { get; set; }
        public string NOMBRE { get; set; }
    }
}
