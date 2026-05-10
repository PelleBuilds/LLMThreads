using MongoDB.Driver;
using MongoDB.Bson;
using ThreadMapLLM.Models;
using System.Security.Cryptography.X509Certificates;
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

        //public async Task AddAsync(Chat chat, string userId)
        //{
        //    bool existingChat = false;
        //    foreach(Chat c in await GetAllChats(userId))
        //    {
        //        if(chat.ConversationId == c.ConversationId)
        //        {
        //            existingChat = true;
        //        }
        //    }
        //    if (existingChat == false)
        //    {
        //        using (var session = await _collection.Database.Client.StartSessionAsync())
        //        {

        //            session.StartTransaction();
        //            try
        //            {

        //                await _collection.InsertOneAsync(session, chat);
        //                await session.CommitTransactionAsync();
        //            }
        //            catch (Exception)
        //            {
        //                await session.AbortTransactionAsync();
        //                throw;
        //            }
        //        }
        //    }
        //    else
        //    {
                

        //    }
            
        //}
        public async Task SaveChatAsync(Chat chat)
        {
            var filter = Builders<Chat>.Filter.Eq(
                c => c.ConversationId,
                chat.ConversationId
            );

            var existingChat = await _collection
                .Find(filter)
                .FirstOrDefaultAsync();

            // Finns inte -> spara HELA objektet
            if (existingChat == null)
            {
                await _collection.InsertOneAsync(chat);
                return;
            }

            // Finns -> lägg bara till nya messages
            if (chat.ChatMessages != null && chat.ChatMessages.Any())
            {
                var update = Builders<Chat>.Update.PushEach(
                    c => c.ChatMessages,
                    chat.ChatMessages
                );

                await _collection.UpdateOneAsync(filter, update);
            }
        }
        //public async Task UpdateOneAsync(string conversationId, ChatMessageViewModel message )
        //{
        //    //var conversationId = chat.ConversationId;
        //    //var messages = chat.ChatMessages;
        //    using (var session = await _collection.Database.Client.StartSessionAsync())
        //    {
        //        var filter = Builders<Chat>.Filter.Eq("ConversationId", conversationId);
        //        var update = Builders<Chat>.Update.Push("ChatMessages", message);
        //        var options = new UpdateOptions
        //        {
        //            IsUpsert = true
        //        };
        //        session.StartTransaction();
        //        try
        //        {
        //            await _collection.UpdateOneAsync(session, filter, update, options);
        //            await session.CommitTransactionAsync();
        //        }
        //        catch (Exception)
        //        {
        //            await session.AbortTransactionAsync();
        //            throw;
        //        }
        //    }
        //}

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
