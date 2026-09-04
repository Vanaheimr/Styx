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
using System.Security.Cryptography.X509Certificates;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// Extension methods for certificates.
    /// </summary>
    public static class CertificateExtensions
    {

        #region (private) SubjectAlternativeNameExtension(this Certificate)

        /// <summary>
        /// OID 2.5.29.17 — id-ce-subjectAltName.
        /// </summary>
        private const String SubjectAlternativeNameOID = "2.5.29.17";

        /// <summary>
        /// The certificate's Subject Alternative Name extension, decoded, or null when it has
        /// none or the extension cannot be read.
        /// </summary>
        public static X509SubjectAlternativeNameExtension? SubjectAlternativeNameExtension(this X509Certificate2 Certificate)
        {

            var extension = Certificate.Extensions.FirstOrDefault(extension => extension.Oid?.Value == SubjectAlternativeNameOID);

            if (extension is null)
                return null;

            try
            {
                return new X509SubjectAlternativeNameExtension(
                           extension.RawData,
                           extension.Critical
                       );
            }
            catch (CryptographicException)
            {
                // A certificate can carry a malformed extension. Callers asking "which names?"
                // are better served by "none" than by an exception from a property read.
                return null;
            }

        }

        #endregion

        #region GetDNSNames                   (this Certificate)

        /// <summary>
        /// The DNS Name entries of the certificate's Subject Alternative Name extension.
        /// </summary>
        /// <param name="Certificate">A certificate.</param>
        public static IEnumerable<String> GetDNSNames(this X509Certificate2 Certificate)

            => Certificate.SubjectAlternativeNameExtension()?.EnumerateDnsNames() ?? [];

        #endregion

        #region GetIPAddresses                (this Certificate)

        /// <summary>
        /// The IP Address entries of the certificate's Subject Alternative Name extension.
        /// </summary>
        /// <param name="Certificate">A certificate.</param>
        public static IEnumerable<System.Net.IPAddress> GetIPAddresses(this X509Certificate2 Certificate)

            => Certificate.SubjectAlternativeNameExtension()?.EnumerateIPAddresses() ?? [];

        #endregion

        #region DecodeSubjectAlternativeNames (this Certificate)

        /// <summary>
        /// The certificate's subject alternative names, as "DNS-Name=..." and "IP-Address=..."
        /// strings.
        ///
        /// Prefer <see cref="GetDNSNames"/> or <see cref="GetIPAddresses"/>: they return the
        /// values themselves, and cannot be misread.
        ///
        /// This used to render the extension with AsnEncodedData.Format(), which delegates to
        /// the operating system — CryptFormatObject on Windows — and is localized. The same
        /// certificate produced "DNS-Name=" on a German installation and "DNS Name=" on an
        /// English one, so any caller matching on the prefix worked only on the machine it was
        /// written on. Norn's NTS-KE hostname verification did exactly that and silently found
        /// no names anywhere else. The prefixes below are now written here, in one language,
        /// whatever the host is set to.
        /// </summary>
        /// <param name="Certificate">A certificate.</param>
        public static IEnumerable<String> DecodeSubjectAlternativeNames(this X509Certificate2 Certificate)
        {

            var extension = Certificate.SubjectAlternativeNameExtension();

            if (extension is null)
                return [];

            return [
                       .. extension.EnumerateDnsNames().   Select(dnsName   => $"DNS-Name={dnsName}"),
                       .. extension.EnumerateIPAddresses().Select(ipAddress => $"IP-Address={ipAddress}")
                   ];

        }

        #endregion

    }

}
