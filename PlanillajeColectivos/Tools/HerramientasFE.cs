using FE.Documentos;
using FE.ServiciosWeb;
using FE.ServiciosWeb.DianServices;
using FEDIAN.Util;
using Grpc.Core;
using PlanillajeColectivos.DTO;
using PlanillajeColectivos.DTO.FacturacionElectronica;
using PlanillajeColectivos.DTO.Products;
using QRCoder;
using Rotativa;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Security.Cryptography;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

namespace PlanillajeColectivos.Tools
{
    public static class HerramientasFE
    {
        public static string getCUFE(ViewModelCUFE CUFE)
        {
            string horaFact = Time.FechaLocal.GetHoraColombia().ToString();
            var cadena = CUFE.NumFac + CUFE.FecFac + horaFact + CUFE.ValorBruto + CUFE.CodImp1 + CUFE.ValorImp1 + CUFE.CodImp2 + CUFE.ValorImp2 + CUFE.CodImp3 + CUFE.ValorImp3 + CUFE.ValTot + CUFE.NitFE + CUFE.NumAdq + CUFE.CITec + CUFE.TipoAmbiente;
            //var cadena = CUFE.NumFac + CUFE.FecFac + CUFE.HoraFac + CUFE.ValorBruto + CUFE.CodImp1 + CUFE.ValorImp1 + CUFE.ValTot + CUFE.NitFE + CUFE.NumAdq + CUFE.CITec + CUFE.TipoAmbiente;
            using (var sha384Hash = SHA384.Create())
            {
                var sourceBytes = Encoding.UTF8.GetBytes(cadena);
                var hashBytes = sha384Hash.ComputeHash(sourceBytes);
                var hash = BitConverter.ToString(hashBytes).Replace("-", string.Empty);

                return hash.ToLower();
            }
            
        }


        public static string FORMATOSHA384(string str)
        {
            //SHA384 sha384 = SHA384Managed.Create();
            //ASCIIEncoding encoding = new ASCIIEncoding();
            //byte[] stream = null;
            //StringBuilder sb = new StringBuilder();
            //stream = sha384.ComputeHash(encoding.GetBytes(str));
            //for (int i = 0; i < stream.Length; i++) sb.AppendFormat("{0:x2}", stream[i]);
            //return sb.ToString();
            string cadena = CryptographyHelper.Sha384(str);
            return cadena;
        }


        #region estructura XML
        public static string formHeadXML()
        {
            string cadena = "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"no\"?>" +
                "<Invoice xmlns=\"urn:oasis:names:specification:ubl:schema:xsd:Invoice-2\" xmlns:cac=\"urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2\" xmlns:cbc=\"urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2\" xmlns:ds=\"http://"+"www.w3.org/2000/09/xmldsig#\" xmlns:ext=\"urn:oasis:names:specification:ubl:schema:xsd:CommonExtensionComponents-2\" xmlns:sts=\"dian:gov:co:facturaelectronica:Structures-2-1\" xmlns:xades=\"h"+"ttp://uri.etsi.org/01903/v1.3.2#\" xmlns:xades141=\"h"+"ttp://uri.etsi.org/01903/v1.4.1#\" xmlns:xsi=\"h"+"ttp://www.w3.org/2001/XMLSchema-instance\" xsi:schemaLocation=\"urn:oasis:names:specification:ubl:schema:xsd:Invoice-2     h"+"ttp://docs.oasis-open.org/ubl/os-UBL-2.1/xsd/maindoc/UBL-Invoice-2.1.xsd\">";
            return cadena;
        }

        public static string formExtensionXML(ExtensionXML modelExtension)
        {
            var stsInvoiceAuthorization = modelExtension.InvoiceAuthorization;
            var cbcStartDate = modelExtension.cbcStartDate;
            var cbcEndDate = modelExtension.cbcEndDate;
            var stsPrefix = modelExtension.stsPrefix;
            var stsFrom = modelExtension.stsFrom;
            var stsTo = modelExtension.stsTo;
            var cacPartyIdentification = modelExtension.cacPartyIdentification;
            var stsSoftwareID = modelExtension.stsSoftwareID;
            var stsSoftwareSecurityCode = modelExtension.stsSoftwareSecurityCode;
            var QRCode = modelExtension.QRCode;
            var DigitalSignature = modelExtension.DigitalSignature;

           var cadena= "<ext:UBLExtensions>"
              + "<ext:UBLExtension>"
                  + "<ext:ExtensionContent>"
                  + "<sts:DianExtensions>"
                       + "<sts:InvoiceControl>"
                              + "<sts:InvoiceAuthorization>"+stsInvoiceAuthorization+"</sts:InvoiceAuthorization>"
                                   + "<sts:AuthorizationPeriod>"
                        + "<cbc:StartDate>" + cbcStartDate + "</cbc:StartDate>"
                                + "<cbc:EndDate>" + cbcEndDate + "</cbc:EndDate>"
                                        + "</sts:AuthorizationPeriod>"
                                        + "<sts:AuthorizedInvoices>"
                                            + "<sts:Prefix>" + stsPrefix + "</sts:Prefix>"
                                                + "<sts:From>" + stsFrom + "</sts:From>"
                                                + "<sts:To>" + stsTo + "</sts:To>"
                                                + "</sts:AuthorizedInvoices>"
                                                + "</sts:InvoiceControl>"
                                                + "<sts:InvoiceSource>"
                                                    + "<cbc:IdentificationCode listAgencyID = \"6\" listAgencyName=\"United Nations Economic Commission for Europe\" listSchemeURI=\"urn:oasis:names:specification:ubl:codelist:gc:CountryIdentificationCode-2.1\">CO</cbc:IdentificationCode>"
                                                    + "</sts:InvoiceSource>"
                                                    + "<sts:SoftwareProvider>"
                                                        + "<sts:ProviderID schemeAgencyID=\"195\" schemeAgencyName=\"CO, DIAN (Dirección de Impuestos y Aduanas Nacionales)\" schemeID=\"6\" schemeName=\"31\">" + cacPartyIdentification + "</sts:ProviderID>"
                                                        + "<sts:SoftwareID schemeAgencyID=\"195\" schemeAgencyName=\"CO, DIAN (Dirección de Impuestos y Aduanas Nacionales)\">" + stsSoftwareID + "</sts:SoftwareID>"
                                                        + "</sts:SoftwareProvider>"
                                                        + "<sts:SoftwareSecurityCode schemeAgencyID=\"195\" schemeAgencyName=\"CO, DIAN (Dirección de Impuestos y Aduanas Nacionales)\">" + stsSoftwareSecurityCode + "</sts:SoftwareSecurityCode>"
                                                        + "<sts:AuthorizationProvider>"
                                                            + "<sts:AuthorizationProviderID schemeAgencyID=\"195\" schemeAgencyName=\"CO, DIAN (Dirección de Impuestos y Aduanas Nacionales)\" schemeID=\"4\" schemeName=\"31\">800197268</sts:AuthorizationProviderID>"
                                                                        + "</sts:AuthorizationProvider>"
                                                                        + "<sts:QRCode>" + QRCode + "</sts:QRCode>"
                   + "</sts:DianExtensions>"
                 + "</ext:ExtensionContent>"
               + "</ext:UBLExtension>"
               + "<ext:UBLExtension>"
                    + "<ext:ExtensionContent></ext:ExtensionContent>"
                + "</ext:UBLExtension>"
               + "</ext:UBLExtensions>";

            return cadena;
        }

