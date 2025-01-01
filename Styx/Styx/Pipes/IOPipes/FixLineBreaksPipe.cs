/*
 * Copyright (c) 2010-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
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
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace org.GraphDefined.Vanaheimr.Styx
{


    #region FixLineBreaksPipeExtensions

    /// <summary>
    /// Extension methods for FixLineBreaks pipes.
    /// </summary>
    public static class FixLineBreaksPipeExtensions
    {

        #region FixLineBreaks(this IEnumerable, StartOfNewLineRegExpr, NewLineSeperator = "<br>", IEnumerable = null, IEnumerator = null)

        /// <summary>
        /// Sometimes there are unwanted new line characters within CSV files.
        /// This pipe tries to detect real new lines based on the given regular expression
        /// and concatenates the dangling lines using the given NewLineSeperator.
        /// </summary>
        /// <param name="IEnumerable">An enumeration of lines.</param>
        /// <param name="StartOfNewLineRegExpr">A regular expression detecting real new lines.</param>
        /// <param name="NewLineSeperator">The new line seperator between the prior dangling lines.</param>
        public static IEnumerable<String> FixLineBreaks(this IEndPipe<String>  SourcePipe,
                                                        Regex                  StartOfNewLineRegExpr,
                                                        String                 NewLineSeperator = "<br>")
        {
            return new FixLineBreaksPipe(SourcePipe, StartOfNewLineRegExpr, NewLineSeperator).AsEnumerable();
        }

        #endregion

        #region FixLineBreaks(this IEnumerator, StartOfNewLineRegExprString, NewLineSeperator = "<br>", IEnumerable = null, IEnumerator = null)

        /// <summary>
        /// Sometimes there are unwanted new line characters within CSV files.
        /// This pipe tries to detect real new lines based on the given regular expression
        /// and concatenates the dangling lines using the given NewLineSeperator.
        /// </summary>
        /// <param name="IEnumerator">An enumerator of lines.</param>
        /// <param name="StartOfNewLineRegExprString">A regular expression detecting real new lines.</param>
        /// <param name="NewLineSeperator">The new line seperator between the prior dangling lines.</param>
        public static IEnumerable<String> FixLineBreaks(this IEndPipe<String>  SourcePipe,
                                                        String                 StartOfNewLineRegExprString,
                                                        String                 NewLineSeperator = "<br>")
        {
            return new FixLineBreaksPipe(SourcePipe, StartOfNewLineRegExprString, NewLineSeperator).AsEnumerable();
        }

        #endregion

    }

    #endregion

    #region FixLineBreaksPipe

    public class FixLineBreaksPipe : AbstractPipe<String, String>
    {

        #region Data

        private readonly Regex         StartOfNewLineRegExpr;
        private          Boolean       IgnoreFirstLineMatch;
        private readonly StringBuilder FixedLine;
        private readonly String        NewLineSeperator;

        #endregion

        #region Constructor(s)

        #region FixLineBreaksPipe(SourcePipe, StartOfNewLineRegExpr, NewLineSeperator, IEnumerable = null, IEnumerator = null)

        /// <summary>
        /// Sometimes there are unwanted new line characters within CSV files.
        /// This pipe tries to detect real new lines based on the given regular expression
        /// and concatenates the dangling lines using the given NewLineSeperator.
        /// </summary>
        /// <param name="StartOfNewLineRegExpr">A regular expression detecting real new lines.</param>
        /// <param name="NewLineSeperator">The new line seperator between the prior dangling lines.</param>
        /// <param name="IEnumerable">An enumeration of lines.</param>
        /// <param name="IEnumerator">An enumerator of lines.</param>
        public FixLineBreaksPipe(IEndPipe<String>  SourcePipe,
                                 Regex             StartOfNewLineRegExpr,
                                 String            NewLineSeperator)

            : base(SourcePipe)

        {

            #region Initial checks

            if (StartOfNewLineRegExpr == null)
                throw new ArgumentNullException("StartOfNewLineRegExpr", "The parameter 'StartOfNewLineRegExpr' must not be null!");

            if (NewLineSeperator == null)
                throw new ArgumentNullException("NewLineSeperator", "The parameter 'NewLineSeperator' must not be null!");

            #endregion

            this.StartOfNewLineRegExpr = StartOfNewLineRegExpr;
            this.IgnoreFirstLineMatch  = true;
            this.FixedLine             = new StringBuilder();
            this.NewLineSeperator      = NewLineSeperator;

        }

        #endregion

        #region FixLineBreaksPipe(StartOfNewLineRegExprString, NewLineSeperator, IEnumerable = null, IEnumerator = null)

        /// <summary>
        /// Sometimes there are unwanted new line characters within CSV files.
        /// This pipe tries to detect real new lines based on the given regular expression
        /// and concatenates the dangling lines using the given NewLineSeperator.
        /// </summary>
        /// <param name="StartOfNewLineRegExprString">A regular expression detecting real new lines.</param>
        /// <param name="NewLineSeperator">The new line seperator between the prior dangling lines.</param>
        /// <param name="IEnumerable">An enumeration of lines.</param>
        /// <param name="IEnumerator">An enumerator of lines.</param>
        public FixLineBreaksPipe(IEndPipe<String>  SourcePipe,
                                 String            StartOfNewLineRegExprString,
                                 String            NewLineSeperator)

            : base(SourcePipe)

        {

            #region Initial checks

            if (StartOfNewLineRegExprString.IsNullOrEmpty())
                throw new ArgumentNullException("StartOfNewLineRegExprString", "The parameter 'StartOfNewLineRegExprString' must not be null!");

            if (NewLineSeperator == null)
                throw new ArgumentNullException("NewLineSeperator", "The parameter 'NewLineSeperator' must not be null!");

            #endregion

            this.StartOfNewLineRegExpr = new Regex(StartOfNewLineRegExprString);
            this.IgnoreFirstLineMatch  = true;
            this.FixedLine             = new StringBuilder();
            this.NewLineSeperator      = NewLineSeperator;

        }

        #endregion

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

            if (SourcePipe == null)
                return false;

            while (SourcePipe.MoveNext())
            {

                if (this.StartOfNewLineRegExpr.IsMatch(SourcePipe.Current))
                {

                    if (this.IgnoreFirstLineMatch)
                    {
                        this.IgnoreFirstLineMatch = false;
                        this.FixedLine.Append(SourcePipe.Current).Append(this.NewLineSeperator);
                    }

                    else
                    {
                        _CurrentElement = this.FixedLine.ToString();
                        this.FixedLine.Clear();
                        this.FixedLine.Append(SourcePipe.Current).Append(this.NewLineSeperator);
                        return true;
                    }

                }

                else
                    this.FixedLine.Append(SourcePipe.Current).Append(this.NewLineSeperator);

            }

            return false;

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
