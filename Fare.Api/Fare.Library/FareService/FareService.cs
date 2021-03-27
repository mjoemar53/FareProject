using Fare.Library.Connection;
using Fare.Library.Models;
using JsonFlatFileDataStore;
using System;
using System.Linq;
using System.Net;

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
                var card = cardCollection.AsQueryable().Where(c => c.Id == cardId).FirstOrDefault();
                bool updateResult;

                // Validate Card
                if (card == null)
                {
                    return new ServiceResult<string>
                    {
                        IsSuccessful = false,
                        ErrorMessage = "Unable to find card info.",
                        StatusCode = (int)HttpStatusCode.BadRequest
                    };
                }

                // Lock Card
                if (card.LastLine == null && card.LastStation == null)
                {
                    card.LastLine = lineId;
                    card.LastStation = stationId;
                    updateResult = cardCollection.UpdateOne(card.Id, card);
                    if (!updateResult)
                    {
                        return new ServiceResult<string>
                        {
                            IsSuccessful = false,
                            Result = "Unable to lock card.",
                            StatusCode = (int)HttpStatusCode.BadRequest
                        };
                    }

                    return new ServiceResult<string>
                    {
                        IsSuccessful = true,
                        Result = $"Balance: {card.Load}",
                        StatusCode = (int)HttpStatusCode.Accepted
                    };
                }

                // Card Validity
                if (DateTime.UtcNow.Year > card.ValidUntil.Year)
                {
                    return new ServiceResult<string>
                    {
                        IsSuccessful = false,
                        ErrorMessage = "Card is no longer valid.",
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

                totalDiscount = sameDateTransactions < lookup.MaxDailyDiscountCounter ? totalDiscount + lookup.CompoundDiscount : totalDiscount;

                decimal FinalAmount = totalCost - (totalCost * totalDiscount);
                decimal RemainingBalance = card.Load - FinalAmount;

                // Update Card
                card.Load = RemainingBalance;
                card.CompletedTransactions.Add(new Transaction() { Line = lineId, Entry = card.LastLine, Exit = stationId });
                card.LastUsed = DateTime.UtcNow;
                card.LastLine = null;
                card.LastStation = null;

                updateResult = cardCollection.UpdateOne(card.Id, card);
                if(!updateResult)
                {
                    return new ServiceResult<string>
                    {
                        IsSuccessful = false,
                        ErrorMessage = $"Failed to update card {card.Id}",
                        StatusCode = (int)HttpStatusCode.NotFound
                    };
                }

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
        
    }
}
