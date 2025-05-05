using MarketAI.Worker.Application;
using MarketReportAI.AIClient;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MarketAI;

public class BackgroundAgent : BackgroundService
{
    private readonly ILogger<BackgroundAgent> _logger;
    private readonly DataCacheService _dataService;

    public BackgroundAgent(ILogger<BackgroundAgent> logger, DataCacheService dataService)
    {
        ArgumentNullException.ThrowIfNull(dataService);
        ArgumentNullException.ThrowIfNull(logger);

        _dataService = dataService;
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

        foreach (var s in news)
        {
            Console.WriteLine(s.Summary + s.Source);
        }
    }
}
