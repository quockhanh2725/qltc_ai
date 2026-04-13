namespace qltc_ai.Service.Base
{
    public interface ITransactionService
    {
        bool AddTransaction(int accid, int idCate, decimal money, string note);
    }
}
