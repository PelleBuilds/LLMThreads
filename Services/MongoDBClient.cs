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

        public async Task UpdateOneAsync(string conversationId, ChatMessageViewModel message )
        {
            //var conversationId = chat.ConversationId;
            //var messages = chat.ChatMessages;
            using (var session = await _collection.Database.Client.StartSessionAsync())
            {
                var filter = Builders<ChatViewModel?>.Filter.Eq("ConversationId", conversationId);
                var update = Builders<ChatViewModel?>.Update.Push("ChatMessages", message);
                var options = new UpdateOptions
                {
                    IsUpsert = true
                };
                session.StartTransaction();
                try
                {
                    await _collection.UpdateOneAsync(session, filter, update, options);
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
