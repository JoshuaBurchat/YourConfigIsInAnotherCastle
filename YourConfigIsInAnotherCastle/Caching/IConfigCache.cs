using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YourConfigIsInAnotherCastle.Models;

namespace YourConfigIsInAnotherCastle.Caching
{
    public class CacheInformation
    {
        public object Value { get; set; }
        public TimeSpan Duration { get; set; }
    }
    public interface IConfigCache
    {
        object Get(string sectionName);
        object Cache(string sectionName, CacheInformation configurationValue, Action<string, CacheInformation> cacheExpired);
        object GetCache(string sectionName, Func<CacheInformation> retrievalFunction, Action<string, CacheInformation> cacheExpired);

        void Clear(string sectionName = null);
    }
}
