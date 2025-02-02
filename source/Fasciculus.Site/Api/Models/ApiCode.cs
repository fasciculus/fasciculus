using Fasciculus.CodeAnalysis.Models;
using System.Text;

namespace Fasciculus.Site.Api.Models
{
    public static class ApiCode
    {
        public static string Create(IConstructorSymbol constructor)
        {
            StringBuilder sb = new();
            bool firstParameter = true;

            sb.Append(constructor.Modifiers).Append(' ');
            sb.Append(constructor.BareName).Append('(');

            foreach (var parameter in constructor.Parameters)
            {
                if (!firstParameter)
                {
                    sb.Append(", ");
                }

                firstParameter = false;

                sb.Append(parameter.Type.Name);
                sb.Append(' ');
                sb.Append(parameter.Name.Name);
            }

            sb.Append(')');

            return sb.ToString();
        }
    }
}
