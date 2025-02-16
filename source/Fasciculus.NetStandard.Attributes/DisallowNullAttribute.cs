namespace System.Diagnostics.CodeAnalysis
{
    // available in netstandard2.1

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property, Inherited = false)]
    internal sealed class DisallowNullAttribute : Attribute
    {
        public DisallowNullAttribute() { }
    }
}
