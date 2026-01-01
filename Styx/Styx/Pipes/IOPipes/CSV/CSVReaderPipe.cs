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

using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;

#endregion

namespace org.GraphDefined.Vanaheimr.Styx
{

    #region CSVReaderPipeExtensions

    /// <summary>
    /// Extension methods for CSV pipes.
    /// </summary>
    public static class CSVReaderPipeExtensions
    {

        #region ToCSV(this IEnumerable, IgnoreLines = null, Separators = null, StringSplitOptions = None, ExpectedNumberOfColumns = null, FailOnWrongNumberOfColumns = false, TrimColumns = true)

        /// <summary>
        /// Splits a given strings into elements by a given sperator.
        /// </summary>
        /// <param name="SourcePipe">An enumeration of strings.</param>
        /// <param name="IgnoreLines">A regular expression indicating which input strings should be ignored. Default: All lines starting with a '#'.</param>
        /// <param name="Separators">An array of string used to split the input strings.</param>
        /// <param name="StringSplitOptions">Split options, e.g. remove empty entries.</param>
        /// <param name="ExpectedNumberOfColumns">If the CSV file had a schema, a specific number of columns can be expected. If instead it is a list of values no such value can be expected.</param>
        /// <param name="FailOnWrongNumberOfColumns">What to do when the current and expected number of columns do not match.</param>
        /// <param name="TrimColumns">Remove leading and trailing whitespaces.</param>
        /// <returns>An enumeration of string arrays.</returns>
        public static IEndPipe<String[]> ToCSV(this IEndPipe<String>    SourcePipe,
                                                    Regex               IgnoreLines                = null,
                                                    String[]            Separators                 = null,
                                                    StringSplitOptions  StringSplitOptions         = StringSplitOptions.None,
                                                    UInt16?             ExpectedNumberOfColumns    = null,
                                                    Boolean             FailOnWrongNumberOfColumns = false,
                                                    Boolean             TrimColumns                = true)
        {
            return new CSVReaderPipe(SourcePipe, IgnoreLines, Separators, StringSplitOptions, ExpectedNumberOfColumns, FailOnWrongNumberOfColumns, TrimColumns);
        }

        #endregion

    }

    #endregion

    #region CSVReaderPipe

    /// <summary>
    /// Splits a given strings into elements by a given sperator.
    /// The side effect is the current line number
    /// </summary>
    public class CSVReaderPipe : AbstractSideEffectPipe<String, String[], UInt64>
    {

        #region Data

        private readonly Regex               IgnoreLines;  
        private readonly String[]            Separators;
        private readonly StringSplitOptions  StringSplitOptions;
        private readonly UInt16?             ExpectedNumberOfColumns;
        private readonly Boolean             FailOnWrongNumberOfColumns;
        private readonly Boolean             TrimColumns;

        private readonly Regex               EmptyColumRegex;

        private          String              _CurrentLine;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Splits a given strings into elements by a given sperator.
        /// </summary>
        /// <param name="SourcePipe"></param>
        /// <param name="IgnoreLines">A regular expression indicating which input strings should be ignored. Default: All lines starting with a '#'.</param>
        /// <param name="Separators">An array of string used to split the input strings.</param>
        /// <param name="StringSplitOptions">Split options, e.g. remove empty entries.</param>
        /// <param name="ExpectedNumberOfColumns">If the CSV file had a schema, a specific number of columns can be expected. If instead it is a list of values no such value can be expected.</param>
        /// <param name="FailOnWrongNumberOfColumns">What to do when the current and expected number of columns do not match.</param>
        /// <param name="TrimColumns">Remove leading and trailing whitespaces.</param>
        public CSVReaderPipe(IEndPipe<String>    SourcePipe,
                             Regex               IgnoreLines                = null,
                             String[]            Separators                 = null,
                             StringSplitOptions  StringSplitOptions         = StringSplitOptions.None,
                             UInt16?             ExpectedNumberOfColumns    = null,
                             Boolean             FailOnWrongNumberOfColumns = false,
                             Boolean             TrimColumns                = true)

