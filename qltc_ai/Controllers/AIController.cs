using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using qltc_ai.Models.AI;
using qltc_ai.Service.Base;
using qltc_ai.Service.Base.AI;
using System.Text.Json;

namespace qltc_ai.Controllers
{
    [Route("ai")]
    public class AIController : Controller
    {
        private readonly AIService _aiService;
        private readonly ITransactionService _tranService;
        private readonly ChatParserService _parser;
        private readonly IInvoiceScanService _scanService;
        private readonly IScanTokenService _scanTokenService;
        private readonly IChatService _chatService;

        public AIController(
            AIService aiService,
            ITransactionService tranService,
            ChatParserService parser,
            IInvoiceScanService scanService,
            IScanTokenService scanTokenService,
            IChatService chatService)
        {
            _aiService = aiService;
            _tranService = tranService;
            _parser = parser;
            _scanService = scanService;
            _scanTokenService = scanTokenService;
            _chatService = chatService;
        }

        private int? ResolveAccountId(string? token = null)
        {
            var fromSession = HttpContext.Session.GetInt32("AccountId");
            if (fromSession != null) return fromSession;
            return _scanTokenService.GetAccountId(token);
        }

        [HttpGet("history")]
        public IActionResult GetHistory()
        {
            var accId = HttpContext.Session.GetInt32("AccountId");
            if (accId == null) return Unauthorized(new { message = "Chưa đăng nhập" });

            var messages = _chatService.GetHistory(accId.Value)
                .Select(m => new { role = m.Role, content = m.Content, time = m.Time });

            return Ok(new { messages });
        }

        [HttpPost("chat")]
        public async Task<IActionResult> Chat(string messages)
        {
            if (string.IsNullOrWhiteSpace(messages))
                return BadRequest(new { message = "Tin nhắn không được để trống" });

            List<ChatMessage>? history;
            try
            {
                history = JsonSerializer.Deserialize<List<ChatMessage>>(messages,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            catch { return BadRequest(new { message = "Định dạng messages không hợp lệ" }); }

            if (history == null || history.Count == 0)
                return BadRequest(new { message = "Tin nhắn trống" });

            var accId = HttpContext.Session.GetInt32("AccountId")!.Value;

            try
            {
                var reply = await _chatService.SendAsync(accId, history);
                return Ok(new { reply });
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(502, new { message = "Lỗi AI", detail = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpPost("/transaction/chat")]
        public IActionResult AddByChat(string text, string? token)
        {
            if (string.IsNullOrWhiteSpace(text))
                return BadRequest(new { message = "Nội dung không hợp lệ" });

            var accId = ResolveAccountId(token) ?? HttpContext.Session.GetInt32("AccountId");
            if (accId == null)
                return Unauthorized(new { message = "Phiên hết hạn, vui lòng đăng nhập lại." });

            var (note, money) = _parser.Parse(text);
            if (money <= 0)
                return BadRequest(new { message = "Không nhận diện được số tiền" });

            var result = _tranService.AddByChat(accId.Value, text, money);
            if (!result.Success)
                return BadRequest(new { message = result.Message });

            var fmt = money.ToString("N0") + "đ";
            var aiReply = $"✅ **{note ?? text}** — **{fmt}**\n{result.Message}";
            _chatService.SaveTransactionMessages(accId.Value, text, aiReply);

            return Ok(new { message = result.Message, note, money });
        }

        [AllowAnonymous]
        [HttpPost("scan-image")]
        public async Task<IActionResult> ScanImage(IFormFile image, string? token)
        {
            if (ResolveAccountId(token) == null)
                return Unauthorized(new { message = "Phiên hết hạn, vui lòng quét lại QR." });

            var result = await _scanService.ScanImageAsync(image);
            return result.Success ? Ok(result.Data) : BadRequest(new { message = result.Message });
        }

        [AllowAnonymous]
        [HttpPost("scan-qr")]
        public async Task<IActionResult> ScanQr(string qrText, string? token)
        {
            if (ResolveAccountId(token) == null)
                return Unauthorized(new { message = "Phiên hết hạn, vui lòng quét lại QR." });

            var result = await _scanService.ScanQrTextAsync(qrText);
            return result.Success ? Ok(result.Data) : BadRequest(new { message = result.Message });
        }

        [AllowAnonymous]
        [HttpPost("scan-done")]
        public IActionResult ScanDone([FromQuery] string? token)
        {
            if (string.IsNullOrWhiteSpace(token)) return BadRequest();
            if (!_scanTokenService.TryConsume(token)) return Unauthorized();
            return Ok();
        }

        [AllowAnonymous]
        [HttpGet("scan-status")]
        public IActionResult ScanStatus([FromQuery] string? token)
        {
            if (string.IsNullOrWhiteSpace(token)) return BadRequest();
            return Ok(new { done = _scanTokenService.IsDone(token) });
        }

        [HttpGet("ngrok-url")]
        public IActionResult GetNgrokUrl()
        {
            var accId = HttpContext.Session.GetInt32("AccountId");
            if (accId == null) return Unauthorized(new { message = "Chưa đăng nhập" });

            var token = _scanTokenService.GetOrCreateToken(accId.Value);
            return Ok(new { url = _scanService.GetNgrokUrl(), token });
        }

        [HttpPost("ngrok-url")]
        public IActionResult SetNgrokUrl(string url)
        {
            var accId = HttpContext.Session.GetInt32("AccountId");
            if (accId == null) return Unauthorized(new { message = "Chưa đăng nhập" });

            _scanService.SetNgrokUrl(url);
            var token = _scanTokenService.GetOrCreateToken(accId.Value);
            return Ok(new { url = _scanService.GetNgrokUrl(), token });
        }

        [AllowAnonymous]
        [HttpPost("classify")]
        public IActionResult Classify(string text)
            => Ok(new { category = _aiService.PredictCategory(text) });

        [HttpPost("retrain")]
        public IActionResult Retrain()
        {
            MLModelsTrainer.Train();
            return Ok(new { message = "Retrain success" });
        }

        [AllowAnonymous]
        [HttpGet("scan")]
        public IActionResult ScanMobile(string? token)
        {
            ViewBag.ScanToken = token ?? string.Empty;
            return View("ScanMobile");
        }
    }
}