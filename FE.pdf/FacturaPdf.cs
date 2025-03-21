using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FE.Tipos.Standard;
using iTextSharp.text;
using iTextSharp.text.pdf;
using PlanillajeColectivos.DTO.FacturacionElectronica.FacturaPD;

namespace FE.pdf
{
    public class FacturaPdf : DocumentoPdf
    {
        //public InvoiceType Invoice { get; private set; }
        public ViewModelFEpdf InvoicePDF { get; private set; }

        public override string Tipo { get; }

        public override string TextoConstancia { get; set; } = TEXTO_CONSTANCIA_FACTURA_DEFAULT;
        
        public FacturaPdf(ViewModelFEpdf invoice, string tipo = TIPO_FACTURA_VENTA)
        {
            InvoicePDF = invoice;
            Tipo = tipo;
        }


        public override byte[] Generar()
        {
            var docPdf = new Document(PageSize.A4, 25, 25, 25, 25);
            var streamPdf = new MemoryStream();
            var writer = PdfWriter.GetInstance(docPdf, streamPdf);
            writer.PageEvent = new CustomPageEventHelper();
            docPdf.Open();

            var encabezado = new PdfPTable(3);
            encabezado.HorizontalAlignment = Element.ALIGN_LEFT;
            encabezado.WidthPercentage = 100;
            encabezado.SetWidths(new float[] { 2, 4, 2 });
            encabezado.DefaultCell.Border = Rectangle.NO_BORDER;

            var cEmpty5 = new PdfPCell(new Phrase(" "))
            {
                Border = Rectangle.NO_BORDER,
                FixedHeight = 5
            };
            var cEmpty15 = new PdfPCell(new Phrase(" "))
            {
                Border = Rectangle.NO_BORDER,
                FixedHeight = 15
            };

            if (RutaLogo != null)
            {
                var logo = Image.GetInstance(RutaLogo);
                logo.BackgroundColor = BaseColor.WHITE;
                logo.Alignment = Element.ALIGN_MIDDLE;

                encabezado.AddCell(logo);
            }
            else
            {
                encabezado.AddCell(cEmpty15);
            }

            var tEmisor = new PdfPTable(1);
            tEmisor.WidthPercentage = 100;
            tEmisor.DefaultCell.Border = Rectangle.NO_BORDER;

            var fnt1 = FontFactory.GetFont("Helvetica", 10, Font.BOLD); //f12
            string nombreEmisor = InvoicePDF.ParametrosFE.EMISOR_NOMBRE;
            var cNombreEmisor = new PdfPCell(new Phrase(nombreEmisor, fnt1));
            cNombreEmisor.Border = Rectangle.NO_BORDER;
            cNombreEmisor.HorizontalAlignment = Element.ALIGN_CENTER;

            var fnt2 = FontFactory.GetFont("Helvetica", 9, Font.BOLD); //f11
            var nitEmisor = InvoicePDF.ParametrosFE.COMPANY_NIT;
            var dvEmisor = "";
            var cNitEmisor = new PdfPCell(new Phrase("NIT: " + nitEmisor+"-8", fnt2));
            cNitEmisor.Border = Rectangle.NO_BORDER;
            cNitEmisor.HorizontalAlignment = Element.ALIGN_CENTER;

            var fnt3 = FontFactory.GetFont("Helvetica", 8, Font.NORMAL); //f9
            var cEncabezado = new PdfPCell(new Phrase(TextoEncabezado, fnt3));
            cEncabezado.Border = Rectangle.NO_BORDER;
            cEncabezado.HorizontalAlignment = Element.ALIGN_CENTER;

            var direccionEmisor = InvoicePDF.ParametrosFE.cityFK?.departamentoFK?.paisFK?.Nom_pais + ", " +
                InvoicePDF.ParametrosFE.cityFK?.departamentoFK?.Nom_dep + ", " +
                InvoicePDF.ParametrosFE.cityFK?.Nom_muni + " \r\n" +
                InvoicePDF.ParametrosFE.Direccion;
            var cDireccionEmisor = new PdfPCell(new Phrase(direccionEmisor.ToUpper().Trim(), fnt3));
            cDireccionEmisor.Border = Rectangle.NO_BORDER;
            cDireccionEmisor.HorizontalAlignment = Element.ALIGN_CENTER;

            var correoEmisor = InvoicePDF.ParametrosFE.EMISOR_CORREO_ELECTRONICO;
            var cCorreoEmisor = new PdfPCell(new Phrase(correoEmisor, fnt3));
            cCorreoEmisor.Border = Rectangle.NO_BORDER;
            cCorreoEmisor.HorizontalAlignment = Element.ALIGN_CENTER;

            var telefonoEmisor = InvoicePDF.ParametrosFE.EMISOR_NUMERO_TELEFONICO;
            var cTelefonoEmisor = new PdfPCell(new Phrase(telefonoEmisor, fnt3));
            cTelefonoEmisor.Border = Rectangle.NO_BORDER;
            cTelefonoEmisor.HorizontalAlignment = Element.ALIGN_CENTER;

            tEmisor.AddCell(cEmpty5);
            tEmisor.AddCell(cNombreEmisor);
            tEmisor.AddCell(cNitEmisor);
            tEmisor.AddCell(cEmpty5);
            tEmisor.AddCell(cEncabezado);
            tEmisor.AddCell(cEmpty15);
            tEmisor.AddCell(cDireccionEmisor);
            tEmisor.AddCell(cCorreoEmisor);
            tEmisor.AddCell(cTelefonoEmisor);

            encabezado.AddCell(tEmisor);

            var tInfoGeneral = new PdfPTable(1);
            tInfoGeneral.WidthPercentage = 100;
            tInfoGeneral.DefaultCell.Border = Rectangle.NO_BORDER;

            var fnt4 = FontFactory.GetFont("Helvetica", 7, Font.NORMAL); //f10
            var ciTipoFactura = new PdfPCell(new Phrase(Tipo, fnt4));
            ciTipoFactura.Border = Rectangle.NO_BORDER;
            ciTipoFactura.HorizontalAlignment = Element.ALIGN_CENTER;
            ciTipoFactura.PaddingBottom = 5;

            var fnt4x = FontFactory.GetFont("Helvetica", 8, Font.NORMAL); //f10
            var vNumeroFactura = InvoicePDF.NumeroFactura;
            var cNumeroFactura = new PdfPCell(new Phrase(vNumeroFactura, fnt4x));
            cNumeroFactura.BackgroundColor = BaseColor.WHITE;
            cNumeroFactura.BorderColor = BaseColor.GRAY;
            cNumeroFactura.Border = Rectangle.BOX;
            cNumeroFactura.HorizontalAlignment = Element.ALIGN_CENTER;
            cNumeroFactura.PaddingTop = 5;
            cNumeroFactura.PaddingBottom = 7;

            if (string.IsNullOrEmpty(TextoQR)) TextoQR = InvoicePDF.QR;
            var bmQr = QRHelper.Crear(TextoQR, 500, 500);
            var imageQr = Image.GetInstance(bmQr, BaseColor.WHITE);
            imageQr.Border = Rectangle.NO_BORDER;
            imageQr.Alignment = Element.ALIGN_CENTER;

            tInfoGeneral.AddCell(ciTipoFactura);
            tInfoGeneral.AddCell(cNumeroFactura);
            tInfoGeneral.AddCell(imageQr);

            encabezado.AddCell(tInfoGeneral);

            docPdf.Add(encabezado);

            docPdf.Add(new Phrase("   ", fnt3));

            var encabezado2 = new PdfPTable(3);
            encabezado2.HorizontalAlignment = Element.ALIGN_LEFT;
            encabezado2.WidthPercentage = 100;
            encabezado2.SetWidths(new float[] { 4, 1.25f, 1.25f });
            encabezado2.SpacingAfter = 10;

            var nombreAdquiriente = InvoicePDF.Adquiriente.NombreAdquiriente;
            var identificacionAdquiriente = InvoicePDF.Adquiriente.DocumentoAdquiriente;
            var direccionAdquiriente = InvoicePDF.Adquiriente.DireccionAdquiriente;
            var correoAdquiriente = InvoicePDF.Adquiriente.CorreoAdquiriente;
            var telefonoAdquiriente = InvoicePDF.Adquiriente.ContactoAdquiriente;

            var iAdquiriente = "ADQUIRIENTE: " + nombreAdquiriente + "\r\n" +
                "DOCUMENTO DE IDENTIFICACIÓN: " + identificacionAdquiriente + "\r\n" +
                "DIRECCIÓN: " + direccionAdquiriente.ToUpper().Replace("\r\n", " ");
            if (!string.IsNullOrEmpty(correoAdquiriente))
                iAdquiriente += "\r\nCORREO ELECTRÓNICO: " + correoAdquiriente;
            if (!string.IsNullOrEmpty(telefonoAdquiriente))
                iAdquiriente += "\r\nCONTACTO: " + telefonoAdquiriente;

            var fnt5 = FontFactory.GetFont("Helvetica", 8, Font.NORMAL); //f9
            var cAdquiriente = new PdfPCell(new Phrase(iAdquiriente, fnt5));
            cAdquiriente.BorderColor = BaseColor.GRAY;
            cAdquiriente.Border = Rectangle.BOX;
            cAdquiriente.VerticalAlignment = Element.ALIGN_MIDDLE;
            cAdquiriente.ExtraParagraphSpace = 3;
            cAdquiriente.Padding = 4;
            cAdquiriente.PaddingRight = 7;
            cAdquiriente.PaddingLeft = 7;
            cAdquiriente.Rowspan = 2;

            var fechaEmision = InvoicePDF.FechaEmision;
            var horaEmision = InvoicePDF.HoraEmision;
            var iEmisionFactura = "FECHA DE EMISIÓN\r\n" + fechaEmision + "\r\n" + horaEmision;
            var fnt6 = FontFactory.GetFont("Helvetica", 8, Font.NORMAL); //f9
            var cEmisionFactura = new PdfPCell(new Phrase(iEmisionFactura, fnt6));
            cEmisionFactura.BorderColor = BaseColor.GRAY;
            cEmisionFactura.Border = Rectangle.BOX;
            cEmisionFactura.HorizontalAlignment = Element.ALIGN_CENTER;
            cEmisionFactura.VerticalAlignment = Element.ALIGN_MIDDLE;
            cEmisionFactura.Padding = 4;

            var iVenciFactura = "FECHA DE VENCIMIENTO\r\n" + InvoicePDF.FechaVencimiento;
            var cVenciFactura = new PdfPCell(new Phrase(iVenciFactura, fnt6));
            cVenciFactura.BorderColor = BaseColor.GRAY;
            cVenciFactura.Border = Rectangle.BOX;
            cVenciFactura.HorizontalAlignment = Element.ALIGN_CENTER;
            cVenciFactura.VerticalAlignment = Element.ALIGN_MIDDLE;
            cVenciFactura.Padding = 4;

            var cufe = InvoicePDF.Cufe;
            var iCufe = "CUFE\r\n" + cufe;
            var cCufe = new PdfPCell(new Phrase(iCufe, fnt6));
            cCufe.BorderColor = BaseColor.GRAY;
            cCufe.Border = Rectangle.BOX;
            cCufe.HorizontalAlignment = Element.ALIGN_CENTER;
            cCufe.VerticalAlignment = Element.ALIGN_MIDDLE;
            cCufe.Padding = 4;
            cCufe.Colspan = 2;

            encabezado2.AddCell(cAdquiriente);
            encabezado2.AddCell(cEmisionFactura);
            encabezado2.AddCell(cVenciFactura);
            encabezado2.AddCell(cCufe);

            docPdf.Add(encabezado2);

            // agregar líneas
            var totalLineas = InvoicePDF.ListOperaciones.Count();
            if (totalLineas > LINEAS_PRIMERA_PAGINA_CON_TOTALES)
            {
                agregarLineas(docPdf, 0, LINEAS_PRIMERA_PAGINA_SIN_TOTALES,InvoicePDF.ListOperaciones);

                if (totalLineas > LINEAS_PRIMERA_PAGINA_SIN_TOTALES)
                {
                    bool masPaginas = true;
                    int inicio = LINEAS_PRIMERA_PAGINA_SIN_TOTALES;

                    do
                    {
                        docPdf.NewPage();

                        agregarLineas(docPdf, inicio, LINEAS_PAGINAS_EXTRA, InvoicePDF.ListOperaciones);

                        inicio += LINEAS_PAGINAS_EXTRA;
                        masPaginas = (inicio < totalLineas);
                    }
                    while (masPaginas);

                    var lineasUltimaPagina = (totalLineas - LINEAS_PRIMERA_PAGINA_SIN_TOTALES) % LINEAS_PAGINAS_EXTRA;
                    var difLineasPrimeraPagina = LINEAS_PRIMERA_PAGINA_SIN_TOTALES - LINEAS_PRIMERA_PAGINA_SIN_TOTALES;

                    if (lineasUltimaPagina > LINEAS_PAGINAS_EXTRA - difLineasPrimeraPagina)
                    {
                        docPdf.NewPage();
                    }
                }
                else
                {
                    docPdf.NewPage();
                }               
            }
            else
            {
                agregarLineas(docPdf, 0, LINEAS_PRIMERA_PAGINA_CON_TOTALES,InvoicePDF.ListOperaciones);
            }

            // tabla de totales
            var tTotales = new PdfPTable(3);
            tTotales.WidthPercentage = 100;
            tTotales.SetWidths(new float[] { 4.5f, 3f, 1.5f });

            var iDatos = (TextoAdicional + "\r\n\r\n" + TextoConstancia + "\r\n\r\n" + TextoResolucion).Trim();
            var fnt7 = FontFactory.GetFont("Helvetica", 8, Font.NORMAL); //f9
            var cDatos = new PdfPCell(new Phrase(iDatos, fnt7));
            cDatos.BorderColor = BaseColor.GRAY;
            cDatos.Border = Rectangle.BOX;
            cDatos.HorizontalAlignment = Element.ALIGN_LEFT;
            cDatos.VerticalAlignment = Element.ALIGN_MIDDLE;
            cDatos.Rowspan = 11;
            cDatos.Padding = 4;
            cDatos.PaddingRight = 7;
            cDatos.PaddingLeft = 7;

            var fnt8 = FontFactory.GetFont("Helvetica", 9, Font.BOLD); //f10
            var ciSubtotalPU = new PdfPCell(new Phrase("Subtotal Precio Unitario (=)", fnt8));
            ciSubtotalPU.BorderColor = BaseColor.GRAY;
            ciSubtotalPU.Border = Rectangle.BOX;
            ciSubtotalPU.BorderWidthBottom = 0f;
            ciSubtotalPU.HorizontalAlignment = Element.ALIGN_CENTER;
            ciSubtotalPU.VerticalAlignment = Element.ALIGN_MIDDLE;
            ciSubtotalPU.Padding = 4;

            var fnt9 = FontFactory.GetFont("Helvetica", 9, Font.NORMAL); //f10
            var descuentosDetalle = InvoicePDF.DescuentosDetalle;
            var subtotalPU = InvoicePDF.SubtotalPrecioUnitario;
            var ivSubtotaoPU = subtotalPU;
            var cvSubtotalPU = new PdfPCell(new Phrase(ivSubtotaoPU, fnt9));
            cvSubtotalPU.BorderColor = BaseColor.GRAY;
            cvSubtotalPU.Border = Rectangle.BOX;
            cvSubtotalPU.BorderWidthBottom = 0f;
            cvSubtotalPU.HorizontalAlignment = Element.ALIGN_CENTER;
            cvSubtotalPU.VerticalAlignment = Element.ALIGN_MIDDLE;
            cvSubtotalPU.Padding = 4;

            var ciDescuentosDetalle = new PdfPCell(new Phrase("Descuentos detalle (-)", fnt8));
            ciDescuentosDetalle.BorderColor = BaseColor.GRAY;
            ciDescuentosDetalle.Border = Rectangle.BOX;
            ciDescuentosDetalle.BorderWidthTop = 0f;
            ciDescuentosDetalle.BorderWidthBottom = 0f;
            ciDescuentosDetalle.HorizontalAlignment = Element.ALIGN_CENTER;
            ciDescuentosDetalle.VerticalAlignment = Element.ALIGN_MIDDLE;
            ciDescuentosDetalle.Padding = 4;

            var ivDescuentosDetalle = descuentosDetalle;
            var cvDescuentosDetalle = new PdfPCell(new Phrase(ivDescuentosDetalle, fnt9));
            cvDescuentosDetalle.BorderColor = BaseColor.GRAY;
            cvDescuentosDetalle.Border = Rectangle.BOX;
            cvDescuentosDetalle.BorderWidthTop = 0f;
            cvDescuentosDetalle.BorderWidthBottom = 0f;
            cvDescuentosDetalle.HorizontalAlignment = Element.ALIGN_CENTER;
            cvDescuentosDetalle.VerticalAlignment = Element.ALIGN_MIDDLE;
            cvDescuentosDetalle.Padding = 4;

            var ciRecargosDetalle = new PdfPCell(new Phrase("Recargos detalle (+)", fnt8));
            ciRecargosDetalle.BorderColor = BaseColor.GRAY;
            ciRecargosDetalle.Border = Rectangle.BOX;
            ciRecargosDetalle.BorderWidthTop = 0f;
            ciRecargosDetalle.HorizontalAlignment = Element.ALIGN_CENTER;
            ciRecargosDetalle.VerticalAlignment = Element.ALIGN_MIDDLE;
            ciRecargosDetalle.Padding = 4;

            var recargosDetalle = InvoicePDF.RecargosDetalle;
            var iRecargosDetalle = recargosDetalle;
            var cvRecargosDetalle = new PdfPCell(new Phrase(iRecargosDetalle, fnt9));
            cvRecargosDetalle.BorderColor = BaseColor.GRAY;
            cvRecargosDetalle.Border = Rectangle.BOX;
            cvRecargosDetalle.BorderWidthTop = 0f;
            cvRecargosDetalle.HorizontalAlignment = Element.ALIGN_CENTER;
            cvRecargosDetalle.VerticalAlignment = Element.ALIGN_MIDDLE;
            cvRecargosDetalle.Padding = 4;

            var ciSubtotalNG = new PdfPCell(new Phrase("Subtotal No Gravados (=)", fnt8));
            ciSubtotalNG.BorderColor = BaseColor.GRAY;
            ciSubtotalNG.Border = Rectangle.BOX;
            ciSubtotalNG.BorderWidthBottom = 0f;
            ciSubtotalNG.HorizontalAlignment = Element.ALIGN_CENTER;
            ciSubtotalNG.VerticalAlignment = Element.ALIGN_MIDDLE;
            ciSubtotalNG.Padding = 4;

            var vSubtotalNG = InvoicePDF.SubtotalNoGravados;
            var iSubtotalNG = vSubtotalNG;
            var cvSubtotalNG = new PdfPCell(new Phrase(iSubtotalNG, fnt9));
            cvSubtotalNG.BorderColor = BaseColor.GRAY;
            cvSubtotalNG.Border = Rectangle.BOX;
            cvSubtotalNG.BorderWidthBottom = 0f;
            cvSubtotalNG.HorizontalAlignment = Element.ALIGN_CENTER;
            cvSubtotalNG.VerticalAlignment = Element.ALIGN_MIDDLE;
            cvSubtotalNG.Padding = 4;

            var ciSubtotalBG = new PdfPCell(new Phrase("Subtotal Base Gravable (=)", fnt8));
            ciSubtotalBG.BorderColor = BaseColor.GRAY;
            ciSubtotalBG.Border = Rectangle.BOX;
            ciSubtotalBG.BorderWidthTop = 0f;
            ciSubtotalBG.BorderWidthBottom = 0f;
            ciSubtotalBG.HorizontalAlignment = Element.ALIGN_CENTER;
            ciSubtotalBG.VerticalAlignment = Element.ALIGN_MIDDLE;
            ciSubtotalBG.Padding = 4;

            var iSubtotalBG = InvoicePDF.SubtotalBaseGravable;
            var cvSubtotalBG = new PdfPCell(new Phrase(iSubtotalBG, fnt9));
            cvSubtotalBG.BorderColor = BaseColor.GRAY;
            cvSubtotalBG.Border = Rectangle.BOX;
            cvSubtotalBG.BorderWidthTop = 0f;
            cvSubtotalBG.BorderWidthBottom = 0f;
            cvSubtotalBG.HorizontalAlignment = Element.ALIGN_CENTER;
            cvSubtotalBG.VerticalAlignment = Element.ALIGN_MIDDLE;
            cvSubtotalBG.Padding = 4;

            var ciTotalImpuesto = new PdfPCell(new Phrase("Total impuesto (+)", fnt8));
            ciTotalImpuesto.BorderColor = BaseColor.GRAY;
            ciTotalImpuesto.Border = Rectangle.BOX;
            ciTotalImpuesto.BorderWidthTop = 0f;
            ciTotalImpuesto.BorderWidthBottom = 0f;
            ciTotalImpuesto.HorizontalAlignment = Element.ALIGN_CENTER;
            ciTotalImpuesto.VerticalAlignment = Element.ALIGN_MIDDLE;
            ciTotalImpuesto.Padding = 4;

            var totalImpuesto = InvoicePDF.TotalImpuesto;
            var iTotalImpuesto = totalImpuesto;
            var cvTotalImpuesto = new PdfPCell(new Phrase(iTotalImpuesto, fnt9));
            cvTotalImpuesto.BorderColor = BaseColor.GRAY;
            cvTotalImpuesto.Border = Rectangle.BOX;
            cvTotalImpuesto.BorderWidthTop = 0f;
            cvTotalImpuesto.BorderWidthBottom = 0f;
            cvTotalImpuesto.HorizontalAlignment = Element.ALIGN_CENTER;
            cvTotalImpuesto.VerticalAlignment = Element.ALIGN_MIDDLE;
            cvTotalImpuesto.Padding = 4;

            var ciTotalMI = new PdfPCell(new Phrase("Total más impuesto (=)", fnt8));
            ciTotalMI.BorderColor = BaseColor.GRAY;
            ciTotalMI.Border = Rectangle.BOX;
            ciTotalMI.BorderWidthTop = 0f;
            ciTotalMI.HorizontalAlignment = Element.ALIGN_CENTER;
            ciTotalMI.VerticalAlignment = Element.ALIGN_MIDDLE;
            ciTotalMI.Padding = 4;

            var totalMI = InvoicePDF.TotalMasImpuesto;
            var iTotalMI = totalMI;
            var cvTotalMI = new PdfPCell(new Phrase(iTotalMI, fnt9));
            cvTotalMI.BorderColor = BaseColor.GRAY;
            cvTotalMI.Border = Rectangle.BOX;
            cvTotalMI.BorderWidthTop = 0f;
            cvTotalMI.HorizontalAlignment = Element.ALIGN_CENTER;
            cvTotalMI.VerticalAlignment = Element.ALIGN_MIDDLE;
            cvTotalMI.Padding = 4;

            var ciDescuentoGlobal = new PdfPCell(new Phrase("Descuento Global (-)", fnt8));
            ciDescuentoGlobal.BorderColor = BaseColor.GRAY;
            ciDescuentoGlobal.Border = Rectangle.BOX;
            ciDescuentoGlobal.BorderWidthBottom = 0f;
            ciDescuentoGlobal.HorizontalAlignment = Element.ALIGN_CENTER;
            ciDescuentoGlobal.VerticalAlignment = Element.ALIGN_MIDDLE;
            ciDescuentoGlobal.Padding = 4;

            var descuentoGlobal = InvoicePDF.DescuentoGlobal; 
            var iDescuentoGlobal = descuentoGlobal;
            var cvDescuentoGlobal = new PdfPCell(new Phrase(iDescuentoGlobal, fnt9));
            cvDescuentoGlobal.BorderColor = BaseColor.GRAY;
            cvDescuentoGlobal.Border = Rectangle.BOX;
            cvDescuentoGlobal.BorderWidthBottom = 0f;
            cvDescuentoGlobal.HorizontalAlignment = Element.ALIGN_CENTER;
            cvDescuentoGlobal.VerticalAlignment = Element.ALIGN_MIDDLE;
            cvDescuentoGlobal.Padding = 4;

            var ciRecargoGlobal = new PdfPCell(new Phrase("Recargo Global (+)", fnt8));
            ciRecargoGlobal.BorderColor = BaseColor.GRAY;
            ciRecargoGlobal.Border = Rectangle.BOX;
            ciRecargoGlobal.BorderWidthTop = 0f;
            ciRecargoGlobal.HorizontalAlignment = Element.ALIGN_CENTER;
            ciRecargoGlobal.VerticalAlignment = Element.ALIGN_MIDDLE;
            ciRecargoGlobal.Padding = 4;

            var recargoGlobal = InvoicePDF.RecargoGlobal;
            var iRecargoGlobal = recargoGlobal;
            var cvRecargoGlobal = new PdfPCell(new Phrase(iRecargoGlobal, fnt9));
            cvRecargoGlobal.BorderColor = BaseColor.GRAY;
            cvRecargoGlobal.Border = Rectangle.BOX;
            cvRecargoGlobal.BorderWidthTop = 0f;
            cvRecargoGlobal.HorizontalAlignment = Element.ALIGN_CENTER;
            cvRecargoGlobal.VerticalAlignment = Element.ALIGN_MIDDLE;
            cvRecargoGlobal.Padding = 4;

            var ciAnticipo = new PdfPCell(new Phrase("Anticipo (-)", fnt8));
            ciAnticipo.BorderColor = BaseColor.GRAY;
            ciAnticipo.Border = Rectangle.BOX;
            ciAnticipo.HorizontalAlignment = Element.ALIGN_CENTER;
            ciAnticipo.VerticalAlignment = Element.ALIGN_MIDDLE;
            ciAnticipo.Padding = 4;

            var anticipo = InvoicePDF.Anticipo;
            var iAnticipo = anticipo;
            var cvAnticipo = new PdfPCell(new Phrase(iAnticipo, fnt9));
            cvAnticipo.BorderColor = BaseColor.GRAY;
            cvAnticipo.Border = Rectangle.BOX;
            cvAnticipo.HorizontalAlignment = Element.ALIGN_CENTER;
            cvAnticipo.VerticalAlignment = Element.ALIGN_MIDDLE;
            cvAnticipo.Padding = 4;

            var ciTotalNeto = new PdfPCell(new Phrase("Valor Total", fnt8));
            ciTotalNeto.BorderColor = BaseColor.GRAY;
            ciTotalNeto.Border = Rectangle.BOX;
            ciTotalNeto.HorizontalAlignment = Element.ALIGN_CENTER;
            ciTotalNeto.VerticalAlignment = Element.ALIGN_MIDDLE;
            ciTotalNeto.Padding = 4;

            var vTotalAP = InvoicePDF.ValorTotal;
            var iTotalNeto = vTotalAP;
            var cvTotalNeto = new PdfPCell(new Phrase(iTotalNeto, fnt8));
            cvTotalNeto.BorderColor = BaseColor.GRAY;
            cvTotalNeto.Border = Rectangle.BOX;
            cvTotalNeto.HorizontalAlignment = Element.ALIGN_CENTER;
            cvTotalNeto.VerticalAlignment = Element.ALIGN_MIDDLE;
            cvTotalNeto.Padding = 4;

            var moneda = "COP";
            var iMontoLetras = montoALetras(Convert.ToDecimal(vTotalAP.Replace(".","")), moneda).ToUpper();
            var cMontoLetras = new PdfPCell(new Phrase(iMontoLetras, fnt7));
            cMontoLetras.BorderColor = BaseColor.GRAY;
            cMontoLetras.Border = Rectangle.BOX;
            cMontoLetras.HorizontalAlignment = Element.ALIGN_CENTER;
            cMontoLetras.VerticalAlignment = Element.ALIGN_MIDDLE;
            cMontoLetras.Padding = 6;
            cMontoLetras.Colspan = 3;

            tTotales.AddCell(cDatos);
            tTotales.AddCell(ciSubtotalPU);
            tTotales.AddCell(cvSubtotalPU);
            tTotales.AddCell(ciDescuentosDetalle);
            tTotales.AddCell(cvDescuentosDetalle);
            tTotales.AddCell(ciRecargosDetalle);
            tTotales.AddCell(cvRecargosDetalle);
            tTotales.AddCell(ciSubtotalNG);
            tTotales.AddCell(cvSubtotalNG);
            tTotales.AddCell(ciSubtotalBG);
            tTotales.AddCell(cvSubtotalBG);
            tTotales.AddCell(ciTotalImpuesto);
            tTotales.AddCell(cvTotalImpuesto);
            tTotales.AddCell(ciTotalMI);
            tTotales.AddCell(cvTotalMI);
            tTotales.AddCell(ciDescuentoGlobal);
            tTotales.AddCell(cvDescuentoGlobal);
            tTotales.AddCell(ciRecargoGlobal);
            tTotales.AddCell(cvRecargoGlobal);
            tTotales.AddCell(ciAnticipo);
            tTotales.AddCell(cvAnticipo);
            tTotales.AddCell(ciTotalNeto);
            tTotales.AddCell(cvTotalNeto);
            tTotales.AddCell(cMontoLetras);

            docPdf.Add(tTotales);

            docPdf.AddCreationDate();
            docPdf.AddCreator("SoftFinantec");
            docPdf.AddTitle(vNumeroFactura);

            writer.CloseStream = false;
            docPdf.Close();

            return streamPdf.ToArray();
        }

