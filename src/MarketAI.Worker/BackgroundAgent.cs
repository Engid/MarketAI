using MarketAI.Worker.Application;
using MarketReportAI.AIClient;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace MarketAI;

public class BackgroundAgent : BackgroundService
{
    private readonly ILogger<BackgroundAgent> _logger;
    private readonly DataCacheService _dataService;
    private readonly Kernel _aiKernel;
    private readonly IChatCompletionService _chat;

    public BackgroundAgent(ILogger<BackgroundAgent> logger, 
        DataCacheService dataService ,
        Kernel ai,
        IChatCompletionService chat
        )
    {
        ArgumentNullException.ThrowIfNull(dataService);
        ArgumentNullException.ThrowIfNull(ai);
        ArgumentNullException.ThrowIfNull(chat);
        ArgumentNullException.ThrowIfNull(logger);
        
        _dataService = dataService;
        _aiKernel = ai;
        _chat = chat;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var settings = new OpenAIPromptExecutionSettings
        {
            FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
        };

        // register a tool
        //_aiKernel.Plugins.AddFromType<>("name");

        var news = await _dataService.GetTodaysNewsFor("MSFT");

        // todo: do some AI

        var chatHistory = new ChatHistory("Write a haiku about the stock market");

        //var result = await _aiKernel.InvokePromptAsync("Write a haiku about the stock market");

        var r = await _chat.GetChatMessageContentAsync(chatHistory, kernel: _aiKernel);

        Console.WriteLine(r.ToString());

        chatHistory.AddMessage(r.Role, r.Content ?? "");

        //Console.WriteLine(result.GetValue<string>());

        foreach (var s in news)
        {
            Console.WriteLine(s.Summary + s.Source);
        }
    }
}
