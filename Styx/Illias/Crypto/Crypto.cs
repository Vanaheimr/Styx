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

using Newtonsoft.Json.Linq;

using Org.BouncyCastle.Math;
using Org.BouncyCastle.X509;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{


    public static class Crypto
    {


        #region (static) GenerateKeys(ECParameters)

        public static AsymmetricCipherKeyPair GenerateKeys(X9ECParameters ECParameters)
        {

            var ellipticCurveSpec = new ECDomainParameters(
                                        ECParameters.Curve,
                                        ECParameters.G,
                                        ECParameters.N,
                                        ECParameters.H,
                                        ECParameters.GetSeed()
                                    );

            var g = GeneratorUtilities.GetKeyPairGenerator("ECDH");
            g.Init(new ECKeyGenerationParameters(ellipticCurveSpec, new SecureRandom()));

            return g.GenerateKeyPair();

        }

        #endregion


        #region (static) SerializePrivateKey (PrivateKey)

        public static Byte[] SerializePrivateKey(ECPrivateKeyParameters PrivateKey)
            => PrivateKey.D.ToByteArray();

        #endregion

        #region (static) SerializePublicKey  (PublicKey)

        public static Byte[] SerializePublicKey(ECPublicKeyParameters PublicKey)

            => PublicKey.Q.GetEncoded();

        #endregion

        #region (static) SerializePublicKeyXY(PublicKey)

        public static Tuple<Byte[], Byte[]> SerializePublicKeyXY(ECPublicKeyParameters PublicKey)

            => new (PublicKey.Q.XCoord.ToBigInteger().ToByteArray(),
                    PublicKey.Q.YCoord.ToBigInteger().ToByteArray());

        #endregion


        #region (static) ParsePrivateKeyBytes (ECParameters,      PrivateKeyBytes)

        public static ECPrivateKeyParameters ParsePrivateKeyBytes(X9ECParameters  ECParameters,
                                                                  Byte[]          PrivateKeyBytes)

            => ParsePrivateKeyBytes(new ECDomainParameters(ECParameters.Curve,
                                                           ECParameters.G,
                                                           ECParameters.N,
                                                           ECParameters.H,
                                                           ECParameters.GetSeed()),
                                    PrivateKeyBytes);

        #endregion

        #region (static) ParsePrivateKeyBytes (EllipticCurveSpec, PrivateKeyBytes)

        public static ECPrivateKeyParameters ParsePrivateKeyBytes(ECDomainParameters  EllipticCurveSpec,
                                                                  Byte[]              PrivateKeyBytes)

            => new (new BigInteger(PrivateKeyBytes),
                    EllipticCurveSpec);


        #endregion

        #region (static) ParsePrivateKeyHEX   (ECParameters,      PrivateKeyHEX)

        public static ECPrivateKeyParameters ParsePrivateKeyHEX(X9ECParameters  ECParameters,
                                                         String          PrivateKeyHEX)

            => ParsePrivateKeyHEX(new ECDomainParameters(ECParameters.Curve,
                                                         ECParameters.G,
                                                         ECParameters.N,
                                                         ECParameters.H,
                                                         ECParameters.GetSeed()),
                                  PrivateKeyHEX);

        #endregion

        #region (static) ParsePrivateKeyHEX   (EllipticCurveSpec, PrivateKeyHEX)

        public static ECPrivateKeyParameters ParsePrivateKeyHEX(ECDomainParameters  EllipticCurveSpec,
                                                                String              PrivateKeyHEX)

            => new (new BigInteger(PrivateKeyHEX, 16),
                    EllipticCurveSpec);


        #endregion

        #region (static) ParsePrivateKeyBase64(ECParameters,      PrivateKeyBase64)

        public static ECPrivateKeyParameters ParsePrivateKeyBase64(X9ECParameters  ECParameters,
                                                                   String          PrivateKeyBase64)

            => ParsePrivateKeyBase64(new ECDomainParameters(
                                         ECParameters.Curve,
                                         ECParameters.G,
                                         ECParameters.N,
                                         ECParameters.H,
                                         ECParameters.GetSeed()
                                     ),
                                     PrivateKeyBase64);

        #endregion

        #region (static) ParsePrivateKeyBase64(EllipticCurveSpec, PrivateKeyBase64)

        public static ECPrivateKeyParameters ParsePrivateKeyBase64(ECDomainParameters  EllipticCurveSpec,
                                                                   String              PrivateKeyBase64)

            => new (new BigInteger(PrivateKeyBase64.FromBASE64()),
                    EllipticCurveSpec);

        #endregion


        #region (static) ParsePublicKey       (ECParameters,      PublicKey)

        public static ECPublicKeyParameters ParsePublicKey(X9ECParameters  ECParameters,
                                                           Byte[]          PublicKey)

            => new ("ECDSA",
                    ECParameters.Curve.DecodePoint(PublicKey),
                    new ECDomainParameters(
                        ECParameters.Curve,
                        ECParameters.G,
                        ECParameters.N,
                        ECParameters.H,
                        ECParameters.GetSeed()
                    )
                   );

        #endregion

        #region (static) ParsePublicKey       (EllipticCurveSpec, PublicKey)

        public static  ECPublicKeyParameters ParsePublicKey(ECDomainParameters  EllipticCurveSpec,
                                                            Byte[]              PublicKey)

            => new ("ECDSA",
                    EllipticCurveSpec.Curve.DecodePoint(PublicKey),
                    EllipticCurveSpec);

        #endregion

        #region (static) ParsePublicKeyHEX    (ECParameters,      PublicKeyHEX)

        public static ECPublicKeyParameters ParsePublicKeyHEX(X9ECParameters ECParameters,
                                                              String         PublicKeyHEX)

            => new ("ECDSA",
                    ECParameters.Curve.DecodePoint(PublicKeyHEX.FromHEX()),
                    new ECDomainParameters(
                        ECParameters.Curve,
                        ECParameters.G,
                        ECParameters.N,
                        ECParameters.H,
                        ECParameters.GetSeed()
                    )
                   );

        #endregion

        #region (static) ParsePublicKeyHEX    (EllipticCurveSpec, PublicKeyHEX)

        public static ECPublicKeyParameters ParsePublicKeyHEX(ECDomainParameters  EllipticCurveSpec,
                                                              String              PublicKeyHEX)

            => new ("ECDSA",
                    EllipticCurveSpec.Curve.DecodePoint(PublicKeyHEX.FromHEX()),
                    EllipticCurveSpec);

        #endregion

        #region (static) ParsePublicKeyBase64 (ECParameters,      PublicKeyBase64)

        public static ECPublicKeyParameters ParsePublicKeyBase64(X9ECParameters ECParameters,
                                                                 String         PublicKeyBase64)

            => new ("ECDSA",
                    ECParameters.Curve.DecodePoint(PublicKeyBase64.FromBASE64()),
                    new ECDomainParameters(
                        ECParameters.Curve,
                        ECParameters.G,
                        ECParameters.N,
                        ECParameters.H,
                        ECParameters.GetSeed()
                    ));

        #endregion

        #region (static) ParsePublicKeyBase64 (EllipticCurveSpec, PublicKeyBase64)

        public static ECPublicKeyParameters ParsePublicKeyBase64(ECDomainParameters  EllipticCurveSpec,
                                                                 String              PublicKeyBase64)

            => new ("ECDSA",
                    EllipticCurveSpec.Curve.DecodePoint(PublicKeyBase64.FromBASE64()),
                    EllipticCurveSpec);

        #endregion


        #region (static) CalculatePublicKey(PrivateKey)

        /// <summary>
        /// Calculate the public key only using domainParams.getG() and private key.
        /// </summary>
        /// <param name="PrivateKey"></param>
        public static ECPublicKeyParameters CalculatePublicKey(ECPrivateKeyParameters PrivateKey)

            => new ("ECDSA",
                    PrivateKey.Parameters.Curve.DecodePoint(
                        PrivateKey.Parameters.G.Multiply(
                            new BigInteger(PrivateKey.D.ToByteArray()
                        )
                    ).GetEncoded()),
                    PrivateKey.Parameters);

        #endregion


        #region (static) VerifyMessageSignatures(JSONMessage, AllMustBeValid = true)

        public static Boolean VerifyMessageSignatures(JObject  JSONMessage,
                                                      Boolean  AllMustBeValid   = true)
        {

            if (JSONMessage is null)
                return false;

            if (JSONMessage["signatures"] is not JArray signaturesJSON ||
                signaturesJSON.Type != JTokenType.Array ||
                signaturesJSON.Count < 1)
            {
                return false;
            }

            try
            {

                var JSONMessageCopy    = JObject.Parse(JSONMessage.ToString(Newtonsoft.Json.Formatting.None));
                JSONMessageCopy.Remove("signatures");
                var canonicalPlainText  = CanonicalJSON.ToUTF8Bytes(JSONMessageCopy);
                var legacyPlainText     = JSONMessageCopy.ToString(Newtonsoft.Json.Formatting.None).ToUTF8Bytes();

                var results            = new List<Boolean>();

                foreach (var signatureJSON in signaturesJSON)
                {

                    if (signatureJSON is not JObject ||
                        signatureJSON.Type != JTokenType.Object)
                    {
                        results.Add(false);
                        continue;
                    }

                    var publicKey  = signatureJSON["publicKey"]?.Value<String>()?.FromBASE64();
                    var signature  = signatureJSON["signature"]?.Value<String>()?.FromBASE64();

                    if (publicKey is null     ||
                        signature is null     ||
                        publicKey.Length == 0 ||
                        signature.Length == 0)
                    {
                        results.Add(false);
                        continue;
                    }


                    //Byte[] pubKey = publicKey;
                    //var aa = new X509EncodedKeySpec(signaturePublicKey);
                    //var input = new Asn1InputStream(signaturePublicKey);

                    //Byte[] pubKey = null;

                    //Asn1Object p;
                    //while ((p = input.ReadObject()) is not null)
                    //{
                    //    pubKey = ((p.ToAsn1Object() as Asn1Sequence)[1] as DerBitString).GetBytes();
                    //    Console.WriteLine(p.ToString());
                    //}

                    var ecp           = SecNamedCurves.GetByName("secp256r1");
                    var ecParams      = new ECDomainParameters(ecp.Curve, ecp.G, ecp.N, ecp.H, ecp.GetSeed());
                    var pubKeyParams  = new ECPublicKeyParameters("ECDSA", ecParams.Curve.DecodePoint(publicKey), ecParams);

                    var verifier      = SignerUtilities.GetSigner("NONEwithECDSA");
                    verifier.Init(false, pubKeyParams);
                    var sha256Hash    = SHA256.HashData(canonicalPlainText);
                    verifier.BlockUpdate(sha256Hash);
                    var result        = verifier.VerifySignature(signature);

                    if (!result)
                    {
                        verifier      = SignerUtilities.GetSigner("NONEwithECDSA");
                        verifier.Init(false, pubKeyParams);
                        sha256Hash    = SHA256.HashData(legacyPlainText);
                        verifier.BlockUpdate(sha256Hash);
                        result        = verifier.VerifySignature(signature);
                    }

                    results.Add(result);

                }

                return AllMustBeValid
                           ? results.All(result => result)
                           : results.Any(result => result);

            }
            catch
            {
                return false;
            }

        }

        #endregion

        #region (static) SignMessage(JSONMessage, params KeyPairs)

        public static Boolean SignMessage(JObject JSONMessage, params AsymmetricCipherKeyPair[] KeyPairs)
        {

            if (JSONMessage is null || KeyPairs is null || KeyPairs.Length == 0)
                return false;

            foreach (var KeyPair in KeyPairs)
            {

                if (KeyPair is null)
                    continue;

                if (KeyPair?.Private is not ECPrivateKeyParameters privateKey)
                    continue;

                if (KeyPair?.Public  is not ECPublicKeyParameters  publicKey)
                    continue;

                if (JSONMessage["signatures"] is not null &&
                    JSONMessage["signatures"]?.Type != JTokenType.Array)
                {
                    return false;
                }

                var messageJSON  = JObject.Parse(JSONMessage.ToString(Newtonsoft.Json.Formatting.None));
                messageJSON.Remove("signatures");

                var plainText    = CanonicalJSON.Serialize(messageJSON);
                var sha256Hash   = SHA256.HashData(plainText.ToUTF8Bytes());

                if (JSONMessage["signatures"] is not JArray signaturesJSON)
                {
                    signaturesJSON = [];
                    JSONMessage.Add("signatures", signaturesJSON);
                }

                var signatureJSON = new JObject();
                signaturesJSON.Add(signatureJSON);


                var publicKey_Bytes = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(publicKey).PublicKey.GetBytes();
                signatureJSON.Add(new JProperty("publicKey",     Convert.ToBase64String(publicKey_Bytes)));
                signatureJSON.Add(new JProperty("publicKeyHEX",  publicKey_Bytes.ToHexString()));

                var signer       = SignerUtilities.GetSigner("NONEwithECDSA");
                signer.Init(true, privateKey);
                signer.BlockUpdate(sha256Hash);
                var signature    = signer.GenerateSignature();
                signatureJSON.Add(new JProperty("signature",    Convert.ToBase64String(signature)));
                signatureJSON.Add(new JProperty("signatureHEX", signature.ToHexString()));


                DebugX.Log("Response: "  + JSONMessage.ToString(Newtonsoft.Json.Formatting.None));
                DebugX.Log("PlainText: " + plainText);
                DebugX.Log("sha256: "    + sha256Hash.ToHexString());

                //// Re-Verify...
                //{
                //    var verifier = SignerUtilities.GetSigner("NONEwithECDSA");
                //    verifier.Init(false, publicKey);
                //    verifier.BlockUpdate(SHA256Hash, 0, BlockSize);
                //    Console.WriteLine("Signature Verification(1): " + (verifier.VerifySignature(signature) ? "ok" : "failed!"));
                //}

                {
                    var ecp           = SecNamedCurves.GetByName("secp256r1");
                    var ecParams      = new ECDomainParameters(ecp.Curve, ecp.G, ecp.N, ecp.H, ecp.GetSeed());
                    var pubKeyParams  = new ECPublicKeyParameters("ECDSA", ecParams.Curve.DecodePoint(publicKey_Bytes), ecParams);
                    var verifier      = SignerUtilities.GetSigner("NONEwithECDSA");
                    verifier.Init(false, pubKeyParams);
                    verifier.BlockUpdate(sha256Hash);
                    DebugX.Log("Signature Verification(2): " + (verifier.VerifySignature(signature) ? "ok" : "failed!"));
                }

            }

            return true;

        }

        #endregion


        //// key agreement protocol => ConcatenationKDFGenerator?
        //var keyAgreement  = AgreementUtilities.GetBasicAgreement("ECDH");
        //keyAgreement.Init(backendAPIPrivateKey);
        //var sharedSecret  = keyAgreement.CalculateAgreement(ownerPublicKey).
        //                                 ToByteArrayUnsigned();

        //var sha256Digest  = DigestUtilities.GetDigest("SHA256");
        //var keySize       = 32; // sha256Digest.GetDigestSize()
        //var kdf           = new ECDHKekGenerator(sha256Digest);
        //kdf.Init(new DHKdfParameters(NistObjectIdentifiers.Aes,
        //                             sharedSecret.Length,
        //                             sharedSecret));
        //var symmetricKey  = new Byte[keySize];
        //kdf.GenerateBytes(symmetricKey, 0, keySize);
        //var bigInt        = new BigInteger(1, symmetricKey);


    }

}
