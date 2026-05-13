using Microsoft.ML.Data;

namespace qltc_ai.Models.AI
{
    public class TransactionPrediction
    {
        [ColumnName("PredictedLabel")]
        public string? PredictedLabel { get; set; }

        public float[]? Score { get; set; }
    }
}