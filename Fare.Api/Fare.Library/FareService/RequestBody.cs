using Newtonsoft.Json;

namespace Fare.Library.FareService
{
    public class ChargeFareRequest
    {
        [JsonProperty("cardId")]
        public string CardId { get; set; }
        [JsonProperty("lineId")]
        public string LineId { get; set; }
        [JsonProperty("stationId")]
        public string StationId { get; set; }
    }
}
