using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YourConfigIsInAnotherCastle.Migrations
{
    public class MigrationException : Exception
    {
        public MigrationException(string message, Exception inner) : base(message, inner)
        {

        }

        //TODO add details or fields related to the migration
    }
}
