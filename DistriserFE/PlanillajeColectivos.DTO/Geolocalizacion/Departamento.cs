using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanillajeColectivos.DTO.Geolocalizacion
{
    [Table("geo.Departamento")]
    public class Departamento
    {

        [Key]
        [Required, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id_dep { get; set; }

        [StringLength(60)]
        public string Nom_dep { get; set; }

        [ForeignKey("paisFK")]
        public int Pais_dep { get; set; }

        [ForeignKey("Pais_dep")]
        Pais Pais { get; set; }

        public string codigo { get; set; }


        //[Key]
        //public int Id_dep { get; set; }
        //public string Nom_dep { get; set; }

        //[ForeignKey("paisFK")]
        //public int Pais_dep { get; set; }

        public virtual Pais paisFK { get; set; }
    }
}
