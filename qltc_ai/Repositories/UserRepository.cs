using qltc_ai.Models;

namespace qltc_ai.Repositories
{
    public class UserRepository : BaseRepository, IUserRepository
    {
        public UserRepository(qltcContext context) : base(context)
        {
        }

        public void addUser(Nguoidung ng)
        {
           _context.Nguoidung.Add(ng);
        }

        public Nguoidung? GetByAccountId(int accId)
        {
            return _context.Nguoidung.FirstOrDefault(u => u.IdTaiKhoan == accId);
        }

        public bool IsUsernameTaken(string username)
        {
            return _context.Nguoidung.Any(n => n.TenNguoiDung == username);
        }

        public void Save()
        {
           _context.SaveChanges();
        }

        public void updateUser(Nguoidung ng)
        {
            _context.Update(ng);
        }
    }
}
