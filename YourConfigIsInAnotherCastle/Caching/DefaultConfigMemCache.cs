using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using YourConfigIsInAnotherCastle.Models;

namespace YourConfigIsInAnotherCastle.Caching
{
    public class DefaultConfigMemCache : IConfigCache
    {
        private MemoryCache _cache;
        public DefaultConfigMemCache()
        {
            _cache = new MemoryCache(typeof(DefaultConfigMemCache).FullName);
        }

        public object Cache(string sectionName, CacheInformation configurationValue, Action<string, CacheInformation> cacheExpired)
        {
            if (configurationValue == null || configurationValue.Value == null) return null;

            _cache.Add(sectionName, configurationValue.Value, new CacheItemPolicy()
            {
                AbsoluteExpiration = new DateTimeOffset(DateTime.Now + configurationValue.Duration),
                RemovedCallback = new CacheEntryRemovedCallback((a) =>
                {
                    if (cacheExpired != null) cacheExpired(sectionName, configurationValue);
                })

            });
            return configurationValue.Value;
        }

        public void Clear(string sectionName = null)
        {
            if (string.IsNullOrWhiteSpace(sectionName))
            {
                foreach (var record in this._cache.ToArray())
                {
                    this._cache.Remove(record.Key);
                }
            }
            else
            {
                this._cache.Remove(sectionName);
            }
        }

        public object Get(string sectionName)
        {
            return _cache.Get(sectionName);

        }

        public object GetCache(string sectionName, Func<CacheInformation> retrievalFunction, Action<string, CacheInformation> cacheExpired)
        {
            var results = Get(sectionName);
            if (results == null)
            {
                results = Cache(sectionName, retrievalFunction(), cacheExpired);
            }
            return results;
        }
    }
}
