using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanillajeColectivos.DTO.FacturacionElectronica
{
    public class modelXML
    {
        
        public string InvoiceAuthorization { get; set; } //Número autorización: Número del código de la resolución otorgada para la numeración
        public string StartDate { get; set; } //Fecha inicio autorizada de resolución de facturación
        public string EndDate { get; set; } //Fecha inicio autorizada de resolución de facturación

        public string Prefix { get; set; } //prefijo de la factura
        public long From { get; set; }
        public long To { get; set; }
        public string CompanyNIT  { get; set; }
        public string SoftwareID  { get; set; }//en la base de datos el campo: SOFTWARE_IDENTIFICADOR
        public string pin  { get; set; }
        public string SoftwareSecurityCode { get; set; } //has sh384(SoftwareID,pin)
        public string AuthorizationProviderID { get; set; } //nit de la DIAN
        public string cacPartyIdentification { get; set; } //nit de la empresa desarrolladora (en este caso es el mismo nit de la empresa ya que se lo registró como software propio)
        
        public string QRCode { get; set; } //

        public string CustomizationID { get; set; } //lleva el valor de 10
        public string ProfileExecutionID { get; set; } //valor de 1 para ambiente de producción y 2 para pruebas
        public string ID { get; set; } //prefijo + el numero de la factura (Prefix+From)
        public string UUID { get; set; } //CUFE (revisar anexo técnico numeral 11.1.2)
        public DateTime IssueDate { get; set; } //fecha de expedición del documento formato yyyy-MM-dd
        public DateTime IssueTime { get; set; } //hora de expedición del documento formato: 09:15:23-05:00
        public string InvoiceTypeCode { get; set; } //01 para factura de venta, 02 de exportación, 03 por Contingencia facturador, 04 por contingencia DIAN, 91 nota crédito, 92 nota débito
        public int LineCountNumeric { get; set; }//Número de productos que tiene nuestra factura
        public DateTime InvoicePeriodStartDate { get; set; } //inicio de periodo de facturación: inicio de mes en el cual se realiza la factura
        public DateTime InvoicePeriodEndDate { get; set; }//fin de periodo de facturación: fin de mes en el cual se realiza la factura

        //AccountingSupplierParty: establecimiento que está facturando
        public int AdditionalAccountID { get; set; } //1 para persona jurídica y 2 para persona natural
        public string IndustriyClasificationCode { get; set; } //código de la actividad de la empresa (dentro de la etiqueta party en el xml)
        public string CompanyName { get; set; } //Nombre de la empresa
        public string CompanyPostCode { get; set; } //Código postal de la ciudad de la empresa
        public string CompanyCity { get; set; } //Nombre de la ciudad en donde se ubica la empresa
        public string CompanyDepto { get; set; } //Nombre del departamento  en donde se ubica la empresa
        public string CompanyDeptoCode { get; set; } //Código del departamento  en donde se ubica la empresa (revisar anéxo técnico de la DIAN)
        public string CompanyAddress { get; set; } //Dirección de la empresa
        public string CorporateRegistrationId { get; set; } //Prefijo de numeración
        public string CorporateRegistrationName { get; set; } //Matricula mercantil

        //PartyTaxScheme: Grupo de información tributaria del emisor
        public string RegistrationName { get; set; }// igual a CompanyName
        public string CompanyID { get; set; }// igual a CompanyNIT
        public string TaxLevelCode { get; set; }//responsablidida fiscal (revisar anéxo técnico de la DIAN) 
        public string CityCode { get; set; }// Código de la ciudad donde está la empresa
        public string TaxSchemeId { get; set; }// 01 para IVA (revisar anéxo técnico de la DIAN)
        public string TaxSchemeName { get; set; }//IVA(revisar anéxo técnico de la DIAN)

        //AccountingCustomerParty
        public string CustomerName { get; set; }//Nombre de cliente
        public string CustomerCityCode { get; set; }//Código de la ciudad del cliente
        public string CustomerCity { get; set; }//ciudad del cliente
        public string CustomerDepto { get; set; }//Departamento del cliente
        public string CustomerDeptoCode { get; set; }//Código del dpto del cliente
        public string CustomerCountryIdentificationCode { get; set; }//Código del pais del cliente. ejemplo: CO,EC..
        public string CustomerCountryName { get; set; }//Nombre del pais del cliente
        public string CustomerAddress { get; set; }//Dirección del cliente
        public string CustomerNit { get; set; }//NIT del cliente
        public string CustomerIdCode { get; set; }//Tipo de documento de identidad del cliente(revisar numeral 13.2 anexo técnico de la DIAN)

        //PaymentMeans
        public string PaymentMeansID { get; set; }//Método de pago 1)Contado, 2)Crédito 
        public string PaymentMeansCode { get; set; }//Medio de pago (verificar anexo tecnico de la DIAN 13.3.4.2)
        public string TaxTotalAmount { get; set; }//Total suma de los valores de los impuestos. Ej: totIva19+totIva5

        //TaxTotal: lleva varias etiquetas(puede llevar iva e impuesto al consumo). Son los tributos que generan la factura (revisar anexo técnico de la DIAN numeral 13.2.2)
        public string TaxableAmount { get; set; }//Total taxeable de la factura(antes de iva)
        public string TaxAmount { get; set; }//Total valor del tributo o impuesto
        public string Percent { get; set; }//Valor del impuesto: ejemplo 19.00


        //LegalMonetaryTotal
        public string LineExtensionAmount { get; set; }//Total valor bruto antes de tributos
        public string AllowanceTotalAmount { get; set; }//Valor total de los descuentos
        public string TaxExclusiveAmount { get; set; }//Total valor antes de iva con cálculos de desucuentos (LineExtensionAmount-AllowanceTotalAmount)
        public string TaxInclusiveAmount { get; set; }//sumatoria total
        public string PayableAmount { get; set; }//sumatoria total = TaxInclusiveAmount


        //InvoiceLine: para cada uno de los productos de la factura
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


        //firma digital


    }
}
