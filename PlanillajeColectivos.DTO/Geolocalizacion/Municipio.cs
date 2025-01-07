using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanillajeColectivos.DTO.Geolocalizacion
{
    [Table("geo.Municipio")]
    public class Municipio
    {
        [Key]
        [Required, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id_muni { get; set; }

        [StringLength(60)]
        public string Nom_muni { get; set; }

        [ForeignKey("departamentoFK")]
        public int Dep_muni { get; set; }
        public string codigo { get; set; }

        
        public virtual Departamento departamentoFK { get; set; }

    }
}
