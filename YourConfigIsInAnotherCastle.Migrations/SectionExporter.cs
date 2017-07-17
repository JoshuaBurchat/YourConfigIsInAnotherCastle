using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YourConfigIsInAnotherCastle.Logging;
using YourConfigIsInAnotherCastle.Migrations.Models;
using YourConfigIsInAnotherCastle.Models;
using YourConfigIsInAnotherCastle.Storage.Implementations.EF;

namespace YourConfigIsInAnotherCastle.Migrations
{
    /// <summary>
    /// Basic implementation of the configuration exporter. This class will save the information in the form of 
    /// a configuration file, and use the EF Data Store to store the database records given a database connection.
    /// (Either a local db, or SQL server attached db)
    /// </summary>
    public class SectionExporter : ISectionExporter
    {
        private ILocationRequestor _locationRequestor;
        public SectionExporter(ILocationRequestor locationRequestor)
        {
            _locationRequestor = locationRequestor;
        }
        private string _destinationPath = null;
        private string _connectionString = null;
        public bool Initialize()
        {
            try
            {
                this._destinationPath = _locationRequestor.GetDestinationFolder();

                this._connectionString = _locationRequestor.GetConnectionString();

                return !string.IsNullOrWhiteSpace(_destinationPath) && !string.IsNullOrWhiteSpace(_connectionString);
            }
            catch (Exception exc)
            {
                throw new MigrationException("Error while retrieving destination information", exc);
            }
        }

        private FileInfo SetupDestinationFile(DirectoryInfo directory)
        {
            FileInfo destinationFile = new FileInfo(directory.FullName + @"\migrated.config");
            if (destinationFile.Exists)
            {
                destinationFile.Delete();
            }
            using (StreamWriter writer = new StreamWriter(destinationFile.OpenWrite()))
            {
                writer.WriteLine(@"
                        <?xml version=""1.0"" encoding=""utf-8""?>
                        <configuration>
                        </configuration>
                ".Trim());
            }

            return destinationFile;
        }

        public void SaveConfigurationFile(IEnumerable<SectionMigrated> sections)
        {
            try
            {
                DirectoryInfo destination = new DirectoryInfo(_destinationPath);


                if (!destination.Exists) destination.Create();
                var destinationFile = SetupDestinationFile(destination);

                System.Configuration.Configuration configurationToModify = ConfigurationManager.OpenExeConfiguration(destinationFile.FullName);
                foreach (var section in sections)
                {
                    configurationToModify.Sections.Add(section.SectionName, new YourConfigIsInAnotherCastle.InAnotherCastleConfigSectionRedirect()
                    {
                        CacheDurationInMinutes = 60,
                        SystemName = null, // todo
                        Name = section.SectionName, //TODO allow change
                        Type = section.SectionTypeName,
                        Mode = Mode.Standard,
                    });
                    configurationToModify.Sections[section.SectionName].SectionInformation.Type = typeof(InAnotherCastleHandler).AssemblyQualifiedName;
                }
                configurationToModify.ConnectionStrings.ConnectionStrings.Clear();
                configurationToModify.ConnectionStrings.ConnectionStrings.Add(new ConnectionStringSettings()
                {
                    ConnectionString = this._connectionString,
                    Name = DefaultServiceProvider.ConnectionStringKey,
                    ProviderName = "System.Data.SqlClient"
                });

                configurationToModify.SaveAs(destinationFile.FullName, ConfigurationSaveMode.Full);
            }
            catch (IOException ioException)
            {
                throw new MigrationException("IO exception occurred while creating migrated configuration file", ioException);

            }
            catch (ConfigurationException configException)
            {
                throw new MigrationException("Configuration exception occurred while creating migrated configuration file", configException);
            }

        }

        public void SaveToDataStore(IEnumerable<SectionMigrated> sections)
        {
            try
            {
                using (ConfigurationStorage storage = new ConfigurationStorage(this._connectionString, new DefaultConsoleLogger()))
                {
                    Database.SetInitializer(new CreateDatabaseIfNotExists<InAnotherCastleContext>());

                    foreach (var section in sections)
                    {
                        ConfigurationSaveResults currentResults = null;
                        var addResults = currentResults = storage.AddConfigurationSection(new ConfigurationNew()
                        {
                            JSONSchema = section.JsonDetails.Schema,
                            Name = section.SectionName,
                            SystemName = null, //TODO,
                            XML = section.XmlDetails.RawData,
                            XMLSchema = section.XmlDetails.Schema
                        });
                        if (addResults.Successful)
                        {
                            if (addResults.Record.JSON != section.JsonDetails.RawData)
                            {
                                var updateResults = currentResults = storage.UpdateConfigurationSectionJson(new ConfigurationUpdate()
                                {
                                    Id = addResults.Record.Id,
                                    Value = section.JsonDetails.RawData
                                });
                            }
                        }


                        if (!currentResults.Successful)
                        {
                            throw new MigrationException(
                                "Validation issues occurred while put the migrated configuration into the data store: " +
                                String.Join("\n",
                                    currentResults.Errors.Select(e => e.Code.ToString() + ":" + e.Message)
                                )
                            , null);
                        }
                    }
                }
            }
            catch (SqlException sqlException)
            {
                throw new MigrationException("Sql exception occurred while put the migrated configuration into the data store", sqlException);
            }
        }
    }
}
