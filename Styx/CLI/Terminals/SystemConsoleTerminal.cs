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

namespace org.GraphDefined.Vanaheimr.CLI
{

    /// <summary>
    /// The console of this process: what the command line was typed at before
    /// there was anything else, and still is by default.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Exactly what the line editor did with System.Console itself, moved here
    /// and nothing changed: a key from Console.ReadKey, waited for on the
    /// thread that asks - which is why a program runs its command line on a
    /// thread of its own - the width the narrower of the window and the
    /// buffer, and Ctrl+C from Console.CancelKeyPress.
    /// </para>
    /// <para>
    /// Ctrl+C is taken for as long as this lives, and the process is not let
    /// end by it: the command line used to say so in its constructor, for
    /// every command line made, and a program that stops at Ctrl+C says so in
    /// a handler of its own beside this one, as a node does. Disposing of this
    /// lets go of it, which a command line made per session elsewhere could not
    /// do while the handler was its own.
    /// </para>
    /// </remarks>
    public sealed class SystemConsoleTerminal : ICLITerminal,
                                                IDisposable
    {

        #region Data

        private readonly ConsoleCancelEventHandler  onCancelKeyPress;
        private          Int32                      disposed;

        #endregion

        #region Properties

        /// <summary>
        /// The narrower of the window and the buffer, because the cursor can only
        /// be put where both are.
        /// </summary>
        public Int32    Width
            => Math.Max(1, Math.Min(Console.WindowWidth, Console.BufferWidth));

        /// <summary>
        /// No: Ctrl+C at a console is a signal, and arrives as <see cref="Interrupted"/>.
        /// </summary>
        public Boolean  InterruptsArriveAsKeys
            => false;

        #endregion

        #region Events

        /// <inheritdoc />
        public event Action?  Interrupted;

        /// <summary>
        /// Never raised: a console is not told when its window changes, and the
        /// width is asked at every redraw anyway.
        /// </summary>
        public event Action?  Resized { add { } remove { } }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// The console of this process, with Ctrl+C taken until this is disposed of.
        /// </summary>
        public SystemConsoleTerminal()
        {

            onCancelKeyPress = (sender, eventArgs) => {
                eventArgs.Cancel = true;
                Interrupted?.Invoke();
            };

            Console.CancelKeyPress += onCancelKeyPress;

        }

        #endregion


        #region ReadKeyAsync(CancellationToken)

        /// <summary>
        /// The next key, waited for on the calling thread - Console.ReadKey cannot
        /// be waited for any other way. Throws where there is no console to read
        /// from, as Console.ReadKey does.
        /// </summary>
        public ValueTask<ConsoleKeyInfo?> ReadKeyAsync(CancellationToken CancellationToken = default)

            => ValueTask.FromResult<ConsoleKeyInfo?>(Console.ReadKey(intercept: true));

        #endregion

        #region Write(Text, Foreground = null)

        /// <inheritdoc />
        public void Write(String         Text,
                          ConsoleColor?  Foreground  = null)
        {

            if (Foreground is not ConsoleColor colour)
            {
                Console.Write(Text);
                return;
            }

            var previous = Console.ForegroundColor;

            try
            {
                Console.ForegroundColor = colour;
                Console.Write(Text);
            }
            finally
            {
                Console.ForegroundColor = previous;
            }

        }

        #endregion

        #region WriteLine(Text = "", Foreground = null)

        /// <summary>
        /// Write the given text and end the line as this platform ends one -
        /// Console.WriteLine, as the editor always did, so that a console whose
        /// output goes into a file gets the same file as before.
        /// </summary>
        public void WriteLine(String         Text        = "",
                              ConsoleColor?  Foreground  = null)
        {

            if (Foreground is null)
            {
                Console.WriteLine(Text);
                return;
            }

            Write(Text, Foreground);
            Console.WriteLine();

        }

        #endregion

        #region Dispose()

        /// <summary>
        /// Let go of Ctrl+C.
        /// </summary>
        public void Dispose()
        {
            if (Interlocked.Exchange(ref disposed, 1) == 0)
                Console.CancelKeyPress -= onCancelKeyPress;
        }

        #endregion

    }

}
