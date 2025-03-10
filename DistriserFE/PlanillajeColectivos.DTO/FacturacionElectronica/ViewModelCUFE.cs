using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanillajeColectivos.DTO.FacturacionElectronica
{
    public class ViewModelCUFE
    {
        public string NumFac { get; set; }
        public string FecFac { get; set; }
        public string HoraFac { get; set; }
        public string ValorBruto { get; set; }
        public string CodImp1 { get; set; }
        public string ValorImp1 { get; set; }
        public string CodImp2 { get; set; }
        public string ValorImp2 { get; set; }
        public string CodImp3 { get; set; }
        public string ValorImp3 { get; set; }
        public string ValTot { get; set; }
        public string NitFE { get; set; }
        public string NumAdq { get; set; }
        public string CITec { get; set; }
        public string TipoAmbiente { get; set; }
    }
}
