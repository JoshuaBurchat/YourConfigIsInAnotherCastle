using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YourConfigIsInAnotherCastle.Migrations.Models;

namespace YourConfigIsInAnotherCastle.Migrations
{
    /// <summary>
    /// Designed to abstract how the two types of outputs are generated 
    /// the caller has no idea how, or where the files are going, it just needs to supply 
    /// migrated values.
    /// </summary>
    public interface ISectionExporter
    {

        bool Initialize();
        void SaveConfigurationFile(IEnumerable<SectionMigrated> sections);
        void SaveToDataStore(IEnumerable<SectionMigrated> sections);
    }
}
