using Common;
using FE.Documentos;
using FE.pdf;
using FE.ServiciosWeb;
using FE.ServiciosWeb.DianServices;
using FE.Tipos.Standard;
using FEDian.Firma;
using FEDIAN.Util;
using Newtonsoft.Json;
using PlanillajeColectivos.Areas.Contabilidad.Controllers;
using PlanillajeColectivos.Areas.Facturas.Controllers;
using PlanillajeColectivos.DTO;
using PlanillajeColectivos.DTO.Contabilidad;
using PlanillajeColectivos.DTO.FacturacionElectronica;
using PlanillajeColectivos.DTO.FacturacionElectronica.FacturaPD;
using PlanillajeColectivos.DTO.Otros;
using PlanillajeColectivos.DTO.Products;
using PlanillajeColectivos.Tools;
using Rotativa;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Web.Mvc;
using System.Xml;


namespace PlanillajeColectivos.Areas.Procesos.Controllers
{
    public class ProcesosController : Controller
    {
        private AccountingContext db = new AccountingContext();
        private ContextTwo db2 = new ContextTwo();
        NumberFormatInfo formato = new CultureInfo("es-CO").NumberFormat;

        public static bool AMBIENTE_PRUEBAS = true;
        public static AmbienteServicio AMBIENTE_SERVICIO = AMBIENTE_PRUEBAS ? AmbienteServicio.PRUEBAS : AmbienteServicio.PRODUCCION;
        public static AmbienteDestino AMBIENTE_DESTINO = AMBIENTE_PRUEBAS ? AmbienteDestino.PRUEBAS : AmbienteDestino.PRODUCCION;
        public static string RUTA_CERTIFICADO = System.AppDomain.CurrentDomain.BaseDirectory+ "Certificado\\Certificado.pfx";
        //public static string RUTA_CERTIFICADO = "D:\\certificadodefacturacionelectronica.p12";
        public static string CLAVE_CERTIFICADO = "XiHUkxenIYsFjAur";
        public static bool USAR_SET_PUEBAS = false;
        public static string IDENTIFICADOR_SET_PRUEBAS = "e9fdbf66-fd29-428c-9e4b-cf63a31ca19e";


        [Authorize]
        public void migrarMovimientos()
        {
            //var movimientos = db.movimientos.ToList();

            //foreach(var item in movimientos)
            //{
            //    if(item.tipo == "ventaCaja")
            //    {

            //    }
            //}
        }

        [Authorize]
        public ActionResult venderCaja()
        {

            var ListFormasPago = new formasPagoController().GetListFormasPago().ToList().Select(x => new SelectListItem { Text = x.nombre, Value = x.id.ToString(), Selected = false });
            // lista de clientes
            List<SelectListItem> items3 = new List<SelectListItem>();   // Creo una lista
            IList<persons> lista4 = db.persons.Where(m => m.tipeId == 1 && m.id != 1274).ToList();// extraigo los elementos desde la DB

            foreach (var item in lista4)		// recorro los elementos de la db
            {
                items3.Add(new SelectListItem { Text = item.name + "|" + item.nit, Value = item.id.ToString() });  // agrego los elementos de la db a la primera lista que cree
                //text: el texto que se muestra
                //value: el valor interno del dropdown
            }

            ViewBag.clientes = items3;
            ViewBag.ListFormasPago = ListFormasPago;
            var products = db.products.ToList();
          
          
            return View(products);
        }

