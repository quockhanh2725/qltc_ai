using qltc_ai.Models;

namespace qltc_ai.Repositories
{
    public class AccountRepository : BaseRepository, IAccountRepository
    {
        public AccountRepository(qltcContext context) : base(context)
        {
        }

        public void AddAccount(Taikhoan tk)
        {
            _context.Taikhoan.Add(tk);
        }

        public void DeleteAccount(Taikhoan tk)
        {
            _context.Taikhoan.Remove(tk);
        }

        public Taikhoan? FindById(int id)
        {
            return _context.Taikhoan.Find(id);
        }

        public List<Taikhoan> GetAll()
        {
            return _context.Taikhoan.ToList();
        }

        public Taikhoan? GetByEmailAndPassword(string email, string password)
        {
            return _context.Taikhoan.FirstOrDefault(u => u.Email == email && u.MatKhau == password);
        }

        public bool IsEmailTaken(string email)
        {
            return _context.Taikhoan.Any(e => e.Email == email);
        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
