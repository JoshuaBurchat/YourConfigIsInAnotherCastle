﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YourConfigIsInAnotherCastle.Models;
using YourConfigIsInAnotherCastle.Storage.Implementations.EF;

namespace YourConfigIsInAnotherCastle.Test.Mocks
{
    public class MockInAnotherCastleContext : IInAnotherCastleContext
    {
        public MockInAnotherCastleContext()
        {
            ConfigurationValues = new FakeDbSet<ConfigurationValue>((v) => new object[] { v.Id }, null);
            Tags = new FakeDbSet<Tag>((v) => new object[] { v.Id }, null);
        }
        public List<Object> Added = new List<object>();
        public List<Object> Modified = new List<object>();
        public List<Object> Deleted = new List<object>();
        public IDbSet<ConfigurationValue> ConfigurationValues { get; set; }
        public MockInAnotherCastleContext AddConfigurationValues(params ConfigurationValue[] configurationValues)
        {
            return AddConfigurationValues(false, configurationValues);
        }
        public MockInAnotherCastleContext AddConfigurationValues(bool clearLocal, params ConfigurationValue[] configurationValues)
        {
            foreach (var record in configurationValues) ConfigurationValues.Add(record);
            if (clearLocal) ((FakeDbSet<ConfigurationValue>)ConfigurationValues).ClearLocal();
            return this;
        }

        public IDbSet<Tag> Tags { get; set; }
        public MockInAnotherCastleContext AddTags(params Tag[] tags)
        {
            foreach (var record in tags) Tags.Add(record);
            return this;
        }
     
        public void AddLogging(Action<string> log)
        {
            //TODO
        }
        public void Dispose()
        {
        }

        public Func<int> SaveChangesFunc { get; set; }
        public int SaveChanges()
        {
            if (SaveChangesFunc != null)
            {
                return SaveChangesFunc();
            }
            else
            {
                var modified = Modified.Count;
                var deleted = Deleted.Count;

                Modified.Clear();
                Deleted.Clear();
                return modified + deleted;
            }
        }

        public void SetDeleted<T>(T entity) where T : class
        {
            this.Deleted.Add(entity);
        }

        public void SetModified<T>(T entity) where T : class
        {
            this.Modified.Add(entity);
        }

        public void SetAdded<T>(T entity) where T : class
        {
            this.Added.Add(entity);
        }
    }
}
