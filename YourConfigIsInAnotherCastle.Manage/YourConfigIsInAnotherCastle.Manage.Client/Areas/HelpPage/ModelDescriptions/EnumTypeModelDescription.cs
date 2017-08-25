using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace YourConfigIsInAnotherCastle.Manage.Client.Areas.HelpPage.ModelDescriptions
{
    public class EnumTypeModelDescription : ModelDescription
    {
        public EnumTypeModelDescription()
        {
            Values = new Collection<EnumValueDescription>();
        }

        public Collection<EnumValueDescription> Values { get; private set; }
    }
}