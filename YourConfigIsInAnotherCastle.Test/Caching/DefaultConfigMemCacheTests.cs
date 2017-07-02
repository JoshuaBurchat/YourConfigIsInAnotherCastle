using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YourConfigIsInAnotherCastle.Caching;
using DeepEqual.Syntax;
using System.Threading;

namespace YourConfigIsInAnotherCastle.Test.Caching
{
    [TestClass]
    public class DefaultConfigMemCacheTests
    {

        private class CacheTest1
        {

        }
        private class CacheTest2
        {

        }

        [TestMethod]
        public void DefaultConfigMemCache_Get_ExistingCacheRetrieved()
        {
            var test1 = new CacheTest1();
            var test2 = new CacheTest2();
            DefaultConfigMemCache cache = new DefaultConfigMemCache();
            cache.Cache("Test1", new CacheInformation() { Value = test1, Duration = new TimeSpan(999, 0, 0) }, (a, b) => { });
            cache.Cache("Test2", new CacheInformation() { Value = test2, Duration = new TimeSpan(999, 0, 0) }, (a, b) => { });

            var results = cache.Get("Test1");

            Assert.AreEqual(test1, results, "Should be the same item in as out");
        }
        [TestMethod]
        public void DefaultConfigMemCache_Get_NoCacheExistsForSection_NullReturned()
        {
            DefaultConfigMemCache cache = new DefaultConfigMemCache();

            var results = cache.Get("Test1");

            Assert.IsNull(results, "Result should be null as nothing was cacheda");
        }
        [TestMethod]
        public void DefaultConfigMemCache_GetCache_ExistingCacheRetrieved()
        {
            var test1 = new CacheTest1();
            var test2 = new CacheTest2();
            DefaultConfigMemCache cache = new DefaultConfigMemCache();
            cache.Cache("Test1", new CacheInformation() { Value = test1, Duration = new TimeSpan(999, 0, 0) }, (a, b) => { });
            cache.Cache("Test2", new CacheInformation() { Value = test2, Duration = new TimeSpan(999, 0, 0) }, (a, b) => { });

            var results = cache.GetCache("Test1", () =>
            {
                Assert.Fail("Item should be present as it was cached previously.");
                return null;
            }, (a, b) => { });

            Assert.AreEqual(test1, results, "Should be the same item in as out");
        }
        [TestMethod]
        public void DefaultConfigMemCache_GetCache_NoCacheExistsForSection_DelegateCalled()
        {
            DefaultConfigMemCache cache = new DefaultConfigMemCache();

            bool called = false;
            var results = cache.GetCache("Test1", () =>
            {
                called = true;
                return new CacheInformation() { Value = new CacheTest1(), Duration = new TimeSpan(999, 0, 0) };
            }, (a, b) => { });

            Assert.IsTrue(called, "Func should have been called because cache didnt exist");
            Assert.IsNotNull(results, "Func should have been called because cache didnt exist, and this value should be returned.");
        }
        [TestMethod]
        public void DefaultConfigMemCache_ClearAllCache_AllRecordsRemoved()
        {
            var test1 = new CacheTest1();
            var test2 = new CacheTest2();
            DefaultConfigMemCache cache = new DefaultConfigMemCache();
            cache.Cache("Test1", new CacheInformation() { Value = test1, Duration = new TimeSpan(999, 0, 0) }, (a, b) => { });
            cache.Cache("Test2", new CacheInformation() { Value = test2, Duration = new TimeSpan(999, 0, 0) }, (a, b) => { });

            cache.Clear();

            Assert.IsNull(cache.Get("Test1"), "Test 1 shouldnot exist in cache due to clear");
            Assert.IsNull(cache.Get("Test2"), "Test 2 shouldnot exist in cache due to clear");
        }
        [TestMethod]
        public void DefaultConfigMemCache_ClearSingleCache_AllRecordsRemoved()
        {
            var test1 = new CacheTest1();
            var test2 = new CacheTest2();
            DefaultConfigMemCache cache = new DefaultConfigMemCache();
            cache.Cache("Test1", new CacheInformation() { Value = test1, Duration = new TimeSpan(999, 0, 0) }, (a, b) => { });
            cache.Cache("Test2", new CacheInformation() { Value = test2, Duration = new TimeSpan(999, 0, 0) }, (a, b) => { });

            cache.Clear("Test2");
            Assert.AreEqual(cache.Get("Test1"), test1, "Test 1 should exist in cache");
            Assert.IsNull(cache.Get("Test2"), "Test 2 shouldnot exist in cache due to clear");
        }
        //TODO find a way to test this, delays dont work..
        //[TestMethod]
        //public  void DefaultConfigMemCache_CacheDuration_Triggered_ValueUnCachedAndEventTriggered()
        //{
        //    var test1 = new CacheTest1();
        //    DefaultConfigMemCache cache = new DefaultConfigMemCache();
        //    bool expiryTriggered = false;
        //    cache.Cache("Test1", new CacheInformation() { Value = test1, Duration = new TimeSpan(-9999) }, (a, b) => {
        //        expiryTriggered = true;
        //    });
        //    Task.Delay(1000).Wait();


        //    Assert.IsTrue(expiryTriggered, "Expiry should have been triggered");
        //}
    }
}
