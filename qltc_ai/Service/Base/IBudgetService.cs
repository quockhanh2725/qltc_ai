using qltc_ai.Models;

namespace qltc_ai.Service.Base
{
    public interface IBudgetService
    {
        void AutoResetIfNeeded(int accId);
        bool AutoAddNewAccount(int accId);
        Ngansach? GetBudgetByMonth(int accid , int month , int year);
        Ngansach? GetLatestBudget(int budgetId);
    }
}
