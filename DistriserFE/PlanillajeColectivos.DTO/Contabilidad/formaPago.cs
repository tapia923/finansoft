using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanillajeColectivos.DTO.Contabilidad
{
    [Table("dbo.FormaPago")]
    public class FormaPago
    {
        [Key]
        public int id { get; set; }
        public string nombre { get; set; }
        public string codigo { get; set; }

        [ForeignKey("CuentaVentaFK")]
        public string CuentaContableVenta { get; set; }
        [ForeignKey("CuentaCompraFK")]
        public string CuentaContableCompra { get; set; }

        public virtual planCuentas CuentaVentaFK { get; set; }
        public virtual planCuentas CuentaCompraFK { get; set; }
    }
}