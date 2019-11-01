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
           
            var featureTransform = _context.Transforms.Conversion.ConvertType(
                outputColumnName: "Features",
                inputColumnName: "Pixels",
                outputKind: DataKind.Single);

            var labelToKeyTransform = _context.Transforms.Conversion.MapValueToKey(
                outputColumnName: "LabelAsKey",
                inputColumnName: "Label",
                keyOrdinality: ValueToKeyMappingEstimator.KeyOrdinality.ByValue);

            var dropLabelTransform = _context.Transforms.DropColumns("Label", "Pixels");

            var trainer = _context.MulticlassClassification.Trainers.SdcaMaximumEntropy(
                labelColumnName: "LabelAsKey",
                featureColumnName: "Features");


            var trainerInput = featureTransform
                .Append(labelToKeyTransform)
                .Append(dropLabelTransform)
                .AppendCacheCheckpoint(_context);

            var pipeline = trainerInput
                .Append(trainer);

            _trainedModel = pipeline.Fit(data);
            PrintColumns("Model", _trainedModel, data);
        }


        public void LoadModelFromFile(string modelFileName)
        {
            _trainedModel = _context.Model.Load(modelFileName, out DataViewSchema _);
        }

        public void SaveModelToFile(string modelFileName)
        {
            var data = _context.Data.LoadFromEnumerable(
                new List<MnistImage>() { new MnistImage(null, 0) });
            _context.Model.Save(_trainedModel, data.Schema, modelFileName);
        }


        public FashionLabel Predict(MnistImage image)
        {
            if (_trainedModel == null)
                return FashionLabel.Unknown;

            var engine = _context.Model.CreatePredictionEngine<MnistImage, MnistPrediction>(_trainedModel);

            var prediction = engine.Predict(image);

            return (FashionLabel)(prediction.PredictedLabel -1); // !!
        }

        private void PrintColumns(string name, ITransformer t, IDataView data)
        {
            var debuggerView = t.Preview(data, maxRows: 3);
            Debug.WriteLine($"Columns of '{name}'");
            Debug.Write("[");
            foreach (var columnInfo in debuggerView.ColumnView)
            {
                Debug.Write($"{columnInfo.Column.Name} ({columnInfo.Column.Type})");
                if (debuggerView.ColumnView.IndexOf(columnInfo) != debuggerView.ColumnView.Length - 1)
                    Debug.Write(" | ");
            }
            Debug.Write("]");
        }


    }
}
