using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using YourConfigIsInAnotherCastle.Models;
using YourConfigIsInAnotherCastle.Storage.Implementations.EF.Maps;
using System.Data.Entity.Infrastructure;

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

    //TODO add EF as an extention project
    public partial class InAnotherCastleContext : DbContext, IInAnotherCastleContext
    {
        public InAnotherCastleContext(string connectionString) : base(connectionString)
        {

            this.Configuration.AutoDetectChangesEnabled = false;
        }

        public virtual IDbSet<ConfigurationValue> ConfigurationValues { get; set; }
        public virtual IDbSet<Tag> Tags { get; set; }

        public void AddLogging(Action<string> log)
        {
            this.Database.Log += log;
        }

        public void SetAdded<T>(T entity) where T : class
        {
            this.Entry(entity).State = EntityState.Added;
        }

        public void SetDeleted<T>(T entity) where T : class
        {
            this.Entry(entity).State = EntityState.Deleted;
        }

        public void SetModified<T>(T entity) where T : class
        {
            this.Entry(entity).State = EntityState.Modified;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new ConfigurationValueMap());
            modelBuilder.Configurations.Add(new TagMap());

            base.OnModelCreating(modelBuilder);
        }
    }
}
