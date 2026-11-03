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

using System.Security.Cryptography;

using NUnit.Framework;

using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Crypto.Parameters;

using BCBigInteger = Org.BouncyCastle.Math.BigInteger;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias.Tests
{

    /// <summary>
    /// An RSA key of three primes: the multi-prime form of RFC 8230,
    /// Section 4, whose further primes travel as an array at label -9, each
    /// a map of r_i, d_i and t_i at -10, -11 and -12.
    ///
    /// The key was generated once by OpenSSL (genrsa -primes 3 2048),
    /// because nothing else in reach generates one: three primes of 683, 683
    /// and 682 bits and a 2048-bit modulus. It secures nothing.
    ///
    /// Every signature is checked against the first form of RSASP1, s =
    /// m^d mod n [RFC 8017, Section 5.1.2], which needs neither the primes
    /// nor the CRT and is Bouncy Castle's own computation - so the multi-
    /// prime steps must arrive at the very same bytes - and against the RSA
    /// of .NET, which verifies with n and e and has never heard of a third
    /// prime.
    /// </summary>
    [TestFixture]
    public class COSERSAMultiPrimeTests
    {

        #region Data

        private static readonly Byte[] content  = "This is the content.".ToUTF8Bytes();

        private static readonly Byte[] n        = Convert.FromHexString("BCB36284D8B8480ECD5173C3A45C735FEAC58571FADA993CA377A1E0819304B82DAFE070FD80803B4090422958205E01384EE6F7A24FD4FB9B914E8157BF28874D29B755E5E179BF7576B27DD8BB184BD1F5519D18FA98CCCB53F782A378273280B5DF3EBB5EF18D10F1C55CB5EF77ACCBD4674DBDF67B2AB49A406B31021E52532A5C3EB4FDAC93EA122B3145933195C33F87F94D313D6CFD65E102A54BB5F111E208FABAF4A2B4BF11A5B9357495EF423B38718C0EFD59E1E4ADC3450C4E3C406EF4AA78CE935A98C4AECDF27AED1F3971CED402A26481300D9562F4E14264770D9BE4D1652EE4D046DD51658B61747C23406E02CA752D8683C5C75EEBA903");
        private static readonly Byte[] e        = Convert.FromHexString("010001");
        private static readonly Byte[] d        = Convert.FromHexString("3CA58EA95199D096E9481F63A6848C142CB934CD5029478CA4A10366CB5E3BEB11CBEFDF80A9F70C941089230E8EC087B5D82E0532A663B645E66A8BC96EC790CCACF69E65D694B18308160FBA434F68B6DF184EDC20F483812F7D86F6E25DF7758ED38495C543AF5193303FE02FD5059A0FF034F68C6FFAD9D93D56A56C96AD1B6DB544206E3BEFC36B248029B25A2492525BE2809B98D360E55619B490443038CC5CF9ADEE12357AA9B5465331862A363317E8B2B56CECF239AD5E76E56FEBE1A285E53C06C9C1AB5988538734E23CD44F240F5B1AB2BFEB17E31B0FEAD63F4D2445FF22C0DD19C177777E724629507F2262E66204C4A93691778A7E330421");
        private static readonly Byte[] p        = Convert.FromHexString("079AA5EA2806BF86F41F00AFDD13CD547AD64B3E9BC86E7DE108D7D7F4B5015749E145FC5AAAAD00CBD140083154B1C9F7B561E6D8B07C34E156EC7CCFDBECE64ABAF76834221D80CB905CFB1A9701379A45244A7FF5");
        private static readonly Byte[] q        = Convert.FromHexString("077E3CB613734832ADD09A392A27135A76C45C99095F25C578D1EC42D84B6CEFAD7A96975BAE84D04291809BB828D99803BE4634E40BE655D13C123103326F88D491FF2EB28786ECC31386DB793EA981EE0F78EFF0AD");
        private static readonly Byte[] dP       = Convert.FromHexString("0294A7AE7A7B1FD14B54A2450D40C534E0A38DD1E7C8F446E072C1DE56FC7F42029EE3C3AB79B55149DAF062576BA913247078938203138456DDCB50E8B2790A075EC0CCAAB4D1527EC3B5EABAB1790647E32357ED69");
        private static readonly Byte[] dQ       = Convert.FromHexString("0200377DF71BA57FC2E21D0C7EB1087DF390E39909E39A566CB8C179E7FFDC0AF42BF3E25EDCEF221D64750F1FD1E8A8AAF738100F850DF0847AA1F0EC859F3ACE685DD7BE5C6A2EF6B8541E6EF5C895A55804572DD1");
        private static readonly Byte[] qInv     = Convert.FromHexString("0531B5EA58EBCEFD727D60B34FB58442E5E3AB2813ACE337AAA42AC769A9294FCAA6F4A1D6BB413D2C405ABA36E5BF05F6EB5CA775BEFD3917CA6C6AFEDB59DA29A9EC3F4DD6DB5633441465CF01654C66DF2E5297E6");
        private static readonly Byte[] r3       = Convert.FromHexString("034FD1F20CBE22ED6B924FF7C1C9E82402974866447E3FE1AC70CCD2F81CDA5D23DA0CC1C8B50407CFB6F0AD3F9C35B3F459FB8CE68CB6025BBD758E4F032154FCF1DC218A12E267C326D4CBE701ED76C27C05AC6253");
        private static readonly Byte[] d3       = Convert.FromHexString("013040F8AB6804A2CD6AE975D13EC3274122C761A40CE6A1F73F57D21226509B0245BB3443F1EA2B5F789870A4164C13738A6342D26E06D75BD2E700160CDA86646BEB82C1C93B24DF9CECDAEBE0FE36135F6CADCB6F");
        private static readonly Byte[] t3       = Convert.FromHexString("E792A46FF2BC4C7C0CA3F0516FE2201094C6DEFB78E91B7007C64F45D48D3328172E1AE52C658A801A89B8AD535922EEF2875D149B98CF0F6F5C11E42284F8F9E9BB76A870BA39D963229CE9189D9958CCDDFF3998");

        /// <summary>
        /// The thumbprint COSE.TS computes for the same key [RFC 9679].
        /// </summary>
        private static readonly Byte[] thumbprint = Convert.FromHexString("0DE1843997C89C2947FB72839000074675DB697D68C3595C2AF5184CAABF60A5");

        private static readonly COSEAlgorithm[] algorithms = [
            COSEAlgorithm.PS256, COSEAlgorithm.PS384, COSEAlgorithm.PS512,
            COSEAlgorithm.RS256, COSEAlgorithm.RS384, COSEAlgorithm.RS512
        ];

        #endregion

        #region (private static) Helpers

        private static CBORValue Prime(Byte[] R, Byte[] D, Byte[] T)

            => CBORValue.FromMap([
                   new (COSEKey.RILabel, CBORValue.FromBytes(R)),
                   new (COSEKey.DILabel, CBORValue.FromBytes(D)),
                   new (COSEKey.TILabel, CBORValue.FromBytes(T))
               ]);

        /// <summary>
        /// The key as a CBOR map, with the given value at label -9.
        /// </summary>
        private static CBORValue KeyMap(CBORValue OtherPrimes)

            => CBORValue.FromMap([
                   new (COSEKey.KeyTypeLabel,     CBORValue.FromInt64(3)),
                   new (COSEKey.NLabel,           CBORValue.FromBytes(n)),
                   new (COSEKey.ELabel,           CBORValue.FromBytes(e)),
                   new (COSEKey.RSADLabel,        CBORValue.FromBytes(d)),
                   new (COSEKey.PLabel,           CBORValue.FromBytes(p)),
                   new (COSEKey.QLabel,           CBORValue.FromBytes(q)),
                   new (COSEKey.DPLabel,          CBORValue.FromBytes(dP)),
                   new (COSEKey.DQLabel,          CBORValue.FromBytes(dQ)),
                   new (COSEKey.QInvLabel,        CBORValue.FromBytes(qInv)),
                   new (COSEKey.OtherPrimesLabel, OtherPrimes)
               ]);

        private static COSEKey Key(Byte[]? R = null, Byte[]? D = null, Byte[]? T = null)

            => new (COSEKeyType.RSA,
                    D:                     d,
                    AdditionalParameters:  [ new (COSEKey.OtherPrimesLabel, CBORValue.FromArray([ Prime(R ?? r3, D ?? d3, T ?? t3) ])) ],
                    N:                     n,
                    E:                     e,
                    P:                     p,
                    Q:                     q,
                    DP:                    dP,
                    DQ:                    dQ,
                    QInv:                  qInv);

        private static RSAMultiPrimePrivateKeyParameters PrivateKey()
            => (RSAMultiPrimePrivateKeyParameters) Key().ToPrivateKey();

        /// <summary>
        /// The same key as a bare exponent: the first form of RSASP1.
        /// </summary>
        private static RsaKeyParameters FirstForm()
            => new (true, new BCBigInteger(1, n), new BCBigInteger(1, d));

        private static RSA DotNetPublicKey()
        {
            var rsa = RSA.Create();
            rsa.ImportParameters(new RSAParameters { Modulus = n, Exponent = e });
            return rsa;
        }

        private static (HashAlgorithmName, RSASignaturePadding) DotNet(COSEAlgorithm Algorithm)

            => (Algorithm.HashAlgorithm!.Value,
                Algorithm.Family == COSEAlgorithmFamily.RSAPSS
                    ? RSASignaturePadding.Pss
                    : RSASignaturePadding.Pkcs1);

        #endregion


        #region A_COSE_key_with_three_primes_becomes_a_multi_prime_private_key()

        [Test]
        public void A_COSE_key_with_three_primes_becomes_a_multi_prime_private_key()
        {

            var cbor        = KeyMap(CBORValue.FromArray([ Prime(r3, d3, t3) ])).ToByteArray();
            var key         = COSEKey.Parse(cbor);
            var privateKey  = key.ToPrivateKey();

            Assert.That(privateKey,  Is.InstanceOf<RSAMultiPrimePrivateKeyParameters>());

            var multiPrime  = (RSAMultiPrimePrivateKeyParameters) privateKey;

            Assert.That(multiPrime.OtherPrimes,                Has.Count.EqualTo(1));
            Assert.That(multiPrime.OtherPrimes[0].Prime,       Is.EqualTo(new BCBigInteger(1, r3)));
            Assert.That(multiPrime.OtherPrimes[0].Exponent,    Is.EqualTo(new BCBigInteger(1, d3)));
            Assert.That(multiPrime.OtherPrimes[0].Coefficient, Is.EqualTo(new BCBigInteger(1, t3)));

            // Back to a COSE key, and the very same bytes: -9 after qInv.
            Assert.That(key.ToByteArray(),                     Is.EqualTo(cbor));
            Assert.That(COSEKey.From(privateKey).ToByteArray(),  Is.EqualTo(cbor));

        }

        #endregion

        #region The_public_half_and_the_thumbprint_know_nothing_of_the_further_primes()

        [Test]
        public void The_public_half_and_the_thumbprint_know_nothing_of_the_further_primes()
        {

            var key        = Key();
            var publicKey  = key.ToPublicCOSEKey();

            // RFC 8230, Section 4: for a public key every field but n and e
            // "MUST be absent", and the further primes are factors of n.
            Assert.That(publicKey.AdditionalParameters,  Is.Empty);
            Assert.That(publicKey.ToPublicKey(),         Is.EqualTo(new RsaKeyParameters(false, new BCBigInteger(1, n), new BCBigInteger(1, e))));

            // RFC 9679: kty, n and e - and the thumbprint COSE.TS computes.
            Assert.That(key.Thumbprint(),                Is.EqualTo(thumbprint));
            Assert.That(publicKey.Thumbprint(),          Is.EqualTo(thumbprint));

        }

        #endregion

        #region Every_RSA_algorithm_signs_with_three_primes_what_the_first_form_signs()

        [Test]
        public void Every_RSA_algorithm_signs_with_three_primes_what_the_first_form_signs()
        {

            using var dotNet  = DotNetPublicKey();
            var publicKey     = new RsaKeyParameters(false, new BCBigInteger(1, n), new BCBigInteger(1, e));

            foreach (var algorithm in algorithms)
            {

                // PSS with the deterministic zero salt, so that the bytes compare.
                var byThreePrimes  = algorithm.Sign(content, PrivateKey(), Deterministic: true);
                var byFirstForm    = algorithm.Sign(content, FirstForm(),  Deterministic: true);

                Assert.That(byThreePrimes,                                            Is.EqualTo(byFirstForm),  algorithm.Name);
                Assert.That(algorithm.Verify(content, byThreePrimes, publicKey, out var error),  Is.True,  error);

                var (hash, padding) = DotNet(algorithm);

                Assert.That(dotNet.VerifyData(content, byThreePrimes, hash, padding),  Is.True,  algorithm.Name);

                // And with a random salt, which only a verifier can check.
                if (algorithm.Family == COSEAlgorithmFamily.RSAPSS)
                    Assert.That(dotNet.VerifyData(content, algorithm.Sign(content, PrivateKey()), hash, padding),  Is.True,  algorithm.Name);

            }

        }

        #endregion

        #region A_COSE_Sign1_signed_with_three_primes_verifies_under_n_and_e()

        [Test]
        public void A_COSE_Sign1_signed_with_three_primes_verifies_under_n_and_e()
        {

            var key      = Key();
            var message  = COSESign1.Sign(content, key.ToPrivateKey(), COSEAlgorithm.RS256, KeyIdentifier: "RSA2048/3".ToUTF8Bytes());
            var parsed   = COSESign1.Parse(message.ToByteArray());

            Assert.That(message.Signature.Length,                               Is.EqualTo(256));
            Assert.That(parsed.Verify(key.ToPublicCOSEKey(), out var error),    Is.True,  error);

            var tampered = message.ToByteArray();
            tampered[^1] ^= 0x01;

            Assert.That(COSESign1.Parse(tampered).Verify(key.ToPublicCOSEKey(), out _),  Is.False);

        }

        #endregion


        #region The_multi_prime_computation_is_blinded()

        /// <summary>
        /// Hands every input on to the multi-prime core and remembers it.
        /// </summary>
        private sealed class RecordingCore : IRsa
        {

            private readonly RSAMultiPrimeCore core = new();

            public List<BCBigInteger> Inputs { get; } = [];

            public void         Init(Boolean ForEncryption, ICipherParameters Parameters)  => core.Init(ForEncryption, Parameters);
            public Int32        GetInputBlockSize()                                         => core.GetInputBlockSize();
            public Int32        GetOutputBlockSize()                                        => core.GetOutputBlockSize();
            public BCBigInteger ConvertInput(Byte[] InBuffer, Int32 InOffset, Int32 InLength)  => core.ConvertInput(InBuffer, InOffset, InLength);
            public Byte[]       ConvertOutput(BCBigInteger Result)                          => core.ConvertOutput(Result);

            public BCBigInteger ProcessBlock(BCBigInteger Input)
            {
                Inputs.Add(Input);
                return core.ProcessBlock(Input);
            }

        }

        [Test]
        public void The_multi_prime_computation_is_blinded()
        {

            // Bouncy Castle blinds the private operation of a CRT key only,
            // and this is why the multi-prime key extends one: the core is
            // handed m·r^e, never m, and a different r every time.
            var recorder  = new RecordingCore();
            var engine    = new RsaBlindedEngine(recorder);
            var block     = content;

            engine.Init(true, PrivateKey());

            var first     = engine.ProcessBlock(block, 0, block.Length);
            var second    = engine.ProcessBlock(block, 0, block.Length);

            var plain     = new RsaEngine();
            plain.Init(true, FirstForm());

            Assert.That(first,                Is.EqualTo(plain.ProcessBlock(block, 0, block.Length)));
            Assert.That(second,               Is.EqualTo(first));

            Assert.That(recorder.Inputs,      Has.Count.EqualTo(2));
            Assert.That(recorder.Inputs[0],   Is.Not.EqualTo(new BCBigInteger(1, block)));
            Assert.That(recorder.Inputs[1],   Is.Not.EqualTo(recorder.Inputs[0]));

        }

        #endregion

        #region A_faulty_multi_prime_result_is_withheld()

        [Test]
        public void A_faulty_multi_prime_result_is_withheld()
        {

            // A t_i that does not belong to p·q, past the checks of COSEKey:
            // the CRT result is wrong modulo r_3 alone, and would give r_3 away.
            var key     = PrivateKey();
            var faulty  = new RSAMultiPrimePrivateKeyParameters(key.Modulus, key.PublicExponent, key.Exponent,
                                                                key.P, key.Q, key.DP, key.DQ, key.QInv,
                                                                [ key.OtherPrimes[0] with { Coefficient = key.OtherPrimes[0].Coefficient.Add(BCBigInteger.One) } ]);

            Assert.That(() => COSEAlgorithm.RS256.Sign(content, faulty),
                        Throws.InstanceOf<CryptoException>().With.Message.Contains("withheld"));

        }

        #endregion

        #region Bouncy_Castles_own_core_does_not_release_a_two_prime_result()

        [Test]
        public void Bouncy_Castles_own_core_does_not_release_a_two_prime_result()
        {

            // The reason the multi-prime key extends the two-prime one is safe:
            // an engine that knows two primes computes modulo p and q only, and
            // Bouncy Castle checks its result before it lets go of it.
            var signer = new RsaDigestSigner(new Sha256Digest());

            signer.Init(true, PrivateKey());
            signer.BlockUpdate(content, 0, content.Length);

            Assert.That(() => signer.GenerateSignature(),
                        Throws.Exception.With.Message.Contains("faulty"));

        }

        #endregion


        #region Malformed_further_primes_are_refused()

        [Test]
        public void Malformed_further_primes_are_refused()
        {

            void Refused(CBORValue OtherPrimes, String Expected)
            {
                Assert.That(COSEKey.TryParse(KeyMap(OtherPrimes), out _, out var error),  Is.False,  Expected);
                Assert.That(error,                                                       Does.Contain(Expected));
            }

            Refused(CBORValue.FromBytes(r3),                     "must be an array of maps");
            Refused(CBORValue.FromArray([]),                     "at least one prime");
            Refused(CBORValue.FromArray([ CBORValue.FromBytes(r3) ]),  "the entry of prime 3 is not one");

            Refused(CBORValue.FromArray([ CBORValue.FromMap([
                        new (COSEKey.RILabel, CBORValue.FromBytes(r3)),
                        new (COSEKey.DILabel, CBORValue.FromBytes(d3))
                    ]) ]),
                    "the map of prime 3 carries no CRT coefficient t_i");

            Refused(CBORValue.FromArray([ Prime([ 0x00, .. r3 ], d3, t3) ]),
                    "prime factor r_i of prime 3 of an RSA key must be encoded in the minimum number of octets");

        }

        #endregion

        #region Further_primes_that_do_not_belong_to_the_key_are_refused()

        [Test]
        public void Further_primes_that_do_not_belong_to_the_key_are_refused()
        {

            void Refused(COSEKey Key, String Expected)
            {
                Assert.That(Key.TryToPrivateKey(out _, out var error),  Is.False,  Expected);
                Assert.That(error,                                     Does.Contain(Expected));
            }

            var other = new BCBigInteger(1, r3);

            Refused(Key(R: other.Add(BCBigInteger.Two).ToByteArrayUnsigned()),   "The 3 prime factors of the multi-prime RSA key are not the factors of its modulus n");
            Refused(Key(D: new BCBigInteger(1, d3).Add(BCBigInteger.Two).ToByteArrayUnsigned()),  "d_i and t_i of prime 3");
            Refused(Key(T: new BCBigInteger(1, t3).Add(BCBigInteger.One).ToByteArrayUnsigned()),  "d_i and t_i of prime 3");

            // Read as a key of two primes, it is still refused as one.
            var asTwo = new COSEKey(COSEKeyType.RSA, D: d, N: n, E: e, P: p, Q: q, DP: dP, DQ: dQ, QInv: qInv);

            Refused(asTwo,  "The prime factors p and q of the RSA key are not the factors of its modulus n");

        }

        #endregion

    }

}
