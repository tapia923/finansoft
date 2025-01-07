using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanillajeColectivos.DTO.Contabilidad
{
    [Table("dbo.location")]
    public class location
    {
        public int id { get; set; }
        public string name { get; set; }
        public int dep { get; set; }
        public int pais { get; set; }
    }
}