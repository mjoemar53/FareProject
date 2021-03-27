using Fare.Library.Models;

namespace Fare.Library.FareService
{
    public interface IFareService
    {
        ServiceResult<string> Charge(ChargeFareRequest request);
        ServiceResult<string> TopUp(string cardId, double amount, double change);
    }
}
