using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Diagnostics;
using Microsoft.AspNet.FileProviders;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Mvc.Razor;
using Microsoft.Framework.ConfigurationModel;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.DependencyInjection.Fallback;
using Microsoft.Framework.Logging;
using Microsoft.Framework.Logging.Console;
using Microsoft.Framework.Runtime;
using ModularVNext.Extensions;
using ModularVNext.Infrastructure;

namespace ModularVNext
{
    public class Startup
    {
        private readonly string WebRoot;

        public Startup(IHostingEnvironment env)
        {
            WebRoot = env.WebRoot;
            Configuration = new Configuration()
                .AddJsonFile("config.json")
                .AddEnvironmentVariables();
        }

        public IConfiguration Configuration { get; set; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            // TODO - probably want to set this to be debug only as it allows serving content outside the root directory
            var basePaths = Configuration.Get("additionalFileProviderBasePaths")?.Split(';') ?? new string[] { };
            var redirectedFileProviders = basePaths
                .Select(path => Path.IsPathRooted(path) ? path : Path.Combine(WebRoot, path))
                .Select(root => new PhysicalFileProvider(root));

            // TODO - replace this with a discovery mechanism!
            // NOTE: Type.Assembly doesn't exist in coreCLR. Need to look into that! For now, not targeting CoreCLR
            var modulesAssemblies = new[]
            {
                typeof(Module1.Controllers.Module1Controller).Assembly,
                typeof(Module2.Controllers.WeatherController).Assembly
            };
            var resourceFileProviders = modulesAssemblies.Select(a => new EmbeddedFileProvider(a));


            services.Configure<RazorViewEngineOptions>(o =>
            {
                o.FileProvider = new CompositeFileProvider(
                    o.FileProvider
                        .Concat(redirectedFileProviders)
                        .Concat(resourceFileProviders)
                        );
            });

            services.AddInstance(Configuration);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerfactory)
        {
            loggerfactory.AddConsole();

            if (string.Equals(env.EnvironmentName, "Development", StringComparison.OrdinalIgnoreCase))
            {
                app.UseBrowserLink();
                app.UseErrorPage(ErrorPageOptions.ShowAll);
            }
            else
            {
                app.UseErrorHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "areaRoute",
                    template: "{area:exists}/{controller}/{action}",
                    defaults: new { controller = "Home", action = "Index" }
                    );
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action}/{id?}",
                    defaults: new { controller = "Home", action = "Index" });
            });
        }
    }
}