        private void agregarLineas(Document document, int desdeIndex, int cantidad,List<ModelListProductos> ListProductos)
        {
            // tabla de items
            var tItems = new PdfPTable(6);
            tItems.WidthPercentage = 100;
            tItems.SetWidths(new float[] { 0.5f, 3.5f, 0.5f, 1.25f, 1.75f, 1.5f });

            // encabezado de items
            var fnt71 = FontFactory.GetFont("Helvetica", 8, Font.NORMAL, BaseColor.WHITE); //f10
            var cILinea = new PdfPCell(new Phrase("#", fnt71));
            cILinea.BackgroundColor = new BaseColor(ColorPrimario.R, ColorPrimario.G, ColorPrimario.B);
            cILinea.BorderColor = new BaseColor(ColorPrimario.R, ColorPrimario.G, ColorPrimario.B);
            cILinea.Border = Rectangle.BOX;
            cILinea.HorizontalAlignment = Element.ALIGN_CENTER;
            cILinea.VerticalAlignment = Element.ALIGN_MIDDLE;
            cILinea.Padding = 4;

            var cIDescripcion = new PdfPCell(new Phrase("DESCRIPCIÓN", fnt71));
            cIDescripcion.BackgroundColor = new BaseColor(ColorPrimario.R, ColorPrimario.G, ColorPrimario.B);
            cIDescripcion.BorderColor = new BaseColor(ColorPrimario.R, ColorPrimario.G, ColorPrimario.B);
            cIDescripcion.Border = Rectangle.BOX;
            cIDescripcion.HorizontalAlignment = Element.ALIGN_CENTER;
            cIDescripcion.VerticalAlignment = Element.ALIGN_MIDDLE;
            cIDescripcion.Padding = 4;

            var cICantidad = new PdfPCell(new Phrase("CTD.", fnt71));
            cICantidad.BackgroundColor = new BaseColor(ColorPrimario.R, ColorPrimario.G, ColorPrimario.B);
            cICantidad.BorderColor = new BaseColor(ColorPrimario.R, ColorPrimario.G, ColorPrimario.B);
            cICantidad.Border = Rectangle.BOX;
            cICantidad.HorizontalAlignment = Element.ALIGN_CENTER;
            cICantidad.VerticalAlignment = Element.ALIGN_MIDDLE;
            cICantidad.Padding = 4;

            var cIUnitario = new PdfPCell(new Phrase("VALOR\r\nUNITARIO", fnt71));
            cIUnitario.BackgroundColor = new BaseColor(ColorPrimario.R, ColorPrimario.G, ColorPrimario.B);
            cIUnitario.BorderColor = new BaseColor(ColorPrimario.R, ColorPrimario.G, ColorPrimario.B);
            cIUnitario.Border = Rectangle.BOX;
            cIUnitario.HorizontalAlignment = Element.ALIGN_CENTER;
            cIUnitario.VerticalAlignment = Element.ALIGN_MIDDLE;
            cIUnitario.Padding = 4;

            var cIImpuestos = new PdfPCell(new Phrase("IMPUESTOS", fnt71));
            cIImpuestos.BackgroundColor = new BaseColor(ColorPrimario.R, ColorPrimario.G, ColorPrimario.B);
            cIImpuestos.BorderColor = new BaseColor(ColorPrimario.R, ColorPrimario.G, ColorPrimario.B);
            cIImpuestos.Border = Rectangle.BOX;
            cIImpuestos.HorizontalAlignment = Element.ALIGN_CENTER;
            cIImpuestos.VerticalAlignment = Element.ALIGN_MIDDLE;
            cIImpuestos.Padding = 4;

            var cITotal = new PdfPCell(new Phrase("SUBTOTAL", fnt71));
            cITotal.BackgroundColor = new BaseColor(ColorPrimario.R, ColorPrimario.G, ColorPrimario.B);
            cITotal.BorderColor = new BaseColor(ColorPrimario.R, ColorPrimario.G, ColorPrimario.B);
            cITotal.Border = Rectangle.BOX;
            cITotal.HorizontalAlignment = Element.ALIGN_CENTER;
            cITotal.VerticalAlignment = Element.ALIGN_MIDDLE;
            cITotal.Padding = 4;

            tItems.AddCell(cILinea);
            tItems.AddCell(cIDescripcion);
            tItems.AddCell(cICantidad);
            tItems.AddCell(cIUnitario);
            tItems.AddCell(cIImpuestos);
            tItems.AddCell(cITotal);

            var fnt82 = FontFactory.GetFont("Helvetica", 7, Font.NORMAL); //f9

            // agregar lineas
            var totalLineas = ListProductos.Count();
            for (int i = 0; i < cantidad; i++)
            {
                int x = (desdeIndex + i);

                // verificar y agregar
                if (x < totalLineas)
                {
                    var linea = ListProductos[x];

                    bool lastLinea = (i == cantidad - 1 || x + 1 >= totalLineas);
                    int bordes = Rectangle.LEFT_BORDER | Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER;
                    if (!lastLinea) bordes = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;

                    var vxLinea = (x + 1).ToString();
                    var cXLinea = new PdfPCell(new Phrase(vxLinea, fnt82));
                    cXLinea.Border = bordes;
                    cXLinea.BorderColor = BaseColor.GRAY;
                    cXLinea.HorizontalAlignment = Element.ALIGN_CENTER;
                    cXLinea.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cXLinea.Padding = 3;

                    var vxDescripcion = linea.Descripcion;
                    var cXDescripcion = new PdfPCell(new Phrase(vxDescripcion, fnt82));
                    cXDescripcion.FixedHeight = 25f;
                    cXDescripcion.Border = bordes;
                    cXDescripcion.BorderColor = BaseColor.GRAY;
                    cXDescripcion.HorizontalAlignment = Element.ALIGN_CENTER;
                    cXDescripcion.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cXDescripcion.Padding = 3;
                    cXDescripcion.PaddingRight = 7;
                    cXDescripcion.PaddingLeft = 7;

                    var vxCantidad = linea.Cantidad.ToString();
                    var cXCantidad = new PdfPCell(new Phrase(vxCantidad, fnt82));
                    cXCantidad.Border = bordes;
                    cXCantidad.BorderColor = BaseColor.GRAY;
                    cXCantidad.HorizontalAlignment = Element.ALIGN_CENTER;
                    cXCantidad.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cXCantidad.Padding = 3;

                    var vxPrecio = linea.ValorUnitario;
                    var vxDescuento = 0;
                    var vxCargo = 0;

                    var vxUnitario = vxPrecio;
                    var cXUnitario = new PdfPCell(new Phrase(vxUnitario, fnt82));
                    cXUnitario.Border = bordes;
                    cXUnitario.BorderColor = BaseColor.GRAY;
                    cXUnitario.HorizontalAlignment = Element.ALIGN_CENTER;
                    cXUnitario.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cXUnitario.Padding = 3;

                    var vxImpuesto = linea.Impuesto;
                    
                    var cXImpuestos = new PdfPCell(new Phrase(vxImpuesto, fnt82));
                    cXImpuestos.Border = bordes;
                    cXImpuestos.BorderColor = BaseColor.GRAY;
                    cXImpuestos.HorizontalAlignment = Element.ALIGN_CENTER;
                    cXImpuestos.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cXImpuestos.Padding = 3;

                    var vxTotal = linea.ValorTotal;
                    var cXTotal = new PdfPCell(new Phrase(vxTotal, fnt82));
                    cXTotal.Border = bordes;
                    cXTotal.BorderColor = BaseColor.GRAY;
                    cXTotal.HorizontalAlignment = Element.ALIGN_CENTER;
                    cXTotal.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cXTotal.Padding = 3;

                    tItems.AddCell(cXLinea);
                    tItems.AddCell(cXDescripcion);
                    tItems.AddCell(cXCantidad);
                    tItems.AddCell(cXUnitario);
                    tItems.AddCell(cXImpuestos);
                    tItems.AddCell(cXTotal);
                }
            }

            document.Add(tItems);
        }
    }
}