using Fare.Library.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fare.Library.CardService
{
    public interface ICardService
    {
        ServiceResult<string> CreateNew();
        ServiceResult<string> CreateNew(RequestBody requestBody);
    }
}
