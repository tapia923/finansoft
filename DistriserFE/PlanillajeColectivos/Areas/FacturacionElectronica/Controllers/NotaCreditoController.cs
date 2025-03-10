using Common;
using FE.Documentos;
using FE.pdf;
using FE.ServiciosWeb;
using FE.ServiciosWeb.DianServices;
using FE.Tipos.Standard;
using FEDian.Firma;
using FEDIAN.Util;
using PlanillajeColectivos.Areas.Procesos.Controllers;
using PlanillajeColectivos.Areas.Products.Controllers;
using PlanillajeColectivos.DTO;
using PlanillajeColectivos.DTO.FacturacionElectronica;
using PlanillajeColectivos.DTO.Products;
using PlanillajeColectivos.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml;

namespace PlanillajeColectivos.Areas.FacturacionElectronica.Controllers
{
    [Authorize]
    public class NotaCreditoController : Controller
    {

        public static bool AMBIENTE_PRUEBAS = true;
        public static bool USAR_SET_PUEBAS = true;
        public static AmbienteServicio AMBIENTE_SERVICIO = AMBIENTE_PRUEBAS ? AmbienteServicio.PRUEBAS : AmbienteServicio.PRODUCCION;
        public static string RUTA_CERTIFICADO = System.AppDomain.CurrentDomain.BaseDirectory + "Certificado\\Certificado.pfx";
        public static string CLAVE_CERTIFICADO = "Granero2022";

        // GET: FacturacionElectronica/NotaCredito
        public ActionResult Index()
        {
            return View();
        }

        
        public ActionResult NotaCredito(int Id) //Página principal para realizar una nota credito 
        {
            using(var ctx = new AccountingContext())
            {
                var Clientes = new ConsultasController().GetTerceros().ToList().Select(p => new SelectListItem { Text = p.nit + " || " + p.name, Value = p.nit, Selected = false });
                var MotivosNC = new MotivoNotaCredito().Listar().ToList().Select(p => new SelectListItem { Text = p.Descripcion, Value = p.Id.ToString(), Selected = false }); ;

                var products = ctx.products.ToList();
                
                var InfoFE = ctx.InvoiceFacturasElectronicas.Find(Id);
                

                ViewBag.clientes = Clientes;
                ViewBag.Id = Id;
                ViewBag.NumFactura = InfoFE.Prefijo + InfoFE.Numero;
                ViewBag.MotivosNC = MotivosNC;
                return View(products);
            }
            
        }

