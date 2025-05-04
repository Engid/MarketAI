using MarketReportAI.AIClient;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MarketAI;

public class BackgroundAgent : BackgroundService
{
    private readonly ILogger<BackgroundAgent> _logger;
    private readonly IMarketDataClient _alphaVantageClient;

    public BackgroundAgent(ILogger<BackgroundAgent> logger, IMarketDataClient alphaClient)
    {
        ArgumentNullException.ThrowIfNull(alphaClient);
        ArgumentNullException.ThrowIfNull(logger);

        _alphaVantageClient = alphaClient;
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

        var result = await _alphaVantageClient.GetNews("MSFT");

        Console.WriteLine(result);
    }
}
