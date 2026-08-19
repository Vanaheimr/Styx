# Vanaheimr Styx COSE

**CBOR Object Signing and Encryption** ([RFC 9052](https://www.rfc-editor.org/rfc/rfc9052))
on top of the [Styx CBOR implementation](../CBOR/README.md), for signing
measurement data, metering records and any other CBOR payload.

Signing is what turns the [metrological value extension](https://github.com/OpenChargingTechnology/Whitepapers/blob/master/MetrologicalCBOR/README.md)
into something a third party can check: the encoding of a reading is a pure
function of its value, scale, unit, prefix and uncertainty, so the same reading
always produces the same bytes — and therefore the same signature.

[**A signed metrological record, end to end**](https://github.com/OpenChargingTechnology/Whitepapers/blob/master/MetrologicalCBOR/tag-44252-signed-example.md)
puts all of the below to work on one charging transaction: two meter readings
with their GUM uncertainties, signed by the meter, bundled by the charging
station, countersigned by the operator — 713 bytes, every one of them verified
by a test.

## What is implemented

- **`COSESign1`** — a payload signed by a single signer (CBOR tag 18):
  sign, verify, detached payloads, external additional authenticated data,
  and the `crit` header parameter.
- **`COSESign`** / **`COSESignature`** — one payload, several signers
  (CBOR tag 98). Each signature carries its own header buckets, so every
  party signs with its own algorithm and its own key.
- **`COSEKey`** — COSE keys of key type `EC2` (RFC 9052 §7), `OKP` and `AKP`
  (RFC 9964), with conversions to and from the Bouncy Castle keys used
  elsewhere in Styx ([`Crypto.cs`](../Crypto/Crypto.cs)), and COSE Key
  Thumbprints (RFC 9679).
- **`COSEAlgorithm` / `COSECurve`** — the IANA registries, including the
  fully-specified algorithms of [RFC 9864](https://www.rfc-editor.org/rfc/rfc9864)
  and the brainpool curves registered by ISO/IEC 18013-5.

| Algorithm | Id | Curve | Digest |
|-----------|---:|-------|--------|
| `ES256` / `ES384` / `ES512` | −7 / −35 / −36 | any (deprecated by RFC 9864) | SHA-256 / 384 / 512 |
| `ESP256` / `ESP384` / `ESP512` | −9 / −51 / −52 | P-256 / P-384 / P-521 | SHA-256 / 384 / 512 |
| `ESB256` / `ESB320` / `ESB384` / `ESB512` | −265 / −266 / −267 / −268 | brainpoolP256r1 / P320r1 / P384r1 / P512r1 | SHA-256 / 384 / 384 / 512 |
| `ES256K` | −47 | secp256k1 | SHA-256 |
| `Ed25519` / `Ed448` | −19 / −53 | Ed25519 / Ed448 | *(none — pure)* |
| `ML-DSA-44` / `-65` / `-87` | −48 / −49 / −50 | *(none — an algorithm key pair)* | *(none — pure)* |

- **Countersignatures** ([RFC 9338](https://www.rfc-editor.org/rfc/rfc9338),
  header parameter 11) on a `COSE_Sign1` — a signature *of a signature*.
- **X.509 certificate chains** ([RFC 9360](https://www.rfc-editor.org/rfc/rfc9360)) —
  `x5chain` and `x5t`, validated against configured trust anchors.

### The two pure families

EdDSA ([RFC 8032](https://www.rfc-editor.org/rfc/rfc8032)) and ML-DSA
([FIPS 204](https://doi.org/10.6028/NIST.FIPS.204), registered for COSE by
[RFC 9964](https://www.rfc-editor.org/rfc/rfc9964)) sign the `Sig_structure`
**itself** rather than a digest of it. Handing a pure signer a hash yields a
signature that is valid for that hash and that nobody else will ever accept, so
the family is a property of the algorithm here rather than an afterthought.

Their key types are not merely two more shapes either. An EdDSA public key is
the whole of `x` and has no `y`; an ML-DSA key belongs to an algorithm rather
than to a curve, and there the labels shift underfoot — `−1` is the public key
and `−2` the private one, where an EC2 or OKP key has the curve and the x
coordinate. `COSEKey` therefore establishes the key type before it reads
anything else, because a parser that switched on the label alone would read a
1312-byte ML-DSA public key as a curve identifier and report nothing wrong at
all. Its `priv` is the **32-byte seed** rather than the expanded secret key —
which keeps a private ML-DSA-87 key at 32 bytes instead of 4896 — and its
thumbprint covers `alg`, unlike every other key type, because an ML-DSA public
key does not say which parameter set produced it.

And the size is the point rather than the drawback. An ML-DSA-87 signature is
4627 bytes over a metrological reading of about thirty, which is precisely the
case where carrying it in CBOR stops being a nicety: a byte string costs its
bytes and a three-byte head, while base64 within JSON would add a further third
to the largest field in the message.

Not implemented: `COSE_Countersignature0` (the abbreviated form, header
parameter 12), MAC and encryption. The CBOR tags of those structures
are defined in `CBORTag`, so they are recognized rather than silently
misread.

## Signing and verifying

```csharp
var signed    = COSESign1.Sign(payload, privateKey, COSEAlgorithm.ES256, keyIdentifier);
var bytes     = signed.ToByteArray();

var message   = COSESign1.Parse(bytes);
var verified  = message.Verify(publicKey, out var errorResponse);
```

A failed verification is not an exception — it is the expected outcome of
checking untrusted data, and `errorResponse` says why.

Passing `Deterministic: true` derives the ECDSA nonce from the private key and
the message ([RFC 6979](https://www.rfc-editor.org/rfc/rfc6979)) instead of
drawing it at random. Signing the same data then yields the same bytes every
time — which makes published examples recomputable, and which matters rather
more for a device that has no dependable source of randomness, since a
repeated nonce hands over the private key. The RFC's own P-256 vectors are
reproduced by a test.

The same flag means the corresponding thing to the other two families.
**EdDSA ignores it, because it has nothing to draw**: RFC 8032 derives the
nonce from the key and the message and offers no alternative, which is why its
published vectors are reproduced rather than merely verified. **ML-DSA takes
the deterministic variant of FIPS 204**, where the per-signature randomness is
32 zero bytes instead of drawn; RFC 9964 declines to choose between the two,
and the choice is what decides whether two implementations can be compared byte
for byte or only asked whether each accepts the other's signature.

Whenever the signing key does not live in this process — a meter, a smart card,
a hardware security module — `ToBeSigned()` hands out exactly the byte string
that has to be signed:

```csharp
var toBeSigned = COSESign1.ToBeSigned(protectedHeaderBytes, payload);
```

## The compact form: only the sender travels

For a protocol that already agrees on its algorithm — every request and
response of one profile — the algorithm does not belong in the message. It is
a property of the key, and the key is found from the key identifier:

```csharp
var signed    = COSESign1.SignWithApplicationAlgorithm(body, privateKey, COSEAlgorithm.ES256, senderId);

var received  = COSESign1.Parse(bytes);
var senderKey = keyStore[received.KeyIdentifier];       // carries its own alg
received.Verify(senderKey, out var errorResponse);
```

That leaves the protected bucket empty, and the whole framing costs
**9 bytes plus the key identifier** — 17 bytes with an 8-byte one — on top of
the payload and the signature:

```
D2                    tag 18                       1  (droppable when the context says so)
84                    array(4)                     1
40                    protected = h''              1
A1 04 48 <8 bytes>    {4: kid}                    11
54 <20 bytes>         payload                      1 + payload
58 40 <64 bytes>      signature                    2 + 64
```

101 bytes for a 20-byte payload, pinned by a test. Naming the algorithm in the
protected bucket costs 3 more; dropping the tag saves 1.

Leaving it out is not only shorter but **safer than the obvious alternative**:
an algorithm in the *unprotected* bucket can be changed by anyone along the
way, while one that is not in the message at all cannot. The verifier uses the
algorithm it has configured, and refuses to guess when it has none. The price
is that agility becomes a property of the profile rather than of the message.

### Key identifiers that anyone can recompute

`kid` is an opaque byte string, so any sender identifier fits. There is a
standard one: the **COSE Key Thumbprint** ([RFC 9679](https://www.rfc-editor.org/rfc/rfc9679)),
a hash over exactly the required key parameters in their deterministic
encoding.

```csharp
var provisioned = COSEKey.From(publicKey, null, COSEAlgorithm.ES256).
                          WithThumbprintKeyIdentifier();   // 8 leading bytes
```

Two properties make it worth preferring over a self-chosen prefix. Everyone
holding the public key can recompute the identifier, so no registry and no
agreement beyond its length is needed. And because the thumbprint covers the
curve, a signer who changes algorithm necessarily has a different key and thus
a different identifier — an algorithm downgrade under an unchanged identity is
not expressible. Optional parameters are excluded from the computation, so the
public and the private half of one key pair share one identity.

## Several signers

`COSE_Sign` is for the case where more than one party signs one and the same
payload — the meter signs the reading, the backend counter-signs it later:

```csharp
var signed         = COSESign.Sign(payload, meterKey, COSEAlgorithm.ES256, meterKeyId);
var counterSigned  = signed.AddSignature(backendKey, COSEAlgorithm.ES384, backendKeyId);

counterSigned.Verify(counterSigned.Signatures[0], meterPublicKey,   out var error);
counterSigned.TryVerifyAny(backendPublicKey, out var signature,     out var reason);
```

The signatures are independent: each covers the body, its own header bucket
and the payload, but never another signature. Adding one therefore leaves the
existing ones byte-for-byte valid, and the second party never needs the first
party's key. A signature *over* another signature is a different mechanism —
the countersignature of RFC 9338, see below.

Its `Sig_structure` has **five** elements rather than four, with the protected
bucket of the individual signature between the body and the external data:
`["Signature", body_protected, sign_protected, external_aad, payload]`.

## Where the key came from

Verifying with a public key answers *“was this signed with that key”*. It does
not answer *“why should I believe that key belongs to that meter”* — and in a
regulated setting the second question is the one that matters. A certificate
chain answers it, if it is checked properly:

```csharp
message.VerifyWithCertificateChain(trustAnchors, out var signer, out var errorResponse);
// signer is the end-entity certificate: CN=Meter 1ISA0000000042
```

**The two halves are never checked apart.** A chain that validates beautifully
says nothing about the message it arrived with unless the key of its
end-entity certificate is the key that verified the signature. Presenting
somebody else's genuine certificate while signing with your own key is the
obvious attack, and it fails here on the signature, not on the chain — there
is a test that does exactly that.

What is validated: every certificate signed by the next, the last one anchored
in a trust anchor the caller supplies, validity periods (at a chosen point in
time, so an old record can be checked as of when it was made), the CA flag and
`keyCertSign` on every issuer, `digitalSignature` on the end entity, and the
`x5t` thumbprint against the certificate that actually travelled.

What is **not**: revocation, name constraints, policies, path length limits.
And the answer stops at *“this key belongs to that subject, attested by
someone I trust”* — whether that subject may state what it states is the
caller's question.

A single certificate travels as a bare byte string, two or more as an array,
end-entity first; the trust anchor need not be included.

### `crit` and honesty about what was checked

`COSEHeaderLabel.IsUnderstood` deliberately does *not* list `x5chain`, because
whether it is understood depends on what the caller is doing:
`Verify(publicKey, …)` has not looked at any certificate and must not claim
otherwise, so a message whose sender marked `x5chain` critical is **refused**
there and **accepted** by `VerifyWithCertificateChain`. Same message, different
answer, and the difference is truthful.

## Countersignatures: signing someone else's signature

A countersignature (RFC 9338) is not another signature of the payload — it is
a signature **of a signature**. It lives in the *unprotected* bucket, so it can
be added to a finished message without changing a byte of what was signed:

```csharp
var released = signedByStation.AddCountersignature(operatorKey, COSEAlgorithm.ES384, operatorKeyId);

released.Verify(stationPublicKey, out var bodyError);                    // still valid, same bytes
released.VerifyCountersignature(released.Countersignatures[0],
                                operatorPublicKey, out var counterError);
```

**Which of the two to reach for** is decided by one question: does the party
have something of its own to say?

| Situation | Mechanism |
|-----------|-----------|
| A charging station bundles signed meter readings and adds its own metadata | it signs a **new payload** — nest a `COSE_Sign1` whose payload contains the readings |
| An operator vouches for the station's signature before the data goes to the customer | it says nothing new — **countersign**, so the station's signature stays independently verifiable |
| Several parties each assert the same payload | one `COSE_Sign` with several signatures |

The version 2 structure of RFC 9338 is the one implemented here, and the
difference is not cosmetic: the countersignature of RFC 8152 covered the
payload but **not** the signature it was countersigning, so it never actually
attested to having seen it. Version 2 appends that signature as
`other_fields`, which is why replacing the body signature — even with another
valid one over the same payload — invalidates the countersignature. There is a
test that does exactly that.

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
pinned against the ECDSA examples of RFC 9052 — Appendix C.2.1 for
`COSE_Sign1`, C.1.1 and C.1.2 for `COSE_Sign`, the latter with one signature
on P-256 and one on P-521 — and the `sign1-tests` of the
[COSE working group example repository](https://github.com/cose-wg/Examples),
taken from their machine-readable form rather than retyped. The
countersignature vector is the worked example of RFC 9338 Appendix A.2.1,
which the RFC prints in diagnostic notation only: the message is assembled
from the documented parts, and that both its body signature and its
countersignature then verify against the published keys is what proves the
assembly and the transcription.

**EdDSA** is pinned against RFC 8032 — Section 7.1 for Ed25519, Section 7.4 for
Ed448 — and those are checked harder than any ECDSA vector allows: EdDSA has no
nonce to draw, so its published signatures are not merely verifiable but
*recomputable*, and the tests reproduce them byte for byte.

ECDSA is randomized, so published signature bytes generally cannot be
reproduced by signing — but they can be *verified*, which is the stronger
statement: a single wrong byte anywhere in the `Sig_structure`, the header
buckets or the key would make the verification fail. Where the signer used
RFC 6979, reproduction is available too, and the tests take it.

**ML-DSA** has no small published vector to paste in, so it is pinned by the
properties RFC 9964 fixes — the key and signature sizes, the seed-derived key
pair, the label-shifting AKP parameters — and then, decisively, by the
[conformance suite](https://github.com/Vanaheimr/MCBORConformanceTests), which
signs every case with this implementation *and* with the TypeScript one and
compares the bytes. That is also where the open question of RFC 9964 was
settled by measurement: Bouncy Castle's deterministic ML-DSA and
`@noble/post-quantum`'s `extraEntropy: false` produce identical signatures.

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
- [RFC 6979](https://www.rfc-editor.org/rfc/rfc6979) — Deterministic ECDSA
- [RFC 8032](https://www.rfc-editor.org/rfc/rfc8032) — EdDSA: Ed25519 and Ed448
- [RFC 9964](https://www.rfc-editor.org/rfc/rfc9964) — ML-DSA for JOSE and COSE
- [FIPS 204](https://doi.org/10.6028/NIST.FIPS.204) — Module-Lattice-Based Digital Signature Standard
- [RFC 9338](https://www.rfc-editor.org/rfc/rfc9338) — COSE: Countersignatures
- [RFC 9360](https://www.rfc-editor.org/rfc/rfc9360) — COSE: Header Parameters for X.509 Certificates
- [RFC 9679](https://www.rfc-editor.org/rfc/rfc9679) — COSE Key Thumbprint
- [IANA COSE registries](https://www.iana.org/assignments/cose/cose.xhtml)
