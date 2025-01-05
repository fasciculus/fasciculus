using Fasciculus.CodeAnalysis.Models;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Fasciculus.ApiDoc.Models
{
    public class ApiClasses : IEnumerable<ApiClass>
    {
        private readonly List<ApiClass> classes;

        public ApiClasses(ClassCollection classes, ApiNamespace @namespace)
        {
            this.classes = classes.Select(c => new ApiClass(c, @namespace)).ToList();
        }

        public IEnumerator<ApiClass> GetEnumerator()
            => classes.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => classes.GetEnumerator();
    }
}
