using SkiaSharp;
using System.IO;

namespace Fasciculus.Assets
{
    public static class SKBitmapExtensions
    {
        public static byte[] ToPngBytes(this SKBitmap bitmap)
        {
            using SKData data = bitmap.Encode(SKEncodedImageFormat.Png, 100);
            using MemoryStream stream = new();

            data.SaveTo(stream);

            return stream.ToArray();
        }
    }
}
