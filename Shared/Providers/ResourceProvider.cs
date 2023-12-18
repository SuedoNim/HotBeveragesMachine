using Shared.Entities;
using Shared.Interfaces;

namespace Shared.Providers;

public class ResourceProvider<T> : IDisposable where T : IEntry
{
    private readonly IDocumentDataBaseAPI _dataService;
    private readonly string collectionName;
    public ResourceProvider(IDocumentDataBaseAPI dataService)
    {
        _dataService = dataService;
        collectionName = typeof(T).Name;
    }

    public List<T> Get()
    {
        return _dataService.LoadDocuments<T>(collectionName);
    }
    public void Create(T entry)
    {
        _dataService.InsertDocument(collectionName, entry);
    }
    public void Update(T entry)
    {
        _dataService.UpsertDocument(collectionName, entry);
    }
    public void Remove(T entry)
    {
        _dataService.DeleteRecord<T>(collectionName,entry.Id);
    }
    public void Dispose()
    {
        _dataService?.Dispose();
    }
}
