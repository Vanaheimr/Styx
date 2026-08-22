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
using Org.BouncyCastle.Crypto.Parameters;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias.Tests
{

    /// <summary>
    /// The worked example of the tag 44252 specification
    /// (MetrologicalCBOR/tag-44252-signed-example.md in the
    /// OpenChargingTechnology Whitepapers repository):
    /// a charging transaction as a signed metrological record, from the meter
    /// through the charging station to the customer.
    ///
    /// Everything below is derived from ONE constant, the 713 bytes the
    /// customer receives, and from the three published public keys. That
    /// every signature within it verifies is what makes the document a worked
    /// example rather than an illustration - and it is why the document may
    /// be regenerated but never retyped.
    ///
    /// Every signature within it is deterministic (RFC 6979), so the record
    /// is not merely verifiable but recomputable: one of the tests below
    /// rebuilds it from the private keys and gets the same 713 bytes.
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
            "D28443A10126A204484F4E4267CBA434400B8344A1013822A104486B1F337BA0EC88BB586056AA831918D6215BFE6ABAA02791C8FB619E" +
            "0C2661F55E8C1F95967A67A02863E1ACC9EB090F4A2DD5BE6134380A29D65BA71661A2BA7D337C84C4E4C2C2D87F8925618D0CC7EF3E1E" +
            "BD6D4279B55514A156B4E5315237488B681C20118283175901FFA36F6368617267696E6753746174696F6E7244452A4745462A45313233" +
            "34353637382A316B7472616E73616374696F6E6861346631633965326872656164696E67738258DDD28445A101390108A10448C6738177" +
            "A6E6D04B5886A5656D657465726E31495341303030303030303034326B7472616E73616374696F6E68613466316339653267636F6E7465" +
            "7874715472616E73616374696F6E2E426567696E6474696D65C074323032362D30382D31355430383A31343A30305A66656E65726779D9" +
            "ACDC84C482221A0012D6870203A401C48220187B020203C48221185F040158406A40B66B6D228217D87F6751D1919BA82CCA959F079EFC" +
            "98F805BAE4CBC340A3611ABAC58B3AA2E1FB51EA85CACB978C03DCF78F407039DA41A2E653A60E138958DBD28445A101390108A10448C6" +
            "738177A6E6D04B5884A5656D657465726E31495341303030303030303034326B7472616E73616374696F6E68613466316339653267636F" +
            "6E746578746F5472616E73616374696F6E2E456E646474696D65C074323032362D30382D31355430393A30323A30305A66656E65726779" +
            "D9ACDC84C482221A0013395D0203A401C48220187E020203C48221185F040158401D92018570E22306441FDD0E1645124C03F63CDE0D75" +
            "A154B7ECD784112020F25834508FD5D9A6A016025A85B8BD7F5DF27056B33EDFC7A823E55449061562CC5840C521E083F44F35D056F5B6" +
            "F75893B7B2AD8E32CFB2F60DFEAA405466083C16267C6E9256110BDBD204D81878E195A9E4BE644FE034BC7A640A42F82CC931AA2E";

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

        #region The_record_is_reproducible_from_the_keys_alone()

        [Test]
        public void The_record_is_reproducible_from_the_keys_alone()
        {

            // The private halves of the three published example keys.
            var meter    = new ECPrivateKeyParameters(new BigInteger("08F001BB03BEF4FBD1C59F10B50555CD37D2B53421331DBFA98815A581326FB3", 16),
                                                      COSECurve.BrainpoolP256r1.DomainParameters!);

            var station  = new ECPrivateKeyParameters(new BigInteger("875E51ECF18073E8B970E6DCC5A115433456E13DF966034A5A782945D2B684D3", 16),
                                                      COSECurve.P256.DomainParameters!);

            var cpo      = new ECPrivateKeyParameters(new BigInteger("6952487A0A16EACE6E9A69EFD062D7671D68D23FF68722326348827C3A94E2A1" +
                                                                     "743A1DF8901B948412CCA26CA4372CED", 16),
                                                      COSECurve.P384.DomainParameters!);

            // Every key identifier within the record is the thumbprint of the
            // key that signed, so the private halves above really are the
            // halves of the published public ones.
            Assert.That(COSEKey.From(meter,   null, COSEAlgorithm.ESB256).ThumbprintKeyIdentifier(),  Is.EqualTo(MeterKey().  KeyIdentifier));
            Assert.That(COSEKey.From(station, null, COSEAlgorithm.ES256). ThumbprintKeyIdentifier(),  Is.EqualTo(StationKey().KeyIdentifier));
            Assert.That(COSEKey.From(cpo,     null, COSEAlgorithm.ES384). ThumbprintKeyIdentifier(),  Is.EqualTo(OperatorKey().KeyIdentifier));

            var samples  = new (String Context, String Time, Decimal Energy, Decimal Uncertainty)[] {
                               ("Transaction.Begin", "2026-08-15T08:14:00Z", 1234.567m, 12.3m),
                               ("Transaction.End",   "2026-08-15T09:02:00Z", 1259.869m, 12.6m)
                           };

            var readings = new List<COSESign1>();

            // Both signing calls below pass CanonicalizePayload: false,
            // because the published document predates that default: its maps
            // are in reading order rather than sorted by encoded key, so
            // canonicalizing them would rebuild a DIFFERENT record - same
            // size, same values, different bytes and different signatures.
            // What that costs is measured by
            // The_published_record_does_not_survive_being_forwarded() below.
            foreach (var sample in samples)
            {

                var energy  = new MetrologicalValue(
                                  sample.Energy,
                                  UnitOfMeasure.WattHour,
                                  SIPrefix.Kilo,
                                  new MeasurementUncertainty(sample.Uncertainty, 2, 0.95, UncertaintyDistribution.Normal)
                              );

                var reading = CBORValue.FromMap([
                                  new (CBORValue.FromText("meter"),        CBORValue.FromText("1ISA0000000042")),
                                  new (CBORValue.FromText("transaction"),  CBORValue.FromText("a4f1c9e2")),
                                  new (CBORValue.FromText("context"),      CBORValue.FromText(sample.Context)),
                                  new (CBORValue.FromText("time"),         CBORValue.Tagged(CBORTag.DateTimeString, CBORValue.FromText(sample.Time))),
                                  new (CBORValue.FromText("energy"),       energy.ToCBOR())
                              ]);

                readings.Add(
                    COSESign1.Sign(reading.ToByteArray(),
                                   meter,
                                   COSEAlgorithm.ESB256,
                                   MeterKey().KeyIdentifier,
                                   Deterministic:        true,
                                   CanonicalizePayload:  false)
                );

            }

            var bundle   = CBORValue.FromMap([
                               new (CBORValue.FromText("chargingStation"),  CBORValue.FromText("DE*GEF*E12345678*1")),
                               new (CBORValue.FromText("transaction"),      CBORValue.FromText("a4f1c9e2")),
                               new (CBORValue.FromText("readings"),         CBORValue.FromArray(readings.Select(static reading => CBORValue.FromBytes(reading.ToByteArray()))))
                           ]);

            var rebuilt  = COSESign1.Sign(bundle.ToByteArray(),
                                          station,
                                          COSEAlgorithm.ES256,
                                          StationKey().KeyIdentifier,
                                          Deterministic:        true,
                                          CanonicalizePayload:  false).
                                     AddCountersignature(cpo,
                                                         COSEAlgorithm.ES384,
                                                         OperatorKey().KeyIdentifier,
                                                         Deterministic: true);

            // Because every signature is deterministic, rebuilding the record
            // reproduces it byte for byte. That is what allows the document to
            // be recomputed rather than merely believed - and it is what a
            // reviewer at a metrology institute is entitled to do.
            Assert.That(Convert.ToHexString(rebuilt.ToByteArray()),
                        Is.EqualTo(ReleasedRecord));

        }

        #endregion

        #region The_published_record_does_not_survive_being_forwarded()

        [Test]
        public void The_published_record_does_not_survive_being_forwarded()
        {

            var released   = Released();

            // Every map within the document is in reading order -
            // chargingStation, transaction, readings - which is what a person
            // wants and not what RFC 8949, Section 4.2.1 asks for: sorted by
            // the encoded key, which puts the shortest name first.
            Assert.That(COSEPayload.IsCanonical(released.Payload!),  Is.False);

            foreach (var reading in Readings(CBORValue.Parse(released.Payload!)))
                Assert.That(COSEPayload.IsCanonical(reading.Payload!),  Is.False);

            // So a receiver that decodes the bundle and encodes it again -
            // a backend, a roaming hub, anything that keeps a record as a
            // model rather than as bytes - passes on a record whose signature
            // no longer holds, having altered nothing about it. The bytes are
            // even the same length: sorting a map moves them without adding
            // any.
            var forwarded  = new COSESign1(
                                 released.ProtectedHeaderBytes,
                                 released.UnprotectedHeader,
                                 COSEPayload.Canonicalize(released.Payload!),
                                 released.Signature,
                                 released.IsTagged
                             );

            Assert.That(forwarded.Payload!.Length,  Is.EqualTo(released.Payload!.Length));

            Assert.That(forwarded.Verify(StationKey(), out var errorResponse),  Is.False);
            Assert.That(errorResponse,  Is.EqualTo("The signature is invalid!"));

            // This is why COSESign1.Sign canonicalizes by default now, and
            // why the rebuild above must opt out of it: the published bytes
            // are what they are. Regenerating the document would make this
            // test fail, and that would be the good news.

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
            // printed in the specification's tag-44252-signed-example.md is
            // asserted here.
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