        public static string formVersionXML(VersionXML modelVersion)
        {
            var CustomizationID = modelVersion.CustomizationID;
            var ProfileExecutionID = modelVersion.ProfileExecutionID;
            var ID = modelVersion.ID;
            var UUID = modelVersion.UUID;
            var IssueDate = modelVersion.IssueDate;
            var IssueTime = modelVersion.IssueTime;
            var InvoiceTypeCode = modelVersion.InvoiceTypeCode;
            var LineCountNumeric = modelVersion.LineCountNumeric;
            var InvoicePeriodStartDate = modelVersion.InvoicePeriodStartDate;
            var InvoicePeriodEndDate = modelVersion.InvoicePeriodEndDate;


            string cadena = "<cbc:UBLVersionID>UBL 2.1</cbc:UBLVersionID>"
              + "<cbc:CustomizationID>" + CustomizationID + "</cbc:CustomizationID>"
               + "<cbc:ProfileID>DIAN 2.1: Factura Electrónica de Venta</cbc:ProfileID>"
                    + "<cbc:ProfileExecutionID>" + ProfileExecutionID + "</cbc:ProfileExecutionID>"
                         + "<cbc:ID>" + ID + "</cbc:ID>"
                              + "<cbc:UUID schemeID=\"2\" schemeName=\"CUFE-SHA384\">" + UUID + "</cbc:UUID>" //2para pruebas y 1 para producción
                                       + "<cbc:IssueDate>" + IssueDate + "</cbc:IssueDate>"
                                                + "<cbc:IssueTime>" + IssueTime + "</cbc:IssueTime>"
                                                       + "<cbc:InvoiceTypeCode>" + InvoiceTypeCode + "</cbc:InvoiceTypeCode>"
                                                                + "<cbc:DocumentCurrencyCode listAgencyID=\"6\" listAgencyName=\"United Nations Economic Commission for Europe\" listID=\"ISO 4217 Alpha\">COP</cbc:DocumentCurrencyCode>"
                                                                        + "<cbc:LineCountNumeric>" + LineCountNumeric + "</cbc:LineCountNumeric>"
                                                                              + "<cac:InvoicePeriod>"
                                                                                    + "<cbc:StartDate>" + InvoicePeriodStartDate + "</cbc:StartDate>"
                                                                                     + "<cbc:EndDate>" + InvoicePeriodEndDate + "</cbc:EndDate>"
                                                                                + "</cac:InvoicePeriod>";

            return cadena;
        }

        public static string formCompanyXML(CompanyXML modelCompany)
        {

            var AdditionalAccountID = modelCompany.AdditionalAccountID;
            var IndustriyClasificationCode = modelCompany.IndustriyClasificationCode;
            var CompanyName = modelCompany.CompanyName;
            var CompanyPostCode = modelCompany.CompanyPostCode;
            var CompanyCity = modelCompany.CompanyCity;
            var CompanyCityCode = modelCompany.CompanyCityCode;
            var CompanyDepto = modelCompany.CompanyDepto;
            var CompanyDeptoCode = modelCompany.CompanyDeptoCode;
            var CompanyAddress = modelCompany.CompanyAddress;
            var CompanyNIT = modelCompany.CompanyNIT;
            var TaxLevelCode = modelCompany.TaxLevelCode;
            var CityCode = modelCompany.CityCode;
            var TaxSchemeId = modelCompany.TaxSchemeId;
            var TaxSchemeName = modelCompany.TaxSchemeName;
            var CompanyEmail = modelCompany.CompanyEmail;
            var CorporateRegistrationId = modelCompany.CorporateRegistrationId;
            var CorporateRegistrationName = modelCompany.CorporateRegistrationName;

            var cadena = "<cac:AccountingSupplierParty>"
        + "<cbc:AdditionalAccountID>" + AdditionalAccountID + "</cbc:AdditionalAccountID>"
             + "<cac:Party>"
             + "<cbc:IndustryClassificationCode>" + IndustriyClasificationCode + "</cbc:IndustryClassificationCode>"
                 + "<cac:PartyName>"
                      + "<cbc:Name>" + CompanyName + "</cbc:Name>"
                  + "</cac:PartyName>"
                  + "<cac:PhysicalLocation>"
                     + "<cac:Address>"
                         + "<cbc:ID>" + CompanyCityCode + "</cbc:ID>"
                         + "<cbc:CityName>" + CompanyCity + "</cbc:CityName>"
                         + "<cbc:CountrySubentity>" + CompanyDepto + "</cbc:CountrySubentity>"
                         + "<cbc:CountrySubentityCode>" + CompanyDeptoCode + "</cbc:CountrySubentityCode>"
                         + "<cac:AddressLine>"
                             + "<cbc:Line>" + CompanyAddress + "</cbc:Line>"
                         + "</cac:AddressLine>"
                         + "<cac:Country>"
                             + "<cbc:IdentificationCode>CO</cbc:IdentificationCode>"
                             + "<cbc:Name languageID=\"es\">Colombia</cbc:Name>"
                         + "</cac:Country>"
                     + "</cac:Address>"
                          + "</cac:PhysicalLocation>"
                           + "<cac:PartyTaxScheme>"
                               + "<cbc:RegistrationName>" + CompanyName + "</cbc:RegistrationName>"
                               //+ "<cbc:CompanyID schemeAgencyID=\"195\" schemeAgencyName=\"CO, DIAN (Dirección de Impuestos y Aduanas Nacionales)\" schemeID=\"4\" schemeName=\"31\">" + CompanyNIT + "</cbc:CompanyID>" (DESCOMENTAR ESTA LINEA SI EL EMISOR TIENE REGISTRO CON NIT Y DIGITO DE VERIFICACION ) 
                               + "<cbc:CompanyID schemeAgencyID=\"195\" schemeAgencyName=\"CO, DIAN (Dirección de Impuestos y Aduanas Nacionales)\" schemeName=\"13\">" + CompanyNIT + "</cbc:CompanyID>"//(COMENTAR ESTA LINEA SI SE DESCOMENTA LA DE ARRIBA)
                               + "<cbc:TaxLevelCode listName=\"05\">" + TaxLevelCode + "</cbc:TaxLevelCode>"
                               + "<cac:RegistrationAddress>"
                                     + "<cbc:ID>" + CityCode + "</cbc:ID>"
                                     + "<cbc:CityName>" + CompanyCity + "</cbc:CityName>"
                                     + "<cbc:CountrySubentity>" + CompanyDepto + "</cbc:CountrySubentity>"
                                     + "<cbc:CountrySubentityCode>" + CompanyDeptoCode + "</cbc:CountrySubentityCode>"
                                     + "<cac:AddressLine>"
                                        + "<cbc:Line>" + CompanyAddress + "</cbc:Line>"
                                     + "</cac:AddressLine>"
                                     + "<cac:Country>"
                                         + "<cbc:IdentificationCode>CO</cbc:IdentificationCode>"
                                         + "<cbc:Name languageID=\"es\">Colombia</cbc:Name>"
                                     + "</cac:Country>"
                              + "</cac:RegistrationAddress>"
                              + "<cac:TaxScheme>"
                                 + "<cbc:ID>" + TaxSchemeId + "</cbc:ID>"
                                 + "<cbc:Name>" + TaxSchemeName + "</cbc:Name>"
                              + "</cac:TaxScheme>"
                        + "</cac:PartyTaxScheme>"
                        + "<cac:PartyLegalEntity>"
                            + "<cbc:RegistrationName>" + CompanyName + "</cbc:RegistrationName>"
                            + "<cbc:CompanyID schemeAgencyID=\"195\" schemeAgencyName=\"CO, DIAN (Dirección de Impuestos y Aduanas Nacionales)\" schemeID=\"6\" schemeName=\"31\">" + CompanyNIT + "</cbc:CompanyID>"
                            +"<cac:CorporateRegistrationScheme>"
                            +"<cbc:ID>"+CorporateRegistrationId+"</cbc:ID>"
                            +"<cbc:Name>"+CorporateRegistrationName+"</cbc:Name>"
                           +"</cac:CorporateRegistrationScheme>"
                        +"</cac:PartyLegalEntity>"
                        +"<cac:Contact>"
                            +"<cbc:ElectronicMail>"+CompanyEmail+"</cbc:ElectronicMail>"
                        +"</cac:Contact>"
                + "</cac:Party>"
             + "</cac:AccountingSupplierParty>";

            return cadena;
        }

