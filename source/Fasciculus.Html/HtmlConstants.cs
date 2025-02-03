using System;
using System.Collections.Generic;

namespace Fasciculus.Html
{
    /// <summary>
    /// Constants used in HTML 5.
    /// </summary>
    public static class HtmlConstants
    {
        internal class TagNameSet : SortedSet<string>
        {
            public TagNameSet(bool _)
            {
                Add("a");
                Add("abbr");
                Add("address");
                Add("area");
                Add("article");
                Add("aside");
                Add("audio");
                Add("b");
                Add("base");
                Add("bdi");
                Add("bdo");
                Add("blockquote");
                Add("body");
                Add("br");
                Add("button");
                Add("canvas");
                Add("caption");
                Add("cite");
                Add("code");
                Add("col");
                Add("colgroup");
                Add("data");
                Add("datalist");
                Add("dd");
                Add("del");
                Add("details");
                Add("dfn");
                Add("dialog");
                Add("div");
                Add("dl");
                Add("dt");
                Add("em");
                Add("embed");
                Add("fieldset");
                Add("figcaption");
                Add("figure");
                Add("footer");
                Add("form");
                Add("h1");
                Add("h2");
                Add("h3");
                Add("h4");
                Add("h5");
                Add("h6");
                Add("head");
                Add("header");
                Add("hgroup");
                Add("hr");
                Add("html");
                Add("i");
                Add("iframe");
                Add("img");
                Add("input");
                Add("ins");
                Add("kbd");
                Add("label");
                Add("legend");
                Add("li");
                Add("link");
                Add("main");
                Add("map");
                Add("mark");
                Add("menu");
                Add("meta");
                Add("meter");
                Add("nav");
                Add("noscript");
                Add("object");
                Add("ol");
                Add("optgroup");
                Add("option");
                Add("output");
                Add("p");
                Add("param");
                Add("picture");
                Add("pre");
                Add("progress");
                Add("q");
                Add("rp");
                Add("rt");
                Add("ruby");
                Add("s");
                Add("samp");
                Add("script");
                Add("search");
                Add("section");
                Add("select");
                Add("small");
                Add("source");
                Add("span");
                Add("strong");
                Add("style");
                Add("sub");
                Add("summary");
                Add("sup");
                Add("svg");
                Add("table");
                Add("tbody");
                Add("td");
                Add("template");
                Add("textarea");
                Add("tfoot");
                Add("th");
                Add("thead");
                Add("time");
                Add("title");
                Add("tr");
                Add("track");
                Add("u");
                Add("ul");
                Add("var");
                Add("video");
                Add("wbr");
            }
        }

        private static readonly Lazy<TagNameSet> tagNames = new(() => new(true), true);

        /// <summary>
        /// All tag names of HTML 5.
        /// </summary>
        public static IReadOnlyCollection<string> TagNames => tagNames.Value;
    }
}
