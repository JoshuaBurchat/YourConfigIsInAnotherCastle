using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YourConfigIsInAnotherCastle.Configuration;
using YourConfigIsInAnotherCastle.Models;

namespace YourConfigIsInAnotherCastle.Storage
{
    public interface IConfigurationStorage : IDisposable
    {


        IQueryable<ConfigurationValue> GetConfigurationSections(IEnumerable<int> tags = null);
        IQueryable<ConfigurationValue> GetConfigurationSections(string systemName);
        ConfigurationValue GetConfigurationSection(string name, string systemName = null);
        ConfigurationValue GetConfigurationSection(int id);

        ConfigurationSaveResults AddConfigurationSection(ConfigurationNew configuration);
        ConfigurationSaveResults UpdateConfigurationSection(int id, ConfigurationNew configuration);
        ConfigurationSaveResults RemoveConfigurationSection(int id);
        ConfigurationSaveResults UpdateConfigurationSectionJson(ConfigurationUpdate update);
        ConfigurationSaveResults UpdateConfigurationSectionXml(ConfigurationUpdate update);


        IQueryable<Tag> GetTags(bool includeInactive = false);

    }
}
