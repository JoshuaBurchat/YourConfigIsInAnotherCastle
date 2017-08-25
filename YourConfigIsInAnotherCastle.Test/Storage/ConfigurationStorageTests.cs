using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YourConfigIsInAnotherCastle.Models;
using YourConfigIsInAnotherCastle.Storage.Implementations.EF;
using YourConfigIsInAnotherCastle.Test.Mocks;
using DeepEqual.Syntax;
using DeepEqual;
using YourConfigIsInAnotherCastle.Storage;

namespace YourConfigIsInAnotherCastle.Test.Storage
{
    [TestClass]
    public class ConfigurationStorageTests
    {



        //XML
        [TestMethod]
        public void SaveValidXML_SuccessfulResults()
        {
            var customer = ConfigurationValueSeeds.Customers.BuildFullConfig();

            var newValuesCustomer = ConfigurationValueSeeds.Customers.BuildSingleConfig();
            var logger = new MockLogger();

            var fakeContext = new MockInAnotherCastleContext().AddConfigurationValues(customer);

            //Should match the new value record
            ConfigurationSaveResults expectedResults = new ConfigurationSaveResults()
            {
                Errors = new List<ErrorMap>(),
                Record = newValuesCustomer
            };

            ConfigurationSaveResults results = null;
            using (ConfigurationStorage storage = new ConfigurationStorage(fakeContext, logger))
            {
                results = storage.UpdateConfigurationSectionXml(new ConfigurationUpdate()
                {
                    Id = customer.Id,
                    Value = newValuesCustomer.XML
                });
            }
            //Ensure output is formatted the same without spaces
            ConfigurationValueSeeds.TrimWhiteSpace(results.Record);
            results.WithDeepEqual(expectedResults).Assert();
        }

        [TestMethod]
        public void SaveValidXML_IdNotExists_ErrorResultsRecordNotExists()
        {
            var logger = new MockLogger();

            var fakeContext = new MockInAnotherCastleContext();

            ConfigurationSaveResults results = null;
            using (ConfigurationStorage storage = new ConfigurationStorage(fakeContext, logger))
            {
                results = storage.UpdateConfigurationSectionXml(new ConfigurationUpdate()
                {
                    Id = 10000
                });
            }
            Assert.AreEqual(results.Errors.Count, 1, "Should be one error");
            Assert.AreEqual(results.Errors[0].Code, DataStorageError.RecordNotFound, "Record does not exist");
        }

        [TestMethod]
        public void SaveInvalidXML_ErrorResultsInvalidXML()
        {
            var customer = ConfigurationValueSeeds.Customers.BuildFullConfig();
            var logger = new MockLogger();

            var fakeContext = new MockInAnotherCastleContext().AddConfigurationValues(customer);

            ConfigurationSaveResults results = null;
            using (ConfigurationStorage storage = new ConfigurationStorage(fakeContext, logger))
            {
                results = storage.UpdateConfigurationSectionXml(new ConfigurationUpdate()
                {
                    Id = customer.Id,
                    Value = @"<Invalid>Hi</Invalid>"
                });
            }
            Assert.AreEqual(results.Errors.Count, 1, "Should be one error");
            Assert.AreEqual(results.Errors[0].Code, DataStorageError.DoesNotMatchXMlSchema, "XML schema should not match");
        }
        [TestMethod]
        public void SaveValidXMLMisMatchedJsonSchema_ErrorResultsInvalidJson()
        {
            //The results of this test would occur if the Json schema and XML schema are not in sync, or there is a difference and the generation makes
            //Something slightly out of line.
            var customer = ConfigurationValueSeeds.Customers.BuildFullConfig();
            //Invalid Json Schema that does not match the XML schema
            customer.JSONSchema = @"{
                ""$schema"": ""http://json-schema.org/draft-04/schema#"",
                        ""definitions"": {
                        },
                ""id"": ""http://example.com/example.json"",
                ""properties"": {
                            ""Invalid"": {
                                ""id"": ""/properties/Invalid"",
                        ""type"": ""string""
                            }
                        },
                ""type"": ""object"",
                ""required"": [""Invalid""]
            }";
            var logger = new MockLogger();

            var fakeContext = new MockInAnotherCastleContext().AddConfigurationValues(customer);

            ConfigurationSaveResults results = null;
            using (ConfigurationStorage storage = new ConfigurationStorage(fakeContext, logger))
            {
                results = storage.UpdateConfigurationSectionXml(new ConfigurationUpdate()
                {
                    Id = customer.Id,
                    Value = customer.XML
                });
            }
            Assert.AreEqual(results.Errors.Count, 1, "Should be one error");
            Assert.AreEqual(results.Errors[0].Code, DataStorageError.DoesNotMatchJsonSchema, "Json schema should not match");
        }
        //Json
        [TestMethod]
        public void SaveValidJson_SuccessfulResults()
        {
            var customer = ConfigurationValueSeeds.Customers.BuildFullConfig();
            var newValuesCustomer = ConfigurationValueSeeds.Customers.BuildSingleConfig();
            var logger = new MockLogger();

            var fakeContext = new MockInAnotherCastleContext().AddConfigurationValues(customer);

            //Should match the new value record
            ConfigurationSaveResults expectedResults = new ConfigurationSaveResults()
            {
                Errors = new List<ErrorMap>(),
                Record = newValuesCustomer
            };

            ConfigurationSaveResults results = null;
            using (ConfigurationStorage storage = new ConfigurationStorage(fakeContext, logger))
            {
                results = storage.UpdateConfigurationSectionJson(new ConfigurationUpdate()
                {
                    Id = customer.Id,
                    Value = newValuesCustomer.JSON
                });
            }
            //Ensure output is formatted the same without spaces
            ConfigurationValueSeeds.TrimWhiteSpace(results.Record);
            results.WithDeepEqual(expectedResults).Assert();
        }
        [TestMethod]
        public void SaveInvalidJson_ErrorResultsInvalidJson()
        {
            var customer = ConfigurationValueSeeds.Customers.BuildFullConfig();
            var logger = new MockLogger();

            var fakeContext = new MockInAnotherCastleContext().AddConfigurationValues(customer);

            ConfigurationSaveResults results = null;
            using (ConfigurationStorage storage = new ConfigurationStorage(fakeContext, logger))
            {
                results = storage.UpdateConfigurationSectionJson(new ConfigurationUpdate()
                {
                    Id = customer.Id,
                    Value = @"{ Invalid : ""Hi"" }"
                });
            }
            Assert.AreEqual(results.Errors.Count, 1, "Should be one error");
            Assert.AreEqual(results.Errors[0].Code, DataStorageError.DoesNotMatchJsonSchema, "Json schema should not match");
        }
        [TestMethod]
        public void SaveValidJsonMisMatchedXmlSchema_ErrorResultsInvalidXML()
        {
            //The results of this test would occur if the Json schema and XML schema are not in sync, or there is a difference and the generation makes
            //Something slightly out of line.
            var customer = ConfigurationValueSeeds.Customers.BuildFullConfig();
            //Invalid Json Schema that does not match the XML schema

            customer.XMLSchema = @"<xs:schema attributeFormDefault=""unqualified"" elementFormDefault=""qualified"" xmlns:xs=""http://www.w3.org/2001/XMLSchema"">
                                      <xs:element name=""Invalid"" type=""xs:byte""/>
                                    </xs:schema>";


            var logger = new MockLogger();

            var fakeContext = new MockInAnotherCastleContext().AddConfigurationValues(customer);

            ConfigurationSaveResults results = null;
            using (ConfigurationStorage storage = new ConfigurationStorage(fakeContext, logger))
            {
                results = storage.UpdateConfigurationSectionJson(new ConfigurationUpdate()
                {
                    Id = customer.Id,
                    Value = customer.JSON
                });
            }
            Assert.AreEqual(results.Errors.Count, 1, "Should be one error");
            Assert.AreEqual(results.Errors[0].Code, DataStorageError.DoesNotMatchXMlSchema, "Xml schema should not match");
        }
        [TestMethod]
        public void SaveValidJson_IdNotExists_ErrorResultsRecordNotExists()
        {
            var logger = new MockLogger();

            var fakeContext = new MockInAnotherCastleContext();

            ConfigurationSaveResults results = null;
            using (ConfigurationStorage storage = new ConfigurationStorage(fakeContext, logger))
            {
                results = storage.UpdateConfigurationSectionJson(new ConfigurationUpdate()
                {
                    Id = 10000
                });
            }
            Assert.AreEqual(results.Errors.Count, 1, "Should be one error");
            Assert.AreEqual(results.Errors[0].Code, DataStorageError.RecordNotFound, "Record does not exist");
        }

        [TestMethod]
        public void GetRecordByNameAndSystemName_Exists_RecordReturned()
        {
            var customer = ConfigurationValueSeeds.Customers.BuildFullConfig();
            customer.Name = Guid.NewGuid().ToString();//TO avoid hard coding false positive
            customer.SystemName = Guid.NewGuid().ToString();//TO avoid hard coding false positive
            var logger = new MockLogger();

            var fakeContext = new MockInAnotherCastleContext().AddConfigurationValues(customer, ConfigurationValueSeeds.Books.BuildFullConfig());

            ConfigurationValue results = null;
            using (ConfigurationStorage storage = new ConfigurationStorage(fakeContext, logger))
            {
                results = storage.GetConfigurationSection(customer.Name, customer.SystemName);
            }
            results.WithDeepEqual(customer).Assert();
        }
        [TestMethod]
        public void GetRecordByNameAndSystemName_NotExists_NullReturned()
        {
            var customer = ConfigurationValueSeeds.Customers.BuildFullConfig();
            customer.Name = Guid.NewGuid().ToString();//TO avoid hard coding false positive
            var logger = new MockLogger();

            var fakeContext = new MockInAnotherCastleContext().AddConfigurationValues(customer, ConfigurationValueSeeds.Books.BuildFullConfig());

            ConfigurationValue results = null;
            using (ConfigurationStorage storage = new ConfigurationStorage(fakeContext, logger))
            {
                results = storage.GetConfigurationSection(customer.Name + "X", customer.SystemName + "X");
            }
            Assert.IsNull(results, "Invalid names  should return no results");
        }

        [TestMethod]
        public void GetRecordByNullName_NullReturned()
        {
            var customer = ConfigurationValueSeeds.Customers.BuildFullConfig();
            customer.SystemName = null;//Ensure name is null or else this may be a false positive
            customer.Name = Guid.NewGuid().ToString();//TO avoid hard coding false positive
            var logger = new MockLogger();

            var fakeContext = new MockInAnotherCastleContext().AddConfigurationValues(customer, ConfigurationValueSeeds.Books.BuildFullConfig());

            ConfigurationValue results = null;
            using (ConfigurationStorage storage = new ConfigurationStorage(fakeContext, logger))
            {
                results = storage.GetConfigurationSection(null);
            }
            Assert.IsNull(customer.SystemName, "System name must be null while searching, this is to ensure test data is correct.");
            Assert.IsNull(results, "Null name should return no results");
        }
        [TestMethod]
        public void GetRecordByIncorrectName_NullReturned()
        {
            var customer = ConfigurationValueSeeds.Customers.BuildFullConfig();
            customer.SystemName = null;//Ensure name is null or else this may be a false positive
            customer.Name = Guid.NewGuid().ToString();//TO avoid hard coding false positive
            var logger = new MockLogger();

            var fakeContext = new MockInAnotherCastleContext().AddConfigurationValues(customer, ConfigurationValueSeeds.Books.BuildFullConfig());

            ConfigurationValue results = null;
            using (ConfigurationStorage storage = new ConfigurationStorage(fakeContext, logger))
            {
                //Note name changed to Guid aboved
                results = storage.GetConfigurationSection("Customer");
            }

            Assert.IsNull(customer.SystemName, "System name must be null while searching, this is to ensure test data is correct.");
            Assert.IsNull(results, "Invalid name should return no results");
        }


