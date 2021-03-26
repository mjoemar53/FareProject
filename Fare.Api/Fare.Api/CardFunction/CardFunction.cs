using Fare.Library.CardService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Fare.Api.CardFunction
{
    public class CardFunction
    {
        private readonly ICardService _cardService;

        public CardFunction(ICardService cardService)
        {
            _cardService = cardService;
        }

        [FunctionName("CreateNewCard")]
        public async Task<IActionResult> CreateNewCard(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            var serviceResult = _cardService.CreateNew();
            if (serviceResult.IsSuccessful)
            {
                return new OkObjectResult(serviceResult.Result);
            }
            else
            {
                log.LogInformation(serviceResult.ErrorMessage);
                log.LogError(serviceResult.ErrorTrace);
                return new BadRequestObjectResult("Unable to process. Try again later.");
            }
        }

        [FunctionName("CreateNewRegisteredCard")]
        public async Task<IActionResult> CreateNewRegisteredCard(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] CreateCardRequest body, HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            var serviceResult = _cardService.CreateNew(body);
            if (serviceResult.IsSuccessful)
            {
                return new OkObjectResult(serviceResult.Result);
            }
            else
            {
                log.LogInformation(serviceResult.ErrorMessage);
                log.LogError(serviceResult.ErrorTrace);
                return new BadRequestObjectResult("Unable to process. Try again later.");
            }
        }
    }
}

