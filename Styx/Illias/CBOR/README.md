# Vanaheimr Styx CBOR

A self-contained implementation of the **Concise Binary Object Representation**
(CBOR, [RFC 8949](https://www.rfc-editor.org/rfc/rfc8949)) without any external
dependencies, plus a CBOR extension for **metrological values**.

## Features

- **`CBORWriter`** — a streaming encoder over `IBufferWriter<Byte>`:
  Integer heads always use their shortest form; floating-point values shrink
  to the shortest lossless width (preferred serialization, RFC 8949 §4.1).
  The deterministic mode enforces the *Core Deterministic Encoding
  Requirements* (RFC 8949 §4.2.1) including the bytewise lexicographic
  ordering of map keys and the rejection of duplicate keys.
- **`CBORReader`** — a zero-copy `ref struct` decoder over `ReadOnlySpan<Byte>`:
  Never allocates memory based on claimed lengths, guards against depth bombs
  (`MaxDepth`, tags included), validates UTF-8 per chunk (RFC 8949 §3.2.3)
  and optionally verifies deterministic encoding on-the-fly.
- **`CBORValue`** — a memory-efficient document model (tagged union struct,
  no allocations for scalars) with arbitrary map key types, tag support,
  duplicate key policies and RFC 8949 §8 diagnostic notation.
- **Numbers done right** — the full CBOR integer range −2⁶⁴..2⁶⁴−1
  (`Int128`), bignums (tags 2/3, `BigInteger`) and **exact, scale-preserving
  `System.Decimal` support** via decimal fractions (tag 4): `1.10` and `1.1`
  are different representations and survive the roundtrip.
- **`ParseMandatory` / `ParseOptional`** — the well-known three-way parsing
  contract of the Vanaheimr JSON extensions, for text **and** integer map
  keys (COSE style).

## The metrological value extension (tag 44252, `0xACDC`)

There is no standardized CBOR tag for physical quantities. This library
defines tag **44252** (`0xACDC`, First-Come-First-Served range):

```cddl
metrological-value = #6.44252([
    value         : number,          ; the reading
    unit          : unit-ref,        ; named unit or product of powers
    ? prefix      : int,             ; SI prefix as a power of ten
    ? uncertainty : number / uncertainty-map
])
```

In short: readings are integers or scale-preserving decimal fractions and
**never** binary floats, so `1.10 kWh` stays distinct from `1.1 kWh`. Units
are compact numeric identifications, either a single named unit or a product
of powers with rational exponents, so `V·Hz^-1/2` is expressible. The SI
prefix stays separate from the value, so `5.00 mA` never silently becomes
`0.005 A`. The uncertainty follows the GUM (JCGM 100:2008) and keeps the
coverage factor a calibration certificate was issued with.

| Quantity          | Encoding                                  | Size |
|-------------------|-------------------------------------------|------|
| `5 A`             | `D9 ACDC 82 05 04`                        |  5 B |
| `5.0 mA`          | `D9 ACDC 83 C482201832 04 22`             | 11 B |

In diagnostic notation, `5.0 mA` reads as `44252([4([-1, 50]), 4, -3])`.

A generic CBOR decoder without knowledge of the tag still sees a well-formed
tagged array of plain integers and standard tag-4 decimal fractions.
### Specification and registration

The normative wire format specification lives in the
[OpenChargingTechnology Whitepapers](https://github.com/OpenChargingTechnology/Whitepapers/blob/master/MetrologicalCBOR/README.md)
repository, and its
[worked example](https://github.com/OpenChargingTechnology/Whitepapers/blob/master/MetrologicalCBOR/tag-44252-signed-example.md)
walks a complete charging transaction through three signature layers — meter,
charging station, operator — whose every byte is verified by a test here.

*IANA note:* Tag 44252 lies in the First-Come-First-Served range and is,
as of 2026-08-18, unassigned. The prepared registration request is
[IANA-registration.md](https://github.com/OpenChargingTechnology/Whitepapers/blob/master/MetrologicalCBOR/IANA-registration.md).

### Text format and JSON documents

A metrological value also has a **one-string text form** — `1.10 kWh`,
`9.81 m·s^-2`, `(230.00 ±0.12) V, k=2` — which `ToString()` writes and
`MetrologicalValue.TryParse(String, …)` reads back losslessly.

**`CBORJSON`** converts whole documents between CBOR and JSON on that basis,
so that a measurement stays *one* JSON value instead of falling apart into an
object of four properties:

```csharp
var json = CBORJSON.ToJSONText(cborBytes);
// {"meter":"EVSE-42","readings":["1.10 kWh","(230.00 ±0.12) V, k=2","5.0 mA"]}

var cbor = CBORJSON.ToCBOR(json);          // ...and back into the same bytes
```

Both JSON worlds are served: a Newtonsoft `JToken` tree (`ToJSON` / `ToCBOR`)
and UTF-8 text written and read directly, without a tree in between
(`ToJSONUTF8` / `WriteJSONTo` / `ToCBOR(ReadOnlySpan<Byte>)`). The grammar,
the mapping table and what does and does not round-trip are specified in
[metrological-text.md](https://github.com/OpenChargingTechnology/Whitepapers/blob/master/MetrologicalCBOR/metrological-text.md).
