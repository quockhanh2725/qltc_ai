using qltc_ai.Models;

namespace qltc_ai.Repositories
{
    public interface IBudgetRepository
    {
        Ngansach? GetByMonth(int accountId, int month, int year);
        Ngansach? GetLatest(int accId);
        void Add(Ngansach ns);
        void Update(Ngansach ns);
        void Save();
        Ngansach? GetBudgetById(int id);
    }
}
