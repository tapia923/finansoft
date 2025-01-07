
namespace PlanillajeColectivos.DTO
{
    using PlanillajeColectivos.DTO.EntidadDos;
    using System.Data.Entity;


    public class ContextTwo : DbContext
    {
        public object descuentosPlanillas;

        public ContextTwo()
            : base("AccContextTwo")
        {
        }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer<AccountingContext>(null);
            base.OnModelCreating(modelBuilder);
        }

        public virtual DbSet<cuposCredito> cuposCredito { get; set; }
    }
}
