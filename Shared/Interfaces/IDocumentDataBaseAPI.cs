namespace Shared.Interfaces;

public interface IDocumentDataBaseAPI : IDisposable
{
    bool InsertDocument<T>(string collectionName, T document);
    bool InsertMany<T>(string collectionName, List<T> collection, bool deleteCurrentCollection = true);
    List<T> LoadDocuments<T>(string collectionName);
    bool DeleteCollection(string collectionName);
    bool IsCollectionExists(string collectionName);
    bool UpsertDocument<T>(string collectionName, T document);
    void DeleteRecord<T>(string table, Guid id);
    void DeleteRecord<T>(string table, int id);
    string DatabaseName { get; }
}
