using PlanillajeColectivos.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PlanillajeColectivos.Areas.Geo.Controllers
{
    public class GeoController : Controller
    {
        AccountingContext db = new AccountingContext();
        // GET: Geo/Geo
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetDepartamento(int id)
        {

            var dpto = db.Departamento.Where(x => x.Pais_dep == id).ToList();
            if (dpto.Count==0)
            {
                return new JsonResult { Data = new { status = false } };
            }
            else
            {

                List<Array> departamento = new List<Array>();
                foreach(var item in dpto)
                {
                    string[] data = new string[2];
                    data[0] = item.Nom_dep+" ("+item.Id_dep+")";
                    data[1] = item.Id_dep.ToString();
                    departamento.Add(data);
                }


                return new JsonResult { Data = new { status = true, departamento } };
            }

        }

        [HttpPost]
        public JsonResult GetMunicipio(int id)
        {

            var muni = db.Municipio.Where(x => x.Dep_muni == id).ToList();
            if (muni.Count == 0)
            {
                return new JsonResult { Data = new { status = false } };
            }
            else
            {

                List<Array> municipio = new List<Array>();
                foreach (var item in muni)
                {
                    string[] data = new string[2];
                    data[0] = item.Nom_muni + " (" + item.Id_muni + ")";
                    data[1] = item.Id_muni.ToString();
                    municipio.Add(data);
                }


                return new JsonResult { Data = new { status = true, municipio } };
            }

        }
    }
}