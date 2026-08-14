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

            var publicKey   = key.ToPublicKey();
            var privateKey  = key.ToPrivateKey();

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
            Assert.That(publicKey!.Q.Normalize(),  Is.EqualTo(ExampleKey().ToPublicKey().Q.Normalize()));

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

        #region Only_elliptic_curve_keys_of_key_type_EC2_are_supported()

        [Test]
        public void Only_elliptic_curve_keys_of_key_type_EC2_are_supported()
        {

            var okp = new COSEKey(
                          COSEKeyType.OKP,
                          COSECurve.Ed25519,
                          new Byte[32]
                      );

            Assert.That(okp.TryToPublicKey(out _, out var errorResponse),  Is.False);
            Assert.That(errorResponse,  Does.Contain("key type EC2"));

            // ...and a key that names no curve at all can not be used either.
            Assert.That(new COSEKey(COSEKeyType.EC2).TryToPublicKey(out _, out var withoutCurve),  Is.False);
            Assert.That(withoutCurve,  Does.Contain("must name an elliptic curve"));

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
