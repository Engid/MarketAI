using MarketAI.Worker.Data;
using MarketAI.Worker.Integration.Responses;
using MarketReportAI.AIClient;
using System.Text.Json;

namespace MarketAI.Worker.Application;


/// <summary>
/// This service resolves the loading of todays data. it either gets it from the file system, or calls the api.
/// </summary>
public class DataCacheService
{
    private readonly IMarketDataClient _alphaClient;
    private readonly IFileStorage _fileStorage;
    private readonly Func<DateOnly> _dateProvider;
    public DataCacheService(IMarketDataClient alphaClient, IFileStorage storage)
    {
        ArgumentNullException.ThrowIfNull(alphaClient);
        ArgumentNullException.ThrowIfNull(storage);

        _alphaClient = alphaClient;
        _fileStorage = storage;

        // this could be mocked later..
        _dateProvider = () => DateOnly.FromDateTime(DateTime.UtcNow);
    }


    public async Task<List<AlphaNewsItem>> GetTodaysNewsFor(string symbol)
    {
        var name = CreateFileName(symbol);
        var existingFiles = _fileStorage.GetMatchingFileNames(name);

        if (existingFiles.Count == 0)
        {
            var response = await _alphaClient.GetNews(symbol);

            // TODO: maybe just save the raw string instead of reserializing it
            var serialized = JsonSerializer.Serialize(response);
            _fileStorage.StoreFile(name, serialized);


            return response.ToList();
        }
        else
        {
            // load data
            var fileData = _fileStorage.LoadFile(name);
            var parsed = JsonSerializer.Deserialize<List<AlphaNewsItem>>(fileData);
            
            return parsed?.ToList() ?? [];
        }

    }

    private string CreateFileName(string symbol)
    {
        var today = _dateProvider();

        var fileName = $"{symbol}_{today:yyyyMMdd}";

        return fileName;
    }
}
