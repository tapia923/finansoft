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
using PlanillajeColectivos.DTO.Geolocalizacion;
using PlanillajeColectivos.DTO.Products;

namespace PlanillajeColectivos.Areas.Persons.Controllers
{
    public class providersCController : Controller
    {
        private AccountingContext db = new AccountingContext();

        // GET: Persons/providersC
        public ActionResult Index()
        {
            return View(db.providers.ToList());
        }

        // GET: Persons/providersC/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            providers providers = db.providers.Find(id);
            if (providers == null)
            {
                return HttpNotFound();
            }
            return View(providers);
        }

        // GET: Persons/providersC/Create
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            List<SelectListItem> items = new List<SelectListItem>();   // Creo una lista
            items.Add(new SelectListItem { Text = "--SELECCIONE--", Value = "0" });
            IList<economicActivity> lista = db.economicActivities.ToList();// extraigo los elementos desde la DB

            foreach (var item in lista)		// recorro los elementos de la db
            {
                items.Add(new SelectListItem { Text = item.description + "|" + item.activityCode, Value = item.activityCode.ToString() });  // agrego los elementos de la db a la primera lista que cree
            }

            ViewBag.economicActivities = items;

            List<SelectListItem> items1 = new List<SelectListItem>();   // Creo una lista
            items1.Add(new SelectListItem { Text = "--SELECCIONE--", Value = "0" });
            IList<location> lista1 = db.locations.ToList();// extraigo los elementos desde la DB

            foreach (var item in lista1)		// recorro los elementos de la db
            {
                items1.Add(new SelectListItem { Text = item.name, Value = item.id.ToString() });  // agrego los elementos de la db a la primera lista que cree
            }

            ViewBag.locations = items1;

            List<SelectListItem> items2 = new List<SelectListItem>();   // Creo una lista
            items2.Add(new SelectListItem { Text = "--SELECCIONE--", Value = "0" });
            IList<retention> lista2 = db.retentions.ToList();// extraigo los elementos desde la DB

            foreach (var item in lista2)		// recorro los elementos de la db
            {
                items2.Add(new SelectListItem { Text = item.description, Value = item.id.ToString() });  // agrego los elementos de la db a la primera lista que cree
            }

            ViewBag.retentions = items2;

            List<SelectListItem> items3 = new List<SelectListItem>();   // Creo una lista
            items3.Add(new SelectListItem { Text = "--SELECCIONE--", Value = "0" });
            items3.Add(new SelectListItem { Text = "Declarante de Renta", Value = "1" });
            items3.Add(new SelectListItem { Text = "No Declarante de Renta", Value = "2" });

            ViewBag.taxClassification = items3;

            ViewBag.retentions = items2;

            List<SelectListItem> items4 = new List<SelectListItem>();   // Creo una lista
            items4.Add(new SelectListItem { Text = "Autoretenedor", Value = "1" });
            items4.Add(new SelectListItem { Text = "No Autoretenedor", Value = "2" });

            ViewBag.industryAndCommerceList = items4;

            List<SelectListItem> items5 = new List<SelectListItem>();   // Creo una lista
            items5.Add(new SelectListItem { Text = "Responsable de IVA", Value = "1" });
            items5.Add(new SelectListItem { Text = "No Responsable de IVA", Value = "2" });
            items5.Add(new SelectListItem { Text = "Gran Contribuyente", Value = "3" });

            ViewBag.iva = items5;

            //inicio select list pais
            List<SelectListItem> pais = new List<SelectListItem>();
            pais.Add(new SelectListItem { Text = "--Seleccione--", Value = "" });
            IList<Pais> listadoPais = db.Pais.ToList();
            foreach (var item in listadoPais)
            {
                pais.Add(new SelectListItem { Text = item.Nom_pais, Value = item.Id_pais.ToString() });
            }
            //-----------------------------------

            ViewBag.pais = pais;