        [TestMethod]
        public void GetTags_ExcludeInactive_AllTagsAreUnRelated_NoTagsReturned()
        {
            var logger = new MockLogger();
            var config1 = new ConfigurationValue() { Id = 1 };
            var config2 = new ConfigurationValue() { Id = 2 };
            var config3 = new ConfigurationValue() { Id = 3 };
            var config4 = new ConfigurationValue() { Id = 4 };

            var tag1 = new Tag() { Id = 1, Value = "Tag1" };
            var tag2 = new Tag() { Id = 2, Value = "Tag2" };
            //Note no relationships

            var fakeContext = new MockInAnotherCastleContext().AddTags(
                tag1,
                tag2
            ).AddConfigurationValues(
                config1,
                config2,
                config3,
                config4
            );
            IEnumerable<Tag> results = null;
            using (ConfigurationStorage storage = new ConfigurationStorage(fakeContext, logger))
            {
                results = storage.GetTags(false);
            }
            Assert.AreEqual(results.Count(), 0, "No tags should be returned");
        }
        [TestMethod]
        public void GetTags_ExcludeInactive_AllTagsAreRelated_AllTagsReturned()
        {
            var logger = new MockLogger();
            var config1 = new ConfigurationValue() { Id = 1 };
            var config2 = new ConfigurationValue() { Id = 2 };
            var config3 = new ConfigurationValue() { Id = 3 };
            var config4 = new ConfigurationValue() { Id = 4 };

            var tag1 = new Tag() { Id = 1, Value = "Tag1" };
            tag1.ConfigurationValues.Add(config1);
            tag1.ConfigurationValues.Add(config2);
            config1.Tags.Add(tag1);
            config2.Tags.Add(tag1);
            var tag2 = new Tag() { Id = 2, Value = "Tag2" };
            tag2.ConfigurationValues.Add(config3);
            tag2.ConfigurationValues.Add(config4);
            config3.Tags.Add(tag2);
            config4.Tags.Add(tag2);

            var fakeContext = new MockInAnotherCastleContext().AddTags(
                tag1,
                tag2
            ).AddConfigurationValues(
                config1,
                config2,
                config3,
                config4
            );
            IEnumerable<Tag> results = null;
            using (ConfigurationStorage storage = new ConfigurationStorage(fakeContext, logger))
            {
                //Note name changed to Guid aboved
                results = storage.GetTags(false);
            }
            CollectionAssert.AreEquivalent(results.ToArray(), new[] { tag1, tag2 }, "results should contain all of the tags");
        }
        [TestMethod]
        public void GetTags_ExcludeInactive_SomeTagsAreRelated_RelatedTagsReturned()
        {
            var logger = new MockLogger();
            var config1 = new ConfigurationValue() { Id = 1,  };
            var config2 = new ConfigurationValue() { Id = 2 };
            //Next two will be unrelated
            //Note deleted so counts as unrelated
            var config3 = new ConfigurationValue() { Id = 3, Deleted = true };
            var config4 = new ConfigurationValue() { Id = 4 };

            var tag1 = new Tag() { Id = 1, Value = "Tag1" };
            tag1.ConfigurationValues.Add(config1);
            tag1.ConfigurationValues.Add(config2);
            config1.Tags.Add(tag1);
            config2.Tags.Add(tag1);
            var tag2 = new Tag() { Id = 2, Value = "Tag2" };
            tag2.ConfigurationValues.Add(config3);
            config3.Tags.Add(tag2);

            var fakeContext = new MockInAnotherCastleContext().AddTags(
                tag1,
                tag2
            ).AddConfigurationValues(
                config1,
                config2,
                config3,
                config4
            );
            IEnumerable<Tag> results = null;
            using (ConfigurationStorage storage = new ConfigurationStorage(fakeContext, logger))
            {
                //Note name changed to Guid aboved
                results = storage.GetTags(false);
            }
            CollectionAssert.AreEquivalent(results.ToArray(), new[] { tag1 }, "Only tag one should be present as Two is only related to a deleted config");
        }

        [TestMethod]
        public void GetTags_IncludeInactive_AllTagsAreUnRelated_AllTagsReturned()
        {

            var logger = new MockLogger();
            var config1 = new ConfigurationValue() { Id = 1 };
            var config2 = new ConfigurationValue() { Id = 2 };
            var config3 = new ConfigurationValue() { Id = 3 };
            var config4 = new ConfigurationValue() { Id = 4 };

            var tag1 = new Tag() { Id = 1, Value = "Tag1" };
            var tag2 = new Tag() { Id = 2, Value = "Tag2" };
            //Note no relationships

            var fakeContext = new MockInAnotherCastleContext().AddTags(
                tag1,
                tag2
            ).AddConfigurationValues(
                config1,
                config2,
                config3,
                config4
            );
            IEnumerable<Tag> results = null;
            using (ConfigurationStorage storage = new ConfigurationStorage(fakeContext, logger))
            {
                //Note true
                results = storage.GetTags(true);
            }
            CollectionAssert.AreEquivalent(results.ToArray(), new[] { tag1, tag2 }, "results should contain all of the tags, as inactive records are requested.");
        }
        [TestMethod]
        public void GetTags_IncludeInactive_AllTagsAreRelated_AllTagsReturned()
        {
            var logger = new MockLogger();
            var config1 = new ConfigurationValue() { Id = 1 };
            var config2 = new ConfigurationValue() { Id = 2 };
            var config3 = new ConfigurationValue() { Id = 3 };
            var config4 = new ConfigurationValue() { Id = 4 };

            var tag1 = new Tag() { Id = 1, Value = "Tag1" };
            tag1.ConfigurationValues.Add(config1);
            tag1.ConfigurationValues.Add(config2);
            config1.Tags.Add(tag1);
            config2.Tags.Add(tag1);
            var tag2 = new Tag() { Id = 2, Value = "Tag2" };
            tag2.ConfigurationValues.Add(config3);
            tag2.ConfigurationValues.Add(config4);
            config3.Tags.Add(tag2);
            config4.Tags.Add(tag2);

            var fakeContext = new MockInAnotherCastleContext().AddTags(
                tag1,
                tag2
            ).AddConfigurationValues(
                config1,
                config2,
                config3,
                config4
            );
            IEnumerable<Tag> results = null;
            using (ConfigurationStorage storage = new ConfigurationStorage(fakeContext, logger))
            {
                //Note name changed to Guid aboved
                results = storage.GetTags(true);
            }
            CollectionAssert.AreEquivalent(results.ToArray(), new[] { tag1, tag2 }, "results should contain all of the tags, as inactive records are requested.");
        }
        [TestMethod]
        public void GetTags_IncludeInactive_SomeTagsAreRelated_AllTagsReturned()
        {
            var logger = new MockLogger();
            var config1 = new ConfigurationValue() { Id = 1, };
            var config2 = new ConfigurationValue() { Id = 2 };
            //Next two will be unrelated
            //Note deleted so counts as unrelated
            var config3 = new ConfigurationValue() { Id = 3, Deleted = true };
            var config4 = new ConfigurationValue() { Id = 4 };

            var tag1 = new Tag() { Id = 1, Value = "Tag1" };
            tag1.ConfigurationValues.Add(config1);
            tag1.ConfigurationValues.Add(config2);
            config1.Tags.Add(tag1);
            config2.Tags.Add(tag1);
            var tag2 = new Tag() { Id = 2, Value = "Tag2" };
            tag2.ConfigurationValues.Add(config3);
            config3.Tags.Add(tag2);

            var fakeContext = new MockInAnotherCastleContext().AddTags(
                tag1,
                tag2
            ).AddConfigurationValues(
                config1,
                config2,
                config3,
                config4
            );
            IEnumerable<Tag> results = null;
            using (ConfigurationStorage storage = new ConfigurationStorage(fakeContext, logger))
            {
                //Note name changed to Guid aboved
                results = storage.GetTags(true);
            }
            CollectionAssert.AreEquivalent(results.ToArray(), new[] { tag1, tag2 }, "results should contain all of the tags, as inactive records are requested.");
        }

        [TestMethod]
        public void GetRecordByNameAndEmptySystemNamePassed_Exists_RecordReturned()
        {
            var customer = ConfigurationValueSeeds.Customers.BuildFullConfig();
            customer.SystemName = null;//Ensure name is null or else this may be a false positive
            customer.Name = Guid.NewGuid().ToString();//TO avoid hard coding false positive
            var logger = new MockLogger();

            var fakeContext = new MockInAnotherCastleContext().AddConfigurationValues(customer, ConfigurationValueSeeds.Books.BuildFullConfig());

            ConfigurationValue results = null;
            using (ConfigurationStorage storage = new ConfigurationStorage(fakeContext, logger))
            {
                //Note name changed to Guid aboved
                results = storage.GetConfigurationSection(customer.Name, string.Empty);
            }
            Assert.IsNull(customer.SystemName, "System name must be null while searching, this is to ensure test data is correct.");
            results.WithDeepEqual(customer).Assert();

        }
        [TestMethod]
        public void GetRecordByNameAndEmptySystemNameInStored_Exists_RecordReturned()
        {
            var customer = ConfigurationValueSeeds.Customers.BuildFullConfig();
            customer.SystemName = string.Empty;//Ensure name is null or else this may be a false positive
            customer.Name = Guid.NewGuid().ToString();//TO avoid hard coding false positive
            var logger = new MockLogger();

            var fakeContext = new MockInAnotherCastleContext().AddConfigurationValues(customer, ConfigurationValueSeeds.Books.BuildFullConfig());

            ConfigurationValue results = null;
            using (ConfigurationStorage storage = new ConfigurationStorage(fakeContext, logger))
            {
                //Note name changed to Guid aboved
                results = storage.GetConfigurationSection(customer.Name, null);
            }
            Assert.AreEqual(customer.SystemName, string.Empty, "System name must be empty while searching, this is to ensure test data is correct.");
            results.WithDeepEqual(customer).Assert();

        }
        [TestMethod]
        public void GetRecordByNameAndNullSystemName_NotExists_NullReturned()
        {
            var customer = ConfigurationValueSeeds.Customers.BuildFullConfig();
            customer.SystemName = Guid.NewGuid().ToString();//TO avoid hard coding false positive
            customer.Name = Guid.NewGuid().ToString();//TO avoid hard coding false positive
            var logger = new MockLogger();

            var fakeContext = new MockInAnotherCastleContext().AddConfigurationValues(customer, ConfigurationValueSeeds.Books.BuildFullConfig());

            ConfigurationValue results = null;
            using (ConfigurationStorage storage = new ConfigurationStorage(fakeContext, logger))
            {
                //Note name changed to Guid aboved and system name is not null
                results = storage.GetConfigurationSection(customer.Name, null);
            }

            Assert.IsNull(results, "Missing or incorrect system ane should return no results");

        }
        [TestMethod]
        public void GetRecordById_Exists_RecordReturned()
        {
            var customer = ConfigurationValueSeeds.Customers.BuildFullConfig();
            customer.Id = 12212;
            var logger = new MockLogger();

            var fakeContext = new MockInAnotherCastleContext().AddConfigurationValues(customer, ConfigurationValueSeeds.Books.BuildFullConfig());

            ConfigurationValue results = null;
            using (ConfigurationStorage storage = new ConfigurationStorage(fakeContext, logger))
            {
                results = storage.GetConfigurationSection(customer.Id);
            }
            results.WithDeepEqual(customer).Assert();
        }
        [TestMethod]
        public void GetRecordById_NotExists_NullReturned()
        {
            var customer = ConfigurationValueSeeds.Customers.BuildFullConfig();
            customer.Id = 12212;
            var logger = new MockLogger();

            var fakeContext = new MockInAnotherCastleContext().AddConfigurationValues(customer, ConfigurationValueSeeds.Books.BuildFullConfig());

            ConfigurationValue results = null;
            using (ConfigurationStorage storage = new ConfigurationStorage(fakeContext, logger))
            {
                //Does not exist
                results = storage.GetConfigurationSection(999);
            }
            Assert.IsNull(results, "Invalid id ane should return no results");
        }

