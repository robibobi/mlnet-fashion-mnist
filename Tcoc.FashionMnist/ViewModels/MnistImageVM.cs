using Prism.Mvvm;
using System.Windows.Media.Imaging;
using Tcoc.FashionMnist.Model;
using Tcoc.FashionMnist.Util;

namespace Tcoc.FashionMnist.ViewModels
{
    class MnistImageVM : BindableBase
    {
        public MnistImage Image { get; }

        public BitmapSource Source { get; }

        public FashionLabel Label { get; }

        public FashionLabel PredictedLabel { get; private set; }

        public bool PredictionCorrect { get; private set; }

        public MnistImageVM(MnistImage image)
        {
            Image = image;
            Source = BitmapUtils.BitmapFromArray(image.Pixels, MnistImage.ImgWidth, MnistImage.ImgHeight);
            Label = (FashionLabel)image.Label;
            PredictedLabel = FashionLabel.Unknown;
        }

        public void SetPredictedLabel(FashionLabel predLbl)
        {
            PredictedLabel = predLbl;
            RaisePropertyChanged(nameof(PredictedLabel));
            PredictionCorrect = Label == PredictedLabel;
            RaisePropertyChanged(nameof(PredictionCorrect));
        }
    }
}
