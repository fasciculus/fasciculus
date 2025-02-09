---
Title: "Valid HTML Tags"
Published: 2025-02-03
Author: "Roger H. Jörg"
Summary: "Constants for all valid HTML tags?"
---
# Valid HTML Tags

!frontmatter{Author, Published}

I am writing a library to create the API documentation of this site directly from
the source code.

The XML documentation in the source code contains elements specific to this type of
documentation (e.g. `summary` or `paramref`) as well as HTML elements.

To make sure I captured all XML documentation specific elements (or at least those I use
in my comments), the library keeps track of the handled elements and the elements
actually used.

Obviously valid HTML elements are automagically "handled". But where to get a list of
these?

I'm using the [HtmlAgilityPack][1] (also known as "HAP") to manipulate HTML docoments,
but could not find a list of tag names in there.

Since I plan to add some additional HTML functionality anyway, I set up a mini-project
and added the constants [therein][2], using this [HTML Element Reference][3].

Whilst compiling the list, I realized that `<list>`, `<item>` and `<c>` are
XML documentation specific and not valid HTML. I replaced `<list>` and `<item>` with
`<ul>` and `<li>` respectively in the source code where they occurred. The `<c>` elements
get converted to `code` elements in the documentation parser.

[1]: https://github.com/zzzprojects/html-agility-pack/
[2]: https://github.com/fasciculus/fasciculus/blob/main/source/Fasciculus.Html/HtmlConstants.cs
[3]: https://www.w3schools.com/tags/default.asp
