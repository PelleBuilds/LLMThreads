using MongoDB.Driver;
using MongoDB.Bson;
using ThreadMapLLM.Models;
namespace ThreadMapLLM.Services
{
    public class MongoDBClient
    {
        private readonly IMongoCollection<ChatViewModel?> _collection;

        public MongoDBClient(IMongoClient client) 
        {
            var database = client.GetDatabase("Chats");
            _collection = database.GetCollection<ChatViewModel?>("Chats");

        }

        public async Task AddAsync(ChatViewModel chat)
        {
            using (var session = await _collection.Database.Client.StartSessionAsync())
            {
                session.StartTransaction();
                try
                {
                    await _collection.InsertOneAsync(session, chat);
                    await session.CommitTransactionAsync();
                }
                catch (Exception)
                {
                    await session.AbortTransactionAsync();
                    throw;
                }
            }
        }
    }
    
}
