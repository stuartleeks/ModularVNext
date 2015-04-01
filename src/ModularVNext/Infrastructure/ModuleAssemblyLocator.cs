using System;
using System.Collections.Generic;
using System.Reflection;

namespace ModularVNext.Infrastructure
{
    public class ModuleAssemblyLocator
    {
        public ModuleAssemblyLocator(IEnumerable<Assembly> moduleAssemblies)
        {
            ModuleAssemblies = moduleAssemblies;
        }

        public IEnumerable<Assembly> ModuleAssemblies { get; private set; }
    }
}
