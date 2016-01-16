using System;
using System.Collections.Generic;

namespace AzureSupport.TheBall.CORE
{
    public static class LinqExt
    {
        public static IEnumerable<T> TSort<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>> dependencyRetriever, bool throwOnCycle = false)
        {
            var sorted = new List<T>();
            var visited = new HashSet<T>();

            foreach (var item in source)
                Visit(item, visited, sorted, dependencyRetriever, throwOnCycle);

            return sorted;
        }

        private static void Visit<T>(T item, HashSet<T> visited, List<T> sorted, Func<T, IEnumerable<T>> dependencyRetriever, bool throwOnCycle)
        {
            if (!visited.Contains(item))
            {
                visited.Add(item);

                if (dependencyRetriever != null)
                {
                    var dependencies = dependencyRetriever(item);
                    if (dependencies != null)
                    {
                        foreach (var dep in dependencies)
                            Visit(dep, visited, sorted, dependencyRetriever, throwOnCycle);
                    }
                }

                sorted.Add(item);
            }
            else
            {
                if (throwOnCycle && !sorted.Contains(item))
                    throw new Exception("Cyclic dependency found");
            }
        }
    }
}