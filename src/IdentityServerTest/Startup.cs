using IdentityServer3.Core.Configuration;
using IdentityServerTest.Configuration;
using Microsoft.AspNet.Builder;
using Microsoft.Framework.DependencyInjection;

namespace IdentityServerTest
{
    public class Startup
    {
        // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDataProtection();
        }

        public void Configure(IApplicationBuilder app)
        {
            var idsrvOptions = new IdentityServerOptions
            {
                Factory = new IdentityServerServiceFactory()
                    .UseInMemoryUsers(Users.Get())
                    .UseInMemoryClients(Clients.Get())
                    .UseInMemoryScopes(Scopes.Get()),

                RequireSsl = false
            };

            app.UseIdentityServer(idsrvOptions);
        }
    }
}