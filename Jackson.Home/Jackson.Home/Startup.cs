using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Jackson.Home.Startup))]
namespace Jackson.Home
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
