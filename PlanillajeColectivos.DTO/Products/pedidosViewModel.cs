using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanillajeColectivos.DTO.Products
{
    public class pedidosViewModel
    {      
        public int cod { get; set; }
        public int cantidad { get; set; }
        public string codigoBarras { get; set; }
        public string nombre { get; set; }
        
        public string iva { get; set; }
        public decimal unidad { get; set; }
        public decimal subtotal { get; set; }
        public decimal descuento { get; set; }
        public decimal total { get; set; }
        public int operatioId { get; set; }
        public string nombreProveedor { get; set; }
    }
}