        public static string formCustomerXMl(CustomerXML modelCustomer)
        {

            var AdditionalAccountID = modelCustomer.AdditionalAccountID;
            var CustomerName = modelCustomer.CustomerName;
            var CustomerCityCode = modelCustomer.CustomerCityCode;
            var CustomerCity = modelCustomer.CustomerCity;
            var CustomerDepto = modelCustomer.CustomerDepto;
            var CustomerDeptoCode = modelCustomer.CustomerDeptoCode;
            var CustomerAddress = modelCustomer.CustomerAddress;
            var CustomerCountryIdentificationCode = modelCustomer.CustomerCountryIdentificationCode;
            var CustomerCountryName = modelCustomer.CustomerCountryName;
            var CustomerIdCode = modelCustomer.CustomerIdCode;
            var CustomerNit = modelCustomer.CustomerNit;
            var CustomerResponsabilidadFiscal = modelCustomer.CustomerResponsabilidadFiscal;
            

            var cadena = "<cac:AccountingCustomerParty>"
              + "<cbc:AdditionalAccountID>" + AdditionalAccountID + "</cbc:AdditionalAccountID>"
                   + "<cac:Party>"
                        +"<cac:PartyIdentification>"
                            +"<cbc:ID schemeAgencyID=\"195\" schemeAgencyName=\"CO, DIAN (Dirección de Impuestos y Aduanas Nacionales)\" schemeName=\""+CustomerIdCode+"\">"+CustomerNit+"</cbc:ID>"
                        +"</cac:PartyIdentification>"
                       + "<cac:PartyName>"
                           + "<cbc:Name>" + CustomerName + "</cbc:Name>"
                       + "</cac:PartyName>"
                       + "<cac:PhysicalLocation>"
                            + "<cac:Address>"
                                + "<cbc:ID>" + CustomerCityCode + "</cbc:ID>"
                                + "<cbc:CityName>" + CustomerCity + "</cbc:CityName>"
                                + "<cbc:CountrySubentity>" + CustomerDepto + "</cbc:CountrySubentity>"
                                + "<cbc:CountrySubentityCode>" + CustomerDeptoCode + "</cbc:CountrySubentityCode>"
                                + "<cac:AddressLine>"
                                     + "<cbc:Line>" + CustomerAddress + "</cbc:Line>"
                                + "</cac:AddressLine>"
                                + "<cac:Country>"
                                    + "<cbc:IdentificationCode>"+ CustomerCountryIdentificationCode + "</cbc:IdentificationCode>"
                                    + "<cbc:Name languageID=\"es\">"+ CustomerCountryName + "</cbc:Name>"
                                + "</cac:Country>"
                            + "</cac:Address>"
                        + "</cac:PhysicalLocation>"
                        + "<cac:PartyTaxScheme>"
                            + "<cbc:RegistrationName>" + CustomerName + "</cbc:RegistrationName>"
                            + "<cbc:CompanyID schemeAgencyID=\"195\" schemeAgencyName=\"CO, DIAN (Dirección de Impuestos y Aduanas Nacionales)\"  schemeName=\""+CustomerIdCode+"\">"+CustomerNit+"</cbc:CompanyID>"
                            +"<cbc:TaxLevelCode>"+CustomerResponsabilidadFiscal+"</cbc:TaxLevelCode>"// linea opcional(actualmente presenta error cuando se factura con consumidor final)"
                             + "<cac:TaxScheme>"
                                + "<cbc:ID>ZZ</cbc:ID>"
                                + "<cbc:Name>No aplica</cbc:Name>"
                            + "</cac:TaxScheme>"
                        + "</cac:PartyTaxScheme>"
                        + "<cac:PartyLegalEntity>"
                            + "<cbc:RegistrationName>"+CustomerName+"</cbc:RegistrationName>"
                            + "<cbc:CompanyID schemeAgencyID=\"195\" schemeAgencyName=\"CO, DIAN (Dirección de Impuestos y Aduanas Nacionales)\" schemeID=\"3\" schemeName=\""+CustomerIdCode+"\">"+CustomerNit+"</cbc:CompanyID>"
                        + "</cac:PartyLegalEntity>"
                    + "</cac:Party>"
                + "</cac:AccountingCustomerParty>";

            return cadena;
        }

