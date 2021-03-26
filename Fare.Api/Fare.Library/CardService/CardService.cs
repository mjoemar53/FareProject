using Fare.Library.Connection;
using Fare.Library.Models;
using JsonFlatFileDataStore;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Fare.Library.CardService
{
    public class CardService : ICardService
    {
        private IConnection _connection { get; set; }
        private IDocumentCollection<Card> cardCollection { get; set; }
        public CardService(IConnection connection)
        {
            _connection = connection;
            cardCollection = _connection.Db.GetCollection<Card>();
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

        public ServiceResult<string> CreateNew(RequestBody requestBody)
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
    }
}
