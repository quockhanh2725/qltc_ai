namespace qltc_ai.Service.Base
{
    public interface ITransactionService
    {
        bool AddTransaction(int accid, int idCate, decimal money, string note);
        bool UpdateTransaction(int accid , int idTran , decimal newMoney, string newNote);
        bool DeleteTransaction(int accid, int idTran);
    }
}
