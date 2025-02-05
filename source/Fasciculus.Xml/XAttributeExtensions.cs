using System;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace Fasciculus.Xml
{
    /// <summary>
    /// Extensions for
    /// </summary>
    public static class XAttributeExtensions
    {
        public static bool ToBool(this XAttribute? attribute, bool defaultValue = false)
            => XConvert.ToBool(attribute?.Value, defaultValue);

        public static byte ToByte(this XAttribute? attribute, byte defaultValue = 0)
            => XConvert.ToByte(attribute?.Value, defaultValue);

        public static DateTime ToDateTime(this XAttribute? attribute, DateTime? defaultValue = null,
            XmlDateTimeSerializationMode mode = XmlDateTimeSerializationMode.Utc)
            => XConvert.ToDateTime(attribute?.Value, defaultValue, mode);

        public static DateTimeOffset ToDateTimeOffset(this XAttribute? attribute, DateTimeOffset? defaultValue = null)
            => XConvert.ToDateTimeOffset(attribute?.Value, defaultValue);

        public static decimal ToDecimal(this XAttribute? attribute, decimal defaultValue = 0)
            => XConvert.ToDecimal(attribute?.Value, defaultValue);

        public static double ToDouble(this XAttribute? attribute, double defaultValue = 0)
            => XConvert.ToDouble(attribute?.Value, defaultValue);

        public static Guid ToGuid(this XAttribute? attribute, Guid? defaultValue = null)
            => XConvert.ToGuid(attribute?.Value, defaultValue);

        public static short ToInt16(this XAttribute? attribute, short defaultValue = 0)
            => XConvert.ToInt16(attribute?.Value, defaultValue);

        public static int ToInt32(this XAttribute? attribute, int defaultValue = 0)
            => XConvert.ToInt32(attribute?.Value, defaultValue);

        public static long ToInt64(this XAttribute? attribute, long defaultValue = 0)
            => XConvert.ToInt64(attribute?.Value, defaultValue);

        public static sbyte ToSByte(this XAttribute? attribute, sbyte defaultValue = 0)
            => XConvert.ToSByte(attribute?.Value, defaultValue);

        public static float ToSingle(this XAttribute? attribute, float defaultValue = 0)
            => XConvert.ToSingle(attribute?.Value, defaultValue);

        public static TimeSpan ToTimeSpan(this XAttribute? attribute, TimeSpan? defaultValue = null)
            => XConvert.ToTimeSpan(attribute?.Value, defaultValue);

        public static ushort ToUInt16(this XAttribute? attribute, ushort defaultValue = 0)
            => XConvert.ToUInt16(attribute?.Value, defaultValue);

        public static uint ToUInt32(this XAttribute? attribute, uint defaultValue = 0)
            => XConvert.ToUInt32(attribute?.Value, defaultValue);

        public static ulong ToUInt64(this XAttribute? attribute, ulong defaultValue = 0)
            => XConvert.ToUInt64(attribute?.Value, defaultValue);

        public static string[] ToStrings(this XAttribute? attribute)
        {
            return (attribute?.Value ?? string.Empty)
                .Split([' '])
                .Where(s => !string.IsNullOrEmpty(s))
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .ToArray();
        }

        public static bool[] ToBools(this XAttribute? attribute)
            => attribute.ToStrings().Select(s => XConvert.ToBool(s)).ToArray();

        public static byte[] ToBytes(this XAttribute? attribute)
            => attribute.ToStrings().Select(s => XConvert.ToByte(s)).ToArray();

        public static DateTime[] ToDateTimes(this XAttribute? attribute, XmlDateTimeSerializationMode mode = XmlDateTimeSerializationMode.Utc)
            => attribute.ToStrings().Select(s => XConvert.ToDateTime(s, null, mode)).ToArray();

        public static DateTimeOffset[] ToDateTimeOffsets(this XAttribute? attribute)
            => attribute.ToStrings().Select(s => XConvert.ToDateTimeOffset(s)).ToArray();

        public static decimal[] ToDecimals(this XAttribute? attribute)
            => attribute.ToStrings().Select(s => XConvert.ToDecimal(s)).ToArray();

        public static double[] ToDoubles(this XAttribute? attribute)
            => attribute.ToStrings().Select(s => XConvert.ToDouble(s)).ToArray();

        public static Guid[] ToGuids(this XAttribute? attribute)
            => attribute.ToStrings().Select(s => XConvert.ToGuid(s)).ToArray();

        public static short[] ToInt16s(this XAttribute? attribute)
            => attribute.ToStrings().Select(s => XConvert.ToInt16(s)).ToArray();

        public static int[] ToInt32s(this XAttribute? attribute)
            => attribute.ToStrings().Select(s => XConvert.ToInt32(s)).ToArray();

        public static long[] ToInt64s(this XAttribute? attribute)
            => attribute.ToStrings().Select(s => XConvert.ToInt64(s)).ToArray();

        public static sbyte[] ToSBytes(this XAttribute? attribute)
            => attribute.ToStrings().Select(s => XConvert.ToSByte(s)).ToArray();

        public static float[] ToSingles(this XAttribute? attribute)
            => attribute.ToStrings().Select(s => XConvert.ToSingle(s)).ToArray();

        public static TimeSpan[] ToTimeSpans(this XAttribute? attribute)
            => attribute.ToStrings().Select(s => XConvert.ToTimeSpan(s)).ToArray();

        public static ushort[] ToUInt16s(this XAttribute? attribute)
            => attribute.ToStrings().Select(s => XConvert.ToUInt16(s)).ToArray();

        public static uint[] ToUInt32s(this XAttribute? attribute)
            => attribute.ToStrings().Select(s => XConvert.ToUInt32(s)).ToArray();

        public static ulong[] ToUInt64s(this XAttribute? attribute)
            => attribute.ToStrings().Select(s => XConvert.ToUInt64(s)).ToArray();
    }
}