        public static string formTotalXML(TotalXML modelTotal)
        {

            var PaymentMeansID = modelTotal.PaymentMeansID;
            var PaymentMeansCode = modelTotal.PaymentMeansCode;
            var TaxAmount = modelTotal.TaxAmount;
            var TaxSubTotal = modelTotal.TaxSubTotal;
            var LineExtensionAmount = modelTotal.LineExtensionAmount;
            var AllowanceTotalAmount = modelTotal.AllowanceTotalAmount;
            var TaxExclusiveAmount = modelTotal.TaxExclusiveAmount;
            var TaxInclusiveAmount = modelTotal.TaxInclusiveAmount;
            var PayableAmount = modelTotal.PayableAmount;

            var cadena = "<cac:PaymentMeans>"
            + "<cbc:ID>"+PaymentMeansID+"</cbc:ID>"
                + "<cbc:PaymentMeansCode>"+PaymentMeansCode+"</cbc:PaymentMeansCode>"
                + "</cac:PaymentMeans>"
              + "<cac:TaxTotal>"
                  + "<cbc:TaxAmount currencyID=\"COP\">" + TaxAmount + " </cbc:TaxAmount>";
            foreach(var item in TaxSubTotal)
            {
                cadena += "<cac:TaxSubtotal>"
                           + "<cbc:TaxableAmount currencyID=\"COP\">"+item.TaxableAmount+"</cbc:TaxableAmount>"
                           + "<cbc:TaxAmount currencyID=\"COP\">"+item.TaxAmount+"</cbc:TaxAmount>"
                           + "<cac:TaxCategory>"
                               + "<cbc:Percent>"+item.Percent+"</cbc:Percent>"
                               + "<cac:TaxScheme>"
                                   + "<cbc:ID>01</cbc:ID>"
                                   + "<cbc:Name>IVA</cbc:Name>"
                               + "</cac:TaxScheme>"
                           + "</cac:TaxCategory>"
                + "</cac:TaxSubtotal>";
            }
            
             //cadena+="</cac:TaxTotal>"
            //+ "<cac:TaxTotal>"
            //    + "<cbc:TaxAmount currencyID=\"COP\">0</cbc:TaxAmount>"
            //   cadena += "<cac:TaxSubtotal>"
            //         + "<cbc:TaxableAmount currencyID=\"COP\">0</cbc:TaxableAmount>"
            //         + "<cbc:TaxAmount currencyID=\"COP\">0</cbc:TaxAmount>"
            //         + "<cac:TaxCategory>"
            //            + "<cbc:Percent>0</cbc:Percent>"
            //            + "<cac:TaxScheme>"
            //                + "<cbc:ID>04</cbc:ID>"
            //                + "<cbc:Name>Impuesto Nacional al Consumo</cbc:Name>"
            //            + "</cac:TaxScheme>"
            //         + "</cac:TaxCategory>"
            //    + "</cac:TaxSubtotal>"
            ////+ "</cac:TaxTotal>"
            ////+ "<cac:TaxTotal>"
            ////    + "<cbc:TaxAmount currencyID=\"COP\">0</cbc:TaxAmount>"
            //    + "<cac:TaxSubtotal>"
            //        + "<cbc:TaxableAmount currencyID=\"COP\">0</cbc:TaxableAmount>"
            //        + "<cbc:TaxAmount currencyID=\"COP\">0</cbc:TaxAmount>"
            //        + "<cac:TaxCategory>"
            //            + "<cbc:Percent>0</cbc:Percent>"
            //            + "<cac:TaxScheme>"
            //                + "<cbc:ID>03</cbc:ID>"
            //                + "<cbc:Name>ICA</cbc:Name>"
            //            + "</cac:TaxScheme>"
            //         + "</cac:TaxCategory>"
            //    + "</cac:TaxSubtotal>"
           cadena += "</cac:TaxTotal>"
            + "<cac:LegalMonetaryTotal>"
                +"<cbc:LineExtensionAmount currencyID=\"COP\">"+ LineExtensionAmount + "</cbc:LineExtensionAmount>"
                + "<cbc:TaxExclusiveAmount currencyID=\"COP\">" + TaxExclusiveAmount + "</cbc:TaxExclusiveAmount>"
                + "<cbc:TaxInclusiveAmount currencyID=\"COP\" >" + TaxInclusiveAmount + "</cbc:TaxInclusiveAmount>"
                + "<cbc:AllowanceTotalAmount currencyID=\"COP\">"+ AllowanceTotalAmount + "</cbc:AllowanceTotalAmount>"
                +"<cbc:PayableAmount currencyID=\"COP\">"+ PayableAmount + "</cbc:PayableAmount>"
             +"</cac:LegalMonetaryTotal>";


            return cadena;
        }

        public static string formLinesXML(List<LineXML> lines)
        {
            string cadena = "";

            foreach(var line in lines)
            {
                var LineID = line.LineID;
                var InvoicedQuantity=line.InvoicedQuantity;
                var InvoiceLineExtensionAmount = line.InvoiceLineExtensionAmount;
                var LineTax = line.LineTax;
                var LineTotal = line.LineTotal;
                var LineTaxPercentage = line.LineTaxPercentage;
                var LineItemName = line.LineItemName;
                var LineItemTotal = line.LineItemTotal;


                cadena += "<cac:InvoiceLine>"
                + "<cbc:ID>" + LineID + "</cbc:ID>"
                + "<cbc:InvoicedQuantity unitCode=\"EA\">" + InvoicedQuantity + "</cbc:InvoicedQuantity>"
                + "<cbc:LineExtensionAmount currencyID=\"COP\">" + InvoiceLineExtensionAmount + "</cbc:LineExtensionAmount>"
                + "<cac:TaxTotal>"
                    + "<cbc:TaxAmount currencyID=\"COP\">" + LineTax + "</cbc:TaxAmount>"
                    + "<cac:TaxSubtotal>"
                        + "<cbc:TaxableAmount currencyID=\"COP\">" + LineTotal + "</cbc:TaxableAmount>"
                        + "<cbc:TaxAmount currencyID=\"COP\">" + LineTax + "</cbc:TaxAmount>"
                        + "<cac:TaxCategory>"
                            + "<cbc:Percent>" + LineTaxPercentage + "</cbc:Percent>"
                            + "<cac:TaxScheme>"
                                + "<cbc:ID>01</cbc:ID>"
                                + "<cbc:Name>IVA</cbc:Name>"
                            + "</cac:TaxScheme>"
                        + "</cac:TaxCategory>"
                    + "</cac:TaxSubtotal>"
                + "</cac:TaxTotal>"
                + "<cac:Item>"
                    + "<cbc:Description>" + LineItemName + "</cbc:Description>"
                + "</cac:Item>"
                + "<cac:Price>"
                    + "<cbc:PriceAmount currencyID=\"COP\">" + LineItemTotal + "</cbc:PriceAmount>"
                    + "<cbc:BaseQuantity unitCode=\"EA\">" + InvoicedQuantity + "</cbc:BaseQuantity>"
                + "</cac:Price>"
            + "</cac:InvoiceLine>";
            }

            return cadena+"</Invoice>";
        }

