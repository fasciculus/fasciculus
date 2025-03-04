using SkiaSharp;

namespace Fasciculus.Assets
{
    public static class CreateToDo
    {
        public static byte[] Create()
        {
            using SKBitmap bitmap = new(16, 16);
            using SKCanvas canvas = new(bitmap);

            Create(canvas);

            return bitmap.ToPngBytes();
        }

        public static void Create(SKCanvas canvas)
        {
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
        }
    }
}
