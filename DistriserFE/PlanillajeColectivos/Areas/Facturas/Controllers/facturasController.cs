using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Common;
using Newtonsoft.Json;
using PlanillajeColectivos.Areas.Contabilidad.Controllers;
using PlanillajeColectivos.Areas.Persons.Controllers;
using PlanillajeColectivos.Areas.Procesos.Controllers;
using PlanillajeColectivos.DTO;
using PlanillajeColectivos.DTO.Contabilidad;
using PlanillajeColectivos.DTO.DataTable;
using PlanillajeColectivos.DTO.FacturacionElectronica;
using PlanillajeColectivos.DTO.FacturacionElectronica.FacturaPD;
using PlanillajeColectivos.DTO.Products;
using PlanillajeColectivos.Tools;
using Rotativa;

namespace PlanillajeColectivos.Areas.Facturas.Controllers
{
    public class facturasController : Controller
    {

        private AccountingContext db = new AccountingContext();
        NumberFormatInfo formato = new CultureInfo("es-CO").NumberFormat;
        // GET: Facturas/facturas/facturasPedidos
        [Authorize]
        public ActionResult facturasPedidos()
        {
            var userId = CurrentUser.Get.UserId;
            return View(db.factura.Where(f => f.operationTypeId == 3 && f.userId == userId).ToList());
        }

        [Authorize(Roles = "Empaque")]
        public ActionResult facturasEnEmpaque()
        {
            return View(db.factura.Where(f => f.operationTypeId == 4).ToList());
        }

        [Authorize]
        public ActionResult facturasEnDespachos()
        {
            return View(db.factura.Where(f => f.operationTypeId == 5).ToList());
        }

        [Authorize(Roles = "Admin")]
        public ActionResult facturasTerminadas()
        {
            return View(db.factura.Where(f => f.operationTypeId == 6 || f.operationTypeId == 12 || f.operationTypeId == 16).ToList());
        }

        [Authorize(Roles = "Admin")]
        public ActionResult facturasContabilizadas()
        {
            return View(db.factura.Where(f => f.operationTypeId == 6).ToList());
        }

        [Authorize(Roles = "Admin")]
        public ActionResult facturasAbastecimientos()
        {
            return View(db.factura.Where(f => f.operationTypeId == 2).ToList());
        }
        [Authorize(Roles = "Admin")]
        public ActionResult IrIndexFacturaCompra(string IdFactura)
        {
            using (var ctx = new AccountingContext())
            {
                var ListadoOperaciones = db.operation.Where(x => x.facturaId == IdFactura && x.operationTypeId == 20).ToList();
                foreach (var item in ListadoOperaciones)
                {
                    db.Entry(item).State = System.Data.Entity.EntityState.Deleted;
                }
                db.SaveChanges();

                return RedirectToAction("facturasAbastecimientos");
            }
        }

        [Authorize]
        public ActionResult misFacturas()
        {
            var userId = CurrentUser.Get.UserId;
            return View(db.factura.Where(f => f.operationTypeId == 6 && f.userId == userId).ToList());
        }

        [Authorize]
        public ActionResult editarFacturaPedido(int id)
        {
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
            ViewBag.facturaId = id;
            return View(products);
        }

        [Authorize(Roles = "Empaque")]
        public ActionResult editarFacturaEmpaque(int id)
        {
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
            ViewBag.facturaId = id;
            return View(products);
        }

        [Authorize]
        public ActionResult editarFacturaDespachos(int id)
        {
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
            ViewBag.facturaId = id;
            return View(products);
        }

