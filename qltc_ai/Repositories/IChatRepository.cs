using qltc_ai.Models;

namespace qltc_ai.Repositories
{
    public interface IChatRepository
    {
        Trochuyen? GetActiveConversation(int accountId);
        Trochuyen CreateConversation(int accountId);
        void SaveMessage(int conversationId, string sender, string content);
        List<Tinnhan> GetMessages(int conversationId);
    }
}
