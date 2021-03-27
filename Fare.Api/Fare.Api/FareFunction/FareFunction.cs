using Fare.Library.FareService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Threading.Tasks;

namespace Fare.Api.FareFunction
{
    public class FareFunction
    {
        private readonly IFareService _fareService;

        public FareFunction(IFareService fareService)
        {
            _fareService = fareService;
        }

        [FunctionName("ChargeFare")]
        public async Task<IActionResult> ChargeFare(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] ChargeFareRequest body, HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            var serviceResult = _fareService.Charge(body);
            if (serviceResult.IsSuccessful)
            {
                return new OkObjectResult(new { Message = serviceResult.Result });
            }
            else
            {
                log.LogInformation(serviceResult.ErrorMessage);
                log.LogError(serviceResult.ErrorTrace);
                if (serviceResult.StatusCode == (int)HttpStatusCode.InternalServerError)
                {
                    return new BadRequestObjectResult(new { ErrorMessage = "Unable to process. Try again later." });
                }
                else
                {
                    return new BadRequestObjectResult(new { ErrorMessage = serviceResult.ErrorMessage });
                }
            }
        }
    }
}

