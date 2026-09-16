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

using System.Text;

namespace org.GraphDefined.Vanaheimr.Illias.Tests
{

    /// <summary>
    /// Base32 tests.
    /// </summary>
    /// <remarks>
    /// The pair has to be a pair. ToBase32 used to walk its input two and three
    /// bytes at a time and lose track of where it was, so what came out could
    /// not be read by FromBASE32 beside it - twenty-five bytes through the two
    /// came back as thirty-eight. Nothing in this repository called the
    /// encoder, so nothing noticed, and the first caller outside it wrote a
    /// second encoder rather than find out why the first one did not work.
    /// </remarks>
    [TestFixture]
    public class Base32_Tests
    {

        #region KnownVectors_AreEncodedAsRFC4648Says(Text, Expected)

        /// <summary>
        /// The test vectors of RFC 4648 section 10, which exist so that two
        /// implementations can be shown to agree without either one being
        /// trusted.
        /// </summary>
        [TestCase("",        "")]
        [TestCase("f",       "MY======")]
        [TestCase("fo",      "MZXQ====")]
        [TestCase("foo",     "MZXW6===")]
        [TestCase("foob",    "MZXW6YQ=")]
        [TestCase("fooba",   "MZXW6YTB")]
        [TestCase("foobar",  "MZXW6YTBOI======")]
        public void KnownVectors_AreEncodedAsRFC4648Says(String Text, String Expected)
        {

            Assert.That(Encoding.ASCII.GetBytes(Text).ToBase32(), Is.EqualTo(Expected));

        }

        #endregion

        #region KnownVectors_AreDecodedBack(Text, Base32)

        [TestCase("f",       "MY======")]
        [TestCase("fo",      "MZXQ====")]
        [TestCase("foo",     "MZXW6===")]
        [TestCase("foob",    "MZXW6YQ=")]
        [TestCase("fooba",   "MZXW6YTB")]
        [TestCase("foobar",  "MZXW6YTBOI======")]
        public void KnownVectors_AreDecodedBack(String Text, String Base32)
        {

            Assert.That(Encoding.ASCII.GetString(Base32.FromBASE32()), Is.EqualTo(Text));

        }

        #endregion

        #region EveryLength_SurvivesTheRoundTrip()

        /// <summary>
        /// Every length from nothing to a hundred bytes, there and back.
        /// </summary>
        /// <remarks>
        /// Every length, because base32 packs eight characters out of five
        /// bytes and an encoder that is wrong is usually only wrong for the
        /// lengths that do not divide evenly. Twenty-five bytes - an Alfen
        /// public key - is one of those that did divide evenly and still came
        /// back as thirty-eight.
        /// </remarks>
        [Test]
        public void EveryLength_SurvivesTheRoundTrip()
        {

            var random = new Random(20260916);

            Assert.Multiple(() => {

                for (var length = 0; length <= 100; length++)
                {

                    var bytes    = new Byte[length];
                    random.NextBytes(bytes);

                    var base32   = bytes.ToBase32();
                    var back     = base32.Length == 0 ? [] : base32.FromBASE32();

                    Assert.That(back, Is.EqualTo(bytes), $"{length} bytes");

                    // And the length the format prescribes, so that a reader
                    // checking it before decoding is not surprised.
                    Assert.That(base32.Length % 8, Is.Zero, $"{length} bytes, padded to a multiple of eight");

                }

            });

        }

        #endregion

        #region TheLengthsTheAlfenFormatPrescribes()

        /// <summary>
        /// The three field lengths of an Alfen signed meter value, which is
        /// where the broken encoder was found.
        /// </summary>
        [Test]
        public void TheLengthsTheAlfenFormatPrescribes()
        {

            var random = new Random(20260916);

            Assert.Multiple(() => {

                foreach (var length in new[] { 25, 48, 82 })
                {

                    var bytes = new Byte[length];
                    random.NextBytes(bytes);

                    Assert.That(bytes.ToBase32().FromBASE32().Length, Is.EqualTo(length), $"{length} bytes");

                }

            });

        }

        #endregion

        #region TheAlphabetIsTheOneRFC4648Names()

        /// <summary>
        /// Uppercase letters and the digits two to seven, and nothing else - a
        /// receipt is typed back in by hand, and the missing digits are the
        /// ones somebody would confuse with a letter.
        /// </summary>
        [Test]
        public void TheAlphabetIsTheOneRFC4648Names()
        {

            var bytes = new Byte[256];

            for (var i = 0; i < bytes.Length; i++)
                bytes[i] = (Byte) i;

            var base32 = bytes.ToBase32();

            Assert.That(base32.TrimEnd('=').All(character => "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567".Contains(character)),
                        Is.True,
                        base32);

        }

        #endregion

    }

}