        [HttpPost]
        public JsonResult verificarContabilizada(string facturaId)
        {
            var factura = (from pc in db.factura where pc.id == facturaId select pc).Single();
            if (factura.operationTypeId == 12)
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }
            else if (factura.operationTypeId == 6)
            {
                return Json(1, JsonRequestBehavior.AllowGet);
            }
            return Json(2, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public ActionResult verFacturaTipo(string id)
        {
            var factura = (from pc in db.factura where pc.id == id select pc).Single();
            if (factura.tipo == 1)
            {
                return RedirectToAction("verFacturaContado", new { id = id });
            }
            else if (factura.tipo == 2)
            {
                return RedirectToAction("verFacturaCredito", new { id = id });
            }
            else
            {
                return RedirectToAction("misFacturas");
            }
        }

        [Authorize(Roles = "Admin")]
        public ActionResult verFacturaTerminada(string id)
        {
            var factura = (from pc in db.factura where pc.id == id select pc).Single();
            var Usuario = (from pc in db.usersTabla where pc.id == factura.userId select pc).Single();

            //lista de Costos Adicionales
            List<SelectListItem> pedidoContado = new List<SelectListItem>();   // Creo una lista
            var listaOperaciones = db.operation.Where(p => (p.operationTypeId == 6 || p.operationTypeId == 12 || p.operationTypeId == 16) && p.facturaId == id).ToList();

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
        public ActionResult verFacturaContado(string id)
        {
            var factura = (from pc in db.factura where pc.id == id select pc).Single();
            var Usuario = (from pc in db.usersTabla where pc.id == factura.userId select pc).Single();

            //lista de Costos Adicionales
            List<SelectListItem> pedidoContado = new List<SelectListItem>();   // Creo una lista
            var listaOperaciones = db.operation.Where(p => (p.operationTypeId == 6 || p.operationTypeId == 12) && p.facturaId == id).ToList();

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
                    unidad = item.products.priceOut;
                }
                else
                {
                    unidad = item.products.priceOut2;
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

                enviarPedidosContado.Add(productos);
            }

            return View(enviarPedidosContado);
        }

        [Authorize]
        public ActionResult verFacturaAbastecimiento(string id)
        {
            var factura = (from pc in db.factura where pc.id == id select pc).Single();
            var listaOperaciones = db.operation.Where(p => p.operationTypeId == 2 && p.facturaId == id).ToList();

            List<pedidosViewModel> enviarPedidosContado = new List<pedidosViewModel>();

            decimal SubTotal = 0;
            decimal Iva = 0;
            decimal Total = 0;
            foreach (var item in listaOperaciones)
            {
                SubTotal = item.price * item.quantity;
                Iva = (SubTotal - item.discount) * Decimal.Divide(item.products.ivaFK.value, 100);
                Total = SubTotal - item.discount + Iva;

                var productos = new pedidosViewModel()
                {
                    cantidad = item.quantity,
                    codigoBarras = item.products.barcode.ToString(),
                    nombre = item.products.name,
                    unidad = item.price,
                    subtotal = SubTotal,
                    descuento = item.discount,
                    iva = Iva.ToString("N2"),
                    total = Total,
                    operatioId = item.id,
                    nombreProveedor = item.products.providerFK.name
                };

                ViewBag.total = Convert.ToInt32(factura.total - factura.totalDiscount);

                enviarPedidosContado.Add(productos);
            }

            return View(enviarPedidosContado);
        }

        [HttpPost]
        [Authorize]
        public JsonResult contabilizarFactura(string facturaId)
        {
            var factura = (from pc in db.factura where pc.id == facturaId select pc).Single();
            if (factura.tipo == 1)//SI ES DE CONTADO
            {
                var listaOperaciones = db.operation.Where(p => p.facturaId == facturaId).ToList();

                decimal iva5 = 0;
                decimal iva19 = 0;
                decimal costoDeVenta = 0;
                decimal totalPorProducto = 0;
                decimal ivaEnDecimal = Convert.ToDecimal(0.19);
                decimal disciminadoPorcentaje = 0;
                decimal total = 0;

                foreach (var item in listaOperaciones)
                {
                    decimal subTotal = 0;
                    subTotal = item.quantity * item.price;
                    total = total + subTotal;

                    totalPorProducto = 0;
                    if (item.products.ivaId == 1)
                    {
                        totalPorProducto = item.products.priceOut * item.quantity;
                        disciminadoPorcentaje = totalPorProducto * ivaEnDecimal;
                        iva19 = iva19 + disciminadoPorcentaje;
                    }
                    else if (item.products.ivaId == 1)
                    {
                        iva5 = iva5 + ((item.products.priceOut * item.quantity) * (5 / 100));
                    }

                    costoDeVenta = costoDeVenta + (item.products.priceIn * item.quantity);

                    item.operationTypeId = 12;
                    db.Entry(item).State = System.Data.Entity.EntityState.Modified;

                }

                //MOVIMIENTOS CUENTAS
                //var caja = new movimientos()
                //{
                //    facturaId = facturaId,
                //    codigoCuenta = "11050501",
                //    debito = total,
                //    credito = 0,
                //    terceroId = factura.personId,
                //    tipo = "ventaContadoContabilizada",
                //    fecha = DateTime.Now
                //};
                //db.movimientos.Add(caja);

                //if (iva19 != 0)
                //{
                //    var MovIva19 = new movimientos()
                //    {
                //        facturaId = facturaId,
                //        codigoCuenta = "24080501",
                //        debito = 0,
                //        credito = iva19,
                //        terceroId = factura.personId,
                //        tipo = "ventaContadoContabilizada",
                //        fecha = DateTime.Now
                //    };
                //    db.movimientos.Add(MovIva19);
                //}

                //if (iva5 != 0)
                //{
                //    var MovIva5 = new movimientos()
                //    {
                //        facturaId = facturaId,
                //        codigoCuenta = "24080502",
                //        debito = 0,
                //        credito = iva5,
                //        terceroId = factura.personId,
                //        tipo = "ventaContadoContabilizada",
                //        fecha = DateTime.Now
                //    };
                //    db.movimientos.Add(MovIva5);
                //}

                //decimal ventas = total - iva19 - iva5;
                //var MovVentas = new movimientos()
                //{
                //    facturaId = facturaId,
                //    codigoCuenta = "41350501",
                //    debito = 0,
                //    credito = ventas,
                //    terceroId = factura.personId,
                //    tipo = "ventaContadoContabilizada",
                //    fecha = DateTime.Now
                //};
                //db.movimientos.Add(MovVentas);

                //var MovCostoVenta = new movimientos()
                //{
                //    facturaId = facturaId,
                //    codigoCuenta = "61350501",
                //    debito = costoDeVenta,
                //    credito = 0,
                //    terceroId = factura.personId,
                //    tipo = "ventaContadoContabilizada",
                //    fecha = DateTime.Now
                //};
                //db.movimientos.Add(MovCostoVenta);

                //var MovInventarios = new movimientos()
                //{
                //    facturaId = facturaId,
                //    codigoCuenta = "14350501",
                //    debito = 0,
                //    credito = costoDeVenta,
                //    terceroId = 0,
                //    tipo = "ventaContadoContabilizada",
                //    fecha = DateTime.Now
                //};
                //db.movimientos.Add(MovInventarios);

                factura.total = total;
                factura.operationTypeId = 12;
                db.Entry(factura).State = System.Data.Entity.EntityState.Modified;

                db.SaveChanges();
            }
            else if (factura.tipo == 2)
            {
                var listaOperaciones = db.operation.Where(p => p.facturaId == facturaId).ToList();

                decimal iva5 = 0;
                decimal iva19 = 0;
                decimal costoDeVenta = 0;
                decimal totalPorProducto = 0;
                decimal ivaEnDecimal = Convert.ToDecimal(0.19);
                decimal disciminadoPorcentaje = 0;
                decimal total = 0;

                foreach (var item in listaOperaciones)
                {
                    decimal subTotal = 0;
                    subTotal = item.quantity * item.price;
                    total = total + subTotal;

                    totalPorProducto = 0;
                    if (item.products.ivaId == 1)
                    {
                        totalPorProducto = item.products.priceOut2 * item.quantity;
                        disciminadoPorcentaje = totalPorProducto * ivaEnDecimal;
                        iva19 = iva19 + disciminadoPorcentaje;
                    }
                    else if (item.products.ivaId == 1)
                    {
                        iva5 = iva5 + ((item.products.priceOut2 * item.quantity) * (5 / 100));
                    }

                    costoDeVenta = costoDeVenta + (item.products.priceIn * item.quantity);

                    item.operationTypeId = 12;
                }

                //MOVIMIENTOS CUENTAS
                //var clientes = new movimientos()
                //{
                //    facturaId = facturaId,
                //    codigoCuenta = "13050501",
                //    debito = total,
                //    credito = 0,
                //    terceroId = factura.personId,
                //    tipo = "ventaCreditoContabilizada",
                //    fecha = DateTime.Now
                //};
                //db.movimientos.Add(clientes);

                //if (iva19 != 0)
                //{
                //    var MovIva19 = new movimientos()
                //    {
                //        facturaId = facturaId,
                //        codigoCuenta = "24080501",
                //        debito = 0,
                //        credito = iva19,
                //        terceroId = factura.personId,
                //        tipo = "ventaCreditoContabilizada",
                //        fecha = DateTime.Now
                //    };
                //    db.movimientos.Add(MovIva19);
                //}

                //if (iva5 != 0)
                //{
                //    var MovIva5 = new movimientos()
                //    {
                //        facturaId = facturaId,
                //        codigoCuenta = "24080502",
                //        debito = 0,
                //        credito = iva5,
                //        terceroId = factura.personId,
                //        tipo = "ventaCreditoContabilizada",
                //        fecha = DateTime.Now
                //    };
                //    db.movimientos.Add(MovIva5);
                //}

                //decimal ventas = total - iva19 - iva5;
                //var MovVentas = new movimientos()
                //{
                //    facturaId = facturaId,
                //    codigoCuenta = "41350501",
                //    debito = 0,
                //    credito = ventas,
                //    terceroId = factura.personId,
                //    tipo = "ventaCreditoContabilizada",
                //    fecha = DateTime.Now
                //};
                //db.movimientos.Add(MovVentas);

                //var MovCostoVenta = new movimientos()
                //{
                //    facturaId = facturaId,
                //    codigoCuenta = "61350501",
                //    debito = costoDeVenta,
                //    credito = 0,
                //    terceroId = factura.personId,
                //    tipo = "ventaCreditoContabilizada",
                //    fecha = DateTime.Now
                //};
                //db.movimientos.Add(MovCostoVenta);

                //var MovInventarios = new movimientos()
                //{
                //    facturaId = facturaId,
                //    codigoCuenta = "14350501",
                //    debito = 0,
                //    credito = costoDeVenta,
                //    terceroId = 0,
                //    tipo = "ventaCreditoContabilizada",
                //    fecha = DateTime.Now
                //};
                //db.movimientos.Add(MovInventarios);

                factura.total = total;
                factura.operationTypeId = 12;
                db.Entry(factura).State = System.Data.Entity.EntityState.Modified;

                db.SaveChanges();
            }
            return Json(1, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public ActionResult verFacturaCredito(string id)
        {
            var factura = (from pc in db.factura where pc.id == id select pc).Single();
            var Usuario = (from pc in db.usersTabla where pc.id == factura.userId select pc).Single();

            //lista de Costos Adicionales
            List<SelectListItem> pedidoContado = new List<SelectListItem>();   // Creo una lista
            var listaOperaciones = db.operation.Where(p => (p.operationTypeId == 6 || p.operationTypeId == 12) && p.facturaId == id).ToList();

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
                    unidad = item.products.priceOut;
                }
                else
                {
                    unidad = item.products.priceOut2;
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
                ViewBag.facturaId = id;
                ViewBag.saldo = Convert.ToInt32(factura.saldoCredito);

                ViewBag.vendedor = Usuario.cedula + " " + Usuario.nombre + " " + Usuario.apellido;

                enviarPedidosContado.Add(productos);
            }

            return View(enviarPedidosContado);
        }


        [Authorize(Roles = "Admin")]
        public ActionResult imprimirFactura(string id)
        {
            var factura = (from pc in db.factura where pc.id == id select pc).Single();
            var userId = CurrentUser.Get.UserId;
            List<SelectListItem> pedidoContado = new List<SelectListItem>();   // Creo una lista
            var listaOperaciones = db.operation.Where(p => p.operationTypeId == 5 && p.facturaId == id).ToList();

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
                    unidad = item.products.priceOut;
                }
                else
                {
                    unidad = item.products.priceOut2;
                }


                var productos = new pedidosViewModel()
                {
                    cantidad = item.quantity,
                    nombre = item.products.name,
                    unidad = unidad,
                    total = total
                };

                enviarPedidosContado.Add(productos);
            }

            ViewBag.cliente = factura.persons.name;
            ViewBag.direccion = factura.persons.direccion;
            ViewBag.celular = factura.persons.celular;
            ViewBag.vendedor = factura.usersTabla.nombre + " " + factura.usersTabla.apellido;
            ViewBag.numeroFactura = factura.id;
            ViewBag.PrefijoFactura = factura.codConsecutivo;
            var fecha = DateTime.Now;
            ViewBag.fecha = fecha;
            ViewBag.total = Convert.ToDecimal(factura.total).ToString("#,##");
            if (factura.tipo == 2)
            {
                ViewBag.credito = Convert.ToDecimal(factura.total).ToString("#,##");
                ViewBag.efectivo = 0;
            }
            else
            {
                ViewBag.credito = 0;
                ViewBag.efectivo = Convert.ToDecimal(factura.total).ToString("#,##");
            }

            factura.stateId = 1;
            db.Entry(factura).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            return View(enviarPedidosContado);
        }

