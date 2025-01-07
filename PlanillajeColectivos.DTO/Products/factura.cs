using PlanillajeColectivos.DTO.Contabilidad;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanillajeColectivos.DTO.Products
{
    [Table("dbo.factura")]
    public class factura
    {
        [Key]
        public string id { get; set; }

        [ForeignKey("persons")]
        public int personId { get; set; }

        [ForeignKey("usersTabla")]
        public string userId { get; set; }

        public int operationTypeId { get; set; }

        [ForeignKey("FormaPagoFK")]
        public int IdFormaPago { get; set; }
        public decimal cash { get; set; }
        public decimal payCash { get; set; }
        public decimal payCredit { get; set; }
        public decimal payTdebit { get; set; }
        public decimal payTcredit { get; set; }
        public decimal totalDiscount { get; set; }
        public DateTime date { get; set; }
        public string observation { get; set; }
        public int stateId { get; set; }
        public decimal total { get; set; }
        public int tipo { get; set; }
        public decimal saldoCredito { get; set; }
        public string fechaPagoCredito { get; set; }

        public string codConsecutivo { get; set; }
        public int numeroFactura { get; set; }

        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal valorTotalExcentos { get; set; }

        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal valorTotalExcluidos { get; set; }

        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal baseIVA19 { get; set; }

        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal baseIVA5 { get; set; }

        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal valorIVA19 { get; set; }

        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal valorIVA5 { get; set; }

        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal totalBolsas { get; set; }

        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal valorConvenio { get; set; }

        public string facturaFisica { get; set; }
        public string facturacion { get; set; }

        public string PrefijoComprobante { get; set; }
        public string NumeroComprobante { get; set; }

        public bool estado { get; set; }

        [ForeignKey("ProveedorFK")]
        public int IdProveedor { get; set; }


        public virtual persons persons { get; set; }
        public virtual usersTabla usersTabla { get; set; }
        public virtual providers ProveedorFK { get; set; }
        public virtual FormaPago FormaPagoFK { get; set; }
    }
}