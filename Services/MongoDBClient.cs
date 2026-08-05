using MongoDB.Driver;
using MongoDB.Bson;
using ThreadMapLLM.Models;
namespace ThreadMapLLM.Services
{
    public class MongoDBClient
    {
        private readonly IMongoCollection<Chat> _collection;

        public MongoDBClient(IMongoClient client)
        {
            var database = client.GetDatabase("Chats");
            _collection = database.GetCollection<Chat>("Chats");

        }

      
        public async Task SaveChatAsync(Chat chat)
        {
            var filter = Builders<Chat>.Filter.Eq(
                c => c.ConversationId,
                chat.ConversationId
            );

            var existingChat = await _collection
                .Find(filter)
                .FirstOrDefaultAsync();

            if (existingChat == null)
            {
                await _collection.InsertOneAsync(chat);
                return;
            }

            if (chat.ChatMessages != null && chat.ChatMessages.Any())
            {
                var update = Builders<Chat>.Update.PushEach(
                    c => c.ChatMessages,
                    chat.ChatMessages
                );

                await _collection.UpdateOneAsync(filter, update);
            }
        }
        

        public async Task<List<Chat>> GetAllChats(string userID)
        {
            var filter = Builders<Chat>.Filter.Eq(c => c.UserId, userID);
            try
            {
                var cursor = await _collection.FindAsync(filter);
                var chats = await cursor.ToListAsync();
                return chats;

            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
