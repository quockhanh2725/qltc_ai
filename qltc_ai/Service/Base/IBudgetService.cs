namespace qltc_ai.Service.Base
{
    public interface IBudgetService
    {
        bool AddBudget(int accId, decimal money);
        void AutoResetIfNeeded(int accId);
        bool AutoAddNewAccount(int accId);
    }
}
