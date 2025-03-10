using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanillajeColectivos.DTO.Products
{
    [Table("dbo.consecutivosFacturas")]
    public class consecutivosFacturas
    {
        public int id { get; set; }
        public string cod { get; set; }
        public string descripcion { get; set; }
        public int desde { get; set; }
        public int hasta { get; set; }
        public int actual { get; set; }
    }
}