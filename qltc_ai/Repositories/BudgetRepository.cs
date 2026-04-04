using qltc_ai.Models;

namespace qltc_ai.Repositories
{
    public class BudgetRepository : BaseRepository, IBudgetRepository
    {
        public BudgetRepository(qltcContext context) : base(context)
        {
        }
    }
}
