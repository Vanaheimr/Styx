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

using System.Text;

#endregion

namespace org.GraphDefined.Vanaheimr.CLI.Tests
{

    /// <summary>
    /// What the keys of a terminal that is only bytes come to: every spelling
    /// of every key the editor uses, as PuTTY, xterm and the Linux console send
    /// it - and nothing at all for what the editor has no use for.
    /// </summary>
    [TestFixture]
    public class VT100KeyDecoder_Tests
    {

        #region Data

        private const String ESC = "\x1b";

        #endregion

        #region (static) Keys(Text)

        /// <summary>
        /// The keys the given bytes come to, fed at once.
        /// </summary>
        private static IReadOnlyList<ConsoleKeyInfo> Keys(String Text)

            => new VT100KeyDecoder().Feed(Encoding.UTF8.GetBytes(Text));

        #endregion


        #region TheSpecialKeysInEverySpelling()

        /// <summary>
        /// Every way of sending each key the editor knows.
        /// </summary>
        [TestCase(ESC + "[A",   ConsoleKey.UpArrow,     TestName = "Up, CSI")]
        [TestCase(ESC + "OA",   ConsoleKey.UpArrow,     TestName = "Up, SS3 (application cursor keys)")]
        [TestCase(ESC + "[B",   ConsoleKey.DownArrow,   TestName = "Down, CSI")]
        [TestCase(ESC + "OB",   ConsoleKey.DownArrow,   TestName = "Down, SS3")]
        [TestCase(ESC + "[C",   ConsoleKey.RightArrow,  TestName = "Right, CSI")]
        [TestCase(ESC + "OC",   ConsoleKey.RightArrow,  TestName = "Right, SS3")]
        [TestCase(ESC + "[D",   ConsoleKey.LeftArrow,   TestName = "Left, CSI")]
        [TestCase(ESC + "OD",   ConsoleKey.LeftArrow,   TestName = "Left, SS3")]
        [TestCase(ESC + "[1;5D", ConsoleKey.LeftArrow,  TestName = "Ctrl+Left is still Left")]
        [TestCase(ESC + "[1~",  ConsoleKey.Home,        TestName = "Home, PuTTY's default")]
        [TestCase(ESC + "[H",   ConsoleKey.Home,        TestName = "Home, xterm")]
        [TestCase(ESC + "OH",   ConsoleKey.Home,        TestName = "Home, SS3")]
        [TestCase(ESC + "[7~",  ConsoleKey.Home,        TestName = "Home, rxvt")]
        [TestCase(ESC + "[4~",  ConsoleKey.End,         TestName = "End, PuTTY's default")]
        [TestCase(ESC + "[F",   ConsoleKey.End,         TestName = "End, xterm")]
        [TestCase(ESC + "OF",   ConsoleKey.End,         TestName = "End, SS3")]
        [TestCase(ESC + "[8~",  ConsoleKey.End,         TestName = "End, rxvt")]
        [TestCase(ESC + "[3~",  ConsoleKey.Delete,      TestName = "Delete")]
        [TestCase(ESC + "[2~",  ConsoleKey.Insert,      TestName = "Insert")]
        [TestCase(ESC + "[5~",  ConsoleKey.PageUp,      TestName = "Page up")]
        [TestCase(ESC + "[6~",  ConsoleKey.PageDown,    TestName = "Page down")]
        [TestCase("\r",         ConsoleKey.Enter,       TestName = "Enter, CR")]
        [TestCase("\n",         ConsoleKey.Enter,       TestName = "Enter, LF")]
        [TestCase("\r\n",       ConsoleKey.Enter,       TestName = "Enter, CR LF is one")]
        [TestCase(ESC + "OM",   ConsoleKey.Enter,       TestName = "Enter on the keypad")]
        [TestCase("\t",         ConsoleKey.Tab,         TestName = "Tab")]
        [TestCase("\x7f",       ConsoleKey.Backspace,   TestName = "Backspace, DEL")]
        [TestCase("\b",         ConsoleKey.Backspace,   TestName = "Backspace, BS")]
        public void TheSpecialKeysInEverySpelling(String Sent, ConsoleKey Expected)
        {

            var keys = Keys(Sent);

            Assert.That(keys,           Has.Count.EqualTo(1));
            Assert.That(keys[0].Key,    Is.EqualTo(Expected));

        }

        #endregion

        #region TheControlKeysAreTheirCharacters()

        /// <summary>
        /// Ctrl+A, C, D and E as the characters the editor goes by.
        /// </summary>
        [TestCase("\x01", '\x01')]
        [TestCase("\x03", '\x03')]
        [TestCase("\x04", '\x04')]
        [TestCase("\x05", '\x05')]
        public void TheControlKeysAreTheirCharacters(String Sent, Char Expected)
        {

            var keys = Keys(Sent);

            Assert.That(keys,                Has.Count.EqualTo(1));
            Assert.That(keys[0].KeyChar,     Is.EqualTo(Expected));
            Assert.That(keys[0].Modifiers,   Is.EqualTo(ConsoleModifiers.Control));

        }

