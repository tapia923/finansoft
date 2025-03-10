using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanillajeColectivos.DTO.Products
{
    [Table("dbo.iva")]
    public class iva
    {
        [Key]
        public int id { get; set; }
        public string name { get; set; }
        public decimal value { get; set; }
    }
}