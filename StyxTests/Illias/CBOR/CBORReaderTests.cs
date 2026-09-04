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
    /// Tests for the CBOR reader (RFC 8949).
    /// </summary>
    [TestFixture]
    public class CBORReaderTests
    {

        #region PeekState_walks_a_mixed_document()

        [Test]
        public void PeekState_walks_a_mixed_document()
        {

            // {"a": 1, "b": [_ 2]}
            var data    = Convert.FromHexString("A261610161629F02FF");
            var reader  = new CBORReader(data);

            Assert.That(reader.PeekState(),       Is.EqualTo(CBORReaderState.StartMap));
            Assert.That(reader.ReadStartMap(),    Is.EqualTo(2));

            Assert.That(reader.PeekState(),       Is.EqualTo(CBORReaderState.TextString));
            Assert.That(reader.ReadTextString(),  Is.EqualTo("a"));

            Assert.That(reader.PeekState(),       Is.EqualTo(CBORReaderState.UnsignedInteger));
            Assert.That(reader.ReadUInt64(),      Is.EqualTo(1));

            Assert.That(reader.ReadTextString(),  Is.EqualTo("b"));

            Assert.That(reader.PeekState(),       Is.EqualTo(CBORReaderState.StartArray));
            Assert.That(reader.ReadStartArray(),  Is.Null);

            Assert.That(reader.ReadUInt64(),      Is.EqualTo(2));

            Assert.That(reader.PeekState(),       Is.EqualTo(CBORReaderState.EndArray));
            reader.ReadEndArray();

            Assert.That(reader.PeekState(),       Is.EqualTo(CBORReaderState.EndMap));
            reader.ReadEndMap();

            Assert.That(reader.PeekState(),       Is.EqualTo(CBORReaderState.Finished));
            Assert.That(reader.BytesRemaining,    Is.EqualTo(0));

        }

        #endregion

        #region Integers_decode_across_the_full_range()

        [Test]
        public void Integers_decode_across_the_full_range()
        {

            Assert.That(new CBORReader(Convert.FromHexString("00")).ReadUInt64(),                  Is.EqualTo(0));
            Assert.That(new CBORReader(Convert.FromHexString("1818")).ReadUInt64(),                Is.EqualTo(24));
            Assert.That(new CBORReader(Convert.FromHexString("1BFFFFFFFFFFFFFFFF")).ReadUInt64(),  Is.EqualTo(UInt64.MaxValue));

            Assert.That(new CBORReader(Convert.FromHexString("20")).ReadInt64(),                   Is.EqualTo(-1));
            Assert.That(new CBORReader(Convert.FromHexString("3903E7")).ReadInt64(),               Is.EqualTo(-1000));
            Assert.That(new CBORReader(Convert.FromHexString("3B7FFFFFFFFFFFFFFF")).ReadInt64(),   Is.EqualTo(Int64.MinValue));

        }

        #endregion

        #region ReadInt64_throws_OverflowException_below_Int64Min_but_ReadInt128_succeeds()

        [Test]
        public void ReadInt64_throws_OverflowException_below_Int64Min_but_ReadInt128_succeeds()
        {

            Assert.That(() => new CBORReader(Convert.FromHexString("3BFFFFFFFFFFFFFFFF")).ReadInt64(),
                        Throws.TypeOf<OverflowException>());

            Assert.That(new CBORReader(Convert.FromHexString("3BFFFFFFFFFFFFFFFF")).ReadInt128(),
                        Is.EqualTo(Int128.Parse("-18446744073709551616")));

            Assert.That(() => new CBORReader(Convert.FromHexString("1BFFFFFFFFFFFFFFFF")).ReadInt64(),
                        Throws.TypeOf<OverflowException>());

        }

        #endregion

        #region ReadBigInteger_reads_plain_integers_and_bignums()

        [Test]
        public void ReadBigInteger_reads_plain_integers_and_bignums()
        {

            Assert.That(new CBORReader(Convert.FromHexString("1903E8")).ReadBigInteger(),
                        Is.EqualTo(new BigInteger(1000)));

            Assert.That(new CBORReader(Convert.FromHexString("C249010000000000000000")).ReadBigInteger(),
                        Is.EqualTo(BigInteger.Parse("18446744073709551616")));

            Assert.That(new CBORReader(Convert.FromHexString("C349010000000000000000")).ReadBigInteger(),
                        Is.EqualTo(BigInteger.Parse("-18446744073709551617")));

            // Non-canonical bignums with leading zeros are tolerated...
            Assert.That(new CBORReader(Convert.FromHexString("C24300002A")).ReadBigInteger(),
                        Is.EqualTo(new BigInteger(42)));

        }

        #endregion

        #region Widening_float_reads_are_allowed_and_narrowing_reads_are_rejected()

        [Test]
        public void Widening_float_reads_are_allowed_and_narrowing_reads_are_rejected()
        {

            Assert.That(new CBORReader(Convert.FromHexString("F93E00")).ReadHalf(),                 Is.EqualTo((Half) 1.5));
            Assert.That(new CBORReader(Convert.FromHexString("F93E00")).ReadSingle(),               Is.EqualTo(1.5f));
            Assert.That(new CBORReader(Convert.FromHexString("F93E00")).ReadDouble(),               Is.EqualTo(1.5));

            Assert.That(new CBORReader(Convert.FromHexString("FA47C35000")).ReadSingle(),           Is.EqualTo(100000.0f));
            Assert.That(new CBORReader(Convert.FromHexString("FA47C35000")).ReadDouble(),           Is.EqualTo(100000.0));

            Assert.That(new CBORReader(Convert.FromHexString("FB3FF199999999999A")).ReadDouble(),   Is.EqualTo(1.1));

            Assert.That(() => new CBORReader(Convert.FromHexString("FA47C35000")).ReadHalf(),
                        Throws.TypeOf<CBORException>());

            Assert.That(() => new CBORReader(Convert.FromHexString("FB3FF199999999999A")).ReadSingle(),
                        Throws.TypeOf<CBORException>());

        }

        #endregion

        #region Indefinite_string_chunks_are_concatenated()

        [Test]
        public void Indefinite_string_chunks_are_concatenated()
        {

            Assert.That(new CBORReader(Convert.FromHexString("5F42010243030405FF")).ReadByteString(),
                        Is.EqualTo(new Byte[] { 1, 2, 3, 4, 5 }));

            Assert.That(new CBORReader(Convert.FromHexString("7F657374726561646D696E67FF")).ReadTextString(),
                        Is.EqualTo("streaming"));

            Assert.That(new CBORReader(Convert.FromHexString("5FFF")).ReadByteString(),
                        Is.Empty);

            Assert.That(new CBORReader(Convert.FromHexString("7FFF")).ReadTextString(),
                        Is.EqualTo(""));

        }

        #endregion

        #region SkipValue_skips_nested_structures_and_tags()

        [Test]
        public void SkipValue_skips_nested_structures_and_tags()
        {

            // [1, {"a": [_ 1, 2]}, "x"]
            var reader = new CBORReader(Convert.FromHexString("8301A161619F0102FF6178"));

            Assert.That(reader.ReadStartArray(),  Is.EqualTo(3));
            Assert.That(reader.ReadUInt64(),      Is.EqualTo(1));

            reader.SkipValue();

            Assert.That(reader.ReadTextString(),  Is.EqualTo("x"));
            reader.ReadEndArray();

            Assert.That(reader.PeekState(),       Is.EqualTo(CBORReaderState.Finished));


            // A tagged decimal fraction is skipped as a single data item...
            var reader2 = new CBORReader(Convert.FromHexString("82C48220183205"));

            Assert.That(reader2.ReadStartArray(), Is.EqualTo(2));
            reader2.SkipValue();
            Assert.That(reader2.ReadUInt64(),     Is.EqualTo(5));
            reader2.ReadEndArray();

        }

        #endregion

        #region ReadEncodedValue_returns_the_exact_item_bytes()

        [Test]
        public void ReadEncodedValue_returns_the_exact_item_bytes()
        {

            var reader = new CBORReader(Convert.FromHexString("82C48220183205"));

            Assert.That(reader.ReadStartArray(),  Is.EqualTo(2));

            Assert.That(Convert.ToHexString(reader.ReadEncodedValue()),
                        Is.EqualTo("C482201832"));

            Assert.That(reader.ReadUInt64(),      Is.EqualTo(5));

            reader.ReadEndArray();

        }

        #endregion

        #region ReadDecimal_accepts_plain_integers_positive_exponents_and_bignum_mantissas()

        [Test]
        public void ReadDecimal_accepts_plain_integers_positive_exponents_and_bignum_mantissas()
        {

            // 4([-1, 50]) == 5.0 with a preserved scale of 1...
            var fivePointZero = new CBORReader(Convert.FromHexString("C48220 1832".Replace(" ", ""))).ReadDecimal();
            Assert.That(fivePointZero,        Is.EqualTo(5.0m));
            Assert.That(fivePointZero.Scale,  Is.EqualTo(1));

            // A plain integer...
            var five = new CBORReader(Convert.FromHexString("05")).ReadDecimal();
            Assert.That(five,                 Is.EqualTo(5m));
            Assert.That(five.Scale,           Is.EqualTo(0));

            // A negative integer...
            Assert.That(new CBORReader(Convert.FromHexString("3903E7")).ReadDecimal(),  Is.EqualTo(-1000m));

            // A positive exponent: 4([2, 5]) == 500...
            var fiveHundred = new CBORReader(Convert.FromHexString("C4820205")).ReadDecimal();
            Assert.That(fiveHundred,          Is.EqualTo(500m));
            Assert.That(fiveHundred.Scale,    Is.EqualTo(0));

            // A bignum mantissa crossing the 64 bit boundary...
            Assert.That(new CBORReader(Convert.FromHexString("C48220C2490A0000000000000005")).ReadDecimal(),
                        Is.EqualTo(18446744073709551616.5m));

            // A bignum without a decimal fraction...
            Assert.That(new CBORReader(Convert.FromHexString("C249010000000000000000")).ReadDecimal(),
                        Is.EqualTo(18446744073709551616m));

            // Reduce-if-exact: 4([-30, 1500]) == 15 * 10^-28...
            Assert.That(new CBORReader(Convert.FromHexString("C482381D1905DC")).ReadDecimal(),
                        Is.EqualTo(new Decimal(15, 0, 0, false, 28)));

            // ...but 4([-30, 7]) is not representable without loss!
            Assert.That(() => new CBORReader(Convert.FromHexString("C482381D07")).ReadDecimal(),
                        Throws.TypeOf<OverflowException>());

            // 4([29, 1]) is too large...
            Assert.That(() => new CBORReader(Convert.FromHexString("C482181D01")).ReadDecimal(),
                        Throws.TypeOf<OverflowException>());

            // A mantissa of 2^96 does not fit...
            Assert.That(() => new CBORReader(Convert.FromHexString("C48200C24D01000000000000000000000000")).ReadDecimal(),
                        Throws.TypeOf<OverflowException>());

        }

        #endregion

        #region ReadDateTime_reads_tag0_text_and_tag1_epoch_values()

        [Test]
        public void ReadDateTime_reads_tag0_text_and_tag1_epoch_values()
        {

            var expected = new DateTimeOffset(2013, 3, 21, 20, 4, 0, TimeSpan.Zero);

            Assert.That(new CBORReader(Convert.FromHexString("C074323031332D30332D32315432303A30343A30305A")).ReadDateTime(),
                        Is.EqualTo(expected));

            Assert.That(new CBORReader(Convert.FromHexString("C11A514B67B0")).ReadDateTime(),
                        Is.EqualTo(expected));

            // A fractional epoch timestamp: 1363896240.5...
            Assert.That(new CBORReader(Convert.FromHexString("C1FB41D452D9EC200000")).ReadDateTime(),
                        Is.EqualTo(expected.AddMilliseconds(500)));

        }

        #endregion

        #region A_COSE_key_map_with_integer_keys_can_be_read()

        [Test]
        public void A_COSE_key_map_with_integer_keys_can_be_read()
        {

            // The shape of a WebAuthn EC2 COSE_Key:
            // {1: 2, 3: -7, -1: 1, -2: x-coordinate, -3: y-coordinate}
            var xCoordinate  = new Byte[32];
            var yCoordinate  = new Byte[32];

            for (var i = 0; i < 32; i++)
            {
                xCoordinate[i] = (Byte)  i;
                yCoordinate[i] = (Byte) (i + 32);
            }

            var hex     = "A5" +
                          "0102" +
                          "0326" +
                          "2001" +
                          "215820" + Convert.ToHexString(xCoordinate) +
                          "225820" + Convert.ToHexString(yCoordinate);

            var reader  = new CBORReader(Convert.FromHexString(hex));
            var pairs   = reader.ReadStartMap();

            Assert.That(pairs,  Is.EqualTo(5));

            Byte[]? x = null;
            Byte[]? y = null;

            for (var i = 0; i < pairs!.Value; i++)
            {

                var key = reader.ReadInt64();

                switch (key)
                {

                    case -2:
                        x = reader.ReadByteString();
                        break;

                    case -3:
                        y = reader.ReadByteString();
                        break;

                    default:
                        reader.SkipValue();
                        break;

                }

            }

            reader.ReadEndMap();

            Assert.That(reader.PeekState(),  Is.EqualTo(CBORReaderState.Finished));
            Assert.That(x,                   Is.EqualTo(xCoordinate));
            Assert.That(y,                   Is.EqualTo(yCoordinate));

        }

        #endregion

        #region Only_a_single_top_level_item_can_be_read()

        [Test]
        public void Only_a_single_top_level_item_can_be_read()
        {

            var data    = Convert.FromHexString("0102");
            var reader  = new CBORReader(data);

            Assert.That(reader.ReadUInt64(),        Is.EqualTo(1));
            Assert.That(reader.PeekState(),         Is.EqualTo(CBORReaderState.Finished));
            Assert.That(reader.BytesRemaining,      Is.EqualTo(1));

            Assert.That(() => {
                            var secondReader = new CBORReader(Convert.FromHexString("0102"));
                            secondReader.ReadUInt64();
                            secondReader.ReadUInt64();
                        },
                        Throws.TypeOf<CBORException>());

        }

        #endregion

        #region TryRead_restores_the_position_on_failure()

        [Test]
        public void TryRead_restores_the_position_on_failure()
        {

            var reader = new CBORReader(Convert.FromHexString("20"));

            Assert.That(reader.TryReadUInt64(out _),        Is.False);
            Assert.That(reader.TryReadTextString(out _),    Is.False);
            Assert.That(reader.TryReadInt64(out var value), Is.True);
            Assert.That(value,                              Is.EqualTo(-1));

        }

        #endregion

        #region RequireDeterministic_reader_rejects_nonshortest_and_unsorted_input()

        [Test]
        public void RequireDeterministic_reader_rejects_nonshortest_and_unsorted_input()
        {

            // A canonically sorted map is fine...
            var reader = new CBORReader(Convert.FromHexString("A2616101616202"), CBORReaderOptions.Canonical);
            reader.ReadStartMap();
            reader.ReadTextString();  reader.ReadUInt64();
            reader.ReadTextString();  reader.ReadUInt64();
            reader.ReadEndMap();

            // Keys out of order...
            Assert.That(() => {
                            var r = new CBORReader(Convert.FromHexString("A2616202616101"), CBORReaderOptions.Canonical);
                            r.ReadStartMap();
                            r.ReadTextString();  r.ReadUInt64();
                            r.ReadTextString();  r.ReadUInt64();
                        },
                        Throws.TypeOf<CBORException>());

            // Duplicate keys...
            Assert.That(() => {
                            var r = new CBORReader(Convert.FromHexString("A2616101616102"), CBORReaderOptions.Canonical);
                            r.ReadStartMap();
                            r.ReadTextString();  r.ReadUInt64();
                            r.ReadTextString();  r.ReadUInt64();
                        },
                        Throws.TypeOf<CBORException>());

            // A non-shortest integer head: 23 encoded in two bytes...
            Assert.That(() => new CBORReader(Convert.FromHexString("1817"), CBORReaderOptions.Canonical).ReadUInt64(),
                        Throws.TypeOf<CBORException>());

            // A non-shortest float: Infinity as a single...
            Assert.That(() => new CBORReader(Convert.FromHexString("FA7F800000"), CBORReaderOptions.Canonical).ReadDouble(),
                        Throws.TypeOf<CBORException>());

            // ...but as a half it is fine!
            Assert.That(new CBORReader(Convert.FromHexString("F97C00"), CBORReaderOptions.Canonical).ReadDouble(),
                        Is.EqualTo(Double.PositiveInfinity));

            // A non-canonical NaN...
            Assert.That(() => new CBORReader(Convert.FromHexString("FB7FF8000000000000"), CBORReaderOptions.Canonical).ReadDouble(),
                        Throws.TypeOf<CBORException>());

            // Indefinite lengths...
            Assert.That(() => new CBORReader(Convert.FromHexString("9FFF"), CBORReaderOptions.Canonical).ReadStartArray(),
                        Throws.TypeOf<CBORException>());

        }

        #endregion

    }

}
