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
    /// What a command line is typed at and written on: keys in, text out, and
    /// how wide a row is.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The line editor knew one of these, the console of its own process, and
    /// said so in every line of it. That is the right one for a program
    /// somebody started in a terminal, and the wrong one for everybody else
    /// who might want to type at it - somebody signed in over SSH, say, whose
    /// keys arrive as bytes on a channel and whose screen is at the other end
    /// of it. <see cref="SystemConsoleTerminal"/> is the console as it always
    /// was; <see cref="VT100Terminal"/> is a terminal that is only bytes.
    /// </para>
    /// <para>
    /// Nothing in here asks the terminal anything. The editor moves the cursor
    /// along its row and never sends it to a column - see LineView - so a
    /// terminal needs to know how wide it is and nothing about where its
    /// cursor stands, which a terminal at the far end of a network could only
    /// answer slowly, and the console of a Linux process only under a lock
    /// that Console.ReadKey holds.
    /// </para>
    /// <para>
    /// Writing is never awaited: the editor writes while it holds the lock
    /// that keeps a log entry from landing in the middle of a line being
    /// typed, and every thread that logs waits on that lock. A terminal whose
    /// other end is slow queues what it is given rather than making all of
    /// them wait for it.
    /// </para>
    /// </remarks>
    public interface ICLITerminal
    {

        #region Properties

        /// <summary>
        /// How many columns a row has. At least one.
        /// </summary>
        Int32    Width                   { get; }

        /// <summary>
        /// Whether Ctrl+C arrives as a key among the others - a byte from the
        /// keyboard at the far end - rather than as <see cref="Interrupted"/>.
        /// </summary>
        /// <remarks>
        /// The console of a process gets Ctrl+C from the operating system as
        /// a signal, beside the keys and whether or not anybody is reading
        /// them. A terminal that is bytes gets it in the bytes, and then a
        /// command that is running can only be stopped by somebody reading the
        /// keys while it runs - which the command line does for a terminal
        /// that says so here, and leaves alone for one that does not.
        /// </remarks>
        Boolean  InterruptsArriveAsKeys  { get; }

        #endregion

        #region Events

        /// <summary>
        /// Somebody asked for whatever is running to stop, beside the keys:
        /// Ctrl+C at a console, a signal from the far end of a connection.
        /// </summary>
        event Action?  Interrupted;

        /// <summary>
        /// The row has a different width now.
        /// </summary>
        event Action?  Resized;

        #endregion


        #region ReadKeyAsync(CancellationToken)

        /// <summary>
        /// The next key, or null once no key will ever come again: the far end
        /// closed, or said it has nothing more to send.
        /// </summary>
        /// <param name="CancellationToken">An optional token to stop waiting.</param>
        ValueTask<ConsoleKeyInfo?> ReadKeyAsync(CancellationToken CancellationToken = default);

        #endregion

        #region Write(Text, Foreground = null)

        /// <summary>
        /// Write the given text, in the given colour where there is one, and in
        /// the terminal's own colour again afterwards.
        /// </summary>
        /// <remarks>
        /// "\n" is the end of a line, whatever the terminal needs written for
        /// it; "\r" and "\b" move the cursor along its row, as everywhere.
        /// </remarks>
        /// <param name="Text">What to write.</param>
        /// <param name="Foreground">The colour of the text; the terminal's own when null.</param>
        void Write(String         Text,
                   ConsoleColor?  Foreground  = null);

        #endregion

        #region WriteLine(Text = "", Foreground = null)

        /// <summary>
        /// Write the given text and end the line.
        /// </summary>
        /// <param name="Text">What to write.</param>
        /// <param name="Foreground">The colour of the text; the terminal's own when null.</param>
        void WriteLine(String         Text        = "",
                       ConsoleColor?  Foreground  = null)

            => Write(Text + "\n", Foreground);

        #endregion

    }

}
