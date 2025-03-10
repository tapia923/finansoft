using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanillajeColectivos.DTO.Contabilidad
{
    [Table("dbo.economicActivity")]
    public class economicActivity
    {
        [Key]
        public int activityCode { get; set; }
        public string description { get; set; }
        public decimal tariff { get; set; }
    }
}