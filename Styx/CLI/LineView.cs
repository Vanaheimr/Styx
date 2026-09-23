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
    /// What of a command line is shown on the screen - one row, never its last
    /// column - and where the cursor stands in it.
    /// </summary>
    /// <remarks>
    /// <para>
    /// One row, always. The rest of the line editor rests on that: a line
    /// being typed is taken off the screen by clearing the row the cursor is
    /// on and put back by writing that row again, which is how the log gets to
    /// write in the middle of somebody typing. A line that wrapped onto a
    /// second row would leave the first one behind every time - and the cursor
    /// arithmetic did not even get that far: it threw, because the column past
    /// the edge of the buffer is not a column.
    /// </para>
    /// <para>
    /// So a line longer than the row is shown through a window onto it that
    /// follows the cursor. A '&lt;' in its first cell says there is more to the
    /// left, a '&gt;' in its last cell that there is more to the right. The
    /// cursor never stands on either, so what is under it is always the
    /// character Delete would take. The window moves only as far as the cursor
    /// needs it to, so that the text does not slide about under somebody who
    /// is editing the middle of it.
    /// </para>
    /// <para>
    /// The last column is never written. A character written there leaves the
    /// cursor in different places on different terminals - on the next row at
    /// once on some, only with the next character on others - and either way
    /// the row the editor keeps track of would not be the one it is on.
    /// </para>
    /// <para>
    /// One character is one column, as everywhere in this editor.
    /// </para>
    /// </remarks>
    /// <param name="Text">What to write, from the first column: the prompt if it fits, and as much of the line as does.</param>
    /// <param name="CursorColumn">The column the cursor goes to afterwards.</param>
    /// <param name="Offset">The position in the line shown in the first cell of the window, and where the next view starts looking from.</param>
    /// <param name="LineColumn">The column of that first cell: the length of the prompt, or 0 where the prompt did not fit.</param>
    /// <param name="ShowsAll">Whether this is the prompt and the whole line, as it would be written without a window.</param>
    internal readonly record struct LineView(String   Text,
                                             Int32    CursorColumn,
                                             Int32    Offset,
                                             Int32    LineColumn,
                                             Boolean  ShowsAll)
    {

        #region Data

        /// <summary>
        /// The fewest cells a line gets beside its prompt: a marker on each side
        /// and one character between them. With fewer than that, the prompt
        /// makes way.
        /// </summary>
        public const Int32 MinimumRoom = 3;

        #endregion

        #region (static) Of(Prompt, Input, Cursor, Width, Offset = 0)

        /// <summary>
        /// The row for the given line with the cursor at the given position.
        /// </summary>
        /// <param name="Prompt">What stands in front of the line.</param>
        /// <param name="Input">The line as it has been typed so far.</param>
        /// <param name="Cursor">The position of the cursor in it, from 0 up to its length.</param>
        /// <param name="Width">How many columns the console has.</param>
        /// <param name="Offset">Where the window started the last time; 0 for a new line.</param>
        public static LineView Of(String               Prompt,
                                  IReadOnlyList<Char>  Input,
                                  Int32                Cursor,
                                  Int32                Width,
                                  Int32                Offset = 0)
        {

            var count   = Input.Count;
            var cursor  = Math.Clamp(Cursor, 0, count);

            // Every column but the last - see the remarks.
            var usable  = Math.Max(1, Width - 1);

            // The prompt, unless it would leave the line no room to speak of:
            // in a console that narrow, what is being typed matters more than
            // the words in front of it.
            var prompt  = usable - Prompt.Length >= MinimumRoom
                              ? Prompt
                              : "";

            // Cells for the line, the one the cursor takes after its last
            // character included.
            var room    = usable - prompt.Length;

            // All of it fits: the ordinary case, and the only one this editor
            // used to know about.
            if (count + 1 <= room)
                return new LineView(prompt + new String(Input.ToArray()),
                                    prompt.Length + cursor,
                                    0,
                                    prompt.Length,
                                    prompt.Length == Prompt.Length);

            // Too narrow for markers as well, and then a plain window.
            var marks   = room >= MinimumRoom;

            // The window flush with the end of the line, the cursor's cell after
            // its last character included, is as far right as it ever goes.
            var last    = count + 1 - room;
            var offset  = Math.Clamp(Offset, 0, last);

            // The positions the cursor may take in this window: all of them but
            // those under a marker.
            var first   = offset + (marks && offset > 0 ? 1 : 0);
            var final   = offset + room - 1 - (marks && offset + room < count ? 1 : 0);

            // The window moves only as far as it takes to bring the cursor back
            // into it: one cell right of the marker on the left, one cell left
            // of the marker on the right - where the other side still needs one.
            if (cursor < first)
                offset = marks
                             ? Math.Max(0, cursor - 1)
                             : cursor;

            else if (cursor > final)
                offset = marks
                             ? cursor - room + 1 + (cursor + 1 < count ? 1 : 0)
                             : cursor - room + 1;

            offset = Math.Clamp(offset, 0, last);

            var cells = new Char[room];

            for (var i = 0; i < room; i++)
                cells[i] = offset + i < count
                               ? Input[offset + i]
                               : ' ';

            if (marks && offset > 0)
                cells[0] = '<';

            if (marks && offset + room < count)
                cells[room - 1] = '>';

            return new LineView(prompt + new String(cells),
                                prompt.Length + cursor - offset,
                                offset,
                                prompt.Length,
                                false);

        }

        #endregion

    }

}