        public static string getSignature()
        {
            string cadena = "<ds:Signature xmlns:ds=\"h"+"+ttp://www.w3.org/2000/09/xmldsig#\" Id=\"Signature-56762736-634d-4f3a-8637-5d4b6be70f57\">"
                +"<ds:SignedInfo><ds:CanonicalizationMethod Algorithm=\"h"+"ttp://www.w3.org/TR/2001/REC-xml-c14n-20010315\" />"
                +"<ds:SignatureMethod Algorithm=\"h"+"ttp://www.w3.org/2001/04/xmldsig-more#rsa-sha256\" /><ds:Reference Id=\"Reference-b209507c-c5d2-4e4f-b7fa-085c7289c275\" URI=\"\">" +
                "<ds:Transforms><ds:Transform Algorithm=\"h"+"ttp://www.w3.org/2000/09/xmldsig#enveloped-signature\" /></ds:Transforms><ds:DigestMethod Algorithm=\"h"+"ttp://www.w3.org/2001/04/xmlenc#sha256\" />" +
                "<ds:DigestValue>epcnBvcDT84oqDV4nRaAWPCNmx/L0TZVBufHIGs6iBI=</ds:DigestValue></ds:Reference><ds:Reference Id=\"ReferenceKeyInfo\" URI=\"#Signature-56762736-634d-4f3a-8637-5d4b6be70f57-KeyInfo\">" +
                "<ds:DigestMethod Algorithm=\"h"+"ttp://www.w3.org/2001/04/xmlenc#sha256\" /><ds:DigestValue>2FMGm6iPJ/ag9kc/a4Us8OgX+wD2Nc+ntH0J0flGEyw=</ds:DigestValue></ds:Reference>" +
                "<ds:Reference Type=\"h"+"ttp://uri.etsi.org/01903#SignedProperties\" URI=\"#xmldsig-Signature-56762736-634d-4f3a-8637-5d4b6be70f57-signedprops\"><ds:DigestMethod Algorithm=\"h"+"ttp://www.w3.org/2001/04/xmlenc#sha256\" />" +
                "<ds:DigestValue>6NVTRVj7Xwx/JMLCdd6+AqUq2BI2dBEsR9AR8L0NWiE=</ds:DigestValue></ds:Reference></ds:SignedInfo><ds:SignatureValue Id=\"SignatureValue-56762736-634d-4f3a-8637-5d4b6be70f57\">M7OnlJ6Q+pbNM1dJ9QrN0FF36SjNdTGi40Tq" +
                "CAzkG+V2OVCqWp/RGvngYLyQXP5Tw6LJU8AQZ1xGmTBExXM4q7EDBK4NZM3UBaTrrjlrQuIodSu4sQrrqsmXngoXyDy1IIDBAi3C3J67+FF9yfYuhHVCa2SPtIrvI2ABhPLQDM9lVZtuTbOtmj5sxHhHayNe/L7WxWq6oUTkWei86D7Y9N8+W1vNrAtp558/R65O1cBzHmHF2otLM4pBUfssg3XK+o" +
                "RwJAvev6NAaPSC4lO9A4mbmc4RMrdYBQE/T4Wx1mFt0vO/CKQkCuD4c94Oextol+ez3ujLd5Ekcwk09IgOhg==</ds:SignatureValue><ds:KeyInfo Id=\"Signature-56762736-634d-4f3a-8637-5d4b6be70f57-KeyInfo\">" +
                "<ds:X509Data><ds:X509Certificate>MIIIcjCCBlqgAwIBAgIIRpFMZrxD47MwDQYJKoZIhvcNAQELBQAwgbYxIzAhBgkqhkiG9w0BCQEWFGluZm9AYW5kZXNzY2QuY29tLmNvMSYwJAYDVQQDEx1DQSBBTkRFUyBTQ0QgUy5BLiBDbGF" +
                "zZSBJSSB2MjEwMC4GA1UECxMnRGl2aXNpb24gZGUgY2VydGlmaWNhY2lvbiBlbnRpZGFkIGZpbmFsMRIwEAYDVQQKEwlBbmRlcyBTQ0QxFDASBgNVBAcTC0JvZ290YSBELkMuMQswCQYDVQQGEwJDTzAeFw0yMDA3MDgwMzQwMDBaFw0yMTA" +
                "3MDgwMzM5MDBaMIIBMTEVMBMGA1UECRMMQ0wgMTYgIDcgIDIyMSQwIgYJKoZIhvcNAQkBFhVyb2Jpbi0zNjVAaG90bWFpbC5jb20xKDAmBgNVBAMTH0FORFJFQSBDQVJPTElOQSBIRVJSRVJBIFNBTkNIRVoxETAPBgNVBAUTCDMzMzY5MzQ" +
                "4MTUwMwYDVQQMEyxFbWlzb3IgRmFjdHVyYSBFbGVjdHJvbmljYSAtIFBlcnNvbmEgTmF0dXJhbDE6MDgGA1UECxMxRW1pdGlkbyBwb3IgQW5kZXMgU0NEIEFjIDI2IDY5QyAwMyBUb3JyZSBCIE9mIDcwMTERMA8GA1UEChMIR2VyZW5jaWE" +
                "xEDAOBgNVBAcTB0lQSUFMRVMxEDAOBgNVBAgMB05BUknDkU8xCzAJBgNVBAYTAkNPMIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEA3vHWAmVRLlqXq5/n22qrMpCzC90r+VPEEuKDiiwlkHpMnyC+5QaEj9Dkh5v1bjyKFjgtjku" +
                "D+Jo4raDmTd7B6GnKWW7S/jeG+PUOdVlGYvVBhDk9Kz3pb3fUP0NxuFWV8t70zMi3z5nYFaE3vwyhEgFtwF2hq0QRR5w5EThnGvBcVzA6FyTKU31RDu5joy5Rrd/a45sr/Entnp+fbCVNulongONbP9Vz9yh2rpE6KqUp/mzGo9iuWdfrGT9" +
                "tk0RzIyWXqXjSiN9QIl+wa6GkJcrcVIkJT651Ec/p9VK6K4hVGpp9qe92kdtBWR2idqn/Ww/9e0+dHwW+FzFY7ubnDwIDAQABo4IDBDCCAwAwDAYDVR0TAQH/BAIwADAfBgNVHSMEGDAWgBQ6V1DQdxs+1ovq/5eZ1/+EAkgpDzA3BggrBgE" +
                "FBQcBAQQrMCkwJwYIKwYBBQUHMAGGG2h0dHA6Ly9vY3NwLmFuZGVzc2NkLmNvbS5jbzAgBgNVHREEGTAXgRVyb2Jpbi0zNjVAaG90bWFpbC5jb20wggHpBgNVHSAEggHgMIIB3DCCAdgGDSsGAQQBgfRIAQIGAQMwggHFMIIBfgYIKwYBBQU" +
                "HAgIwggFwHoIBbABMAGEAIAB1AHQAaQBsAGkAegBhAGMAaQDzAG4AIABkAGUAIABlAHMAdABlACAAYwBlAHIAdABpAGYAaQBjAGEAZABvACAAZQBzAHQA4QAgAHMAdQBqAGUAdABhACAAYQAgAGwAYQBzACAAUABvAGwA7QB0AGkAYwBhAHM" +
                "AIABkAGUAIABDAGUAcgB0AGkAZgBpAGMAYQBkAG8AIABkAGUAIABDAG8AbQB1AG4AaQBkAGEAZAAgAEEAYwBhAGQA6QBtAGkAYwBhACAAKABQAEMAKQAgAHkAIABEAGUAYwBsAGEAcgBhAGMAaQDzAG4AIABkAGUAIABQAHIA4QBjAHQAaQB" +
                "jAGEAcwAgAGQAZQAgAEMAZQByAHQAaQBmAGkAYwBhAGMAaQDzAG4AIAAoAEQAUABDACkAIABlAHMAdABhAGIAbABlAGMAaQBkAGEAcwAgAHAAbwByACAAQQBuAGQAZQBzACAAUwBDAEQwQQYIKwYBBQUHAgEWNWh0dHA6Ly93d3cuYW5kZXN" +
                "zY2QuY29tLmNvL2RvY3MvRFBDX0FuZGVzU0NEX1YzLjIucGRmMB0GA1UdJQQWMBQGCCsGAQUFBwMCBggrBgEFBQcDBDA5BgNVHR8EMjAwMC6gLKAqhihodHRwOi8vY3JsLmFuZGVzc2NkLmNvbS5jby9DbGFzZUlJdjIuY3JsMB0GA1UdDgQ" +
                "WBBR/pD3X/71xrw6NRNeNlT5PLRKGyTAOBgNVHQ8BAf8EBAMCBeAwDQYJKoZIhvcNAQELBQADggIBAC8waFipeCXEGgV3Qq3zbrPjBKZ/XC6Z5keNsdi7FXZgeyig752ON1e46wx2Z4Gcn142l8MXdaUDHXCU4b4QbD1+V82vKWG3sQzPn17" +
                "9qbIru1alSZjkNyQ4eZ4fpg3CNHkc9p81zDUdhWUwxRR5v+nmkOsMYg4lH0zetCluuklH9Hhjur27atVSdRiXb1P9lQp0SURdDgWsjKJxhsc/SaCK6+cQNrI+vd9LhUmjC83RN9mccDSacPOFX6VesKyb/nqkg9SqUGOtXhocXavZLe1Mq2i" +
                "0pshRgd+6Xt0N7B5m8Py6WayGAWKHXyuEdogsGQo7h8Ki9pBlfPfnb4Pf2faOEBUipWSn5gZAs32maQkRWFs737vFigcsyilGAB0XEvxabRlqBqU/S61Bk+T6lcQFg3y4phhzMrOi/Zt8BWJassBn4YgFy/rmowxZnYqN8BBHeehUYzCfz3W" +
                "+/T0khuw548b48KsLU/BpskXhOWwl6swcIkhRGsvsQZzHw+Lirj00RazBg0omyPyFFuHyW01+worSPQ4WAUf86MdLDWSuMysEwRkMLvwOQgRT0g4Dje9nzAkk/dB7PMbZfh194T16vKvEHGoWnb1mUH30wGjYc2I/AQYStYK5RNh4Wm1127x" +
                "SzDNWNxKRUr4iOHV/kQp6sdAlO9bDxKnHFAj5zL7T</ds:X509Certificate></ds:X509Data><ds:KeyValue><ds:RSAKeyValue><ds:Modulus>3vHWAmVRLlqXq5/n22qrMpCzC90r+VPEEuKDiiwlkHpMnyC+5QaEj9Dkh5v1bjy" +
                "KFjgtjkuD+Jo4raDmTd7B6GnKWW7S/jeG+PUOdVlGYvVBhDk9Kz3pb3fUP0NxuFWV8t70zMi3z5nYFaE3vwyhEgFtwF2hq0QRR5w5EThnGvBcVzA6FyTKU31RDu5joy5Rrd/a45sr/Entnp+fbCVNulongONbP9Vz9yh2rpE6KqUp/mzGo9i" +
                "uWdfrGT9tk0RzIyWXqXjSiN9QIl+wa6GkJcrcVIkJT651Ec/p9VK6K4hVGpp9qe92kdtBWR2idqn/Ww/9e0+dHwW+FzFY7ubnDw==</ds:Modulus><ds:Exponent>AQAB</ds:Exponent></ds:RSAKeyValue></ds:KeyValue>" +
                "</ds:KeyInfo><ds:Object Id=\"XadesObjectId-e710ab52-1837-4f97-874c-b11c48f1b548\"><xades:QualifyingProperties xmlns:xades=\"h"+"ttp://uri.etsi.org/01903/v1.3.2#\" Id=\"QualifyingProperties-5b548868-2adb-49de-86a1-fa64a5a93f9a\" Target=\"#Signature-56762736-634d-4f3a-8637-5d4b6be70f57\">" +
                "<xades:SignedProperties Id=\"xmldsig-Signature-56762736-634d-4f3a-8637-5d4b6be70f57-signedprops\"><xades:SignedSignatureProperties><xades:SigningTime>2021-01-30T16:37:06+01:00</xades:SigningTime>" +
                "<xades:SigningCertificate><xades:Cert><xades:CertDigest><ds:DigestMethod Algorithm=\"h"+"ttp://www.w3.org/2001/04/xmlenc#sha256\" /><ds:DigestValue>v6LIqZxmwLiphF6tPJFBPaubVyivba6CBZYTNXrnkTY=</ds:DigestValue>" +
                "</xades:CertDigest><xades:IssuerSerial><ds:X509IssuerName>C=CO, L=Bogota D.C., O=Andes SCD, OU=Division de certificacion entidad final, CN=CA ANDES SCD S.A. Clase II v2, E=info@andesscd.com.co</ds:X509IssuerName>" +
                "<ds:X509SerialNumber>5084929458406941619</ds:X509SerialNumber></xades:IssuerSerial></xades:Cert></xades:SigningCertificate><xades:SignaturePolicyIdentifier><xades:SignaturePolicyId><xades:SigPolicyId>" +
                "<xades:Identifier>h"+"ttps://facturaelectronica.dian.gov.co/politicadefirma/v2/politicadefirmav2.pdf</xades:Identifier><xades:Description /></xades:SigPolicyId><xades:SigPolicyHash>" +
                "<ds:DigestMethod Algorithm=\"h"+"ttp://www.w3.org/2001/04/xmlenc#sha256\" /><ds:DigestValue>dMoMvtcG5aIzgYo0tIsSQeVJBDnUnfSOfBpxXrmor0Y=</ds:DigestValue></xades:SigPolicyHash>" +
                "</xades:SignaturePolicyId></xades:SignaturePolicyIdentifier><xades:SignerRole><xades:ClaimedRoles><xades:ClaimedRole>supplier</xades:ClaimedRole></xades:ClaimedRoles></xades:SignerRole>" +
                "</xades:SignedSignatureProperties><xades:SignedDataObjectProperties><xades:DataObjectFormat ObjectReference=\"#Reference-b209507c-c5d2-4e4f-b7fa-085c7289c275\"><xades:MimeType>text/xml</xades:MimeType>" +
                "<xades:Encoding>UTF-8</xades:Encoding></xades:DataObjectFormat></xades:SignedDataObjectProperties></xades:SignedProperties></xades:QualifyingProperties></ds:Object></ds:Signature>";

            
            return cadena;
        }
        #endregion


