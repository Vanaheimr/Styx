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

#if !SILVERLIGHT

#region Usings

using System;
using System.IO;
using System.Collections.Generic;

#endregion

namespace org.GraphDefined.Vanaheimr.Styx
{

    public class ReadTextStreamPipe : AbstractPipe<Stream, String>
    {

        #region Data

        private StreamReader _StreamReader;

        #endregion

        #region Constructor(s)

        #region ReadTextStreamPipe(FileMode, FileAccess, FileShare, BufferSize, FileOptions)

        public ReadTextStreamPipe(String RegExpr)
        {


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

            if (_StreamReader == null)
            {

                while (SourcePipe.MoveNext())
                {

                    if (SourcePipe.Current != null)
                        continue;

                    _StreamReader = new StreamReader(SourcePipe.Current);
                    break;

                }

            }

            if (_StreamReader != null)
            {

                do
                {

                    try
                    {

                        _CurrentElement = _StreamReader.ReadLine();

                    }

                    catch
                    {
                        return false;
                    }


                }
                while (!_CurrentElement.StartsWith("#"));

                return true;

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

}

#endif

