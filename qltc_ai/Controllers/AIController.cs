using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.Crmf;
using qltc_ai.Models.AI;
using qltc_ai.Service.Base.AI;
using System.Net.Http;
using System.Text.Json;
using System.Text;
using qltc_ai.Service.Base;

namespace qltc_ai.Controllers
{
    [Route("ai")]
    public class AIController : Controller
    {
        private readonly AIService _aiService;
        private readonly ITransactionService _tranService;
        private readonly IBudgetService _budgetService;
        private readonly ChatParserService _parser;
        private readonly IConfiguration _config;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ICategoryService _cateService;

        public AIController(
            AIService aiService,
            ITransactionService tranService,
            IBudgetService budgetService,
            ChatParserService parser,
            IConfiguration config,
            IHttpClientFactory httpClientFactory,
            ICategoryService cateService)
        {
            _aiService = aiService;
            _tranService = tranService;
            _budgetService = budgetService;
            _parser = parser;
            _config = config;
            _httpClientFactory = httpClientFactory;
            _cateService = cateService;
        }

        [HttpPost("classify")]
        public IActionResult Classify(string text)
        {
            int category = _aiService.PredictCategory(text);

            return Ok(new
            {
                category
            });
        }

        [HttpPost("retrain")]
        public IActionResult Retrain()
        {
            MLModelsTrainer.Train();

            return Ok(new
            {
                message = "Retrain success"
            });
        }
    }
}