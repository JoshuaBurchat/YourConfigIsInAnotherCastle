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
        public void GetRecordBySystemName_Success_RecordsReturned()
        {

            var customerFull = ConfigurationValueSeeds.Customers.BuildFullConfig();
            customerFull.SystemName = "Test 1";
            var customerSingle = ConfigurationValueSeeds.Customers.BuildSingleConfig();
            customerSingle.SystemName = "Test 1";
            var bookFull = ConfigurationValueSeeds.Books.BuildFullConfig();
            var bookSingle = ConfigurationValueSeeds.Books.BuildSingleConfig();


            var fakeContext = new MockInAnotherCastleContext().AddConfigurationValues(customerFull, customerSingle, bookFull, bookSingle);

            var logger = new MockLogger();

            ConfigurationValue[] results = null;
            using (ConfigurationStorage storage = new ConfigurationStorage(fakeContext, logger))
            {

                results = storage.GetConfigurationSections("Test 1").ToArray();
            }
            Assert.AreEqual(results.Length, 2, "Should only have the two customer records");
            CollectionAssert.Contains(results, customerFull);
            CollectionAssert.Contains(results, customerSingle);
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
                wasRemoved = fakeContext.Deleted.Contains(customerFull);
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
                wasRemoved = fakeContext.Deleted.Cast<ConfigurationValue>().Any(d => d.Id == customerFull.Id);
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

        //TODO save with tags
        //TODO save with new tags

        //TODO Tags add before 
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
        }

    }
}