        #region estructura XML nota credito
        public static ExtensionDian GetExtensionDian(ParametrosConfiguracionFE configuracion)
        {
            using(var ctx = new AccountingContext())
            {
                var extensionDian = new ExtensionDian
                {
                    RangoNumeracion = new RangoNumeracion
                    {
                        NumeroResolucion = configuracion.RANGO_NUMERO_RESOLUCION,
                        FechaResolucion = configuracion.RANGO_FECHA_RESOLUCION,
                        Prefijo = configuracion.RANGO_PREFIJO,
                        Desde = configuracion.RANGO_DESDE,
                        Hasta = configuracion.RANGO_HASTA,
                        VigenciaDesde = configuracion.RANGO_VIGENCIA_DESDE,
                        VigenciaHasta = configuracion.RANGO_VIGENCIA_HASTA
                    },
                    PaisOrigen = Pais.COLOMBIA,
                    SoftwareProveedorNit = configuracion.EMISOR_NIT,
                    SoftwareIdentificador = configuracion.SOFTWARE_IDENTIFICADOR,
                    SoftwarePin = configuracion.SOFTWARE_PIN
                };
                return extensionDian;
            }
        }

        public static Emisor GetEmisor(ParametrosConfiguracionFE configuracion)
        {
            using(var ctx = new AccountingContext())
            {
                var emisor = new Emisor
                {
                    Nit = configuracion.EMISOR_NIT,
                    Nombre = configuracion.EMISOR_NOMBRE,
                    DireccionFisica = new DireccionFisica
                    { 
                        Departamento = new Departamento
                        { 
                            Codigo = configuracion.cityFK.departamentoFK.codigo,
                            Nombre = configuracion.cityFK.departamentoFK.Nom_dep
                        },
                        Municipio = new Municipio
                        {
                            Codigo = configuracion.cityFK.codigo,
                            Nombre = configuracion.cityFK.Nom_muni
                        },
                        Linea = configuracion.Direccion,
                        Pais = Pais.COLOMBIA
                    },
                    TipoContribuyente = "2",//2 persona natural
                    RegimenFiscal = "48",//responsable de IVA
                    ResponsabilidadFiscal = configuracion.ResponsabilidadFiscalFK.codigo,
                    PrefijoRangoNumeracion = configuracion.RANGO_PREFIJO,
                    MatriculaMercantil = configuracion.EMISOR_MATRICULA_MERCANTIL,
                    CorreoElectronico = configuracion.EMISOR_CORREO_ELECTRONICO,
                    NumeroTelefonico = configuracion.EMISOR_NUMERO_TELEFONICO

                };

                return emisor;
            }
        }

