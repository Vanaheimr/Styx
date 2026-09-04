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

using System.Numerics;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias.Tests
{

    /// <summary>
    /// Tests for the CBOR document model.
    /// </summary>
    [TestFixture]
    public class CBORValueTests
    {

        #region Nonnegative_integers_are_normalized_across_construction_paths()

        [Test]
        public void Nonnegative_integers_are_normalized_across_construction_paths()
        {

            Assert.That(CBORValue.FromInt64(5),                    Is.EqualTo(CBORValue.FromUInt64(5)));
            Assert.That(CBORValue.FromInt128(5),                   Is.EqualTo(CBORValue.FromUInt64(5)));
            Assert.That(CBORValue.FromBigInteger(5),               Is.EqualTo(CBORValue.FromUInt64(5)));
            Assert.That(CBORValue.FromInt64(5).Kind,               Is.EqualTo(CBORValueKind.UnsignedInteger));

            Assert.That(CBORValue.FromInt64(-5),                   Is.EqualTo(CBORValue.FromInt128(-5)));
            Assert.That(CBORValue.FromInt64(-5).Kind,              Is.EqualTo(CBORValueKind.NegativeInteger));

            // Map keys are therefore found independently of their construction...
            var map = new CBORMap {
                          { CBORValue.FromUInt64(4), "found" }
                      }.ToValue();

            Assert.That(map[4L].AsText(),                          Is.EqualTo("found"));

        }

        #endregion

        #region Indexers_find_text_and_integer_keys()

        [Test]
        public void Indexers_find_text_and_integer_keys()
        {

            // {"a": 1, 2: "two", -3: [1, 2]}
            var map = CBORValue.Parse(Convert.FromHexString("A3616101026374776F228201 02".Replace(" ", "")));

            Assert.That(map.Count,                Is.EqualTo(3));
            Assert.That(map["a"].AsUInt64(),      Is.EqualTo(1));
            Assert.That(map[2L].AsText(),         Is.EqualTo("two"));
            Assert.That(map[-3L][1].AsUInt64(),   Is.EqualTo(2));

            Assert.That(() => map["missing"],     Throws.TypeOf<CBORException>());
            Assert.That(() => map[0],             Throws.TypeOf<CBORException>());

        }

        #endregion

        #region TryGetValue_matches_arbitrary_key_kinds()

        [Test]
        public void TryGetValue_matches_arbitrary_key_kinds()
        {

            var arrayKey  = CBORValue.FromArray(1, 2);

            var map       = new CBORMap {
                                { arrayKey,           "array key!" },
                                { CBORValue.FromBytes([ 0xFF ]),  "byte key!" }
                            }.ToValue();

            Assert.That(map.TryGetValue(CBORValue.FromArray(1, 2), out var value1),  Is.True);
            Assert.That(value1.AsText(),                                             Is.EqualTo("array key!"));

            Assert.That(map.TryGetValue(CBORValue.FromBytes([ 0xFF ]), out var value2),  Is.True);
            Assert.That(value2.AsText(),                                             Is.EqualTo("byte key!"));

            Assert.That(map.TryGetValue(CBORValue.FromArray(2, 1), out _),           Is.False);

        }

        #endregion

        #region Tag_wrapping_and_unwrapping()

        [Test]
        public void Tag_wrapping_and_unwrapping()
        {

            var tagged = CBORValue.FromText("http://example.com").WithTag(CBORTag.URI);

            Assert.That(tagged.Kind,                          Is.EqualTo(CBORValueKind.Tagged));
            Assert.That(tagged.Tag,                           Is.EqualTo(CBORTag.URI));
            Assert.That(tagged.HasTag(CBORTag.URI),           Is.True);
            Assert.That(tagged.HasTag(CBORTag.UUID),          Is.False);
            Assert.That(tagged.UntaggedValue.AsText(),        Is.EqualTo("http://example.com"));

            Assert.That(() => CBORValue.FromInt64(1).Tag,     Throws.TypeOf<CBORException>());

        }

        #endregion

        #region FromDecimal_creates_tag4_nodes_and_AsDecimal_reads_them_back()

        [Test]
        public void FromDecimal_creates_tag4_nodes_and_AsDecimal_reads_them_back()
        {

            var node = CBORValue.FromDecimal(1.10m);

            Assert.That(node.HasTag(CBORTag.DecimalFraction),      Is.True);
            Assert.That(node.ToDiagnosticString(),                 Is.EqualTo("4([-2, 110])"));

            var roundtripped = node.AsDecimal();

            Assert.That(roundtripped,                              Is.EqualTo(1.10m));
            Assert.That(roundtripped.Scale,                        Is.EqualTo(2));

            // The scale distinguishes 1.1 from 1.10...
            Assert.That(CBORValue.FromDecimal(1.1m),               Is.Not.EqualTo(CBORValue.FromDecimal(1.10m)));

            // Decimal.MaxValue needs a bignum mantissa...
            Assert.That(CBORValue.FromDecimal(Decimal.MaxValue).AsDecimal(),
                        Is.EqualTo(Decimal.MaxValue));

            Assert.That(CBORValue.FromDecimal(Decimal.MinValue).AsDecimal(),
                        Is.EqualTo(Decimal.MinValue));

            Assert.That(Convert.ToHexString(CBORValue.FromDecimal(273.15m).ToByteArray()),
                        Is.EqualTo("C48221196AB3"));

        }

        #endregion

        #region Equality_is_representational()

        [Test]
        public void Equality_is_representational()
        {

            // A half-precision 1.0 is not a double-precision 1.0...
            Assert.That(CBORValue.FromHalf((Half) 1.0),      Is.Not.EqualTo(CBORValue.FromDouble(1.0)));

            // ...but identical NaNs are equal!
            Assert.That(CBORValue.FromDouble(Double.NaN),    Is.EqualTo(CBORValue.FromDouble(Double.NaN)));

            // 0.0 and -0.0 differ by their bits...
            Assert.That(CBORValue.FromDouble(0.0),           Is.Not.EqualTo(CBORValue.FromDouble(-0.0)));

            // Structural equality of containers...
            Assert.That(CBORValue.FromArray(1, "a", true),   Is.EqualTo(CBORValue.FromArray(1, "a", true)));
            Assert.That(CBORValue.FromArray(1, "a"),         Is.Not.EqualTo(CBORValue.FromArray("a", 1)));

            // Tagged values compare their tag and inner value...
            Assert.That(CBORValue.FromInt64(1).WithTag(2),   Is.EqualTo(CBORValue.FromInt64(1).WithTag(2)));
            Assert.That(CBORValue.FromInt64(1).WithTag(2),   Is.Not.EqualTo(CBORValue.FromInt64(1).WithTag(3)));

            // ...and equal values have equal hash codes!
            Assert.That(CBORValue.FromArray(1, "a").GetHashCode(),
                        Is.EqualTo(CBORValue.FromArray(1, "a").GetHashCode()));

        }

        #endregion

        #region Builders_support_collection_initializers()

        [Test]
        public void Builders_support_collection_initializers()
        {

            CBORValue array = new CBORArray { 1, "text", true, CBORValue.Null };

            Assert.That(array.Count,                 Is.EqualTo(4));
            Assert.That(array[0].AsUInt64(),         Is.EqualTo(1));
            Assert.That(array[1].AsText(),           Is.EqualTo("text"));
            Assert.That(array[2].AsBoolean(),        Is.True);
            Assert.That(array[3].Kind,               Is.EqualTo(CBORValueKind.Null));

            CBORValue map = new CBORMap {
                                { "key",  1        },
                                { 2,      "value"  },
                                { -3,     new CBORArray { 1, 2 } }
                            };

            Assert.That(map.Count,                   Is.EqualTo(3));
            Assert.That(map["key"].AsUInt64(),       Is.EqualTo(1));
            Assert.That(map[2L].AsText(),            Is.EqualTo("value"));
            Assert.That(map[-3L].Count,              Is.EqualTo(2));

        }

        #endregion

        #region Duplicate_map_keys_follow_the_configured_policy()

        [Test]
        public void Duplicate_map_keys_follow_the_configured_policy()
        {

            // {"a": 1, "a": 2}
            var duplicateKeys = Convert.FromHexString("A2616101616102");

            Assert.That(() => CBORValue.Parse(duplicateKeys),
                        Throws.TypeOf<CBORException>());

            Assert.That(CBORValue.Parse(duplicateKeys,
                                        new CBORReaderOptions { DuplicateKeyPolicy = CBORDuplicateKeyPolicy.TakeFirst })["a"].AsUInt64(),
                        Is.EqualTo(1));

            Assert.That(CBORValue.Parse(duplicateKeys,
                                        new CBORReaderOptions { DuplicateKeyPolicy = CBORDuplicateKeyPolicy.TakeLast })["a"].AsUInt64(),
                        Is.EqualTo(2));

        }

        #endregion

        #region TryParse_reports_errors_in_house_style()

        [Test]
        public void TryParse_reports_errors_in_house_style()
        {

            Assert.That(CBORValue.TryParse(Convert.FromHexString("83010203"), out var cbor, out var errorResponse),  Is.True);
            Assert.That(cbor.Count,         Is.EqualTo(3));
            Assert.That(errorResponse,      Is.Null);

            Assert.That(CBORValue.TryParse(Convert.FromHexString("830102"), out _, out var errorResponse2),          Is.False);
            Assert.That(errorResponse2,     Is.Not.Null);

            // Trailing bytes are an error...
            Assert.That(CBORValue.TryParse(Convert.FromHexString("0102"), out _, out var errorResponse3),            Is.False);
            Assert.That(errorResponse3,     Does.Contain("trailing"));

        }

        #endregion

        #region ReadFrom_composes_with_the_low_level_reader()

        [Test]
        public void ReadFrom_composes_with_the_low_level_reader()
        {

            // [1, {"a": true}, h'FF']
            var reader = new CBORReader(Convert.FromHexString("8301A16161F541FF"));

            Assert.That(reader.ReadStartArray(),   Is.EqualTo(3));
            Assert.That(reader.ReadUInt64(),       Is.EqualTo(1));

            var innerMap = CBORValue.ReadFrom(ref reader);

            Assert.That(innerMap["a"].AsBoolean(), Is.True);

            Assert.That(reader.ReadByteString(),   Is.EqualTo(new Byte[] { 0xFF }));

            reader.ReadEndArray();

            Assert.That(reader.PeekState(),        Is.EqualTo(CBORReaderState.Finished));

        }

        #endregion

    }

}
