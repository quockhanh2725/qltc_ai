using qltc_ai.Models;
using qltc_ai.Repositories;

namespace qltc_ai.Service.Base
{
    public class BudgetService : IBudgetService
    {
        private readonly IBudgetRepository _repo;
        public BudgetService(IBudgetRepository repo)
        {
            _repo = repo;
        }

        public bool addBudget(int accId, decimal money)
        {
            if (money <= 0)
                return false;

            var now = DateTime.Now;
            var ns = _repo.GetByMonth(accId, now.Month, now.Year);

            if (ns == null)
            {
                var timeNow = new DateTime(now.Year, now.Month, 1);

                ns = new Ngansach
                {
                    IdTaiKhoan = accId,
                    TongTien = money,
                    Thang = timeNow
                };

                _repo.Add(ns);
                _repo.Save();

                return true;
            }
            ns.TongTien = ns.TongTien + money;

            _repo.Update(ns);
            _repo.Save();

            return true;
        }
    }
}
