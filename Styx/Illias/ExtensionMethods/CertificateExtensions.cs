/*
 * Copyright (c) 2010-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of Illias <https://www.github.com/Vanaheimr/Illias>
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
using System.Security.Cryptography.X509Certificates;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// Extensions for certificates.
    /// </summary>
    public static class CertificateExtensions
    {

        #region DecodeSubjectAlternativeNames(this Certificate)

        public static IEnumerable<String> DecodeSubjectAlternativeNames(this X509Certificate2 Certificate)
        {

            foreach (var extension in Certificate.Extensions)
            {

                // OID for Subject Alternative Name
                if (extension.Oid?.Value == "2.5.29.17")
                    return new AsnEncodedData(extension.Oid, extension.RawData).Format(true).Split("\r\n", StringSplitOptions.RemoveEmptyEntries);

            }

            return [];

        }

        #endregion

    }

}
