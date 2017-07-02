using System;
using YourConfigIsInAnotherCastle.Caching;

namespace YourConfigIsInAnotherCastle.Test.Mocks
{
    public partial class MockProvider : IInAnotherCastleServiceProvider
    {
        public class MockCache : IConfigCache
        {
            public object Value { get; set; }
            public bool ExecuteCache { get; set; }
            public object Cache(string sectionName, CacheInformation configurationValue, Action<string, CacheInformation> cacheExpired)
            {
                return Value;
            }

            public void Clear(string sectionName = null)
            {
                Value = null;
            }

            public object Get(string sectionName)
            {
                return Value;
            }

            public object GetCache(string sectionName, Func<CacheInformation> retrievalFunction, Action<string, CacheInformation> cacheExpired)
            {
                if (ExecuteCache) return retrievalFunction().Value;
                return Value;
            }
        }
    }
}
