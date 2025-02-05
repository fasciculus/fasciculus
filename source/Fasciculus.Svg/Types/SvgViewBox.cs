using Fasciculus.Collections;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Fasciculus.Svg.Types
{
    [DebuggerDisplay("{MinX} {MinY} {Width} {Height}")]
    public class SvgViewBox : IEnumerable<double>
    {
        public double MinX { get; set; }
        public double MinY { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }

        public SvgViewBox(double minX, double minY, double width, double height)
        {
            MinX = minX;
            MinY = minY;
            Width = width;
            Height = height;
        }

        public SvgViewBox(double minX, double minY, double width)
            : this(minX, minY, width, 0) { }

        public SvgViewBox(double minX, double minY)
            : this(minX, minY, 0, 0) { }

        public SvgViewBox(double minX)
            : this(minX, 0, 0, 0) { }

        public SvgViewBox()
            : this(0, 0, 0, 0) { }

        public SvgViewBox(IEnumerable<double> values)
        {
            double[] xywh = [.. values.Take(4, 0.0)];

            MinX = xywh[0];
            MinY = xywh[1];
            Width = xywh[2];
            Height = xywh[3];
        }

        private IEnumerable<double> GetValues()
        {
            yield return MinX;
            yield return MinY;
            yield return Width;
            yield return Height;
        }

        public IEnumerator<double> GetEnumerator()
            => GetValues().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => GetValues().GetEnumerator();
    }
}
