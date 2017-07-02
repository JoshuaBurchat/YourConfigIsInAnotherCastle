using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YourConfigIsInAnotherCastle.Models;

namespace YourConfigIsInAnotherCastle.Parser
{
    //TODO make us in the IInAnotherCastleConfiguration  
    //TODO should the XML field be the config value field so that it could later be parsed from json???
    public interface IInAnotherCastleParser
    {
        IRedirectIdentifier ParseRedirectDetails(string xml);
        object StandardConfigSectionParse(string xml, Type expectedType);
        object POCOConfigSectionParse(string xml, Type expectedType);

        
    }
}
