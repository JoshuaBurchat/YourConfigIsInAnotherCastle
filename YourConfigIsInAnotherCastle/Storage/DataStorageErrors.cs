using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YourConfigIsInAnotherCastle.Storage
{
    public enum DataStorageError
    {
        AlreadyExists,
        RecordNotFound,
        RecordsUnchanged,
        DoesNotMatchXMlSchema,
        DoesNotMatchJsonSchema,
        InvalidXmlSchema,
        MissingNameField
    }
}
