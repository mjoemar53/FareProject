using Newtonsoft.Json;

namespace Fare.Library.CardService
{
    public class CreateCardRequest
    {
        [JsonProperty("registeredId")]
        public string RegisteredId { get; set; }
    }
}
