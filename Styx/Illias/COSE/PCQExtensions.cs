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
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Security;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// Extension methods for generating ML-DSA key material from a selected parameter set.
    /// </summary>
    public static class PCQExtensions
    {

        #region NewMLDSAKeyPair (this ParameterSet, SecureRandom = null)

        /// <summary>
        /// Generates a new ML-DSA private key for the specified parameter set.
        /// </summary>
        /// <remarks>Uses a cryptographically secure random source during key generation.</remarks>
        /// <param name="ParameterSet">The ML-DSA parameter set to use for key generation.</param>
        public static AsymmetricCipherKeyPair NewMLDSAKeyPair(this MLDsaParameters  ParameterSet,
                                                              SecureRandom?         SecureRandom   = null)
        {

            var generator = new MLDsaKeyPairGenerator();

            generator.Init(
                new MLDsaKeyGenerationParameters(
                    SecureRandom ?? new SecureRandom(),
                    ParameterSet
                )
            );

            return generator.GenerateKeyPair();

        }

        #endregion

        #region NewPrivateKey   (this ParameterSet, SecureRandom = null)

        /// <summary>
        /// Generates a new ML-DSA private key for the specified parameter set.
        /// </summary>
        /// <remarks>Uses a cryptographically secure random source during key generation.</remarks>
        /// <param name="ParameterSet">The ML-DSA parameter set to use for key generation.</param>
        public static MLDsaPrivateKeyParameters NewPrivateKey(this MLDsaParameters  ParameterSet,
                                                              SecureRandom?         SecureRandom   = null)

            => (MLDsaPrivateKeyParameters) ParameterSet.NewMLDSAKeyPair(SecureRandom).Private;

        #endregion

    }

}
