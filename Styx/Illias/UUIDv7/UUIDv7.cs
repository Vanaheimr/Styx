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

#region Usings

using System.Security.Cryptography;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    public static class UUIDv7
    {

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

    }

}