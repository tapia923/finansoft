using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PlanillajeColectivos.DTO;
using PlanillajeColectivos.DTO.Otros;

namespace PlanillajeColectivos.Areas.cashBack.Controllers
{
    //comentario
    public class cashBackController : Controller
    {
        private AccountingContext db = new AccountingContext();

        // GET: cashBack/cashBack
        public ActionResult Index()
        {
            return View(db.cashBackAcco.ToList());
        }

        public ActionResult Index2()
        {
            return View(db.cashBackAcco.ToList());
        }

        // GET: cashBack/cashBack/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            cashBackModel cashBackModel = db.cashBackAcco.Find(id);
            if (cashBackModel == null)
            {
                return HttpNotFound();
            }
            return View(cashBackModel);
        }

        // GET: cashBack/cashBack/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: cashBack/cashBack/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,terceroId,fechaInicio,fechaEntrega,valorEntregado,fechaEntregado,periodoEnMeses,valorActual,porcetaje,destino")] cashBackModel cashBackModel)
        {
            if (ModelState.IsValid)
            {
                db.cashBackAcco.Add(cashBackModel);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(cashBackModel);
        }

        // GET: cashBack/cashBack/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            cashBackModel cashBackModel = db.cashBackAcco.Find(id);
            if (cashBackModel == null)
            {
                return HttpNotFound();
            }
            return View(cashBackModel);
        }

        // POST: cashBack/cashBack/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,terceroId,fechaInicio,fechaEntrega,valorEntregado,fechaEntregado,periodoEnMeses,valorActual,porcetaje,destino")] cashBackModel cashBackModel)
        {
            if (ModelState.IsValid)
            {
                db.Entry(cashBackModel).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(cashBackModel);
        }

        // GET: cashBack/cashBack/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            cashBackModel cashBackModel = db.cashBackAcco.Find(id);
            if (cashBackModel == null)
            {
                return HttpNotFound();
            }
            return View(cashBackModel);
        }

        // POST: cashBack/cashBack/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            cashBackModel cashBackModel = db.cashBackAcco.Find(id);
            db.cashBackAcco.Remove(cashBackModel);
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
