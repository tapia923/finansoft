using PlanillajeColectivos.DTO.Contabilidad;
using PlanillajeColectivos.DTO.Products;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace PlanillajeColectivos.DTO.Contabilidad
{
    [Table("dbo.movimientos")]
    public class movimientos
    {
        [Key]
        public int id { get; set; }
        public string tipoComprobante { get; set; }
        public string numero { get; set; }

        [ForeignKey("planCuentas")]
        public string cuenta { get; set; }

        [ForeignKey("persons")]
        public int terceroId { get; set; }
        public string detalle { get; set; }
        public decimal debito { get; set; }
        public decimal credito { get; set; }
        public decimal baseMov { get; set; }
        public int centroCostoId { get; set; }
        public DateTime fechaCreado { get; set; }
        public string documento { get; set; }


        public virtual persons persons { get; set; }
        public virtual planCuentas planCuentas { get; set; }
    }
}