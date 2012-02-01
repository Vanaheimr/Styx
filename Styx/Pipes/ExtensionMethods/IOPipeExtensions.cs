/*
 * Copyright (c) 2010-2012, Achim 'ahzf' Friedland <code@ahzf.de>
 * This file is part of Pipes.NET <http://www.github.com/ahzf/Pipes.NET>
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
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

#endregion

namespace de.ahzf.Styx
{

    public static class IOPipeExtensions
    {

        #region FileFilterPipe(this IEnumerable, SearchPattern = "*", SearchOption = TopDirectoryOnly, FileFilter = null)

        /// <summary>
        /// Scans the given directories for files matching the given filters.
        /// </summary>
        /// <param name="IEnumerable">An enumeration of directories.</param>
        /// <param name="SearchPattern">A simple search pattern like "*.jpg".</param>
        /// <param name="SearchOption">Include or do not include subdirectories.</param>
        /// <param name="FileFilter">A delegate for filtering the found files.</param>
        /// <returns>An enumeration of file infos.</returns>
        public static IEnumerable<FileInfo> FileFilterPipe(this IEnumerable<String> IEnumerable,
                                                                String              SearchPattern = "*",
                                                                SearchOption        SearchOption  = SearchOption.TopDirectoryOnly,
                                                                FileFilter          FileFilter    = null)
        {
            return new FileFilterPipe(SearchPattern, SearchOption, FileFilter, IEnumerable, null);
        }

        #endregion

        #region FileFilterPipe(this IEnumerator, SearchPattern = "*", SearchOption = TopDirectoryOnly, FileFilter = null)

        /// <summary>
        /// Scans the given directories for files matching the given filters.
        /// </summary>
        /// <param name="IEnumerator">An enumerator of directories.</param>
        /// <param name="SearchPattern">A simple search pattern like "*.jpg".</param>
        /// <param name="SearchOption">Include or do not include subdirectories.</param>
        /// <param name="FileFilter">A delegate for filtering the found files.</param>
        /// <returns>An enumeration of file infos.</returns>
        public static IEnumerable<FileInfo> FileFilterPipe(this IEnumerator<String> IEnumerator,
                                                                String              SearchPattern = "*",
                                                                SearchOption        SearchOption  = SearchOption.TopDirectoryOnly,
                                                                FileFilter          FileFilter    = null)
        {
            return new FileFilterPipe(SearchPattern, SearchOption, FileFilter, null, IEnumerator);
        }

        #endregion


        #region CSVPipe(this IEnumerable, IgnoreLines = null, Seperators = null, StringSplitOptions = None, ExpectedNumberOfColumns = null, FailOnWrongNumberOfColumns = false, TrimColumns = true)

        /// <summary>
        /// Splits a given strings into elements by a given sperator.
        /// </summary>
        /// <param name="IEnumerable">An enumeration of strings.</param>
        /// <param name="IgnoreLines">A regular expression indicating which input strings should be ignored. Default: All lines starting with a '#'.</param>
        /// <param name="Seperators">An array of string used to split the input strings.</param>
        /// <param name="StringSplitOptions">Split options, e.g. remove empty entries.</param>
        /// <param name="ExpectedNumberOfColumns">If the CSV file had a schema, a specific number of columns can be expected. If instead it is a list of values no such value can be expected.</param>
        /// <param name="FailOnWrongNumberOfColumns">What to do when the current and expected number of columns do not match.</param>
        /// <param name="TrimColumns">Remove leading and trailing whitespaces.</param>
        /// <returns>An enumeration of string arrays.</returns>
        public static IEnumerable<String[]> CSVPipe(this IEnumerable<String> IEnumerable,
                                                         Regex               IgnoreLines                = null,
                                                         String[]            Seperators                 = null,
                                                         StringSplitOptions  StringSplitOptions         = StringSplitOptions.None,
                                                         UInt16?             ExpectedNumberOfColumns    = null,
                                                         Boolean             FailOnWrongNumberOfColumns = false,
                                                         Boolean             TrimColumns                = true)
        {
            return new CSVReaderPipe(IgnoreLines, Seperators, StringSplitOptions, ExpectedNumberOfColumns, FailOnWrongNumberOfColumns, TrimColumns, IEnumerable, null);
        }

        #endregion

        #region CSVPipe(this IEnumerator, IgnoreLines = null, Seperators = null, StringSplitOptions = None, ExpectedNumberOfColumns = null, FailOnWrongNumberOfColumns = false, TrimColumns = true)

        /// <summary>
        /// Splits a given strings into elements by a given sperator.
        /// </summary>
        /// <param name="IEnumerator">An enumerator of strings.</param>
        /// <param name="IgnoreLines">A regular expression indicating which input strings should be ignored. Default: All lines starting with a '#'.</param>
        /// <param name="Seperators">An array of string used to split the input strings.</param>
        /// <param name="StringSplitOptions">Split options, e.g. remove empty entries.</param>
        /// <param name="ExpectedNumberOfColumns">If the CSV file had a schema, a specific number of columns can be expected. If instead it is a list of values no such value can be expected.</param>
        /// <param name="FailOnWrongNumberOfColumns">What to do when the current and expected number of columns do not match.</param>
        /// <param name="TrimColumns">Remove leading and trailing whitespaces.</param>
        /// <returns>An enumeration of string arrays.</returns>
        public static IEnumerable<String[]> CSVPipe(this IEnumerator<String> IEnumerator,
                                                         Regex               IgnoreLines                = null,
                                                         String[]            Seperators                 = null,
                                                         StringSplitOptions  StringSplitOptions         = StringSplitOptions.None,
                                                         UInt16?             ExpectedNumberOfColumns    = null,
                                                         Boolean             FailOnWrongNumberOfColumns = false,
                                                         Boolean             TrimColumns                = true)
        {
            return new CSVReaderPipe(IgnoreLines, Seperators, StringSplitOptions, ExpectedNumberOfColumns, FailOnWrongNumberOfColumns, TrimColumns, null, IEnumerator);
        }

        #endregion

    }

}