        public static Adquiriente GetAdquiriente(int IdAdquiriente)
        {
            using(var ctx = new AccountingContext())
            {
                var cliente = ctx.persons.Find(IdAdquiriente);

                var adquiriente = new Adquiriente
                {
                    TipoIdentificacion = cliente.TipoIdentificacionFK.codigo,
                    Identificacion = cliente.nit,
                    Nombre = cliente.name,
                    DireccionFisica = new DireccionFisica
                    {
                        Departamento = new Departamento
                        {
                            Codigo = cliente.municipioFK.departamentoFK.codigo,
                            Nombre = cliente.municipioFK.departamentoFK.Nom_dep,
                        },
                        Municipio = new Municipio
                        {
                            Codigo = cliente.municipioFK.codigo,
                            Nombre = cliente.municipioFK.Nom_muni
                        },
                        Linea = cliente.direccion,
                        Pais = new Pais
                        {
                            Codigo = cliente.municipioFK.departamentoFK.paisFK.Nomenclatura,
                            Nombre = cliente.municipioFK.departamentoFK.paisFK.Nom_pais,
                        }
                    },
                    TipoContribuyente = cliente.tipoContribuyente.ToString(),
                    RegimenFiscal = cliente.regimenFiscalFK.codigo,
                    ResponsabilidadFiscal = cliente.responsabilidadFiscal,
                    MatriculaMercantil = "00000-00",
                    CorreoElectronico = (cliente.email != null) ? cliente.email : "",
                    NumeroTelefonico = (cliente.celular != null) ? cliente.celular : ""
                };
                return adquiriente;
            }
        }

        public static List<Linea> GetLineas(List<operation> operaciones)
        {
            List<Linea> Lineas = new List<Linea>();
            decimal subtotal = 0, iva = 0, TotalSinIva=0;
            
            foreach(var item in operaciones)
            {
                if (item.products.ivaId == 1)
                {
                    subtotal = (item.price) / Convert.ToDecimal(1.19);
                    TotalSinIva = subtotal - item.discount;
                    iva = TotalSinIva * Decimal.Divide(19, 100);

                    var linea = new Linea
                    {
                        Cantidad = item.quantity,
                        Descripcion = item.products.barcode + " - " + item.products.name,
                        PrecioUnitario = Math.Round(subtotal, 2, MidpointRounding.ToEven),
                        CostoTotal = Math.Round(TotalSinIva*item.quantity, 2, MidpointRounding.ToEven),
                        Impuestos =
                        {
                            new Impuesto
                            {
                                Tipo = TipoImpuesto.IVA,
                                Porcentaje = item.products.ivaFK.value,
                                Importe = Math.Round(iva, 2, MidpointRounding.ToEven),
                                BaseImponible = Math.Round(TotalSinIva, 2, MidpointRounding.ToEven),
                            }
                        }
                    };

                    Lineas.Add(linea);

                }
                else if (item.products.ivaId == 2)
                {
                    subtotal = (item.price) / Convert.ToDecimal(1.05);
                    TotalSinIva = subtotal - item.discount;
                    iva = TotalSinIva * Decimal.Divide(5, 100);

                    var linea = new Linea
                    {
                        Cantidad = item.quantity,
                        Descripcion = item.products.barcode + " - " + item.products.name,
                        PrecioUnitario = Math.Round(subtotal, 2, MidpointRounding.ToEven),
                        CostoTotal = Math.Round(TotalSinIva*item.quantity, 2, MidpointRounding.ToEven),
                        Impuestos =
                        {
                            new Impuesto
                            {
                                Tipo = TipoImpuesto.IVA,
                                Porcentaje = item.products.ivaFK.value,
                                Importe = Math.Round(iva, 2, MidpointRounding.ToEven),
                                BaseImponible = Math.Round(TotalSinIva, 2, MidpointRounding.ToEven),
                            }
                        }
                    };
                    Lineas.Add(linea);
                }
                else {
                    var linea = new Linea
                    {
                        Cantidad = item.quantity,
                        Descripcion = item.products.barcode + " - " + item.products.name,
                        PrecioUnitario = Math.Round(item.price, 2, MidpointRounding.ToEven),
                        CostoTotal = Math.Round(item.price, 2, MidpointRounding.ToEven)
                    };
                    Lineas.Add(linea);
                }
                
            }

            return Lineas;
        }

        public static List<ResumenImpuesto> GetImpuestos(List<operation> operaciones)
        {
            List<ResumenImpuesto> Impuestos = new List<ResumenImpuesto>();

            var ListIva19 = operaciones.Where(x => x.products.ivaId == 1).ToList();
            var ListIva5 = operaciones.Where(x => x.products.ivaId == 2).ToList();
            decimal TotalSubTotal = 0,TotalIva=0,TotalSubTotalSinIva = 0;//acumuladores
            decimal subtotal = 0, iva = 0, SubTotalSinIva = 0;

            if (ListIva19.Count>0)
            {
                foreach(var item in ListIva19)
                {
                    subtotal = (item.price * item.quantity) / Convert.ToDecimal(1.19);
                    SubTotalSinIva = subtotal - (item.discount*item.quantity);
                    iva = SubTotalSinIva * Decimal.Divide(19, 100);

                    TotalSubTotal += subtotal;
                    TotalSubTotalSinIva += SubTotalSinIva;
                    TotalIva += iva;

                }
                var impuesto = new ResumenImpuesto
                {
                    Tipo = TipoImpuesto.IVA,
                    Importe = Math.Round(TotalIva, 2, MidpointRounding.ToEven),
                    Detalles =
                    {
                        new Impuesto
                        {
                            Porcentaje = 19M,
                            Importe = Math.Round(TotalIva, 2, MidpointRounding.ToEven),
                            BaseImponible = Math.Round(TotalSubTotalSinIva, 2, MidpointRounding.ToEven),
                        }
                    }
                };

                Impuestos.Add(impuesto);

                TotalSubTotal =0;
                TotalSubTotalSinIva =0;
                TotalIva=0;
            }
            if (ListIva5.Count > 0)
            {
                foreach (var item in ListIva5)
                {
                    subtotal = (item.price * item.quantity) / Convert.ToDecimal(1.05);
                    SubTotalSinIva = subtotal - (item.discount * item.quantity);
                    iva = SubTotalSinIva * Decimal.Divide(5, 100);

                    TotalSubTotal += subtotal;
                    TotalSubTotalSinIva += SubTotalSinIva;
                    TotalIva += iva;

                }
                var impuesto = new ResumenImpuesto
                {
                    Tipo = TipoImpuesto.IVA,
                    Importe = Math.Round(TotalIva, 2, MidpointRounding.ToEven),
                    Detalles =
                    {
                        new Impuesto
                        {
                            Porcentaje = 5M,
                            Importe = Math.Round(TotalIva, 2, MidpointRounding.ToEven),
                            BaseImponible = Math.Round(TotalSubTotalSinIva, 2, MidpointRounding.ToEven),
                        }
                    }
                };

                Impuestos.Add(impuesto);

                TotalSubTotal = 0;
                TotalSubTotalSinIva = 0;
                TotalIva = 0;
            }

            //var impuesto2 = new ResumenImpuesto
            //{
            //    Tipo = TipoImpuesto.IC,
            //    Importe = 0M
            //};

            //var impuesto3 = new ResumenImpuesto
            //{
            //    Tipo = TipoImpuesto.ICA,
            //    Importe = 0M
            //};
            //Impuestos.Add(impuesto2);
            //Impuestos.Add(impuesto3);

            return Impuestos;
        }

