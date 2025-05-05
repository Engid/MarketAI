using MarketAI.Worker.Data;
using MarketAI.Worker.Integration.Responses;
using MarketReportAI.AIClient;

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


    public List<AlphaNewsItem> GetTodaysNewsFor(string symbol)
    {
        var name = CreateFileName(symbol);
        var existingFiles = _fileStorage.GetMatchingFileNames(name);

        List<AlphaNewsItem> data = [];

        if (existingFiles.Count == 0)
        {
            var response = _alphaClient.GetNews(symbol);

            // save to file and return data
        }
        else
        {
            // load data
        }

        return data;
    }

    private string CreateFileName(string symbol)
    {
        var today = _dateProvider();

        var fileName = $"{symbol}_{today:YYYYMMDD}";

        return fileName;
    }
}
