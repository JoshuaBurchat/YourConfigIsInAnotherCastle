using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using YourConfigIsInAnotherCastle.Models;

namespace YourConfigIsInAnotherCastle.Storage.Implementations.EF
{
    public interface IInAnotherCastleContext : IDisposable
    {
        IDbSet<ConfigurationValue> ConfigurationValues { get; set; }
        IDbSet<Tag> Tags { get; set; }
        void AddLogging(Action<string> log);


        void SetAdded<T>(T entity) where T : class;
        void SetModified<T>(T entity) where T : class;
        void SetDeleted<T>(T entity) where T : class;


        int SaveChanges();
    }

}
