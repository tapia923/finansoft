using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanillajeColectivos.DTO.Products
{
    [Table("dbo.operation")]
    public class operation
    {
        public int id { get; set; }

        [ForeignKey("products")]
        public int productId { get; set; }

        public int quantity { get; set; }

        [ForeignKey("operationType")]
        public int operationTypeId { get; set; }

        public string facturaId { get; set; }
        public DateTime date { get; set; }
        public decimal price { get; set; }
        public decimal discount { get; set; }
        public int? IvaId { get; set; }
        public string userId { get; set; }

        public virtual products products { get; set; }
        public virtual operationType operationType { get; set; }

        
    }
}