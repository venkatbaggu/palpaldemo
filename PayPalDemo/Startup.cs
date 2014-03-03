using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(PayPalDemo.Startup))]
namespace PayPalDemo
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
