using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Transforms;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Tcoc.FashionMnist.Model;

namespace Tcoc.FashionMnist.ML
{
    /// <summary>
    /// <see cref="https://github.com/dotnet/machinelearning/blob/master/docs/code/MlNetHighLevelConcepts.md"/>
    /// </summary>
    class Trainer
    {
        private ITransformer _trainedModel;
        private MLContext _context;

        public Trainer()
        {
            _context = new MLContext();
        }

        public void TrainModel(IEnumerable<MnistImage> images)
        {

            IDataView data = _context.Data.LoadFromEnumerable(images);
            
            var featureTransform = _context.Transforms.Conversion
                .ConvertType("Features", nameof(MnistImage.Pixels), DataKind.Single);

            var labelToKeyTransform = _context.Transforms.Conversion.MapValueToKey(
                outputColumnName: "LabelAsKey",
                inputColumnName: "Label",
                keyOrdinality: ValueToKeyMappingEstimator.KeyOrdinality.ByValue);

            var dropLabelTransform = _context.Transforms.DropColumns("Label");

            var trainer = _context.MulticlassClassification.Trainers.SdcaMaximumEntropy(
                labelColumnName: "LabelAsKey",
                featureColumnName: "Features");


            var trainerInput = featureTransform
                .Append(labelToKeyTransform)
                .Append(dropLabelTransform)
                .AppendCacheCheckpoint(_context);

            var pipeline = trainerInput
                .Append(trainer);

            var preview = trainerInput.Preview(data);
            // TODO: Add "explain" method that describes Columns

            _trainedModel = pipeline.Fit(data);

        }

        public MulticlassClassificationMetrics EvaluateModel(IEnumerable<MnistImage> testImages)
        {
            if (_trainedModel == null)
                return null;

            var testData = _context.Data.LoadFromEnumerable(testImages);
            var predictions = _trainedModel.Transform(testData);
            var metrics = _context.MulticlassClassification.Evaluate(data: predictions, labelColumnName: "LabelAsKey", scoreColumnName: "Score");
            
            var sb = new StringBuilder();

            sb.Append($"************************************************************\n");
            sb.Append($"*    Metrics for multi-class classification model   \n");
            sb.Append($"*-----------------------------------------------------------");
            sb.Append($"    AccuracyMacro = {metrics.MacroAccuracy:0.####}, a value between 0 and 1, the closer to 1, the better\n");
            sb.Append($"    AccuracyMicro = {metrics.MicroAccuracy:0.####}, a value between 0 and 1, the closer to 1, the better\n");
            sb.Append($"    LogLoss = {metrics.LogLoss:0.####}, the closer to 0, the better\n");
            sb.Append($"    LogLoss for class 1 = {metrics.PerClassLogLoss[0]:0.####}, the closer to 0, the better\n");
            sb.Append($"    LogLoss for class 2 = {metrics.PerClassLogLoss[1]:0.####}, the closer to 0, the better\n");
            sb.Append($"    LogLoss for class 3 = {metrics.PerClassLogLoss[2]:0.####}, the closer to 0, the better\n");
            sb.Append($"************************************************************\n");

            string result = sb.ToString();
            Debug.WriteLine(result);

            return metrics;
        }


        class MnistTestImage
        {

            [VectorType(MnistImage.ImgWidth * MnistImage.ImgHeight)]
            public byte[] Pixels { get; }

            public MnistTestImage(byte[] pixels)
            {
                Pixels = pixels;
            }
        }

        public FashionLabel Predict(MnistImage image)
        {
            if (_trainedModel == null)
                return FashionLabel.Unknown;
            var testImage = new MnistTestImage(image.Pixels);

            // TODO: Label aus Testdaten entfernen
            var engine = _context.Model.CreatePredictionEngine<MnistTestImage, MnistPrediction>(_trainedModel);

            var prediction = engine.Predict(testImage);


            // Alternativ
            //_trainedModel.MakePredictionFunction(...)

            return (FashionLabel)prediction.PredictedLabel;
        }

       
    }
}
