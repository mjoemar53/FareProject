using Newtonsoft.Json;

namespace Fare.Library.CardService
{
    public class CreateCardRequest
    {
        [JsonProperty("registeredId")]
        public string RegisteredId { get; set; }
    }

    public class RegisterCardRequest : CreateCardRequest
    {
        [JsonProperty("cardId")]
        public string CardId { get; set; }
    }

    public class TopUpRequest
    {
        [JsonProperty("cardId")]
        public string CardId { get; set; }
        [JsonProperty("amount")]
        public string Amount { get; set; }
        [JsonProperty("cashAmount")]
        public string CashAmount { get; set; }
    }

    public class TopUpResult
    {
        [JsonProperty("newBalance")]
        public decimal NewBalance { get; set; }
        [JsonProperty("change")]
        public decimal Change { get; set; }
    }
}