        [Authorize]
        public JsonResult GetInfoPerson(int id)
        {
            var persona = db.persons.Find(id);

            var getTerceroCount = (from pc in db2.cuposCredito where pc.NIT == persona.nit select pc).Count();
            if(getTerceroCount != 0)
            {
                var getTercero = (from pc in db2.cuposCredito where pc.NIT == persona.nit select pc).First();
                return Json(getTercero.acumulado, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("no", JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize]
        public ActionResult vender()
        {
            // lista de clientes
            List<SelectListItem> items3 = new List<SelectListItem>();   // Creo una lista
            items3.Add(new SelectListItem { Text = "Seleccione Un Cliente", Value = "" });
            IList<persons> lista4 = db.persons.Where(m => m.tipeId == 1).ToList();// extraigo los elementos desde la DB

            //IList<Garantias> lista4 = db.Garantias.Where(m => m.Garantias_Id == 1 && m.Garantias_Codeudor == true).ToList();
            foreach (var item in lista4)		// recorro los elementos de la db
            {
                //items3.Add(new SelectListItem { Text = item.Garantias_Descripcion + " | " + item.Garantias_Codeudor, Value = item.Garantias_Id.ToString() });  // agrego los elementos de la db a la primera lista que cree
                items3.Add(new SelectListItem { Text = item.name + " | " +item.nit, Value = item.id.ToString() });  // agrego los elementos de la db a la primera lista que cree
                //text: el texto que se muestra
                //value: el valor interno del dropdown
            }

            ViewBag.clientes = items3;

            var products = db.products.ToList();

            foreach (var item in products)
            {
                var productInOperation = (from pc in db.operation where pc.productId == item.id select pc).ToList();
                var cantidad = 0;
                foreach (var operation in productInOperation)
                {
                    if (operation.operationType.tipo == 1)
                    {
                        cantidad = cantidad + operation.quantity;
                    }
                    else if (operation.operationType.tipo == 2)
                    {
                        cantidad = cantidad - operation.quantity;
                    }                   
                }
                item.initialQuantity = cantidad;
            }
            return View(products);
        }

        // GET: Procesos/Procesos
        [Authorize(Roles = "Admin")]
        public ActionResult abastecer()
        {
            List<SelectListItem> items = new List<SelectListItem>();   // Creo una lista
            items.Add(new SelectListItem { Text = "--SELECCIONE--", Value = "0" });
            IList<providers> lista = db.providers.ToList();// extraigo los elementos desde la DB

            foreach (var item in lista)		// recorro los elementos de la db
            {
                items.Add(new SelectListItem { Text = item.name + "|" + item.nit, Value = item.id.ToString() });  // agrego los elementos de la db a la primera lista que cree
            }

            var ListIva = new ConsultasController().GetListIva().ToList().Select(x => new SelectListItem { Text = x.name, Value = x.id.ToString(), Selected = false });
            var ListFormasPago = new formasPagoController().GetListFormasPago().ToList().Select(x => new SelectListItem { Text = x.nombre, Value = x.id.ToString(), Selected = false });

            ViewBag.providers = items;
            ViewBag.ListIva = ListIva;
            ViewBag.ListFormasPago = ListFormasPago;

            var products = db.products.ToList();
            
            return View(products);
        }

        [Authorize]
        public ActionResult createCredit()
        {
            // lista de clientes
            List<SelectListItem> items3 = new List<SelectListItem>();   // Creo una lista
            items3.Add(new SelectListItem { Text = "CONSUMIDOR FINAL", Value = "1274" });
            IList<persons> lista4 = db.persons.Where(m => m.tipeId == 1 && m.id != 1274).ToList();// extraigo los elementos desde la DB

            foreach (var item in lista4)		// recorro los elementos de la db
            {
                items3.Add(new SelectListItem { Text = item.name + "|" + item.nit, Value = item.id.ToString() });  // agrego los elementos de la db a la primera lista que cree
                //text: el texto que se muestra
                //value: el valor interno del dropdown
            }

            ViewBag.clientes = items3;

            var products = db.products.ToList();

            foreach (var item in products)
            {
                var productInOperation = (from pc in db.operation where pc.productId == item.id select pc).ToList();
                var cantidad = 0;
                foreach (var operation in productInOperation)
                {
                    if (operation.operationType.tipo == 1)
                    {
                        cantidad = cantidad + operation.quantity;
                    }
                    else if (operation.operationType.tipo == 2)
                    {
                        cantidad = cantidad - operation.quantity;
                    }
                }
                item.initialQuantity = cantidad;
            }
            return View(products);
        }

        [Authorize]
        public ActionResult pedidoCredito()
        {
            // lista de clientes
            List<SelectListItem> items3 = new List<SelectListItem>();   // Creo una lista
            items3.Add(new SelectListItem { Text = "Seleccione Un Cliente", Value = "" });
            IList<persons> lista4 = db.persons.Where(m => m.tipeId == 1).ToList();// extraigo los elementos desde la DB

            //IList<Garantias> lista4 = db.Garantias.Where(m => m.Garantias_Id == 1 && m.Garantias_Codeudor == true).ToList();
            foreach (var item in lista4)		// recorro los elementos de la db
            {
                //items3.Add(new SelectListItem { Text = item.Garantias_Descripcion + " | " + item.Garantias_Codeudor, Value = item.Garantias_Id.ToString() });  // agrego los elementos de la db a la primera lista que cree
                items3.Add(new SelectListItem { Text = item.name, Value = item.id.ToString() });  // agrego los elementos de la db a la primera lista que cree
                //text: el texto que se muestra
                //value: el valor interno del dropdown
            }

            ViewBag.clientes = items3;

            var products = db.products.ToList();

            foreach (var item in products)
            {
                var productInOperation = (from pc in db.operation where pc.productId == item.id select pc).ToList();
                var cantidad = 0;
                foreach (var operation in productInOperation)
                {
                    if (operation.operationType.tipo == 1)
                    {
                        cantidad = cantidad + operation.quantity;
                    }
                    else if (operation.operationType.tipo == 2)
                    {
                        cantidad = cantidad - operation.quantity;
                    }
                }
                item.initialQuantity = cantidad;
            }
            return View(products);
        }

        [Authorize]
        public ActionResult pedidoContado()
        {
            //lista de clientes
            List<SelectListItem> items3 = new List<SelectListItem>();   // Creo una lista
            items3.Add(new SelectListItem { Text = "Seleccione Un Cliente", Value = "" });
            IList<persons> lista4 = db.persons.Where(m => m.tipeId == 1).ToList();// extraigo los elementos desde la DB

            //IList<Garantias> lista4 = db.Garantias.Where(m => m.Garantias_Id == 1 && m.Garantias_Codeudor == true).ToList();
            foreach (var item in lista4)		// recorro los elementos de la db
            {
                //items3.Add(new SelectListItem { Text = item.Garantias_Descripcion + " | " + item.Garantias_Codeudor, Value = item.Garantias_Id.ToString() });  // agrego los elementos de la db a la primera lista que cree
                items3.Add(new SelectListItem { Text = item.name, Value = item.id.ToString() });  // agrego los elementos de la db a la primera lista que cree
                //text: el texto que se muestra
                //value: el valor interno del dropdown
            }

            ViewBag.clientes = items3;

            var products = db.products.ToList();

            foreach (var item in products)
            {
                var productInOperation = (from pc in db.operation where pc.productId == item.id select pc).ToList();
                var cantidad = 0;
                foreach (var operation in productInOperation)
                {
                    if (operation.operationType.tipo == 1)
                    {
                        cantidad = cantidad + operation.quantity;
                    }
                    else if (operation.operationType.tipo == 2)
                    {
                        cantidad = cantidad - operation.quantity;
                    }
                }
                item.initialQuantity = cantidad;
            }
            return View(products);
        }

        [Authorize]
        public JsonResult AddProductoVendiendoCaja(int id, int cantidad, int precioSelect, decimal precioOtro)
        {
            var producto = (from pc in db.products where pc.id == id select pc).Single();
            var userId = CurrentUser.Get.UserId;

            decimal precioColocar = 0;
            if (precioSelect == 1)
            {
                precioColocar = producto.priceOut;
            }
            else if (precioSelect == 2)
            {
                precioColocar = producto.priceOut2;
            }
            else if (precioSelect == 3)
            {
                precioColocar = precioOtro;
            }
            var nuevaOpercion = new operation()
            {
                productId = id,
                quantity = cantidad,
                operationTypeId = 14,
                date = DateTime.Now,
                price = precioColocar,
                userId = userId
            };

            db.operation.Add(nuevaOpercion);
            db.SaveChanges();

            return Json(1, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public JsonResult AddProductoDevolviendoNC(int id, int cantidad, int precioSelect, decimal precioOtro, int IdFacturaFE)
        {
            var producto = (from pc in db.products where pc.id == id select pc).Single();
            var userId = CurrentUser.Get.UserId;

            var InfoFE = db.InvoiceFacturasElectronicas.Find(IdFacturaFE);
            var RegistroFactura = db.factura.Where(x => x.codConsecutivo == InfoFE.Prefijo && x.numeroFactura == InfoFE.Numero).FirstOrDefault();

            decimal precioColocar = 0;
            if (precioSelect == 1)
            {
                precioColocar = producto.priceOut;
            }
            else if (precioSelect == 2)
            {
                precioColocar = producto.priceOut2;
            }
            else if (precioSelect == 3)
            {
                precioColocar = precioOtro;
            }
            var nuevaOpercion = new operation()
            {
                productId = id,
                quantity = cantidad,
                operationTypeId = 19,
                date = DateTime.Now,
                price = precioColocar,
                userId = userId,
                facturaId = RegistroFactura.id
            };

            db.operation.Add(nuevaOpercion);
            db.SaveChanges();

            return Json(1, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public JsonResult AddProductoVendiendo(int id, int cantidad, decimal valor)
        {
            var producto = (from pc in db.products where pc.id == id select pc).Single();
            var userId = CurrentUser.Get.UserId;

            var nuevaOpercion = new operation()
            {
                productId = id,
                quantity = cantidad,
                operationTypeId = 10,
                date = DateTime.Now,
                price = valor,
                userId = userId
            };

            db.operation.Add(nuevaOpercion);
            db.SaveChanges();

            return Json(1, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public JsonResult AddProductoAbastecer(int id, int cantidad, decimal nuevoPrecio, int OD,string VD,string IvaId)
        {
            var userId = CurrentUser.Get.UserId;
            decimal ValorDescuento = 0;
            if(OD == 1)
            {
                VD = VD.Replace(".", "");
                ValorDescuento = Convert.ToDecimal(VD);
                if(ValorDescuento>(nuevoPrecio*cantidad))
                {
                    ValorDescuento = nuevoPrecio * cantidad;
                }
            }
            else
            {
                decimal Porcentaje = decimal.Divide(Convert.ToInt32(VD),100);
                ValorDescuento = Convert.ToInt32(nuevoPrecio*Porcentaje)*cantidad;
            }

            var nuevaOpercion = new operation()
            {
                productId = id,
                quantity = cantidad,
                operationTypeId = 12,
                date = DateTime.Now,
                price = nuevoPrecio,
                discount = ValorDescuento,
                IvaId = (IvaId != "") ? Convert.ToInt32(IvaId):0,
                userId = userId
            };

            db.operation.Add(nuevaOpercion);
            db.SaveChanges();

            return Json(1, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public JsonResult AddProductoContado(int id, int cantidad)
        {
            var producto = (from pc in db.products where pc.id == id select pc).Single();
            var userId = CurrentUser.Get.UserId;

            var nuevaOpercion = new operation()
            {
                productId = id,
                quantity = cantidad,
                operationTypeId = 8,
                date = DateTime.Now,
                price = producto.priceOut,
                userId = userId
            };

            db.operation.Add(nuevaOpercion);
            db.SaveChanges();

            return Json(1, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public JsonResult AddProductoCredito(int id, int cantidad)
        {
            var producto = (from pc in db.products where pc.id == id select pc).Single();
            var userId = CurrentUser.Get.UserId;

            var nuevaOpercion = new operation()
            {
                productId = id,
                quantity = cantidad,
                operationTypeId = 9,
                date = DateTime.Now,
                price = producto.priceOut2,
                userId = userId
            };

            db.operation.Add(nuevaOpercion);
            db.SaveChanges();

            return Json(1, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public JsonResult AddProductoVenderCredito(int id, int cantidad)
        {
            var producto = (from pc in db.products where pc.id == id select pc).Single();
            var userId = CurrentUser.Get.UserId;

            var nuevaOpercion = new operation()
            {
                productId = id,
                quantity = cantidad,
                operationTypeId = 11,
                date = DateTime.Now,
                price = producto.priceOut2,
                userId = userId
            };

            db.operation.Add(nuevaOpercion);
            db.SaveChanges();

            return Json(1, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public JsonResult verificarExistencia(int id, int cantidadPedida)
        {
            var Data = db.products.Find(id);
            if (Data.initialQuantity < cantidadPedida)
            {
                return new JsonResult { Data = new { status = false } };
            }
            else { return new JsonResult { Data = new { status = true } }; }

            
        }

        [Authorize]
        public JsonResult DeleteProductoContado(int id)
        {
            operation operation = db.operation.Find(id);
            db.operation.Remove(operation);
            db.SaveChanges();

            return Json(1, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public ActionResult getPedidoContado()
        {
            var userId = CurrentUser.Get.UserId;
            //lista de Costos Adicionales
            List<SelectListItem> pedidoContado = new List<SelectListItem>();   // Creo una lista
            var listaOperaciones = db.operation.Where(p => p.operationTypeId == 8 && p.userId == userId).ToList();

            List<pedidosViewModel> enviarPedidosContado = new List<pedidosViewModel>();
            foreach (var item in listaOperaciones)
            {
                decimal total = 0;
                total = item.quantity * item.price;
                var productos = new pedidosViewModel()
                {
                    cantidad = item.quantity,
                    codigoBarras = item.products.barcode.ToString(),
                    nombre = item.products.name,
                    unidad = item.price,
                    total = total,
                    operatioId = item.id
                };

                enviarPedidosContado.Add(productos);
            }

            string json = JsonConvert.SerializeObject(enviarPedidosContado);

            return Json(json);
        }

        [Authorize]
        public ActionResult getVendiendoCaja()
        {
            var userId = CurrentUser.Get.UserId;
            //lista de Costos Adicionales
            List<SelectListItem> pedidoContado = new List<SelectListItem>();   // Creo una lista
            var listaOperaciones = db.operation.Where(p => p.operationTypeId == 14 && p.userId == userId).ToList();

            List<pedidosViewModel> enviarPedidosContado = new List<pedidosViewModel>();
            foreach (var item in listaOperaciones)
            {
                decimal total = 0;
                total = item.quantity * item.price;
                var productos = new pedidosViewModel()
                {
                    cantidad = item.quantity,
                    codigoBarras = item.products.barcode.ToString(),
                    nombre = item.products.name,
                    unidad = item.price,
                    total = total,
                    operatioId = item.id
                };

                enviarPedidosContado.Add(productos);
            }

            string json = JsonConvert.SerializeObject(enviarPedidosContado);

            return Json(json);
        }

        [Authorize]
        public ActionResult getDevolviendoNC(int IdFacturaFE)
        {
            var FacturaE = db.InvoiceFacturasElectronicas.Find(IdFacturaFE);//encontramos el registro de la factura electronica
            var Factura = db.factura.Where(x => x.codConsecutivo == FacturaE.Prefijo && x.numeroFactura == FacturaE.Numero).FirstOrDefault();
            var listaOperaciones = db.operation.Where(p => p.operationTypeId == 19 && p.facturaId == Factura.id).ToList();

            List<pedidosViewModel> Devoluciones = new List<pedidosViewModel>();
            foreach (var item in listaOperaciones)
            {
                decimal total = 0;
                total = item.quantity * item.price;
                var productos = new pedidosViewModel()
                {
                    cantidad = item.quantity,
                    codigoBarras = item.products.barcode.ToString(),
                    nombre = item.products.name,
                    unidad = item.price,
                    total = total,
                    operatioId = item.id
                };

                Devoluciones.Add(productos);
            }

            string json = JsonConvert.SerializeObject(Devoluciones);

            return Json(json);
        }
        [Authorize]
        public ActionResult getVendiendo()
        {
            var userId = CurrentUser.Get.UserId;
            //lista de Costos Adicionales
            List<SelectListItem> pedidoContado = new List<SelectListItem>();   // Creo una lista
            var listaOperaciones = db.operation.Where(p => p.operationTypeId == 10 && p.userId == userId).ToList();

            List<pedidosViewModel> enviarPedidosContado = new List<pedidosViewModel>();
            foreach (var item in listaOperaciones)
            {
                decimal total = 0;
                total = item.quantity * item.price;
                var productos = new pedidosViewModel()
                {
                    cantidad = item.quantity,
                    codigoBarras = item.products.barcode.ToString(),
                    nombre = item.products.name,
                    unidad = item.price,
                    total = total,
                    operatioId = item.id
                };

                enviarPedidosContado.Add(productos);
            }

            string json = JsonConvert.SerializeObject(enviarPedidosContado);

            return Json(json);
        }

        [Authorize]
        public ActionResult getAbasteciendo()
        {
            var userId = CurrentUser.Get.UserId;
            //lista de Costos Adicionales
            List<SelectListItem> pedidoContado = new List<SelectListItem>();   // Creo una lista
            var listaOperaciones = db.operation.Where(p => p.operationTypeId == 12 && p.userId == userId).ToList();

            decimal SubTotal = 0;
            decimal Iva = 0;
            decimal total = 0;
            List<pedidosViewModel> enviarPedidosContado = new List<pedidosViewModel>();
            foreach (var item in listaOperaciones)
            {
                SubTotal = item.price * item.quantity;
                Iva = (SubTotal - item.discount) * Decimal.Divide(item.products.ivaFK.value, 100);
                total = SubTotal+Iva-item.discount;
                

                var productos = new pedidosViewModel()
                {
                    cantidad = item.quantity,
                    codigoBarras = item.products.barcode.ToString(),
                    nombre = item.products.name,
                    unidad = item.price,
                    subtotal = SubTotal,
                    descuento = item.discount,
                    iva = Math.Round(Iva, 0, MidpointRounding.ToEven).ToString(),
                    total = Math.Round(total, 0, MidpointRounding.ToEven),
                    operatioId = item.id
                };

                enviarPedidosContado.Add(productos);
            }

            string json = JsonConvert.SerializeObject(enviarPedidosContado);

            return Json(json);
        }

        [Authorize]
        public ActionResult getVenderCredito()
        {
            var userId = CurrentUser.Get.UserId;
            //lista de Costos Adicionales
            List<SelectListItem> pedidoContado = new List<SelectListItem>();   // Creo una lista
            var listaOperaciones = db.operation.Where(p => p.operationTypeId == 11 && p.userId == userId).ToList();

            List<pedidosViewModel> enviarPedidosContado = new List<pedidosViewModel>();
            foreach (var item in listaOperaciones)
            {
                decimal total = 0;
                total = item.quantity * item.price;
                var productos = new pedidosViewModel()
                {
                    cantidad = item.quantity,
                    codigoBarras = item.products.barcode.ToString(),
                    nombre = item.products.name,
                    unidad = item.price,
                    total = total,
                    operatioId = item.id
                };

                enviarPedidosContado.Add(productos);
            }

            string json = JsonConvert.SerializeObject(enviarPedidosContado);

            return Json(json);
        }

        [Authorize]
        public ActionResult getPedidoCredito()
        {
            var userId = CurrentUser.Get.UserId;
            //lista de Costos Adicionales
            List<SelectListItem> pedidoContado = new List<SelectListItem>();   // Creo una lista
            var listaOperaciones = db.operation.Where(p => p.operationTypeId == 9 && p.userId == userId).ToList();

            List<pedidosViewModel> enviarPedidosContado = new List<pedidosViewModel>();
            foreach (var item in listaOperaciones)
            {
                decimal total = 0;
                total = item.quantity * item.price;
                var productos = new pedidosViewModel()
                {
                    cantidad = item.quantity,
                    codigoBarras = item.products.barcode.ToString(),
                    nombre = item.products.name,
                    unidad = item.price,
                    total = total,
                    operatioId = item.id
                };

                enviarPedidosContado.Add(productos);
            }

            string json = JsonConvert.SerializeObject(enviarPedidosContado);

            return Json(json);
        }

        // GET: Procesos/Procesos/Details/5
        [Authorize]
        public ActionResult Details(int id)
        {
            return View();
        }

        [Authorize]
        public ActionResult informeDeProductosExcentos()
        {
            var operations = (from pc in db.operation where (pc.products.ivaId == 3 && (pc.operationTypeId == 6 || pc.operationTypeId == 15 || pc.operationTypeId == 18)) select pc).ToList();
            List<products> enviarProductos = new List<products>();   // Creo una lista
            foreach (var item in operations)
            {
                var factura = (from pc in db.factura where (pc.id == item.facturaId) select pc).Single();
                var nuevoProducto = new products()
                {
                    name = item.products.name,
                    detalleIva = item.date.ToString(),
                    inventaryMin = item.quantity,
                    priceIn = item.quantity * item.price,
                    barcode = factura.codConsecutivo + factura.numeroFactura.ToString()
                };
                enviarProductos.Add(nuevoProducto);
            }
            return View(enviarProductos);
        }

        [Authorize]
        public ActionResult cierreCaja()
        {
            var movimientos = (from pc in db.movimientos where (pc.cuenta == "11050502") select pc).ToList();
            decimal efectivoCaja = 0;
            var fechaActual = DateTime.Now;

            foreach (var item in movimientos)
            {
                var diaMov = item.fechaCreado.Day;
                var mesMov = item.fechaCreado.Month;
                var anioMov = item.fechaCreado.Year;

                if (diaMov == fechaActual.Day && mesMov == fechaActual.Month && anioMov == fechaActual.Year)
                {
                    efectivoCaja = efectivoCaja + item.debito;
                }
            }
            ViewBag.efectivoCaja = Convert.ToDecimal(efectivoCaja).ToString("#,##");

            return View();
        }

        [Authorize]
        public JsonResult grabarRegistroCashBack(int id, int destino, int periodo)
        {
            var fechaEntrega = DateTime.Now.AddMonths(periodo);
            var porcenaje = 15;

            if(destino == 2)
            {
                porcenaje = 40;
            }
            else
            {
                if(periodo == 6)
                {
                    porcenaje = 15;
                }
                else
                {
                    porcenaje = 30;
                }
            }

            var newCashBack = new cashBackModel()
            {
                terceroId = id,
                fechaInicio = DateTime.Now,
                fechaEntrega = fechaEntrega,
                valorEntregado = 0,
                periodoEnMeses = periodo,
                valorActual = 0,
                porcetaje = porcenaje,
                destino = destino
            };
            db.cashBackAcco.Add(newCashBack);
            db.SaveChanges();
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public JsonResult GetCashBack(int id)
        {
            var registroCashBack = (from pc in db.cashBackAcco where pc.terceroId == id select pc).First();
            return Json(registroCashBack.valorActual, JsonRequestBehavior.AllowGet);       
        }

        [Authorize]
        public JsonResult comprobarCashBack(int id)
        {
            var registroCashBackCount = (from pc in db.cashBackAcco where pc.terceroId == id select pc).Count();
            if(registroCashBackCount > 0)
            {
                var registroCashBack = (from pc in db.cashBackAcco where pc.terceroId == id select pc).First();
                if(registroCashBack.destino == 1)//1 ES SUBSIDIO 2 ES CASHBACK
                {
                    return Json(11, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(12, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                var persona = db.persons.Find(id);

                var getTerceroCount = (from pc in db2.cuposCredito where pc.NIT == persona.nit select pc).Count();
                if (getTerceroCount != 0)
                {
                    var getTercero = (from pc in db2.cuposCredito where pc.NIT == persona.nit select pc).First();
                    return Json(21, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(22, JsonRequestBehavior.AllowGet);
                }
            }
        }

        [Authorize]
        public ActionResult verFacturaCaja(string id)
        {
            var factura = (from pc in db.factura where pc.id == id select pc).Single();
            var Usuario = (from pc in db.usersTabla where pc.id == factura.userId select pc).Single();

            //lista de Costos Adicionales
            List<SelectListItem> pedidoContado = new List<SelectListItem>();   // Creo una lista
            var listaOperaciones = db.operation.Where(p => p.operationTypeId == 15 && p.facturaId == id).ToList();

            List<pedidosViewModel> enviarPedidosContado = new List<pedidosViewModel>();

            decimal valorTotal = 0;
            foreach (var item in listaOperaciones)
            {
                decimal total = 0;
                total = item.quantity * item.price;
                valorTotal = valorTotal + total;
                decimal unidad = 0;
                if (factura.tipo == 1)
                {
                    unidad = item.price;
                }
                else
                {
                    unidad = item.price;
                }


                var productos = new pedidosViewModel()
                {
                    cantidad = item.quantity,
                    codigoBarras = item.products.barcode.ToString(),
                    nombre = item.products.name,
                    unidad = unidad,
                    total = total,
                    operatioId = item.id
                };

                ViewBag.cliente = factura.persons.name;
                ViewBag.total = Convert.ToInt32(factura.total);

                ViewBag.vendedor = Usuario.cedula + " " + Usuario.nombre + " " + Usuario.apellido;
                ViewBag.facturaId = id;
                enviarPedidosContado.Add(productos);
            }

            return View(enviarPedidosContado);
        }

        [Authorize]
        public ActionResult verFacturaCredito(string id)
        {
            var factura = (from pc in db.factura where pc.id == id select pc).Single();
            var Usuario = (from pc in db.usersTabla where pc.id == factura.userId select pc).Single();

            //lista de Costos Adicionales
            List<SelectListItem> pedidoContado = new List<SelectListItem>();   // Creo una lista
            var listaOperaciones = db.operation.Where(p => p.operationTypeId == 18 && p.facturaId == id).ToList();

            List<pedidosViewModel> enviarPedidosContado = new List<pedidosViewModel>();

            decimal valorTotal = 0;
            foreach (var item in listaOperaciones)
            {
                decimal total = 0;
                total = item.quantity * item.price;
                valorTotal = valorTotal + total;
                decimal unidad = 0;
                if (factura.tipo == 1)
                {
                    unidad = item.price;
                }
                else
                {
                    unidad = item.price;
                }


                var productos = new pedidosViewModel()
                {
                    cantidad = item.quantity,
                    codigoBarras = item.products.barcode.ToString(),
                    nombre = item.products.name,
                    unidad = unidad,
                    total = total,
                    operatioId = item.id
                };

                ViewBag.cliente = factura.persons.name;
                ViewBag.total = Convert.ToInt32(factura.total);

                ViewBag.vendedor = Usuario.cedula + " " + Usuario.nombre + " " + Usuario.apellido;
                ViewBag.facturaId = id;
                enviarPedidosContado.Add(productos);
            }

            return View(enviarPedidosContado);
        }

        [Authorize]
        public ActionResult facturasCaja()
        {
            var facturas = db.factura.Where(f => f.operationTypeId == 15).OrderByDescending(x => x.date).ToList();
            return View(facturas);
        }

        [Authorize]
        public ActionResult facturasCreditos()
        {
            return View(db.factura.Where(f => f.operationTypeId == 18).ToList());
        }



        [HttpPost]
        [Authorize]
        public JsonResult terminarFacturaVenderCaja(int cliente, decimal efectivo, string facturacion, string fecha, int IdFormaPago)
        {
            formato.CurrencyGroupSeparator = ".";
            formato.NumberDecimalSeparator = ",";

            string IdFactura = Guid.NewGuid().ToString();
            var userId = CurrentUser.Get.UserId;
            var listaOperaciones = db.operation.Where(p => p.operationTypeId == 14 && p.userId == userId).ToList();
            var InfoPago = db.FormaPagos.Find(IdFormaPago);
            int siConvenio = 0;
            decimal valorDestinado = 0;
            decimal valorCashBack = 0;

            //VARIABLES SI SE ESCOGE FACTURA ELECTRONICA Y LA DIAN LAS APRUEBA
            bool EstadoFE = false;
            byte[] XmlBase64Response=null;
            string NombreDocumento = "";
            string StringCufe = "";
            //

            int request = -10;


            decimal totalCompra = 0;
            decimal valorExcentos = 0;
            decimal valorExcluidos = 0;
            decimal iva5 = 0;
            decimal ivaGaseosas19 = 0;
            decimal ivaGaseosas5 = 0;
            decimal iva19 = 0;
            decimal ventas19 = 0;
            decimal ventas5 = 0;
            decimal ventasGaseosas = 0;
            decimal ventasExcentos = 0;
            decimal ventasExcluidos = 0;
            decimal costo19 = 0;
            decimal costo5 = 0;
            decimal costoGaseosas = 0;
            decimal costoExcentos = 0;
            decimal costoExcluidos = 0;
            decimal inventario19 = 0;
            decimal inventario5 = 0;
            decimal inventarioGaseosas = 0;
            decimal inventarioExcentos = 0;
            decimal inventarioExcluidos = 0;
            decimal inventarioBolsas = 0;
            decimal valorTotalBolsas = 0;
            decimal baseIva19Gaseosas = 0;
            decimal baseIva5Gaseosas = 0;

            //variables para crear el XML de facturación electrónica si la hay
            var listProductos = new List<LineXML>();
            int key = 1;
            

            foreach (var item in listaOperaciones)
            {
                decimal ivaTemp19 = 0;
                decimal ivaTemp5 = 0;
                decimal subtotal = 0;
                totalCompra = totalCompra + item.quantity * item.price;

                //if (item.products.categoriaId == 1)//GASEOSAS
                //{
                //    if(item.products.ivaId == 1)//19%
                //    {
                //        ivaTemp19 = (item.products.priceIn * item.quantity) - (item.products.priceIn * item.quantity / Convert.ToDecimal(1.19));
                //        ivaGaseosas19 = ivaGaseosas19 + (item.products.priceIn * item.quantity) - (item.products.priceIn * item.quantity / Convert.ToDecimal(1.19));
                //        baseIva19Gaseosas = baseIva19Gaseosas + (item.products.priceIn * item.quantity - ivaTemp19);
                //    }
                //    else if(item.products.ivaId == 2)//5%
                //    {
                //        ivaTemp5 = (item.products.priceIn * item.quantity) - (item.products.priceIn * item.quantity / Convert.ToDecimal(1.05));
                //        ivaGaseosas5 = ivaGaseosas5 + (item.products.priceIn * item.quantity) - (item.products.priceIn * item.quantity / Convert.ToDecimal(1.05));
                //        baseIva5Gaseosas = baseIva5Gaseosas + (item.products.priceIn * item.quantity - ivaTemp5);
                //    }
                //    var thisVenta = item.price * item.quantity - ivaTemp19 - ivaTemp5;
                //    if (thisVenta > 0)
                //    {
                //        ventasGaseosas = ventasGaseosas + (item.price * item.quantity - ivaTemp19 - ivaTemp5);
                //    }
                //    costoGaseosas = costoGaseosas + item.products.priceIn * item.quantity;
                //    inventarioGaseosas = costoGaseosas;
                //}
                //if (item.products.categoriaId == 2)// BOLSAS
                //{
                //    valorTotalBolsas = valorTotalBolsas + item.price * item.quantity;
                //    inventarioBolsas = inventarioBolsas + item.products.priceIn * item.quantity;
                //}

                ivaTemp19 = 0;
                ivaTemp5 = 0;

                if (item.products.ivaId == 1)
                {
                    subtotal = item.price * item.quantity / Convert.ToDecimal(1.19);
                    ivaTemp19 = (item.price * item.quantity) - subtotal;
                    iva19 = iva19 + ((item.price * item.quantity) - subtotal);
                    ventas19 = ventas19 + (item.price * item.quantity - ivaTemp19);
                    costo19 = costo19 + item.products.priceIn * item.quantity;
                    inventario19 = costo19;

                    if(facturacion == "ELECTRONICA")
                    {
                        var line = new LineXML()
                        {
                            LineID = key.ToString(),
                            InvoicedQuantity = item.quantity.ToString(),
                            AllowanceChargeID = 1.ToString(),
                            AllowanceChargeReason = "",
                            BaseAmount = Math.Round(subtotal, 2, MidpointRounding.ToEven).ToString().Replace(",", "."),
                            MultiplierFactorNumeric = "0.00",
                            Amount = "0.00",
                            InvoiceLineExtensionAmount = Math.Round(subtotal, 2, MidpointRounding.ToEven).ToString().Replace(",", "."),
                            LineTax = Math.Round(ivaTemp19, 2, MidpointRounding.ToEven).ToString().Replace(",", "."),
                            LineTaxPercentage = "19.00",
                            LineItemName = item.products.name,
                            LineTotal = Math.Round(subtotal, 2, MidpointRounding.ToEven).ToString().Replace(",", "."),
                            LineItemTotal = Math.Round(subtotal + ivaTemp19, 2, MidpointRounding.ToEven).ToString().Replace(",", "."),
                            ChargeIndicator = false
                        };

                        listProductos.Add(line);
                    }
                }
                if (item.products.ivaId == 2)
                {
                    subtotal = item.price * item.quantity / Convert.ToDecimal(1.05);
                    ivaTemp5 = (item.price * item.quantity) - subtotal;
                    iva5 = iva5 + ((item.price * item.quantity) - subtotal);
                    ventas5 = ventas5 + (item.price * item.quantity - ivaTemp5);
                    costo5 = costo5 + item.products.priceIn * item.quantity;
                    inventario5 = costo5;
                    if (facturacion == "ELECTRONICA")
                    {
                        var line = new LineXML()
                        {
                            LineID = key.ToString(),
                            InvoicedQuantity = item.quantity.ToString(),
                            AllowanceChargeID = 1.ToString(),
                            AllowanceChargeReason = "",
                            BaseAmount = Math.Round(subtotal, 2, MidpointRounding.ToEven).ToString().Replace(",", "."),
                            MultiplierFactorNumeric = "0.00",
                            Amount = "0.00",
                            InvoiceLineExtensionAmount = Math.Round(subtotal, 2, MidpointRounding.ToEven).ToString().Replace(",", "."),
                            LineTax = Math.Round(ivaTemp5, 2, MidpointRounding.ToEven).ToString().Replace(",", "."),
                            LineTaxPercentage = "5.00",
                            LineItemName = item.products.name,
                            LineTotal = Math.Round(subtotal, 2, MidpointRounding.ToEven).ToString().Replace(",", "."),
                            LineItemTotal = Math.Round(subtotal + ivaTemp5, 2, MidpointRounding.ToEven).ToString().Replace(",", "."),
                            ChargeIndicator = false
                        };

                        listProductos.Add(line);
                    }
                }
                if (item.products.ivaId == 3)
                {
                    subtotal = item.price * item.quantity;
                    valorExcentos = valorExcentos + item.price * item.quantity;
                    ventasExcentos = ventasExcentos + item.price * item.quantity;
                    costoExcentos = costoExcentos + item.products.priceIn * item.quantity; ;
                    inventarioExcentos = costoExcentos;
                    if (facturacion == "ELECTRONICA")
                    {
                        var line = new LineXML()
                        {
                            LineID = key.ToString(),
                            InvoicedQuantity = item.quantity.ToString(),
                            AllowanceChargeID = 1.ToString(),
                            AllowanceChargeReason = "",
                            BaseAmount = Math.Round(subtotal, 2, MidpointRounding.ToEven).ToString().Replace(",", "."),
                            MultiplierFactorNumeric = "0.00",
                            Amount = "0.00",
                            InvoiceLineExtensionAmount = Math.Round(subtotal, 2, MidpointRounding.ToEven).ToString().Replace(",", "."),
                            LineTax = "0.00",
                            LineTaxPercentage = "0.00",
                            LineItemName = item.products.name,
                            LineTotal = Math.Round(subtotal, 2, MidpointRounding.ToEven).ToString().Replace(",", "."),
                            LineItemTotal = Math.Round(subtotal, 2, MidpointRounding.ToEven).ToString().Replace(",", "."),
                            ChargeIndicator = false
                        };

                        listProductos.Add(line);
                    }
                }
                if (item.products.ivaId == 4)
                {
                    subtotal = item.price * item.quantity;
                    valorExcluidos = valorExcluidos + item.price * item.quantity;
                    ventasExcluidos = ventasExcluidos + item.price * item.quantity;
                    costoExcluidos = costoExcluidos + item.products.priceIn * item.quantity; ;
                    inventarioExcluidos = costoExcluidos;
                    if (facturacion == "ELECTRONICA")
                    {
                        var line = new LineXML()
                        {
                            LineID = key.ToString(),
                            InvoicedQuantity = item.quantity.ToString(),
                            AllowanceChargeID = 1.ToString(),
                            AllowanceChargeReason = "",
                            BaseAmount = Math.Round(subtotal, 2, MidpointRounding.ToEven).ToString().Replace(",", "."),
                            MultiplierFactorNumeric = "0.00",
                            Amount = "0.00",
                            InvoiceLineExtensionAmount = Math.Round(subtotal, 2, MidpointRounding.ToEven).ToString().Replace(",", "."),
                            LineTax = "0.00",
                            LineTaxPercentage = "0.00",
                            LineItemName = item.products.name,
                            LineTotal = Math.Round(subtotal, 2, MidpointRounding.ToEven).ToString().Replace(",", "."),
                            LineItemTotal = Math.Round(subtotal, 2, MidpointRounding.ToEven).ToString().Replace(",", "."),
                            ChargeIndicator = false
                        };

                        listProductos.Add(line);
                    }
                }
                key++;
            }//FIN foreach

            decimal totalSinIva = totalCompra - iva19 - iva5 - ivaGaseosas19 - ivaGaseosas5;

            
            if(fecha=="")
            {
                fecha = Time.FechaLocal.GetFechaColombia().ToString();
            }
            DateTime auxFecha = Convert.ToDateTime(fecha);
            consecutivosFacturas consecutivoFactura = null;
            if (facturacion == "ELECTRONICA"){ consecutivoFactura = db.consecutivosFacturas.Find(4); } else { consecutivoFactura = db.consecutivosFacturas.Find(1); }

            var Comprobante = (from pc in db.TipoComprobantes where pc.codigo == "CC1" select pc).FirstOrDefault();


            var nuevaFactura = new factura()
            {
                id = IdFactura,
                personId = cliente,
                userId = userId,
                operationTypeId = 15,
                IdFormaPago = IdFormaPago,
                date = Convert.ToDateTime(fecha),
                total = totalCompra,
                tipo = 1,//1 equivale a contado
                fechaPagoCredito = "N",
                cash = efectivo,
                codConsecutivo = consecutivoFactura.cod,
                numeroFactura = consecutivoFactura.actual,
                valorTotalExcentos = ventasExcentos,
                valorTotalExcluidos = ventasExcluidos,
                baseIVA19 = ventas19 + baseIva19Gaseosas,
                baseIVA5 = ventas5 + baseIva5Gaseosas,
                valorIVA19 = iva19 + ivaGaseosas19,
                valorIVA5 = iva5 + ivaGaseosas5,
                totalBolsas = valorTotalBolsas,
                valorConvenio = valorCashBack + valorDestinado,
                facturacion = facturacion,
                IdProveedor = 1,
                PrefijoComprobante = "CC1",
                NumeroComprobante = Comprobante.consecutivo,
                estado = true

            };

            db.factura.Add(nuevaFactura);
            consecutivoFactura.actual = consecutivoFactura.actual + 1;
            db.Entry(consecutivoFactura).State = System.Data.Entity.EntityState.Modified;
            //db.SaveChanges();

            #region FACTURACIÓN ELÉCTRONICA
            if (facturacion == "ELECTRONICA")
            {
                //verificamos que exista la configuración de los parámetros para la facturación electrónica
                var configuracion = db.ParametrosFE.FirstOrDefault();
                if (configuracion != null)
                {
                    var client = db.persons.Where(x => x.id == cliente).FirstOrDefault();//informacion del cliente
                    //calculamos el CUFE (verifcar anexo técnico de la DIAN numeral 11.1.2)
                    var modelCUFE = new ViewModelCUFE()
                    {
                        NumFac = consecutivoFactura.cod + "" + (consecutivoFactura.actual - 1),
                        FecFac = Convert.ToDateTime(fecha).ToString("yyyy-MM-dd"),
                        HoraFac = Convert.ToDateTime(fecha).ToString("HH:mm:ss"),//+"-05:00",
                        ValorBruto = Math.Round(totalSinIva, 2, MidpointRounding.ToEven).ToString().Replace(",", "."), //Método de redondeo half-to-even exigido por la DIAN
                        CodImp1 = "01",
                        ValorImp1 = Math.Round(iva19 + iva5 + ivaGaseosas19 + ivaGaseosas5, 2, MidpointRounding.ToEven).ToString().Replace(",", "."),
                        CodImp2 = "04",
                        ValorImp2 = "0.00",
                        CodImp3 = "03",
                        ValorImp3 = "0.00",
                        ValTot = Math.Round(totalCompra, 2, MidpointRounding.ToEven).ToString().Replace(",", "."),
                        NitFE = configuracion.COMPANY_NIT,
                        NumAdq = client.nit,
                        CITec = configuracion.RANGO_CLAVE_TECNICA,
                        TipoAmbiente = configuracion.TIPO_AMBIENTE.ToString()

                    };
                    var LlaveCUFE = HerramientasFE.getCUFE(modelCUFE);

                    //creamos el código QR
                    var QRCode = "NroFactura=" + modelCUFE.NumFac + "NitFacturador="
                        + modelCUFE.NitFE + "NitAdquiriente="
                        + modelCUFE.NumAdq + "FechaFactura="
                        + modelCUFE.FecFac + "ValorTotalFactura="
                        + modelCUFE.ValTot + "CUFE=" + LlaveCUFE
                        + "URL=" + "https://catalogo‐vpfe‐hab.dian.gov.co/document/searchqr?documentkey=" + LlaveCUFE;

                    //creamos el XML

                    var modelExtension = new ExtensionXML()
                    {
                        InvoiceAuthorization = configuracion.RANGO_NUMERO_RESOLUCION.ToString(),
                        cbcStartDate = configuracion.RANGO_VIGENCIA_DESDE.ToString("yyyy-MM-dd"),
                        cbcEndDate = configuracion.RANGO_VIGENCIA_HASTA.ToString("yyyy-MM-dd"),
                        stsPrefix = configuracion.RANGO_PREFIJO,
                        stsFrom = configuracion.RANGO_DESDE.ToString(),
                        stsTo = configuracion.RANGO_HASTA.ToString(),
                        cacPartyIdentification = configuracion.COMPANY_NIT,
                        stsSoftwareID = configuracion.SOFTWARE_IDENTIFICADOR,
                        stsSoftwareSecurityCode = HerramientasFE.FORMATOSHA384(configuracion.SOFTWARE_IDENTIFICADOR+configuracion.SOFTWARE_PIN+consecutivoFactura.cod+(consecutivoFactura.actual-1)),
                        QRCode = QRCode,
                        DigitalSignature = HerramientasFE.getSignature()

                    };

                    Time.FechaLocal.GetFechaColombia();
                    string horaFact = Time.FechaLocal.GetHoraColombia().ToString();

                    int nDias = DateTime.DaysInMonth(auxFecha.Year, auxFecha.Month);
                    var modelVersion = new VersionXML()
                    {
                        CustomizationID = "10",
                        ProfileExecutionID = configuracion.TIPO_AMBIENTE.ToString(),
                        ID = consecutivoFactura.cod + "" + (consecutivoFactura.actual-1),
                        UUID = LlaveCUFE,
                        IssueDate = Convert.ToDateTime(fecha).ToString("yyyy-MM-dd"),
                        //IssueTime = Convert.ToDateTime(fecha).ToString("HH:mm:ss"),// + "-5:00",
                        IssueTime = horaFact,
                        InvoiceTypeCode = "01",
                        LineCountNumeric = listaOperaciones.Count.ToString(),
                        InvoicePeriodStartDate = new DateTime(auxFecha.Year, auxFecha.Month, 01).ToString("yyyy-MM-dd"),
                        InvoicePeriodEndDate = new DateTime(auxFecha.Year, auxFecha.Month, nDias).ToString("yyyy-MM-dd")
                    };
                    var modelCompany = new CompanyXML()
                    {
                        AdditionalAccountID = "2",
                        IndustriyClasificationCode = "4772",
                        CompanyNIT = configuracion.COMPANY_NIT,
                        CompanyName = configuracion.CompanyName,
                        CompanyPostCode = configuracion.CompanyPostCode,
                        CompanyCity = configuracion.cityFK.Nom_muni.ToUpper(),
                        CompanyCityCode = configuracion.CityCode.ToString(),
                        CompanyDepto = configuracion.cityFK.departamentoFK.Nom_dep.ToUpper(),
                        CompanyDeptoCode = configuracion.cityFK.departamentoFK.codigo,
                        CompanyAddress = configuracion.Direccion,
                        TaxLevelCode = configuracion.ResponsabilidadFiscalFK.codigo,
                        CityCode = configuracion.cityFK.codigo,
                        TaxSchemeId = "01",
                        TaxSchemeName = "IVA",
                        CompanyEmail = configuracion.EMISOR_CORREO_ELECTRONICO,
                        CorporateRegistrationId = configuracion.RANGO_PREFIJO,
                        CorporateRegistrationName = configuracion.EMISOR_MATRICULA_MERCANTIL,
                    };
                    //string idCliente = cliente.ToString();
                    
                    var modelCustomer = new CustomerXML()
                    {
                        AdditionalAccountID = "2",
                        CustomerName = client.name,
                        CustomerCity = client.municipioFK.Nom_muni,
                        CustomerCityCode = client.municipioFK.codigo,
                        CustomerDepto = client.municipioFK.departamentoFK.Nom_dep,
                        CustomerDeptoCode = client.municipioFK.departamentoFK.codigo,
                        CustomerCountryIdentificationCode = client.municipioFK.departamentoFK.paisFK.Nomenclatura,
                        CustomerCountryName = client.municipioFK.departamentoFK.paisFK.Nom_pais,
                        CustomerAddress = client.direccion,
                        CustomerNit = client.nit,
                        CustomerIdCode = client.TipoIdentificacionFK.codigo,
                        CustomerResponsabilidadFiscal = client.responsabilidadFiscal
                    };
                    var modelTotal19 = new TaxSubTotal()
                    {
                        TaxableAmount = Math.Round(ventas19, 2, MidpointRounding.ToEven).ToString().Replace(",", "."),
                        TaxAmount = Math.Round(iva19, 2, MidpointRounding.ToEven).ToString().Replace(",", "."),
                        Percent = "19.00"
                    };
                    var modelTotal5 = new TaxSubTotal()
                    {
                        TaxableAmount = Math.Round(ventas5, 2, MidpointRounding.ToEven).ToString().Replace(",", "."),
                        TaxAmount = Math.Round(iva5, 2, MidpointRounding.ToEven).ToString().Replace(",", "."),
                        Percent = "5.00"
                    };
                    var modelTotalExcluidos = new TaxSubTotal()
                    {
                        TaxableAmount = Math.Round(ventasExcluidos, 2, MidpointRounding.ToEven).ToString().Replace(",", "."),
                        TaxAmount = "0.00",
                        Percent = "0.00"
                    }; var modelTotalExcentos = new TaxSubTotal()
                    {
                        TaxableAmount = Math.Round(ventasExcentos, 2, MidpointRounding.ToEven).ToString().Replace(",", "."),
                        TaxAmount = "0.00",
                        Percent = "0.00"
                    };
                    var ListSubtotal = new List<TaxSubTotal>();
                    if (ventas19 > 0) { ListSubtotal.Add(modelTotal19); }
                    if (ventas5 > 0) { ListSubtotal.Add(modelTotal5); }
                    if (ventasExcluidos > 0) { ListSubtotal.Add(modelTotalExcluidos); }
                    if (ventasExcentos > 0) { ListSubtotal.Add(modelTotalExcentos); }
                    var modelTotal = new TotalXML()
                    {
                        PaymentMeansID = "1",
                        PaymentMeansCode = "10",
                        TaxAmount = Math.Round(iva19 + iva5, 2, MidpointRounding.ToEven).ToString().Replace(",", "."),
                        TaxSubTotal = ListSubtotal,
                        LineExtensionAmount = Math.Round(totalSinIva, 2, MidpointRounding.ToEven).ToString().Replace(",", "."),
                        AllowanceTotalAmount = "0.00",
                        TaxExclusiveAmount = Math.Round(totalSinIva, 2, MidpointRounding.ToEven).ToString().Replace(",", "."),
                        TaxInclusiveAmount = Math.Round(totalCompra, 2, MidpointRounding.ToEven).ToString().Replace(",", "."),
                        PayableAmount = Math.Round(totalCompra, 2, MidpointRounding.ToEven).ToString().Replace(",", "."),
                    };
                    string headXML = HerramientasFE.formHeadXML();
                    string extensionXML = HerramientasFE.formExtensionXML(modelExtension);
                    string versionXML = HerramientasFE.formVersionXML(modelVersion);
                    string companyXML = HerramientasFE.formCompanyXML(modelCompany);
                    string customerXML = HerramientasFE.formCustomerXMl(modelCustomer);
                    string TotalXML = HerramientasFE.formTotalXML(modelTotal);
                    string LineXML = HerramientasFE.formLinesXML(listProductos);

                    var XML = headXML+ extensionXML + versionXML + companyXML + customerXML + TotalXML + LineXML;

                    //validamos el XML con el xsd proporcionado por la DIAN llamado "UBL-Invoice-2.1.xsd"
                    string validarXML = HerramientasFE.ValidateXML(XML);
                    if(validarXML=="")
                    {
                        
                        XmlDocument xml = new XmlDocument();
                        xml.LoadXml(XML);
                        var XmlInvoice = HerramientasFE.ToXmlString(xml);

                        var firmaElectronica = new FirmaElectronica
                        {
                            RolFirmante = RolFirmante.EMISOR,
                            RutaCertificado = RUTA_CERTIFICADO,
                            ClaveCertificado = CLAVE_CERTIFICADO
                        };

                        var fechaa = DateTimeHelper.GetColombianDate();

                        var signedBytes = firmaElectronica.FirmarFactura(XmlInvoice, fechaa);

                        string NombreArchivo = consecutivoFactura.cod+(consecutivoFactura.actual-1)+DateTime.Now.ToString("yyyyMMddhhmmss");
                        //var signedBytes = HerramientasFE.GetXmlByte(xml,NombreArchivo);
                        var clienteDian = new ClienteServicioDian
                        {
                            Ambiente = AMBIENTE_SERVICIO,
                            RutaCertificado = RUTA_CERTIFICADO,
                            ClaveCertificado = CLAVE_CERTIFICADO
                        };
                        request = -1;
                        DianResponse dianResponse = null;
                        UploadDocumentResponse uploadResponse = null;

                        try
                        {
                            if (AMBIENTE_PRUEBAS && USAR_SET_PUEBAS)
                            {
                                var documentos = new List<byte[]>() { signedBytes };

                                uploadResponse = clienteDian.EnviarSetPruebas(documentos, IDENTIFICADOR_SET_PRUEBAS);
                                

                                //Console.WriteLine("ZipKey: {0}", uploadResponse.ZipKey);

                                //if (uploadResponse.ErrorMessageList != null)
                                //{
                                //    foreach (var itemList in uploadResponse.ErrorMessageList)
                                //    {
                                //        Console.WriteLine("[{0}] {1}", itemList.DocumentKey, itemList.ProcessedMessage);
                                //    }
                                //}


                              

                                dianResponse = clienteDian.ConsultarDocumentos(uploadResponse.ZipKey)[0];

                                //Guardamos el registro de la petición al web service de la DIAN
                                string Registro = "FACTURA_VENTA" + auxFecha.ToString("dd/MM/yyyy HH:mm:ss") + modelVersion.ID + modelVersion.UUID + " : " + dianResponse.StatusCode + " : " + dianResponse.StatusMessage;
                                var RegistroFE = new ConsultasController().GetModelRegistroFE(Registro, dianResponse.ErrorMessage);
                                db.RegistrosFE.Add(RegistroFE);


                                var XMLBase64 = dianResponse.XmlBase64Bytes;
                                if (dianResponse.StatusCode == RespuestaDian.PROCESADO_CORRECTAMENTE || dianResponse.StatusCode == RespuestaDian.VALIDACION_EXITOSA)
                                {
                                    if (XMLBase64 != null)//cambiar a != el == sólo es para hacer pruebas de envío del zip al cliente
                                    {
                                        XmlBase64Response = XMLBase64;
                                    }
                                    else
                                    {
                                        XmlBase64Response = signedBytes;
                                    }

                                    EstadoFE = true;
                                    NombreDocumento = NombreArchivo;
                                    StringCufe = LlaveCUFE;
                                    var GenerarFacturaPDF = GetFEpdf(cliente, nuevaFactura, LlaveCUFE, QRCode, listaOperaciones, NombreDocumento);
                                }
                                else
                                {
                                 
                                    request = -2;
                                }



                            }
                            else if (AMBIENTE_PRUEBAS)
                            {
                                //Ambientes de pruebas 
                                dianResponse = clienteDian.EnviarDocumento(signedBytes);
                                Thread.Sleep(2000);

                                Console.WriteLine("{0} - {1} - {2}", dianResponse.StatusCode, dianResponse.StatusMessage, dianResponse.StatusDescription);
                                if (dianResponse.StatusCode != RespuestaDian.PROCESADO_CORRECTAMENTE)
                                {
                                    if (dianResponse.ErrorMessage != null)
                                    {

                                       /* try
                                        {
                                            string errorMessageConcatenado = string.Join(";", dianResponse.ErrorMessage);
                                            GuardarLogFacturacionElectronicaAsync(NombreArchivo, cliente, dianResponse.StatusCode, dianResponse.StatusMessage + "--" + errorMessageConcatenado, dianResponse.XmlDocumentKey);
                                        }
                                        catch (Exception ex)
                                        {
                                            EscribirEnArchivo("1574" + ex.Message);
                                            Console.WriteLine("Error al llamar a GuardarLogFacturacionElectronicaAsync: " + ex.Message);
                                        }*/
                                        return Json(dianResponse.ErrorMessage, JsonRequestBehavior.AllowGet);
                                    }

                                }
                                else
                                {
                                    var appResponse = (dianResponse.XmlBase64Bytes == null) ? "null" : Encoding.UTF8.GetString(dianResponse.XmlBase64Bytes);
                                    /*try
                                    {
                                        GuardarLogFacturacionElectronicaAsync(NombreArchivo, cliente, dianResponse.StatusCode, dianResponse.StatusMessage, dianResponse.XmlDocumentKey);
                                    }
                                    catch (Exception ex)
                                    {
                                        EscribirEnArchivo("1590" + ex.Message);
                                        Console.WriteLine("Error al llamar a GuardarLogFacturacionElectronicaAsync: " + ex.Message);
                                    }*/

                                    EstadoFE = true;
                                    NombreDocumento = NombreArchivo;
                                    StringCufe = LlaveCUFE;
                                    var GenerarFacturaPDF = GetFEpdf(cliente, nuevaFactura, LlaveCUFE, QRCode, listaOperaciones, NombreDocumento);
                                }
                            }
                            else
                            {
                                //Ambiemte de produccion.
                                dianResponse = clienteDian.EnviarDocumento(signedBytes);
                                //Guardamos el registro de la petición al web service de la DIAN
                                string Registro = "FACTURA_VENTA" + auxFecha.ToString("dd/MM/yyyy HH:mm:ss") + modelVersion.ID + modelVersion.UUID + " : " + dianResponse.StatusCode + " : " + dianResponse.StatusMessage;
                                var RegistroFE = new ConsultasController().GetModelRegistroFE(Registro, dianResponse.ErrorMessage);
                                db.RegistrosFE.Add(RegistroFE);
                                var XMLBase64 = dianResponse.XmlBase64Bytes;
                                if (dianResponse.StatusCode == RespuestaDian.PROCESADO_CORRECTAMENTE || dianResponse.StatusCode == RespuestaDian.VALIDACION_EXITOSA)
                                {
                                    if (XMLBase64 != null)//cambiar a != el == sólo es para hacer pruebas de envío del zip al cliente
                                    {
                                        XmlBase64Response = XMLBase64;
                                    }
                                    else
                                    {
                                        XmlBase64Response = signedBytes;
                                    }

                                   /* try
                                    {
                                        string errorMessageConcatenado = string.Join(";", dianResponse.ErrorMessage);
                                        GuardarLogFacturacionElectronicaAsync(NombreArchivo, cliente, dianResponse.StatusCode, dianResponse.StatusMessage + "--" + errorMessageConcatenado, dianResponse.XmlDocumentKey);
                                    }
                                    catch (Exception ex)
                                    {
                                        EscribirEnArchivo("1631" + ex.Message);
                                        Console.WriteLine("Error al llamar a GuardarLogFacturacionElectronicaAsync: " + ex.Message);
                                    }*/


                                    EstadoFE = true;
                                    NombreDocumento = NombreArchivo;
                                    StringCufe = LlaveCUFE;
                                    var GenerarFacturaPDF = GetFEpdf(cliente, nuevaFactura, LlaveCUFE, QRCode, listaOperaciones, NombreDocumento);

                                }
                                else
                                {

                                   /* try
                                    {
                                        string errorMessageConcatenado = string.Join(";", dianResponse.ErrorMessage);
                                        GuardarLogFacturacionElectronicaAsync(NombreArchivo, cliente, dianResponse.StatusCode, dianResponse.StatusMessage + "--" + errorMessageConcatenado, dianResponse.XmlDocumentKey);
                                    }
                                    catch (Exception ex)
                                    {

                                        Console.WriteLine("Error al llamar a GuardarLogFacturacionElectronicaAsync: " + ex.Message);
                                    }*/
                                    return Json(dianResponse.ErrorMessage, JsonRequestBehavior.AllowGet);

                                }


                            }
                        }
                        catch (Exception ex)
                        {
                            //Console.WriteLine(ex.Message);
                            request = 0;
                            //return Json(request, JsonRequestBehavior.AllowGet);
                            return Json("DIAN: error al procesar la factura electronica " + ex.Message, JsonRequestBehavior.AllowGet);
                        }

                    }
                    
                }
            }


            #endregion
         
            
            var fechaActual = DateTime.Now.AddHours(2);

            #region MOVIMIENTOS Y COMPROBANTE


            //COMPROBANTE
            var ComprobanteNew = new comprobante()
            {
                tipoComprobante = "CC1",
                numero = Comprobante.consecutivo,
                centroCostoId = 3,
                detalle = "Venta Caja",
                terceroId = cliente,
                valorTotal = totalCompra+costo19+costo5+costoExcentos+costoExcluidos,
                anio = fechaActual.Year,
                mes = fechaActual.Month,
                dia = fechaActual.Day,
                fechaCreacion = Convert.ToDateTime(fecha),
                usuarioId = userId,
                documento = (nuevaFactura.id).ToString(),
                formaPagoId = 1,
                estado = true
            };
            db.comprobantes.Add(ComprobanteNew);

            //MOVIMIENTOS CUENTAS
            

            var caja = new movimientos()
            {
                tipoComprobante = "CC1",
                numero = Comprobante.consecutivo,
                cuenta = InfoPago.CuentaVentaFK.codigo,
                terceroId = cliente,
                detalle = "Venta Caja",
                debito = totalCompra,
                credito = 0,
                baseMov = 0,
                centroCostoId = 3,
                fechaCreado = Convert.ToDateTime(fecha),
                documento = (nuevaFactura.id).ToString()
            };
            db.movimientos.Add(caja);

            if (iva19 > 0)
            {
                var MovIva19 = new movimientos()
                {
                    tipoComprobante = "CC1",
                    numero = Comprobante.consecutivo,
                    cuenta = "24080501",
                    terceroId = cliente,
                    detalle = "Venta Caja",
                    debito = 0,
                    credito = iva19,
                    baseMov = 0,
                    centroCostoId = 3,
                    fechaCreado = Convert.ToDateTime(fecha),
                    documento = (nuevaFactura.id).ToString()
                };
                db.movimientos.Add(MovIva19);
            }

            if (iva5 > 0)
            {
                var MovIva5 = new movimientos()
                {
                    tipoComprobante = "CC1",
                    numero = Comprobante.consecutivo,
                    cuenta = "24080502",
                    terceroId = cliente,
                    detalle = "Venta Caja",
                    debito = 0,
                    credito = iva5,
                    baseMov = 0,
                    centroCostoId = 3,
                    fechaCreado = Convert.ToDateTime(fecha),
                    documento = (nuevaFactura.id).ToString()
                };
                db.movimientos.Add(MovIva5);
            }

            if (ivaGaseosas19 > 0 || ivaGaseosas5 > 0)
            {
                var MovIvaGaseosas = new movimientos()
                {
                    tipoComprobante = "CC1",
                    numero = Comprobante.consecutivo,
                    cuenta = "24080503",
                    terceroId = cliente,
                    detalle = "Venta Caja",
                    debito = 0,
                    credito = ivaGaseosas19 + ivaGaseosas5,
                    baseMov = 0,
                    centroCostoId = 3,
                    fechaCreado = Convert.ToDateTime(fecha),
                    documento = (nuevaFactura.id).ToString()
                };
                db.movimientos.Add(MovIvaGaseosas);
            }

            if(ventas19 > 0)
            {
                var MovVentas19 = new movimientos()
                {
                    tipoComprobante = "CC1",
                    numero = Comprobante.consecutivo,
                    cuenta = "41350501",
                    terceroId = cliente,
                    detalle = "Venta Caja",
                    debito = 0,
                    credito = ventas19,
                    baseMov = 0,
                    centroCostoId = 3,
                    fechaCreado = Convert.ToDateTime(fecha),
                    documento = (nuevaFactura.id).ToString()
                };
                db.movimientos.Add(MovVentas19);
            }

            if (ventasGaseosas > 0)
            {
                var MovVentasGaseosas = new movimientos()
                {
                    tipoComprobante = "CC1",
                    numero = Comprobante.consecutivo,
                    cuenta = "41350502",
                    terceroId = cliente,
                    detalle = "Venta Caja",
                    debito = 0,
                    credito = ventasGaseosas,
                    baseMov = 0,
                    centroCostoId = 3,
                    fechaCreado = Convert.ToDateTime(fecha),
                    documento = (nuevaFactura.id).ToString()
                };
                db.movimientos.Add(MovVentasGaseosas);
            }

            if (ventas5 > 0)
            {
                var MovVentas5 = new movimientos()
                {
                    tipoComprobante = "CC1",
                    numero = Comprobante.consecutivo,
                    cuenta = "41350503",
                    terceroId = cliente,
                    detalle = "Venta Caja",
                    debito = 0,
                    credito = ventas5,
                    baseMov = 0,
                    centroCostoId = 3,
                    fechaCreado = Convert.ToDateTime(fecha),
                    documento = (nuevaFactura.id).ToString()
                };
                db.movimientos.Add(MovVentas5);
            }

            if (ventasExcentos > 0)
            {
                var MovVentasExcentos = new movimientos()
                {
                    tipoComprobante = "CC1",
                    numero = Comprobante.consecutivo,
                    cuenta = "41350504",
                    terceroId = cliente,
                    detalle = "Venta Caja",
                    debito = 0,
                    credito = ventasExcentos,
                    baseMov = 0,
                    centroCostoId = 3,
                    fechaCreado = Convert.ToDateTime(fecha),
                    documento = (nuevaFactura.id).ToString()
                };
                db.movimientos.Add(MovVentasExcentos);
            }

            if (ventasExcluidos > 0)
            {
                var MovVentasExcluidos = new movimientos()
                {
                    tipoComprobante = "CC1",
                    numero = Comprobante.consecutivo,
                    cuenta = "41350505",
                    terceroId = cliente,
                    detalle = "Venta Caja",
                    debito = 0,
                    credito = ventasExcluidos,
                    baseMov = 0,
                    centroCostoId = 3,
                    fechaCreado = Convert.ToDateTime(fecha),
                    documento = (nuevaFactura.id).ToString()
                };
                db.movimientos.Add(MovVentasExcluidos);
            }

            if (costo19 > 0)
            {
                var MovCosto19 = new movimientos()
                {
                    tipoComprobante = "CC1",
                    numero = Comprobante.consecutivo,
                    cuenta = "61350501",
                    terceroId = cliente,
                    detalle = "Venta Caja",
                    debito = costo19,
                    credito = 0,
                    baseMov = 0,
                    centroCostoId = 3,
                    fechaCreado = Convert.ToDateTime(fecha),
                    documento = (nuevaFactura.id).ToString()
                };
                db.movimientos.Add(MovCosto19);
            }

            if (costoGaseosas > 0)
            {
                var MovCostoGaseosas = new movimientos()
                {
                    tipoComprobante = "CC1",
                    numero = Comprobante.consecutivo,
                    cuenta = "61350502",
                    terceroId = cliente,
                    detalle = "Venta Caja",
                    debito = costoGaseosas,
                    credito = 0,
                    baseMov = 0,
                    centroCostoId = 3,
                    fechaCreado = Convert.ToDateTime(fecha),
                    documento = (nuevaFactura.id).ToString()
                };
                db.movimientos.Add(MovCostoGaseosas);
            }

            if (costo5 > 0)
            {
                var MovCosto5 = new movimientos()
                {
                    tipoComprobante = "CC1",
                    numero = Comprobante.consecutivo,
                    cuenta = "61350503",
                    terceroId = cliente,
                    detalle = "Venta Caja",
                    debito = costo5,
                    credito = 0,
                    baseMov = 0,
                    centroCostoId = 3,
                    fechaCreado = Convert.ToDateTime(fecha),
                    documento = (nuevaFactura.id).ToString()
                };
                db.movimientos.Add(MovCosto5);
            }

            if (costoExcentos > 0)
            {
                var MovCostoExcentos = new movimientos()
                {
                    tipoComprobante = "CC1",
                    numero = Comprobante.consecutivo,
                    cuenta = "61350504",
                    terceroId = cliente,
                    detalle = "Venta Caja",
                    debito = costoExcentos,
                    credito = 0,
                    baseMov = 0,
                    centroCostoId = 3,
                    fechaCreado = Convert.ToDateTime(fecha),
                    documento = (nuevaFactura.id).ToString()
                };
                db.movimientos.Add(MovCostoExcentos);
            }

            if (costoExcluidos > 0)
            {
                var MovCostoExcluidos = new movimientos()
                {
                    tipoComprobante = "CC1",
                    numero = Comprobante.consecutivo,
                    cuenta = "61350505",
                    terceroId = cliente,
                    detalle = "Venta Caja",
                    debito = costoExcluidos,
                    credito = 0,
                    baseMov = 0,
                    centroCostoId = 3,
                    fechaCreado = Convert.ToDateTime(fecha),
                    documento = (nuevaFactura.id).ToString()
                };
                db.movimientos.Add(MovCostoExcluidos);
            }

            if(inventario19 > 0)
            {
                var MovInventarios19 = new movimientos()
                {
                    tipoComprobante = "CC1",
                    numero = Comprobante.consecutivo,
                    cuenta = "14350501",
                    terceroId = cliente,
                    detalle = "Venta Caja",
                    debito = 0,
                    credito = inventario19,
                    baseMov = 0,
                    centroCostoId = 3,
                    fechaCreado = Convert.ToDateTime(fecha),
                    documento = (nuevaFactura.id).ToString()
                };
                db.movimientos.Add(MovInventarios19);
            }

            if (inventarioGaseosas > 0)
            {
                var MovInventariosGaseosas = new movimientos()
                {
                    tipoComprobante = "CC1",
                    numero = Comprobante.consecutivo,
                    cuenta = "14350502",
                    terceroId = cliente,
                    detalle = "Venta Caja",
                    debito = 0,
                    credito = inventarioGaseosas,
                    baseMov = 0,
                    centroCostoId = 3,
                    fechaCreado = Convert.ToDateTime(fecha),
                    documento = (nuevaFactura.id).ToString()
                };
                db.movimientos.Add(MovInventariosGaseosas);
            }

            if (inventario5 > 0)
            {
                var MovInventarios5 = new movimientos()
                {
                    tipoComprobante = "CC1",
                    numero = Comprobante.consecutivo,
                    cuenta = "14350503",
                    terceroId = cliente,
                    detalle = "Venta Caja",
                    debito = 0,
                    credito = inventario5,
                    baseMov = 0,
                    centroCostoId = 3,
                    fechaCreado = Convert.ToDateTime(fecha),
                    documento = (nuevaFactura.id).ToString()
                };
                db.movimientos.Add(MovInventarios5);
            }

            if (inventarioExcentos > 0)
            {
                var MovInventariosExcentos = new movimientos()
                {
                    tipoComprobante = "CC1",
                    numero = Comprobante.consecutivo,
                    cuenta = "14350504",
                    terceroId = cliente,
                    detalle = "Venta Caja",
                    debito = 0,
                    credito = inventarioExcentos,
                    baseMov = 0,
                    centroCostoId = 3,
                    fechaCreado = Convert.ToDateTime(fecha),
                    documento = (nuevaFactura.id).ToString()
                };
                db.movimientos.Add(MovInventariosExcentos);
            }

            if (inventarioExcluidos > 0)
            {
                var MovInventariosExcluidos = new movimientos()
                {
                    tipoComprobante = "CC1",
                    numero = Comprobante.consecutivo,
                    cuenta = "14350505",
                    terceroId = cliente,
                    detalle = "Venta Caja",
                    debito = 0,
                    credito = inventarioExcluidos,
                    baseMov = 0,
                    centroCostoId = 3,
                    fechaCreado = Convert.ToDateTime(fecha),
                    documento = (nuevaFactura.id).ToString()
                };
                db.movimientos.Add(MovInventariosExcluidos);
            }

            if (inventarioBolsas > 0)
            {
                var MovInventarioBolsas = new movimientos()
                {
                    tipoComprobante = "CC1",
                    numero = Comprobante.consecutivo,
                    cuenta = "14350580",                   
                    terceroId = cliente,
                    detalle = "Venta Caja",
                    debito = 0,
                    credito = inventarioBolsas,
                    baseMov = 0,
                    centroCostoId = 3,
                    fechaCreado = Convert.ToDateTime(fecha),
                    documento = (nuevaFactura.id).ToString()
                };
                db.movimientos.Add(MovInventarioBolsas);

                var MovImpuestoBolsas = new movimientos()
                {
                    tipoComprobante = "CC1",
                    numero = Comprobante.consecutivo,
                    cuenta = "529540",
                    terceroId = cliente,
                    detalle = "Venta Caja",
                    debito = inventarioBolsas,
                    credito = 0,
                    baseMov = 0,
                    centroCostoId = 3,
                    fechaCreado = Convert.ToDateTime(fecha),
                    documento = (nuevaFactura.id).ToString()
                };
                db.movimientos.Add(MovImpuestoBolsas);

                var MovImpuestoBolsas2 = new movimientos()
                {
                    tipoComprobante = "CC1",
                    numero = Comprobante.consecutivo,
                    cuenta = "241001",
                    terceroId = cliente,
                    detalle = "Venta Caja",
                    debito = 0,
                    credito = valorTotalBolsas,
                    baseMov = 0,
                    centroCostoId = 3,
                    fechaCreado = Convert.ToDateTime(fecha),
                    documento = (nuevaFactura.id).ToString()
                };
                db.movimientos.Add(MovImpuestoBolsas2);
            }
            #endregion

            var numCompr = Convert.ToInt32(Comprobante.consecutivo);
            Comprobante.consecutivo = (1 + numCompr).ToString();
            db.Entry(Comprobante).State = System.Data.Entity.EntityState.Modified;


            if(facturacion == "ELECTRONICA" && EstadoFE)
            {
                db.SaveChanges();
                foreach (var item in listaOperaciones)
                {
                    var producto = (from pc in db.products where pc.id == item.productId select pc).FirstOrDefault();

                    producto.initialQuantity -= item.quantity;
                    db.Entry(producto).State = System.Data.Entity.EntityState.Modified;

                    item.operationTypeId = 15;
                    item.facturaId = nuevaFactura.id;
                    db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                }

                var SendMail = HerramientasFE.SendCompressUser(cliente, XmlBase64Response, NombreDocumento);//enviamos el correo al cliente con el xml y la factura en pdf

                var InvoiceFE = new ConsultasController().GetModelInvoiceFE(nuevaFactura.codConsecutivo,nuevaFactura.numeroFactura,StringCufe,auxFecha,cliente,SendMail,NombreDocumento,IdFactura);
                db.InvoiceFacturasElectronicas.Add(InvoiceFE);

                db.SaveChanges();
                var respuestaFactura = new
                {
                    numeroFactura = nuevaFactura.numeroFactura, // Número de factura
                    status = true // Estado de la operación
                };

                // Retorna el resultado como JSON
                return Json(respuestaFactura, JsonRequestBehavior.AllowGet);

            }
            else if(facturacion == "POS")
            {
                db.SaveChanges();
                foreach (var item in listaOperaciones)
                {
                    var producto = (from pc in db.products where pc.id == item.productId select pc).FirstOrDefault();

                    producto.initialQuantity -= item.quantity;
                    db.Entry(producto).State = System.Data.Entity.EntityState.Modified;

                    item.operationTypeId = 15;
                    item.facturaId = nuevaFactura.id;
                    db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                }
                db.SaveChanges();
                
                return Json(nuevaFactura.id, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(request, JsonRequestBehavior.AllowGet);
            }

            
        }


        //[HttpPost]
        //[Authorize]
//        public JsonResult terminarFacturaVenderContado(int cliente, int total, DateTime fechaParam, int tipo)
//        {
//            var userId = CurrentUser.Get.UserId;
//            var nuevaFactura = new factura()
//            {
//                personId = cliente,
//                userId = userId,
//                operationTypeId = 16,
//                date = fechaParam,
//                total = total,
//                tipo = tipo,
//                fechaPagoCredito = "N",
//                cash = total
//            };

//            db.factura.Add(nuevaFactura);
//            db.SaveChanges();

//            var listaOperaciones = db.operation.Where(p => p.operationTypeId == 10 && p.userId == userId).ToList();

//            decimal iva5 = 0;
//            decimal iva19 = 0;
//            decimal costoDeVenta = 0;
//            decimal totalPorProducto = 0;
//            decimal ivaEnDecimal = Convert.ToDecimal(0.19);
//            decimal disciminadoPorcentaje = 0;

//            foreach (var item in listaOperaciones)
//            {
//                totalPorProducto = 0;
//;               if (item.products.ivaId == 1)
//                {
//                    totalPorProducto = item.products.priceOut * item.quantity;
//                    disciminadoPorcentaje = totalPorProducto * ivaEnDecimal;
//                    iva19 = iva19 + disciminadoPorcentaje;
//                }
//                else if(item.products.ivaId == 2)
//                {
//                    iva5 = iva5 + ((item.products.priceOut * item.quantity) * (5 / 100));
//                }

//                costoDeVenta = costoDeVenta + (item.products.priceIn * item.quantity);

//                var nuevoPedidoInicial = new pedidoInicial()
//                {
//                    productId = item.productId,
//                    userId = userId,
//                    quantity = item.quantity,
//                    date = fechaParam,
//                    personId = cliente,
//                    facturaId = nuevaFactura.id,
//                    tipo = tipo //1 equivale a contado

//                };
//                item.operationTypeId = 16;
//                item.date = fechaParam;
//                item.facturaId = nuevaFactura.id;
//                db.Entry(item).State = System.Data.Entity.EntityState.Modified;

//                db.pedidoInicial.Add(nuevoPedidoInicial);
//            }

//            var Comprobante = (from pc in db.TipoComprobantes where pc.codigo == "CC2" select pc).Single();
//            var fechaActual = fechaParam;

//            //COMPROBANTE
//            var ComprobanteNew = new comprobante()
//            {
//                tipoComprobante = "CC2",
//                numero = Comprobante.consecutivo,
//                centroCostoId = 3,
//                detalle = "FACTURA ANTERIOR",
//                terceroId = cliente,
//                valorTotal = total,
//                anio = fechaActual.Year,
//                mes = fechaActual.Month,
//                dia = fechaActual.Day,
//                fechaCreacion = fechaActual,
//                usuarioId = userId,
//                documento = (nuevaFactura.id).ToString(),
//                formaPagoId = 1,
//                estado = 1
//            };
//            db.comprobantes.Add(ComprobanteNew);

//            //MOVIMIENTOS CUENTAS           

//            var caja = new movimientos()
//            {
//                tipoComprobante = "CC2",
//                numero = Comprobante.consecutivo,
//                cuenta = "11050502",
//                terceroId = cliente,
//                detalle = "FACTURA ANTERIOR",
//                debito = total,
//                credito = 0,
//                baseMov = 0,
//                centroCostoId = 3,
//                fechaCreado = fechaParam,
//                documento = (nuevaFactura.id).ToString()
//            };
//            db.movimientos.Add(caja);

//            if (iva19 != 0)
//            {
//                var MovIva19 = new movimientos()
//                {
//                    tipoComprobante = "CC2",
//                    numero = Comprobante.consecutivo,
//                    cuenta = "24080501",
//                    terceroId = cliente,
//                    detalle = "FACTURA ANTERIOR",
//                    debito = 0,
//                    credito = iva19,
//                    baseMov = 0,
//                    centroCostoId = 3,
//                    fechaCreado = fechaParam,
//                    documento = (nuevaFactura.id).ToString()
//                };
//                db.movimientos.Add(MovIva19);
//            }

//            if (iva5 != 0)
//            {
//                var MovIva5 = new movimientos()
//                {
//                    tipoComprobante = "CC2",
//                    numero = Comprobante.consecutivo,
//                    cuenta = "24080502",
//                    terceroId = cliente,
//                    detalle = "FACTURA ANTERIOR",
//                    debito = 0,
//                    credito = iva5,
//                    baseMov = 0,
//                    centroCostoId = 3,
//                    fechaCreado = fechaParam,
//                    documento = (nuevaFactura.id).ToString()
//                };
//                db.movimientos.Add(MovIva5);
//            }

//            decimal ventas = total - iva19 - iva5;
//            var MovVentas = new movimientos()
//            {
//                tipoComprobante = "CC2",
//                numero = Comprobante.consecutivo,
//                cuenta = "41350501",
//                terceroId = cliente,
//                detalle = "FACTURA ANTERIOR",
//                debito = 0,
//                credito = ventas,
//                baseMov = 0,
//                centroCostoId = 3,
//                fechaCreado = fechaParam,
//                documento = (nuevaFactura.id).ToString()
//            };
//            db.movimientos.Add(MovVentas);

//            var MovCostoVenta = new movimientos()
//            {
//                tipoComprobante = "CC2",
//                numero = Comprobante.consecutivo,
//                cuenta = "61350501",
//                terceroId = cliente,
//                detalle = "FACTURA ANTERIOR",
//                debito = costoDeVenta,
//                credito = 0,
//                baseMov = 0,
//                centroCostoId = 3,
//                fechaCreado = fechaParam,
//                documento = (nuevaFactura.id).ToString()
//            };
//            db.movimientos.Add(MovCostoVenta);

//            var MovInventarios = new movimientos()
//            {
//                tipoComprobante = "CC2",
//                numero = Comprobante.consecutivo,
//                cuenta = "14350501",
//                terceroId = cliente,
//                detalle = "FACTURA ANTERIOR",
//                debito = 0,
//                credito = costoDeVenta,
//                baseMov = 0,
//                centroCostoId = 3,
//                fechaCreado = fechaParam,
//                documento = (nuevaFactura.id).ToString()
//            };
//            db.movimientos.Add(MovInventarios);

//            var numCompr = Convert.ToInt32(Comprobante.consecutivo);
//            Comprobante.consecutivo = (1 + numCompr).ToString();
//            db.Entry(Comprobante).State = System.Data.Entity.EntityState.Modified;

//            db.SaveChanges();
//            return Json(1, JsonRequestBehavior.AllowGet);
//        }


        [Authorize]
        public ActionResult imprimirFacturaCaja(string id)
        {          
            var factura = (from pc in db.factura where pc.id == id select pc).Single();

            var userId = CurrentUser.Get.UserId;
            List<SelectListItem> pedidoContado = new List<SelectListItem>();   // Creo una lista
            var listaOperaciones = db.operation.Where(p => p.operationTypeId == 15 && p.facturaId == id).ToList();

            List<pedidosViewModel2> enviarPedidosContado = new List<pedidosViewModel2>();

            foreach (var item in listaOperaciones)
            {
                decimal total = 0;
                total = item.quantity * item.price;

                var productos = new pedidosViewModel2()
                {
                    cod = item.products.id,
                    Referencia = item.products.barcode,
                    cantidad = item.quantity,
                    nombre = item.products.name + " " + item.products.detalleIva,
                    unidad = Convert.ToDecimal(item.price).ToString("#,##"),
                    iva = item.products.ivaFK.name,
                    total = Convert.ToDecimal(total).ToString("#,##"),
                };

                enviarPedidosContado.Add(productos);
            }

            ViewBag.identificacion = factura.persons.nit;
            ViewBag.cliente = factura.persons.name;
            ViewBag.direccion = factura.persons.direccion;
            ViewBag.celular = factura.persons.celular;
            ViewBag.correo = factura.persons.email;
            ViewBag.vendedor = factura.usersTabla.nombre + " " + factura.usersTabla.apellido;
            ViewBag.numeroFactura = factura.numeroFactura;
            ViewBag.PrefijoFactura = factura.codConsecutivo;
            ViewBag.fecha = factura.date;
            ViewBag.valorTotalExcentos = Convert.ToDecimal(factura.valorTotalExcentos).ToString("#,##");
            ViewBag.valorTotalExcluidos = Convert.ToDecimal(factura.valorTotalExcluidos).ToString("#,##");
            ViewBag.baseIVA19 = Convert.ToDecimal(factura.baseIVA19).ToString("#,##");
            ViewBag.valorIVA19 = Convert.ToDecimal(factura.valorIVA19).ToString("#,##");
            ViewBag.baseIVA5 = Convert.ToDecimal(factura.baseIVA5).ToString("#,##");
            ViewBag.valorIVA5 = Convert.ToDecimal(factura.valorIVA5).ToString("#,##");
            ViewBag.totalBolsas = Convert.ToDecimal(factura.totalBolsas).ToString("#,##");
            ViewBag.valorConvenio = Convert.ToDecimal(factura.valorConvenio).ToString("#,##");
            ViewBag.total = Convert.ToDecimal(factura.total).ToString("#,##");

            //var registroCashBackCount = (from pc in db.cashBackAcco where pc.terceroId == factura.personId select pc).Count();
            //if(registroCashBackCount > 0)
            //{
            //    var registroCashBack = (from pc in db.cashBackAcco where pc.terceroId == factura.personId select pc).First();
            //    if (registroCashBack.destino == 1)
            //    {
            //        ViewBag.beneficio = 1;
            //    }
            //    else
            //    {
            //        ViewBag.beneficio = 2;
            //    }
            //}
            //else
            //{
            //    ViewBag.beneficio = 2;
            //}
            if(factura.valorConvenio > 0)
            {
                ViewBag.beneficio = 1;
            }
            else
            {
                ViewBag.beneficio = 2;
            }


            return View(enviarPedidosContado);
        }

        [Authorize]
        public ActionResult imprimirFacturaCredito(string id)
        {
            var factura = (from pc in db.factura where pc.id == id select pc).Single();

            var userId = CurrentUser.Get.UserId;
            List<SelectListItem> pedidoContado = new List<SelectListItem>();   // Creo una lista
            var listaOperaciones = db.operation.Where(p => p.operationTypeId == 18 && p.facturaId == id).ToList();

            List<pedidosViewModel2> enviarPedidosContado = new List<pedidosViewModel2>();

            foreach (var item in listaOperaciones)
            {
                decimal total = 0;
                total = item.quantity * item.price;

                var productos = new pedidosViewModel2()
                {
                    cod = item.products.id,
                    cantidad = item.quantity,
                    nombre = item.products.name + " " + item.products.detalleIva,
                    unidad = Convert.ToDecimal(item.price).ToString("#,##"),
                    iva = item.products.ivaFK.name,
                    total = Convert.ToDecimal(total).ToString("#,##"),
                };

                enviarPedidosContado.Add(productos);
            }

            ViewBag.identificacion = factura.persons.nit;
            ViewBag.cliente = factura.persons.name;
            ViewBag.direccion = factura.persons.direccion;
            ViewBag.celular = factura.persons.celular;
            ViewBag.correo = factura.persons.email;
            ViewBag.vendedor = factura.usersTabla.nombre + " " + factura.usersTabla.apellido;
            ViewBag.numeroFactura = factura.numeroFactura;
            ViewBag.fecha = factura.date;
            ViewBag.valorTotalExcentos = Convert.ToDecimal(factura.valorTotalExcentos).ToString("#,##");
            ViewBag.valorTotalExcluidos = Convert.ToDecimal(factura.valorTotalExcluidos).ToString("#,##");
            ViewBag.baseIVA19 = Convert.ToDecimal(factura.baseIVA19).ToString("#,##");
            ViewBag.valorIVA19 = Convert.ToDecimal(factura.valorIVA19).ToString("#,##");
            ViewBag.baseIVA5 = Convert.ToDecimal(factura.baseIVA5).ToString("#,##");
            ViewBag.valorIVA5 = Convert.ToDecimal(factura.valorIVA5).ToString("#,##");
            ViewBag.totalBolsas = Convert.ToDecimal(factura.totalBolsas).ToString("#,##");
            ViewBag.valorConvenio = Convert.ToDecimal(factura.valorConvenio).ToString("#,##");
            ViewBag.total = Convert.ToDecimal(factura.total).ToString("#,##");
            ViewBag.fechaPago = factura.fechaPagoCredito;

            //var registroCashBackCount = (from pc in db.cashBackAcco where pc.terceroId == factura.personId select pc).Count();
            //if(registroCashBackCount > 0)
            //{
            //    var registroCashBack = (from pc in db.cashBackAcco where pc.terceroId == factura.personId select pc).First();
            //    if (registroCashBack.destino == 1)
            //    {
            //        ViewBag.beneficio = 1;
            //    }
            //    else
            //    {
            //        ViewBag.beneficio = 2;
            //    }
            //}
            //else
            //{
            //    ViewBag.beneficio = 2;
            //}
            if (factura.valorConvenio > 0)
            {
                ViewBag.beneficio = 1;
            }
            else
            {
                ViewBag.beneficio = 2;
            }


            return View(enviarPedidosContado);
        }


        [HttpPost]
        [Authorize(Roles = "Admin")]
        public JsonResult terminarAbastecimiento(string facturaFisica, int provider, int IdFormaPago)
        {
            try
            {
                var Fecha = Time.FechaLocal.GetFechaColombia();
                var userId = CurrentUser.Get.UserId;
                var listaOperaciones = db.operation.Where(p => p.operationTypeId == 12 && p.userId == userId).ToList();
                var Comprobante = (from pc in db.TipoComprobantes where pc.codigo == "CC3" select pc).Single();
                string IdFactura = Guid.NewGuid().ToString();//Este será el Id o llave primaria de la factura de compra
                var InfoPago = db.FormaPagos.Find(IdFormaPago);

                //actualizamos primero los productos si se cambió el valor de entrada y el iva
                foreach (var item in listaOperaciones)
                {
                    var producto = (from pc in db.products where pc.id == item.productId select pc).Single();
                    producto.priceIn = item.price;
                    producto.ivaId = (item.IvaId != 0) ? Convert.ToInt32(item.IvaId) : producto.ivaId;
                    db.Entry(producto).State = System.Data.Entity.EntityState.Modified;
                }
                db.SaveChanges();

                decimal totalCompra = 0;
                decimal valorExcentos = 0;
                decimal valorExcluidos = 0;
                decimal iva5 = 0;
                decimal iva19 = 0;
                decimal ComprasExcentos = 0;
                decimal ComprasExcluidos = 0;
                decimal costo19 = 0;
                decimal costo5 = 0;
                decimal costoExcentos = 0;
                decimal costoExcluidos = 0;
                decimal inventario19 = 0;
                decimal inventario5 = 0;

                List<movimientos> Movimientos19 = new List<movimientos>();
                List<movimientos> Movimientos5 = new List<movimientos>();
                List<movimientos> MovimientosExcentos = new List<movimientos>();
                List<movimientos> MovimientosExcluidos = new List<movimientos>();

                foreach (var item in listaOperaciones)
                {
                    decimal ivaTemp19 = 0;
                    decimal ivaTemp5 = 0;
                    decimal subtotal = 0;
                    decimal precioConIva = item.price;

                    //CANTIDAD ACTUAL
                    var productInOperation = (from pc in db.operation where pc.productId == item.productId select pc).ToList();
                    var cantidad = 0;
                    foreach (var operation in productInOperation)
                    {
                        if (operation.operationType.tipo == 1)
                        {
                            cantidad = cantidad + operation.quantity;
                        }
                        else if (operation.operationType.tipo == 2)
                        {
                            cantidad = cantidad - operation.quantity;
                        }
                    }
                    //FIN CANTIDAD ACTUAL

                    item.products.priceIn = (precioConIva * item.quantity + item.products.priceIn * cantidad) / (cantidad + item.quantity);

                    totalCompra += item.price * item.quantity;


                    ivaTemp19 = 0;
                    ivaTemp5 = 0;

                    if (item.products.ivaId == 1)
                    {
                        subtotal = (item.price * item.quantity) - item.discount;
                        ivaTemp19 = subtotal * (Decimal.Divide(item.products.ivaFK.value, 100));
                        iva19 += ivaTemp19;
                        costo19 = costo19 + item.products.priceIn * item.quantity;
                        inventario19 += subtotal;

                        var Mov = new movimientos()
                        {
                            tipoComprobante = "CC3",
                            numero = Comprobante.consecutivo,
                            cuenta = "1435200501",
                            terceroId = provider,
                            detalle = "Producto: " + item.products.name + " Cant: " + item.quantity,
                            debito = Math.Round(item.price * item.quantity, 0, MidpointRounding.ToEven),
                            credito = 0,
                            baseMov = 0,
                            centroCostoId = 3,
                            fechaCreado = Fecha,
                            documento = IdFactura
                        };
                        Movimientos19.Add(Mov);
                    }
                    if (item.products.ivaId == 2)
                    {
                        subtotal = (item.price * item.quantity) - item.discount;
                        ivaTemp5 = subtotal * (Decimal.Divide(item.products.ivaFK.value, 100));
                        iva5 += ivaTemp5;
                        costo5 = costo5 + item.products.priceIn * item.quantity;
                        inventario5 += subtotal;

                        var Mov = new movimientos()
                        {
                            tipoComprobante = "CC3",
                            numero = Comprobante.consecutivo,
                            cuenta = "1435200502",
                            terceroId = provider,
                            detalle = "Producto: " + item.products.name + " Cant: " + item.quantity,
                            debito = Math.Round(item.price * item.quantity, 0, MidpointRounding.ToEven),
                            credito = 0,
                            baseMov = 0,
                            centroCostoId = 3,
                            fechaCreado = Fecha,
                            documento = IdFactura
                        };
                        Movimientos5.Add(Mov);
                    }
                    if (item.products.ivaId == 3)
                    {
                        valorExcentos = (item.price * item.quantity) - item.discount;
                        costoExcentos = costoExcentos + item.products.priceIn * item.quantity; ;
                        ComprasExcentos = valorExcentos;

                        var Mov = new movimientos()
                        {
                            tipoComprobante = "CC3",
                            numero = Comprobante.consecutivo,
                            cuenta = "1435200503",
                            terceroId = provider,
                            detalle = "Producto: " + item.products.name + " Cant: " + item.quantity,
                            debito = Math.Round(item.price * item.quantity, 0, MidpointRounding.ToEven),
                            credito = 0,
                            baseMov = 0,
                            centroCostoId = 3,
                            fechaCreado = Fecha,
                            documento = IdFactura
                        };
                        MovimientosExcentos.Add(Mov);
                    }
                    if (item.products.ivaId == 4)
                    {
                        valorExcluidos = (item.price * item.quantity) - item.discount;
                        costoExcluidos = costoExcluidos + item.products.priceIn * item.quantity; ;
                        ComprasExcluidos = valorExcluidos;

                        var Mov = new movimientos()
                        {
                            tipoComprobante = "CC3",
                            numero = Comprobante.consecutivo,
                            cuenta = "1435200504",
                            terceroId = provider,
                            detalle = "Producto: " + item.products.name + " Cant: " + item.quantity,
                            debito = Math.Round(item.price * item.quantity, 0, MidpointRounding.ToEven),
                            credito = 0,
                            baseMov = 0,
                            centroCostoId = 3,
                            fechaCreado = Fecha,
                            documento = IdFactura
                        };
                        MovimientosExcluidos.Add(Mov);
                    }
                }//FIN foreach

                //Calculamos el descuento total del abastecimiento
                decimal TotalDescuento = listaOperaciones.Select(x => x.discount).Sum();


                var nuevaFactura = new factura()
                {
                    id = IdFactura,
                    personId = 1,
                    userId = userId,
                    operationTypeId = 2,
                    IdFormaPago = IdFormaPago,
                    date = Fecha,
                    totalDiscount = TotalDescuento,
                    total = totalCompra + iva19 + iva5,
                    tipo = 3,//equivale a abastecimiento
                    fechaPagoCredito = "N",
                    cash = 0,
                    valorTotalExcentos = ComprasExcentos,
                    valorTotalExcluidos = ComprasExcluidos,
                    baseIVA19 = inventario19,
                    baseIVA5 = inventario5,
                    valorIVA19 = iva19,
                    valorIVA5 = iva5,
                    totalBolsas = 0,
                    valorConvenio = 0,
                    facturaFisica = facturaFisica,
                    IdProveedor = provider,
                    PrefijoComprobante = "CC3",
                    NumeroComprobante = Comprobante.consecutivo,
                    estado = true

                };

                db.factura.Add(nuevaFactura);

                foreach (var item in listaOperaciones)
                {
                    var producto = (from pc in db.products where pc.id == item.productId select pc).FirstOrDefault();

                    producto.initialQuantity += item.quantity;
                    db.Entry(producto).State = System.Data.Entity.EntityState.Modified;

                    item.operationTypeId = 2;
                    item.facturaId = nuevaFactura.id;
                    db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                }



                //COMPROBANTE
                var ComprobanteNew = new comprobante()
                {
                    tipoComprobante = "CC3",
                    numero = Comprobante.consecutivo,
                    centroCostoId = 3,
                    detalle = "Abastecimiento",
                    terceroId = provider,
                    valorTotal = totalCompra+iva19+iva5,
                    anio = Fecha.Year,
                    mes = Fecha.Month,
                    dia = Fecha.Day,
                    fechaCreacion = Fecha,
                    usuarioId = userId,
                    documento = (nuevaFactura.id).ToString(),
                    formaPagoId = 1,
                    estado = true
                };
                db.comprobantes.Add(ComprobanteNew);

                //MOVIMIENTOS CUENTAS
                var caja = new movimientos()
                {
                    tipoComprobante = "CC3",
                    numero = Comprobante.consecutivo,
                    cuenta = InfoPago.CuentaCompraFK.codigo,
                    terceroId = provider,
                    detalle = "Abastecimiento",
                    debito = 0,
                    credito = totalCompra+iva19+iva5 - TotalDescuento,
                    baseMov = 0,
                    centroCostoId = 3,
                    fechaCreado = Fecha,
                    documento = (nuevaFactura.id).ToString()
                };
                db.movimientos.Add(caja);

                if (iva19 > 0)
                {
                    var MovIva19 = new movimientos()
                    {
                        tipoComprobante = "CC3",
                        numero = Comprobante.consecutivo,
                        cuenta = "2408001",
                        terceroId = provider,
                        detalle = "IVA 19% Base: " + Math.Round(inventario19, 0, MidpointRounding.ToEven),
                        debito = Math.Round(iva19, 0, MidpointRounding.ToEven),
                        credito = 0,
                        baseMov = 0,
                        centroCostoId = 3,
                        fechaCreado = Fecha,
                        documento = (nuevaFactura.id).ToString()
                    };
                    db.movimientos.Add(MovIva19);
                }

                if (iva5 > 0)
                {
                    var MovIva5 = new movimientos()
                    {
                        tipoComprobante = "CC3",
                        numero = Comprobante.consecutivo,
                        cuenta = "24081003",
                        terceroId = provider,
                        detalle = "IVA 5% Base: " + Math.Round(inventario5, 0, MidpointRounding.ToEven),
                        debito = Math.Round(iva5, 0, MidpointRounding.ToEven),
                        credito = 0,
                        baseMov = 0,
                        centroCostoId = 3,
                        fechaCreado = Fecha,
                        documento = (nuevaFactura.id).ToString()
                    };
                    db.movimientos.Add(MovIva5);
                }

                if (ComprasExcentos > 0)
                {
                    var Mov = new movimientos()
                    {
                        tipoComprobante = "CC3",
                        numero = Comprobante.consecutivo,
                        cuenta = "24081004",
                        terceroId = provider,
                        detalle = "IVA 0% Base: " + Math.Round(ComprasExcentos, 0, MidpointRounding.ToEven),
                        debito = 0,
                        credito = 0,
                        baseMov = 0,
                        centroCostoId = 3,
                        fechaCreado = Fecha,
                        documento = (nuevaFactura.id).ToString()
                    };
                    db.movimientos.Add(Mov);
                }
                if (ComprasExcluidos > 0)
                {
                    var Mov = new movimientos()
                    {
                        tipoComprobante = "CC3",
                        numero = Comprobante.consecutivo,
                        cuenta = "24081005",
                        terceroId = provider,
                        detalle = "NO GRAVADOS Base: " + Math.Round(ComprasExcluidos, 0, MidpointRounding.ToEven),
                        debito = 0,
                        credito = 0,
                        baseMov = 0,
                        centroCostoId = 3,
                        fechaCreado = Fecha,
                        documento = (nuevaFactura.id).ToString()
                    };
                    db.movimientos.Add(Mov);
                }

                if (TotalDescuento > 0)
                {
                    var MovDescuento = new movimientos()
                    {
                        tipoComprobante = "CC3",
                        numero = Comprobante.consecutivo,
                        cuenta = "42104001",
                        terceroId = provider,
                        detalle = "Descuento",
                        debito = 0,
                        credito = TotalDescuento,
                        baseMov = 0,
                        centroCostoId = 3,
                        fechaCreado = Fecha,
                        documento = (nuevaFactura.id).ToString()
                    };
                    db.movimientos.Add(MovDescuento);
                }

                if (Movimientos19.Count > 0) { db.movimientos.AddRange(Movimientos19); }
                if (Movimientos5.Count > 0) { db.movimientos.AddRange(Movimientos5); }
                if (MovimientosExcentos.Count > 0) { db.movimientos.AddRange(MovimientosExcentos); }
                if (MovimientosExcluidos.Count > 0) { db.movimientos.AddRange(MovimientosExcluidos); }


                var numCompr = Convert.ToInt32(Comprobante.consecutivo);
                Comprobante.consecutivo = (1 + numCompr).ToString();
                db.Entry(Comprobante).State = System.Data.Entity.EntityState.Modified;

                db.SaveChanges();
                return new JsonResult { Data = new { status = true } };
            }
            catch (Exception ex)
            {
                return new JsonResult { Data = new { status = false } };
                throw;
            }

            
        }

        [HttpPost]
        [Authorize]
        public JsonResult terminarFacturaVenderCredito(int cliente, string fecha)
        {
            var userId = CurrentUser.Get.UserId;
            var listaOperaciones = db.operation.Where(p => p.operationTypeId == 11 && p.userId == userId).ToList();
            int siConvenio = 0;
            decimal valorDestinado = 0;
            decimal valorCashBack = 0;

            decimal totalCompra = 0;
            decimal valorExcentos = 0;
            decimal valorExcluidos = 0;
            decimal iva5 = 0;
            decimal ivaGaseosas19 = 0;
            decimal ivaGaseosas5 = 0;
            decimal iva19 = 0;
            decimal ventas19 = 0;
            decimal ventas5 = 0;
            decimal ventasGaseosas = 0;
            decimal ventasExcentos = 0;
            decimal ventasExcluidos = 0;
            decimal costo19 = 0;
            decimal costo5 = 0;
            decimal costoGaseosas = 0;
            decimal costoExcentos = 0;
            decimal costoExcluidos = 0;
            decimal inventario19 = 0;
            decimal inventario5 = 0;
            decimal inventarioGaseosas = 0;
            decimal inventarioExcentos = 0;
            decimal inventarioExcluidos = 0;
            decimal inventarioBolsas = 0;
            decimal valorTotalBolsas = 0;
            decimal baseIva19Gaseosas = 0;
            decimal baseIva5Gaseosas = 0;

            foreach (var item in listaOperaciones)
            {
                decimal ivaTemp19 = 0;
                decimal ivaTemp5 = 0;
                totalCompra = totalCompra + item.quantity * item.price;

                if (item.products.categoriaId == 1)
                {
                    if (item.products.ivaId == 1)
                    {
                        ivaTemp19 = item.price * item.quantity * Convert.ToDecimal(0.19);
                        ivaGaseosas19 = ivaGaseosas19 + item.price * item.quantity * Convert.ToDecimal(0.19);
                        baseIva19Gaseosas = baseIva19Gaseosas + (item.price * item.quantity - ivaTemp19);
                    }
                    else if (item.products.ivaId == 2)
                    {
                        ivaTemp5 = item.price * item.quantity * Convert.ToDecimal(0.05);
                        ivaGaseosas5 = ivaGaseosas5 + item.price * Convert.ToDecimal(0.05);
                        baseIva5Gaseosas = baseIva5Gaseosas + (item.price * item.quantity - ivaTemp5);
                    }
                    ventasGaseosas = ventasGaseosas + (item.price * item.quantity - ivaTemp19 - ivaTemp5);
                    costoGaseosas = costoGaseosas + item.products.priceIn * item.quantity;
                    inventarioGaseosas = costoGaseosas;
                }
                if (item.products.categoriaId == 2)
                {
                    valorTotalBolsas = valorTotalBolsas + item.price * item.quantity;
                    inventarioBolsas = inventarioBolsas + item.products.priceIn * item.quantity;
                }

                ivaTemp19 = 0;
                ivaTemp5 = 0;

                if (item.products.ivaId == 1 && item.products.categoriaId != 1 && item.products.categoriaId != 2)
                {
                    ivaTemp19 = item.price * item.quantity * Convert.ToDecimal(0.19);
                    iva19 = iva19 + item.price * item.quantity * Convert.ToDecimal(0.19);
                    ventas19 = ventas19 + (item.price * item.quantity - ivaTemp19);
                    costo19 = costo19 + item.products.priceIn * item.quantity;
                    inventario19 = costo19;
                }
                if (item.products.ivaId == 2 && item.products.categoriaId != 1 && item.products.categoriaId != 2)
                {
                    ivaTemp5 = item.price * item.quantity * Convert.ToDecimal(0.05);
                    iva5 = iva5 + item.price * item.quantity * Convert.ToDecimal(0.05);
                    ventas5 = ventas5 + (item.price * item.quantity - ivaTemp5);
                    costo5 = costo5 + item.products.priceIn * item.quantity;
                    inventario5 = costo5;
                }
                if (item.products.ivaId == 3 && item.products.categoriaId != 1 && item.products.categoriaId != 2)
                {
                    valorExcentos = valorExcentos + item.price * item.quantity;
                    ventasExcentos = ventasExcentos + item.price * item.quantity;
                    costoExcentos = costoExcentos + item.products.priceIn * item.quantity; ;
                    inventarioExcentos = costoExcentos;
                }
                if (item.products.ivaId == 4 && item.products.categoriaId != 1 && item.products.categoriaId != 2)
                {
                    valorExcluidos = valorExcluidos + item.price * item.quantity;
                    ventasExcluidos = ventasExcluidos + item.price * item.quantity;
                    costoExcluidos = costoExcluidos + item.products.priceIn * item.quantity; ;
                    inventarioExcluidos = costoExcluidos;
                }
            }//FIN foreach

            decimal totalSinIva = totalCompra - iva19 - iva5 - ivaGaseosas19 - ivaGaseosas5;

            var persona = db.persons.Find(cliente);
            var getTerceroCount = (from pc in db2.cuposCredito where pc.NIT == persona.nit select pc).Count();
            if (getTerceroCount > 0)
            {
                double porcentaje = 0.4;
                valorDestinado = Convert.ToDecimal(totalSinIva) * Convert.ToDecimal(porcentaje);
                var getTercero = (from pc in db2.cuposCredito where pc.NIT == persona.nit select pc).First();
                getTercero.acumulado = getTercero.acumulado + valorDestinado;
                db2.Entry(getTercero).State = System.Data.Entity.EntityState.Modified;
                db2.SaveChanges();
                siConvenio = 1;
            }

            //GUARDAR VALOR CASHBACK

            //if(siConvenio != 1)
            //{
            //    var registroCashBack = (from pc in db.cashBackAcco where pc.terceroId == cliente select pc).First();
            //    double porcentajeCashBack = 0.4;

            //    if (registroCashBack.porcetaje == 15)
            //    {
            //        porcentajeCashBack = 0.15;
            //    }
            //    if (registroCashBack.porcetaje == 30)
            //    {
            //        porcentajeCashBack = 0.3;
            //    }
            //    valorCashBack = Convert.ToDecimal(totalSinIva) * Convert.ToDecimal(porcentajeCashBack);
            //    registroCashBack.valorActual = registroCashBack.valorActual + valorCashBack;
            //    db.Entry(registroCashBack).State = System.Data.Entity.EntityState.Modified;
            //}

            var consecutivoFactura = db.consecutivosFacturas.Find(2);
            var nuevaFactura = new factura()
            {
                personId = cliente,
                userId = userId,
                operationTypeId = 18,
                date = DateTime.Now.AddHours(2),
                total = totalCompra,
                tipo = 2,//1 equivale a contado
                fechaPagoCredito = fecha,
                cash = 0,
                codConsecutivo = consecutivoFactura.cod,
                numeroFactura = (consecutivoFactura.actual-1),
                valorTotalExcentos = ventasExcentos,
                valorTotalExcluidos = ventasExcluidos,
                baseIVA19 = ventas19 + baseIva19Gaseosas,
                baseIVA5 = ventas5 + baseIva5Gaseosas,
                valorIVA19 = iva19 + ivaGaseosas19,
                valorIVA5 = iva5 + ivaGaseosas5,
                totalBolsas = valorTotalBolsas,
                valorConvenio = valorCashBack + valorDestinado
            };
            db.factura.Add(nuevaFactura);
            consecutivoFactura.actual = consecutivoFactura.actual + 1;
            db.Entry(consecutivoFactura).State = System.Data.Entity.EntityState.Modified;//PARA QUE SIRVE ESTA LINEA?
            db.SaveChanges();

            foreach (var item in listaOperaciones)
            {
                item.operationTypeId = 18;
                item.facturaId = nuevaFactura.id;
                db.Entry(item).State = System.Data.Entity.EntityState.Modified;
            }

            var Comprobante = (from pc in db.TipoComprobantes where pc.codigo == "CC1" select pc).Single();
            var fechaActual = DateTime.Now.AddHours(2);

            //COMPROBANTE
            var ComprobanteNew = new comprobante()
            {
                tipoComprobante = "CC4",
                numero = Comprobante.consecutivo,
                centroCostoId = 3,
                detalle = "Venta Credito",
                terceroId = cliente,
                valorTotal = totalCompra,
                anio = fechaActual.Year,
                mes = fechaActual.Month,
                dia = fechaActual.Day,
                fechaCreacion = fechaActual,
                usuarioId = userId,
                documento = (nuevaFactura.id).ToString(),
                formaPagoId = 1,
                estado = true
            };
            db.comprobantes.Add(ComprobanteNew);

            //MOVIMIENTOS CUENTAS
            if (siConvenio == 1)
            {
                var convenioMov = new movimientos()
                {
                    tipoComprobante = "CC4",
                    numero = Comprobante.consecutivo,
                    cuenta = "26480501",
                    terceroId = cliente,
                    detalle = "Convenio Coomisol",
                    debito = valorDestinado,
                    credito = 0,
                    baseMov = 0,
                    centroCostoId = 3,
                    fechaCreado = fechaActual,
                    documento = (nuevaFactura.id).ToString()
                };
                db.movimientos.Add(convenioMov);

                var convenioMov2 = new movimientos()
                {
                    tipoComprobante = "CC4",
                    numero = Comprobante.consecutivo,
                    cuenta = "26080501",
                    terceroId = cliente,
                    detalle = "Convenio Coomisol",
                    debito = 0,
                    credito = valorDestinado,
                    baseMov = 0,
                    centroCostoId = 3,
                    fechaCreado = fechaActual,
                    documento = (nuevaFactura.id).ToString()
                };
                db.movimientos.Add(convenioMov2);
            }

            if (valorCashBack > 0)
            {
                var cashBackMov = new movimientos()
                {
                    tipoComprobante = "CC1",
                    numero = Comprobante.consecutivo,
                    cuenta = "26480502",
                    terceroId = cliente,
                    detalle = "CASH BACK",
                    debito = valorCashBack,
                    credito = 0,
                    baseMov = 0,
                    centroCostoId = 3,
                    fechaCreado = fechaActual,
                    documento = (nuevaFactura.id).ToString()
                };
                db.movimientos.Add(cashBackMov);

                var cashBackMov2 = new movimientos()
                {
                    tipoComprobante = "CC1",
                    numero = Comprobante.consecutivo,
                    cuenta = "26080502",
                    terceroId = cliente,
                    detalle = "CASH BACK",
                    debito = 0,
                    credito = valorCashBack,
                    baseMov = 0,
                    centroCostoId = 3,
                    fechaCreado = fechaActual,
                    documento = (nuevaFactura.id).ToString()
                };
                db.movimientos.Add(cashBackMov);
            }

            var caja = new movimientos()//no es caja,es clientes
            {
                tipoComprobante = "CC4",
                numero = Comprobante.consecutivo,
                cuenta = "13050501",
                terceroId = cliente,
                detalle = "Venta Credito",
                debito = totalCompra,
                credito = 0,
                baseMov = 0,
                centroCostoId = 3,
                fechaCreado = fechaActual,
                documento = (nuevaFactura.id).ToString()
            };
            db.movimientos.Add(caja);

            if (iva19 > 0)
            {
                var MovIva19 = new movimientos()
                {
                    tipoComprobante = "CC4",
                    numero = Comprobante.consecutivo,
                    cuenta = "24080501",
                    terceroId = cliente,
                    detalle = "Venta Credito",
                    debito = 0,
                    credito = iva19,
                    baseMov = 0,
                    centroCostoId = 3,
                    fechaCreado = fechaActual,
                    documento = (nuevaFactura.id).ToString()
                };
                db.movimientos.Add(MovIva19);
            }

            if (iva5 > 0)
            {
                var MovIva5 = new movimientos()
                {
                    tipoComprobante = "CC4",
                    numero = Comprobante.consecutivo,
                    cuenta = "24080502",
                    terceroId = cliente,
                    detalle = "Venta Credito",
                    debito = 0,
                    credito = iva5,
                    baseMov = 0,
                    centroCostoId = 3,
                    fechaCreado = fechaActual,
                    documento = (nuevaFactura.id).ToString()
                };
                db.movimientos.Add(MovIva5);
            }

            if (ivaGaseosas19 > 0 || ivaGaseosas5 > 0)
            {
                var MovIvaGaseosas = new movimientos()
                {
                    tipoComprobante = "CC4",
                    numero = Comprobante.consecutivo,
                    cuenta = "24080503",
                    terceroId = cliente,
                    detalle = "Venta Credito",
                    debito = 0,
                    credito = ivaGaseosas19 + ivaGaseosas5,
                    baseMov = 0,
                    centroCostoId = 3,
                    fechaCreado = fechaActual,
                    documento = (nuevaFactura.id).ToString()
                };
                db.movimientos.Add(MovIvaGaseosas);
            }

            if (ventas19 > 0)
            {
                var MovVentas19 = new movimientos()
                {
                    tipoComprobante = "CC4",
                    numero = Comprobante.consecutivo,
                    cuenta = "41350501",
                    terceroId = cliente,
                    detalle = "Venta Credito",
                    debito = 0,
                    credito = ventas19,
                    baseMov = 0,
                    centroCostoId = 3,
                    fechaCreado = fechaActual,
                    documento = (nuevaFactura.id).ToString()
                };
                db.movimientos.Add(MovVentas19);
            }

            if (ventasGaseosas > 0)
            {
                var MovVentasGaseosas = new movimientos()
                {
                    tipoComprobante = "CC4",
                    numero = Comprobante.consecutivo,
                    cuenta = "41350502",
                    terceroId = cliente,
                    detalle = "Venta Credito",
                    debito = 0,
                    credito = ventasGaseosas,
                    baseMov = 0,
                    centroCostoId = 3,
                    fechaCreado = fechaActual,
                    documento = (nuevaFactura.id).ToString()
                };
                db.movimientos.Add(MovVentasGaseosas);
            }

            if (ventas5 > 0)
            {
                var MovVentas5 = new movimientos()
                {
                    tipoComprobante = "CC4",
                    numero = Comprobante.consecutivo,
                    cuenta = "41350503",
                    terceroId = cliente,
                    detalle = "Venta Credito",
                    debito = 0,
                    credito = ventas5,
                    baseMov = 0,
                    centroCostoId = 3,
                    fechaCreado = fechaActual,
                    documento = (nuevaFactura.id).ToString()
                };
                db.movimientos.Add(MovVentas5);
            }

            if (ventasExcentos > 0)
            {
                var MovVentasExcentos = new movimientos()
                {
                    tipoComprobante = "CC4",
                    numero = Comprobante.consecutivo,
                    cuenta = "41350504",
                    terceroId = cliente,
                    detalle = "Venta Credito",
                    debito = 0,
                    credito = ventasExcentos,
                    baseMov = 0,
                    centroCostoId = 3,
                    fechaCreado = fechaActual,
                    documento = (nuevaFactura.id).ToString()
                };
                db.movimientos.Add(MovVentasExcentos);
            }

            if (ventasExcluidos > 0)
            {
                var MovVentasExcluidos = new movimientos()
                {
                    tipoComprobante = "CC4",
                    numero = Comprobante.consecutivo,
                    cuenta = "41350505",
                    terceroId = cliente,
                    detalle = "Venta Credito",
                    debito = 0,
                    credito = ventasExcluidos,
                    baseMov = 0,
                    centroCostoId = 3,
                    fechaCreado = fechaActual,
                    documento = (nuevaFactura.id).ToString()
                };
                db.movimientos.Add(MovVentasExcluidos);
            }

            if (costo19 > 0)
            {
                var MovCosto19 = new movimientos()
                {
                    tipoComprobante = "CC4",
                    numero = Comprobante.consecutivo,
                    cuenta = "61350501",
                    terceroId = cliente,
                    detalle = "Venta Credito",
                    debito = costo19,
                    credito = 0,
                    baseMov = 0,
                    centroCostoId = 3,
                    fechaCreado = fechaActual,
                    documento = (nuevaFactura.id).ToString()
                };
                db.movimientos.Add(MovCosto19);
            }

            if (costoGaseosas > 0)
            {
                var MovCostoGaseosas = new movimientos()
                {
                    tipoComprobante = "CC4",
                    numero = Comprobante.consecutivo,
                    cuenta = "61350502",
                    terceroId = cliente,
                    detalle = "Venta Credito",
                    debito = costoGaseosas,
                    credito = 0,
                    baseMov = 0,
                    centroCostoId = 3,
                    fechaCreado = fechaActual,
                    documento = (nuevaFactura.id).ToString()
                };
                db.movimientos.Add(MovCostoGaseosas);
            }

            if (costo5 > 0)
            {
                var MovCosto5 = new movimientos()
                {
                    tipoComprobante = "CC4",
                    numero = Comprobante.consecutivo,
                    cuenta = "61350503",
                    terceroId = cliente,
                    detalle = "Venta Credito",
                    debito = costo5,
                    credito = 0,
                    baseMov = 0,
                    centroCostoId = 3,
                    fechaCreado = fechaActual,
                    documento = (nuevaFactura.id).ToString()
                };
                db.movimientos.Add(MovCosto5);
            }

            if (costoExcentos > 0)
            {
                var MovCostoExcentos = new movimientos()
                {
                    tipoComprobante = "CC4",
                    numero = Comprobante.consecutivo,
                    cuenta = "61350504",
                    terceroId = cliente,
                    detalle = "Venta Credito",
                    debito = costoExcentos,
                    credito = 0,
                    baseMov = 0,
                    centroCostoId = 3,
                    fechaCreado = fechaActual,
                    documento = (nuevaFactura.id).ToString()
                };
                db.movimientos.Add(MovCostoExcentos);
            }

            if (costoExcluidos > 0)
            {
                var MovCostoExcluidos = new movimientos()
                {
                    tipoComprobante = "CC4",
                    numero = Comprobante.consecutivo,
                    cuenta = "61350505",
                    terceroId = cliente,
                    detalle = "Venta Credito",
                    debito = costoExcluidos,
                    credito = 0,
                    baseMov = 0,
                    centroCostoId = 3,
                    fechaCreado = fechaActual,
                    documento = (nuevaFactura.id).ToString()
                };
                db.movimientos.Add(MovCostoExcluidos);
            }

            if (inventario19 > 0)
            {
                var MovInventarios19 = new movimientos()
                {
                    tipoComprobante = "CC4",
                    numero = Comprobante.consecutivo,
                    cuenta = "14350501",
                    terceroId = cliente,
                    detalle = "Venta Credito",
                    debito = 0,
                    credito = inventario19,
                    baseMov = 0,
                    centroCostoId = 3,
                    fechaCreado = fechaActual,
                    documento = (nuevaFactura.id).ToString()
                };
                db.movimientos.Add(MovInventarios19);
            }

            if (inventarioGaseosas > 0)
            {
                var MovInventariosGaseosas = new movimientos()
                {
                    tipoComprobante = "CC4",
                    numero = Comprobante.consecutivo,
                    cuenta = "14350502",
                    terceroId = cliente,
                    detalle = "Venta Credito",
                    debito = 0,
                    credito = inventarioGaseosas,
                    baseMov = 0,
                    centroCostoId = 3,
                    fechaCreado = fechaActual,
                    documento = (nuevaFactura.id).ToString()
                };
                db.movimientos.Add(MovInventariosGaseosas);
            }

            if (inventario5 > 0)
            {
                var MovInventarios5 = new movimientos()
                {
                    tipoComprobante = "CC4",
                    numero = Comprobante.consecutivo,
                    cuenta = "14350503",
                    terceroId = cliente,
                    detalle = "Venta Credito",
                    debito = 0,
                    credito = inventario5,
                    baseMov = 0,
                    centroCostoId = 3,
                    fechaCreado = fechaActual,
                    documento = (nuevaFactura.id).ToString()
                };
                db.movimientos.Add(MovInventarios5);
            }

            if (inventarioExcentos > 0)
            {
                var MovInventariosExcentos = new movimientos()
                {
                    tipoComprobante = "CC4",
                    numero = Comprobante.consecutivo,
                    cuenta = "14350504",
                    terceroId = cliente,
                    detalle = "Venta Credito",
                    debito = 0,
                    credito = inventarioExcentos,
                    baseMov = 0,
                    centroCostoId = 3,
                    fechaCreado = fechaActual,
                    documento = (nuevaFactura.id).ToString()
                };
                db.movimientos.Add(MovInventariosExcentos);
            }

            if (inventarioExcluidos > 0)
            {
                var MovInventariosExcluidos = new movimientos()
                {
                    tipoComprobante = "CC4",
                    numero = Comprobante.consecutivo,
                    cuenta = "14350505",
                    terceroId = cliente,
                    detalle = "Venta Credito",
                    debito = 0,
                    credito = inventarioExcluidos,
                    baseMov = 0,
                    centroCostoId = 3,
                    fechaCreado = fechaActual,
                    documento = (nuevaFactura.id).ToString()
                };
                db.movimientos.Add(MovInventariosExcluidos);
            }

            if (inventarioBolsas > 0)
            {
                var MovInventarioBolsas = new movimientos()
                {
                    tipoComprobante = "CC4",
                    numero = Comprobante.consecutivo,
                    cuenta = "14350580",
                    terceroId = cliente,
                    detalle = "Venta Credito",
                    debito = 0,
                    credito = inventarioBolsas,
                    baseMov = 0,
                    centroCostoId = 3,
                    fechaCreado = fechaActual,
                    documento = (nuevaFactura.id).ToString()
                };
                db.movimientos.Add(MovInventarioBolsas);

                var MovImpuestoBolsas = new movimientos()
                {
                    tipoComprobante = "CC4",
                    numero = Comprobante.consecutivo,
                    cuenta = "529540",
                    terceroId = cliente,
                    detalle = "Venta Credito",
                    debito = inventarioBolsas,
                    credito = 0,
                    baseMov = 0,
                    centroCostoId = 3,
                    fechaCreado = fechaActual,
                    documento = (nuevaFactura.id).ToString()
                };
                db.movimientos.Add(MovImpuestoBolsas);

                var MovImpuestoBolsas2 = new movimientos()
                {
                    tipoComprobante = "CC4",
                    numero = Comprobante.consecutivo,
                    cuenta = "241001",
                    terceroId = cliente,
                    detalle = "Venta Credito",
                    debito = 0,
                    credito = valorTotalBolsas,
                    baseMov = 0,
                    centroCostoId = 3,
                    fechaCreado = fechaActual,
                    documento = (nuevaFactura.id).ToString()
                };
                db.movimientos.Add(MovImpuestoBolsas2);
            }


            var numCompr = Convert.ToInt32(Comprobante.consecutivo);
            Comprobante.consecutivo = (1 + numCompr).ToString();
            db.Entry(Comprobante).State = System.Data.Entity.EntityState.Modified;

            db.SaveChanges();
            return Json(nuevaFactura.id, JsonRequestBehavior.AllowGet);
        }

        

        

        // GET: Procesos/Procesos/Create
        [Authorize]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Procesos/Procesos/Create
        [HttpPost]
        [Authorize]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Procesos/Procesos/Edit/5
        [Authorize]
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Procesos/Procesos/Edit/5
        [HttpPost]
        [Authorize]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Procesos/Procesos/Delete/5
        [Authorize]
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Procesos/Procesos/Delete/5
        [HttpPost]
        [Authorize]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public bool GetFEpdf(int IdCliente, factura factura, string cufe, string codigoQR, List<operation> ListOperaciones, string NombreDocumento)
        {
            try
            {
                string path = System.AppDomain.CurrentDomain.BaseDirectory+"Certificado\\"+NombreDocumento+".pdf";

                formato.CurrencyGroupSeparator = ".";
                formato.NumberDecimalSeparator = ",";
                string RutaLogo = System.AppDomain.CurrentDomain.BaseDirectory + "Content\\imagenes\\Logo.jpeg";
                var InfoEmisor = db.ParametrosFE.FirstOrDefault();
                var InfoAdquiriente = new ConsultasController().GetModelAdquiriente(IdCliente);
                var InfoProductos = new ConsultasController().ListProductos(ListOperaciones);
                //byte[] QRbyte = HerramientasFE.GetImageQR(codigoQR);
                var ModelFacturaPdf = new ViewModelFEpdf()
                {
                    ParametrosFE = InfoEmisor,
                    Adquiriente = InfoAdquiriente,
                    ListOperaciones = InfoProductos,
                    FechaEmision = factura.date.ToString("yyyy-MM-dd"),
                    HoraEmision = factura.date.ToString("HH:mm:ss"),
                    FechaVencimiento = factura.date.ToString("yyyy-MM-dd"),
                    Cufe = cufe,
                    NumeroFactura = factura.codConsecutivo + factura.numeroFactura,
                    SubtotalPrecioUnitario = Math.Round(factura.valorTotalExcentos + factura.valorTotalExcluidos + factura.baseIVA19 + factura.baseIVA5, 0, MidpointRounding.ToEven).ToString("N0", formato),
                    DescuentosDetalle = 0.ToString(),
                    RecargosDetalle = 0.ToString(),
                    SubtotalNoGravados = Math.Round(factura.valorTotalExcentos + factura.valorTotalExcluidos, 0, MidpointRounding.ToEven).ToString("N0", formato),
                    SubtotalBaseGravable = Math.Round(factura.baseIVA19 + factura.baseIVA5, 0, MidpointRounding.ToEven).ToString("N0", formato),
                    TotalImpuesto = Math.Round(factura.valorIVA19 + factura.valorIVA5, 0, MidpointRounding.ToEven).ToString("N0", formato),
                    TotalMasImpuesto = Math.Round(factura.total, 0, MidpointRounding.ToEven).ToString("N0", formato),
                    DescuentoGlobal = 0.ToString(),
                    RecargoGlobal = 0.ToString(),
                    Anticipo = 0.ToString(),
                    ValorTotal = Math.Round(factura.total - factura.totalDiscount, 0, MidpointRounding.ToEven).ToString("N0", formato),
                    ValorTotalEnLetras = "valor en letras",
                    QR = codigoQR
                };


                var facturaPdf = new FacturaPdf(ModelFacturaPdf, DocumentoPdf.TIPO_FACTURA_VENTA)
                {
                    RutaLogo = RutaLogo,
                    TextoEncabezado = "Persona Natural \r\n Responsable de IVA",
                    TextoConstancia = DocumentoPdf.TEXTO_CONSTANCIA_FACTURA_DEFAULT,
                    TextoQR = codigoQR,
                    TextoResolucion = "A esta factura de venta aplican las normas relativas a la letra de cambio (articulo 5 Ley 1231 de 2008). Con esta el comprador declara haber recibido real y materialmente las mercancías o prestación de servicios descritos en este título - valor. Número Autorización 18764015297112 aprobado en 20210721 prefijo POS desde el número 582 al 1000 Vigencia: 6 meses Responsable de IVA - Actividad Económica 4711 Comercio al por menor en establecimientos no especializados con surtido compuesto principalmente por alimentos, bebidas (alcohólicas y no alcohólicas) o tabaco Tarifa 4.9 X MIL."
                };  

                var bytesPdf = facturaPdf.Generar();
                System.IO.FileStream stream = new FileStream(path, FileMode.CreateNew);
                System.IO.BinaryWriter writer = new BinaryWriter(stream);
                writer.Write(bytesPdf, 0, bytesPdf.Length);
                writer.Close();

                return true;
            }
            catch (Exception ex)
            {
                return false;
                throw;
            }
        }

        

        public ActionResult SetFEpdf(string array)
        {
            var adqui = new ModelAdquiriente()
            {
                ContactoAdquiriente = "",
                CorreoAdquiriente = "",
                DireccionAdquiriente = "",
                DocumentoAdquiriente = "2",
                NombreAdquiriente = "asdf"
            };
            var nuevaFactura = new factura()
            {
                personId = 1,
                userId = "e986b3c0-65a9-401c-b507-5c1812b5d2e5",
                operationTypeId = 15,
                date = DateTime.Now,
                total = 0,
                tipo = 1,//1 equivale a contado
                fechaPagoCredito = "N",
                cash = 0,
                codConsecutivo = "set",
                numeroFactura = 45,
                valorTotalExcentos = 0,
                valorTotalExcluidos = 0,
                baseIVA19 = 0,
                baseIVA5 = 0,
                valorIVA19 = 0,
                valorIVA5 = 0,
                totalBolsas = 0,
                valorConvenio = 0,
                facturacion = "sdfsfd"
            };
            var prod = new ModelListProductos()
            {
                Cantidad = 1,
                Descripcion = "sdfsf",
                Id = 1,
                Impuesto = "0",
                ValorTotal = "0",
                ValorUnitario = "0",
            };
            List<ModelListProductos> listado = new List<ModelListProductos>();
            listado.Add(prod);

            var para = db.ParametrosFE.FirstOrDefault();
            var ModelFacturaPdf = new ViewModelFEpdf()
            {
                Adquiriente = adqui,
                Anticipo = "00",
                Cufe = "adfsaf",
                DescuentoGlobal = "asfddsf",
                DescuentosDetalle = "fdff",
                Factura = nuevaFactura,
                FechaEmision = "dddd",
                FechaVencimiento = "sfd",
                ListOperaciones = listado,
                NumeroFactura = "dff",
                ParametrosFE = para,
                RecargoGlobal = "0",
                RecargosDetalle = "0",
                SubtotalBaseGravable = "0",
                SubtotalNoGravados = "0",
                SubtotalPrecioUnitario = "0",
                TotalImpuesto = "0",
                TotalMasImpuesto = "0",
                ValorTotal = "0",
                ValorTotalEnLetras = "0"
            };

            return View(ModelFacturaPdf);

        }
        public void GetRangoNumeracion()
        {
            var clienteDian = new ClienteServicioDian
            {
                Ambiente = AMBIENTE_SERVICIO,
                RutaCertificado = RUTA_CERTIFICADO,
                ClaveCertificado = CLAVE_CERTIFICADO
            };


            var DatosRango = clienteDian.ObtenerRangosNumeracion("27493819", "c63d6265-d2c1-4531-bafa-c594bc6f86af");
            var pausa = "";
        }
}
}
