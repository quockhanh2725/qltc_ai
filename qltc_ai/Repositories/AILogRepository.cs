using qltc_ai.Models;

namespace qltc_ai.Repositories
{
    public class AILogRepository : BaseRepository, IAILogRepository
    {
        public AILogRepository(qltcContext context) : base(context)
        {

        }

        public void SaveLog(int accountId, string question, string answer)
        {
            var log = new AiLog
            {
                IdTaiKhoan = accountId,
                CauHoi = question.Length > 255 ? question[..255] : question,
                TraLoi = answer.Length > 255 ? answer[..255] : answer,
                NgayTao = DateTime.Now
            };
            _context.AiLog.Add(log);
            _context.SaveChanges();
        }
    }
}
