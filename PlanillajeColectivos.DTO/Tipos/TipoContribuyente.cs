﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanillajeColectivos.DTO.Tipos
{
    [Table("dbo.tipoContribuyente")]
    public class TipoContribuyente
    {
        [Key]
        public int id { get; set; }
        public string descripcion { get; set; }
        public string codigo { get; set; }
    }
}
