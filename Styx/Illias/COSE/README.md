# Vanaheimr Styx COSE

**CBOR Object Signing and Encryption** ([RFC 9052](https://www.rfc-editor.org/rfc/rfc9052))
on top of the [Styx CBOR implementation](../CBOR/README.md), for signing
measurement data, metering records and any other CBOR payload.

Signing is what turns the [metrological value extension](../CBOR/tag-44252.md)
into something a third party can check: the encoding of a reading is a pure
function of its value, scale, unit, prefix and uncertainty, so the same reading
always produces the same bytes — and therefore the same signature.

## What is implemented

- **`COSESign1`** — a payload signed by a single signer (CBOR tag 18):
  sign, verify, detached payloads, external additional authenticated data,
  and the `crit` header parameter.
- **`COSEKey`** — COSE keys of key type `EC2` (RFC 9052 §7), with conversions
  to and from the Bouncy Castle elliptic curve keys used elsewhere in Styx
  ([`Crypto.cs`](../Crypto/Crypto.cs)).
- **`COSEAlgorithm` / `COSECurve`** — the IANA registries, including the
  fully-specified algorithms of [RFC 9864](https://www.rfc-editor.org/rfc/rfc9864)
  and the brainpool curves registered by ISO/IEC 18013-5.

| Algorithm | Id | Curve | Digest |
|-----------|---:|-------|--------|
| `ES256` / `ES384` / `ES512` | −7 / −35 / −36 | any (deprecated by RFC 9864) | SHA-256 / 384 / 512 |
| `ESP256` / `ESP384` / `ESP512` | −9 / −51 / −52 | P-256 / P-384 / P-521 | SHA-256 / 384 / 512 |
| `ESB256` / `ESB320` / `ESB384` / `ESB512` | −265 / −266 / −267 / −268 | brainpoolP256r1 / P320r1 / P384r1 / P512r1 | SHA-256 / 384 / 384 / 512 |
| `ES256K` | −47 | secp256k1 | SHA-256 |

Not implemented: `COSE_Sign` (multiple signers, tag 98), counter signatures
(RFC 9338), EdDSA, MAC and encryption. The CBOR tags of those structures are
defined in `CBORTag`, so they are recognized rather than silently misread.

## Signing and verifying

```csharp
var signed    = COSESign1.Sign(payload, privateKey, COSEAlgorithm.ES256, keyIdentifier);
var bytes     = signed.ToByteArray();

var message   = COSESign1.Parse(bytes);
var verified  = message.Verify(publicKey, out var errorResponse);
```

A failed verification is not an exception — it is the expected outcome of
checking untrusted data, and `errorResponse` says why.

Whenever the signing key does not live in this process — a meter, a smart card,
a hardware security module — `ToBeSigned()` hands out exactly the byte string
that has to be signed:

```csharp
var toBeSigned = COSESign1.ToBeSigned(protectedHeaderBytes, payload);
```

## Five things that are easy to get wrong

1. **The signature never covers the message.** It covers the `Sig_structure`
   `["Signature1", protected, external_aad, payload]` (RFC 9052 §4.4).
   The CBOR tag is therefore *not* signed: the same message with and without
   tag 18 carries the very same signature bytes.
2. **The protected bucket is kept verbatim.** A re-serialization that differs
   in a single byte — a non-preferred integer head, a different map order —
   invalidates every signature made over the original bytes. `COSESign1` never
   re-encodes it.
3. **An empty protected bucket is `h''`, not `h'A0'`.** A zero-length byte
   string, not an encoded empty map. `COSEHeaders.ToProtectedByteArray()`
   returns the empty array for it, and the parser never "repairs" it.
4. **ECDSA signatures are `r ‖ s`, not DER.** Each component is zero-padded to
   the width of the group order (RFC 9053 §2.1) — 64 bytes on P-256, 132 on
   P-521. Bouncy Castle and the .NET signers produce DER by default; handing a
   DER signature to `Verify` produces an error message that says so.
5. **Only what is protected can be trusted.** The key identifier of nearly
   every deployed message lives in the *unprotected* bucket. `Verify` refuses
   to silently trust an algorithm stated there: pass `ExpectedAlgorithm` to
   accept it deliberately.

Key material has its own version of the same trap: coordinates and private
keys are fixed-width byte strings whose **leading zeroes must be preserved**
(RFC 9053 §7.1.1). A plain big-integer serialization shortens them, and the
resulting keys are rejected elsewhere. `COSEKey` always pads, and verifies the
width when reading.

## Golden vectors

The tests in [`StyxTests/Illias/COSE/`](../../../StyxTests/Illias/COSE) are
pinned against the ECDSA examples of RFC 9052 Appendix C.2.1 and the
`sign1-tests` of the [COSE working group example repository](https://github.com/cose-wg/Examples),
taken from their machine-readable form rather than retyped.

ECDSA is randomized, so published signature bytes cannot be reproduced by
signing — but they can be *verified*, which is the stronger statement: a single
wrong byte anywhere in the `Sig_structure`, the header buckets or the key would
make the verification fail.

## A note on German calibration law

COSE does not make a data format legally usable in the regulated part of the
charging infrastructure. That follows from the type approval of the measuring
instrument and from the data being checkable with the verification software the
conformity assessment covers — in practice OCMF or a signed meter format today,
not a free-form CBOR structure.

What this implementation is for is the integrity of measurement data along the
rest of the chain, and for the conversation about what a digital, signed SI
quantity should look like.

## References

- [RFC 9052](https://www.rfc-editor.org/rfc/rfc9052) — COSE: Structures and Process
- [RFC 9053](https://www.rfc-editor.org/rfc/rfc9053) — COSE: Initial Algorithms
- [RFC 9864](https://www.rfc-editor.org/rfc/rfc9864) — Fully-Specified Algorithms for JOSE and COSE
- [RFC 9360](https://www.rfc-editor.org/rfc/rfc9360) — COSE: Header Parameters for X.509 Certificates
- [IANA COSE registries](https://www.iana.org/assignments/cose/cose.xhtml)
