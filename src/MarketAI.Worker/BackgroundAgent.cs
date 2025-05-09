using MarketAI.Worker.Application;
using MarketReportAI.AIClient;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace MarketAI;

public class BackgroundAgent : BackgroundService
{
    private readonly ILogger<BackgroundAgent> _logger;
    private readonly DataCacheService _dataService;
    private readonly Kernel _aiKernel;

    public BackgroundAgent(ILogger<BackgroundAgent> logger, 
        DataCacheService dataService ,
        Kernel ai
        )
    {
        ArgumentNullException.ThrowIfNull(dataService);
        ArgumentNullException.ThrowIfNull(ai);
        ArgumentNullException.ThrowIfNull(logger);
        
        _dataService = dataService;
        _aiKernel = ai;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        //while (!stoppingToken.IsCancellationRequested)
        //{
        //    if (_logger.IsEnabled(LogLevel.Information))
        //    {
        //        _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
        //    }



        //    await Task.Delay(1000, stoppingToken);
        //}

        var news = await _dataService.GetTodaysNewsFor("MSFT");

        // todo: do some AI

        var chatHistory = new ChatHistory();

        var result = await _aiKernel.InvokePromptAsync("Write a haiku about the {topic}", new() { ["topic"] = "stock market" });

        Console.WriteLine(result.GetValue<string>());

        foreach (var s in news)
        {
            Console.WriteLine(s.Summary + s.Source);
        }
    }
}
