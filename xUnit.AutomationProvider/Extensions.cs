using System.Collections.Generic;

namespace xUnit.AutomationProvider
{
    static class Extensions
    {
        public static void ToSink<T>(this IEnumerable<T> source, ICollection<T> destination)
        {
            foreach (var item in source)
                destination.Add(item);
        }
    }
}