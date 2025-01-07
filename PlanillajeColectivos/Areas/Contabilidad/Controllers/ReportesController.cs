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
using OfficeOpenXml;
using System.Globalization;
using OfficeOpenXml.Style;

namespace PlanillajeColectivos.Areas.Contabilidad.Controllers
{
    public class ReportesController : Controller
    {
        private AccountingContext db = new AccountingContext();
        // GET: Contabilidad/Reportes
        public ActionResult ComprobanteInformeDiario()

        {
            var fechaRegistrob = DateTime.Now;
            DateTime Fechas = Convert.ToDateTime(fechaRegistrob);
            
            string Date = Fechas.ToString("yyyy-MM-dd");
            ViewBag.Fechas = Date;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ComprobanteInformeDiario(comprobante comprobanteR)
        {
            if (!ModelState.IsValid)
                return View();

            try
            {
                using (AccountingContext db = new AccountingContext())
                {

                    var fecha = comprobanteR.fechaCreacion;
                    return RedirectToAction("ReporteComprobante", new { FechaR = fecha });

                }
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Error al Agregar ");
                return View();
            }
        }

        public ActionResult InformeInventarios()
        {
            NumberFormatInfo formato = new CultureInfo("es-CO").NumberFormat;

            formato.CurrencyGroupSeparator = ".";
            formato.NumberDecimalSeparator = ",";
            DateTime FechaActual = DateTime.Now;

            DateTime FechaAct = Convert.ToDateTime(FechaActual);
            DateTime Fecha = new DateTime(FechaAct.Year, FechaAct.Month, FechaAct.Day, 0, 0, 0);
            DateTime Fechas = Convert.ToDateTime(Fecha);

            string FechaString = Fechas.ToString("yyyy-MM-dd");

            var Productos = db.products.Where(x => x.activo == true).ToList();
          

            Response.Clear();
            Response.ClearContent();
            Response.ClearHeaders();
            Response.Buffer = true;
            Response.ContentEncoding = System.Text.Encoding.UTF8;
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment;filename=InformeInventario.xlsx");

            using (ExcelPackage pack = new ExcelPackage())
            {
                ExcelWorksheet ws = pack.Workbook.Worksheets.Add("InformeInventario");

                ws.Cells["D1:F1"].Merge = true;
                ws.Cells["D1:F1"].Value = "INFORME DE INVENTARIOS";
                ws.Cells["D1:F1"].Style.Font.Bold = true;
                ws.Cells["D1:F1"].Style.Font.Size = 14;
                ws.Cells["D1:F1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;

                ws.Cells["D2:F2"].Merge = true;
                ws.Cells["D2:F2"].Value = "DISTRIPOLLO LOS ANGELES";
                ws.Cells["D2:F2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                ws.Cells["D2:F2"].Style.Font.Bold = true;
                ws.Cells["D2:F2"].Style.Font.Size = 12;

                ws.Cells["D3:F3"].Merge = true;
                ws.Cells["D3:F3"].Value = "37.087.259-9";
                ws.Cells["D3:F3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                ws.Cells["D3:F3"].Style.Font.Bold = true;
                ws.Cells["D3:F3"].Style.Font.Size = 12;

                ws.Cells["D4:E4"].Merge = true;
                ws.Cells["D4:E4"].Value = "Fecha del Informe:";
                ws.Cells["D4:E4"].Style.Font.Size = 10;
                ws.Cells["D4:E4"].Style.Font.Bold = true;

                ws.Cells["F" + 4].Value = FechaString;
                ws.Cells["F" + 4].Style.Font.Size = 10;
                ws.Cells["F" + 4].Style.Font.Bold = true;

                ws.Cells["A6"].Value = "REFERENCIA";
                ws.Cells["A6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                ws.Cells["A6"].Style.Font.Bold = true;
                ws.Cells["A6"].Style.Font.Size = 10;

                ws.Cells["B6"].Value = "NOMBRE";
                ws.Cells["B6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                ws.Cells["B6"].Style.Font.Bold = true;
                ws.Cells["B6"].Style.Font.Size = 10;

                ws.Cells["C6"].Value = "PROVEEDOR";
                ws.Cells["C6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                ws.Cells["C6"].Style.Font.Bold = true;
                ws.Cells["C6"].Style.Font.Size = 10;

                ws.Cells["D6"].Value = "CATEGORIA";
                ws.Cells["D6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                ws.Cells["D6"].Style.Font.Bold = true;
                ws.Cells["D6"].Style.Font.Size = 10;

                ws.Cells["E6"].Value = "PRECIO DE ENTRADA";
                ws.Cells["E6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                ws.Cells["E6"].Style.Font.Bold = true;
                ws.Cells["E6"].Style.Font.Size = 10;

                ws.Cells["F6"].Value = "IVA %";
                ws.Cells["F6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                ws.Cells["F6"].Style.Font.Bold = true;
                ws.Cells["F6"].Style.Font.Size = 10;

                ws.Cells["G6"].Value = "STOCK";
                ws.Cells["G6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                ws.Cells["G6"].Style.Font.Bold = true;
                ws.Cells["G6"].Style.Font.Size = 10;

                ws.Cells["H6"].Value = "VALOR UNI/CON IVA";
                ws.Cells["H6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                ws.Cells["H6"].Style.Font.Bold = true;
                ws.Cells["H6"].Style.Font.Size = 10;

                ws.Cells["I6"].Value = "SUBTOTAL/SIN IVA";
                ws.Cells["I6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                ws.Cells["I6"].Style.Font.Bold = true;
                ws.Cells["I6"].Style.Font.Size = 10;

                ws.Cells["J6"].Value = "VALOR TOTAL";
                ws.Cells["J6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                ws.Cells["J6"].Style.Font.Bold = true;
                ws.Cells["J6"].Style.Font.Size = 10;

               

                int j = 7;

                decimal totalVUCI = 0, SubTotalSinIva = 0, total = 0;
                foreach (var item in Productos)
                {

                    
                    var valoriva = ((item.priceIn * item.ivaFK.value)/100);
                    var subtotalsiniva = item.priceIn * item.initialQuantity;
                    SubTotalSinIva += subtotalsiniva;


                    var valoruniconiva = item.priceIn + valoriva;
                    totalVUCI += valoruniconiva;
                    var valortotal = valoruniconiva * item.initialQuantity;
                    total += valortotal; 

                    ws.Cells["A" + j].Value = item.barcode;
                    ws.Cells["B" + j].Value = item.name;
                    ws.Cells["C" + j].Value = item.providerFK.name;
                    ws.Cells["D" + j].Value = item.categoriaFK.descripcion;
                    ws.Cells["E" + j].Value = item.priceIn;
                    ws.Cells["E" + j].Style.Numberformat.Format = "$#,##0.00";
                    ws.Cells["F" + j].Value = item.ivaFK.name;
                    ws.Cells["G" + j].Value = item.initialQuantity;
                    ws.Cells["H" + j].Value = valoruniconiva;
                    ws.Cells["H" + j].Style.Numberformat.Format = "$#,##0.00";
                    ws.Cells["I" + j].Value = subtotalsiniva;
                    ws.Cells["I" + j].Style.Numberformat.Format = "$#,##0.00";
                    ws.Cells["J" + j].Value = valortotal;
                    ws.Cells["J" + j].Style.Numberformat.Format = "$#,##0.00";
                    j++;
                }

                j += 2;

                var TOTALENTRADAS = Productos.Where(x => x.activo == true).Select(x => x.priceIn).Sum();

            ws.Cells["A" + j].Value = "TOTALES";
                ws.Cells["A" + j].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                ws.Cells["A" + j].Style.Font.Bold = true;
                ws.Cells["A" + j].Style.Font.Size = 10;
                

                ws.Cells["E" + j].Value = TOTALENTRADAS;
                ws.Cells["E" + j].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                ws.Cells["E" + j].Style.Font.Bold = true;
                ws.Cells["E" + j].Style.Font.Size = 10;
                ws.Cells["E" + j].Style.Numberformat.Format = "$#,##0.00";

                ws.Cells["H" + j].Value = totalVUCI;
                ws.Cells["H" + j].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                ws.Cells["H" + j].Style.Font.Bold = true;
                ws.Cells["H" + j].Style.Font.Size = 10;
                ws.Cells["H" + j].Style.Numberformat.Format = "$#,##0.00";

                ws.Cells["I" + j].Value = SubTotalSinIva;
                ws.Cells["I" + j].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                ws.Cells["I" + j].Style.Font.Bold = true;
                ws.Cells["I" + j].Style.Font.Size = 10;
                ws.Cells["I" + j].Style.Numberformat.Format = "$#,##0.00";

                ws.Cells["J" + j].Value = total;
                ws.Cells["J" + j].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                ws.Cells["J" + j].Style.Font.Bold = true;
                ws.Cells["J" + j].Style.Font.Size = 10;
                ws.Cells["J" + j].Style.Numberformat.Format = "$#,##0.00";

                ws.Cells[ws.Dimension.Address].AutoFitColumns();
                var ms = new System.IO.MemoryStream();
                pack.SaveAs(ms);
                ms.WriteTo(Response.OutputStream);
            }
            Response.End();

            return RedirectToAction("../Informes/Index");
        }

        public ActionResult ReporteComprobante(DateTime FechaR)
        {
            NumberFormatInfo formato = new CultureInfo("es-CO").NumberFormat;

            formato.CurrencyGroupSeparator = ".";
            formato.NumberDecimalSeparator = ",";
            DateTime FechaActual = DateTime.Now;

            var Datos = db.persons.ToList();
            var FechaRep = FechaR;

                DateTime FA = Convert.ToDateTime(FechaRep);
                DateTime FD = Convert.ToDateTime(FechaRep);
                DateTime fechAntes = new DateTime(FA.Year, FA.Month, FA.Day, 0, 0, 0);
                DateTime fechDespues = new DateTime(FD.Year, FD.Month, FD.Day, 23, 59, 59);
                
            

            var Comprobantes = db.comprobantes.Where(x => x.fechaCreacion >= fechAntes && x.fechaCreacion <= fechDespues && x.tipoComprobante == "CC1" && x.estado == true).ToList();
            var Facturas = db.factura.Where(x => x.date >= fechAntes && x.date <= fechDespues && x.operationTypeId == 15 && x.estado == true).ToList();


            decimal TotalComprobantes = 0, CantidadTransacciones = 0, BaseGrab19 = 0, BaseGrab5 = 0, BaseGrab0 = 0, Excluidos = 0, iva19 = 0, iva5 = 0 ; 
            string inicial = "1", final = "2";

            foreach (var item in Facturas)
            {
                
                TotalComprobantes = Facturas.Where(x => x.estado == true).Select(x => x.total).Sum();
                
            }
            int TotalCom = Convert.ToInt32(TotalComprobantes);
            foreach (var item in Comprobantes)
            {

                CantidadTransacciones = Comprobantes.Where(x => x.tipoComprobante == item.tipoComprobante).Select(x => x.valorTotal).Count();

            }
            int CantTras = Convert.ToInt32(CantidadTransacciones);

            foreach (var item in Comprobantes)
            {

                inicial = Comprobantes.Where(x => x.tipoComprobante == item.tipoComprobante).Select(x => x.numero).Min();

            }
            
            int Cominicial = Int32.Parse(inicial);
            foreach (var item in Comprobantes)
            {

                final = Comprobantes.Where(x => x.tipoComprobante == item.tipoComprobante).Select(x => x.numero).Max();

            }
            
            int comfinal = Int32.Parse(final);

            foreach (var item in Facturas)
            {

                BaseGrab19 = Facturas.Where(x => x.operationTypeId == 15).Select(x => x.baseIVA19).Sum();

            }
            int BaseGrab19int = Convert.ToInt32(BaseGrab19);

            foreach (var item in Facturas)
            {

                BaseGrab5 = Facturas.Where(x => x.operationTypeId == 15).Select(x => x.baseIVA5).Sum();

            }
            int BaseGrab5int = Convert.ToInt32(BaseGrab5);

            foreach (var item in Facturas)
            {

                BaseGrab0 = Facturas.Where(x => x.operationTypeId == 15).Select(x => x.valorTotalExcentos).Sum();

            }
            int BaseGrab0int = Convert.ToInt32(BaseGrab0);

            foreach (var item in Facturas)
            {

                Excluidos = Facturas.Where(x => x.operationTypeId == 15).Select(x => x.baseIVA5).Sum();

            }
            int Excluidosint = Convert.ToInt32(Excluidos);

            int TotalesGravables = BaseGrab19int + BaseGrab5int + BaseGrab0int + Excluidosint;

            foreach (var item in Facturas)
            {

                iva19 = Facturas.Where(x => x.operationTypeId == 15).Select(x => x.valorIVA19).Sum();

            }
            int iva19int = Convert.ToInt32(iva19);

            foreach (var item in Facturas)
            {

                iva5 = Facturas.Where(x => x.operationTypeId == 15).Select(x => x.valorIVA5).Sum();

            }
            int iva5int = Convert.ToInt32(iva5);

            int ValorNeto19 = BaseGrab19int + iva19int;
            int ValorNeto5 = BaseGrab5int + iva5int;
            int Valorneto0 = BaseGrab0int;
            int ValorExcluido = Excluidosint;

            int TotalIva = iva19int + iva5int;
            int TotalValorNeto = ValorNeto19 + ValorNeto5 + Valorneto0 + ValorExcluido;
            int TotalBase = BaseGrab19int + BaseGrab5int + BaseGrab0int;

            int efectivo = 0;
            foreach (var item in Facturas)
            {

                efectivo = Facturas.Where(x => x.operationTypeId == 15 && x.cash != 0).Select(x => x.cash).Count();

            }
            int Vcredito = 0;
            foreach (var item in Facturas)
            {

                Vcredito = Facturas.Where(x => x.operationTypeId == 15 && x.payCredit != 0).Select(x => x.payCredit).Count();

            }
            int Tdebito = 0;
            foreach (var item in Facturas)
            {

                Tdebito = Facturas.Where(x => x.operationTypeId == 15 && x.payTdebit != 0).Select(x => x.payTdebit).Count();

            }
            int Tcredito = 0;
            foreach (var item in Facturas)
            {

                Tcredito = Facturas.Where(x => x.operationTypeId == 15 && x.payTcredit != 0).Select(x => x.payTcredit).Count();

            }
            int Ntotaltransacciones = efectivo + Vcredito + Tdebito + Tcredito;
            DateTime Fechas = Convert.ToDateTime(FechaRep); 

            string FechaString = Fechas.ToString("yyyy-MM-dd");

            Response.Clear();
            Response.ClearContent();
            Response.ClearHeaders();
            Response.Buffer = true;
            Response.ContentEncoding = System.Text.Encoding.UTF8;
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment;filename=Comprobante Informe Diario.xlsx");

            using (ExcelPackage pack = new ExcelPackage())
            {
                ExcelWorksheet ws = pack.Workbook.Worksheets.Add("Comprobante Informe Diario");


                ws.Cells["C1:G1"].Merge = true;
                ws.Cells["C1:G1"].Value = "COMPROBANTE INFORME DIARIO";
                ws.Cells["C1:G1"].Style.Font.Bold = true;
                ws.Cells["C1:G1"].Style.Font.Size = 12;
                ws.Cells["C1:G1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;

                ws.Cells["C2:G2"].Merge = true;
                ws.Cells["C2:G2"].Value = "DISTRIPOLLO LOS ANGELES";
                ws.Cells["C2:G2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                ws.Cells["C2:G2"].Style.Font.Bold = true;

                ws.Cells["D3:F3"].Merge = true;
                ws.Cells["D3:F3"].Value = "37.087.259-9";
                ws.Cells["D3:F3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                ws.Cells["D3:F3"].Style.Font.Bold = true;

                ws.Cells["D4:E4"].Merge = true;
                ws.Cells["D4:E4"].Value = "Fecha comprobante diario:";
                ws.Cells["D4:E4"].Style.Font.Size = 9;
                ws.Cells["D4:E4"].Style.Font.Bold = true;

                ws.Cells["F" + 4].Value = FechaString;
                ws.Cells["F" + 4].Style.Font.Size = 9;

                ws.Cells["A" + 7].Value = FechaString;
                ws.Cells["A" + 7].Style.Font.Size = 9;

                ws.Cells["A6:B6"].Merge = true;
                ws.Cells["A6:B6"].Style.Font.Size = 10;
                ws.Cells["A6:B6"].Style.Font.Bold = true;
                ws.Cells["A6:B6"].Value = "Fecha de Cierre Diario";

                ws.Cells["A9:B9"].Merge = true;
                ws.Cells["A9:B9"].Style.Font.Size = 9;
                ws.Cells["A9:B9"].Style.Font.Bold = true;
                ws.Cells["A9:B9"].Value = "Cantidad de transacciones";

                ws.Cells["A" + 10].Value = CantTras;
                ws.Cells["A" + 10].Style.Font.Size = 9;

                ws.Cells["D9:E9"].Merge = true;
                ws.Cells["D9:E9"].Style.Font.Size = 10;
                ws.Cells["D9:E9"].Style.Font.Bold = true;
                ws.Cells["D9:E9"].Value = "Valor de transacciones";

                ws.Cells["D" + 10].Value = TotalCom;
                ws.Cells["D" + 10].Style.Font.Size = 9;
                ws.Cells["D" + 10].Style.Numberformat.Format = "$#,##0.00";

                ws.Cells["A12:B12"].Merge = true;
                ws.Cells["A12:B12"].Style.Font.Size = 10;
                ws.Cells["A12:B12"].Style.Font.Bold = true;
                ws.Cells["A12:B12"].Value = "Tipo de comprobante";

                ws.Cells["A" + 13].Value = "INGRESOS CAJA";
                ws.Cells["A" + 13].Style.Font.Size = 9;

                ws.Cells["C12:D12"].Merge = true;
                ws.Cells["C12:D12"].Style.Font.Size = 10;
                ws.Cells["C12:D12"].Style.Font.Bold = true;
                ws.Cells["C12:D12"].Value = "Comprobante";

                ws.Cells["C" + 13].Value = "CC1-15-POS";
                ws.Cells["C" + 13].Style.Font.Size = 9;

                ws.Cells["E12:F12"].Merge = true;
                ws.Cells["E12:F12"].Style.Font.Size = 10;
                ws.Cells["E12:F12"].Style.Font.Bold = true;
                ws.Cells["E12:F12"].Value = "Numero inicial";

                ws.Cells["E" + 13].Value = Cominicial;
                ws.Cells["E" + 13].Style.Font.Size = 9;

                ws.Cells["G12:H12"].Merge = true;
                ws.Cells["G12:H12"].Style.Font.Size = 10;
                ws.Cells["G12:H12"].Style.Font.Bold = true;
                ws.Cells["G12:H12"].Value = "Numero final";

                ws.Cells["G" + 13].Value = comfinal;
                ws.Cells["G" + 13].Style.Font.Size = 9;

                ws.Cells["A15:C15"].Merge = true;
                ws.Cells["A15:C15"].Style.Font.Size = 10;
                ws.Cells["A15:C15"].Style.Font.Bold = true;
                ws.Cells["A15:C15"].Value = "Totales ventas por dependencia";

                ws.Cells["A17:B17"].Merge = true;
                ws.Cells["A17:B17"].Style.Font.Size = 10;
                ws.Cells["A17:B17"].Style.Font.Bold = true;
                ws.Cells["A17:B17"].Value = "Nombre dependencia";

                ws.Cells["A" + 18].Value = "Venta Caja";
                ws.Cells["A" + 18].Style.Font.Size = 9;

                ws.Cells["A" + 19].Value = "Venta Caja";
                ws.Cells["A" + 19].Style.Font.Size = 9;

                ws.Cells["A" + 20].Value = "Venta Caja";
                ws.Cells["A" + 20].Style.Font.Size = 9;

                ws.Cells["A" + 21].Value = "Venta Caja";
                ws.Cells["A" + 21].Style.Font.Size = 9;

                ws.Cells["C17"].Merge = true;
                ws.Cells["C17"].Style.Font.Size = 10;
                ws.Cells["C17"].Style.Font.Bold = true;
                ws.Cells["C17"].Value = "Operación";

                ws.Cells["C" + 18].Value = "Gravado";
                ws.Cells["C" + 18].Style.Font.Size = 9;

                ws.Cells["C" + 19].Value = "Gravado";
                ws.Cells["C" + 19].Style.Font.Size = 9;

                ws.Cells["C" + 20].Value = "Gravado";
                ws.Cells["C" + 20].Style.Font.Size = 9;

                ws.Cells["C" + 21].Value = "Excluido";
                ws.Cells["C" + 21].Style.Font.Size = 9;

                ws.Cells["D17"].Merge = true;
                ws.Cells["D17"].Style.Font.Size = 10;
                ws.Cells["D17"].Style.Font.Bold = true;
                ws.Cells["D17"].Value = "Tarifa";

                ws.Cells["D" + 18].Value = "19%";
                ws.Cells["D" + 18].Style.Font.Size = 9;

                ws.Cells["D" + 19].Value = "5%";
                ws.Cells["D" + 19].Style.Font.Size = 9;

                ws.Cells["D" + 20].Value = "0%";
                ws.Cells["D" + 20].Style.Font.Size = 9;

                ws.Cells["D" + 21].Value = "";
                ws.Cells["D" + 21].Style.Font.Size = 9;

                ws.Cells["E17:F17"].Merge = true;
                ws.Cells["E17:F17"].Style.Font.Size = 10;
                ws.Cells["E17:F17"].Style.Font.Bold = true;
                ws.Cells["E17:F17"].Value = "Base gravable";

                ws.Cells["E" + 18].Value = BaseGrab19int;
                ws.Cells["E" + 18].Style.Font.Size = 9;
                ws.Cells["E" + 18].Style.Numberformat.Format = "$#,##0.00";

                ws.Cells["E" + 19].Value = BaseGrab5int;
                ws.Cells["E" + 19].Style.Font.Size = 9;
                ws.Cells["E" + 19].Style.Numberformat.Format = "$#,##0.00";

                ws.Cells["E" + 20].Value = BaseGrab0int;
                ws.Cells["E" + 20].Style.Font.Size = 9;
                ws.Cells["E" + 20].Style.Numberformat.Format = "$#,##0.00";


                ws.Cells["G17"].Merge = true;
                ws.Cells["G17"].Style.Font.Size = 10;
                ws.Cells["G17"].Style.Font.Bold = true;
                ws.Cells["G17"].Value = "Valor IVA";

                ws.Cells["G" + 18].Value = iva19int;
                ws.Cells["G" + 18].Style.Font.Size = 9;
                ws.Cells["G" + 18].Style.Numberformat.Format = "$#,##0.00";

                ws.Cells["G" + 19].Value = iva5int;
                ws.Cells["G" + 19].Style.Font.Size = 9;
                ws.Cells["G" + 19].Style.Numberformat.Format = "$#,##0.00";

                ws.Cells["G" + 20].Value = 0;
                ws.Cells["G" + 20].Style.Font.Size = 9;
                ws.Cells["G" + 20].Style.Numberformat.Format = "$#,##0.00";

                ws.Cells["H17"].Merge = true;
                ws.Cells["H17"].Style.Font.Size = 10;
                ws.Cells["H17"].Style.Font.Bold = true;
                ws.Cells["H17"].Value = "Valor neto";

                ws.Cells["H" + 18].Value = ValorNeto19;
                ws.Cells["H" + 18].Style.Font.Size = 9;
                ws.Cells["H" + 18].Style.Numberformat.Format = "$#,##0.00";

                ws.Cells["H" + 19].Value = ValorNeto5;
                ws.Cells["H" + 19].Style.Font.Size = 9;
                ws.Cells["H" + 19].Style.Numberformat.Format = "$#,##0.00";

                ws.Cells["H" + 20].Value = Valorneto0;
                ws.Cells["H" + 20].Style.Font.Size = 9;
                ws.Cells["H" + 20].Style.Numberformat.Format = "$#,##0.00";

                ws.Cells["H" + 21].Value = ValorExcluido;
                ws.Cells["H" + 21].Style.Font.Size = 9;
                ws.Cells["H" + 21].Style.Numberformat.Format = "$#,##0.00";

                ws.Cells["A22:B22"].Merge = true;
                ws.Cells["A22:B22"].Style.Font.Size = 10;
                ws.Cells["A22:B22"].Style.Font.Bold = true;
                ws.Cells["A22:B22"].Value = "Total por dependiencia:";

                ws.Cells["E" + 22].Value = TotalBase;
                ws.Cells["E" + 22].Style.Font.Size = 9;
                ws.Cells["E" + 22].Style.Numberformat.Format = "$#,##0.00";

                ws.Cells["G" + 22].Value = TotalIva;
                ws.Cells["G" + 22].Style.Font.Size = 9;
                ws.Cells["G" + 22].Style.Numberformat.Format = "$#,##0.00";

                ws.Cells["H" + 22].Value = TotalValorNeto;
                ws.Cells["H" + 22].Style.Font.Size = 9;
                ws.Cells["H" + 22].Style.Numberformat.Format = "$#,##0.00";

                ws.Cells["A25:C25"].Merge = true;
                ws.Cells["A25:C25"].Style.Font.Size = 10;
                ws.Cells["A25:C25"].Style.Font.Bold = true;
                ws.Cells["A25:C25"].Value = "Totales por tarifa de IVA";

                ws.Cells["A27"].Merge = true;
                ws.Cells["A27"].Style.Font.Size = 10;
                ws.Cells["A27"].Style.Font.Bold = true;
                ws.Cells["A27"].Value = "Operación";

                ws.Cells["A" + 28].Value = "Gravado";
                ws.Cells["A" + 28].Style.Font.Size = 9;

                ws.Cells["A" + 29].Value = "Gravado";
                ws.Cells["A" + 29].Style.Font.Size = 9;

                ws.Cells["A" + 30].Value = "Gravado";
                ws.Cells["A" + 30].Style.Font.Size = 9;


                ws.Cells["B27"].Merge = true;
                ws.Cells["B27"].Style.Font.Size = 10;
                ws.Cells["B27"].Style.Font.Bold = true;
                ws.Cells["B27"].Value = "Tarifa";

                ws.Cells["B" + 28].Value = "19%";
                ws.Cells["B" + 28].Style.Font.Size = 9;

                ws.Cells["B" + 29].Value = "5%";
                ws.Cells["B" + 29].Style.Font.Size = 9;

                ws.Cells["B" + 30].Value = "0%";
                ws.Cells["B" + 30].Style.Font.Size = 9;

                ws.Cells["C27:D27"].Merge = true;
                ws.Cells["C27:D27"].Style.Font.Size = 10;
                ws.Cells["C27:D27"].Style.Font.Bold = true;
                ws.Cells["C27:D27"].Value = "Base gravable";

                ws.Cells["C" + 28].Value = BaseGrab19int;
                ws.Cells["C" + 28].Style.Font.Size = 9;
                ws.Cells["C" + 28].Style.Numberformat.Format = "$#,##0.00";

                ws.Cells["C" + 29].Value = BaseGrab5int;
                ws.Cells["C" + 29].Style.Font.Size = 9;
                ws.Cells["C" + 29].Style.Numberformat.Format = "$#,##0.00";

                ws.Cells["C" + 30].Value = BaseGrab0int;
                ws.Cells["C" + 30].Style.Font.Size = 9;
                ws.Cells["C" + 30].Style.Numberformat.Format = "$#,##0.00";

                ws.Cells["E27"].Merge = true;
                ws.Cells["E27"].Style.Font.Size = 10;
                ws.Cells["E27"].Style.Font.Bold = true;
                ws.Cells["E27"].Value = "Valor IVA";

                ws.Cells["E" + 28].Value = iva19int;
                ws.Cells["E" + 28].Style.Font.Size = 9;
                ws.Cells["E" + 28].Style.Numberformat.Format = "$#,##0.00";

                ws.Cells["E" + 29].Value = iva5int;
                ws.Cells["E" + 29].Style.Font.Size = 9;
                ws.Cells["E" + 29].Style.Numberformat.Format = "$#,##0.00";

                ws.Cells["E" + 30].Value = 0;
                ws.Cells["E" + 30].Style.Font.Size = 9;
                ws.Cells["E" + 30].Style.Numberformat.Format = "$#,##0.00";

                ws.Cells["A32:C32"].Merge = true;
                ws.Cells["A32:C32"].Style.Font.Size = 10;
                ws.Cells["A32:C32"].Style.Font.Bold = true;
                ws.Cells["A32:C32"].Value = "Totales por medio de pago";

                ws.Cells["A34:B34"].Merge = true;
                ws.Cells["A34:B34"].Style.Font.Size = 9;
                ws.Cells["A34:B34"].Style.Font.Bold = true;
                ws.Cells["A34:B34"].Value = "Nombre medio de pago";


                ws.Cells["D34:E34"].Merge = true;
                ws.Cells["D34:E34"].Style.Font.Size = 9;
                ws.Cells["D34:E34"].Style.Font.Bold = true;
                ws.Cells["D34:E34"].Value = "Numero transacion";
                ws.Cells["D34:E34"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                ws.Cells["A35"].Merge = true;
                ws.Cells["A35"].Style.Font.Size = 10;
                ws.Cells["A35"].Value = "Efectivo";
                ws.Cells["E35"].Value = efectivo;

                ws.Cells["A36:B36"].Merge = true;
                ws.Cells["A36:B36"].Style.Font.Size = 10;
                ws.Cells["A36:B36"].Value = "Venta a credito";
                ws.Cells["E36"].Value = Vcredito;

                ws.Cells["A37:B37"].Merge = true;
                ws.Cells["A37:B37"].Style.Font.Size = 10;
                ws.Cells["A37:B37"].Value = "Tarjeta debito";
                ws.Cells["E37"].Value = Tdebito;

                ws.Cells["A38:B38"].Merge = true;
                ws.Cells["A38:B38"].Style.Font.Size = 10;
                ws.Cells["A38:B38"].Value = "Tarjeta credito";
                ws.Cells["E38"].Value = Tcredito;

                ws.Cells["A40:B40"].Merge = true;
                ws.Cells["A40:B40"].Style.Font.Size = 10;
                ws.Cells["A40:B40"].Style.Font.Bold = true;
                ws.Cells["A40:B40"].Value = "Transacciones Registradas:";
                ws.Cells["E40"].Value = Ntotaltransacciones;

                

                ws.Cells[ws.Dimension.Address].AutoFitColumns();
                var ms = new System.IO.MemoryStream();
                pack.SaveAs(ms);
                ms.WriteTo(Response.OutputStream);
            }
            Response.End();

            return RedirectToAction("../Reportes/ComprobanteInformeDiario");
        }

    }
}
