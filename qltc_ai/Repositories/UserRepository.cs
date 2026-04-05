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

        public bool IsUsernameTaken(string username)
        {
            return _context.Nguoidung.Any(n => n.TenNguoiDung == username);
        }

        public void Save()
        {
           _context.SaveChanges();
        }
    }
}