            return View();
        }

        // POST: Persons/providersC/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,economicActivityCode,locationId,retentionId,taxClassification,industryAndCommerce,ivaId,name,nit,address,cell,phone,email,municipio")] providers providers)
        {
            if (ModelState.IsValid)
            {
                db.providers.Add(providers);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            List<SelectListItem> items = new List<SelectListItem>();   // Creo una lista
            items.Add(new SelectListItem { Text = "--SELECCIONE--", Value = "0" });
            IList<economicActivity> lista = db.economicActivities.ToList();// extraigo los elementos desde la DB

            foreach (var item in lista)		// recorro los elementos de la db
            {
                items.Add(new SelectListItem { Text = item.description + "|" + item.activityCode, Value = item.activityCode.ToString() });  // agrego los elementos de la db a la primera lista que cree
            }

            ViewBag.economicActivities = items;

            List<SelectListItem> items1 = new List<SelectListItem>();   // Creo una lista
            items1.Add(new SelectListItem { Text = "--SELECCIONE--", Value = "0" });
            IList<location> lista1 = db.locations.ToList();// extraigo los elementos desde la DB

            foreach (var item in lista1)		// recorro los elementos de la db
            {
                items1.Add(new SelectListItem { Text = item.name, Value = item.id.ToString() });  // agrego los elementos de la db a la primera lista que cree
            }

            ViewBag.locations = items1;

            List<SelectListItem> items2 = new List<SelectListItem>();   // Creo una lista
            items2.Add(new SelectListItem { Text = "--SELECCIONE--", Value = "0" });
            IList<retention> lista2 = db.retentions.ToList();// extraigo los elementos desde la DB

            foreach (var item in lista2)		// recorro los elementos de la db
            {
                items2.Add(new SelectListItem { Text = item.description, Value = item.id.ToString() });  // agrego los elementos de la db a la primera lista que cree
            }

            ViewBag.retentions = items2;

            List<SelectListItem> items3 = new List<SelectListItem>();   // Creo una lista
            items3.Add(new SelectListItem { Text = "--SELECCIONE--", Value = "0" });
            items3.Add(new SelectListItem { Text = "Declarante de Renta", Value = "1" });
            items3.Add(new SelectListItem { Text = "No Declarante de Renta", Value = "2" });

            ViewBag.taxClassification = items3;

            ViewBag.retentions = items2;

            List<SelectListItem> items4 = new List<SelectListItem>();   // Creo una lista
            items4.Add(new SelectListItem { Text = "Autoretenedor", Value = "1" });
            items4.Add(new SelectListItem { Text = "No Autoretenedor", Value = "2" });

            ViewBag.industryAndCommerceList = items4;

            List<SelectListItem> items5 = new List<SelectListItem>();   // Creo una lista
            items5.Add(new SelectListItem { Text = "Responsable de IVA", Value = "1" });
            items5.Add(new SelectListItem { Text = "No Responsable de IVA", Value = "2" });
            items5.Add(new SelectListItem { Text = "Gran Contribuyente", Value = "3" });

            ViewBag.iva = items5;

            //inicio select list pais
            List<SelectListItem> pais = new List<SelectListItem>();
            pais.Add(new SelectListItem { Text = "--Seleccione--", Value = "" });
            IList<Pais> listadoPais = db.Pais.ToList();
            foreach (var item in listadoPais)
            {
                pais.Add(new SelectListItem { Text = item.Nom_pais, Value = item.Id_pais.ToString() });
            }
            //-----------------------------------

            ViewBag.pais = pais;

            return View(providers);
        }

        // GET: Persons/providersC/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            providers providers = db.providers.Find(id);
            if (providers == null)
            {
                return HttpNotFound();
            }



            List<SelectListItem> items = new List<SelectListItem>();   // Creo una lista
            items.Add(new SelectListItem { Text = "--SELECCIONE--", Value = "0" });
            IList<economicActivity> lista = db.economicActivities.ToList();// extraigo los elementos desde la DB

            foreach (var item in lista)		// recorro los elementos de la db
            {
                items.Add(new SelectListItem { Text = item.description + "|" + item.activityCode, Value = item.activityCode.ToString() });  // agrego los elementos de la db a la primera lista que cree
            }

            ViewBag.economicActivities = items;

            List<SelectListItem> items1 = new List<SelectListItem>();   // Creo una lista
            items1.Add(new SelectListItem { Text = "--SELECCIONE--", Value = "0" });
            IList<location> lista1 = db.locations.ToList();// extraigo los elementos desde la DB

            foreach (var item in lista1)		// recorro los elementos de la db
            {
                items1.Add(new SelectListItem { Text = item.name, Value = item.id.ToString() });  // agrego los elementos de la db a la primera lista que cree
            }

            ViewBag.locations = items1;

            List<SelectListItem> items2 = new List<SelectListItem>();   // Creo una lista
            items2.Add(new SelectListItem { Text = "--SELECCIONE--", Value = "0" });
            IList<retention> lista2 = db.retentions.ToList();// extraigo los elementos desde la DB

            foreach (var item in lista2)		// recorro los elementos de la db
            {
                items2.Add(new SelectListItem { Text = item.description, Value = item.id.ToString() });  // agrego los elementos de la db a la primera lista que cree
            }

            ViewBag.retentions = items2;

            List<SelectListItem> items3 = new List<SelectListItem>();   // Creo una lista
            items3.Add(new SelectListItem { Text = "--SELECCIONE--", Value = "0" });
            items3.Add(new SelectListItem { Text = "Declarante de Renta", Value = "1" });
            items3.Add(new SelectListItem { Text = "No Declarante de Renta", Value = "2" });

            ViewBag.taxClassification = items3;

            ViewBag.retentions = items2;

            List<SelectListItem> items4 = new List<SelectListItem>();   // Creo una lista
            items4.Add(new SelectListItem { Text = "Autoretenedor", Value = "1" });
            items4.Add(new SelectListItem { Text = "No Autoretenedor", Value = "2" });

            ViewBag.industryAndCommerceList = items4;

            List<SelectListItem> items5 = new List<SelectListItem>();   // Creo una lista
            items5.Add(new SelectListItem { Text = "Responsable de IVA", Value = "1" });
            items5.Add(new SelectListItem { Text = "No Responsable de IVA", Value = "2" });
            items5.Add(new SelectListItem { Text = "Gran Contribuyente", Value = "3" });

            ViewBag.iva = items5;

            //inicio select list pais
            List<SelectListItem> pais = new List<SelectListItem>();
            pais.Add(new SelectListItem { Text = "--Seleccione--", Value = "" });
            IList<Pais> listadoPais = db.Pais.ToList();
            foreach (var item in listadoPais)
            {
                pais.Add(new SelectListItem { Text = item.Nom_pais, Value = item.Id_pais.ToString() });
            }
            //-----------------------------------

            ViewBag.pais = pais;


            return View(providers);
        }

        // POST: Persons/providersC/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,economicActivityCode,locationId,retentionId,taxClassification,industryAndCommerce,ivaId,name,nit,address,cell,phone,email,municipio")] providers providers)
        {
            if (ModelState.IsValid)
            {
                db.Entry(providers).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            List<SelectListItem> items = new List<SelectListItem>();   // Creo una lista
            items.Add(new SelectListItem { Text = "--SELECCIONE--", Value = "0" });
            IList<economicActivity> lista = db.economicActivities.ToList();// extraigo los elementos desde la DB

            foreach (var item in lista)		// recorro los elementos de la db
            {
                items.Add(new SelectListItem { Text = item.description + "|" + item.activityCode, Value = item.activityCode.ToString() });  // agrego los elementos de la db a la primera lista que cree
            }

            ViewBag.economicActivities = items;

            List<SelectListItem> items1 = new List<SelectListItem>();   // Creo una lista
            items1.Add(new SelectListItem { Text = "--SELECCIONE--", Value = "0" });
            IList<location> lista1 = db.locations.ToList();// extraigo los elementos desde la DB

            foreach (var item in lista1)		// recorro los elementos de la db
            {
                items1.Add(new SelectListItem { Text = item.name, Value = item.id.ToString() });  // agrego los elementos de la db a la primera lista que cree
            }

            ViewBag.locations = items1;

            List<SelectListItem> items2 = new List<SelectListItem>();   // Creo una lista
            items2.Add(new SelectListItem { Text = "--SELECCIONE--", Value = "0" });
            IList<retention> lista2 = db.retentions.ToList();// extraigo los elementos desde la DB

            foreach (var item in lista2)		// recorro los elementos de la db
            {
                items2.Add(new SelectListItem { Text = item.description, Value = item.id.ToString() });  // agrego los elementos de la db a la primera lista que cree
            }

            ViewBag.retentions = items2;

            List<SelectListItem> items3 = new List<SelectListItem>();   // Creo una lista
            items3.Add(new SelectListItem { Text = "--SELECCIONE--", Value = "0" });
            items3.Add(new SelectListItem { Text = "Declarante de Renta", Value = "1" });
            items3.Add(new SelectListItem { Text = "No Declarante de Renta", Value = "2" });

            ViewBag.taxClassification = items3;

            ViewBag.retentions = items2;

            List<SelectListItem> items4 = new List<SelectListItem>();   // Creo una lista
            items4.Add(new SelectListItem { Text = "Autoretenedor", Value = "1" });
            items4.Add(new SelectListItem { Text = "No Autoretenedor", Value = "2" });

            ViewBag.industryAndCommerceList = items4;

            List<SelectListItem> items5 = new List<SelectListItem>();   // Creo una lista
            items5.Add(new SelectListItem { Text = "Responsable de IVA", Value = "1" });
            items5.Add(new SelectListItem { Text = "No Responsable de IVA", Value = "2" });
            items5.Add(new SelectListItem { Text = "Gran Contribuyente", Value = "3" });

            ViewBag.iva = items5;

            //inicio select list pais
            List<SelectListItem> pais = new List<SelectListItem>();
            pais.Add(new SelectListItem { Text = "--Seleccione--", Value = "" });
            IList<Pais> listadoPais = db.Pais.ToList();
            foreach (var item in listadoPais)
            {
                pais.Add(new SelectListItem { Text = item.Nom_pais, Value = item.Id_pais.ToString() });
            }
            //-----------------------------------

            ViewBag.pais = pais;

            return View(providers);
        }

        // GET: Persons/providersC/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            providers providers = db.providers.Find(id);
            if (providers == null)
            {
                return HttpNotFound();
            }
            return View(providers);
        }

        // POST: Persons/providersC/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            providers providers = db.providers.Find(id);
            db.providers.Remove(providers);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public List<providers> GetProveedores()
        {
            using(var ctx = new AccountingContext())
            {
                var data = ctx.providers.ToList();
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
