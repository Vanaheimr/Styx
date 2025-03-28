﻿/*
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
using System.IO;
using System.Collections.Generic;

#endregion

namespace org.GraphDefined.Vanaheimr.Styx
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
        public static IEnumerable<FileInfo> FileFilterPipe(this IEndPipe<String>  SourcePipe,
                                                                String              SearchPattern = "*",
                                                                SearchOption        SearchOption  = SearchOption.TopDirectoryOnly,
                                                                FileFilter          FileFilter    = null)
        {
            return new FileFilterPipe(SourcePipe, SearchPattern, SearchOption, FileFilter).AsEnumerable();
        }

        #endregion

    }

}
