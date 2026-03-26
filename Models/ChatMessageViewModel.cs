namespace ThreadMapLLM.Models
{
    public class ChatMessageViewModel
    {
        public int? Id { get; set; }
        public string? Content { get; set; }
        public string? Role { get; set; }
        public DateTime? TimeStamp { get; set; }
    }
}
