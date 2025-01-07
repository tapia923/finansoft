using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanillajeColectivos.DTO.FacturacionElectronica
{
    public class LineXML
    {
        public string LineID { get; set; }//Comienza desde 1 e incrementa por cada producto de la factura
        public string InvoicedQuantity { get; set; }//Cantidad de unidades vendidas del producto
        public string InvoiceLineExtensionAmount { get; set; }//Cantidad * precio menos desucuentos más recargos de ese producto

        //si el producto tiene descuento
        public string AllowanceChargeID { get; set; }//Consecutivo de descuentos que presenta el producto
        public bool ChargeIndicator { get; set; }//false si es descuento, true si es cargo
        public string BaseAmount { get; set; }//Valor del producto*cantidad antes del descuento
        public string AllowanceChargeReason { get; set; }//Razón del descuento
        public string MultiplierFactorNumeric { get; set; }//Porcentaje de descuento. ej: 10.00
        public string Amount { get; set; }//Valor total del cargo o descuento
        public string LineTotal { get; set; }//BaseAmount+/-Amount
        public string LineTax { get; set; }//valor del iva sacado a LineTotal(valor base)
        public string LineTaxPercentage { get; set; }//valor del iva o tributo
        public string LineItemName { get; set; }//Nombre del producto
        public string LineItemTotal { get; set; }//Todo el total del producto(1 fila de la factura)
    }
}
