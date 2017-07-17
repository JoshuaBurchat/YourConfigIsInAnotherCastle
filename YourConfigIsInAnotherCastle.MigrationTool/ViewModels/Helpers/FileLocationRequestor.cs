using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YourConfigIsInAnotherCastle.Migrations;

namespace YourConfigIsInAnotherCastle.MigrationTool.ViewModels
{
    /// <summary>
    /// Will be used to retieve the connection string from the MigrationViewModel model, and file/folder paths using 
    /// the file/folder dialogs from windows forms.
    /// 
    /// In a way this class is an extension of the MigrationViewModel, but the goal was to take out the file path retrieval for 
    /// unit test purposes.
    /// </summary>
    public class FileLocationRequestor : ILocationRequestor
    {
        private string _configurationFilePath;

        public MigrationViewModel MigrationModel { get; set; }

        public FileLocationRequestor()
        {
        }

        public string DestinationFolder { get; set; }
        public string ConfigurationFilePath
        {
            get { return _configurationFilePath; }
            set
            {
                _configurationFilePath = value;
                MigrationModel.SourcePath = _configurationFilePath;
            }
        }
        public string ConnectionString
        {
            //The connection information is a lot easier to enter on the model screen. If not this could be its own dialog.
            get { return MigrationModel.DestinationConnectionString.BuildConnection(DestinationFolder); }
        }

        public bool UseWebConfiguration { get { return false; } }

        public string GetConfigurationFilePath()
        {
            var dialog = new System.Windows.Forms.OpenFileDialog();
            //The exe or dialog must be loaded as they have some assembly details that are needed when openning the config file with the configuration manager.
            //TODO maybe it would make sense to not use configuration manager and find a solution that can read a .config file to manipulate it.
            //Because really we dont need anything more that the XML from each section defined.
            dialog.Filter = "*.exe|*.dll";
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                return (this.ConfigurationFilePath = dialog.FileName);
            }
            return null;
        }

        public string GetConnectionString()
        {
            return ConnectionString;
        }

        public string GetDestinationFolder()
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                return (this.DestinationFolder = dialog.SelectedPath);
            }
            return null;
        }
    }
}
