using Microsoft.CSharp;
using Newtonsoft.Json;
using NJsonSchema;
using NJsonSchema.CodeGeneration.CSharp;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using YourConfigIsInAnotherCastle.Migrations.Models;

namespace YourConfigIsInAnotherCastle.Migrations
{
    /// <summary>
    /// This Section Migrator will transform Configuration Sections into the proper XML, XSD, JSON, and JSONSchema
    /// that can be used the the In Another Castle library.
    /// </summary>
    public class SectionMigrator : ISectionMigrator
    {
        System.Configuration.Configuration _currentConfiguration;
        private ILocationRequestor _locationRequestor;
        public SectionMigrator(ILocationRequestor locationRequestor)
        {
            this._locationRequestor = locationRequestor;
        }
        public List<string> GetSections()
        {

            try
            {
                if (_currentConfiguration == null)
                    throw new MigrationException("Initialize must be called before get sections is invoked, in order to load the proper configuration", null);

                Regex ignoreDefaults = new Regex("System.Configuration, Version=(.*) Culture=neutral, PublicKeyToken=(.*)");
                return _currentConfiguration.Sections.Cast<ConfigurationSection>()
                    .Where(c => !(ignoreDefaults.IsMatch(c.GetType().Assembly.FullName)))
                    .Select(c => c.SectionInformation.Name).ToList();
            }
            catch (ConfigurationException exc)
            {
                throw new MigrationException("There was an error while reading the configuration sections", exc);
            }
        }
        /// <summary>
        /// Pulls the XML out of a Configuration section, and then generates its XSD
        /// </summary>
        /// <param name="section"></param>
        /// <returns></returns>
        public XmlDetails GetXmlDetails(ConfigurationSection section)
        {
            try
            {

                var xml = section.SectionInformation.GetRawXml();
                if (string.IsNullOrWhiteSpace(xml)) return null;
                //Get schema
                XmlReader reader = XmlReader.Create(new StringReader(xml));
                XmlSchemaSet schemaSet = new XmlSchemaInference().InferSchema(reader);
                var schema = schemaSet.Schemas().Cast<XmlSchema>().First();
                StringBuilder xmlSchemaBuilder = new StringBuilder();
                StringWriter writer = new StringWriter(xmlSchemaBuilder);
                schema.Write(writer);

                return new XmlDetails()
                {
                    RawData = xml,
                    Schema = xmlSchemaBuilder.ToString()
                };
            }
            catch (Exception exc)
            {
                throw new MigrationException("There was an error while migrating the XML details", exc);
            }

        }
        /// <summary>
        /// Generates the JSON Details based on the XML,this is the direction that the migration is going
        /// As In Another Castle is focused on XML config files .Net 4.5 and lower.
        /// </summary>
        /// <param name="xmlDetails"></param>
        /// <returns></returns>
        public JsonDetails GetJsonDetails(XmlDetails xmlDetails)
        {
            try
            {
                string json = JsonConvert.SerializeXmlNode(xmlDetails.Document, Newtonsoft.Json.Formatting.Indented, false);
                var schema = JsonSchema4.FromData(json);
                //TODO, there might be away to map between the XSD and the JSON properties... and then determine which fields are required.
                //This is one thing I could not seem to achieve and might be more complicated than its worth. It might be easier just to
                //Add the attributes visually based on your situation.
                return new JsonDetails()
                {
                    Schema = schema.ToJson(),
                    RawData = json
                };
            }
            catch (Exception exc)
            {
                throw new MigrationException("There was an error while migrating the JSON details", exc);
            }
        }
        public SectionMigrated Migrate(string sectionName)
        {
            if (_currentConfiguration == null)
                throw new MigrationException("Initialize must be called before get Migrate is invoked, in order to load the proper configuration", null);

            var section = _currentConfiguration.Sections[sectionName];
            //TODO if null
            var xmlDetails = GetXmlDetails(section);
            JsonDetails jsonDetails = xmlDetails == null ? null : GetJsonDetails(xmlDetails);

            return new SectionMigrated()
            {
                JsonDetails = jsonDetails,
                XmlDetails = xmlDetails,
                SectionName = sectionName,
                SectionTypeName = section.SectionInformation.Type
                //TODO add error result property?
            };
        }
        public IEnumerable<SectionMigrated> Migrate(IEnumerable<string> sectionNames)
        {
            foreach (var sectionName in sectionNames)
            {
                var mirated = Migrate(sectionName);
                if (mirated.JsonDetails != null && mirated.XmlDetails != null)
                    yield return mirated;
            }
            yield break;
        }

        public string SourceName => _currentConfiguration != null ? _currentConfiguration.FilePath : string.Empty;

        public bool Initialize()
        {
            try
            {
                var configurationFilePath = this._locationRequestor.GetConfigurationFilePath();

                if (this._locationRequestor.UseWebConfiguration)
                {
                    _currentConfiguration =   System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~");
                }
                else
                {
                     _currentConfiguration = ConfigurationManager.OpenExeConfiguration(configurationFilePath);
                }
                configurationFilePath = _currentConfiguration.FilePath;



                return !string.IsNullOrWhiteSpace(configurationFilePath);
            }
            catch (Exception exc)
            {
                throw new MigrationException("There was an error while openning the specific configuration file", exc);
            }
        }

        public IEnumerable<SectionMigrated> MigrateAll()
        {
            return Migrate(GetSections());
        }
    }
}
