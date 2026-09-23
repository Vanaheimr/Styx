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

namespace org.GraphDefined.Vanaheimr.CLI.Tests
{

    /// <summary>
    /// What of a command line is on the screen: the whole of it where it fits,
    /// a window onto it where it does not - and one row either way.
    /// </summary>
    /// <remarks>
    /// This is the arithmetic the line editor used to do inline, and got wrong
    /// for a line wider than the console: the cursor was sent to a column past
    /// the edge of the buffer, which threw and took the command line with it.
    /// It is a function of a few numbers now, so the claims about it are asked
    /// of every combination of them rather than of the three somebody thought
    /// of. The screen itself is not needed for any of it.
    /// </remarks>
    [TestFixture]
    public class LineView_Tests
    {

        #region Data

        private const String Prompt = "EV> ";

        /// <summary>
        /// A line of the given length made of letters, so that neither a space
        /// nor a marker can be mistaken for part of it.
        /// </summary>
        private static List<Char> Line(Int32 Length)

            => [.. Enumerable.Range(0, Length).Select(i => (Char) ('a' + i % 26))];

        /// <summary>
        /// Every width from a console of one column up to one wider than any
        /// line here, every length from nothing to more than two rows' worth,
        /// every cursor position, and a window that starts at the beginning,
        /// in the middle and at the end.
        /// </summary>
        private static IEnumerable<(Int32 Width, List<Char> Input, Int32 Cursor, Int32 Offset)> EveryCase()
        {
            for (var width = 1; width <= 40; width++)
                for (var length = 0; length <= 90; length++)
                {
                    var input = Line(length);
                    for (var cursor = 0; cursor <= length; cursor++)
                        foreach (var offset in new[] { 0, length / 2, length })
                            yield return (width, input, cursor, offset);
                }
        }

        #endregion


        #region A_line_that_fits_is_shown_whole()

        /// <summary>
        /// The ordinary case, and the one the editor always knew: the prompt,
        /// the line, and the cursor where it is in it.
        /// </summary>
        [Test]
        public void A_line_that_fits_is_shown_whole()
        {

            var view = LineView.Of(Prompt, [.. "discover"], 3, 80);

            Assert.Multiple(() => {
                Assert.That(view.Text,          Is.EqualTo("EV> discover"));
                Assert.That(view.CursorColumn,  Is.EqualTo(Prompt.Length + 3));
                Assert.That(view.Offset,        Is.Zero);
                Assert.That(view.ShowsAll,      Is.True);
            });

        }

        #endregion

        #region The_line_the_prompt_used_to_break_on()

        /// <summary>
        /// Ninety characters behind a prompt of seventeen, in eighty columns -
        /// measured to throw "Parameter 'left', actual value was 80" before.
        /// </summary>
        /// <remarks>
        /// The end of the line is on the screen, where the cursor is, with a
        /// marker in front of it saying that the start is not; and the cursor
        /// is inside the row, clear of its last column.
        /// </remarks>
        [Test]
        public void The_line_the_prompt_used_to_break_on()
        {

            var prompt  = "ChargingStation> ";
            var input   = new List<Char>(new String('x', 89) + "y");
            var view    = LineView.Of(prompt, input, input.Count, 80);

            Assert.Multiple(() => {

                Assert.That(view.Text,          Does.StartWith(prompt + "<"));
                Assert.That(view.Text.TrimEnd(), Does.EndWith("xy"));
                Assert.That(view.Text.Length,   Is.LessThanOrEqualTo(79));
                Assert.That(view.CursorColumn,  Is.EqualTo(78),
                            "The cursor belongs after the last character, and that is the last column but one.");
                Assert.That(view.ShowsAll,      Is.False);

            });

        }

        #endregion

        #region The_last_column_is_never_written_and_the_cursor_never_leaves_the_row()

