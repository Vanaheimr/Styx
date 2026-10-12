# Vanaheimr Styx canonical JSON and deterministic CBOR

Applications that hash or sign JSON documents need **one byte sequence per
document**, independent of property order, whitespace and the library that
wrote it. Styx provides two such representations of the same JSON document:

- **canonical JSON** — `CanonicalJSON.ToUTF8Bytes(…)`, and
- **deterministic CBOR** — `CBORJSON.ToCBOR(…)` written with
  `CBORWriterOptions.Canonical`.

This document specifies both, as implemented, so that other implementations
can produce the same bytes. The test vectors below are checked by
`StyxTests/Illias/JSON/CanonicalEncodingVectorsTests.cs`.

The words MUST, MUST NOT, SHOULD and MAY are used as in RFC 2119.


## 1. Reading JSON documents to be hashed

A JSON document is a tree of objects, arrays, strings, numbers, `true`,
`false` and `null` (RFC 8259). Applications that read JSON text in order to
hash it again MUST keep two things exactly as they were written:

- **numbers with a fraction or an exponent are decimals**, never binary
  floating-point numbers, so that `1.50` stays `1.50`
  (Newtonsoft: `FloatParseHandling.Decimal`);
- **strings stay strings**, even when they look like a point in time
  (Newtonsoft: `DateParseHandling.None`).

An object MUST NOT have two properties of the same name.


## 2. Canonical JSON

The canonical JSON of a document is its UTF-8 encoded text, without a byte
order mark and without any whitespace outside of strings, written as follows.

### 2.1 Objects

Properties are written in the ascending **ordinal order of their names as
UTF-16 code units**, as in RFC 8785 Section 3.2.3: `"B"` before `"a"` before
`"aa"` before `"b"`, and a character beyond the Basic Multilingual Plane
sorts by its high surrogate (`U+1F600`, `D83D DE00`, before `U+FF61`).
Nested objects are sorted alike.

### 2.2 Arrays

Arrays keep the order of their items.

### 2.3 Strings

A string is written in double quotes. These characters are escaped:

| Character(s)                                                        | Written as                   |
|---------------------------------------------------------------------|------------------------------|
| `"` and `\`                                                         | `\"` and `\\`                |
| U+0008, U+000C, U+000A, U+000D, U+0009                              | `\b`, `\f`, `\n`, `\r`, `\t` |
| other control characters: U+0000…U+001F, U+007F…U+009F              | `\uXXXX`                     |
| space separators except U+0020, e.g. U+00A0, U+1680, U+2000…U+200A, U+202F, U+205F, U+3000 | `\uXXXX` |
| line and paragraph separator U+2028, U+2029                         | `\uXXXX`                     |
| private use characters, e.g. U+E000                                 | `\uXXXX`                     |
| unassigned code points and noncharacters, e.g. U+0378, U+FDD0, U+FFFE | `\uXXXX`                   |
| the byte order mark U+FEFF                                          | `\uXXXX`                     |
| **every** character beyond the Basic Multilingual Plane, U+10000…U+10FFFF | `\uXXXX\uXXXX` (its surrogate pair) |

`XXXX` are four **uppercase** hexadecimal digits. Every other character is
written as it is, in UTF-8, including `/`, `<`, `>`, `&`, `'`, `` ` ``,
`+`, letters and combining marks of every script, and format characters such
as U+00AD, U+200B, U+200D and U+2066.

A string MUST NOT contain an unpaired surrogate; Styx writes U+FFFD instead,
so that such strings can not be told apart from ones that contain U+FFFD.

This escaping is the one of .NET's `JavaScriptEncoder.UnsafeRelaxedJsonEscaping`.
Which code points are *unassigned* depends on the Unicode version of the
runtime, see section 5.

### 2.4 Numbers

- An **integer** is written in decimal digits, with a leading `-` when
  negative, without a leading `+` and without leading zeros. Its range is
  not limited: `18446744073709551615` and `-9223372036854775809` are fine.
- A **decimal** is written in plain decimal notation **keeping its scale**:
  `1.0`, `1.50`, `0.000` and `0.10` stay as they are, an exponent is never
  written (`1e-7` is written `0.0000001`, `1.5e2` is written `150`). A
  negative zero loses its sign (`-0.0` is written `0.0`). Decimals follow
  `System.Decimal`: at most 28 decimal places and a mantissa of at most
  96 bits.
- Binary floating-point numbers are not part of this profile (see section 1).

### 2.5 Literals

`true`, `false` and `null`.

### 2.6 Relation to RFC 8785

The order of properties is the one of the JSON Canonicalization Scheme
(RFC 8785), but numbers and strings are written differently: RFC 8785 writes
numbers as IEEE 754 doubles (`1.50` becomes `1.5`), and it escapes only
control characters, `"` and `\`. Canonical JSON of Styx is therefore **not**
RFC 8785 JSON.


