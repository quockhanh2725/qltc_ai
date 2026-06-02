using qltc_ai.Models.AI;

namespace qltc_ai.Service.Base
{
    public interface IChatService
    {
        List<ChatHistoryItem> GetHistory(int accountId);
        Task<string> SendAsync(int accountId, List<ChatMessage> history);
        void SaveTransactionMessages(int accountId, string userText, string aiReply);
    }
}
