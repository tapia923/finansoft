using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(PlanillajeColectivos.Startup))]
namespace PlanillajeColectivos
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
