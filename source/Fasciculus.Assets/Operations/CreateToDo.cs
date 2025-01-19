using Fasciculus.Assets.Support;
using Fasciculus.IO;
using SkiaSharp;
using System.IO;

namespace Fasciculus.Assets.Operations
{
    public static class CreateToDo
    {
        private static readonly FileInfo ToDoPngFile = AssetsDirectories.Documents.File("todo.png");
        private static readonly FileInfo DonePngFile = AssetsDirectories.Documents.File("done.png");

        public static void Execute()
        {
            using SKBitmap bitmap = new(16, 16);
            using SKCanvas canvas = new(bitmap);
            SKColor color = SKColors.Black;

            canvas.Clear(SKColors.Transparent);

            for (int x = 3; x < 14; ++x)
            {
                canvas.DrawPoint(x, 3, color);
                canvas.DrawPoint(x, 13, color);
            }

            for (int y = 4; y < 13; ++y)
            {
                canvas.DrawPoint(3, y, color);
                canvas.DrawPoint(13, y, color);
            }

            Save(bitmap, ToDoPngFile);

            canvas.DrawPoint(5, 8, color);
            canvas.DrawPoint(6, 9, color);
            canvas.DrawPoint(7, 10, color);
            canvas.DrawPoint(8, 9, color);
            canvas.DrawPoint(9, 8, color);
            canvas.DrawPoint(10, 7, color);
            canvas.DrawPoint(11, 6, color);

            Save(bitmap, DonePngFile);
        }

        private static void Save(SKBitmap bitmap, FileInfo file)
        {
            using SKData data = bitmap.Encode(SKEncodedImageFormat.Png, 100);
            using Stream stream = file.OpenWrite();

            data.SaveTo(stream);
        }
    }
}
