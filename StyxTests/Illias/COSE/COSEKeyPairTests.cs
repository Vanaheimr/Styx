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

using NUnit.Framework;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias.Tests
{

    /// <summary>
    /// The key pairs of COSEKeyPair: one for every signature algorithm this
    /// library knows, each of which signs with its algorithm and verifies.
    /// </summary>
    [TestFixture]
    public class COSEKeyPairTests
    {

        private static readonly Byte[] content = "This is the content.".ToUTF8Bytes();

        // RSA keys of 2048 bits, the smallest allowed, as they take the longest.
        private static readonly Func<COSEKeyPair>[] factories = [
            () => COSEKeyPair.NewES256(),
            () => COSEKeyPair.NewES384(),
            () => COSEKeyPair.NewES512(),
            () => COSEKeyPair.NewES256K(),
            () => COSEKeyPair.NewESP256(),
            () => COSEKeyPair.NewESP384(),
            () => COSEKeyPair.NewESP512(),
            () => COSEKeyPair.NewESB256(),
            () => COSEKeyPair.NewESB320(),
            () => COSEKeyPair.NewESB384(),
            () => COSEKeyPair.NewESB512(),
            () => COSEKeyPair.NewEdDSA(),
            () => COSEKeyPair.NewEd25519(),
            () => COSEKeyPair.NewEd448(),
            () => COSEKeyPair.NewMLDsa44(),
            () => COSEKeyPair.NewMLDsa65(),
            () => COSEKeyPair.NewMLDsa87(),
            () => COSEKeyPair.NewPS256(KeySizeInBits: 2048),
            () => COSEKeyPair.NewPS384(KeySizeInBits: 2048),
            () => COSEKeyPair.NewPS512(KeySizeInBits: 2048),
            () => COSEKeyPair.NewRS256(KeySizeInBits: 2048),
            () => COSEKeyPair.NewRS384(KeySizeInBits: 2048),
            () => COSEKeyPair.NewRS512(KeySizeInBits: 2048)
        ];


        #region Every_signature_algorithm_has_a_key_pair()

        [Test]
        public void Every_signature_algorithm_has_a_key_pair()
        {

            var algorithms = factories.Select(factory => factory().Algorithm).ToArray();

            Assert.That(algorithms,  Is.Unique);
            Assert.That(algorithms,  Is.EquivalentTo(COSEAlgorithm.All.Where(algorithm => algorithm.IsSupportedForSigning)));

        }

        #endregion

        #region Every_key_pair_signs_with_its_algorithm_and_verifies()

        [Test]
        public void Every_key_pair_signs_with_its_algorithm_and_verifies()
        {

            foreach (var factory in factories)
            {

                var keyPair    = factory();
                var signature  = keyPair.Algorithm.Sign(content, keyPair.Private);

                Assert.That(keyPair.Private.IsPrivate,  Is.True,   keyPair.Algorithm.Name);
                Assert.That(keyPair.Public. IsPrivate,  Is.False,  keyPair.Algorithm.Name);
                Assert.That(keyPair.Algorithm.Verify(content, signature, keyPair.Public, out var error),  Is.True,  $"{keyPair.Algorithm.Name}: {error}");

                // ...and the same through a COSE key.
                var key        = COSEKey.From(keyPair.Private, Algorithm: keyPair.Algorithm);

                Assert.That(COSESign1.Sign(content, key).Verify(key.ToPublicCOSEKey(), out error),  Is.True,  $"{keyPair.Algorithm.Name}: {error}");

            }

        }

        #endregion

        #region RSA_keys_below_2048_bits_are_not_made()

        [Test]
        public void RSA_keys_below_2048_bits_are_not_made()
        {

            Assert.That(() => COSEKeyPair.NewPS256(KeySizeInBits: 1024),
                        Throws.TypeOf<COSEException>().With.Message.Contains("at least 2048 bits"));

        }

        #endregion

    }

}
