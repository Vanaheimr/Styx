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

namespace org.GraphDefined.Vanaheimr.CLI
{

    /// <summary>
    /// What a VT100-style terminal sends when a key is pressed, read back into
    /// the keys the line editor knows.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A key is a byte, a few bytes of UTF-8, or an escape sequence - and the
    /// same key is not the same sequence everywhere. PuTTY sends Home as
    /// ESC [ 1 ~ by default and ESC [ H in its xterm mode, xterm sends the
    /// arrows as ESC [ A until an application asks for ESC O A, and Backspace
    /// is DEL on most terminals and BS on some. So every spelling of a key
    /// this editor uses is read as that key, and that is the whole table.
    /// </para>
    /// <para>
    /// A sequence that is not in the table is read and dropped whole. Its
    /// bytes are never typed: F5 pressed by mistake must not leave "[15~"
    /// in the middle of a command.
    /// </para>
    /// <para>
    /// Escape on its own is a sequence that never goes on. Whether more is
    /// coming cannot be known from the bytes, so the terminal asks
    /// <see cref="Flush"/> after a moment of nothing - the same guess every
    /// terminal program makes.
    /// </para>
    /// <para>
    /// Enter is CR, LF or CR LF, and CR LF is one Enter: PuTTY sends CR,
    /// a client feeding a script sends LF, and some send both.
    /// </para>
    /// </remarks>
    internal sealed class VT100KeyDecoder
    {

        #region Data

        private const Byte    ESC                = 0x1B;

        /// <summary>
        /// The longest sequence worth waiting for. Anything longer is somebody
        /// sending nonsense, and is dropped rather than collected without end.
        /// </summary>
        private const Int32   MaxSequenceLength  = 32;

        private readonly Decoder     utf8      = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: false).GetDecoder();
        private readonly List<Byte>  sequence  = [];
        private readonly Char[]      chars     = new Char[2];

        private Boolean  inSequence;
        private Boolean  lastWasCR;

        #endregion

        #region Properties

        /// <summary>
        /// Whether a sequence has begun and not ended: Escape, or the start of
        /// something longer that the next bytes will finish.
        /// </summary>
        public Boolean HasPendingSequence
            => inSequence;

        #endregion


        #region Feed(Bytes)

        /// <summary>
        /// The keys in the given bytes, as far as they go. What is left of a
        /// sequence that has not ended waits for the next bytes, or for
        /// <see cref="Flush"/>.
        /// </summary>
        /// <param name="Bytes">What the terminal sent.</param>
        public IReadOnlyList<ConsoleKeyInfo> Feed(ReadOnlySpan<Byte> Bytes)
        {

            var keys = new List<ConsoleKeyInfo>();

            foreach (var b in Bytes)
                Take(b, keys);

            return keys;

        }

        #endregion

        #region Flush()

        /// <summary>
        /// Nothing more came: Escape where Escape was all there was, and nothing
        /// for the beginning of a longer sequence, which is dropped.
        /// </summary>
        public IReadOnlyList<ConsoleKeyInfo> Flush()
        {

            if (!inSequence)
                return [];

            var lone = sequence.Count == 1;

            EndSequence();

            return lone
                       ? [ Key('\x1b', ConsoleKey.Escape) ]
                       : [];

        }

        #endregion


        #region (private) Take(Byte, Keys)

        private void Take(Byte Byte, List<ConsoleKeyInfo> Keys)
        {

            if (inSequence)
            {
                Continue(Byte, Keys);
                return;
            }

            if (Byte == ESC)
            {
                StartSequence();
                return;
            }

            // CR LF is one Enter.
            if (Byte == 0x0A && lastWasCR)
            {
                lastWasCR = false;
                return;
            }

            lastWasCR = Byte == 0x0D;

            if (Byte < 0x20 || Byte == 0x7F)
            {

                // A control character between the bytes of one character of
                // UTF-8 ends that character, unfinished.
                utf8.Reset();

                Keys.Add(Byte switch {
                    0x0D or 0x0A  => Key('\r',   ConsoleKey.Enter),
                    0x09          => Key('\t',   ConsoleKey.Tab),
                    0x08 or 0x7F  => Key('\b',   ConsoleKey.Backspace),
                    0x00          => Key('\0',   ConsoleKey.Spacebar, Control: true),
                    _             => Key((Char) Byte, ConsoleKey.A + (Byte - 1), Control: true)
                });

                return;

            }

            var count = utf8.GetChars([ Byte ], 0, 1, chars, 0, flush: false);

            for (var i = 0; i < count; i++)
                Keys.Add(Typed(chars[i]));

        }

        #endregion

        #region (private) Continue(Byte, Keys)

