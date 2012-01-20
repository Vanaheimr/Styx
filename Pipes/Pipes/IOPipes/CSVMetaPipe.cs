/*
 * Copyright (c) 2010-2011, Achim 'ahzf' Friedland <code@ahzf.de>
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
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;

#endregion

namespace de.ahzf.Pipes
{

    /// <summary>
    /// The CSVMetaPipe splits the lines of the found csv files into pieces.
    /// </summary>
    public class CSVMetaPipe : AbstractPipe<String, String[]>, IMetaPipe
    {

        #region Data

        private readonly FileFilterPipe                               FileFilterPipe;  
        private readonly OpenStreamPipe                               OpenStreamPipe;
        private readonly FuncPipe<Stream, StreamReader>               StreamReaderPipe;
        private readonly FuncPipe<StreamReader, IEnumerable<String>>  GetLinesPipe;
        private readonly UnrollPipe<String>                           UnrollPipe;
        private readonly CSVPipe                                      CSVPipe;

        #endregion

        #region Constructor(s)

        #region CSVMetaPipe(IgnoreLines = null, Seperators = null, StringSplitOptions = None, ExpectedNumberOfColumns = null, FailOnWrongNumberOfColumns = false)

        /// <summary>
        /// The CSVMetaPipe splits the lines of the found csv files into pieces.
        /// </summary>
        /// <param name="IgnoreLines">A regular expression indicating which input strings should be ignored. Default: All lines starting with a '#'.</param>
        /// <param name="Seperators">An array of string used to split the input strings.</param>
        /// <param name="StringSplitOptions">Split options, e.g. remove empty entries.</param>
        /// <param name="ExpectedNumberOfColumns">If the CSV file had a schema, a specific number of columns can be expected. If instead it is a list of values no such value can be expected.</param>
        /// <param name="FailOnWrongNumberOfColumns">What to do when the current and expected number of columns do not match.</param>
        /// <param name="IEnumerable">An optional IEnumerable&lt;S&gt; as element source.</param>
        /// <param name="IEnumerator">An optional IEnumerator&lt;S&gt; as element source.</param>
        public CSVMetaPipe(// Parameters for the FileFilterPipe
                           String              SearchPattern              = "*",
                           SearchOption        SearchOption               = SearchOption.TopDirectoryOnly,
                           FileFilter          FileFilter                 = null,
                           // Parameters for the CSVPipe
                           Regex               IgnoreLines                = null,
                           String[]            Seperators                 = null,
                           StringSplitOptions  StringSplitOptions         = StringSplitOptions.None,
                           UInt16?             ExpectedNumberOfColumns    = null,
                           Boolean             FailOnWrongNumberOfColumns = false,
                           IEnumerable<String> IEnumerable                = null,
                           IEnumerator<String> IEnumerator                = null)

            : base(IEnumerable, IEnumerator)
                           
        {

            this.FileFilterPipe   = new FileFilterPipe (SearchPattern: SearchPattern,
                                                        SearchOption:  SearchOption,
                                                        FileFilter:    FileFilter,
                                                        IEnumerable:   IEnumerable);

            if (IEnumerable == null && IEnumerator == null)
                this.FileFilterPipe.SetSource(Directory.GetCurrentDirectory());

            this.OpenStreamPipe   = new OpenStreamPipe (FileMode.Open,
                                                        FileAccess.Read,
                                                        FileShare.Read,
                                                        64000,
                                                        FileOptions.SequentialScan,
                                                        IEnumerable:   FileFilterPipe);

            this.StreamReaderPipe = new FuncPipe<Stream, StreamReader>((stream) => new StreamReader(stream),
                                                                       IEnumerable: OpenStreamPipe);

            this.GetLinesPipe     = new FuncPipe<StreamReader, IEnumerable<String>>((streamReader) => streamReader.GetLines(),
                                                                                    IEnumerable: StreamReaderPipe);

            this.UnrollPipe       = new UnrollPipe<String>(GetLinesPipe);

            this.CSVPipe          = new CSVPipe(IgnoreLines:                IgnoreLines,
                                                Seperators:                 Seperators,
                                                StringSplitOptions:         StringSplitOptions,
                                                ExpectedNumberOfColumns:    ExpectedNumberOfColumns,
                                                FailOnWrongNumberOfColumns: FailOnWrongNumberOfColumns,
                                                IEnumerable:                UnrollPipe);




            //var _FileFilterPipe = new FileFilterPipe("*.csv", SearchOption.TopDirectoryOnly);
            //_FileFilterPipe.SetSource(Directory.GetCurrentDirectory());

            //var _OpenStreamPipe = new OpenStreamPipe(FileMode.Open,
            //                                         FileAccess.Read,
            //                                         FileShare.Read,
            //                                         64000,
            //                                         FileOptions.SequentialScan,
            //                                         IEnumerable: _FileFilterPipe);

            //var _StreamReaderPipe = new FuncPipe<Stream, StreamReader>((stream) => new StreamReader(stream),
            //                                                           IEnumerable: _OpenStreamPipe);

            //var _GetLinesPipe = new FuncPipe<StreamReader, IEnumerable<String>>((streamReader) => streamReader.GetLines(),
            //                                                                     IEnumerable: _StreamReaderPipe);

            //var _UnrollPipe = new UnrollPipe<String>(_GetLinesPipe);

            //var _CSVPipe = new CSVPipe(Seperators: new String[1] { "³" }, IEnumerable: _UnrollPipe);


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

            if (CSVPipe.MoveNext())
            {
                _CurrentElement = CSVPipe.Current;
                return true;
            }

            return false;

        }

        #endregion

        public override IPipe<String, String[]> SetSource(String SourceElement)
        {

            this.FileFilterPipe.SetSource(SourceElement);

            return this;

        }


        #region ToString()

        /// <summary>
        /// A string representation of this pipe.
        /// </summary>
        public override String ToString()
        {
            return base.ToString() + "<" + _InternalEnumerator.Current + ">";
        }

        #endregion


        public IEnumerable<IPipe> Pipes
        {
            get { throw new NotImplementedException(); }
        }

    }

}
