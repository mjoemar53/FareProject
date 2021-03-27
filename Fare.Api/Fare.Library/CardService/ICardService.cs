using Fare.Library.Models;

namespace Fare.Library.CardService
{
    public interface ICardService
    {
        ServiceResult<string> CreateNew();
        ServiceResult<string> CreateNew(CreateCardRequest requestBody);

        ServiceResult<TopUpResult> TopUp(TopUpRequest requestBody);
    }
}
