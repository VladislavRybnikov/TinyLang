using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace TinyLang.IDE.Core
{
    public static class Resources
    {
        private static Lazy<string> _iconFolder = new Lazy<string>(GetIconFolder);

        public static string IconFolder => _iconFolder.Value;

        private static string GetIconFolder()
        {
            string execName =
               Assembly.GetExecutingAssembly().GetModules()[0].FullyQualifiedName;

            string currentFolder = Path.GetDirectoryName(execName);

            string icons = Path.Combine(currentFolder, "resources", "img");

            return icons;
        }
    }
}
