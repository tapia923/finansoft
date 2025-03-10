
using OfficeOpenXml;
using PlanillajeColectivos.Areas.Procesos.Controllers;
using PlanillajeColectivos.DTO;
using PlanillajeColectivos.DTO.Contabilidad;
using PlanillajeColectivos.DTO.Informes;
using PlanillajeColectivos.DTO.Products;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PlanillajeColectivos.Areas.Informes.Controllers
{
    public class InformesController : Controller
    {
        AccountingContext db = new AccountingContext();
        // GET: Informes/Informes
        [Authorize]
        public ActionResult Index()
        {
            //inicio select list año
            List<SelectListItem> anio = new List<SelectListItem>();
            anio.Add(new SelectListItem { Text = "AÑO", Value = "0" });
            int a = DateTime.Now.Year;
            for (int i = 2020; i <= a; i++)
            {
                anio.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
            }
            //.....................

            //inicio select list mes
            List<SelectListItem> mes = new List<SelectListItem>();
            mes.Add(new SelectListItem { Text = "MES", Value = "0" });
            mes.Add(new SelectListItem { Text = "Enero", Value = "1" });
            mes.Add(new SelectListItem { Text = "Febrero", Value = "2" });
            mes.Add(new SelectListItem { Text = "Marzo", Value = "3" });
            mes.Add(new SelectListItem { Text = "Abril", Value = "4" });
            mes.Add(new SelectListItem { Text = "Mayo", Value = "5" });
            mes.Add(new SelectListItem { Text = "Junio", Value = "6" });
            mes.Add(new SelectListItem { Text = "Julio", Value = "7" });
            mes.Add(new SelectListItem { Text = "Agosto", Value = "8" });
            mes.Add(new SelectListItem { Text = "Septiembre", Value = "9" });
            mes.Add(new SelectListItem { Text = "Octubre", Value = "10" });
            mes.Add(new SelectListItem { Text = "Noviembre", Value = "11" });
            mes.Add(new SelectListItem { Text = "Diciembre", Value = "12" });
            //.....................

            //inicio select cuentas
            List<SelectListItem> cuentas = new List<SelectListItem>();
            cuentas.Add(new SelectListItem { Text = "Todas las Cuentas", Value = "0" });
            var cuenta = db.planCuentas.Where(x => x.codigo.Length == 8).ToList();
            foreach (var item in cuenta)
            {
                cuentas.Add(new SelectListItem { Text = item.Nombre + " || " + item.codigo, Value = item.codigo });
            }
            //.........

            //inicio nivel
            List<SelectListItem> niveles = new List<SelectListItem>();
            niveles.Add(new SelectListItem { Text = "Nivel", Value = "0" });
            niveles.Add(new SelectListItem { Text = "1", Value = "1" });
            niveles.Add(new SelectListItem { Text = "2", Value = "2" });
            niveles.Add(new SelectListItem { Text = "3", Value = "3" });
            niveles.Add(new SelectListItem { Text = "4", Value = "4" });
            //....

            var niveles2 = new ConsultasController().GetNiveles();
            var CentroCostos = new ConsultasController().GetCentroCostos();

            ViewBag.anio = anio;
            ViewBag.mes = mes;
            ViewBag.cuentas = cuentas;
            ViewBag.niveles = niveles;
            ViewBag.niveles2 = niveles2;
            ViewBag.CentroCostos = CentroCostos;

            return View();
        }

        public string GetMes(int mes)
        {
            string m = "";

            if (mes == 1) { m = "ENERO"; }
            else if (mes == 2) { m = "FEBRERO"; }
            else if (mes == 3) { m = "MARZO"; }
            else if (mes == 4) { m = "ABRIL"; }
            else if (mes == 5) { m = "MAYO"; }
            else if (mes == 6) { m = "JUNIO"; }
            else if (mes == 7) { m = "JULIO"; }
            else if (mes == 8) { m = "AGOSTO"; }
            else if (mes == 9) { m = "SEPTIEMBRE"; }
            else if (mes == 10) { m = "OCTUBRE"; }
            else if (mes == 11) { m = "NOVIEMBRE"; }
            else if (mes == 12) { m = "DICIEMBRE"; }

            return m;
        }

        public decimal GetEstadoResultado(int anio, int mes)
        {

            decimal saldo4 = 0, saldo5 = 0, saldo6 = 0;
            var cuentas = db.movimientos.Where(x => (x.cuenta.StartsWith("4") || x.cuenta.StartsWith("5") || x.cuenta.StartsWith("6")) && x.fechaCreado.Year == anio && x.fechaCreado.Month <= mes).OrderBy(x => x.cuenta).Select(x => x.cuenta).Distinct().ToList();
            foreach (var item in cuentas)
            {
                var dataCuenta = db.planCuentas.Where(X => X.codigo == item).FirstOrDefault(); //naturaleza de la cuenta
                var naturaleza = dataCuenta.naturaleza;
                var nomCuenta = dataCuenta.Nombre;
                decimal debito = 0, credito = 0, saldo = 0;

                var cuentas2 = db.movimientos.Where(x => x.cuenta == item && x.fechaCreado.Year == anio && x.fechaCreado.Month <= mes).ToList();

                debito = cuentas2.Sum(x => x.debito);
                credito = cuentas2.Sum(x => x.credito);
                if (naturaleza == "D")
                {
                    saldo = (debito - credito);
                }
                else
                {
                    saldo = (credito - debito);
                }


            }

            decimal credit4 = 0, credit5 = 0, credit6 = 0, debit4 = 0, debit5 = 0, debit6 = 0;

            var credito4 = db.movimientos.Where(x => x.cuenta.StartsWith("4") && x.fechaCreado.Year == anio && x.fechaCreado.Month <= mes).ToList();
            if (credito4 != null)
            {
                credit4 = credito4.Sum(x => x.credito);
                debit4 = credito4.Sum(x => x.debito);
            }

            var credito5 = db.movimientos.Where(x => x.cuenta.StartsWith("5") && x.fechaCreado.Year == anio && x.fechaCreado.Month <= mes).ToList();
            if (credito5 != null)
            {
                credit5 = credito5.Sum(x => x.credito);
                debit5 = credito5.Sum(x => x.debito);
            }

            var credito6 = db.movimientos.Where(x => x.cuenta.StartsWith("6") && x.fechaCreado.Year == anio && x.fechaCreado.Month <= mes).ToList();
            if (credito6 != null)
            {
                credit6 = credito6.Sum(x => x.credito);
                debit6 = credito6.Sum(x => x.debito);
            }

            saldo4 = credit4 - debit4;
            saldo5 = debit5 - credit5;
            saldo6 = debit6 - credit6;

            decimal total = saldo4 - saldo5 - saldo6;

            return total;

        }
        [Authorize]
        public ActionResult Excel(FormCollection coll)
        {
            NumberFormatInfo formato = new CultureInfo("es-CO").NumberFormat;

            formato.CurrencyGroupSeparator = ".";
            formato.NumberDecimalSeparator = ",";

            string fechDesde = (coll["fechDesde"]);
            string fechHasta = (coll["fechHasta"]);
            string nivel = (coll["nivel"]);
            var anio = Int32.Parse(coll["anio"]);
            var mes = Int32.Parse(coll["mes"]);
            var informe = Int32.Parse(coll["informe"]);
            var cuenta = coll["cuenta"];
            var chkTercero = coll["chkTercero"];
            var nivel2 = coll["nivel2"];
            var costo = coll["costo"];
            var archivo = "";

            if (informe == 1) archivo = "attachment;filename=BalanceDeComprobacion.xlsx";
            else if (informe == 2) archivo = "attachment;filename=EstadoDeResultados.xlsx";
            else if (informe == 3) archivo = "attachment;filename=BalanceGeneral.xlsx";
            else if (informe == 4) archivo = "attachment;filename=Facturacion.xlsx";
            else if (informe == 5) archivo = "attachment;filename=Abastecimientos.xlsx";
            else if (informe == 6) archivo = "attachment;filename=LibroAuxiliar.xlsx";
            else if (informe == 8) archivo = "attachment;filename=AuxiliarPorCuenta.xlsx";

            Response.Clear();
            Response.ClearContent();
            Response.ClearHeaders();
            Response.Buffer = true;
            Response.ContentEncoding = System.Text.Encoding.UTF8;
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", archivo);

            using (ExcelPackage pack = new ExcelPackage())
            {
                if (informe == 1)
                {
                    #region BALANCE DE COMPROBACION


                    var ws = pack.Workbook.Worksheets.Add("Balance De Comprobación");
                    if (chkTercero == "on")
                    {
                        ws.Cells["A" + 2].Value = "LUIS BOLIVAR SERRANO QUISTANCHALA";
                        ws.Cells["A" + 3].Value = "BALANCE DE COMPROBACIÓN";

                        ws.Cells["A2:J2"].Merge = true;//une columnas en una fila
                        ws.Cells["A3:J3"].Merge = true;

                        ws.Cells["B" + 5].Value = "CUENTA";
                        ws.Cells["C" + 5].Value = "NOMBRE CUENTA";
                        ws.Cells["E" + 5].Value = "DOCUMENTO TERCERO";
                        ws.Cells["F" + 5].Value = "NOMBRE TERCERO";
                        ws.Cells["G" + 5].Value = "SALDO INICIAL";
                        ws.Cells["H" + 5].Value = "DÉBITO";
                        ws.Cells["I" + 5].Value = "CRÉDITO";
                        ws.Cells["J" + 5].Value = "SALDO";
                    }
                    else
                    {
                        ws.Cells["A" + 2].Value = "LUIS BOLIVAR SERRANO QUISTANCHALA";
                        ws.Cells["A" + 3].Value = "BALANCE DE COMPROBACIÓN";

                        ws.Cells["A2:H2"].Merge = true;//une columnas en una fila
                        ws.Cells["A3:H3"].Merge = true;

                        ws.Cells["B" + 5].Value = "CUENTA";
                        ws.Cells["C" + 5].Value = "NOMBRE CUENTA";
                        ws.Cells["E" + 5].Value = "SALDO INICIAL";
                        ws.Cells["F" + 5].Value = "DÉBITO";
                        ws.Cells["G" + 5].Value = "CRÉDITO";
                        ws.Cells["H" + 5].Value = "SALDO";
                    }

                    int j = 7;
                    List<MovimientoAuxiliar> movtosSaldos = new List<MovimientoAuxiliar>();
                    List<MovimientoAuxiliar> movtosActuales = new List<MovimientoAuxiliar>();
                    List<planCuentas> auxiliar = new List<planCuentas>();

                    if (fechDesde != "" && fechHasta != "")
                    {
                        DateTime fh = Convert.ToDateTime(fechHasta);
                        DateTime fd = Convert.ToDateTime(fechDesde);
                        DateTime fechaHasta = new DateTime(fh.Year, fh.Month, fh.Day, 23, 59, 59);
                        DateTime fechaDesde = new DateTime(fd.Year, fd.Month, fd.Day, 0, 0, 0);

                        movtosActuales = db.Database.SqlQuery<MovimientoAuxiliar>(
                            "dbo.sp_BalanceComprobacion @fecha",
                            new SqlParameter("@fecha", fechHasta)
                            ).ToList();
                        //movtosActuales = db.Movimientos.Where(x => x.FECHAMOVIMIENTO <= fechHasta && x.Comprobante.ANULADO == false).ToList(); 267621 267545 
                        if (costo != "")
                        {
                            movtosActuales = movtosActuales.Where(x => x.CCOSTO == costo).ToList();
                        }
                        movtosSaldos = movtosActuales.Where(x => x.FECHAMOVIMIENTO < fechaDesde).ToList();
                        movtosActuales = movtosActuales.Where(x => x.FECHAMOVIMIENTO >= fechaDesde).ToList();

                    }
                    else if (fechDesde != "")
                    {

                        DateTime fd = Convert.ToDateTime(fechDesde);
                        DateTime fechaDesde = new DateTime(fd.Year, fd.Month, fd.Day, 0, 0, 0);
                        movtosActuales = db.Database.SqlQuery<MovimientoAuxiliar>(
                            "dbo.sp_BalanceComprobacion @fecha",
                            new SqlParameter("@fecha", fechDesde)
                            ).ToList();
                        //movtosActuales = db.Movimientos.Where(x => x.FECHAMOVIMIENTO == fechDesde && x.Comprobante.ANULADO == false).ToList();
                        if (costo != "")
                        {
                            movtosActuales = movtosActuales.Where(x => x.CCOSTO == costo).ToList();
                        }
                        movtosSaldos = movtosActuales.Where(x => x.FECHAMOVIMIENTO < fechaDesde).ToList();

                    }

                    var cuentas = db.planCuentas.ToList();

                    //nivel 1
                    if (nivel2 == "1")
                    {
                        auxiliar = cuentas.Where(x => x.codigo.Length == 1).ToList();
                    }
                    else if (nivel2 == "2")
                    {
                        auxiliar = cuentas.Where(x => x.codigo.Length == 1 || x.codigo.Length == 2).ToList();
                    }
                    else if (nivel2 == "3")
                    {
                        auxiliar = cuentas.Where(x => x.codigo.Length == 1 || x.codigo.Length == 2 || x.codigo.Length == 4).ToList();
                    }
                    else if (nivel2 == "4")
                    {
                        auxiliar = cuentas.Where(x => x.codigo.Length == 1 || x.codigo.Length == 2 || x.codigo.Length == 4 || x.codigo.Length == 6).ToList();
                    }
                    else if (nivel2 == "5")
                    {
                        auxiliar = cuentas.Where(x => x.codigo.Length == 1 || x.codigo.Length == 2 || x.codigo.Length == 4 || x.codigo.Length == 6 || x.codigo.Length == 9).ToList();
                    }

                    List<spBalanceComprobacionL5> slPC = new List<spBalanceComprobacionL5>();
                    DateTime fdAuxilar = Convert.ToDateTime(fechDesde);
                    DateTime fDesdeAuxiliar = new DateTime(fdAuxilar.Year, 1, 1, 0, 0, 0);

                    if (auxiliar.Count > 0)
                    {

                        if (chkTercero != "on")
                        {
                            decimal saldoInicial = 0, debitoActual = 0, creditoActual = 0;
                            decimal debitoAnterior = 0; decimal creditoAnterior = 0;
                            decimal saldo = 0;
                            foreach (var item2 in auxiliar)
                            {
                                var dataActual = movtosActuales.Where(x => x.CUENTA.StartsWith(item2.codigo)).ToList();
                                var dataAnterior = movtosSaldos.Where(x => x.CUENTA.StartsWith(item2.codigo)).ToList();
                                var dataAnteriorAuxiliar = dataAnterior.Where(x => x.FECHAMOVIMIENTO >= fDesdeAuxiliar).ToList();

                                if (dataActual.Count != 0 || dataAnterior.Count != 0)
                                {

                                    if (dataAnterior.Count == 0 || item2.codigo.StartsWith("4") || item2.codigo.StartsWith("5") || item2.codigo.StartsWith("6") || item2.codigo.StartsWith("7"))
                                    {
                                        debitoAnterior = dataAnteriorAuxiliar.Select(x => x.DEBITO).Sum();
                                        creditoAnterior = dataAnteriorAuxiliar.Select(x => x.CREDITO).Sum();
                                    }
                                    else
                                    {
                                        debitoAnterior = dataAnterior.Select(x => x.DEBITO).Sum();
                                        creditoAnterior = dataAnterior.Select(x => x.CREDITO).Sum();
                                    }

                                    debitoActual = dataActual.Select(x => x.DEBITO).Sum();
                                    creditoActual = dataActual.Select(x => x.CREDITO).Sum();

                                    if (item2.naturaleza == "D")
                                    {
                                        saldoInicial = debitoAnterior - creditoAnterior;
                                        saldo = (debitoActual - creditoActual) + saldoInicial;
                                    }
                                    else
                                    {
                                        saldoInicial = creditoAnterior - debitoAnterior;
                                        saldo = (creditoActual - debitoActual) + saldoInicial;
                                    }

                                    var objeto = new spBalanceComprobacionL5()
                                    {
                                        codigo = item2.codigo,
                                        nombre = item2.Nombre,
                                        SaldoInicial = saldoInicial.ToString("N0", formato),
                                        Debito = debitoActual.ToString("N0", formato),
                                        Credito = creditoActual.ToString("N0", formato),
                                        Saldo = saldo.ToString("N0", formato)
                                    };
                                    slPC.Add(objeto);

                                    //j++;
                                }

                            }
                        }
                        else
                        {
                            decimal saldoInicial = 0, debitoActual = 0, creditoActual = 0;
                            decimal debitoAnterior = 0; decimal creditoAnterior = 0;
                            decimal saldo = 0;
                            foreach (var item2 in auxiliar)
                            {
                                var dataActual = movtosActuales.Where(x => x.CUENTA.StartsWith(item2.codigo)).ToList();
                                var dataAnterior = movtosSaldos.Where(x => x.CUENTA.StartsWith(item2.codigo)).ToList();
                                var dataAnteriorAuxiliar = dataAnterior.Where(x => x.FECHAMOVIMIENTO >= fDesdeAuxiliar).ToList();

                                if (dataActual.Count != 0 || dataAnterior.Count != 0)
                                {
                                    if (item2.codigo.Length != 9)
                                    {
                                        if (dataAnterior.Count == 0 || item2.codigo.StartsWith("4") || item2.codigo.StartsWith("5") || item2.codigo.StartsWith("6") || item2.codigo.StartsWith("7"))
                                        {
                                            debitoAnterior = dataAnteriorAuxiliar.Select(x => x.DEBITO).Sum();
                                            creditoAnterior = dataAnteriorAuxiliar.Select(x => x.CREDITO).Sum();
                                        }
                                        else
                                        {
                                            debitoAnterior = dataAnterior.Select(x => x.DEBITO).Sum();
                                            creditoAnterior = dataAnterior.Select(x => x.CREDITO).Sum();
                                        }

                                        debitoActual = dataActual.Select(x => x.DEBITO).Sum();
                                        creditoActual = dataActual.Select(x => x.CREDITO).Sum();

                                        if (item2.codigo == "D")
                                        {
                                            saldoInicial = debitoAnterior - creditoAnterior;
                                            saldo = (debitoActual - creditoActual) + saldoInicial;
                                        }
                                        else
                                        {
                                            saldoInicial = creditoAnterior - debitoAnterior;
                                            saldo = (creditoActual - debitoActual) + saldoInicial;
                                        }


                                        //ws.Cells["G" + j].Value = saldoInicial.ToString("N0", formato);
                                        //ws.Cells["H" + j].Value = debitoActual.ToString("N0", formato);
                                        //ws.Cells["I" + j].Value = creditoActual.ToString("N0", formato);
                                        //ws.Cells["J" + j].Value = saldo.ToString("N0", formato);

                                        var objeto = new spBalanceComprobacionL5()
                                        {
                                            codigo = item2.codigo,
                                            nombre = item2.codigo,
                                            SaldoInicial = saldoInicial.ToString("N0", formato),
                                            Debito = debitoActual.ToString("N0", formato),
                                            Credito = creditoActual.ToString("N0", formato),
                                            Saldo = saldo.ToString("N0", formato)
                                        };
                                        slPC.Add(objeto);

                                        //j++;
                                    }//fin if != 9
                                    else
                                    {
                                        if (dataAnterior.Count == 0 || item2.codigo.StartsWith("4") || item2.codigo.StartsWith("5") || item2.codigo.StartsWith("6") || item2.codigo.StartsWith("7"))
                                        {
                                            var info = (from da in dataActual
                                                        select new { da.TERCERO, da.CUENTA, da.NOMBRE }
                                                        ).OrderBy(x => x.CUENTA).Distinct();

                                            foreach (var item3 in info)
                                            {
                                                var actual = dataActual.Where(x => x.TERCERO == item3.TERCERO && x.CUENTA == item2.codigo).ToList();
                                                var anterior = dataAnteriorAuxiliar.Where(x => x.TERCERO == item3.TERCERO && x.CUENTA == item2.codigo).ToList();


                                                debitoAnterior = anterior.Select(x => x.DEBITO).Sum();
                                                creditoAnterior = anterior.Select(x => x.CREDITO).Sum();

                                                debitoActual = actual.Select(x => x.DEBITO).Sum();
                                                creditoActual = actual.Select(x => x.CREDITO).Sum();


                                                if (item2.naturaleza == "D")
                                                {
                                                    saldoInicial = debitoAnterior - creditoAnterior;
                                                    saldo = (debitoActual - creditoActual) + saldoInicial;
                                                }
                                                else
                                                {
                                                    saldoInicial = creditoAnterior - debitoAnterior;
                                                    saldo = (creditoActual - debitoActual) + saldoInicial;
                                                }

                                                var objeto = new spBalanceComprobacionL5()
                                                {
                                                    codigo = item2.codigo,
                                                    nombre = item2.Nombre,
                                                    DocumentoTercero = item3.TERCERO,
                                                    NombreTercero = item3.NOMBRE,
                                                    SaldoInicial = saldoInicial.ToString("N0", formato),
                                                    Debito = debitoActual.ToString("N0", formato),
                                                    Credito = creditoActual.ToString("N0", formato),
                                                    Saldo = saldo.ToString("N0", formato)
                                                };
                                                slPC.Add(objeto);

                                                //j++;

                                            }

                                        }
                                        else
                                        {
                                            var info = (from da in dataActual
                                                        select new { da.TERCERO, da.CUENTA, da.NOMBRE }
                                                        ).OrderBy(x => x.CUENTA).Distinct();

                                            foreach (var item3 in info)
                                            {
                                                var actual = dataActual.Where(x => x.TERCERO == item3.TERCERO && x.CUENTA == item2.codigo).ToList();
                                                var anterior = dataAnterior.Where(x => x.TERCERO == item3.TERCERO && x.CUENTA == item2.codigo).ToList();


                                                debitoAnterior = anterior.Select(x => x.DEBITO).Sum();
                                                creditoAnterior = anterior.Select(x => x.CREDITO).Sum();

                                                debitoActual = actual.Select(x => x.DEBITO).Sum();
                                                creditoActual = actual.Select(x => x.CREDITO).Sum();


                                                if (item2.naturaleza == "D")
                                                {
                                                    saldoInicial = debitoAnterior - creditoAnterior;
                                                    saldo = (debitoActual - creditoActual) + saldoInicial;
                                                }
                                                else
                                                {
                                                    saldoInicial = creditoAnterior - debitoAnterior;
                                                    saldo = (creditoActual - debitoActual) + saldoInicial;
                                                }

                                                var objeto = new spBalanceComprobacionL5()
                                                {
                                                    codigo = item2.codigo,
                                                    nombre = item2.Nombre,
                                                    DocumentoTercero = item3.TERCERO,
                                                    NombreTercero = item3.NOMBRE,
                                                    SaldoInicial = saldoInicial.ToString("N0", formato),
                                                    Debito = debitoActual.ToString("N0", formato),
                                                    Credito = creditoActual.ToString("N0", formato),
                                                    Saldo = saldo.ToString("N0", formato)
                                                };
                                                slPC.Add(objeto);

                                                //j++;

                                            }
                                        }


                                    }//fin else != 9

                                }//fin if dataActual != 0

                            }//fin for item2
                        }


                    }

                    if (chkTercero != "on")
                    {
                        foreach (var ob in slPC)
                        {
                            ws.Cells["B" + j].Value = ob.codigo;
                            ws.Cells["C" + j].Value = ob.nombre;
                            ws.Cells["E" + j].Value = ob.SaldoInicial;
                            ws.Cells["F" + j].Value = ob.Debito;
                            ws.Cells["G" + j].Value = ob.Credito;
                            ws.Cells["H" + j].Value = ob.Saldo;
                            j++;
                        }
                    }
                    else
                    {
                        foreach (var ob in slPC)
                        {
                            ws.Cells["B" + j].Value = ob.codigo;
                            ws.Cells["C" + j].Value = ob.nombre;
                            ws.Cells["E" + j].Value = ob.DocumentoTercero;
                            ws.Cells["F" + j].Value = ob.NombreTercero;
                            ws.Cells["G" + j].Value = ob.SaldoInicial;
                            ws.Cells["H" + j].Value = ob.Debito;
                            ws.Cells["I" + j].Value = ob.Credito;
                            ws.Cells["J" + j].Value = ob.Saldo;
                            j++;
                        }
                    }

                    ws.Cells[ws.Dimension.Address].AutoFitColumns();//siempre al final de todo. le da tamaño ajustado a cada columna
                    movtosActuales = null;
                    movtosSaldos = null;
                    auxiliar = null;

                    #endregion
                }
                else if (informe == 2)
                {
                    ExcelWorksheet ws = pack.Workbook.Worksheets.Add("EstadoDeResultados");
                    ws.Cells["A" + 2].Value = "CUENTA";
                    ws.Cells["B" + 2].Value = "NOMBRE";
                    ws.Cells["C" + 2].Value = "SALDO";
                    int j = 3;

                    if (anio != 0 && mes != 0)
                    {
                        decimal saldo4 = 0, saldo5 = 0, saldo6 = 0;
                        var cuentas = db.movimientos.Where(x => (x.cuenta.StartsWith("4") || x.cuenta.StartsWith("5") || x.cuenta.StartsWith("6")) && x.fechaCreado.Year == anio && x.fechaCreado.Month <= mes).OrderBy(x => x.cuenta).Select(x => x.cuenta).Distinct().ToList();
                        foreach (var item in cuentas)
                        {
                            var dataCuenta = db.planCuentas.Where(X => X.codigo == item).FirstOrDefault(); //naturaleza de la cuenta
                            var naturaleza = dataCuenta.naturaleza;
                            var nomCuenta = dataCuenta.Nombre;
                            decimal debito = 0, credito = 0, saldo = 0;

                            var cuentas2 = db.movimientos.Where(x => x.cuenta == item && x.fechaCreado.Year == anio && x.fechaCreado.Month <= mes).ToList();

                            debito = cuentas2.Sum(x => x.debito);
                            credito = cuentas2.Sum(x => x.credito);
                            if (naturaleza == "D")
                            {
                                saldo = (debito - credito);
                            }
                            else
                            {
                                saldo = (credito - debito);
                            }

                            ws.Cells["A" + j].Value = item;
                            ws.Cells["B" + j].Value = nomCuenta;
                            ws.Cells["C" + j].Value = saldo.ToString("N0", formato);

                            j++;
                        }

                        decimal credit4 = 0, credit5 = 0, credit6 = 0, debit4 = 0, debit5 = 0, debit6 = 0;

                        var credito4 = db.movimientos.Where(x => x.cuenta.StartsWith("4") && x.fechaCreado.Year == anio && x.fechaCreado.Month <= mes).ToList();
                        if (credito4 != null)
                        {
                            credit4 = credito4.Sum(x => x.credito);
                            debit4 = credito4.Sum(x => x.debito);
                        }

                        var credito5 = db.movimientos.Where(x => x.cuenta.StartsWith("5") && x.fechaCreado.Year == anio && x.fechaCreado.Month <= mes).ToList();
                        if (credito5 != null)
                        {
                            credit5 = credito5.Sum(x => x.credito);
                            debit5 = credito5.Sum(x => x.debito);
                        }

                        var credito6 = db.movimientos.Where(x => x.cuenta.StartsWith("6") && x.fechaCreado.Year == anio && x.fechaCreado.Month <= mes).ToList();
                        if (credito6 != null)
                        {
                            credit6 = credito6.Sum(x => x.credito);
                            debit6 = credito6.Sum(x => x.debito);
                        }

                        saldo4 = credit4 - debit4;
                        saldo5 = debit5 - credit5;
                        saldo6 = debit6 - credit6;

                        decimal total = saldo4 - saldo5 - saldo6;

                        ws.Cells["E" + 6].Value = "UTILIDAD O PERDIDA DEL EJERCICIO";
                        ws.Cells["B" + 6].Value = total.ToString("N0", formato);

                    }// fin if



                }
                else if (informe == 3)
                {
                    ExcelWorksheet ws = pack.Workbook.Worksheets.Add("BalanceGeneral");
                    ws.Cells["B" + 2].Value = "CUENTA";
                    ws.Cells["C" + 2].Value = "NOMBRE";
                    ws.Cells["D" + 2].Value = "SALDO";

                    ws.Cells["G" + 2].Value = "CUENTA";
                    ws.Cells["H" + 2].Value = "NOMBRE";
                    ws.Cells["I" + 2].Value = "SALDO";
                    int j = 3;

                    if (anio != 0 && mes != 0)
                    {
                        //procesos para cuentas que comienzan en 1
                        var cuentas = db.movimientos.Where(x => x.cuenta.StartsWith("1") && x.fechaCreado.Year == anio && x.fechaCreado.Month <= mes).OrderBy(x => x.cuenta).Select(x => x.cuenta).Distinct().ToList();
                        if (cuentas != null)
                        {
                            decimal total = 0;
                            foreach (var item in cuentas)
                            {
                                var dataCuenta = db.planCuentas.Where(X => X.codigo == item).FirstOrDefault(); //naturaleza de la cuenta
                                var nomCuenta = dataCuenta.Nombre;
                                decimal debito = 0, credito = 0, saldo = 0;
                                var cuentas2 = db.movimientos.Where(x => x.cuenta == item && x.fechaCreado.Year == anio && x.fechaCreado.Month <= mes).ToList();
                                debito = cuentas2.Sum(x => x.debito);
                                credito = cuentas2.Sum(x => x.credito);
                                saldo = debito - credito;
                                total += saldo;

                                ws.Cells["B" + j].Value = item;
                                ws.Cells["C" + j].Value = nomCuenta;
                                ws.Cells["D" + j].Value = saldo.ToString("N0", formato);

                                j++;
                            } //fin foreach
                            ws.Cells["C" + j].Value = "SUMA ACTIVOS";
                            ws.Cells["D" + j].Value = total.ToString("N0", formato);
                        }//fin if cuentas != null

                        //fin proceso cuentas que comienzan en 1

                        //procesos para cuentas que comienzn en 2 y 3
                        j = 3;
                        var cuentas1 = db.movimientos.Where(x => (x.cuenta.StartsWith("2") || x.cuenta.StartsWith("3")) && x.fechaCreado.Year == anio && x.fechaCreado.Month <= mes).OrderBy(x => x.cuenta).Select(x => x.cuenta).Distinct().ToList();
                        if (cuentas1 != null)
                        {
                            decimal total = 0;
                            foreach (var item in cuentas)
                            {
                                var dataCuenta = db.planCuentas.Where(X => X.codigo == item).FirstOrDefault(); //naturaleza de la cuenta
                                var nomCuenta = dataCuenta.Nombre;
                                decimal debito = 0, credito = 0, saldo = 0;
                                var cuentas2 = db.movimientos.Where(x => x.cuenta == item && x.fechaCreado.Year == anio && x.fechaCreado.Month <= mes).ToList();
                                debito = cuentas2.Sum(x => x.debito);
                                credito = cuentas2.Sum(x => x.credito);
                                saldo = debito - credito;
                                total += saldo;

                                ws.Cells["G" + j].Value = item;
                                ws.Cells["H" + j].Value = nomCuenta;
                                ws.Cells["I" + j].Value = saldo.ToString("N0", formato);

                                j++;
                            } //fin foreach
                            j++;

                            decimal EstResultados = GetEstadoResultado(anio, mes);
                            total += EstResultados;
                            ws.Cells["G" + j].Value = cuenta;
                            ws.Cells["H" + j].Value = "ESTADO DE RESULTADOS";
                            ws.Cells["I" + j].Value = EstResultados.ToString("N0", formato);


                            j++;
                            ws.Cells["H" + j].Value = "PASIVO + PATRIMONIO";
                            ws.Cells["I" + j].Value = total.ToString("N0", formato);
                        }//fin if cuentas != null


                        //fin procesos de cuetnas que comienzan en 2 y 3

                    }//fin año y mes = 0
                }
                else if (informe == 4)
                {
                    ExcelWorksheet ws = pack.Workbook.Worksheets.Add("facturacion");
                    ws.Cells["A" + 2].Value = "NÚMERO FACTURA";
                    ws.Cells["B" + 2].Value = "FECHA GENERADA";
                    ws.Cells["C" + 2].Value = "VENDEDOR";
                    ws.Cells["D" + 2].Value = "TERCERO";
                    ws.Cells["E" + 2].Value = "IVA 19%";
                    ws.Cells["F" + 2].Value = "IVA 5%";
                    ws.Cells["G" + 2].Value = "TIPO";
                    ws.Cells["H" + 2].Value = "SALDO CREDITO";
                    ws.Cells["I" + 2].Value = "TOTAL ANTES DE IVA";
                    ws.Cells["J" + 2].Value = "TOTAL CON IVA";
                    ws.Cells["K" + 2].Value = "HISTORICO";

                    int j = 3;

                    var facturas = db.factura.Where(x => x.operationTypeId == 6 || x.operationTypeId == 15).ToList();

                    if (facturas != null)
                    {
                        if (fechDesde != "")
                        {

                            DateTime desde = Convert.ToDateTime(fechDesde);
                            if (fechHasta != "")
                            {
                                DateTime hasta = Convert.ToDateTime(fechHasta + " " + "23:59:59");
                                facturas = facturas.Where(x => x.date >= desde && x.date <= hasta).ToList();
                            }
                            else
                            {
                                facturas = facturas.Where(x => x.date.Year == desde.Year && x.date.Month == desde.Month && x.date.Day == desde.Day).ToList();
                            }
                        }// fin desde != ""

                        foreach (var item in facturas)
                        {
                            string tipo = "";
                            string historico = "";
                            if (item.tipo == 1) { tipo = "CONTADO"; }
                            else if (item.tipo == 2) { tipo = "CRÉDITO"; }

                            if (item.operationTypeId == 6) { historico = "VENDIDO"; }
                            else if (item.operationTypeId == 15) { historico = "VENDIDO POR CAJA"; }

                            //calculos del iva
                            var dataFac = db.operation.Where(x => x.facturaId == item.id).ToList();
                            decimal iva1 = 0, iva2 = 0, totalAntesIva = 0;
                            foreach (var item2 in dataFac)
                            {
                                if (item2.products.ivaId == 1)
                                {

                                    decimal val = db.iva.Where(x => x.id == 1).Select(x => x.value).FirstOrDefault();
                                    decimal precioBase = item2.price / (1 + (val / 100));
                                    decimal valorIva = item2.price - precioBase;
                                    totalAntesIva += precioBase * item2.quantity;
                                    iva1 += valorIva * item2.quantity;
                                    //decimal valorIva = ((item2.price * val) / 100);
                                    //totalAntesIva += (item2.price - valorIva) * item2.quantity;
                                    //iva1 += valorIva * item2.quantity;

                                }
                                else if (item2.products.ivaId == 2)
                                {
                                    decimal val = db.iva.Where(x => x.id == 2).Select(x => x.value).FirstOrDefault();
                                    decimal precioBase = item2.price / (1 + (val / 100));
                                    decimal valorIva = item2.price - precioBase;
                                    totalAntesIva += precioBase * item2.quantity;
                                    iva2 += valorIva * item2.quantity;
                                    //decimal valorIva = ((item2.price * val) / 100);
                                    //totalAntesIva += (item2.price - valorIva) * item2.quantity;
                                    //iva2 += valorIva;
                                }
                            }
                            //....

                            ws.Cells["A" + j].Value = item.numeroFactura;
                            ws.Cells["B" + j].Value = item.date.ToString();
                            ws.Cells["C" + j].Value = item.usersTabla.nombre + " " + item.usersTabla.apellido;
                            ws.Cells["D" + j].Value = item.persons.name;
                            ws.Cells["E" + j].Value = iva1;
                            ws.Cells["F" + j].Value = iva2;
                            ws.Cells["G" + j].Value = tipo;
                            ws.Cells["H" + j].Value = item.saldoCredito;
                            ws.Cells["I" + j].Value = totalAntesIva;
                            ws.Cells["J" + j].Value = item.total;
                            ws.Cells["K" + j].Value = historico;
                            j++;
                        }//fin foreach


                    }//fin facturas != null



                }
                else if (informe == 5)
                {
                    ExcelWorksheet ws = pack.Workbook.Worksheets.Add("abastecimientos");
                    ws.Cells["A" + 2].Value = "NÚMERO FACTURA";
                    ws.Cells["B" + 2].Value = "FECHA GENERADA";
                    ws.Cells["C" + 2].Value = "VENDEDOR";
                    ws.Cells["D" + 2].Value = "TERCERO";
                    ws.Cells["E" + 2].Value = "IVA 19%";
                    ws.Cells["F" + 2].Value = "IVA 5%";
                    ws.Cells["G" + 2].Value = "TIPO";
                    ws.Cells["H" + 2].Value = "SALDO CREDITO";
                    ws.Cells["I" + 2].Value = "TOTAL ANTES DE IVA";
                    ws.Cells["J" + 2].Value = "TOTAL CON IVA";


                    int j = 3;

                    var facturas = db.factura.Where(x => x.operationTypeId == 2).ToList();

                    if (facturas != null)
                    {
                        if (fechDesde != "")
                        {

                            DateTime desde = Convert.ToDateTime(fechDesde);
                            if (fechHasta != "")
                            {
                                DateTime hasta = Convert.ToDateTime(fechHasta + " " + "23:59:59");
                                facturas = facturas.Where(x => x.date >= desde && x.date <= hasta).ToList();
                            }
                            else
                            {
                                facturas = facturas.Where(x => x.date.Year == desde.Year && x.date.Month == desde.Month && x.date.Day == desde.Day).ToList();
                            }
                        }// fin desde != ""

                        foreach (var item in facturas)
                        {
                            string tipo = "";
                            if (item.tipo == 1) { tipo = "CONTADO"; }
                            else if (item.tipo == 2) { tipo = "CRÉDITO"; }


                            //calculos del iva
                            var dataFac = db.operation.Where(x => x.facturaId == item.id).ToList();
                            decimal iva1 = 0, iva2 = 0, totalAntesIva = 0;
                            foreach (var item2 in dataFac)
                            {
                                if (item2.products.ivaId == 1)
                                {
                                    decimal val = db.iva.Where(x => x.id == 1).Select(x => x.value).FirstOrDefault();
                                    decimal valorIva = ((item2.price * val) / 100);
                                    totalAntesIva += (item2.price - valorIva) * item2.quantity;
                                    iva1 += valorIva * item2.quantity;

                                }
                                else if (item2.products.ivaId == 2)
                                {
                                    decimal val = db.iva.Where(x => x.id == 2).Select(x => x.value).FirstOrDefault();
                                    decimal valorIva = ((item2.price * val) / 100);
                                    totalAntesIva += (item2.price - valorIva) * item2.quantity;
                                    iva2 += valorIva;
                                }
                            }
                            //....

                            ws.Cells["A" + j].Value = item.id;
                            ws.Cells["B" + j].Value = item.date.ToString();
                            ws.Cells["C" + j].Value = item.usersTabla.nombre + " " + item.usersTabla.apellido;
                            ws.Cells["D" + j].Value = item.persons.name;
                            ws.Cells["E" + j].Value = iva1;
                            ws.Cells["F" + j].Value = iva2;
                            ws.Cells["G" + j].Value = tipo;
                            ws.Cells["H" + j].Value = item.saldoCredito;
                            ws.Cells["I" + j].Value = totalAntesIva;
                            ws.Cells["J" + j].Value = item.total;

                            j++;
                        }//fin foreach


                    }//fin facturas != null
                }
                else if (informe == 6)
                {
                    ExcelWorksheet ws = pack.Workbook.Worksheets.Add("LibroAuxiliar");
                    ws.Cells["A" + 2].Value = "CUENTA";
                    ws.Cells["B" + 2].Value = "NOMBRE";
                    ws.Cells["C" + 2].Value = "DEBITO";
                    ws.Cells["D" + 2].Value = "CREDITO";
                    int j = 4;

                    if (nivel != "0" && anio != 0)
                    {
                        var cuentas = db.planCuentas.ToList();
                        var movimientos = db.movimientos.ToList();
                        if (nivel == "1")
                        {
                            cuentas = cuentas.Where(x => x.codigo.Length == 1).Distinct().ToList();
                        }
                        else if (nivel == "2")
                        {
                            cuentas = cuentas.Where(x => x.codigo.Length == 2).Distinct().ToList();
                        }
                        else if (nivel == "3")
                        {
                            cuentas = cuentas.Where(x => x.codigo.Length == 4).Distinct().ToList();
                        }
                        else if (nivel == "4")
                        {
                            cuentas = cuentas.Where(x => x.codigo.Length == 6).Distinct().ToList();
                        }

                        if (mes != 0)
                        {
                            movimientos = movimientos.Where(x => x.fechaCreado.Year == anio && x.fechaCreado.Month <= mes).ToList();
                        }
                        else
                        {
                            movimientos = movimientos.Where(x => x.fechaCreado.Year == anio).ToList();
                        }

                        if (nivel == "1")
                        {
                            foreach (var item1 in cuentas)
                            {
                                string cuen = ""; string nom = ""; decimal debito = 0, credito = 0, totalDebito = 0, totalCredito = 0;

                                int n = movimientos.Where(x => x.cuenta.StartsWith(item1.codigo)).Count();
                                if (n > 0)
                                {
                                    var data = movimientos.Where(x => x.cuenta == item1.codigo).ToList();

                                    if (data != null)
                                    {
                                        cuen = item1.codigo;
                                        nom = item1.Nombre;
                                        debito = data.Select(x => x.debito).Sum();
                                        credito = data.Select(x => x.credito).Sum();

                                        totalDebito += debito;
                                        totalCredito += credito;
                                    }

                                    ws.Cells["A" + j].Value = cuen;
                                    ws.Cells["B" + j].Value = nom;
                                    ws.Cells["C" + j].Value = debito.ToString("N3", formato);
                                    ws.Cells["D" + j].Value = credito.ToString("N3", formato);

                                    j += 2;

                                }



                                var cuentas2 = db.planCuentas.Where(x => x.codigo.Length == 2 && x.codigo.StartsWith(item1.codigo)).OrderBy(x => x.codigo).Distinct().ToList();
                                foreach (var item2 in cuentas2)
                                {
                                    int num = movimientos.Where(x => x.cuenta.StartsWith(item2.codigo)).Count();


                                    if (num > 0)
                                    {
                                        var data2 = movimientos.Where(x => x.cuenta.StartsWith(item2.codigo)).ToList();
                                        string nomCuenta = ""; decimal debiCuenta = 0, crediCuenta = 0;
                                        nomCuenta = db.planCuentas.Where(x => x.codigo == item2.codigo).Select(x => x.Nombre).FirstOrDefault();
                                        debiCuenta = data2.Select(x => x.debito).Sum();
                                        crediCuenta = data2.Select(x => x.credito).Sum();
                                        totalDebito += debiCuenta;
                                        totalCredito += crediCuenta;

                                        ws.Cells["A" + j].Value = item2.codigo;
                                        ws.Cells["B" + j].Value = nomCuenta;
                                        ws.Cells["C" + j].Value = debiCuenta.ToString("N3", formato);
                                        ws.Cells["D" + j].Value = crediCuenta.ToString("N3", formato);
                                        j++;

                                    }


                                }
                                if (totalCredito != 0 || totalDebito != 0)
                                {
                                    ws.Cells["B" + j].Value = "TOTAL";
                                    ws.Cells["C" + j].Value = totalDebito.ToString("N3", formato);
                                    ws.Cells["D" + j].Value = totalCredito.ToString("N3", formato);
                                    j += 3;
                                }



                            }
                        }
                        else if (nivel == "2")
                        {
                            foreach (var item1 in cuentas)
                            {
                                string cuen = ""; string nom = ""; decimal debito = 0, credito = 0, totalDebito = 0, totalCredito = 0;

                                int n = movimientos.Where(x => x.cuenta.StartsWith(item1.codigo)).Count();
                                if (n > 0)
                                {
                                    var data = movimientos.Where(x => x.cuenta == item1.codigo).ToList();

                                    if (data != null)
                                    {
                                        cuen = item1.codigo;
                                        nom = item1.Nombre;
                                        debito = data.Select(x => x.debito).Sum();
                                        credito = data.Select(x => x.credito).Sum();

                                        totalDebito += debito;
                                        totalCredito += credito;
                                    }

                                    ws.Cells["A" + j].Value = cuen;
                                    ws.Cells["B" + j].Value = nom;
                                    ws.Cells["C" + j].Value = debito.ToString("N3", formato);
                                    ws.Cells["D" + j].Value = credito.ToString("N3", formato);

                                    j += 2;

                                }



                                var cuentas2 = db.planCuentas.Where(x => x.codigo.Length == 4 && x.codigo.StartsWith(item1.codigo)).OrderBy(x => x.codigo).Distinct().ToList();
                                foreach (var item2 in cuentas2)
                                {
                                    int num = movimientos.Where(x => x.cuenta.StartsWith(item2.codigo)).Count();


                                    if (num > 0)
                                    {
                                        var data2 = movimientos.Where(x => x.cuenta.StartsWith(item2.codigo)).ToList();
                                        string nomCuenta = ""; decimal debiCuenta = 0, crediCuenta = 0;
                                        nomCuenta = db.planCuentas.Where(x => x.codigo == item2.codigo).Select(x => x.Nombre).FirstOrDefault();
                                        debiCuenta = data2.Select(x => x.debito).Sum();
                                        crediCuenta = data2.Select(x => x.credito).Sum();
                                        totalDebito += debiCuenta;
                                        totalCredito += crediCuenta;

                                        ws.Cells["A" + j].Value = item2.codigo;
                                        ws.Cells["B" + j].Value = nomCuenta;
                                        ws.Cells["C" + j].Value = debiCuenta.ToString("N3", formato);
                                        ws.Cells["D" + j].Value = crediCuenta.ToString("N3", formato);
                                        j++;

                                    }


                                }
                                if (totalCredito != 0 || totalDebito != 0)
                                {
                                    ws.Cells["B" + j].Value = "TOTAL";
                                    ws.Cells["C" + j].Value = totalDebito.ToString("N3", formato);
                                    ws.Cells["D" + j].Value = totalCredito.ToString("N3", formato);
                                    j += 3;
                                }



                            }

                        }
                        else if (nivel == "3")
                        {
                            foreach (var item1 in cuentas)
                            {
                                string cuen = ""; string nom = ""; decimal debito = 0, credito = 0, totalDebito = 0, totalCredito = 0;


                                int n = movimientos.Where(x => x.cuenta.StartsWith(item1.codigo)).Count();

                                if (n > 0)
                                {
                                    var data = movimientos.Where(x => x.cuenta == item1.codigo).ToList();

                                    if (data != null)
                                    {
                                        cuen = item1.codigo;
                                        nom = item1.Nombre;
                                        debito = data.Select(x => x.debito).Sum();
                                        credito = data.Select(x => x.credito).Sum();

                                        totalDebito += debito;
                                        totalCredito += credito;
                                    }

                                    ws.Cells["A" + j].Value = cuen;
                                    ws.Cells["B" + j].Value = nom;
                                    ws.Cells["C" + j].Value = debito.ToString("N3", formato);
                                    ws.Cells["D" + j].Value = credito.ToString("N3", formato);

                                    j += 2;
                                }



                                var cuentas2 = db.planCuentas.Where(x => x.codigo.Length == 6 && x.codigo.StartsWith(item1.codigo)).OrderBy(x => x.codigo).Distinct().ToList();
                                foreach (var item2 in cuentas2)
                                {
                                    int num = movimientos.Where(x => x.cuenta.StartsWith(item2.codigo)).Count();


                                    if (num > 0)
                                    {
                                        var data2 = movimientos.Where(x => x.cuenta.StartsWith(item2.codigo)).ToList();
                                        string nomCuenta = ""; decimal debiCuenta = 0, crediCuenta = 0;
                                        nomCuenta = db.planCuentas.Where(x => x.codigo == item2.codigo).Select(x => x.Nombre).FirstOrDefault();
                                        debiCuenta = data2.Select(x => x.debito).Sum();
                                        crediCuenta = data2.Select(x => x.credito).Sum();
                                        totalDebito += debiCuenta;
                                        totalCredito += crediCuenta;

                                        ws.Cells["A" + j].Value = item2.codigo;
                                        ws.Cells["B" + j].Value = nomCuenta;
                                        ws.Cells["C" + j].Value = debiCuenta.ToString("N3", formato);
                                        ws.Cells["D" + j].Value = crediCuenta.ToString("N3", formato);
                                        j++;

                                    }


                                }
                                if (totalCredito != 0 || totalDebito != 0)
                                {
                                    ws.Cells["B" + j].Value = "TOTAL";
                                    ws.Cells["C" + j].Value = totalDebito.ToString("N3", formato);
                                    ws.Cells["D" + j].Value = totalCredito.ToString("N3", formato);
                                    j += 3;
                                }


                            }
                        }
                        else if (nivel == "4")
                        {
                            foreach (var item1 in cuentas)
                            {
                                string cuen = ""; string nom = ""; decimal debito = 0, credito = 0, totalDebito = 0, totalCredito = 0;


                                int n = movimientos.Where(x => x.cuenta.StartsWith(item1.codigo)).Count();

                                if (n > 0)
                                {
                                    var data = movimientos.Where(x => x.cuenta == item1.codigo).ToList();

                                    if (data != null)
                                    {
                                        cuen = item1.codigo;
                                        nom = item1.Nombre;
                                        debito = data.Select(x => x.debito).Sum();
                                        credito = data.Select(x => x.credito).Sum();

                                        totalDebito += debito;
                                        totalCredito += credito;
                                    }

                                    ws.Cells["A" + j].Value = cuen;
                                    ws.Cells["B" + j].Value = nom;
                                    ws.Cells["C" + j].Value = debito.ToString("N3", formato);
                                    ws.Cells["D" + j].Value = credito.ToString("N3", formato);

                                    j += 2;
                                }



                                var cuentas2 = db.planCuentas.Where(x => x.codigo.Length == 8 && x.codigo.StartsWith(item1.codigo)).OrderBy(x => x.codigo).Distinct().ToList();
                                foreach (var item2 in cuentas2)
                                {
                                    int num = movimientos.Where(x => x.cuenta.StartsWith(item2.codigo)).Count();


                                    if (num > 0)
                                    {
                                        var data2 = movimientos.Where(x => x.cuenta.StartsWith(item2.codigo)).ToList();
                                        string nomCuenta = ""; decimal debiCuenta = 0, crediCuenta = 0;
                                        nomCuenta = db.planCuentas.Where(x => x.codigo == item2.codigo).Select(x => x.Nombre).FirstOrDefault();
                                        debiCuenta = data2.Select(x => x.debito).Sum();
                                        crediCuenta = data2.Select(x => x.credito).Sum();
                                        totalDebito += debiCuenta;
                                        totalCredito += crediCuenta;

                                        ws.Cells["A" + j].Value = item2.codigo;
                                        ws.Cells["B" + j].Value = nomCuenta;
                                        ws.Cells["C" + j].Value = debiCuenta.ToString("N3", formato);
                                        ws.Cells["D" + j].Value = crediCuenta.ToString("N3", formato);
                                        j++;

                                    }


                                }
                                if (totalCredito != 0 || totalDebito != 0)
                                {
                                    ws.Cells["B" + j].Value = "TOTAL";
                                    ws.Cells["C" + j].Value = totalDebito.ToString("N3", formato);
                                    ws.Cells["D" + j].Value = totalCredito.ToString("N3", formato);
                                    j += 3;
                                }


                            }
                        }


                    }//fin nivel != 0

                }
                else if (informe == 7)
                {
                    var movimientosOriginal = db.movimientos.ToList();
                    var movimientos = movimientosOriginal;
                    if (cuenta != "0")
                    {
                        movimientos = movimientos.Where(x => x.cuenta == cuenta).ToList();
                    }
                    if (fechDesde != "")
                    {
                        DateTime afd = Convert.ToDateTime(fechDesde);
                        DateTime FD = new DateTime(afd.Year, afd.Month, afd.Day, 0, 0, 0);
                        movimientos = movimientos.Where(x => x.fechaCreado >= FD).ToList();

                        if (fechHasta != "")
                        {
                            DateTime afh = Convert.ToDateTime(fechHasta);
                            DateTime FH = new DateTime(afh.Year, afh.Month, afh.Day, 23, 59, 59);
                            movimientos = movimientos.Where(x => x.fechaCreado <= FH).ToList();
                            movimientosOriginal = movimientosOriginal.Where(x => x.fechaCreado <= FH).ToList();
                        }

                    }

                    var cuentasTerceros = (from ct in movimientos
                                           select new { ct.cuenta, ct.terceroId, ct.persons }).Distinct().OrderBy(x => x.cuenta).ThenBy(x => x.terceroId).ToList();


                    ExcelWorksheet ws = pack.Workbook.Worksheets.Add("Auxiliar Por Tercero Detallado");
                    ws.Cells["A" + 1].Value = "CUENTA";
                    ws.Cells["B" + 1].Value = "TIPO";
                    ws.Cells["C" + 1].Value = "NÚMERO";
                    ws.Cells["D" + 1].Value = "FECHA";
                    ws.Cells["E" + 1].Value = "TERCERO";
                    ws.Cells["F" + 1].Value = "NOMBRE";
                    ws.Cells["G" + 1].Value = "DETALLE";
                    ws.Cells["H" + 1].Value = "SALDO ANTERIOR";
                    ws.Cells["I" + 1].Value = "DÉBITO";
                    ws.Cells["J" + 1].Value = "CRÉDITO";
                    ws.Cells["K" + 1].Value = "SALDO";
                    ws.Cells["L" + 1].Value = "BASE";

                    int i = 2;

                    foreach (var item in cuentasTerceros)
                    {
                        decimal credito = 0, debito = 0, saldo = 0, saldoAnterior = 0;
                        var datos = movimientosOriginal.Where(x => x.cuenta == item.cuenta && x.terceroId == item.terceroId).OrderBy(x => x.fechaCreado).ToList();
                        var ultimaTupla = datos.OrderByDescending(x => x.fechaCreado).FirstOrDefault();
                        string nomTercero = item.persons.name;

                        foreach (var item2 in datos)
                        {
                            saldoAnterior = saldo;

                            debito = item2.debito;
                            credito = item2.credito;

                            if (item2.cuenta.StartsWith("1") || item2.cuenta.StartsWith("5") || item2.cuenta.StartsWith("6") || item2.cuenta.StartsWith("7") || item2.cuenta.StartsWith("9"))
                            {
                                saldo = saldoAnterior + (debito - credito);
                            }
                            else
                            {
                                saldo = saldoAnterior + (credito - debito);
                            }

                            ws.Cells["A" + i].Value = item.cuenta;
                            ws.Cells["B" + i].Value = item2.tipoComprobante;
                            ws.Cells["C" + i].Value = item2.numero;
                            ws.Cells["D" + i].Value = item2.fechaCreado.ToString("yyyy-MM-dd");
                            ws.Cells["E" + i].Value = item2.terceroId;
                            ws.Cells["F" + i].Value = nomTercero;
                            ws.Cells["G" + i].Value = item2.detalle;
                            ws.Cells["H" + i].Value = saldoAnterior.ToString("N0", formato);
                            ws.Cells["I" + i].Value = item2.debito.ToString("N0", formato);
                            ws.Cells["J" + i].Value = item2.credito.ToString("N0", formato);
                            ws.Cells["K" + i].Value = saldo.ToString("N0", formato);

                            i++;

                        }
                    }

                }
                else if (informe == 8)
                {
                    ExcelWorksheet ws = null;
                    var movimiento = new List<movimientos>();
                    var AuxMovimiento = new List<movimientos>();
                    if (cuenta != "0")
                    {
                        movimiento = db.movimientos.Where(x => x.cuenta == cuenta).ToList();
                    }
                    else { movimiento = db.movimientos.ToList(); }

                    if (fechDesde != "")
                    {
                        DateTime auxfd = Convert.ToDateTime(fechDesde);
                        DateTime fd = new DateTime(auxfd.Year, auxfd.Month, auxfd.Day, 0, 0, 0);
                        if (fechHasta != "")
                        {
                            DateTime auxfh = Convert.ToDateTime(fechHasta);
                            DateTime fh = new DateTime(auxfh.Year, auxfh.Month, auxfh.Day, 23, 59, 59);
                            AuxMovimiento = movimiento.Where(X => X.fechaCreado >= fd && X.fechaCreado <= fh).ToList();
                            movimiento = null;
                        }
                        else
                        {

                            AuxMovimiento = movimiento.Where(X => X.fechaCreado.Year == fd.Year && X.fechaCreado.Month == fd.Month && X.fechaCreado.Day == fd.Day).ToList();
                            movimiento = null;
                        }
                    }

                    ws = pack.Workbook.Worksheets.Add("AuxiliarCuentas");
                    ws.Cells["A" + 1].Value = "CÓDIGO";
                    ws.Cells["B" + 1].Value = "NOMBRE";
                    ws.Cells["C" + 1].Value = "COMPROBANTE";
                    ws.Cells["D" + 1].Value = "FECHA";
                    ws.Cells["E" + 1].Value = "TERCERO";
                    ws.Cells["F" + 1].Value = "NOMBRE TERCERO";
                    ws.Cells["G" + 1].Value = "DÉBITO";
                    ws.Cells["H" + 1].Value = "CRÉDITO";
                    ws.Cells["I" + 1].Value = "SALDO";

                    var mov = (from m in AuxMovimiento
                               orderby m.cuenta
                               select new { m.cuenta, m.planCuentas }).Distinct().ToList();
                    int i = 3;
                    decimal saldoT = 0, saldoTotal = 0;
                    foreach (var item in mov)
                    {
                        var dataMov = AuxMovimiento.Where(x => x.cuenta == item.cuenta).OrderBy(x => x.fechaCreado).ToList();
                        decimal debito = dataMov.Select(x => x.debito).Sum();
                        decimal credito = dataMov.Select(x => x.credito).Sum();
                        ws.Cells["A" + i].Value = item.cuenta;
                        ws.Cells["B" + i].Value = (item.planCuentas != null) ? item.planCuentas.Nombre : "";
                        string naturaleza = item.planCuentas.naturaleza;
                        i++;
                        foreach (var item2 in dataMov)
                        {
                            if (naturaleza == "D")
                            {
                                saldoT = item2.debito - item2.credito;
                            }
                            else
                            {
                                saldoT = item2.credito - item2.debito;
                            }

                            ws.Cells["C" + i].Value = item2.tipoComprobante + " " + item2.numero;
                            ws.Cells["D" + i].Value = item2.fechaCreado.ToString("yyyy-MM-dd");
                            ws.Cells["E" + i].Value = item2.terceroId;
                            if (item2.persons != null)
                            {
                                ws.Cells["F" + i].Value = item2.persons.name;
                            }
                            else { ws.Cells["F" + i].Value = ""; }

                            ws.Cells["G" + i].Value = item2.debito.ToString("N0", formato);
                            ws.Cells["H" + i].Value = item2.credito.ToString("N0", formato);
                            ws.Cells["I" + i].Value = saldoT.ToString("N0", formato);
                            i++;
                        }
                        if (naturaleza == "D")
                        {
                            saldoTotal = debito - credito;
                        }
                        else
                        {
                            saldoTotal = credito - debito;
                        }
                        ws.Cells["F" + i].Value = "TOTAL";
                        ws.Cells["G" + i].Value = debito.ToString("N0", formato);
                        ws.Cells["H" + i].Value = credito.ToString("N0", formato);
                        ws.Cells["I" + i].Value = saldoTotal.ToString("N0", formato);
                        i += 2;
                    }
                    AuxMovimiento = null;
                    ws.Cells[ws.Dimension.Address].AutoFitColumns();

                }
                else if (informe == 9)
                {
                    var FechaActual = coll["FechaActual"];
                    List<factura> facturas = new List<factura>();
                    if (FechaActual != "")
                    {
                        DateTime FA = Convert.ToDateTime(FechaActual);
                        facturas = db.factura.Where(x => (x.operationTypeId == 6 || x.operationTypeId == 15) && (x.date.Year == FA.Year && x.date.Month == FA.Month && x.date.Day == FA.Day)).ToList();
                    }
                    ExcelWorksheet ws = pack.Workbook.Worksheets.Add("ComprobanteInformeDiario");
                    ws.Cells["A" + 1].Value = "COMPROBANTE INFORME DIARIO";
                    ws.Cells["A" + 2].Value = "LUIS BOLIVAR SERRANO QUISTANCHALA";
                    ws.Cells["A" + 3].Value = "13015277-6";
                    ws.Cells["A" + 4].Value = "Fecha comprobante diario " + Convert.ToDateTime(FechaActual).ToString("yyyy/MM/dd");

                    ws.Cells["A1:I1"].Merge = true;//une columnas en una fila
                    ws.Cells["A2:I2"].Merge = true;
                    ws.Cells["A3:I3"].Merge = true;
                    ws.Cells["A4:I4"].Merge = true;


                    if (facturas.Count > 0)
                    {

                    }

                }


                var ms = new System.IO.MemoryStream();
                pack.SaveAs(ms);
                ms.WriteTo(Response.OutputStream);
            }
            Response.End();
            return RedirectToAction("../Informes/Index");
        }

        public ActionResult ViewinformeDiario()
        {
            return View();
        }

        public ActionResult informeDiario(Array[] gastos, Array[] otros)
        {
            var archivo = "attachment;filename=InformeDiario.xlsx";
            Response.Clear();
            Response.ClearContent();
            Response.ClearHeaders();
            Response.Buffer = true;
            Response.ContentEncoding = System.Text.Encoding.UTF8;
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", archivo);

            Array[] gast = null;

            List<Array> me = new List<Array>();
            if (Request.Cookies["gastos"] != null)
            {
                var value = Request.Cookies["gastos"].Value;

                string[] aux = value.Split(',');

                foreach (var item in aux)
                {
                    me.Add(item.ToArray());

                }

                gast = me.ToArray();
                Response.Cookies["gastos"].Expires = DateTime.Now.AddDays(-1);
            } //Array de mes



            using (ExcelPackage pack = new ExcelPackage())
            {
                ExcelWorksheet ws = pack.Workbook.Worksheets.Add("InformeDiario");
                int j = 1;
                int totalGastos = 0;
                decimal debito = 0, totalConsignar = 0, totalFactura = 0;
                DateTime fecha = DateTime.Now;
                ws.Cells["A" + j].Value = "FECHA";
                ws.Cells["B" + j].Value = fecha.ToString("dd") + " DE " + fecha.ToString("MMMM").ToUpper() + " DE " + fecha.Year;

                j++;
                var movimientos = db.movimientos.Where(x => x.cuenta == "11050502" && x.fechaCreado.Year == fecha.Year && x.fechaCreado.Month == fecha.Month && x.fechaCreado.Day == fecha.Day).ToList();
                if (movimientos != null)
                {
                    debito = movimientos.Select(x => x.debito).Sum();
                }
                ws.Cells["A" + j].Value = "TOTAL MOVIMIENTOS CAJA";
                ws.Cells["B" + j].Value = debito.ToString("N0");

                if (gastos != null)
                {

                    j += 2;
                    ws.Cells["A" + j].Value = "GASTOS";
                    j++;

                    foreach (Array item in gastos)
                    {
                        ws.Cells["A" + j].Value = item.GetValue(0);
                        ws.Cells["B" + j].Value = item.GetValue(1);
                        string cadena = item.GetValue(1).ToString();
                        cadena = cadena.Replace(".", "");
                        totalGastos += Convert.ToInt32(cadena);
                        j++;
                    }
                    ws.Cells["A" + j].Value = "TOTAL GASTOS";
                    ws.Cells["B" + j].Value = totalGastos.ToString("N0");
                }

                if (otros != null)
                {

                    j += 2;
                    ws.Cells["A" + j].Value = "OTROS";
                    j++;

                    foreach (Array item in gastos)
                    {
                        ws.Cells["A" + j].Value = item.GetValue(0);
                        ws.Cells["B" + j].Value = item.GetValue(1);
                        j++;
                    }

                }

                j += 2;
                totalConsignar = debito - totalGastos;
                ws.Cells["A" + j].Value = "TOTAL PARA CONSIGNAR";
                ws.Cells["B" + j].Value = totalConsignar.ToString("N2");

                j += 2;
                var facturas = db.factura.Where(x => x.operationTypeId == 15 && x.date.Year == fecha.Year && x.date.Month == fecha.Month && x.date.Day == fecha.Day).ToList();
                if (facturas != null)
                {
                    ws.Cells["A" + j].Value = "CLIENTE";
                    ws.Cells["B" + j].Value = "VALOR TOTAL";
                    j++;
                    foreach (var item in facturas)
                    {
                        ws.Cells["A" + j].Value = item.persons.name;
                        ws.Cells["B" + j].Value = item.total.ToString("N0");
                        j++;
                    }
                    totalFactura = facturas.Select(x => x.total).Sum();
                    ws.Cells["A" + j].Value = "TOTAL";
                    ws.Cells["B" + j].Value = totalFactura.ToString("N0");
                }

                var ms = new System.IO.MemoryStream();
                pack.SaveAs(ms);
                ms.WriteTo(Response.OutputStream);
            }
            Response.End();

            return RedirectToAction("../Informes/ViewinformeDiario");
        }

        public JsonResult GetCuentasHasta(string cuenta)
        {
            List<Array> cuentas = new List<Array>();


            return Json(cuentas, JsonRequestBehavior.AllowGet);
        }


    }
}