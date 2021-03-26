using Fare.Api;
using Fare.Library.CardService;
using Fare.Library.Connection;
using Fare.Library.FareService;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(StartUp))]
namespace Fare.Api
{
    public class StartUp : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddTransient<IConnection, Connection>();
            builder.Services.AddTransient<IFareService, FareService>();
            builder.Services.AddTransient<ICardService, CardService>();
            builder.Services.AddLogging();
        }
    }
}
