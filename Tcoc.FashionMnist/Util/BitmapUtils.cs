using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Tcoc.FashionMnist.Util
{
    static class BitmapUtils
    {
        public static BitmapSource BitmapFromArray(byte[] data, int w, int h)
        {
            var format = PixelFormats.Gray8; //grey scale image 0-255

            WriteableBitmap wbm = new WriteableBitmap(w, h, 96, 96, format, null);
            wbm.WritePixels(new Int32Rect(0, 0, w, h), data, w, 0);

            return wbm;
        }
    }
}
