/*
 * Copyright (c) 2010-2024 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// Extensions to the Stream class.
    /// </summary>
    public static class StreamExtensions
    {

        #region SeekAndCopyTo(this InputStream, DestinationStream, SkipFromBeginning)

        /// <summary>
        /// Reads the bytes from the given stream and writes them to another stream.
        /// May skip the given number of bytes.
        /// </summary>
        /// <param name="InputStream">The source stream.</param>
        /// <param name="DestinationStream">The destination stream.</param>
        /// <param name="SkipFromBeginning">Anumber of bytes to skip from the beginning of the source stream.</param>
        public static void SeekAndCopyTo(this Stream  InputStream,
                                         Stream       DestinationStream,
                                         Int32        SkipFromBeginning)
        {
            if (InputStream.CanSeek)
            {
                InputStream.Seek(SkipFromBeginning, SeekOrigin.Begin);
                InputStream.CopyTo(DestinationStream);
            }
        }

        #endregion

        #region ToByteArray  (this InputStream)

        /// <summary>
        /// Copy the content of the given stream into a byte array.
        /// </summary>
        /// <param name="InputStream">The source stream.</param>
        public static Byte[] ToByteArray(this Stream InputStream)
        {

            if (InputStream.CanSeek)
            {
                var bytes = new Byte[InputStream.Length];
                InputStream.Seek(0, SeekOrigin.Begin);
                InputStream.Read(bytes);
                return bytes;
            }

            else
            {
                using (var memoryStream = new MemoryStream())
                {
                    InputStream.CopyTo(memoryStream);
                    return memoryStream.ToArray();
                }
            }

        }

        #endregion

    }

}
