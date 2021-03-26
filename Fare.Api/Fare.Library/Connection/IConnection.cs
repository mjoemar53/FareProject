using JsonFlatFileDataStore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fare.Library.Connection
{
    public interface IConnection
    {
        IDataStore Db { get; set; }
    }
}
