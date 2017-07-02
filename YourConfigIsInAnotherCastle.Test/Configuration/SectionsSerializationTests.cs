using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YourConfigIsInAnotherCastle.Configuration;
using System.Xml;
using System.IO;
using DeepEqual.Syntax;
using YourConfigIsInAnotherCastle.Test.Mocks;
using System.Configuration;

namespace YourConfigIsInAnotherCastle.Test.Configuration
{
    [TestClass]
    public class SectionsSerializationTests
    {

        [TestMethod]
        public void LoadInAnotherCastleConfigSection_ValidXml_ProperObjectReturned()
        {
            InAnotherCastleConfigSection section = new InAnotherCastleConfigSection();
            string xml = @"<InAnotherCastleConfigSection ServiceProviderType=""TestType""></InAnotherCastleConfigSection>";
            using (StringReader stringReader = new StringReader(xml))
            using (XmlReader reader = XmlReader.Create(stringReader))
            {
                section.ExposedDeserializeSection(reader);
            }
            section.WithDeepEqual(new MockInAnotherCastleConfigurationSection()
            {
                ServiceProviderTypeName = "TestType"
            }).IgnoreUnmatchedProperties().Assert();

        }

        [TestMethod]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        public void LoadInAnotherCastleConfigSection_MissingRequiredFieldsXml_ConfigurationExceptionThrown()
        {

            InAnotherCastleConfigSection section = new InAnotherCastleConfigSection();
            //missing ServiceProviderType
            string xml = @"<InAnotherCastleConfigSection></InAnotherCastleConfigSection>";
            using (StringReader stringReader = new StringReader(xml))
            using (XmlReader reader = XmlReader.Create(stringReader))
            {
                section.ExposedDeserializeSection(reader);
            }

        }
        [TestMethod]
        #region Redirects
        public void LoadInAnotherCastleRedirectConfigSection_ValidXml_ProperObjectReturned()
        {

            var section = new InAnotherCastleConfigSectionRedirect();
            string xml = @"<InAnotherCastleConfigSectionRedirect Mode=""Poco"" Name=""TestName"" SystemName=""TestSystem"" Type=""TestType"" CacheDurationInMinutes=""23"" ></InAnotherCastleConfigSectionRedirect>";
            using (StringReader stringReader = new StringReader(xml))
            using (XmlReader reader = XmlReader.Create(stringReader))
            {
                section.ExposedDeserializeSection(reader);
            }
            new MockRedirectIdentifier()
            {
                CacheDurationInMinutes = 23,
                Name = "TestName",
                Type = "TestType",
                SystemName = "TestSystem",
                Mode = Mode.Poco,
                 PocoBody = new PocoBody()
            }.WithDeepEqual(  section).IgnoreUnmatchedProperties().Assert();

        }
        [TestMethod]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        public void LoadInAnotherCastleRedirectConfigSection_MissingRequiredFieldsXml_Name_ConfigurationExceptionThrown()
        {
            var section = new InAnotherCastleConfigSectionRedirect();
            //missing Name
            string xml = @"<InAnotherCastleConfigSectionRedirect Mode=""Poco""  SystemName=""SystemNameTest"" Type=""TestType"" CacheDurationInMinutes=""23"" ></InAnotherCastleConfigSectionRedirect>";
            using (StringReader stringReader = new StringReader(xml))
            using (XmlReader reader = XmlReader.Create(stringReader))
            {
                section.ExposedDeserializeSection(reader);
            }
        
        }
        [TestMethod]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        public void LoadInAnotherCastleRedirectConfigSection_MissingRequiredFieldsXml_Type_ConfigurationExceptionThrown()
        {
            var section = new InAnotherCastleConfigSectionRedirect();
            //missing Type
            string xml = @"<InAnotherCastleConfigSectionRedirect Mode=""Poco"" Name=""TestName""  SystemName=""SystemNameTest"" CacheDurationInMinutes=""23"" ></InAnotherCastleConfigSectionRedirect>";
            using (StringReader stringReader = new StringReader(xml))
            using (XmlReader reader = XmlReader.Create(stringReader))
            {
                section.ExposedDeserializeSection(reader);
            }

        }
        [TestMethod]
        public void LoadInAnotherCastleRedirectConfigSection_MissingRequiredFieldsXml_CacheDuration_Valid()
        {
            var section = new InAnotherCastleConfigSectionRedirect();
            //missing CacheDurationInMinutes
            string xml = @"<InAnotherCastleConfigSectionRedirect Mode=""Poco"" Name=""TestName"" SystemName=""SystemNameTest"" Type=""TestType"" ></InAnotherCastleConfigSectionRedirect>";
            using (StringReader stringReader = new StringReader(xml))
            using (XmlReader reader = XmlReader.Create(stringReader))
            {
                section.ExposedDeserializeSection(reader);
            }
            section.WithDeepEqual(new MockRedirectIdentifier()
            {
                //By default will be assigned 1
                CacheDurationInMinutes = 1,
                Name = "TestName",
                Type = "TestType",
                SystemName = "SystemNameTest",
                Mode = Mode.Poco,
                 PocoBody = new PocoBody()
            }).IgnoreUnmatchedProperties().Assert();
        }
        [TestMethod]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        public void LoadInAnotherCastleRedirectConfigSection_MissingRequiredFieldsXml_Mode_ConfigurationExceptionThrown()
        {
            var section = new InAnotherCastleConfigSectionRedirect();
            //missing CacheDurationInMinutes
            string xml = @"<InAnotherCastleConfigSectionRedirect  Name=""TestName"" SystemName=""SystemNameTest"" Type=""TestType"" CacheDurationInMinutes=""23"" ></InAnotherCastleConfigSectionRedirect>";
            using (StringReader stringReader = new StringReader(xml))
            using (XmlReader reader = XmlReader.Create(stringReader))
            {
                section.ExposedDeserializeSection(reader);
            }

        }
        [TestMethod]
        public void LoadInAnotherCastleRedirectConfigSection_PocoXMLPresent()
        {
            var section = new InAnotherCastleConfigSectionRedirect();
            string xml = @"<InAnotherCastleConfigSectionRedirect Mode=""Poco"" Name=""TestName"" SystemName=""TestSystem"" Type=""TestType"" CacheDurationInMinutes=""23"" >
                          <PocoBody>  <![CDATA[<TestTag></TestTag>]]></PocoBody>
                           </InAnotherCastleConfigSectionRedirect>";
            using (StringReader stringReader = new StringReader(xml))
            using (XmlReader reader = XmlReader.Create(stringReader))
            {
                section.ExposedDeserializeSection(reader);
            }
            section.WithDeepEqual(new MockRedirectIdentifier()
            {
                CacheDurationInMinutes = 23,
                Name = "TestName",
                Type = "TestType",
                SystemName = "TestSystem",
                Mode = Mode.Poco,
                PocoBody = new PocoBody() { Value = "<TestTag></TestTag>" }
            }).IgnoreUnmatchedProperties().Assert();
        }
        #endregion Redirects

    }
}
