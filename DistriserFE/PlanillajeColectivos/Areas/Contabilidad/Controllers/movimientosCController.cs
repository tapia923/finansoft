using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Common;
using PlanillajeColectivos.DTO;
using PlanillajeColectivos.DTO.Contabilidad;
using PlanillajeColectivos.DTO.Products;

namespace PlanillajeColectivos.Areas.Contabilidad.Controllers
{
    public class movimientosCController : Controller
    {
        private AccountingContext db = new AccountingContext();

        // GET: Contabilidad/movimientosC
        public ActionResult Index()
        {
            return View(db.comprobantes.Where(x => x.estado).ToList());
        }

        public ActionResult verMovimiento()
        {
            return View();
        }

        // GET: Contabilidad/movimientosC/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            movimientos movimientos = db.movimientos.Find(id);
            if (movimientos == null)
            {
                return HttpNotFound();
            }
            return View(movimientos);
        }

        public ActionResult verComprobante(int? id)
        {
            string tipoComprobante = "";


            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            comprobante comprobante = db.comprobantes.Find(id);
            if (comprobante == null)
            {
                return HttpNotFound();
            }

            var movimientos = db.movimientos.Where(x => x.tipoComprobante == comprobante.tipoComprobante && x.numero == comprobante.numero).ToList();
            tipoComprobante = db.TipoComprobantes.Where(x => x.codigo == comprobante.tipoComprobante).Select(x => x.nombre).FirstOrDefault();
            
            ViewBag.movimientos = movimientos;
             ViewBag.tipoComprobante = tipoComprobante;
            return View(comprobante);
        }

