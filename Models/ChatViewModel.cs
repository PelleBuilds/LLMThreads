namespace ThreadMapLLM.Models
{
    public class ChatViewModel
    {
        public string? ConversationId { get; set; }
        public string? UserId { get; set; }
        public List<ChatMessageViewModel>? ChatMessages { get; set; }

        public bool IsDeleted = false;

        
    }
}
