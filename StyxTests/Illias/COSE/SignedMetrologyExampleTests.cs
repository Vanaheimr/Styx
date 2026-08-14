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

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias.Tests
{

    /// <summary>
    /// The worked example of Styx/Illias/CBOR/tag-44252-signed-example.md:
    /// a charging transaction as a signed metrological record, from the meter
    /// through the charging station to the customer.
    ///
    /// Everything below is derived from ONE constant, the 713 bytes the
    /// customer receives, and from the three published public keys. That
    /// every signature within it verifies is what makes the document a worked
    /// example rather than an illustration - and it is why the document may
    /// be regenerated but never retyped.
    ///
    /// ECDSA is randomized, so these signature bytes cannot be reproduced by
    /// signing again; they are to be verified, exactly as the examples of
    /// RFC 9052 and RFC 9338 are.
    /// </summary>
    [TestFixture]
    public class SignedMetrologyExampleTests
    {

        #region Data

        /// <summary>
        /// The released record: the charging station's signed bundle of two
        /// signed meter readings, countersigned by the operator.
        /// </summary>
        public const String ReleasedRecord =
            "D28443A10126A204484F4E4267CBA434400B8344A1013822A104486B1F337BA0EC88BB58607B4DBDCF562B33FB55BC349B3D1CB8759" +
            "5CF5B3C693529447654C798EEE3BCA480AEA0EC9F9CF44FEDF0554C03CB74FA9D953CBA658BA3B9174677101A137A071A37B0A0FD9F" +
            "0A501B01DDF3921503125C9A77CD3B9DC38A346771EA8B1AFB075901FFA36F6368617267696E6753746174696F6E7244452A4745462A" +
            "4531323334353637382A316B7472616E73616374696F6E6861346631633965326872656164696E67738258DDD28445A101390108A104" +
            "48C6738177A6E6D04B5886A5656D657465726E31495341303030303030303034326B7472616E73616374696F6E6861346631633965326" +
            "7636F6E74657874715472616E73616374696F6E2E426567696E6474696D65C074323032362D30382D31355430383A31343A30305A6665" +
            "6E65726779D9ACDC84C482221A0012D6870203A401C48220187B020203C48221185F0401584053235DD1FD9502CBD0F5D362CC27E74F8" +
            "0106638E47CC338471B79E4D923AEDB9A954BDFB7FFF35C23FFBE5F5AD8EBADC60E188771003A92FA12131BF6934B2858DBD28445A101" +
            "390108A10448C6738177A6E6D04B5884A5656D657465726E31495341303030303030303034326B7472616E73616374696F6E686134663" +
            "1633965326763"                                                                                              +
            "6F6E746578746F5472616E73616374696F6E2E456E646474696D65C074323032362D30382D31355430393A30323A30305A66656E65726" +
            "779D9ACDC84C482221A0013395D0203A401C48220187E020203C48221185F0401584075244FCDEE271896EC275255A227FDACAD3A0EC8" +
            "F9AD052937CBE2121CA0F3FF6FB924A31C38626E5F8A8E88EBF0F7930F16CAB287D293D1455CE333F18E66A1584080324890A418CE16FB" +
            "28FA186973D55BE2C2C2A5EDDBDD0E9B6B6A44E707E19639D2A6C6F320676AE62E1A438EE86ED822B174C2BB71BF917D471A1C56DA861A";

        // The three published example keys. These exist for this example only
        // and secure nothing.
        private static COSEKey MeterKey()

            => new (COSEKeyType.EC2,
                    COSECurve.BrainpoolP256r1,
                    Convert.FromHexString("A734FB1962C381113C746BDDBCBC774801E3B73FA7F73479615D290E91E48889"),
                    Convert.FromHexString("8A188C8261A560197B37C73044E3009BA1DAED226C324A35FEE76AA144740678"),
                    null,
                    Convert.FromHexString("C6738177A6E6D04B"),
                    COSEAlgorithm.ESB256);

        private static COSEKey StationKey()

            => new (COSEKeyType.EC2,
                    COSECurve.P256,
                    Convert.FromHexString("7951E32509303CD4DB14127765B3FC9F32F62AC5C0F12350BD3ED7C746C72FE9"),
                    Convert.FromHexString("A35716031E2C44A942D886626C5D4C41E0FF62E44FED7EDA3ACC1408D90720DC"),
                    null,
                    Convert.FromHexString("4F4E4267CBA43440"),
                    COSEAlgorithm.ES256);

        private static COSEKey OperatorKey()

            => new (COSEKeyType.EC2,
                    COSECurve.P384,
                    Convert.FromHexString("5DEF24F33251A911F43205134D568C1FB3547E2BD0B602D4B18A5FA476FF1FB8E6D321CC4ED1DCF754A81159C63389D2"),
                    Convert.FromHexString("D8298F873104BC9AE145888BB7DC574AB26501E1E78DC4613CCB4B4C1B842720724671655551F9E2918C8943EAE8C2FA"),
                    null,
                    Convert.FromHexString("6B1F337BA0EC88BB"),
                    COSEAlgorithm.ES384);

        #endregion

        #region (private static) Released() / Readings(Bundle)

        private static COSESign1 Released()

            => COSESign1.Parse(Convert.FromHexString(ReleasedRecord));

        private static IEnumerable<COSESign1> Readings(CBORValue Bundle)

            => Bundle["readings"].AsArray().
                                  Select(static reading => COSESign1.Parse(reading.AsBytes()));

        #endregion


        #region The_whole_record_verifies_from_the_published_bytes_alone()

        [Test]
        public void The_whole_record_verifies_from_the_published_bytes_alone()
        {

            var released = Released();

            // The operator vouched for the charging station's signature...
            Assert.That(released.Countersignatures.Count,     Is.EqualTo(1));
            Assert.That(released.Countersignatures[0].KeyIdentifier,  Is.EqualTo(OperatorKey().KeyIdentifier));

            Assert.That(released.VerifyCountersignature(released.Countersignatures[0],
                                                        OperatorKey().ToPublicKey(),
                                                        out var operatorError),  Is.True, operatorError);

            // ...the charging station signed the bundle...
            Assert.That(released.KeyIdentifier,  Is.EqualTo(StationKey().KeyIdentifier));
            Assert.That(released.Verify(StationKey(), out var stationError),  Is.True, stationError);

            // ...and the meter signed every reading within it, on the
            // brainpool curve the German conformity assessment works with.
            var bundle    = CBORValue.Parse(released.Payload!);
            var readings  = Readings(bundle).ToArray();

            Assert.That(readings.Length,  Is.EqualTo(2));

            foreach (var reading in readings)
            {
                Assert.That(reading.Algorithm,      Is.EqualTo(COSEAlgorithm.ESB256));
                Assert.That(reading.KeyIdentifier,  Is.EqualTo(MeterKey().KeyIdentifier));
                Assert.That(reading.Verify(MeterKey(), out var meterError),  Is.True, meterError);
            }

            Assert.That(bundle["chargingStation"].AsText(),  Is.EqualTo("DE*GEF*E12345678*1"));
            Assert.That(bundle["transaction"].    AsText(),  Is.EqualTo("a4f1c9e2"));

        }

        #endregion

        #region The_readings_are_metrological_values_that_kept_their_scale()

        [Test]
        public void The_readings_are_metrological_values_that_kept_their_scale()
        {

            var bundle    = CBORValue.Parse(Released().Payload!);
            var readings  = Readings(bundle).Select(static reading => CBORValue.Parse(reading.Payload!)).ToArray();

            var expected  = new (String Context, String Time, Decimal Energy, Decimal Uncertainty)[] {
                                ("Transaction.Begin", "2026-08-15T08:14:00Z", 1234.567m, 12.3m),
                                ("Transaction.End",   "2026-08-15T09:02:00Z", 1259.869m, 12.6m)
                            };

            for (var i = 0; i < expected.Length; i++)
            {

                Assert.That(readings[i]["meter"].      AsText(),  Is.EqualTo("1ISA0000000042"));
                Assert.That(readings[i]["context"].    AsText(),  Is.EqualTo(expected[i].Context));
                Assert.That(readings[i]["time"].UntaggedValue.AsText(),  Is.EqualTo(expected[i].Time));

                Assert.That(MetrologicalValue.TryParse(readings[i]["energy"], out var energy, out var errorResponse),
                            Is.True, errorResponse);

                Assert.That(energy.Value,                       Is.EqualTo(expected[i].Energy),         expected[i].Context);
                Assert.That(energy.Unit.SingleUnit,             Is.EqualTo(UnitOfMeasure.WattHour),     expected[i].Context);
                Assert.That(energy.Prefix,                      Is.EqualTo(SIPrefix.Kilo),              expected[i].Context);

                // The decimal scale of the instrument survived the wire: three
                // decimal places, not a binary float that merely looks like one.
                Assert.That(energy.Value.ToString(),            Is.EqualTo(expected[i].Energy.ToString()),  expected[i].Context);
                Assert.That(Decimal.GetBits(energy.Value)[3] >> 16 & 0xFF,  Is.EqualTo(3),               expected[i].Context);

                // The uncertainty is a complete GUM statement, not a bare number.
                Assert.That(energy.Uncertainty.HasValue,                        Is.True,                 expected[i].Context);
                Assert.That(energy.Uncertainty!.Value.Value,                    Is.EqualTo(expected[i].Uncertainty));
                Assert.That(energy.Uncertainty!.Value.CoverageFactor,           Is.EqualTo(2));
                Assert.That(energy.Uncertainty!.Value.CoverageProbability,      Is.EqualTo(0.95));
                Assert.That(energy.Uncertainty!.Value.Distribution,             Is.EqualTo(UncertaintyDistribution.Normal));

                // ...so the standard uncertainty is derivable: U / k.
                Assert.That(energy.Uncertainty!.Value.StandardUncertainty,      Is.EqualTo(expected[i].Uncertainty / 2));

            }

            // What the customer is billed for is the difference of two
            // readings that each carry the signature of the meter.
            Assert.That(readings[1]["energy"], Is.Not.EqualTo(readings[0]["energy"]));

            MetrologicalValue.TryParse(readings[0]["energy"], out var begin, out _);
            MetrologicalValue.TryParse(readings[1]["energy"], out var end,   out _);

            Assert.That(end.Value - begin.Value,  Is.EqualTo(25.302m));

        }

        #endregion

        #region The_record_re_encodes_byte_exact()

        [Test]
        public void The_record_re_encodes_byte_exact()
        {

            Assert.That(Convert.ToHexString(Released().ToByteArray()),
                        Is.EqualTo(ReleasedRecord));

            Assert.That(Released().ToByteArray().Length,  Is.EqualTo(713));

        }

        #endregion

        #region A_single_altered_digit_is_caught_at_the_meter()

        [Test]
        public void A_single_altered_digit_is_caught_at_the_meter()
        {

            var released  = Released();
            var bundle    = CBORValue.Parse(released.Payload!);
            var reading   = Readings(bundle).First();
            var values    = CBORValue.Parse(reading.Payload!);

            MetrologicalValue.TryParse(values["energy"], out var energy, out _);

            // One thousandth of a kilowatt hour more...
            var tampered  = CBORValue.FromMap(
                                values.AsMap().
                                       Select(entry => entry.Key == CBORValue.FromText("energy")
                                                           ? new KeyValuePair<CBORValue, CBORValue>(
                                                                 entry.Key,
                                                                 new MetrologicalValue(
                                                                     energy.Value + 0.001m,
                                                                     energy.Unit,
                                                                     energy.Prefix,
                                                                     energy.Uncertainty
                                                                 ).ToCBOR()
                                                             )
                                                           : entry)
                            );

            var forged    = new COSESign1(
                                reading.ProtectedHeaderBytes,
                                reading.UnprotectedHeader,
                                tampered.ToByteArray(),
                                reading.Signature
                            );

            // ...and the meter's signature no longer holds. Everything above
            // it - the station, the operator - would still verify against the
            // old bundle, which is precisely why the reading carries its own
            // signature rather than relying on the layers around it.
            Assert.That(forged.Verify(MeterKey(), out var errorResponse),  Is.False);
            Assert.That(errorResponse,  Is.EqualTo("The signature is invalid!"));

        }

        #endregion

        #region Every_size_the_document_states_is_the_measured_one()

        [Test]
        public void Every_size_the_document_states_is_the_measured_one()
        {

            var released  = Released();
            var bundle    = CBORValue.Parse(released.Payload!);
            var readings  = Readings(bundle).ToArray();

            // Whenever a document and the code it describes disagree, it is
            // the document that is wrong and nobody notices. So every number
            // printed in tag-44252-signed-example.md is asserted here.
            Assert.That(readings[0].Payload!.Length,        Is.EqualTo(134),  "one meter reading, unsigned");
            Assert.That(readings[0].ToByteArray().Length,   Is.EqualTo(221),  "one meter reading, signed");
            Assert.That(readings[1].ToByteArray().Length,   Is.EqualTo(219),  "the second reading");
            Assert.That(released.Payload!.Length,           Is.EqualTo(511),  "both readings plus the station's metadata");
            Assert.That(released.ToByteArray().Length,      Is.EqualTo(713),  "the station's signed bundle");

            // The metrological value itself: value, scale, unit, prefix and a
            // complete GUM uncertainty.
            var energy = CBORValue.Parse(readings[0].Payload!)["energy"];

            Assert.That(energy.ToByteArray().Length,        Is.EqualTo(31),   "the metrological value");

            // r and s, each as wide as the group order of its curve.
            Assert.That(readings[0].Signature.Length,       Is.EqualTo( 64),  "the meter's signature, brainpoolP256r1");
            Assert.That(released.Signature.Length,          Is.EqualTo( 64),  "the station's signature, P-256");
            Assert.That(released.Countersignatures[0].Signature.Length,  Is.EqualTo(96),  "the operator's countersignature, P-384");

            // ...and the header buckets the document dissects byte by byte.
            Assert.That(Convert.ToHexString(readings[0].ProtectedHeaderBytes),                Is.EqualTo("A101390108"));
            Assert.That(Convert.ToHexString(released.ProtectedHeaderBytes),                   Is.EqualTo("A10126"));
            Assert.That(Convert.ToHexString(released.Countersignatures[0].ProtectedHeaderBytes),  Is.EqualTo("A1013822"));

        }

        #endregion

        #region Every_key_identifier_is_the_thumbprint_of_its_own_key()

        [Test]
        public void Every_key_identifier_is_the_thumbprint_of_its_own_key()
        {

            // The identifiers within the record are not arbitrary labels:
            // each is the leading eight bytes of the RFC 9679 thumbprint of
            // the key that signed, so a verifier can check that the key it
            // was handed is the key the record names.
            foreach (var key in new[] { MeterKey(), StationKey(), OperatorKey() })
            {
                Assert.That(key.KeyIdentifier,  Is.EqualTo(key.ThumbprintKeyIdentifier()),  key.Curve!.Value.Name);
            }

        }

        #endregion

    }

}
