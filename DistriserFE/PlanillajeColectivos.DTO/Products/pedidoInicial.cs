using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanillajeColectivos.DTO.Products
{
    [Table("dbo.pedidoInicial")]
    public class pedidoInicial
    {
        public int id { get; set; }
        public int productId { get; set; }
        public string userId { get; set; }
        public int quantity { get; set; }
        public DateTime date { get; set; }
        public string observation { get; set; }
        public int personId { get; set; }
        public string facturaId { get; set; }
        public int tipo { get; set; }
    }
}