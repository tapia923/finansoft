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
    public class formasPagoController : Controller
    {
        private AccountingContext db = new AccountingContext();

        // GET: Contabilidad/formasPago
        public ActionResult Index()
        {
            return View(db.FormaPagos.ToList());
        }

        // GET: Contabilidad/formasPago/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FormaPago formaPago = db.FormaPagos.Find(id);
            if (formaPago == null)
            {
                return HttpNotFound();
            }
            return View(formaPago);
        }

        // GET: Contabilidad/formasPago/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Contabilidad/formasPago/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,nombre")] FormaPago formaPago)
        {
            if (ModelState.IsValid)
            {
                db.FormaPagos.Add(formaPago);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(formaPago);
        }

        // GET: Contabilidad/formasPago/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FormaPago formaPago = db.FormaPagos.Find(id);
            if (formaPago == null)
            {
                return HttpNotFound();
            }
            return View(formaPago);
        }

        // POST: Contabilidad/formasPago/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,nombre")] FormaPago formaPago)
        {
            if (ModelState.IsValid)
            {
                db.Entry(formaPago).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(formaPago);
        }

        // GET: Contabilidad/formasPago/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FormaPago formaPago = db.FormaPagos.Find(id);
            if (formaPago == null)
            {
                return HttpNotFound();
            }
            return View(formaPago);
        }

        // POST: Contabilidad/formasPago/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            FormaPago formaPago = db.FormaPagos.Find(id);
            db.FormaPagos.Remove(formaPago);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [Authorize]
        public List<FormaPago> GetListFormasPago()
        {
            using (var ctx = new AccountingContext())
            {
                var data = ctx.FormaPagos.ToList();
                return data;
            }
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
