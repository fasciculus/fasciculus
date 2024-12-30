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

        private const float Y0 = 0.200f * Height; // top of upper bar
        private const float Y1 = 0.300f * Height; // bottom of upper bar

        private const float Y2 = 0.700f * Height; // top of lower bar
        private const float Y3 = 0.800f * Height; // bottom of lower bar

        private const float X0 = 0.200f * Width; // left of bars
        private const float X1 = 0.341f * Width;
        private const float X2 = 0.659f * Width;
        private const float X3 = 0.800f * Width; // right of bars

        private const float Y00 = 0.020f * Height;
        private const float Y01 = 0.185f * Height;
        private const float Y02 = 0.815f * Height;
        private const float Y03 = 0.980f * Height;

        private const float Y10 = 0.310f * Height;
        private const float Y121 = 0.365f * Height;
        private const float Y122 = 0.418f * Height;

        private const float Y30 = 0.685f * Height;
        private const float Y320 = 0.583f * Height;
        private const float Y321 = 0.635f * Height;

        private const float Y20 = 0.359f * Height;
        private const float Y21 = 0.409f * Height;
        private const float Y22 = 0.591f * Height;
        private const float Y23 = 0.641f * Height;

        private const float Y24 = 0.434f * Height;
        private const float Y25 = 0.500f * Height;
        private const float Y26 = 0.500f * Height;
        private const float Y27 = 0.566f * Height;

        private const float X00 = 0.250f * Width;
        private const float X01 = 0.310f * Width;
        private const float X02 = 0.330f * Width;
        private const float X03 = 0.430f * Width;
        private const float X04 = 0.450f * Width;
        private const float X05 = 0.550f * Width;
        private const float X06 = 0.570f * Width;
        private const float X07 = 0.670f * Width;
        private const float X08 = 0.690f * Width;
        private const float X09 = 0.750f * Width;

        private const float X110 = 0.380f * Width;
        private const float X111 = 0.422f * Width;
        private const float Y110 = 0.315f * Height;
        private const float Y111 = 0.350f * Width;

        // cos(30) = .866
        // cos(60) = .500
        // 30 + 60 + 90 + 60 + 30 = 3.732
        // .4 / 3.732 = 0.107
        // 0.107 * .5 = 0.54
        // 0.107 * .866 = 0.93

        private static readonly SvgColourServer ForegroundColor = new(Color.FromArgb(255, 48, 48, 160));

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

            document.Children.Add(CreateUpperBar());
            document.Children.Add(CreateLowerBar());
            document.Children.Add(CreateBackwardBar());
            document.Children.Add(CreateForwardBar());

            document.Children.Add(CreateRod00());
            document.Children.Add(CreateRod01());
            document.Children.Add(CreateRod02());
            document.Children.Add(CreateRod03());
            document.Children.Add(CreateRod04());

            document.Children.Add(CreateRod11());
            document.Children.Add(CreateRod12());
            document.Children.Add(CreateRod32());

            document.Children.Add(CreateRod20());
            document.Children.Add(CreateRod21());
            document.Children.Add(CreateRod23());
            document.Children.Add(CreateRod24());

            document.Children.Add(CreateRod40());
            document.Children.Add(CreateRod41());
            document.Children.Add(CreateRod42());
            document.Children.Add(CreateRod43());
            document.Children.Add(CreateRod44());

            return document;
        }

        private static SvgPath CreateBar(SvgPathSegmentList segments)
        {
            return new()
            {
                PathData = segments,
                Stroke = ForegroundColor,
                StrokeWidth = 0.001f,
                StrokeLineCap = SvgStrokeLineCap.Square,
                Fill = ForegroundColor,
            };
        }

        private static SvgPath CreateUpperBar()
        {
            SvgPathSegmentList segments =
            [
                new SvgMoveToSegment(false, new PointF(X0, Y0)),
                new SvgLineSegment(false, new PointF(X3, Y0)),
                new SvgLineSegment(false, new PointF(X3, Y1)),
                new SvgLineSegment(false, new PointF(X0, Y1)),
                new SvgClosePathSegment(false)
            ];

            return CreateBar(segments);
        }

        private static SvgPath CreateLowerBar()
        {
            SvgPathSegmentList segments =
            [
                new SvgMoveToSegment(false, new PointF(X0, Y2)),
                new SvgLineSegment(false, new PointF(X3, Y2)),
                new SvgLineSegment(false, new PointF(X3, Y3)),
                new SvgLineSegment(false, new PointF(X0, Y3)),
                new SvgClosePathSegment(false)
            ];

            return CreateBar(segments);
        }

        private static SvgPath CreateBackwardBar()
        {
            SvgPathSegmentList segments =
            [
                new SvgMoveToSegment(false, new PointF(X0, Y1 - 0.002f * Height)),
                new SvgLineSegment(false, new PointF(X1, Y1 - 0.002f * Height)),
                new SvgLineSegment(false, new PointF(X3, Y2 + 0.002f * Height)),
                new SvgLineSegment(false, new PointF(X2, Y2 + 0.002f * Height)),
                new SvgClosePathSegment(false)
            ];

            return CreateBar(segments);
        }

        private static SvgPath CreateForwardBar()
        {
            SvgPathSegmentList segments =
            [
                new SvgMoveToSegment(false, new PointF(X0, Y2 + 0.002f * Height)),
                new SvgLineSegment(false, new PointF(X2, Y1 - 0.002f * Height)),
                new SvgLineSegment(false, new PointF(X3, Y1 - 0.002f * Height)),
                new SvgLineSegment(false, new PointF(X1, Y2 + 0.002f * Height)),
                new SvgClosePathSegment(false)
            ];

            return CreateBar(segments);
        }

        private static SvgPath CreateRod00()
        {
            SvgPathSegmentList segments =
            [
                new SvgMoveToSegment(false, new PointF(X00, Y00)),
                new SvgLineSegment(false, new PointF(X01, Y00)),
                new SvgLineSegment(false, new PointF(X01, Y01)),
                new SvgLineSegment(false, new PointF(X00, Y01)),
                new SvgClosePathSegment(false)
            ];

            return CreateBar(segments);
        }

        private static SvgPath CreateRod40()
        {
            SvgPathSegmentList segments =
            [
                new SvgMoveToSegment(false, new PointF(X00, Y02)),
                new SvgLineSegment(false, new PointF(X01, Y02)),
                new SvgLineSegment(false, new PointF(X01, Y03)),
                new SvgLineSegment(false, new PointF(X00, Y03)),
                new SvgClosePathSegment(false)
            ];

            return CreateBar(segments);
        }

        private static SvgPath CreateRod01()
        {
            SvgPathSegmentList segments =
            [
                new SvgMoveToSegment(false, new PointF(X02, Y00)),
                new SvgLineSegment(false, new PointF(X03, Y00)),
                new SvgLineSegment(false, new PointF(X03, Y01)),
                new SvgLineSegment(false, new PointF(X02, Y01)),
                new SvgClosePathSegment(false)
            ];

            return CreateBar(segments);
        }

        private static SvgPath CreateRod41()
        {
            SvgPathSegmentList segments =
            [
                new SvgMoveToSegment(false, new PointF(X02, Y02)),
                new SvgLineSegment(false, new PointF(X03, Y02)),
                new SvgLineSegment(false, new PointF(X03, Y03)),
                new SvgLineSegment(false, new PointF(X02, Y03)),
                new SvgClosePathSegment(false)
            ];

            return CreateBar(segments);
        }

        private static SvgPath CreateRod02()
        {
            SvgPathSegmentList segments =
            [
                new SvgMoveToSegment(false, new PointF(X04, Y00)),
                new SvgLineSegment(false, new PointF(X05, Y00)),
                new SvgLineSegment(false, new PointF(X05, Y01)),
                new SvgLineSegment(false, new PointF(X04, Y01)),
                new SvgClosePathSegment(false)
            ];

            return CreateBar(segments);
        }

        private static SvgPath CreateRod42()
        {
            SvgPathSegmentList segments =
            [
                new SvgMoveToSegment(false, new PointF(X04, Y02)),
                new SvgLineSegment(false, new PointF(X05, Y02)),
                new SvgLineSegment(false, new PointF(X05, Y03)),
                new SvgLineSegment(false, new PointF(X04, Y03)),
                new SvgClosePathSegment(false)
            ];

            return CreateBar(segments);
        }

        private static SvgPath CreateRod03()
        {
            SvgPathSegmentList segments =
            [
                new SvgMoveToSegment(false, new PointF(X06, Y00)),
                new SvgLineSegment(false, new PointF(X07, Y00)),
                new SvgLineSegment(false, new PointF(X07, Y01)),
                new SvgLineSegment(false, new PointF(X06, Y01)),
                new SvgClosePathSegment(false)
            ];

            return CreateBar(segments);
        }

        private static SvgPath CreateRod43()
        {
            SvgPathSegmentList segments =
            [
                new SvgMoveToSegment(false, new PointF(X06, Y02)),
                new SvgLineSegment(false, new PointF(X07, Y02)),
                new SvgLineSegment(false, new PointF(X07, Y03)),
                new SvgLineSegment(false, new PointF(X06, Y03)),
                new SvgClosePathSegment(false)
            ];

            return CreateBar(segments);
        }

        private static SvgPath CreateRod04()
        {
            SvgPathSegmentList segments =
            [
                new SvgMoveToSegment(false, new PointF(X08, Y00)),
                new SvgLineSegment(false, new PointF(X09, Y00)),
                new SvgLineSegment(false, new PointF(X09, Y01)),
                new SvgLineSegment(false, new PointF(X08, Y01)),
                new SvgClosePathSegment(false)
            ];

            return CreateBar(segments);
        }

        private static SvgPath CreateRod44()
        {
            SvgPathSegmentList segments =
            [
                new SvgMoveToSegment(false, new PointF(X08, Y02)),
                new SvgLineSegment(false, new PointF(X09, Y02)),
                new SvgLineSegment(false, new PointF(X09, Y03)),
                new SvgLineSegment(false, new PointF(X08, Y03)),
                new SvgClosePathSegment(false)
            ];

            return CreateBar(segments);
        }

        private static SvgPath CreateRod20()
        {
            SvgPathSegmentList segments =
            [
                new SvgMoveToSegment(false, new PointF(X00, Y20)),
                new SvgLineSegment(false, new PointF(X01, Y21)),
                new SvgLineSegment(false, new PointF(X01, Y22)),
                new SvgLineSegment(false, new PointF(X00, Y23)),
                new SvgClosePathSegment(false)
            ];

            return CreateBar(segments);
        }

        private static SvgPath CreateRod21()
        {
            SvgPathSegmentList segments =
            [
                new SvgMoveToSegment(false, new PointF(X02, Y24)),
                new SvgLineSegment(false, new PointF(X03 - 0.015f * Width, Y25)),
                new SvgLineSegment(false, new PointF(X03 - 0.015f * Width, Y26)),
                new SvgLineSegment(false, new PointF(X02, Y27)),
                new SvgClosePathSegment(false)
            ];

            return CreateBar(segments);
        }

        private static SvgPath CreateRod23()
        {
            SvgPathSegmentList segments =
            [
                new SvgMoveToSegment(false, new PointF(X06 + 0.015f * Width, Y25)),
                new SvgLineSegment(false, new PointF(X07, Y24)),
                new SvgLineSegment(false, new PointF(X07, Y27)),
                new SvgLineSegment(false, new PointF(X06 + 0.015f * Width, Y26)),
                new SvgClosePathSegment(false)
            ];

            return CreateBar(segments);
        }

        private static SvgPath CreateRod24()
        {
            SvgPathSegmentList segments =
            [
                new SvgMoveToSegment(false, new PointF(X08, Y21)),
                new SvgLineSegment(false, new PointF(X09, Y20)),
                new SvgLineSegment(false, new PointF(X09, Y23)),
                new SvgLineSegment(false, new PointF(X08, Y22)),
                new SvgClosePathSegment(false)
            ];

            return CreateBar(segments);
        }

        private static SvgPath CreateRod12()
        {
            SvgPathSegmentList segments =
            [
                new SvgMoveToSegment(false, new PointF(X04, Y10)),
                new SvgLineSegment(false, new PointF(X05, Y10)),
                new SvgLineSegment(false, new PointF(X05, Y121)),
                new SvgLineSegment(false, new PointF(0.5f * Width, Y122)),
                new SvgLineSegment(false, new PointF(X04, Y121)),
                new SvgClosePathSegment(false)
            ];

            return CreateBar(segments);
        }

        private static SvgPath CreateRod32()
        {
            SvgPathSegmentList segments =
            [
                new SvgMoveToSegment(false, new PointF(X04, Y321)),
                new SvgLineSegment(false, new PointF(0.5f * Width, Y320)),
                new SvgLineSegment(false, new PointF(X05, Y321)),
                new SvgLineSegment(false, new PointF(X05, Y30)),
                new SvgLineSegment(false, new PointF(X04, Y30)),
                new SvgClosePathSegment(false)
            ];

            return CreateBar(segments);
        }

        private static SvgPath CreateRod11()
        {
            SvgPathSegmentList segments =
            [
                new SvgMoveToSegment(false, new PointF(X110, Y10)),
                new SvgLineSegment(false, new PointF(X111, Y10)),
                new SvgLineSegment(false, new PointF(X111, Y111)),
                new SvgLineSegment(false, new PointF(X110, Y110)),
                new SvgClosePathSegment(false)
            ];

            return CreateBar(segments);
        }

        private static void SaveDocument(SvgDocument document)
        {
            FileInfo svgFile = AssetsDirectories.Documents.File("fasciculus.svg");
            FileInfo pngFile = AssetsDirectories.Documents.File("fasciculus.png");

            document.Write(svgFile.FullName);

            using SKSvg svg = SKSvg.CreateFromSvgDocument(document);

            svg.Save(pngFile.FullName, SKColors.Empty, SKEncodedImageFormat.Png, 100, 1f, 1f);
        }
    }
}
