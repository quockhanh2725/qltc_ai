using qltc_ai.Models;

namespace qltc_ai.Repositories
{
    public class TransactionRepository : BaseRepository, ITransactionRepository
    {
        public TransactionRepository(qltcContext context) : base(context)
        {
        }
    }
}
