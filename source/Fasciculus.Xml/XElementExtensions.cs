using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace Fasciculus.Xml
{
    /// <summary>
    /// Extensions for <see cref="XElement"/>.
    /// </summary>
    public static class XElementExtensions
    {
        /// <summary>
        /// Opens a reader for the given <paramref name="element"/>, passes it to the given <paramref name="read"/> function,
        /// closes the reader and returns the <paramref name="read"/>'s result.
        /// </summary>
        public static string ReadContent(this XElement? element, Func<XmlReader, string> read)
        {
            if (element is not null)
            {
                using XmlReader reader = element.CreateReader();

                reader.MoveToContent();

                return read(reader);
            }

            return string.Empty;
        }

        /// <summary>
        /// Returns the inner XML of the given <paramref name="element"/>.
        /// </summary>
        public static string InnerXml(this XElement? element)
            => element.ReadContent(r => r.ReadInnerXml());

        public static bool GetBool(this XElement element, XName name, bool defaultValue = false)
            => element.Attribute(name).ToBool(defaultValue);

        public static void SetBool(this XElement element, XName name, bool value)
            => element.SetAttributeValue(name, value);

        public static byte GetByte(this XElement element, XName name, byte defaultValue = 0)
            => element.Attribute(name).ToByte(defaultValue);

        public static void SetByte(this XElement element, XName name, byte value)
            => element.SetAttributeValue(name, value);

        public static DateTime GetDateTime(this XElement element, XName name, DateTime? defaultValue, XmlDateTimeSerializationMode mode)
            => element.Attribute(name).ToDateTime(defaultValue, mode);

        public static DateTime GetDateTime(this XElement element, XName name, XmlDateTimeSerializationMode mode)
            => element.GetDateTime(name, null, mode);

        public static DateTime GetDateTime(this XElement element, XName name, DateTime defaultValue)
            => element.GetDateTime(name, defaultValue, XmlDateTimeSerializationMode.Utc);

        public static DateTime GetDateTime(this XElement element, XName name)
            => element.GetDateTime(name, null, XmlDateTimeSerializationMode.Utc);

        public static void SetDateTime(this XElement element, XName name, DateTime value)
            => element.SetAttributeValue(name, value.ToUniversalTime());

        public static DateTimeOffset GetDateTimeOffset(this XElement element, XName name, DateTimeOffset? defaultValue = null)
            => element.Attribute(name).ToDateTimeOffset(defaultValue);

        public static void SetDateTimeOffset(this XElement element, XName name, DateTimeOffset value)
            => element.SetAttributeValue(name, value);

        public static decimal GetDecimal(this XElement element, XName name, decimal defaultValue = 0)
            => element.Attribute(name).ToDecimal(defaultValue);

        public static void SetDecimal(this XElement element, XName name, decimal value)
            => element.SetAttributeValue(name, value);

        public static double GetDouble(this XElement element, XName name, double defaultValue = 0)
            => element.Attribute(name).ToDouble(defaultValue);

        public static void SetDouble(this XElement element, XName name, double value)
            => element.SetAttributeValue(name, value);

        public static Guid GetGuid(this XElement element, XName name, Guid? defaultValue = null)
            => element.Attribute(name).ToGuid(defaultValue);

        public static void SetGuid(this XElement element, XName name, Guid value)
            => element.SetAttributeValue(name, value);

        public static short GetInt16(this XElement element, XName name, short defaultValue = 0)
            => element.Attribute(name).ToInt16(defaultValue);

        public static void SetInt16(this XElement element, XName name, short value)
            => element.SetAttributeValue(name, value);

        public static int GetInt32(this XElement element, XName name, int defaultValue = 0)
            => element.Attribute(name).ToInt32(defaultValue);

        public static void SetInt32(this XElement element, XName name, int value)
            => element.SetAttributeValue(name, value);

        public static long GetInt64(this XElement element, XName name, long defaultValue = 0)
            => element.Attribute(name).ToInt64(defaultValue);

        public static void SetInt64(this XElement element, XName name, long value)
            => element.SetAttributeValue(name, value);

        public static sbyte GetSByte(this XElement element, XName name, sbyte defaultValue = 0)
            => element.Attribute(name).ToSByte(defaultValue);

        public static void SetSByte(this XElement element, XName name, sbyte value)
            => element.SetAttributeValue(name, value);

        public static float GetSingle(this XElement element, XName name, float defaultValue = 0)
            => element.Attribute(name).ToSingle(defaultValue);

        public static void SetSingle(this XElement element, XName name, float value)
            => element.SetAttributeValue(name, value);

        public static TimeSpan GetTimeSpan(this XElement element, XName name, TimeSpan? defaultValue = null)
            => element.Attribute(name).ToTimeSpan(defaultValue);

        public static void SetTimeSpan(this XElement element, XName name, TimeSpan value)
            => element.SetAttributeValue(name, value);

        public static ushort GetUInt16(this XElement element, XName name, ushort defaultValue = 0)
            => element.Attribute(name).ToUInt16(defaultValue);

        public static void SetUInt16(this XElement element, XName name, ushort value)
            => element.SetAttributeValue(name, value);

        public static uint GetUInt32(this XElement element, XName name, uint defaultValue = 0)
            => element.Attribute(name).ToUInt32(defaultValue);

        public static void SetUInt32(this XElement element, XName name, uint value)
            => element.SetAttributeValue(name, value);

        public static ulong GetUInt64(this XElement element, XName name, ulong defaultValue = 0)
            => element.Attribute(name).ToUInt64(defaultValue);

        public static void SetUInt64(this XElement element, XName name, ulong value)
            => element.SetAttributeValue(name, value);

        public static string[] GetStrings(this XElement element, XName name)
            => element.Attribute(name).ToStrings();

        public static void SetStrings(this XElement element, XName name, IEnumerable<string> values)
            => element.SetAttributeValue(name, string.Join(" ", values));

        public static bool[] GetBools(this XElement element, XName name)
            => element.Attribute(name).ToBools();

        public static void SetBools(this XElement element, XName name, IEnumerable<bool> values)
            => element.SetStrings(name, values.Select(XmlConvert.ToString));

        public static byte[] GetBytes(this XElement element, XName name)
            => element.Attribute(name).ToBytes();

        public static void SetBytes(this XElement element, XName name, IEnumerable<byte> values)
            => element.SetStrings(name, values.Select(XmlConvert.ToString));

        public static DateTime[] GetDateTimes(this XElement element, XName name)
            => element.Attribute(name).ToDateTimes();

        public static void SetDateTimes(this XElement element, XName name, IEnumerable<DateTime> values)
            => element.SetStrings(name, values.Select(x => XmlConvert.ToString(x.ToUniversalTime(), XmlDateTimeSerializationMode.Utc)));

        public static DateTimeOffset[] GetDateTimeOffsets(this XElement element, XName name)
            => element.Attribute(name).ToDateTimeOffsets();

        public static void SetDateTimeOffsets(this XElement element, XName name, IEnumerable<DateTimeOffset> values)
            => element.SetStrings(name, values.Select(XmlConvert.ToString));

        public static decimal[] GetDecimals(this XElement element, XName name)
            => element.Attribute(name).ToDecimals();

        public static void SetDecimals(this XElement element, XName name, IEnumerable<decimal> values)
            => element.SetStrings(name, values.Select(XmlConvert.ToString));

        public static double[] GetDoubles(this XElement element, XName name)
            => element.Attribute(name).ToDoubles();

        public static void SetDoubles(this XElement element, XName name, IEnumerable<double> values)
            => element.SetStrings(name, values.Select(XmlConvert.ToString));

        public static Guid[] GetGuids(this XElement element, XName name)
            => element.Attribute(name).ToGuids();

        public static void SetGuids(this XElement element, XName name, IEnumerable<Guid> values)
            => element.SetStrings(name, values.Select(XmlConvert.ToString));

        public static short[] GetInt16s(this XElement element, XName name)
            => element.Attribute(name).ToInt16s();

        public static void SetInt16s(this XElement element, XName name, IEnumerable<short> values)
            => element.SetStrings(name, values.Select(XmlConvert.ToString));

        public static int[] GetInt32s(this XElement element, XName name)
            => element.Attribute(name).ToInt32s();

        public static void SetInt32s(this XElement element, XName name, IEnumerable<int> values)
            => element.SetStrings(name, values.Select(XmlConvert.ToString));

        public static long[] GetInt64s(this XElement element, XName name)
            => element.Attribute(name).ToInt64s();

        public static void SetInt64s(this XElement element, XName name, IEnumerable<long> values)
            => element.SetStrings(name, values.Select(XmlConvert.ToString));

        public static sbyte[] GetSBytes(this XElement element, XName name)
            => element.Attribute(name).ToSBytes();

        public static void SetSBytes(this XElement element, XName name, IEnumerable<sbyte> values)
            => element.SetStrings(name, values.Select(XmlConvert.ToString));

        public static float[] GetSingles(this XElement element, XName name)
            => element.Attribute(name).ToSingles();

        public static void SetSingles(this XElement element, XName name, IEnumerable<float> values)
            => element.SetStrings(name, values.Select(XmlConvert.ToString));

        public static TimeSpan[] GetTimeSpans(this XElement element, XName name)
            => element.Attribute(name).ToTimeSpans();

        public static void SetTimeSpans(this XElement element, XName name, IEnumerable<TimeSpan> values)
            => element.SetStrings(name, values.Select(XmlConvert.ToString));

        public static ushort[] GetUInt16s(this XElement element, XName name)
            => element.Attribute(name).ToUInt16s();

        public static void SetUInt16s(this XElement element, XName name, IEnumerable<ushort> values)
            => element.SetStrings(name, values.Select(XmlConvert.ToString));

        public static uint[] GetUInt32s(this XElement element, XName name)
            => element.Attribute(name).ToUInt32s();

        public static void SetUInt32s(this XElement element, XName name, IEnumerable<uint> values)
            => element.SetStrings(name, values.Select(XmlConvert.ToString));

        public static ulong[] GetUInt64s(this XElement element, XName name)
            => element.Attribute(name).ToUInt64s();

        public static void SetUInt64s(this XElement element, XName name, IEnumerable<ulong> values)
            => element.SetStrings(name, values.Select(XmlConvert.ToString));
    }
}
