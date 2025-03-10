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
    public class comprobantesController : Controller
    {
        private AccountingContext db = new AccountingContext();

        // GET: Contabilidad/comprobantes
        public ActionResult Index()
        {
            return View(db.comprobantes.ToList());
        }

        // GET: Contabilidad/comprobantes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            comprobante comprobante = db.comprobantes.Find(id);
            if (comprobante == null)
            {
                return HttpNotFound();
            }
            return View(comprobante);
        }

        // GET: Contabilidad/comprobantes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Contabilidad/comprobantes/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,tipoComprobante,numero,centroCostoId,detalle,terceroId,valorTotal,anio,mes,dia,fechaCreacion,usuarioId,formaPagoId,documento")] comprobante comprobante)
        {
            if (ModelState.IsValid)
            {
                db.comprobantes.Add(comprobante);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(comprobante);
        }

        // GET: Contabilidad/comprobantes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            comprobante comprobante = db.comprobantes.Find(id);
            if (comprobante == null)
            {
                return HttpNotFound();
            }
            return View(comprobante);
        }

        // POST: Contabilidad/comprobantes/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,tipoComprobante,numero,centroCostoId,detalle,terceroId,valorTotal,anio,mes,dia,fechaCreacion,usuarioId,formaPagoId,documento")] comprobante comprobante)
        {
            if (ModelState.IsValid)
            {
                db.Entry(comprobante).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(comprobante);
        }

        // GET: Contabilidad/comprobantes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            comprobante comprobante = db.comprobantes.Find(id);
            if (comprobante == null)
            {
                return HttpNotFound();
            }
            return View(comprobante);
        }

        // POST: Contabilidad/comprobantes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            comprobante comprobante = db.comprobantes.Find(id);
            db.comprobantes.Remove(comprobante);
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
