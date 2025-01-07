using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanillajeColectivos.DTO.FacturacionElectronica
{
    public class VersionXML
    {
        public string CustomizationID { get; set; }
        public string ProfileExecutionID { get; set; }
        public string ID { get; set; }
        public string UUID { get; set; }
        public string IssueDate { get; set; }
        public string IssueTime { get; set; }
        public string InvoiceTypeCode { get; set; }
        public string LineCountNumeric { get; set; }
        public string InvoicePeriodStartDate { get; set; }
        public string InvoicePeriodEndDate { get; set; }
    }
}
