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
    public class clasesDeComprobantesController : Controller
    {
        private AccountingContext db = new AccountingContext();

        // GET: Contabilidad/clasesDeComprobantes
        public ActionResult Index()
        {
            return View(db.ClaseComprobantes.ToList());
        }

        // GET: Contabilidad/clasesDeComprobantes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            claseComprobante claseComprobante = db.ClaseComprobantes.Find(id);
            if (claseComprobante == null)
            {
                return HttpNotFound();
            }
            return View(claseComprobante);
        }

        // GET: Contabilidad/clasesDeComprobantes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Contabilidad/clasesDeComprobantes/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,codigo,nombre")] claseComprobante claseComprobante)
        {
            if (ModelState.IsValid)
            {
                db.ClaseComprobantes.Add(claseComprobante);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(claseComprobante);
        }

        // GET: Contabilidad/clasesDeComprobantes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            claseComprobante claseComprobante = db.ClaseComprobantes.Find(id);
            if (claseComprobante == null)
            {
                return HttpNotFound();
            }
            return View(claseComprobante);
        }

        // POST: Contabilidad/clasesDeComprobantes/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,codigo,nombre")] claseComprobante claseComprobante)
        {
            if (ModelState.IsValid)
            {
                db.Entry(claseComprobante).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(claseComprobante);
        }

        // GET: Contabilidad/clasesDeComprobantes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            claseComprobante claseComprobante = db.ClaseComprobantes.Find(id);
            if (claseComprobante == null)
            {
                return HttpNotFound();
            }
            return View(claseComprobante);
        }

        // POST: Contabilidad/clasesDeComprobantes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            claseComprobante claseComprobante = db.ClaseComprobantes.Find(id);
            db.ClaseComprobantes.Remove(claseComprobante);
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
