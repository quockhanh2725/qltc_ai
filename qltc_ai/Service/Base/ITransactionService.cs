using qltc_ai.Models;

namespace qltc_ai.Service.Base
{
    public interface ITransactionService
    {
        List<Giaodich> GetByAccount(int accId);
        List<Giaodich> GetByAccountAndMonth(int accId, int month, int year);
        bool AddTransaction(int accId, int idDetail, decimal money, string note, string typeTran);
        bool UpdateTransaction(int accId, int idTran, decimal newMoney, string newNote);
        bool DeleteTransaction(int idTran);
    }
}
