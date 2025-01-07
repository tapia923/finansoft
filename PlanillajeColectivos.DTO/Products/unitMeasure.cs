using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanillajeColectivos.DTO.Products
{
    [Table("dbo.unitMeasure")]
    public class unitMeasure
    {
        [Key]
        public int id { get; set; }
        public string name { get; set; }
        public string siglas { get; set; }
    }
}