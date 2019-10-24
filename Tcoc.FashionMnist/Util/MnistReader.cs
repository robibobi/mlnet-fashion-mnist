using System.Collections.Generic;
using System.IO;
using Tcoc.FashionMnist.Model;

namespace Tcoc.FashionMnist.Util
{
    class MnistReader
    {
        public IEnumerable<MnistImage> ReadDataset(FileInfo imageFile, FileInfo labelFile, int imageCount)
        {
            var result = new List<MnistImage>();

            using FileStream ifsLabels = new FileStream(labelFile.FullName, FileMode.Open); // labels
            using FileStream ifsImages = new FileStream(imageFile.FullName, FileMode.Open); // images

            using BinaryReader brLabels = new BinaryReader(ifsLabels);
            using BinaryReader brImages = new BinaryReader(ifsImages);

            // Image file headers
            int magic1 = brImages.ReadInt32(); // discard
            int numImages = brImages.ReadInt32();
            int numRows = brImages.ReadInt32();
            int numCols = brImages.ReadInt32();
            // Label file header
            int magic2 = brLabels.ReadInt32();
            int numLabels = brLabels.ReadInt32();
            
            int bytesPerImage = MnistImage.ImgWidth * MnistImage.ImgHeight;

            for (int di = 0; di < imageCount; ++di)
            {
                byte[] pixels1d = new byte[bytesPerImage]; 
                for (int i = 0; i < bytesPerImage; ++i)
                {
                    byte b = brImages.ReadByte();
                    pixels1d[i] = b;
                }

                byte label = brLabels.ReadByte();

                MnistImage dImage = new MnistImage(pixels1d, label);
                result.Add(dImage);
            } 

            return result;
        }
    }
}
