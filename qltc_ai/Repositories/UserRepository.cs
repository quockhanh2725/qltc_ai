using qltc_ai.Models;

namespace qltc_ai.Repositories
{
    public class UserRepository : BaseRepository, IUserRepository
    {
        public UserRepository(qltcContext context) : base(context)
        {
        }
    }
}
