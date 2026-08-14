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
defines tag **44252** (`0xACDC`, First-Come-First-Served range) as follows:

```cddl
metrological-value = #6.44252([
    value         : int / decfrac,   ; decfrac = #6.4([exponent: int, mantissa: int / bignum])
    unit          : uint / tstr,     ; registry id (write default) or unit symbol
    ? prefix      : int,             ; SI prefix as a power of ten (3 = kilo, -3 = milli), absent = 0
    ? uncertainty : int / decfrac    ; ± standard uncertainty u (k=1), same unit & prefix, >= 0
])
```

### Semantics

1. **`value`** is the reading of a physical quantity as displayed by the
   measuring instrument: An integer, or a decimal fraction (tag 4) whose
   decimal scale is preserved — `1.10 kWh` is not `1.1 kWh`.
   Binary floating-point values are **not allowed**.
2. **`unit`** is either an unsigned integer from the unit registry below,
   or a text string holding the unit symbol (or one of its aliases).
3. **`prefix`** is the decimal power of the SI prefix the value is scaled
   by. It is deliberately kept separate from the value: `5.00 mA` stays
   `5.00 mA` and does not silently become `0.005 A`. Only the 25 canonical
   SI prefix exponents (0, ±1, ±2, ±3, ±6, …, ±30) are valid. When absent,
   the prefix is 0 — it must however be written explicitly whenever an
   `uncertainty` follows.
4. **`uncertainty`** is the symmetric **standard measurement uncertainty u**
   with coverage factor k=1, as defined by the *Guide to the Expression of
   Uncertainty in Measurement* (GUM, JCGM 100:2008, BIPM), expressed in the
   same unit and prefix as the value and encoded by the same rules.
   It is never negative. Expanded uncertainties U = k·u must be normalized
   to k=1 by the producer — the format deliberately carries no coverage
   factor.
5. Array lengths other than 2..4, unknown units, non-canonical prefix
   exponents and negative uncertainties are **errors**. Extensibility means
   a new tag, not a longer array.

### Examples

| Quantity          | Encoding                                         | Size |
|-------------------|--------------------------------------------------|------|
| `5 A`             | `D9 ACDC 82 05 04`                               |  5 B |
| `230 V`           | `D9 ACDC 82 18E6 0E`                             |  7 B |
| `5.0 mA`          | `D9 ACDC 83 C48220 1832 04 22`                   | 11 B |
| `1.10 kWh`        | `D9 ACDC 83 C48221 186E 1832 03`                 | 12 B |
| `(5.00 ±0.02) mA` | `D9 ACDC 84 C48221 1901F4 04 22 C4822102`        | 16 B |

In diagnostic notation, `5.0 mA` reads as `44252([4([-1, 50]), 4, -3])`.

### The unit registry

The numeric identifications are **stable** and must never be renumbered.
`0` is reserved and never valid on the wire; identifications `>= 32768` are
available for user-registered units via `UnitOfMeasure.Register(...)`.

| Range   | Units                                                                     |
|---------|---------------------------------------------------------------------------|
| 1..7    | SI base in SI order: `s`(1), `m`(2), **`g`(3)**, `A`(4), `K`(5), `mol`(6), `cd`(7) |
| 8..29   | Named derived SI: `Hz`(8), `N`(9), `Pa`(10), `J`(11), `W`(12), `C`(13), `V`(14), `F`(15), `Ω`(16), `S`(17), `Wb`(18), `T`(19), `H`(20), `°C`(21), `lm`(22), `lx`(23), `Bq`(24), `Gy`(25), `Sv`(26), `kat`(27), `rad`(28), `sr`(29) |
| 30..49  | Accepted non-SI: `min`(30), `h`(31), `d`(32), `°`(33), `l`(34), `t`(35), `%`(36), `‰`(37), `ppm`(38) |
| 50..69  | Electrotechnical: `Wh`(50), `VA`(51), `var`(52), `varh`(53), `Ah`(54)     |
| 70..89  | Data: `bit`(70), `B`(71), `bit/s`(72), `B/s`(73)                          |
| 90..    | Geometric: `m²`(90), `m³`(91)                                             |
| ≥ 32768 | User-registered units                                                     |

Note the metrological subtlety of mass: The SI base unit is the kilogram,
but SI prefixes attach to the **gram** — therefore the registry contains
the gram (3) and a kilogram is expressed as `(value, Gram, SIPrefix.Kilo)`.

### Interoperability

A generic CBOR decoder without knowledge of tag 44252 still reads a
well-formed tagged array of plain integers and standard tag-4 decimal
fractions. Deterministic encoding (RFC 8949 §4.2.1) is fully supported,
which keeps the format ready for COSE signatures over measurement data.

*IANA note:* Tag 44252 lies in the First-Come-First-Served range and is,
as of 2026-08-14, unassigned. This document serves as the semantics
reference for a future FCFS registration.
