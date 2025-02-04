using Antlr4.Runtime;
using Fasciculus.Experiments.Calculator;
using System.Diagnostics;

namespace Fasciculus.Experiments
{
    internal class Program
    {
        static void Main(string[] args)
        {
            AntlrInputStream input = new("2 + 3");
            ExprLexer lexer = new(input);
            CommonTokenStream tokens = new(lexer);
            ExprParser parser = new(tokens);

            var tree = parser.prog();

            Debug.WriteLine(tree.ToStringTree());
        }
    }
}
