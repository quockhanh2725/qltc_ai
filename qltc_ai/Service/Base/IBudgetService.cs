using qltc_ai.Models;

namespace qltc_ai.Service.Base
{
    public interface IBudgetService
    {
        bool AddBudget(int accId, decimal money);
        void AutoResetIfNeeded(int accId);
        bool AutoAddNewAccount(int accId);
        Ngansach? GetBudgetByMonth(int accid , int month , int year);
    }
}
