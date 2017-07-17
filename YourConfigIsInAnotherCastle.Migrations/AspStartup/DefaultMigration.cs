[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(YourConfigIsInAnotherCastle.Migrations.AspStartup.DefaultMigration), "MigrateUseConfigure")]


namespace YourConfigIsInAnotherCastle.Migrations.AspStartup
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;
    public class DefaultMigration
    {

        public const string ConfigurationSectionName = "InAnotherCastleMigrationSettings";

        private class MigrationLocationRequestor : ILocationRequestor
        {
            public string DestinationFolder { get; set; }
            public string ConfigurationFilePath { get; set; }
            public string ConnectionString { get; set; }

            public bool UseWebConfiguration { get; set; }

            public string GetConfigurationFilePath()
            {
                return ConfigurationFilePath;
            }

            public string GetConnectionString()
            {
                return ConnectionString;
            }

            public string GetDestinationFolder()
            {
                return DestinationFolder;
            }
        }
        public static void MigrateUseConfigure()
        {
            Migrate(ConfigurationManager.GetSection(ConfigurationSectionName) as IInAnotherCastleMigrationSettings);
        }
        public static void Migrate(IInAnotherCastleMigrationSettings settings)
        {
            try
            {
                if (settings != null)
                {
                    var locationRequestor = new MigrationLocationRequestor()
                    {
                        ConfigurationFilePath = Assembly.GetExecutingAssembly().Location,
                        ConnectionString = settings.ConnectionString,
                        DestinationFolder = settings.DestinationFolder,
                        UseWebConfiguration = settings.IsRunningFromWebApplication
                    };
                    SectionMigrator migrator = new SectionMigrator(locationRequestor);
                    SectionExporter exporter = new SectionExporter(locationRequestor);
                    if (migrator.Initialize() && exporter.Initialize())
                    {

                        //TODO include System Name, and tags
                        var migratedSections = migrator.Migrate(settings.Select(s => s.SectionName)).ToArray();
                        exporter.SaveConfigurationFile(migratedSections);
                        exporter.SaveToDataStore(migratedSections);

                    }
                    else
                    {
                        throw new MigrationException("Migration tools could not be initialized, please confirm your paths are configured.", null);
                    }

                }
            }
            catch (Exception exc)
            {
                throw new MigrationException("Error while migrating the current configuration file to the configured locations", exc);
            }
        }
    }
}
