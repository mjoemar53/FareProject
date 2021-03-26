using System;
using System.Collections.Generic;
using System.Text;

namespace Fare.Library.FareService
{
    public class ChargeFareRequest
    {
        public string CardId { get; set; }
        public string LineId { get; set; }
        public string StationId { get; set; }
    }
}
