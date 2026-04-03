using Microsoft.EntityFrameworkCore;

namespace qltc_ai.Models
{
    public partial class qltcContext : DbContext
    {
        public qltcContext() { }

        public qltcContext(DbContextOptions<qltcContext> options) : base(options)
        {

        }
    }
}
