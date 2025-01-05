using Fasciculus.CodeAnalysis.Models;
using System.Linq;

namespace Fasciculus.ApiDoc.Models
{
    public class ApiClass : ApiElement
    {
        public override ApiLink Link { get; }

        public ApiClass(ClassInfo @class, ApiNamespace @namespace)
            : base(@class)
        {
            Link = @namespace.Link.Combine(@class.UntypedName, @class.Parameters.Count());
        }
    }
}
