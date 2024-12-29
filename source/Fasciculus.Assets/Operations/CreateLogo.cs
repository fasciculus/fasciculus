using Fasciculus.Assets.Support;
using SkiaSharp;
using Svg;
using Svg.Pathing;
using Svg.Skia;
using System.Drawing;
using System.IO;

namespace Fasciculus.Assets.Operations
{
    public class CreateLogo
    {
        private const float Width = 64;
        private const float Height = 64;

        private static readonly SvgColourServer BackgroundColor = new(Color.Black);
        private static readonly SvgColourServer ForegroundColor = new(Color.FromArgb(255, 77, 77, 255));

        public static void Execute()
        {
            SvgDocument document = CreateDocument();

            SaveDocument(document);
        }

        private static SvgDocument CreateDocument()
        {
            SvgDocument document = new()
            {
                Width = Width,
                Height = Height,
            };

            document.Children.Add(CreateBinding());

            return document;
        }

        private static SvgPath CreateBinding()
        {
            SvgPathSegmentList segments =
            [
                new SvgMoveToSegment(false, new PointF(Width * 0.2f, Height * 0.2f)),
                new SvgLineSegment(false, new PointF(Width * 0.8f, Height * 0.2f)),
                new SvgLineSegment(false, new PointF(Width * 0.8f, Height * 0.8f)),
                new SvgLineSegment(false, new PointF(Width * 0.2f, Height * 0.8f)),
                new SvgClosePathSegment(false)
            ];

            return new()
            {
                PathData = segments,
                Stroke = ForegroundColor,
                Fill = ForegroundColor,
            };
        }

        private static void SaveDocument(SvgDocument document)
        {
            FileInfo svgFile = AssetsDirectories.Documents.File("fasciculus.svg");
            FileInfo pngFile = AssetsDirectories.Documents.File("fasciculus.png");

            document.Write(svgFile.FullName);

            using SKSvg svg = SKSvg.CreateFromSvgDocument(document);

            svg.Save(pngFile.FullName, SKColors.Black, SKEncodedImageFormat.Png, 100, 1f, 1f);
        }
    }
}
