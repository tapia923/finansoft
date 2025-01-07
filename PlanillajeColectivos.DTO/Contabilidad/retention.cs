using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanillajeColectivos.DTO.Contabilidad
{
    [Table("dbo.retention")]
    public class retention
    {
        public int id { get; set; }
        public string description { get; set; }
        public bool isRetained { get; set; }
    }
}