        /// <summary>
        /// Whatever the line, the width and the cursor: at most every column but
        /// the last, and the cursor on one of those.
        /// </summary>
        /// <remarks>
        /// The first half is what keeps the line on one row on every terminal,
        /// whether it wraps as soon as the last column is written or only with
        /// the next character. The second is the one the editor broke: a
        /// cursor column the console does not have.
        /// </remarks>
        [Test]
        public void The_last_column_is_never_written_and_the_cursor_never_leaves_the_row()
        {

            foreach (var (width, input, cursor, offset) in EveryCase())
            {

                var view   = LineView.Of(Prompt, input, cursor, width, offset);
                var usable = Math.Max(1, width - 1);

                if (view.Text.Length > usable || view.CursorColumn < 0 || view.CursorColumn >= usable)
                    Assert.Fail($"width {width}, {input.Count} character(s), cursor at {cursor}, window from {offset}: " +
                                $"'{view.Text}' ({view.Text.Length}), cursor in column {view.CursorColumn}");

            }

        }

        #endregion

        #region Every_cell_shows_what_is_there_and_the_cursor_is_never_on_a_marker()

        /// <summary>
        /// Every cell of the window is the character at its position, a space
        /// after the end, or a marker for what the window does not show - and
        /// the cursor is where its position is, never on a marker.
        /// </summary>
        /// <remarks>
        /// What is under the cursor is what Delete would take, which is the
        /// promise a line editor makes whether or not it scrolls. A marker
        /// under the cursor would hide exactly that character.
        /// </remarks>
        [Test]
        public void Every_cell_shows_what_is_there_and_the_cursor_is_never_on_a_marker()
        {

            foreach (var (width, input, cursor, offset) in EveryCase())
            {

                var view = LineView.Of(Prompt, input, cursor, width, offset);

                String Where() => $"width {width}, {input.Count} character(s), cursor at {cursor}, window from {offset}: '{view.Text}'";

                if (view.CursorColumn != view.LineColumn + cursor - view.Offset)
                    Assert.Fail($"{Where()} - the cursor is in column {view.CursorColumn}, and its position says {view.LineColumn + cursor - view.Offset}");

                var cells = view.Text[view.LineColumn..];

                for (var i = 0; i < cells.Length; i++)
                {

                    var position = view.Offset + i;
                    var cell     = cells[i];

                    var expected = position < input.Count ? input[position] : ' ';

                    if (cell == expected)
                        continue;

                    var leftMarker  = i == 0                && cell == '<' && view.Offset > 0;
                    var rightMarker = i == cells.Length - 1 && cell == '>' && position < input.Count - 1;

                    if (!leftMarker && !rightMarker)
                        Assert.Fail($"{Where()} - cell {i} shows '{cell}' where position {position} is '{expected}'");

                    if (view.LineColumn + i == view.CursorColumn)
                        Assert.Fail($"{Where()} - the cursor is on the marker '{cell}'");

                }

            }

        }

        #endregion

        #region The_window_stays_put_while_the_cursor_moves_inside_it()

        /// <summary>
        /// Walking the cursor back from the end of a long line: the window stays
        /// where it is until the cursor reaches its left edge, and from there on
        /// moves one cell with each step - never jumping about.
        /// </summary>
        [Test]
        public void The_window_stays_put_while_the_cursor_moves_inside_it()
        {

            var input   = Line(60);
            var view    = LineView.Of(Prompt, input, input.Count, 30);
            var offsets = new List<Int32> { view.Offset };

            for (var cursor = input.Count - 1; cursor >= 0; cursor--)
            {
                view = LineView.Of(Prompt, input, cursor, 30, view.Offset);
                offsets.Add(view.Offset);
            }

            // 30 columns, 29 of them usable, 25 beside the prompt: at the end,
            // the window holds the last 24 characters and the cell after them.
            Assert.Multiple(() => {

                Assert.That(offsets[0],                                    Is.EqualTo(60 + 1 - 25));

                Assert.That(offsets.Zip(offsets.Skip(1), (a, b) => a - b), Is.All.InRange(0, 1),
                            "The window jumped rather than following the cursor.");

                Assert.That(offsets.Count(o => o == offsets[0]),           Is.EqualTo(25 - 1),
                            "The window did not stay put while the cursor was inside it.");

                Assert.That(offsets[^1],                                   Is.Zero);

            });

        }

        #endregion

