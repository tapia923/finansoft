using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanillajeColectivos.DTO.Products
{
    [Table("dbo.presentation")]
    public class presentation
    {
        [Key]
        public int id { get; set; }
        public int nivel { get; set; }
        public string descripction { get; set; }
    }
}