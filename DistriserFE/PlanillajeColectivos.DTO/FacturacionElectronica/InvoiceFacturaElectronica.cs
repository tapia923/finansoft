using PlanillajeColectivos.DTO.Products;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanillajeColectivos.DTO.FacturacionElectronica
{
    [Table("dbo.invoiceFacturasElectronicas")]
    public class InvoiceFacturaElectronica
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Prefijo { get; set; }
        [Required]
        public int Numero { get; set; }
        [Required]
        public string Cufe { get; set; }
        [Required]
        public string AlgoritmoCufe { get; set; }
        [Required]
        public string TipoDocumento { get; set; }
        [Required]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime Fecha { get; set; }

        [Required]
        [ForeignKey("ClienteFK")]
        public int IdCliente { get; set; }

        [Required]
        public bool EstadoEnvioEmail { get; set; }

        [Required]
        public string NombreDocumento { get; set; }

        [Required]
        [ForeignKey("FacturaFK")]
        public string IdFactura { get; set; }

        public virtual persons ClienteFK { get; set; }
        public virtual factura FacturaFK { get; set; }
    }
}
