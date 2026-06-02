namespace qltc_ai.Repositories
{
    public interface IAILogRepository
    {
        void SaveLog(int accountId, string question, string answer);
    }
}