            : base(SourcePipe, 0UL)

        {

            this.IgnoreLines                = (IgnoreLines is null) ? new Regex(@"^\#")  : IgnoreLines;
            this.Separators                 = (Separators  is null) ? new String[] {","} : Separators;
            this.StringSplitOptions         = StringSplitOptions;
            this.ExpectedNumberOfColumns    = ExpectedNumberOfColumns;
            this.FailOnWrongNumberOfColumns = FailOnWrongNumberOfColumns;
            this.TrimColumns                = TrimColumns;

            this.EmptyColumRegex            = new Regex("\\" + this.Separators[0] + "\\s\\" + this.Separators[0]);

        }

        #endregion


        #region MoveNext()

        /// <summary>
        /// Advances the enumerator to the next element of the collection.
        /// </summary>
        /// <returns>
        /// True if the enumerator was successfully advanced to the next
        /// element; false if the enumerator has passed the end of the
        /// collection.
        /// </returns>
        public override Boolean MoveNext()
        {

            if (SourcePipe is null)
                return false;

            while (true)
            {

                if (SourcePipe.MoveNext())
                {

                    // Remove leading and trailing whitespaces
                    _CurrentLine = SourcePipe.Current.Trim();

                    // The side effect is the current line number
                    InternalSideEffect++;


                    // Ignore empty lines
                    if (_CurrentLine is null | _CurrentLine == "")
                        continue;


                    // Ignore lines matching the IgnoreLines regular expression
                    if (IgnoreLines.IsMatch(_CurrentLine))
                        continue;


                    // Remove patterns like ",  ,"
                    if (StringSplitOptions == StringSplitOptions.RemoveEmptyEntries)
                        _CurrentLine = EmptyColumRegex.Replace(_CurrentLine, Separators[0]);


                    // Split the current line
                    _CurrentElement = _CurrentLine.Split(Separators, StringSplitOptions);


                    // Remove empty arrays
                    if (StringSplitOptions == StringSplitOptions.RemoveEmptyEntries &
                        _CurrentElement.Length == 0)
                        continue;


                    // Original columns having the separator character(s) within quotation marks \"..., ...\"
                    //   might end up as multiple columns. This will fix it!
                    for (var i = 0; i < _CurrentElement.Length; i++)
                    {

                        if (_CurrentElement[i] is null)
                            continue;

                        if (_CurrentElement[i].StartsWith(@"""") &&
                           !_CurrentElement[i].EndsWith  (@""""))
                        {

                            var j=i+1;

                            do
                            {
                                _CurrentElement[i] += _CurrentElement[j];
                                _CurrentElement[j] = null;
                                j++;
                            }
                            while (!_CurrentElement[i].EndsWith(@""""));


                        }

                        if (_CurrentElement[i].StartsWith(@"""") &&
                            _CurrentElement[i].EndsWith  (@""""))
                        {
                            _CurrentElement[i] = _CurrentElement[i].Substring(1, _CurrentElement[i].Length - 2);
                        }

                        _CurrentElement[i] = _CurrentElement[i].Replace(@"""""", @"""");

                    }

                    _CurrentElement = _CurrentElement.Where(column => column is not null).ToArray();

                    // The found number of columns does not fit the expected number
                    if (ExpectedNumberOfColumns is not null &&
                        ExpectedNumberOfColumns != _CurrentElement.Length)
                    {
                        if (FailOnWrongNumberOfColumns)
                            throw new Exception("CVSPipe!");
                        else
                            continue;
                    }


                    // Remove leading and trailing whitespaces from each column
                    if (TrimColumns)
                        _CurrentElement = _CurrentElement.Select(s => s.Trim()).ToArray();

                    return true;

                }

                return false;

            }

        }

        #endregion


        #region (override) ToString()

        /// <summary>
        /// A string representation of this pipe.
        /// </summary>
        public override String ToString()
        {
            return base.ToString() + "<" + SourcePipe.Current + ">";
        }

        #endregion

    }

    #endregion

}
