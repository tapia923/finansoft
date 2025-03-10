using PlanillajeColectivos.DTO.Geolocalizacion;
using PlanillajeColectivos.DTO.Tipos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace PlanillajeColectivos.DTO.Products
{
    [Table("dbo.persons")]
    public class persons
    {

        public int id { get; set; }

        [ForeignKey("TipoIdentificacionFK")]
        public int tipeId { get; set; }

        [Display(Name = "Nombre")]
        [Required]
        public string name { get; set; }

        [RegularExpression(@"^\d+$", ErrorMessage = "SÓLO SE PERMITE NÚMEROS")]
        [Display(Name = "Identificacion")]
        [Required]
        public string nit { get; set; }

        [Display(Name = "Direccion")]
        public string direccion { get; set; }

        [RegularExpression(@"^\d+$", ErrorMessage = "SÓLO SE PERMITE NÚMEROS")]
        [Display(Name = "Celular")]
        public string celular { get; set; }

        [RegularExpression(@"^\d+$", ErrorMessage = "SÓLO SE PERMITE NÚMEROS")]
        [Display(Name = "Telefono - Celular")]
        public string telefono { get; set; }

        [Display(Name = "Correo Electronico")]
        public string email { get; set; }

        [Display(Name = "Cupo Credito")]
        [Required]
        public decimal cupocredito { get; set; }

        public string regime { get; set; }

        public string numeroTarjeta { get; set; }

        [Display(Name = "Regimen Fiscal")]
        [ForeignKey("regimenFiscalFK")]
        [Required]
        public int regimenFiscal { get; set; }

        [Display(Name = "Tipo Contribuyente")]
        [ForeignKey("tipoContribuyenteFK")]
        [Required]
        public int tipoContribuyente { get; set; }

        public string responsabilidadFiscal { get; set; }

        //[Display(Name = "Pais")]
        //[Required]
        //[ForeignKey("paisFK")]
        //public int pais { get; set; }
        


        //[Display(Name = "Departamento")]
        //[Required]
        //[ForeignKey("departamentoFK")]
        //public int departamento { get; set; }

        [Display(Name = "Municipio")]
        [Required]
        [ForeignKey("municipioFK")]
        public int municipio { get; set; }

        public virtual RegimenFiscal regimenFiscalFK { get; set; }
        public virtual TipoContribuyente tipoContribuyenteFK { get; set; }
        //public virtual Pais paisFK { get; set; }
        //public virtual Departamento departamentoFK { get; set; }
        public virtual Municipio municipioFK { get; set; }
        public virtual TipoIdentificacion TipoIdentificacionFK { get; set; }
    }
}