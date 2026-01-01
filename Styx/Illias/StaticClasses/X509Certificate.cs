/*
 * Copyright (c) 2010-2026 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

using System.Security.Cryptography.X509Certificates;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// X.509 certificate loading extensions.
    /// </summary>
    public static class Certificate
    {

        /// <summary>
        /// Will load a X.509 certificate from the given PEM encoded files
        /// within the base directory of the application and
        /// take care of Windows specific issues.
        /// </summary>
        /// <param name="CertificatePEMFile">The path to the PEM encoded certificate file.</param>
        /// <param name="PrivateKeyPEMFile">The path to the PEM encoded private key file.</param>
        public static X509Certificate2 LoadFromBaseDirectory(String CertificatePEMFile,
                                                             String PrivateKeyPEMFile)

             => LoadPEM(
                    Path.Combine(AppContext.BaseDirectory, CertificatePEMFile),
                    Path.Combine(AppContext.BaseDirectory, PrivateKeyPEMFile)
                );


        /// <summary>
        /// Will load a X.509 certificate from the given PEM encoded files
        /// and take care of Windows specific issues.
        /// </summary>
        /// <param name="CertificatePEMFile">The path to the PEM encoded certificate file.</param>
        /// <param name="PrivateKeyPEMFile">The path to the PEM encoded private key file.</param>
        public static X509Certificate2 LoadPEM(String CertificatePEMFile,
                                               String PrivateKeyPEMFile)
        {

             var certificate  = X509Certificate2.CreateFromPemFile(
                                    CertificatePEMFile,
                                    PrivateKeyPEMFile
                                );

            if (!OperatingSystem.IsWindows())
                return certificate;

            // Because Windows is stupid and will generate strange errors otherwise...
            var pfxBytes = certificate.Export(X509ContentType.Pfx);

            return X509CertificateLoader.LoadPkcs12(
                       pfxBytes,
                       null,
                       X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.Exportable
                   );

        }

    }

}
