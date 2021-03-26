using JsonFlatFileDataStore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fare.Library.Connection
{
    public class Connection : IConnection
    {
        public IDataStore Db { get; set; }

        public Connection()
        {
            Db = new DataStore("db.json");
        }
    }
}
