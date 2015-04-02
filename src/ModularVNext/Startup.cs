using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Diagnostics;
using Microsoft.AspNet.FileProviders;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Razor;
using Microsoft.AspNet.StaticFiles;
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
        private IFileProvider _modulesFileProvider;
        private readonly string ApplicationBasePath;
        private readonly IAssemblyLoadContextAccessor _assemblyLoadContextAccessor;
        private readonly IAssemblyLoaderContainer _assemblyLoaderContainer;

        public Startup(IHostingEnvironment hostingEnvironment,
            IApplicationEnvironment applicationEnvironment,
            IAssemblyLoaderContainer assemblyLoaderContainer,
            IAssemblyLoadContextAccessor assemblyLoadContextAccessor)
        {
            _assemblyLoadContextAccessor = assemblyLoadContextAccessor;
            _assemblyLoaderContainer = assemblyLoaderContainer;
            ApplicationBasePath = applicationEnvironment.ApplicationBasePath;
            Configuration = new Configuration()
                .AddJsonFile("config.json")
                .AddEnvironmentVariables();
        }

        public IConfiguration Configuration { get; set; }

        public void ConfigureServices(IServiceCollection services)
        {
            var basePaths = Configuration.Get("additionalFileProviderBasePaths")?.Split(';') ?? new string[] { };
            var modulesPath = Path.Combine(ApplicationBasePath, Configuration.Get("moduleLoadPath"));

            var moduleAssemblies = LoadAssembliesFrom(modulesPath, _assemblyLoaderContainer, _assemblyLoadContextAccessor);

            _modulesFileProvider = GetModulesFileProvider(basePaths, moduleAssemblies);


            services.AddInstance(Configuration);

            services.AddMvc();

            services.Configure<RazorViewEngineOptions>(o =>
            {
                o.FileProvider = _modulesFileProvider;
            });

            services.AddInstance(new ModuleAssemblyLocator(moduleAssemblies));
            services.AddTransient<DefaultAssemblyProvider>();
            services.AddTransient<IAssemblyProvider, ModuleAwareAssemblyProvider>();
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

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = _modulesFileProvider
            });

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

        private List<Assembly> LoadAssembliesFrom(string modulesDirectory,
            IAssemblyLoaderContainer assemblyLoaderContainer,
            IAssemblyLoadContextAccessor loadContextAccessor)
        {
            var assemblies = new List<Assembly>();
            var loadContext = _assemblyLoadContextAccessor.GetLoadContext(typeof(Startup).GetTypeInfo().Assembly);
            using (assemblyLoaderContainer.AddLoader(new DirectoryLoader(modulesDirectory, loadContext)))
            {
                foreach (var modulePath in Directory.EnumerateFiles(modulesDirectory, "*.dll"))
                {
                    var name = Path.GetFileNameWithoutExtension(modulePath);
                    assemblies.Add(loadContext.Load(name));
                }
            }
            return assemblies;
        }
        private IFileProvider GetModulesFileProvider(string[] basePaths, List<Assembly> moduleAssemblies)
        {
            // TODO - probably want to set this to be debug only as it allows serving content outside the root directory
            var redirectedFileProviders = basePaths
                .Select(path => Path.IsPathRooted(path) ? path : Path.Combine(ApplicationBasePath, path))
                .Select(root => new PhysicalFileProvider(root));

            var resourceFileProviders = moduleAssemblies.Select(a => new SafeEmbeddedFileProvider(a));

            IFileProvider rootProvider = new PhysicalFileProvider(ApplicationBasePath);
            return new CompositeFileProvider(
                    rootProvider
                        .Concat(redirectedFileProviders)
                        .Concat(resourceFileProviders)
                        );
        }
    }
}
