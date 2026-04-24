namespace ThreadMapLLM.Models
{
    public class ChatViewModel
    {
        public required string ConversationId { get; set; }
        public required string UserId { get; set; } 
        public List<ChatMessageViewModel> ChatMessages { get; set; } = new();

        public bool IsDeleted = false;

        
    }
}
