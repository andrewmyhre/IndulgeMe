using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(BlessTheWeb.MVC5.Startup))]
namespace BlessTheWeb.MVC5
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
