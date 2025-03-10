﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanillajeColectivos.DTO.Contabilidad
{
    [Table("dbo.clasesComprobantes")]
    public class claseComprobante
    {
        public int id { get; set; }
        public string codigo { get; set; }
        public string nombre { get; set; }
    }
}