        #endregion

        #region TypedTextIsItsCharacters()

        /// <summary>
        /// Text, UTF-8 included, is its characters - and a character never
        /// passes for a special key. '.' is 46, and so is ConsoleKey.Delete.
        /// </summary>
        [Test]
        public void TypedTextIsItsCharacters()
        {

            var keys = Keys("a.Ü±");

            Assert.That(keys.Select(key => key.KeyChar), Is.EqualTo("a.Ü±".ToCharArray()));
            Assert.That(keys.Select(key => key.Key),     Has.None.EqualTo(ConsoleKey.Delete));

        }

        #endregion

        #region AUTF8CharacterSplitAcrossFeedsIsOne()

        /// <summary>
        /// The bytes of one character may arrive in two packets.
        /// </summary>
        [Test]
        public void AUTF8CharacterSplitAcrossFeedsIsOne()
        {

            var decoder  = new VT100KeyDecoder();
            var bytes    = Encoding.UTF8.GetBytes("Ü");

            Assert.That(decoder.Feed(bytes.AsSpan(0, 1)), Is.Empty);

            var keys     = decoder.Feed(bytes.AsSpan(1));

            Assert.That(keys.Select(key => key.KeyChar), Is.EqualTo(new[] { 'Ü' }));

        }

        #endregion

        #region ASequenceSplitAcrossFeedsIsOneKey()

        /// <summary>
        /// The bytes of one sequence may arrive in several packets, and they
        /// still come to one key - not to Escape and some text.
        /// </summary>
        [Test]
        public void ASequenceSplitAcrossFeedsIsOneKey()
        {

            var decoder = new VT100KeyDecoder();

            Assert.Multiple(() => {
                Assert.That(decoder.Feed([ 0x1b ]),                  Is.Empty);
                Assert.That(decoder.HasPendingSequence,              Is.True);
                Assert.That(decoder.Feed("["u8),                     Is.Empty);
                Assert.That(decoder.Feed("3"u8),                     Is.Empty);
                Assert.That(decoder.Feed("~"u8).Single().Key,        Is.EqualTo(ConsoleKey.Delete));
                Assert.That(decoder.HasPendingSequence,              Is.False);
            });

        }

        #endregion

        #region EscapeOnItsOwnIsEscapeOnceNothingFollows()

        /// <summary>
        /// Escape on its own is only known to be Escape once nothing follows it:
        /// the terminal flushes after a moment of nothing.
        /// </summary>
        [Test]
        public void EscapeOnItsOwnIsEscapeOnceNothingFollows()
        {

            var decoder = new VT100KeyDecoder();

            Assert.That(decoder.Feed([ 0x1b ]),             Is.Empty);
            Assert.That(decoder.Flush().Single().Key,        Is.EqualTo(ConsoleKey.Escape));
            Assert.That(decoder.HasPendingSequence,          Is.False);

        }

        #endregion

        #region EscapeAndAnotherKeyAreTwoKeys()

        /// <summary>
        /// Escape followed by something that begins no sequence is Escape, and
        /// then that - which is also what Alt with a key sends.
        /// </summary>
        [Test]
        public void EscapeAndAnotherKeyAreTwoKeys()
        {

            var keys = Keys(ESC + "x");

            Assert.That(keys,              Has.Count.EqualTo(2));
            Assert.That(keys[0].Key,       Is.EqualTo(ConsoleKey.Escape));
            Assert.That(keys[1].KeyChar,   Is.EqualTo('x'));

        }

        #endregion

        #region ASequenceOfNoUseIsDroppedWhole()

        /// <summary>
        /// F5, F1 on the Linux console, a focus report: read and dropped whole,
        /// and nothing of them typed - "[15~" must not end up in a command.
        /// </summary>
        [TestCase(ESC + "[15~",  TestName = "F5")]
        [TestCase(ESC + "[[A",   TestName = "F1 on the Linux console")]
        [TestCase(ESC + "OP",    TestName = "F1, SS3")]
        [TestCase(ESC + "[I",    TestName = "Focus in")]
        [TestCase(ESC + "[Z",    TestName = "Shift+Tab")]
        public void ASequenceOfNoUseIsDroppedWhole(String Sent)
        {

            var keys = Keys(Sent + "a");

            Assert.That(keys.Select(key => key.KeyChar), Is.EqualTo(new[] { 'a' }));

        }

        #endregion

        #region ASequenceWithoutEndIsNotCollectedForEver()

        /// <summary>
        /// Parameters without a final byte are dropped once there are more of
        /// them than any sequence has, rather than collected without end.
        /// </summary>
        [Test]
        public void ASequenceWithoutEndIsNotCollectedForEver()
        {

            var decoder = new VT100KeyDecoder();

            decoder.Feed(Encoding.ASCII.GetBytes(ESC + "[" + new String('1', 100)));

            Assert.That(decoder.HasPendingSequence, Is.False);

        }

        #endregion

    }

}
