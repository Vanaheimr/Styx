# IANA registration of CBOR tag 44252

**Status: prepared, not yet submitted.**

This document holds the ready-to-send registration request for CBOR tag
44252 (`0xACDC`) in the IANA registry
["Concise Binary Object Representation (CBOR) Tags"](https://www.iana.org/assignments/cbor-tags/cbor-tags.xhtml),
together with everything needed to submit it and to record the outcome.

## Why this is possible without an RFC

The registry defines three registration policies ([RFC 8949], Section 9.2):

| Tag range | Policy |
|---|---|
| 0..23 | Standards Action |
| 24..32767 | Specification Required |
| 32768..18446744073709551615 | **First Come First Served** |

Tag 44252 lies in the First Come First Served range: no RFC, no working group
and no expert review are required. A stable, publicly reachable description
of the semantics and a point of contact suffice.

**Availability check (2026-08-14):** 44252 is unassigned. The nearest
neighbours are 43000/43001 (complex numbers); the range 43002..49999 is
entirely unassigned. Note that First Come First Served literally means first
come, first served — the number is only ours once IANA has recorded it.

## The request

Submit exactly this template:

```
Tag:                             44252

Data item:                       array

Semantics (short form):          Metrological value (quantity with unit of
                                 measure, SI prefix and measurement uncertainty)

Point of contact:                Achim Friedland <achim.friedland@graphdefined.com>

Description of semantics (URL):  https://github.com/Vanaheimr/Styx/blob/master/Styx/Illias/CBOR/tag-44252.md
```

The field names follow the templates of existing First Come First Served
registrations (verified against the submitted templates of tags 40919 and
41728, which IANA publishes under
`https://www.iana.org/assignments/cbor-tags/template/<tag>`).

### How to submit

Use the IANA request form for protocol parameter assignments:

<https://www.iana.org/form/protocol-assignment>

Select the registry "Concise Binary Object Representation (CBOR) Tags" and
paste the template above. Alternatively the request can be mailed to
<iana@iana.org> with the registry name in the subject line. IANA confirms by
mail and assigns a ticket number; First Come First Served requests are
usually processed within a few business days.

### Please check before sending

- **The contact details become public.** IANA publishes the contact name in
  the registry's *Reference* column and the mail address in the archived
  template (obfuscated, e.g. `achim.friedland&graphdefined.com`). The
  address used here is the one already present in every source file header
  of this repository, so nothing new is disclosed — swap it for a role
  address if you would rather not have a personal one in the registry.
- **The URL is a `master` branch link.** It stays valid as long as the file
  keeps its path, and it always shows the current specification. If you
  prefer immutability over currentness, use a commit permalink instead —
  but then errata will never reach readers of the registry.

## What the registry entry will look like

| Tag | Data Item | Semantics | Reference | Template |
|---|---|---|---|---|
| 44252 | array | Metrological value (quantity with unit of measure, SI prefix and measurement uncertainty) | Achim_Friedland | template/44252 |

## After the assignment

1. Note the assignment date and the IANA ticket number in this file, and set
   the status above to *registered*.
2. Update Section 8 of [tag-44252.md](tag-44252.md) and the closing note of
   [README.md](README.md) from "unassigned" to the assignment.
3. The tag number itself needs no code change: it already lives in exactly
   one place, `CBORTag.MetrologicalValue` in
   [CBORTag.cs](CBORTag.cs), whose XML documentation is worded to serve as
   the registration's semantics text.

## If 44252 is taken in the meantime

Should IANA report the number as assigned, pick another free number in the
First Come First Served range and change `CBORTag.MetrologicalValue` — that
single constant, the specification, this document and the golden vectors in
`StyxTests/Illias/Metrology/MetrologicalValueTests.cs` are the only places
where the number appears.

[RFC 8949]: https://www.rfc-editor.org/rfc/rfc8949
