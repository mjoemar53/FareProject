using Fare.Library.Connection;
using Fare.Library.Models;
using JsonFlatFileDataStore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fare.Library.FareService
{
    public class FareService : IFareService
    {
        private IConnection _connection { get; set; }
        private IDocumentCollection<Card> cardCollection { get; set; }
        public FareService(IConnection connection)
        {
            _connection = connection;
            cardCollection = _connection.Db.GetCollection<Card>();
        }
    }
}
