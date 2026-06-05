using System.Text;
using System.Text.Json;
using qltc_ai.Models.AI;
using qltc_ai.Repositories;
using qltc_ai.Service.Base;

namespace qltc_ai.Service
{
    public class ChatService : IChatService
    {
        private readonly IChatRepository _chatRepo;
        private readonly IAILogRepository _aiLogRepo;
        private readonly IBudgetService _budgetService;
        private readonly ICategoryService _cateService;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _config;

        public ChatService(
            IChatRepository chatRepo,
            IAILogRepository aiLogRepo,
            IBudgetService budgetService,
            ICategoryService cateService,
            IHttpClientFactory httpClientFactory,
            IConfiguration config)
        {
            _chatRepo = chatRepo;
            _aiLogRepo = aiLogRepo;
            _budgetService = budgetService;
            _cateService = cateService;
            _httpClientFactory = httpClientFactory;
            _config = config;
        }

        public List<ChatHistoryItem> GetHistory(int accountId)
        {
            var conv = _chatRepo.GetActiveConversation(accountId);
            if (conv == null) return [];

            return _chatRepo.GetMessages(conv.IdTroChuyen)
                .Select(m => new ChatHistoryItem(m.NguoiGui, m.NoiDung, m.ThoiGianGui ?? DateTime.Now))
                .ToList();
        }

        public async Task<string> SendAsync(int accountId, List<ChatMessage> history)
        {
            var conv = _chatRepo.GetActiveConversation(accountId)
                       ?? _chatRepo.CreateConversation(accountId);

            var lastUser = history.Last(m => m.Role == "user");
            _chatRepo.SaveMessage(conv.IdTroChuyen, "user", lastUser.Content);

            var reply = await CallGroqAsync(accountId, history);

            _chatRepo.SaveMessage(conv.IdTroChuyen, "assistant", reply);
            _aiLogRepo.SaveLog(accountId, lastUser.Content, reply);

            return reply;
        }

        public void SaveTransactionMessages(int accountId, string userText, string aiReply)
        {
            var conv = _chatRepo.GetActiveConversation(accountId)
                       ?? _chatRepo.CreateConversation(accountId);

            _chatRepo.SaveMessage(conv.IdTroChuyen, "user", userText);
            _chatRepo.SaveMessage(conv.IdTroChuyen, "assistant", aiReply);
            _aiLogRepo.SaveLog(accountId, userText, aiReply);
        }

        private async Task<string> CallGroqAsync(int accountId, List<ChatMessage> history)
        {
            var now = DateTime.Now;
            var budget = _budgetService.GetBudgetByMonth(accountId, now.Month, now.Year);

            var category = budget != null
                ? _cateService.GetCategoriesByBudget(budget.IdNganSach)
                    .Select(c => new
                    {
                        ten = c.IdDanhMucNavigation.TenDanhMuc,
                        gioiHan = c.GioiHanTien ?? 0,
                        daTieu = c.TienDaTieu ?? 0,
                        conLai = (c.GioiHanTien ?? 0) - (c.TienDaTieu ?? 0),
                        danhGia = c.DanhGia ?? ""
                    }).ToList<object>()
                : null;

            var budgetInfo = budget != null
                ? (object)new { tongTien = budget.TongTien ?? 0, thang = budget.Thang }
                : null;

            var prevMonth = now.Month == 1 ? 12 : now.Month - 1;
            var prevYear = now.Month == 1 ? now.Year - 1 : now.Year;
            var prevBudget = _budgetService.GetBudgetByMonth(accountId, prevMonth, prevYear)
                             ?? _budgetService.GetLatestBudget(accountId);

            List<object>? prevCategory = null;
            if (prevBudget != null && prevBudget.IdNganSach != (budget?.IdNganSach ?? 0))
            {
                prevCategory = _cateService.GetCategoriesByBudget(prevBudget.IdNganSach)
                    .Select(c => new
                    {
                        ten = c.IdDanhMucNavigation.TenDanhMuc,
                        gioiHan = c.GioiHanTien ?? 0,
                        daTieu = c.TienDaTieu ?? 0
                    }).ToList<object>();
            }

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_config["Groq:ApiKey"]}");

            var body = JsonSerializer.Serialize(new
            {
                model = "llama-3.3-70b-versatile",
                max_tokens = 1024,
                temperature = 0.7,
                messages = new object[] { new { role = "system", content = BuildSystemPrompt(budgetInfo, category , prevCategory) } }
                    .Concat(history.Select(m => new { role = m.Role, content = m.Content }))
            });

            var resp = await client.PostAsync(
                "https://api.groq.com/openai/v1/chat/completions",
                new StringContent(body, Encoding.UTF8, "application/json"));

            if (!resp.IsSuccessStatusCode)
                throw new HttpRequestException($"Groq {(int)resp.StatusCode}: {await resp.Content.ReadAsStringAsync()}");

            using var doc = JsonDocument.Parse(await resp.Content.ReadAsStringAsync());
            return doc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString() ?? "...";
        }

        private static string BuildSystemPrompt(object? budget, object? category , object? prevCategory)
        {
            var budgetJson = budget != null ? JsonSerializer.Serialize(budget) : "{}";
            var danhMucJson = category != null ? JsonSerializer.Serialize(category) : "[]";
            var prevJson = prevCategory != null ? JsonSerializer.Serialize(prevCategory) : "[]";

            return $"""
                Bạn là trợ lý tài chính AI của ứng dụng "Ví Thông Minh".

                Ngân sách tháng này:
                {budgetJson}

                Chi tiết từng danh mục (đã tiêu / giới hạn):
                {danhMucJson}

                Chi tiết danh mục tháng trước (dùng làm tỉ lệ tham khảo khi phân bổ):
                {prevJson}

                Phạm vi hỗ trợ:
                • Chỉ trả lời các câu hỏi liên quan đến tài chính cá nhân: chi tiêu, thu nhập, ngân sách, tiết kiệm, đầu tư, nợ, mục tiêu tài chính.
                • Nếu câu hỏi KHÔNG liên quan đến tài chính, chỉ trả lời đúng 1 câu: "Xin lỗi, tôi chỉ hỗ trợ các vấn đề về tài chính cá nhân." — không giải thích thêm.

                Quy tắc phân bổ ngân sách:
                • Khi gợi ý phân bổ, CHỈ dùng đúng tên danh mục có trong dữ liệu "Chi tiết từng danh mục" ở trên — không được tự đặt tên mới.
                • Phải liệt kê ĐẦY ĐỦ tất cả danh mục loại ChiTieu trong danh sách — không được bỏ sót bất kỳ danh mục nào.
                • Không gom nhóm thành "Danh mục khác" hay bất kỳ tên nào ngoài danh sách.
                • Tổng số tiền phân bổ phải bằng đúng số tiền người dùng yêu cầu — phân bổ phần còn lại vào danh mục cuối nếu cần.

                Phong cách trả lời:
                • Tiếng Việt thân thiện, ngắn gọn, thực tế
                • Tối đa 3 câu hoặc 2 gạch đầu dòng ngắn — không viết dài
                • Dùng số liệu cụ thể từ dữ liệu trên, **in đậm** số quan trọng
                • Dùng emoji hợp lý (không quá 2/tin)
                • Không bịa số liệu ngoài dữ liệu được cung cấp
                """;
        }
    }
}