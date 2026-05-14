using Microsoft.ML;
using qltc_ai.Models.AI;

namespace qltc_ai.Service.Base.AI
{
    public class MLModelsTrainer
    {
        public static void Train()
        {
            var ml = new MLContext();

            var data = ml.Data.LoadFromTextFile<TransactionData>(
                path: "data/data.txt",
                hasHeader: false,
                separatorChar: '|'
            );

            var pipeline =
    ml.Transforms.Text.FeaturizeText(
        outputColumnName: "Features",
        inputColumnName: nameof(TransactionData.Text))
    .Append(
        ml.Transforms.Conversion.MapValueToKey(
            outputColumnName: "Label",
            inputColumnName: nameof(TransactionData.Label)))
    .Append(
        ml.MulticlassClassification.Trainers.SdcaMaximumEntropy(
            labelColumnName: "Label",
            featureColumnName: "Features"))
    .Append(
        ml.Transforms.Conversion.MapKeyToValue(
            outputColumnName: "PredictedLabel",
            inputColumnName: "PredictedLabel"));

            var model = pipeline.Fit(data);
            ml.Model.Save(model, data.Schema, "data/model.zip");
        }
    }
}