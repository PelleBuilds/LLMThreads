using Microsoft.AspNetCore.Identity;
namespace ThreadMapLLM.Models
{
    public class User : IdentityUser
    {
        public required string UserId { get; set; }
        public required string Username { get; set; }
        public required string Password { get; set; }
        public List<ChatViewModel>? Chats;


    }
}
