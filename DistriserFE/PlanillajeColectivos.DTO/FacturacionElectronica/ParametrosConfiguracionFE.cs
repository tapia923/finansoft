using PlanillajeColectivos.DTO.Geolocalizacion;
using PlanillajeColectivos.DTO.Tipos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanillajeColectivos.DTO.FacturacionElectronica
{
    [Table("dbo.parametrosFacturacionElectronica")]
    public class ParametrosConfiguracionFE
    {
        [Key]
        public int id { get; set; }

        public string COMPANY_NIT { get; set; }
        public string EMISOR_NIT { get; set; }
        public string EMISOR_NOMBRE { get; set; }
        public string CompanyName { get; set; }
        public string CompanyPostCode { get; set; }

        [ForeignKey("cityFK")]
        public int CityCode { get; set; }
        public string Direccion { get; set; }
        public string EMISOR_MATRICULA_MERCANTIL { get; set; }
        public string EMISOR_CORREO_ELECTRONICO { get; set; }
        public string EMISOR_NUMERO_TELEFONICO { get; set; }
        public string SOFTWARE_IDENTIFICADOR { get; set; }
        public string SOFTWARE_PIN { get; set; }
        public string IDENTIFICADOR_SET_PRUEBAS { get; set; }
        public long RANGO_NUMERO_RESOLUCION { get; set; }

        [DataType(DataType.Date)]
        public DateTime RANGO_FECHA_RESOLUCION { get; set; }
        public string RANGO_PREFIJO { get; set; }
        public long RANGO_DESDE { get; set; }
        public long RANGO_HASTA { get; set; }
        public string RANGO_CLAVE_TECNICA { get; set; }

        [DataType(DataType.Date)]
        public DateTime RANGO_VIGENCIA_DESDE { get; set; }

        [DataType(DataType.Date)]
        public DateTime RANGO_VIGENCIA_HASTA { get; set; }
        public string CLAVE_CERTIFICADO { get; set; }
        public int TIPO_AMBIENTE { get; set; }

        [ForeignKey("ResponsabilidadFiscalFK")]
        public int ResponsabilidadFiscal { get; set; }
        public bool estado { get; set; }

        public virtual Municipio cityFK { get; set; } 
        public virtual ResponsabilidadFiscal ResponsabilidadFiscalFK { get; set; } 
    }
}
