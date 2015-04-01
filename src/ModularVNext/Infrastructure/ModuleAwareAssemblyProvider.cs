using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNet.Mvc;
using Microsoft.Framework.Runtime;
using ModularVNext.Extensions;

namespace ModularVNext.Infrastructure
{
    /// <summary>
    /// Provides the module assemblies back to MVC to search for controllers
    /// </summary>
    public class ModuleAwareAssemblyProvider : IAssemblyProvider
    {
        private readonly DefaultAssemblyProvider _defaultProvider;

        // TODO - update this post beta3: http://www.strathweb.com/2015/04/asp-net-mvc-6-discovers-controllers/
        private readonly IEnumerable<Assembly> _moduleAssemblies;

        public ModuleAwareAssemblyProvider(DefaultAssemblyProvider defaultProvider,
            ModuleAssemblyLocator moduleAssemblyProvider)
        {
            _defaultProvider = defaultProvider;
            _moduleAssemblies = moduleAssemblyProvider.ModuleAssemblies;
        }

        public IEnumerable<Assembly> CandidateAssemblies
        {
            get
            {
                return _defaultProvider.CandidateAssemblies.Concat(_moduleAssemblies);
            }
        }
    }
}