using MongoDB.Bson.Serialization.Attributes;
namespace ThreadMapLLM.Models
{
    [BsonIgnoreExtraElements]
    public class Chat
    {
        [BsonElement]
        public required string ConversationId { get; set; }
        [BsonElement]
        public required string UserId { get; set; }
        [BsonElement]
        public required List<ChatMessageViewModel?> ChatMessages { get; set; }
        [BsonElement]
        public bool IsDeleted = false;
        
    }
}
