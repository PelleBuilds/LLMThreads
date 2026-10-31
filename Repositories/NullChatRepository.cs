using ThreadMapLLM.Interfaces;
using ThreadMapLLM.Models;

namespace ThreadMapLLM.Repositories;

internal class NullChatRepository : IChatRepository
{
    public Task SaveChatAsync(Chat chat)
    {
        return Task.CompletedTask;
    }

    public Task<List<Chat>> GetAllChats(string userId)
    {
        return Task.FromResult(new List<Chat>());
    }
}
