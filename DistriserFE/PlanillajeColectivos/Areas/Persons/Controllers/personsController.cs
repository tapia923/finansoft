using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using PlanillajeColectivos.DTO;
using PlanillajeColectivos.DTO.Contabilidad;
using PlanillajeColectivos.DTO.Products;
using PlanillajeColectivos.DTO.Tipos;
using System.Data.Entity.Validation;
using System.Web.Http;
using System.Web.Mvc;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNetCore.Mvc;
using PlanillajeColectivos.DTO.Geolocalizacion;

namespace PlanillajeColectivos.Areas.Persons.Controllers
{
    public class personsController : Controller
    {
        private AccountingContext db = new AccountingContext();

        // GET: Persons/persons
        [System.Web.Mvc.Authorize(Roles = "Admin")]
        public ActionResult IndexProveedores()
        {
            return View(db.persons.ToList());
        }

        // GET: Persons/persons
        [System.Web.Mvc.Authorize]
        public ActionResult Index()
        {
            return View(db.persons.ToList());
        }

        // GET: Persons/persons/Details/5
        [System.Web.Mvc.Authorize]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            persons persons = db.persons.Find(id);
            if (persons == null)
            {
                return HttpNotFound();
            }
            return View(persons);
        }

        // GET: Persons/persons/Create
        [System.Web.Mvc.Authorize]
        public ActionResult Create()
        {
            //inicio select list regimenFiscal
            List<SelectListItem> fiscal = new List<SelectListItem>();
            fiscal.Add(new SelectListItem { Text = "--Seleccione--", Value = "" });
            IList<RegimenFiscal> listadoFiscal = db.RegimenFiscal.ToList();
            foreach (var item in listadoFiscal)	
            {
                fiscal.Add(new SelectListItem { Text = item.descripcion, Value = item.id.ToString() }); 
            }
            //fin select list para regimen fiscal

            //inicio select list tipo contribuyente
            List<SelectListItem> contribuyente = new List<SelectListItem>();
            contribuyente.Add(new SelectListItem { Text = "--Seleccione--", Value = "" });
            IList<TipoContribuyente> listadoContribuyente = db.TipoContribuyente.ToList();
            foreach (var item in listadoContribuyente)
            {
                contribuyente.Add(new SelectListItem { Text = item.descripcion, Value = item.id.ToString() });
            }
            //fin select list para tipo contribuyente

            //inicio select list pais
            List<SelectListItem> pais = new List<SelectListItem>();
            pais.Add(new SelectListItem { Text = "--Seleccione--", Value = "" });
            IList<Pais> listadoPais = db.Pais.ToList();
            foreach (var item in listadoPais)
            {
                pais.Add(new SelectListItem { Text = item.Nom_pais, Value = item.Id_pais.ToString() });
            }
            //-----------------------------------


            ViewBag.fiscal = fiscal;
            ViewBag.contribuyente = contribuyente;
            ViewBag.pais = pais;


            return View();
        }

        // POST: Persons/persons/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Create([Bind(Include = "id,tipeId,name,nit,direccion,celular,telefono,email,cupocredito,regime,numeroTarjeta,regimenFiscal,tipoContribuyente,municipio")] persons persons)
        {
            if (ModelState.IsValid)
            {
                persons.tipeId = 1;
                persons.responsabilidadFiscal = "R-99-PN";
                
                db.persons.Add(persons);
                db.SaveChanges();
                return RedirectToAction("Index");
            }


            //inicio select list regimenFiscal
            List<SelectListItem> fiscal = new List<SelectListItem>();
            fiscal.Add(new SelectListItem { Text = "--Seleccione--", Value = "" });
            IList<RegimenFiscal> listadoFiscal = db.RegimenFiscal.ToList();
            foreach (var item in listadoFiscal)
            {
                fiscal.Add(new SelectListItem { Text = item.descripcion, Value = item.id.ToString() });
            }
            //fin select list para rutas

            //inicio select list regimenFiscal
            List<SelectListItem> contribuyente = new List<SelectListItem>();
            contribuyente.Add(new SelectListItem { Text = "--Seleccione--", Value = "" });
            IList<TipoContribuyente> listadoContribuyente = db.TipoContribuyente.ToList();
            foreach (var item in listadoContribuyente)
            {
                contribuyente.Add(new SelectListItem { Text = item.descripcion, Value = item.id.ToString() });
            }
            //fin select list para rutas

            //inicio select list pais
            List<SelectListItem> pais = new List<SelectListItem>();
            pais.Add(new SelectListItem { Text = "--Seleccione--", Value = "" });
            IList<Pais> listadoPais = db.Pais.ToList();
            foreach (var item in listadoPais)
            {
                pais.Add(new SelectListItem { Text = item.Nom_pais, Value = item.Id_pais.ToString() });
            }
            //-----------------------------------

            ViewBag.fiscal = fiscal;
            ViewBag.contribuyente = contribuyente;
            ViewBag.pais = pais;

            return View(persons);
        }

