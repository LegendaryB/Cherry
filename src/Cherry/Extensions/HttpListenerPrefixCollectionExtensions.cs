using System.Net;

namespace Cherry.Extensions
{
    internal static class HttpListenerPrefixCollectionExtensions
    {
        internal static void AddRange(this HttpListenerPrefixCollection collection, IEnumerable<string> prefixes)
        {
            foreach (var prefix in prefixes)
                collection.Add(prefix);
        }
    }
}
