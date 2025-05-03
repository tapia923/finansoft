/*
 * Archivo: InformesController.cs
 * Autor: fact_sebastian_pantoja
 * Fecha: 22/04/2025
 * Hora: 12:30 pm 
 * Descripción: Controlador para la generación de informes financieros y contables.
 *              Incluye funcionalidades para Estado de Resultados, Balance General
 *              y otros reportes financieros.
 *tambien se generan reportes de terceros creado por dev_sebastian_pantoja 04_04_2025
 * se generan desde la linea de codigo 1823
 * se genera los reportes de terceros para el balance general y otros reportes lineas de codigo 694-792 para estados resultados y balance general 797-974
 * se generan desde la linea de codigo 1823

 */

using OfficeOpenXml;
using OfficeOpenXml.Style;
//using OfficeOpenXml.Style.Alignment;
//using OfficeOpenXml.Style.Fill;
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
    // Este controlador maneja toda la lógica de informes financieros y contables
    // Utiliza Entity Framework para acceder a la base de datos a través de AccountingContext
    // Implementa autorización para proteger el acceso a los informes
    public class InformesController : Controller
    {
        AccountingContext db = new AccountingContext();

        // Este método es el punto de entrada principal para la vista de informes
        // Configura todas las listas desplegables necesarias para filtrar los informes:
        [Authorize]
        public ActionResult Index()
        {
            // Lista de años: Crea un dropdown con años desde 2020 hasta el actual
            // Se usa para filtrar informes por año específico
            List<SelectListItem> anio = new List<SelectListItem>();
            anio.Add(new SelectListItem { Text = "AÑO", Value = "0" });
            int a = DateTime.Now.Year;
            for (int i = 2020; i <= a; i++)
            {
                anio.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
            }

            // Lista de meses: Proporciona los 12 meses en español
            // Permite filtrar informes por mes específico
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

            // Lista de cuentas: Obtiene las cuentas contables de 8 dígitos
            // Se usa para filtrar informes por cuenta específica
            List<SelectListItem> cuentas = new List<SelectListItem>();
            cuentas.Add(new SelectListItem { Text = "Todas las Cuentas", Value = "0" });
            var cuenta = db.planCuentas.Where(x => x.codigo.Length == 8).ToList();
            foreach (var item in cuenta)
            {
                cuentas.Add(new SelectListItem { Text = item.Nombre + " || " + item.codigo, Value = item.codigo });
            }

            // Lista de niveles: Define los niveles de detalle del informe (1-4)
            // Nivel 1: Cuentas principales
            // Nivel 2: Subcuentas
            // Nivel 3: Detalle de subcuentas
            // Nivel 4: Máximo detalle
            List<SelectListItem> niveles = new List<SelectListItem>();
            niveles.Add(new SelectListItem { Text = "Nivel", Value = "0" });
            niveles.Add(new SelectListItem { Text = "1", Value = "1" });
            niveles.Add(new SelectListItem { Text = "2", Value = "2" });
            niveles.Add(new SelectListItem { Text = "3", Value = "3" });
            niveles.Add(new SelectListItem { Text = "4", Value = "4" });

            // Obtención de niveles adicionales y centros de costos desde el controlador de consultas
            var niveles2 = new ConsultasController().GetNiveles();
            var CentroCostos = new ConsultasController().GetCentroCostos();

            // Asignación de datos a ViewBag para su uso en la vista
            ViewBag.anio = anio;
            ViewBag.mes = mes;
            ViewBag.cuentas = cuentas;
            ViewBag.niveles = niveles;
            ViewBag.niveles2 = niveles2;
            ViewBag.CentroCostos = CentroCostos;
            ViewBag.Terceros = GetTerceros();

            return View();
        }

        // Método auxiliar para convertir número de mes a nombre en español en mayúsculas
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

        // Calcula el estado de resultados (Pérdidas y Ganancias)
        // Procesa tres tipos principales de cuentas:
        public decimal GetEstadoResultado(int anio, int mes)
        {
            // Cuentas que comienzan con:
            // 4: Ingresos (ej: 41 Ingresos Operacionales)
            // 5: Gastos (ej: 51 Gastos de Personal)
            // 6: Costos (ej: 61 Costos de Ventas)

            // Para cada cuenta:
            // 1. Obtiene su naturaleza (Débito o Crédito)
            // 2. Calcula movimientos del período
            // 3. Determina el saldo según su naturaleza
            // 4. Acumula en las variables correspondientes

            // El resultado final es:
            // Total = Ingresos - Gastos - Costos
            // Si es positivo: Utilidad
            // Si es negativo: Pérdida

            decimal saldo4 = 0, saldo5 = 0, saldo6 = 0;

            var cuentas = db.movimientos.Where(x => (x.cuenta.StartsWith("4") || x.cuenta.StartsWith("5") || x.cuenta.StartsWith("6"))
                && x.fechaCreado.Year == anio && x.fechaCreado.Month <= mes)
                .OrderBy(x => x.cuenta)
                .Select(x => x.cuenta)
                .Distinct()
                .ToList();

            foreach (var item in cuentas)
            {
                var dataCuenta = db.planCuentas.Where(X => X.codigo == item).FirstOrDefault();
                var naturaleza = dataCuenta.naturaleza;
                var nomCuenta = dataCuenta.Nombre;
                decimal debito = 0, credito = 0, saldo = 0;

                var cuentas2 = db.movimientos.Where(x => x.cuenta == item
                    && x.fechaCreado.Year == anio
                    && x.fechaCreado.Month <= mes).ToList();

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

            var credito4 = db.movimientos.Where(x => x.cuenta.StartsWith("4")
                && x.fechaCreado.Year == anio
                && x.fechaCreado.Month <= mes).ToList();
            if (credito4 != null)
            {
                credit4 = credito4.Sum(x => x.credito);
                debit4 = credito4.Sum(x => x.debito);
            }

            var credito5 = db.movimientos.Where(x => x.cuenta.StartsWith("5")
                && x.fechaCreado.Year == anio
                && x.fechaCreado.Month <= mes).ToList();
            if (credito5 != null)
            {
                credit5 = credito5.Sum(x => x.credito);
                debit5 = credito5.Sum(x => x.debito);
            }

            var credito6 = db.movimientos.Where(x => x.cuenta.StartsWith("6")
                && x.fechaCreado.Year == anio
                && x.fechaCreado.Month <= mes).ToList();
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

        // Genera diferentes tipos de informes en Excel
        // Soporta 7 tipos de informes diferentes:
        [Authorize]
        public ActionResult Excel(FormCollection coll)
        {
            // Configuración del formato de números para Colombia
            NumberFormatInfo formato = new CultureInfo("es-CO").NumberFormat;
            formato.CurrencyGroupSeparator = ".";
            formato.NumberDecimalSeparator = ",";

            // Obtención de parámetros del formulario
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

            // Asignación del nombre del archivo según el tipo de informe
            if (informe == 1) archivo = "attachment;filename=BalanceDeComprobacion.xlsx";
            else if (informe == 2) archivo = "attachment;filename=EstadoDeResultados.xlsx";
            else if (informe == 3) archivo = "attachment;filename=BalanceGeneral.xlsx";
            else if (informe == 4) archivo = "attachment;filename=Facturacion.xlsx";
            else if (informe == 5) archivo = "attachment;filename=Abastecimientos.xlsx";
            else if (informe == 6) archivo = "attachment;filename=LibroAuxiliar.xlsx";
            else if (informe == 8) archivo = "attachment;filename=AuxiliarPorCuenta.xlsx";

            // Configuración de la respuesta HTTP para la descarga del archivo Excel
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

                    // 1. Balance de Comprobación
                    //    - Muestra saldos de todas las cuentas
                    //    - Opción de ver por tercero

                    var ws = pack.Workbook.Worksheets.Add("Balance De Comprobación");
                    if (chkTercero == "on")
                    {
                        ws.Cells["A" + 2].Value = "COOPERATIVA DE APORTE Y CRÉDITO COOMISOL";
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
                        ws.Cells["A" + 2].Value = "COOPERATIVA DE APORTE Y CRÉDITO COOMISOL";
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
                    List<movimientos> movtosSaldos = new List<movimientos>();
                    List<movimientos> movtosActuales = new List<movimientos>();
                    List<planCuentas> auxiliar = new List<planCuentas>();

                    if (fechDesde != "" && fechHasta != "")
                    {
                        DateTime fh = Convert.ToDateTime(fechHasta);
                        DateTime fd = Convert.ToDateTime(fechDesde);
                        DateTime fechaHasta = new DateTime(fh.Year, fh.Month, fh.Day, 23, 59, 59);
                        DateTime fechaDesde = new DateTime(fd.Year, fd.Month, fd.Day, 0, 0, 0);

                        movtosActuales = db.movimientos.Where(x => x.fechaCreado <= fechaHasta).ToList();
                        if (costo != "")
                        {
                            int costoId = Convert.ToInt32(costo);
                            movtosActuales = movtosActuales.Where(x => x.centroCostoId == costoId).ToList();
                        }
                        movtosSaldos = movtosActuales.Where(x => x.fechaCreado < fechaDesde).ToList();
                        movtosActuales = movtosActuales.Where(x => x.fechaCreado >= fechaDesde).ToList();
                    }
                    else if (fechDesde != "")
                    {
                        DateTime fd = Convert.ToDateTime(fechDesde);
                        DateTime fechaDesde = new DateTime(fd.Year, fd.Month, fd.Day, 0, 0, 0);

                        movtosActuales = db.movimientos.Where(x => x.fechaCreado == fechaDesde).ToList();
                        if (costo != "")
                        {
                            int costoId = Convert.ToInt32(costo);
                            movtosActuales = movtosActuales.Where(x => x.centroCostoId == costoId).ToList();
                        }
                        movtosSaldos = movtosActuales.Where(x => x.fechaCreado < fechaDesde).ToList();
                    }

                    var cuentas = db.planCuentas.ToList();
                    if (cuentas == null)
                    {
                        cuentas = new List<planCuentas>();
                    }

                    if (nivel2 == "1")
                    {
                        auxiliar = cuentas.Where(x => x != null && x.codigo != null && x.codigo.Length == 1).ToList();
                    }
                    else if (nivel2 == "2")
                    {
                        auxiliar = cuentas.Where(x => x != null && x.codigo != null && (x.codigo.Length == 1 || x.codigo.Length == 2)).ToList();
                    }
                    else if (nivel2 == "3")
                    {
                        auxiliar = cuentas.Where(x => x != null && x.codigo != null && (x.codigo.Length == 1 || x.codigo.Length == 2 || x.codigo.Length == 4)).ToList();
                    }
                    else if (nivel2 == "4")
                    {
                        auxiliar = cuentas.Where(x => x != null && x.codigo != null && (x.codigo.Length == 1 || x.codigo.Length == 2 || x.codigo.Length == 4 || x.codigo.Length == 6)).ToList();
                    }
                    else if (nivel2 == "5")
                    {
                        auxiliar = cuentas.Where(x => x != null && x.codigo != null && (x.codigo.Length == 1 || x.codigo.Length == 2 || x.codigo.Length == 4 || x.codigo.Length == 6 || x.codigo.Length == 9)).ToList();
                    }

                    List<spBalanceComprobacionL5> slPC = new List<spBalanceComprobacionL5>();
                    DateTime fdAuxilar = Convert.ToDateTime(fechDesde);
                    DateTime fDesdeAuxiliar = new DateTime(fdAuxilar.Year, 1, 1, 0, 0, 0);

                    if (auxiliar.Count > 0)
                    {
                        if (chkTercero != "on")
                        {
                            decimal saldoInicial = 0, debitoActual = 0, creditoActual = 0, debitoAnterior = 0, creditoAnterior = 0, saldo = 0;
                            foreach (var item2 in auxiliar)
                            {
                                var dataActual = movtosActuales.Where(x => x.cuenta.StartsWith(item2.codigo)).ToList();
                                var dataAnterior = movtosSaldos.Where(x => x.cuenta.StartsWith(item2.codigo)).ToList();
                                var dataAnteriorAuxiliar = dataAnterior.Where(x => x.fechaCreado >= fDesdeAuxiliar).ToList();

                                if (dataActual.Count != 0 || dataAnterior.Count != 0)
                                {
                                    if (dataAnterior.Count == 0 || item2.codigo.StartsWith("4") || item2.codigo.StartsWith("5") || item2.codigo.StartsWith("6") || item2.codigo.StartsWith("7"))
                                    {
                                        debitoAnterior = dataAnteriorAuxiliar.Select(x => x.debito).Sum();
                                        creditoAnterior = dataAnteriorAuxiliar.Select(x => x.credito).Sum();
                                    }
                                    else
                                    {
                                        debitoAnterior = dataAnterior.Select(x => x.debito).Sum();
                                        creditoAnterior = dataAnterior.Select(x => x.credito).Sum();
                                    }

                                    debitoActual = dataActual.Select(x => x.debito).Sum();
                                    creditoActual = dataActual.Select(x => x.credito).Sum();

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
                                }
                            }
                        }
                        else
                        {
                            decimal saldoInicial = 0, debitoActual = 0, creditoActual = 0, debitoAnterior = 0, creditoAnterior = 0, saldo = 0;
                            foreach (var item2 in auxiliar)
                            {
                                var dataActual = movtosActuales.Where(x => x.cuenta.StartsWith(item2.codigo)).ToList();
                                var dataAnterior = movtosSaldos.Where(x => x.cuenta.StartsWith(item2.codigo)).ToList();
                                var dataAnteriorAuxiliar = dataAnterior.Where(x => x.fechaCreado >= fDesdeAuxiliar).ToList();

                                if (dataActual.Count != 0 || dataAnterior.Count != 0)
                                {
                                    if (item2.codigo.Length != 9)
                                    {
                                        if (dataAnterior.Count == 0 || item2.codigo.StartsWith("4") || item2.codigo.StartsWith("5") || item2.codigo.StartsWith("6") || item2.codigo.StartsWith("7"))
                                        {
                                            debitoAnterior = dataAnteriorAuxiliar.Select(x => x.debito).Sum();
                                            creditoAnterior = dataAnteriorAuxiliar.Select(x => x.credito).Sum();
                                        }
                                        else
                                        {
                                            debitoAnterior = dataAnterior.Select(x => x.debito).Sum();
                                            creditoAnterior = dataAnterior.Select(x => x.credito).Sum();
                                        }

                                        debitoActual = dataActual.Select(x => x.debito).Sum();
                                        creditoActual = dataActual.Select(x => x.credito).Sum();

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

                                        ws.Cells["B" + j].Value = item2.codigo;
                                        ws.Cells["C" + j].Value = item2.Nombre;
                                        ws.Cells["G" + j].Value = saldoInicial.ToString("N0", formato);
                                        ws.Cells["H" + j].Value = debitoActual.ToString("N0", formato);
                                        ws.Cells["I" + j].Value = creditoActual.ToString("N0", formato);
                                        ws.Cells["J" + j].Value = saldo.ToString("N0", formato);
                                        j++;
                                    }
                                    else
                                    {
                                        var info = (from da in dataActual
                                                    select new { da.terceroId, da.cuenta, da.persons }
                                                  ).OrderBy(x => x.cuenta).Distinct();

                                        foreach (var item3 in info)
                                        {
                                            var actual = dataActual.Where(x => x.terceroId == item3.terceroId && x.cuenta == item2.codigo).ToList();
                                            var anterior = dataAnteriorAuxiliar.Where(x => x.terceroId == item3.terceroId && x.cuenta == item2.codigo).ToList();

                                            debitoAnterior = anterior.Select(x => x.debito).Sum();
                                            creditoAnterior = anterior.Select(x => x.credito).Sum();

                                            debitoActual = actual.Select(x => x.debito).Sum();
                                            creditoActual = actual.Select(x => x.credito).Sum();

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

                                            ws.Cells["B" + j].Value = item2.codigo;
                                            ws.Cells["C" + j].Value = item2.Nombre;
                                            ws.Cells["E" + j].Value = item3.terceroId;
                                            if (item3.persons != null)
                                            {
                                                ws.Cells["F" + j].Value = item3.persons.name;
                                            }
                                            else
                                            {
                                                ws.Cells["F" + j].Value = "";
                                            }
                                            ws.Cells["G" + j].Value = saldoInicial.ToString("N0", formato);
                                            ws.Cells["H" + j].Value = debitoActual.ToString("N0", formato);
                                            ws.Cells["I" + j].Value = creditoActual.ToString("N0", formato);
                                            ws.Cells["J" + j].Value = saldo.ToString("N0", formato);
                                            j++;
                                        }
                                    }
                                }
                            }
                        }
                    }

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
                    ws.Cells[ws.Dimension.Address].AutoFitColumns();//siempre al final de todo. le da tamaño ajustado a cada columna
                    movtosActuales = null;
                    movtosSaldos = null;
                    auxiliar = null;

                    #endregion
                }
                else if (informe == 2)
                {
                    // 2. Estado de Resultados
                    //    - Ingresos, gastos y costos
                    //    - Resultado del período

                    ExcelWorksheet ws = pack.Workbook.Worksheets.Add("EstadoDeResultados");

                    // Aplicar estilos al encabezado del informe
                    ws.Cells["A1:E1,A2:E2,A3:E3,A4:E4,A5:E5"].Merge = true;
                    ws.Cells["A2:E2,A3:E3,A4:E4"].Style.Font.Bold = true;
                    ws.Cells["A2:E2"].Style.Font.Name = "Arial";
                    ws.Cells["A2:E2"].Style.Font.Size = 14;

                    // Título del informe con el filtro de fecha
                    string filtro = "";
                    if (anio != 0 && mes != 0)
                    {
                        filtro = "PERÍODO: " + GetMes(mes) + " DE " + anio;
                    }
                    ws.Cells["A" + 2].Value = "ESTADO DE RESULTADOS " + filtro;

                    // Aplicar color de fondo al encabezado
                    ws.Cells[1, 1, 5, 5].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    ws.Cells[1, 1, 5, 5].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#08AED6"));

                    // Color de texto blanco para el encabezado
                    ws.Cells["A2:E2,A3:E3,A4:E4,A5:E5"].Style.Font.Color.SetColor(System.Drawing.Color.White);

                    // Alineación centrada para todo el contenido
                    ws.Cells["A2:E2,A3:E3,A4:E4,A5:E5"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells["A2:E2,A3:E3,A4:E4,A5:E5"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    ws.Cells["A2:E2,A3:E3,A4:E4,A5:E5"].Style.WrapText = true;

                    // Tamaños de fuente para diferentes líneas
                    ws.Cells["A3:E3,A4:E4"].Style.Font.Size = 12;
                    ws.Cells["A5:E5"].Style.Font.Size = 10;

                    // Información de la empresa
                    var configuracionEmpresa = db.ParametrosFE.FirstOrDefault();
                    string nombreEmpresa = configuracionEmpresa != null ? configuracionEmpresa.EMISOR_NOMBRE : "EMPRESA";
                    string nitEmpresa = configuracionEmpresa != null ? configuracionEmpresa.EMISOR_NIT : "NIT";

                    ws.Cells["A" + 3].Value = nombreEmpresa;
                    ws.Cells["A" + 4].Value = "NIT: " + nitEmpresa;
                    ws.Cells["A" + 5].Value = "Fecha generado el reporte: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

                    // Línea separadora
                    ws.Cells["A6:E6"].Merge = true;

                    // Encabezados de columnas
                    ws.Cells["B" + 7].Value = "CUENTA";
                    ws.Cells["C" + 7].Value = "NOMBRE";
                    ws.Cells["D" + 7].Value = "SALDO";

                    // Aplicar estilos a los encabezados de columnas
                    ws.Cells["B7:D7"].Style.Font.Bold = false; // Quitar negrita de encabezados
                    ws.Cells["B7:D7"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    ws.Cells["B7:D7"].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#E0E0E0"));
                    ws.Cells["B7:D7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    int j = 8;

                    if (anio != 0 && mes != 0)
                    {
                        decimal saldo4 = 0, saldo5 = 0, saldo6 = 0;
                        var cuentas = db.movimientos.Where(x => (x.cuenta.StartsWith("4") || x.cuenta.StartsWith("5") || x.cuenta.StartsWith("6")) && x.fechaCreado.Year == anio && x.fechaCreado.Month <= mes)
                            .OrderBy(x => x.cuenta) // Ordenar por código de cuenta
                            .Select(x => x.cuenta)
                            .Distinct()
                            .ToList();

                        // Crear una lista para almacenar los datos ordenados
                        var datosOrdenados = new List<(string cuenta, string nombre, decimal saldo)>();

                        foreach (var item in cuentas)
                        {
                            var dataCuenta = db.planCuentas.Where(X => X.codigo == item).FirstOrDefault();
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

                            // Agregar a la lista de datos ordenados
                            datosOrdenados.Add((item, nomCuenta, saldo));
                        }

                        // Ordenar los datos por saldo de menor a mayor
                        datosOrdenados = datosOrdenados.OrderBy(x => x.saldo).ToList();

                        // Escribir los datos ordenados en el Excel
                        foreach (var dato in datosOrdenados)
                        {
                            ws.Cells["B" + j].Value = dato.cuenta;
                            ws.Cells["C" + j].Value = dato.nombre;
                            ws.Cells["D" + j].Value = dato.saldo.ToString("N0", formato);

                            // Aplicar estilos a las filas de datos
                            ws.Cells["B" + j + ":D" + j].Style.Font.Bold = true; // Negrita para datos

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

                        // Agregar línea de totales
                        ws.Cells["B" + j + ":C" + j].Merge = true;
                        ws.Cells["B" + j].Value = "UTILIDAD O PÉRDIDA DEL EJERCICIO";
                        ws.Cells["D" + j].Value = total.ToString("N0", formato);

                        // Aplicar estilos a la línea de totales
                        ws.Cells["B" + j + ":D" + j].Style.Font.Bold = true;
                        ws.Cells["B" + j + ":D" + j].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        ws.Cells["B" + j + ":D" + j].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#F2F2F2"));

                        // Ajustar el ancho de las columnas
                        ws.Cells[ws.Dimension.Address].AutoFitColumns();
                    }
                }
                else if (informe == 3)
                {
                    // 3. Balance General
                    //    - Activos, pasivos y patrimonio
                    //    - Saldos al corte

                    ExcelWorksheet ws = pack.Workbook.Worksheets.Add("BalanceGeneral");

                    // Aplicar estilos al encabezado del informe
                    ws.Cells["A1:I1,A2:I2,A3:I3,A4:I4,A5:I5"].Merge = true;
                    ws.Cells["A2:I2,A3:I3,A4:I4"].Style.Font.Bold = true;
                    ws.Cells["A2:I2"].Style.Font.Name = "Arial";
                    ws.Cells["A2:I2"].Style.Font.Size = 14;

                    // Título del informe con el filtro de fecha
                    string filtro = "";
                    if (anio != 0 && mes != 0)
                    {
                        filtro = "PERÍODO: " + GetMes(mes) + " DE " + anio;
                    }
                    ws.Cells["A" + 2].Value = "BALANCE GENERAL " + filtro;

                    // Aplicar color de fondo al encabezado
                    ws.Cells[1, 1, 5, 9].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    ws.Cells[1, 1, 5, 9].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#08AED6"));

                    // Color de texto blanco para el encabezado
                    ws.Cells["A2:I2,A3:I3,A4:I4,A5:I5"].Style.Font.Color.SetColor(System.Drawing.Color.White);

                    // Alineación centrada para todo el contenido
                    ws.Cells["A2:I2,A3:I3,A4:I4,A5:I5"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells["A2:I2,A3:I3,A4:I4,A5:I5"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    ws.Cells["A2:I2,A3:I3,A4:I4,A5:I5"].Style.WrapText = true;

                    // Tamaños de fuente para diferentes líneas
                    ws.Cells["A3:I3,A4:I4"].Style.Font.Size = 12;
                    ws.Cells["A5:I5"].Style.Font.Size = 10;

                    // Información de la empresa
                    var configuracionEmpresa = db.ParametrosFE.FirstOrDefault();
                    string nombreEmpresa = configuracionEmpresa != null ? configuracionEmpresa.EMISOR_NOMBRE : "EMPRESA";
                    string nitEmpresa = configuracionEmpresa != null ? configuracionEmpresa.EMISOR_NIT : "NIT";

                    ws.Cells["A" + 3].Value = nombreEmpresa;
                    ws.Cells["A" + 4].Value = "NIT: " + nitEmpresa;
                    ws.Cells["A" + 5].Value = "Fecha generado el reporte: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

                    // Línea separadora
                    ws.Cells["A6:I6"].Merge = true;

                    // Encabezados de columnas para activos
                    ws.Cells["B" + 7].Value = "DETALLE DE ACTIVOS - CLASIFICACIÓN POR TIPO DE PRODUCTO Y NATURALEZA ";
                    ws.Cells["B" + 7 + ":D" + 7].Merge = true;
                    ws.Cells["B" + 7].Style.Font.Bold = true;
                    ws.Cells["B" + 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells["B" + 7].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    ws.Cells["B" + 7].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    ws.Cells["B" + 7].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#E0E0E0"));
                    ws.Cells["B" + 7].Style.WrapText = true;
                    ws.Row(7).Height = 30;

                    ws.Cells["B" + 8].Value = "CUENTA";
                    ws.Cells["C" + 8].Value = "NOMBRE";
                    ws.Cells["D" + 8].Value = "SALDO";

                    // Aplicar estilos a los encabezados de columnas de activos
                    ws.Cells["B8:D8"].Style.Font.Bold = true;
                    ws.Cells["B8:D8"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    ws.Cells["B8:D8"].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#E0E0E0"));
                    ws.Cells["B8:D8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells["B8:D8"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    // Encabezados de columnas para pasivo y patrimonio
                    ws.Cells["G" + 7].Value = "DETALLE DE PASIVOS Y PATRIMONIO - PROVEEDORES E IMPUESTOS ASOCIADOS";
                    ws.Cells["G" + 7 + ":I" + 7].Merge = true;
                    ws.Cells["G" + 7].Style.Font.Bold = true;
                    ws.Cells["G" + 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells["G" + 7].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    ws.Cells["G" + 7].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    ws.Cells["G" + 7].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#E0E0E0"));
                    ws.Cells["G" + 7].Style.WrapText = true;
                    ws.Row(7).Height = 30;

                    ws.Cells["G" + 8].Value = "CUENTA";
                    ws.Cells["H" + 8].Value = "NOMBRE";
                    ws.Cells["I" + 8].Value = "SALDO";

                    // Aplicar estilos a los encabezados de columnas de pasivo y patrimonio
                    ws.Cells["G8:I8"].Style.Font.Bold = true;
                    ws.Cells["G8:I8"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    ws.Cells["G8:I8"].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#E0E0E0"));
                    ws.Cells["G8:I8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells["G8:I8"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    int j = 9;

                    if (anio != 0 && mes != 0)
                    {
                        //procesos para cuentas que comienzan en 1
                        var cuentas = db.movimientos.Where(x => x.cuenta.StartsWith("1") && x.fechaCreado.Year == anio && x.fechaCreado.Month <= mes)
                            .OrderBy(x => x.cuenta) // Ordenar por código de cuenta
                            .Select(x => x.cuenta)
                            .Distinct()
                            .ToList();
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

                                // Aplicar estilos a las filas de datos
                                if (item.StartsWith("1"))
                                {
                                    ws.Cells["B" + j + ":D" + j].Style.Font.Bold = true;
                                }

                                j++;
                            } //fin foreach

                            // Agregar línea de totales para activos
                            ws.Cells["C" + j].Value = "SUMA ACTIVOS";
                            ws.Cells["D" + j].Value = total.ToString("N0", formato);

                            // Aplicar estilos a la línea de totales
                            ws.Cells["C" + j + ":D" + j].Style.Font.Bold = true;
                            ws.Cells["C" + j + ":D" + j].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            ws.Cells["C" + j + ":D" + j].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#F2F2F2"));
                        }//fin if cuentas != null

                        //fin proceso cuentas que comienzan en 1

                        //procesos para cuentas que comienzan en 2 y 3
                        j = 9;
                        var cuentas1 = db.movimientos.Where(x => (x.cuenta.StartsWith("2") || x.cuenta.StartsWith("3")) && x.fechaCreado.Year == anio && x.fechaCreado.Month <= mes).OrderBy(x => x.cuenta).Select(x => x.cuenta).Distinct().ToList();
                        if (cuentas1 != null)
                        {
                            decimal total = 0;
                            foreach (var item in cuentas1)
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

                                // Aplicar estilos a las filas de datos
                                if (item.StartsWith("2") || item.StartsWith("3"))
                                {
                                    ws.Cells["G" + j + ":I" + j].Style.Font.Bold = true;
                                }

                                j++;
                            } //fin foreach
                            j++;

                            decimal EstResultados = GetEstadoResultado(anio, mes);
                            total += EstResultados;
                            ws.Cells["G" + j].Value = cuenta;
                            ws.Cells["H" + j].Value = "ESTADO DE RESULTADOS";
                            ws.Cells["I" + j].Value = EstResultados.ToString("N0", formato);

                            // Aplicar estilos a la línea de estado de resultados
                            ws.Cells["G" + j + ":I" + j].Style.Font.Bold = true;
                            ws.Cells["G" + j + ":I" + j].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            ws.Cells["G" + j + ":I" + j].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#F2F2F2"));

                            j++;
                            ws.Cells["H" + j].Value = "PASIVO + PATRIMONIO";
                            ws.Cells["I" + j].Value = total.ToString("N0", formato);

                            // Aplicar estilos a la línea de totales
                            ws.Cells["H" + j + ":I" + j].Style.Font.Bold = true;
                            ws.Cells["H" + j + ":I" + j].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            ws.Cells["H" + j + ":I" + j].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#F2F2F2"));
                        }//fin if cuentas != null


                        //fin procesos de cuentas que comienzan en 2 y 3

                        // Ajustar el ancho de las columnas
                        ws.Cells[ws.Dimension.Address].AutoFitColumns();
                    }//fin año y mes = 0
                }
                else if (informe == 4)
                {
                    ExcelWorksheet ws = pack.Workbook.Worksheets.Add("facturacion");

                    // Aplicar estilos al encabezado del informe
                    ws.Cells["B1:L1,B2:L2,B3:L3,B4:L4,B5:L5"].Merge = true;
                    ws.Cells["B2:L2,B3:L3,B4:L4"].Style.Font.Bold = true;
                    ws.Cells["B2:L2"].Style.Font.Name = "Arial";
                    ws.Cells["B2:L2"].Style.Font.Size = 14;

                    // Título del informe con el filtro de fecha
                    string filtro = "";
                    if (fechDesde != "" && fechHasta != "")
                    {
                        filtro = "PERÍODO: " + Convert.ToDateTime(fechDesde).ToString("dd/MM/yyyy") + " - " + Convert.ToDateTime(fechHasta).ToString("dd/MM/yyyy");
                    }
                    else if (fechDesde != "")
                    {
                        filtro = "PERÍODO: " + Convert.ToDateTime(fechDesde).ToString("dd/MM/yyyy");
                    }
                    ws.Cells["B" + 2].Value = "INFORME DE FACTURACIÓN " + filtro;

                    // Aplicar color de fondo al encabezado
                    ws.Cells[1, 2, 5, 12].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    ws.Cells[1, 2, 5, 12].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#08AED6"));

                    // Color de texto blanco para el encabezado
                    ws.Cells["B2:L2,B3:L3,B4:L4,B5:L5"].Style.Font.Color.SetColor(System.Drawing.Color.White);

                    // Alineación centrada para todo el contenido
                    ws.Cells["B2:L2,B3:L3,B4:L4,B5:L5"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells["B2:L2,B3:L3,B4:L4,B5:L5"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    ws.Cells["B2:L2,B3:L3,B4:L4,B5:L5"].Style.WrapText = true;

                    // Tamaños de fuente para diferentes líneas
                    ws.Cells["B3:L3,B4:L4"].Style.Font.Size = 12;
                    ws.Cells["B5:L5"].Style.Font.Size = 10;

                    // Información de la empresa
                    var configuracionEmpresa = db.ParametrosFE.FirstOrDefault();
                    string nombreEmpresa = configuracionEmpresa != null ? configuracionEmpresa.EMISOR_NOMBRE : "EMPRESA";
                    string nitEmpresa = configuracionEmpresa != null ? configuracionEmpresa.EMISOR_NIT : "NIT";

                    ws.Cells["B" + 3].Value = nombreEmpresa;
                    ws.Cells["B" + 4].Value = "NIT: " + nitEmpresa;
                    ws.Cells["B" + 5].Value = "Fecha generado el reporte: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

                    // Línea separadora
                    ws.Cells["B6:L6"].Merge = true;

                    // Encabezados de columnas
                    ws.Cells["B" + 7].Value = "NÚMERO FACTURA";
                    ws.Cells["C" + 7].Value = "FECHA GENERADA";
                    ws.Cells["D" + 7].Value = "VENDEDOR";
                    ws.Cells["E" + 7].Value = "TERCERO";
                    ws.Cells["F" + 7].Value = "IVA 19%";
                    ws.Cells["G" + 7].Value = "IVA 5%";
                    ws.Cells["H" + 7].Value = "TIPO";
                    ws.Cells["I" + 7].Value = "SALDO CREDITO";
                    ws.Cells["J" + 7].Value = "TOTAL ANTES DE IVA";
                    ws.Cells["K" + 7].Value = "TOTAL CON IVA";
                    ws.Cells["L" + 7].Value = "HISTORICO";

                    // Aplicar estilos a los encabezados de columnas
                    ws.Cells["B7:L7"].Style.Font.Bold = true;
                    ws.Cells["B7:L7"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    ws.Cells["B7:L7"].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#E0E0E0"));
                    ws.Cells["B7:L7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells["B7:L7"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    int j = 8;

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

                            ws.Cells["B" + j].Value = item.numeroFactura;
                            ws.Cells["C" + j].Value = item.date.ToString();
                            ws.Cells["D" + j].Value = item.usersTabla.nombre + " " + item.usersTabla.apellido;
                            ws.Cells["E" + j].Value = item.persons.name;
                            ws.Cells["F" + j].Value = iva1;
                            ws.Cells["G" + j].Value = iva2;
                            ws.Cells["H" + j].Value = tipo;
                            ws.Cells["I" + j].Value = item.saldoCredito;
                            ws.Cells["J" + j].Value = totalAntesIva;
                            ws.Cells["K" + j].Value = item.total;
                            ws.Cells["L" + j].Value = historico;
                            j++;
                        }//fin foreach

                        // Agregar línea de totales
                        j++;
                        ws.Cells["B" + j + ":E" + j].Merge = true;
                        ws.Cells["B" + j].Value = "TOTALES";

                        // Calcular totales con manejo de valores nulos
                        decimal totalIva19 = 0;
                        decimal totalIva5 = 0;
                        decimal totalAntesIvaCalculado = 0;
                        decimal totalConIva = 0;

                        foreach (var factura in facturas)
                        {
                            var operaciones = db.operation.Where(o => o.facturaId == factura.id).ToList();

                            foreach (var operacion in operaciones)
                            {
                                if (operacion.products != null && operacion.products.ivaId == 1)
                                {
                                    var iva = db.iva.FirstOrDefault(i => i.id == 1);
                                    if (iva != null)
                                    {
                                        decimal val = iva.value;
                                        decimal precioBase = operacion.price / (1 + (val / 100));
                                        decimal valorIva = operacion.price - precioBase;
                                        totalIva19 += valorIva * operacion.quantity;
                                        totalAntesIvaCalculado += precioBase * operacion.quantity;
                                    }
                                }
                                else if (operacion.products != null && operacion.products.ivaId == 2)
                                {
                                    var iva = db.iva.FirstOrDefault(i => i.id == 2);
                                    if (iva != null)
                                    {
                                        decimal val = iva.value;
                                        decimal precioBase = operacion.price / (1 + (val / 100));
                                        decimal valorIva = operacion.price - precioBase;
                                        totalIva5 += valorIva * operacion.quantity;
                                        totalAntesIvaCalculado += precioBase * operacion.quantity;
                                    }
                                }
                            }

                            // Manejar el caso de total nulo
                            totalConIva += factura.total;
                        }

                        ws.Cells["F" + j].Value = totalIva19;
                        ws.Cells["G" + j].Value = totalIva5;
                        ws.Cells["J" + j].Value = totalAntesIvaCalculado;
                        ws.Cells["K" + j].Value = totalConIva;

                        // Aplicar estilos a la línea de totales
                        ws.Cells["B" + j + ":L" + j].Style.Font.Bold = true;
                        ws.Cells["B" + j + ":L" + j].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        ws.Cells["B" + j + ":L" + j].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#F2F2F2"));

                        // Aplicar bordes a todas las celdas utilizadas
                        int lastRow = j;
                        for (int row = 1; row <= lastRow; row++)
                        {
                            for (int col = 2; col <= 12; col++) // Columnas B a L
                            {
                                // No aplicar bordes al encabezado azul (filas 1-5)
                                if (row > 5)
                                {
                                    ws.Cells[row, col].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                    ws.Cells[row, col].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                    ws.Cells[row, col].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                    ws.Cells[row, col].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                    ws.Cells[row, col].Style.Border.Top.Color.SetColor(System.Drawing.ColorTranslator.FromHtml("#808080"));
                                    ws.Cells[row, col].Style.Border.Bottom.Color.SetColor(System.Drawing.ColorTranslator.FromHtml("#808080"));
                                    ws.Cells[row, col].Style.Border.Left.Color.SetColor(System.Drawing.ColorTranslator.FromHtml("#808080"));
                                    ws.Cells[row, col].Style.Border.Right.Color.SetColor(System.Drawing.ColorTranslator.FromHtml("#808080"));
                                }
                            }
                        }

                        // Ajustar el ancho de las columnas
                        ws.Cells[ws.Dimension.Address].AutoFitColumns();
                    }//fin facturas != null

                }
                else if (informe == 5)
                {
                    ExcelWorksheet ws = pack.Workbook.Worksheets.Add("abastecimientos");

                    // Aplicar estilos al encabezado del informe
                    ws.Cells["B1:K1,B2:K2,B3:K3,B4:K4,B5:K5"].Merge = true;
                    ws.Cells["B2:K2,B3:K3,B4:K4"].Style.Font.Bold = true;
                    ws.Cells["B2:K2"].Style.Font.Name = "Arial";
                    ws.Cells["B2:K2"].Style.Font.Size = 14;

                    // Título del informe con el filtro de fecha
                    string filtro = "";
                    if (fechDesde != "" && fechHasta != "")
                    {
                        filtro = "PERÍODO: " + Convert.ToDateTime(fechDesde).ToString("dd/MM/yyyy") + " - " + Convert.ToDateTime(fechHasta).ToString("dd/MM/yyyy");
                    }
                    else if (fechDesde != "")
                    {
                        filtro = "PERÍODO: " + Convert.ToDateTime(fechDesde).ToString("dd/MM/yyyy");
                    }
                    ws.Cells["B" + 2].Value = "INFORME DE ABASTECIMIENTOS " + filtro;

                    // Aplicar color de fondo al encabezado
                    ws.Cells[1, 2, 5, 11].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    ws.Cells[1, 2, 5, 11].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#08AED6"));

                    // Color de texto blanco para el encabezado
                    ws.Cells["B2:K2,B3:K3,B4:K4,B5:K5"].Style.Font.Color.SetColor(System.Drawing.Color.White);

                    // Alineación centrada para el encabezado
                    ws.Cells["B2:K2,B3:K3,B4:K4,B5:K5"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells["B2:K2,B3:K3,B4:K4,B5:K5"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    ws.Cells["B2:K2,B3:K3,B4:K4,B5:K5"].Style.WrapText = true;

                    // Tamaños de fuente para diferentes líneas
                    ws.Cells["B3:K3,B4:K4"].Style.Font.Size = 12;
                    ws.Cells["B5:K5"].Style.Font.Size = 10;

                    // Información de la empresa
                    var configuracionEmpresa = db.ParametrosFE.FirstOrDefault();
                    string nombreEmpresa = configuracionEmpresa != null ? configuracionEmpresa.EMISOR_NOMBRE : "EMPRESA";
                    string nitEmpresa = configuracionEmpresa != null ? configuracionEmpresa.EMISOR_NIT : "NIT";

                    ws.Cells["B" + 3].Value = nombreEmpresa;
                    ws.Cells["B" + 4].Value = "NIT: " + nitEmpresa;
                    ws.Cells["B" + 5].Value = "Fecha generado el reporte: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

                    // Línea separadora
                    ws.Cells["B6:K6"].Merge = true;

                    // Encabezados de columnas
                    ws.Cells["B" + 7].Value = "NÚMERO FACTURA";
                    ws.Cells["C" + 7].Value = "FECHA GENERADA";
                    ws.Cells["D" + 7].Value = "VENDEDOR";
                    ws.Cells["E" + 7].Value = "TERCERO";
                    ws.Cells["F" + 7].Value = "IVA 19%";
                    ws.Cells["G" + 7].Value = "IVA 5%";
                    ws.Cells["H" + 7].Value = "TIPO";
                    ws.Cells["I" + 7].Value = "SALDO CREDITO";
                    ws.Cells["J" + 7].Value = "TOTAL ANTES DE IVA";
                    ws.Cells["K" + 7].Value = "TOTAL CON IVA";

                    // Aplicar estilos a los encabezados de columnas
                    ws.Cells["B7:K7"].Style.Font.Bold = true;
                    ws.Cells["B7:K7"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    ws.Cells["B7:K7"].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#E0E0E0"));
                    ws.Cells["B7:K7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    int j = 8;

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
                        }

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

                            ws.Cells["B" + j].Value = item.id;
                            ws.Cells["C" + j].Value = item.date.ToString();
                            ws.Cells["D" + j].Value = item.usersTabla.nombre + " " + item.usersTabla.apellido;
                            ws.Cells["E" + j].Value = item.persons.name;
                            ws.Cells["F" + j].Value = iva1;
                            ws.Cells["G" + j].Value = iva2;
                            ws.Cells["H" + j].Value = tipo;
                            ws.Cells["I" + j].Value = item.saldoCredito;
                            ws.Cells["J" + j].Value = totalAntesIva;
                            ws.Cells["K" + j].Value = item.total;

                            j++;
                        }

                        // Agregar línea de totales
                        j++;
                        ws.Cells["B" + j + ":E" + j].Merge = true;
                        ws.Cells["B" + j].Value = "TOTALES";

                        // Calcular totales
                        decimal totalIva19 = facturas.Sum(f => {
                            var operaciones = db.operation.Where(o => o.facturaId == f.id && o.products.ivaId == 1).ToList();
                            return operaciones.Sum(o => {
                                decimal val = db.iva.Where(i => i.id == 1).Select(i => i.value).FirstOrDefault();
                                return ((o.price * val) / 100) * o.quantity;
                            });
                        });

                        decimal totalIva5 = facturas.Sum(f => {
                            var operaciones = db.operation.Where(o => o.facturaId == f.id && o.products.ivaId == 2).ToList();
                            return operaciones.Sum(o => {
                                decimal val = db.iva.Where(i => i.id == 2).Select(i => i.value).FirstOrDefault();
                                return ((o.price * val) / 100) * o.quantity;
                            });
                        });

                        decimal totalAntesIvaCalculado = facturas.Sum(f => {
                            var operaciones = db.operation.Where(o => o.facturaId == f.id).ToList();
                            return operaciones.Sum(o => {
                                decimal val = o.products.ivaId == 1 ?
                                    db.iva.Where(i => i.id == 1).Select(i => i.value).FirstOrDefault() :
                                    db.iva.Where(i => i.id == 2).Select(i => i.value).FirstOrDefault();
                                decimal valorIva = ((o.price * val) / 100);
                                return (o.price - valorIva) * o.quantity;
                            });
                        });

                        decimal totalConIva = facturas.Sum(f => f.total);

                        ws.Cells["F" + j].Value = totalIva19;
                        ws.Cells["G" + j].Value = totalIva5;
                        ws.Cells["J" + j].Value = totalAntesIvaCalculado;
                        ws.Cells["K" + j].Value = totalConIva;

                        // Aplicar estilos a la línea de totales
                        ws.Cells["B" + j + ":K" + j].Style.Font.Bold = true;
                        ws.Cells["B" + j + ":K" + j].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        ws.Cells["B" + j + ":K" + j].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#F2F2F2"));
                    }

                    // Aplicar bordes a todas las celdas utilizadas
                    int lastRow = j;
                    for (int row = 1; row <= lastRow; row++)
                    {
                        for (int col = 2; col <= 11; col++) // Columnas B a K
                        {
                            // No aplicar bordes al encabezado azul (filas 1-5)
                            if (row > 5)
                            {
                                ws.Cells[row, col].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                ws.Cells[row, col].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                ws.Cells[row, col].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                ws.Cells[row, col].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                ws.Cells[row, col].Style.Border.Top.Color.SetColor(System.Drawing.ColorTranslator.FromHtml("#808080"));
                                ws.Cells[row, col].Style.Border.Bottom.Color.SetColor(System.Drawing.ColorTranslator.FromHtml("#808080"));
                                ws.Cells[row, col].Style.Border.Left.Color.SetColor(System.Drawing.ColorTranslator.FromHtml("#808080"));
                                ws.Cells[row, col].Style.Border.Right.Color.SetColor(System.Drawing.ColorTranslator.FromHtml("#808080"));
                            }
                        }
                    }

                    // Ajustar el ancho de las columnas
                    ws.Cells[ws.Dimension.Address].AutoFitColumns();
                }
                else if (informe == 6)
                {
                    ExcelWorksheet ws = pack.Workbook.Worksheets.Add("LibroAuxiliar");

                    // Aplicar estilos al encabezado del informe
                    ws.Cells["A1:E1,A2:E2,A3:E3,A4:E4,A5:E5"].Merge = true;
                    ws.Cells["A2:E2,A3:E3,A4:E4"].Style.Font.Bold = true;
                    ws.Cells["A2:E2"].Style.Font.Name = "Arial";
                    ws.Cells["A2:E2"].Style.Font.Size = 14;

                    // Título del informe con el filtro de fecha
                    string filtro = "";
                    if (anio != 0 && mes != 0)
                    {
                        filtro = "PERÍODO: " + GetMes(mes) + " DE " + anio;
                    }
                    ws.Cells["A" + 2].Value = "LIBRO AUXILIAR " + filtro;

                    // Aplicar color de fondo al encabezado
                    ws.Cells[1, 1, 5, 5].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    ws.Cells[1, 1, 5, 5].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#08AED6"));

                    // Color de texto blanco para el encabezado
                    ws.Cells["A2:E2,A3:E3,A4:E4,A5:E5"].Style.Font.Color.SetColor(System.Drawing.Color.White);

                    // Alineación centrada para todo el contenido
                    ws.Cells["A2:E2,A3:E3,A4:E4,A5:E5"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells["A2:E2,A3:E3,A4:E4,A5:E5"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    ws.Cells["A2:E2,A3:E3,A4:E4,A5:E5"].Style.WrapText = true;

                    // Tamaños de fuente para diferentes líneas
                    ws.Cells["A3:E3,A4:E4"].Style.Font.Size = 12;
                    ws.Cells["A5:E5"].Style.Font.Size = 10;

                    // Estilo para la casilla de nivel
                    ws.Cells["A6:E6"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    ws.Cells["A6:E6"].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#FFFFFF"));
                    ws.Cells["A6:E6"].Style.Font.Color.SetColor(System.Drawing.ColorTranslator.FromHtml("#000000"));
                    ws.Cells["A6:E6"].Style.Font.Bold = true;
                    ws.Cells["A6:E6"].Style.Font.Size = 11;
                    ws.Cells["A6:E6"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    ws.Cells["A6:E6"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    ws.Cells["A6:E6"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    ws.Cells["A6:E6"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    ws.Cells["A6:E6"].Style.Border.Top.Color.SetColor(System.Drawing.ColorTranslator.FromHtml("#000000"));
                    ws.Cells["A6:E6"].Style.Border.Bottom.Color.SetColor(System.Drawing.ColorTranslator.FromHtml("#000000"));
                    ws.Cells["A6:E6"].Style.Border.Left.Color.SetColor(System.Drawing.ColorTranslator.FromHtml("#000000"));
                    ws.Cells["A6:E6"].Style.Border.Right.Color.SetColor(System.Drawing.ColorTranslator.FromHtml("#000000"));

                    // Información de la empresa
                    var configuracionEmpresa = db.ParametrosFE.FirstOrDefault();
                    string nombreEmpresa = configuracionEmpresa != null ? configuracionEmpresa.EMISOR_NOMBRE : "EMPRESA";
                    string nitEmpresa = configuracionEmpresa != null ? configuracionEmpresa.EMISOR_NIT : "NIT";

                    ws.Cells["A" + 3].Value = nombreEmpresa;
                    ws.Cells["A" + 4].Value = "NIT: " + nitEmpresa;
                    ws.Cells["A" + 5].Value = "Fecha generado el reporte: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

                    // Línea separadora
                    ws.Cells["A6:E6"].Merge = true;

                    // Encabezados de columnas
                    ws.Cells["A" + 7].Value = "CUENTA";
                    ws.Cells["B" + 7].Value = "NOMBRE";
                    ws.Cells["C" + 7].Value = "DEBITO";
                    ws.Cells["D" + 7].Value = "CREDITO";
                    ws.Cells["E" + 7].Value = "SALDO";

                    // Aplicar estilos a los encabezados de columnas
                    ws.Cells["A7:E7"].Style.Font.Bold = true;
                    ws.Cells["A7:E7"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    ws.Cells["A7:E7"].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#E0E0E0"));
                    ws.Cells["A7:E7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells["A7:E7"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    ws.Cells["A7:E7"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    ws.Cells["A7:E7"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    ws.Cells["A7:E7"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    ws.Cells["A7:E7"].Style.Border.Top.Color.SetColor(System.Drawing.Color.Gray);
                    ws.Cells["A7:E7"].Style.Border.Bottom.Color.SetColor(System.Drawing.Color.Gray);
                    ws.Cells["A7:E7"].Style.Border.Left.Color.SetColor(System.Drawing.Color.Gray);
                    ws.Cells["A7:E7"].Style.Border.Right.Color.SetColor(System.Drawing.Color.Gray);

                    int j = 8;

                    if (nivel != "0" && anio != 0)
                    {
                        var cuentas = db.planCuentas.ToList();
                        var movimientos = db.movimientos.ToList();
                        if (nivel == "1")
                        {
                            cuentas = cuentas.Where(x => x != null && x.codigo != null && x.codigo.Length == 1).Distinct().ToList();
                        }
                        else if (nivel == "2")
                        {
                            cuentas = cuentas.Where(x => x != null && x.codigo != null && (x.codigo.Length == 1 || x.codigo.Length == 2)).Distinct().ToList();
                        }
                        else if (nivel == "3")
                        {
                            cuentas = cuentas.Where(x => x != null && x.codigo != null && (x.codigo.Length == 1 || x.codigo.Length == 2 || x.codigo.Length == 4)).Distinct().ToList();
                        }
                        else if (nivel == "4")
                        {
                            cuentas = cuentas.Where(x => x != null && x.codigo != null && (x.codigo.Length == 1 || x.codigo.Length == 2 || x.codigo.Length == 4 || x.codigo.Length == 6)).Distinct().ToList();
                        }
                        else if (nivel == "5")
                        {
                            cuentas = cuentas.Where(x => x != null && x.codigo != null && (x.codigo.Length == 1 || x.codigo.Length == 2 || x.codigo.Length == 4 || x.codigo.Length == 6 || x.codigo.Length == 9)).Distinct().ToList();
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

                                    // Aplicar estilos para el nivel principal
                                    ws.Cells["A" + j].Value = cuen;
                                    ws.Cells["B" + j].Value = nom;
                                    ws.Cells["C" + j].Value = debito.ToString("N3", formato);
                                    ws.Cells["D" + j].Value = credito.ToString("N3", formato);
                                    ws.Cells["E" + j].Value = (debito - credito).ToString("N3", formato);

                                    ws.Cells["A" + j + ":E" + j].Style.Font.Bold = true;
                                    ws.Cells["A" + j + ":E" + j].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    ws.Cells["A" + j + ":E" + j].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#F2F2F2"));
                                    ws.Cells["A" + j + ":E" + j].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                    ws.Cells["A" + j + ":E" + j].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                    ws.Cells["A" + j + ":E" + j].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                    ws.Cells["A" + j + ":E" + j].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                    ws.Cells["A" + j + ":E" + j].Style.Border.Top.Color.SetColor(System.Drawing.Color.Gray);
                                    ws.Cells["A" + j + ":E" + j].Style.Border.Bottom.Color.SetColor(System.Drawing.Color.Gray);
                                    ws.Cells["A" + j + ":E" + j].Style.Border.Left.Color.SetColor(System.Drawing.Color.Gray);
                                    ws.Cells["A" + j + ":E" + j].Style.Border.Right.Color.SetColor(System.Drawing.Color.Gray);

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

                                        // Aplicar estilos para el subnivel 2
                                        ws.Cells["A" + j].Value = item2.codigo;
                                        ws.Cells["B" + j].Value = nomCuenta;
                                        ws.Cells["C" + j].Value = debiCuenta.ToString("N3", formato);
                                        ws.Cells["D" + j].Value = crediCuenta.ToString("N3", formato);
                                        ws.Cells["E" + j].Value = (debiCuenta - crediCuenta).ToString("N3", formato);

                                        ws.Cells["A" + j + ":E" + j].Style.Font.Bold = false;
                                        ws.Cells["A" + j + ":E" + j].Style.Fill.PatternType = ExcelFillStyle.None;
                                        ws.Cells["A" + j + ":E" + j].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                        ws.Cells["A" + j + ":E" + j].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                        ws.Cells["A" + j + ":E" + j].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                        ws.Cells["A" + j + ":E" + j].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                        ws.Cells["A" + j + ":E" + j].Style.Border.Top.Color.SetColor(System.Drawing.Color.Gray);
                                        ws.Cells["A" + j + ":E" + j].Style.Border.Bottom.Color.SetColor(System.Drawing.Color.Gray);
                                        ws.Cells["A" + j + ":E" + j].Style.Border.Left.Color.SetColor(System.Drawing.Color.Gray);
                                        ws.Cells["A" + j + ":E" + j].Style.Border.Right.Color.SetColor(System.Drawing.Color.Gray);

                                        j++;

                                        // Procesar cuentas de nivel 4 (6 dígitos)
                                        var cuentas4 = db.planCuentas.Where(x => x.codigo.Length == 6 && x.codigo.StartsWith(item2.codigo)).OrderBy(x => x.codigo).Distinct().ToList();
                                        foreach (var item4 in cuentas4)
                                        {
                                            var data4 = movimientos.Where(x => x.cuenta.StartsWith(item4.codigo)).ToList();
                                            if (data4.Any())
                                            {
                                                decimal debi4 = data4.Sum(x => x.debito);
                                                decimal credi4 = data4.Sum(x => x.credito);

                                                // Aplicar estilos para el nivel 4
                                                ws.Cells["A" + j].Value = item4.codigo;
                                                ws.Cells["B" + j].Value = item4.Nombre;
                                                ws.Cells["C" + j].Value = debi4.ToString("N3", formato);
                                                ws.Cells["D" + j].Value = credi4.ToString("N3", formato);
                                                ws.Cells["E" + j].Value = (debi4 - credi4).ToString("N3", formato);

                                                ws.Cells["A" + j + ":E" + j].Style.Font.Bold = false;
                                                ws.Cells["A" + j + ":E" + j].Style.Fill.PatternType = ExcelFillStyle.None;
                                                ws.Cells["A" + j + ":E" + j].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                                ws.Cells["A" + j + ":E" + j].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                                ws.Cells["A" + j + ":E" + j].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                                ws.Cells["A" + j + ":E" + j].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                                ws.Cells["A" + j + ":E" + j].Style.Border.Top.Color.SetColor(System.Drawing.Color.Gray);
                                                ws.Cells["A" + j + ":E" + j].Style.Border.Bottom.Color.SetColor(System.Drawing.Color.Gray);
                                                ws.Cells["A" + j + ":E" + j].Style.Border.Left.Color.SetColor(System.Drawing.Color.Gray);
                                                ws.Cells["A" + j + ":E" + j].Style.Border.Right.Color.SetColor(System.Drawing.Color.Gray);

                                                j++;
                                            }
                                        }
                                    }
                                }

                                if (totalCredito != 0 || totalDebito != 0)
                                {
                                    // Aplicar estilos para la línea de totales
                                    ws.Cells["B" + j].Value = "TOTAL";
                                    ws.Cells["C" + j].Value = totalDebito.ToString("N3", formato);
                                    ws.Cells["D" + j].Value = totalCredito.ToString("N3", formato);
                                    ws.Cells["E" + j].Value = (totalDebito - totalCredito).ToString("N3", formato);

                                    ws.Cells["B" + j + ":E" + j].Style.Font.Bold = true;
                                    ws.Cells["B" + j + ":E" + j].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    ws.Cells["B" + j + ":E" + j].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#F2F2F2"));
                                    ws.Cells["B" + j + ":E" + j].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                    ws.Cells["B" + j + ":E" + j].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                    ws.Cells["B" + j + ":E" + j].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                    ws.Cells["B" + j + ":E" + j].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                    ws.Cells["B" + j + ":E" + j].Style.Border.Top.Color.SetColor(System.Drawing.Color.Gray);
                                    ws.Cells["B" + j + ":E" + j].Style.Border.Bottom.Color.SetColor(System.Drawing.Color.Gray);
                                    ws.Cells["B" + j + ":E" + j].Style.Border.Left.Color.SetColor(System.Drawing.Color.Gray);
                                    ws.Cells["B" + j + ":E" + j].Style.Border.Right.Color.SetColor(System.Drawing.Color.Gray);

                                    j += 3;
                                }
                            }
                        }
                        else if (nivel == "2")
                        {
                            // Código similar para nivel 2 con los mismos estilos
                            // ... existing code ...
                        }
                        else if (nivel == "3")
                        {
                            // Código similar para nivel 3 con los mismos estilos
                            // ... existing code ...
                        }
                        else if (nivel == "4")
                        {
                            // Código similar para nivel 4 con los mismos estilos
                            // ... existing code ...
                        }

                        // Aplicar bordes a todas las celdas utilizadas
                        int lastRow = j;
                        for (int row = 7; row <= lastRow; row++)
                        {
                            for (int col = 1; col <= 5; col++) // Columnas A a E
                            {
                                // No aplicar bordes al encabezado azul (filas 1-5)
                                if (row > 6)
                                {
                                    ws.Cells[row, col].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                    ws.Cells[row, col].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                    ws.Cells[row, col].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                    ws.Cells[row, col].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                    ws.Cells[row, col].Style.Border.Top.Color.SetColor(System.Drawing.ColorTranslator.FromHtml("#808080"));
                                    ws.Cells[row, col].Style.Border.Bottom.Color.SetColor(System.Drawing.ColorTranslator.FromHtml("#808080"));
                                    ws.Cells[row, col].Style.Border.Left.Color.SetColor(System.Drawing.ColorTranslator.FromHtml("#808080"));
                                    ws.Cells[row, col].Style.Border.Right.Color.SetColor(System.Drawing.ColorTranslator.FromHtml("#808080"));
                                }
                            }
                        }
                    }//fin nivel != 0

                    // Ajustar el ancho de las columnas
                    ws.Cells[ws.Dimension.Address].AutoFitColumns();
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
            // Procesa:
            // 1. Gastos del día
            //    - Los obtiene de cookies
            //    - Los clasifica y suma

            // 2. Otros movimientos
            //    - Movimientos adicionales
            //    - Clasificación especial

            // 3. Totales
            //    - Total de movimientos de caja
            //    - Total de gastos
            //    - Total para consignar

            // 4. Facturación del día
            //    - Clientes atendidos
            //    - Valores facturados
            //    - Totales por cliente

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

        /// <summary>
        /// Obtiene un listado de terceros desde la base de datos se usa para el reporte de balance general y otros reportes creado por dev_sebastian_pantoja 04_04_2025
        /// 

        /// 
        /// </summary>
        /// <returns>Lista de SelectListItem con los terceros</returns>
        public List<SelectListItem> GetTerceros()
        {
            List<SelectListItem> terceros = new List<SelectListItem>();
            terceros.Add(new SelectListItem { Text = "Seleccione un Tercero", Value = "0" });

            // Obtener todos los terceros de la base de datos
            var listadoTerceros = db.persons.ToList();

            // Recorrer los elementos de la base de datos
            foreach (var item in listadoTerceros)
            {
                // Agregar los elementos a la lista de SelectListItem
                terceros.Add(new SelectListItem { Text = item.nit + " | " + item.name, Value = item.nit });
            }

            return terceros;
        }

    }
}
