using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using YourConfigIsInAnotherCastle.Logging;
using YourConfigIsInAnotherCastle.Models;

namespace YourConfigIsInAnotherCastle.Storage.Implementations.EF
{
    public class ConfigurationStorage : IConfigurationStorage, IDisposable
    {
        public IInAnotherCastleContext Context { get; set; }
        private ILogger _logger;


        public ConfigurationStorage(IInAnotherCastleContext context, ILogger logger)
        {
            _logger = logger;
            Context = context;
        }

        public ConfigurationStorage(string connectionString, ILogger logger)
        {
            _logger = logger;
            Context = new InAnotherCastleContext(connectionString);
            Context.AddLogging((m) => logger.LogTrace(m));
        }

        public IQueryable<Models.ConfigurationValue> GetConfigurationSections(IEnumerable<int> tags = null)
        {
            IQueryable<ConfigurationValue> records = this.Context.ConfigurationValues;
            if (tags != null && tags.Any())
            {
                records = records.Where(r => r.Tags.Any(t => tags.Contains(t.Id)));
            }
            return records;
        }
        public IQueryable<ConfigurationValue> GetConfigurationSections(string systemName)
        {

            if (!string.IsNullOrWhiteSpace(systemName))
            {
                return Context.ConfigurationValues.Where(c => c.SystemName == systemName);
            }
            else
            {
                return Context.ConfigurationValues.Where(c => (c.SystemName == null || c.SystemName.Trim() == string.Empty));
            }
        }
        public Models.ConfigurationValue GetConfigurationSection(int id)
        {
            var results = Context.ConfigurationValues.Find(id);
            return results;
        }


        public ConfigurationSaveResults AddConfigurationSection(ConfigurationNew newConfiguration)
        {
            var results = new ConfigurationSaveResults()
            {
            };
            if (string.IsNullOrWhiteSpace(newConfiguration.Name))
            {
                results.AddError("The Json does not match the schema", DataStorageError.MissingNameField);
            }
            else
            {
                var existingRecord = GetConfigurationSection(newConfiguration.Name, newConfiguration.SystemName);
                if (existingRecord == null)
                {

                    var record = new ConfigurationValue()
                    {
                        JSONSchema = newConfiguration.JSONSchema,
                        Name = newConfiguration.Name,
                        SystemName = newConfiguration.SystemName,
                        XML = newConfiguration.XML,
                        XMLSchema = newConfiguration.XMLSchema
                    };
                    var xmlSchema = GetSchema(record.XMLSchema);
                    if (xmlSchema != null)
                    {
                        XmlDocument document = null;
                        if (ValidateXml(record.XML, xmlSchema, out document))
                        {
                            this.AssignJsonArrayAttributes(document, document.ChildNodes);
                            string json = JsonConvert.SerializeXmlNode(document, Newtonsoft.Json.Formatting.Indented, false);
                            if (ValidateJson(json, record.JSONSchema))
                            {
                                record.JSON = json;
                                Context.ConfigurationValues.Add(record);
                                Context.SetAdded(record);
                                var changes = Context.SaveChanges();
                                if (changes == 0)
                                    results.AddError("No records were removed ensure the id is correct.", DataStorageError.RecordsUnchanged);
                                else
                                    results.Record = record;
                            }
                            else
                            {
                                results.AddError("The Json does not match the schema", DataStorageError.DoesNotMatchJsonSchema);
                            }
                        }
                        else
                        {
                            results.AddError("The Xml does not match the schema", DataStorageError.DoesNotMatchXMlSchema);
                        }
                    }
                    else
                    {
                        results.AddError("The Xml Schema is invalid", DataStorageError.InvalidXmlSchema);
                    }
                }
                else
                {
                    results.AddError("Name and System Name already exist", DataStorageError.AlreadyExists);
                }
            }
            return results;
        }


        public ConfigurationValue GetConfigurationSection(string name, string systemName = null)
        {
            if (!string.IsNullOrWhiteSpace(systemName))
            {
                return Context.ConfigurationValues.Where(c => c.Name == name && c.SystemName == systemName).FirstOrDefault();
            }
            else
            {
                //note treat null system and empty system the same
                return Context.ConfigurationValues.Where(c => c.Name == name && (c.SystemName == null || c.SystemName.Trim() == string.Empty)).FirstOrDefault();
            }
        }

        public ConfigurationSaveResults RemoveConfigurationSection(int id)
        {
            var results = new ConfigurationSaveResults();
            //If instance exists locally us it, or else this will cause an error.
            ConfigurationValue record = Context.ConfigurationValues.Local.FirstOrDefault(c => c.Id == id) ??
            new ConfigurationValue()
            {
                Id = id
            };

            Context.ConfigurationValues.Attach(record);
            Context.SetDeleted(record);
            var changes = Context.SaveChanges();
            if (changes == 0)
            {
                results.AddError("No records were removed ensure the id is correct.", DataStorageError.RecordsUnchanged);
            }
            else
                results.Record = record;

            return results;
        }

