using System.Text;
using System.Text.Json;

namespace qltc_ai.Service.Base.AI
{
    public class InvoiceScanService : IInvoiceScanService
    {
        private readonly IHttpClientFactory _http;
        private readonly string _groqApiKey;
        private readonly string? _defaultNgrokUrl;
        private static string? _ngrokUrl;

        private const string GroqUrl = "https://api.groq.com/openai/v1/chat/completions";
        private const string GroqModel = "meta-llama/llama-4-scout-17b-16e-instruct";

        private const string Prompt = """
            Phân tích hoá đơn / QR và trả về ĐÚNG 2 dòng, không thêm gì khác:

            GHI CHÚ: tôi <động từ> <mô tả> <tại [địa điểm] nếu có> <số tiền>
            SỐ TIỀN: <chỉ chữ số>

            Ví dụ:
            GHI CHÚ: tôi mua cà phê tại Highlands 45000 
            SỐ TIỀN: 45000

            GHI CHÚ: tôi thanh toán điện tháng 6 tại EVN 320000 
            SỐ TIỀN: 320000

            Quy tắc: luôn bắt đầu bằng "tôi", bỏ địa điểm nếu không rõ, SỐ TIỀN: 0 nếu không xác định được.
            """;

        public InvoiceScanService(IConfiguration config, IHttpClientFactory http)
        {
            _groqApiKey = config["Groq:ApiKey"] ?? throw new Exception("Thiếu Groq:ApiKey");
            _http = http;
            _defaultNgrokUrl = config["Ngrok:Url"]?.TrimEnd('/');
        }

        public string? GetNgrokUrl() => _ngrokUrl ?? _defaultNgrokUrl;
        public void SetNgrokUrl(string url) => _ngrokUrl = url.TrimEnd('/');

        
        public async Task<ScanResult> ScanImageAsync(IFormFile image)
        {
            if (image is null || image.Length == 0) return ScanResult.Fail("Không có file ảnh.");
            if (image.Length > 10 * 1024 * 1024) return ScanResult.Fail("File quá lớn (tối đa 10MB).");

            try
            {
                await using var ms = new MemoryStream();
                await image.CopyToAsync(ms);
                var base64 = Convert.ToBase64String(ms.ToArray());
                var dataUrl = $"data:{image.ContentType};base64,{base64}";

                var payload = new
                {
                    model = GroqModel,
                    messages = new[]
                    {
                        new { role = "user", content = new object[]
                        {
                            new { type = "text",      text      = Prompt },
                            new { type = "image_url", image_url = new { url = dataUrl } }
                        }}
                    },
                    max_tokens = 128,
                    temperature = 0.1
                };

                var (note, money) = await CallGroq(payload);
                return ScanResult.Ok(note, money);
            }
            catch (Exception ex) { return ScanResult.Fail("Lỗi phân tích ảnh: " + ex.Message); }
        }

        public Task<ScanResult> ScanQrTextAsync(string qrText)
        {
            throw new NotImplementedException();
        }
    }
}