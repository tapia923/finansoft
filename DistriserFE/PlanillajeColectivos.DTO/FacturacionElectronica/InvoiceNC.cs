using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanillajeColectivos.DTO.FacturacionElectronica
{
    [Table("fe.InvoiceNC")]
    public class InvoiceNC
    {
        [Key]
        public int Id { get; set; }
        public string Prefijo { get; set; }
        public int Numero { get; set; }
        public string Cufe { get; set; }
        public string AlgoritmoCufe { get; set; }
        public string TipoDocumento { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime Fecha { get; set; }

        [ForeignKey("InvoiceFEFK")]
        public int IdFE { get; set; }
        public bool EstadoEnvioEmail { get; set; }
        public string NombreDocumento { get; set; }

        [ForeignKey("MotivoNCFK")]
        public int IdMotivo { get; set; }
        public string RazonMotivo { get; set; }

        public virtual MotivoNotaCredito MotivoNCFK { get; set; }
        public virtual InvoiceFacturaElectronica InvoiceFEFK { get; set; }
    }
}
