using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YourConfigIsInAnotherCastle.MigrationTool.ViewModels
{
    /// <summary>
    /// Use to be bound to a UI to build a SQL Server connection string.
    /// </summary>
    public class ConnectionStringViewModel : ObservableObject
    {
        private string _serverNamePath;
        public string ServerNamePath
        {
            get { return _serverNamePath; }
            set
            {
                _serverNamePath = value;
                RaisePropertyChangedEvent("ServerNamePath");
            }
        }

        private string _databaseName;
        public string DatabaseName
        {
            get { return _databaseName; }
            set
            {
                _databaseName = value;
                RaisePropertyChangedEvent("DatabaseName");
            }
        }
        private bool _isLocalDatabase;
        public bool IsLocalDatabase
        {
            get { return _isLocalDatabase; }
            set
            {
                _isLocalDatabase = value;
                RaisePropertyChangedEvent("IsLocalDatabase");
            }
        }


        public string BuildConnection(string folderPath = null)
        {
            if (IsLocalDatabase)
            {
                //I dont like this, this proves that the flow to this application or how I setup the VMs is flawed.
                //Because the caller knows or should know how this is implemented.
                if (string.IsNullOrWhiteSpace(folderPath))
                {
                    throw new InvalidOperationException("For local connections folder path must be supplied");
                }
                if (string.IsNullOrWhiteSpace(ServerNamePath) || string.IsNullOrWhiteSpace(this.DatabaseName)) return null;
                string format = @"Server=(localdb)\v11.0;Integrated Security=true;AttachDbFileName={0}\{1}.mdf;";
                return string.Format(format, folderPath.TrimEnd('\\'), this.DatabaseName);
                // return @"";
            }
            else
            {
                if (string.IsNullOrWhiteSpace(ServerNamePath) || string.IsNullOrWhiteSpace(this.DatabaseName)) return null;
                //TODO use SQL connection builder for now this is so basic, but validation would be easier with the builder
                string format = "Server={0};Database={1};Trusted_Connection=True;MultipleActiveResultSets=true;";
                return string.Format(format, this.ServerNamePath, this.DatabaseName);
            }
        }
    }
}
