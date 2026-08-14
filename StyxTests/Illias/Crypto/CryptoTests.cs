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
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto.Parameters;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias.Tests
{

    /// <summary>
    /// The elliptic curve key serialization of the Crypto helpers.
    ///
    /// Everything here turns on one property: Key material is a FIXED-WIDTH
    /// unsigned magnitude, and its leading zero octets are part of the
    /// encoding. A signed two's complement serialization strips them and
    /// prepends a zero byte whenever the leading bit is set, which produces
    /// keys that other implementations reject - or worse, misread.
    /// </summary>
    [TestFixture]
    public class CryptoTests
    {

        #region (private static) Curve            (Name)

        /// <summary>
        /// The elliptic curve domain parameters of the given named curve.
        /// </summary>
        private static ECDomainParameters Curve(String Name)
        {

            var parameters = ECNamedCurveTable.GetByName(Name);

            return new ECDomainParameters(
                       parameters.Curve,
                       parameters.G,
                       parameters.N,
                       parameters.H,
                       parameters.GetSeed()
                   );

        }

        #endregion

        #region (private static) PrivateKeyOf     (Curve, D)

        /// <summary>
        /// An elliptic curve private key of the given value.
        /// </summary>
        private static ECPrivateKeyParameters PrivateKeyOf(ECDomainParameters Curve, BigInteger D)

            => new ("ECDSA", D, Curve);

        #endregion

        #region (private static) PublicKeyOf      (Curve, D)

        /// <summary>
        /// The public key belonging to the given private key value.
        /// </summary>
        private static ECPublicKeyParameters PublicKeyOf(ECDomainParameters Curve, BigInteger D)

            => new ("ECDSA", Curve.G.Multiply(D).Normalize(), Curve);

        #endregion


        #region Private_keys_keep_their_leading_zeroes()

        [Test]
        public void Private_keys_keep_their_leading_zeroes()
        {

            var p256        = Curve("secp256r1");

            // The private key 1 is the sharpest case of the padding rule:
            // 31 zero bytes followed by a single 1. A signed two's complement
            // serialization would shorten it to one single byte.
            var serialized  = Crypto.SerializePrivateKey(PrivateKeyOf(p256, BigInteger.One));

            Assert.That(serialized.Length,   Is.EqualTo(32));
            Assert.That(serialized[..31],    Is.EqualTo(new Byte[31]));
            Assert.That(serialized[31],      Is.EqualTo(1));

            // ...and it comes back as the very same key.
            Assert.That(Crypto.ParsePrivateKeyBytes(p256, serialized).D,  Is.EqualTo(BigInteger.One));

            // What the naive serialization would have produced, so that this
            // test can not quietly start passing for the wrong reason.
            Assert.That(BigInteger.One.ToByteArray().Length,  Is.EqualTo(1));

        }

        #endregion

        #region Private_keys_with_a_leading_bit_set_are_not_read_as_negative()

        [Test]
        public void Private_keys_with_a_leading_bit_set_are_not_read_as_negative()
        {

            var p256        = Curve("secp256r1");

            // The group order of P-256 begins with 0xFF, so n-1 is the largest
            // valid private key AND has its leading bit set - the case a signed
            // two's complement reading turns into a negative number, silently
            // yielding a different key.
            var d           = p256.N.Subtract(BigInteger.One);
            var serialized  = Crypto.SerializePrivateKey(PrivateKeyOf(p256, d));

            Assert.That(serialized.Length,  Is.EqualTo(32));
            Assert.That(serialized[0],      Is.GreaterThanOrEqualTo(0x80),  "The test vector must have its leading bit set!");

            var parsed      = Crypto.ParsePrivateKeyBytes(p256, serialized);

            Assert.That(parsed.D.SignValue,  Is.EqualTo(1));
            Assert.That(parsed.D,            Is.EqualTo(d));
            Assert.That(parsed.D,            Is.LessThan(p256.N));

            // The very same bytes read as signed two's complement really are
            // a negative number - the unsigned reading is doing actual work
            // here, and this assertion says so for good.
            Assert.That(new BigInteger(serialized).SignValue,  Is.EqualTo(-1));

        }

        #endregion

        #region The_three_private_key_representations_agree()

        [Test]
        public void The_three_private_key_representations_agree()
        {

            var parameters  = ECNamedCurveTable.GetByName("secp256r1");
            var p256        = Curve("secp256r1");

            foreach (var d in new[] {
                                  BigInteger.One,
                                  p256.N.Subtract(BigInteger.One),
                                  new BigInteger("57C92077664146E876760C9520D054AA93C3AFB04E306705DB6090308507B4D3", 16)
                              })
            {

                var serialized  = Crypto.SerializePrivateKey(PrivateKeyOf(p256, d));

                Assert.That(serialized.Length,  Is.EqualTo(32),  d.ToString(16));

                // The byte, the hexadecimal and the base64 representation of
                // one and the same key must yield one and the same key.
                Assert.That(Crypto.ParsePrivateKeyBytes (parameters, serialized).D,
                            Is.EqualTo(d),  d.ToString(16));

                Assert.That(Crypto.ParsePrivateKeyHEX   (parameters, Convert.ToHexStringLower(serialized)).D,
                            Is.EqualTo(d),  d.ToString(16));

                Assert.That(Crypto.ParsePrivateKeyBase64(parameters, Convert.ToBase64String(serialized)).D,
                            Is.EqualTo(d),  d.ToString(16));

            }

        }

        #endregion

        #region Public_key_coordinates_are_as_wide_as_a_field_element()

        [Test]
        public void Public_key_coordinates_are_as_wide_as_a_field_element()
        {

            var p256            = Curve("secp256r1");
            var leadingBitSeen  = false;
            var signedWiderSeen = false;

            // The first multiples of the generator point are a deterministic
            // set of real points, about half of whose coordinates have their
            // leading bit set - which is exactly what a signed serialization
            // would widen to 33 bytes.
            for (var i = 1; i <= 8; i++)
            {

                var publicKey  = PublicKeyOf(p256, BigInteger.ValueOf(i));
                var (x, y)     = Crypto.SerializePublicKeyXY(publicKey);

                Assert.That(x.Length,  Is.EqualTo(32),  $"{i} * G, x");
                Assert.That(y.Length,  Is.EqualTo(32),  $"{i} * G, y");

                leadingBitSeen   = leadingBitSeen  || x[0] >= 0x80 || y[0] >= 0x80;

                // What the naive serialization would have produced for the
                // very same point.
                var point        = publicKey.Q.Normalize();
                signedWiderSeen  = signedWiderSeen ||
                                   point.AffineXCoord.ToBigInteger().ToByteArray().Length != 32 ||
                                   point.AffineYCoord.ToBigInteger().ToByteArray().Length != 32;

            }

            Assert.That(leadingBitSeen,   Is.True,  "None of the test vectors had its leading bit set!");
            Assert.That(signedWiderSeen,  Is.True,  "None of the test vectors would have been mis-serialized by the signed encoding!");

            // The coordinates of the generator point are the published ones.
            var generator   = p256.G.Normalize();
            var (gx, gy)    = Crypto.SerializePublicKeyXY(PublicKeyOf(p256, BigInteger.One));

            Assert.That(gx,  Is.EqualTo(generator.AffineXCoord.ToBigInteger().ToByteArrayUnsigned()));
            Assert.That(gy,  Is.EqualTo(generator.AffineYCoord.ToBigInteger().ToByteArrayUnsigned()));

        }

        #endregion

        #region Public_key_coordinates_are_fixed_width_on_every_curve()

        [Test]
        public void Public_key_coordinates_are_fixed_width_on_every_curve()
        {

            var expected = new (String CurveName, Int32 FieldSize, Int32 OrderSize)[] {
                               ("secp256r1",       32, 32),
                               ("secp384r1",       48, 48),
                               ("secp521r1",       66, 66),
                               ("brainpoolP256r1", 32, 32),
                               ("brainpoolP512r1", 64, 64)
                           };

            foreach (var vector in expected)
            {

                var curve   = Curve(vector.CurveName);
                var keyPair = Crypto.GenerateKeys(ECNamedCurveTable.GetByName(vector.CurveName));

                var (x, y)  = Crypto.SerializePublicKeyXY((ECPublicKeyParameters) keyPair.Public);

                Assert.That(x.Length,  Is.EqualTo(vector.FieldSize),  $"{vector.CurveName}, x");
                Assert.That(y.Length,  Is.EqualTo(vector.FieldSize),  $"{vector.CurveName}, y");

                Assert.That(Crypto.SerializePrivateKey((ECPrivateKeyParameters) keyPair.Private).Length,
                            Is.EqualTo(vector.OrderSize),
                            vector.CurveName);

                // P-521 is the reminder that the width follows the curve and
                // not the byte count of any particular key: its group order is
                // 521 bits, hence 66 bytes, of which the first holds a single bit.
                Assert.That(curve.Curve.FieldSize,  Is.EqualTo(vector.CurveName == "secp521r1" ? 521 : vector.FieldSize * 8),
                            vector.CurveName);

            }

        }

        #endregion

        #region Public_keys_roundtrip_through_their_point_encoding()

        [Test]
        public void Public_keys_roundtrip_through_their_point_encoding()
        {

            var parameters  = ECNamedCurveTable.GetByName("secp256r1");
            var p256        = Curve("secp256r1");
            var publicKey   = PublicKeyOf(p256, BigInteger.ValueOf(7));

            var serialized  = Crypto.SerializePublicKey(publicKey);

            // An uncompressed point: 0x04 and both coordinates.
            Assert.That(serialized.Length,  Is.EqualTo(65));
            Assert.That(serialized[0],      Is.EqualTo(0x04));

            var (x, y)      = Crypto.SerializePublicKeyXY(publicKey);

            Assert.That(serialized[1..33],   Is.EqualTo(x));
            Assert.That(serialized[33..65],  Is.EqualTo(y));

            Assert.That(Crypto.ParsePublicKey      (parameters, serialized).Q.Normalize(),
                        Is.EqualTo(publicKey.Q.Normalize()));

            Assert.That(Crypto.ParsePublicKeyHEX   (parameters, Convert.ToHexStringLower(serialized)).Q.Normalize(),
                        Is.EqualTo(publicKey.Q.Normalize()));

            Assert.That(Crypto.ParsePublicKeyBase64(parameters, Convert.ToBase64String(serialized)).Q.Normalize(),
                        Is.EqualTo(publicKey.Q.Normalize()));

        }

        #endregion

        #region A_serialized_key_pair_survives_a_full_roundtrip()

        [Test]
        public void A_serialized_key_pair_survives_a_full_roundtrip()
        {

            var parameters = ECNamedCurveTable.GetByName("secp256r1");

            // Generating, storing and reloading a key pair must yield the very
            // same keys - for every key, not merely for those whose leading
            // bit happens to be clear.
            for (var i = 0; i < 16; i++)
            {

                var keyPair     = Crypto.GenerateKeys(parameters);
                var privateKey  = (ECPrivateKeyParameters) keyPair.Private;
                var publicKey   = (ECPublicKeyParameters)  keyPair.Public;

                var reloaded    = Crypto.ParsePrivateKeyBytes(parameters, Crypto.SerializePrivateKey(privateKey));

                Assert.That(reloaded.D,  Is.EqualTo(privateKey.D));

                Assert.That(Crypto.CalculatePublicKey(reloaded).Q.Normalize(),
                            Is.EqualTo(publicKey.Q.Normalize()));

                Assert.That(Crypto.ParsePublicKey(parameters, Crypto.SerializePublicKey(publicKey)).Q.Normalize(),
                            Is.EqualTo(publicKey.Q.Normalize()));

            }

        }

        #endregion

    }

}
