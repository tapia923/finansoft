using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanillajeColectivos.DTO.Products
{
    [Table("dbo.categorias")]
    public class categorias
    {
        [Key]
        public int id { get; set; }
        public string descripcion { get; set; }
        public string cuentaVentas { get; set; }
        public string cuentaIva { get; set; }
        public string cuentaInventario { get; set; }
        public string cuentaCosto { get; set; }
    }
}