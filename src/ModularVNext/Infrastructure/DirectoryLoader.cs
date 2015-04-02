// Copyright(c) Microsoft Open Technologies, Inc.All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use
// these files except in compliance with the License.You may obtain a copy of the
// License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software distributed
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR
// CONDITIONS OF ANY KIND, either express or implied. See the License for the
// specific language governing permissions and limitations under the License.

using System.IO;
using System.Reflection;
using Microsoft.Framework.Runtime;

namespace ModularVNext.Infrastructure
{
    //
    // This class is based on code from https://raw.githubusercontent.com/aspnet/Entropy/
    //
    public class DirectoryLoader : IAssemblyLoader
    {
        private readonly IAssemblyLoadContext _context;
        private readonly string _path;

        public DirectoryLoader(string path, IAssemblyLoadContext context)
        {
            _path = path;
            _context = context;
        }

        public Assembly Load(string name)
        {
            return _context.LoadFile(Path.Combine(_path, name + ".dll"));
        }
    }
}