        public ActionResult GetCuentasParaSelect2(string term = "")
        {
            var entities = new AccountingContext().planCuentas.Where(x => (x.tipo == "AX") && ((x.Nombre.Contains(term)) || (x.codigo.Contains(term)))).ToList();
            return Json(new { results = entities.Select(x => new { id = x.codigo, text = x.Nombre }) }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetTercerosSelect2(string term = "")
        {
            var entities = new AccountingContext().persons.Where(x => x.name.Contains(term) || x.nit.Contains(term)).ToList();
            return Json(new { results = entities.Select(x => new { id = x.id, text = x.name }) }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetCCSelect2(string term = "")
        {
            var entities = new AccountingContext().centroCostos.Where(x => x.nombre.Contains(term) || x.codigo.Contains(term)).ToList();
            return Json(new { results = entities.Select(x => new { id = x.id, text = x.nombre }) }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult crearMovimiento(Array[] movs, Array otrosDatos)
        {
            var userId = CurrentUser.Get.UserId;
            var fecha = otrosDatos.GetValue(0).ToString();
            var fechaFormateada = Convert.ToDateTime(fecha);


            var tipoComprobante = Convert.ToInt32(otrosDatos.GetValue(1));
            var formaPago = Convert.ToInt32(otrosDatos.GetValue(2));
            var detalle = otrosDatos.GetValue(3).ToString();
            decimal total = Convert.ToDecimal(otrosDatos.GetValue(4));

            int centroCostosId = Convert.ToInt32(otrosDatos.GetValue(5));
            var terceroId = Convert.ToInt32(otrosDatos.GetValue(6));

            //COMPROBANTE
            var comprobante = (from pc in db.TipoComprobantes where pc.id == tipoComprobante select pc).Single();
            var ComprobanteNew = new comprobante()
            {
                tipoComprobante = comprobante.codigo,
                numero = comprobante.consecutivo,
                centroCostoId = centroCostosId,
                detalle = detalle,
                terceroId = terceroId,
                valorTotal = total,
                anio = fechaFormateada.Year,
                mes = fechaFormateada.Month,
                dia = fechaFormateada.Day,
                fechaCreacion = DateTime.Now,
                usuarioId = userId,
                formaPagoId = formaPago,
                estado = true
            };
            db.comprobantes.Add(ComprobanteNew);



            foreach (Array item in movs)
            {
                var mov = new movimientos()
                {
                    tipoComprobante = comprobante.codigo,
                    numero = comprobante.consecutivo,
                    cuenta = item.GetValue(0).ToString(),
                    terceroId = Convert.ToInt32(item.GetValue(1)),
                    detalle = detalle,
                    debito = Convert.ToDecimal(item.GetValue(4).ToString()),
                    credito = Convert.ToDecimal(item.GetValue(5).ToString()),
                    baseMov = Convert.ToDecimal(item.GetValue(3).ToString()),
                    centroCostoId = Convert.ToInt32(item.GetValue(2)),
                    fechaCreado = DateTime.Now
                };
                db.movimientos.Add(mov);
            }

            var numCompr = Convert.ToInt32(comprobante.consecutivo);
            comprobante.consecutivo = (1 + numCompr).ToString();
            db.Entry(comprobante).State = System.Data.Entity.EntityState.Modified;

            db.SaveChanges();
            return Json("si", JsonRequestBehavior.AllowGet);
        }

        // GET: Contabilidad/movimientosC/Create
        public ActionResult Create()
        {
            List<SelectListItem> FormasPago = new List<SelectListItem>();   // Creo una lista
            FormasPago.Add(new SelectListItem { Text = "Seleccione Forma Pago", Value = "" });
            IList<FormaPago> ListaDeFormasPago = db.FormaPagos.ToList();// extraigo los elementos desde la DB

            foreach (var item in ListaDeFormasPago)		// recorro los elementos de la db
            {
                FormasPago.Add(new SelectListItem { Text = item.nombre, Value = item.id.ToString() });  // agrego los elementos de la db a la primera lista que cree
            }

            ViewBag.FormasPago = FormasPago;

            List<SelectListItem> tiposComprobantes = new List<SelectListItem>();   // Creo una lista
            tiposComprobantes.Add(new SelectListItem { Text = "Seleccione Tipo Comprobante", Value = "" });
            IList<tipoComprobante> ListaDeTiposComprobantes = db.TipoComprobantes.ToList();// extraigo los elementos desde la DB

            foreach (var item in ListaDeTiposComprobantes)		// recorro los elementos de la db
            {
                tiposComprobantes.Add(new SelectListItem { Text = item.nombre + " || " + item.codigo, Value = item.id.ToString() });  // agrego los elementos de la db a la primera lista que cree
            }

            ViewBag.tiposComprobantes = tiposComprobantes;

            List<SelectListItem> terceros = new List<SelectListItem>();   // Creo una lista
            terceros.Add(new SelectListItem { Text = "Seleccione Tercero", Value = "" });
            IList<persons> ListaDeTerceros = db.persons.ToList();// extraigo los elementos desde la DB

            foreach (var item in ListaDeTerceros)		// recorro los elementos de la db
            {
                terceros.Add(new SelectListItem { Text = item.name + " || " + item.nit, Value = item.id.ToString() });  // agrego los elementos de la db a la primera lista que cree
            }

            ViewBag.terceroComprobante = terceros;

            List<SelectListItem> centroDeCostos = new List<SelectListItem>();   // Creo una lista
            centroDeCostos.Add(new SelectListItem { Text = "Seleccione Centro de Costos", Value = "" });
            IList<centroCosto> ListaCentroDeCostos = db.centroCostos.ToList();// extraigo los elementos desde la DB

            foreach (var item in ListaCentroDeCostos)		// recorro los elementos de la db
            {
                centroDeCostos.Add(new SelectListItem { Text = item.codigo + " || " + item.nombre, Value = item.id.ToString() });  // agrego los elementos de la db a la primera lista que cree
            }

            ViewBag.centroDeCostos = centroDeCostos;

            return View();
        }

        // POST: Contabilidad/movimientosC/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,tipoComprobante,numero,cuenta,terceroId,detalle,debito,credito,baseMov,centroCostoId,fechaCreado,documento")] movimientos movimientos)
        {
            if (ModelState.IsValid)
            {
                db.movimientos.Add(movimientos);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(movimientos);
        }

        // GET: Contabilidad/movimientosC/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            movimientos movimientos = db.movimientos.Find(id);
            if (movimientos == null)
            {
                return HttpNotFound();
            }
            return View(movimientos);
        }

        // POST: Contabilidad/movimientosC/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,tipoComprobante,numero,cuenta,terceroId,detalle,debito,credito,baseMov,centroCostoId,fechaCreado,documento")] movimientos movimientos)
        {
            if (ModelState.IsValid)
            {
                db.Entry(movimientos).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(movimientos);
        }

        // GET: Contabilidad/movimientosC/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            movimientos movimientos = db.movimientos.Find(id);
            if (movimientos == null)
            {
                return HttpNotFound();
            }
            return View(movimientos);
        }

        // POST: Contabilidad/movimientosC/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            movimientos movimientos = db.movimientos.Find(id);
            db.movimientos.Remove(movimientos);
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
