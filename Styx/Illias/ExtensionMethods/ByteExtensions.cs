﻿/*
 * Copyright (c) 2010-2018 Achim 'ahzf' Friedland <achim.friedland@graphdefined.com>
 * This file is part of Illias <http://www.github.com/Vanaheimr/Illias>
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
using System.Linq;
using System.Collections.Generic;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// Extensions for byte and byte arrays.
    /// </summary>
    public static class ByteExtensions
    {

        #region ToHexString(this ByteArray)

        /// <summary>
        /// Converts an array of bytes into its hexadecimal string representation.
        /// </summary>
        /// <param name="ByteArray">An array of bytes.</param>
        public static String ToHexString(this Byte[] ByteArray,
                                         Boolean     ToLower = true)
        {

            Byte b;
            var  c = new Char[ByteArray.Length * 2];

            for(Int32 y=0, x=0; y<ByteArray.Length; ++y, ++x)
            {
                b      = ((byte) (ByteArray[y] >> 4));
                c[x]   =  (char) (b>9 ? b+0x37 : b+0x30);
                b      = ((byte) (ByteArray[y] & 0xF));
                c[++x] =  (char) (b>9 ? b+0x37 : b+0x30);
            }

            return ToLower ? new String(c).ToLower() : new String(c);

        }

        #endregion


        #region HexStringToByteArray(this HexValue)

        /// <summary>
        /// Convert a hex representation of an array of bytes
        /// back into an array of bytes.
        /// </summary>
        /// <param name="HexValue">hex representation of a byte array.</param>
        public static Byte[] HexStringToByteArray(this String HexValue)
        {

            if (HexValue.IsNullOrEmpty())
                return new Byte[0];

            return Enumerable.Range(0, HexValue.Length).
                              Where (x => x % 2 == 0).
                              Select(x => Convert.ToByte(HexValue.Substring(x, 2), 16)).
                              ToArray();

        }

        #endregion

        #region Reverse(this ByteArray)

        /// <summary>
        /// Reverse the given byte array.
        /// </summary>
        /// <param name="ByteArray">An array of bytes.</param>
        public static Byte[] Reverse(this Byte[] ByteArray)
        {
            Array.Reverse(ByteArray, 0, ByteArray.Length);
            return ByteArray;
        }

        #endregion

        #region Reverse(this ByteArray, Skip, Take)

        /// <summary>
        /// Reverse the given byte array.
        /// </summary>
        /// <param name="ByteArray">An array of bytes.</param>
        /// <param name="Skip">Skip the given number of bytes in the beginning.</param>
        /// <param name="Take">Take the given number of bytes.</param>
        public static Byte[] Reverse(this Byte[] ByteArray, UInt32 Skip, UInt32 Take)

            => ByteArray.Skip(Skip).
                         Take(Take).
                         Reverse().
                         ToArray();

        #endregion


        #region ToInt16s<T>(this IEnumerable, NetworkByteOrder = true)

        /// <summary>
        /// Converts the given enumeration of enumerated bytes into an enumeration of Int16s.
        /// </summary>
        /// <param name="IEnumerable">An enumeration of enumerated bytes.</param>
        /// <param name="NetworkByteOrder">Whether the bytes are enumerated in network byte order (default) or not.</param>
        public static IEnumerable<Int16> ToInt16s(this IEnumerable<IEnumerable<Byte>> IEnumerable, Boolean NetworkByteOrder = true)
        {

            if (NetworkByteOrder)
                foreach (var Value in IEnumerable)
                    yield return BitConverter.ToInt16(Value.ToArray(), 0);

            else
                foreach (var Value in IEnumerable)
                    yield return BitConverter.ToInt16(Value.Reverse().ToArray(), 0);

        }

        #endregion

        #region ToUInt16s<T>(this IEnumerable, NetworkByteOrder = true)

        /// <summary>
        /// Converts the given enumeration of enumerated bytes into an enumeration of UInt16s.
        /// </summary>
        /// <param name="IEnumerable">An enumeration of enumerated bytes.</param>
        /// <param name="NetworkByteOrder">Whether the bytes are enumerated in network byte order (default) or not.</param>
        public static IEnumerable<UInt16> ToUInt16s(this IEnumerable<IEnumerable<Byte>> IEnumerable, Boolean NetworkByteOrder = true)
        {

            if (NetworkByteOrder)
                foreach (var Value in IEnumerable)
                    yield return BitConverter.ToUInt16(Value.ToArray(), 0);

            else
                foreach (var Value in IEnumerable)
                    yield return BitConverter.ToUInt16(Value.Reverse().ToArray(), 0);

        }

        #endregion


        #region ToInt32s<T>(this IEnumerable, NetworkByteOrder = true)

        /// <summary>
        /// Converts the given enumeration of enumerated bytes into an enumeration of Int32s.
        /// </summary>
        /// <param name="IEnumerable">An enumeration of enumerated bytes.</param>
        /// <param name="NetworkByteOrder">Whether the bytes are enumerated in network byte order (default) or not.</param>
        public static IEnumerable<Int32> ToInt32s(this IEnumerable<IEnumerable<Byte>> IEnumerable, Boolean NetworkByteOrder = true)
        {

            if (NetworkByteOrder)
                foreach (var Value in IEnumerable)
                    yield return BitConverter.ToInt32(Value.ToArray(), 0);

            else
                foreach (var Value in IEnumerable)
                    yield return BitConverter.ToInt32(Value.Reverse().ToArray(), 0);

        }

        #endregion

        #region ToUInt32s<T>(this IEnumerable, NetworkByteOrder = true)

        /// <summary>
        /// Converts the given enumeration of enumerated bytes into an enumeration of UInt32s.
        /// </summary>
        /// <param name="IEnumerable">An enumeration of enumerated bytes.</param>
        /// <param name="NetworkByteOrder">Whether the bytes are enumerated in network byte order (default) or not.</param>
        public static IEnumerable<UInt32> ToUInt32s(this IEnumerable<IEnumerable<Byte>> IEnumerable, Boolean NetworkByteOrder = true)
        {

            if (NetworkByteOrder)
                foreach (var Value in IEnumerable)
                    yield return BitConverter.ToUInt32(Value.ToArray(), 0);

            else
                foreach (var Value in IEnumerable)
                    yield return BitConverter.ToUInt32(Value.Reverse().ToArray(), 0);

        }

        #endregion


        #region ToInt64s<T>(this IEnumerable, NetworkByteOrder = true)

        /// <summary>
        /// Converts the given enumeration of enumerated bytes into an enumeration of Int64s.
        /// </summary>
        /// <param name="IEnumerable">An enumeration of enumerated bytes.</param>
        /// <param name="NetworkByteOrder">Whether the bytes are enumerated in network byte order (default) or not.</param>
        public static IEnumerable<Int64> ToInt64s(this IEnumerable<IEnumerable<Byte>> IEnumerable, Boolean NetworkByteOrder = true)
        {

            if (NetworkByteOrder)
                foreach (var Value in IEnumerable)
                    yield return BitConverter.ToInt64(Value.ToArray(), 0);

            else
                foreach (var Value in IEnumerable)
                    yield return BitConverter.ToInt64(Value.Reverse().ToArray(), 0);

        }

        #endregion

        #region ToUInt64s<T>(this IEnumerable, NetworkByteOrder = true)

        /// <summary>
        /// Converts the given enumeration of enumerated bytes into an enumeration of UInt64s.
        /// </summary>
        /// <param name="IEnumerable">An enumeration of enumerated bytes.</param>
        /// <param name="NetworkByteOrder">Whether the bytes are enumerated in network byte order (default) or not.</param>
        public static IEnumerable<UInt64> ToUInt64s(this IEnumerable<IEnumerable<Byte>> IEnumerable, Boolean NetworkByteOrder = true)
        {

            if (NetworkByteOrder)
                foreach (var Value in IEnumerable)
                    yield return BitConverter.ToUInt64(Value.ToArray(), 0);

            else
                foreach (var Value in IEnumerable)
                    yield return BitConverter.ToUInt64(Value.Reverse().ToArray(), 0);

        }

        #endregion


        #region ToSingles<T>(this IEnumerable, NetworkByteOrder = true)

        /// <summary>
        /// Converts the given enumeration of enumerated bytes into an enumeration of Singles.
        /// </summary>
        /// <param name="IEnumerable">An enumeration of enumerated bytes.</param>
        /// <param name="NetworkByteOrder">Whether the bytes are enumerated in network byte order (default) or not.</param>
        public static IEnumerable<Single> ToSingles(this IEnumerable<IEnumerable<Byte>> IEnumerable, Boolean NetworkByteOrder = true)
        {

            if (NetworkByteOrder)
                foreach (var Value in IEnumerable)
                    yield return BitConverter.ToSingle(Value.ToArray(), 0);

            else
                foreach (var Value in IEnumerable)
                    yield return BitConverter.ToSingle(Value.Reverse().ToArray(), 0);

        }

        #endregion

        #region ToDoubles<T>(this IEnumerable, NetworkByteOrder = true)

        /// <summary>
        /// Converts the given enumeration of enumerated bytes into an enumeration of Doubles.
        /// </summary>
        /// <param name="IEnumerable">An enumeration of enumerated bytes.</param>
        /// <param name="NetworkByteOrder">Whether the bytes are enumerated in network byte order (default) or not.</param>
        public static IEnumerable<Double> ToDoubles(this IEnumerable<IEnumerable<Byte>> IEnumerable, Boolean NetworkByteOrder = true)
        {

            if (NetworkByteOrder)
                foreach (var Value in IEnumerable)
                    yield return BitConverter.ToDouble(Value.ToArray(), 0);

            else
                foreach (var Value in IEnumerable)
                    yield return BitConverter.ToDouble(Value.Reverse().ToArray(), 0);

        }

        #endregion


        /// <summary>
        /// The beginning of the UNIX universe.
        /// </summary>
        public static DateTime UNIXTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);

        #region ToDateTime32s<T>(this IEnumerable, NetworkByteOrder = true)

        /// <summary>
        /// Converts the given enumeration of four enumerated bytes into an enumeration of DateTimes.
        /// </summary>
        /// <param name="IEnumerable">An enumeration of enumerated bytes.</param>
        /// <param name="NetworkByteOrder">Whether the bytes are enumerated in network byte order (default) or not.</param>
        public static IEnumerable<DateTime> ToDateTime32s(this IEnumerable<IEnumerable<Byte>> IEnumerable, Boolean NetworkByteOrder = true)
        {

            if (NetworkByteOrder)
                foreach (var Value in IEnumerable)
                    yield return UNIXTime.AddSeconds(BitConverter.ToInt32(Value.ToArray(), 0));

            else
                foreach (var Value in IEnumerable)
                    yield return UNIXTime.AddSeconds(BitConverter.ToInt32(Value.Reverse().ToArray(), 0));

        }

        #endregion

        #region ToDateTime64s<T>(this IEnumerable, NetworkByteOrder = true)

        /// <summary>
        /// Converts the given enumeration of eight enumerated bytes into an enumeration of DateTimes.
        /// </summary>
        /// <param name="IEnumerable">An enumeration of enumerated bytes.</param>
        /// <param name="NetworkByteOrder">Whether the bytes are enumerated in network byte order (default) or not.</param>
        public static IEnumerable<DateTime> ToDateTime64s(this IEnumerable<IEnumerable<Byte>> IEnumerable, Boolean NetworkByteOrder = true)
        {

            if (NetworkByteOrder)
                foreach (var Value in IEnumerable)
                    yield return UNIXTime.AddSeconds(BitConverter.ToInt64(Value.ToArray(), 0));

            else
                foreach (var Value in IEnumerable)
                    yield return UNIXTime.AddSeconds(BitConverter.ToInt64(Value.Reverse().ToArray(), 0));

        }

        #endregion


        public static void Write(this Stream Stream, Byte[] ByteArray)
        {
            Stream.Write(ByteArray, 0, ByteArray.Length);
        }

    }

}