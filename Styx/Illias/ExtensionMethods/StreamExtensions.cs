/*
 * Copyright (c) 2010-2024 GraphDefined GmbH <achim.friedland@graphdefined.com> <achim.friedland@graphdefined.com>
 * This file is part of Illias <https://www.github.com/Vanaheimr/Illias>
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

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// Extensions to the Stream class.
    /// </summary>
    public static class StreamExtensions
    {

        #region SeekAndCopyTo(this SourceStream, DestinationStream, SkipFromBeginning)

        /// <summary>
        /// Reads the bytes from the given stream and writes them to another stream.
        /// May skip the given number of bytes.
        /// </summary>
        /// <param name="SourceStream">The source stream.</param>
        /// <param name="DestinationStream">The destination stream.</param>
        /// <param name="SkipFromBeginning">Anumber of bytes to skip from the beginning of the source stream.</param>
        public static void SeekAndCopyTo(this Stream  SourceStream,
                                         Stream       DestinationStream,
                                         Int32        SkipFromBeginning)
        {

            if (SourceStream == null)
                return;

            SourceStream.Seek(SkipFromBeginning, SeekOrigin.Begin);
            SourceStream.CopyTo(DestinationStream);

        }

        #endregion

    }

}
