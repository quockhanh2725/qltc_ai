using qltc_ai.Models;

namespace qltc_ai.Repositories
{
    public interface IBudgetRepository
    {
        Ngansach? GetByMonth(int accountId, int month, int year);
        Ngansach? GetLatest(int accId);
        void AddBudget(Ngansach ns);
        void UpdateBudget(Ngansach ns);
        void Save();
        Ngansach? FindById(int id);
    }
}