        private bool ValidateJson(string json, string schema)
        {
            try
            {
                var schemaCheck = JSchema.Parse(schema);
                IList<string> errors = null;
                var results = JObject.Parse(json).IsValid(schemaCheck, out errors);

                return results;
            }
            catch (Exception exc)
            {
                _logger.LogError("Invalid Json", exc);
            }
            return false;
        }
        private XmlSchema GetSchema(string schema)
        {
            try
            {
                TextReader stringReader = new StringReader(schema);
                var xsd = XmlSchema.Read(stringReader, (a, b) => { });
                return xsd;
            }
            catch (Exception exc)
            {
                _logger.LogError("Building Schema", exc);
                return null;
            }
        }
        private bool ValidateXml(string xml, XmlSchema schema)
        {
            XmlDocument document = null;
            return ValidateXml(xml, schema, out document);
        }
        private bool ValidateXml(string xml, XmlSchema schema, out XmlDocument document)
        {
            document = null;
            try
            {
                document = new XmlDocument();
                document.LoadXml(xml);
                document.Schemas.Add(schema);
                bool isValid = true;
                document.Validate((a, b) =>
                {
                    isValid = false;
                });
                return isValid;
            }
            catch (XmlException exc)
            {
                _logger.LogError("Invalid XML", exc);
            }
            return false;
        }


        public ConfigurationSaveResults UpdateConfigurationSectionJson(ConfigurationUpdate update)
        {
            var results = new ConfigurationSaveResults();

            var id = update.Id;
            var json = update.Value;

            ConfigurationValue record = Context.ConfigurationValues.Find(id);
            if (record != null)
            {

                if (ValidateJson(json, record.JSONSchema))
                {
                    var xmlSchema = GetSchema(record.XMLSchema);
                    //  string root = ((XmlSchemaElement)xmlSchema.Items[0]).Name;
                    //TODO some checks on the type throw exception
                    string xml = JsonConvert.DeserializeXmlNode(json).OuterXml;
                    if (ValidateXml(xml, xmlSchema))
                    {
                        record.XML = xml;
                        record.JSON = json;
                        Context.SetModified(record);
                        var changes = Context.SaveChanges();
                        if (changes == 0)
                            results.AddError("No records were removed ensure the id is correct.", DataStorageError.RecordsUnchanged);
                        else
                            results.Record = record;
                    }
                    else
                    {
                        results.AddError("The Xml does not match the schema", DataStorageError.DoesNotMatchXMlSchema);
                    }

                }
                else
                {
                    results.AddError("The Json does not match the schema", DataStorageError.DoesNotMatchJsonSchema);
                }

            }
            else
            {
                results.AddError("Record not found.", DataStorageError.RecordNotFound);
            }

            return results;
        }

        //TODO unit test, and refactor out all XML stuff into its own testable class/interface
        /// <summary>
        /// Adds xml attribute for json serialization to array
        /// 
        /// </summary>
        private void AssignJsonArrayAttributes(XmlDocument document, XmlNodeList nodes)
        {
            foreach (XmlNode node in nodes)
            {
                XmlSchemaElement schemaElement = node.SchemaInfo.SchemaElement as XmlSchemaElement;
                //When it is an array in XML it will have max occurs not 0 and therefore multi
                if (schemaElement != null && schemaElement.MaxOccurs > 1)
                {
                    //This attribute is used by the Json converter we are using to let it know its an array element and not an object type
                    var attribute = document.CreateAttribute("json", "Array", "http://james.newtonking.com/projects/json");
                    attribute.Value = "true";
                    node.Attributes.Append(attribute);
                }
                if (node.ChildNodes.Count != 0)
                    AssignJsonArrayAttributes(document, node.ChildNodes);
            }
        }
        public ConfigurationSaveResults UpdateConfigurationSectionXml(ConfigurationUpdate update)
        {
            var results = new ConfigurationSaveResults();
            var id = update.Id;
            var xml = update.Value;
            ConfigurationValue record = Context.ConfigurationValues.Find(id);
            if (record != null)
            {

                var xmlSchema = GetSchema(record.XMLSchema);
                XmlDocument document = null;
                if (ValidateXml(xml, xmlSchema, out document))
                {
                    this.AssignJsonArrayAttributes(document, document.ChildNodes);
                    string json = JsonConvert.SerializeXmlNode(document, Newtonsoft.Json.Formatting.Indented, false);
                    if (ValidateJson(json, record.JSONSchema))
                    {
                        record.XML = xml;
                        record.JSON = json;
                        Context.SetModified(record);
                        var changes = Context.SaveChanges();
                        if (changes == 0)
                            results.AddError("No records were removed ensure the id is correct.", DataStorageError.RecordsUnchanged);
                        else
                            results.Record = record;
                    }
                    else
                    {
                        results.AddError("The Json does not match the schema", DataStorageError.DoesNotMatchJsonSchema);
                    }
                }
                else
                {
                    results.AddError("The Xml does not match the schema", DataStorageError.DoesNotMatchXMlSchema);
                }
            }
            else
            {
                results.AddError("Record not found.", DataStorageError.RecordNotFound);
            }

            return results;
        }

        public void Dispose()
        {
            Context.Dispose();
        }


    }


}