        public JsonResult RealizarNC(int IdFactura, int Motivo, string RazonMotivo)
        {
            using(var ctx = new AccountingContext())
            {
                int request = -10;
                try
                {

                    //variables por si se aprueba la nota credito
                    bool EstadoFE = false;
                    byte[] XmlBase64Response = null;
                    string NombreDocumento = "";
                    string StringCufe = "";
                    //.....

                    

                    var UserId = CurrentUser.Get.UserId;
                    var InfoFE = ctx.InvoiceFacturasElectronicas.Find(IdFactura);
                    var listaOperaciones = ctx.operation.Where(p => p.operationTypeId == 19 && p.facturaId == InfoFE.FacturaFK.id).ToList();
                    var InfoConsecutivo = ctx.consecutivosFacturas.Find(5);//5 representa el Id del registro para consecutivos de nota credito (se toma siempre el actual y si todo sale bien se aumenta +1)
                    var configuracion = ctx.ParametrosFE.FirstOrDefault();
                    var RegistroFactura = ctx.factura.Where(x => x.id == InfoFE.FacturaFK.id).FirstOrDefault();
                    var InfoMotivo = ctx.MotivosNotaCredito.Find(Motivo);

                    var fecha = DateTimeHelper.GetColombianDate();

                    var extension = HerramientasFE.GetExtensionDian(configuracion);
                    var emisor = HerramientasFE.GetEmisor(configuracion);
                    var adquiriente = HerramientasFE.GetAdquiriente(InfoFE.IdCliente);
                    var lineas = HerramientasFE.GetLineas(listaOperaciones);
                    var impuestos = HerramientasFE.GetImpuestos(listaOperaciones);
                    var detallePago = HerramientasFE.GetDetallePago(InfoFE);
                    var totales = HerramientasFE.GetTotales(lineas, impuestos);
                    var concepto = new ConceptoNotaCredito
                    {
                        Codigo = Motivo.ToString(),
                        Descripcion = InfoMotivo.Descripcion
                    };
                    var referencia = HerramientasFE.GetReferencia(InfoFE);

                    var generador = new GeneradorNotaCredito(configuracion.TIPO_AMBIENTE.ToString(), concepto, referencia);
                    generador.ConNumero(InfoConsecutivo.cod + InfoConsecutivo.actual);
                    generador.ConFecha(fecha);
                    generador.ConNota("Nota Credito");
                    generador.ConMoneda(Moneda.PESO_COLOMBIANO);
                    generador.ConExtensionDian(extension);
                    generador.ConEmisor(emisor);
                    generador.ConAdquiriente(adquiriente);
                    foreach(var item in lineas)
                    {
                        generador.AgregarLinea(item);
                    }
                    foreach(var item in impuestos)
                    {
                        generador.AgregarResumenImpuesto(item);
                    }
                    generador.ConTotales(totales);
                    generador.ConDetallePago(detallePago);
                    generador.AsignarCUDE(configuracion.SOFTWARE_PIN);


                    CreditNoteType NotaCredito = generador.Obtener();
                    XmlDocument xmlNotaCredito = NotaCredito.SerializeXmlDocument();
                    var stringNotaCredito = HerramientasFE.ToXmlString(xmlNotaCredito);

                    var firmaElectronica = new FirmaElectronica
                    {
                        RolFirmante = RolFirmante.EMISOR,
                        RutaCertificado = RUTA_CERTIFICADO,
                        ClaveCertificado = CLAVE_CERTIFICADO
                    };

                    var signedBytes = firmaElectronica.FirmarNotaCredito(stringNotaCredito, fecha);

                    string NombreArchivo = InfoConsecutivo.cod + InfoConsecutivo.actual+ fecha.ToString("yyyyMMddhhmmss");
                    var clienteDian = new ClienteServicioDian
                    {
                        Ambiente = AMBIENTE_SERVICIO,
                        RutaCertificado = RUTA_CERTIFICADO,
                        ClaveCertificado = CLAVE_CERTIFICADO
                    };

                    request = -1;
                    DianResponse dianResponse = null;
                    UploadDocumentResponse uploadResponse = null;

                    try
                    {
                        if (AMBIENTE_PRUEBAS && USAR_SET_PUEBAS)
                        {
                            var documentos = new List<byte[]>() { signedBytes };

                            uploadResponse = clienteDian.EnviarSetPruebas(documentos,configuracion.IDENTIFICADOR_SET_PRUEBAS);

                            dianResponse = clienteDian.ConsultarDocumentos(uploadResponse.ZipKey)[0];


                            //Guardamos el registro de la petición al web service de la DIAN
                            string Registro = "NOTA_CREDITO" + fecha.ToString("dd/MM/yyyy HH:mm:ss") + NotaCredito.ID.Value + NotaCredito.UUID.Value+ " : " + dianResponse.StatusCode + " : " + dianResponse.StatusMessage;
                            var RegistroFE = new ConsultasController().GetModelRegistroFE(Registro, dianResponse.ErrorMessage);
                            ctx.RegistrosFE.Add(RegistroFE);


                            var XMLBase64 = dianResponse.XmlBase64Bytes;
                            if (dianResponse.StatusCode == RespuestaDian.PROCESADO_CORRECTAMENTE || dianResponse.StatusCode == RespuestaDian.VALIDACION_EXITOSA)
                            {
                                if (XMLBase64 != null)//cambiar a != el == sólo es para hacer pruebas de envío del zip al cliente
                                {
                                    XmlBase64Response = XMLBase64;
                                }
                                else
                                {
                                    XmlBase64Response = signedBytes;
                                }

                                EstadoFE = true;
                                NombreDocumento = NombreArchivo;
                                StringCufe = NotaCredito.UUID.Value;
                                var GenerarNotaCreditoPDF = GetNCpdf(NotaCredito, NombreDocumento, configuracion);
                            }
                            else
                            {
                                request = -2;
                            }

                        }
                        else if (AMBIENTE_PRUEBAS)
                        {
                            
                        }
                        else
                        {
                            dianResponse = clienteDian.EnviarDocumento(signedBytes);

                            //Guardamos el registro de la petición al web service de la DIAN
                            string Registro = "NOTA_CREDITO" + fecha.ToString("dd/MM/yyyy HH:mm:ss") + NotaCredito.ID.Value + NotaCredito.UUID.Value + " : " + dianResponse.StatusCode + " : " + dianResponse.StatusMessage;
                            var RegistroFE = new ConsultasController().GetModelRegistroFE(Registro, dianResponse.ErrorMessage);
                            ctx.RegistrosFE.Add(RegistroFE);

                            var XMLBase64 = dianResponse.XmlBase64Bytes;
                            if (dianResponse.StatusCode == RespuestaDian.PROCESADO_CORRECTAMENTE || dianResponse.StatusCode == RespuestaDian.VALIDACION_EXITOSA)
                            {
                                if (XMLBase64 != null)//cambiar a != el == sólo es para hacer pruebas de envío del zip al cliente
                                {
                                    XmlBase64Response = XMLBase64;
                                }
                                else
                                {
                                    XmlBase64Response = signedBytes;
                                }

                                EstadoFE = true;
                                NombreDocumento = NombreArchivo;
                                StringCufe = NotaCredito.UUID.Value;
                                var GenerarNotaCreditoPDF = GetNCpdf(NotaCredito, NombreDocumento, configuracion);

                            }
                            else
                            {
                                
                                request = -2;

                            }
                        }
                        
                        
                        if (EstadoFE)
                        {

                            var Comprobante = (from pc in ctx.TipoComprobantes where pc.codigo == "CC1" select pc).FirstOrDefault();
                            string NuevoIdFactura = Guid.NewGuid().ToString();
                            var nuevaFactura = new factura() //generamos la factura
                            {
                                id = NuevoIdFactura,
                                personId = InfoFE.IdCliente,
                                userId = RegistroFactura.userId,
                                operationTypeId = 7,
                                date = Convert.ToDateTime(fecha),
                                total = totales.TotalPagable,
                                tipo = 1,//1 equivale a contado
                                fechaPagoCredito = "N",
                                cash = totales.TotalPagable,
                                codConsecutivo = InfoConsecutivo.cod,
                                numeroFactura = InfoConsecutivo.actual,
                                valorTotalExcentos = new OperacionesController().GetTotalExcentos(listaOperaciones),
                                valorTotalExcluidos = new OperacionesController().GetTotalExcluidos(listaOperaciones),
                                baseIVA19 = new OperacionesController().GetTotalIva19(impuestos)[1],
                                baseIVA5 = new OperacionesController().GetTotalIva5(impuestos)[1],
                                valorIVA19 = new OperacionesController().GetTotalIva19(impuestos)[0],
                                valorIVA5 = new OperacionesController().GetTotalIva5(impuestos)[0],
                                totalBolsas = 0,
                                valorConvenio = 0,
                                facturacion = "NOTA CREDITO",
                                IdProveedor = 1,
                                PrefijoComprobante = "CC1",
                                NumeroComprobante = Comprobante.consecutivo,
                                estado = true
                            };

                            ctx.factura.Add(nuevaFactura);

                            InfoConsecutivo.actual += 1; //actualizamos consecutivo de notacredito
                            Comprobante.consecutivo = (Convert.ToInt32(Comprobante.consecutivo) + 1).ToString();// actualizamos consecutivo del comprobante


                            ctx.SaveChanges();
                        }
                        


                    }
                    catch (Exception ex)
                    {

                        return new JsonResult { Data = new { status = false, request } };
                    }//terminar try catch de NotaCredito

                    return new JsonResult { Data = new { status = true } };
                }
                catch(Exception ex)
                {
                    return new JsonResult { Data = new { status = false,request } };
                }

                
            }
            
        }

