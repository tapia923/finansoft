using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanillajeColectivos.DTO.FacturacionElectronica.FacturaPD
{
    public class ModelListProductos
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }
        public int Cantidad { get; set; }
        public string ValorUnitario { get; set; }
        public string Impuesto { get; set; }
        public string ValorTotal { get; set; }
    }
}
