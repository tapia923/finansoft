using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanillajeColectivos.DTO.Products
{
    [Table("dbo.operationType")]
    public class operationType
    {
        public int id { get; set; }
        public string name { get; set; }
        public int tipo { get; set; }
    }
}