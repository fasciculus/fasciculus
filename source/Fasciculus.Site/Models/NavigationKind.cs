namespace Fasciculus.Site.Models
{
    public static class NavigationKind
    {
        public const int Unknown = 0;
        public const int Overview = 1;

        public const int BlogYear = 10;
        public const int BlogMonth = 11;
        public const int BlogEntry = 12;

        public const int LicenseList = 20;

        public const int ApiPackage = 100;
        public const int ApiNamespace = 101;

        public const int ApiEnums = 102;
        public const int ApiEnum = 103;
        public const int ApiInterfaces = 104;
        public const int ApiInterface = 105;
        public const int ApiClasses = 106;
        public const int ApiClass = 107;

        public const int ApiMembers = 108;
        public const int ApiMember = 109;
        public const int ApiFields = 110;
        public const int ApiField = 111;
        public const int ApiEvents = 112;
        public const int ApiEvent = 113;
        public const int ApiProperties = 114;
        public const int ApiProperty = 115;

        public const int ApiConstructors = 116;

        public static bool IsLeaf(int kind)
        {
            return kind switch
            {
                BlogEntry => true,
                LicenseList => true,
                ApiField => true,
                ApiMember => true,
                ApiEvent => true,
                ApiProperty => true,
                ApiFields => true,
                ApiEvents => true,
                ApiConstructors => true,
                _ => false,
            };
        }
    }
}
