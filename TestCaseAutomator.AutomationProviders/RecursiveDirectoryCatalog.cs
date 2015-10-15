using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.IO;

namespace TestCaseAutomator.AutomationProviders
{
    /// <summary>
    /// A part catalog that includes subdirectories of a root directory in its search.
    /// </summary>
    public class RecursiveDirectoryCatalog : AggregateCatalog
    {
        /// <summary>
        /// Initializes a new <see cref="RecursiveDirectoryCatalog"/>.
        /// </summary>
        /// <param name="path">
        /// The root directory path. In addition to the root, subdirectories of this path are searched for parts.
        /// </param>
        public RecursiveDirectoryCatalog(string path)
        {
            foreach (var catalog in GetCatalogs(path))
                Catalogs.Add(catalog);
        }

        private static IEnumerable<ComposablePartCatalog> GetCatalogs(string root)
        {
            yield return new DirectoryCatalog(root);

            foreach (var directory in Directory.EnumerateDirectories(root))
                yield return new DirectoryCatalog(directory);
        } 
    }
}