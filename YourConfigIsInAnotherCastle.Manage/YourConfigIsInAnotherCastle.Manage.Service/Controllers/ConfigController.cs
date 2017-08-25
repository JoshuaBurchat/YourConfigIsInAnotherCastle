using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Http;
using YourConfigIsInAnotherCastle.Logging;
using YourConfigIsInAnotherCastle.Models;
using YourConfigIsInAnotherCastle.Storage;
using YourConfigIsInAnotherCastle.Storage.Implementations.EF;

namespace YourConfigIsInAnotherCastle.Manage.Service
//namespace $rootnamespace$.Controllers
{

    public class ConfigController : ApiController
    {
        private IConfigurationStorage _dataStore;
        public ConfigController()
        {
            IConfigurationStorage dataStore = new ConfigurationStorage(
                ConfigurationManager.ConnectionStrings["ConfigurationStorage"].ConnectionString,
                new DefaultConsoleLogger()
            );

            _dataStore = dataStore;

            //add this into your global configuration and then you can remove camel-object-formatter.js
            //HttpConfiguration config = GlobalConfiguration.Configuration;
            //config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
            //config.Formatters.JsonFormatter.UseDataContractJsonSerializer = false;
        }
        private static readonly Func<ConfigurationValue, DTOConfigurationValue> _convertConfigurationValue = (c) =>
                c != null ?
                new DTOConfigurationValue()
                {
                    Id = c.Id,
                    JSON = c.JSON,
                    JSONSchema = c.JSONSchema,
                    Name = c.Name,
                    SystemName = c.SystemName,
                    Tags = c.Tags.Select(t => new DTOTag() { Id = t.Id, Value = t.Value }).ToList(),
                    XML = c.XML,
                    XMLSchema = c.XMLSchema
                } : (DTOConfigurationValue)null;
        private static readonly Func<ConfigurationSaveResults, DTOConfigurationSaveResults> _convertSaveResults = (c) =>
             new DTOConfigurationSaveResults()
             {
                 Errors = c.Errors,
                 Successful = c.Successful,
                 Record = _convertConfigurationValue(c.Record)
             };
        [HttpGet]
        public IEnumerable<DTOConfigurationValue> Get([FromUri] int[] tagIds)
        {
            return _dataStore.GetConfigurationSections(tagIds).Select(_convertConfigurationValue);
        }
        [HttpGet]
        public DTOConfigurationValue Get(int id)
        {
            return _convertConfigurationValue(_dataStore.GetConfigurationSection(id));
        }
        [HttpPost]
        public DTOConfigurationSaveResults Post([FromBody]DTOConfigurationIncludingSchemaDetails configurationDetails)
        {
            ConfigurationNew newRecord = new ConfigurationNew()
            {
                JSONSchema = configurationDetails.JSONSchema,
                Name = configurationDetails.Name,
                SystemName = configurationDetails.SystemName,
                XML = configurationDetails.XML,
                XMLSchema = configurationDetails.XMLSchema,
                Tags = configurationDetails.Tags
            };
            //TODO tags
            if (configurationDetails.Id.HasValue && configurationDetails.Id.Value != 0)
            {
                return _convertSaveResults(_dataStore.UpdateConfigurationSection(configurationDetails.Id.Value, newRecord));
            }
            else
            {
                return _convertSaveResults(_dataStore.AddConfigurationSection(newRecord));
            }
        }

        [HttpDelete]
        public DTOConfigurationSaveResults Delete([FromBody] DTOConfigurationDeletion container)
        {
            return _convertSaveResults(_dataStore.RemoveConfigurationSection(container.Id));
        }
        [HttpPut]
        public DTOConfigurationSaveResults Put([FromBody]DTOConfigurationUpdate dtoUpdate)
        {
            var update = new ConfigurationUpdate()
            {
                Id = dtoUpdate.Id,
                Value = dtoUpdate.Value
            };
            var format = dtoUpdate.Format;
            if (format == FormatTypeId.Json)
            {
                return _convertSaveResults(_dataStore.UpdateConfigurationSectionJson(update));
            }
            else if (format == FormatTypeId.Xml)
            {
                return _convertSaveResults(_dataStore.UpdateConfigurationSectionXml(update));
            }
            else
            {
                //TODO log or handle the exception
                throw new InvalidOperationException("Invalid format type specified");
            }

        }
        [HttpGet]
        [Route("api/Config/tags")]
        public DTOTag[] GetTags()
        {
            return this._dataStore.GetTags().Select(t => new DTOTag()
            {
                Id = t.Id,
                Value = t.Value
            }).ToArray();
        }
    }
}
