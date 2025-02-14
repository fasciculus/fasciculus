---
Title: "The Fuzzy String Desaster"
Published: 2025-02-14
Author: "Roger H. Jörg"
Summary: "Where creating fuzzy strings blow into my face."
---
# The Fuzzy String Desaster

... or where creating fuzzy strings blow into my face.

!frontmatter{Author, Published}

## Motivation

While researching possibilities to create a future search engine or auto-completion feature, I
stumbled over fuzzyness of strings. This could be interesting for search engines or auto-completion.

## Edit Distance

The edit distance between two strings is defined as the number of single-character edits
(insertions, deletions or replacements) required to transform one string into the other.

Some examples:

| First String | Second String | Edits | Edit Distance |
| --- | --- | --- | :---: |
| `"ab"` | `"acb"` | insert `c` | 1 |
| `"axe"` | `"aeg"` | remove `x`, insert `g` | 2 |
| `"Hello"` | `"hello"` | replace `H` with `h` | 1 |
| `"nada"` | `"nada"` | | 0 |

Given a list of possible entries for some auto-completion, the edit distance between the typed-in
text and any entry in the list may control the order of the proposed completions: the lower the
edit distance, the higher up the proposal.

## Fuzzy Strings

Now this is somewhat the reverse of the edit distance. Given some arbitrary `start` string and
some maximum edit distance `maxDistance` we create all strings (the fuzzy strings) that have an edit
distance up to the required maximum.

Removing characters from `start` is easy. The number of created fuzzy strings is proportional
to the number of characters in `start` and may even be 0, if `start` is the empty string.

Insertions and replacements are a whole new ballgame.

C# (as well as most modern programming languages) works with 16-bit Unicode characters. There
are some of them (up to $2^{16} = 65536$). So even a short `start` like `"ab"` creates more
than 300K fuzzy strings at an edit distance of 1.

In an attempt to drastically reduce this large number, I had the following idea:

- Convert the `start` string into a byte array `startBytes` (using UTF8).
- Create fuzzy byte arrays from `startBytes`
- Try to convert these fuzzy byte arrays back into strings (again using UTF8). Not all byte
  arrays are valid UTF8 strings.

## Fuzzy Byte Arrays (`FuzzyBytes`)

The edit distance between two byte arrays can be defined the same as the edit distance between two
strings, but on a byte-level instead of a character-level.

Creating `FuzzyBytes` from a given start array `startBytes` works the same as creating
fuzzy strings, but now with just 256 possible values for insertions or replacements.

Again, the removal operation creates `FuzzyBytes` proportional to the length of `startBytes`.

The insertion operation creates $256 (n + 1)$, the replacement operation $256 n - 1$ `FuzzyBytes` at
an edit distance of 1 where $n$ is the length of `startBytes`.

Still, the number of created `FuzzyBytes` grows exponentially with `maxDistance`. The number
of created `FuzzyBytes` is 65792 with an empty `startBytes` array and a `maxDistance` of 2.

## Fuzzy Strings (Continued)

Now using `FuzzyBytes` I created fuzzy strings. Well, I tried to do so.

```cs
FuzzyString.Generate(start: "Hello, world!", maxDistance: 1, ignoreCase: true);
```
returned 2011 fuzzy strings. Setting `maxDistance` to 2 took more time than my patience allowed for.
I stopped the process after a few minutes.

## Conclusion

Given that a list of proposals may have thousands of entries and waiting tens of thousands of minutes
to create all the fuzzy strings is definively a DESASTER.

Maybe such fuzzy strings can be created using some kind of rule sets according to typical typos.
Where to get such rule sets? What about multi-language support?
