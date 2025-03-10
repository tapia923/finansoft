using PlanillajeColectivos.DTO.Geolocalizacion;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanillajeColectivos.DTO.Products
{
    [Table("dbo.providers")]
    public class providers
    {
        public int id { get; set; }

        public int economicActivityCode { get; set; }
        public int locationId { get; set; }
        public int retentionId { get; set; }
        public int taxClassification { get; set; }
        public int industryAndCommerce { get; set; }
        public int ivaId { get; set; }

        [Display(Name = "Nombre")]
        public string name { get; set; }

        [Display(Name = "NIT")]
        public string nit { get; set; }

        [Display(Name = "Direccion")]
        public string address { get; set; }

        [RegularExpression(@"^\d+$", ErrorMessage = "SÓLO SE PERMITE NÚMEROS")]
        [Display(Name = "Celular")]
        public string cell { get; set; }

        [RegularExpression(@"^\d+$", ErrorMessage = "SÓLO SE PERMITE NÚMEROS")]
        [Display(Name = "Telefono - Celular")]
        public string phone { get; set; }

        [Display(Name = "Correo Electronico")]
        public string email { get; set; }

        [Required]
        [ForeignKey("municipioFK")]
        public int municipio { get; set; }

        public virtual Municipio municipioFK { get; set; }
    }
}