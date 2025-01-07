using PlanillajeColectivos.DTO.Products;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanillajeColectivos.DTO.Contabilidad
{
    [Table("dbo.comprobantes")]
    public class comprobante
    {
        public int id { get; set; }
        public string tipoComprobante { get; set; }
        public string numero { get; set; }

        [ForeignKey("centroCosto")]
        public int centroCostoId { get; set; }

        public string detalle { get; set; }

        [ForeignKey("persons")]
        public int terceroId { get; set; }

        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal valorTotal { get; set; }

        public int anio { get; set; }
        public int mes { get; set; }
        public int dia { get; set; }
        public DateTime fechaCreacion { get; set; }

        [ForeignKey("user")]
        public string usuarioId { get; set; }

        [ForeignKey("formaPago")]
        public int formaPagoId { get; set; }
        public string documento { get; set; }
        public bool estado { get; set; }
        
        public virtual persons persons { get; set; }
        public virtual centroCosto centroCosto { get; set; }
        public virtual usersTabla user { get; set; }
        public virtual FormaPago formaPago { get; set; }
    }
}