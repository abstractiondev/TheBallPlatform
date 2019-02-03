using System;
using System.Collections.Generic;

namespace WebCoreLayer.Controllers
{
    internal static class CacheSupport
    {
        private const double STATIC_CONTENT_MINUTES = 5.0;

        private static Dictionary<string, TimeSpan> CacheMaxAgeDict = new Dictionary<string, TimeSpan>()
        {
            // Static content
            { ".html", TimeSpan.FromMinutes(STATIC_CONTENT_MINUTES) },
            { ".htm", TimeSpan.FromMinutes(STATIC_CONTENT_MINUTES) },
            { ".css", TimeSpan.FromMinutes(STATIC_CONTENT_MINUTES) },
            { ".js", TimeSpan.FromMinutes(STATIC_CONTENT_MINUTES) },
            { ".jpg", TimeSpan.FromMinutes(STATIC_CONTENT_MINUTES) },
            { ".jpeg", TimeSpan.FromMinutes(STATIC_CONTENT_MINUTES) },
            { ".png", TimeSpan.FromMinutes(STATIC_CONTENT_MINUTES) },
            { ".gif", TimeSpan.FromMinutes(STATIC_CONTENT_MINUTES) },
            // Dynamic content will default to TimeSpan.Zero on lookup, so not listed here
        };



        public static TimeSpan GetExtensionBasedMaxAge(string extension)
        {
            var lookupKey = extension.ToLower();
            TimeSpan result;
            if (CacheMaxAgeDict.TryGetValue(lookupKey, out result))
                return result;
            return TimeSpan.Zero;
        }
    }
}