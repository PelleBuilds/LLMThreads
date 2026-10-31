using ThreadMapLLM.Models;

namespace ThreadMapLLM.Interfaces
{
    public interface IChatRepository
    {
        Task SaveChatAsync(Chat chat);
        Task<List<Chat>> GetAllChats(string userID);

    }
}
