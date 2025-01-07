using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanillajeColectivos.DTO.FacturacionElectronica
{
    [Table("fe.MotivosNotaCredito")]
    public class MotivoNotaCredito
    {
        [Key]
        public int Id { get; set; }
        public string Descripcion { get; set; }



        public List<MotivoNotaCredito> Listar()
        {
            var Motivos = new List<MotivoNotaCredito>();
            try
            {
                using (var ctx = new AccountingContext())
                {
                    Motivos = ctx.MotivosNotaCredito.ToList();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return Motivos;
        }
    }
}