        #region A_console_narrower_than_the_prompt_keeps_the_line()

        /// <summary>
        /// Three columns, and a prompt of seventeen characters: the prompt makes
        /// way, and what is being typed is still on the screen, cursor included.
        /// </summary>
        /// <remarks>
        /// The case the programs using this editor measured as "a console no
        /// prompt can be drawn on at all". It can now; it is not much of one.
        /// </remarks>
        [Test]
        public void A_console_narrower_than_the_prompt_keeps_the_line()
        {

            var view = LineView.Of("ChargingStation> ", [.. "sync"], 4, 3);

            Assert.Multiple(() => {
                Assert.That(view.Text,          Does.Not.Contain("ChargingStation"));
                Assert.That(view.Text.Length,   Is.LessThanOrEqualTo(2));
                Assert.That(view.CursorColumn,  Is.InRange(0, 1));
                Assert.That(view.ShowsAll,      Is.False);
            });

        }

        #endregion


        #region A_row_is_cleared_and_drawn_with_plain_characters()

        /// <summary>
        /// What goes to the console, spelt out once: carriage returns, blanks,
        /// the text, and backspaces back to the cursor's column.
        /// </summary>
        [Test]
        public void A_row_is_cleared_and_drawn_with_plain_characters()
        {

            var view = LineView.Of(Prompt, [.. "discover"], 3, 20);

            Assert.Multiple(() => {
                Assert.That(LineView.Clearing(20),  Is.EqualTo("\r" + new String(' ', 19) + "\r"));
                Assert.That(view.Drawing,           Is.EqualTo("EV> discover" + new String('\b', 5)));
            });

        }

        #endregion

        #region A_row_is_drawn_by_walking_the_cursor_along_it()

        /// <summary>
        /// Whatever stood on the row and wherever the cursor was in it: after
        /// Clearing and Drawing the row shows the view and nothing else, and
        /// the cursor is in the view's column.
        /// </summary>
        /// <remarks>
        /// And it got there by walking - a carriage return, characters and
        /// backspaces, nothing a terminal has to be asked about. The editor used
        /// to send the cursor to its column, which took Console.CursorTop; on
        /// Linux that is a question to the terminal which waits for whoever is
        /// in Console.ReadKey, and a log entry from another thread hung until
        /// the next key.
        ///
        /// Played out on a model of one row of a terminal: a character goes
        /// where the cursor is and moves it on, '\r' takes it to the start and
        /// '\b' one back. Anything else fails, and so do a character in the
        /// last column and a backspace from the first.
        /// </remarks>
        [Test]
        public void A_row_is_drawn_by_walking_the_cursor_along_it()
        {

            foreach (var (width, input, cursor, offset) in EveryCase())
            {

                var view    = LineView.Of(Prompt, input, cursor, width, offset);
                var usable  = Math.Max(1, width - 1);

                // What was there before: something in every column the editor
                // writes to, and the cursor anywhere among them.
                var row     = Enumerable.Repeat('#', usable).ToArray();
                var column  = (cursor + offset) % usable;

                String Where() => $"width {width}, {input.Count} character(s), cursor at {cursor}, window from {offset}";

                foreach (var character in LineView.Clearing(width) + view.Drawing)
                {

                    if (character == '\r')
                        column = 0;

                    else if (character == '\b')
                    {
                        if (column == 0)
                            Assert.Fail($"{Where()} - a backspace from the first column");
                        column--;
                    }

                    else if (Char.IsControl(character))
                        Assert.Fail($"{Where()} - 0x{(Int32) character:X2} is neither a character, a carriage return nor a backspace");

                    else
                    {
                        if (column >= usable)
                            Assert.Fail($"{Where()} - '{character}' written into column {column}, and only {usable} are the editor's");
                        row[column++] = character;
                    }

                }

                var shown = new String(row);

                if (shown != view.Text.PadRight(usable) || column != view.CursorColumn)
                    Assert.Fail($"{Where()} - the row reads '{shown}' with the cursor in column {column}, " +
                                $"and the view is '{view.Text}' with the cursor in column {view.CursorColumn}");

            }

        }

        #endregion

    }

}
