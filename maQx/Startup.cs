using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(maQx.Startup))]
namespace maQx
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
