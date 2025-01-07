using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanillajeColectivos.DTO.FacturacionElectronica
{
    public class ViewModelInvoiceFE
    {
        public int Id { get; set; }
        public string Prefijo { get; set; }
        public string Numero { get; set; }
        public string Cufe { get; set; }
        public string Fecha { get; set; }
        public string Hora { get; set; }
        public string Cliente { get; set; }
        public bool EstadoEnvioEmail { get; set; }
    }
}
