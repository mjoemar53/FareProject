﻿using Fare.Library.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fare.Library.FareService
{
    public interface IFareService
    {
        ServiceResult<string> Charge(ChargeFareRequest request);
        ServiceResult<string> TopUp(string cardId, double amount, double change);
    }
}
