using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanillajeColectivos.DTO.Tipos
{
    [Table("dbo.tipoIdentificacion")]
    public class TipoIdentificacion
    {
        public int id { get; set; }
        public string descripcion { get; set; }
        public string codigo { get; set; }
    }
}
