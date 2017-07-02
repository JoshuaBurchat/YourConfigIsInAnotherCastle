using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using YourConfigIsInAnotherCastle.Models;

namespace YourConfigIsInAnotherCastle.Test.Mocks
{
    public static class ConfigurationValueSeeds
    {
        public static class Customers
        {
            public const string XMLSchema = @"<xs:schema attributeFormDefault=""unqualified"" elementFormDefault=""qualified"" xmlns:xs=""http://www.w3.org/2001/XMLSchema"">
				  <xs:element name=""Customers"">
					<xs:complexType>
					  <xs:sequence>
						<xs:element name=""Customer"" maxOccurs=""unbounded"" minOccurs=""0"">
						  <xs:complexType>
							<xs:sequence>
							  <xs:element type=""xs:string"" name=""CompanyName""/>
							  <xs:element type=""xs:string"" name=""ContactName""/>
							  <xs:element type=""xs:string"" name=""ContactTitle""/>
							  <xs:element type=""xs:string"" name=""Phone""/>
							  <xs:element type=""xs:string"" name=""Fax"" minOccurs=""0""/>
							  <xs:element name=""FullAddress"">
								<xs:complexType>
								  <xs:sequence>
									<xs:element type=""xs:string"" name=""Address""/>
									<xs:element type=""xs:string"" name=""City""/>
									<xs:element type=""xs:string"" name=""Region""/>
									<xs:element type=""xs:int"" name=""PostalCode""/>
									<xs:element type=""xs:string"" name=""Country""/>
								  </xs:sequence>
								</xs:complexType>
							  </xs:element>
							</xs:sequence>
							<xs:attribute type=""xs:string"" name=""CustomerID"" use=""optional""/>
						  </xs:complexType>
						</xs:element>
					  </xs:sequence>
					</xs:complexType>
				  </xs:element>
				</xs:schema>";
            public const string JsonSchema = @"
                                {
                                    ""$schema"": ""http://json-schema.org/draft-04/schema#"",
                                    ""definitions"": {},
                                    ""id"": ""http://example.com/example.json"",
                                    ""properties"": {
                                        ""Customers"": {
                                            ""id"": ""http://example.com/example.json/properties/Customers"",
                                            ""properties"": {
                                                ""Customer"": {
                                                    ""id"": ""http://example.com/example.json/properties/Customers/properties/Customer"",
                                                    ""items"": {
                                                        ""id"": ""http://example.com/example.json/properties/Customers/properties/Customer/items"",
                                                        ""properties"": {
                                                            ""@CustomerID"": {
                                                                ""id"": ""http://example.com/example.json/properties/Customers/properties/Customer/items/properties/@CustomerID"",
                                                                ""type"": ""string""
                                                            },
                                                            ""CompanyName"": {
                                                                ""id"": ""http://example.com/example.json/properties/Customers/properties/Customer/items/properties/CompanyName"",
                                                                ""type"": ""string""
                                                            },
                                                            ""ContactName"": {
                                                                ""id"": ""http://example.com/example.json/properties/Customers/properties/Customer/items/properties/ContactName"",
                                                                ""type"": ""string""
                                                            },
                                                            ""ContactTitle"": {
                                                                ""id"": ""http://example.com/example.json/properties/Customers/properties/Customer/items/properties/ContactTitle"",
                                                                ""type"": ""string""
                                                            },
                                                            ""FullAddress"": {
                                                                ""id"": ""http://example.com/example.json/properties/Customers/properties/Customer/items/properties/FullAddress"",
                                                                ""properties"": {
                                                                    ""Address"": {
                                                                        ""id"": ""http://example.com/example.json/properties/Customers/properties/Customer/items/properties/FullAddress/properties/Address"",
                                                                        ""type"": ""string""
                                                                    },
                                                                    ""City"": {
                                                                        ""id"": ""http://example.com/example.json/properties/Customers/properties/Customer/items/properties/FullAddress/properties/City"",
                                                                        ""type"": ""string""
                                                                    },
                                                                    ""Country"": {
                                                                        ""id"": ""http://example.com/example.json/properties/Customers/properties/Customer/items/properties/FullAddress/properties/Country"",
                                                                        ""type"": ""string""
                                                                    },
                                                                    ""PostalCode"": {
                                                                        ""id"": ""http://example.com/example.json/properties/Customers/properties/Customer/items/properties/FullAddress/properties/PostalCode"",
                                                                        ""type"": ""string""
                                                                    },
                                                                    ""Region"": {
                                                                        ""id"": ""http://example.com/example.json/properties/Customers/properties/Customer/items/properties/FullAddress/properties/Region"",
                                                                        ""type"": ""string""
                                                                    }
                                                                },
                                                                ""type"": ""object""
                                                            },
                                                            ""Phone"": {
                                                                ""id"": ""http://example.com/example.json/properties/Customers/properties/Customer/items/properties/Phone"",
                                                                ""type"": ""string""
                                                            }
                                                        },
                                                        ""type"": ""object""
                                                    },
                                                    ""type"": ""array""
                                                }
                                            },
                                            ""type"": ""object""
                                        }
                                    },
                                    ""type"": ""object"",
                                    ""required"": [""Customers""]
                                }";
            public static ConfigurationValue BuildSingleConfig(params Tag[] tags)
            {
                var results = new ConfigurationValue()
                {
                    Id = 1,
                    XML = @"<Customers>
				        <Customer CustomerID=""GREAL"">
				          <CompanyName>Great Lakes Food Market</CompanyName>
				          <ContactName>Howard Snyder</ContactName>
				          <ContactTitle>Marketing Manager</ContactTitle>
				          <Phone>(503) 555-7555</Phone>
				          <FullAddress>
					        <Address>2732 Baker Blvd.</Address>
					        <City>Eugene</City>
					        <Region>OR</Region>
					        <PostalCode>97403</PostalCode>
					        <Country>USA</Country>
				          </FullAddress>
				        </Customer>
			          </Customers>",
                    JSON = @"{
                       ""Customers"":{
                          ""Customer"":[
                             {
                                ""@CustomerID"":""GREAL"",
                                ""CompanyName"":""Great Lakes Food Market"",
                                ""ContactName"":""Howard Snyder"",
                                ""ContactTitle"":""Marketing Manager"",
                                ""Phone"":""(503) 555-7555"",
                                ""FullAddress"":{
                                   ""Address"":""2732 Baker Blvd."",
                                   ""City"":""Eugene"",
                                   ""Region"":""OR"",
                                   ""PostalCode"":""97403"",
                                   ""Country"":""USA""
                                }
                             }
                          ]
                       }
                    }",
                    XMLSchema = XMLSchema,
                    JSONSchema = JsonSchema,
                    Name = "Customer",
                    SystemName = "Test System",
                    
                };
                results.AssignTags(tags);
                return TrimWhiteSpace(results);
            }
            public static ConfigurationValue BuildFullConfig(params Tag[] tags)
            {
                return TrimWhiteSpace(new ConfigurationValue()
                {
                    Id = 1,
                    XML = @"<Customers>
				        <Customer CustomerID=""GREAL"">
				          <CompanyName>Great Lakes Food Market</CompanyName>
				          <ContactName>Howard Snyder</ContactName>
				          <ContactTitle>Marketing Manager</ContactTitle>
				          <Phone>(503) 555-7555</Phone>
				          <FullAddress>
					        <Address>2732 Baker Blvd.</Address>
					        <City>Eugene</City>
					        <Region>OR</Region>
					        <PostalCode>97403</PostalCode>
					        <Country>USA</Country>
				          </FullAddress>
				        </Customer>
				        <Customer CustomerID=""HUNGC"">
				          <CompanyName>Hungry Coyote Import Store</CompanyName>
				          <ContactName>Yoshi Latimer</ContactName>
				          <ContactTitle>Sales Representative</ContactTitle>
				          <Phone>(503) 555-6874</Phone>
				          <Fax>(503) 555-2376</Fax>
				          <FullAddress>
					        <Address>City Center Plaza 516 Main St.</Address>
					        <City>Elgin</City>
					        <Region>OR</Region>
					        <PostalCode>97827</PostalCode>
					        <Country>USA</Country>
				          </FullAddress>
				        </Customer>
				        <Customer CustomerID=""LAZYK"">
				          <CompanyName>Lazy K Kountry Store</CompanyName>
				          <ContactName>John Steel</ContactName>
				          <ContactTitle>Marketing Manager</ContactTitle>
				          <Phone>(509) 555-7969</Phone>
				          <Fax>(509) 555-6221</Fax>
				          <FullAddress>
					        <Address>12 Orchestra Terrace</Address>
					        <City>Walla Walla</City>
					        <Region>WA</Region>
					        <PostalCode>99362</PostalCode>
					        <Country>USA</Country>
				          </FullAddress>
				        </Customer>
				        <Customer CustomerID=""LETSS"">
				          <CompanyName>Lets Stop N Shop</CompanyName>
				          <ContactName>Jaime Yorres</ContactName>
				          <ContactTitle>Owner</ContactTitle>
				          <Phone>(415) 555-5938</Phone>
				          <FullAddress>
					        <Address>87 Polk St. Suite 5</Address>
					        <City>San Francisco</City>
					        <Region>CA</Region>
					        <PostalCode>94117</PostalCode>
					        <Country>USA</Country>
				          </FullAddress>
				        </Customer>
			          </Customers>",
                    JSON = @"{
                    ""Customers"":{ 
			          ""Customer"": [
				        {
				          ""@CustomerID"": ""GREAL"",
				          ""CompanyName"": "" Great Lakes Food Market "",
				          ""ContactName"": "" Howard Snyder "",
				          ""ContactTitle"": "" Marketing Manager "",
				          ""Phone"": "" (503) 555 - 7555 "",
				          ""FullAddress"": {
					        ""Address"": "" 2732 Baker Blvd."",
					        ""City"": "" Eugene "",
					        ""Region"": "" OR "",
					        ""PostalCode"": "" 97403 "",
					        ""Country"": "" USA ""
				          }
				        },
				        {
				          ""@CustomerID"": ""HUNGC"",
				          ""CompanyName"": "" Hungry Coyote Import Store "",
				          ""ContactName"": "" Yoshi Latimer "",
				          ""ContactTitle"": "" Sales Representative "",
				          ""Phone"": "" (503) 555 - 6874 "",
				          ""Fax"": "" (503) 555 - 2376 "",
				          ""FullAddress"": {
					        ""Address"": "" City Center Plaza 516 Main St."",
					        ""City"": "" Elgin "",
					        ""Region"": "" OR "",
					        ""PostalCode"": "" 97827 "",
					        ""Country"": "" USA ""
				          }
				        },
				        {
				          ""@CustomerID"": ""LAZYK"",
				          ""CompanyName"": "" Lazy K Kountry Store "",
				          ""ContactName"": "" John Steel "",
				          ""ContactTitle"": "" Marketing Manager "",
				          ""Phone"": "" (509) 555 - 7969 "",
				          ""Fax"": "" (509) 555 - 6221 "",
				          ""FullAddress"": {
					        ""Address"": "" 12 Orchestra Terrace "",
					        ""City"": "" Walla Walla "",
					        ""Region"": "" WA "",
					        ""PostalCode"": "" 99362 "",
					        ""Country"": "" USA ""
				          }
				        },
				        {
				          ""@CustomerID"": ""LETSS"",
				          ""CompanyName"": "" Lets Stop N Shop "",
				          ""ContactName"": "" Jaime Yorres "",
				          ""ContactTitle"": "" Owner "",
				          ""Phone"": "" (415) 555 - 5938 "",
				          ""FullAddress"": {
					        ""Address"": "" 87 Polk St.Suite 5 "",
					        ""City"": "" San Francisco "",
					        ""Region"": "" CA "",
					        ""PostalCode"": "" 94117 "",
					        ""Country"": "" USA ""
				          }
				        }
			          ]
                      }   
			        }",
                    XMLSchema = XMLSchema,
                    JSONSchema = JsonSchema,
                    Name = "Customer",
                    SystemName = "Test System",
                }).AssignTags(tags);

            }

        }



        public static class Books
        {
            public const string XMLSchema = @"<xs:schema attributeFormDefault=""unqualified"" elementFormDefault=""qualified"" xmlns:xs=""http://www.w3.org/2001/XMLSchema"">
                                              <xs:element name=""Books"" >
                                                <xs:complexType>
                                                  <xs:sequence>
                                                    <xs:element name=""Book"" maxOccurs=""unbounded"" minOccurs=""0"">
                                                      <xs:complexType>
                                                        <xs:sequence>
                                                          <xs:element type=""xs:string"" name=""Author""/>
                                                          <xs:element type=""xs:string"" name=""Title""/>
                                                          <xs:element type=""xs:string"" name=""Genre""/>
                                                          <xs:element type=""xs:float"" name=""Price""/>
                                                          <xs:element type=""xs:date"" name=""PublishDate""/>
                                                          <xs:element type=""xs:string"" name=""Description""/>
                                                        </xs:sequence>
                                                        <xs:attribute type=""xs:string"" name=""Id"" use=""optional""/>
                                                      </xs:complexType>
                                                    </xs:element>
                                                  </xs:sequence>
                                                </xs:complexType>
                                              </xs:element>
                                            </xs:schema>";
            public const string JsonSchema = @"{
                                            ""$schema"": ""http://json-schema.org/draft-04/schema#"",
                                            ""definitions"": {},
                                            ""id"": ""http://example.com/example.json"",
                                            ""properties"": {
                                                ""Books"": {
                                                    ""id"": ""/properties/Books"",
                                                    ""properties"": {
                                                        ""Book"": {
                                                            ""id"": ""/properties/Books/properties/Book"",
                                                            ""items"": {
                                                                ""id"": ""/properties/Books/properties/Book/items"",
                                                                ""properties"": {
                                                                    ""@Id"": {
                                                                        ""id"": ""/properties/Books/properties/Book/items/properties/@Id"",
                                                                        ""type"": ""string""
                                                                    },
                                                                    ""Author"": {
                                                                        ""id"": ""/properties/Books/properties/Book/items/properties/Author"",
                                                                        ""type"": ""string""
                                                                    },
                                                                    ""Description"": {
                                                                        ""id"": ""/properties/Books/properties/Book/items/properties/Description"",
                                                                        ""type"": ""string""
                                                                    },
                                                                    ""Genre"": {
                                                                        ""id"": ""/properties/Books/properties/Book/items/properties/Genre"",
                                                                        ""type"": ""string""
                                                                    },
                                                                    ""Price"": {
                                                                        ""id"": ""/properties/Books/properties/Book/items/properties/Price"",
                                                                        ""type"": ""string""
                                                                    },
                                                                    ""PublishDate"": {
                                                                        ""id"": ""/properties/Books/properties/Book/items/properties/PublishDate"",
                                                                        ""type"": ""string""
                                                                    },
                                                                    ""Title"": {
                                                                        ""id"": ""/properties/Books/properties/Book/items/properties/Title"",
                                                                        ""type"": ""string""
                                                                    }
                                                                },
                                                                ""type"": ""object""
                                                            },
                                                            ""type"": ""array""
                                                        }
                                                    },
                                                    ""type"": ""object""
                                                }
                                            },
                                            ""type"": ""object"",
                                            ""required"":[""Books""]
                                        }";
            public static ConfigurationValue BuildSingleConfig(params Tag[] tags)
            {
                var results = new ConfigurationValue()
                {
                    Id = 1,
                    XML = @"<Books>
                                <Book Id=""bk101"">
                                    <Author>Gambardella, Matthew</Author>
                                    <Title>XML Developer's GuIde</Title>
                                    <Genre>Computer</Genre>
                                    <Price>44.95</Price>
                                    <PublishDate>2000-10-01</PublishDate>
                                    <Description>An in-depth look at creating applications 
                                    with XML.</Description>
                                </Book>
                            </Books>",
                    JSON = @"{
                              ""Books"": {
                                ""Book"": [
                                  {
                                                ""@Id"": ""bk101"",
                                    ""Author"": ""Gambardella, Matthew"",
                                    ""Title"": ""XML Developer's GuIde"",
                                    ""Genre"": ""Computer"",
                                    ""Price"": ""44.95"",
                                    ""PublishDate"": ""2000-10-01"",
                                    ""Description"": ""An in-depth look at creating applications \r\n                                    with XML.""
                                  }
                                ]
                              }
                    }",
                    XMLSchema = XMLSchema,
                    JSONSchema = JsonSchema,
                    Name = "Customer",
                    SystemName = "Test System"
                };
                results.AssignTags(tags);

                return TrimWhiteSpace(results);
            }
            public static ConfigurationValue BuildFullConfig(params Tag[] tags)
            {
                return TrimWhiteSpace(new ConfigurationValue()
                {
                    Id = 1,
                    XML = @"<Books>
                                <Book Id=""bk101"">
                                    <Author>Gambardella, Matthew</Author>
                                    <Title>XML Developer's GuIde</Title>
                                    <Genre>Computer</Genre>
                                    <Price>44.95</Price>
                                    <PublishDate>2000-10-01</PublishDate>
                                    <Description>An in-depth look at creating applications 
                                    with XML.</Description>
                                </Book>
                                <Book Id=""bk102"">
                                    <Author>Ralls, Kim</Author>
                                    <Title>MIdnight Rain</Title>
                                    <Genre>Fantasy</Genre>
                                    <Price>5.95</Price>
                                    <PublishDate>2000-12-16</PublishDate>
                                    <Description>A former architect battles corporate zombies, 
                                    an evil sorceress, and her own childhood to become queen 
                                    of the world.</Description>
                                </Book>
                                <Book Id=""bk103"">
                                    <Author>Corets, Eva</Author>
                                    <Title>Maeve Ascendant</Title>
                                    <Genre>Fantasy</Genre>
                                    <Price>5.95</Price>
                                    <PublishDate>2000-11-17</PublishDate>
                                    <Description>After the collapse of a nanotechnology 
                                    society in England, the young survivors lay the 
                                    foundation for a new society.</Description>
                                </Book>
                                <Book Id=""bk104"">
                                    <Author>Corets, Eva</Author>
                                    <Title>Oberon's Legacy</Title>
                                    <Genre>Fantasy</Genre>
                                    <Price>5.95</Price>
                                    <PublishDate>2001-03-10</PublishDate>
                                    <Description>In post-apocalypse England, the mysterious 
                                    agent known only as Oberon helps to create a new life 
                                    for the inhabitants of London. Sequel to Maeve 
                                    Ascendant.</Description>
                                </Book>
                                <Book Id=""bk105"">
                                    <Author>Corets, Eva</Author>
                                    <Title>The Sundered Grail</Title>
                                    <Genre>Fantasy</Genre>
                                    <Price>5.95</Price>
                                    <PublishDate>2001-09-10</PublishDate>
                                    <Description>The two daughters of Maeve, half-sisters, 
                                    battle one another for control of England. Sequel to 
                                    Oberon's Legacy.</Description>
                                </Book>
                                <Book Id=""bk106"">
                                    <Author>Randall, Cynthia</Author>
                                    <Title>Lover Birds</Title>
                                    <Genre>Romance</Genre>
                                    <Price>4.95</Price>
                                    <PublishDate>2000-09-02</PublishDate>
                                    <Description>When Carla meets Paul at an ornithology 
                                    conference, tempers fly as feathers get ruffled.</Description>
                                </Book>
                                <Book Id=""bk107"">
                                    <Author>Thurman, Paula</Author>
                                    <Title>Splish Splash</Title>
                                    <Genre>Romance</Genre>
                                    <Price>4.95</Price>
                                    <PublishDate>2000-11-02</PublishDate>
                                    <Description>A deep sea diver finds true love twenty 
                                    thousand leagues beneath the sea.</Description>
                                </Book>
                                <Book Id=""bk108"">
                                    <Author>Knorr, Stefan</Author>
                                    <Title>Creepy Crawlies</Title>
                                    <Genre>Horror</Genre>
                                    <Price>4.95</Price>
                                    <PublishDate>2000-12-06</PublishDate>
                                    <Description>An anthology of horror stories about roaches,
                                    centipedes, scorpions  and other insects.</Description>
                                </Book>
                                <Book Id=""bk109"">
                                    <Author>Kress, Peter</Author>
                                    <Title>Paradox Lost</Title>
                                    <Genre>Science Fiction</Genre>
                                    <Price>6.95</Price>
                                    <PublishDate>2000-11-02</PublishDate>
                                    <Description>After an inadvertant trip through a Heisenberg
                                    Uncertainty Device, James Salway discovers the problems 
                                    of being quantum.</Description>
                                </Book>
                                <Book Id=""bk110"">
                                    <Author>O'Brien, Tim</Author>
                                    <Title>Microsoft .NET: The Programming Bible</Title>
                                    <Genre>Computer</Genre>
                                    <Price>36.95</Price>
                                    <PublishDate>2000-12-09</PublishDate>
                                    <Description>Microsoft's .NET initiative is explored in 
                                    detail in this deep programmer's reference.</Description>
                                </Book>
                                <Book Id=""bk111"">
                                    <Author>O'Brien, Tim</Author>
                                    <Title>MSXML3: A Comprehensive GuIde</Title>
                                    <Genre>Computer</Genre>
                                    <Price>36.95</Price>
                                    <PublishDate>2000-12-01</PublishDate>
                                    <Description>The Microsoft MSXML3 parser is covered in 
                                    detail, with attention to XML DOM interfaces, XSLT processing, 
                                    SAX and more.</Description>
                                </Book>
                                <Book Id=""bk112"">
                                    <Author>Galos, Mike</Author>
                                    <Title>Visual Studio 7: A Comprehensive GuIde</Title>
                                    <Genre>Computer</Genre>
                                    <Price>49.95</Price>
                                    <PublishDate>2001-04-16</PublishDate>
                                    <Description>Microsoft Visual Studio 7 is explored in depth,
                                    looking at how Visual Basic, Visual C++, C#, and ASP+ are 
                                    integrated into a comprehensive development 
                                    environment.</Description>
                                </Book>
                            </Books>",
                    JSON = @"{
                              ""Books"": {
                                ""Book"": [
                                  {
                                                ""@Id"": ""bk101"",
                                    ""Author"": ""Gambardella, Matthew"",
                                    ""Title"": ""XML Developer's GuIde"",
                                    ""Genre"": ""Computer"",
                                    ""Price"": ""44.95"",
                                    ""PublishDate"": ""2000-10-01"",
                                    ""Description"": ""An in-depth look at creating applications \r\n                                    with XML.""
                                  },
                                  {
                                                ""@Id"": ""bk102"",
                                    ""Author"": ""Ralls, Kim"",
                                    ""Title"": ""MIdnight Rain"",
                                    ""Genre"": ""Fantasy"",
                                    ""Price"": ""5.95"",
                                    ""PublishDate"": ""2000-12-16"",
                                    ""Description"": ""A former architect battles corporate zombies, \r\n                                    an evil sorceress, and her own childhood to become queen \r\n                                    of the world.""
                                  },
                                  {
                                                ""@Id"": ""bk103"",
                                    ""Author"": ""Corets, Eva"",
                                    ""Title"": ""Maeve Ascendant"",
                                    ""Genre"": ""Fantasy"",
                                    ""Price"": ""5.95"",
                                    ""PublishDate"": ""2000-11-17"",
                                    ""Description"": ""After the collapse of a nanotechnology \r\n                                    society in England, the young survivors lay the \r\n                                    foundation for a new society.""
                                  },
                                  {
                                                ""@Id"": ""bk104"",
                                    ""Author"": ""Corets, Eva"",
                                    ""Title"": ""Oberon's Legacy"",
                                    ""Genre"": ""Fantasy"",
                                    ""Price"": ""5.95"",
                                    ""PublishDate"": ""2001-03-10"",
                                    ""Description"": ""In post-apocalypse England, the mysterious \r\n                                    agent known only as Oberon helps to create a new life \r\n                                    for the inhabitants of London. Sequel to Maeve \r\n                                    Ascendant.""
                                  },
                                  {
                                                ""@Id"": ""bk105"",
                                    ""Author"": ""Corets, Eva"",
                                    ""Title"": ""The Sundered Grail"",
                                    ""Genre"": ""Fantasy"",
                                    ""Price"": ""5.95"",
                                    ""PublishDate"": ""2001-09-10"",
                                    ""Description"": ""The two daughters of Maeve, half-sisters, \r\n                                    battle one another for control of England. Sequel to \r\n                                    Oberon's Legacy.""
                                  },
                                  {
                                                ""@Id"": ""bk106"",
                                    ""Author"": ""Randall, Cynthia"",
                                    ""Title"": ""Lover Birds"",
                                    ""Genre"": ""Romance"",
                                    ""Price"": ""4.95"",
                                    ""PublishDate"": ""2000-09-02"",
                                    ""Description"": ""When Carla meets Paul at an ornithology \r\n                                    conference, tempers fly as feathers get ruffled.""
                                  },
                                  {
                                                ""@Id"": ""bk107"",
                                    ""Author"": ""Thurman, Paula"",
                                    ""Title"": ""Splish Splash"",
                                    ""Genre"": ""Romance"",
                                    ""Price"": ""4.95"",
                                    ""PublishDate"": ""2000-11-02"",
                                    ""Description"": ""A deep sea diver finds true love twenty \r\n                                    thousand leagues beneath the sea.""
                                  },
                                  {
                                                ""@Id"": ""bk108"",
                                    ""Author"": ""Knorr, Stefan"",
                                    ""Title"": ""Creepy Crawlies"",
                                    ""Genre"": ""Horror"",
                                    ""Price"": ""4.95"",
                                    ""PublishDate"": ""2000-12-06"",
                                    ""Description"": ""An anthology of horror stories about roaches,\r\n                                    centipedes, scorpions  and other insects.""
                                  },
                                  {
                                                ""@Id"": ""bk109"",
                                    ""Author"": ""Kress, Peter"",
                                    ""Title"": ""Paradox Lost"",
                                    ""Genre"": ""Science Fiction"",
                                    ""Price"": ""6.95"",
                                    ""PublishDate"": ""2000-11-02"",
                                    ""Description"": ""After an inadvertant trip through a Heisenberg\r\n                                    Uncertainty Device, James Salway discovers the problems \r\n                                    of being quantum.""
                                  },
                                  {
                                                ""@Id"": ""bk110"",
                                    ""Author"": ""O'Brien, Tim"",
                                    ""Title"": ""Microsoft .NET: The Programming Bible"",
                                    ""Genre"": ""Computer"",
                                    ""Price"": ""36.95"",
                                    ""PublishDate"": ""2000-12-09"",
                                    ""Description"": ""Microsoft's .NET initiative is explored in \r\n                                    detail in this deep programmer's reference.""
                                  },
                                  {
                                                ""@Id"": ""bk111"",
                                    ""Author"": ""O'Brien, Tim"",
                                    ""Title"": ""MSXML3: A Comprehensive GuIde"",
                                    ""Genre"": ""Computer"",
                                    ""Price"": ""36.95"",
                                    ""PublishDate"": ""2000-12-01"",
                                    ""Description"": ""The Microsoft MSXML3 parser is covered in \r\n                                    detail, with attention to XML DOM interfaces, XSLT processing, \r\n                                    SAX and more.""
                                  },
                                  {
                                                ""@Id"": ""bk112"",
                                    ""Author"": ""Galos, Mike"",
                                    ""Title"": ""Visual Studio 7: A Comprehensive GuIde"",
                                    ""Genre"": ""Computer"",
                                    ""Price"": ""49.95"",
                                    ""PublishDate"": ""2001-04-16"",
                                    ""Description"": ""Microsoft Visual Studio 7 is explored in depth,\r\n                                    looking at how Visual Basic, Visual C++, C#, and ASP+ are \r\n                                    integrated into a comprehensive development \r\n                                    environment.""
                                  }
                                ]
                              }
                    }",
                    XMLSchema = XMLSchema,
                    JSONSchema = JsonSchema,
                    Name = "Customer",
                    SystemName = "Test System"
                }).AssignTags(tags);

            }

        }
 
        public static ConfigurationValue TrimWhiteSpace(ConfigurationValue value)
        {
            if (value == null) return value;
            Regex regexXml = new Regex(@">\s*<");
            value.XML = regexXml.Replace(value.XML, "><").Trim();

            value.JSON = Regex.Replace(value.JSON, "(\"(?:[^\"\\\\]|\\\\.)*\")|\\s+", "$1").Trim();
            return value;
        }

        public static ConfigurationValue AssignTags(this ConfigurationValue value, IEnumerable<Tag> tags )
        {
            foreach(var tag in tags) {
                value.Tags.Add(tag);
                tag.ConfigurationValues.Add(value);
            }
            return value;
        }
    }
}
