/*
 * Copyright (c) 2010-2022 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

#endregion

namespace org.GraphDefined.Vanaheimr.Illias.Endianness
{

    /// <summary>
    /// Some utilities to help in the hell of endianness.
    /// </summary>
    public static class EndiannessHell
    {

        #region SwapBytes(this Int16 Value)

        /// <summary>
        /// Swap the byte representation of the given Int16 value.
        /// </summary>
        /// <param name="Value">A Int16.</param>
        public static Int16 SwapBytes(this Int16 Value)
        {
            return (Int16) ((Value & 0xFFU) << 8 | (Value & 0xFF00U) >> 8);
        }

        #endregion

        #region SwapBytes(this UInt16 Value)

        /// <summary>
        /// Swap the byte representation of the given UInt16 value.
        /// </summary>
        /// <param name="Value">A UInt16.</param>
        public static UInt16 SwapBytes(this UInt16 Value)
        {
            return (UInt16)((Value & 0xFFU) << 8 | (Value & 0xFF00U) >> 8);
        }

        #endregion

        #region SwapBytes(this Int32 Value)

        /// <summary>
        /// Swap the byte representation of the given Int32 value.
        /// </summary>
        /// <param name="Value">A Int32.</param>
        public static Int32 SwapBytes(this Int32 Value)
        {
            return (Value >> 24 & 255) | (Value >> 8 & 65280) | (Value << 8 & 16711680) | Value << 24;
        }

        #endregion

        #region SwapBytes(this UInt32 Value)

        /// <summary>
        /// Swap the byte representation of the given UInt32 value.
        /// </summary>
        /// <param name="Value">A UInt32.</param>
        public static UInt32 SwapBytes(this UInt32 Value)
        {
            return (Value & 0x000000FFU) << 24 | (Value & 0x0000FF00U) << 8 |
                   (Value & 0x00FF0000U) >> 8  | (Value & 0xFF000000U) >> 24;
        }

        #endregion

        #region SwapBytes(this Int64 Value)

        /// <summary>
        /// Swap the byte representation of the given Int64 value.
        /// </summary>
        /// <param name="Value">A Int64.</param>
        public static Int64 SwapBytes(this Int64 Value)
        {
            return ((int) Value >> 56 & 255)                | ((int) Value >> 40 & 65280) |
                   ((int) Value >> 24 & 16711680)           | ((int) Value >> 8 & (long)-16777216) |
                   ((int) Value << 8 & 1095216660480L)      | ((int) Value << 24 & 280375465082880L) |
                   ((int) Value << 40 & 71776119061217280L) |  (int) Value << 56;
        }

        #endregion

        #region SwapBytes(this UInt64 Value)

        /// <summary>
        /// Swap the byte representation of the given UInt64 value.
        /// </summary>
        /// <param name="Value">A UInt64.</param>
        public static UInt64 SwapBytes(this UInt64 Value)
        {
            return (Value & 0x00000000000000FFUL) << 56 | (Value & 0x000000000000FF00UL) << 40 |
                   (Value & 0x0000000000FF0000UL) << 24 | (Value & 0x00000000FF000000UL) << 8  |
                   (Value & 0x000000FF00000000UL) >> 8  | (Value & 0x0000FF0000000000UL) >> 24 |
                   (Value & 0x00FF000000000000UL) >> 40 | (Value & 0xFF00000000000000UL) >> 56;
        }

        #endregion

    }

}
