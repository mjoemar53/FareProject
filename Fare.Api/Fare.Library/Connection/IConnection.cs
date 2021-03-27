using JsonFlatFileDataStore;

namespace Fare.Library.Connection
{
    public interface IConnection
    {
        IDataStore Db { get; set; }
    }
}
