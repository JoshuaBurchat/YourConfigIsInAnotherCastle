using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YourConfigIsInAnotherCastle.Migrations.AspStartup
{
    public interface IInAnotherCastleMigrationSettings : IEnumerable<IMigrationConfigurationSection>
    {
        string DestinationFolder { get; set; }
        string ConnectionString { get; set; }
        bool IsRunningFromWebApplication { get; set; }

    }
    public interface IMigrationConfigurationSection
    {
        string SectionName { get; set; }
        string SystemName { get; set; }

    }
    public class InAnotherCastleMigrationSettings : ConfigurationSection, IInAnotherCastleMigrationSettings
    {

        [ConfigurationProperty("DestinationFolder", IsRequired = true)]
        public string DestinationFolder
        {
            get { return this["DestinationFolder"].ToString(); }
            set { this["DestinationFolder"] = value.ToString(); }
        }

        [ConfigurationProperty("ConnectionString", IsRequired = true)]
        public string ConnectionString
        {
            get { return this["ConnectionString"].ToString(); }
            set { this["ConnectionString"] = value.ToString(); }
        }
        [ConfigurationProperty("IsRunningFromWebApplication", IsRequired = true)]
        public bool IsRunningFromWebApplication
        {
            get { return bool.Parse( this["IsRunningFromWebApplication"].ToString()); }
            set { this["IsRunningFromWebApplication"] = value.ToString(); }
        }
        


       [ConfigurationProperty("", IsRequired = true, IsDefaultCollection = true)]
        [ConfigurationCollection(typeof(MigrationConfigurationSectionCollection),
                                   AddItemName = "add",
                                   ClearItemsName = "clear",
                                   RemoveItemName = "remove")]
        public MigrationConfigurationSectionCollection SectionCollection
        {
            get { return (MigrationConfigurationSectionCollection)this[""]; }
            set { this[""] = value; }
        }

        public IEnumerator<IMigrationConfigurationSection> GetEnumerator()
        {
            return SectionCollection.Cast<MigrationConfigurationSection>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return SectionCollection.Cast<MigrationConfigurationSection>().GetEnumerator();
        }
    }
    public class MigrationConfigurationSectionCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new MigrationConfigurationSection();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((MigrationConfigurationSection)element).SectionName;
        }
    }

    public class MigrationConfigurationSection : ConfigurationElement, IMigrationConfigurationSection
    {

        [ConfigurationProperty("SectionName", IsRequired = true)]
        public string SectionName
        {
            get { return this["SectionName"].ToString(); }
            set { this["SectionName"] = value.ToString(); }
        }

        [ConfigurationProperty("SystemName")]
        public string SystemName
        {
            get { return this["SystemName"].ToString(); }
            set { this["SystemName"] = value.ToString(); }
        }
    }
}