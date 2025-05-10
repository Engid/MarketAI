using MarketAI.Worker.Application;
using MarketReportAI.AIClient;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace MarketAI;

public class BackgroundAgent : BackgroundService
{
    private readonly DataCacheService _dataService;
    private readonly Kernel _aiKernel;
    private readonly IChatCompletionService _chat;
    private readonly ISerializer yamlSerializer;
    private readonly ILogger<BackgroundAgent> _logger;

    public BackgroundAgent(ILogger<BackgroundAgent> logger, 
                           DataCacheService dataService,
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

        yamlSerializer = new SerializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

        // register a tool
        //_aiKernel.Plugins.AddFromType<SomeTool>("name");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            await RunAgent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occured while processing");
        }
    }

    private async Task RunAgent()
    {
        var settings = new OpenAIPromptExecutionSettings
        {
            FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
        };

        var news = await _dataService.GetTodaysNewsFor("MSFT");

        var chatHistory = new ChatHistory();

        string msg;
        ChatMessageContent result;

        var systemPrompt = """
            You are a helpful stock market analyst who specializes in making market predictions. 
            You will be shown a series of news articles summaries and asked to give your thoughts.
            Each summary includes the headline, a summary, and estimations about the companies market performance. 
            Keep your responses concise as we will look at many article summaries.

            """;

        chatHistory.AddSystemMessage(systemPrompt);

        foreach (var s in news.Take(25))
        {
            var yamlNews = yamlSerializer.Serialize(s);

            msg = $"""
                Below is an analysis of a news article about Microsoft for today. 
                Write a few lines with your thoughts about the summary and wheather 
                you agree or dissagree with the sentiments and explain why. Keep it concise. 

                {yamlNews}
                """;
                
            AddUserMessage(msg, chatHistory);

            result = await _chat.GetChatMessageContentAsync(chatHistory, settings, kernel: _aiKernel);
            AddAgentResponse(result, chatHistory);

            // delay to avoid rate limiting
            await Task.Delay(5000);
        }

        msg = """
            Now that you've seen some summaries of the daily news, write a summary with your final thoughts and answer the question: 
            Do you think Microsoft's stock price will go up or down tomorrow? Please explain why.
            """;

        AddUserMessage(msg, chatHistory);

        result = await _chat.GetChatMessageContentAsync(chatHistory, settings, kernel: _aiKernel);
        AddAgentResponse(result, chatHistory);

        var historyStr = HistoryToString(chatHistory);

        _dataService.StoreChatHistory(historyStr);
    }

    private string HistoryToString(ChatHistory chatHistory)
    {
        var sb = new StringBuilder();
        foreach (var item in chatHistory)
        {
            sb.AppendLine($"{item.Role}: {item.Content}");
        }
        return sb.ToString();
    }

    private void AddUserMessage(string msg, ChatHistory chatHistory)
    {
        chatHistory.AddUserMessage(msg);
        Console.WriteLine($"User: {msg}");
    }

    private void AddAgentResponse(ChatMessageContent r, ChatHistory chatHistory)
    {
        chatHistory.AddMessage(r.Role, r.Content ?? "");
        Console.WriteLine($"Agent: {r}");
    }
}