        public bool GetNCpdf(CreditNoteType NotaCredito, string NombreDocumento, ParametrosConfiguracionFE Configuracion)
        {
            try
            {
                string path = System.AppDomain.CurrentDomain.BaseDirectory + "Certificado\\" + NombreDocumento + ".pdf";

                string RutaLogo = System.AppDomain.CurrentDomain.BaseDirectory + "Content\\img\\Logo.jpg";

                var NotaCreditoPdf = new NotaCreditoPdf(NotaCredito)
                {
                    RutaLogo = RutaLogo,
                    TextoEncabezado = "Persona Natural \r\n Responsable de IVA",
                    TextoConstancia = DocumentoPdf.TEXTO_CONSTANCIA_FACTURA_DEFAULT,
                    TextoQR = NotaCredito.UUID.Value,
                    TextoResolucion = DocumentoPdf.CrearTextoResolucion(Configuracion.RANGO_NUMERO_RESOLUCION, Configuracion.RANGO_FECHA_RESOLUCION, Configuracion.RANGO_PREFIJO, Configuracion.RANGO_DESDE, Configuracion.RANGO_HASTA, Configuracion.RANGO_VIGENCIA_HASTA),
                };

                var bytesPdf = NotaCreditoPdf.Generar();
                System.IO.FileStream stream = new FileStream(path, FileMode.CreateNew);
                System.IO.BinaryWriter writer = new BinaryWriter(stream);
                writer.Write(bytesPdf, 0, bytesPdf.Length);
                writer.Close();

                return true;
            }
            catch (Exception ex)
            {
                return false;
                throw;
            }
        }

        public ActionResult IrIndexFE(int IdFactura)
        {
            using (var ctx = new AccountingContext())
            {
                var factura = ctx.InvoiceFacturasElectronicas.Find(IdFactura);
                var ListadoOperaciones = ctx.operation.Where(x => x.facturaId == factura.FacturaFK.id && x.operationTypeId == 19).ToList();
                foreach (var item in ListadoOperaciones)
                {
                    ctx.Entry(item).State = System.Data.Entity.EntityState.Deleted;
                }
                ctx.SaveChanges();

            }
            return RedirectToAction("ListFacturasElectronicas", "facturas",new { @area="Facturas"});
        }
    }
}