        public static DetallePago GetDetallePago(InvoiceFacturaElectronica InvoiceFE)
        {
            var detallePago = new DetallePago
            {
                Forma = FormaPago.CONTADO,
                Metodo = MetodoPago.EFECTIVO,
                Fecha = InvoiceFE.Fecha
            };
            return detallePago;
        }

        public static Totales GetTotales(List<Linea> lineas, List<ResumenImpuesto> impuestos)
        {
            decimal ValorBruto = lineas.Select(x => x.CostoTotal).Sum();
            decimal BaseImponible = lineas.Select(x => x.CostoTotal).Sum();
            decimal Impuestos = impuestos.Select(x => x.Importe).Sum();
            decimal ValorBrutoConImpuestos = ValorBruto + Impuestos;
            decimal Descuentos = 0, Cargos = 0;
            var totales = new Totales
            {
                ValorBruto = ValorBruto,
                BaseImponible = BaseImponible,
                ValorBrutoConImpuestos = ValorBrutoConImpuestos,
                Descuentos = 0,
                Cargos = 0,
                TotalPagable = ValorBrutoConImpuestos - Descuentos + Cargos
            };
            return totales;
        }

        public static ReferenciaDocumento GetReferencia(InvoiceFacturaElectronica InfoFE)
        {
            var referencia = new ReferenciaDocumento
            {
                Numero = InfoFE.Numero.ToString(),
                Cufe = InfoFE.Cufe,
                AlgoritmoCufe = new AlgoritmoSeguridadUUID { Codigo = InfoFE.AlgoritmoCufe},
                TipoDocumento = new TipoDocumento { Codigo = InfoFE.TipoDocumento},
                Fecha = InfoFE.Fecha
            };
            return referencia;
        }

        #endregion

        //este método verifica si el string que conforma el XML es correcto
        public static string ValidateXML(string doc)
        {
            var schemas = new XmlSchemaSet();

            try
            {
                //string xsdPath = @"D:\DIAN_UBL_Structures.xsd";
                //XmlReaderSettings settings = new XmlReaderSettings();
                //settings.Schemas.Add(null, xsdPath);
                //settings.ValidationType = ValidationType.Schema;
                XmlDocument xml = new XmlDocument();
                xml.LoadXml(doc);
                //XmlReader xmlReader = XmlReader.Create(new StringReader(xml.InnerXml), settings);
                //while (xmlReader.Read()) { }
                return "";
            }
            catch (Exception e)
            {
                return e.Message;
            }

        }


        public static byte[] GetXmlByte(XmlDocument xml, string NombreArchivo)
        {
            string path1 = System.AppDomain.CurrentDomain.BaseDirectory + "/TempData/"+NombreArchivo+".xml";
            xml.PreserveWhitespace = true;
            xml.Save(path1);
            byte[] XmlByte = File.ReadAllBytes(path1);
            
            return XmlByte;
        }

        public static bool SendCompressUser(int IdUser,byte[] document,string NombreDocumento)
        {
            
            Dictionary<string, byte[]> ZipDocumentos = new Dictionary<string, byte[]>();
            
            ZipDocumentos.Add(NombreDocumento+".xml", document);
            //ZipDocumentos.Add(NombreDocumento+".pdf", PdfBytes);
            try
            {
                
                var ZipAttachedDocument = ZipHelper.Compress(ZipDocumentos);
                var SendEmail = SendEmailUser(ZipAttachedDocument, NombreDocumento,IdUser);
                string safd = "";
                
                return true;
            }
            catch (Exception ex)
            {
                return false;
                throw;
            }

        }

        public static bool SendEmailUser(byte[] DocumentZip,string NombreDocumento,int IdUser)
        {
            try
            {
                
                using(var ctx = new AccountingContext())
                {
                    
                    //consultamos el correo del usuario
                    var InfoUsuario = ctx.persons.Where(x => x.id == IdUser).FirstOrDefault();
                    if(InfoUsuario.email!=null && InfoUsuario.email!="")
                    {
                        
                        string path = System.AppDomain.CurrentDomain.BaseDirectory+"Certificado\\"+NombreDocumento+".pdf";
                        
                        MemoryStream ZipStreamxml = new MemoryStream(DocumentZip);
                        Attachment ZipAttachment = new Attachment(ZipStreamxml, NombreDocumento + ".zip");
                        Attachment ZipAttachment2 = new Attachment(path);

                        MailMessage correo = new MailMessage();
                        correo.From = new MailAddress("bolivarserranoq@gmail.com");
                        correo.To.Add(InfoUsuario.email);
                        correo.Subject = "Factura Electrónica";
                        correo.Body = "";
                        correo.IsBodyHtml = true;
                        correo.Priority = MailPriority.Normal;
                        

                        //configuración del servidor smtp

                        SmtpClient smtp = new SmtpClient("domain-com.mail.protection.outlook.com");
                        smtp.Host = "smtp.gmail.com";//"smtp.office365.com";
                        smtp.Port = 587;
                        smtp.EnableSsl = true;
                        smtp.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
                        smtp.Timeout = 100000;//
                        smtp.DeliveryMethod = SmtpDeliveryMethod.Network;//
                        smtp.UseDefaultCredentials = false;
                        string cuentaCorreo = "bolivarserranoq@gmail.com";
                        string passwordCorreo = "130152776";
                        smtp.Credentials = new System.Net.NetworkCredential(cuentaCorreo, passwordCorreo);

                        correo.BodyEncoding = UTF8Encoding.UTF8;//
                        correo.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;//
                        correo.Attachments.Add(ZipAttachment);
                        correo.Attachments.Add(ZipAttachment2);
                        smtp.Send(correo);

                        return true;
                    }
                    else
                    {
                        return false;
                    }

                    
                }

            }
            catch (Exception ex)
            {
                return false;
                throw;
            }
            
            
        }

        

        public static string ToXmlString(this XmlDocument document, bool omitXmlDeclaration = false)
        {
            using (var sw = new StringWriter())
            {
                var settings = new XmlWriterSettings
                {
                    NewLineChars = "\r\n",
                    NewLineHandling = NewLineHandling.Replace,
                    Indent = true,
                    OmitXmlDeclaration = omitXmlDeclaration
                };

                using (var xw = XmlWriter.Create(sw, settings))
                {
                    document.WriteTo(xw);

                    xw.Flush();
                    sw.Flush();

                    return sw.ToString();
                }
            }
        }


        public static byte[] GetImageQR(string QR)
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(QR, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);
            System.Web.UI.WebControls.Image imgQRCode = new System.Web.UI.WebControls.Image();
            imgQRCode.Height = 50;
            imgQRCode.Width = 50;
            using (Bitmap bitMap = qrCode.GetGraphic(15))
            {
                using(MemoryStream ms = new MemoryStream())
                {
                    bitMap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    var QRByte = ms.ToArray();
                    //imgQRCode.ImageUrl = "data:image/png;base64," + Convert.ToBase64String(QRByte);
                    return QRByte;
                }
                //PHQRCode.Controls.Add(imgQRCode);
            }
            
        }

        
        

    }
}