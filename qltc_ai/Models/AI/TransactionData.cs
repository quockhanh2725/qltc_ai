using Microsoft.ML.Data;

namespace qltc_ai.Models.AI
{
    public class TransactionData
    {
        [LoadColumn(0)]
        
        public string? Text { get; set; }

        [LoadColumn(1)]
        
        public string? Label { get; set; }
    }
}
