using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.FileProviders;
using Microsoft.Framework.Expiration.Interfaces;

namespace ModularVNext.Infrastructure
{
    public class CompositeFileProvider : IFileProvider
    {
        private List<IFileProvider> _fileProviders;

        public CompositeFileProvider(IEnumerable<IFileProvider> fileProviders)
        {
            _fileProviders = fileProviders.ToList();
        }

        public IDirectoryContents GetDirectoryContents(string subpath)
        {
            foreach (var fileProvider in _fileProviders)
            {
                var contents = fileProvider.GetDirectoryContents(subpath);
                if (contents != null && contents.Exists)
                {
                    return contents;
                }
            }
            return new NotFoundDirectoryContents();
        }

        public IFileInfo GetFileInfo(string subpath)
        {
            foreach (var fileProvider in _fileProviders)
            {
                var fileInfo = fileProvider.GetFileInfo(subpath);
                if (fileInfo != null && fileInfo.Exists)
                {
                    return fileInfo;
                }
            }
            return new NotFoundFileInfo(subpath);
        }

        public IExpirationTrigger Watch(string filter)
        {
            // TODO - need a composite expiration trigger!
            throw new NotImplementedException();
        }
    }
}