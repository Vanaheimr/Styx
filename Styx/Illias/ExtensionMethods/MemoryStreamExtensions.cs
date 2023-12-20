/*
 * Copyright (c) 2010-2023 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

using System.Text;
using System.Buffers.Binary;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// Extensions to memory streams.
    /// </summary>
    public static class MemoryStreamExtensions
    {

        #region ReadUInt16 (this Stream)

        public static UInt16 ReadUInt16(this MemoryStream Stream)
        {

            if (Stream.Length - Stream.Position < 2)
                throw new InvalidOperationException("Not enough data in the stream to read an UInt16!");

            Span<Byte> buffer = stackalloc Byte[2];
            var read = Stream.Read(buffer);

            if (read < 2)
                throw new InvalidOperationException("Not enough data in the stream to read an UInt16!");

            return BinaryPrimitives.ReadUInt16BigEndian(buffer);

        }

        #endregion

        #region WriteUInt16(this Stream, Number)

        public static void WriteUInt16(this MemoryStream Stream, UInt16 Number)
        {
            Span<Byte> buffer = stackalloc Byte[2];
            BinaryPrimitives.WriteUInt16BigEndian(buffer, Number);
            Stream.Write(buffer);
        }

        #endregion


        #region ReadUInt32 (this Stream)

        public static UInt32 ReadUInt32(this MemoryStream Stream)
        {

            if (Stream.Length - Stream.Position < 4)
                throw new InvalidOperationException("Not enough data in the stream to read an UInt32!");

            Span<Byte> buffer = stackalloc Byte[4];
            var read = Stream.Read(buffer);

            if (read < 4)
                throw new InvalidOperationException("Not enough data in the stream to read an UInt32!");

            return BinaryPrimitives.ReadUInt32BigEndian(buffer);

        }

        #endregion

        #region WriteUInt32(this Stream, Number)

        public static void WriteUInt32(this MemoryStream Stream, UInt32 Number)
        {
            Span<Byte> buffer = stackalloc Byte[4];
            BinaryPrimitives.WriteUInt32BigEndian(buffer, Number);
            Stream.Write(buffer);
        }

        #endregion


        #region ReadUInt64 (this Stream)

        public static UInt64 ReadUInt64(this MemoryStream Stream)
        {

            if (Stream.Length - Stream.Position < 8)
                throw new InvalidOperationException("Not enough data in the stream to read an UInt64!");

            Span<Byte> buffer = stackalloc Byte[8];
            var read = Stream.Read(buffer);

            if (read < 8)
                throw new InvalidOperationException("Not enough data in the stream to read an UInt64!");

            return BinaryPrimitives.ReadUInt64BigEndian(buffer);

        }

        #endregion

        #region WriteUInt64(this Stream, Number)

        public static void WriteUInt64(this MemoryStream Stream, UInt64 Number)
        {
            Span<Byte> buffer = stackalloc Byte[8];
            BinaryPrimitives.WriteUInt64BigEndian(buffer, Number);
            Stream.Write(buffer);
        }

        #endregion



        #region ReadUTF8String(this Stream, Length)

        public static String ReadUTF8String(this MemoryStream  Stream,
                                            UInt16             Length)
        {

            if (Length == 0)
                return String.Empty;

            if (Stream.Length - Stream.Position < Length)
                throw new InvalidOperationException($"Not enough data in the stream to read a string of length {Length}!");

            Span<Byte> buffer = stackalloc Byte[Length];
            var read = Stream.Read(buffer);

            if (read < Length)
                throw new InvalidOperationException($"Not enough data in the stream to read a string of length {Length}!");

            return Encoding.UTF8.GetString(buffer);

        }

        #endregion

        #region ReadBytes     (this Stream, Length)

        public static Byte[] ReadBytes(this MemoryStream  Stream,
                                       UInt64             Length)
        {

            if (Length == 0)
                return [];

            if (Stream.Length - Stream.Position < (Int32) Length)
                throw new InvalidOperationException($"Not enough data in the stream to read a byte array of length {Length}!");

            var buffer = new Byte[Length];
            var read = Stream.Read(buffer);

            if (read < (Int32) Length)
                throw new InvalidOperationException($"Not enough data in the stream to read a byte array of length {Length}!");

            return buffer;

        }

        #endregion


    }

}
