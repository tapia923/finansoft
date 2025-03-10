using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanillajeColectivos.DTO.Products
{
    public class pedidosViewModel2
    {      
        
        public int cod { get; set; }
        public int cantidad { get; set; }
        public string Referencia { get; set; }
        public string codigoBarras { get; set; }
        public string nombre { get; set; }
        public string iva { get; set; }
        public string unidad { get; set; }
        public string total { get; set; }
        public int operatioId { get; set; }
    }
}