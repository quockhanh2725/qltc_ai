using qltc_ai.Models;

namespace qltc_ai.Repositories
{
    public class AccountRepository : BaseRepository, IAccountRepository
    {
        public AccountRepository(qltcContext context) : base(context)
        {
        }

        public List<Taikhoan> GetAll()
        {
            return _context.Taikhoan.ToList();
        }
    }
}
