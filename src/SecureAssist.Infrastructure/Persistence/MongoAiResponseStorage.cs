using MongoDB.Bson;
using MongoDB.Driver;
using SecureAssist.Application.Interfaces;
using System.Threading.Tasks;

namespace SecureAssist.Infrastructure.Persistence;

public class MongoAiResponseStorage : IAiResponseStorage
{
    private readonly IMongoCollection<BsonDocument> _collection;

    public MongoAiResponseStorage()
    {
        var connectionUri = Environment.GetEnvironmentVariable("MONGODB_CONNECTION_URI") ?? "mongodb://localhost:27017";
        var databaseName = Environment.GetEnvironmentVariable("MONGODB_DATABASE_NAME") ?? "SecureAssistDb";
        var collectionName = Environment.GetEnvironmentVariable("MONGODB_COLLECTION_NAME") ?? "AIInteractions";

        var client = new MongoClient(connectionUri);
        var database = client.GetDatabase(databaseName);
        _collection = database.GetCollection<BsonDocument>(collectionName);
    }

    public async Task StoreResponseAsync(string interactionId, string prompt, string response, string metadata)
    {
        var document = new BsonDocument
        {
            { "interactionId", interactionId },
            { "prompt", prompt },
            { "response", response },
            { "metadata", BsonDocument.Parse(metadata ?? "{}") },
            { "timestamp", DateTime.UtcNow }
        };

        await _collection.InsertOneAsync(document);
    }
}
