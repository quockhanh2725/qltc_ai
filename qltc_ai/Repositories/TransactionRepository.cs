using qltc_ai.Models;

namespace qltc_ai.Repositories
{
    public class TransactionRepository : BaseRepository, ITransactionRepository
    {
        public TransactionRepository(qltcContext context) : base(context)
        {
        }

        public void AddTransaction(Giaodich gd)
        {
            _context.Giaodich.Add(gd);
        }

        public void DeleteTransaction(Giaodich gd)
        {
            _context.Remove(gd);
        }

        public List<Giaodich>? GetAll()
        {
            return _context.Giaodich.ToList();
        }

        public Giaodich? FindById(int id)
        {
            return _context.Giaodich.Find(id);
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        public void Update(Giaodich gd)
        {
           _context.Update(gd);
        }
    }
}
