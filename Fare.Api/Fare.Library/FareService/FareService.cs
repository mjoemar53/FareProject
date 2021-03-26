using Fare.Library.Connection;
using Fare.Library.Models;
using JsonFlatFileDataStore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Fare.Library.FareService
{
    public class FareService : IFareService
    {
        private IConnection _connection { get; set; }
        private IDocumentCollection<Card> cardCollection { get; set; }
        private IDocumentCollection<Line> lineCollection { get; set; }
        private IDocumentCollection<Lookup> lookupCollection { get; set; }
        public FareService(IConnection connection)
        {
            _connection = connection;
            cardCollection = _connection.Db.GetCollection<Card>();
            lineCollection = _connection.Db.GetCollection<Line>();
            lookupCollection = _connection.Db.GetCollection<Lookup>();
        }

        public ServiceResult<string> Charge(ChargeFareRequest request)
        {
            string cardId = request.CardId;
            int lineId = int.Parse(request.LineId);
            int stationId = int.Parse(request.StationId);

            try
            {
                // Validate Card
                var card = cardCollection.AsQueryable().Where(c => c.Id == cardId).FirstOrDefault();
                if (card == null)
                {
                    return new ServiceResult<string>
                    {
                        IsSuccessful = false,
                        ErrorMessage = "Unable to find card info.",
                        StatusCode = (int)HttpStatusCode.BadRequest
                    };
                }

                if (card.ValidUntil == null)
                {
                    return new ServiceResult<string>
                    {
                        IsSuccessful = false,
                        ErrorMessage = "Unable to find card info.",
                        StatusCode = (int)HttpStatusCode.BadRequest
                    };
                }


                // Validate Entry Line
                if (card.LastLine != lineId)
                {
                    return new ServiceResult<string>
                    {
                        IsSuccessful = false,
                        ErrorMessage = "Invalid Entry Line.",
                        StatusCode = (int)HttpStatusCode.Conflict
                    };
                }

                var line = lineCollection.AsQueryable().Where(l => l.Id == lineId).FirstOrDefault();
                var stations = line.Stations;

                // Validate Exit Station
                if (!stations.Any(st => st.Id == stationId))
                {
                    return new ServiceResult<string>
                    {
                        IsSuccessful = false,
                        ErrorMessage = "Invalid Exit Station.",
                        StatusCode = (int)HttpStatusCode.Conflict
                    };
                }

                // Validate Exit Station
                if (!stations.Any(st => st.Id == card.LastStation))
                {
                    return new ServiceResult<string>
                    {
                        IsSuccessful = false,
                        ErrorMessage = "Invalid Entry Station.",
                        StatusCode = (int)HttpStatusCode.Conflict
                    };
                }

                // Compute Distance Travelled
                var entryDistance = stations.Where(st => st.Id == card.LastStation).FirstOrDefault().Distance;
                var exitDistance = stations.Where(st => st.Id == stationId).FirstOrDefault().Distance;
                var travelled = (decimal)(Math.Abs(exitDistance - entryDistance));
                var totalCost = line.BaseFare + (travelled * line.ExcessCharge);

                // Compute Discount
                Lookup lookup = lookupCollection.AsQueryable().FirstOrDefault();
                decimal totalDiscount = 0;

                totalDiscount = card.Discounted ? totalDiscount + lookup.Discount : 0;

                // Check for same day transactions
                int sameDateTransactions = card.CompletedTransactions.Count(trans => trans.TransactionDate.Date == DateTime.UtcNow.Date);

                totalDiscount = sameDateTransactions <= 4 ? totalDiscount + lookup.CompoundDiscount : totalDiscount;

                decimal FinalAmount = totalCost - (totalCost * totalDiscount);
                decimal RemainingBalance = card.Load - FinalAmount;

                // Update 
                card.Load = RemainingBalance;
                card.CompletedTransactions.Add(new Transaction() { Line = lineId, Entry = card.LastLine, Exit = stationId });
                card.LastUsed = DateTime.UtcNow;
                card.LastLine = null;
                card.LastStation = null;

                cardCollection.UpdateOne(card.Id, card);

                return new ServiceResult<string>
                {
                    IsSuccessful = true,
                    Result = $"Amount: {FinalAmount} Balance: {RemainingBalance}",
                    StatusCode = (int)HttpStatusCode.OK
                };
            }
            catch (Exception ex)
            {

                return new ServiceResult<string>
                {
                    IsSuccessful = false,
                    ErrorMessage = ex.Message,
                    ErrorTrace = ex.StackTrace,
                    StatusCode = (int)HttpStatusCode.InternalServerError
                };
            }
        }

        public ServiceResult<string> TopUp(string cardId, double amount, double change)
        {
            throw new NotImplementedException();
        }
    }
}
