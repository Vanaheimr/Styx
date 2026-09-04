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
using Org.BouncyCastle.Utilities;
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

        /// <summary>
        /// Serialize the given private key as an unsigned byte array,
        /// zero-padded to the width of the group order of its elliptic curve:
        /// 32 bytes on P-256, 66 bytes on P-521.
        /// The fixed width is not cosmetic. Leading zero octets MUST be
        /// preserved (RFC 9053, Section 7.1.1 says so for COSE keys, JWK,
        /// PKCS#11 and the metering formats of the charging infrastructure
        /// require the same), and a signed two's complement serialization -
        /// which is what BigInteger.ToByteArray returns - both strips them and
        /// prepends a zero byte whenever the leading bit is set.
        /// </summary>
        /// <param name="PrivateKey">An elliptic curve private key.</param>
        public static Byte[] SerializePrivateKey(ECPrivateKeyParameters PrivateKey)

            => BigIntegers.AsUnsignedByteArray(
                   (PrivateKey.Parameters.N.BitLength + 7) / 8,
                   PrivateKey.D
               );

        #endregion

        #region (static) SerializePublicKey  (PublicKey)

        /// <summary>
        /// Serialize the given public key as an uncompressed elliptic curve
        /// point: The octet 0x04 followed by both coordinates, each of them
        /// as wide as a field element (SEC 1, Section 2.3.3).
        /// </summary>
        /// <param name="PublicKey">An elliptic curve public key.</param>
        public static Byte[] SerializePublicKey(ECPublicKeyParameters PublicKey)

            => PublicKey.Q.GetEncoded();

        #endregion

        #region (static) SerializePublicKeyXY(PublicKey)

        /// <summary>
        /// Serialize the two affine coordinates of the given public key,
        /// each as an unsigned byte array of the width of a field element:
        /// 32 bytes on P-256, 66 bytes on P-521.
        /// As with the private key, the leading zero octets are part of the
        /// encoding and must not be stripped.
        /// </summary>
        /// <param name="PublicKey">An elliptic curve public key.</param>
        public static Tuple<Byte[], Byte[]> SerializePublicKeyXY(ECPublicKeyParameters PublicKey)
        {

            // A point that was computed rather than decoded may still be in
            // projective coordinates, where XCoord is NOT the affine x.
            var point = PublicKey.Q.Normalize();

            return new (point.AffineXCoord.GetEncoded(),
                        point.AffineYCoord.GetEncoded());

        }

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

        /// <summary>
        /// Parse the given unsigned byte array as an elliptic curve private key.
        /// The bytes are read as an unsigned magnitude, as every standard
        /// encoding of a private key is: Reading them as signed two's
        /// complement - which is what the plain BigInteger(Byte[])
        /// constructor of Bouncy Castle does - would turn every key whose
        /// leading bit is set, roughly one in two, into a negative number and
        /// thus into a different key, silently.
        /// The width has to be exactly the width of the group order and the
        /// value has to lie within it. Key material that arrives without its
        /// leading zeroes, with a two's complement sign byte in front of it,
        /// or outside the group is malformed, and saying so is far more
        /// useful than quietly repairing it into something that signs.
        /// This is the one implementation all three representations go
        /// through, so that they can not drift apart again.
        /// </summary>
        /// <param name="EllipticCurveSpec">The elliptic curve domain parameters.</param>
        /// <param name="PrivateKeyBytes">The private key as an unsigned byte array.</param>
        public static ECPrivateKeyParameters ParsePrivateKeyBytes(ECDomainParameters  EllipticCurveSpec,
                                                                  Byte[]              PrivateKeyBytes)
        {

            var orderSizeInBytes = (EllipticCurveSpec.N.BitLength + 7) / 8;

            if (PrivateKeyBytes.Length != orderSizeInBytes)
                throw new ArgumentException($"An elliptic curve private key on this curve must be {orderSizeInBytes} bytes wide, including leading zeroes, but was {PrivateKeyBytes.Length} bytes wide!",
                                            nameof(PrivateKeyBytes));

            var d = new BigInteger(1, PrivateKeyBytes);

            if (d.SignValue <= 0 || d.CompareTo(EllipticCurveSpec.N) >= 0)
                throw new ArgumentException("The given elliptic curve private key is not within the group order of its elliptic curve!",
                                            nameof(PrivateKeyBytes));

            return new (d, EllipticCurveSpec);

        }

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

        /// <summary>
        /// Parse the given hexadecimal private key.
        /// The digits are decoded and then read exactly as within
        /// ParsePrivateKeyBytes, which is the point: This used to be its own
        /// implementation, and it disagreed with the byte representation of
        /// one and the same key for every key whose leading bit was set.
        /// </summary>
        /// <param name="EllipticCurveSpec">The elliptic curve domain parameters.</param>
        /// <param name="PrivateKeyHEX">The private key as a hexadecimal unsigned byte array.</param>
        public static ECPrivateKeyParameters ParsePrivateKeyHEX(ECDomainParameters  EllipticCurveSpec,
                                                                String              PrivateKeyHEX)

            => ParsePrivateKeyBytes(EllipticCurveSpec,
                                    PrivateKeyHEX.FromHEX());

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

        /// <summary>
        /// Parse the given base64 encoded private key.
        /// The decoded bytes are read exactly as within ParsePrivateKeyBytes.
        /// </summary>
        /// <param name="EllipticCurveSpec">The elliptic curve domain parameters.</param>
        /// <param name="PrivateKeyBase64">The private key as a base64 encoded unsigned byte array.</param>
        public static ECPrivateKeyParameters ParsePrivateKeyBase64(ECDomainParameters  EllipticCurveSpec,
                                                                   String              PrivateKeyBase64)

            => ParsePrivateKeyBytes(EllipticCurveSpec,
                                    PrivateKeyBase64.FromBASE64());

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
