using PlanillajeColectivos.DTO;
using PlanillajeColectivos.DTO.Products;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace PlanillajeColectivos.Areas.Products.Controllers
{
    public class productsController : Controller
    {
        private AccountingContext db = new AccountingContext();
        private ContextTwo cxt = new ContextTwo();

        // GET: Products/products/colocarCodigoBarras
        [Authorize]
        public ActionResult colocarCodigoBarras()
        {
            var personas = db.persons.ToList();
            foreach (var item in personas)
            {
                var otrabase = (from pc in cxt.cuposCredito where pc.NIT == item.nit select pc).Count();
                if (otrabase != 0)
                {
                    var otrabaseRegistro = (from pc in cxt.cuposCredito where pc.NIT == item.nit select pc).First();
                    item.numeroTarjeta = otrabaseRegistro.numeroTarjeta;
                    db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
            }
            return View();
        }

        [Authorize]
        public ActionResult Index()
        {
            var products = db.products.ToList();
            var operaciones = db.operation.ToList();

           
            return View(products);
        }

        // GET: Products/products/Details/5
        [Authorize]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            products products = db.products.Find(id);
            if (products == null)
            {
                return HttpNotFound();
            }
            return View(products);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult cambioPrecios()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        public JsonResult cambioPreciosPost(string porcentajeContado, string porcentajeCredito)
        {
            var productsAll = db.products.ToList();
            foreach (var item in productsAll)
            {
                item.priceOut = item.priceIn / Convert.ToDecimal(porcentajeContado);
                item.priceOut2 = item.priceIn / Convert.ToDecimal(porcentajeCredito);
                db.Entry(item).State = System.Data.Entity.EntityState.Modified;
            }
            db.SaveChanges();
            return Json(1, JsonRequestBehavior.AllowGet);
        }

        // GET: Products/products/Create
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            //inicio select list categorias
            List<SelectListItem> categorias = new List<SelectListItem>();
            categorias.Add(new SelectListItem { Text = "--Seleccione--", Value = "" });
            IList<categorias> listadoCategorias = db.categorias.ToList();
            foreach (var item in listadoCategorias)
            {
                categorias.Add(new SelectListItem { Text = item.descripcion, Value = item.id.ToString() });
            }
            //------------------------

            //inicio select list proveedor
            List<SelectListItem> proveedores = new List<SelectListItem>();
            proveedores.Add(new SelectListItem { Text = "--Seleccione--", Value = "" });
            IList<providers> listadoProveedores = db.providers.ToList();
            foreach (var item in listadoProveedores)
            {
                proveedores.Add(new SelectListItem { Text = item.name, Value = item.id.ToString() });
            }
            //------------------------


            //inicio select list medidas
            List<SelectListItem> medidas = new List<SelectListItem>();
            medidas.Add(new SelectListItem { Text = "--Seleccione--", Value = "" });
            IList<unitMeasure> listadoMedidas = db.unitMeasure.ToList();
            foreach (var item in listadoMedidas)
            {
                medidas.Add(new SelectListItem { Text = item.name + " (" + item.siglas + ")", Value = item.id.ToString() });
            }
            //------------------------

            //inicio select list iva
            List<SelectListItem> iva = new List<SelectListItem>();
            iva.Add(new SelectListItem { Text = "--Seleccione--", Value = "" });
            IList<iva> listadoIva = db.iva.ToList();
            foreach (var item in listadoIva)
            {
                iva.Add(new SelectListItem { Text = item.name, Value = item.id.ToString() });
            }
            //------------------------

            //inicio select list presentacion
            List<SelectListItem> presentacion = new List<SelectListItem>();
            IList<presentation> listadoPresentacion = db.presentation.ToList();
            foreach (var item in listadoPresentacion)
            {
                presentacion.Add(new SelectListItem { Text = item.descripction, Value = item.id.ToString() });
            }
            //------------------------


            ViewBag.categorias = categorias;
            ViewBag.proveedores = proveedores;
            ViewBag.medidas = medidas;
            ViewBag.iva = iva;
            ViewBag.presentacion = presentacion;

            return View();
        }

        // POST: Products/products/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,productIdSubPresentation,barcode,name,inventaryMin,priceIn,priceOut,priceOut2,presentationId,userId,providerId,ivaId,unitMeasureId,initialQuantity,categoriaId,detalleIva")] products products)
        {
            
            var errors = ModelState.Values.SelectMany(v => v.Errors);
            if (ModelState.IsValid)
            {
                db.products.Add(products);
                db.SaveChanges();
                var operacion = new operation()
                {
                    productId = products.id,
                    quantity = products.initialQuantity,
                    operationTypeId = 1,
                    date = DateTime.Now,
                    price = products.priceIn
                };
                db.operation.Add(operacion);
                db.SaveChanges();

                return RedirectToAction("Index");
            }


            //inicio select list categorias
            List<SelectListItem> categorias = new List<SelectListItem>();
            categorias.Add(new SelectListItem { Text = "--Seleccione--", Value = "" });
            IList<categorias> listadoCategorias = db.categorias.ToList();
            foreach (var item in listadoCategorias)
            {
                categorias.Add(new SelectListItem { Text = item.descripcion, Value = item.id.ToString() });
            }
            //------------------------

            //inicio select list proveedor
            List<SelectListItem> proveedores = new List<SelectListItem>();
            proveedores.Add(new SelectListItem { Text = "--Seleccione--", Value = "" });
            IList<providers> listadoProveedores = db.providers.ToList();
            foreach (var item in listadoProveedores)
            {
                proveedores.Add(new SelectListItem { Text = item.name, Value = item.id.ToString() });
            }
            //------------------------


            //inicio select list medidas
            List<SelectListItem> medidas = new List<SelectListItem>();
            medidas.Add(new SelectListItem { Text = "--Seleccione--", Value = "" });
            IList<unitMeasure> listadoMedidas = db.unitMeasure.ToList();
            foreach (var item in listadoMedidas)
            {
                medidas.Add(new SelectListItem { Text = item.name + " (" + item.siglas + ")", Value = item.id.ToString() });
            }
            //------------------------

            //inicio select list iva
            List<SelectListItem> iva = new List<SelectListItem>();
            iva.Add(new SelectListItem { Text = "--Seleccione--", Value = "" });
            IList<iva> listadoIva = db.iva.ToList();
            foreach (var item in listadoIva)
            {
                iva.Add(new SelectListItem { Text = item.name, Value = item.id.ToString() });
            }
            //------------------------

            //inicio select list presentacion
            List<SelectListItem> presentacion = new List<SelectListItem>();
            IList<presentation> listadoPresentacion = db.presentation.ToList();
            foreach (var item in listadoPresentacion)
            {
                presentacion.Add(new SelectListItem { Text = item.descripction, Value = item.id.ToString() });
            }
            //------------------------


            ViewBag.categorias = categorias;
            ViewBag.proveedores = proveedores;
            ViewBag.medidas = medidas;
            ViewBag.iva = iva;
            ViewBag.presentacion = presentacion;


            return View(products);
        }

        // GET: Products/products/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            products products = db.products.Find(id);
            
            if (products == null)
            {
                return HttpNotFound();
            }

            //inicio select list categorias
            List<SelectListItem> categorias = new List<SelectListItem>();
            categorias.Add(new SelectListItem { Text = "--Seleccione--", Value = "" });
            IList<categorias> listadoCategorias = db.categorias.ToList();
            foreach (var item in listadoCategorias)
            {
                categorias.Add(new SelectListItem { Text = item.descripcion, Value = item.id.ToString() });
            }
            //------------------------

            //inicio select list proveedor
            List<SelectListItem> proveedores = new List<SelectListItem>();
            proveedores.Add(new SelectListItem { Text = "--Seleccione--", Value = "" });
            IList<providers> listadoProveedores = db.providers.ToList();
            foreach (var item in listadoProveedores)
            {
                proveedores.Add(new SelectListItem { Text = item.name, Value = item.id.ToString() });
            }
            //------------------------


            //inicio select list medidas
            List<SelectListItem> medidas = new List<SelectListItem>();
            medidas.Add(new SelectListItem { Text = "--Seleccione--", Value = "" });
            IList<unitMeasure> listadoMedidas = db.unitMeasure.ToList();
            foreach (var item in listadoMedidas)
            {
                medidas.Add(new SelectListItem { Text = item.name + " (" + item.siglas + ")", Value = item.id.ToString() });
            }
            //------------------------

            //inicio select list iva
            List<SelectListItem> iva = new List<SelectListItem>();
            iva.Add(new SelectListItem { Text = "--Seleccione--", Value = "" });
            IList<iva> listadoIva = db.iva.ToList();
            foreach (var item in listadoIva)
            {
                iva.Add(new SelectListItem { Text = item.name, Value = item.id.ToString() });
            }
            //------------------------

            //inicio select list presentacion
            List<SelectListItem> presentacion = new List<SelectListItem>();
            IList<presentation> listadoPresentacion = db.presentation.ToList();
            foreach (var item in listadoPresentacion)
            {
                presentacion.Add(new SelectListItem { Text = item.descripction, Value = item.id.ToString() });
            }
            //------------------------


            ViewBag.categorias = categorias;
            ViewBag.proveedores = proveedores;
            ViewBag.medidas = medidas;
            ViewBag.iva = iva;
            ViewBag.presentacion = presentacion;

            return View(products);
        }

        // POST: Products/products/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,productIdSubPresentation,barcode,name,inventaryMin,priceIn,priceOut,priceOut2,presentationId,userId,providerId,ivaId,unitMeasureId,initialQuantity,categoriaId,detalleIva")] products products)
        {
            if (ModelState.IsValid)
            {
                db.Entry(products).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }


            //inicio select list categorias
            List<SelectListItem> categorias = new List<SelectListItem>();
            categorias.Add(new SelectListItem { Text = "--Seleccione--", Value = "" });
            IList<categorias> listadoCategorias = db.categorias.ToList();
            foreach (var item in listadoCategorias)
            {
                categorias.Add(new SelectListItem { Text = item.descripcion, Value = item.id.ToString() });
            }
            //------------------------

            //inicio select list proveedor
            List<SelectListItem> proveedores = new List<SelectListItem>();
            proveedores.Add(new SelectListItem { Text = "--Seleccione--", Value = "" });
            IList<providers> listadoProveedores = db.providers.ToList();
            foreach (var item in listadoProveedores)
            {
                proveedores.Add(new SelectListItem { Text = item.name, Value = item.id.ToString() });
            }
            //------------------------


            //inicio select list medidas
            List<SelectListItem> medidas = new List<SelectListItem>();
            medidas.Add(new SelectListItem { Text = "--Seleccione--", Value = "" });
            IList<unitMeasure> listadoMedidas = db.unitMeasure.ToList();
            foreach (var item in listadoMedidas)
            {
                medidas.Add(new SelectListItem { Text = item.name + " (" + item.siglas + ")", Value = item.id.ToString() });
            }
            //------------------------

            //inicio select list iva
            List<SelectListItem> iva = new List<SelectListItem>();
            iva.Add(new SelectListItem { Text = "--Seleccione--", Value = "" });
            IList<iva> listadoIva = db.iva.ToList();
            foreach (var item in listadoIva)
            {
                iva.Add(new SelectListItem { Text = item.name, Value = item.id.ToString() });
            }
            //------------------------

            //inicio select list presentacion
            List<SelectListItem> presentacion = new List<SelectListItem>();
            IList<presentation> listadoPresentacion = db.presentation.ToList();
            foreach (var item in listadoPresentacion)
            {
                presentacion.Add(new SelectListItem { Text = item.descripction, Value = item.id.ToString() });
            }
            //------------------------


            ViewBag.categorias = categorias;
            ViewBag.proveedores = proveedores;
            ViewBag.medidas = medidas;
            ViewBag.iva = iva;
            ViewBag.presentacion = presentacion;

            return View(products);
        }

        // GET: Products/products/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            products products = db.products.Find(id);
            if (products == null)
            {
                return HttpNotFound();
            }
            return View(products);
        }

        // POST: Products/products/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            products products = db.products.Find(id);
            db.products.Remove(products);
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

        [Authorize]
        public List<products> Listar()
        {
            var Productos = new List<products>();
            try
            {
                using (var ctx = new AccountingContext())
                {
                    Productos = ctx.products.ToList();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return Productos;
        }
    }
}
