using Microsoft.ML.Data;
using Prism.Commands;
using Prism.Common;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Tcoc.FashionMnist.ML;
using Tcoc.FashionMnist.Model;
using Tcoc.FashionMnist.Util;

namespace Tcoc.FashionMnist.ViewModels
{
    class MainWindowVM : BindableBase
    {
        const string TestImagePath = "DataRaw/t10k-images-idx3-ubyte";
        const string TestLabelPath = "DataRaw/t10k-labels-idx1-ubyte";
        const int TestImageCount = 10000;

        const string TrainImagePath = "DataRaw/train-images-idx3-ubyte";
        const string TrainLabelPath = "DataRaw/train-labels-idx1-ubyte";
        const int TrainImageCount = 60000;

        const string ModelFileName = "TrainedModel.model";

        private readonly Trainer _trainer;
        private MnistImageVM _selectedTestImage;

        public List<MnistImageVM> TrainImages { get; }
        public List<MnistImageVM> TestImages { get; }
        private FashionLabel _predictedLabelForSelectedImage;

        public MnistImageVM SelectedTestImage
        {
            get { return _selectedTestImage; }
            set 
            {
                _selectedTestImage = value; 
                RaisePropertyChanged(nameof(SelectedTestImage));
                SelectedTestImageChanged();
            }
        }


        public FashionLabel PredictedLabelForSelectedImage
        {
            get { return _predictedLabelForSelectedImage; }
            set { _predictedLabelForSelectedImage = value; RaisePropertyChanged(nameof(PredictedLabelForSelectedImage)); }
        }

        private double _score;

        public double Score
        {
            get { return _score; }
            set { _score = value; RaisePropertyChanged(nameof(Score)); }
        }


        public MulticlassClassificationMetrics EvaluationMetrics { get; private set; }

        public DelegateCommand TrainModelCommand { get; }
        public DelegateCommand SaveModelCommand { get; }
        public DelegateCommand LoadModelCommand { get; }

        public MainWindowVM()
        {
            var trainImageFile = new FileInfo(TrainImagePath);
            var trainLabelFile = new FileInfo(TrainLabelPath);
            var testImageFile = new FileInfo(TestImagePath);
            var testLabelFile = new FileInfo(TestLabelPath);

            TrainModelCommand = new DelegateCommand(TrainModel);
            LoadModelCommand = new DelegateCommand(LoadModel);
            SaveModelCommand = new DelegateCommand(SaveModel);

            MnistReader reader = new MnistReader();
            _trainer = new Trainer();

            TrainImages = reader.ReadDataset(trainImageFile, trainLabelFile, TrainImageCount)
                .Select(CreateVM)
                .ToList();

            TestImages = reader.ReadDataset(testImageFile, testLabelFile, TestImageCount)
                .Select(CreateVM)
                .ToList();
        }

        private void SaveModel()
        {
            _trainer.SaveModelToFile(ModelFileName);
        }

        private void LoadModel()
        {
            _trainer.LoadModelFromFile(ModelFileName);
        }

        private void SelectedTestImageChanged()
        {
            if(SelectedTestImage != null)
            {
                PredictLabel(SelectedTestImage.Image);
            }
        }

        private void PredictLabel(MnistImage img)
        {
            PredictedLabelForSelectedImage = _trainer.Predict(img);
        }


        private MnistImageVM CreateVM(MnistImage img) => new MnistImageVM(img);

        private void TrainModel()
        {
            _trainer.TrainModel(TrainImages.Select(i => i.Image));

            foreach(MnistImageVM testImageVM in TestImages)
            {
                testImageVM.SetPredictedLabel(_trainer.Predict(testImageVM.Image));
            }

            Score = (double)TestImages
                .Where(i => i.PredictionCorrect)
                .Count() / TestImageCount;
        }
    }
}
