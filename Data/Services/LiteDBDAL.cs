using LiteDB;
using Shared.Interfaces;

namespace Data.Services;
//1.    Can I extend T to include an ID property for duplicate insertion protection.
public class LiteDBDAL : IDocumentDataBaseAPI
{
    private const string _dbName = "LiteDB";
    public string DatabaseName => _dbName;

    private const string _dbDir = @"Data.db";
    private readonly LiteDatabase _db;
    public LiteDBDAL()
    {
        _db = new LiteDatabase(_dbDir);
    }

    public bool DeleteCollection(string collectionName)
    {
        if (string.IsNullOrWhiteSpace(collectionName))
            throw new ArgumentNullException("table undefined");
        
        return _db.DropCollection(collectionName);
    }

    public void DeleteRecord<T>(string table, Guid id)
    {
        if (string.IsNullOrWhiteSpace(table))
            throw new ArgumentNullException("table undefined");
        
        var collection = _db.GetCollection(table);
        collection.Delete(id);
    }

    public void DeleteRecord<T>(string table, int id)
    {
        if (string.IsNullOrWhiteSpace(table))
            throw new ArgumentNullException("table undefined");
        
        var collection = _db.GetCollection(table);
        collection.Delete(id);
    }

    public bool InsertDocument<T>(string collectionName, T document)
    {
        if (string.IsNullOrWhiteSpace(collectionName))
            throw new ArgumentNullException("table undefined");
        
        var collection = _db.GetCollection<T>(collectionName);
        collection.Insert(document);
        
        return true;
    }

    public bool InsertMany<T>(string collectionName, List<T> collection, bool deleteCurrentCollection = true)
    {
        if (string.IsNullOrWhiteSpace(collectionName))
            throw new ArgumentNullException("table undefined");
            
        var dbcollection = _db.GetCollection<T>(collectionName);
        dbcollection.InsertBulk(collection);

        return collection.All(document => dbcollection.Exists(entry=>entry.Equals(document)));
    }

    public bool IsCollectionExists(string collectionName)
    {
        if (string.IsNullOrWhiteSpace(collectionName))
            throw new ArgumentNullException("table undefined");
        
        return _db.CollectionExists(collectionName);
    }

    public List<T> LoadDocuments<T>(string collectionName)
    {
        if (string.IsNullOrWhiteSpace(collectionName))
            throw new ArgumentNullException("table undefined");
        
        return _db.GetCollection<T>(collectionName).FindAll().ToList();
    }

    public bool UpsertDocument<T>(string collectionName, T document)
    {
        if (string.IsNullOrWhiteSpace(collectionName))
            throw new ArgumentNullException("table undefined");
        
        return _db.GetCollection<T>(collectionName).Update(document);
    }

    public void Dispose()
    {
        _db?.Dispose();
    }
}
