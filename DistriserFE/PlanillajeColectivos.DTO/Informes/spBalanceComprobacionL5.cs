using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanillajeColectivos.DTO.Informes
{
    public partial class spBalanceComprobacionL5
    {
        [Key]
        public string codigo { get; set; }
        public string nombre { get; set; }
        public string DocumentoTercero { get; set; }
        public string NombreTercero { get; set; }

        public string SaldoInicial { get; set; }
        public string Debito { get; set; }
        public string Credito { get; set; }
        public string Saldo { get; set; }
    }
}