        [Authorize]
        public ActionResult getAbonosCreditos(int facturaId)
        {
            var userId = CurrentUser.Get.UserId;
            var listaAbonos = db.creditos.Where(p => p.facturaId == facturaId).ToList();

            List<creditos> enviarAbonos = new List<creditos>();
            foreach (var item in listaAbonos)
            {
                var abono = new creditos()
                {
                    fechaPago = item.fechaPago,
                    valorPagado = item.valorPagado,
                    saldo = item.saldo
                };

                enviarAbonos.Add(abono);
            }

            string json = JsonConvert.SerializeObject(enviarAbonos);

            return Json(json);
        }

        [Authorize(Roles = "Empaque")]
        [HttpPost]
        public JsonResult enviarADespachos(string facturaId, int total)
        {
            var factura = (from pc in db.factura where pc.id == facturaId select pc).Single();
            var userId = CurrentUser.Get.UserId;

            var listaOperaciones = db.operation.Where(p => p.operationTypeId == 4 && p.facturaId == facturaId).ToList();

            foreach (var item in listaOperaciones)
            {
                item.operationTypeId = 5;
                db.Entry(item).State = System.Data.Entity.EntityState.Modified;
            }

            factura.total = total;
            factura.operationTypeId = 5;
            db.Entry(factura).State = System.Data.Entity.EntityState.Modified;

            db.SaveChanges();

            return Json(1, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        [HttpPost]
        public JsonResult enviarFacturaTerminada(string facturaId, int total)
        {
            var factura = (from pc in db.factura where pc.id == facturaId select pc).Single();
            var userId = CurrentUser.Get.UserId;

            var listaOperaciones = db.operation.Where(p => p.operationTypeId == 5 && p.facturaId == facturaId).ToList();

            foreach (var item in listaOperaciones)
            {
                item.operationTypeId = 6;
                db.Entry(item).State = System.Data.Entity.EntityState.Modified;
            }

            factura.total = total;
            factura.operationTypeId = 6;
            db.Entry(factura).State = System.Data.Entity.EntityState.Modified;

            db.SaveChanges();

            return Json(1, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        [HttpPost]
        public JsonResult enviarAPreparacion(string facturaId, int total)
        {
            var factura = (from pc in db.factura where pc.id == facturaId select pc).Single();
            var userId = CurrentUser.Get.UserId;

            var listaOperaciones = db.operation.Where(p => p.operationTypeId == 3 && p.userId == userId && p.facturaId == facturaId).ToList();

            foreach (var item in listaOperaciones)
            {
                item.operationTypeId = 4;
                db.Entry(item).State = System.Data.Entity.EntityState.Modified;
            }

            factura.total = total;
            factura.operationTypeId = 4;
            db.Entry(factura).State = System.Data.Entity.EntityState.Modified;

            db.SaveChanges();

            return Json(1, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public ActionResult getPedido(string facturaId)
        {
            var factura = (from pc in db.factura where pc.id == facturaId select pc).Single();
            var userId = CurrentUser.Get.UserId;
            //lista de Costos Adicionales
            List<SelectListItem> pedidoContado = new List<SelectListItem>();   // Creo una lista
            var listaOperaciones = db.operation.Where(p => p.operationTypeId == 3 && p.userId == userId && p.facturaId == facturaId).ToList();

            List<pedidosViewModel> enviarPedidosContado = new List<pedidosViewModel>();

            decimal valorTotal = 0;
            foreach (var item in listaOperaciones)
            {
                decimal total = 0;
                total = item.quantity * item.price;
                valorTotal = valorTotal + total;
                decimal unidad = 0;
                unidad = item.price;

                var productos = new pedidosViewModel()
                {
                    cantidad = item.quantity,
                    codigoBarras = item.products.barcode.ToString(),
                    nombre = item.products.name,
                    unidad = unidad,
                    total = total,
                    operatioId = item.id
                };

                enviarPedidosContado.Add(productos);
            }

            factura.total = valorTotal;
            db.Entry(factura).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            string json = JsonConvert.SerializeObject(enviarPedidosContado);

            return Json(json);
        }

        [Authorize]
        public ActionResult getPedidoEmpaque(string facturaId)
        {
            var factura = (from pc in db.factura where pc.id == facturaId select pc).Single();
            var userId = CurrentUser.Get.UserId;
            //lista de Costos Adicionales
            List<SelectListItem> pedidoContado = new List<SelectListItem>();   // Creo una lista
            var listaOperaciones = db.operation.Where(p => p.operationTypeId == 4 && p.facturaId == facturaId).ToList();

            List<pedidosViewModel> enviarPedidosContado = new List<pedidosViewModel>();

            decimal valorTotal = 0;
            foreach (var item in listaOperaciones)
            {
                decimal total = 0;
                total = item.quantity * item.price;
                valorTotal = valorTotal + total;
                decimal unidad = 0;
                unidad = item.price;

                var productos = new pedidosViewModel()
                {
                    cantidad = item.quantity,
                    codigoBarras = item.products.barcode.ToString(),
                    nombre = item.products.name,
                    unidad = unidad,
                    total = total,
                    operatioId = item.id
                };
                enviarPedidosContado.Add(productos);
            }
            factura.total = valorTotal;
            db.Entry(factura).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            string json = JsonConvert.SerializeObject(enviarPedidosContado);

            return Json(json);
        }

        [Authorize]
        public ActionResult getPedidoDespachos(string facturaId)
        {
            var factura = (from pc in db.factura where pc.id == facturaId select pc).Single();
            var userId = CurrentUser.Get.UserId;
            //lista de Costos Adicionales
            List<SelectListItem> pedidoContado = new List<SelectListItem>();   // Creo una lista
            var listaOperaciones = db.operation.Where(p => p.operationTypeId == 5 && p.facturaId == facturaId).ToList();

            List<pedidosViewModel> enviarPedidosContado = new List<pedidosViewModel>();

            decimal valorTotal = 0;
            foreach (var item in listaOperaciones)
            {
                decimal total = 0;
                total = item.quantity * item.price;
                valorTotal = valorTotal + total;
                decimal unidad = 0;
                unidad = item.price;

                var productos = new pedidosViewModel()
                {
                    cantidad = item.quantity,
                    codigoBarras = item.products.barcode.ToString(),
                    nombre = item.products.name,
                    unidad = unidad,
                    total = total,
                    operatioId = item.id
                };

                enviarPedidosContado.Add(productos);
            }
            factura.total = valorTotal;
            db.Entry(factura).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            string json = JsonConvert.SerializeObject(enviarPedidosContado);

            return Json(json);
        }

        [Authorize]
        public ActionResult Index()
        {
            return View(db.factura.ToList());
        }

        [Authorize]
        public JsonResult DeleteProducto(int id)
        {
            operation operation = db.operation.Find(id);
            db.operation.Remove(operation);
            db.SaveChanges();

            return Json(1, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public JsonResult AddProducto(int id, int cantidad, string facturaId)
        {
            var factura = (from pc in db.factura where pc.id == facturaId select pc).Single();
            var producto = (from pc in db.products where pc.id == id select pc).Single();
            var userId = CurrentUser.Get.UserId;
            decimal price = 0;
            if (factura.tipo == 1)
            {
                price = producto.priceOut;
            }
            else
            {
                price = producto.priceOut2;
            }

            var nuevaOpercion = new operation()
            {
                productId = id,
                quantity = cantidad,
                operationTypeId = 3,
                facturaId = facturaId,
                date = DateTime.Now,
                price = price,
                userId = userId
            };

            db.operation.Add(nuevaOpercion);
            db.SaveChanges();

            return Json(1, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public JsonResult AddProductoEmpaque(int id, int cantidad, string facturaId)
        {
            var factura = (from pc in db.factura where pc.id == facturaId select pc).Single();
            var producto = (from pc in db.products where pc.id == id select pc).Single();
            var userId = CurrentUser.Get.UserId;
            decimal price = 0;
            if (factura.tipo == 1)
            {
                price = producto.priceOut;
            }
            else
            {
                price = producto.priceOut2;
            }

            var nuevaOpercion = new operation()
            {
                productId = id,
                quantity = cantidad,
                operationTypeId = 4,
                facturaId = facturaId,
                date = DateTime.Now,
                price = price,
                userId = userId
            };

            db.operation.Add(nuevaOpercion);
            db.SaveChanges();

            return Json(1, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public JsonResult AddProductoDespacho(int id, int cantidad, string facturaId)
        {
            var factura = (from pc in db.factura where pc.id == facturaId select pc).Single();
            var producto = (from pc in db.products where pc.id == id select pc).Single();
            var userId = CurrentUser.Get.UserId;
            decimal price = 0;
            if (factura.tipo == 1)
            {
                price = producto.priceOut;
            }
            else
            {
                price = producto.priceOut2;
            }

            var nuevaOpercion = new operation()
            {
                productId = id,
                quantity = cantidad,
                operationTypeId = 5,
                facturaId = facturaId,
                date = DateTime.Now,
                price = price,
                userId = userId
            };

            db.operation.Add(nuevaOpercion);
            db.SaveChanges();

            return Json(1, JsonRequestBehavior.AllowGet);
        }

        // GET: Facturas/facturas/Details/5
        [Authorize]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            factura factura = db.factura.Find(id);
            if (factura == null)
            {
                return HttpNotFound();
            }
            return View(factura);
        }

        // GET: Facturas/facturas/Create
        [Authorize]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Facturas/facturas/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,personId,userId,operationTypeId,cash,payCash,payCredit,payTdebit,payTcredit,totalDiscount,date,observation,stateId,facturaNumber,tipo")] factura factura)
        {
            if (ModelState.IsValid)
            {
                db.factura.Add(factura);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(factura);
        }

        // GET: Facturas/facturas/Edit/5
        [Authorize]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            factura factura = db.factura.Find(id);
            if (factura == null)
            {
                return HttpNotFound();
            }
            return View(factura);
        }

        // POST: Facturas/facturas/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Edit([Bind(Include = "id,personId,userId,operationTypeId,cash,payCash,payCredit,payTdebit,payTcredit,totalDiscount,date,observation,stateId,facturaNumber,tipo")] factura factura)
        {
            if (ModelState.IsValid)
            {
                db.Entry(factura).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(factura);
        }

        [Authorize]
        public JsonResult verificarExistencia(int id, int cantidadPedida)
        {
            var productInOperation = (from pc in db.operation where pc.productId == id select pc).ToList();
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
            if (cantidad < cantidadPedida)
            {
                cantidad = 0;
            }

            return Json(cantidad, JsonRequestBehavior.AllowGet);
        }

        // GET: Facturas/facturas/Delete/5
        [Authorize]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            factura factura = db.factura.Find(id);
            if (factura == null)
            {
                return HttpNotFound();
            }
            return View(factura);
        }

        // POST: Facturas/facturas/Delete/5
        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            factura factura = db.factura.Find(id);
            db.factura.Remove(factura);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [Authorize]
        public JsonResult GetListFE()
        {//comentario de prueba
            using (var ctx = new AccountingContext())
            {

                var result = ctx.InvoiceFacturasElectronicas.ToList().Select(x => new ViewModelInvoiceFE
                {
                    Id = x.Id,
                    Prefijo = x.Prefijo,
                    Numero = x.Numero.ToString(),
                    Cufe = x.Cufe,
                    Fecha = x.Fecha.ToString("dd-MM-yyyy"),
                    Hora = x.Fecha.ToString("HH:mm:ss"),
                    Cliente = (x.ClienteFK != null) ? x.ClienteFK.name : "",
                    EstadoEnvioEmail = x.EstadoEnvioEmail
                }).ToList();

                var respuesta = new DTODataTables<ViewModelInvoiceFE>
                {
                    data = result
                };

                return Json(respuesta, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize]

        public ActionResult ListFacturasElectronicas()
        {
            return View();
        }

        [Authorize]
        public FileResult DescargarFEPdf(int id)
        {
            using (var ctx = new AccountingContext())
            {

                string NombreDocumento = "FE.pdf";
                var DatosFE = ctx.InvoiceFacturasElectronicas.Find(id);
                if (DatosFE.NombreDocumento != "" && DatosFE.NombreDocumento != null)
                {
                    NombreDocumento = DatosFE.NombreDocumento + ".pdf";
                }

                var rutas = System.AppDomain.CurrentDomain.BaseDirectory + "Certificado\\" + NombreDocumento;
                var ex = ".pdf";
                return File(rutas, "aplication/" + ex, NombreDocumento + ex);
            }
        }

        [Authorize(Roles = "Admin")]
        public ActionResult EditarFacturaCompra(string Id)
        {
            using (var ctx = new AccountingContext())
            {

                var items = ctx.operation.Where(x => x.facturaId == Id).ToList();
                var condicion = items.Where(x => x.operationTypeId == 20).ToList();
                if (condicion.Count == 0)
                {
                    foreach (var item in items)
                    {
                        var NuevaOperacion = new operation
                        {
                            productId = item.productId,
                            quantity = item.quantity,
                            operationTypeId = 20,
                            facturaId = item.facturaId,
                            date = item.date,
                            price = item.price,
                            discount = item.discount,
                            IvaId = item.IvaId,
                            userId = item.userId
                        };
                        ctx.operation.Add(NuevaOperacion);
                    }
                }



                var Factura = ctx.factura.Find(Id);

                var ListProveedores = new providersCController().GetProveedores().ToList().Select(x => new SelectListItem { Text = x.name, Value = x.id.ToString(), Selected = false });
                var ListIva = new ConsultasController().GetListIva().ToList().Select(x => new SelectListItem { Text = x.name, Value = x.id.ToString(), Selected = false });
                var ListFormasPago = new formasPagoController().GetListFormasPago().ToList().Select(x => new SelectListItem { Text = x.nombre, Value = x.id.ToString(), Selected = false });

                ViewBag.providers = ListProveedores;
                ViewBag.ListIva = ListIva;
                ViewBag.SelectProveedor = Factura.IdProveedor.ToString();
                ViewBag.FacturaFisica = Factura.facturaFisica;
                ViewBag.IdFactura = Id;
                ViewBag.ListFormasPago = ListFormasPago;
                ViewBag.SelectFormaPago = Factura.IdFormaPago;

                var products = db.products.ToList();

                ctx.SaveChanges();
                return View(products);
            }

        }

        [Authorize(Roles = "Admin")]
        public ActionResult GetEditandoFacuturaCompra(string IdFactura)
        {
            var userId = CurrentUser.Get.UserId;
            //lista de Costos Adicionales
            List<SelectListItem> pedidoContado = new List<SelectListItem>();   // Creo una lista
            var listaOperaciones = db.operation.Where(p => p.operationTypeId == 20 && p.facturaId == IdFactura).ToList();

            decimal SubTotal = 0;
            decimal Iva = 0;
            decimal total = 0;
            List<pedidosViewModel> enviarPedidosContado = new List<pedidosViewModel>();
            foreach (var item in listaOperaciones)
            {
                SubTotal = item.price * item.quantity;
                Iva = (SubTotal - item.discount) * Decimal.Divide(item.products.ivaFK.value, 100);
                total = SubTotal + Iva - item.discount;


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

        [Authorize(Roles = "Admin")]
        public JsonResult AddProductoEditarCompra(int id, int cantidad, decimal nuevoPrecio, int OD, string VD, string IvaId, string IdFactura)
        {
            var userId = CurrentUser.Get.UserId;
            decimal ValorDescuento = 0;
            if (OD == 1)
            {
                VD = VD.Replace(".", "");
                ValorDescuento = Convert.ToDecimal(VD);
                if (ValorDescuento > (nuevoPrecio * cantidad))
                {
                    ValorDescuento = nuevoPrecio * cantidad;
                }
            }
            else
            {
                decimal Porcentaje = decimal.Divide(Convert.ToInt32(VD), 100);
                ValorDescuento = Convert.ToInt32(nuevoPrecio * Porcentaje) * cantidad;
            }

            var nuevaOpercion = new operation()
            {
                productId = id,
                quantity = cantidad,
                operationTypeId = 20,
                date = DateTime.Now,
                price = nuevoPrecio,
                discount = ValorDescuento,
                IvaId = (IvaId != "") ? Convert.ToInt32(IvaId) : 0,
                userId = userId,
                facturaId = IdFactura
            };

            db.operation.Add(nuevaOpercion);
            db.SaveChanges();

            return Json(1, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Admin")]
        public JsonResult EliminarProductoEditarFC(int id)
        {
            operation operation = db.operation.Find(id);
            db.operation.Remove(operation);
            db.SaveChanges();

            return Json(1, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public JsonResult TerminarEditarFacturaCompra(string IdFactura, string facturaFisica, int provider, int IdFormaPago)
        {
            try
            {
                var Fecha = Time.FechaLocal.GetFechaColombia();
                var userId = CurrentUser.Get.UserId;
                var NuevaLista = db.operation.Where(p => p.operationTypeId == 20 && p.facturaId == IdFactura).ToList();
                var ListaActual = db.operation.Where(p => p.operationTypeId == 2 && p.facturaId == IdFactura).ToList();
                var Comprobante = (from pc in db.TipoComprobantes where pc.codigo == "CC3" select pc).Single();
                var FacturaEditar = db.factura.Find(IdFactura);
                var ComprobanteActual = db.comprobantes.Where(x => x.tipoComprobante == FacturaEditar.PrefijoComprobante && x.numero == FacturaEditar.NumeroComprobante).FirstOrDefault();
                var InfoPago = db.FormaPagos.Find(IdFormaPago);

                //actualizamos primero los productos si se cambió el valor de entrada y el iva
                foreach (var item in NuevaLista)
                {
                    var producto = (from pc in db.products where pc.id == item.productId select pc).Single();
                    producto.priceIn = item.price;
                    producto.ivaId = (item.IvaId != 0) ? Convert.ToInt32(item.IvaId) : producto.ivaId;
                    db.Entry(producto).State = System.Data.Entity.EntityState.Modified;
                }
                db.SaveChanges();

                //Disminuimos cantidad de los productos del listado actual
                foreach (var item in ListaActual)
                {
                    var producto = (from pc in db.products where pc.id == item.productId select pc).Single();
                    producto.initialQuantity -= item.quantity;
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

                foreach (var item in NuevaLista)
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
                decimal TotalDescuento = NuevaLista.Select(x => x.discount).Sum();

                #region modificar factura
                FacturaEditar.totalDiscount = TotalDescuento;
                FacturaEditar.total = totalCompra + iva19 + iva5;
                FacturaEditar.valorTotalExcentos = ComprasExcentos;
                FacturaEditar.valorTotalExcluidos = ComprasExcluidos;
                FacturaEditar.baseIVA19 = inventario19;
                FacturaEditar.baseIVA5 = inventario5;
                FacturaEditar.valorIVA19 = iva19;
                FacturaEditar.valorIVA5 = iva5;
                FacturaEditar.facturaFisica = facturaFisica;
                FacturaEditar.NumeroComprobante = Comprobante.consecutivo;
                FacturaEditar.IdProveedor = provider;

                db.Entry(FacturaEditar).State = System.Data.Entity.EntityState.Modified;
                #endregion

                foreach (var item in ListaActual)//eliminamos las operaciones actuales
                {

                    db.Entry(item).State = System.Data.Entity.EntityState.Deleted;

                }

                foreach (var item in NuevaLista)//registramos las nuevas operaciones
                {
                    var producto = (from pc in db.products where pc.id == item.productId select pc).FirstOrDefault();

                    producto.initialQuantity += item.quantity;
                    db.Entry(producto).State = System.Data.Entity.EntityState.Modified;

                    item.operationTypeId = 2;
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
                    valorTotal = totalCompra + iva19 + iva5,
                    anio = Fecha.Year,
                    mes = Fecha.Month,
                    dia = Fecha.Day,
                    fechaCreacion = Fecha,
                    usuarioId = userId,
                    documento = IdFactura,
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
                    credito = totalCompra + iva19 + iva5 - TotalDescuento,
                    baseMov = 0,
                    centroCostoId = 3,
                    fechaCreado = Fecha,
                    documento = IdFactura
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
                        documento = IdFactura
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
                        documento = IdFactura
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
                        documento = IdFactura
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
                        documento = IdFactura
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
                        documento = IdFactura
                    };
                    db.movimientos.Add(MovDescuento);
                }

                if (Movimientos19.Count > 0) { db.movimientos.AddRange(Movimientos19); }
                if (Movimientos5.Count > 0) { db.movimientos.AddRange(Movimientos5); }
                if (MovimientosExcentos.Count > 0) { db.movimientos.AddRange(MovimientosExcentos); }
                if (MovimientosExcluidos.Count > 0) { db.movimientos.AddRange(MovimientosExcluidos); }

                //anulamos el comprobante actual
                ComprobanteActual.estado = false;

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
        //comentario de prueba

        [Authorize]
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
