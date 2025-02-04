using System;
using System.Xml;

namespace Fasciculus.Xml
{
    public static class XConvert
    {
        public static T Convert<T>(string? value, Func<string, T> convert, T defaultValue)
        {
            if (value is null)
            {
                return defaultValue;
            }

            try
            {
                return convert(value);
            }
            catch
            {
                return defaultValue;
            }
        }

        public static bool ToBool(string? value, bool defaultValue = false)
            => Convert(value, XmlConvert.ToBoolean, defaultValue);

        public static byte ToByte(string? value, byte defaultValue = 0)
            => Convert(value, XmlConvert.ToByte, defaultValue);

        public static DateTime ToDateTime(string? value, DateTime? defaultValue = null,
            XmlDateTimeSerializationMode mode = XmlDateTimeSerializationMode.Utc)
            => Convert(value, s => XmlConvert.ToDateTime(s, mode), defaultValue ?? DateTime.FromBinary(0));

        public static DateTimeOffset ToDateTimeOffset(string? value, DateTimeOffset? defaultValue = null)
            => Convert(value, XmlConvert.ToDateTimeOffset, defaultValue ?? DateTime.FromBinary(0));

        public static decimal ToDecimal(string? value, decimal defaultValue = 0)
            => Convert(value, XmlConvert.ToDecimal, defaultValue);

        public static double ToDouble(string? value, double defaultValue = 0)
            => Convert(value, XmlConvert.ToDouble, defaultValue);

        public static Guid ToGuid(string? value, Guid? defaultValue = null)
            => Convert(value, XmlConvert.ToGuid, defaultValue ?? Guid.Empty);

        public static short ToInt16(string? value, short defaultValue = 0)
            => Convert(value, XmlConvert.ToInt16, defaultValue);

        public static int ToInt32(string? value, int defaultValue = 0)
            => Convert(value, XmlConvert.ToInt32, defaultValue);

        public static long ToInt64(string? value, long defaultValue = 0)
            => Convert(value, XmlConvert.ToInt64, defaultValue);

        public static sbyte ToSByte(string? value, sbyte defaultValue = 0)
            => Convert(value, XmlConvert.ToSByte, defaultValue);

        public static float ToSingle(string? value, float defaultValue = 0)
            => Convert(value, XmlConvert.ToSingle, defaultValue);

        public static TimeSpan ToTimeSpan(string? value, TimeSpan? defaultValue = null)
            => Convert(value, XmlConvert.ToTimeSpan, defaultValue ?? new(0));

        public static ushort ToUInt16(string? value, ushort defaultValue = 0)
            => Convert(value, XmlConvert.ToUInt16, defaultValue);

        public static uint ToUInt32(string? value, uint defaultValue = 0)
            => Convert(value, XmlConvert.ToUInt32, defaultValue);

        public static ulong ToUInt64(string? value, ulong defaultValue = 0)
            => Convert(value, XmlConvert.ToUInt64, defaultValue);
    }
}
