namespace Fasciculus.Site.Models
{
    public static class NavigationKind
    {
        public const int Unknown = 0;

        public const int BlogYear = 10;
        public const int BlogMonth = 11;
        public const int BlogEntry = 12;

        public const int ApiPackage = 100;
        public const int ApiNamespace = 101;
        public const int ApiEnum = 102;
        public const int ApiInterface = 103;
        public const int ApiClass = 104;
        public const int ApiField = 105;
        public const int ApiEnumMember = 106;
        public const int ApiProperty = 107;

        public static bool IsLeaf(int kind)
        {
            return kind switch
            {
                BlogEntry => true,
                ApiField => true,
                ApiEnumMember => true,
                ApiProperty => true,
                _ => false,
            };
        }

        public static bool HasOverview(int kind)
        {
            return kind switch
            {
                ApiPackage => true,
                ApiNamespace => true,
                ApiEnum => true,
                ApiInterface => true,
                ApiClass => true,
                _ => false,
            };
        }
    }
}
