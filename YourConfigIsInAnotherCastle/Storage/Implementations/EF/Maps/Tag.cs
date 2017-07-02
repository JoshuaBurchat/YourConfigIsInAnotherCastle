using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;
using YourConfigIsInAnotherCastle.Models;

namespace YourConfigIsInAnotherCastle.Storage.Implementations.EF.Maps
{
    internal class TagMap : EntityTypeConfiguration<Tag>
    {
        public TagMap()
        {

        }
    }
}