        [TestMethod]
        public void GetRecordBySingleTag_Exists_RecordsReturned()
        {
            //Will be searched by
            Tag searchedBy = new Tag() { Id = 101, Value = "testby" };

            //Just to use in the other values to ensure it is ignored
            Tag notSearchedBy = new Tag() { Id = 200, Value = "nottestby" };

            var customerFull = ConfigurationValueSeeds.Customers.BuildFullConfig(searchedBy);
            var customerSingle = ConfigurationValueSeeds.Customers.BuildSingleConfig(searchedBy);


            var fakeContext = new MockInAnotherCastleContext().AddConfigurationValues(customerFull, customerSingle,
                ConfigurationValueSeeds.Books.BuildFullConfig(notSearchedBy), ConfigurationValueSeeds.Books.BuildSingleConfig(notSearchedBy));

            var logger = new MockLogger();

            ConfigurationValue[] results = null;
            using (ConfigurationStorage storage = new ConfigurationStorage(fakeContext, logger))
            {
                //Does not exist
                results = storage.GetConfigurationSections(new int[] { searchedBy.Id }).ToArray();
            }
            Assert.AreEqual(results.Length, 2, "Should only have the two customer records");
            CollectionAssert.Contains(results, customerFull, "Should contain full customer");
            CollectionAssert.Contains(results, customerSingle, "Should contain single customer");
        }

        [TestMethod]
        public void GetRecordByMultiTags_Exists_RecordsReturned()
        {
            //Will be searched by
            Tag searchedBy1 = new Tag() { Id = 101, Value = "testby1" };
            Tag searchedBy2 = new Tag() { Id = 202, Value = "testby2" };

            //Just to use in the other values to ensure it is ignored
            Tag notSearchedBy = new Tag() { Id = 300, Value = "nottestby" };

            var customerFull = ConfigurationValueSeeds.Customers.BuildFullConfig(searchedBy1);
            var customerSingle = ConfigurationValueSeeds.Customers.BuildSingleConfig(searchedBy2);


            var fakeContext = new MockInAnotherCastleContext().AddConfigurationValues(customerFull, customerSingle,
                ConfigurationValueSeeds.Books.BuildFullConfig(notSearchedBy), ConfigurationValueSeeds.Books.BuildSingleConfig(notSearchedBy));

            var logger = new MockLogger();

            ConfigurationValue[] results = null;
            using (ConfigurationStorage storage = new ConfigurationStorage(fakeContext, logger))
            {
                results = storage.GetConfigurationSections(new int[] { searchedBy1.Id, searchedBy2.Id }).ToArray();
            }
            Assert.AreEqual(results.Length, 2, "Should only have the two customer records");
            CollectionAssert.Contains(results, customerFull, "Should contain full customer");
            CollectionAssert.Contains(results, customerSingle, "Should contain single customer");
        }
        [TestMethod]
        public void GetRecordByNoTags_AllRecordsReturned()
        {
            //records have tags to prove they arent filtered on empty
            Tag notSearchedBy1 = new Tag() { Id = 101, Value = "testby1" };
            Tag notSearchedBy2 = new Tag() { Id = 202, Value = "testby2" };
            Tag notSearchedBy3 = new Tag() { Id = 300, Value = "nottestby" };

            var customerFull = ConfigurationValueSeeds.Customers.BuildFullConfig(notSearchedBy1);
            var customerSingle = ConfigurationValueSeeds.Customers.BuildSingleConfig(notSearchedBy2);
            var bookFull = ConfigurationValueSeeds.Books.BuildFullConfig(notSearchedBy3);
            var bookSingle = ConfigurationValueSeeds.Books.BuildSingleConfig(notSearchedBy3);


            var fakeContext = new MockInAnotherCastleContext().AddConfigurationValues(customerFull, customerSingle, bookFull, bookSingle);

            var logger = new MockLogger();

            ConfigurationValue[] results = null;
            using (ConfigurationStorage storage = new ConfigurationStorage(fakeContext, logger))
            {

                results = storage.GetConfigurationSections().ToArray();
            }
            Assert.AreEqual(results.Length, 4, "Should only have the four customer records");
            CollectionAssert.Contains(results, customerFull);
            CollectionAssert.Contains(results, customerSingle);
            CollectionAssert.Contains(results, bookFull);
            CollectionAssert.Contains(results, bookSingle);
        }



        [TestMethod]
        public void GetRecordBySystemName_WhenSomeAreDeleted__RecordsReturnedNoneWhichAreDeleted()
        {

            var customerFull = ConfigurationValueSeeds.Customers.BuildFullConfig();
            var customerSingle = ConfigurationValueSeeds.Customers.BuildSingleConfig();
            customerSingle.Deleted = true;
            var bookFull = ConfigurationValueSeeds.Books.BuildFullConfig();
            bookFull.Deleted = true;
            var bookSingle = ConfigurationValueSeeds.Books.BuildSingleConfig();
            customerFull.SystemName =
            customerSingle.SystemName =
            bookFull.SystemName =
            bookSingle.SystemName = "SystemName";


            var fakeContext = new MockInAnotherCastleContext().AddConfigurationValues(customerFull, customerSingle, bookFull, bookSingle);

            var logger = new MockLogger();

            ConfigurationValue[] results = null;
            using (ConfigurationStorage storage = new ConfigurationStorage(fakeContext, logger))
            {

                results = storage.GetConfigurationSections("SystemName").ToArray();
            }
            Assert.AreEqual(results.Length, 2, "Should only have the two customer records");
            CollectionAssert.Contains(results, customerFull);
            CollectionAssert.Contains(results, bookSingle);
        }

        [TestMethod]
        public void GetRecordByTag_WhenSomeAreDeleted__RecordsReturnedNoneWhichAreDeleted()
        {

            var customerFull = ConfigurationValueSeeds.Customers.BuildFullConfig();
            var customerSingle = ConfigurationValueSeeds.Customers.BuildSingleConfig();
            customerSingle.Deleted = true;
            var bookFull = ConfigurationValueSeeds.Books.BuildFullConfig();
            bookFull.Deleted = true;
            var bookSingle = ConfigurationValueSeeds.Books.BuildSingleConfig();
            var tag = new Tag() { Id = 22, Value = "Tag1" };
            customerFull.Tags.Add(tag);
            customerSingle.Tags.Add(tag);
            bookFull.Tags.Add(tag);
            bookSingle.Tags.Add(tag);

            var fakeContext = new MockInAnotherCastleContext().AddConfigurationValues(customerFull, customerSingle, bookFull, bookSingle);

            var logger = new MockLogger();

            ConfigurationValue[] results = null;
            using (ConfigurationStorage storage = new ConfigurationStorage(fakeContext, logger))
            {

                results = storage.GetConfigurationSections(new[] { 22 }).ToArray();
            }
            Assert.AreEqual(results.Length, 2, "Should only have the two customer records");
            CollectionAssert.Contains(results, customerFull);
            CollectionAssert.Contains(results, bookSingle);
        }

        [TestMethod]
        public void GetRecordByNullSystemName_Success_RecordsReturned()
        {
            //Note null and empty should return under same condition
            var customerFull = ConfigurationValueSeeds.Customers.BuildFullConfig();
            customerFull.SystemName = "";
            var customerSingle = ConfigurationValueSeeds.Customers.BuildSingleConfig();
            customerSingle.SystemName = null;
            var bookFull = ConfigurationValueSeeds.Books.BuildFullConfig();
            var bookSingle = ConfigurationValueSeeds.Books.BuildSingleConfig();


            var fakeContext = new MockInAnotherCastleContext().AddConfigurationValues(customerFull, customerSingle, bookFull, bookSingle);

            var logger = new MockLogger();

            ConfigurationValue[] results = null;
            using (ConfigurationStorage storage = new ConfigurationStorage(fakeContext, logger))
            {

                results = storage.GetConfigurationSections((string)null).ToArray();
            }
            Assert.AreEqual(results.Length, 2, "Should only have the two customer records");
            CollectionAssert.Contains(results, customerFull);
            CollectionAssert.Contains(results, customerSingle);
        }
        [TestMethod]
        public void GetRecordByEmptySystemName_Success_RecordsReturned()
        {
            //Note null and empty should return under same condition
            var customerFull = ConfigurationValueSeeds.Customers.BuildFullConfig();
            customerFull.SystemName = "";
            var customerSingle = ConfigurationValueSeeds.Customers.BuildSingleConfig();
            customerSingle.SystemName = null;
            var bookFull = ConfigurationValueSeeds.Books.BuildFullConfig();
            var bookSingle = ConfigurationValueSeeds.Books.BuildSingleConfig();


            var fakeContext = new MockInAnotherCastleContext().AddConfigurationValues(customerFull, customerSingle, bookFull, bookSingle);

            var logger = new MockLogger();

            ConfigurationValue[] results = null;
            using (ConfigurationStorage storage = new ConfigurationStorage(fakeContext, logger))
            {

                results = storage.GetConfigurationSections("").ToArray();
            }
            Assert.AreEqual(results.Length, 2, "Should only have the two customer records");
            CollectionAssert.Contains(results, customerFull);
            CollectionAssert.Contains(results, customerSingle);
        }
        [TestMethod]
        public void GetRecordByInvalidSystemName_NoRecordsReturned()
        {
            //Note null and empty should return under same condition
            var customerFull = ConfigurationValueSeeds.Customers.BuildFullConfig();
            var customerSingle = ConfigurationValueSeeds.Customers.BuildSingleConfig();
            var bookFull = ConfigurationValueSeeds.Books.BuildFullConfig();
            var bookSingle = ConfigurationValueSeeds.Books.BuildSingleConfig();


            var fakeContext = new MockInAnotherCastleContext().AddConfigurationValues(customerFull, customerSingle, bookFull, bookSingle);

            var logger = new MockLogger();

            ConfigurationValue[] results = null;
            using (ConfigurationStorage storage = new ConfigurationStorage(fakeContext, logger))
            {
                results = storage.GetConfigurationSections("Non Existing Name").ToArray();
            }
            Assert.AreEqual(results.Length, 0, "Should be no records returned under an invalid name");
        }

