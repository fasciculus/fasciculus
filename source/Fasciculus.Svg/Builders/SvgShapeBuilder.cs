using Fasciculus.Svg.Elements;
using System;

namespace Fasciculus.Svg.Builders
{
    public abstract class SvgShapeBuilder<B, E> : SvgElementBuilder<B, E>
        where B : notnull, SvgShapeBuilder<B, E>
        where E : notnull, SvgShape
    {
        protected SvgShapeBuilder(Func<E> factory)
            : base(factory) { }

        public B Fill(string fill)
        {
            element.Fill = fill;

            return This();
        }

        public B Stroke(string stroke)
        {
            element.Stroke = stroke;

            return This();
        }

        public B StrokeWidth(string strokeWidth)
        {
            element.StrokeWidth = strokeWidth;

            return This();
        }

    }
}
