/*
 * Copyright (c) 2010-2012 Achim 'ahzf' Friedland <achim@graph-database.org>
 * This file is part of Illias Commons <http://www.github.com/ahzf/Illias>
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
using System.Text;

#endregion

namespace de.ahzf.Illias.Commons
{

    /// <summary>
    /// Extensions to the DateTime class.
    /// </summary>
    public static class DateTimeExtensions
    {

        #region UNIXTime conversion

        private static DateTime _UNIXEpoch = new DateTime(1970, 1, 1, 0, 0, 0);

        public static Int64 ToUnixTimeStamp(this DateTime myDateTime)
        {
            return myDateTime.Subtract(_UNIXEpoch).Ticks;
        }

        public static DateTime FromUnixTimeStamp(this Int64 myTimestamp)
        {
            return _UNIXEpoch.AddTicks(myTimestamp);
        }

        #endregion

    }

}
