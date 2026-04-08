using qltc_ai.Models;

namespace qltc_ai.Repositories
{
    public interface IBudgetRepository
    {
        Ngansach? GetByMonth(int accountId, int month, int year);
        void Add(Ngansach ns);
        void Update(Ngansach ns);
        void Save();
    }
}