        [TestMethod]
        public void RemoveConfigurationItem_ExistsLocally_RecordRemoved()
        {
            //Note null and empty should return under same condition
            var customerFull = ConfigurationValueSeeds.Customers.BuildFullConfig();
            customerFull.Id = 1002;
            var customerSingle = ConfigurationValueSeeds.Customers.BuildSingleConfig();
            var bookFull = ConfigurationValueSeeds.Books.BuildFullConfig();
            var bookSingle = ConfigurationValueSeeds.Books.BuildSingleConfig();


            var fakeContext = new MockInAnotherCastleContext().AddConfigurationValues(customerFull, customerSingle, bookFull, bookSingle);
            bool wasRemoved = false;
            fakeContext.SaveChangesFunc = () =>
            {
                wasRemoved = fakeContext.Modified.Contains(customerFull);
                return 1;
            };
            var logger = new MockLogger();

            ConfigurationSaveResults results = null;
            using (ConfigurationStorage storage = new ConfigurationStorage(fakeContext, logger))
            {
                results = storage.RemoveConfigurationSection(customerFull.Id);
            }
            Assert.IsTrue(results.Successful);
            Assert.IsTrue(wasRemoved, "Was removed is checking that the fake context has tracked the record as deleted.");

        }

        [TestMethod]
        public void RemoveConfigurationItem_ExistsInDataStored_RecordRemoved()
        {
            //Note null and empty should return under same condition
            var customerFull = ConfigurationValueSeeds.Customers.BuildFullConfig();
            customerFull.Id = 1002;
            var customerSingle = ConfigurationValueSeeds.Customers.BuildSingleConfig();
            var bookFull = ConfigurationValueSeeds.Books.BuildFullConfig();
            var bookSingle = ConfigurationValueSeeds.Books.BuildSingleConfig();

            //Note clear for local
            var fakeContext = new MockInAnotherCastleContext().AddConfigurationValues(true, customerFull, customerSingle, bookFull, bookSingle);
            bool wasRemoved = false;
            fakeContext.SaveChangesFunc = () =>
            {
                wasRemoved = fakeContext.Modified.Cast<ConfigurationValue>().Any(d => d.Id == customerFull.Id && d.Deleted);
                return 1;
            };
            var logger = new MockLogger();

            ConfigurationSaveResults results = null;
            using (ConfigurationStorage storage = new ConfigurationStorage(fakeContext, logger))
            {
                results = storage.RemoveConfigurationSection(customerFull.Id);
            }
            Assert.IsTrue(results.Successful);
            Assert.IsTrue(wasRemoved, "Was removed is checking that the fake context has tracked the record as deleted.");

        }



        [TestMethod]
        public void AddNewConfigurationSectionRecord_Valid_SuccessfulResultsAndRecordReturned()
        {
            var fullRecord = ConfigurationValueSeeds.Books.BuildSingleConfig();
            fullRecord.Id = 111;



            var fakeContext = new MockInAnotherCastleContext();
            fakeContext.SaveChangesFunc = () =>
            {
                foreach (var value in fakeContext.Added.Cast<ConfigurationValue>())
                {
                    value.Id = fullRecord.Id;
                }
                return 1;
            };
            var logger = new MockLogger();

            ConfigurationSaveResults expectedResults = new ConfigurationSaveResults()
            {
                Record = fullRecord,
                Errors = new List<ErrorMap>()
            };
            ConfigurationSaveResults results = null;
            using (ConfigurationStorage storage = new ConfigurationStorage(fakeContext, logger))
            {
                results = storage.AddConfigurationSection(new ConfigurationNew()
                {
                    JSONSchema = fullRecord.JSONSchema,
                    Name = fullRecord.Name,
                    SystemName = fullRecord.SystemName,
                    XML = fullRecord.XML,
                    XMLSchema = fullRecord.XMLSchema
                });
            }
            ConfigurationValueSeeds.TrimWhiteSpace(results.Record);
            CollectionAssert.AreEquivalent(fakeContext.Added, new object[] { results.Record });
            expectedResults.WithDeepEqual(results).IgnoreUnmatchedProperties().Assert();
        }
        [TestMethod]
        public void AddNewConfigurationSectionRecord_InvalidJson_ErrorResultsInvalidJson()
        {
            var fullRecord = ConfigurationValueSeeds.Books.BuildSingleConfig();
            fullRecord.Id = 111;
            //Json schema does not match the book schema
            fullRecord.JSONSchema = @"{
                ""$schema"": ""http://json-schema.org/draft-04/schema#"",
                        ""definitions"": {
                        },
                ""id"": ""http://example.com/example.json"",
                ""properties"": {
                            ""Invalid"": {
                                ""id"": ""/properties/Invalid"",
                        ""type"": ""string""
                            }
                        },
                ""type"": ""object"",
                ""required"": [""Invalid""]
            }";
            var fakeContext = new MockInAnotherCastleContext();
            fakeContext.SaveChangesFunc = () =>
            {
                foreach (var value in fakeContext.Added.Cast<ConfigurationValue>())
                {
                    value.Id = fullRecord.Id;
                }
                return 1;
            };
            var logger = new MockLogger();

