using System.IO;
using System.Reflection;
using Microsoft.Framework.Runtime;

namespace ModularVNext.Infrastructure
{
    public class DirectoryAssemblyLoader : IAssemblyLoader
    {
        private readonly IAssemblyLoadContext _loadContext;
        private readonly string _directory;

        public DirectoryAssemblyLoader(string directory, IAssemblyLoadContext loadContext)
        {
            _directory = directory;
            _loadContext = loadContext;
        }

        public Assembly Load(string assemblyName)
        {
            var assemblyFullPath = Path.Combine(_directory, assemblyName + ".dll");
            return _loadContext.LoadFile(assemblyFullPath);
        }
    }
}