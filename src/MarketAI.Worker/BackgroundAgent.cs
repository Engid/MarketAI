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
    //private readonly 
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
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var yamlSerializer = new SerializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();


        var settings = new OpenAIPromptExecutionSettings
        {
            FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
        };

        // register a tool
        //_aiKernel.Plugins.AddFromType<>("name");

        var news = await _dataService.GetTodaysNewsFor("MSFT");

        // todo: do some AI

        var chatHistory = new ChatHistory();




        //Console.WriteLine(result.GetValue<string>());

        ChatMessageContent r;
        var msg = "";
        var userMsg = "";
        var respMsg = "";
        var sb = new StringBuilder();

        foreach (var s in news.Skip(5).Take(10))
        {
            var yamlNews = yamlSerializer.Serialize(s);

            msg = $"Think about this market news headline for Microsoft. Write a few lines with your thoughts, then I will show you another headline. \n{yamlNews}";
            AddUserMessage(msg, chatHistory, sb);

            r = await _chat.GetChatMessageContentAsync(chatHistory, kernel: _aiKernel);
            AddAgentResponse(r, chatHistory, sb);

            // delay to avoid rate limiting
            await Task.Delay(1000);
        }

        msg = "Now that you've seen the headlines, write a summary with your final thoughts and answer the question: Do you think Microsoft will go up or down tomorrow?";
        AddUserMessage(msg, chatHistory, sb);

        r = await _chat.GetChatMessageContentAsync(chatHistory, kernel: _aiKernel);
        AddAgentResponse(r, chatHistory, sb);

        _dataService.StoreChatHistory(sb.ToString());
    }

    private void AddUserMessage(string msg, ChatHistory chatHistory, StringBuilder sb)
    {
        chatHistory.AddUserMessage(msg);
        var userMsg = $"User: {msg}";
        sb.AppendLine(userMsg);
        Console.WriteLine(userMsg);
    }

    private void AddAgentResponse(ChatMessageContent r, ChatHistory chatHistory, StringBuilder sb)
    {
        chatHistory.AddMessage(r.Role, r.Content ?? "");
        var respMsg = $"Agent: {r}";
        sb.AppendLine(respMsg);
        Console.WriteLine(respMsg);
    }
}
