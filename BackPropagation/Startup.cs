using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(BackPropagation.Startup))]
namespace BackPropagation
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
