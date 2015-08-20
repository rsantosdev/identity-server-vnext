using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Authentication;
using Microsoft.AspNet.Authentication.OpenIdConnect;
using Microsoft.AspNet.Builder;
using Microsoft.Framework.DependencyInjection;

namespace MvcClient
{
    public class Startup
    {
        // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app)
        {
            JwtSecurityTokenHandler.InboundClaimTypeMap = new Dictionary<string, string>();

            app.UseCookieAuthentication(options =>
            {
                options.AuthenticationScheme = "Cookies";
                options.AutomaticAuthentication = true;
            });

            app.UseOpenIdConnectAuthentication(options =>
            {
                options.Authority = "http://localhost:60404";
                options.ClientId = "mvc6";
                options.ResponseType = "id_token token";
                options.Scope = "openid email profile";
                options.RedirectUri = "http://localhost:61751/";

                options.SignInScheme = "Cookies";
                options.AutomaticAuthentication = true;

                options.Notifications = new OpenIdConnectAuthenticationNotifications
                {
                    SecurityTokenValidated = n =>
                    {
                        var incoming = n.AuthenticationTicket.Principal;

                        // create application identity
                        var id = new ClaimsIdentity("application", "given_name", "role");
                        id.AddClaim(incoming.FindFirst("sub"));
                        id.AddClaim(incoming.FindFirst("email"));
                        id.AddClaim(incoming.FindFirst("email_verified"));
                        id.AddClaim(incoming.FindFirst("given_name"));
                        id.AddClaim(incoming.FindFirst("family_name"));
                        id.AddClaim(new Claim("token", n.ProtocolMessage.AccessToken));

                        n.AuthenticationTicket = new AuthenticationTicket(
                            new ClaimsPrincipal(id),
                            n.AuthenticationTicket.Properties,
                            n.AuthenticationTicket.AuthenticationScheme);

                        // this skips nonce checking & cleanup 
                        // see https://github.com/aspnet/Security/issues/372
                        n.HandleResponse();
                        return Task.FromResult(0);
                    }
                };
            });

            app.UseMvcWithDefaultRoute();
        }
    }
}