using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanillajeColectivos.DTO.Geolocalizacion
{
    [Table("geo.Pais")]
    public class Pais
    {
        [Key]
        public int Id_pais { get; set; }
        public string Nom_pais { get; set; }
        public string Codigo { get; set; }
        public string Nomenclatura { get; set; }
    }
}
