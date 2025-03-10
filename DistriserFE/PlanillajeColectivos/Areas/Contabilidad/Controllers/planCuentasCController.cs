using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PlanillajeColectivos.DTO;
using PlanillajeColectivos.DTO.Contabilidad;

namespace PlanillajeColectivos.Areas.Contabilidad.Controllers
{
    public class planCuentasCController : Controller
    {
        private AccountingContext db = new AccountingContext();

        //Contabilidad/planCuentasC/migrarMovs
        public void migrarMovs()
        {
            for (int i = 0; i < 4000; i++)
            {
                var primerMovCount = db.movimientosDos.Where(p => p.tipo == "ventaCaja" || p.tipo == "convenio").Count();

                if(primerMovCount > 0)
                {
                    var primerMov = db.movimientosDos.Where(p => p.tipo == "ventaCaja" || p.tipo == "convenio").First();
                    var porFactura = db.movimientosDos.Where(p => p.facturaId == primerMov.facturaId).ToList();
                    var bandera = 0;
                    var comprobante = (from pc in db.TipoComprobantes where pc.codigo == "CC1" select pc).Single();

                    foreach (var itemDos in porFactura)
                    {
                        if (bandera == 0)
                        {
                            decimal total = 0;
                            foreach (var itemTres in porFactura)
                            {
                                total = itemTres.debito + total;
                            }

                            //COMPROBANTE
                            var ComprobanteNew = new comprobante()
                            {
                                tipoComprobante = comprobante.codigo,
                                numero = comprobante.consecutivo,
                                centroCostoId = 3,
                                detalle = "Venta Caja",
                                terceroId = primerMov.terceroId,
                                valorTotal = total,
                                anio = primerMov.fecha.Year,
                                mes = primerMov.fecha.Month,
                                dia = primerMov.fecha.Day,
                                fechaCreacion = primerMov.fecha,
                                usuarioId = "709ef606-943e-49d2-8bb3-1e70ef01345e",
                                formaPagoId = 1,
                                documento = itemDos.facturaId.ToString()
                            };
                            db.comprobantes.Add(ComprobanteNew);
                            bandera = 1;
                        }
                        var elDetalle = "ventaCaja";
                        var cuentaReal = itemDos.codigoCuenta;
                        if (itemDos.tipo == "convenio")
                        {
                            elDetalle = "Convenio Coomisol";
                            cuentaReal = "26480501";

                            var mo2v = new movimientos()
                            {
                                tipoComprobante = comprobante.codigo,
                                numero = comprobante.consecutivo,
                                cuenta = "26080501",
                                terceroId = itemDos.terceroId,
                                detalle = elDetalle,
                                debito = 0,
                                credito = itemDos.debito,
                                baseMov = 0,
                                centroCostoId = 3,
                                fechaCreado = itemDos.fecha,
                                documento = itemDos.facturaId.ToString()
                            };
                            db.movimientos.Add(mo2v);
                        }

                        var mov = new movimientos()
                        {
                            tipoComprobante = comprobante.codigo,
                            numero = comprobante.consecutivo,
                            cuenta = cuentaReal,
                            terceroId = itemDos.terceroId,
                            detalle = elDetalle,
                            debito = itemDos.debito,
                            credito = itemDos.credito,
                            baseMov = 0,
                            centroCostoId = 3,
                            fechaCreado = itemDos.fecha,
                            documento = itemDos.facturaId.ToString()
                        };
                        db.movimientos.Add(mov);
                    }

                    var numCompr = Convert.ToInt32(comprobante.consecutivo);
                    comprobante.consecutivo = (1 + numCompr).ToString();
                    db.Entry(comprobante).State = System.Data.Entity.EntityState.Modified;

                    foreach (var elim in porFactura)
                    {
                        db.movimientosDos.Remove(elim);
                    }
                    db.SaveChanges();
                }
            }
            
        }

        // GET: Contabilidad/planCuentasC
        public ActionResult Index()
        {
            var movs = db.movimientos.ToList();
            var cuentas = db.planCuentas.ToList();
            decimal debito = 0;
            decimal credito = 0;
            //foreach (var item in cuentas)
            //{
            //    var cuenta = movs.Where(p => p.cuenta == item.codigo).ToList();
            //    debito = cuenta.Select(x => x.debito).Sum();
            //    credito = cuenta.Select(x => x.credito).Sum();
            //    if(item.naturaleza == "D")
            //    {
            //        item.saldo = debito - credito;
            //    }
            //    else
            //    {
            //        item.saldo = credito - debito;
            //    }
            //}
            movs = null;
            return View(cuentas);
        }

        // GET: Contabilidad/planCuentasC/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            planCuentas planCuentas = db.planCuentas.Find(id);
            if (planCuentas == null)
            {
                return HttpNotFound();
            }
            return View(planCuentas);
        }

        // GET: Contabilidad/planCuentasC/Create

        
        public ActionResult Create()
        {
            
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(planCuentas PCuenta)
        {
            if (!ModelState.IsValid)
                return View();

            try
            {
                using (AccountingContext db = new AccountingContext())
                {

                    db.planCuentas.Add(PCuenta);
                    db.SaveChanges();
                    return RedirectToAction("Index");

                }
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Error (¡Ya existe una cuenta con ese mismo CODIGO!) ");
                return View();
            }
        }

        // GET: Contabilidad/planCuentasC/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            planCuentas planCuentas = db.planCuentas.Find(id);
            if (planCuentas == null)
            {
                return HttpNotFound();
            }
            return View(planCuentas);
        }

        // POST: Contabilidad/planCuentasC/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,nombre,codigo,naturaleza,reqTercero,reqCosto,corriente,validaSaldo,saldo,tipo")] planCuentas planCuentas)
        {
            if (ModelState.IsValid)
            {
                db.Entry(planCuentas).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(planCuentas);
        }

        // GET: Contabilidad/planCuentasC/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            planCuentas planCuentas = db.planCuentas.Find(id);
            if (planCuentas == null)
            {
                return HttpNotFound();
            }
            return View(planCuentas);
        }

        // POST: Contabilidad/planCuentasC/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            planCuentas planCuentas = db.planCuentas.Find(id);
            db.planCuentas.Remove(planCuentas);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

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
