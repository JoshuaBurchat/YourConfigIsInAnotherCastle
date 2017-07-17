using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.IO;
using System.Configuration;
using YourConfigIsInAnotherCastle.Migrations;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using System.Collections.Specialized;

namespace YourConfigIsInAnotherCastle.MigrationTool.ViewModels
{


    public class MigrationViewModel : ObservableObject
    {

        private ISectionMigrator _migrator;
        private ISectionExporter _sectionExporter;


        private SeletableSectionMigratedViewModel _selectedSection;
        private string _sourcePath;
        private string _systemName;
        private string[] _errorMessages = new string[0];
        private SeletableSectionMigratedViewModel[] _sections;

        public MigrationViewModel(ISectionMigrator migrator, ISectionExporter sectionExporter)
        {
            this._migrator = migrator;
            this._sectionExporter = sectionExporter;

            DestinationConnectionString = new ConnectionStringViewModel()
            {
                //Just incase you run more than one of these, the db is meant to be throw away
                DatabaseName = "YourConifguration_" + Guid.NewGuid().ToString().Substring(0,4),
                ServerNamePath = "localhost"
            };
        }


        public string SystemName
        {
            get { return _systemName; }
            set
            {
                _systemName = value;
                RaisePropertyChangedEvent("SystemName");
            }
        }
        public string SourcePath
        {
            get { return _sourcePath; }
            set
            {
                _sourcePath = value;
                RaisePropertyChangedEvent("SourcePath");
            }
        }
        public ConnectionStringViewModel DestinationConnectionString { get; private set; }

        public SeletableSectionMigratedViewModel SelectedSection
        {
            get { return _selectedSection; }
            set
            {
                _selectedSection = value;
                RaisePropertyChangedEvent("SelectedSection");
            }
        }

        public SeletableSectionMigratedViewModel[] Sections
        {
            get { return _sections; }
            private set
            {
                _sections = value;
                RaisePropertyChangedEvent("Sections");
            }
        }

        public string[] ErrorMessages
        {
            get { return _errorMessages; }
            set
            {
                _errorMessages = value;
                RaisePropertyChangedEvent("ErrorMessages");
            }
        }

        private void OnSelectInput(object param)
        {
            OnSelectInput();
        }
        public void OnSelectInput()
        {
            ErrorMessages = new string[0];
            try
            {
                if (this._migrator.Initialize())
                {
                    Sections = this._migrator.MigrateAll().Select(c => new SeletableSectionMigratedViewModel(c)).ToArray();
                }
            }
            catch (Exception exc)
            {
                ErrorMessages = new string[] { exc.ToString() };
            }
        }


        private void OnSaveOutput(object param)
        {
            OnSaveOutput();
        }
        public void OnSaveOutput()
        {
            ErrorMessages = new string[0];
            if (string.IsNullOrWhiteSpace(SourcePath))
            {
                ErrorMessages = new string[] { "Source Path must be selected" };
                return;
            }
            var sectionsToMigrate = this.Sections.Where(s => s.IsSelectedForMigration).Select(s => s.SectionConverted).ToArray();
            if (sectionsToMigrate.Length == 0)
            {
                ErrorMessages = new string[] { "No sections have been selected for Migration" };
                return;
            }
            try
            {
                if (_sectionExporter.Initialize())
                {
                    _sectionExporter.SaveConfigurationFile(sectionsToMigrate);
                    _sectionExporter.SaveToDataStore(sectionsToMigrate);


                }
            }
            catch (Exception exc)
            {
                ErrorMessages = new string[] { exc.ToString() };
            }
        }



        public void OnSetSelectedSection(object parameter)
        {
            this.SelectedSection = (SeletableSectionMigratedViewModel)parameter;
        }
        #region Commands
        public ICommand SelectInput
        {
            get { return new DelegateCommand(OnSelectInput); }
        }
        public ICommand SaveOutput
        {
            get { return new DelegateCommand(OnSaveOutput); }
        }
        public ICommand SetSelectedSection
        {
            get { return new DelegateCommand(OnSetSelectedSection); }
        }
        #endregion Commands

    }

}

