using System.Web.Mvc;

namespace PlanillajeColectivos.Areas.cashBack
{
    public class cashBackAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "cashBack";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "cashBack_default",
                "cashBack/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}