using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanillajeColectivos.DTO.Products
{
    [Table("dbo.products")]
    public class products
    {
        [Key]
        public int id { get; set; }

        [Display(Name = "Familia de Producto")]
        public int productIdSubPresentation { get; set; }

        [Display(Name = "Codigo de Barras")]
        public string barcode { get; set; }

        [Display(Name = "Nombre")]
        [Required]
        public string name { get; set; }

        [Display(Name = "Minima en Inventario")]
        [Required]
        public int inventaryMin { get; set; }

        [Display(Name = "Precio de Entrada")]
        [Required]
        public decimal priceIn { get; set; }

        [Display(Name = "Precio 1")]
        [Required]
        public decimal priceOut { get; set; }

        [Display(Name = "Precio 2")]
        [Required]
        public decimal priceOut2 { get; set; }

        [Display(Name = "Tipo de Presentacion")]
        [ForeignKey("presentationFK")]
        public int presentationId { get; set; }
        public string userId { get; set; }

        [Display(Name = "Proveedor")]
        [ForeignKey("providerFK")]
        [Required]
        public int providerId { get; set; }

        [ForeignKey("ivaFK")]
        [Display(Name = "Iva")]
        [Required]
        public int ivaId { get; set; }

        [Display(Name = "Unidad de Medida")]
        [ForeignKey("medidaFK")]
        [Required]
        public int unitMeasureId { get; set; }

        [Display(Name = "Inventario Inicial")]
        [Required]
        public int initialQuantity { get; set; }

        [ForeignKey("categoriaFK")]
        [Display(Name = "Categoria")]
        [Required]
        public int categoriaId { get; set; }

        public string detalleIva { get; set; }

        public bool activo { set; get; }
   
        public virtual  categorias categoriaFK { get; set; }
        public virtual  iva ivaFK { get; set; }
        public virtual  providers providerFK { get; set; }
        public virtual  unitMeasure medidaFK { get; set; }
        public virtual  presentation presentationFK { get; set; }


        

    }
}