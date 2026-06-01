using qltc_ai.Models;

namespace qltc_ai.Repositories
{
    public class ChatRepository : BaseRepository, IChatRepository
    {
        public ChatRepository(qltcContext context) : base(context)
        {
        }

        public Trochuyen? GetActiveConversation(int accountId)
        {
            return _context.Trochuyen
                .FirstOrDefault(t => t.IdTaiKhoan == accountId && t.TrangThai == "DangHoatDong");
        }

        public Trochuyen CreateConversation(int accountId)
        {
            var conv = new Trochuyen
            {
                IdTaiKhoan = accountId,
                TrangThai = "DangHoatDong",
                NgayTao = DateTime.Now
            };
            _context.Trochuyen.Add(conv);
            _context.SaveChanges();
            return conv;
        }

        public void SaveMessage(int conversationId, string sender, string content)
        {
            var msg = new Tinnhan
            {
                IdTroChuyen = conversationId,
                NguoiGui = sender,
                NoiDung = content,
                ThoiGianGui = DateTime.Now
            };
            _context.Tinnhan.Add(msg);
            _context.SaveChanges();
        }

        public List<Tinnhan> GetMessages(int conversationId)
        {
            return _context.Tinnhan
                .Where(t => t.IdTroChuyen == conversationId)
                .OrderBy(t => t.ThoiGianGui)
                .ToList();
        }
    }
}
