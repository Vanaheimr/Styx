# A signed metrological record, end to end

A worked example of a charging transaction carried as CBOR: two meter readings
expressed as [metrological values](tag-44252.md), signed by the meter, bundled
by the charging station, and endorsed by the operator before the customer
receives them.

Everything below is real output. The 713 bytes of Section 6 are pinned by
[`SignedMetrologyExampleTests`](../../../StyxTests/Illias/COSE/SignedMetrologyExampleTests.cs),
which derives every other listing in this document from them and verifies all
three signatures against the published public keys. The document is generated
from that constant, never retyped.

Every signature here is **deterministic** ([RFC 6979](https://www.rfc-editor.org/rfc/rfc6979)):
the nonce is derived from the private key and the message rather than drawn at
random. So this document is not merely verifiable, it is **recomputable** —
sign the same readings with the same keys and the same 713 bytes come out,
on any implementation. The test does exactly that before it checks anything
else.

## 1. The three layers

| Layer | Who | Mechanism | Says |
|-------|-----|-----------|------|
| 1 | the meter | `COSE_Sign1` per reading | "I measured this" |
| 2 | the charging station | `COSE_Sign1` over a payload of its own | "these readings belong to this transaction at this point" |
| 3 | the operator | countersignature (RFC 9338) | "I saw the station's signature" |

Layers 1 and 2 are **nested**: the station has something of its own to say, so
it signs a new payload that contains the signed readings. Layer 3 is a
**countersignature**: the operator asserts nothing new about the data, so it
endorses the station's signature instead of wrapping it. That distinction
matters for the customer: the station's signature stays verifiable on its own,
and every reading keeps the meter's signature no matter how many parties
handled it afterwards.

## 2. The meter reading

The payload, in CBOR diagnostic notation:

```
{"meter": "1ISA0000000042", "transaction": "a4f1c9e2",
 "context": "Transaction.Begin", "time": 0("2026-08-15T08:14:00Z"),
 "energy": 44252([4([-3, 1234567]), 2, 3,
                  {1: 4([-1, 123]), 2: 2, 3: 4([-2, 95]), 4: 1}])}
```

134 bytes, of which the reading itself is 31:

```
A5656D657465726E31495341303030303030303034326B7472616E73616374696F6E6861
3466316339653267636F6E74657874715472616E73616374696F6E2E426567696E647469
6D65C074323032362D30382D31355430383A31343A30305A66656E65726779D9ACDC84C4
82221A0012D6870203A401C48220187B020203C48221185F0401
```

The `energy` member is the point of this document. Read from the inside out:

| Element | Meaning |
|---------|---------|
| `4([-3, 1234567])` | **1234.567** — a decimal fraction, not a binary float. The instrument showed three decimal places and the wire says so; `1234.567` and `1234.5670` stay distinguishable. |
| `2` | the unit: watt hour |
| `3` | the SI prefix: kilo. It scales the quantity, it is not folded into the value — `1234.567 kWh` never silently becomes `1234567 Wh`. |
| `{1: …, 2: 2, 3: …, 4: 1}` | the measurement uncertainty per [GUM](https://www.bipm.org/en/committees/jc/jcgm/publications): magnitude **12.3**, coverage factor **k = 2**, coverage probability **0.95**, distribution **normal**. The standard uncertainty follows as *u* = *U*/*k* = 6.15 kWh. |

So the whole statement — value, scale, unit, prefix and a complete GUM
uncertainty — is 31 bytes, and a generic CBOR decoder that has never heard of
tag 44252 still sees a well-formed tagged array of integers and standard
decimal fractions.

Signed by the meter with **ESB256** (ECDSA on brainpoolP256r1 with SHA-256),
the reading becomes 221 bytes:

```
D28445A101390108A10448C6738177A6E6D04B5886A5656D657465726E31495341303030
303030303034326B7472616E73616374696F6E68613466316339653267636F6E74657874
715472616E73616374696F6E2E426567696E6474696D65C074323032362D30382D313554
30383A31343A30305A66656E65726779D9ACDC84C482221A0012D6870203A401C4822018
7B020203C48221185F040158406A40B66B6D228217D87F6751D1919BA82CCA959F079EFC
98F805BAE4CBC340A3611ABAC58B3AA2E1FB51EA85CACB978C03DCF78F407039DA41A2E6
53A60E1389
```

- `D2` — CBOR tag 18, a `COSE_Sign1`
- `45 A101390108` — the protected bucket: `{1: -265}`, the algorithm ESB256,
  covered by the signature
- `A1 04 48 C6738177A6E6D04B` — the unprotected bucket: the key identifier,
  the leading 8 bytes of the meter key's [RFC 9679](https://www.rfc-editor.org/rfc/rfc9679)
  thumbprint, which anyone holding the meter's public key can recompute
- `5886 …` — the payload above
- `5840 …` — the signature: *r* ‖ *s*, 32 bytes each

The second reading, `Transaction.End` at 1259.869 kWh, is 219 bytes of the
same shape. The billed quantity is the difference of two independently signed
readings: **25.302 kWh**.

## 3. The charging station's bundle

The station has its own statement to make — which readings belong to which
transaction at which point — so it signs a payload of its own:

```
{"chargingStation": "DE*GEF*E12345678*1", "transaction": "a4f1c9e2",
 "readings": <2 signed readings>}
```

The readings go in as byte strings, complete with the meter's signatures. That
payload is 511 bytes; signed with **ES256** on P-256 the message is 713.

## 4. The operator's countersignature

The operator adds nothing to the data. It vouches for the station's signature,
and does so with a countersignature in the *unprotected* bucket — which is why
the station's message keeps its bytes and stays verifiable without knowing that
anyone endorsed it:

```
A2                        the unprotected bucket, two parameters
  04 48 4F4E…3440         the station's key identifier
  0B                      11: countersignature (RFC 9338)
    83                      a COSE_Countersignature
      44 A1013822           protected: {1: -35}, ES384
      A1 04 48 6B1F…88BB    the operator's key identifier
      5860 …                the signature, 48 bytes of r and s each
```

The version 2 structure of RFC 9338 is what is used here, and the difference is
not cosmetic. Its predecessor covered the payload but **not** the signature it
was countersigning, so it never actually attested to having seen it. Version 2
appends that signature to the signed structure — replacing the station's
signature, even with another valid signature over the same payload, therefore
invalidates the endorsement.

## 5. Verifying it

```csharp
var record   = COSESign1.Parse(bytes);

// The operator vouched for the station's signature...
record.VerifyCountersignature(record.Countersignatures[0], operatorKey, out var e1);

// ...the station signed the bundle...
record.Verify(stationKey, out var e2);

// ...and the meter signed every reading within it.
foreach (var reading in CBORValue.Parse(record.Payload!)["readings"].AsArray())
    COSESign1.Parse(reading.AsBytes()).Verify(meterKey, out var e3);
```

Each layer answers a different question, and a failure at one does not
invalidate the answers of the others. A single altered digit in a reading is
caught by the meter's signature even if the station's and the operator's
signatures were recomputed around it.

### Recomputing it

```csharp
COSESign1.Sign(payload, meterKey, COSEAlgorithm.ESB256, meterKeyId, Deterministic: true);
```

`Deterministic` derives the ECDSA nonce from the private key and the message
as RFC 6979 defines, so the signature stops being a random value that merely
verifies and becomes a function of what it signs. Rebuilding this record from
the keys of Section 7 therefore reproduces these exact 713 bytes.

That is worth having beyond documentation. A meter or a smart card has no
dependable source of randomness, and an ECDSA nonce that repeats hands over
the private key — a determinism that removes the requirement removes the
failure mode with it.

## 6. The complete record

713 bytes:

```
D28443A10126A204484F4E4267CBA434400B8344A1013822A104486B1F337BA0EC88BB58
6056AA831918D6215BFE6ABAA02791C8FB619E0C2661F55E8C1F95967A67A02863E1ACC9
EB090F4A2DD5BE6134380A29D65BA71661A2BA7D337C84C4E4C2C2D87F8925618D0CC7EF
3E1EBD6D4279B55514A156B4E5315237488B681C20118283175901FFA36F636861726769
6E6753746174696F6E7244452A4745462A4531323334353637382A316B7472616E736163
74696F6E6861346631633965326872656164696E67738258DDD28445A101390108A10448
C6738177A6E6D04B5886A5656D657465726E31495341303030303030303034326B747261
6E73616374696F6E68613466316339653267636F6E74657874715472616E73616374696F
6E2E426567696E6474696D65C074323032362D30382D31355430383A31343A30305A6665
6E65726779D9ACDC84C482221A0012D6870203A401C48220187B020203C48221185F0401
58406A40B66B6D228217D87F6751D1919BA82CCA959F079EFC98F805BAE4CBC340A3611A
BAC58B3AA2E1FB51EA85CACB978C03DCF78F407039DA41A2E653A60E138958DBD28445A1
01390108A10448C6738177A6E6D04B5884A5656D657465726E3149534130303030303030
3034326B7472616E73616374696F6E68613466316339653267636F6E746578746F547261
6E73616374696F6E2E456E646474696D65C074323032362D30382D31355430393A30323A
30305A66656E65726779D9ACDC84C482221A0013395D0203A401C48220187E020203C482
21185F040158401D92018570E22306441FDD0E1645124C03F63CDE0D75A154B7ECD78411
2020F25834508FD5D9A6A016025A85B8BD7F5DF27056B33EDFC7A823E55449061562CC58
40C521E083F44F35D056F5B6F75893B7B2AD8E32CFB2F60DFEAA405466083C16267C6E92
56110BDBD204D81878E195A9E4BE644FE034BC7A640A42F82CC931AA2E
```

| | bytes |
|---|---:|
| one meter reading, unsigned | 134 |
| one meter reading, signed | 221 |
| both readings plus the station's metadata | 511 |
| the station's signed bundle | 713 |
| the operator's countersignature, within those 713 | 96 of signature |

Three signatures, two complete metrological statements with their
uncertainties, and the identities of all three signers, in 713 bytes.

## 7. The keys

Example keys. They were generated for this document, they secure nothing, and
they must never appear anywhere else.

| | curve | algorithm | key identifier |
|---|---|---|---|
| meter | brainpoolP256r1 | ESB256 (−265) | `C6738177A6E6D04B` |
| charging station | P-256 | ES256 (−7) | `4F4E4267CBA43440` |
| operator | P-384 | ES384 (−35) | `6B1F337BA0EC88BB` |

```
meter    x  A734FB1962C381113C746BDDBCBC774801E3B73FA7F73479615D290E91E48889
         y  8A188C8261A560197B37C73044E3009BA1DAED226C324A35FEE76AA144740678
         d  08F001BB03BEF4FBD1C59F10B50555CD37D2B53421331DBFA98815A581326FB3

station  x  7951E32509303CD4DB14127765B3FC9F32F62AC5C0F12350BD3ED7C746C72FE9
         y  A35716031E2C44A942D886626C5D4C41E0FF62E44FED7EDA3ACC1408D90720DC
         d  875E51ECF18073E8B970E6DCC5A115433456E13DF966034A5A782945D2B684D3

operator x  5DEF24F33251A911F43205134D568C1FB3547E2BD0B602D4B18A5FA476FF1FB8
            E6D321CC4ED1DCF754A81159C63389D2
         y  D8298F873104BC9AE145888BB7DC574AB26501E1E78DC4613CCB4B4C1B842720
            724671655551F9E2918C8943EAE8C2FA
         d  6952487A0A16EACE6E9A69EFD062D7671D68D23FF68722326348827C3A94E2A1
            743A1DF8901B948412CCA26CA4372CED
```

Every key identifier above is the leading 8 bytes of the RFC 9679 thumbprint of
its own key, so a verifier can check that the key it was handed is the key the
record names. Because the thumbprint covers the curve, a signer who changes
algorithm necessarily gets a different identifier — an algorithm downgrade
under an unchanged identity is not expressible.

## 8. What this is and is not

This is a proposal for how a digital, signed SI quantity can look on the wire,
and a demonstration that the pieces fit: the metrological content of
[tag 44252](tag-44252.md) — value, scale, unit, prefix, GUM uncertainty —
carried through three independent signature layers without losing a decimal
place or a coverage factor.

It is not a conformity statement. In the regulated part of the charging
infrastructure that follows from the type approval of the measuring instrument
and from the data being checkable with the verification software the approval
covers. What this format offers is the part underneath: a representation whose
encoding is a pure function of the measured quantity, so that the same reading
always produces the same bytes and therefore the same signature.

## References

- [tag-44252.md](tag-44252.md) — the metrological value extension
- [RFC 6979](https://www.rfc-editor.org/rfc/rfc6979) — deterministic ECDSA
- [RFC 8949](https://www.rfc-editor.org/rfc/rfc8949) — CBOR
- [RFC 9052](https://www.rfc-editor.org/rfc/rfc9052) / [RFC 9053](https://www.rfc-editor.org/rfc/rfc9053) — COSE
- [RFC 9338](https://www.rfc-editor.org/rfc/rfc9338) — COSE countersignatures
- [RFC 9679](https://www.rfc-editor.org/rfc/rfc9679) — COSE Key Thumbprint
- [RFC 9864](https://www.rfc-editor.org/rfc/rfc9864) — fully-specified algorithms, incl. the brainpool curves
- JCGM 100:2008 — Guide to the expression of uncertainty in measurement
