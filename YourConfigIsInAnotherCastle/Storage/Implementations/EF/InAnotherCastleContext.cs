using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using YourConfigIsInAnotherCastle.Models;
using YourConfigIsInAnotherCastle.Storage.Implementations.EF.Maps;
using System.Data.Entity.Infrastructure;
using System.Linq.Expressions;

namespace YourConfigIsInAnotherCastle.Storage.Implementations.EF
{

    //TODO add EF as an extention project
    public partial class InAnotherCastleContext : DbContext, IInAnotherCastleContext
    {

        static InAnotherCastleContext()
        {
            // Database.SetInitializer<InAnotherCastleContext>(null);
        }

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

        public void ClearChanges()
        {
            foreach (var entry in this.ChangeTracker.Entries())
            {
                switch (entry.State)
                {
                    case EntityState.Modified:
                    case EntityState.Deleted:
                        entry.State = EntityState.Modified; //Revert changes made to deleted entity.
                        entry.State = EntityState.Unchanged;
                        break;
                    case EntityState.Added:
                        entry.State = EntityState.Detached;
                        break;
                }
            }
        }
        public void SetAdded<T>(T entity) where T : class
        {
            this.Entry(entity).State = EntityState.Added;
        }

            //Note this is to ensure that the tags used are in the current context and are attached properly
        public void SetChangesToConfigurationTags(ConfigurationValue configurationValue, IEnumerable<Tag> added, IEnumerable<Tag> removed)
        {
            var adapter = (IObjectContextAdapter)this;
            foreach(var tag in added)
            {
                adapter.ObjectContext.ObjectStateManager.ChangeRelationshipState(configurationValue, tag, c => c.Tags, EntityState.Added);
                adapter.ObjectContext.ObjectStateManager.ChangeRelationshipState(tag, configurationValue, c => c.ConfigurationValues, EntityState.Added);
            }
            foreach (var tag in removed)
            {
                adapter.ObjectContext.ObjectStateManager.ChangeRelationshipState(configurationValue, tag, c => c.Tags, EntityState.Deleted);
                adapter.ObjectContext.ObjectStateManager.ChangeRelationshipState(tag, configurationValue, c => c.ConfigurationValues, EntityState.Deleted);
            }
         
        }
     
        public void SetDeleted<T>(T entity) where T : class
        {
            this.Entry(entity).State = EntityState.Deleted;
        }
        public void SetModified<T>(T entity) where T : class
        {
            this.Entry(entity).State = EntityState.Modified;
        }
        //ctx
        //.ObjectContext
        //.ObjectStateManager
        //.ChangeRelationshipState(
        //    a,
        //    b,
        //    getNavigationProperty,
        //    state
        //);


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new ConfigurationValueMap());
            modelBuilder.Configurations.Add(new TagMap());

            base.OnModelCreating(modelBuilder);
        }

    
    }
}
