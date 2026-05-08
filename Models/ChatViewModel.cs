namespace ThreadMapLLM.Models
{
    public class ChatViewModel
    {
        public required string ConversationId { get; set; }
        public required string UserId { get; set; } 
        public required List<ChatMessageViewModel?> ChatMessages { get; set; }

        public bool IsDeleted = false;

        
    }
}
