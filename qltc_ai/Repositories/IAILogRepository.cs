using qltc_ai.Models.AI;

namespace qltc_ai.Repositories
{
    public interface IAILogRepository
    {
        void SaveLog(int accountId, string question, string answer);
        List<AiLogDto> GetAll();
        void DeleteAll();
    }
}
