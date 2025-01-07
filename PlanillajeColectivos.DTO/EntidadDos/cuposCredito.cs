using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanillajeColectivos.DTO.EntidadDos
{
    [Table(name: "dbo.estadoTercerosEntidadTres")]
    public class cuposCredito
    {
        [Key]
        public string NIT { get; set; }

        [Required]
        public decimal cupo { get; set; }

        public string numeroTarjeta { get; set; }
        public decimal acumulado { get; set; }
    }
}
