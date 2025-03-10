using System.Web.Mvc;

namespace PlanillajeColectivos.Areas.FacturacionElectronica
{
    public class FacturacionElectronicaAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "FacturacionElectronica";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "FacturacionElectronica_default",
                "FacturacionElectronica/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}