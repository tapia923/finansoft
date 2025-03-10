using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanillajeColectivos.DTO.FacturacionElectronica
{
    public class ExtensionXML
    {
        public string InvoiceAuthorization { get; set; }
        public string cbcStartDate { get; set; }
        public string cbcEndDate { get; set; }
        public string stsPrefix { get; set; }
        public string stsFrom { get; set; }
        public string stsTo { get; set; }
        public string cacPartyIdentification { get; set; }
        public string stsSoftwareID { get; set; }
        public string stsSoftwareSecurityCode { get; set; }
        public string QRCode { get; set; }
        public string DigitalSignature { get; set; }
        
    }
}
