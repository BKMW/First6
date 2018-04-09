using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(First6.Startup))]
namespace First6
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
