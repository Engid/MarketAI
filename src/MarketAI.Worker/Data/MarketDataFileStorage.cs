
namespace MarketAI.Worker.Data;

public interface IFileStorage
{
    public void StoreFile(string fileName, string fileContents);
    public string LoadFile(string fileName);
    List<string> GetMatchingFileNames(string searchTerm);

}

// Store to local file system for now.. later consider Auzrite emulator for blob storage
public class MarketDataFileStorage : IFileStorage
{
    private readonly string _storageDirectory;

    public MarketDataFileStorage()
    {
        // Define a directory for storing files (e.g., "DataStorage" in the current working directory)
        _storageDirectory = Path.Combine(Directory.GetCurrentDirectory(), "DataStorage");

        // Ensure the directory exists
        if (!Directory.Exists(_storageDirectory))
        {
            Directory.CreateDirectory(_storageDirectory);
        }
    }

    public void StoreFile(string fileName, string fileContents)
    {
        // Combine the directory path with the file name
        var filePath = Path.Combine(_storageDirectory, fileName);

        // Write the file contents to the specified file
        File.WriteAllText(filePath, fileContents);
    }

    public string LoadFile(string fileName)
    {
        // Combine the directory path with the file name
        var filePath = Path.Combine(_storageDirectory, fileName);

        // Check if the file exists
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"The file '{fileName}' was not found in the storage directory.");
        }

        // Read and return the file contents
        return File.ReadAllText(filePath);
    }

    /// <summary>
    /// Returns files that match the search term. Only returns the full file name (not including path)
    /// </summary>
    /// <param name="searchTerm"></param>
    /// <returns></returns>
    public List<string> GetMatchingFileNames(string searchTerm)
    {
        // Get all files in the storage directory
        var files = Directory.GetFiles(_storageDirectory);

        // Filter files that contain the search term (case-insensitive)
        var matchingFiles = files
            .Where(file => Path.GetFileName(file).Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
            .Select(filename => Path.GetFileName(filename) ?? "") // Return only the file names, not full paths
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .ToList();

        return matchingFiles ?? [];
    }
}
