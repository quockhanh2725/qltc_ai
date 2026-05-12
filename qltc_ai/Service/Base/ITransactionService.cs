using qltc_ai.Models;
using qltc_ai.Models.AI;

namespace qltc_ai.Service.Base
{
    public interface ITransactionService
    {
        List<Giaodich> GetByAccount(int accId);
        List<Giaodich> GetByAccountAndMonth(int accId, int month, int year);
        ServiceResult AddTransaction(int accId, int idDetail, decimal money, string note, string typeTran);
        ServiceResult AddByChat(int accId, string text, decimal money);
        bool UpdateTransaction(int accId, int idTran, decimal newMoney, string newNote);
        bool DeleteTransaction(int idTran);
    }
}
