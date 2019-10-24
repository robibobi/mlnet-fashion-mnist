using Microsoft.ML.Data;

namespace Tcoc.FashionMnist.Model
{

    /// <summary>
    /// Eigenschaften
    /// - Auflösung 28x28px
    /// - grayscale
    /// - 10 classes (wie Mnist classic)
    /// </summary>
    class MnistImage
    {
        public const int ImgWidth = 28;
        public const int ImgHeight = 28;

        [VectorType(ImgWidth * ImgHeight)]
        public byte[] Pixels { get; }

        public int Label { get; }

        public MnistImage(byte[] pixels, int label)
        {
            Pixels = pixels;
            Label = label;
        }
    }
}
