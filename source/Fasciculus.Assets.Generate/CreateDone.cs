using SkiaSharp;

namespace Fasciculus.Assets
{
    public static class CreateDone
    {
        public static byte[] Create()
        {
            using SKBitmap bitmap = new(16, 16);
            using SKCanvas canvas = new(bitmap);
            SKColor color = SKColors.Black;

            CreateToDo.Create(canvas);

            canvas.DrawPoint(5, 8, color);
            canvas.DrawPoint(6, 9, color);
            canvas.DrawPoint(7, 10, color);
            canvas.DrawPoint(8, 9, color);
            canvas.DrawPoint(9, 8, color);
            canvas.DrawPoint(10, 7, color);
            canvas.DrawPoint(11, 6, color);

            return bitmap.ToPngBytes();
        }
    }
}
