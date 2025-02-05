---
title: "Programmatically Create SVG"
published: 2025-02-05
author: "Roger H. Jörg"
summary: "How to programmatically create SVG files with C#"
---
# Programmatically Create SVG

I am not good at manually painting graphics. Neither do I like using graphics editors
to create them. The result is usually not precise enough for my "gusto".

I prefer creating graphics running a piece of software I write on the fly. Even the
logo on this site was created programmatically.

When it comes to SVG, things get somewhat more complicated.

## Existing Libraries

There are several packages out there to operate on SVG. Namely
[Svg][1] and its adaptions [Svg.Custom][2], [Svg.Model][3] and [Svg.Skia][4].

They all focus on rendering SVGs, which is not my goal. The browser renders them.
I addition, they do not really reflect the fact that SVG is a subset of XML.

## Fasciculus.Xml

I want to use the BCL classes `XElement` and `XAttribute` of the .Net base class library
(BCL) to represent SVG elements and their attributes.

SVG elements have a lot of attributes. Some of them may contain multiple values as in

```xml
<svg viewBox="0 0 160 80" />
```

Since there is no easy way to read or write such attributes, I set up the
Fasciculus.Xml project adding a bunch of helpers and extensions allowing me
to do so.

Now I have very forgiving helpers in the class `XConvert` like

```cs
public static T Convert<T>(string? value, Func<string, T> convert, T defaultValue)
{
    if (value is null) return defaultValue;
    try { return convert(value); } catch { return defaultValue; }
}

public static double ToDouble(string? value, double defaultValue = 0)
    => Convert(value, XmlConvert.ToDouble, defaultValue);
```

These helpers use the existing `XmlConvert` BCL class to convert from string to various
types.

Then there are the extensions for `XAttribute`, like

```cs
public static double ToDouble(this XAttribute? attribute, double defaultValue = 0)
    => XConvert.ToDouble(attribute?.Value, defaultValue);
```

and finally the extension for `XElement`, like

```cs
public static double GetDouble(this XElement element, XName name, double defaultValue = 0)
    => element.Attribute(name).ToDouble(defaultValue);
public static void SetDouble(this XElement element, XName name, double value)
    => element.SetAttributeValue(name, value);
```

where `XAttribute? Attribute(XName name)` and
`void SetAttributeValue(XName name, object? value)` are provided by the `XElement` class.

All the above allows me to write (in `SvgRect`):

```cs
public double X
{
    get => this.GetDouble("x");
    set => this.SetDouble("x", value);
}
```

And, yepp, there are e.g.

```cs
public static double[] ToDoubles(this XAttribute? attribute);
public static double[] GetDoubles(this XElement element, XName name);
public static void SetDoubles(this XElement element, XName name, IEnumerable<double> values);
```

allowing me to get the (four) values out of the `viewBox` attribute.

## Fasciculus.Svg



[1]: https://www.nuget.org/packages/Svg
[2]: https://www.nuget.org/packages/Svg.Custom
[3]: https://www.nuget.org/packages/Svg.Model
[4]: https://www.nuget.org/packages/Svg.Skia