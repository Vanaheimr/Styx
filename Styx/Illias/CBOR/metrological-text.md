# The Metrological Text Format, and CBOR/JSON Conversion

**Status:** Stable.

**Version:** 1.0 (2026-08-18)

**Normative input:** [tag-44252.md](tag-44252.md), the specification of CBOR
tag 44252.

**Point of contact:** Achim Friedland &lt;achim.friedland@graphdefined.com&gt;,
GraphDefined GmbH

**Implementation:** [Vanaheimr Styx](https://github.com/Vanaheimr/Styx) —
`Styx/Illias/Metrology/MetrologicalValue.cs` (the text format) and
`Styx/Illias/CBOR/CBORJSON.cs` (the document conversion), Apache License 2.0.

**Sibling implementation:** [MetrologicalCBOR.TS](https://github.com/Vanaheimr/MetrologicalCBOR.TS),
whose work plan raised the idea this document specifies.

## 1. Why one string

A metrological value is one reading, and in a JSON document it should stay one
value. Spelling it out as an object of four properties —

```json
{ "energy": { "value": 1.10, "unit": "Wh", "prefix": "k" } }
```

— makes every consumer reassemble it, invites half of them to read `value`
alone, and turns a copy into a merge. The alternative is a text that says the
same thing and that a human reads without a schema:

```json
{ "energy": "1.10 kWh" }
```

This only works if the text is **lossless**: CBOR → text → CBOR has to
reproduce the same bytes, or the string is a rendering and not a
representation. That is what Section 2 specifies, and it is the reason the
grammar is strict where a display format would be forgiving.

## 2. The text format

### 2.1 Grammar

```abnf
metrological-value = ( plain / uncertain ) SP unit-part *( "," SP statement )

plain              = number [ scale ]
uncertain          = "(" number SP "±" uncertainty ")" [ scale ]

number             = [ "-" / "+" ] 1*DIGIT [ "." 1*DIGIT ] [ ( "e" / "E" ) [ "-" / "+" ] 1*DIGIT ]
uncertainty        = number                      ; MUST NOT be negative

scale              = ( "×" / "*" ) "10^" [ "-" ] 1*DIGIT

unit-part          = [ prefix ] unit-expression
unit-expression    = unit-factor *( ( "·" / "*" ) unit-factor )
unit-factor        = symbol [ "^" exponent ]
exponent           = [ "-" ] 1*DIGIT [ "/" 1*DIGIT ]

prefix             = "Q"/"R"/"Y"/"Z"/"E"/"P"/"T"/"G"/"M"/"k"/"h"/"da"/
                     "d"/"c"/"m"/"µ"/"n"/"p"/"f"/"a"/"z"/"y"/"r"/"q"

statement          = "k="    number
                   / "p="    number
                   / "dist=" ( "normal" / "rectangular" / "triangular" / "u-shaped" / "t" )
                   / "ν="    number
```

`symbol` is a unit symbol, alias or name of the registry of
[tag-44252.md](tag-44252.md) Section 4. The whole grammar is **anchored**:
the text is the entire string, with no leading or trailing content beyond
whitespace.

### 2.2 What the numbers mean

The digits are data. `1.10` and `1.1` are different readings, and the scale
survives the round trip in both directions — this is the same rule as
Section 3.1 of the tag specification, and for the same reason.

Scientific notation is **accepted but never written**: `4.5e-9 V` is read as
mantissa 45, exponent −10 and comes back as `0.0000000045 V`. A value that
wants the shorter spelling states an SI prefix instead, which is what prefixes
are for.

### 2.3 Where the prefix goes

An SI prefix is folded into the leading unit symbol — `mA`, `kWh`, `nV·Hz^-1/2`
— but **only where that does not change the meaning**:

| Case | Why it is not folded | Written as |
|---|---|---|
| `m²`, `m³` | `km²` reads as square kilometre: 10⁶ m², not the 10³ m² meant | `5×10^3 m²` |
| leading exponent ≠ 1 | `ks^-2` reads as (ks)⁻², a millionth of what is meant | `2×10^3 s^-2` |
| symbol collision | `cd` is the candela, so a centi-day cannot be spelled that way | `1.25×10^-2 d` |

The rule an implementation follows is simply: **fold only what reads back as
itself.** The renderer hands its own candidate to its own parser and falls
back to the explicit scale whenever the answer differs. That is one rule
instead of a table of exceptions, and it cannot go stale when the registry
grows a symbol that collides with a prefixed one.

A `scale` is **not** a general power of ten: only the 25 canonical SI prefix
exponents are valid, exactly as on the wire.

### 2.4 Reading a unit symbol

The whole token is looked up first, and only when that fails is a prefix split
off:

- `cd` → candela, never centi-day. `min` → minute. `Pa` → pascal. `rad`,
  `mol`, `kat`, `t`, `h`, `T`, `Wb`, `lm`, `Gy` likewise.
- `mA` → milli + ampere, because there is no unit `mA`.
- `kg` → kilo + gram, because the registry has the gram and not the kilogram
  (Section 4 of the tag specification).
- `dam` → deca + metre: the longest prefix wins, so `da` beats `d`.

Only the **leading** factor of a product may carry a prefix; the prefix always
applies to the quantity as a whole.

Two consequences worth stating rather than discovering: `dB` reads as
deci + byte, because the bel is not a registered unit — and a `°C` reading
with a prefix is a temperature *difference* (Section 3.3 of the tag
specification).

### 2.5 Uncertainty

The magnitude goes in the parenthesis, everything else the [GUM] lets a
producer state follows the unit as a comma-separated list, in this order:

```
(230.00 ±0.12) V, k=2, p=0.95, dist=normal, ν=45
```

`k` is written only when it is not 1; the others only when they are stated. A
statement without an uncertainty is an error, as is an unknown statement, as
is the same statement twice. The magnitude is the number **as reported** — the
format normalises nothing, exactly as Section 3.4 of the tag specification
requires.

### 2.6 What is accepted beyond the canonical spelling

Input is tolerant where tolerance cannot create ambiguity, and strict
everywhere else. Accepted: `+-` for `±`, `*` for `·`, `nu=` for `ν=`, both
code points of the micro sign (U+00B5, U+03BC) and of the ohm sign (U+2126,
U+03A9), scientific notation, statements in any order, and whitespace around
them. Case is **never** ignored: `m` is milli and `M` is mega, `t` is the
tonne and `T` the tesla.

A metrological text always states a unit. A bare number is not a metrological
value — which is also what keeps the document conversion of Section 3 from
reading prose as a measurement.

### 2.7 Examples

The examples of Section 5 of the tag specification, in this format:

| CBOR | Text |
|---|---|
| `D9ACDC 82 05 04` | `5 A` |
| `D9ACDC 82 18E6 05` | `230 V` |
| `D9ACDC 83 C482201832 04 22` | `5.0 mA` |
| `D9ACDC 83 C48221186E 02 03` | `1.10 kWh` |
| `D9ACDC 84 C482211901F4 04 22 C4822102` | `(5.00 ±0.02) mA` |
| `D9ACDC 84 05 04 00 C4822005` | `(5 ±0.5) A` |
| `D9ACDC 82 C482211903D5 82 820F01 820821` | `9.81 m·s^-2` |
| `D9ACDC 84 C482211959D8 05 00 A201C482210C0202` | `(230.00 ±0.12) V, k=2` |
| `D9ACDC 83 C48220182D 82 820501 820982200228` | `4.5 nV·Hz^-1/2` |

The tenth row of that section, `5.0 mA` with a symbolic unit, has the same text
as the third: the text format states the unit and not how the unit was written
on the wire, so it comes back with the numeric identification (Section 3.3).

## 3. Document-level CBOR/JSON conversion

The base is [RFC 8949] Section 6.1, extended by one rule: **tag 44252 becomes
one JSON string in the format of Section 2.**

### 3.1 CBOR to JSON

| CBOR item | JSON |
|---|---|
| tag 44252 | string in the metrological text format |
| unsigned / negative integer | number, exactly — including beyond 2⁵³ |
| tag 2 / 3 (bignum) | number, exactly |
| tag 4 (decimal fraction) | number, with its scale |
| half / single / double float | number; NaN and the infinities are not covered |
| text string | string |
| byte string | string: Base64URL (default), Base64 or lowercase hex |
| array | array |
| map | object; a non-text key is an error unless stringification is asked for |
| tag 0 / 1 | ISO 8601 string |
| tag 37 | UUID string |
| tag 32 / 33 / 34 / 36 | the string they wrap |
| tag 55799 | transparent |
| true / false / null | native |
| anything else | error, or diagnostic notation ([RFC 8949] Section 8) on request |

Integers beyond 2⁵³ are written as numbers and not as strings. They are exact
in the document and exact on the way back; what they are not is safe in
JavaScript's `JSON.parse`, which is a property of that parser and not of the
document.

### 3.2 JSON to CBOR

Strings are try-parsed against the grammar of Section 2; a full match becomes
tag 44252, everything else stays a string. Because the grammar is anchored,
strict and requires a unit, false positives are rare — but `"1 h"` is a
perfectly good measurement *and* a perfectly good piece of prose, so a caller
can decide per [RFC 6901] JSON Pointer path which strings are examined at all.

Numbers never become binary floats: an integer becomes a CBOR integer or a
bignum, everything else an exact decimal fraction (tag 4) built from the digits
as written.

### 3.3 What round-trips, and what does not

**Byte for byte**, given deterministic CBOR encoding: metrological values,
text strings, numbers, booleans, null, arrays, and maps with text keys.

**Not**, because JSON has no room for the distinction and guessing it back
would be worse than losing it:

- byte strings — a string of Base64 is just a string;
- non-text map keys — a COSE map comes back with text keys;
- binary floats — they come back as exact decimals;
- tags 0, 1 and 37 — they come back as the strings they became;
- anything rendered in diagnostic notation.

A metrological value written with a symbolic unit (`"A"` instead of `4`) comes
back with the numeric identification, which is what the canonical encoding of
Section 6 of the tag specification asks for.

## 4. References

- [RFC 8949] C. Bormann, P. Hoffman, *Concise Binary Object Representation (CBOR)*, STD 94, December 2020. <https://www.rfc-editor.org/rfc/rfc8949>
- [RFC 6901] P. Bryan, K. Zyp, M. Nottingham, *JavaScript Object Notation (JSON) Pointer*, April 2013. <https://www.rfc-editor.org/rfc/rfc6901>
- [RFC 8259] T. Bray, *The JavaScript Object Notation (JSON) Data Interchange Format*, STD 90, December 2017. <https://www.rfc-editor.org/rfc/rfc8259>
- [RFC 4648] S. Josefsson, *The Base16, Base32, and Base64 Data Encodings*, October 2006. <https://www.rfc-editor.org/rfc/rfc4648>
- [GUM] JCGM 100:2008, *Evaluation of measurement data — Guide to the expression of uncertainty in measurement*, BIPM. <https://www.bipm.org/en/committees/jc/jcgm/publications>
- [SI] BIPM, *The International System of Units (SI)*, 9th edition, 2019.
