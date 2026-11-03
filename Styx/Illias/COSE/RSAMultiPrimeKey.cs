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
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Parameters;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// A prime of a multi-prime RSA key after the first two: the prime r_i
    /// itself, its CRT exponent d_i = d mod (r_i - 1), and its CRT
    /// coefficient t_i = (r_1 · ... · r_(i-1))^-1 mod r_i [RFC 8017,
    /// Section 3.2].
    /// </summary>
    /// <param name="Prime">The prime factor r_i.</param>
    /// <param name="Exponent">The CRT exponent d_i.</param>
    /// <param name="Coefficient">The CRT coefficient t_i.</param>
    public sealed record RSAOtherPrime(BigInteger  Prime,
                                       BigInteger  Exponent,
                                       BigInteger  Coefficient);


    /// <summary>
    /// An RSA private key of more than two primes [RFC 8017, Section 3.2],
    /// for which Bouncy Castle has no class of its own.
    ///
    /// It extends the two-prime CRT key on purpose, twice over. Bouncy
    /// Castle blinds the private operation only for a CRT key, so this one
    /// is blinded as well. And an engine that does not know of the further
    /// primes computes with p and q alone, gets a signature that is wrong
    /// modulo every other prime, and Bouncy Castle's own fault check refuses
    /// to release it ("RSA engine faulty decryption/signing detected"): such
    /// a mistake fails loudly instead of handing out a signature that
    /// factors n. Only RSAMultiPrimeCore computes with all of the primes.
    /// </summary>
    public sealed class RSAMultiPrimePrivateKeyParameters : RsaPrivateCrtKeyParameters
    {

        #region Properties

        /// <summary>
        /// The primes after p and q, in their order: each coefficient t_i
        /// depends on the primes before it.
        /// </summary>
        public IReadOnlyList<RSAOtherPrime>  OtherPrimes    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new RSA private key of more than two primes.
        /// </summary>
        /// <param name="Modulus">The modulus n, the product of all primes.</param>
        /// <param name="PublicExponent">The public exponent e.</param>
        /// <param name="PrivateExponent">The private exponent d.</param>
        /// <param name="P">The first prime factor.</param>
        /// <param name="Q">The second prime factor.</param>
        /// <param name="DP">The first CRT exponent dP = d mod (p - 1).</param>
        /// <param name="DQ">The second CRT exponent dQ = d mod (q - 1).</param>
        /// <param name="QInv">The CRT coefficient qInv = q^-1 mod p.</param>
        /// <param name="OtherPrimes">The further primes, at least one.</param>
        public RSAMultiPrimePrivateKeyParameters(BigInteger                  Modulus,
                                                 BigInteger                  PublicExponent,
                                                 BigInteger                  PrivateExponent,
                                                 BigInteger                  P,
                                                 BigInteger                  Q,
                                                 BigInteger                  DP,
                                                 BigInteger                  DQ,
                                                 BigInteger                  QInv,
                                                 IEnumerable<RSAOtherPrime>  OtherPrimes)

            : base(Modulus, PublicExponent, PrivateExponent, P, Q, DP, DQ, QInv)

        {

            this.OtherPrimes = OtherPrimes.ToArray();

            if (this.OtherPrimes.Count == 0)
                throw new ArgumentException("A multi-prime RSA key has at least one prime besides p and q; a key of two primes is a RsaPrivateCrtKeyParameters!",
                                            nameof(OtherPrimes));

        }

        #endregion

    }


    /// <summary>
    /// The private RSA operation for a key of more than two primes: the
    /// second form of RSASP1 [RFC 8017, Section 5.1.2], which is the
    /// two-prime CRT followed by one step per further prime.
    ///
    /// It replaces only the exponentiation inside Bouncy Castle's engine.
    /// Wrapped in a RsaBlindedEngine it is blinded as any CRT key is, and
    /// the PKCS #1 v1.5 and PSS encodings around it stay Bouncy Castle's.
    ///
    /// Like Bouncy Castle's own core it checks every result against the
    /// public key before releasing it: a CRT signature that is wrong
    /// modulo one prime and right modulo the others gives that prime away
    /// to anybody who holds the message [Boneh, DeMillo, Lipton 1997], and
    /// with more primes there are more halves to get wrong.
    /// </summary>
    public sealed class RSAMultiPrimeCore : IRsa
    {

        #region Data

        // The conversions and the block sizes are those of a plain RSA core:
        // they depend on the modulus alone.
        private readonly RsaCoreEngine                       conversion = new();
        private          RSAMultiPrimePrivateKeyParameters?  key;

        #endregion

        #region Init(ForEncryption, Parameters)

        /// <summary>
        /// Initialise this core with a multi-prime RSA private key.
        /// </summary>
        /// <param name="ForEncryption">Whether the core is used to encrypt (or sign).</param>
        /// <param name="Parameters">A multi-prime RSA private key.</param>
        public void Init(Boolean            ForEncryption,
                         ICipherParameters  Parameters)
        {

            if (Parameters is ParametersWithRandom withRandom)
                Parameters = withRandom.Parameters;

            if (Parameters is not RSAMultiPrimePrivateKeyParameters multiPrimeKey)
                throw new ArgumentException($"The multi-prime RSA core computes with a multi-prime RSA private key only, but was given a {Parameters.GetType().Name}!",
                                            nameof(Parameters));

            conversion.Init(ForEncryption, multiPrimeKey);
            key = multiPrimeKey;

        }

        #endregion

        #region GetInputBlockSize() / GetOutputBlockSize()

        /// <summary>
        /// The maximum size of an input block.
        /// </summary>
        public Int32 GetInputBlockSize()
            => conversion.GetInputBlockSize();

        /// <summary>
        /// The maximum size of an output block.
        /// </summary>
        public Int32 GetOutputBlockSize()
            => conversion.GetOutputBlockSize();

        #endregion

        #region ConvertInput(InBuffer, InOffset, InLength) / ConvertOutput(Result)

        /// <summary>
        /// Convert the given input block into an integer smaller than the modulus.
        /// </summary>
        public BigInteger ConvertInput(Byte[]  InBuffer,
                                       Int32   InOffset,
                                       Int32   InLength)

            => conversion.ConvertInput(InBuffer, InOffset, InLength);

        /// <summary>
        /// Convert the given result into an output block.
        /// </summary>
        public Byte[] ConvertOutput(BigInteger Result)
            => conversion.ConvertOutput(Result);

        #endregion

        #region ProcessBlock(Input)

        /// <summary>
        /// Compute Input^d mod n from the primes of the key.
        /// </summary>
        /// <param name="Input">An integer smaller than the modulus.</param>
        public BigInteger ProcessBlock(BigInteger Input)
        {

            var key = this.key
                          ?? throw new InvalidOperationException("The multi-prime RSA core has not been initialised!");

            // Step i, the two-prime CRT, and steps iii and iv to join its halves.
            var s1     = Input.Remainder(key.P).ModPow(key.DP, key.P);
            var s2     = Input.Remainder(key.Q).ModPow(key.DQ, key.Q);
            var s      = s1.Subtract(s2).Multiply(key.QInv).Mod(key.P).Multiply(key.Q).Add(s2);

            // Steps ii and v, once per further prime: R is the product of the
            // primes before r_i, which is what t_i is the inverse of.
            var R      = key.P;
            var before = key.Q;

            foreach (var other in key.OtherPrimes)
            {

                R = R.Multiply(before);

                var si = Input.Remainder(other.Prime).ModPow(other.Exponent, other.Prime);
                s      = s.Add(R.Multiply(si.Subtract(s).Multiply(other.Coefficient).Mod(other.Prime)));

                before = other.Prime;

            }

            if (!s.ModPow(key.PublicExponent, key.Modulus).Equals(Input))
                throw new CryptoException("The multi-prime RSA computation does not invert under the public key, so its result is withheld: a faulty CRT result gives a prime factor away [Boneh, DeMillo, Lipton 1997]!");

            return s;

        }

        #endregion

    }

}
