using Fasciculus.Svg.Elements;

namespace Fasciculus.Svg.Builders
{
    public abstract class SvgShapeBuilder<B, E> : SvgElementBuilder<B, E>
        where B : notnull, SvgShapeBuilder<B, E>
        where E : notnull, SvgShapeElement, new()
    {
        public B Fill(string fill)
        {
            element.Fill = fill;

            return GetThis();
        }
    }
}
