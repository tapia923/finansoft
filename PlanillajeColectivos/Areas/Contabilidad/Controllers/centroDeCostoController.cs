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
    public class centroDeCostoController : Controller
    {
        private AccountingContext db = new AccountingContext();

        // GET: Contabilidad/centroDeCosto
        public ActionResult Index()
        {
            return View(db.centroCostos.ToList());
        }

        // GET: Contabilidad/centroDeCosto/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            centroCosto centroCosto = db.centroCostos.Find(id);
            if (centroCosto == null)
            {
                return HttpNotFound();
            }
            return View(centroCosto);
        }

        // GET: Contabilidad/centroDeCosto/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Contabilidad/centroDeCosto/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,codigo,nombre,activo")] centroCosto centroCosto)
        {
            if (ModelState.IsValid)
            {
                db.centroCostos.Add(centroCosto);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(centroCosto);
        }

        // GET: Contabilidad/centroDeCosto/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            centroCosto centroCosto = db.centroCostos.Find(id);
            if (centroCosto == null)
            {
                return HttpNotFound();
            }
            return View(centroCosto);
        }

        // POST: Contabilidad/centroDeCosto/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,codigo,nombre,activo")] centroCosto centroCosto)
        {
            if (ModelState.IsValid)
            {
                db.Entry(centroCosto).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(centroCosto);
        }

        // GET: Contabilidad/centroDeCosto/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            centroCosto centroCosto = db.centroCostos.Find(id);
            if (centroCosto == null)
            {
                return HttpNotFound();
            }
            return View(centroCosto);
        }

        // POST: Contabilidad/centroDeCosto/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            centroCosto centroCosto = db.centroCostos.Find(id);
            db.centroCostos.Remove(centroCosto);
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