        /// <summary>
        /// The next byte of an escape sequence: ESC [ (CSI) followed by
        /// parameters and one final byte, or ESC O (SS3) followed by one.
        /// </summary>
        private void Continue(Byte Byte, List<ConsoleKeyInfo> Keys)
        {

            // ESC and then?
            if (sequence.Count == 1)
            {

                if (Byte is (Byte) '[' or (Byte) 'O')
                {
                    sequence.Add(Byte);
                    return;
                }

                // Escape, and then something else: Escape pressed, and then
                // that - which is also what Alt with a key sends.
                EndSequence();
                Keys.Add(Key('\x1b', ConsoleKey.Escape));
                Take(Byte, Keys);
                return;

            }

            var introducer = sequence[1];

            // SS3: one more byte, and done.
            if (introducer == (Byte) 'O')
            {

                EndSequence();

                switch ((Char) Byte)
                {
                    case 'A':  Keys.Add(Key('\0', ConsoleKey.UpArrow));     break;
                    case 'B':  Keys.Add(Key('\0', ConsoleKey.DownArrow));   break;
                    case 'C':  Keys.Add(Key('\0', ConsoleKey.RightArrow));  break;
                    case 'D':  Keys.Add(Key('\0', ConsoleKey.LeftArrow));   break;
                    case 'H':  Keys.Add(Key('\0', ConsoleKey.Home));        break;
                    case 'F':  Keys.Add(Key('\0', ConsoleKey.End));         break;
                    case 'M':  Keys.Add(Key('\r', ConsoleKey.Enter));       break;   // Enter on the keypad in application mode
                }

                return;

            }

            // CSI: ESC [ [ x is a function key of the Linux console, one byte
            // longer than the rest.
            if (sequence.Count == 2 && Byte == (Byte) '[')
            {
                sequence.Add(Byte);
                return;
            }

            if (sequence.Count == 3 && sequence[2] == (Byte) '[')
            {
                EndSequence();
                return;
            }

            // Parameters and intermediates.
            if (Byte is >= 0x20 and <= 0x3F)
            {

                sequence.Add(Byte);

                if (sequence.Count > MaxSequenceLength)
                    EndSequence();

                return;

            }

            // The final byte.
            if (Byte is >= 0x40 and <= 0x7E)
            {

                var parameters = Encoding.ASCII.GetString(sequence.ToArray(), 2, sequence.Count - 2);

                EndSequence();

                if (Final((Char) Byte, parameters) is ConsoleKeyInfo key)
                    Keys.Add(key);

                return;

            }

            // A control character in the middle of a sequence: the sequence
            // is broken, and the character is itself.
            EndSequence();
            Take(Byte, Keys);

        }

        #endregion

        #region (private static) Final(Final, Parameters)

        /// <summary>
        /// The key a complete CSI sequence stands for, or null for one this
        /// editor has no use for. Modifiers - the ";5" of Ctrl with an arrow -
        /// are read past: the arrow is still the arrow.
        /// </summary>
        private static ConsoleKeyInfo? Final(Char    Final,
                                             String  Parameters)
        {

            switch (Final)
            {

                case 'A':  return Key('\0', ConsoleKey.UpArrow);
                case 'B':  return Key('\0', ConsoleKey.DownArrow);
                case 'C':  return Key('\0', ConsoleKey.RightArrow);
                case 'D':  return Key('\0', ConsoleKey.LeftArrow);
                case 'H':  return Key('\0', ConsoleKey.Home);
                case 'F':  return Key('\0', ConsoleKey.End);

                case '~':

                    var first = Parameters.Split(';')[0];

                    if (!Int32.TryParse(first, out var number))
                        return null;

                    return number switch {
                        1 or 7  => Key('\0', ConsoleKey.Home),
                        2       => Key('\0', ConsoleKey.Insert),
                        3       => Key('\0', ConsoleKey.Delete),
                        4 or 8  => Key('\0', ConsoleKey.End),
                        5       => Key('\0', ConsoleKey.PageUp),
                        6       => Key('\0', ConsoleKey.PageDown),
                        _       => null
                    };

                default:
                    return null;

            }

        }

        #endregion


        #region (private) StartSequence() / EndSequence()

        private void StartSequence()
        {
            utf8.Reset();
            sequence.Clear();
            sequence.Add(ESC);
            inSequence = true;
            lastWasCR  = false;
        }

        private void EndSequence()
        {
            sequence.Clear();
            inSequence = false;
        }

        #endregion

        #region (private static) Key(Char, Key, Control = false) / Typed(Char)

        private static ConsoleKeyInfo Key(Char        Char,
                                          ConsoleKey  Key,
                                          Boolean     Control  = false)

            => new (Char, Key, shift: false, alt: false, control: Control);

        /// <summary>
        /// A character somebody typed. Its ConsoleKey says nothing: the editor
        /// goes by the character, and a ConsoleKey taken from the character
        /// would be wrong in the worst way - '.' is 46, and so is
        /// ConsoleKey.Delete.
        /// </summary>
        private static ConsoleKeyInfo Typed(Char Char)

            => new (Char, 0, shift: false, alt: false, control: false);

        #endregion

    }

}
