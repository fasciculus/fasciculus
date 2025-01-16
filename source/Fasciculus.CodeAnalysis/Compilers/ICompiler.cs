namespace Fasciculus.CodeAnalysis.Compilers
{
    public interface ICompiler
    {
        public void PushComment();

        public void PopComment();
    }
}
