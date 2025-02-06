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
In addition, they do not really reflect the fact that SVG is a subset of XML.

## Fasciculus.Xml

I want to use the BCL classes `XElement` and `XAttribute` of the Net base class library
(BCL) to represent SVG elements and their attributes.

SVG elements have a lot of attributes. Some of them may contain multiple values as in

```xml
<svg viewBox="0 0 160 80" />
```

Since there is no easy way to read or write such attributes, I set up the
Fasciculus.Xml project adding a bunch of helpers and extensions allowing me
to do so.

Now I have very forgiving helpers in the class `XConvert`. E.g.

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

Then there are the extensions for `XAttribute`. E.g.

```cs
public static double ToDouble(this XAttribute? attribute, double defaultValue = 0)
    => XConvert.ToDouble(attribute?.Value, defaultValue);
```

and finally the extension for `XElement`. E.g.

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

With the above out of the way, I started the Fasciculus.Svg project. As of this
writing, `SvgSvg` (the `svg` element) and `ScgRect` are implemented only. They both
come with their associated "builder" allowing fluent creation.

The code

```cs
SvgSvg svg = SvgSvg.Create(0, 0, 160, 80).Width("16rem").Height("8rem");
SvgRect rect1 = SvgRect.Create(10, 10, 40, 20).Fill("red").Stroke("black").StrokeWidth("2");
SvgRect rect2 = SvgRect.Create(30, 20, 100, 40).Fill("green").Stroke("black").StrokeWidth("2");
SvgRect rect3 = SvgRect.Create(110, 50, 40, 20).Fill("blue").Stroke("black").StrokeWidth("2");
svg.Add(rect1); svg.Add(rect2); svg.Add(rect3);
Console.WriteLine(svg.ToString());
```

creates the grapic

<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 160 80" width="16rem" height="8rem">
  <rect x="10" y="10" width="40" height="20" fill="red" stroke="black" stroke-width="2" />
  <rect x="30" y="20" width="100" height="40" fill="green" stroke="black" stroke-width="2" />
  <rect x="110" y="50" width="40" height="20" fill="blue" stroke="black" stroke-width="2" />
</svg>

## Side-Note `xmlns`

To be able to preview the generated SVG in Edge or Chrome, the `svg` element must have its
`xmlns` attribute set properly:

```xml
<svg xmlns="http://www.w3.org/2000/svg" />
```

Visual Studio Code shows a preview, if Simon Siefke's "Svg Preview" extension is installed,
even without the `xmlns` attribute. When embedding SVG into HTML documents, the attribute
isn't required neither, but it doesn't hurt.

I had some difficulties setting this attribute until I stumbled over Microsot's
[article][5] describing how to do it.

[1]: https://www.nuget.org/packages/Svg
[2]: https://www.nuget.org/packages/Svg.Custom
[3]: https://www.nuget.org/packages/Svg.Model
[4]: https://www.nuget.org/packages/Svg.Skia
[5]: https://learn.microsoft.com/en-us/dotnet/fundamentals/runtime-libraries/system-xml-linq-xnamespace#create-a-default-namespace