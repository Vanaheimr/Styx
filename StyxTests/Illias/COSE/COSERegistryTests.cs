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

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias.Tests
{

    /// <summary>
    /// The COSE algorithm and elliptic curve identifiers.
    ///
    /// These numbers are not ours: They come from the IANA "COSE Algorithms"
    /// and "COSE Elliptic Curves" registries and are part of every signed
    /// message on the wire. Getting one of them wrong produces signatures
    /// nobody else can verify, which is why the whole table is pinned here.
    /// </summary>
    [TestFixture]
    public class COSERegistryTests
    {

        #region Registered_algorithm_identifications_are_stable()

        [Test]
        public void Registered_algorithm_identifications_are_stable()
        {

            var expected = new (COSEAlgorithm Algorithm, Int32 Value, String Name, Boolean SupportedForSigning, Boolean Deprecated, COSECurve? FixedCurve)[] {

                               (COSEAlgorithm.ES256,     -7, "ES256",   true,  true,  null),
                               (COSEAlgorithm.ES384,    -35, "ES384",   true,  true,  null),
                               (COSEAlgorithm.ES512,    -36, "ES512",   true,  true,  null),
                               (COSEAlgorithm.ES256K,   -47, "ES256K",  true,  false, COSECurve.Secp256k1),

                               (COSEAlgorithm.ESP256,    -9, "ESP256",  true,  false, COSECurve.P256),
                               (COSEAlgorithm.ESP384,   -51, "ESP384",  true,  false, COSECurve.P384),
                               (COSEAlgorithm.ESP512,   -52, "ESP512",  true,  false, COSECurve.P521),

                               (COSEAlgorithm.ESB256,  -265, "ESB256",  true,  false, COSECurve.BrainpoolP256r1),
                               (COSEAlgorithm.ESB320,  -266, "ESB320",  true,  false, COSECurve.BrainpoolP320r1),
                               (COSEAlgorithm.ESB384,  -267, "ESB384",  true,  false, COSECurve.BrainpoolP384r1),
                               (COSEAlgorithm.ESB512,  -268, "ESB512",  true,  false, COSECurve.BrainpoolP512r1),

                               // EdDSA [RFC 8032]: pure, and deterministic
                               // without being asked. The un-suffixed
                               // identifier leaves the curve to the key.
                               (COSEAlgorithm.EdDSA,     -8, "EdDSA",   true,  true,  null),
                               (COSEAlgorithm.Ed25519,  -19, "Ed25519", true,  false, COSECurve.Ed25519),
                               (COSEAlgorithm.Ed448,    -53, "Ed448",   true,  false, COSECurve.Ed448),

                               // ML-DSA [FIPS 204, RFC 9964]: pure as well,
                               // and on no curve at all — its keys are
                               // algorithm key pairs.
                               (COSEAlgorithm.MLDsa44,  -48, "ML-DSA-44", true, false, null),
                               (COSEAlgorithm.MLDsa65,  -49, "ML-DSA-65", true, false, null),
                               (COSEAlgorithm.MLDsa87,  -50, "ML-DSA-87", true, false, null),

                               // HMAC [RFC 9053, Section 3.1]: message
                               // authentication and NOT signature, so
                               // IsSupportedForSigning is false for all four —
                               // whoever can verify a MAC can produce one, and
                               // this library refuses to let the two be
                               // confused.
                               (COSEAlgorithm.HMAC256_64,    4, "HMAC 256/64",  false, false, null),
                               (COSEAlgorithm.HMAC256_256,   5, "HMAC 256/256", false, false, null),
                               (COSEAlgorithm.HMAC384_384,   6, "HMAC 384/384", false, false, null),
                               (COSEAlgorithm.HMAC512_512,   7, "HMAC 512/512", false, false, null),

                               // AES-GCM [RFC 9053, Section 4.1]: content
                               // ENCRYPTION, so they sign nothing either — and
                               // an encrypted message says nothing about who
                               // sent it, which is why a record is signed and
                               // then encrypted rather than merely encrypted.
                               (COSEAlgorithm.A128GCM,    1, "A128GCM", false, false, null),
                               (COSEAlgorithm.A192GCM,    2, "A192GCM", false, false, null),
                               (COSEAlgorithm.A256GCM,    3, "A256GCM", false, false, null),

                               // The recipient algorithms: they carry a content
                               // key rather than content. The width named by a
                               // key wrap identifier is that of the KEY-ENCRYPTION
                               // key, not of the key being wrapped.
                               (COSEAlgorithm.A128KW,    -3, "A128KW",  false, false, null),
                               (COSEAlgorithm.A192KW,    -4, "A192KW",  false, false, null),
                               (COSEAlgorithm.A256KW,    -5, "A256KW",  false, false, null),
                               (COSEAlgorithm.Direct,    -6, "direct",  false, false, null),

                               // Hash algorithms [RFC 9054]: they sign nothing,
                               // they name the digest of a thumbprint.
                               (COSEAlgorithm.SHA256,   -16, "SHA-256", false, false, null),
                               (COSEAlgorithm.SHA384,   -43, "SHA-384", false, false, null),
                               (COSEAlgorithm.SHA512,   -44, "SHA-512", false, false, null),
                               (COSEAlgorithm.SHA1,     -14, "SHA-1",   false, true,  null)

                           };

            foreach (var vector in expected)
            {

                Assert.That(vector.Algorithm.Value,                  Is.EqualTo(vector.Value),                vector.Name);
                Assert.That(vector.Algorithm.Name,                   Is.EqualTo(vector.Name),                 vector.Name);
                Assert.That(vector.Algorithm.IsKnown,                Is.True,                                 vector.Name);
                Assert.That(vector.Algorithm.IsSupportedForSigning,  Is.EqualTo(vector.SupportedForSigning),  vector.Name);
                Assert.That(vector.Algorithm.IsDeprecated,           Is.EqualTo(vector.Deprecated),           vector.Name);
                Assert.That(vector.Algorithm.FixedCurve,             Is.EqualTo(vector.FixedCurve),           vector.Name);

                Assert.That(COSEAlgorithm.Parse(vector.Name),        Is.EqualTo(vector.Algorithm),            vector.Name);
                Assert.That(COSEAlgorithm.TryParse(vector.Value, out var byNumber),  Is.True,                 vector.Name);
                Assert.That(byNumber,                                Is.EqualTo(vector.Algorithm),            vector.Name);

                // The identifier on the wire is a plain CBOR integer.
                Assert.That(vector.Algorithm.ToCBOR().AsInt64(),     Is.EqualTo(vector.Value),                vector.Name);

            }

            Assert.That(COSEAlgorithm.All.Count(),  Is.EqualTo(expected.Length));

        }

        #endregion

        #region Registered_curve_identifications_are_stable()

        [Test]
        public void Registered_curve_identifications_are_stable()
        {

            var expected = new (COSECurve Curve, Int32 Value, String Name, COSEKeyType KeyType, Int32? FieldSize, Int32? OrderSize)[] {

                               (COSECurve.P256,               1, "P-256",             COSEKeyType.EC2,   32,   32),
                               (COSECurve.P384,               2, "P-384",             COSEKeyType.EC2,   48,   48),
                               (COSECurve.P521,               3, "P-521",             COSEKeyType.EC2,   66,   66),
                               (COSECurve.X25519,             4, "X25519",            COSEKeyType.OKP, null, null),
                               (COSECurve.X448,               5, "X448",              COSEKeyType.OKP, null, null),
                               // An EdDSA key is a fixed-width octet string
                               // rather than a point, and Ed448 is 57 bytes
                               // and not 56: RFC 8032 appends a sign bit.
                               (COSECurve.Ed25519,            6, "Ed25519",           COSEKeyType.OKP,   32,   32),
                               (COSECurve.Ed448,              7, "Ed448",             COSEKeyType.OKP,   57,   57),
                               (COSECurve.Secp256k1,          8, "secp256k1",         COSEKeyType.EC2,   32,   32),
                               (COSECurve.BrainpoolP256r1,  256, "brainpoolP256r1",   COSEKeyType.EC2,   32,   32),
                               (COSECurve.BrainpoolP320r1,  257, "brainpoolP320r1",   COSEKeyType.EC2,   40,   40),
                               (COSECurve.BrainpoolP384r1,  258, "brainpoolP384r1",   COSEKeyType.EC2,   48,   48),
                               (COSECurve.BrainpoolP512r1,  259, "brainpoolP512r1",   COSEKeyType.EC2,   64,   64)

                           };

            foreach (var vector in expected)
            {

                Assert.That(vector.Curve.Value,               Is.EqualTo(vector.Value),      vector.Name);
                Assert.That(vector.Curve.Name,                Is.EqualTo(vector.Name),       vector.Name);
                Assert.That(vector.Curve.IsKnown,             Is.True,                       vector.Name);
                Assert.That(vector.Curve.KeyType,             Is.EqualTo(vector.KeyType),    vector.Name);
                Assert.That(vector.Curve.FieldSizeInBytes,    Is.EqualTo(vector.FieldSize),  vector.Name);
                Assert.That(vector.Curve.OrderSizeInBytes,    Is.EqualTo(vector.OrderSize),  vector.Name);

                Assert.That(COSECurve.Parse(vector.Name),     Is.EqualTo(vector.Curve),      vector.Name);

                // Every EC2 curve must be one this implementation can compute
                // with, and must be found again from its domain parameters.
                if (vector.KeyType == COSEKeyType.EC2)
                {

                    Assert.That(vector.Curve.DomainParameters,  Is.Not.Null,  vector.Name);

                    Assert.That(COSECurve.TryGetFor(vector.Curve.DomainParameters!, out var found),  Is.True,  vector.Name);
                    Assert.That(found,  Is.EqualTo(vector.Curve),  vector.Name);

                }

            }

            Assert.That(COSECurve.All.Count(),  Is.EqualTo(expected.Length));

        }

        #endregion

        #region Unknown_identifications_stay_inspectable()

        [Test]
        public void Unknown_identifications_stay_inspectable()
        {

            // An algorithm registered after this implementation was written
            // must not make a message unparsable - it must make verification
            // fail with a clear reason instead.
            var unknown = new COSEAlgorithm(-1000);

            Assert.That(unknown.IsKnown,                Is.False);
            Assert.That(unknown.Name,                   Is.EqualTo("-1000"));
            Assert.That(unknown.IsSupportedForSigning,  Is.False);
            Assert.That(unknown.HashAlgorithm,          Is.Null);
            Assert.That(unknown.ToCBOR().AsInt64(),     Is.EqualTo(-1000));

            Assert.That(COSEAlgorithm.TryParse(-1000, out _),      Is.False);
            Assert.That(COSEAlgorithm.TryParse("es256", out _),    Is.False);
            Assert.That(() => COSEAlgorithm.Parse("HS256"),        Throws.TypeOf<COSEException>());

            var unknownCurve = new COSECurve(1000);

            Assert.That(unknownCurve.IsKnown,           Is.False);
            Assert.That(unknownCurve.DomainParameters,  Is.Null);
            Assert.That(unknownCurve.FieldSizeInBytes,  Is.Null);

            Assert.That(COSECurve.TryParse("p-256", out _),  Is.False);

        }

        #endregion

        #region The_COSE_CBOR_tags_are_the_registered_ones()

        [Test]
        public void The_COSE_CBOR_tags_are_the_registered_ones()
        {

            Assert.That(CBORTag.COSEEncrypt0.Value,  Is.EqualTo(16));
            Assert.That(CBORTag.COSEMac0.    Value,  Is.EqualTo(17));
            Assert.That(CBORTag.COSESign1.   Value,  Is.EqualTo(18));
            Assert.That(CBORTag.COSEEncrypt. Value,  Is.EqualTo(96));
            Assert.That(CBORTag.COSEMac.     Value,  Is.EqualTo(97));
            Assert.That(CBORTag.COSESign.    Value,  Is.EqualTo(98));

        }

        #endregion

    }

}
