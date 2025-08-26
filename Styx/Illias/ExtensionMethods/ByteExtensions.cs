/*
 * Copyright (c) 2010-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

    public delegate T       CustomBinaryParserDelegate<T>    (Byte[]  Binary,     T       DataObject);

    public delegate Byte[]  CustomBinarySerializerDelegate<T>(T       DataObject, Byte[]  Binary);

    /// <summary>
    /// Extensions for byte and byte arrays.
    /// </summary>
    public static class ByteExtensions
    {

        #region SubSequence(this ByteArray, StartIndex  = 0, Length = null)

        /// <summary>
        /// Converts an array of bytes into its hexadecimal string representation.
        /// </summary>
        /// <param name="ByteArray">An array of bytes.</param>
        /// <param name="StartIndex">The zero-based starting byte position of a subsequence in this instance.</param>
        /// <param name="Length">The number of bytes in the subsequence.</param>
        public static Byte[] SubSequence(this Byte[]  ByteArray,
                                         UInt32       StartIndex,
                                         UInt32?      Length = null)
        {

            if (ByteArray is null)
                throw new ArgumentNullException(nameof(ByteArray), "The given byte array must not be null!");

            var array = new Byte[Length ?? ByteArray.Length - StartIndex];

            Array.Copy(ByteArray,
                       StartIndex,
                       array,
                       0,
                       Length ?? ByteArray.Length - StartIndex);

            return array;

        }

        #endregion


        #region Reverse(this ByteArray)

        /// <summary>
        /// Reverse the given byte array.
        /// </summary>
        /// <param name="ByteArray">An array of bytes.</param>
        public static Byte[] Reverse(this Byte[] ByteArray)
        {

            var response = new Byte[ByteArray.Length];

            Array.Copy   (ByteArray, response, ByteArray.Length);
            Array.Reverse(response, 0, response.Length);

            return response;

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
        public static DateTime UNIXTime = new (1970, 1, 1, 0, 0, 0, 0);

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

            => Stream.Write(ByteArray, 0, ByteArray.Length);

        public static async Task WriteAsync(this Stream Stream, Byte[] ByteArray, CancellationToken CancellationToken = default)

            => await Stream.WriteAsync(ByteArray, CancellationToken).ConfigureAwait(false);


        #region Aggregate  (this ByteArrays)

        /// <summary>
        /// Aggregates multiple byte arrays into a single byte array.
        /// </summary>
        /// <param name="ByteArrays">An enumeration of byte arrays.</param>
        public static Byte[] Aggregate(this IEnumerable<Byte[]> ByteArrays)
        {

            var length = ByteArrays.Sum(array => array.Length);
            var result = new Byte[length];
            var offset = 0;

            foreach (var array in ByteArrays)
            {
                Array.Copy(array, 0, result, offset, array.Length);
                offset += array.Length;
            }

            return result;

        }

        #endregion

        #region Pad        (this ByteArray, Size)

        /// <summary>
        /// Pads the given byte array to the given size.
        /// </summary>
        /// <param name="ByteArray">A byte array.</param>
        /// <param name="Size">The size of the resulting byte array.</param>
        public static Byte[] Pad(this Byte[]  ByteArray,
                                 UInt32       Size)
        {

            if (Size <= ByteArray.Length)
                return ByteArray;

            var output = new Byte[Size];
            Buffer.BlockCopy(ByteArray, 0, output, 0, ByteArray.Length);

            return output;

        }

        #endregion

        #region IsEqualTo  (this ByteArray1, ByteArray2)

        /// <summary>
        /// Compares two byte arrays for equality.
        /// </summary>
        public static Boolean IsEqualTo(this Byte[] ByteArray1, Byte[] ByteArray2)
        {

            if (ByteArray1 is null || ByteArray2 is null || ByteArray1.Length != ByteArray2.Length)
                return false;

            for (var i = 0; i < ByteArray1.Length; i++)
            {
                if (ByteArray1[i] != ByteArray2[i])
                    return false;
            }

            return true;

        }

        #endregion

        #region IsPrefixOf (this ByteArray, Data)

        /// <summary>
        /// Checks whether the first byte array is a prefix of the second byte array.
        /// </summary>
        /// <param name="Prefix">An array of bytes that is expected to be a prefix of the 2nd array of bytes.</param>
        /// <param name="Data">An array of bytes that is expected to start with the given prefix.</param>
        public static Boolean IsPrefixOf(this Byte[] Prefix, Byte[] Data)
        {

            if (Prefix.Length > Data.Length)
                return false;

            for (var i = 0; i < Prefix.Length; i++)
            {
                if (Prefix[i] != Data[i])
                    return false;
            }

            return true;

        }

        #endregion

    }

}