            ConfigurationSaveResults results = null;
            using (ConfigurationStorage storage = new ConfigurationStorage(fakeContext, logger))
            {
                results = storage.AddConfigurationSection(new ConfigurationNew()
                {
                    JSONSchema = fullRecord.JSONSchema,
                    Name = fullRecord.Name,
                    SystemName = fullRecord.SystemName,
                    XML = fullRecord.XML,
                    XMLSchema = fullRecord.XMLSchema
                });
            }
            Assert.AreEqual(results.Errors.Count, 1, "Should be one error");
            Assert.AreEqual(results.Errors[0].Code, DataStorageError.DoesNotMatchJsonSchema, "Json schema should not match");
            Assert.AreEqual(fakeContext.Added.Count + fakeContext.Modified.Count, 0, "Should be no changes due to errors");

        }
        [TestMethod]
        public void AddNewConfigurationSectionRecord_InvalidXml_ErrorResultsInvalidJson()
        {
            var fullRecord = ConfigurationValueSeeds.Books.BuildSingleConfig();
            fullRecord.Id = 111;
            //Xml schema does not match the book schema
            fullRecord.XMLSchema = @"<xs:schema attributeFormDefault=""unqualified"" elementFormDefault=""qualified"" xmlns:xs=""http://www.w3.org/2001/XMLSchema"">
                                      <xs:element name=""Invalid"" type=""xs:byte""/>
                                    </xs:schema>"; ;
            var fakeContext = new MockInAnotherCastleContext();
            fakeContext.SaveChangesFunc = () =>
            {
                foreach (var value in fakeContext.Added.Cast<ConfigurationValue>())
                {
                    value.Id = fullRecord.Id;
                }
                return 1;
            };
            var logger = new MockLogger();


            ConfigurationSaveResults results = null;
            using (ConfigurationStorage storage = new ConfigurationStorage(fakeContext, logger))
            {
                results = storage.AddConfigurationSection(new ConfigurationNew()
                {
                    JSONSchema = fullRecord.JSONSchema,
                    Name = fullRecord.Name,
                    SystemName = fullRecord.SystemName,
                    XML = fullRecord.XML,
                    XMLSchema = fullRecord.XMLSchema
                });
            }
            Assert.AreEqual(results.Errors.Count, 1, "Should be one error");
            Assert.AreEqual(results.Errors[0].Code, DataStorageError.DoesNotMatchXMlSchema, "Xml schema should not match");
            Assert.AreEqual(fakeContext.Added.Count + fakeContext.Modified.Count, 0, "Should be no changes due to errors");

        }
        [TestMethod]
        public void AddNewConfigurationSectionRecord_MissingFields_XML_ValidationError()
        {
            var fullRecord = ConfigurationValueSeeds.Books.BuildSingleConfig();
            var fakeContext = new MockInAnotherCastleContext();
            fakeContext.SaveChangesFunc = () =>
            {
                foreach (var value in fakeContext.Added.Cast<ConfigurationValue>())
                {
                    value.Id = 1;
                }
                return 1;
            };
            var logger = new MockLogger();

            ConfigurationSaveResults results = null;
            using (ConfigurationStorage storage = new ConfigurationStorage(fakeContext, logger))
            {
                results = storage.AddConfigurationSection(new ConfigurationNew()
                {
                    JSONSchema = fullRecord.JSONSchema,
                    Name = fullRecord.Name,
                    SystemName = fullRecord.SystemName,
                    XML = "",
                    XMLSchema = fullRecord.XMLSchema
                });
            }
            Assert.AreEqual(results.Errors.Count, 1, "Should be one error");
            Assert.AreEqual(results.Errors[0].Code, DataStorageError.DoesNotMatchXMlSchema, "Xml schema should not match");
            Assert.AreEqual(fakeContext.Added.Count + fakeContext.Modified.Count, 0, "Should be no changes due to errors");
        }

        [TestMethod]
        public void AddNewConfigurationSectionRecord_MissingFields_XMLSchema_ValidationError()
        {
            var fullRecord = ConfigurationValueSeeds.Books.BuildSingleConfig();
            var fakeContext = new MockInAnotherCastleContext();
            fakeContext.SaveChangesFunc = () =>
            {
                foreach (var value in fakeContext.Added.Cast<ConfigurationValue>())
                {
                    value.Id = 1;
                }
                return 1;
            };
            var logger = new MockLogger();

            ConfigurationSaveResults results = null;
            using (ConfigurationStorage storage = new ConfigurationStorage(fakeContext, logger))
            {
                results = storage.AddConfigurationSection(new ConfigurationNew()
                {
                    JSONSchema = fullRecord.JSONSchema,
                    Name = fullRecord.Name,
                    SystemName = fullRecord.SystemName,
                    XML = fullRecord.XML,
                    XMLSchema = ""
                });
            }
            Assert.AreEqual(results.Errors.Count, 1, "Should be one error");
            Assert.AreEqual(results.Errors[0].Code, DataStorageError.InvalidXmlSchema, "Xml schema is invalid");
            Assert.AreEqual(fakeContext.Added.Count + fakeContext.Modified.Count, 0, "Should be no changes due to errors");
        }
        [TestMethod]
        public void AddNewConfigurationSectionRecord_MissingFields_Name_ValidationError()
        {
            var fullRecord = ConfigurationValueSeeds.Books.BuildSingleConfig();
            var fakeContext = new MockInAnotherCastleContext();
            fakeContext.SaveChangesFunc = () =>
            {
                foreach (var value in fakeContext.Added.Cast<ConfigurationValue>())
                {
                    value.Id = 1;
                }
                return 1;
            };
            var logger = new MockLogger();

            ConfigurationSaveResults results = null;
            using (ConfigurationStorage storage = new ConfigurationStorage(fakeContext, logger))
            {
                results = storage.AddConfigurationSection(new ConfigurationNew()
                {
                    JSONSchema = fullRecord.JSONSchema,
                    Name = "",
                    SystemName = fullRecord.SystemName,
                    XML = fullRecord.XML,
                    XMLSchema = fullRecord.XMLSchema
                });
            }
            Assert.AreEqual(results.Errors.Count, 1, "Should be one error");
            Assert.AreEqual(results.Errors[0].Code, DataStorageError.MissingNameField, "Name field is required");
            Assert.AreEqual(fakeContext.Added.Count + fakeContext.Modified.Count, 0, "Should be no changes due to errors");
        }
        [TestMethod]
        public void AddNewConfigurationSectionRecord_MissingFields_JsonSchema_ValidationError()
        {
            var fullRecord = ConfigurationValueSeeds.Books.BuildSingleConfig();
            var fakeContext = new MockInAnotherCastleContext();
            fakeContext.SaveChangesFunc = () =>
            {
                foreach (var value in fakeContext.Added.Cast<ConfigurationValue>())
                {
                    value.Id = 1;
                }
                return 1;
            };
            var logger = new MockLogger();

            ConfigurationSaveResults results = null;
            using (ConfigurationStorage storage = new ConfigurationStorage(fakeContext, logger))
            {
                results = storage.AddConfigurationSection(new ConfigurationNew()
                {
                    JSONSchema = "",
                    Name = fullRecord.Name,
                    SystemName = fullRecord.SystemName,
                    XML = fullRecord.XML,
                    XMLSchema = fullRecord.XMLSchema
                });
            }
            Assert.AreEqual(results.Errors.Count, 1, "Should be one error");
            Assert.AreEqual(results.Errors[0].Code, DataStorageError.DoesNotMatchJsonSchema, "Json schema does not match");
            Assert.AreEqual(fakeContext.Added.Count + fakeContext.Modified.Count, 0, "Should be no changes due to errors");
        }
        [TestMethod]
        public void AddNewConfigurationSectionRecord_NameAndSystemExist_ErrorResultsAlreadyExists()
        {
            var fullRecord = ConfigurationValueSeeds.Books.BuildSingleConfig();

            //Note record added
            var fakeContext = new MockInAnotherCastleContext().AddConfigurationValues(fullRecord);
            var logger = new MockLogger();

            ConfigurationSaveResults results = null;
            using (ConfigurationStorage storage = new ConfigurationStorage(fakeContext, logger))
            {
                results = storage.AddConfigurationSection(new ConfigurationNew()
                {
                    JSONSchema = fullRecord.JSONSchema,
                    Name = fullRecord.Name,
                    SystemName = fullRecord.SystemName,
                    XML = fullRecord.XML,
                    XMLSchema = fullRecord.XMLSchema
                });
            }
            Assert.AreEqual(results.Errors.Count, 1, "Should be one error");
            Assert.AreEqual(results.Errors[0].Code, DataStorageError.AlreadyExists, "Record should already exist as it was added to the test context");
            Assert.AreEqual(fakeContext.Added.Count + fakeContext.Modified.Count, 0, "Should be no changes due to errors");
        }
        [TestMethod]
        public void AddNewConfigurationSectionRecord_WithTagsThatExist_RecordAddedTagsMapped()
        {

            var existingTag1 = new Tag() { Id = 1, Value = "Test1" };
            var existingTag2 = new Tag() { Id = 2, Value = "Test2" };

            //Note record added
            var fakeContext = new MockInAnotherCastleContext().AddTags(existingTag1, existingTag2);
            var logger = new MockLogger();

            var fullRecord = ConfigurationValueSeeds.Books.BuildSingleConfig();
            fullRecord.Tags.Add(existingTag1);
            fullRecord.Tags.Add(existingTag2);
            fullRecord.Id = 0;
            ConfigurationSaveResults results = null;
            using (ConfigurationStorage storage = new ConfigurationStorage(fakeContext, logger))
            {
                results = storage.AddConfigurationSection(new ConfigurationNew()
                {
                    JSONSchema = fullRecord.JSONSchema,
                    Name = fullRecord.Name,
                    SystemName = fullRecord.SystemName,
                    XML = fullRecord.XML,
                    XMLSchema = fullRecord.XMLSchema,
                    Tags = new List<Tag>()
                    {
                        existingTag1, existingTag2
                    }
                });
            }


            ConfigurationValueSeeds.TrimWhiteSpace(results.Record);
            Assert.IsTrue(results.Successful, "should be a successful save");

            fullRecord.WithDeepEqual(results.Record).IgnoreSourceProperty(c => c.Tags).IgnoreUnmatchedProperties().Assert();


            fullRecord.Tags.OrderBy(c => c.Id).WithDeepEqual(results.Record.Tags.OrderBy(c => c.Id)).Assert();
            CollectionAssert.AreEquivalent(fakeContext.Added, new object[] { results.Record }, "The record should be added, the tags exist so should not be added");


            Assert.AreEqual(fakeContext.ConfigTagAddTransactions.First().Value.Count(), 2, "Contains two adds for tags");
            Assert.IsTrue(fakeContext.ConfigTagAddTransactions.First().Value
                    .Any(t => t.Id == 1)
            , "Tag to Config Mapping should contain first tag");
            Assert.IsTrue(fakeContext.ConfigTagAddTransactions.First().Value
                    .Any(t => t.Id == 2)
            , "Tag to Config Mapping should contain second tag");
        }

        [TestMethod]
        public void AddNewConfigurationSectionRecord_WithNewTags_RecordAddedTagsAddedAndMapped()
        {
            var newTag1 = new Tag() { Id = 0, Value = "Test1" };
            var newTag2 = new Tag() { Id = 0, Value = "Test2" };

            //Note record added
            var fakeContext = new MockInAnotherCastleContext();
            var logger = new MockLogger();

            var fullRecord = ConfigurationValueSeeds.Books.BuildSingleConfig();
            fullRecord.Tags.Add(newTag1);
            fullRecord.Tags.Add(newTag2);
            fullRecord.Id = 0;
            ConfigurationSaveResults results = null;
            using (ConfigurationStorage storage = new ConfigurationStorage(fakeContext, logger))
            {
                results = storage.AddConfigurationSection(new ConfigurationNew()
                {
                    JSONSchema = fullRecord.JSONSchema,
                    Name = fullRecord.Name,
                    SystemName = fullRecord.SystemName,
                    XML = fullRecord.XML,
                    XMLSchema = fullRecord.XMLSchema,
                    Tags = new List<Tag>()
                    {
                        newTag1, newTag2
                    }
                });
            }


            ConfigurationValueSeeds.TrimWhiteSpace(results.Record);
            Assert.IsTrue(results.Successful, "should be a successful save");

            fullRecord.WithDeepEqual(results.Record).IgnoreSourceProperty(c => c.Tags).IgnoreUnmatchedProperties().Assert();

            fullRecord.Tags.OrderBy(c => c.Id).WithDeepEqual(results.Record.Tags.OrderBy(c => c.Id)).Assert();

            CollectionAssert.AreEquivalent(fakeContext.Added, new object[] { results.Record, newTag1, newTag2 }, "The record as well as the tags should be added");


            Assert.AreEqual(fakeContext.ConfigTagAddTransactions.First().Value.Count(), 2, "Contains two adds for tags");
            Assert.IsTrue(fakeContext.ConfigTagAddTransactions.First().Value
                    .Any(t => t.Value == "Test1")
            , "Should contain first tag");
            Assert.IsTrue(fakeContext.ConfigTagAddTransactions.First().Value
                    .Any(t => t.Value == "Test2")
            , "Should contain second tag");
        }
        [TestMethod]
        public void AddNewConfigurationSectionRecord_WithTagsThatExistAndNewTags_RecordAddedTagsMapped()
        {
            var newTag1 = new Tag() { Id = 0, Value = "Test1" };
            var existingTag2 = new Tag() { Id = 2, Value = "Test2" };

            var fullRecord = ConfigurationValueSeeds.Books.BuildSingleConfig();
            fullRecord.Tags.Add(newTag1);
            fullRecord.Tags.Add(existingTag2);
            fullRecord.Id = 0;
            //Note record added
            var fakeContext = new MockInAnotherCastleContext().AddTags(existingTag2);
            var logger = new MockLogger();


            ConfigurationSaveResults results = null;
            using (ConfigurationStorage storage = new ConfigurationStorage(fakeContext, logger))
            {
                results = storage.AddConfigurationSection(new ConfigurationNew()
                {
                    JSONSchema = fullRecord.JSONSchema,
                    Name = fullRecord.Name,
                    SystemName = fullRecord.SystemName,
                    XML = fullRecord.XML,
                    XMLSchema = fullRecord.XMLSchema,
                    Tags = new List<Tag>()
                    {
                        newTag1, existingTag2
                    }
                });
            }

            ConfigurationValueSeeds.TrimWhiteSpace(results.Record);
            Assert.IsTrue(results.Successful, "should be a successful save");

            fullRecord.WithDeepEqual(results.Record).IgnoreSourceProperty(c => c.Tags).IgnoreUnmatchedProperties().Assert();

            fullRecord.Tags.OrderBy(c => c.Id).WithDeepEqual(results.Record.Tags.OrderBy(c => c.Id)).Assert();
            CollectionAssert.AreEquivalent(fakeContext.Added, new object[] { newTag1, results.Record }, "New tag and record should be added");


            Assert.AreEqual(fakeContext.ConfigTagAddTransactions.First().Value.Count(), 2, "Contains two adds for tags");
            Assert.IsTrue(fakeContext.ConfigTagAddTransactions.First().Value
                    .Any(t => t.Value == newTag1.Value)
            , "Tag to Config Mapping should contain first tag");
            Assert.IsTrue(fakeContext.ConfigTagAddTransactions.First().Value
                    .Any(t => t.Id == 2)
            , "Tag to Config Mapping should contain second tag");


        }

        [TestMethod]
        public void AddNewConfigurationSectionRecord_TagValueExistId0_RecordAddedTagsMapped()
        {
            var newTag1 = new Tag() { Id = 0, Value = "Test1" };
            //Note same value, except the new value has ID 0, the store will resolve the ID
            var existingTag2 = new Tag() { Id = 12323, Value = "Test1" };

            var expectedFullRecord = ConfigurationValueSeeds.Books.BuildSingleConfig();
            //Note this is the expected tag
            expectedFullRecord.Tags.Add(existingTag2);
            expectedFullRecord.Id = 0;
            //Note record added
            var fakeContext = new MockInAnotherCastleContext().AddTags(existingTag2);
            var logger = new MockLogger();


            ConfigurationSaveResults results = null;
            using (ConfigurationStorage storage = new ConfigurationStorage(fakeContext, logger))
            {
                results = storage.AddConfigurationSection(new ConfigurationNew()
                {
                    JSONSchema = expectedFullRecord.JSONSchema,
                    Name = expectedFullRecord.Name,
                    SystemName = expectedFullRecord.SystemName,
                    XML = expectedFullRecord.XML,
                    XMLSchema = expectedFullRecord.XMLSchema,
                    Tags = new List<Tag>()
                    {
                        //new tag with id 0
                        newTag1
                    }
                });
            }

            ConfigurationValueSeeds.TrimWhiteSpace(results.Record);
            Assert.IsTrue(results.Successful, "should be a successful save");

            expectedFullRecord.WithDeepEqual(results.Record).IgnoreSourceProperty(c => c.Tags).IgnoreUnmatchedProperties().Assert();

            expectedFullRecord.Tags.OrderBy(c => c.Id).WithDeepEqual(results.Record.Tags.OrderBy(c => c.Id)).Assert();
            CollectionAssert.AreEquivalent(fakeContext.Added, new object[] { results.Record }, "Only new record added, no tags should be changed");


            Assert.AreEqual(fakeContext.ConfigTagAddTransactions.First().Value.Count(), 1, "Contains one add for tags");

            Assert.IsTrue(fakeContext.ConfigTagAddTransactions.First().Value
                    .Any(t => t.Id == existingTag2.Id)
            , "Tag to Config Mapping should contain second tag");


        }
        [TestMethod]
        public void AddNewConfigurationSectionRecord_InvalidTagId_RecordAddedTagsMapped()
        {
            //Existing

            //Note it has an Id and hasnt been added to the context
            var invalidTag = new Tag() { Id = 123242, Value = "Test1" };

            var expectedFullRecord = ConfigurationValueSeeds.Books.BuildSingleConfig();
            expectedFullRecord.Id = 0;
            //Note this is the expected tag
            expectedFullRecord.Tags.Add(invalidTag);


            var fakeContext = new MockInAnotherCastleContext();
            var logger = new MockLogger();


            ConfigurationSaveResults results = null;
            using (ConfigurationStorage storage = new ConfigurationStorage(fakeContext, logger))
            {
                results = storage.AddConfigurationSection(new ConfigurationNew()
                {
                    JSONSchema = expectedFullRecord.JSONSchema,
                    Name = expectedFullRecord.Name,
                    SystemName = expectedFullRecord.SystemName,
                    XML = expectedFullRecord.XML,
                    XMLSchema = expectedFullRecord.XMLSchema,
                    Tags = new List<Tag>()
                    {
                        //new tag with id 0
                        invalidTag
                    }
                });
            }

            Assert.AreEqual(results.Errors.Count, 1, "Should be one error");
            Assert.AreEqual(results.Errors[0].Code, DataStorageError.RecordNotFound, "Invalid tag Id could not be found");
            Assert.AreEqual(fakeContext.Added.Count + fakeContext.Modified.Count, 0, "Should be no changes due to errors");
        }

        [TestMethod]
        public void UpdateConfigurationSectionRecord_Valid_SuccessfulResultsAndRecordReturned()
        {

            //Will be the current value
            var singleRecord = ConfigurationValueSeeds.Books.BuildSingleConfig();
            singleRecord.Id = 111;

            var fakeContext = new MockInAnotherCastleContext().AddConfigurationValues(singleRecord);

            //New Value
            var fullRecord = ConfigurationValueSeeds.Books.BuildSingleConfig();
            fullRecord.Id = 111;


            var logger = new MockLogger();

            ConfigurationSaveResults expectedResults = new ConfigurationSaveResults()
            {
                Record = fullRecord,
                Errors = new List<ErrorMap>()
            };
            ConfigurationSaveResults results = null;
            using (ConfigurationStorage storage = new ConfigurationStorage(fakeContext, logger))
            {
                results = storage.UpdateConfigurationSection(fullRecord.Id, new ConfigurationNew()
                {
                    JSONSchema = fullRecord.JSONSchema,
                    Name = fullRecord.Name,
                    SystemName = fullRecord.SystemName,
                    XML = fullRecord.XML,
                    XMLSchema = fullRecord.XMLSchema
                });
            }
            ConfigurationValueSeeds.TrimWhiteSpace(results.Record);
            CollectionAssert.AreEquivalent(fakeContext.Modified, new object[] { results.Record });
            expectedResults.WithDeepEqual(results).IgnoreUnmatchedProperties().Assert();
        }
        [TestMethod]
        public void UpdateConfigurationSectionRecord_InvalidJson_ErrorResultsInvalidJson()
        {
            var fullRecord = ConfigurationValueSeeds.Books.BuildSingleConfig();
            fullRecord.Id = 111;
            //Json schema does not match the book schema
            fullRecord.JSONSchema = @"{
                ""$schema"": ""http://json-schema.org/draft-04/schema#"",
                        ""definitions"": {
                        },
                ""id"": ""http://example.com/example.json"",
                ""properties"": {
                            ""Invalid"": {
                                ""id"": ""/properties/Invalid"",
                        ""type"": ""string""
                            }
                        },
                ""type"": ""object"",
                ""required"": [""Invalid""]
            }";
            //Existing record
            var singleRecord = ConfigurationValueSeeds.Books.BuildSingleConfig();
            singleRecord.Id = fullRecord.Id;

            var fakeContext = new MockInAnotherCastleContext().AddConfigurationValues(singleRecord);

            var logger = new MockLogger();

            ConfigurationSaveResults results = null;
            using (ConfigurationStorage storage = new ConfigurationStorage(fakeContext, logger))
            {
                results = storage.UpdateConfigurationSection(singleRecord.Id, new ConfigurationNew()
                {
                    JSONSchema = fullRecord.JSONSchema,
                    Name = fullRecord.Name,
                    SystemName = fullRecord.SystemName,
                    XML = fullRecord.XML,
                    XMLSchema = fullRecord.XMLSchema
                });
            }
            Assert.AreEqual(results.Errors.Count, 1, "Should be one error");
            Assert.AreEqual(results.Errors[0].Code, DataStorageError.DoesNotMatchJsonSchema, "Json schema should not match");
            Assert.AreEqual(fakeContext.Added.Count + fakeContext.Modified.Count, 0, "Should be no changes due to errors");
        }
        [TestMethod]
        public void UpdateConfigurationSectionRecord_InvalidXml_ErrorResultsInvalidJson()
        {
            var fullRecord = ConfigurationValueSeeds.Books.BuildSingleConfig();
            fullRecord.Id = 111;
            //Xml schema does not match the book schema
            fullRecord.XMLSchema = @"<xs:schema attributeFormDefault=""unqualified"" elementFormDefault=""qualified"" xmlns:xs=""http://www.w3.org/2001/XMLSchema"">
                                      <xs:element name=""Invalid"" type=""xs:byte""/>
                                    </xs:schema>"; ;
            //Existing record
            var singleRecord = ConfigurationValueSeeds.Books.BuildSingleConfig();
            singleRecord.Id = fullRecord.Id;
            var fakeContext = new MockInAnotherCastleContext().AddConfigurationValues(singleRecord);

            var logger = new MockLogger();

            ConfigurationSaveResults results = null;
            using (ConfigurationStorage storage = new ConfigurationStorage(fakeContext, logger))
            {
                results = storage.UpdateConfigurationSection(fullRecord.Id, new ConfigurationNew()
                {
                    JSONSchema = fullRecord.JSONSchema,
                    Name = fullRecord.Name,
                    SystemName = fullRecord.SystemName,
                    XML = fullRecord.XML,
                    XMLSchema = fullRecord.XMLSchema
                });
            }
            Assert.AreEqual(results.Errors.Count, 1, "Should be one error");
            Assert.AreEqual(results.Errors[0].Code, DataStorageError.DoesNotMatchXMlSchema, "Xml schema should not match");
            Assert.AreEqual(fakeContext.Added.Count + fakeContext.Modified.Count, 0, "Should be no changes due to errors");

        }
        [TestMethod]
        public void UpdateConfigurationSectionRecord_MissingFields_XML_ValidationError()
        {
            var fullRecord = ConfigurationValueSeeds.Books.BuildSingleConfig();
            fullRecord.Id = 112;
            //Existing record
            var singleRecord = ConfigurationValueSeeds.Books.BuildSingleConfig();
            singleRecord.Id = fullRecord.Id;
            var fakeContext = new MockInAnotherCastleContext().AddConfigurationValues(singleRecord);
            var logger = new MockLogger();

            ConfigurationSaveResults results = null;
            using (ConfigurationStorage storage = new ConfigurationStorage(fakeContext, logger))
            {
                results = storage.UpdateConfigurationSection(fullRecord.Id, new ConfigurationNew()
                {
                    JSONSchema = fullRecord.JSONSchema,
                    Name = fullRecord.Name,
                    SystemName = fullRecord.SystemName,
                    XML = "",
                    XMLSchema = fullRecord.XMLSchema
                });
            }
            Assert.AreEqual(results.Errors.Count, 1, "Should be one error");
            Assert.AreEqual(results.Errors[0].Code, DataStorageError.DoesNotMatchXMlSchema, "Xml schema should not match");
            Assert.AreEqual(fakeContext.Added.Count + fakeContext.Modified.Count, 0, "Should be no changes due to errors");
        }

        [TestMethod]
        public void UpdateConfigurationSectionRecord_MissingFields_XMLSchema_ValidationError()
        {
            var fullRecord = ConfigurationValueSeeds.Books.BuildSingleConfig();
            fullRecord.Id = 113;
            //Existing record
            var singleRecord = ConfigurationValueSeeds.Books.BuildSingleConfig();
            singleRecord.Id = fullRecord.Id;
            var fakeContext = new MockInAnotherCastleContext().AddConfigurationValues(singleRecord);
            var logger = new MockLogger();

            ConfigurationSaveResults results = null;
            using (ConfigurationStorage storage = new ConfigurationStorage(fakeContext, logger))
            {
                results = storage.UpdateConfigurationSection(fullRecord.Id, new ConfigurationNew()
                {
                    JSONSchema = fullRecord.JSONSchema,
                    Name = fullRecord.Name,
                    SystemName = fullRecord.SystemName,
                    XML = fullRecord.XML,
                    XMLSchema = ""
                });
            }
            Assert.AreEqual(results.Errors.Count, 1, "Should be one error");
            Assert.AreEqual(results.Errors[0].Code, DataStorageError.InvalidXmlSchema, "Xml schema is invalid");
            Assert.AreEqual(fakeContext.Added.Count + fakeContext.Modified.Count, 0, "Should be no changes due to errors");
        }
        [TestMethod]
        public void UpdateConfigurationSectionRecord_MissingFields_Name_ValidationError()
        {
            var fullRecord = ConfigurationValueSeeds.Books.BuildSingleConfig();
            fullRecord.Id = 114;
            //Existing record
            var singleRecord = ConfigurationValueSeeds.Books.BuildSingleConfig();
            singleRecord.Id = fullRecord.Id;
            var fakeContext = new MockInAnotherCastleContext().AddConfigurationValues(singleRecord); new MockLogger();

            var logger = new MockLogger();
            ConfigurationSaveResults results = null;
            using (ConfigurationStorage storage = new ConfigurationStorage(fakeContext, logger))
            {
                results = storage.UpdateConfigurationSection(fullRecord.Id, new ConfigurationNew()
                {
                    JSONSchema = fullRecord.JSONSchema,
                    Name = "",
                    SystemName = fullRecord.SystemName,
                    XML = fullRecord.XML,
                    XMLSchema = fullRecord.XMLSchema
                });
            }
            Assert.AreEqual(results.Errors.Count, 1, "Should be one error");
            Assert.AreEqual(results.Errors[0].Code, DataStorageError.MissingNameField, "Name field is required");
            Assert.AreEqual(fakeContext.Added.Count + fakeContext.Modified.Count, 0, "Should be no changes due to errors");
        }
        [TestMethod]
        public void UpdateConfigurationSectionRecord_MissingFields_JsonSchema_ValidationError()
        {
            var fullRecord = ConfigurationValueSeeds.Books.BuildSingleConfig();
            fullRecord.Id = 115;
            //Existing record
            var singleRecord = ConfigurationValueSeeds.Books.BuildSingleConfig();
            singleRecord.Id = fullRecord.Id;
            var fakeContext = new MockInAnotherCastleContext().AddConfigurationValues(singleRecord);
            var logger = new MockLogger();

            ConfigurationSaveResults results = null;
            using (ConfigurationStorage storage = new ConfigurationStorage(fakeContext, logger))
            {
                results = storage.UpdateConfigurationSection(fullRecord.Id, new ConfigurationNew()
                {
                    JSONSchema = "",
                    Name = fullRecord.Name,
                    SystemName = fullRecord.SystemName,
                    XML = fullRecord.XML,
                    XMLSchema = fullRecord.XMLSchema
                });
            }
            Assert.AreEqual(results.Errors.Count, 1, "Should be one error");
            Assert.AreEqual(results.Errors[0].Code, DataStorageError.DoesNotMatchJsonSchema, "Json schema does not match");
            Assert.AreEqual(fakeContext.Added.Count + fakeContext.Modified.Count, 0, "Should be no changes due to errors");
        }
        [TestMethod]
        public void UpdateConfigurationSectionRecord_NameAndSystemExist_ErrorResultsAlreadyExists()
        {
            var fullRecord = ConfigurationValueSeeds.Books.BuildSingleConfig();
            fullRecord.Id = 222;
            fullRecord.Name = "2";
            fullRecord.SystemName = "2";
            //Note record added
            var singleRecord = ConfigurationValueSeeds.Books.BuildSingleConfig();
            singleRecord.Id = 111;
            singleRecord.Name = "1";
            singleRecord.SystemName = "1";
            var fakeContext = new MockInAnotherCastleContext().AddConfigurationValues(fullRecord, singleRecord);
            var logger = new MockLogger();

            ConfigurationSaveResults results = null;
            using (ConfigurationStorage storage = new ConfigurationStorage(fakeContext, logger))
            {
                results = storage.UpdateConfigurationSection(fullRecord.Id, new ConfigurationNew()
                {
                    JSONSchema = fullRecord.JSONSchema,
                    ///Note change the name to another existing record single
                    Name = singleRecord.Name,
                    SystemName = singleRecord.SystemName,
                    XML = fullRecord.XML,
                    XMLSchema = fullRecord.XMLSchema
                });
            }
            Assert.AreEqual(results.Errors.Count, 1, "Should be one error");
            Assert.AreEqual(results.Errors[0].Code, DataStorageError.AlreadyExists, "Record should already exist as it was added to the test context");
            Assert.AreEqual(fakeContext.Added.Count + fakeContext.Modified.Count, 0, "Should be no changes due to errors");
        }

        [TestMethod]
        public void UpdateConfigurationSectionRecord_IDNotExist_ErrorResultsRecordNotFound()
        {
            var fullRecord = ConfigurationValueSeeds.Books.BuildSingleConfig();
            fullRecord.Id = 110;
            //Note fake context is empty no records exist
            var fakeContext = new MockInAnotherCastleContext();
            var logger = new MockLogger();

            ConfigurationSaveResults results = null;
            using (ConfigurationStorage storage = new ConfigurationStorage(fakeContext, logger))
            {
                results = storage.UpdateConfigurationSection(fullRecord.Id, new ConfigurationNew()
                {
                    JSONSchema = fullRecord.JSONSchema,
                    Name = fullRecord.Name,
                    SystemName = fullRecord.SystemName,
                    XML = fullRecord.XML,
                    XMLSchema = fullRecord.XMLSchema
                });
            }
            Assert.AreEqual(results.Errors.Count, 1, "Should be one error");
            Assert.AreEqual(results.Errors[0].Code, DataStorageError.RecordNotFound, "Record under the given id was not added to the data store before updating");
            Assert.AreEqual(fakeContext.Added.Count + fakeContext.Modified.Count, 0, "Should be no changes due to errors");
        }

        [TestMethod]
        public void UpdateConfigurationSectionRecord_WithTagsThatExist_RecordAddedTagsMapped()
        {
            var existingTag1 = new Tag() { Id = 1, Value = "Test1" };
            var existingTag2 = new Tag() { Id = 2, Value = "Test2" };

            var fullRecord = ConfigurationValueSeeds.Books.BuildSingleConfig();
            fullRecord.Id = 2123123;
            //Note record added
            var fakeContext = new MockInAnotherCastleContext().AddTags(existingTag1, existingTag2).AddConfigurationValues(fullRecord);
            var logger = new MockLogger();


            ConfigurationSaveResults results = null;
            using (ConfigurationStorage storage = new ConfigurationStorage(fakeContext, logger))
            {
                results = storage.UpdateConfigurationSection(fullRecord.Id, new ConfigurationNew()
                {
                    JSONSchema = fullRecord.JSONSchema,
                    Name = fullRecord.Name,
                    SystemName = fullRecord.SystemName,
                    XML = fullRecord.XML,
                    XMLSchema = fullRecord.XMLSchema,
                    Tags = new List<Tag>()
                    {
                        existingTag1, existingTag2
                    }
                });
            }

            ConfigurationValueSeeds.TrimWhiteSpace(results.Record);
            Assert.IsTrue(results.Successful, "should be a successful save");

            fullRecord.WithDeepEqual(results.Record).IgnoreSourceProperty(c => c.Tags).IgnoreUnmatchedProperties().Assert();

            fullRecord.Tags.OrderBy(c => c.Id).WithDeepEqual(results.Record.Tags.OrderBy(c => c.Id)).Assert();
            CollectionAssert.AreEquivalent(fakeContext.Modified, new object[] { results.Record }, "The record should be modified");
            CollectionAssert.AreEquivalent(fakeContext.Added, new object[] { }, "Nothing should be added");


            Assert.AreEqual(fakeContext.ConfigTagAddTransactions.First().Value.Count(), 2, "Contains two adds for tags");
            Assert.IsTrue(fakeContext.ConfigTagAddTransactions.First().Value
                    .Any(t => t.Id == 1)
            , "Tag to Config Mapping should contain first tag");
            Assert.IsTrue(fakeContext.ConfigTagAddTransactions.First().Value
                    .Any(t => t.Id == 2)
            , "Tag to Config Mapping should contain second tag");


        }
        [TestMethod]
        public void UpdateConfigurationSectionRecord_WithNewTags_RecordAddedTagsAddedAndMapped()
        {
            var newTag1 = new Tag() { Id = 0, Value = "Test1" };
            var newTag2 = new Tag() { Id = 0, Value = "Test2" };

            var fullRecord = ConfigurationValueSeeds.Books.BuildSingleConfig();
            fullRecord.Id = 123124;

            //Note record added
            var fakeContext = new MockInAnotherCastleContext().AddConfigurationValues(fullRecord);
            var logger = new MockLogger();


            ConfigurationSaveResults results = null;
            using (ConfigurationStorage storage = new ConfigurationStorage(fakeContext, logger))
            {
                results = storage.UpdateConfigurationSection(fullRecord.Id, new ConfigurationNew()
                {
                    JSONSchema = fullRecord.JSONSchema,
                    Name = fullRecord.Name,
                    SystemName = fullRecord.SystemName,
                    XML = fullRecord.XML,
                    XMLSchema = fullRecord.XMLSchema,
                    Tags = new List<Tag>()
                    {
                        newTag1, newTag2
                    }
                });
            }


            ConfigurationValueSeeds.TrimWhiteSpace(results.Record);
            Assert.IsTrue(results.Successful, "should be a successful save");

            fullRecord.WithDeepEqual(results.Record).IgnoreSourceProperty(c => c.Tags).IgnoreUnmatchedProperties().Assert();

            fullRecord.Tags.OrderBy(c => c.Id).WithDeepEqual(results.Record.Tags.OrderBy(c => c.Id)).Assert();

            CollectionAssert.AreEquivalent(fakeContext.Added, new object[] { newTag1, newTag2 }, "The tags should be added");
            CollectionAssert.AreEquivalent(fakeContext.Modified, new object[] { results.Record }, "The record as well as the tags should be modified");


            Assert.AreEqual(fakeContext.ConfigTagAddTransactions.First().Value.Count(), 2, "Contains two adds for tags");
            Assert.IsTrue(fakeContext.ConfigTagAddTransactions.First().Value
                    .Any(t => t.Value == "Test1")
            , "Should contain first tag");
            Assert.IsTrue(fakeContext.ConfigTagAddTransactions.First().Value
                    .Any(t => t.Value == "Test2")
            , "Should contain second tag");
        }
        [TestMethod]
        public void UpdateConfigurationSectionRecord_WithTagsThatExistAndNewTags_RecordAddedTagsMapped()
        {
            var newTag1 = new Tag() { Id = 0, Value = "Test1" };
            var existingTag2 = new Tag() { Id = 2, Value = "Test2" };

            var fullRecord = ConfigurationValueSeeds.Books.BuildSingleConfig();
            fullRecord.Id = 2123123;
            //Note record added
            var fakeContext = new MockInAnotherCastleContext().AddTags(existingTag2).AddConfigurationValues(fullRecord);
            var logger = new MockLogger();


            ConfigurationSaveResults results = null;
            using (ConfigurationStorage storage = new ConfigurationStorage(fakeContext, logger))
            {
                results = storage.UpdateConfigurationSection(fullRecord.Id, new ConfigurationNew()
                {
                    JSONSchema = fullRecord.JSONSchema,
                    Name = fullRecord.Name,
                    SystemName = fullRecord.SystemName,
                    XML = fullRecord.XML,
                    XMLSchema = fullRecord.XMLSchema,
                    Tags = new List<Tag>()
                    {
                        newTag1, existingTag2
                    }
                });
            }

            ConfigurationValueSeeds.TrimWhiteSpace(results.Record);
            Assert.IsTrue(results.Successful, "should be a successful save");

            fullRecord.WithDeepEqual(results.Record).IgnoreSourceProperty(c => c.Tags).IgnoreUnmatchedProperties().Assert();

            fullRecord.Tags.OrderBy(c => c.Id).WithDeepEqual(results.Record.Tags.OrderBy(c => c.Id)).Assert();
            CollectionAssert.AreEquivalent(fakeContext.Modified, new object[] { results.Record }, "The record should be modified");
            CollectionAssert.AreEquivalent(fakeContext.Added, new object[] { newTag1 }, "Only the new tag should be added");


            Assert.AreEqual(fakeContext.ConfigTagAddTransactions.First().Value.Count(), 2, "Contains two adds for tags");
            Assert.IsTrue(fakeContext.ConfigTagAddTransactions.First().Value
                    .Any(t => t.Value == newTag1.Value)
            , "Tag to Config Mapping should contain first tag");
            Assert.IsTrue(fakeContext.ConfigTagAddTransactions.First().Value
                    .Any(t => t.Id == 2)
            , "Tag to Config Mapping should contain second tag");


        }

        [TestMethod]
        public void UpdateConfigurationSectionRecord_TagValueExistId0_RecordAddedTagsMapped()
        {
            //Existing
            var existingSingleRecord = ConfigurationValueSeeds.Books.BuildSingleConfig();
            existingSingleRecord.Id = 11234;


            var newTag1 = new Tag() { Id = 0, Value = "Test1" };
            //Note same value, except the new value has ID 0, the store will resolve the ID
            var existingTag2 = new Tag() { Id = 12323, Value = "Test1" };

            var expectedFullRecord = ConfigurationValueSeeds.Books.BuildSingleConfig();
            expectedFullRecord.Id = existingSingleRecord.Id;
            //Note this is the expected tag
            expectedFullRecord.Tags.Add(existingTag2);
            //Note record added
            var fakeContext = new MockInAnotherCastleContext().AddTags(existingTag2).AddConfigurationValues(existingSingleRecord);
            var logger = new MockLogger();


            ConfigurationSaveResults results = null;
            using (ConfigurationStorage storage = new ConfigurationStorage(fakeContext, logger))
            {
                results = storage.UpdateConfigurationSection(existingSingleRecord.Id, new ConfigurationNew()
                {
                    JSONSchema = expectedFullRecord.JSONSchema,
                    Name = expectedFullRecord.Name,
                    SystemName = expectedFullRecord.SystemName,
                    XML = expectedFullRecord.XML,
                    XMLSchema = expectedFullRecord.XMLSchema,
                    Tags = new List<Tag>()
                    {
                        //new tag with id 0
                        newTag1
                    }
                });
            }

            ConfigurationValueSeeds.TrimWhiteSpace(results.Record);
            Assert.IsTrue(results.Successful, "should be a successful save");

            expectedFullRecord.WithDeepEqual(results.Record).IgnoreSourceProperty(c => c.Tags).IgnoreUnmatchedProperties().Assert();

            expectedFullRecord.Tags.OrderBy(c => c.Id).WithDeepEqual(results.Record.Tags.OrderBy(c => c.Id)).Assert();
            CollectionAssert.AreEquivalent(fakeContext.Added, new object[] { }, "No records should be added");
            CollectionAssert.AreEquivalent(fakeContext.Modified, new object[] { results.Record }, "Only new record added, no tags should be changed");


            Assert.AreEqual(fakeContext.ConfigTagAddTransactions.First().Value.Count(), 1, "Contains one add for tags");

            Assert.IsTrue(fakeContext.ConfigTagAddTransactions.First().Value
                    .Any(t => t.Id == existingTag2.Id)
            , "Tag to Config Mapping should contain second tag");


        }
        [TestMethod]
        public void UpdateConfigurationSectionRecord_RemoveTag_RecordRemovedTagsMapped()
        {
            var existingSingleRecord = ConfigurationValueSeeds.Books.BuildSingleConfig();
            existingSingleRecord.Id = 11234;


            var existingTag1 = new Tag() { Id = 212323, Value = "Test1" };
            var existingTag2 = new Tag() { Id = 12323, Value = "Test2" };
            existingTag1.ConfigurationValues.Add(existingSingleRecord);
            existingTag2.ConfigurationValues.Add(existingSingleRecord);
            existingSingleRecord.Tags.Add(existingTag1);
            existingSingleRecord.Tags.Add(existingTag2);


            var expectedFullRecord = ConfigurationValueSeeds.Books.BuildSingleConfig();
            expectedFullRecord.Id = existingSingleRecord.Id;
            //Note this is the expected tag
            expectedFullRecord.Tags.Add(existingTag2);
            //Note record added
            var fakeContext = new MockInAnotherCastleContext().AddTags(existingTag1, existingTag2).AddConfigurationValues(existingSingleRecord);
            var logger = new MockLogger();


            ConfigurationSaveResults results = null;
            using (ConfigurationStorage storage = new ConfigurationStorage(fakeContext, logger))
            {
                results = storage.UpdateConfigurationSection(existingSingleRecord.Id, new ConfigurationNew()
                {
                    JSONSchema = expectedFullRecord.JSONSchema,
                    Name = expectedFullRecord.Name,
                    SystemName = expectedFullRecord.SystemName,
                    XML = expectedFullRecord.XML,
                    XMLSchema = expectedFullRecord.XMLSchema,
                    Tags = new List<Tag>()
                    {
                        //Note the absence of 1
                        existingTag2
                    }
                });
            }

            ConfigurationValueSeeds.TrimWhiteSpace(results.Record);
            Assert.IsTrue(results.Successful, "should be a successful save");

            expectedFullRecord.WithDeepEqual(results.Record).IgnoreSourceProperty(c => c.Tags).IgnoreUnmatchedProperties().Assert();

            expectedFullRecord.Tags.OrderBy(c => c.Id).WithDeepEqual(results.Record.Tags.OrderBy(c => c.Id)).Assert();
            CollectionAssert.AreEquivalent(fakeContext.Added, new object[] { }, "No records should be added");
            CollectionAssert.AreEquivalent(fakeContext.Modified, new object[] { results.Record }, "Only new record added, no tags should be changed");


            Assert.AreEqual(fakeContext.ConfigTagAddTransactions.First().Value.Count(), 0, "Contains no adds for tags");
            Assert.AreEqual(fakeContext.ConfigTagRemoveTransactions.First().Value.Count(), 1, "Contains one removes for tags");
            Assert.IsTrue(fakeContext.ConfigTagRemoveTransactions.First().Value
                    .Any(t => t.Id == existingTag1.Id)
            , "Tag to Config Mapping should contain first tag");
        }

        [TestMethod]
        public void UpdateConfigurationSectionRecord_NoChangeToTags_RecordUpdaedTagsUnchanged()
        {
            var existingSingleRecord = ConfigurationValueSeeds.Books.BuildSingleConfig();
            existingSingleRecord.Id = 11234;


            var existingTag1 = new Tag() { Id = 212323, Value = "Test1" };
            var existingTag2 = new Tag() { Id = 12323, Value = "Test2" };
            existingTag1.ConfigurationValues.Add(existingSingleRecord);
            existingTag2.ConfigurationValues.Add(existingSingleRecord);
            existingSingleRecord.Tags.Add(existingTag1);
            existingSingleRecord.Tags.Add(existingTag2);


            var expectedFullRecord = ConfigurationValueSeeds.Books.BuildSingleConfig();
            expectedFullRecord.Id = existingSingleRecord.Id;
            //Note this is the expected tag
            expectedFullRecord.Tags.Add(existingTag1);
            expectedFullRecord.Tags.Add(existingTag2);
            //Note record added
            var fakeContext = new MockInAnotherCastleContext().AddTags(existingTag1, existingTag2).AddConfigurationValues(existingSingleRecord);
            var logger = new MockLogger();


            ConfigurationSaveResults results = null;
            using (ConfigurationStorage storage = new ConfigurationStorage(fakeContext, logger))
            {
                results = storage.UpdateConfigurationSection(existingSingleRecord.Id, new ConfigurationNew()
                {
                    JSONSchema = expectedFullRecord.JSONSchema,
                    Name = expectedFullRecord.Name,
                    SystemName = expectedFullRecord.SystemName,
                    XML = expectedFullRecord.XML,
                    XMLSchema = expectedFullRecord.XMLSchema,
                    Tags = new List<Tag>()
                    {
                        //Same tags therefore should be no change
                        existingTag1, existingTag2
                    }
                });
            }

            ConfigurationValueSeeds.TrimWhiteSpace(results.Record);
            Assert.IsTrue(results.Successful, "should be a successful save");

            expectedFullRecord.WithDeepEqual(results.Record).IgnoreSourceProperty(c => c.Tags).IgnoreUnmatchedProperties().Assert();

            expectedFullRecord.Tags.OrderBy(c => c.Id).WithDeepEqual(results.Record.Tags.OrderBy(c => c.Id)).Assert();
            CollectionAssert.AreEquivalent(fakeContext.Added, new object[] { }, "No records should be added");
            CollectionAssert.AreEquivalent(fakeContext.Modified, new object[] { results.Record }, "Only new record added, no tags should be changed");


            Assert.AreEqual(fakeContext.ConfigTagAddTransactions.First().Value.Count(), 0, "Contains no adds for tags");
            Assert.AreEqual(fakeContext.ConfigTagRemoveTransactions.First().Value.Count(), 0, "Contains no removes for tags");

        }


        [TestMethod]
        public void UpdateConfigurationSectionRecord_InvalidTagId_RecordAddedTagsMapped()
        {
            //Existing
            var existingSingleRecord = ConfigurationValueSeeds.Books.BuildSingleConfig();
            existingSingleRecord.Id = 11234;

            //Note it has an Id and hasnt been added to the context
            var invalidTag = new Tag() { Id = 123242, Value = "Test1" };

            var expectedFullRecord = ConfigurationValueSeeds.Books.BuildSingleConfig();
            expectedFullRecord.Id = existingSingleRecord.Id;
            //Note this is the expected tag
            expectedFullRecord.Tags.Add(invalidTag);
            //Note record added
            var fakeContext = new MockInAnotherCastleContext().AddConfigurationValues(existingSingleRecord);
            var logger = new MockLogger();


            ConfigurationSaveResults results = null;
            using (ConfigurationStorage storage = new ConfigurationStorage(fakeContext, logger))
            {
                results = storage.UpdateConfigurationSection(existingSingleRecord.Id, new ConfigurationNew()
                {
                    JSONSchema = expectedFullRecord.JSONSchema,
                    Name = expectedFullRecord.Name,
                    SystemName = expectedFullRecord.SystemName,
                    XML = expectedFullRecord.XML,
                    XMLSchema = expectedFullRecord.XMLSchema,
                    Tags = new List<Tag>()
                    {
                        //new tag with id 0
                        invalidTag
                    }
                });
            }

            Assert.AreEqual(results.Errors.Count, 1, "Should be one error");
            Assert.AreEqual(results.Errors[0].Code, DataStorageError.RecordNotFound, "Invalid tag Id could not be found");
            Assert.AreEqual(fakeContext.Added.Count + fakeContext.Modified.Count, 0, "Should be no changes due to errors");
        }


        //Get Tags
    }
}
