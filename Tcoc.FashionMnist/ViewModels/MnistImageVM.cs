using System.Windows.Media.Imaging;
using Tcoc.FashionMnist.Model;
using Tcoc.FashionMnist.Util;

namespace Tcoc.FashionMnist.ViewModels
{
    class MnistImageVM
    {
        public MnistImage Image { get; }

        public BitmapSource Source { get; }

        public FashionLabel Label { get; }

        public MnistImageVM(MnistImage image)
        {
            Image = image;
            Source = BitmapUtils.BitmapFromArray(image.Pixels, MnistImage.ImgWidth, MnistImage.ImgHeight);
            Label = (FashionLabel)image.Label;
        }
    }
}
