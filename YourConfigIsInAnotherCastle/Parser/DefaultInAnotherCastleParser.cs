using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using YourConfigIsInAnotherCastle.Models;

namespace YourConfigIsInAnotherCastle.Parser
{

    public class DefaultInAnotherCastleParser : IInAnotherCastleParser
    {

        public IRedirectIdentifier ParseRedirectDetails(string xml)
        {
            //TODO get exceptions and wrap
            using (StringReader stringReader = new StringReader(xml.Trim()))
            using (XmlReader reader = XmlReader.Create(stringReader, new XmlReaderSettings() { CloseInput = true }))
            {
                var redirectDetails = new InAnotherCastleConfigSectionRedirect();
                redirectDetails.ExposedDeserializeSection(reader);
                return redirectDetails;
            }
        }

        public object StandardConfigSectionParse(string xml, Type expectedType)
        {
            try
            {
                var section = Activator.CreateInstance(expectedType);
                using (StringReader stringReader = new StringReader(xml.Trim()))
                using (XmlReader reader = XmlReader.Create(stringReader, new XmlReaderSettings() { CloseInput = true }))
                {
                    //This is terrible but the other options could be much worse... the goal is to get around affecting the actual config items
                    MethodInfo dynMethod = section.GetType().GetMethod("DeserializeSection", BindingFlags.NonPublic | BindingFlags.Instance);
                    dynMethod.Invoke(section, new object[] { reader });

                }
                return section;
            }
            catch (TargetInvocationException exc)
            {
                //TODO custom exception
                throw exc.InnerException;
            }
        }
        public object POCOConfigSectionParse(string xml, Type expectedType)
        {
            //TODO catch and wrap InvalidOperationException on failure to deserialize
            XmlSerializer serializer = new XmlSerializer(expectedType);
            using (MemoryStream reader = new MemoryStream(Encoding.UTF8.GetBytes(xml.Trim())))
            {
                var results = serializer.Deserialize(reader);
                return results;
            }
        }
    }
}
