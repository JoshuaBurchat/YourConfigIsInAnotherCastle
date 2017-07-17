using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YourConfigIsInAnotherCastle.Migrations.Models;

namespace YourConfigIsInAnotherCastle.Migrations
{
    /// <summary>
    /// Designed to abstract away how a configuration sections are migrated to the In Another Castle format.
    /// </summary>
    public interface  ISectionMigrator
    {
        string SourceName { get; }
        bool Initialize();
        IEnumerable<SectionMigrated> Migrate(IEnumerable<string> sectionNames);
        IEnumerable<SectionMigrated> MigrateAll();
    }
}
