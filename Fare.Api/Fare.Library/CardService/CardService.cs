using Fare.Library.Connection;
using Fare.Library.Models;
using JsonFlatFileDataStore;
using System;
using System.Linq;
using System.Net;

namespace Fare.Library.CardService
{
    public class CardService : ICardService
    {
        private IConnection _connection { get; set; }
        private IDocumentCollection<Card> cardCollection { get; set; }
        private IDocumentCollection<Lookup> lookupCollection { get; set; }
        public CardService(IConnection connection)
        {
            _connection = connection;
            cardCollection = _connection.Db.GetCollection<Card>();
            lookupCollection = _connection.Db.GetCollection<Lookup>();
        }

        public ServiceResult<string> CreateNew()
        {
            Card newCard = new Card();
            try
            {
                cardCollection.InsertOne(newCard);
                return new ServiceResult<string>()
                {
                    Result = newCard.Id,
                    IsSuccessful = true,
                    StatusCode = (int)HttpStatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult<string>()
                {
                    ErrorMessage = ex.Message,
                    ErrorTrace = ex.StackTrace,
                    IsSuccessful = false,
                    StatusCode = (int)HttpStatusCode.InternalServerError
                };
            }
            
        }

        public ServiceResult<string> CreateNew(CreateCardRequest requestBody)
        {
            Card newCard = new Card();
            string registeredId = requestBody.RegisteredId;
            //Todo: add validation to RegisteredId
            if (string.IsNullOrEmpty(registeredId))
            {
                return new ServiceResult<string>()
                {
                    ErrorMessage = "Please enter id number",
                    IsSuccessful = false,
                    StatusCode = (int)HttpStatusCode.BadRequest
                };
            }

            newCard.RegisteredId = registeredId;
            newCard.Discounted = true;

            try
            {
                cardCollection.InsertOne(newCard);
                return new ServiceResult<string>()
                {
                    Result = newCard.Id,
                    IsSuccessful = true,
                    StatusCode = (int)HttpStatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult<string>()
                {
                    ErrorMessage = ex.Message,
                    ErrorTrace = ex.StackTrace,
                    IsSuccessful = false,
                    StatusCode = (int)HttpStatusCode.InternalServerError
                };
            }
            
        }

        public ServiceResult<TopUpResult> TopUp(TopUpRequest requestBody)
        {
            string cardId = requestBody.CardId;
            bool parseAmount = decimal.TryParse(requestBody.Amount, out decimal amount);
            bool parseCashAmount = decimal.TryParse(requestBody.CashAmount, out decimal cashAmount);
            
            if (!parseAmount || !parseCashAmount)
            {
                return new ServiceResult<TopUpResult>
                {
                    IsSuccessful = false,
                    ErrorMessage = "Invalid Amount Format.",
                    StatusCode = (int)HttpStatusCode.Forbidden
                };
            }

            try
            {
                Card card = cardCollection.AsQueryable().Where(c => c.Id == cardId).FirstOrDefault();
                Lookup lookup = lookupCollection.AsQueryable().FirstOrDefault();
                // Validate Min and Max Top Up
                if (amount < lookup.MinAllowedTopUp || amount > lookup.MaxAllowedTopUp)
                {
                    return new ServiceResult<TopUpResult>
                    {
                        IsSuccessful = false,
                        ErrorMessage = $"Amount must be between {lookup.MinAllowedTopUp} and {lookup.MaxAllowedTopUp}.",
                        StatusCode = (int)HttpStatusCode.BadRequest
                    };
                }

                // Validate Cash Amount
                if (cashAmount < amount)
                {
                    return new ServiceResult<TopUpResult>
                    {
                        IsSuccessful = false,
                        ErrorMessage = "The topup amount cannot be greater than the cash amount",
                        StatusCode = (int)HttpStatusCode.Conflict
                    };
                }

                // Validate Card
                if (card == null)
                {
                    return new ServiceResult<TopUpResult>
                    {
                        IsSuccessful = false,
                        ErrorMessage = "Unable to find card info.",
                        StatusCode = (int)HttpStatusCode.BadRequest
                    };
                }

                var change = cashAmount - amount;
                var finalBalance = card.Load + amount;

                // Update Card
                card.Load = finalBalance;
                card.LastUsed = DateTime.UtcNow;

                bool updateResult = cardCollection.UpdateOne(card.Id, card);
                if (!updateResult)
                {
                    return new ServiceResult<TopUpResult>
                    {
                        IsSuccessful = false,
                        ErrorMessage = $"Failed to update card {card.Id}",
                        StatusCode = (int)HttpStatusCode.NotFound
                    };
                }

                return new ServiceResult<TopUpResult>
                {
                    IsSuccessful = true,
                    Result = new TopUpResult { Change = change, NewBalance = card.Load },
                    StatusCode = (int)HttpStatusCode.OK
                };

            }
            catch (Exception ex)
            {
                return new ServiceResult<TopUpResult>
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
