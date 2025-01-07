using FE.Documentos;
using PlanillajeColectivos.DTO.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PlanillajeColectivos.Areas.Procesos.Controllers
{
    public class OperacionesController : Controller
    {
        public decimal GetTotalExcentos(List<operation> operaciones)
        {
            decimal total = 0;
            operaciones = operaciones.Where(x => x.products.ivaId == 3).ToList();
            if (operaciones.Count > 0)
            {
                foreach (var item in operaciones)
                {
                    total += item.price * item.quantity;
                }
            }
            
            return total;
        }
        public decimal GetTotalExcluidos(List<operation> operaciones)
        {
            decimal total = 0;
            operaciones = operaciones.Where(x => x.products.ivaId == 4).ToList();
            if (operaciones.Count > 0)
            {
                foreach (var item in operaciones)
                {
                    total += item.price * item.quantity;
                }
            }

            return total;
        }

        public decimal[] GetTotalIva19(List<ResumenImpuesto> impuestos)
        {
            decimal TotalIva19 = 0;
            decimal TotalBase19 = 0;
            foreach (var item in impuestos)
            {
                foreach (var item2 in item.Detalles)
                {
                    if (item2.Porcentaje == 19M)
                    {
                        TotalIva19 += item2.Importe;
                        TotalIva19 += item2.BaseImponible;
                    }
                }

            }

            var array = new decimal[2];
            array[0] = TotalIva19;
            array[1] = TotalBase19;

            return array;
        }
        public decimal[] GetTotalIva5(List<ResumenImpuesto> impuestos)
        {
            decimal TotalIva5 = 0;
            decimal TotalBase5 = 0;
            foreach (var item in impuestos)
            {
                foreach (var item2 in item.Detalles)
                {
                    if (item2.Porcentaje == 5M)
                    {
                        TotalIva5 += item2.Importe;
                        TotalIva5 += item2.BaseImponible;
                    }
                }

            }

            var array = new decimal[2];
            array[0] = TotalIva5;
            array[1] = TotalBase5;

            return array;
        }
    }
}