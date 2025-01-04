using Fasciculus.Assets.Support;
using Fasciculus.IO;
using SkiaSharp;
using System.IO;

namespace Fasciculus.Assets.Operations
{
    public static class CreateLogo
    {
        private static readonly FileInfo PngFile = AssetsDirectories.Documents.File("fasciculus.png");

        private static readonly SKColor Color = new(48, 48, 160, 255);

        public static void Execute()
        {
            using SKBitmap bitmap = new(64, 64);
            using SKCanvas canvas = new(bitmap);

            canvas.Clear(SKColors.Transparent);

            FillRect(canvas, 14, 1, 4, 8);
            FillRect(canvas, 20, 1, 6, 8);
            FillRect(canvas, 28, 1, 5, 8);

            FillRect(canvas, 13, 11, 19, 6);

            for (int i = 0; i < 15; ++i)
            {
                FillRect(canvas, i + 14, i + 17, 8, 1);
            }

            FillRect(canvas, 14, 21, 1, 1);
            FillRect(canvas, 14, 22, 2, 1);
            FillRect(canvas, 14, 23, 3, 1);
            FillRect(canvas, 14, 24, 4, 8);

            FillRect(canvas, 20, 27, 1, 1);
            FillRect(canvas, 20, 28, 2, 1);
            FillRect(canvas, 20, 29, 3, 1);
            FillRect(canvas, 20, 30, 4, 1);
            FillRect(canvas, 20, 31, 5, 1);

            FillRect(canvas, 25, 18, 1, 1);

            FillRect(canvas, 28, 18, 4, 3);
            FillRect(canvas, 29, 21, 3, 1);
            FillRect(canvas, 30, 22, 2, 1);
            FillRect(canvas, 31, 23, 1, 1);

            using SKData pngData = bitmap.Encode(SKEncodedImageFormat.Png, 100);
            using Stream pngStream = PngFile.OpenWrite();

            pngData.SaveTo(pngStream);
        }

        private static void FillRect(SKCanvas canvas, int x, int y, int w, int h)
        {
            int x1 = x;
            int x2 = 64 - x - w;
            int y1 = y;
            int y2 = 64 - y - h;

            DoFillRect(canvas, x1, y1, w, h);
            DoFillRect(canvas, x2, y1, w, h);
            DoFillRect(canvas, x1, y2, w, h);
            DoFillRect(canvas, x2, y2, w, h);
        }

        private static void DoFillRect(SKCanvas canvas, int x, int y, int w, int h)
        {
            for (int j = y, m = y + h; j < m; ++j)
            {
                for (int i = x, n = x + w; i < n; ++i)
                {
                    canvas.DrawPoint(i, j, Color);
                }
            }
        }
    }
}
