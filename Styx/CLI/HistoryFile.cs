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
using System.Collections.Concurrent;

#endregion

namespace org.GraphDefined.Vanaheimr.CLI
{

    /// <summary>
    /// The history of a command line kept in a file between sessions, as a
    /// shell keeps it: one command line per line, the newest last, the oldest
    /// let go of beyond the last so many.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Every line is written as it is given, and not when the session ends: a
    /// session over a connection that breaks never ends in any way that could
    /// be relied on to write anything. The file is written whole to a file
    /// beside it and moved over it, so that whoever reads it finds the old
    /// lines or the new ones and never half of them; command lines that share
    /// the file - two sessions of the same account - take turns at it.
    /// </para>
    /// <para>
    /// The file and the directory it is made in are the owner's alone where
    /// the platform can say so in one call; on Windows they inherit the ACL of
    /// the directory above. A history file is not a place for secrets all the
    /// same: what a command says may not be kept, is not given to it - see
    /// ICLICommand.MayBeKept.
    /// </para>
    /// <para>
    /// Nothing that goes wrong with the file stops a command: a history that
    /// cannot be read starts empty, a line that cannot be written is the only
    /// one lost.
    /// </para>
    /// </remarks>
    /// <param name="Path">The file.</param>
    /// <param name="MaxLines">How many of the newest lines are kept.</param>
    public sealed class HistoryFile(String  Path,
                                    UInt16  MaxLines = HistoryFile.DefaultMaxLines)
    {

        #region Data

        /// <summary>
        /// How many lines are kept, unless it is said otherwise: as many as
        /// bash keeps in memory.
        /// </summary>
        public const UInt16 DefaultMaxLines = 1000;

        /// <summary>
        /// One lock per file, for every command line of this process that
        /// keeps its history in it.
        /// </summary>
        private static readonly ConcurrentDictionary<String, Lock> locks = new (OperatingSystem.IsWindows() || OperatingSystem.IsMacOS()
                                                                                    ? StringComparer.OrdinalIgnoreCase
                                                                                    : StringComparer.Ordinal);

        private static readonly UTF8Encoding utf8 = new (encoderShouldEmitUTF8Identifier: false);

        #endregion

        #region Properties

        /// <summary>
        /// The file.
        /// </summary>
        public String  Path        { get; } = System.IO.Path.GetFullPath(Path);

        /// <summary>
        /// How many of the newest lines are kept.
        /// </summary>
        public UInt16  MaxLines    { get; } = MaxLines;

        #endregion


        #region Read()

        /// <summary>
        /// The lines kept, the oldest first; none where there is no file yet,
        /// or it cannot be read.
        /// </summary>
        public IReadOnlyList<String> Read()
        {
            lock (LockOf(Path))
            {
                return ReadLines();
            }
        }

        #endregion

        #region Add(Line)

        /// <summary>
        /// Keep the given command line as the newest, unless it is the newest
        /// already, and let go of the oldest beyond MaxLines.
        /// </summary>
        /// <param name="Line">The command line, as it was typed.</param>
        /// <returns>Whether it is in the file now. Not a line that would not be one line in it.</returns>
        public Boolean Add(String Line)
        {

            if (String.IsNullOrWhiteSpace(Line) || Line.Any(Char.IsControl))
                return false;

            lock (LockOf(Path))
            {

                var lines = ReadLines().ToList();

                if (lines.Count > 0 && lines[^1] == Line)
                    return true;

                lines.Add(Line);

                if (lines.Count > MaxLines)
                    lines.RemoveRange(0, lines.Count - MaxLines);

                return Write(lines);

            }

        }

        #endregion


        #region (private) ReadLines()

        private List<String> ReadLines()
        {

            try
            {

                if (!File.Exists(Path))
                    return [];

                var lines = File.ReadAllLines(Path, utf8).
                                 Where (line => line.Length > 0).
                                 ToList();

                return lines.Count > MaxLines
                           ? lines[^MaxLines..]
                           : lines;

            }
            catch (Exception)
            {
                return [];
            }

        }

        #endregion

        #region (private) Write(Lines)

        private Boolean Write(IEnumerable<String> Lines)
        {

            var next = $"{Path}.{System.Environment.ProcessId}.next";

            try
            {

                var directory = System.IO.Path.GetDirectoryName(Path);

                if (directory is not null && !Directory.Exists(directory))
                {
                    if (OperatingSystem.IsWindows())
                        Directory.CreateDirectory(directory);
                    else
                        Directory.CreateDirectory(directory, UnixFileMode.UserRead | UnixFileMode.UserWrite | UnixFileMode.UserExecute);
                }

                // Made anew, so that it is made with the mode below: a file of
                // the same name left by a process that died before its move
                // would keep whatever mode it had.
                File.Delete(next);

                var options = new FileStreamOptions {
                                  Mode    = FileMode.CreateNew,
                                  Access  = FileAccess.Write
                              };

                // Not even null on Windows, which refuses the property itself.
                if (!OperatingSystem.IsWindows())
                    options.UnixCreateMode = UnixFileMode.UserRead | UnixFileMode.UserWrite;

                using (var file   = new FileStream(next, options))
                using (var writer = new StreamWriter(file, utf8))
                {
                    foreach (var line in Lines)
                    {
                        writer.Write(line);
                        writer.Write('\n');
                    }
                }

                File.Move(next, Path, overwrite: true);

                return true;

            }
            catch (Exception)
            {

                try
                {
                    File.Delete(next);
                }
                catch (Exception)
                { }

                return false;

            }

        }

        #endregion

        #region (private static) LockOf(Path)

        private static Lock LockOf(String Path)

            => locks.GetOrAdd(Path, _ => new Lock());

        #endregion


        #region (override) ToString()

        public override String ToString()

            => Path;

        #endregion

    }

}
