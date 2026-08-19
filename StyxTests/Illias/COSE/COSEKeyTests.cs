/*
 * Copyright (c) 2010-2026 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of Styx <https://www.github.com/Vanaheimr/Styx>
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

#region Usings

using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Crypto.Parameters;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias.Tests
{

    /// <summary>
    /// COSE keys [RFC 9052, Section 7] and their bridge to the elliptic
    /// curve keys of Bouncy Castle.
    ///
    /// The example key is the ECDSA P-256 key of RFC 9052, Appendix C.7,
    /// base64url encoded exactly as within the example repository of the
    /// COSE working group (https://github.com/cose-wg/Examples).
    /// </summary>
    [TestFixture]
    public class COSEKeyTests
    {

        #region Data

        private const String  exampleKeyX     = "usWxHK2PmfnHKwXPS54m0kTcGJ90UiglWiGahtagnv8";
        private const String  exampleKeyY     = "IBOL-C3BttVivg-lSreASjpkttcsz-1rb7btKLv8EX4";
        private const String  exampleKeyD     = "V8kgd2ZBRuh2dgyVINBUqpPDr7BOMGcF22CQMIUHtNM";

        private static readonly Byte[]  keyIdentifier   = "11".ToUTF8Bytes();

        #endregion

        #region (private static) FromBase64URL(Text)

        private static Byte[] FromBase64URL(String Text)

            => Convert.FromBase64String(
                   Text.Replace('-', '+').
                        Replace('_', '/').
                        PadRight(Text.Length + (4 - Text.Length % 4) % 4, '=')
               );

        #endregion

        #region (private static) ExampleKey()

        private static COSEKey ExampleKey()

            => new (COSEKeyType.EC2,
                    COSECurve.P256,
                    FromBase64URL(exampleKeyX),
                    FromBase64URL(exampleKeyY),
                    FromBase64URL(exampleKeyD),
                    keyIdentifier,
                    COSEAlgorithm.ES256);

        #endregion


        #region The_published_example_key_bridges_to_BouncyCastle_and_back()

        [Test]
        public void The_published_example_key_bridges_to_BouncyCastle_and_back()
        {

            var key         = ExampleKey();

            Assert.That(key.IsPrivate,   Is.True);
            Assert.That(key.X!.Length,   Is.EqualTo(32));
            Assert.That(key.Y!.Length,   Is.EqualTo(32));
            Assert.That(key.D!.Length,   Is.EqualTo(32));

            var publicKey   = (ECPublicKeyParameters)  key.ToPublicKey();
            var privateKey  = (ECPrivateKeyParameters) key.ToPrivateKey();

            // The public point of the published private key is the published
            // public point - which only holds when every one of those 96 key
            // bytes was read correctly.
            var derived     = Crypto.CalculatePublicKey(privateKey);

            Assert.That(derived.Q.Normalize(),  Is.EqualTo(publicKey.Q.Normalize()));

            // ...and back again.
            Assert.That(COSEKey.From(publicKey).X,   Is.EqualTo(key.X));
            Assert.That(COSEKey.From(publicKey).Y,   Is.EqualTo(key.Y));
            Assert.That(COSEKey.From(publicKey).D,   Is.Null);

            Assert.That(COSEKey.From(privateKey).X,  Is.EqualTo(key.X));
            Assert.That(COSEKey.From(privateKey).Y,  Is.EqualTo(key.Y));
            Assert.That(COSEKey.From(privateKey).D,  Is.EqualTo(key.D));

            Assert.That(COSEKey.From(publicKey).Curve,   Is.EqualTo(COSECurve.P256));
            Assert.That(COSEKey.From(privateKey).Curve,  Is.EqualTo(COSECurve.P256));

        }

        #endregion

        #region A_key_roundtrips_through_CBOR()

        [Test]
        public void A_key_roundtrips_through_CBOR()
        {

            var key       = new COSEKey(
                                COSEKeyType.EC2,
                                COSECurve.P256,
                                FromBase64URL(exampleKeyX),
                                FromBase64URL(exampleKeyY),
                                FromBase64URL(exampleKeyD),
                                keyIdentifier,
                                COSEAlgorithm.ES256,
                                [CBORValue.FromInt64(1), CBORValue.FromInt64(2)],
                                [new (CBORValue.FromText("vendor"), CBORValue.FromText("GraphDefined"))]
                            );

            var reparsed  = COSEKey.Parse(key.ToByteArray());

            Assert.That(reparsed.KeyType,                        Is.EqualTo(COSEKeyType.EC2));
            Assert.That(reparsed.Curve,                          Is.EqualTo(COSECurve.P256));
            Assert.That(reparsed.X,                              Is.EqualTo(key.X));
            Assert.That(reparsed.Y,                              Is.EqualTo(key.Y));
            Assert.That(reparsed.D,                              Is.EqualTo(key.D));
            Assert.That(reparsed.KeyIdentifier,                  Is.EqualTo(keyIdentifier));
            Assert.That(reparsed.Algorithm,                      Is.EqualTo(COSEAlgorithm.ES256));
            Assert.That(reparsed.KeyOperations,                  Is.EqualTo(key.KeyOperations));

            // Unknown key parameters survive the roundtrip...
            Assert.That(reparsed.AdditionalParameters.Count,     Is.EqualTo(1));
            Assert.That(reparsed.AdditionalParameters[0].Value.AsText(),  Is.EqualTo("GraphDefined"));

            Assert.That(Convert.ToHexString(reparsed.ToByteArray()),
                        Is.EqualTo(Convert.ToHexString(key.ToByteArray())));

            // ...and the private key material can be stripped.
            var publicOnly = key.ToPublicCOSEKey();

            Assert.That(publicOnly.IsPrivate,  Is.False);
            Assert.That(publicOnly.D,          Is.Null);
            Assert.That(publicOnly.X,          Is.EqualTo(key.X));

        }

        #endregion

        #region Coordinates_and_private_keys_keep_their_leading_zeroes()

        [Test]
        public void Coordinates_and_private_keys_keep_their_leading_zeroes()
        {

            // The private key 1 is 31 zero bytes followed by a single 1 - the
            // sharpest case for the padding rule of RFC 9053, Section 7.1.1,
            // which a plain big integer serialization would shorten to one
            // single byte and thereby make other implementations reject.
            var privateKey  = new ECPrivateKeyParameters(
                                  "ECDSA",
                                  BigInteger.One,
                                  COSECurve.P256.DomainParameters!
                              );

            var key         = COSEKey.From(privateKey);

            Assert.That(key.D!.Length,      Is.EqualTo(32));
            Assert.That(key.D[..31],        Is.EqualTo(new Byte[31]));
            Assert.That(key.D[31],          Is.EqualTo(1));

            // The public point of the private key 1 is the generator point.
            var generator   = COSECurve.P256.DomainParameters!.G.Normalize();

            Assert.That(key.X,              Is.EqualTo(generator.AffineXCoord.GetEncoded()));
            Assert.That(key.Y,              Is.EqualTo(generator.AffineYCoord.GetEncoded()));
            Assert.That(key.X!.Length,      Is.EqualTo(32));
            Assert.That(key.Y!.Length,      Is.EqualTo(32));

            // Whoever strips those leading zeroes produces a key that is
            // rejected rather than silently misread.
            var stripped    = new COSEKey(
                                  COSEKeyType.EC2,
                                  COSECurve.P256,
                                  key.X,
                                  key.Y,
                                  key.D.SkipWhile(static value => value == 0).ToArray()
                              );

            Assert.That(stripped.TryToPrivateKey(out _, out var errorResponse),  Is.False);
            Assert.That(errorResponse,  Does.Contain("must be 32 bytes wide, including leading zeroes, but was 1 bytes wide"));

            Assert.That(stripped.TryToPublicKey(out _, out _),  Is.True);

        }

        #endregion

        #region A_public_point_that_is_not_on_the_curve_is_rejected()

        [Test]
        public void A_public_point_that_is_not_on_the_curve_is_rejected()
        {

            var x    = FromBase64URL(exampleKeyX);

            // Using the x coordinate as the y coordinate as well yields a
            // valid pair of field elements that is not a point of the curve.
            var key  = new COSEKey(
                           COSEKeyType.EC2,
                           COSECurve.P256,
                           x,
                           x
                       );

            Assert.That(key.TryToPublicKey(out _, out var errorResponse),  Is.False);
            Assert.That(errorResponse,  Does.Contain("does not lie on the curve"));

            // A coordinate of the wrong width is rejected as well.
            Assert.That(new COSEKey(COSEKeyType.EC2, COSECurve.P256, x, [.. x, 0x00]).
                            TryToPublicKey(out _, out var widthError),  Is.False);

            Assert.That(widthError,  Does.Contain("must be 32 bytes wide"));

        }

        #endregion

        #region A_compressed_y_coordinate_is_decompressed()

        [Test]
        public void A_compressed_y_coordinate_is_decompressed()
        {

            var x         = FromBase64URL(exampleKeyX);
            var y         = FromBase64URL(exampleKeyY);

            // RFC 9053, Section 7.1.1: The y coordinate may be a boolean
            // holding the sign bit of the point instead of the full value.
            var compressed = CBORValue.FromMap([
                                 new (COSEKey.KeyTypeLabel,  CBORValue.FromInt64((Int64) COSEKeyType.EC2)),
                                 new (COSEKey.CurveLabel,    COSECurve.P256.ToCBOR()),
                                 new (COSEKey.XLabel,        CBORValue.FromBytes(x)),
                                 new (COSEKey.YLabel,        CBORValue.FromBoolean((y[^1] & 1) == 1))
                             ]);

            var key        = COSEKey.Parse(compressed);

            Assert.That(key.Y,  Is.EqualTo(y));

            Assert.That(key.TryToPublicKey(out var publicKey, out var errorResponse),  Is.True, errorResponse);
            Assert.That(((ECPublicKeyParameters) publicKey!).Q.Normalize(),
                        Is.EqualTo(((ECPublicKeyParameters) ExampleKey().ToPublicKey()).Q.Normalize()));

            // The wrong sign bit yields the other point of the curve,
            // which is a valid point but a different key.
            var flipped    = COSEKey.Parse(
                                 CBORValue.FromMap([
                                     new (COSEKey.KeyTypeLabel,  CBORValue.FromInt64((Int64) COSEKeyType.EC2)),
                                     new (COSEKey.CurveLabel,    COSECurve.P256.ToCBOR()),
                                     new (COSEKey.XLabel,        CBORValue.FromBytes(x)),
                                     new (COSEKey.YLabel,        CBORValue.FromBoolean((y[^1] & 1) != 1))
                                 ])
                             );

            Assert.That(flipped.Y,  Is.Not.EqualTo(y));
            Assert.That(flipped.TryToPublicKey(out _, out _),  Is.True);

        }

        #endregion

        #region Every_key_type_is_read_in_its_own_light()

        [Test]
        public void Every_key_type_is_read_in_its_own_light()
        {

            // A key agreement curve signs nothing, whatever key type it is
            // wrapped in.
            var x25519 = new COSEKey(
                             COSEKeyType.OKP,
                             COSECurve.X25519,
                             new Byte[32]
                         );

            Assert.That(x25519.TryToPublicKey(out _, out var notASigningCurve),  Is.False);
            Assert.That(notASigningCurve,  Does.Contain("not an EdDSA signature curve"));

            // An EdDSA key of the wrong width is refused before it reaches the
            // curve arithmetic: Ed448 is 57 bytes, not 56.
            var tooShort = new COSEKey(
                               COSEKeyType.OKP,
                               COSECurve.Ed448,
                               new Byte[56]
                           );

            Assert.That(tooShort.TryToPublicKey(out _, out var wrongWidth),  Is.False);
            Assert.That(wrongWidth,  Does.Contain("57 bytes long"));

            // An algorithm key pair without an algorithm can not be used at
            // all: its public key does not say which parameter set made it.
            var withoutAlgorithm = new COSEKey(
                                       COSEKeyType.AKP,
                                       Pub: new Byte[1312]
                                   );

            Assert.That(withoutAlgorithm.TryToPublicKey(out _, out var noAlgorithm),  Is.False);
            Assert.That(noAlgorithm,  Does.Contain("must name its algorithm"));

            // ...and an elliptic curve key that names no curve either.
            Assert.That(new COSEKey(COSEKeyType.EC2).TryToPublicKey(out _, out var withoutCurve),  Is.False);
            Assert.That(withoutCurve,  Does.Contain("must name an elliptic curve"));

        }

        #endregion

        #region The_published_key_thumbprint_is_reproduced()

        [Test]
        public void The_published_key_thumbprint_is_reproduced()
        {

            // The worked example of RFC 9679, Section 6.
            var key = new COSEKey(
                          COSEKeyType.EC2,
                          COSECurve.P256,
                          Convert.FromHexString("65EDA5A12577C2BAE829437FE338701A10AAA375E1BB5B5DE108DE439C08551D"),
                          Convert.FromHexString("1E52ED75701163F7F9E40DDF9F341B3DC9BA860AF7E0CA7CA7E9EECD0084D19C")
                      );

            Assert.That(Convert.ToHexStringLower(key.Thumbprint()),
                        Is.EqualTo("496bd8afadf307e5b08c64b0421bf9dc01528a344a43bda88fadd1669da253ec"));

            // The hashed input is the required parameters only, deterministically
            // encoded: kty, crv, x, y - whose labels 1, -1, -2 and -3 encode as
            // 0x01, 0x20, 0x21 and 0x22 and therefore sort in that order.
            Assert.That(CBORValue.Parse(key.ThumbprintInput()).ToDiagnosticString(),
                        Is.EqualTo("{1: 2, -1: 1, -2: h'65eda5a12577c2bae829437fe338701a10aaa375e1bb5b5de108de439c08551d', -3: h'1e52ed75701163f7f9e40ddf9f341b3dc9ba860af7e0ca7ca7e9eecd0084d19c'}"));

        }

        #endregion

        #region Optional_key_parameters_do_not_change_the_thumbprint()

        [Test]
        public void Optional_key_parameters_do_not_change_the_thumbprint()
        {

            var bare        = new COSEKey(
                                  COSEKeyType.EC2,
                                  COSECurve.P256,
                                  FromBase64URL(exampleKeyX),
                                  FromBase64URL(exampleKeyY)
                              );

            // The private key, the key identifier, the algorithm and any
            // additional parameter are all left out of the computation, so the
            // public and the private half of one key pair share an identity.
            var full        = ExampleKey();

            Assert.That(full.D,           Is.Not.Null);
            Assert.That(full.Thumbprint(),  Is.EqualTo(bare.Thumbprint()));

            Assert.That(full.ToPublicCOSEKey().Thumbprint(),  Is.EqualTo(bare.Thumbprint()));

            var decorated   = new COSEKey(
                                  COSEKeyType.EC2,
                                  COSECurve.P256,
                                  FromBase64URL(exampleKeyX),
                                  FromBase64URL(exampleKeyY),
                                  null,
                                  "whatever".ToUTF8Bytes(),
                                  COSEAlgorithm.ESP256,
                                  [CBORValue.FromInt64(2)],
                                  [new (CBORValue.FromText("vendor"), CBORValue.FromText("GraphDefined"))]
                              );

            Assert.That(decorated.Thumbprint(),  Is.EqualTo(bare.Thumbprint()));

            // A different key of course has a different thumbprint.
            var other       = new COSEKey(
                                  COSEKeyType.EC2,
                                  COSECurve.P256,
                                  Convert.FromHexString("65EDA5A12577C2BAE829437FE338701A10AAA375E1BB5B5DE108DE439C08551D"),
                                  Convert.FromHexString("1E52ED75701163F7F9E40DDF9F341B3DC9BA860AF7E0CA7CA7E9EECD0084D19C")
                              );

            Assert.That(other.Thumbprint(),  Is.Not.EqualTo(bare.Thumbprint()));

        }

        #endregion

        #region A_compressed_public_point_has_the_same_thumbprint()

        [Test]
        public void A_compressed_public_point_has_the_same_thumbprint()
        {

            var x           = FromBase64URL(exampleKeyX);
            var y           = FromBase64URL(exampleKeyY);

            var compressed  = COSEKey.Parse(
                                  CBORValue.FromMap([
                                      new (COSEKey.KeyTypeLabel,  CBORValue.FromInt64((Int64) COSEKeyType.EC2)),
                                      new (COSEKey.CurveLabel,    COSECurve.P256.ToCBOR()),
                                      new (COSEKey.XLabel,        CBORValue.FromBytes(x)),
                                      new (COSEKey.YLabel,        CBORValue.FromBoolean((y[^1] & 1) == 1))
                                  ])
                              );

            // The y coordinate is recovered on parsing, so how the point was
            // written does not change the identity of the key.
            Assert.That(compressed.Thumbprint(),
                        Is.EqualTo(new COSEKey(COSEKeyType.EC2, COSECurve.P256, x, y).Thumbprint()));

        }

        #endregion

        #region A_key_identifier_is_the_leading_bytes_of_the_thumbprint()

        [Test]
        public void A_key_identifier_is_the_leading_bytes_of_the_thumbprint()
        {

            var key         = ExampleKey();
            var thumbprint  = key.Thumbprint();

            Assert.That(thumbprint.Length,                        Is.EqualTo(32));
            Assert.That(key.ThumbprintKeyIdentifier().Length,      Is.EqualTo(8));
            Assert.That(key.ThumbprintKeyIdentifier(),             Is.EqualTo(thumbprint[..8]));
            Assert.That(key.ThumbprintKeyIdentifier(16),           Is.EqualTo(thumbprint[..16]));

            // The one line a key needs when it is provisioned...
            var provisioned = key.WithThumbprintKeyIdentifier();

            Assert.That(provisioned.KeyIdentifier,  Is.EqualTo(thumbprint[..8]));
            Assert.That(provisioned.X,              Is.EqualTo(key.X));
            Assert.That(provisioned.D,              Is.EqualTo(key.D));

            // ...and setting it does not change the thumbprint it came from,
            // so the operation is idempotent.
            Assert.That(provisioned.Thumbprint(),                     Is.EqualTo(thumbprint));
            Assert.That(provisioned.WithThumbprintKeyIdentifier().KeyIdentifier,  Is.EqualTo(provisioned.KeyIdentifier));

            Assert.That(() => key.ThumbprintKeyIdentifier(0),   Throws.TypeOf<COSEException>());
            Assert.That(() => key.ThumbprintKeyIdentifier(33),  Throws.TypeOf<COSEException>());

        }

        #endregion

        #region Malformed_keys_are_rejected()

        [Test]
        public void Malformed_keys_are_rejected()
        {

            // A COSE key is a map...
            Assert.That(COSEKey.TryParse(Convert.FromHexString("80"), out _, out var notAMap),  Is.False);
            Assert.That(notAMap,       Does.Contain("must be a CBOR map"));

            // ...that names its key type...
            Assert.That(COSEKey.TryParse(Convert.FromHexString("A12001"), out _, out var withoutKeyType),  Is.False);
            Assert.That(withoutKeyType,  Does.Contain("must have a key type"));

            // ...as an integer...
            Assert.That(COSEKey.TryParse(Convert.FromHexString("A1016145"), out _, out var textKeyType),  Is.False);
            Assert.That(textKeyType,   Does.Contain("key type"));

            // ...and whose x coordinate is a byte string.
            Assert.That(COSEKey.TryParse(Convert.FromHexString("A2010221187B"), out _, out var numericX),  Is.False);
            Assert.That(numericX,      Does.Contain("x coordinate"));

        }

        #endregion

    }

}
