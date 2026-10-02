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
using System.Text.RegularExpressions;

#endregion

namespace org.GraphDefined.Vanaheimr.CLI.Tests
{

    /// <summary>
    /// The far end of a <see cref="VT100Terminal"/>: everything it was sent,
    /// and the screen that comes to - rows of text, and a cursor.
    /// </summary>
    /// <remarks>
    /// As much of a terminal as the line editor uses and no more: printable
    /// characters overwrite where the cursor is, "\r" goes to the start of the
    /// row, "\b" one column back, "\n" to the next row, and colours are read
    /// past. A row is as long as what was written on it; the width the editor
    /// was told is what keeps it from being longer.
    /// </remarks>
    internal sealed class Screen
    {

        #region Data

        private readonly Lock           padlock   = new ();
        private readonly StringBuilder  received  = new ();

        #endregion

        #region Properties

        /// <summary>
        /// Everything sent so far, as it was sent.
        /// </summary>
        public String Received
        {
            get
            {
                lock (padlock)
                {
                    return received.ToString();
                }
            }
        }

        /// <summary>
        /// The rows of the screen, trailing blanks taken off.
        /// </summary>
        public IReadOnlyList<String> Rows
            => Render().Rows;

        /// <summary>
        /// The row the cursor is on.
        /// </summary>
        public String CursorRow
        {
            get
            {
                var (rows, row, _) = Render();
                return rows[row];
            }
        }

        /// <summary>
        /// The column the cursor is in.
        /// </summary>
        public Int32 CursorColumn
            => Render().Column;

        #endregion


        #region SendAsync(Bytes, CancellationToken)

        /// <summary>
        /// What the terminal is handed to send with.
        /// </summary>
        public ValueTask SendAsync(ReadOnlyMemory<Byte> Bytes, CancellationToken CancellationToken)
        {

            lock (padlock)
            {
                received.Append(Encoding.UTF8.GetString(Bytes.Span));
            }

            return ValueTask.CompletedTask;

        }

        #endregion

        #region WaitForAsync(Condition, Timeout = 5 s)

        /// <summary>
        /// Wait until the screen says what the given condition asks for, and fail
        /// where it does not within the given time.
        /// </summary>
        public async Task WaitForAsync(Func<Screen, Boolean>  Condition,
                                       String                 What,
                                       TimeSpan?              Timeout = null)
        {

            var until = DateTime.UtcNow + (Timeout ?? TimeSpan.FromSeconds(5));

            while (!Condition(this))
            {

                if (DateTime.UtcNow > until)
                    Assert.Fail($"Waited in vain for {What}. The screen:\n{String.Join("\n", Rows)}");

                await Task.Delay(10);

            }

        }

        #endregion


        #region (private) Render()

        private (List<String> Rows, Int32 Row, Int32 Column) Render()
        {

            var text    = Regex.Replace(Received, "\x1b\\[[0-9;]*m", "");
            var rows    = new List<StringBuilder> { new () };
            var row     = 0;
            var column  = 0;

            foreach (var c in text)
            {
                switch (c)
                {

                    case '\r':
                        column = 0;
                        break;

                    case '\n':
                        row++;
                        if (row == rows.Count)
                            rows.Add(new StringBuilder());
                        break;

                    case '\b':
                        column = Math.Max(0, column - 1);
                        break;

                    default:
                        var line = rows[row];
                        while (line.Length <= column)
                            line.Append(' ');
                        line[column] = c;
                        column++;
                        break;

                }
            }

            return ([.. rows.Select(line => line.ToString().TrimEnd())], row, column);

        }

        #endregion

    }

}
