using Microsoft.EntityFrameworkCore;
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

        public List<object> GetAll()
        {
            return _context.Taikhoan
                .Include(n => n.Nguoidung)
                .Select(t => (object)new
                {
                    t.IdTaiKhoan,
                    t.RoleId,
                    t.Email,
                    t.IsActive,
                    t.NgayTao,
                    Nguoidung = t.Nguoidung,
                    SoGiaoDich = t.Giaodich.Count()
                })
                .ToList();
        }

        public Taikhoan? GetByEmailAndPassword(string email, string password)
        {
            var hashed = Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(
                System.Text.Encoding.UTF8.GetBytes(password)))
                .ToLower();

            return _context.Taikhoan
                .Include(a => a.Nguoidung)
                .FirstOrDefault(u => u.Email == email && u.MatKhau == hashed);
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
