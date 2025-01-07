
using PlanillajeColectivos.DTO.FacturacionElectronica;
using System.Data.Entity;

namespace PlanillajeColectivos.DTO
{
    using PlanillajeColectivos.DTO.Contabilidad;
    using PlanillajeColectivos.DTO.FacturacionElectronica;
    using PlanillajeColectivos.DTO.Geolocalizacion;
    using PlanillajeColectivos.DTO.Otros;
    using PlanillajeColectivos.DTO.Products;
    using PlanillajeColectivos.DTO.Tipos;
    using System.Data.Entity;


    public class AccountingContext : DbContext
    {
        public object descuentosPlanillas;

        public AccountingContext()
            : base("AccContext")
        {
        }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer<AccountingContext>(null);
            base.OnModelCreating(modelBuilder);
        }

        public virtual DbSet<products> products { get; set; }
        public virtual DbSet<categorias> categorias { get; set; }
        public virtual DbSet<consecutivosFacturas> consecutivosFacturas { get; set; }
        public virtual DbSet<factura> factura { get; set; }
        public virtual DbSet<iva> iva { get; set; }
        public virtual DbSet<operation> operation { get; set; }
        public virtual DbSet<pedidoInicial> pedidoInicial { get; set; }
        public virtual DbSet<persons> persons { get; set; }
        public virtual DbSet<state> state { get; set; }
        public virtual DbSet<unitMeasure> unitMeasure { get; set; }
        public virtual DbSet<usersTabla> usersTabla { get; set; }       
        public virtual DbSet<creditos> creditos { get; set; }
        

        public virtual DbSet<planCuentas> planCuentas { get; set; }
        public virtual DbSet<movimientos> movimientos { get; set; }
        public virtual DbSet<movimientosDos> movimientosDos { get; set; }
        public virtual DbSet<centroCosto> centroCostos { get; set; }
        public virtual DbSet<claseComprobante> ClaseComprobantes { get; set; }
        public virtual DbSet<tipoComprobante> TipoComprobantes { get; set; }
        public virtual DbSet<FormaPago> FormaPagos { get; set; }
        public virtual DbSet<comprobante> comprobantes { get; set; }

        public virtual DbSet<cashBackModel> cashBackAcco { get; set; }
        public virtual DbSet<economicActivity> economicActivities { get; set; }
        public virtual DbSet<location> locations { get; set; }
        public virtual DbSet<retention> retentions { get; set; }
        public virtual DbSet<providers> providers { get; set; }
        public virtual DbSet<RegimenFiscal> RegimenFiscal { get; set; }
        public virtual DbSet<ResponsabilidadFiscal> ResponsabilidadFiscal { get; set; }
        public virtual DbSet<TipoContribuyente> TipoContribuyente { get; set; }
        public virtual DbSet<TipoIdentificacion> TipoIdentificacion { get; set; }
        public virtual DbSet<presentation> presentation { get; set; }
        public virtual DbSet<Pais> Pais { get; set; }
        public virtual DbSet<Departamento> Departamento { get; set; }
        public virtual DbSet<Municipio> Municipio { get; set; }

        #region ACUSE RECIBIDO COMPRAS
        //public DbSet<AcuseReciboCompra> AcuseReciboCompra { get; set; }
        //#endregion


        #region FACTURACIÓN ELECTRÓNICA
        public virtual DbSet<ParametrosConfiguracionFE> ParametrosFE { get; set; }
        public virtual DbSet<RegistroFE> RegistrosFE { get; set; }
        public virtual DbSet<InvoiceFacturaElectronica> InvoiceFacturasElectronicas { get; set; }
        public virtual DbSet<InvoiceNC> InvoicesNC { get; set; }
        public virtual DbSet<MotivoNotaCredito> MotivosNotaCredito { get; set; }
        #endregion
    }
}
#endregion