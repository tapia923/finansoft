using PlanillajeColectivos.DTO;
using PlanillajeColectivos.DTO.Contabilidad;
using PlanillajeColectivos.DTO.FacturacionElectronica;
using PlanillajeColectivos.DTO.FacturacionElectronica.FacturaPD;
using PlanillajeColectivos.DTO.Products;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PlanillajeColectivos.Areas.Procesos.Controllers
{
    public class ConsultasController : Controller
    {
        NumberFormatInfo formato = new CultureInfo("es-CO").NumberFormat;
        public List<iva> GetListIva()
        {
            using(var ctx = new AccountingContext())
            {
                var data = ctx.iva.ToList();
                return data;
            }
        }

        public List<SelectListItem> GetNiveles()
        {
            List<SelectListItem> niveles = new List<SelectListItem>();
            niveles.Add(new SelectListItem { Text = "Nivel", Value = "" });
            niveles.Add(new SelectListItem { Text = "1", Value = "1" });
            niveles.Add(new SelectListItem { Text = "2", Value = "2" });
            niveles.Add(new SelectListItem { Text = "3", Value = "3" });
            niveles.Add(new SelectListItem { Text = "4", Value = "4" });
            niveles.Add(new SelectListItem { Text = "5", Value = "5" });
            //fin list niveles cuentas

            return niveles;
        }

        public List<SelectListItem> GetCentroCostos()
        {

            using (var ctx = new AccountingContext())
            {
                List<SelectListItem> costos = new List<SelectListItem>();
                costos.Add(new SelectListItem { Text = "Centro de costo", Value = "" });
                IList<centroCosto> listadoCentroCostos = ctx.centroCostos.ToList();
                foreach (var item in listadoCentroCostos)
                {
                    costos.Add(new SelectListItem { Text = item.nombre, Value = item.codigo });  // agrego los elementos de la db a la primera lista que cree

                }
                return costos;
            }

        }

        public RegistroFE GetModelRegistroFE(string registro, string[] dianResponse)
        {

            string stringDR = "";
            if(dianResponse!=null)
            {
                foreach (var item in dianResponse)
                {
                    stringDR += item;
                }
            }

            var model = new RegistroFE()
            {
                registro = registro,
                dianResponse = stringDR
            };
            return model;
            
        }

        public InvoiceFacturaElectronica GetModelInvoiceFE(string Prefijo,int Numero,string Cufe,DateTime Fecha,int IdCliente,bool EstadoEmail,string NombreDocumento,string IdFactura)
        {
            var Invoice = new InvoiceFacturaElectronica()
            {
                Prefijo = Prefijo,
                Numero = Numero,
                Cufe = Cufe,
                AlgoritmoCufe = "SHA384",
                TipoDocumento = "01",
                Fecha = Fecha,
                IdCliente = IdCliente,
                EstadoEnvioEmail = EstadoEmail,
                NombreDocumento = NombreDocumento,
                IdFactura = IdFactura
            };
            return Invoice;
        }

        public ModelAdquiriente GetModelAdquiriente(int Id)
        {
            using(var ctx = new AccountingContext())
            {
                var data = ctx.persons.Where(x => x.id == Id).FirstOrDefault();
                ModelAdquiriente model = null;
                if(Id!=1)
                {
                    model = new ModelAdquiriente()
                    {
                        NombreAdquiriente = data.name,
                        DocumentoAdquiriente = data.nit,
                        DireccionAdquiriente = (data.municipioFK!=null) ? (data.municipioFK.departamentoFK.paisFK.Nom_pais + "," + data.municipioFK.departamentoFK.Nom_dep + ", " + data.municipioFK.Nom_muni).ToUpper() + " " + data.direccion : "",
                        CorreoAdquiriente = (data.email!=null) ? data.email : "",
                        ContactoAdquiriente = (data.celular!=null) ? data.celular : "",
                    };
                }
                else
                {
                    model = new ModelAdquiriente()
                    {
                        NombreAdquiriente = data.name,
                        DocumentoAdquiriente = "",
                        DireccionAdquiriente = "",
                        CorreoAdquiriente = "",
                        ContactoAdquiriente = "",
                    };
                }
                
                return model;
            }
            
        }

        public List<ModelListProductos> ListProductos(List<operation> operation)
        {
            formato.CurrencyGroupSeparator = ".";
            formato.NumberDecimalSeparator = ",";
            var Listado = new List<ModelListProductos>();

            int contador = 1;
            decimal ValUnitario = 0;
            decimal Impuesto = 0;
            decimal iva = 0;
            foreach(var item in operation)
            {
                if (item.products.ivaId == 1) { iva = Convert.ToDecimal(1.19); } else if (item.products.ivaId == 2) { iva = Convert.ToDecimal(1.05); } else { iva = 0; }
                ValUnitario = (iva>0) ? Math.Round(item.price / iva, 0, MidpointRounding.ToEven) : item.price;
                Impuesto = item.price - ValUnitario;
                var model = new ModelListProductos()
                {
                    Id = contador,
                    Descripcion = item.products.barcode + "-" + item.products.name,
                    Cantidad = item.quantity,
                    ValorUnitario = ValUnitario.ToString("N0",formato)+" / "+Impuesto.ToString("N0",formato)+" "+item.products.ivaFK.name,
                    Impuesto = (Impuesto*item.quantity).ToString("N0", formato),
                    ValorTotal = (ValUnitario*item.quantity).ToString("N0", formato)
                };
                contador++;
                Listado.Add(model);
            }

            return Listado;
        }

        public List<persons> GetTerceros()
        {
            using(var ctx = new AccountingContext())
            {
                var Listado = ctx.persons.ToList();
                return Listado;
            }
        }
        public List<operation> GetOperation()
        {
            var Operacion = new List<operation>();
            try
            {
                using (var ctx = new AccountingContext())
                {
                    Operacion = ctx.operation.ToList();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return Operacion;
        }

    }
}