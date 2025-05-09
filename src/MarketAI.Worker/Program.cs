using MarketAI;
using MarketAI.Worker.Application;
using MarketAI.Worker.Data;
using MarketReportAI.AIClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.SemanticKernel;


var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHttpClient<IMarketDataClient, AlphaVantageClient>(client =>
{
    client.BaseAddress = new Uri("https://www.alphavantage.co/");
});


// this doesn't work? 
var ai = builder.Configuration.GetSection("OpenAI").Get<OpenAiConfig>();

if (ai == null)
{
    throw new Exception("config not available");
}

//var kernelBuilder = Kernel.CreateBuilder().AddOpenAIChatCompletion(ai.ModelId, ai.ApiKey);

//// I think this registers it too??
//Kernel kernel = kernelBuilder.Build();

builder.Services.AddOpenAIChatCompletion(ai.ModelId, ai.ApiKey);
builder.Services.AddKernel();

builder.Services.AddTransient<IFileStorage, MarketDataFileStorage>();
builder.Services.AddTransient<DataCacheService>();

builder.Services.AddHostedService<BackgroundAgent>();

var host = builder.Build();
host.Run();



record OpenAiConfig
{
   public string ApiKey { get; init; }
   public string ModelId { get; init; }
   public string Endpoint { get; init; }
};