using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanillajeColectivos.DTO.Contabilidad
{
    [Table("dbo.planCuentas")]
    public class planCuentas
    {
        // Propiedades y métodos existentes

        // Constructor sin parámetros

        public planCuentas()
        {
        }

        // Otros Constructores

        public planCuentas(SomeType param1, Constructores param2)
        {
            // Código de inicialización 
        }

        [Required]
        public string Nombre { get; set; }
        
        [Key]
        [Required]
        [RegularExpression("(^[0-9]+$)", ErrorMessage = "Solo se permiten números")]
        public string codigo { get; set; }
        [Required]
        public string naturaleza { get; set; }
        public bool reqTercero { get; set; }
        public bool reqCosto { get; set; }
        public bool corriente { get; set; }
        public bool validaSaldo { get; set; }
        public decimal saldo { get; set; }
        [Required]
        public string tipo { get; set; }
        [Required]
        public string TipoCuenta { get; set; }
    }
   
}
