using PlanillajeColectivos.DTO.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanillajeColectivos.DTO.FacturacionElectronica.FacturaPD
{
    public class ViewModelFEpdf
    {
        public ParametrosConfiguracionFE ParametrosFE { get; set; }
        public factura Factura { get; set; }
        public ModelAdquiriente Adquiriente { get; set; }
        public List<ModelListProductos> ListOperaciones { get; set; }

        public string FechaEmision { get; set; }
        public string HoraEmision { get; set; }
        public string FechaVencimiento { get; set; }
        public string Cufe { get; set; }
        public string NumeroFactura { get; set; }
        public string SubtotalPrecioUnitario { get; set; }
        public string DescuentosDetalle { get; set; }
        public string RecargosDetalle { get; set; }
        public string SubtotalNoGravados { get; set; }
        public string SubtotalBaseGravable { get; set; }
        public string TotalImpuesto { get; set; }
        public string TotalMasImpuesto { get; set; }
        public string DescuentoGlobal { get; set; }
        public string RecargoGlobal { get; set; }
        public string Anticipo { get; set; }
        public string ValorTotal { get; set; }
        public string ValorTotalEnLetras { get; set; }
        public string QR { get; set; }
    }
}
