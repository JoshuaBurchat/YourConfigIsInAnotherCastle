using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YourConfigIsInAnotherCastle.Migrations.Models;

namespace YourConfigIsInAnotherCastle.MigrationTool.ViewModels
{
    public class SeletableSectionMigratedViewModel : ObservableObject
    {
        public SectionMigrated SectionConverted { get; set; }
        public SeletableSectionMigratedViewModel(SectionMigrated sectionConverted)
        {
            this.SectionConverted = sectionConverted;
        }
        private bool _isSelectedForMigration;
        public bool IsSelectedForMigration
        {
            get { return _isSelectedForMigration; }
            set
            {
                _isSelectedForMigration = value;
                RaisePropertyChangedEvent("IsSelectedForMigration");
            }
        }

        public string SectionName
        {
            get { return SectionConverted.SectionName; }

        }

        public XmlDetails XmlDetails
        {
            get { return SectionConverted.XmlDetails; }
        }
        public JsonDetails JsonDetails
        {
            get { return SectionConverted.JsonDetails; }

        }
        public string SectionTypeName
        {
            get { return SectionConverted.SectionTypeName; }
        }
    }
}