## 3. Deterministic CBOR of a JSON document

The deterministic CBOR of a JSON document is the CBOR data item that
`CBORJSON.ToCBOR` (default `CBORJSONOptions`) makes of it, encoded following
the **Core Deterministic Encoding Requirements** of RFC 8949 Section 4.2.1:

- every head has its shortest form, every length is definite;
- the keys of a map are sorted in the **bytewise lexicographic order of their
  encodings**, so a shorter key comes first: `"a"`, `"b"`, `"c"`, `"d"`,
  then `"aa"` (unlike canonical JSON);
- a map has no duplicate keys.

| JSON                                     | CBOR                                                                          |
|------------------------------------------|-------------------------------------------------------------------------------|
| object                                   | map (major type 5) with text keys                                             |
| array                                    | array (major type 4)                                                          |
| string                                   | text string (major type 3), never guessed to be a reading or a point in time  |
| `true`, `false`, `null`                  | `f5`, `f4`, `f6`                                                              |
| integer in −2⁶⁴ … 2⁶⁴−1                  | major type 0 or 1                                                             |
| larger integer                           | bignum, tag 2 or 3                                                            |
| decimal with decimal places, e.g. `1.50` | decimal fraction, tag 4: `[exponent, mantissa]` with exponent = −(decimal places), e.g. `4([-2, 150])`; the mantissa is an integer or a bignum |
| decimal without decimal places, e.g. `1.5e2` or a `Decimal` 22000 | the integer it equals, as above: `150`, `22000` |

Under these options a string never becomes a metrological value (tag 44252)
or a point in time (tag 0); a caller may ask for that with
`CBORJSONOptions.Readings` or `DetectMetrologicalValues`, and a document whose
CBOR is hashed MUST say so where it does.

So the deterministic CBOR follows from the canonical JSON alone: whether a number
was held as a decimal or as an integer, or read from text, does not matter.
`CBORJSON.ToJSON` converts the CBOR back: a decimal fraction becomes a decimal
with the same decimal places, so the canonical JSON of the converted document
equals the canonical JSON of the original one.


## 4. Test vectors

The hex strings are the complete deterministic CBOR encodings.

| JSON input                                                              | Canonical JSON                                                   | Deterministic CBOR |
|-------------------------------------------------------------------------|------------------------------------------------------------------|--------------------|
| `{ "b" : 1, "a" : [ 1.0, 1, 1.50, 100, -2, 0.10 ], "aa" : "x", "c" : null, "d" : true }` | `{"a":[1.0,1,1.50,100,-2,0.10],"aa":"x","b":1,"c":null,"d":true}` | `a5 6161 86 c482200a 01 c482211896 1864 21 c482210a 6162 01 6163 f6 6164 f5 626161 6178` |
| `{"aa":1,"b":2,"a":3,"é":4}`                                            | `{"a":3,"aa":1,"b":2,"é":4}`                                     | `a4 6161 03 6162 02 626161 01 62c3a9 04` |
| `{"o":{},"a":[],"s":""}`                                                | `{"a":[],"o":{},"s":""}`                                         | `a3 6161 80 616f a0 6173 60` |
| `[22000.0, -1.5, 0.000, 79228162514264337593543950335]`                 | `[22000.0,-1.5,0.000,79228162514264337593543950335]`             | `84 c482201a00035b60 c482202e c4822200 c24cffffffffffffffffffffffff` |
| `[18446744073709551615, -9223372036854775809]`                          | `[18446744073709551615,-9223372036854775809]`                    | `82 1bffffffffffffffff 3b8000000000000000` |
| `[22000, -0, 79228162514264337593543950335, 1.5e2]`, the first three held as decimals | `[22000,0,79228162514264337593543950335,150]` | `84 1955f0 00 c24cffffffffffffffffffffffff 1896` |
| `[1.0, 1, 1.50, 0.000, -0.0, 100, 1e-7, 1.5e2, 1E-2]`                   | `[1.0,1,1.50,0.000,0.0,100,0.0000001,150,0.01]`                  | |
| `["1.10 kWh", "2026-10-12T12:00:00Z"]`                                  | `["1.10 kWh","2026-10-12T12:00:00Z"]`                            | `82 68312e3130206b5768 74323032362d31302d31325431323a30303a30305a` |
| `{"b":1,"a":2,"B":3,"é":4,"aa":5,"😀":6,"｡":7}`                         | `{"B":3,"a":2,"aa":5,"b":1,"é":4,"\uD83D\uDE00":6,"｡":7}`        | |


## 5. Open points

- **Unicode version.** Whether a code point is escaped depends, for the
  unassigned ones, on the Unicode tables of the runtime. Documents limited to
  assigned characters are not affected.
