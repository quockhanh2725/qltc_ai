using Microsoft.ML.Data;
using Microsoft.ML;
using qltc_ai.Models.AI;
using qltc_ai.Service.Base.AI;

public class AIService
{
    private ITransformer _model;
    private MLContext _ml;
    private PredictionEngine<TransactionInput, TransactionPrediction> _engine;
    private DataViewSchema _schema;

    public void Init()
    {
        _ml = new MLContext();
        var path = Path.Combine(Directory.GetCurrentDirectory(), "data", "model.zip");

        if (!File.Exists(path))
            MLModelsTrainer.Train();


        _model = _ml.Model.Load(path, out _schema);


        _engine = _ml.Model.CreatePredictionEngine<TransactionInput, TransactionPrediction>(
            _model, inputSchemaDefinition: SchemaDefinition.Create(typeof(TransactionInput)));
    }

    public int PredictCategory(string text)
    {
        var result = _engine.Predict(new TransactionInput { Text = text });
        return int.Parse(result.PredictedLabel!);
    }
}