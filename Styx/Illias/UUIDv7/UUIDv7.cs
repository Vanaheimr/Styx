/*
 * Copyright (c) 2010-2026 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

using System.Security.Cryptography;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// UUID v7 extension methods.
    /// </summary>
    public static class UUIDv7
    {

        #region Generate()

        /// <summary>
        /// Generates a new UUIDv7.
        /// </summary>
        public static Guid Generate()
        {

            // Get current Unix timestamp in milliseconds
            var unixTimeMillis = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            var timestampBytes = BitConverter.GetBytes(unixTimeMillis);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(timestampBytes);

            // Generate 10 random bytes for the remaining part of the UUID
            var randomBytes = new Byte[10];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }

            // Combine timestamp and random bytes
            var uuidBytes = new Byte[16];
            Array.Copy(timestampBytes, 2, uuidBytes, 0,  6); // Timestamp part
            Array.Copy(randomBytes,    0, uuidBytes, 6, 10); // Random part

            // Set version to 7
            uuidBytes[6] = (byte) ((uuidBytes[6] & 0x0F) | 0x70);

            // Set variant to 10xx (RFC 4122)
            uuidBytes[8] = (byte) ((uuidBytes[8] & 0x3F) | 0x80);

            // Create a new GUID from the byte array
            return new Guid(uuidBytes);

        }

        #endregion


        #region IsUUIDv7 (this UUID)

        /// <summary>
        /// Checks whether the given UUID is a valid UUIDv7.
        /// </summary>
        /// <param name="UUID">The UUID to verify.</param>
        public static Boolean IsUUIDv7(this Guid UUID)
            => UUID.ToByteArray().IsUUIDv7();

        #endregion

        #region IsUUIDv7 (this ByteArray)

        /// <summary>
        /// Checks whether the given byte array represents a valid UUIDv7.
        /// </summary>
        /// <param name="ByteArray">The byte array to verify.</param>
        public static Boolean IsUUIDv7(this Byte[] ByteArray)
        {

            if (ByteArray.Length != 16)
                return false;

            // Check version (bits 48–51, upper nibble of byte 6) is 7
            if ((ByteArray[6] & 0xF0) != 0x70)
                return false;

            // Check variant (bits 64–65, high bits of byte 8) is 0b10
            if ((ByteArray[8] & 0xC0) != 0x80)
                return false;

            return true;

        }

        #endregion

        #region IsUUIDv7 (this Text)

        /// <summary>
        /// Checks whether the given text represents a valid UUIDv7.
        /// </summary>
        /// <param name="Text">The text to verify.</param>
        public static Boolean IsUUIDv7(this String Text)
        {

            if (!Guid.TryParse(Text, out var uuid))
                return false;

            return uuid.IsUUIDv7();

        }

        #endregion


        #region ConvertToDateTime       (UUID)

        /// <summary>
        /// Converts a UUIDv7 to its corresponding timestamp (DateTime).
        /// </summary>
        /// <param name="UUID">The UUIDv7 to convert.</param>
        public static DateTime ConvertToDateTime(Guid UUID)
        {

            if (!UUID.IsUUIDv7())
                throw new ArgumentException("The provided UUID is not a valid UUIDv7.", nameof(UUID));

            // Extract the 48-bit timestamp (first 6 bytes)
            var bytes      = UUID.ToByteArray();
            var timestamp  = 0L;
            for (var i = 0; i < 6; i++)
                timestamp  = (timestamp << 8) | bytes[i];

            return DateTime.UnixEpoch.AddMilliseconds(timestamp);

        }

        #endregion

        #region ConvertToDateTimeOffset (UUID)

        /// <summary>
        /// Converts a UUIDv7 to its corresponding timestamp (DateTime).
        /// </summary>
        /// <param name="UUID">The UUIDv7 to convert.</param>
        public static DateTimeOffset ConvertToDateTimeOffset(Guid UUID)
        {

            if (!UUID.IsUUIDv7())
                throw new ArgumentException("The provided UUID is not a valid UUIDv7.", nameof(UUID));

            // Extract the 48-bit timestamp (first 6 bytes)
            var bytes      = UUID.ToByteArray();
            var timestamp  = 0L;
            for (var i = 0; i < 6; i++)
                timestamp  = (timestamp << 8) | bytes[i];

            return DateTimeOffset.UnixEpoch.AddMilliseconds(timestamp);

        }

        #endregion


    }

}