        // GET: Persons/persons/Edit/5
        [Authorize]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            persons persons = db.persons.Find(id);
            if (persons == null)
            {
                return HttpNotFound();
            }


            //inicio select list regimenFiscal
            List<SelectListItem> fiscal = new List<SelectListItem>();
            fiscal.Add(new SelectListItem { Text = "--Seleccione--", Value = "" });
            IList<RegimenFiscal> listadoFiscal = db.RegimenFiscal.ToList();
            foreach (var item in listadoFiscal)
            {
                fiscal.Add(new SelectListItem { Text = item.descripcion, Value = item.id.ToString() });
            }
            //fin select list para rutas

            //inicio select list regimenFiscal
            List<SelectListItem> contribuyente = new List<SelectListItem>();
            contribuyente.Add(new SelectListItem { Text = "--Seleccione--", Value = "" });
            IList<TipoContribuyente> listadoContribuyente = db.TipoContribuyente.ToList();
            foreach (var item in listadoContribuyente)
            {
                contribuyente.Add(new SelectListItem { Text = item.descripcion, Value = item.id.ToString() });
            }
            //fin select list para rutas

            //inicio select list pais
            List<SelectListItem> pais = new List<SelectListItem>();
            pais.Add(new SelectListItem { Text = "--Seleccione--", Value = "" });
            IList<Pais> listadoPais = db.Pais.ToList();
            foreach (var item in listadoPais)
            {
                pais.Add(new SelectListItem { Text = item.Nom_pais, Value = item.Id_pais.ToString() });
            }
            //-----------------------------------

            ViewBag.fiscal = fiscal;
            ViewBag.contribuyente = contribuyente;
            ViewBag.pais = pais;
            

            return View(persons);
        }

        // POST: Persons/persons/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        
        [Authorize]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Edit([Bind(Include = "id,tipeId,name,nit,direccion,celular,telefono,email,cupocredito,regime,numeroTarjeta,regimenFiscal,tipoContribuyente,municipio")] persons persons)
        {
            if (ModelState.IsValid)
            {
                persons.tipeId = 0;
                db.Entry(persons).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }


            //inicio select list regimenFiscal
            List<SelectListItem> fiscal = new List<SelectListItem>();
            fiscal.Add(new SelectListItem { Text = "--Seleccione--", Value = "" });
            IList<RegimenFiscal> listadoFiscal = db.RegimenFiscal.ToList();
            foreach (var item in listadoFiscal)
            {
                fiscal.Add(new SelectListItem { Text = item.descripcion, Value = item.id.ToString() });
            }
            //fin select list para rutas

            //inicio select list regimenFiscal
            List<SelectListItem> contribuyente = new List<SelectListItem>();
            contribuyente.Add(new SelectListItem { Text = "--Seleccione--", Value = "" });
            IList<TipoContribuyente> listadoContribuyente = db.TipoContribuyente.ToList();
            foreach (var item in listadoContribuyente)
            {
                contribuyente.Add(new SelectListItem { Text = item.descripcion, Value = item.id.ToString() });
            }
            //fin select list para rutas

            //inicio select list pais
            List<SelectListItem> pais = new List<SelectListItem>();
            pais.Add(new SelectListItem { Text = "--Seleccione--", Value = "" });
            IList<Pais> listadoPais = db.Pais.ToList();
            foreach (var item in listadoPais)
            {
                pais.Add(new SelectListItem { Text = item.Nom_pais, Value = item.Id_pais.ToString() });
            }
            //-----------------------------------

            ViewBag.fiscal = fiscal;
            ViewBag.contribuyente = contribuyente;
            ViewBag.pais = pais;

            return View(persons);
        }

        // GET: Persons/persons/Delete/5
        [Authorize]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            persons persons = db.persons.Find(id);
            if (persons == null)
            {
                return HttpNotFound();
            }
            return View(persons);
        }

        // POST: Persons/persons/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult DeleteConfirmed(int id)
        {
            persons persons = db.persons.Find(id);
            db.persons.Remove(persons);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

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
