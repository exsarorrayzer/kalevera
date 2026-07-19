using System.IO;
using Kalevera.Models;
using Newtonsoft.Json;

namespace Kalevera.Services;

public class StorageService
{
    private readonly string _basePath;

    public StorageService()
    {
        var appData = System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData);
        _basePath = Path.Combine(appData, "Kalevera");
        Directory.CreateDirectory(_basePath);
    }

    private string CollectionsPath => Path.Combine(_basePath, "collections.json");
    private string HistoryPath => Path.Combine(_basePath, "history.json");
    private string EnvironmentsPath => Path.Combine(_basePath, "environments.json");

    public List<Collection> LoadCollections()
    {
        return LoadFromFile<List<Collection>>(CollectionsPath) ?? new();
    }

    public void SaveCollections(List<Collection> collections)
    {
        SaveToFile(CollectionsPath, collections);
    }

    public List<RequestHistoryEntry> LoadHistory()
    {
        return LoadFromFile<List<RequestHistoryEntry>>(HistoryPath) ?? new();
    }

    public void SaveHistory(List<RequestHistoryEntry> history)
    {
        SaveToFile(HistoryPath, history);
    }

    public List<Models.Environment> LoadEnvironments()
    {
        return LoadFromFile<List<Models.Environment>>(EnvironmentsPath) ?? new();
    }

    public void SaveEnvironments(List<Models.Environment> environments)
    {
        SaveToFile(EnvironmentsPath, environments);
    }

    private T? LoadFromFile<T>(string path) where T : class
    {
        if (!File.Exists(path))
            return null;

        var json = File.ReadAllText(path);
        return JsonConvert.DeserializeObject<T>(json);
    }

    private void SaveToFile<T>(string path, T data)
    {
        var json = JsonConvert.SerializeObject(data, Formatting.Indented);
        File.WriteAllText(path, json);
    }
}
