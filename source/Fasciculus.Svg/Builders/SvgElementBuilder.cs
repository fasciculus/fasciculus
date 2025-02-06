using Fasciculus.Svg.Elements;
using System;

namespace Fasciculus.Svg.Builders
{
    public abstract class SvgElementBuilder<B, E>
        where B : notnull, SvgElementBuilder<B, E>
        where E : notnull, SvgElement
    {
        protected readonly E element;

        protected SvgElementBuilder(Func<E> factory)
        {
            element = factory();
        }

        protected abstract B This();

        public B Class(string @class)
        {
            element.Class = @class;

            return This();
        }

        public B Style(string style)
        {
            element.Style = style;

            return This();
        }

        public B Color(string color)
        {
            element.Color = color;

            return This();
        }

        public B FontFamily(string fontFamily)
        {
            element.FontFamily = fontFamily;

            return This();
        }

        public B FontSize(string fontSize)
        {
            element.FontSize = fontSize;

            return This();
        }

        public virtual E Build()
            => element;

        public static implicit operator E(SvgElementBuilder<B, E> builder)
            => builder.Build();
    }
}
