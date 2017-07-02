using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YourConfigIsInAnotherCastle.Parser;
using YourConfigIsInAnotherCastle.Test.Mocks;
using DeepEqual.Syntax;
using System.Configuration;
using System.Xml.Serialization;

namespace YourConfigIsInAnotherCastle.Test.Parser
{
    [TestClass]
    public class DefaultInAnotherCastleParserTests
    {

        /// <summary>
        /// Has required fields in order to have invalid response
        /// </summary>
        public class ExampleConfigurationSection : ConfigurationSection
        {

            [ConfigurationProperty("Name", DefaultValue = "", IsRequired = true)]
            public string Name
            {
                get { return (string)this["Name"]; }
                set { this["Name"] = value; }
            }

            [ConfigurationProperty("Value", DefaultValue = "", IsRequired = true)]
            public string Value
            {
                get { return (string)this["Value"]; }
                set { this["Value"] = value; }
            }
        }
        public class ExampleConfigurationPoco 
        {
            [XmlAttribute("Name")]
            public string Name { get; set; }
            [XmlAttribute("Value")]
            public string Value { get; set; }
        }
        /// <summary>
        /// The tests for ParseRedirectDetails are simply valid and invalid, this function simply calls into the ExposedDeserializeSection
        /// function within and more indepeth test can be found under SectionsSerializationTests
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        public void DefaultParser_RedirectDetails_InvalidXml()
        {
            DefaultInAnotherCastleParser parser = new DefaultInAnotherCastleParser();
            //missing CacheDurationInMinutes
            string xml = @"<MissingAllRequiredAttributes></MissingAllRequiredAttributes>";
            var results = parser.ParseRedirectDetails(xml);

        }
        /// <summary>
        /// The tests for ParseRedirectDetails are simply valid and invalid, this function simply calls into the ExposedDeserializeSection
        /// function within and more indepeth test can be found under SectionsSerializationTests
        /// </summary>
        [TestMethod]
        public void DefaultParserT_RedirectDetails_ValidXml()
        {
            DefaultInAnotherCastleParser parser = new DefaultInAnotherCastleParser();
            //missing CacheDurationInMinutes
            string xml = @"<InAnotherCastleConfigSectionRedirect Mode=""Standard"" Name=""TestName"" SystemName=""SystemNameTest"" Type=""TestType""  CacheDurationInMinutes=""100"" ></InAnotherCastleConfigSectionRedirect>";
            var results = parser.ParseRedirectDetails(xml);
            results.WithDeepEqual(new MockRedirectIdentifier()
            {
                CacheDurationInMinutes = 100,
                Name = "TestName",
                Type = "TestType",
                SystemName = "SystemNameTest",
                Mode = Mode.Standard,
                PocoBody = new PocoBody()
            }).IgnoreUnmatchedProperties().Assert();
        }

        [TestMethod]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        public void DefaultParser_StandardConfigSection_InvalidXmlForConfig_ConfigurationExceptionThrown()
        {
            DefaultInAnotherCastleParser parser = new DefaultInAnotherCastleParser();
            parser.StandardConfigSectionParse("<TestInvalid></TestInvalid>", typeof(ExampleConfigurationSection));
        }
        [TestMethod]
        public void DefaultParser_StandardConfigSection_ValidXmlForConfig_ExpectedInstanceReturned()
        {
            DefaultInAnotherCastleParser parser = new DefaultInAnotherCastleParser();
            var results = parser.StandardConfigSectionParse(@"<TestInvalid Name=""TestName"" Value=""TestValue""></TestInvalid>", typeof(ExampleConfigurationSection));

            results.WithDeepEqual(new
            {
                Name = "TestName",
                Value = "TestValue"
            }).IgnoreUnmatchedProperties().Assert();
        }

        [TestMethod]
        public void DefaultParser_PocoConfig_ValidXml_ExpectedInstanceReturned()
        {
            DefaultInAnotherCastleParser parser = new DefaultInAnotherCastleParser();
            var results = parser.POCOConfigSectionParse(@"<ExampleConfigurationPoco Name=""TestName"" Value=""TestValue""></ExampleConfigurationPoco>", typeof(ExampleConfigurationPoco));
            results.WithDeepEqual(new ExampleConfigurationPoco
            {
                Name = "TestName",
                Value = "TestValue"
            }).IgnoreUnmatchedProperties().Assert();
        }
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void DefaultParser_PocoConfig_InvalidXml_ConfigurationExceptionThrown()
        {
            DefaultInAnotherCastleParser parser = new DefaultInAnotherCastleParser();
            var results = parser.POCOConfigSectionParse(@"<TestInvalid SomeUnknownProperty=""TestName"" SomeUnknownProperty2=""TestValue""></TestInvalid>", typeof(ExampleConfigurationPoco));
           
        }


    }
}
