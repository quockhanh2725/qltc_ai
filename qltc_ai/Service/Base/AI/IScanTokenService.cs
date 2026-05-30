namespace qltc_ai.Service.Base.AI
{
    public interface IScanTokenService
    {
        string GenerateToken(int accountId);
        string GetOrCreateToken(int accountId);
        bool IsValid(string? token);
        int? GetAccountId(string? token);
        void MarkDone(string token);
        bool IsDone(string token);
        bool TryConsume(string token);
    }
}
