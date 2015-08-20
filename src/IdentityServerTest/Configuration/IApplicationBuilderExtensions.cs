using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer3.Core.Configuration;
using Owin;

namespace Microsoft.AspNet.Builder
{
    using DataProtectionProviderDelegate = Func<string[], Tuple<Func<byte[], byte[]>, Func<byte[], byte[]>>>;
    using DataProtectionTuple = Tuple<Func<byte[], byte[]>, Func<byte[], byte[]>>;

    public static class IApplicationBuilderExtensions
    {
        public static void UseIdentityServer(this IApplicationBuilder app, IdentityServerOptions options)
        {
            app.UseOwin(action =>
            {
                action?.Invoke(next =>
                {
                    var builder = new Microsoft.Owin.Builder.AppBuilder();
                    var provider = app.ApplicationServices.GetService(typeof(DataProtection.IDataProtectionProvider)) as DataProtection.IDataProtectionProvider;
                    if (provider == null)
                    {
                        throw new Exception("Provider can not be null at UseIdentityServer extension method");
                    }

                    builder.Properties["security.DataProtectionProvider"] = new DataProtectionProviderDelegate(
                        purposes =>
                        {
                            var dataProtection = provider.CreateProtector(string.Join(",", purposes));
                            return new DataProtectionTuple(dataProtection.Protect, dataProtection.Unprotect);
                        });

                    builder.UseIdentityServer(options);

                    var appFunc = builder.Build(typeof(Func<IDictionary<string, object>, Task>)) as Func<IDictionary<string, object>, Task>;
                    return appFunc;
                });
            });
        }
    }
}