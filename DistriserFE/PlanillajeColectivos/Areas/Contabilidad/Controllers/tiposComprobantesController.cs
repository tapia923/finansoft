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
    public class tiposComprobantesController : Controller
    {
        private AccountingContext db = new AccountingContext();

        // GET: Contabilidad/tiposComprobantes
        public ActionResult Index()
        {
            return View(db.TipoComprobantes.ToList());
        }

        // GET: Contabilidad/tiposComprobantes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tipoComprobante tipoComprobante = db.TipoComprobantes.Find(id);
            if (tipoComprobante == null)
            {
                return HttpNotFound();
            }
            return View(tipoComprobante);
        }

        // GET: Contabilidad/tiposComprobantes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Contabilidad/tiposComprobantes/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,codigo,claseComprobante,nombre,consecutivo")] tipoComprobante tipoComprobante)
        {
            if (ModelState.IsValid)
            {
                db.TipoComprobantes.Add(tipoComprobante);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tipoComprobante);
        }

        // GET: Contabilidad/tiposComprobantes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tipoComprobante tipoComprobante = db.TipoComprobantes.Find(id);
            if (tipoComprobante == null)
            {
                return HttpNotFound();
            }
            return View(tipoComprobante);
        }

        // POST: Contabilidad/tiposComprobantes/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,codigo,claseComprobante,nombre,consecutivo")] tipoComprobante tipoComprobante)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tipoComprobante).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tipoComprobante);
        }

        // GET: Contabilidad/tiposComprobantes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tipoComprobante tipoComprobante = db.TipoComprobantes.Find(id);
            if (tipoComprobante == null)
            {
                return HttpNotFound();
            }
            return View(tipoComprobante);
        }

        // POST: Contabilidad/tiposComprobantes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tipoComprobante tipoComprobante = db.TipoComprobantes.Find(id);
            db.TipoComprobantes.Remove(tipoComprobante);
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
