using Microsoft.AspNetCore.Identity;
namespace ThreadMapLLM.Models
{
    public class User : IdentityUser
    {

        public List<ChatViewModel>? Chats;


    }
}
