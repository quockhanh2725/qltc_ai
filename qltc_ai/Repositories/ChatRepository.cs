using qltc_ai.Models;

namespace qltc_ai.Repositories
{
    public class ChatRepository : BaseRepository, IChatRepository
    {
        public ChatRepository(qltcContext context) : base(context)
        {
        }
    }
}
