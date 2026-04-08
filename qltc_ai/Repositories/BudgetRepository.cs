using qltc_ai.Models;

namespace qltc_ai.Repositories
{
    public class BudgetRepository : BaseRepository, IBudgetRepository
    {
        public BudgetRepository(qltcContext context) : base(context)
        {
        }

        public void Add(Ngansach ns)
        {
            _context.Add(ns);
        }

        public Ngansach? GetByMonth(int accountId, int month, int year)
        {
            return _context.Ngansach.FirstOrDefault(d => d.IdTaiKhoan == accountId && d.Thang.Value.Month == month && d.Thang.Value.Year == year);
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        public void Update(Ngansach ns)
        {
            _context.Ngansach.Update(ns);
        }
    }
}
