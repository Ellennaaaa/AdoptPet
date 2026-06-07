using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(AdoptPets.Startup))]
namespace AdoptPets
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            
        }
    }
}
