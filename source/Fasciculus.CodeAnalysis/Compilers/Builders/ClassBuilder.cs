using Fasciculus.CodeAnalysis.Models;

namespace Fasciculus.CodeAnalysis.Compilers.Builders
{
    public class ClassBuilder : TypeBuilder<ClassSymbol>
    {
        public override ClassSymbol Build()
        {
            ClassSymbol @class = new(Framework, Package)
            {
                Name = Name,
                Link = Link,
                Modifiers = Modifiers,
                Comment = Comment
            };

            Populate(@class);

            return @class;
        }
    }
}
