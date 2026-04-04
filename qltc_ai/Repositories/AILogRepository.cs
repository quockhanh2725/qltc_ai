using qltc_ai.Models;

namespace qltc_ai.Repositories
{
    public class AILogRepository : BaseRepository, IAILogRepository
    {
        public AILogRepository(qltcContext context) : base(context)
        {
        }
    }
}
