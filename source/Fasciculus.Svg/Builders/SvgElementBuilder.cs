using Fasciculus.Svg.Elements;

namespace Fasciculus.Svg.Builders
{
    public abstract class SvgElementBuilder<B, E>
        where B : notnull, SvgElementBuilder<B, E>
        where E : notnull, SvgElement, new()
    {
        protected readonly E element = new();

        protected abstract B GetThis();

        public B Class(string @class)
        {
            element.Class = @class;

            return GetThis();
        }

        public B Style(string style)
        {
            element.Style = style;

            return GetThis();
        }

        public B Color(string color)
        {
            element.Color = color;

            return GetThis();
        }

        public B FontFamily(string fontFamily)
        {
            element.FontFamily = fontFamily;

            return GetThis();
        }

        public B FontSize(string fontSize)
        {
            element.FontSize = fontSize;

            return GetThis();
        }

        public B Stroke(string stroke)
        {
            element.Stroke = stroke;

            return GetThis();
        }

        public virtual E Build()
            => element;
    }
}
