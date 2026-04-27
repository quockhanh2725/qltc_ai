using qltc_ai.Models;

namespace qltc_ai.Repositories
{
    public interface ITransactionRepository
    {
        List<Giaodich> GetAll();
        List<Giaodich> GetByAccount(int accountId);
        List<Giaodich> GetByCategory(int detailId);
        List<Giaodich> GetByAccountAndMonth(int accountId, int month, int year);
        void AddTransaction(Giaodich gd);
        void Update(Giaodich gd);
        void DeleteTransaction(Giaodich gd);
        void Save();
        Giaodich? FindById(int id);
    }
}