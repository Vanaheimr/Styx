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
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// A COSE signature algorithm together with a key pair to sign with it.
    /// There is a static factory for every signature algorithm this library
    /// knows: given a key pair it pairs it with the algorithm, otherwise it
    /// makes a new one of the kind the algorithm needs.
    /// </summary>
    public readonly struct COSEKeyPair(COSEAlgorithm            Algorithm,
                                       AsymmetricCipherKeyPair  KeyPair)
    {

        #region Properties

        /// <summary>
        /// The COSE algorithm identifier.
        /// </summary>
        public COSEAlgorithm            Algorithm    { get; } = Algorithm;

        /// <summary>
        /// The asymmetric cipher key pair.
        /// </summary>
        public AsymmetricCipherKeyPair  KeyPair      { get; } = KeyPair;


        /// <summary>
        /// The private key parameter.
        /// </summary>
        public AsymmetricKeyParameter Private
            => KeyPair.Private;

        /// <summary>
        /// The public key parameter.
        /// </summary>
        public AsymmetricKeyParameter Public
            => KeyPair.Public;

        #endregion


        #region ECDSA

        // ES256, ES384 and ES512 leave the curve to the key: a new key is on
        // the curve they are used with in practice.
        public static COSEKeyPair NewES256   (AsymmetricCipherKeyPair?  KeyPair        = null,
                                              SecureRandom?             SecureRandom   = null)

            => new (COSEAlgorithm.ES256,
                    KeyPair ?? COSECurve.P256.NewKeyPair(SecureRandom));

        public static COSEKeyPair NewES384   (AsymmetricCipherKeyPair?  KeyPair        = null,
                                              SecureRandom?             SecureRandom   = null)

            => new (COSEAlgorithm.ES384,
                    KeyPair ?? COSECurve.P384.NewKeyPair(SecureRandom));

        public static COSEKeyPair NewES512   (AsymmetricCipherKeyPair?  KeyPair        = null,
                                              SecureRandom?             SecureRandom   = null)

            => new (COSEAlgorithm.ES512,
                    KeyPair ?? COSECurve.P521.NewKeyPair(SecureRandom));

        public static COSEKeyPair NewES256K  (AsymmetricCipherKeyPair?  KeyPair        = null,
                                              SecureRandom?             SecureRandom   = null)

            => new (COSEAlgorithm.ES256K,
                    KeyPair ?? COSECurve.Secp256k1.NewKeyPair(SecureRandom));


        public static COSEKeyPair NewESP256  (AsymmetricCipherKeyPair?  KeyPair        = null,
                                              SecureRandom?             SecureRandom   = null)

            => new (COSEAlgorithm.ESP256,
                    KeyPair ?? COSECurve.P256.NewKeyPair(SecureRandom));

        public static COSEKeyPair NewESP384  (AsymmetricCipherKeyPair?  KeyPair        = null,
                                              SecureRandom?             SecureRandom   = null)

            => new (COSEAlgorithm.ESP384,
                    KeyPair ?? COSECurve.P384.NewKeyPair(SecureRandom));

        public static COSEKeyPair NewESP512  (AsymmetricCipherKeyPair?  KeyPair        = null,
                                              SecureRandom?             SecureRandom   = null)

            => new (COSEAlgorithm.ESP512,
                    KeyPair ?? COSECurve.P521.NewKeyPair(SecureRandom));


        public static COSEKeyPair NewESB256  (AsymmetricCipherKeyPair?  KeyPair        = null,
                                              SecureRandom?             SecureRandom   = null)

            => new (COSEAlgorithm.ESB256,
                    KeyPair ?? COSECurve.BrainpoolP256r1.NewKeyPair(SecureRandom));

        public static COSEKeyPair NewESB320  (AsymmetricCipherKeyPair?  KeyPair        = null,
                                              SecureRandom?             SecureRandom   = null)

            => new (COSEAlgorithm.ESB320,
                    KeyPair ?? COSECurve.BrainpoolP320r1.NewKeyPair(SecureRandom));

        public static COSEKeyPair NewESB384  (AsymmetricCipherKeyPair?  KeyPair        = null,
                                              SecureRandom?             SecureRandom   = null)

            => new (COSEAlgorithm.ESB384,
                    KeyPair ?? COSECurve.BrainpoolP384r1.NewKeyPair(SecureRandom));

        public static COSEKeyPair NewESB512  (AsymmetricCipherKeyPair?  KeyPair        = null,
                                              SecureRandom?             SecureRandom   = null)

            => new (COSEAlgorithm.ESB512,
                    KeyPair ?? COSECurve.BrainpoolP512r1.NewKeyPair(SecureRandom));

        #endregion

        #region EdDSA

        // EdDSA leaves the curve to the key: a new key is an Ed25519 key.
        public static COSEKeyPair NewEdDSA   (AsymmetricCipherKeyPair?  KeyPair        = null,
                                              SecureRandom?             SecureRandom   = null)
        {

            if (KeyPair is not null)
                return new (COSEAlgorithm.EdDSA, KeyPair);

            var privateKey = new Ed25519PrivateKeyParameters(SecureRandom ?? new SecureRandom());

            return new (
                       COSEAlgorithm.EdDSA,
                       new AsymmetricCipherKeyPair(
                           privateKey.GeneratePublicKey(),
                           privateKey
                       )
                   );

        }

        public static COSEKeyPair NewEd25519 (AsymmetricCipherKeyPair?  KeyPair        = null,
                                              SecureRandom?             SecureRandom   = null)
        {

            if (KeyPair is not null)
                return new (COSEAlgorithm.Ed25519, KeyPair);

            var privateKey = new Ed25519PrivateKeyParameters(SecureRandom ?? new SecureRandom());

            return new (
                       COSEAlgorithm.Ed25519,
                       new AsymmetricCipherKeyPair(
                           privateKey.GeneratePublicKey(),
                           privateKey
                       )
                   );

        }

        public static COSEKeyPair NewEd448   (AsymmetricCipherKeyPair?  KeyPair        = null,
                                              SecureRandom?             SecureRandom   = null)
        {

            if (KeyPair is not null)
                return new (COSEAlgorithm.Ed448, KeyPair);

            var privateKey = new Ed448PrivateKeyParameters(SecureRandom ?? new SecureRandom());

            return new (
                       COSEAlgorithm.Ed448,
                       new AsymmetricCipherKeyPair(
                           privateKey.GeneratePublicKey(),
                           privateKey
                       )
                   );

        }

        #endregion

        #region ML-DSA

        public static COSEKeyPair NewMLDsa44 (AsymmetricCipherKeyPair?  KeyPair        = null,
                                              SecureRandom?             SecureRandom   = null)

            => new (COSEAlgorithm.MLDsa44,
                    KeyPair ?? MLDsaParameters.ml_dsa_44.NewKeyPair(SecureRandom));

        public static COSEKeyPair NewMLDsa65 (AsymmetricCipherKeyPair?  KeyPair        = null,
                                              SecureRandom?             SecureRandom   = null)

            => new (COSEAlgorithm.MLDsa65,
                    KeyPair ?? MLDsaParameters.ml_dsa_65.NewKeyPair(SecureRandom));

        public static COSEKeyPair NewMLDsa87 (AsymmetricCipherKeyPair?  KeyPair        = null,
                                              SecureRandom?             SecureRandom   = null)

            => new (COSEAlgorithm.MLDsa87,
                    KeyPair ?? MLDsaParameters.ml_dsa_87.NewKeyPair(SecureRandom));

        #endregion

        #region RSA

        // RSA keys of at least 2048 bits [RFC 8230, Section 6.1]; a new key
        // has 3072 bits, the size NIST holds sufficient beyond 2030.
        public static COSEKeyPair NewPS256   (AsymmetricCipherKeyPair?  KeyPair        = null,
                                              SecureRandom?             SecureRandom   = null,
                                              Int32                     KeySizeInBits  = 3072)

            => new (COSEAlgorithm.PS256,
                    KeyPair ?? NewRSAKeyPair(KeySizeInBits, SecureRandom));

        public static COSEKeyPair NewPS384   (AsymmetricCipherKeyPair?  KeyPair        = null,
                                              SecureRandom?             SecureRandom   = null,
                                              Int32                     KeySizeInBits  = 3072)

            => new (COSEAlgorithm.PS384,
                    KeyPair ?? NewRSAKeyPair(KeySizeInBits, SecureRandom));

        public static COSEKeyPair NewPS512   (AsymmetricCipherKeyPair?  KeyPair        = null,
                                              SecureRandom?             SecureRandom   = null,
                                              Int32                     KeySizeInBits  = 3072)

            => new (COSEAlgorithm.PS512,
                    KeyPair ?? NewRSAKeyPair(KeySizeInBits, SecureRandom));

        public static COSEKeyPair NewRS256   (AsymmetricCipherKeyPair?  KeyPair        = null,
                                              SecureRandom?             SecureRandom   = null,
                                              Int32                     KeySizeInBits  = 3072)

            => new (COSEAlgorithm.RS256,
                    KeyPair ?? NewRSAKeyPair(KeySizeInBits, SecureRandom));

        public static COSEKeyPair NewRS384   (AsymmetricCipherKeyPair?  KeyPair        = null,
                                              SecureRandom?             SecureRandom   = null,
                                              Int32                     KeySizeInBits  = 3072)

            => new (COSEAlgorithm.RS384,
                    KeyPair ?? NewRSAKeyPair(KeySizeInBits, SecureRandom));

        public static COSEKeyPair NewRS512   (AsymmetricCipherKeyPair?  KeyPair        = null,
                                              SecureRandom?             SecureRandom   = null,
                                              Int32                     KeySizeInBits  = 3072)

            => new (COSEAlgorithm.RS512,
                    KeyPair ?? NewRSAKeyPair(KeySizeInBits, SecureRandom));


        private static AsymmetricCipherKeyPair NewRSAKeyPair(Int32          KeySizeInBits,
                                                             SecureRandom?  SecureRandom)
        {

            if (KeySizeInBits < COSEAlgorithm.MinimumRSAModulusSizeInBits)
                throw new COSEException($"An RSA key must have at least {COSEAlgorithm.MinimumRSAModulusSizeInBits} bits [RFC 8230, Section 6.1], not {KeySizeInBits}!");

            var generator = new RsaKeyPairGenerator();

            generator.Init(
                new RsaKeyGenerationParameters(
                    BigInteger.ValueOf(65537),
                    SecureRandom ?? new SecureRandom(),
                    KeySizeInBits,
                    100
                )
            );

            return generator.GenerateKeyPair();

        }

        #endregion

    }

}
